using System.Globalization;

namespace SSR.WebAPI.Extensions;

public static class CommonExtensions
{
    public static string formatDateToString(DateTime datetime)
    {
        if (datetime.ToString("dd/MM/yyyy").Equals("01/01/0001"))
        {
            return "";
        }
        else
        {
            CultureInfo viVn = new CultureInfo("vi-VN");
            if (datetime != default)
            {
                return datetime.ToString("d", viVn);
            }
        }

        return "";
    }

    public static DateTime formatStringToDate(string dateStr = "01/01/1900")
    {
        DateTime dt = new DateTime(2100, 1, 1);
        if (string.IsNullOrEmpty(dateStr))
            dateStr = "01/01/1900";
        string formats = "dd/MM/yyyy";
        if (dateStr != null && !dateStr.Equals(""))
        {
            dt = DateTime.ParseExact(dateStr, formats, CultureInfo.InvariantCulture);
        }
        return dt;
    }

    public static DateTime formatStringToDate_WithStartTime(string dateStr = "01/01/1900")
    {
        DateTime dt = new DateTime(2100, 1, 1, 0, 0, 1);
        if (string.IsNullOrEmpty(dateStr))
            dateStr = "01/01/1900";
        string formats = "dd/MM/yyyy";
        if (dateStr != null && !dateStr.Equals(""))
        {
            dt = DateTime.ParseExact(dateStr, formats, CultureInfo.InvariantCulture);
        }
        return dt;
    }
    public static DateTime formatStringToDate_WithEnSSRme(string dateStr = "01/01/1900")
    {
        DateTime dt = new DateTime(2100, 1, 1, 23, 59, 59);
        if (string.IsNullOrEmpty(dateStr))
            dateStr = "01/01/1900";
        string formats = "dd/MM/yyyy";
        if (dateStr != null && !dateStr.Equals(""))
        {
            dt = DateTime.ParseExact(dateStr, formats, CultureInfo.InvariantCulture);
            dt = dt.AddHours(23).AddMinutes(59).AddSeconds(59);
        }
        return dt;
    }
    public static string GenerateNewRandomDigit()
    {
        Random generator = new Random();
        String r = generator.Next(0, 1000000).ToString("D6");
        if (r.Distinct().Count() == 1)
        {
            r = GenerateNewRandomDigit();
        }
        return r;
    }

    public static string ProgressSlug(string slug)
    {
        if (!string.IsNullOrEmpty(slug))
        {
            slug = slug.Trim();
            slug = slug.ToLower();
            return slug;
        }
        return null;
    }

    public static bool ExistedArrayInArray(this List<string> Array, List<string> array)
    {

        foreach (var item in Array)
        {
            if (array == default)
                return false;
            if (array.Contains(item))
                return true;
        }
        return false;
    }
}