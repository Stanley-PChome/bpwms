using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Text;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.Services;

using Wms3pl.WpfClient.DataServices;
using System.Reflection;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;

namespace Wms3pl.WpfClient.P08.Services
{
	public static class SourceTypeService
	{
		
		/// <summary>
		/// 出貨單來源單據
		/// 對應到F050801.SOURCE_TYPE
		/// </summary>
		/// <returns></returns>
		public static List<NameValuePair<string>> GetSourceType(string functionCode)
		{
			var proxy = ConfigurationHelper.GetProxy<F00Entities>(false, functionCode);

			var data = proxy.F000902s.ToList();
			var list = (from o in data
				select new NameValuePair<string>
				{
					Name = o.SOURCE_NAME,
					Value = o.SOURCE_TYPE
				}).ToList();
			return list;
		}
	}
}
