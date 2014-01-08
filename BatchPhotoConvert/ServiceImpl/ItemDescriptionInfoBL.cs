using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BatchPhotoConvert
{
    public class ItemDescriptionInfoBL : IItemDescriptionServiceInfo
    {
        public string GetWapHtml(string wwwHtml)
        {
            try
            {
                test();
                string wapHtml = wwwHtml;
                //清空标签样式（除包含rowspan，rowspan，src）
                wapHtml = RegexDeleteHtmlStyle1(wwwHtml);
                //清空包含(colspan，rowspan，src）属性的html标签样式,保留
                wapHtml = RegexDeleteHtmlStyle2(wapHtml);
                return wapHtml;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void test()
        {
            string srtSql = @"SELECT TOP 100 ID ,ItemNO ,Description,WapDescription ,CreateUser ,CreateDate ,EditUser ,EditDate FROM    im.dbo.ItemDescriptionInfo";
            DataTable dt = ItemDescriptionInfoDA.GetItemDescriptionInfo(srtSql);
            //DataTable dt = new DataTable();
            //dt.Columns.Add(new DataColumn("ID", typeof(int)));
            //dt.Columns.Add(new DataColumn("ItemNO", typeof(string)));
            //dt.Columns.Add(new DataColumn("Description", typeof(string)));
            //dt.Columns.Add(new DataColumn("WapDescription", typeof(string)));
            //dt.Columns.Add(new DataColumn("CreateDate", typeof(DateTime)));

            //DataRow dr = dt.NewRow();
            //dr["ID"] = 1;
            //dr["ItemNO"] = "10001001";
            //dr["Description"] = "itemDescription";
            //dr["WapDescription"] = "itemWapDescription";
            //dt.Rows.Add(dr);

            IList<ItemDescriptionInfo> itemDescriptionList = DtToModel<ItemDescriptionInfo>.DtToModelList(dt);
            foreach (var item in itemDescriptionList)
            {

            }
        }
        //清空包含(rowspan，rowspan，src）属性的html标签样式
        /// <summary>
        /// 清空包含(rowspan，rowspan，src）属性的html标签样式
        /// </summary>
        /// <param name="wwwHtml"></param>
        /// <returns></returns>
        private string RegexDeleteHtmlStyle2(string wwwHtml)
        {
            string wapHtml = wwwHtml;
            string colspanRegex = @"<(?<element>\w+)[^>]*colspan\s*=\s*['\""](?<colspan>[^'\""]*)['\""][^>]*>|";
            string rowspanRegex = @"<(?<element>\w+)[^>]*rowspan\s*=\s*['\""](?<rowspan>[^'\""]*)['\""][^>]*>|";
            string srcRegex = @"<(?<element>\w+)[^>]*src\s*=\s*['\""](?<src>[^'\""]*)['\""][^>]*>|";
            //string colspanRegex = @"<(?<element>\w+)[^>]*colspan\s*=\s*['\""](?<colspan>[^'\""]*)['\""][^>]*>|";
            //string rowspanRegex = @"<(?<element>\w+)[^>]*rowspan\s*=\s*['\""](?<rowspan>[^'\""]*)['\""][^>]*>|";
            //string srcRegex = @"<(?<element>\w+)[^>]*src\s*=\s*['\""](?<src>[^'\""]*)['\""][^>]*>|";
            Regex regexDeleteStyle = new Regex(colspanRegex + rowspanRegex + srcRegex, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            MatchCollection matchCollection = regexDeleteStyle.Matches(wwwHtml);
            foreach (Match matchItem in matchCollection)
            {
                Group gp0 = matchItem.Groups[0];
                Group gp1 = matchItem.Groups[1];
                Group gpColspan = matchItem.Groups["colspan"];
                Group gpRowspan = matchItem.Groups["rowspan"];
                Group gpSrc = matchItem.Groups["src"];

                if (gp0 != null && gp1 != null)
                {
                    if (gpColspan != null && !string.IsNullOrEmpty(gpColspan.ToString()))
                    {
                        //if (!string.IsNullOrEmpty(gp0.ToString()) && !string.IsNullOrEmpty(gp1.ToString()) && !string.IsNullOrEmpty(gpColspan.ToString()))
                        //{
                        wapHtml = wapHtml.Replace(gp0.ToString(), "<" + gp1.ToString() + " colspan=\"" + gpColspan.ToString() + "\">");
                        //}
                    }
                    else if (gpRowspan != null && !string.IsNullOrEmpty(gpRowspan.ToString()))
                    {
                        //if (!string.IsNullOrEmpty(gp0.ToString()) && !string.IsNullOrEmpty(gp1.ToString()) && !string.IsNullOrEmpty(gpRowspan.ToString()))
                        //{
                        wapHtml = wapHtml.Replace(gp0.ToString(), "<" + gp1.ToString() + " rowspan=\"" + gpRowspan.ToString() + "\">");
                        //}
                    }
                    else if (gpSrc != null && !string.IsNullOrEmpty(gpSrc.ToString()))
                    {
                        //if (!string.IsNullOrEmpty(gp0.ToString()) && !string.IsNullOrEmpty(gp1.ToString()) && !string.IsNullOrEmpty(gpSrc.ToString()))
                        //{
                        wapHtml = wapHtml.Replace(gp0.ToString(), "<" + gp1.ToString() + " src=\"" + gpSrc.ToString() + "\">");
                        //}
                    }
                }
            }
            return wapHtml;
        }

        //清空html标签样式（除包含属性rowspan，rowspan，src的标签）
        /// <summary>
        /// 清空html标签样式（除包含rowspan，rowspan，src）
        /// </summary>
        /// <param name="wwwHtml"></param>
        /// <returns></returns>
        private string RegexDeleteHtmlStyle1(string wwwHtml)
        {
            string wapHtml = wwwHtml;
            Regex regexDeleteStyle = new Regex(@"<(?<element>\w+)(?![^<>]*?rowspan?\s*=[^<>]*?>)(?![^<>]*?colspan?\s*=[^<>]*?>)(?![^<>]*?src?\s*=[^<>]*?>)\s+?.*?>  ", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            MatchCollection matchCollection = regexDeleteStyle.Matches(wwwHtml);
            foreach (Match matchItem in matchCollection)
            {
                Group gp1 = matchItem.Groups[0];
                Group gp2 = matchItem.Groups[1];
                if (gp1 != null && gp2 != null)
                {
                    if (!string.IsNullOrEmpty(gp2.ToString()) && !string.IsNullOrEmpty(gp1.ToString()))
                    {
                        wapHtml = wapHtml.Replace(gp1.ToString(), "<" + gp2.ToString() + ">");
                    }
                }
            }
            return wapHtml;
        }
    }
}
