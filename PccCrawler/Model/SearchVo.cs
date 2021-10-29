using PccCrawler.Extension;

namespace PccCrawler.Model
{
	/// <summary>
	/// 搜尋 Vo
	/// </summary>
	public class SearchVo
	{
		public int pageIndex {  get; set; }
		public string method { get; set; } = "search";
		public bool searchMethod { get; set; } = true;
		public string searchTarget { get; set; } = "TPAM";
		public object orgName { get; set; } = "";
		public object orgId { get; set; } = "";
		public int hid_1 { get; set; } = 1;
		public object tenderName { get; set; } = "";
		public object tenderId { get; set; } = "";
		public object tenderType { get; set; } = "tenderDeclaration";
		/// <summary>
		/// 招標方式
		/// </summary>
		public int? tenderWay { get; set; } = null;
		public object tenderDateRadio { get; set; } = "on";
		public string tenderStartDateStr { get; set; } = DateTime.Now.GetNearestWeekday().ToTWDateString();
		public string tenderEndDateStr { get; set; } = DateTime.Now.GetNearestWeekday().ToTWDateString();
		public string tenderStartDate { get; set; } = DateTime.Now.GetNearestWeekday().ToTWDateString();
		public string tenderEndDate { get; set; } = DateTime.Now.GetNearestWeekday().ToTWDateString();
		public string isSpdt { get; set; } = "N";
		public object spdtStartDate { get; set; } = "";
		public object spdtEndDate { get; set; } = "";
		public object tenderSpdtYmStart { get; set; } = "";
		public object tenderSpdtYmEnd { get; set; } = "";
		public object opdtStartDate { get; set; } = "";
		public object opdtEndDate { get; set; } = "";
		public int? proctrgCate { get; set; } = null;
		/// <summary>
		/// 標的分類
		/// </summary>
		public int? radProctrgCate { get; set; } = null;
		public object tenderRange { get; set; } = "";
		public object minBudget { get; set; } = "";
		public object maxBudget { get; set; } = "";
		public object location { get; set; } = "";
		public object locationPredict { get; set; } = "";
		public object priorityCate { get; set; } = "";
		public object isReConstruct { get; set; } = "";
		public string btnQuery { get; set; } = "查詢";
	}
}