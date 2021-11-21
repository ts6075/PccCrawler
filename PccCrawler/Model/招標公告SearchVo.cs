using PccCrawler.Extension;
using PccCrawler.PccEnum;

namespace PccCrawler.Model
{
	/// <summary>
	/// 招標公告Search Vo
	/// </summary>
	public class 招標公告SearchVo
	{
		/// <summary>
		/// 第N頁
		/// </summary>
		public int pageIndex { get; set; } = 1;
		public string? method { get; set; } = "search";
		public bool searchMethod { get; set; } = true;
		public string? searchTarget { get; set; } = "TPAM";
		/// <summary>
		/// 機關名稱
		/// </summary>
		public string? orgName { get; set; } = null;
		/// <summary>
		/// 機關代碼
		/// </summary>
		public string? orgId { get; set; } = null;
		public string? hid_1 { get; set; } = "1";
		/// <summary>
		/// 標案名稱
		/// </summary>
		public string? tenderName { get; set; } = null;
		/// <summary>
		/// 標案案號
		/// </summary>
		public string? tenderId { get; set; } = null;
		/// <summary>
		/// 招標類型
		/// </summary>
		public TenderType tenderType { get; set; } = TenderType.tenderDeclaration;
		/// <summary>
		/// 招標方式
		/// </summary>
		public int? tenderWay { get; set; } = null;
		public object tenderDateRadio { get; set; } = "on";
		/// <summary>
		/// 公告日期字串 - 起
		/// </summary>
		public string? tenderStartDateStr => tenderStartDate;
		/// <summary>
		/// 公告日期字串 - 訖
		/// </summary>
		public string? tenderEndDateStr => tenderEndDate;
		/// <summary>
		/// 公告日期 - 起
		/// </summary>
		public string? tenderStartDate { get; set; } = DateTime.Now.GetNearestWeekday().ToTWDateString();
		/// <summary>
		/// 公告日期 - 訖
		/// </summary>
		public string? tenderEndDate { get; set; } = DateTime.Now.GetNearestWeekday().ToTWDateString();
		public string? isSpdt { get; set; } = "N";
		public object spdtStartDate { get; set; } = null;
		public object spdtEndDate { get; set; } = null;
		public object tenderSpdtYmStart { get; set; } = null;
		public object tenderSpdtYmEnd { get; set; } = null;
		public object opdtStartDate { get; set; } = null;
		public object opdtEndDate { get; set; } = null;
		public int? proctrgCate { get; set; } = null;
		/// <summary>
		/// 標的分類
		/// </summary>
		public int? radProctrgCate { get; set; } = null;
		public object tenderRange { get; set; } = null;
		/// <summary>
		/// 預算金額 - 起
		/// </summary>
		public decimal? minBudget { get; set; } = null;
		/// <summary>
		/// 預算金額 - 訖
		/// </summary>
		public decimal? maxBudget { get; set; } = null;
		public object location { get; set; } = null;
		public object locationPredict { get; set; } = null;
		/// <summary>
		/// 優先採購分類
		/// </summary>
		public string? priorityCate { get; set; } = null;
		/// <summary>
		/// 災區重建工程 Y/N
		/// </summary>
		public string? isReConstruct { get; set; } = null;
		public string? btnQuery { get; set; } = "查詢";
	}
}