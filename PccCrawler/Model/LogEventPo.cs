namespace PccCrawler.Model
{
    /// <summary>
    /// LogEvent Table Po
    /// </summary>
    public class LogEventPo
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public int Seq {  get; set; }
        /// <summary>
        /// 事件等級
        /// </summary>
        public string EventLevel { get; set; }
        /// <summary>
        /// 事件種類
        /// </summary>
        public string EventType { get; set; }
        /// <summary>
        /// 事件內容
        /// </summary>
        public string EventContent { get; set; }
        /// <summary>
        /// 識別碼
        /// </summary>
        public string CaseId { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
        public string? CreateTime { get; set; }
    }
}