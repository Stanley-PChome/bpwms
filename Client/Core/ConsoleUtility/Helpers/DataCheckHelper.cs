using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.Common.Helpers;

namespace ConsoleUtility.Helpers
{
  public class DataCheckHelper
  {
    public static string CheckInt(string content, string key, string fieldName)
    {
      var errorString = string.Empty;
      int tmp;
      if (!int.TryParse(content, out tmp))
        errorString = $"欄位格式錯誤!!傳入資料'{key}'-[{fieldName}]欄位格式錯誤，'{content}'無法轉換成數值格式";
      return errorString;
    }

    public static string CheckDecimal(string content, string key, string fieldName)
    {
      var errorString = string.Empty;
      decimal tmp;
      if (!decimal.TryParse(content, out tmp))
        errorString = $"欄位格式錯誤!!傳入資料'{key}'-[{fieldName}]欄位格式錯誤，'{content}'無法轉換成數值格式";
      return errorString;
    }

    public static string CheckDateTime(string content, string key, string fieldName)
    {
      var errorString = string.Empty;
      if (!DateTimeHelper.CheckDate(content))
        errorString = $"欄位格式錯誤!!傳入資料'{key}'-[{fieldName}]欄位格式錯誤，'{content}'無法轉換成日期格式";
      return errorString;
    }

    public static string CheckABPOperatorPeriodType(string content, string key, string fieldName)
    {
      var errorString = string.Empty;
      if (content != "上午" && content != "下午")
        errorString = $"欄位格式錯誤!!傳入資料'{key}'-[{fieldName}]欄位格式錯誤，'{content}'非已知時段";
      return errorString;
    }
  }
}
