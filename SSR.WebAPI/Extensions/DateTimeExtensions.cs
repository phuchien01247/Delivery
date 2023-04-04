namespace SSR.WebAPI.Extensions;

public static class DateTimeExtensions
{
    public static DateTime AddWorkingDays(this DateTime current, int days)
    {
        var sign = Math.Sign(days);
        var unsignedDays = Math.Abs(days);
        for (var i = 0; i < unsignedDays; i++)
        {
            do
            {
                current = current.AddDays(sign);
            } while (current.DayOfWeek == DayOfWeek.Saturday ||
                     current.DayOfWeek == DayOfWeek.Sunday);
        }
        return current;
    }

    public static List<DateTime> ListAddWorkingDays(this DateTime current, int days)
    {
        var newDate = new List<DateTime>();
        var sign = Math.Sign(days);
        var unsignedDays = Math.Abs(days);
        for (var i = 0; i < unsignedDays; i++)
        {
            do
            {

                current = current.AddDays(sign);
            } while (current.DayOfWeek == DayOfWeek.Saturday ||
                     current.DayOfWeek == DayOfWeek.Sunday);
            newDate.Add(current);
        }
        return newDate;
    }

    public static DateTime AddWorkingDays(this DateTime current, int days, List<DateTime> holiday)
    {
        var sign = Math.Sign(days);
        var unsignedDays = Math.Abs(days);
        for (var i = 0; i < unsignedDays; i++)
        {
            do
            {
                current = current.AddDays(sign);
            } while (current.DayOfWeek == DayOfWeek.Saturday ||
                     current.DayOfWeek == DayOfWeek.Sunday ||
                     holiday.Contains(current)
                     );
        }
        return current;
    }

    public static DateTime AddWorkingDays(this DateTime current, List<DateTime> holiday)
    {
        var sign = 1;
        if (current.DayOfWeek == DayOfWeek.Saturday ||
            current.DayOfWeek == DayOfWeek.Sunday ||
            holiday.Contains(current))
        {
            do
            {
                current = current.AddDays(sign);
            } while (current.DayOfWeek == DayOfWeek.Saturday ||
                     current.DayOfWeek == DayOfWeek.Sunday ||
                     holiday.Contains(current)
                    );
        }
        return current;
    }

    public static DateTime AddWorkingDays(this DateTime current)
    {
        var sign = 1;
        if (current.DayOfWeek == DayOfWeek.Saturday ||
            current.DayOfWeek == DayOfWeek.Sunday)
        {
            do
            {
                current = current.AddDays(sign);
            } while (current.DayOfWeek == DayOfWeek.Saturday ||
                     current.DayOfWeek == DayOfWeek.Sunday
                    );
        }
        return current;
    }
}