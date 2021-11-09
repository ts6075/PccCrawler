using PccCrawler.Extension;

namespace PccCrawler.Model
{
	/// <summary>
	/// 公開徵求Search Vo
	/// </summary>
	public class 公開徵求SearchVo
	{
		/// <summary>
		/// 第N頁
		/// </summary>
		public int __PageIndex { get; set; } = 1;
		public string method { get; set; } = "searchVendor";
		public string desc { get; set; } = "";
		public string orderBy { get; set; } = "";
		public string isVendor { get; set; } = "Y";
		public string orgId { get; set; } = "";
		public string tenderCaseNo { get; set; } = "";
		public string orgName { get; set; } = "";
		public string tenderCaseName { get; set; } = "";
		/// <summary>
		/// 公告日期字串 - 起
		/// </summary>
		public string startDateStr { get; set; } = DateTime.Now.GetNearestWeekday().ToTWDateString();
		/// <summary>
		/// 公告日期字串 - 訖
		/// </summary>
		public string endDateStr { get; set; } = DateTime.Now.GetNearestWeekday().ToTWDateString();
	}
}