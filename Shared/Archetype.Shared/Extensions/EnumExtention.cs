namespace Archetype.Shared.Extensions;

public static class EnumExtention
{
    public static string GetDescription(this Enum value)
    {
        FieldInfo? fi = value.GetType().GetField(value.ToString());

        if (fi is null)
            return value.ToString();

        if (fi.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Length != 0)
        {
            return attributes[0].Description;
        }

        return value.ToString();
    }

    public static string GetDisplay(this Enum value)
    {
        FieldInfo? fi = value.GetType().GetField(value.ToString());

        if (fi is null)
            return value.ToString();

        if (fi.GetCustomAttributes(typeof(DisplayAttribute), false) is DisplayAttribute[] attributes && attributes.Length != 0)
        {
            return attributes[0].Name ?? value.ToString();
        }

        return value.ToString();
    }
}