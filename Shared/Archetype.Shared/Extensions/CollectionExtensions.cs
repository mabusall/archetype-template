namespace Archetype.Shared.Extensions;

public static class CollectionExtensions
{
    extension<T>(IEnumerable<T> source)
    {
        public bool IsEmpty => !source.Any();

        public bool HasItems => source.Any();

        public int Count => source.Count();
    }
}