using System.Text.Encodings.Web;
using System.Text.Json;

namespace PccCrawler.Extension
{
    /// <summary>
    /// Json工具
    /// </summary>
    public static class JsonExtension
    {
        /// <summary>
        /// 預設Json設定
        /// </summary>
        private readonly static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// 物件轉為Json字串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string ToJson(this object value, JsonSerializerOptions? options = null)
        {
            options ??= _serializerOptions;
            return JsonSerializer.Serialize(value, options);
        }

        /// <summary>
        /// 將Json字串轉為物件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T ToObject<T>(this string value, JsonSerializerOptions? options = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            options ??= _serializerOptions;
            return JsonSerializer.Deserialize<T>(value, options);
        }
    }
}
