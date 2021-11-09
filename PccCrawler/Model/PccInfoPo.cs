namespace PccCrawler.Model
{
    /// <summary>
    /// PccInfo Table Po
    /// </summary>
    public class PccInfoPo
    {
        /// <summary>
        /// 識別碼
        /// </summary>
        public string CaseNo { get; set; } = null!;
        /// <summary>
        /// 類別
        /// </summary>
        public string Category { get; set; } = null!;
        /// <summary>
        /// 欄位名稱
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// 欄位Html資訊
        /// </summary>
        public string? HtmlContent { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}