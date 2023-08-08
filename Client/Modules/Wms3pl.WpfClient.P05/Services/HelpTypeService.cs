using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Text;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;

using Wms3pl.WpfClient.DataServices;
using System.Reflection;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;

namespace Wms3pl.WpfClient.P05.Services
{
    public static class HelpTypeService
    {
		/// <summary>
		/// 求救類型
		/// 對應到F0010.HELP_TYPE
		/// </summary>
		/// <returns></returns>
			public static List<NameValuePair<string>> HelpType(string functionCode)
		{
			return GetBaseTableService.GetF000904List(functionCode, "F0010", "HELP_TYPE");
		}
		/// <summary>
		/// 求救狀態
		/// 對應到F0010.STATUS
		/// </summary>
		/// <returns></returns>
			public static List<NameValuePair<string>> HelpStatus(string functionCode)
		{
			return GetBaseTableService.GetF000904List(functionCode, "F0010", "STATUS");
		}

    }
}
