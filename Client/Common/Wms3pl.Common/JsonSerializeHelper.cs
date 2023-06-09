using System;
using System.Web;
using System.Web.Script.Serialization;

namespace Wms3pl.Common
{
  public class JsonSerializeHelper
  {
    public static string JsonSerialize(object o)
    {
      var oSerializer = new JavaScriptSerializer();
      return HttpUtility.UrlEncode(oSerializer.Serialize(o));
    }

    public static T JsonDeserialize<T>(string jsonobj)
    {
      //修正JsonDeserialize，DateTime會變為UTC時間問題
      var obj = (T)new JavaScriptSerializer().Deserialize(HttpUtility.UrlDecode(jsonobj), typeof(T));
      var type = obj.GetType();
      var propertyInfos = type.GetProperties();
      foreach (var propertyInfo in propertyInfos)
      {
        if (propertyInfo.PropertyType == typeof(DateTime))
        {
          propertyInfo.SetValue(obj, ((DateTime)propertyInfo.GetValue(obj, null)).ToLocalTime(), null);
        }
      }
      return obj;

      //var oSerializer = new JavaScriptSerializer();
      //return (T)oSerializer.Deserialize(HttpUtility.UrlDecode(jsonobj), typeof(T));
    }
  }
}
