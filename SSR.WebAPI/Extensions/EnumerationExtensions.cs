using System.ComponentModel;
using System.Reflection;

namespace SSR.WebAPI.Extensions;
public static class EnumerationExtensions
{
    public static (string, string) GetEnumData<T>(this T source)
    {
        FieldInfo fi = source.GetType().GetField(source.ToString());

        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
            typeof(DescriptionAttribute), false);

        if (attributes != null && attributes.Length > 0) return (source.ToString(), attributes[0].Description);
        else return (source.ToString(), source.ToString());
    }
    public static T ToEnum<T>(this string value, bool ignoreCase = true)
    {
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }
}