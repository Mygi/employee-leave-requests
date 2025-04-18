using System.Diagnostics.Contracts;

static class DateTimeExtensions
{
    ///
    /// Adds the given number of business days to the input date
    /// Extend this method to check for Public holidays
    [Pure]
    public static DateTime AddWorkingDays(DateTime current, int days)
    {
        dynamic sign = Math.Sign(days);
        dynamic unsignedDays = Math.Abs(days);
        for (int i = 0; i <= unsignedDays - 1; i++)
        {
            do
            {
                current = current.AddDays(sign);
            } while (current.DayOfWeek == DayOfWeek.Saturday || current.DayOfWeek == DayOfWeek.Sunday);
        }
        return current;
    }
 
    ///
    /// Subtracts the given number of business days to the .
    ///
    /// The date to be changed.
    /// Number of business days to be subtracted.
    /// A increased by a given number of business days.
    [Pure]
    public static DateTime SubtractWorkingDays(this DateTime current, int days)
    {
        return AddWorkingDays(current, -days);
    }
}