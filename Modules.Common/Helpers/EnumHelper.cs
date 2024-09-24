namespace Modules.Common.Helpers;

public static class EnumHelper
{
    public static TEnum ConvertTo<TEnum>(string enumValue) where TEnum : Enum
    {
        return (TEnum)Enum.Parse(typeof(TEnum), enumValue);
    }
}

