using System.Diagnostics.Contracts;

namespace Vypex.CodingChallenge.Domain.DateExtensions {
    public static class DateTimeExtensions
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
        
        /// <summary>
        /// Calculate the number of works days between two dates
        /// </summary>
        /// <param name="firstDay"></param>
        /// <param name="lastDay"></param>
        /// <param name="publicHolidays"></param>
        /// <returns></returns>
        public static int BusinessDaysUntil(this DateTime firstDay, DateTime lastDay, params DateTime[] publicHolidays)
        {
            int days = 0;
            var start = firstDay;
            // There is an optimisation to run this per week which does iterate less
            // Likely premature optimisation for the use case and this seems neater.
            while(start <= lastDay)
            {
                if(start.DayOfWeek != DayOfWeek.Saturday && start.DayOfWeek != DayOfWeek.Sunday)
                {
                    ++days;
                }
                start = start.AddDays(1);
            }
            
            // subtract the number of bank holidays during the time interval
            foreach (DateTime publicHoliday in publicHolidays)
            {
                DateTime bh = publicHoliday.Date;
                if (firstDay <= bh && bh <= lastDay)
                    --days;
            }

            return days;
        }
        /// <summary>
        /// TimePeriod Library handles this pretty succintly for large sets
        /// Uses the contraction of conditions
        /// tStartA < tStartB && tStartB < tEndA //For case 1
        /// OR
        /// tStartA < tEndB && tEndB <= tEndA //For case 2
        /// OR
        /// tStartB < tStartA  && tEndB > tEndA //For case 3
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool DateRangesOvelap(Tuple<DateTime,DateTime> left, Tuple<DateTime,DateTime> right) {
            return left.Item1 < right.Item2 && right.Item1 < left.Item2;
        }
    }
}