using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.Common.Helpers
{
	public partial class DataServiceQueryHelper
	{
		public static object CombineTypeFormatString(object value)
		{
			if (value == null)
				return null;

			if (value is string)
			{
				var strValue = Convert.ToString(value);
				strValue = strValue.Replace("'", "''");
				strValue = strValue.Replace("&", "chr(38)");
				strValue = strValue.Replace("#", "chr(35)");
				strValue = System.Web.HttpUtility.UrlEncode(strValue);
				return string.Format("'{0}'", strValue);
			}
			else if (value is DateTime)
			{
				return string.Format("DateTime'{0:yyyy-MM-ddTHH:mm:ss}'", (DateTime)value);
			}
			else if (value is bool)
			{
				return value.ToString().ToLower();
			}
			else if (value is decimal)
			{
				return string.Format("{0}M", value);
			}
			else
			{
				return value;
			}
		}
	}
}
