using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.DataSevices
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class F05DataService : DataServiceBase<Wms3plDbContext>
	{
		// This method is called only once to initialize service-wide policies.
		public static void InitializeService(DataServiceConfiguration config)
        {
            DataServiceConfigHelper.SetEntitiesAccess(config, "F05");
            //config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

		#region 由F050301.ORD_NO取F050801出貨單資料
		[WebGet]
		public IQueryable<F050801> GetF050801ByOrderNo(string dcCode, string gupCode, string custCode, string ordNo)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			return f050801Repo.GetF050801ByOrderNo(dcCode, gupCode, custCode, ordNo);
		}
		#endregion

		#region P0806020000 合流作業 傳入合流箱號取商品播種數
		[WebGet]
		public IQueryable<F052902> GetF052902ItemByBoxId(string dcCode, string gupCode, string custCode, string itemCode, string boxIds, string delvDate)
		{
			var f052902Repo = new F052902Repository(Schemas.CoreSchema);
			var delv_Date = Convert.ToDateTime(delvDate);
			return f052902Repo.GetF052902ItemByBoxId(dcCode, gupCode, custCode, itemCode, boxIds, delv_Date);
		}
		#endregion

		#region 由F050801.WMS_ORD_NO出貨單取F050301訂單資料
		[WebGet]
		public IQueryable<F050301> GetDataByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var f050801Repo = new F050301Repository(Schemas.CoreSchema);
			return f050801Repo.GetDatasByWmsOrdNo(dcCode, gupCode, custCode, wmsOrdNo);
		}
		#endregion

		/// <summary>
		/// 取得訂單併單的所有訂單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="ordNo"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<F05030101> GetMergerOrders(string dcCode, string gupCode, string custCode, string ordNo)
		{
			var f05030101Repo = new F05030101Repository(Schemas.CoreSchema);
			return f05030101Repo.GetMergerOrders(dcCode, gupCode, custCode, ordNo);
		}

		[WebGet]
		public IQueryable<F051201> GetDatasByNoVirturlItem(string dcCode, string gupCode, string custCode, DateTime delvDate,
			string pickTime)
		{
			var f051201Repo = new F051201Repository(Schemas.CoreSchema);
			return f051201Repo.GetDatasByNoVirturlItem(dcCode, gupCode, custCode, delvDate, pickTime);
		}

		[WebGet]
		public IQueryable<F052901> GetDataByPickOrdNo(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			var f052901Repo = new F052901Repository(Schemas.CoreSchema);
			return f052901Repo.GetDataByPickOrdNo(dcCode, gupCode, custCode, pickOrdNo);
		}
	}
}
