using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.ExDataServices.P02ExDataService
{
	public partial class P02ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{

		public IQueryable<VendorInfo> GetVendorInfo(String purchaseNo, String dcCode, String gupCode, String custCode)
		{
			return CreateQuery<VendorInfo>("GetVendorInfo")
						.AddQueryExOption("purchaseNo", purchaseNo)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode);
		}

		public IQueryable<ExecuteResult> InsertF020103(String date, String time, String purchaseNo, String pierCode, String vendorCode, String dcCode, String gupCode, String custCode, String userId)
		{
			return CreateQuery<ExecuteResult>("InsertF020103")
						.AddQueryExOption("date", date)
						.AddQueryExOption("time", time)
						.AddQueryExOption("purchaseNo", purchaseNo)
						.AddQueryExOption("pierCode", pierCode)
						.AddQueryExOption("vendorCode", vendorCode)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("userId", userId);
		}

		public IQueryable<ExecuteResult> InsertF020103ForP020102(String date, String bookInTime, String carNumber, String purchaseNo, String pierCode, String vendorCode, String dcCode, String gupCode, String custCode, String userId)
		{
			return CreateQuery<ExecuteResult>("InsertF020103ForP020102")
						.AddQueryExOption("date", date)
						.AddQueryExOption("bookInTime", bookInTime)
						.AddQueryExOption("carNumber", carNumber)
						.AddQueryExOption("purchaseNo", purchaseNo)
						.AddQueryExOption("pierCode", pierCode)
						.AddQueryExOption("vendorCode", vendorCode)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("userId", userId);
		}

		public IQueryable<ExecuteResult> UpdateF020103(String purchaseNo, String serialNo, String dcCode, String gupCode, String custCode, String pierCode, String date, String userId)
		{
			return CreateQuery<ExecuteResult>("UpdateF020103")
						.AddQueryExOption("purchaseNo", purchaseNo)
						.AddQueryExOption("serialNo", serialNo)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("pierCode", pierCode)
						.AddQueryExOption("date", date)
						.AddQueryExOption("userId", userId);
		}

		public IQueryable<ExecuteResult> UpdateF020103ForP020102(String purchaseNo, String serialNo, String dcCode, String gupCode, String custCode, String pierCode, String carNumber, String bookInTime, String userId, String arriveDate)
		{
			return CreateQuery<ExecuteResult>("UpdateF020103ForP020102")
						.AddQueryExOption("purchaseNo", purchaseNo)
						.AddQueryExOption("serialNo", serialNo)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("pierCode", pierCode)
						.AddQueryExOption("carNumber", carNumber)
						.AddQueryExOption("bookInTime", bookInTime)
						.AddQueryExOption("userId", userId)
						.AddQueryExOption("arriveDate", arriveDate);
		}

		public IQueryable<F020103Detail> GetF020103Detail(DateTime? arriveDate, String time, String dcCode, String vendorCode, String custCode, String gupCode)
		{
			return CreateQuery<F020103Detail>("GetF020103Detail")
						.AddQueryExOption("arriveDate", arriveDate)
						.AddQueryExOption("time", time)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("vendorCode", vendorCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("gupCode", gupCode);
		}

		public IQueryable<ExecuteResult> Delete(String date, String serialNo, String purchaseNo, String dcCode, String gupCode, String custCode)
		{
			return CreateQuery<ExecuteResult>("Delete")
						.AddQueryExOption("date", date)
						.AddQueryExOption("serialNo", serialNo)
						.AddQueryExOption("purchaseNo", purchaseNo)
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode);
		}

		public IQueryable<ExecuteResult> InsertF020104(String dcCode, String beginDate, String endDate, String pierCode, String area, String allowIn, String allowOut, String userId)
		{
			return CreateQuery<ExecuteResult>("InsertF020104")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("beginDate", beginDate)
						.AddQueryExOption("endDate", endDate)
						.AddQueryExOption("pierCode", pierCode)
						.AddQueryExOption("area", area)
						.AddQueryExOption("allowIn", allowIn)
						.AddQueryExOption("allowOut", allowOut)
						.AddQueryExOption("userId", userId);
		}

		public IQueryable<F020104Detail> GetF020104Data(String dcCode, DateTime? beginDate, DateTime? endDate, String pierCode, String area, String allowIn, String allowOut)
		{
			return CreateQuery<F020104Detail>("GetF020104Data")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("beginDate", beginDate)
						.AddQueryExOption("endDate", endDate)
						.AddQueryExOption("pierCode", pierCode)
						.AddQueryExOption("area", area)
						.AddQueryExOption("allowIn", allowIn)
						.AddQueryExOption("allowOut", allowOut);
		}

		public IQueryable<ExecuteResult> UpdateF020104(String dcCode, String beginDate, String endDate, String pierCode, String area, String allowIn, String allowOut, String userId)
		{
			return CreateQuery<ExecuteResult>("UpdateF020104")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("beginDate", beginDate)
						.AddQueryExOption("endDate", endDate)
						.AddQueryExOption("pierCode", pierCode)
						.AddQueryExOption("area", area)
						.AddQueryExOption("allowIn", allowIn)
						.AddQueryExOption("allowOut", allowOut)
						.AddQueryExOption("userId", userId);
		}

		public ExecuteResult UpdateP020201(String dcCode, String gupCode, String custCode, String purchaseNo, String carNumber, String userId)
		{
			return CreateQuery<ExecuteResult>("UpdateP020201")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("purchaseNo", purchaseNo)
						.AddQueryExOption("carNumber", carNumber)
						.AddQueryExOption("userId", userId).ToList().FirstOrDefault();
		}

		public IQueryable<P020201ReportData> P020201Report(String dcCode, String gupCode, String custCode, String purchaseNo)
		{
			return CreateQuery<P020201ReportData>("P020201Report")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("purchaseNo", purchaseNo);
		}

		public ExecuteResult UpdateP020203(String dcCode, String gupCode, String custCode, String purchaseNo, DateTime? deliveryDate, String rtNo)
		{
			return CreateQuery<ExecuteResult>("UpdateP020203")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("purchaseNo", purchaseNo)
						.AddQueryExOption("deliveryDate", deliveryDate)
						.AddQueryExOption("rtNo", rtNo).ToList().FirstOrDefault();
		}

		public IQueryable<P020203Data> GetP020203(String dcCode, String gupCode, String custCode, String purchaseNo, String rtNo)
		{
			return CreateQuery<P020203Data>("GetP020203")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("purchaseNo", purchaseNo)
						.AddQueryExOption("rtNo", rtNo);
		}

		public IQueryable<P020203Data> GetP020204(String dcCode, String gupCode, String custCode, String purchaseNo, String rtNo, String vnrCode, String startDt, String endDt)
		{
			return CreateQuery<P020203Data>("GetP020204")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("purchaseNo", purchaseNo)
						.AddQueryExOption("rtNo", rtNo)
						.AddQueryExOption("vnrCode", vnrCode)
						.AddQueryExOption("startDt", startDt)
						.AddQueryExOption("endDt", endDt);
		}

		public IQueryable<ExecuteResult> UpdateP020203RecvQty(String dcCode, String gupCode, String custCode, String purchaseNo, String purchaseSeq, String recvQty, String userId)
		{
			return CreateQuery<ExecuteResult>("UpdateP020203RecvQty")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("purchaseNo", purchaseNo)
						.AddQueryExOption("purchaseSeq", purchaseSeq)
						.AddQueryExOption("recvQty", recvQty)
						.AddQueryExOption("userId", userId);
		}

		public IQueryable<F190206CheckName> GetItemCheckList(String dcCode, String gupCode, String custCode, String itemCode, String purchaseNo, String purchaseSeq, String rtNo, String checkType)
		{
			return CreateQuery<F190206CheckName>("GetItemCheckList")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("purchaseNo", purchaseNo)
						.AddQueryExOption("purchaseSeq", purchaseSeq)
						.AddQueryExOption("rtNo", rtNo)
						.AddQueryExOption("checkType", checkType);
		}

		public IQueryable<ExecuteResult> UploadItemImage(String dcCode, String gupCode, String custCode, String purchaseNo, String purchaseSeq, String itemCode, String imagePath, String userId)
		{
			return CreateQuery<ExecuteResult>("UploadItemImage")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("purchaseNo", purchaseNo)
						.AddQueryExOption("purchaseSeq", purchaseSeq)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("imagePath", imagePath)
						.AddQueryExOption("userId", userId);
		}

		public IQueryable<ExecuteResult> InsertF02020104(String dcCode, String gupCode, String custCode, String purchaseNo, String purchaseSeq, String itemCode, String serialNo, String userId, String status, String isPass, String message, String rtNo, String batchNo)
		{
			return CreateQuery<ExecuteResult>("InsertF02020104")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("purchaseNo", purchaseNo)
						.AddQueryExOption("purchaseSeq", purchaseSeq)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("serialNo", serialNo)
						.AddQueryExOption("userId", userId)
						.AddQueryExOption("status", status)
						.AddQueryExOption("isPass", isPass)
						.AddQueryExOption("message", message)
						.AddQueryExOption("rtNo", rtNo)
						.AddQueryExOption("batchNo", batchNo);
		}

		public IQueryable<AcceptancePurchaseReport> GetAcceptancePurchaseReport(String dcCode, String gupCode, String custCode, String purchaseNo, String rtNo)
		{
			return CreateQuery<AcceptancePurchaseReport>("GetAcceptancePurchaseReport")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("purchaseNo", purchaseNo)
						.AddQueryExOption("rtNo", rtNo);
		}

		public IQueryable<FileUploadData> GetFileUploadSetting(String dcCode, String gupCode, String custCode, String purchaseNo, String rtNo, String includeAllItems)
		{
			return CreateQuery<FileUploadData>("GetFileUploadSetting")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("purchaseNo", purchaseNo)
						.AddQueryExOption("rtNo", rtNo)
						.AddQueryExOption("includeAllItems", includeAllItems);
		}

		public IQueryable<ExecuteResult> UpdateStatusByAfterUploadFile(String dcCode, String gupCode, String custCode, String purchaseNo, String rtNo, String includeAllItems)
		{
			return CreateQuery<ExecuteResult>("UpdateStatusByAfterUploadFile")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("purchaseNo", purchaseNo)
						.AddQueryExOption("rtNo", rtNo)
						.AddQueryExOption("includeAllItems", includeAllItems);
		}

		public IQueryable<F051201ReportDataA> GetF051201ReportDataAs(String dcCode, String gupCode, String custCode, String ordNo)
		{
			return CreateQuery<F051201ReportDataA>("GetF051201ReportDataAs")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("ordNo", ordNo);
		}

		public IQueryable<AllocationBundleSerialLocCount> GetAllocationBundleSerialLocCount(String dcCode, String gupCode, String custCode, String allocationNo)
		{
			return CreateQuery<AllocationBundleSerialLocCount>("GetAllocationBundleSerialLocCount")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo);
		}

		public IQueryable<F15100101Data> GetF15100101Data(String dcCode, String gupCode, String custCode, String allocationNo)
		{
			return CreateQuery<F15100101Data>("GetF15100101Data")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo);
		}

		public IQueryable<ExecuteResult> CheckLocCode(String locCode, String tarDcCode, String tarWareHouseId, String itemCode)
		{
			return CreateQuery<ExecuteResult>("CheckLocCode")
						.AddQueryExOption("locCode", locCode)
						.AddQueryExOption("tarDcCode", tarDcCode)
						.AddQueryExOption("tarWareHouseId", tarWareHouseId)
						.AddQueryExOption("itemCode", itemCode);
		}

		public IQueryable<ExecuteResult> InsertOrUpdateF1510BundleSerialLocData(String dcCode, String gupCode, String custCode, String allocationNo, String serialorBoxNo, String locCode, String itemCode)
		{
			return CreateQuery<ExecuteResult>("InsertOrUpdateF1510BundleSerialLocData")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("serialorBoxNo", serialorBoxNo)
						.AddQueryExOption("locCode", locCode)
						.AddQueryExOption("itemCode", itemCode);
		}

		public IQueryable<ExecuteResult> DeleteF1510BundleSerialLocData(String dcCode, String gupCode, String custCode, String allocationNo, String itemCode, String serialNo)
		{
			return CreateQuery<ExecuteResult>("DeleteF1510BundleSerialLocData")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("itemCode", itemCode)
						.AddQueryExOption("serialNo", serialNo);
		}

		public IQueryable<F1510Data> GetF1510DatasByTar(String dcCode, String gupCode, String custCode, String allocationNo, String allocationDate)
		{
			return CreateQuery<F1510Data>("GetF1510DatasByTar")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("allocationDate", allocationDate);
		}

		public IQueryable<F1510Data> GetF1510Data(String dcCode, String gupCode, String custCode, String allocationNo, String allocationDate, String status, String userId ,String makeNo)
		{
			return CreateQuery<F1510Data>("GetF1510Data")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("allocationDate", allocationDate)
						.AddQueryExOption("status", status)
						.AddQueryExOption("userId", userId)
						.AddQueryExOption("makeNo", makeNo);
		}

		public IQueryable<F1510BundleSerialLocData> GetF1510BundleSerialLocDatas(String dcCode, String gupCode, String custCode, String allocationNo, String checkSerialNo)
		{
			return CreateQuery<F1510BundleSerialLocData>("GetF1510BundleSerialLocDatas")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("checkSerialNo", checkSerialNo);
		}

		public IQueryable<F1510ItemLocData> GetF1510ItemLocDatas(String tarDcCode, String gupCode, String custCode, String allocationNo, String status, String itemCode)
		{
			return CreateQuery<F1510ItemLocData>("GetF1510ItemLocDatas")
						.AddQueryExOption("tarDcCode", tarDcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationNo", allocationNo)
						.AddQueryExOption("status", status)
						.AddQueryExOption("itemCode", itemCode);
		}

		public ExecuteResult ExistsF020301Data(String dcCode, String gupCode, String custCode, String purchaseNo, String itemCode)
		{
			return CreateQuery<ExecuteResult>("ExistsF020301Data")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("purchaseNo", purchaseNo)
						.AddQueryExOption("itemCode", itemCode).ToList().FirstOrDefault();
		}

		public IQueryable<P020205Main> GetJincangNoFileMain(String dcCode, String gupCode, String custCode, DateTime? importStartDate, DateTime? importEndDate, String poNo)
		{
			return CreateQuery<P020205Main>("GetJincangNoFileMain")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("importStartDate", importStartDate)
						.AddQueryExOption("importEndDate", importEndDate)
						.AddQueryExOption("poNo", poNo);
		}

		public IQueryable<P020205Detail> GetJincangNoFileDetail(String dcCode, String gupCode, String custCode, String fileName, String poNo)
		{
			return CreateQuery<P020205Detail>("GetJincangNoFileDetail")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("fileName", fileName)
						.AddQueryExOption("poNo", poNo);
		}

		public IQueryable<F020302Data> GetF020302Data(String dcCode, String gupCode, String custCode, String purchaseNo)
		{
			return CreateQuery<F020302Data>("GetF020302Data")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("purchaseNo", purchaseNo);
		}

		public IQueryable<F151001WithF02020107> GetDatasByTar(String tarDcCode, String gupCode, String custCode, DateTime? allocationDate, String status)
		{
			return CreateQuery<F151001WithF02020107>("GetDatasByTar")
						.AddQueryExOption("tarDcCode", tarDcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("allocationDate", allocationDate)
						.AddQueryExOption("status", status);
		}

		public IQueryable<F020201WithF02020101> GetF020201WithF02020101s(String dcCode, String gupCode, String custCode, String purchaseNo, String rtNo)
		{
			return CreateQuery<F020201WithF02020101>("GetF020201WithF02020101s")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("purchaseNo", purchaseNo)
						.AddQueryExOption("rtNo", rtNo);
		}

		public ExecuteResult CheckPurchaseNo(String dcCode, String gupCode, String custCode, String purchaseNo)
		{
			return CreateQuery<ExecuteResult>("CheckPurchaseNo")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("purchaseNo", purchaseNo).ToList().FirstOrDefault();
		}

		public ExecuteResult CheckShopNo(String dcCode, String gupCode, String custCode, String purchaseNo)
		{
			return CreateQuery<ExecuteResult>("CheckShopNo")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("purchaseNo", purchaseNo).ToList().FirstOrDefault();
		}

		public ExecuteResult UpdateShopNoBePurchaseNo(String dcCode, String gupCode, String custCode, String purchaseNo)
		{
			return CreateQuery<ExecuteResult>("UpdateShopNoBePurchaseNo")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("purchaseNo", purchaseNo).ToList().FirstOrDefault();
		}

		public ExecuteResult IsRecvQtyEqualsSerialTotal(String dcCode, String gupCode, String custCode, String purchaseNo)
		{
			return CreateQuery<ExecuteResult>("IsRecvQtyEqualsSerialTotal")
						.AddQueryExOption("dcCode", dcCode)
						.AddQueryExOption("gupCode", gupCode)
						.AddQueryExOption("custCode", custCode)
						.AddQueryExOption("purchaseNo", purchaseNo).ToList().FirstOrDefault();
		}

        public IQueryable<F02020109Data> GetF02020109Datas(String dcCode, String gupCode, String custCode, string stockNo, int stockSeq)
        {
            return CreateQuery<F02020109Data>("GetF02020109Datas")
                        .AddQueryExOption("dcCode", dcCode)
                        .AddQueryExOption("gupCode", gupCode)
                        .AddQueryExOption("custCode", custCode)
                        .AddQueryExOption("stockNo", stockNo)
                        .AddQueryExOption("stockSeq", stockSeq);
        }

        public IQueryable<F0202Data> GetF0202Datas(String dcCode, String gupCode, String custCode, DateTime? begCrtDate, DateTime? endCrtDate,
            string orderNo, DateTime begCheckinDate, DateTime endCheckinDate, string custOrdNo, string empId, string empName, string itemCode)
        {
            return CreateQuery<F0202Data>("GetF0202Datas")
                         .AddQueryExOption("dcCode", dcCode)
                .AddQueryExOption("gupCode", gupCode)
                .AddQueryExOption("custCode", custCode)
                .AddQueryExOption("begCrtDate", begCrtDate)
                .AddQueryExOption("endCrtDate", endCrtDate)
                .AddQueryExOption("orderNo", orderNo)
                .AddQueryExOption("begCheckinDate", begCheckinDate)
                .AddQueryExOption("endCheckinDate", endCheckinDate)
                .AddQueryExOption("custOrdNo", custOrdNo)
                .AddQueryExOption("empId", empId)
                 .AddQueryExOption("empName", empName)
                 .AddQueryExOption("itemCode", itemCode);
        }
    }
}

