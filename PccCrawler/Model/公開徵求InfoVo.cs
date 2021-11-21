namespace PccCrawler.Model
{
	/// <summary>
	/// 公開徵求Info Vo
	/// </summary>
	public class 公開徵求InfoVo
	{
		public string? pkTpAppeal { get; set; } = "";
		public string? orgId { get; set; } = "";
		public string? orgName { get; set; } = "";
		public string? tenderCaseNo { get; set; } = "";
		public string? tenderSq { get; set; } = "";
		public string? pkTpAppealHis { get; set; } = "";

		/// <summary>
		/// 取得唯一識別碼
		/// </summary>
		/// <returns></returns>
		public string GetCaseNo()
		{
			return $"{tenderCaseNo}_{tenderSq}_{pkTpAppeal}_{pkTpAppealHis}";
		}
	}
}