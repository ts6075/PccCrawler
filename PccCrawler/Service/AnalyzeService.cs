using HtmlAgilityPack;
using PccCrawler.Extension;
using PccCrawler.Model;
using System.Reflection;

namespace PccCrawler.Service
{
    public class AnalyzeService
    {
        public PoType Analyze<PoType>(HtmlDocument htmlNodes)
        {
            if (typeof(PoType) == typeof(招標公告Po))
            {
                return (PoType)Convert.ChangeType(Analyze招標公告(htmlNodes), typeof(PoType));
            }
            return default;
        }

        private 招標公告Po Analyze招標公告(HtmlDocument detailDoc)
        {
            var po = new 招標公告Po();
            var 機關資料Po = new 機關資料();
            var 採購資料Po = new 採購資料();
            var 招標資料Po = new 招標資料();
            var 領投開標Po = new 領投開標();
            var 其他Po = new 其他();
            var result = new Dictionary<string, string>();
            var detailTrNodes = detailDoc.GetElementbyId("print_area")?.SelectNodes("./table/tr");
            if (detailTrNodes == null)
            {
                Console.WriteLine("Get Detail Fail");
                return default;
            }
            var region1 = detailTrNodes.Where(x => x.GetAttributeValue("class", null) == "tender_table_tr_1").ToList();
            var region2 = detailTrNodes.Where(x => x.GetAttributeValue("class", null) == "tender_table_tr_2").ToList();
            var region3 = detailTrNodes.Where(x => x.GetAttributeValue("class", null) == "tender_table_tr_3").ToList();
            var region4 = detailTrNodes.Where(x => x.GetAttributeValue("class", null) == "tender_table_tr_4").ToList();
            var region5 = detailTrNodes.Where(x => x.GetAttributeValue("class", null) == "tender_table_tr_5").ToList();
            #region region1 機關資料
            foreach (var trNode in region1)
            {
                var key = "";
                var value = "";
                var ths = trNode.SelectNodes("./th");
                if (ths.Count != 1)
                {
                    Console.WriteLine($"程式終止:檢測到未處理的特殊欄位，請通知工程師進行例外處理，參數:{Environment.NewLine}" +
                                      $"ths.Count:{ths.Count}{Environment.NewLine}" +
                                      $"InnerHtml:{trNode.InnerHtml}{Environment.NewLine}");
                    break;
                }
                else
                {
                    key = ths.First().GetDirectInnerText().TrimEmpty();
                }

                var normalList = new string[]
                {
                    "機關名稱", "單位名稱", "機關地址", "聯絡人", "聯絡電話", "傳真號碼", "電子郵件信箱"
                };
                var tds = trNode.SelectNodes("./td");
                if (key == "機關代碼" && tds.Count == 2)
                {
                    value = tds.Skip(1).First().GetDirectInnerText().TrimEmpty();
                    SetValue(ref 機關資料Po, key, value);
                }
                else if (normalList.Contains(key) && tds.Count == 1)
                {
                    value = tds.First().GetDirectInnerText().TrimEmpty();
                    SetValue(ref 機關資料Po, key, value);
                }
                else
                {
                    Console.WriteLine($"程式終止:檢測到未處理的特殊欄位，請通知工程師進行例外處理，參數:{Environment.NewLine}" +
                                      $"tds.Count:{tds.Count}{Environment.NewLine}" +
                                      $"InnerHtml:{trNode.InnerHtml}{Environment.NewLine}");
                    break;
                }
                result.Add(key, value);
                Console.WriteLine($"Key:{key}\tValue:{value}");
            }
            #endregion
            #region region2 採購資料
            foreach (var trNode in region2)
            {
                var key = "";
                var value = "";
                var ths = trNode.SelectNodes("./th");
                if (ths.Count != 1)
                {
                    Console.WriteLine($"程式終止:檢測到未處理的特殊欄位，請通知工程師進行例外處理，參數:{Environment.NewLine}" +
                                      $"ths.Count:{ths.Count}{Environment.NewLine}" +
                                      $"InnerHtml:{trNode.InnerHtml}{Environment.NewLine}");
                    break;
                }
                else
                {
                    key = ths.First().GetDirectInnerText().TrimEmpty();
                }

                var normalList = new string[]
                {
                    "標案名稱", "財物採購性質", "採購金額級距", "辦理方式", "依據法條",
                    "本採購是否屬「具敏感性或國安(含資安)疑慮之業務範疇」採購", "本採購是否屬「涉及國家安全」採購",
                    "預算金額", "預算金額是否公開", "後續擴充", "是否受機關補助", "是否含特別預算"
                };
                var tds = trNode.SelectNodes("./td");
                if (key == "標案案號" && tds.Count == 2)
                {
                    value = tds.Skip(1).First().GetDirectInnerText().TrimEmpty();
                    SetValue(ref 採購資料Po, key, value);
                }
                else if (key == "標的分類" && tds.Count == 1)
                {
                    value = tds.First().GetDirectInnerText().Replace(" ", string.Empty).TrimEmpty();
                    SetValue(ref 採購資料Po, key, value);
                }
                else if (key == "是否適用條約或協定之採購" && tds.Count == 1)
                {
                    key = string.Empty;
                    value = string.Empty;
                    var 是否適用條約或協定之採購Po = new 是否適用條約或協定之採購();
                    var innerHtmlSplit = tds.First().InnerHtml.Split("<strong>");
                    // ==================================================
                    // 是否適用WTO政府採購協定(GPA)
                    var sub1 = innerHtmlSplit[1].Split("</strong>");
                    var subKey1 = sub1[0].Replace("：", string.Empty).TrimEmpty();
                    var subValue1 = sub1[1].Replace("<hr>", string.Empty).TrimEmpty();
                    result.Add(subKey1, subValue1);
                    Console.WriteLine($"Key:{subKey1}\tValue:{subValue1}");
                    SetValue(ref 是否適用條約或協定之採購Po, subKey1, subValue1);
                    // ==================================================
                    // 是否適用臺紐經濟合作協定(ANZTEC)
                    var sub2 = innerHtmlSplit[2].Split("</strong>");
                    var subKey2 = sub2[0].Replace("：", string.Empty).TrimEmpty();
                    var subValue2 = sub2[1].Replace("<hr>", string.Empty).TrimEmpty();
                    result.Add(subKey2, subValue2);
                    Console.WriteLine($"Key:{subKey2}\tValue:{subValue2}");
                    SetValue(ref 是否適用條約或協定之採購Po, subKey2, subValue2);
                    // ==================================================
                    // 是否適用臺星經濟夥伴協定(ASTEP)
                    var sub3 = innerHtmlSplit[3].Split("</strong>");
                    var subKey3 = sub3[0].Replace("：", string.Empty).TrimEmpty();
                    var subValue3 = sub3[1].Replace("<hr>", string.Empty).TrimEmpty();
                    result.Add(subKey3, subValue3);
                    Console.WriteLine($"Key:{subKey3}\tValue:{subValue3}");
                    SetValue(ref 是否適用條約或協定之採購Po, subKey3, subValue3);
                    // ==================================================
                    採購資料Po.是否適用條約或協定之採購 = 是否適用條約或協定之採購Po;
                }
                else if (normalList.Contains(key) && tds.Count == 1)
                {
                    value = tds.First().GetDirectInnerText().Replace("<br>", string.Empty).TrimEmpty();
                    SetValue(ref 採購資料Po, key, value);
                }
                else
                {
                    Console.WriteLine($"程式終止:檢測到未處理的特殊欄位，請通知工程師進行例外處理，參數:{Environment.NewLine}" +
                                      $"tds.Count:{tds.Count}{Environment.NewLine}" +
                                      $"InnerHtml:{trNode.InnerHtml}{Environment.NewLine}");
                    break;
                }

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    result.Add(key, value);
                    Console.WriteLine($"Key:{key}\tValue:{value}");
                }
            }
            #endregion
            #region region3 招標資料
            foreach (var trNode in region3)
            {
                var key = "";
                var value = "";
                var ths = trNode.SelectNodes("./th");
                if (ths.Count != 1)
                {
                    Console.WriteLine($"程式終止:檢測到未處理的特殊欄位，請通知工程師進行例外處理，參數:{Environment.NewLine}" +
                                      $"ths.Count:{ths.Count}{Environment.NewLine}" +
                                      $"InnerHtml:{trNode.InnerHtml}{Environment.NewLine}");
                    break;
                }
                else
                {
                    key = ths.First().GetDirectInnerText().TrimEmpty();
                }

                var normalList = new string[]
                {
                    "是否依政府採購法施行細則第64條之2辦理", "新增公告傳輸次數", "招標狀態", "公告日",
                    "是否複數決標", "是否訂有底價", "是否屬特殊採購", "是否已辦理公開閱覽", "是否屬統包", "是否屬共同供應契約採購",
                    "是否屬二以上機關之聯合採購(不適用共同供應契約規定)", "是否應依公共工程專業技師簽證規則實施技師簽證",
                    "是否採行協商措施", "是否適用採購法第104條或105條或招標期限標準第10條或第4條之1",
                    "是否依據採購法第106條第1項第1款辦理"
                };
                var tds = trNode.SelectNodes("./td");
                if (key == "招標方式" && tds.Count == 2)
                {
                    value = tds.Skip(1).First().GetDirectInnerText().TrimEmpty();
                    SetValue(ref 招標資料Po, key, value);
                }
                else if (key == "決標方式" && tds.Count == 1)
                {
                    value = tds.First().GetDirectInnerText().TrimEmpty();
                    SetValue(ref 招標資料Po, key, value);
                }
                else if (normalList.Contains(key) && tds.Count == 1)
                {
                    value = tds.First().GetDirectInnerText().TrimEmpty();
                    SetValue(ref 招標資料Po, key, value);
                }
                else
                {
                    Console.WriteLine($"程式終止:檢測到未處理的特殊欄位，請通知工程師進行例外處理，參數:{Environment.NewLine}" +
                                      $"tds.Count:{tds.Count}{Environment.NewLine}" +
                                      $"InnerHtml:{trNode.InnerHtml}{Environment.NewLine}");
                    break;
                }

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    result.Add(key, value);
                    Console.WriteLine($"Key:{key}\tValue:{value}");
                }
            }
            #endregion
            #region region4 領投開標
            foreach (var trNode in region4)
            {
                var key = "";
                var value = "";
                var ths = trNode.SelectNodes("./th");
                if (ths.Count != 1)
                {
                    Console.WriteLine($"程式終止:檢測到未處理的特殊欄位，請通知工程師進行例外處理，參數:{Environment.NewLine}" +
                                      $"ths.Count:{ths.Count}{Environment.NewLine}" +
                                      $"InnerHtml:{trNode.InnerHtml}{Environment.NewLine}");
                    break;
                }
                else
                {
                    key = ths.First().GetDirectInnerText().TrimEmpty();
                }

                var normalList = new string[]
                {
                    "是否提供電子投標", "開標地點", "投標文字", "收受投標文件地點"
                };
                var tds = trNode.SelectNodes("./td");
                if (key == "是否提供電子領標" && tds.Count == 2)
                {
                    value = tds.Skip(1).First().GetDirectInnerText().TrimEmpty();
                    SetValue(ref 領投開標Po, key, value);
                    // --------------------------------------------------
                    if (value == "是")
                    {
                        var 是否提供電子領標Po = new 是否提供電子領標();
                        var detailTableTrs = tds.Skip(1).First().SelectNodes("./table/tr");
                        // ==================================================
                        // 機關文件費(機關實收)
                        var subKey1 = detailTableTrs[0].SelectSingleNode("./th").GetDirectInnerText().TrimEmpty();
                        var subValue1 = detailTableTrs[0].SelectSingleNode("./td").GetDirectInnerText().TrimEmpty();
                        result.Add(subKey1, subValue1);
                        Console.WriteLine($"Key:{subKey1}\tValue:{subValue1}");
                        SetValue(ref 是否提供電子領標Po, subKey1, subValue1);
                        // ==================================================
                        // 系統使用費
                        var subKey2 = detailTableTrs[1].SelectSingleNode("./th").GetDirectInnerText().TrimEmpty();
                        var subValue2 = detailTableTrs[1].SelectSingleNode("./td").GetDirectInnerText().TrimEmpty();
                        result.Add(subKey2, subValue2);
                        Console.WriteLine($"Key:{subKey2}\tValue:{subValue2}");
                        SetValue(ref 是否提供電子領標Po, subKey2, subValue2);
                        // ==================================================
                        // 文件代收費
                        var subKey3 = detailTableTrs[2].SelectSingleNode("./th").GetDirectInnerText().TrimEmpty();
                        var subValue3 = detailTableTrs[2].SelectSingleNode("./td").GetDirectInnerText().TrimEmpty();
                        result.Add(subKey3, subValue3);
                        Console.WriteLine($"Key:{subKey3}\tValue:{subValue3}");
                        SetValue(ref 是否提供電子領標Po, subKey3, subValue3);
                        // ==================================================
                        // 總計
                        var subKey4 = detailTableTrs[3].SelectSingleNode("./th").GetDirectInnerText().TrimEmpty();
                        var subValue4 = detailTableTrs[3].SelectSingleNode("./td").GetDirectInnerText().TrimEmpty();
                        result.Add(subKey4, subValue4);
                        Console.WriteLine($"Key:{subKey4}\tValue:{subValue4}");
                        SetValue(ref 是否提供電子領標Po, subKey4, subValue4);
                        // ==================================================
                        // 機關文件費指定收款機關單位
                        var i = 4;  // 用來判斷有無"機關文件費指定收款機關單位"欄位，若有則寫入，並且i+1
                        var subKey5 = detailTableTrs[i].SelectSingleNode("./td").GetDirectInnerText().Split("：")[0].TrimEmpty();
                        if (subKey5 == "機關文件費指定收款機關單位")
                        {
                            var subValue5 = detailTableTrs[4].SelectSingleNode("./td").GetDirectInnerText().Split("：")[1].TrimEmpty();
                            result.Add(subKey5, subValue5);
                            Console.WriteLine($"Key:{subKey5}\tValue:{subValue5}");
                            SetValue(ref 是否提供電子領標Po, subKey5, subValue5);
                            i++;
                        }
                        // ==================================================
                        // 是否提供現場領標
                        var subKey6 = detailTableTrs[i].SelectSingleNode("./td").GetDirectInnerText().Split("：")[0].TrimEmpty();
                        if (subKey6 == "是否提供現場領標")
                        {
                            var subValue6 = detailTableTrs[i].SelectSingleNode("./td").GetDirectInnerText().Split("：")[1].TrimEmpty();
                            result.Add(subKey6, subValue6);
                            Console.WriteLine($"Key:{subKey6}\tValue:{subValue6}");
                            SetValue(ref 是否提供電子領標Po, subKey6, subValue6);
                            // --------------------------------------------------
                            if (subValue6 == "是")
                            {
                                var subKey6DetailTableTrs = detailTableTrs[i].SelectSingleNode("./td").SelectNodes("./table/tr");
                                // 招標文件領取地點
                                var subKey6_1 = subKey6DetailTableTrs[0].SelectSingleNode("./th/strong").GetDirectInnerText().TrimEmpty();
                                var subValue6_1 = subKey6DetailTableTrs[0].SelectSingleNode("./td").GetDirectInnerText().TrimEmpty();
                                result.Add(subKey6_1, subValue6_1);
                                Console.WriteLine($"Key:{subKey6_1}\tValue:{subValue6_1}");
                                SetValue(ref 是否提供電子領標Po, subKey6_1, subValue6_1);
                                // 招標文件售價及付款方式
                                var subKey6_2 = subKey6DetailTableTrs[1].SelectSingleNode("./th/strong").GetDirectInnerText().TrimEmpty();
                                var subValue6_2 = subKey6DetailTableTrs[1].SelectSingleNode("./td").GetDirectInnerText().TrimEmpty();
                                result.Add(subKey6_2, subValue6_2);
                                Console.WriteLine($"Key:{subKey6_2}\tValue:{subValue6_2}");
                                SetValue(ref 是否提供電子領標Po, subKey6_2, subValue6_2);
                            }
                        }
                        // ==================================================
                        領投開標Po.是否提供電子領標Po = 是否提供電子領標Po;
                    }
                }
                else if (key == "截止投標" && tds.Count == 1)
                {
                    value = tds.First().InnerText.TrimEmpty();
                    SetValue(ref 領投開標Po, key, value);
                }
                else if (key == "開標時間" && tds.Count == 1)
                {
                    value = tds.First().InnerText.TrimEmpty();
                    SetValue(ref 領投開標Po, key, value);
                }
                else if (key == "是否須繳納押標金" && tds.Count == 1)
                {
                    value = tds.First().GetDirectInnerText().TrimEmpty();
                    SetValue(ref 領投開標Po, key, value);
                    // --------------------------------------------------
                    if (value.StartsWith("是"))
                    {
                        //內容過度複雜，不處理
                    }
                }
                else if (normalList.Contains(key) && tds.Count == 1)
                {
                    value = tds.First().GetDirectInnerText().TrimEmpty();
                    SetValue(ref 領投開標Po, key, value);
                }
                else
                {
                    Console.WriteLine($"程式終止:檢測到未處理的特殊欄位，請通知工程師進行例外處理，參數:{Environment.NewLine}" +
                                      $"tds.Count:{tds.Count}{Environment.NewLine}" +
                                      $"InnerHtml:{trNode.InnerHtml}{Environment.NewLine}");
                    break;
                }

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    result.Add(key, value);
                    Console.WriteLine($"Key:{key}\tValue:{value}");
                }
            }
            #endregion
            #region region5 其他
            foreach (var trNode in region5)
            {
                var key = "";
                var value = "";
                var ths = trNode.SelectNodes("./th");
                if (ths.Count != 1)
                {
                    Console.WriteLine($"程式終止:檢測到未處理的特殊欄位，請通知工程師進行例外處理，參數:{Environment.NewLine}" +
                                      $"ths.Count:{ths.Count}{Environment.NewLine}" +
                                      $"InnerHtml:{trNode.InnerHtml}{Environment.NewLine}");
                    break;
                }
                else
                {
                    key = ths.First().GetDirectInnerText().TrimEmpty();
                }

                var normalList = new string[]
                {
                    "履約地點", "履約期限", "是否刊登公報", "本案採購契約是否採用主管機關訂定之範本",
                    "是否訂有與履約能力有關之基本資格", "是否刊登英文公告"
                };
                var tds = trNode.SelectNodes("./td");
                if (key == "是否依據採購法第99條" && tds.Count == 2)
                {
                    value = tds.Skip(1).First().GetDirectInnerText().TrimEmpty();
                    SetValue(ref 其他Po, key, value);
                }
                else if (key == "廠商資格摘要" && tds.Count == 1)
                {
                    value = tds.First().SelectSingleNode("./table/tr/td").GetDirectInnerText().TrimEmpty();
                    SetValue(ref 其他Po, key, value);
                }
                else if (key == "附加說明" && tds.Count == 1)
                {
                    value = tds.First().InnerHtml.TrimEmpty().Replace("<br><br>", "\r\n").Replace("<br>檢舉受理單位", "檢舉受理單位").Replace("<br>", string.Empty);
                    SetValue(ref 其他Po, key, value);
                }
                else if (key == "疑義、異議、申訴及檢舉受理單位" && tds.Count == 1)
                {
                    key = string.Empty;
                    value = string.Empty;
                    var 疑義_異議_申訴及檢舉受理單位Po = new 疑義_異議_申訴及檢舉受理單位();
                    var detailTableTrs = tds.First().SelectNodes("./table/tr");
                    // ==================================================
                    // 疑義、異議受理單位
                    var subKey1 = detailTableTrs[0].SelectSingleNode("./th").GetDirectInnerText().TrimEmpty();
                    var subValue1 = detailTableTrs[0].SelectSingleNode("./td").GetDirectInnerText().TrimEmpty();
                    result.Add(subKey1, subValue1);
                    Console.WriteLine($"Key:{subKey1}\tValue:{subValue1}");
                    SetValue(ref 疑義_異議_申訴及檢舉受理單位Po, subKey1, subValue1);
                    // ==================================================
                    // 申訴受理單位
                    var subKey2 = detailTableTrs[2].SelectSingleNode("./th").GetDirectInnerText().TrimEmpty();
                    var subValue2 = detailTableTrs[2].SelectSingleNode("./td").GetDirectInnerText().TrimEmpty();
                    result.Add(subKey2, subValue2);
                    Console.WriteLine($"Key:{subKey2}\tValue:{subValue2}");
                    SetValue(ref 疑義_異議_申訴及檢舉受理單位Po, subKey2, subValue2);
                    // ==================================================
                    // 檢舉受理單位
                    var subKey3 = detailTableTrs[4].SelectSingleNode("./th").GetDirectInnerText().TrimEmpty();
                    var subValue3 = detailTableTrs[4].SelectSingleNode("./td").InnerHtml.TrimEmpty().Replace("<br>", "\r\n");
                    result.Add(subKey3, subValue3);
                    Console.WriteLine($"Key:{subKey3}\tValue:{subValue3}");
                    SetValue(ref 疑義_異議_申訴及檢舉受理單位Po, subKey3, subValue3);
                    // ==================================================
                    其他Po.疑義_異議_申訴及檢舉受理單位 = 疑義_異議_申訴及檢舉受理單位Po;
                }
                else if (normalList.Contains(key) && tds.Count == 1)
                {
                    value = tds.First().GetDirectInnerText().TrimEmpty();
                    SetValue(ref 其他Po, key, value);
                }
                else
                {
                    Console.WriteLine($"程式終止:檢測到未處理的特殊欄位，請通知工程師進行例外處理，參數:{Environment.NewLine}" +
                                      $"tds.Count:{tds.Count}{Environment.NewLine}" +
                                      $"InnerHtml:{trNode.InnerHtml}{Environment.NewLine}");
                    break;
                }

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    result.Add(key, value);
                    Console.WriteLine($"Key:{key}\tValue:{value}");
                }
            }
            #endregion

            po.機關資料 = 機關資料Po;
            po.採購資料 = 採購資料Po;
            po.招標資料 = 招標資料Po;
            po.領投開標 = 領投開標Po;
            po.其他 = 其他Po;
            return po;
        }

        /// <summary>
        /// 反射動態設定物件屬性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="po">反射物件</param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        private void SetValue<T>(ref T po, string propertyName, string value)
        {
            var propType = po.GetType();
            var propName = GetPropertyName(propertyName);
            PropertyInfo? prop = propType.GetProperty(propName);
            if (prop == null)
            {
                var errorMsg = $"程式終止:檢測到未處理的特殊欄位，請通知工程師進行例外處理，參數:{Environment.NewLine}" +
                               $"po:{po.GetType()}{Environment.NewLine}" +
                               $"propertyName:{propertyName}{Environment.NewLine}" +
                               $"value:{value}{Environment.NewLine}";
                Console.WriteLine(errorMsg);
                throw new Exception(errorMsg);
            }
            else
            {
                prop.SetValue(po, value);
            }

            // Replace特殊字元
            string GetPropertyName(string key)
            {
                new List<string> { "「", "(", ")", "」", "、" }.ForEach(x =>
                {
                    key = key.Replace(x, "_");
                });
                return key;
            }
        }

    }
}
