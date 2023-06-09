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
using Wms3pl.WpfClient.P02.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.DataServices.F01DataService;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
//using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WpfClient.P02.Services
{
    public static class VendorService
    {

		/// <summary>
		/// 依進倉單號取得廠商資訊
		/// </summary>
			public static VendorInfo GetVendorInfo(string purchaseNo, string dcCode, string gupCode, string custCode, bool isSecretePersonalData, string functionCode)
		{
			var proxy = ConfigurationExHelper.GetExProxy<P02ExDataSource>(isSecretePersonalData, functionCode);
			var tmp = proxy.CreateQuery<VendorInfo>("GetVendorInfo")
				.AddQueryOption("purchaseNo", string.Format("'{0}'", purchaseNo))
				.AddQueryOption("dcCode", string.Format("'{0}'", dcCode))
				.AddQueryOption("gupCode", string.Format("'{0}'", gupCode))
				.AddQueryOption("custCode", string.Format("'{0}'", custCode)).ToList();

			if (tmp != null)
			{
				return tmp.FirstOrDefault();
			}
			else
			{
				return null;
			}
		}

    }
}
