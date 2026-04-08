namespace Archetype.Shared.Extensions;

public static class FluentValidationExtension
{
    public static ValidationFailure Error(string propertyName,
                                          string propertyValue,
                                          string errorMessage)
    {
        return new ValidationFailure(propertyName, errorMessage)
        {
            AttemptedValue = propertyValue,
            ErrorMessage = errorMessage,
            ErrorCode = propertyName,
            Severity = Severity.Error,
            FormattedMessagePlaceholderValues = new Dictionary<string, object>
            {
                { "PropertyName", propertyName },
                { "PropertyValue", propertyValue },
                { "PropertyPath", propertyName }
            }
        };
    }
}