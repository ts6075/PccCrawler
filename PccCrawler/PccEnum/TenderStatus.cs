using System.ComponentModel;

namespace PccCrawler.PccEnum
{
    /// <summary>
    /// 標案狀態(取得Description)
    /// </summary>
    public enum TenderStatus
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Description("5,6,20,28,8,21,22,29,33,9,23")]
        All,

        /// <summary>
        /// 決標公告
        /// </summary>
        [Description("5,6,20,28")]
        Award,

        /// <summary>
        /// 無法決標
        /// </summary>
        [Description("8,21,22,29,33")]
        UnableToAward,

        /// <summary>
        /// 撤銷公告
        /// </summary>
        [Description("9,23")]
        Revoke,
    }
}