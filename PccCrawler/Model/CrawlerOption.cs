namespace PccCrawler.Model
{
    public class CrawlerOption
    {
        /// <summary>
        /// 模式 Debug/Release
        /// </summary>
        public string Mode { get; set; } = "Release";

        /// <summary>
        /// 間隔秒數
        /// </summary>
        public int IntervalSeconds { get; set; } = 15;
    }
}
