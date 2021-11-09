namespace PccCrawler.Model
{
    /// <summary>
    /// PccMaster Table Po
    /// </summary>
    public class PccMasterPo
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
        /// Url
        /// </summary>
        public string? Url { get; set; }
        /// <summary>
        /// 狀態 100:進行中 900:完成
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}