using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Wms3pl.WebServices.DataCommon
{
	public class CommonFunctions
	{
		public static string TO_NVL(string source, string isNullVal = "0", string aliasName = "")
		{
			string str = String.Format("NVL({0}, {1})", source, isNullVal);
			if (String.IsNullOrWhiteSpace(aliasName))
			{
				var pos = source.IndexOf(".");
				if (pos > -1)
					aliasName = source.Substring(pos + 1, source.Length - pos - 1);
				else
					aliasName = source;
			}
			str += " AS " + aliasName;

			return str;
		}

		public static void LogSql(string sql)
		{
			var entry = new LogEntry()
										{
											Message = sql,
											Categories = new string[] { "Sql" }
										};
			Logger.Write(entry);
		}

		public static string TO_DATE(string dateText)
		{
			return string.Format(" TO_DATE('{0}', 'yyyy/mm/dd')", dateText);
		}


	}
}
