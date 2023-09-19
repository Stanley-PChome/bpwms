using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Xml.Linq;
using Wms3pl.WebServices.DataCommon.Enums;

namespace Wms3pl.WebServices.DataCommon
{
    public static class DbSchemaHelper
    {
        public static string GetSchema()
        {
            //先由Cookies裡找Schema資訊(由Wcf Data Service Request送出會將Schema寫於此)
            //var schema = HttpContext.Current.Request.Headers["schema"];
            var schema = string.Empty;
            var isSchedule = false;
            if (HttpContext.Current != null)
            {
                schema = HttpContext.Current.Request.Headers["schema"];
                isSchedule = AesCryptor.Current.Decode(HttpContext.Current.Request.Headers["IsSchedule"]) == "a1234567";
            }

            //如果沒有找到，才再由MessageHeader中找Schema資訊(由WCF Service Request送出會將Schema寫於此)
            if (string.IsNullOrEmpty(schema))
            {
                var headerIndex = -1;
                var scheduleIndex = -1;
                if (OperationContext.Current != null)
                {
                    headerIndex = OperationContext.Current.IncomingMessageHeaders.FindHeader("schema", "http://Wms3pl");
                    scheduleIndex = OperationContext.Current.IncomingMessageHeaders.FindHeader("IsSchedule", "http://Wms3pl");

                }
                if (headerIndex < 0)
                {
                    //var defaultSchema = WebConfigurationManager.AppSettings["Schema"];
                    //return defaultSchema;
                    if (WebConfigurationManager.AppSettings["IsEnabledChangeDb"].ToString() == "0")
                    {
                        return WebConfigurationManager.AppSettings["Schema"].ToString();
                    }
                    //return null;
                }
                else
                {
                    var r = OperationContext.Current.IncomingMessageHeaders.GetReaderAtHeader(headerIndex).ReadSubtree();
                    var data = XElement.Load(r);
                    schema = (string)data;

                    if (scheduleIndex > 0)
                    {
                        var r1 = OperationContext.Current.IncomingMessageHeaders.GetReaderAtHeader(headerIndex).ReadSubtree();
                        var data1 = XElement.Load(r1);
                        isSchedule = AesCryptor.Current.Decode(((string)data)) == "a1234567";
                    }
                }
                
            }
            //是否啟用指定切換資料庫(0:否 1:是)
            if (WebConfigurationManager.AppSettings["IsEnabledChangeDb"].ToString() == "0")
            {
                schema = WebConfigurationManager.AppSettings["Schema"].ToString();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(schema))
                {
                    if (isSchedule || HttpContext.Current != null && HttpContext.Current.User.Identity.IsAuthenticated)
                    {
                        schema = ChangeRealSchema(schema);
                    }
                    else
                        schema = string.Empty;
                }
            }

            if (string.IsNullOrWhiteSpace(schema))
            {
                var dbKey = GetConnBySCCode();

                schema = dbKey != null ? dbKey.ToString() : string.Empty;
            }

            return schema;
        }
        public static string ChangeRealSchema(string encodeSchema)
        {
            var schema = string.Empty;

            //是否啟用指定切換資料庫(0:否 1:是)
            if (WebConfigurationManager.AppSettings["IsEnabledChangeDb"].ToString() == "0")
            {
                schema = WebConfigurationManager.AppSettings["Schema"].ToString();
            }
            else
            {
                //##TRANSON##是判斷是否顯示Login的多語系下拉是否顯示所以在這邊需移除
                var start = AesCryptor.Current.Decode(encodeSchema).IndexOf('#');
                var end = AesCryptor.Current.Decode(encodeSchema).LastIndexOf('#');

                var str = (start == -1 || end == -1) ? string.Empty : AesCryptor.Current.Decode(encodeSchema).Substring(start, (end - start) + 1);
                var decodeValue = string.Empty;
                if (!string.IsNullOrWhiteSpace(str))
                    decodeValue = AesCryptor.Current.Decode(encodeSchema).Replace(str, "");
                else
                    decodeValue = AesCryptor.Current.Decode(encodeSchema);
                //如果解密失敗 防止被猜到而登入
                if (decodeValue == encodeSchema)
                    return schema;
                switch (decodeValue)
                {
									case "BP681BPWMSAZ60MSSQLWMS":
										schema = "BPWMS_MSSQL";
										break;
									case "BP5E3C4W3M8SMMSWMS":
										schema = "PHWMS_DEV";
										break;
                  case "BPE3I1J2F8KW2T5WMS":
										schema = "PHWMS_DEV_A7";
										break;
									case "BP58A2EJ51AMOMSWMS":
										schema = "PHWMS_PDT";
										break;
						      case "BP23A5B4A01KXO2WMS":
										schema = "PHWMS_PDT_A7";
						        break;
                  default:
                    schema = string.Empty;
                    break;
                }
            }
            return schema;
        }
        /// <summary>
        /// 取得連線授權
        /// </summary>
        /// <returns></returns>
        public static DbKeyEnum? GetConnBySCCode()
        {
            DbKeyEnum? res = null;

            if (HttpContext.Current != null)
            {
                var sccode = HttpContext.Current.Request.Headers["SCCode"];
                var decode = AesCryptor.Current.Decode(sccode);

                if (!string.IsNullOrWhiteSpace(decode) && decode.Length == 10)
                {
                    // decode值前7碼
                    string firstDecode = decode.Substring(0, 7);

                    // decode值後3碼
                    string lastDecode = decode.Substring(7, 3);

                    // decode值前7碼將每一個字元透由ASCII轉換數值並累加後值+後3碼數值 MOD 174 取餘數值
                    int key = (Encoding.ASCII.GetBytes(firstDecode).Sum(x => x) + Convert.ToInt32(lastDecode)) % 174;

                    // 找出符合key的Enum
                    var dbKeyEnums = System.Enum.GetValues(typeof(DbKeyEnum)).Cast<DbKeyEnum>();
                    var currKeyEnum = dbKeyEnums.Where(x => (int)x == key);
                    res = currKeyEnum.Count() > 0 ? currKeyEnum.FirstOrDefault() : default(DbKeyEnum?);
                }
            }

            return res;
        }

        /// <summary>
        /// 檢核主機授權碼
        /// </summary>
        /// <returns></returns>
        public static bool CheckSCCode()
        {
            return GetConnBySCCode() != null;
        }

        /// <summary>
        /// 產生主機授權碼
        /// </summary>
        /// <param name="dbKeyEnum"></param>
        /// <returns></returns>
        public static string GenSCCode(DbKeyEnum dbKeyEnum)
        {
            // 前7碼值:取得A-Z,a-z,0-9亂數產生7碼
            string firstDecode = GetRandomString(7);

            // 再將亂數7碼轉ASCII值累加後MOD 174取得餘數
            int firstDecodeAsciiSumMOD = Encoding.ASCII.GetBytes(firstDecode).Sum(x => x) % 174;

            // 後3碼值:用174-餘數+dbKey值 (不足3碼前面補0)
            string lastDecode = Convert.ToString(174 - firstDecodeAsciiSumMOD + (int)dbKeyEnum).PadLeft(3, '0');

            return $"{firstDecode}{lastDecode}";
        }

        /// <summary>
        /// 從A-Z,a-z,0-9亂數產生length碼
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string GetRandomString(int length)
        {
            var str = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var next = new Random();
            var builder = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                builder.Append(str[next.Next(0, str.Length)]);
            }
            return builder.ToString();
        }
    }
}
