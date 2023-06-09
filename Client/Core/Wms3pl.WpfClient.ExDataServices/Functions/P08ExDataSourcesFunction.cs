using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.ExDataServices.P08ExDataService
{
	public partial class P08ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
        public IQueryable<PcHomeDeliveryReport> GetPcHomeDelivery(String dcCode, String gupCode, String custCode, String wmsOrdNo)
        {
            return CreateQuery<PcHomeDeliveryReport>("GetPcHomeDelivery")
                        .AddQueryExOption("dcCode", dcCode)
                        .AddQueryExOption("gupCode", gupCode)
                        .AddQueryExOption("custCode", custCode)
                        .AddQueryExOption("wmsOrdNo", wmsOrdNo);
        }


        public IQueryable<DeliveryReport> GetDeliveryReport(String dcCode, String gupCode, String custCode, String wmsOrdNo, Int16? packageBoxNo)
		{
			return CreateQuery<DeliveryReport>("GetDeliveryReport")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("wmsOrdNo", wmsOrdNo)
						.AddQueryExOption("packageBoxNo", packageBoxNo);
		}

		public IQueryable<DelvdtlInfo> GetDelvdtlInfo(String dcCode, String gupCode, String custCode, String wmsOrdNo)
		{
			return CreateQuery<DelvdtlInfo>("GetDelvdtlInfo")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("wmsOrdNo", wmsOrdNo);
		}

		public IQueryable<ReturnDetailSummary> GetReturnDetailSummary(String dcCode, String gupCode, String custCode, String returnNo)
		{
			return CreateQuery<ReturnDetailSummary>("GetReturnDetailSummary")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("returnNo", returnNo);
		}

		public IQueryable<F051201SelectedData> GetF051201SelectedDatas(String dcCode, String gupCode, String custCode, String delvDate, String pickTime, String isPrinted, String isDevicePrint)
		{
			return CreateQuery<F051201SelectedData>("GetF051201SelectedDatas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("pickTime", pickTime)
						.AddQueryExOption("isPrinted", isPrinted)
						.AddQueryExOption("isDevicePrint", isDevicePrint);
		}

		


		public IQueryable<F0513PickTime> GetNearestPickTime(String dcCode, String gupCode, String custCode)
		{
			return CreateQuery<F0513PickTime>("GetNearestPickTime")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode);
		}

		public String GetNearestTakeTime(String dcCode, String gupCode, String custCode)
		{
			return CreateQuery<String>("GetNearestTakeTime")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode).ToList().FirstOrDefault();
		}

		public IQueryable<DeliveryData> GetDeliveryData(String dcCode, String gupCode, String custCode, String wmsOrdNo, Int16? packageBoxNo)
		{
			return CreateQuery<DeliveryData>("GetDeliveryData")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("wmsOrdNo", wmsOrdNo)
						.AddQueryExOption("packageBoxNo", packageBoxNo);
		}

		public IQueryable<DeliveryData> GetQuantityOfDeliveryInfo(String dcCode, String gupCode, String custCode, String wmsOrdNo, Int16? packageBoxNo)
		{
			return CreateQuery<DeliveryData>("GetQuantityOfDeliveryInfo")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("wmsOrdNo", wmsOrdNo)
						.AddQueryExOption("packageBoxNo", packageBoxNo);
		}

		public IQueryable<F055001Data> GetConsignData(String dcCode, String gupCode, String custCode, String wmsOrdNo, String packageBoxNo)
		{
			return CreateQuery<F055001Data>("GetConsignData")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("wmsOrdNo", wmsOrdNo)
						.AddQueryExOption("packageBoxNo", packageBoxNo);
		}

		

		public IQueryable<ExecuteResult> UpdateBoxStock(String dcCode, String gupCode, String custCode, String boxNum)
		{
			return CreateQuery<ExecuteResult>("UpdateBoxStock")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("boxNum", boxNum);
		}

		public IQueryable<F050801WithF700102> GetF050801WithF700102sForReport(String dcCode, String gupCode, String custCode, DateTime? takeDate, String checkoutTime, String allId, String checkWmsStatus)
		{
			return CreateQuery<F050801WithF700102>("GetF050801WithF700102sForReport")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("takeDate", takeDate)
						.AddQueryExOption("checkoutTime", checkoutTime)
						.AddQueryExOption("allId", allId)
						.AddQueryExOption("checkWmsStatus", checkWmsStatus);
		}

		public IQueryable<F050801WithF700102> GetF050801WithF700102s(String dcCode, String gupCode, String custCode, DateTime? takeDate, String checkoutTime, String allId, String checkWmsStatus)
		{
			return CreateQuery<F050801WithF700102>("GetF050801WithF700102s")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("takeDate", takeDate)
						.AddQueryExOption("checkoutTime", checkoutTime)
						.AddQueryExOption("allId", allId)
						.AddQueryExOption("checkWmsStatus", checkWmsStatus);
		}

		public IQueryable<ExecuteResult> UpdateCarNo(String dcCode, String gupCode, String custCode, String delvDate, String takeTime, String allId, String carNoA, String carNoB, String carNoC, String needSeal)
		{
			return CreateQuery<ExecuteResult>("UpdateCarNo")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("takeTime", takeTime)
						.AddQueryExOption("allId", allId)
						.AddQueryExOption("carNoA", carNoA)
						.AddQueryExOption("carNoB", carNoB)
						.AddQueryExOption("carNoC", carNoC)
						.AddQueryExOption("needSeal", needSeal);
		}

		public IQueryable<ExecuteResult> UpdateIsSeal(String dcCode, String gupCode, String custCode, String delvDate, String takeTime, String allId)
		{
			return CreateQuery<ExecuteResult>("UpdateIsSeal")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("takeTime", takeTime)
						.AddQueryExOption("allId", allId);
		}

		public IQueryable<ExecuteResult> ScanItemCode(String dcCode, String gupCode, String custCode, String pickOrdNo, String pickLoc, String itemCode, String validDate, String serialNo, Int32? addQty, String scanCode, String isBatchAll)
		{
			return CreateQuery<ExecuteResult>("ScanItemCode")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("pickOrdNo", pickOrdNo)
						.AddQueryExOption("pickLoc", pickLoc)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("validDate", validDate)
						.AddQueryExOption("serialNo", serialNo)
						.AddQueryExOption("addQty", addQty)
						.AddQueryExOption("scanCode", scanCode)
						.AddQueryExOption("isBatchAll", isBatchAll);
		}

		public IQueryable<ExecuteResult> OutOfStockCheck(String dcCode, String gupCode, String custCode, String pickOrdNo, String pickLoc, String itemCode, String validDate, String serialNo, Int32? outofStockQty, String userId)
		{
			return CreateQuery<ExecuteResult>("OutOfStockCheck")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("pickOrdNo", pickOrdNo)
						.AddQueryExOption("pickLoc", pickLoc)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("validDate", validDate)
						.AddQueryExOption("serialNo", serialNo)
						.AddQueryExOption("outofStockQty", outofStockQty)
						.AddQueryExOption("userId", userId);
		}

		public IQueryable<ExecuteResult> OutOfStock(String dcCode, String gupCode, String custCode, String pickOrdNo, String pickLoc, String itemCode, String validDate, String serialNo, Int32? outofStockQty, String userId)
		{
			return CreateQuery<ExecuteResult>("OutOfStock")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("pickOrdNo", pickOrdNo)
						.AddQueryExOption("pickLoc", pickLoc)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("validDate", validDate)
						.AddQueryExOption("serialNo", serialNo)
						.AddQueryExOption("outofStockQty", outofStockQty)
						.AddQueryExOption("userId", userId);
		}

		public IQueryable<ExecuteResult> UpdatePickStaffEmpty(String userId, String userName)
		{
			return CreateQuery<ExecuteResult>("UpdatePickStaffEmpty")
						.AddQueryExOption("userId", userId)
						.AddQueryExOption("userName", userName);
		}

		public IQueryable<ExecuteResult> DoChickIn(String dcCode, String gupCode, String custCode, String delvDate, String pickTime, String wmsOrdNo)
		{
			return CreateQuery<ExecuteResult>("DoChickIn")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate)
						.AddQueryExOption("pickTime", pickTime)
						.AddQueryExOption("wmsOrdNo", wmsOrdNo);
		}

		public IQueryable<F050802ItemName> GetShippingItem(String dcCode, String gupCode, String custCode, String itemCodes, String wmsOrderNos)
		{
			return CreateQuery<F050802ItemName>("GetShippingItem")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("itemCodes", itemCodes)
						.AddQueryExOption("wmsOrderNos", wmsOrderNos);
		}

		public IQueryable<ExecuteResult> AddReturnCheck(String dcCode, String consignee, String receiptDate, String transport, String carNo, String returnNo, String pastNo, String barCode)
		{
			return CreateQuery<ExecuteResult>("AddReturnCheck")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("consignee", consignee)
						.AddQueryExOption("receiptDate", receiptDate)
						.AddQueryExOption("transport", transport)
						.AddQueryExOption("carNo", carNo)
						.AddQueryExOption("returnNo", returnNo)
						.AddQueryExOption("pastNo", pastNo)
						.AddQueryExOption("barCode", barCode);
		}

		public IQueryable<F161402Data> GetF161202ReturnDetails(String dcCode, String gupCode, String custCode, String returnNo)
		{
			return CreateQuery<F161402Data>("GetF161202ReturnDetails")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("returnNo", returnNo);
		}

		public IQueryable<F161402Data> GetF161402ReturnDetails(String dcCode, String gupCode, String custCode, String returnNo, String auditStaff, String auditName)
		{
			return CreateQuery<F161402Data>("GetF161402ReturnDetails")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("returnNo", returnNo)
						.AddQueryExOption("auditStaff", auditStaff)
						.AddQueryExOption("auditName", auditName);
		}

		public IQueryable<F190206CheckItemName> GetCheckItems(String gupCode, String custCode, String itemCode, String checkType)
		{
			return CreateQuery<F190206CheckItemName>("GetCheckItems")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("checkType", checkType);
		}

		public IQueryable<F161202SelectedData> GetReturnItems(String dcCode, String gupCode, String custCode, String returnDateStart, String returnDateEnd, String returnNoStart, String returnNoEnd, String itemCode, String itemName)
		{
			return CreateQuery<F161202SelectedData>("GetReturnItems")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("returnDateStart", returnDateStart)
						.AddQueryExOption("returnDateEnd", returnDateEnd)
						.AddQueryExOption("returnNoStart", returnNoStart)
						.AddQueryExOption("returnNoEnd", returnNoEnd)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("itemName", itemName);
		}

		public ExecuteResult DeleteReturnItem(String dcCode, String gupCode, String custCode, String returnNo, String itemCode, String videoNo)
		{
			return CreateQuery<ExecuteResult>("DeleteReturnItem")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("returnNo", returnNo)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("videoNo", videoNo).ToList().FirstOrDefault();
		}

		public ExecuteResult DeleteReturnSerial(String dcCode, String gupCode, String custCode, String returnNo, String itemCode, String serialNo, Int32? logSeq, String videoNo)
		{
			return CreateQuery<ExecuteResult>("DeleteReturnSerial")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("returnNo", returnNo)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("serialNo", serialNo)
						.AddQueryExOption("logSeq", logSeq)
						.AddQueryExOption("videoNo", videoNo).ToList().FirstOrDefault();
		}

		public ExecuteResult DoPosting(String dcCode, String gupCode, String custCode, String returnNo, String auditStaff, String auditName)
		{
			return CreateQuery<ExecuteResult>("DoPosting")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("returnNo", returnNo)
						.AddQueryExOption("auditStaff", auditStaff)
						.AddQueryExOption("auditName", auditName).ToList().FirstOrDefault();
		}

		public ExecuteResult DoForceClose(String dcCode, String gupCode, String custCode, String returnNo, String auditStaff, String auditName)
		{
			return CreateQuery<ExecuteResult>("DoForceClose")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("returnNo", returnNo)
						.AddQueryExOption("auditStaff", auditStaff)
						.AddQueryExOption("auditName", auditName).ToList().FirstOrDefault();
		}

		public IQueryable<F16140101Data> GetSerialItems(String dcCode, String gupCode, String custCode, String returnNo)
		{
			return CreateQuery<F16140101Data>("GetSerialItems")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("returnNo", returnNo);
		}

		public IQueryable<Wms3pl.Datas.F16.F161501> GetGatherItems(String dcCode, String gatherDateStart, String gatherDateEnd, String gatherNoStart, String gatherNoEnd, String fileName)
		{
			return CreateQuery<Wms3pl.Datas.F16.F161501>("GetGatherItems")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gatherDateStart", gatherDateStart)
						.AddQueryExOption("gatherDateEnd", gatherDateEnd)
						.AddQueryExOption("gatherNoStart", gatherNoStart)
						.AddQueryExOption("gatherNoEnd", gatherNoEnd)
						.AddQueryExOption("fileName", fileName);
		}

		public IQueryable<ExecuteResult> DoDelGatherData(String dcCode, String gatherNos)
		{
			return CreateQuery<ExecuteResult>("DoDelGatherData")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gatherNos", gatherNos);
		}

		public IQueryable<ExecuteResult> UpdatePickStaffEmptyByB2B(String userId, String userName)
		{
			return CreateQuery<ExecuteResult>("UpdatePickStaffEmptyByB2B")
						.AddQueryExOption("userId", userId)
						.AddQueryExOption("userName", userName);
		}

		public IQueryable<F151002Data> GetF151002Datas(String srcDcCode, String gupCode, String custCode, String allocationNo, String userId, String userName, String isAllowStatus2, String isDiffWareHouse)
		{
			return CreateQuery<F151002Data>("GetF151002Datas")
						.AddQueryExOption("srcDcCode", srcDcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("userId", userId)
						.AddQueryExOption("userName", userName)
						.AddQueryExOption("isAllowStatus2", isAllowStatus2)
						.AddQueryExOption("isDiffWareHouse", isDiffWareHouse);
		}

		public IQueryable<ExecuteResult> OutOfStockByP080301(String dcCode, String gupCode, String custCode, String allocationNo, String srcLocCode, String itemCode, String validDate, String serialNo)
		{
			return CreateQuery<ExecuteResult>("OutOfStockByP080301")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("srcLocCode", srcLocCode)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("validDate", validDate)
						.AddQueryExOption("serialNo", serialNo);
		}

		public IQueryable<F151002ItemLocData> GetF151002ItemLocDatas(String dcCode, String gupCode, String custCode, String allocationNo, String itemCode, String isDiffWareHouse)
		{
			return CreateQuery<F151002ItemLocData>("GetF151002ItemLocDatas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("isDiffWareHouse", isDiffWareHouse);
		}

		public IQueryable<ExecuteResult> ScanSrcLocItemCodeActualQty(String dcCode, String gupCode, String custCode, String allocationNo, String srcLocCode, String itemCode, String serialNo, String orginalValidDate, String newValidDate, Int32? addActualQty, String scanCode)
		{
			return CreateQuery<ExecuteResult>("ScanSrcLocItemCodeActualQty")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("srcLocCode", srcLocCode)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("serialNo", serialNo)
						.AddQueryExOption("orginalValidDate", orginalValidDate)
						.AddQueryExOption("newValidDate", newValidDate)
						.AddQueryExOption("addActualQty", addActualQty)
						.AddQueryExOption("scanCode", scanCode);
		}

		public IQueryable<ExecuteResult> RemoveSrcLocItemCodeActualQty(String dcCode, String gupCode, String custCode, String allocationNo, String srcLocCode, String itemCode, String removeValidDate)
		{
			return CreateQuery<ExecuteResult>("RemoveSrcLocItemCodeActualQty")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("srcLocCode", srcLocCode)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("removeValidDate", removeValidDate);
		}

		public IQueryable<ExecuteResult> StartDownOrUpItemChangeStatus(String dcCode, String gupCode, String custCode, String allocationNo, String isUp)
		{
			return CreateQuery<ExecuteResult>("StartDownOrUpItemChangeStatus")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("isUp", isUp);
		}

		public IQueryable<ExecuteResult> ChangeAllocationDownOrUpStatusToOrginal(String dcCode, String gupCode, String custCode, String allocationNo, String status, String isUp, Boolean? lackType)
		{
			return CreateQuery<ExecuteResult>("ChangeAllocationDownOrUpStatusToOrginal")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("status", status)
						.AddQueryExOption("isUp", isUp)
						.AddQueryExOption("lackType", lackType);
		}

		public IQueryable<ExecuteResult> ChangeAllocationDownOrUpFinish(String dcCode, String gupCode, String custCode, String allocationNo, String isUp)
		{
			return CreateQuery<ExecuteResult>("ChangeAllocationDownOrUpFinish")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("isUp", isUp);
		}

		public IQueryable<ExecuteResult> InsertOrUpdateF151004(String dcCode, String gupCode, String custCode, String allocationNo, String caseNo)
		{
			return CreateQuery<ExecuteResult>("InsertOrUpdateF151004")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("caseNo", caseNo);
		}

		public IQueryable<F151002DataByTar> GetF151002DataByTars(String tarDcCode, String gupCode, String custCode, String allocationNo, String userId, String userName, String isAllowStatus4, String isDiffWareHouse)
		{
			return CreateQuery<F151002DataByTar>("GetF151002DataByTars")
						.AddQueryExOption("tarDcCode", tarDcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("userId", userId)
						.AddQueryExOption("userName", userName)
						.AddQueryExOption("isAllowStatus4", isAllowStatus4)
						.AddQueryExOption("isDiffWareHouse", isDiffWareHouse);
		}

		public IQueryable<ExecuteResult> OutOfStockByP080302(String dcCode, String gupCode, String custCode, String allocationNo, String sugLocCode, String itemCode, String validDate, String serialNo)
		{
			return CreateQuery<ExecuteResult>("OutOfStockByP080302")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("sugLocCode", sugLocCode)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("validDate", validDate)
						.AddQueryExOption("serialNo", serialNo);
		}

		public IQueryable<F151002ItemLocDataByTar> GetF151002ItemLocDataByTars(String dcCode, String gupCode, String custCode, String allocationNo, String itemCode, String isDiffWareHouse)
		{
			return CreateQuery<F151002ItemLocDataByTar>("GetF151002ItemLocDataByTars")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("isDiffWareHouse", isDiffWareHouse);
		}

		public IQueryable<ExecuteResult> ScanTarLocItemCodeActualQty(String tarDcCode, String dcCode, String gupCode, String custCode, String allocationNo, String sugLocCode, String tarLocCode, String itemCode, String serialNo, String orginalValidDate, String newValidDate, Int32? addActualQty, String userId, String wareHouseId, String scanCode)
		{
			return CreateQuery<ExecuteResult>("ScanTarLocItemCodeActualQty")
						.AddQueryExOption("tarDcCode", tarDcCode)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("sugLocCode", sugLocCode)
						.AddQueryExOption("tarLocCode", tarLocCode)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("serialNo", serialNo)
						.AddQueryExOption("orginalValidDate", orginalValidDate)
						.AddQueryExOption("newValidDate", newValidDate)
						.AddQueryExOption("addActualQty", addActualQty)
						.AddQueryExOption("userId", userId)
						.AddQueryExOption("wareHouseId", wareHouseId)
						.AddQueryExOption("scanCode", scanCode);
		}

		public IQueryable<ExecuteResult> RemoveTarLocItemCodeActualQty(String dcCode, String gupCode, String custCode, String allocationNo, String tarLocCode, String itemCode, String removeValidDate)
		{
			return CreateQuery<ExecuteResult>("RemoveTarLocItemCodeActualQty")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("tarLocCode", tarLocCode)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("removeValidDate", removeValidDate);
		}

		public IQueryable<ExecuteResult> CheckTarLocCode(String dcCode, String wareHouseId, String locCode, String userId, String itemCode)
		{
			return CreateQuery<ExecuteResult>("CheckTarLocCode")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("wareHouseId", wareHouseId)
						.AddQueryExOption("locCode", locCode)
						.AddQueryExOption("userId", userId)
						.AddQueryExOption("itemCode", itemCode);
		}

		public IQueryable<ExecuteResult> SetWmsOrdAudited(String wmsOrdNo, String gupCode, String custCode, String dcCode, String pastNo)
		{
			return CreateQuery<ExecuteResult>("SetWmsOrdAudited")
						.AddQueryExOption("wmsOrdNo", wmsOrdNo)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("pastNo", pastNo);
		}

		public Boolean HasSerialItem(String dcCode, String gupCode, String custCode, String wmsOrdNo)
		{
			return CreateQuery<Boolean>("HasSerialItem")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("wmsOrdNo", wmsOrdNo).ToList().FirstOrDefault();
		}

		public IQueryable<InventoryScanLoc> GetInventoryScanLoc(String dcCode, String gupCode, String custCode, String inventoryNo, String locCode)
		{
			return CreateQuery<InventoryScanLoc>("GetInventoryScanLoc")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("inventoryNo", inventoryNo)
						.AddQueryExOption("locCode", locCode);
		}

		public IQueryable<InventoryScanItem> GetInventoryScanItem(String dcCode, String gupCode, String custCode, String inventoryNo, String locCode, String itemCodeOrSerialNo)
		{
			return CreateQuery<InventoryScanItem>("GetInventoryScanItem")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("inventoryNo", inventoryNo)
						.AddQueryExOption("locCode", locCode)
						.AddQueryExOption("itemCodeOrSerialNo", itemCodeOrSerialNo);
		}

		public IQueryable<InventoryItemQty> UpdateToGetInventoryItemQty(String dcCode, String gupCode, String custCode, String inventoryNo, String locCode, String itemCode, Int32? qty)
		{
			return CreateQuery<InventoryItemQty>("UpdateToGetInventoryItemQty")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("inventoryNo", inventoryNo)
						.AddQueryExOption("locCode", locCode)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("qty", qty);
		}

		public IQueryable<ExecuteResult> ClearInventoryItemQty(String dcCode, String gupCode, String custCode, String inventoryNo, String locCode, String itemCode)
		{
			return CreateQuery<ExecuteResult>("ClearInventoryItemQty")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("inventoryNo", inventoryNo)
						.AddQueryExOption("locCode", locCode)
						.AddQueryExOption("itemCode", itemCode);
		}

		public IQueryable<ExecuteResult> UpdateToF140104OrF140105(String dcCode, String gupCode, String custCode, String inventoryNo, String locCode, String clientName)
		{
			return CreateQuery<ExecuteResult>("UpdateToF140104OrF140105")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("inventoryNo", inventoryNo)
						.AddQueryExOption("locCode", locCode)
						.AddQueryExOption("clientName", clientName);
		}

		public String GetPickTimeSeparator(String dcCode, String gupCode, String custCode, DateTime? delvDate)
		{
			return CreateQuery<String>("GetPickTimeSeparator")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("delvDate", delvDate).ToList().FirstOrDefault();
		}

		public IQueryable<ExecuteResult> ClearSerialByBoxOrCaseNo(String dcCode, String gupCode, String custCode, String wmsOrdNo)
		{
			return CreateQuery<ExecuteResult>("ClearSerialByBoxOrCaseNo")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("wmsOrdNo", wmsOrdNo);
		}

		public IQueryable<SerialNoResult> GetSerialItem(String gupCode, String custCode, String barCode)
		{
			return CreateQuery<SerialNoResult>("GetSerialItem")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("barCode", barCode);
		}

		public IQueryable<ExecuteResult> CheckBarCode(String gupCode, String custCode, String itemCode, String barCode)
		{
			return CreateQuery<ExecuteResult>("CheckBarCode")
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("barCode", barCode);
		}

		public Boolean ExistsF700102ByWmsOrdNo(String dcCode, String gupCode, String custCode, String wmsOrdNo, String sourceNo)
		{
			return CreateQuery<Boolean>("ExistsF700102ByWmsOrdNo")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("wmsOrdNo", wmsOrdNo)
						.AddQueryExOption("sourceNo", sourceNo).ToList().FirstOrDefault();
		}

		public IQueryable<InventoryLocItem> GetInventoryLocItems(String dcCode, String gupCode, String custCode, String inventoryNo)
		{
			return CreateQuery<InventoryLocItem>("GetInventoryLocItems")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("inventoryNo", inventoryNo);
		}

		public IQueryable<LittleWhiteReport> GetLittleWhiteReport(String dcCode, String gupCode, String custCode, String wmsOrdNo)
		{
			return CreateQuery<LittleWhiteReport>("GetLittleWhiteReport")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("wmsOrdNo", wmsOrdNo);
		}
	}
}

