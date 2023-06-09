using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.ExDataServices.ShareExDataService
{
	public partial class ShareExDataSource : global::System.Data.Services.Client.DataServiceContext
	{

		public IQueryable<String> GetNewOrdCode(String ordType)
		{
			return CreateQuery<String>("GetNewOrdCode")
						.AddQueryExOption("ordType", ordType);
		}

		public IQueryable<F050801WmsOrdNo> GetF050801ListBySourceNo(String dcCode, String gupCode, String custCode, String sourceNo)
		{
			return CreateQuery<F050801WmsOrdNo>("GetF050801ListBySourceNo")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("sourceNo", sourceNo);
		}

		public IQueryable<SerialNoResult> CheckSerialNoFull(String dcCode, String gupCode, String custCode, String itemCode, String serialNo, String status, String ignoreCheckOfStatus, String isCombinCheck)
		{
			return CreateQuery<SerialNoResult>("CheckSerialNoFull")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("serialNo", serialNo)
						.AddQueryExOption("status", status)
						.AddQueryExOption("ignoreCheckOfStatus", ignoreCheckOfStatus)
						.AddQueryExOption("isCombinCheck", isCombinCheck);
		}

		public IQueryable<SerialNoResult> GetSerialItem(String dcCode, String gupCode, String custCode, String barCode, String isCombinCheck)
		{
			return CreateQuery<SerialNoResult>("GetSerialItem")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("barCode", barCode)
						.AddQueryExOption("isCombinCheck", isCombinCheck);
		}

		public IQueryable<F1903Plus> GetItemInfo(String gupCode, String custCode, String itemCode)
		{
			return CreateQuery<F1903Plus>("GetItemInfo")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("itemCode", itemCode);
		}

		public IQueryable<Route> GetRoutes(String allId, String dcCode)
		{
			return CreateQuery<Route>("GetRoutes")
						.AddQueryExOption("allId", allId)
						.AddQueryExOption("dcCode", dcCode);
		}

		public ExecuteResult GetForGenClientEntites()
		{
			return CreateQuery<ExecuteResult>("GetForGenClientEntites")
						.ToList().FirstOrDefault();
		}
	}
}

