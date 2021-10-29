using System.ComponentModel;

namespace PccCrawler.PccEnum
{
    /// <summary>
    /// EnumHelper
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// ���o���]�wDescriptionAttribute����
        /// </summary>
        /// <param name="value">���W��</param>
        /// <returns></returns>
        public static string GetDescription(string value)
        {
            Type type = typeof(TenderStatus);
            var name = Enum.GetNames(type)
                            .Where(f => f.Equals(value, StringComparison.CurrentCultureIgnoreCase))
                            .Select(d => d)
                            .FirstOrDefault();

            //// ��L�۹������C�|
            if (name == null)
            {
                return string.Empty;
            }

            //// �Q�ΤϮg��X�۹��������
            var field = type.GetField(name);
            //// ���o���]�wDescriptionAttribute����
            var customAttribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            //// �L�]�wDescription Attribute, �^��Enum���W��
            if (customAttribute == null || customAttribute.Length == 0)
            {
                return name;
            }

            //// �^��Description Attribute���]�w
            return ((DescriptionAttribute)customAttribute[0]).Description;
        }
    }
}