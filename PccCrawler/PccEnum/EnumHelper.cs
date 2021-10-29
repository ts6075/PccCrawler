using System.ComponentModel;

namespace PccCrawler.PccEnum
{
    /// <summary>
    /// EnumHelper
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// 取得欄位設定DescriptionAttribute的值
        /// </summary>
        /// <param name="value">欄位名稱</param>
        /// <returns></returns>
        public static string GetDescription(string value)
        {
            Type type = typeof(TenderStatus);
            var name = Enum.GetNames(type)
                            .Where(f => f.Equals(value, StringComparison.CurrentCultureIgnoreCase))
                            .Select(d => d)
                            .FirstOrDefault();

            //// 找無相對應的列舉
            if (name == null)
            {
                return string.Empty;
            }

            //// 利用反射找出相對應的欄位
            var field = type.GetField(name);
            //// 取得欄位設定DescriptionAttribute的值
            var customAttribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            //// 無設定Description Attribute, 回傳Enum欄位名稱
            if (customAttribute == null || customAttribute.Length == 0)
            {
                return name;
            }

            //// 回傳Description Attribute的設定
            return ((DescriptionAttribute)customAttribute[0]).Description;
        }
    }
}