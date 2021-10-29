using System.ComponentModel;

namespace PccCrawler.PccEnum
{
    /// <summary>
    /// �Юת��A(���oDescription)
    /// </summary>
    public enum TenderStatus
    {
        /// <summary>
        /// ����
        /// </summary>
        [Description("5,6,20,28,8,21,22,29,33,9,23")]
        All,

        /// <summary>
        /// �M�Ф��i
        /// </summary>
        [Description("5,6,20,28")]
        Award,

        /// <summary>
        /// �L�k�M��
        /// </summary>
        [Description("8,21,22,29,33")]
        UnableToAward,

        /// <summary>
        /// �M�P���i
        /// </summary>
        [Description("9,23")]
        Revoke,
    }
}