using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P06.ExDataSources;
using Wms3pl.WebServices.Process.P06.Services;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P06
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class P06ExDataService : DataService<P06ExDataSource>
	{
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("*", EntitySetRights.All);
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

		/// <summary>
		/// 取得虛擬儲位查詢結果
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="delvDate">批次日期</param>
		/// <param name="pickTime">批次時段</param>
		/// <param name="custOrdNo">貨主單號</param>
		/// <param name="ordNo">訂單編號</param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<F05030101Ex> GetP060103Data(string gupCode, string custCode, string dcCode, string delvDate, string pickTime, string custOrdNo, string ordNo, string itemCode)
		{
			DateTime date = DateTime.Parse(delvDate);

			var srv = new P060103Service();
			return srv.GetP060103Data(gupCode, custCode, dcCode, date, pickTime, custOrdNo, ordNo, itemCode);
		}

		#region F051206PickList - 缺貨作業-揀貨列表
		
		[WebGet]
		public IQueryable<F051206Pick> GetGetF051206PicksByQuery(string dcCode, string gupCode, string custCode, DateTime delvDateStart, DateTime delvDateEnd, string status, string pickOrdNo, string wmsOrdNo, string containerCode, string crtOrUpdOpertor)
		{
			var repo = new F051206Repository(Schemas.CoreSchema);
			return repo.GetGetF051206PicksByQuery(dcCode, gupCode, custCode, delvDateStart, delvDateEnd, status, pickOrdNo, wmsOrdNo, containerCode, crtOrUpdOpertor);
		}

		[WebGet]
		public IQueryable<F051206Pick> GetGetF051206PicksByAdd(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string pickOrdNo)
		{
			var repo = new F051206Repository(Schemas.CoreSchema);
			return repo.GetGetF051206PicksByAdd(dcCode, gupCode, custCode, delvDate, pickTime, pickOrdNo);
		}
		#endregion

		#region F051206PickList - 缺貨作業-調撥列表
		[WebGet]
		public IQueryable<F051206AllocationList> GetF051206AllocationLists(string dcCode, string gupCode, string custCode, string editType, string status, string allocation_no)
		{
			var repo = new F051206Repository(Schemas.CoreSchema);
			return repo.GetF051206AllocationLists(dcCode, gupCode, custCode, editType, status, allocation_no).AsQueryable();
		}
		#endregion

		#region F051206LackList - 缺貨作業-缺貨明細
		[WebGet]
		//public IQueryable<F051206LackList> GetF051206LackLists(string PICK_ORD_NO, string WMS_ORD_NO, string editType)
		//{
		//  var repo = new F051206Repository(Schemas.CoreSchema);
		//  return repo.GetF051206LackLists(PICK_ORD_NO, WMS_ORD_NO, editType).AsQueryable();
		//}
		public IQueryable<F051206LackList> GetF051206LackLists(string dcCode, string gupCode, string custCode, string pickOrdNo, string wmsOrdNo, string editType)
		{
			var repo = new F051206Repository(Schemas.CoreSchema);
			return repo.GetF051206LackLists(dcCode,gupCode,custCode,pickOrdNo,wmsOrdNo,editType).AsQueryable();
		}
		[WebGet]
		public IQueryable<F051206LackList_Allot> GetF051206LackLists_Allot(string dcCode, string gupCode, string custCode
			, string allocationNo, string editType , string status)
		{
			var repo = new F051206Repository(Schemas.CoreSchema);
			return repo.GetF051206LackLists_Allot(dcCode, gupCode, custCode, allocationNo, editType, status).AsQueryable();
		}
		#endregion


		/// <summary>
		/// 取得單據類型的唯一單號
		/// </summary>
		/// <param name="ordType">單據類型</param>
		/// <param name="dcCode"></param>
		/// <returns></returns>
		[WebGet]
		public string GetNewOrdCode(string ordType)
		{
			var wmsTransaction = new WmsTransaction();
			var sharedService = new SharedService(wmsTransaction);
			var newOrdCode = sharedService.GetNewOrdCode(ordType);

			wmsTransaction.Complete();
			return newOrdCode;
		}

	

		/// <summary>
		/// 依照批次日期,時段,貨主單號取得出貨單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="delvDate"></param>
		/// <param name="pickTime"></param>
		/// <param name="custOrdNo"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<F050801WmsOrdNo> GetF050801ByDelvPickTime(string dcCode, string gupCode, string custCode, string delvDate, string pickTime, string custOrdNo)
		{
			DateTime delvDateTime;
			DateTime.TryParse(delvDate, out delvDateTime);

			var srv = new P060101Service();
			return srv.GetF050801ByDelvPickTime(dcCode, gupCode, custCode, delvDateTime, pickTime, custOrdNo);
		}

		/// <summary>
		/// 修改缺貨數
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="lackSeq"></param>
		/// <param name="pickOrdNo"></param>
		/// <param name="pickOrdSeq"></param>
		/// <param name="lackQty"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<ExecuteResult> ModifyLackQty(string dcCode, string gupCode, string custCode, int lackSeq, string pickOrdNo, string pickOrdSeq, int lackQty)
		{
			var wmsTransaction = new WmsTransaction();
			var p060201Service = new P060201Service(wmsTransaction);
			var res = p060201Service.ModifyLackQty(dcCode, gupCode, custCode, lackSeq, pickOrdNo, pickOrdSeq, lackQty);
			if (res.IsSuccessed)
			{
				wmsTransaction.Complete();
			}

			return new List<ExecuteResult> { res }.AsQueryable();
		}
	}
}
