namespace Archetype.Shared.Extensions;

public static class LinqQueryableExtensions
{
    // Cache selector lambdas per (T, "A.B.C") to avoid rebuilding expressions repeatedly.
    private static readonly ConcurrentDictionary<(Type Type, string Path, bool IgnoreCase), LambdaExpression> _selectorCache = new();

    extension<T>(IQueryable<T> source)
    {
        /// <summary>
        /// Orders an IQueryable based on a string like:
        /// "Name desc" or "Parent.Child.Name asc" or "Name desc, Age asc".
        /// </summary>
        public IQueryable<T> OrderBy(string orderBy, bool ignoreCase = true, bool useCache = true)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (string.IsNullOrWhiteSpace(orderBy))
                throw new ArgumentException("Order by string cannot be null or empty.", nameof(orderBy));

            // Support multiple clauses separated by commas:
            // "Name desc, Age asc"
            string[] clauses = orderBy.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (clauses.Length == 0)
                throw new ArgumentException("Order by string is invalid.", nameof(orderBy));

            IOrderedQueryable<T>? ordered = null;

            for (int i = 0; i < clauses.Length; i++)
            {
                string clause = clauses[i];
                string[] parts = clause.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (parts.Length is < 1 or > 2)
                    throw new ArgumentException(
                        $"Order by clause '{clause}' must be in the format 'Property [asc|desc]'.",
                        nameof(orderBy));

                string propertyPath = parts[0];
                string dir = parts.Length == 2 ? parts[1] : "asc";

                bool desc = dir.Equals("desc", StringComparison.OrdinalIgnoreCase);
                if (!desc && !dir.Equals("asc", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException(
                        $"Order direction in clause '{clause}' must be 'asc' or 'desc'.",
                        nameof(orderBy));

                // Build selector x => x.Prop.SubProp
                LambdaExpression selector = GetSelector(typeof(T), propertyPath, ignoreCase, useCache, out Type? propertyType);

                // Extracted ternary logic into an independent statement for methodName
                string methodName;
                if (i == 0)
                {
                    methodName = desc ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
                }
                else
                {
                    methodName = desc ? nameof(Queryable.ThenByDescending) : nameof(Queryable.ThenBy);
                }

                MethodCallExpression call = Expression.Call(typeof(Queryable),
                                           methodName,
                                           [typeof(T), propertyType],
                                           (i == 0 ? source.Expression : ordered!.Expression),
                                           Expression.Quote(selector));

                ordered = (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
            }

            return ordered ?? source;
        }
    }

    private static LambdaExpression GetSelector(Type entityType,
                                                string propertyPath,
                                                bool ignoreCase,
                                                bool useCache,
                                                out Type propertyType)
    {
        (Type entityType, string propertyPath, bool ignoreCase) cacheKey = (entityType, propertyPath, ignoreCase);

        if (useCache && _selectorCache.TryGetValue(cacheKey, out LambdaExpression? cached))
        {
            propertyType = cached.ReturnType;
            return cached;
        }

        ParameterExpression parameter = Expression.Parameter(entityType, "x");
        Expression body = parameter;
        Type currentType = entityType;

        foreach (string segment in propertyPath.Split('.', StringSplitOptions.RemoveEmptyEntries))
        {
            // Find property/field with optional ignoreCase for nicer UX.
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            if (ignoreCase) flags |= BindingFlags.IgnoreCase;

            PropertyInfo? prop = currentType.GetProperty(segment, flags);
            if (prop is not null)
            {
                body = Expression.Property(body, prop);
                currentType = prop.PropertyType;
                continue;
            }

            FieldInfo? field = currentType.GetField(segment, flags);
            if (field is not null)
            {
                body = Expression.Field(body, field);
                currentType = field.FieldType;
                continue;
            }

            throw new ArgumentException(
                $"Invalid order by path '{propertyPath}'. Type '{currentType.Name}' does not contain a public instance property/field named '{segment}'.",
                nameof(propertyPath));
        }

        propertyType = currentType;
        LambdaExpression lambda = Expression.Lambda(body, parameter);

        if (useCache)
            _selectorCache[cacheKey] = lambda;

        return lambda;
    }
}