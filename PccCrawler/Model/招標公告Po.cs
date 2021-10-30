namespace PccCrawler.Model
{
    public class 招標公告Po
    {
        public 機關資料? 機關資料 { get; set; }
        public 採購資料? 採購資料 { get; set; }
        public 招標資料? 招標資料 { get; set; }
        public 領投開標? 領投開標 { get; set; }
        public 其他? 其他 { get; set; }
    }
    #region 機關資料
    public class 機關資料
    {
        public string? 機關代碼 { get; set; }
        public string? 機關名稱 { get; set; }
        public string? 單位名稱 { get; set; }
        public string? 機關地址 { get; set; }
        public string? 聯絡人 { get; set; }
        public string? 聯絡電話 { get; set; }
        public string? 傳真號碼 { get; set; }
        public string? 電子郵件信箱 { get; set; }
    }
    #endregion
    #region 採購資料
    public class 採購資料
    {
        public string? 標案案號 { get; set; }
        public string? 標案名稱 { get; set; }
        public string? 標的分類 { get; set; }
        public string? 財物採購性質 { get; set; }
        public string? 採購金額級距 { get; set; }
        public string? 辦理方式 { get; set; }
        public string? 依據法條 { get; set; }
        public 是否適用條約或協定之採購? 是否適用條約或協定之採購 { get; set; }
        public string? 本採購是否屬_具敏感性或國安_含資安_疑慮之業務範疇_採購 { get; set; }
        public string? 本採購是否屬_涉及國家安全_採購 { get; set; }
        public string? 預算金額 { get; set; }
        public string? 預算金額是否公開 { get; set; }
        public string? 後續擴充 { get; set; }
        public string? 是否受機關補助 { get; set; }
        public string? 是否含特別預算 { get; set; }
    }
    /// <summary>
    /// 採購資料 > 是否適用條約或協定之採購
    /// </summary>
    public class 是否適用條約或協定之採購
    {
        public string? 是否適用WTO政府採購協定_GPA_ { get; set; }
        public string? 是否適用臺紐經濟合作協定_ANZTEC_ { get; set; }
        public string? 是否適用臺星經濟夥伴協定_ASTEP_ { get; set; }
    }
    #endregion
    #region 招標資料
    public class 招標資料
    {
        public string? 招標方式 { get; set; }
        public string? 決標方式 { get; set; }
        public string? 是否依政府採購法施行細則第64條之2辦理 { get; set; }
        public string? 新增公告傳輸次數 { get; set; }
        public string? 招標狀態 { get; set; }
        public string? 公告日 { get; set; }
        public string? 是否複數決標 { get; set; }
        public string? 是否訂有底價 { get; set; }
        public string? 是否屬特殊採購 { get; set; }
        public string? 是否已辦理公開閱覽 { get; set; }
        public string? 是否屬統包 { get; set; }
        public string? 是否屬共同供應契約採購 { get; set; }
        public string? 是否屬二以上機關之聯合採購_不適用共同供應契約規定_ { get; set; }
        public string? 是否應依公共工程專業技師簽證規則實施技師簽證 { get; set; }
        public string? 是否採行協商措施 { get; set; }
        public string? 是否適用採購法第104條或105條或招標期限標準第10條或第4條之1 { get; set; }
        public string? 是否依據採購法第106條第1項第1款辦理 { get; set; }
    }
    #endregion
    #region 領投開標
    public class 領投開標
    {
        public 是否提供電子領標? 是否提供電子領標Po { get; set; }
        public string? 是否提供電子領標 { get; set; }
        public string? 是否提供電子投標 { get; set; }
        public string? 截止投標 { get; set; }
        public string? 開標時間 { get; set; }
        public string? 開標地點 { get; set; }
        public string? 是否須繳納押標金 { get; set; }
        public string? 投標文字 { get; set; }
        public string? 收受投標文件地點 { get; set; }
    }
    public class 是否提供電子領標
    {
        public string? 機關文件費_機關實收_ { get; set; }
        public string? 系統使用費 { get; set; }
        public string? 文件代收費 { get; set; }
        public string? 總計 { get; set; }
        public string? 機關文件費指定收款機關單位 { get; set; }
        public string? 是否提供現場領標 { get; set; }
        public string? 招標文件領取地點 { get; set; }
        public string? 招標文件售價及付款方式 { get; set; }
    }
    #endregion
    #region 其他
    public class 其他
    {
        public string? 是否依據採購法第99條 { get; set; }
        public string? 履約地點 { get; set; }
        public string? 履約期限 { get; set; }
        public string? 是否刊登公報 { get; set; }
        public string? 本案採購契約是否採用主管機關訂定之範本 { get; set; }
        public string? 廠商資格摘要 { get; set; }
        public string? 是否訂有與履約能力有關之基本資格 { get; set; }
        public string? 附加說明 { get; set; }
        public string? 是否刊登英文公告 { get; set; }
        public 疑義_異議_申訴及檢舉受理單位? 疑義_異議_申訴及檢舉受理單位 { get; set; }
    }
    public class 疑義_異議_申訴及檢舉受理單位
    {
        public string? 疑義_異議受理單位 { get; set; }
        public string? 申訴受理單位 { get; set; }
        public string? 檢舉受理單位 { get; set; }
    }
    #endregion
}
