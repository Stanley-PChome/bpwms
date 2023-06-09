using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web.Management;
using Wms3pl.Common.Enums;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P05.ExDataSources;
using Wms3pl.WebServices.Process.P05.Services;
using Wms3pl.WebServices.Process.P08.ExDataSources;
using Wms3pl.WebServices.Process.P08.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P08
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class P08ExDataService : DataService<P08ExDataSource>
	{
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("*", EntitySetRights.All);
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

	

		#region P080701 出貨包裝
		/// <summary>
		/// 取得最接近的出車時間
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<F0513PickTime> GetNearestPickTime(string dcCode, string gupCode, string custCode)
		{
			var repo = new F0513Repository(Schemas.CoreSchema);
			var result = repo.GetNearestPickTime(dcCode, gupCode, custCode);
			return result;
		}

		/// <summary>
		/// 取得最接近的出車時間
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		[WebGet]
		public string GetNearestTakeTime(string dcCode, string gupCode, string custCode)
		{
			var repo = new F700102Repository(Schemas.CoreSchema);
			var nearestTakeTime = repo.GetNearestTakeTime(dcCode, gupCode, custCode);
			return nearestTakeTime;
		}

		/// <summary>
		/// 取得出貨統計
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<DeliveryData> GetDeliveryData(string dcCode, string gupCode, string custCode, string wmsOrdNo, short packageBoxNo)
		{
			var f055002Repo = new F055002Repository(Schemas.CoreSchema);
			return f055002Repo.GetDeliveryData(dcCode, gupCode, custCode, wmsOrdNo, packageBoxNo);
		}

		/// <summary>
		/// 取得出貨統計
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<DeliveryData> GetQuantityOfDeliveryInfo(string dcCode, string gupCode, string custCode, string wmsOrdNo, short packageBoxNo)
		{
			var f055002Repo = new F055002Repository(Schemas.CoreSchema);
			return f055002Repo.GetQuantityOfDeliveryInfo(dcCode, gupCode, custCode, wmsOrdNo, itemCode: null, packageBoxNo: packageBoxNo);
		}

		//拖運單
		[WebGet]
		public IQueryable<F055001Data> GetConsignData(string dcCode, string gupCode, string custCode, string wmsOrdNo, string packageBoxNo)
		{
			var srv = new P080701Service();
			var result = srv.GetConsignData(dcCode, gupCode, custCode, wmsOrdNo, packageBoxNo);
			return result;
		}
		//拖運單商品明細
		[WebGet]
		public IQueryable<F055002Data> GetConsignItemData(string dcCode, string gupCode, string custCode, string wmsOrdNo, string pastNo)
		{
			var srv = new P080701Service();
			var result = srv.GetConsignItemData(dcCode, gupCode, custCode, wmsOrdNo, pastNo);
			return result;
		}
	

		[WebGet]
		public IQueryable<ExecuteResult> UpdateBoxStock(string dcCode, string gupCode, string custCode, string boxNum)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P080701Service(wmsTransaction);
			var result = srv.UpdateBoxStock(dcCode, gupCode, custCode, boxNum);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return (new List<ExecuteResult> { result }).AsQueryable();
		}

		#endregion

		#region P080901 出貨裝車
		[WebGet]
		public IQueryable<P080901ShipReport> GetF050801WithF700102sForReport(string dcCode, string gupCode, string custCode, DateTime takeDate, string takeTime, string allId)
		{
			//var wmsTransaction = new WmsTransaction();
			//var p080901Service = new P080901Service(wmsTransaction);
			//var result = p080901Service.SetLoStatusInProgressDeliver(dcCode, gupCode, custCode, takeDate, checkoutTime, allId);
			//if (result.IsSuccessed)
			//	wmsTransaction.Complete();

			var repo = new F700102Repository(Schemas.CoreSchema);
			var datas = repo.GetF050801WithF700102sForReport(dcCode, gupCode, custCode, takeDate, takeTime, allId);
			return datas;
		}
		[WebGet]
		public IQueryable<F050801WithF700102> GetF050801WithF700102s(string dcCode, string gupCode, string custCode, DateTime takeDate, string checkoutTime, string allId, string checkWmsStatus = "0")
		{
			//var wmsTransaction = new WmsTransaction();
			//var p080901Service = new P080901Service(wmsTransaction);
			//var result = p080901Service.SetLoStatusInProgressDeliver(dcCode, gupCode, custCode, takeDate, checkoutTime, allId);
			//if (result.IsSuccessed)
			//	wmsTransaction.Complete();

			var repo = new F700102Repository(Schemas.CoreSchema);
			var datas = repo.GetF050801WithF700102s(dcCode, gupCode, custCode, takeDate, checkoutTime, allId, checkWmsStatus == "1");
			return datas;
		}
		[WebGet]
		public IQueryable<ExecuteResult> UpdateCarNo(string dcCode, string gupCode, string custCode, string delvDate,
			string takeTime, string allId, string carNoA, string carNoB, string carNoC, string needSeal)
		{
			var takeDate = DateTime.Parse(delvDate);

			var wmsTransaction = new WmsTransaction();
			var p080901Service = new P080901Service(wmsTransaction);
			var result = p080901Service.UpdateCarNo(dcCode, gupCode, custCode, takeDate, takeTime, allId, carNoA, carNoB, carNoC, needSeal);
			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
				p080901Service.UpdateDistrCarStatus(dcCode, gupCode, custCode, takeDate, takeTime, allId);
				wmsTransaction.Complete();
			}

			return new List<ExecuteResult> { result }.AsQueryable();
		}
		[WebGet]
		public IQueryable<ExecuteResult> UpdateIsSeal(string dcCode, string gupCode, string custCode, string delvDate,
			string takeTime, string allId)
		{
			var takeDate = DateTime.Parse(delvDate);

			var wmsTransaction = new WmsTransaction();
			var p080901Service = new P080901Service(wmsTransaction);
			var result = p080901Service.UpdateIsSeal(dcCode, gupCode, custCode, takeDate, takeTime, allId);
			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
				p080901Service.UpdateDistrCarStatus(dcCode, gupCode, custCode, takeDate, takeTime, allId);
				wmsTransaction.Complete();
			}

			return new List<ExecuteResult> { result }.AsQueryable();
		}

		#endregion

		#region P080602 合流作業

		[WebGet]
		public IQueryable<ExecuteResult> DoChickIn(string dcCode, string gupCode, string custCode, string delvDate, string pickTime, string wmsOrdNo)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P080602Service(wmsTransaction);
			var result = srv.DoChickIn(dcCode, gupCode, custCode, delvDate, pickTime, wmsOrdNo);
			if (result.All(x => x.IsSuccessed))
				wmsTransaction.Complete();
			return result;
		}

		[WebGet]
		public IQueryable<F050802ItemName> GetShippingItem(string dcCode, string gupCode, string custCode, string itemCodes, string wmsOrderNos)
		{
			var f050802repo = new F050802Repository(Schemas.CoreSchema);
			var result = f050802repo.GetShippingItem(dcCode, gupCode, custCode, itemCodes, wmsOrderNos);
			return result;
		}
		#endregion

		#region P080201 退貨檢驗
		[WebGet]
		public IQueryable<F161402Data> GetF161202ReturnDetails(string dcCode, string gupCode, string custCode, string returnNo)
		{
			var f161202repo = new F161202Repository(Schemas.CoreSchema);
			var result = f161202repo.GetF161202ReturnDetails(dcCode, gupCode, custCode, returnNo);
			return result;
		}

		[WebGet]
		public IQueryable<F161402Data> GetF161402ReturnDetails(string dcCode, string gupCode, string custCode, string returnNo, string auditStaff, string auditName)
		{
			var f161402repo = new F161402Repository(Schemas.CoreSchema);
			var result = f161402repo.GetF161402ReturnDetails(dcCode, gupCode, custCode, returnNo, auditStaff, auditName);
			return result;
		}

		[WebGet]
		public IQueryable<F190206CheckItemName> GetCheckItems(string gupCode, string custCode, string itemCode, string checkType = "")
		{
			var f190206repo = new F190206Repository(Schemas.CoreSchema);
			var result = f190206repo.GetCheckItems(gupCode, custCode, itemCode, checkType);
			return result;
		}

		[WebGet]
		public IQueryable<F161202SelectedData> GetReturnItems(string dcCode, string gupCode, string custCode, string returnDateStart, string returnDateEnd,
			string returnNoStart, string returnNoEnd, string itemCode, string itemName)
		{
			var f161202repo = new F161202Repository(Schemas.CoreSchema);
			var result = f161202repo.GetReturnItems(dcCode, gupCode, custCode, returnDateStart, returnDateEnd, returnNoStart, returnNoEnd, itemCode, itemName);
			return result;
		}

		[WebGet]
		public ExecuteResult DeleteReturnItem(string dcCode, string gupCode, string custCode, string returnNo, string itemCode, string videoNo)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P080201Service(wmsTransaction);
			var result = srv.DeleteReturnItem(dcCode, gupCode, custCode, returnNo, itemCode, videoNo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[WebGet]
		public ExecuteResult DeleteReturnSerial(string dcCode, string gupCode, string custCode, string returnNo, string itemCode, string serialNo, int logSeq, string videoNo)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P080201Service(wmsTransaction);
			var result = srv.DeleteReturnSerial(dcCode, gupCode, custCode, returnNo, itemCode, serialNo, logSeq, videoNo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[WebGet]
		public ExecuteResult DoPosting(string dcCode, string gupCode, string custCode, string returnNo, string auditStaff, string auditName)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P080201Service(wmsTransaction);
			var result = srv.DoPosting(dcCode, gupCode, custCode, returnNo, auditStaff, auditName);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[WebGet]
		public ExecuteResult DoForceClose(string dcCode, string gupCode, string custCode, string returnNo, string auditStaff, string auditName)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P080201Service(wmsTransaction);
			var result = srv.DoPosting(dcCode, gupCode, custCode, returnNo, auditStaff, auditName, true);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[WebGet]
		public IQueryable<F16140101Data> GetSerialItems(string dcCode, string gupCode, string custCode, string returnNo)
		{
			var srv = new P080201Service();
			return srv.GetSerialItems(dcCode, gupCode, custCode, returnNo);
		}

		[WebGet]
		public IQueryable<F161501> GetGatherItems(string dcCode, string gatherDateStart, string gatherDateEnd, string gatherNoStart, string gatherNoEnd, string fileName)
		{
			var f161501repo = new F161501Repository(Schemas.CoreSchema);
			var result = f161501repo.GetGatherItems(dcCode, gatherDateStart, gatherDateEnd, gatherNoStart, gatherNoEnd, fileName);
			return result;
		}

		[WebGet]
		public IQueryable<ExecuteResult> DoDelGatherData(string dcCode, string gatherNos)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P080201Service(wmsTransaction);
			var result = srv.DoDelGatherData(dcCode, gatherNos);
			if (result == null) return null;
			wmsTransaction.Complete();
			return result;
		}


		#endregion

		#region P080301 同倉調撥下架  P080303 跨倉調撥下架

		[WebGet]
		public IQueryable<F151002Data> GetF151002Datas(string srcDcCode, string gupCode, string custCode, string allocationNo,
			string userId, string userName, string isAllowStatus2, string isDiffWareHouse)
		{
			//isAllowStatus2 1:允許Status=2 0:不允許Status = 2
			var p080301Service = new P080301Service();
			return p080301Service.GetF151002Datas(srcDcCode, gupCode, custCode, allocationNo, userId, userName, isAllowStatus2 == "1", isDiffWareHouse == "1");
		}
        [WebGet]
        public IQueryable<ExecuteResult> OutOfStockByP080301(string dcCode, string gupCode, string custCode, string allocationNo,
        string srcLocCode, string itemCode, string validDate, string serialNo, string makeNo, string boxCrtlNo, string palletNo)
        {
            var wmsTransaction = new WmsTransaction();
            var p080301Service = new P080301Service(wmsTransaction);
            var result = p080301Service.OutOfStock(dcCode, gupCode, custCode, allocationNo, srcLocCode, itemCode, validDate, serialNo, makeNo, boxCrtlNo, palletNo);
            if (result.IsSuccessed)
                wmsTransaction.Complete();

            return new List<ExecuteResult> { result }.AsQueryable();
        }
		[WebGet]
		public IQueryable<F151002ItemLocData> GetF151002ItemLocDatas(string dcCode, string gupCode, string custCode,
			string allocationNo, string itemCode, string isDiffWareHouse)
		{
			var p080301Service = new P080301Service();
			return p080301Service.GetF151002ItemLocDatas(dcCode, gupCode, custCode, allocationNo, itemCode, isDiffWareHouse == "1");
		}
		[WebGet]
		public IQueryable<ExecuteResult> ScanSrcLocItemCodeActualQty(string dcCode, string gupCode, string custCode,
			string allocationNo, string srcLocCode, string itemCode,
			string serialNo, string orginalValidDate, string newValidDate, int addActualQty, string scanCode,
            string orginalMakeNo, string newMakeNo, string palletCtrlNo, string boxCtrlNo)
		{
			var wmsTransaction = new WmsTransaction();
			var p080301Service = new P080301Service(wmsTransaction);
            var result = p080301Service.ScanSrcLocItemCodeActualQty(dcCode, gupCode, custCode, allocationNo, srcLocCode, itemCode, serialNo, orginalValidDate, newValidDate, addActualQty, scanCode,
                orginalMakeNo, newMakeNo, palletCtrlNo, boxCtrlNo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return new List<ExecuteResult> { result }.AsQueryable();
		}

		[WebGet]
		public IQueryable<ExecuteResult> RemoveSrcLocItemCodeActualQty(string dcCode, string gupCode, string custCode, string allocationNo, string srcLocCode, string itemCode,
			 string removeValidDate,string removeMakeNo)
		{
			var wmsTransaction = new WmsTransaction();
			var p080301Service = new P080301Service(wmsTransaction);
            var result = p080301Service.RemoveSrcLocItemCodeActualQty(dcCode, gupCode, custCode, allocationNo, srcLocCode, itemCode, removeValidDate, removeMakeNo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return new List<ExecuteResult> { result }.AsQueryable();
		}
		[WebGet]
		public IQueryable<ExecuteResult> StartDownOrUpItemChangeStatus(string dcCode, string gupCode, string custCode, string allocationNo, string isUp)
		{
			var wmsTransaction = new WmsTransaction();
			var p080301Service = new P080301Service(wmsTransaction);
			var result = p080301Service.StartDownOrUpItemChangeStatus(dcCode, gupCode, custCode, allocationNo, isUp == "1");
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return new List<ExecuteResult> { result }.AsQueryable();
		}
		[WebGet]
		public IQueryable<ExecuteResult> ChangeAllocationDownOrUpStatusToOrginal(string dcCode, string gupCode, string custCode,
			string allocationNo, string status, string isUp, bool lackType)
		{
			var wmsTransaction = new WmsTransaction();
			var p080301Service = new P080301Service(wmsTransaction);
			var result = p080301Service.ChangeAllocationDownOrUpStatusToOrginal(dcCode, gupCode, custCode, allocationNo, status, isUp == "1", lackType);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return new List<ExecuteResult> { result }.AsQueryable();
		}
		[WebGet]
		public IQueryable<ExecuteResult> ChangeAllocationDownOrUpFinish(string dcCode, string gupCode, string custCode,
			string allocationNo, string isUp)
		{
			var wmsTransaction = new WmsTransaction();
			var p080301Service = new P080301Service(wmsTransaction);
			var result = p080301Service.ChangeAllocationDownOrUpFinish(dcCode, gupCode, custCode, allocationNo, isUp == "1");
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return new List<ExecuteResult> { result }.AsQueryable();
		}

		[WebGet]
		public IQueryable<ExecuteResult> InsertOrUpdateF151004(string dcCode, string gupCode, string custCode,
			string allocationNo, string caseNo)
		{
			var wmsTransaction = new WmsTransaction();
			var p080301Service = new P080301Service(wmsTransaction);
			var result = p080301Service.InsertOrUpdateF151004(dcCode, gupCode, custCode, allocationNo, caseNo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return new List<ExecuteResult> { result }.AsQueryable();
		}
		#endregion

		#region P080302 同倉調撥上架 P080304 跨倉調撥上架

		[WebGet]
		public IQueryable<F151002DataByTar> GetF151002DataByTars(string tarDcCode, string gupCode, string custCode, string allocationNo,
	string userId, string userName, string isAllowStatus4, string isDiffWareHouse)
		{
			var p080302Service = new P080302Service();
			return p080302Service.GetF151002DataByTars(tarDcCode, gupCode, custCode, allocationNo, userId, userName, isAllowStatus4 == "1", isDiffWareHouse == "1");
		}
		[WebGet]
		public IQueryable<ExecuteResult> OutOfStockByP080302(string dcCode, string gupCode, string custCode, string allocationNo,
		string sugLocCode, string itemCode, string validDate, string serialNo, string makeNo, string boxCrtlNo, string palletNo)
		{
			var wmsTransaction = new WmsTransaction();
			var p080302Service = new P080302Service(wmsTransaction);
            var result = p080302Service.OutOfStockByTar(dcCode, gupCode, custCode, allocationNo, sugLocCode, itemCode, validDate, serialNo, makeNo, boxCrtlNo, palletNo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return new List<ExecuteResult> { result }.AsQueryable();
		}
		[WebGet]
		public IQueryable<F151002ItemLocDataByTar> GetF151002ItemLocDataByTars(string dcCode, string gupCode, string custCode,
			string allocationNo, string itemCode, string isDiffWareHouse)
		{
			var p080302Service = new P080302Service();
			return p080302Service.GetF151002ItemLocDataByTars(dcCode, gupCode, custCode, allocationNo, itemCode, isDiffWareHouse == "1");
		}
		[WebGet]
		public IQueryable<ExecuteResult> ScanTarLocItemCodeActualQty(string tarDcCode, string dcCode, string gupCode, string custCode,
			string allocationNo, string sugLocCode, string tarLocCode, string itemCode,
			string serialNo, string orginalValidDate, string newValidDate, int addActualQty, string userId, string wareHouseId, string scanCode,
            string orginalMakeNo, string newMakeNo, string palletCtrlNo, string boxCtrlNo)
		{
			var wmsTransaction = new WmsTransaction();
			var p080302Service = new P080302Service(wmsTransaction);
            var result = p080302Service.ScanTarLocItemCodeActualQty(tarDcCode, dcCode, gupCode, custCode, allocationNo, sugLocCode, tarLocCode, itemCode, serialNo, orginalValidDate, newValidDate, addActualQty, userId, wareHouseId, scanCode,
                orginalMakeNo, newMakeNo, palletCtrlNo, boxCtrlNo);
			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
				result = p080302Service.UpdateF191204(dcCode, gupCode, custCode, itemCode, allocationNo);
				if(result.IsSuccessed)
					wmsTransaction.Complete();
			}

			return new List<ExecuteResult> { result }.AsQueryable();
		}

		[WebGet]
		public IQueryable<ExecuteResult> RemoveTarLocItemCodeActualQty(string dcCode, string gupCode, string custCode, string allocationNo, string tarLocCode, string itemCode,
             string removeMakeNo ,string removeValidDate)
		{
			var wmsTransaction = new WmsTransaction();
			var p080302Service = new P080302Service(wmsTransaction);
            var result = p080302Service.RemoveTarLocItemCodeActualQty(dcCode, gupCode, custCode, allocationNo, tarLocCode, itemCode, removeValidDate, removeMakeNo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return new List<ExecuteResult> { result }.AsQueryable();
		}
		[WebGet]
		public IQueryable<ExecuteResult> CheckTarLocCode(string dcCode, string wareHouseId, string locCode, string userId, string itemCode)
		{
			var p080302Service = new P080302Service();
			var result = p080302Service.CheckTarLocCode(dcCode, wareHouseId, locCode, userId, itemCode);
			return new List<ExecuteResult> { result }.AsQueryable();
		}


		#endregion

		#region 裝車稽核

		[WebGet]
		public IQueryable<ExecuteResult> SetWmsOrdAudited(string wmsOrdNo, string gupCode, string custCode, string dcCode, string pastNo)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P080802Service(wmsTransaction);
			var result = srv.SetWmsOrdAudited(wmsOrdNo, gupCode, custCode, dcCode, pastNo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return (new List<ExecuteResult> { result }).AsQueryable();
		}
		#endregion 裝車稽核

		#region LO 相關

		//[WebGet]
		//public IQueryable<ExecuteResult> AddLoFinishQtyScan(string gupCode, string custCode, string ticketNo, string itemCode, string serialNo, int addQty)
		//{
		//	var wmsTransaction = new WmsTransaction();
		//	var srv = new P080701Service(wmsTransaction);
		//	var f1903Repo = new F1903Repository(Schemas.CoreSchema);
		//	var f1903 = f1903Repo.Find(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.ITEM_CODE == itemCode);
		//	var result = srv.AddLoFinishQtyScan(ticketNo, itemCode, serialNo, f1903, addQty);
		//	if (result == null)
		//		result = new ExecuteResult { IsSuccessed = true };
		//	if (result.IsSuccessed)
		//		wmsTransaction.Complete();

		//	return (new List<ExecuteResult> { result }).AsQueryable();
		//}

		/// <summary>
		/// 更新包裝(P)的狀態為完成，並依判斷產生稽核(A)的 LO_MAIN
		/// </summary>
		/// <param name="wmsOrdNo"></param>
		/// <returns></returns>
		//[WebGet]
		//public IQueryable<ExecuteResult> SetLoStatusFinishPack(string wmsOrdNo, string gupCode, string custCode, string dcCode)
		//{
		//	var wmsTransaction = new WmsTransaction();
		//	var srv = new P080701Service(wmsTransaction);
		//	var result = srv.SetLoStatusFinishPack(wmsOrdNo);
		//	if (result == null)
		//	{
		//		result = srv.CreateLoMainAudit(wmsOrdNo, gupCode, custCode, dcCode);
		//		if (result == null)
		//			result = new ExecuteResult { IsSuccessed = true };
		//	}
		//	if (result.IsSuccessed)
		//		wmsTransaction.Complete();

		//	return (new List<ExecuteResult> { result }).AsQueryable();
		//}
		#endregion

		/// <summary>
		/// 出貨單單是否含有序號商品
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <returns></returns>
		[WebGet]
		public bool HasSerialItem(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var f050802Repo = new F050802Repository(Schemas.CoreSchema);
			return f050802Repo.HasSerialItem(dcCode, gupCode, custCode, wmsOrdNo);
		}

		#region 行動盤點
		[WebGet]
		public IQueryable<InventoryScanLoc> GetInventoryScanLoc(string dcCode, string gupCode, string custCode, string inventoryNo,
			string locCode)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P081001Service(wmsTransation);
			var result = service.GetInventoryScanLoc(dcCode, gupCode, custCode, inventoryNo, locCode);
			if (result.IsSuccess)
				wmsTransation.Complete();
			return new List<InventoryScanLoc> { result }.AsQueryable();
		}
		[WebGet]
		public IQueryable<InventoryScanItem> GetInventoryScanItem(string dcCode, string gupCode, string custCode, string inventoryNo,
			string locCode, string itemCodeOrSerialNo)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P081001Service(wmsTransation);
			var result = service.GetInventoryScanItem(dcCode, gupCode, custCode, inventoryNo, locCode, itemCodeOrSerialNo);
			if (result.IsSuccess)
				wmsTransation.Complete();
			return new List<InventoryScanItem> { result }.AsQueryable();
		}

		[WebGet]
		public IQueryable<InventoryItemQty> UpdateToGetInventoryItemQty(string dcCode, string gupCode, string custCode, string inventoryNo,
			string locCode, string itemCode, int qty)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P081001Service(wmsTransation);
			var result = service.UpdateToGetInventoryItemQty(dcCode, gupCode, custCode, inventoryNo, locCode, itemCode, qty);
			if (result.IsSuccess)
				wmsTransation.Complete();
			return new List<InventoryItemQty> { result }.AsQueryable();
		}

		[WebGet]
		public IQueryable<ExecuteResult> ClearInventoryItemQty(string dcCode, string gupCode, string custCode, string inventoryNo,
			string locCode, string itemCode)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P081001Service(wmsTransation);
			var result = service.ClearInventoryItemQty(dcCode, gupCode, custCode, inventoryNo, locCode, itemCode);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return new List<ExecuteResult> { result }.AsQueryable();
		}
		[WebGet]
		public IQueryable<ExecuteResult> UpdateToF140104OrF140105(string dcCode, string gupCode, string custCode, string inventoryNo, string locCode, string clientName)
		{
			var wmsTransation = new WmsTransaction();
			var service = new P081001Service(wmsTransation);
			var result = service.UpdateToF140104OrF140105(dcCode, gupCode, custCode, inventoryNo, locCode, clientName);
			if (result.IsSuccessed)
				wmsTransation.Complete();
			return new List<ExecuteResult> { result }.AsQueryable();
		}
		#endregion


		/// <summary>
		/// 取得該批次日期的包裝~出貨的批次時段的分隔文字
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="delvDate"></param>
		/// <returns></returns>
		[WebGet]
		public string GetPickTimeSeparator(string dcCode, string gupCode, string custCode, DateTime delvDate)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			return string.Join(",", f050801Repo.GetPickTimeList(dcCode, gupCode, custCode, delvDate));
		}
		[WebGet]
		public IQueryable<ExecuteResult> ClearSerialByBoxOrCaseNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var wmsTransaction = new WmsTransaction();
			var serialNoSerivce = new SerialNoService(wmsTransaction);
			serialNoSerivce.ClearSerialByBoxOrCaseNo(dcCode, gupCode, custCode, wmsOrdNo, "O");
			wmsTransaction.Complete();
			return new List<ExecuteResult> { new ExecuteResult { IsSuccessed = true, Message = "" } }.AsQueryable();
		}


		[WebGet]
		public IQueryable<SerialNoResult> GetSerialItem(string gupCode, string custCode, string barCode)
		{
			var serialNoService = new SerialNoService();
			return new List<SerialNoResult> { serialNoService.GetSerialItem(gupCode, custCode, barCode) }.AsQueryable();
		}

		[WebGet]
		public IQueryable<ExecuteResult> CheckBarCode(string gupCode, string custCode, string itemCode, string barCode)
		{
			var serialNoService = new SerialNoService();
			return new List<ExecuteResult> { serialNoService.CheckBarCode(gupCode, custCode, itemCode, barCode, false) }.AsQueryable();
		}

		/// <summary>
		/// 是否該出貨單存在於待處理的派車單明細
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <returns></returns>
		[WebGet]
		public bool ExistsF700102ByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo, string sourceNo)
		{
			var repo = new F700102Repository(Schemas.CoreSchema);
			return repo.ExistsF700102ByWmsOrdNo(dcCode, gupCode, custCode, wmsOrdNo, sourceNo);
		}

		[WebGet]
		public IQueryable<InventoryLocItem> GetInventoryLocItems(string dcCode, string gupCode, string custCode,
			string inventoryNo)
		{
			var p081001Service = new P081001Service();
			return p081001Service.GetInventoryLocItems(dcCode, gupCode, custCode, inventoryNo);
		}

		

		/// <summary>
		/// 箱明細
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <param name="packageBoxNo"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<DeliveryReport> GetDeliveryReport(string dcCode, string gupCode, string custCode, string wmsOrdNo, short? packageBoxNo)
		{
			var service = new P080701Service();
			return service.GetDeliveryReport(dcCode, gupCode, custCode, wmsOrdNo, packageBoxNo);
		}
		/// <summary>
		/// 箱明細列印資訊
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<DelvdtlInfo> GetDelvdtlInfo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var service = new P080701Service();
			return service.GetDelvdtlInfo(dcCode, gupCode, custCode, wmsOrdNo);
		}
		[WebGet]
		public IQueryable<ReturnDetailSummary> GetReturnDetailSummary(string dcCode, string gupCode, string custCode, string returnNo)
		{
			var f16140201Repo = new F16140201Repository(Schemas.CoreSchema);
			return f16140201Repo.GetReturnDetailSummary(dcCode, gupCode, custCode, returnNo);
		}

		[WebGet]
		public List<F190905> GetF190905(string dcCode, string gupCode, string custCode, string wmsOrdNo, string allId)
		{
			var service = new P080701Service();
			return new List<F190905> { service.GetF190905(dcCode, gupCode, custCode, wmsOrdNo, allId) };
		}

		[WebGet]
		public ExecuteResult DoRePacking(string dcCode, string gupCode, string custCode, string wmsOrdCode)
		{
			var wmsTransaction = new WmsTransaction();
			var f050304Repo = new F050304Repository(Schemas.CoreSchema);
			//wmsOrdCode 新的出貨單號
			var item = f050304Repo.GetF050304ExData(dcCode, gupCode, custCode, wmsOrdCode);
			ExecuteResult result = new ExecuteResult { };

			if (item != null)
			{
				ExecuteResult resultFor711 = new ExecuteResult { };
				var consignService = new ConsignService(wmsTransaction);
				var tmpF050801 = new F050801()
				{
					GUP_CODE = gupCode,
					DC_CODE = dcCode,
					CUST_CODE = custCode,
					WMS_ORD_NO = wmsOrdCode
				};

				result = consignService.UpdateConsign(tmpF050801);
				if (!result.IsSuccessed)
				{
					return result;
				}

			}
			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
			}
			return result;
		}

		#region P0806130000 單據工號綁定

		/// <summary>
		/// 刷讀工號，傳回單據綁定資料
		/// </summary>
		/// <param name="dcCode">物流中心</param>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="empID">工號</param>
		/// <param name="status">狀態</param>
		/// <returns></returns>
		[WebGet]
		public IEnumerable<F0011BindData> GetP0806130000Detail(string dcCode, string gupCode, string custCode, string empID, string status)
		{
			var f0011Repo = new F0011Repository(Schemas.CoreSchema);
			return f0011Repo.GetF0011List(dcCode, gupCode, custCode, empID, status);
		}

		/// <summary>
		/// 查詢單據綁定資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="empID"></param>
		/// <param name="orderNo"></param>
		/// <returns></returns>
		[WebGet]
		public IEnumerable<F0011BindData> GetP0806130000SearchData(string dcCode, string gupCode, string custCode, string empID, string orderNo, DateTime crtDate)
		{
			var f0011Repo = new F0011Repository(Schemas.CoreSchema);
			return f0011Repo.GetF0011ListSearchData(dcCode, gupCode, custCode, empID, orderNo, crtDate);
		}

		#endregion

		#region P081301 儲位商品搬移
		[WebGet]
		public IQueryable<P081301StockSumQty> GetP081301StockSumQties(string dcCode, string gupCode, string custCode, string scanItemOrLocCode)
		{
			var srv = new P081301Service();
			return srv.GetP081301StockSumQties(dcCode, gupCode, custCode, scanItemOrLocCode);
		}
		[WebGet]
		public IQueryable<P08130101MoveLoc> GetP08130101MoveLocs(string dcCode, string locCode)
		{
			var srv = new P081301Service();
			return srv.GetP08130101MoveLocs(dcCode, locCode);
		}
		[WebGet]
		public IQueryable<P08130101Stock> GetP08130101Stocks(string dcCode, string gupCode, string custCode, string locCode,string itemCode)
		{
			var srv = new P081301Service();
			return srv.GetP08130101Stocks(dcCode, gupCode,custCode,locCode, itemCode);
		}
        #endregion

        [WebGet]
        public IQueryable<PcHomeDeliveryReport> GetPcHomeDelivery(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P080701Service(wmsTransaction);
            return srv.GetPcHomeDelivery(dcCode, gupCode, custCode, wmsOrdNo);
        }

	

		#region 列印小白單
		[WebGet]
		public IQueryable<LittleWhiteReport> GetLittleWhiteReport(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080701Service(wmsTransaction);
			return service.GetLittleWhiteReport(dcCode, gupCode, custCode, wmsOrdNo);
		}
		#endregion

		#region 稽核出庫-箱內明細
		[WebGet]
		public IQueryable<P0808040100_BoxData> GetBoxData(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string sowType, string status)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080804Service(wmsTransaction);
			return service.GetBoxData(dcCode, gupCode, custCode, delvDate, pickTime, sowType, status);
		}

		[WebGet]
		public IQueryable<P0808040100_BoxDetailData> GetBoxDetailData(Int32 refId)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080804Service(wmsTransaction);
			return service.GetBoxDetailData(refId);
		}

		[WebGet]
		public IQueryable<P0808040100_PrintData> GetPrintBoxData(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string moveOutTarget, string containerCode, string sowType)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080804Service(wmsTransaction);
			return service.GetPrintBoxData(dcCode, gupCode, custCode, delvDate, pickTime, moveOutTarget, containerCode, sowType);
		}
		#endregion

		[WebGet]
		public IQueryable<BatchPickData> GetBatchPickData(string dcCode,string gupCode,string custCode,string containerBarcode)
		{
			var _wmsTransaction = new WmsTransaction();
			var srv = new P080804Service(_wmsTransaction);
			var result = srv.GetBatchPickData(dcCode, gupCode, custCode, containerBarcode);
			if (result != null)
				_wmsTransaction.Complete();
			return result;
		}

        #region P0814010000 單人包裝站
        /// <summary>
        /// 查詢出貨商品包裝明細
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsOrdNo"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<SearchWmsOrderPackingDetailRes> SearchWmsOrderPackingDetail(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var srv = new ShipPackageService();
            return srv.SearchWmsOrderPackingDetail(new SearchWmsOrderPackingDetailReq
            {
                DcCode = dcCode,
                GupCode = gupCode,
                CustCode = custCode,
                WmsOrdNo = wmsOrdNo
            }).AsQueryable();
        }

        /// <summary>
        /// 查詢出貨單刷讀紀錄
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="wmsOrdNo"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<SearchWmsOrderScanLogRes> SearchWmsOrderScanLog(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var srv = new ShipPackageService();
            return srv.SearchWmsOrderScanLog(new SearchWmsOrderScanLogReq
            {
                DcCode = dcCode,
                GupCode = gupCode,
                CustCode = custCode,
                WmsOrdNo = wmsOrdNo
            }).AsQueryable();
        }
		#endregion

		
	}
}
