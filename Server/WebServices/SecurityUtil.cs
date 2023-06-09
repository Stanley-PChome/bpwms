using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Globalization;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F19;
using Wms3pl.DBCore;

namespace Wms3pl.WebServices
{
  public class SecurityUtil
  {
    internal static string GetStringValue(NameValueCollection config, string valueName, string defaultValue)
    {
      string value = config[valueName];
      if (string.IsNullOrWhiteSpace(value))
        return defaultValue;
      return value;
    }

    internal static int GetIntValue(NameValueCollection config, string valueName, int defaultValue, bool zeroAllowed, int maxValueAllowed)
    {
      string sValue = config[valueName];

      if (sValue == null)
      {
        return defaultValue;
      }

      int iValue;
      if (!Int32.TryParse(sValue, out iValue))
      {
        if (zeroAllowed)
        {
          throw new ProviderException("Value_must_be_non_negative_integer, " + valueName);
        }

        throw new ProviderException("Value_must_be_positive_integer, " + valueName);
      }

      if (zeroAllowed && iValue < 0)
      {
        throw new ProviderException("Value_must_be_non_negative_integer, " + valueName);
      }

      if (!zeroAllowed && iValue <= 0)
      {
        throw new ProviderException("Value_must_be_positive_integer, " + valueName);
      }

      if (maxValueAllowed > 0 && iValue > maxValueAllowed)
      {
        throw new ProviderException("Value_too_big, " + valueName + ", max: " + maxValueAllowed.ToString(CultureInfo.InvariantCulture));
      }

      return iValue;
    }

    internal static bool GetBooleanValue(NameValueCollection config, string valueName, bool defaultValue)
    {
      string sValue = config[valueName];
      if (sValue == null)
      {
        return defaultValue;
      }

      bool result;
      if (bool.TryParse(sValue, out result))
      {
        return result;
      }
      else
      {
        throw new ProviderException("Value_must_be_boolean, " + valueName);
      }
    }

    internal static void CheckParameter(ref string param, bool checkForNull, bool checkIfEmpty, bool checkForCommas, int maxSize, string paramName)
    {
      if (param == null)
      {
        if (checkForNull)
        {
          throw new ArgumentNullException(paramName);
        }

        return;
      }

      param = param.Trim();
      if (checkIfEmpty && param.Length < 1)
      {
        throw new ArgumentException("Parameter_can_not_be_empty, paramName", paramName);
      }

      if (maxSize > 0 && param.Length > maxSize)
      {
        throw new ArgumentException(paramName + " Parameter_too_long, max:" + maxSize.ToString(CultureInfo.InvariantCulture), paramName);
      }

      if (checkForCommas && param.Contains(","))
      {
        throw new ArgumentException("Parameter_can_not_contain_comma, " + paramName, paramName);
      }
    }

    public static Wms3plDbContext GetContext()
    {
      return DataServiceBase<Wms3plDbContext>.GetDataContext();
      //return new Wms3plDbContext();
    }

    public static Wms3plDbContext GetContext(string schema)
    {
      return DataServiceBase<Wms3plDbContext>.GetDataContext(schema);
      //return new Wms3plDbContext();
    }


  }
}
