namespace PccCrawler.Extension
{
    /// <summary>
    /// DateTime擴充功能
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// 取得與指定日期最近的平日
        /// </summary>
        /// <param name="dt">指定日期</param>
        /// <param name="getBefore">true:取往前最近平日 false:取往後最近平日</param>
        /// <returns></returns>
        public static DateTime GetNearestWeekday(this DateTime dt, bool getBefore = true)
        {
            return (getBefore, dt.DayOfWeek) switch
            {
                (true, DayOfWeek.Saturday) => dt.AddDays(-1),
                (true, DayOfWeek.Sunday) => dt.AddDays(-2),
                (false, DayOfWeek.Saturday) => dt.AddDays(2),
                (false, DayOfWeek.Sunday) => dt.AddDays(1),
                _ => dt
            };
        }

        /// <summary>
        /// 轉換民國年字串
        /// </summary>
        /// <param name="dt">指定日期</param>
        /// <returns></returns>
        public static string ToTWDateString(this DateTime dt)
        {
            if (dt.Year < 1911)
                throw new ArgumentException($"民國年轉換失敗，日期不得小於1911，參數:{dt}");
            return dt.AddYears(-1911).ToString("yyy/MM/dd");
        }
    }
}