using System;
using System.Collections;
using System.Configuration;
using System.Data.Services.Client;
using Wms3pl.WpfClient.Common.WcfDataServices;
using Wms3pl.WpfClient.ExDataServices.P01ExDataService;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.ExDataServices.P14ExDataService;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.ExDataServices.P20ExDataService;
using Wms3pl.WpfClient.ExDataServices.P70ExDataService;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.ExDataServices.P06ExDataService;
using Wms3pl.WpfClient.ExDataServices.P15ExDataService;
using Wms3pl.WpfClient.ExDataServices.P16ExDataService;
using Wms3pl.WpfClient.ExDataServices.P18ExDataService;
using Wms3pl.WpfClient.ExDataServices.P25ExDataService;
using Wms3pl.WpfClient.ExDataServices.P91ExDataService;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.ExDataServices.P50ExDataService;
using Wms3pl.WpfClient.ExDataServices.T05ExDataService;
using Wms3pl.WpfClient.ExDataServices.R01ExDataService;
using Wms3pl.WpfClient.ExDataServices.P21ExDataService;
using Wms3pl.WpfClient.ExDataServices.SignalRExDataService;

namespace Wms3pl.WpfClient.ExDataServices
{

	public class ConfigurationExHelper
	{
		private static Hashtable DataServiceUriStrings
		{
			get
			{
				var uriStrings = new Hashtable();
				uriStrings.Add(typeof(P01ExDataSource), "{0}/P01ExDataService.svc/");
				uriStrings.Add(typeof(P02ExDataSource), "{0}/P02ExDataService.svc/");
				uriStrings.Add(typeof(P05ExDataSource), "{0}/P05ExDataService.svc/");
				uriStrings.Add(typeof(P06ExDataSource), "{0}/P06ExDataService.svc/");
				uriStrings.Add(typeof(P08ExDataSource), "{0}/P08ExDataService.svc/");
				uriStrings.Add(typeof(P14ExDataSource), "{0}/P14ExDataService.svc/");
				uriStrings.Add(typeof(P15ExDataSource), "{0}/P15ExDataService.svc/");
				uriStrings.Add(typeof(P16ExDataSource), "{0}/P16ExDataService.svc/");
				uriStrings.Add(typeof(P18ExDataSource), "{0}/P18ExDataService.svc/");
				uriStrings.Add(typeof(P19ExDataSource), "{0}/P19ExDataService.svc/");
				uriStrings.Add(typeof(P20ExDataSource), "{0}/P20ExDataService.svc/");
				uriStrings.Add(typeof(P21ExDataSource), "{0}/P21ExDataService.svc/");
				uriStrings.Add(typeof(P25ExDataSource), "{0}/P25ExDataService.svc/");
				uriStrings.Add(typeof(P50ExDataSource), "{0}/P50ExDataService.svc/");
				uriStrings.Add(typeof(P70ExDataSource), "{0}/P70ExDataService.svc/");
				uriStrings.Add(typeof(P71ExDataSource), "{0}/P71ExDataService.svc/");
				uriStrings.Add(typeof(P91ExDataSource), "{0}/P91ExDataService.svc/");
				uriStrings.Add(typeof(ShareExDataSource), "{0}/ShareExDataService.svc/");
				uriStrings.Add(typeof(T05ExDataSource), "{0}/T05ExDataService.svc/");
				uriStrings.Add(typeof(R01ExDataSource), "{0}/R01ExDataService.svc/");
				uriStrings.Add(typeof(SignalRExDataSource), "{0}/SignalRExDataService.svc/");
				return uriStrings;
			}
		}

		public static T GetExProxy<T>(bool isSecretePersonalData, string functionCode, bool enableIgnoreProperties = true, bool isLongTermSchema = false) where T : DataServiceContext
		{
			var apServer = ConfigurationManager.AppSettings["APServerUrl"];
			var uri = new Uri(string.Format(DataServiceUriStrings[typeof(T)].ToString(), apServer));
			var proxy = Activator.CreateInstance(typeof(T), uri, enableIgnoreProperties) as T;
			proxy.IgnoreResourceNotFoundException = true;
			proxy.MergeOption = MergeOption.NoTracking;
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			if (globalInfo != null)
			{
				globalInfo.IsSecretePersonalData = isSecretePersonalData;
				globalInfo.FunctionCode = functionCode;
			}
			proxy.BuildingRequest += DataServiceContextEx.BuildingRequest;
			proxy.SendingRequest2 += DataServiceContextEx.OnSendingRequestEx;
			return proxy;
		}
	}
}
