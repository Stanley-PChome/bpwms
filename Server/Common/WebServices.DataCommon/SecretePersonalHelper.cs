using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.DataCommon
{
	public class SecretePersonalHelper
	{
		public static string SecretePersonalColumn(string value, string hideRule)
		{
			if (string.IsNullOrEmpty(value) || hideRule == "NOT")
				return value;
			switch (hideRule)
			{
				case "TEL":
					if (value.Length > 3)
						return string.Format("{0}XXX", value.Substring(0, value.Length - 3));
					else
						return string.Empty.PadRight(value.Length, 'X');
				case "ADDR":
					if (value.Length > 3)
						return string.Format("{0}{1}", value.Substring(0, 3), string.Empty.PadRight(value.Length - 3, 'X'));
					else
						return string.Empty.PadRight(value.Length, 'X');
				case "NAME":
					if (value.Length > 1)
						return string.Format("{0}{1}", value.Substring(0, 1), string.Empty.PadRight(value.Length - 1, 'X'));
					else
						return string.Empty.PadRight(value.Length, 'X');
				default:
					return string.Empty.PadRight(value.Length, 'X');
			}
		}
	}
}
