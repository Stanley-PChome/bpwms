using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Wms3pl.Common.Extensions;
using Wms3pl.WpfClient.Common.WcfDataServices;

namespace Wms3pl.WpfClient.DataServices.WcfDataServices
{
  public static class XDocumentToNonEntityExtensions
  {
    public static IEnumerable<T> ToNonEntities<T>(this XDocument doc)
    {
      XNamespace ns = "http://schemas.microsoft.com/ado/2009/11/dataservices";
      var result = from e in doc.Root.Elements(ns + "element")
                   select GetNonEntity<T>(e);
      return result;
    }

    public static T GetNonEntity<T>(XElement element)
    {
      XNamespace ns = "http://schemas.microsoft.com/ado/2009/11/dataservices";
      XNamespace p2 = "http://schemas.microsoft.com/ado/2009/11/dataservices/metadata";
      //XNamespace.Xmlns + "y", 
      //p2:type="Wms3pl.WebServices.P15.Dal.MoveOutData" xmlns:p2="http://schemas.microsoft.com/ado/2009/11/dataservices/metadata"

      T tempMyClass = (T)Activator.CreateInstance(typeof(T));

      var fis = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

      foreach (PropertyInfo fi in fis)
      {
        var propertyElement = element.Element(ns + fi.Name);
        if (propertyElement != null)
        {
          var edmtype = propertyElement.Attribute(p2 + "type");
          var propertyValue = propertyElement.Value;
          if (edmtype == null)
            fi.SetValue(tempMyClass, propertyValue, null);
          else
          {
            if (string.IsNullOrEmpty(propertyValue))
              fi.SetValue(tempMyClass, null, null);
            else
            {
              SetNewValue(tempMyClass, propertyValue, fi);
            }
          }
        }
      }
      return tempMyClass;
    }

    public static void SetNewValue<T>(T tempMyClass, string propertyValue, PropertyInfo fi)
    {
      bool isNullable = TypeUtility.IsNullableType(fi.PropertyType); //.GetGenericTypeDefinition() == typeof(Nullable<>);
      if (isNullable)
      {
        var value = ClientConvert.ChangeType(propertyValue, Nullable.GetUnderlyingType(fi.PropertyType));
        fi.SetValue(tempMyClass, value, null);
      }
      else
      {
        var value = ClientConvert.ChangeType(propertyValue, fi.PropertyType);
        fi.SetValue(tempMyClass, value, null);
      }
    }
  }
}
