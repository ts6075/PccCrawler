namespace PccCrawler.Model
{
    /// <summary>
    /// PccMaster Table Po
    /// </summary>
    public class PccMasterPo
    {
        /// <summary>
        /// PK
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// Url
        /// </summary>
        public string? Url { get; set; }
        /// <summary>
        /// 處理狀態 100:進行中 900:完成
        /// </summary>
        public int Status { get; set; }
    }
}