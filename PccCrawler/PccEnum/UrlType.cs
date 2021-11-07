namespace PccCrawler.PccEnum
{
    /// <summary>
    /// Url Type
    /// </summary>
    public enum UrlType
    {
        /// <summary>
        /// 招標公告 - 搜尋結果頁
        /// https://web.pcc.gov.tw/tps/pss/tender.do?searchMode=common&searchType=advance
        /// </summary>
        招標公告_搜尋結果頁,

        /// <summary>
        /// 招標公告 - 詳細資料頁
        /// https://web.pcc.gov.tw/tps/tpam/main/tps/tpam/tpam_tender_detail.do?searchMode=common&scope=F&primaryKey=53619472
        /// </summary>
        招標公告_詳細資料頁,

        /// <summary>
        /// 公開徵求 - 搜尋結果頁
        /// https://web.pcc.gov.tw/tps/tps/tp/main/tps/tp/searchAppealVendor.do?pMenu=common
        /// </summary>
        公開徵求_搜尋結果頁,

        /// <summary>
        /// 公開徵求 - 詳細資料頁
        /// https://web.pcc.gov.tw/tps/tps/tp/main/tps/tp/tp.do?method=initialAppealViewVendor&pMenu=common
        /// </summary>
        公開徵求_詳細資料頁,

        /// <summary>
        /// 公開閱覽 - 搜尋結果頁
        /// https://web.pcc.gov.tw/tps/tps/tp/main/pms/tps/tp/QueryPublicReadData.do?pMenu=common
        /// </summary>
        公開閱覽_搜尋結果頁,

        /// <summary>
        /// 公開閱覽 - 檔案下載
        /// https://web.pcc.gov.tw/tps/tps/tom/main/tps/tom/obtainment.do?method=init&primaryKey=50027114&doctype=3
        /// https://web.pcc.gov.tw/tps/tps/tp/main/pms/tps/tp/TenderDocumentGlance.do?method=listTenderDocuments&docType=3&primaryKey=50027114
        /// </summary>
        公開閱覽_檔案下載,

        /// <summary>
        /// 政府採購預告 - 搜尋結果頁
        /// https://web.pcc.gov.tw/tps/pss/tender.do?searchMode=common&searchType=advance
        /// </summary>
        政府採購預告_搜尋結果頁,

        /// <summary>
        /// 政府採購預告 - 詳細資料頁
        /// https://web.pcc.gov.tw/tps/pss/tender.do?searchMode=common&pkTpGpaPredict=50016287&method=glancePredictInformation
        /// </summary>
        政府採購預告_詳細資料頁,

        /// <summary>
        /// 決標公告 - 搜尋結果頁
        /// https://web.pcc.gov.tw/tps/pss/tender.do?searchMode=common&searchType=advance
        /// </summary>
        決標公告_搜尋結果頁,

        /// <summary>
        /// 決標公告 - 詳細資料頁
        /// https://web.pcc.gov.tw/tps/main/pms/tps/atm/atmAwardAction.do?newEdit=false&searchMode=common&method=inquiryForPublic&pkAtmMain=53506049&tenderCaseNo=1100729
        /// </summary>
        決標公告_詳細資料頁,

        /// <summary>
        /// 無法決標 - 搜尋結果頁
        /// https://web.pcc.gov.tw/tps/pss/tender.do?searchMode=common&searchType=advance
        /// </summary>
        無法決標_搜尋結果頁,

        /// <summary>
        /// 無法決標 - 詳細資料頁
        /// https://web.pcc.gov.tw/tps/main/pms/tps/atm/atmNonAwardAction.do?searchMode=common&method=nonAwardContentForPublic&pkAtmMain=53546476
        /// </summary>
        無法決標_詳細資料頁,

        /// <summary>
        /// 撤銷公告 - 搜尋結果頁
        /// https://web.pcc.gov.tw/tps/pss/tender.do?searchMode=common&searchType=advance
        /// </summary>
        撤銷公告_搜尋結果頁,

        /// <summary>
        /// 撤銷公告 - 詳細資料頁
        /// https://web.pcc.gov.tw/tps/main/pms/tps/atm/atmAwardAction.do?newEdit=false&searchMode=common&method=inquiryForPublic&pkAtmMain=53497162&tenderCaseNo=KBE2021-57FD3-1
        /// </summary>
        撤銷公告_詳細資料頁
    }
}
