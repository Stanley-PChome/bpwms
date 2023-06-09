using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Common.Enums;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;

namespace Wms3pl.WebServices.Shared
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class ShareExDataService : DataService<ShareExDataSource>
	{
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("*", EntitySetRights.All);
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

		[WebGet]
		public IQueryable<string> GetNewOrdCode(string ordType)
		{
			var srv = new SharedService();
			return (new List<string> { srv.GetNewOrdCode(ordType) }).AsQueryable();
		}

		[WebGet]
		public IQueryable<F050801WmsOrdNo> GetF050801ListBySourceNo(string dcCode, string gupCode, string custCode,
			string sourceNo)
		{
			var srv = new SharedService();
			return srv.GetF050801ListBySourceNo(dcCode, gupCode, custCode, sourceNo);
		}

		[WebGet]
		public IQueryable<SerialNoResult> CheckSerialNoFull(string dcCode, string gupCode, string custCode, string itemCode, string serialNo,
			string status, string ignoreCheckOfStatus = "", string isCombinCheck = "0")
		{
			var srv = new SerialNoService();
			if (isCombinCheck == "1")
			{
				var combinF2501List = new List<F2501>();
				var combinItemCode = string.Empty;
				var isCombinItem = srv.IsCombinItem(gupCode, custCode, serialNo, out combinF2501List, out combinItemCode);
				if (isCombinItem)
				{
					itemCode = combinItemCode;
					var item = combinF2501List.FirstOrDefault(o => o.ITEM_CODE == combinItemCode);
					if (item != null)
						serialNo = item.SERIAL_NO;
				}
			}
			var serialNoResult = srv.CheckSerialNoFull(dcCode, gupCode, custCode, itemCode, serialNo, status,
				ProcessWork.ScanSerial, ignoreCheckOfStatus);
			return serialNoResult.AsQueryable();
		}

		[WebGet]
		public IQueryable<SerialNoResult> GetSerialItem(string dcCode, string gupCode, string custCode, string barCode, string isCombinCheck = "0")
		{
			var srv = new SerialNoService();
			var serialNoResult = srv.GetSerialItem(gupCode, custCode, barCode, isCombinCheck == "1");
			return new List<SerialNoResult> { serialNoResult }.AsQueryable();
		}

		[WebGet]
		public IQueryable<F1903Plus> GetItemInfo(string gupCode, string custCode, string itemCode)
		{
			var srv = new SharedService();
			return srv.GetItemInfo(gupCode, custCode, itemCode);
		}

		[WebGet]
		public IQueryable<Route> GetRoutes(string allId, string dcCode)
		{
			var f194705Rep = new F194705Repository(Schemas.CoreSchema);
			return f194705Rep.GetRoutes(allId, dcCode);
		}

		[WebGet]
		public ExecuteResult GetForGenClientEntites()
		{
			return new ExecuteResult();
		}
		[WebGet]
		public string GetConnectName(string key)
		{

			if (AesCryptor.Current.Decode(key) == "a1234567")
			{
				var providerName = ConfigurationManager.ConnectionStrings[Schemas.CoreSchema].ProviderName;
				string platform = string.Empty;
				platform = Schemas.CoreSchema.ToString().IndexOf("DEV") != -1 ? "測試機" : "正式機";
				return AesCryptor.Current.Encode(string.Format("{0}{1}", platform, Schemas.CoreSchema.Replace("WMS", "").ToString()));
			}
			else
				return "";
		}
		[WebGet]
		public ExecuteResult ParseAddress(string address)
		{
			var service = new SharedService();
			var consignService = new ConsignService();
			var query = new List<AddressParsedResult>() { new AddressParsedResult
			{
				ADDRESS = address,
				IsNeedParseAddress = true
			}};
			var isOk = false;
			// 最多嘗試三次
			for (int i = 1; i <= 3; i++)
			{
				// 透過郵局或Google來解析地址，並設定到每個傳入的 AddressParsedResult 
				Parallel.ForEach(query, service.ParseAddress);
				if (!string.IsNullOrWhiteSpace(query.First().ADDRESS_PARSE))
				{
					isOk = true;
					break;
				}
				// 如果說解析過，還沒全部解析完成，則先暫停0.5秒。若每次都有失敗，則最後一次不停
				if (i < 3)
					Thread.Sleep(500);
			}
			if (isOk)
			{
				var item = consignService.GetEgsSuda5List(new List<string> { address }).FirstOrDefault();
				if (item != null && item.status == "OK" && !string.IsNullOrEmpty(item.suda5_1))
					return new ExecuteResult(true, item.suda5_1);
			}
			return new ExecuteResult(false, "地址錯誤!");
		}
		[WebGet]
		public ExecuteResult GetSingleVolumeUnit(string gupCode, string custCode, string itemCode, int stockQty)
		{
			var srv = new SharedService();
			var result = new ExecuteResult
			{
				IsSuccessed = true,
				Message = srv.GetSingleVolumeUnit(gupCode, custCode, itemCode, stockQty)
			};
			return result;
		}
		[WebGet]
		public IQueryable<F000904> GetF000904List(string topic, string subtopic)
		{
			var srv = new SharedService();
			return srv.GetF000904List(topic, subtopic);
		}

	}
}
