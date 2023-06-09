using System.Configuration;

namespace Wms3pl.Common
{
  /// <summary>
  /// 欄位長度
  /// </summary>
  public class FieldsLengths
  {
    public static int ORD_NO_Length
    {
      get
      {
        return GetFileldLength("ORD_NO_Length", 13);
      }
    }


    public static int ORD_SEQ_Length
    {
      get
      {
        return GetFileldLength("ORD_SEQ", 2);
      }
    }

    public static int CUST_ORD_NO_Length
    {
      get
      {
        return GetFileldLength("CUST_ORD_NO", 20);
      }
    }

    public static int CUST_ORD_SEQ_Length
    {
      get
      {
        return GetFileldLength("CUST_ORD_SEQ", 3);
      }
    }

    public static int WMS_ORD_NO_Length
    {
      get
      {
        return GetFileldLength("WMS_ORD_NO", 12);
      }
    }

    public static int WMS_ORD_SEQ_Length
    {
      get
      {
        return GetFileldLength("WMS_ORD_SEQ", 3);
      }
    }

    public static int RT_NO_Length
    {
      get
      {
        return GetFileldLength("RT_NO", 10);
      }
    }


    public static int RT_SEQ_Length
    {
      get
      {
        return GetFileldLength("RT_SEQ", 2);
      }
    }

    public static int VRT_NO_Length
    {
      get
      {
        return GetFileldLength("VRT_NO", 13);
      }
    }

    public static int VRT_SEQ_Length
    {
      get
      {
        return GetFileldLength("VRT_SEQ", 3);
      }
    }

    public static int ITEM_CODE_Length
    {
      get
      {
        return GetFileldLength("ITEM_CODE", 6);
      }
    }

    private static int GetFileldLength(string fieldName, int defaultValue)
    {
      var str = ConfigurationManager.AppSettings[fieldName];

      if (!string.IsNullOrWhiteSpace(str))
      {
        var value = int.Parse(str);
        return value;
      }
      else
      {
        return defaultValue; //default value
      }
    }
  }
}
