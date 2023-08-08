using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P02.ExDataSources;
using Wms3pl.WebServices.Process.P02.Services;
using Wms3pl.WebServices.Process.P05.Services;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P02
{
    [System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public partial class P02ExDataService : DataService<P02ExDataSource>
    {
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
            config.UseVerboseErrors = true;
        }

        #region Vendor (廠商)共用
        /// <summary>
        /// 依進倉單號取得廠商資訊
        /// </summary>
        /// <param name="purchaseNo"></param>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <returns></returns>
        [WebGet]
        public IEnumerable<VendorInfo> GetVendorInfo(string purchaseNo, string dcCode, string gupCode, string custCode)
        {
            var srv = new P02Service();

            var result = srv.GetVendorInfo(purchaseNo, dcCode, gupCode, custCode);

            return result;
        }
        #endregion

        #region Pier (碼頭)共用
        #endregion

        #region P020101 - 進場預排
        /// <summary>
        /// 新增進場預排資料
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <param name="purchaseNo"></param>
        /// <param name="pierCode"></param>
        /// <param name="vendorCode"></param>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<ExecuteResult> InsertF020103(string date, string time
                , string purchaseNo, string pierCode, string vendorCode, string dcCode
                , string gupCode, string custCode, string userId)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P020101Service(wmsTransaction);

            // 新增F020103
            var result = srv.InsertF020103(DateTime.Parse(date), time, purchaseNo, pierCode, vendorCode, dcCode, gupCode, custCode, userId);
            if (result.IsSuccessed == true) wmsTransaction.Complete();
            return new List<ExecuteResult>() { result }.AsQueryable();
        }

        /// <summary>
        /// 新增進場預排資料. 從進場管理新增時.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <param name="purchaseNo"></param>
        /// <param name="pierCode"></param>
        /// <param name="vendorCode"></param>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<ExecuteResult> InsertF020103ForP020102(string date, string bookInTime, string carNumber
                , string purchaseNo, string pierCode, string vendorCode, string dcCode
                , string gupCode, string custCode, string userId)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P020102Service(wmsTransaction);
            // 新增F020103
            var result = srv.InsertF020103(DateTime.Parse(date), bookInTime, carNumber, purchaseNo, pierCode, vendorCode, dcCode, gupCode, custCode);
            if (result.IsSuccessed == true) wmsTransaction.Complete();
            return new List<ExecuteResult>() { result }.AsQueryable();
        }

        /// <summary>
        /// 更新碼頭
        /// </summary>
        /// <param name="purchaseNo"></param>
        /// <param name="serialNo"></param>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="pierCode"></param>
        /// <param name="date"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<ExecuteResult> UpdateF020103(string purchaseNo, string serialNo
, string dcCode, string gupCode, string custCode, string pierCode, string date, string userId)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P020101Service(wmsTransaction);
            var result = srv.UpdateF020103(purchaseNo, Convert.ToInt16(serialNo), dcCode, gupCode, custCode, pierCode, date, userId);
            if (result.IsSuccessed == true) wmsTransaction.Complete();
            return new List<ExecuteResult>() { result }.AsQueryable();
        }

        /// <summary>
        /// 更新碼頭. 從進場管理修改時.
        /// </summary>
        /// <param name="purchaseNo"></param>
        /// <param name="serialNo"></param>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="pierCode"></param>
        /// <param name="carNumber"></param>
        /// <param name="bookInTime"></param>
        /// <param name="userId"></param>
        /// <param name="arriveDate"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<ExecuteResult> UpdateF020103ForP020102(string purchaseNo, string serialNo
, string dcCode, string gupCode, string custCode, string pierCode, string carNumber, string bookInTime, string userId,
                        string arriveDate)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P020102Service(wmsTransaction);
            var result = srv.UpdateF020103(purchaseNo, Convert.ToInt16(serialNo), dcCode, gupCode, custCode, pierCode,
                            carNumber, bookInTime, userId, arriveDate);
            if (result.IsSuccessed == true) wmsTransaction.Complete();
            return new List<ExecuteResult>() { result }.AsQueryable();
        }

        /// <summary>
        /// 取得進場預排清單
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <param name="dcCode"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<F020103Detail> GetF020103Detail(DateTime arriveDate, string time
                , string dcCode, string vendorCode, string custCode, string gupCode)
        {
            var srv = new P020101Service();
            var result = srv.GetF020103Detail(arriveDate, time, dcCode, vendorCode, custCode, gupCode);
            return result.AsQueryable();
        }

        /// <summary>
        /// 刪除進場預排資料
        /// </summary>
        /// <param name="date"></param>
        /// <param name="time"></param>
        /// <param name="purchaseNo"></param>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<ExecuteResult> Delete(string date, string serialNo, string purchaseNo
                , string dcCode, string gupCode, string custCode)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P020101Service(wmsTransaction);
            var result = srv.Delete(date, Convert.ToInt16(serialNo), purchaseNo, dcCode, gupCode, custCode);
            if (result.IsSuccessed == true) wmsTransaction.Complete();
            return new List<ExecuteResult>() { result }.AsQueryable();
        }
        #endregion

        #region P020104 - 碼頭期間設定
        /// <summary>
        /// 
        /// </summary>
        /// <param name="f020104Detail"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<ExecuteResult> InsertF020104(string dcCode, string beginDate, string endDate, string pierCode, string area, string allowIn, string allowOut, string userId)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P020104Service(wmsTransaction);
            var result = srv.InsertF020104(dcCode, beginDate, endDate, pierCode, area, allowIn, allowOut, userId);
            if (result.IsSuccessed == true) wmsTransaction.Complete();
            return new List<ExecuteResult>() { result }.AsQueryable();
        }

        /// <summary>
        /// 傳回碼頭期間設定 (F020104)
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="pierCode"></param>
        /// <param name="area"></param>
        /// <param name="allowIn"></param>
        /// <param name="allowOut"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<F020104Detail> GetF020104Data(string dcCode, DateTime beginDate, DateTime endDate, string pierCode, string area, string allowIn, string allowOut)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new F020104Repository(Schemas.CoreSchema, wmsTransaction);
            var result = srv.GetF020104Detail(dcCode, beginDate, endDate, pierCode, area, allowIn, allowOut);
            wmsTransaction.Complete();
            return result;
        }

        /// <summary>
        /// 更新碼頭期間設定
        /// </summary>
        /// <param name="f020104data"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<ExecuteResult> UpdateF020104(string dcCode, string beginDate, string endDate, string pierCode, string area, string allowIn, string allowOut, string userId)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P020104Service(wmsTransaction);
            var result = srv.UpdateF020104(dcCode, beginDate, endDate, pierCode, area, allowIn, allowOut, userId);
            if (result.IsSuccessed == true) wmsTransaction.Complete();
            return new List<ExecuteResult>() { result }.AsQueryable();
        }
        #endregion

        #region P020201 - 廠商報到
        [WebGet]
        public ExecuteResult UpdateP020201(string dcCode, string gupCode, string custCode
                , string purchaseNo, string carNumber, string empId, string empName)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P020201Service(wmsTransaction);
            var result = srv.Update(dcCode, gupCode, custCode, purchaseNo, carNumber, empId, empName);
            if (result.IsSuccessed == true) wmsTransaction.Complete();
            return result;
        }

        /// <summary>
        /// 廠商報到報表
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="purchaseNo"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<P020201ReportData> P020201Report(string dcCode, string gupCode, string custCode, string purchaseNo)
        {
            var srv = new P020201Service();
            var result = srv.P020201Report(dcCode, gupCode, custCode, purchaseNo);
            return result;
        }

        /// <summary>
        /// 呼叫Lms上架倉別指示、Wcssr收單驗貨上架
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="stockNo"></param>
        /// <returns></returns>
        [WebGet]
        public ExecuteResult CallLmsApiWithWcssrApi(string dcCode, string gupCode, string custCode, string stockNo)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P020201Service(wmsTransaction);
            var result = srv.CallLmsApiWithWcssrApi(dcCode, gupCode, custCode, stockNo);
            return result;
        }
        #endregion

        #region P020203 - 商品檢驗
        [WebGet]
        public List<P020203Data> GetP020203Datas(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo, bool isPickLocFirst)
        {
            var wmsTransation = new WmsTransaction();
            var srv = new P020203Service(wmsTransation);
            var result = srv.GetP020203Datas(dcCode, gupCode, custCode, purchaseNo, rtNo, isPickLocFirst);
            if (result.Any())
                wmsTransation.Complete();
            return result;
        }

        /// <summary>
        /// 取得要顯示的驗收單資料集
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="purchaseNo"></param>
        /// <param name="rtNo"></param>
        /// <param name="vnrCode"></param>
        /// <param name="startDt"></param>
        /// <param name="endDt"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<P020203Data> GetP020204(string dcCode, string gupCode, string custCode, string purchaseNo
                        , string rtNo, string vnrCode, string custOrdNo, string allocationNo, string vnrNameConditon, string startDt = "", string endDt = "")
        {
            var srv = new P020203Service();

            var result = srv.Get(dcCode, gupCode, custCode, purchaseNo, rtNo, vnrCode, custOrdNo, allocationNo, vnrNameConditon, startDt, endDt);
            return result;
        }

        /// <summary>
        /// 更新驗收數
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="purchaseNo"></param>
        /// <param name="purchaseSeq"></param>
        /// <param name="userId"></param>
        /// <param name="rtNo"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<ExecuteResult> UpdateP020203RecvQty(string dcCode, string gupCode, string custCode
            , string purchaseNo, string purchaseSeq, string recvQty, string userId, string rtNo)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P020203Service(wmsTransaction);
            var result = srv.UpdateRecvQty(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, Convert.ToInt32(recvQty), userId, rtNo);
            if (result.IsSuccessed == true) wmsTransaction.Complete();
            return new List<ExecuteResult>() { result }.AsQueryable();
        }

        /// <summary>
        /// 取得商品的檢驗項目
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="checkType">空: 全部, 00: 進貨, 01: 出貨</param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<F190206CheckName> GetItemCheckList(string dcCode, string gupCode, string custCode, string itemCode
                , string purchaseNo, string purchaseSeq, string rtNo, string checkType = "")
        {
            var srv = new P020203Service();
            var result = srv.GetItemCheckList(dcCode, gupCode, custCode, itemCode, purchaseNo, purchaseSeq, rtNo, checkType);
            return result;
        }

        /// <summary>
        /// 上傳圖檔並回傳圖檔流水號 (IMAGE_NO)
        /// Memo: 停用, 因為限定商品圖檔只能上傳一次, 且不論上傳幾次都直接覆蓋原檔
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="imagePath"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<ExecuteResult> UploadItemImage(string dcCode, string gupCode, string custCode, string purchaseNo, string purchaseSeq, string itemCode, string imagePath, string userId)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P020203Service(wmsTransaction);
            var result = srv.UploadItemImage(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, itemCode, imagePath, userId);
            if (result.IsSuccessed == true) wmsTransaction.Complete();
            return new List<ExecuteResult>() { result }.AsQueryable();
        }

        [WebGet]
        public IQueryable<ExecuteResult> InsertF02020104(string dcCode, string gupCode, string custCode
                , string purchaseNo, string purchaseSeq, string itemCode, string serialNo, string userId, string status,
                string isPass, string message, string rtNo, string batchNo)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P020203Service(wmsTransaction);
            var f02020104Repo = new F02020104Repository(Schemas.CoreSchema);
            var orginalData = f02020104Repo.GetDatas(dcCode, gupCode, custCode, purchaseNo, purchaseSeq);

            var result = srv.InsertF02020104(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, itemCode, serialNo, status, isPass, message, orginalData.Count() + 1, rtNo, batchNo);
            if (result.IsSuccessed)
                wmsTransaction.Complete();
            return new List<ExecuteResult>() { result }.AsQueryable();
        }

        /// <summary>
        /// 取得商品檢驗驗收單報表
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="purchaseNo"></param>
        /// <param name="purchaseSeq"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<AcceptancePurchaseReport> GetAcceptancePurchaseReport(string dcCode, string gupCode, string custCode
                , string purchaseNo, string rtNo, string isDefect, string isAcceptanceContainer)
        {
            var srv = new P020203Service();
            var tmpIsDefect = isDefect == "1";
			var tmpIsAcceptanceContainer = isAcceptanceContainer == "1";

			var result = srv.GetAcceptancePurchaseReport(dcCode, gupCode, custCode, purchaseNo, rtNo, tmpIsDefect, tmpIsAcceptanceContainer);
            return result;
        }

        [WebGet]
        public IQueryable<FileUploadData> GetFileUploadSetting(string dcCode, string gupCode, string custCode,
                string purchaseNo, string rtNo, string includeAllItems)
        {
            var srv = new P020203Service();
            bool tmp = (includeAllItems == "1");
            var result = srv.GetFileUploadSetting(dcCode, gupCode, custCode, purchaseNo, rtNo, tmp);
            return result;
        }

        /// <summary>
        /// 檔案上傳後更新狀態
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="purchaseNo"></param>
        /// <param name="rtNo"></param>
        /// <param name="includeAllItems"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<ExecuteResult> UpdateStatusByAfterUploadFile(string dcCode, string gupCode, string custCode, string purchaseNo,
                string rtNo, string includeAllItems)
        {
            var srv = new P020203Service();
            bool tmp = (includeAllItems == "1");
            var result = srv.UpdateStatusByAfterUploadFile(dcCode, gupCode, custCode, purchaseNo, rtNo, tmp);
            return new List<ExecuteResult>() { result }.AsQueryable();
        }
        [WebGet]
        public IQueryable<F051201ReportDataA> GetF051201ReportDataAs(string dcCode, string gupCode, string custCode,
                string ordNo)
        {
            var wmsTransaction = new WmsTransaction();
            var f051201Repo = new F051201Repository(Schemas.CoreSchema);
            var datas = f051201Repo.GetDatasByOrdNo(dcCode, gupCode, custCode, ordNo.Split(','));
            var groupF051201List = from o in datas
                                   group o by new { o.DC_CODE, o.GUP_CODE, o.CUST_CODE, o.DELV_DATE, o.PICK_TIME }
                                                                 into g
                                   select g;
            var p050101Service = new P050101Service(wmsTransaction);
            var p050102Service = new P050102Service();
            var reports = new List<F051201ReportDataA>();
            foreach (var groupF051201 in groupF051201List)
            {
                var pickOrdNoList = groupF051201.Select(o => o.PICK_ORD_NO).ToList();
                var result = p050101Service.UpdateF051201IsPrinted(groupF051201.Key.DC_CODE, groupF051201.Key.GUP_CODE,
                        groupF051201.Key.CUST_CODE, groupF051201.Key.DELV_DATE.Value.ToString("yyyy/MM/dd"), groupF051201.Key.PICK_TIME,
                        Current.Staff, Current.StaffName, out pickOrdNoList, false);
                if (result.IsSuccessed)
                    wmsTransaction.Complete();
                reports.AddRange(p050102Service.GetF051201ReportDataAs(groupF051201.Key.DC_CODE, groupF051201.Key.GUP_CODE,
                        groupF051201.Key.CUST_CODE, groupF051201.Key.DELV_DATE.Value, groupF051201.Key.PICK_TIME,
                        string.Join(",", pickOrdNoList.ToArray())));
            }
            return reports.AsQueryable();
        }

        [WebGet]
        public IQueryable<F151001ReportByAcceptance> GetF151001ReportByAcceptance(string dcCode, string gupCode, string custCode,
            string purchaseNo, string rtNo, string allocationNo)
        {
            var wmsTransaction = new WmsTransaction();
            var f02020107Repo = new F02020107Repository(Schemas.CoreSchema);
            //var f151001Repo = new F151001Repository(Schemas.CoreSchema);
            //var f1980Repo = new F1980Repository(Schemas.CoreSchema);
            //var allocationNoList = f02020107Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode
            //															  && x.GUP_CODE == gupCode
            //															  && x.CUST_CODE == custCode
            //															  && x.PURCHASE_NO == purchaseNo
            //															  && x.RT_NO == rtNo)
            //									.Select(x=>x.ALLOCATION_NO);

            var result = f02020107Repo.GetF151001ReportByAcceptance(dcCode, gupCode, custCode, purchaseNo, rtNo, allocationNo);

            //var result = f151001Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode &&
            //										  x.GUP_CODE == gupCode &&
            //										  x.CUST_CODE == custCode &&
            //										  allocationNoList.Contains(x.ALLOCATION_NO))
            //			.Select(x => new F151001ReportByAcceptance
            //			{
            //				DC_CODE = x.DC_CODE,
            //				GUP_CODE = x.GUP_CODE,
            //				CUST_CODE = x.CUST_CODE,
            //				ALLOCATION_NO = x.ALLOCATION_NO,
            //				WAREHOUSE_ID = x.TAR_WAREHOUSE_ID,
            //				WAREHOUSE_NAME = f1980Repo.GetDatasByTrueAndCondition(y=>y.WAREHOUSE_ID == x.TAR_WAREHOUSE_ID).FirstOrDefault().WAREHOUSE_NAME,
            //			});
            return result;
        }
        #endregion

        #region P020301 - 調入上架

        [WebGet]
        public IQueryable<AllocationBundleSerialLocCount> GetAllocationBundleSerialLocCount(string dcCode, string gupCode,
                        string custCode, string allocationNo)
        {
            var p020301Service = new P020301Service();
            return p020301Service.GetAllocationBundleSerialLocCount(dcCode, gupCode, custCode, allocationNo);
        }
        [WebGet]
        public IQueryable<F15100101Data> GetF15100101Data(string dcCode, string gupCode, string custCode, string allocationNo)
        {
            var p020301Service = new P020301Service();
            return p020301Service.GetF15100101Data(dcCode, gupCode, custCode, allocationNo);
        }

        [WebGet]
    public IQueryable<ExecuteResult> CheckLocCode(string locCode, string tarDcCode, string tarWareHouseId, string itemCode, string validDate)
    {
      var sharedService = new SharedService();
      var result = sharedService.CheckLocCodeByMixLocAndMixItem(locCode, tarDcCode, tarWareHouseId, Current.Staff, itemCode, validDate);
      return new List<ExecuteResult> { result }.AsQueryable();
    }

    [WebGet]
        public IQueryable<ExecuteResult> InsertOrUpdateF1510BundleSerialLocData(string dcCode, string gupCode,
                string custCode, string allocationNo, string serialorBoxNo, string locCode, string itemCode)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P020301Service(wmsTransaction);
            var result = srv.InsertOrUpdateF1510BundleSerialLocData(dcCode, gupCode, custCode, allocationNo, serialorBoxNo, locCode, itemCode);
            wmsTransaction.Complete();
            return new List<ExecuteResult> { result }.AsQueryable();
        }

        [WebGet]
        public IQueryable<ExecuteResult> DeleteF1510BundleSerialLocData(string dcCode, string gupCode,
                string custCode, string allocationNo, string itemCode, string serialNo)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P020301Service(wmsTransaction);
            var result = srv.DeleteF1510BundleSerialLocData(dcCode, gupCode, custCode, allocationNo, itemCode, serialNo);
            if (result.IsSuccessed)
                wmsTransaction.Complete();
            return new List<ExecuteResult> { result }.AsQueryable();
        }

        [WebGet]
        public IQueryable<F1510Data> GetF1510DatasByTar(string dcCode, string gupCode, string custCode, string allocationNo, string allocationDate)
        {
            var p020301Service = new P020301Service();
            return p020301Service.GetF1510DatasByTar(dcCode, gupCode, custCode, allocationNo, DateTime.Parse(allocationDate));
        }

        [WebGet]
        public IQueryable<F1510Data> GetF1510Data(string dcCode, string gupCode, string custCode, string allocationNo, string allocationDate, string status, string userId, string makeNo, DateTime enterDate, string srcLocCode)
        {
            var p020301Service = new P020301Service();
            return p020301Service.GetF1510Data(dcCode, gupCode, custCode, allocationNo, allocationDate, status, userId, makeNo, enterDate, srcLocCode);
        }

        [WebGet]
        public IQueryable<F1510BundleSerialLocData> GetF1510BundleSerialLocDatas(string dcCode, string gupCode, string custCode, string allocationNo, string checkSerialNo)
        {
            var p020301Service = new P020301Service();
            return p020301Service.GetF1510BundleSerialLocDatas(dcCode, gupCode, custCode, allocationNo, checkSerialNo);
        }


        [WebGet]
        public IQueryable<F1510ItemLocData> GetF1510ItemLocDatas(string tarDcCode, string gupCode, string custCode,
                        string allocationNo, string status, string itemCode, DateTime validDate, string srcLocCode, string makeNo)
        {
            var p020301Service = new P020301Service();
            return p020301Service.GetF1510ItemLocDatas(tarDcCode, gupCode, custCode, allocationNo, status, itemCode, validDate, srcLocCode, makeNo);
        }



        #endregion

        [WebGet]
        public ExecuteResult ExistsF020301Data(string dcCode, string gupCode, string custCode, string purchaseNo, string itemCode)
        {
            var _wmsTransaction = new WmsTransaction();
            var srv = new P020203Service(_wmsTransaction);
            var result = srv.ExistsF020301Data(dcCode, gupCode, custCode, purchaseNo, itemCode);
            if (result.IsSuccessed)
                _wmsTransaction.Complete();
            return result;
        }

        #region P020205

        [WebGet]
        public IQueryable<P020205Main> GetJincangNoFileMain(string dcCode, string gupCode, string custCode, DateTime importStartDate, DateTime importEndDate, string poNo)
        {
            var service = new P020205Service();
            var result = service.GetJincangNoFileMain(dcCode, gupCode, custCode, importStartDate, importEndDate, poNo);
            return result;
        }

        [WebGet]
        public IQueryable<P020205Detail> GetJincangNoFileDetail(string dcCode, string gupCode, string custCode, string fileName, string poNo)
        {
            var service = new P020205Service();
            var result = service.GetJincangNoFileDetail(dcCode, gupCode, custCode, fileName, poNo);
            return result;
        }
        #endregion

        [WebGet]
        public IQueryable<F020302Data> GetF020302Data(string dcCode, string gupCode, string custCode, string purchaseNo)
        {
            var _wmsTransaction = new WmsTransaction();
            var srv = new P020203Service(_wmsTransaction);
            var result = srv.GetF020302Data(dcCode, gupCode, custCode, purchaseNo);
            if (result != null)
                _wmsTransaction.Complete();
            return result;
        }

        #region P0203010000調入上架
        [WebGet]
        public IQueryable<F151001WithF02020107> GetDatasByTar(string tarDcCode, string gupCode, string custCode, DateTime allocationDate, string status)
        {
            var repo = new F151001Repository(Schemas.CoreSchema);
            return repo.GetDatasByTar(tarDcCode, gupCode, custCode, allocationDate, status.Split(','));
        }

        #endregion

        /// <summary>
        /// 取得驗收單查詢的驗收明細
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="purchaseNo"></param>
        /// <param name="rtNo"></param>
        /// <returns></returns>
        [WebGet]
        public IQueryable<F020201WithF02020101> GetF020201WithF02020101s(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo)
        {
            var repo = new F020201Repository(Schemas.CoreSchema);
            return repo.GetF020201WithF02020101s(dcCode, gupCode, custCode, purchaseNo, rtNo);
        }

        /// <summary>
        /// 商品檢驗->進倉單號查詢前的檢查
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="purchaseNo"></param>
        /// <param name="deliverDate"></param>
        /// <returns></returns>
        [WebGet]
        public ExecuteResult CheckPurchaseNo(string dcCode, string gupCode, string custCode, string purchaseNo)
        {
            var srv = new P020203Service();
            return srv.CheckPurchaseNo(dcCode, gupCode, custCode, purchaseNo);
        }

        #region 當進倉單沒有填採購單號時，詢問是否繼續作業，是的話，採購單號自動填入進倉單號
        /// <summary>
        /// 檢查F010201尚未填採購單號且該採購單有序號商品。
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="purchaseNo"></param>
        /// <returns></returns>
        [WebGet]
        public ExecuteResult CheckShopNo(string dcCode, string gupCode, string custCode, string purchaseNo)
        {
            var srv = new P020203Service();
            return srv.CheckShopNo(dcCode, gupCode, custCode, purchaseNo);
        }

        /// <summary>
        /// 更新採購單號為進倉單號
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="purchaseNo"></param>
        /// <returns></returns>
        [WebGet]
        public ExecuteResult UpdateShopNoBePurchaseNo(string dcCode, string gupCode, string custCode, string purchaseNo)
        {
            var srv = new P020203Service();
            return srv.UpdateShopNoBePurchaseNo(dcCode, gupCode, custCode, purchaseNo);
        }
        #endregion

        /// <summary>
        /// 是否驗收數(F02020101) != 進倉驗收檔(F020302)數量
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="purchaseNo"></param>
        /// <param name="rtNo"></param>
        /// <returns></returns>
        [WebGet]
        public ExecuteResult IsRecvQtyEqualsSerialTotal(string dcCode, string gupCode, string custCode, string purchaseNo)
        {
            var repo = new F02020101Repository(Schemas.CoreSchema);
            bool isNotEqualQty = repo.IsRecvQtyNotEqualsSerialTotal(dcCode, gupCode, custCode, purchaseNo);
            if (isNotEqualQty)
                return new ExecuteResult(false, "進倉單驗收序號數與驗收總數不符");

            return new ExecuteResult(true);
        }

        [WebGet]
        public ExecuteResult UpdateF010201Status(string dcCode, string gupCode, string custCode, string stockNo)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P020203Service(wmsTransaction);

            var result = srv.UpdateF010201(dcCode, gupCode, custCode, stockNo);
            if (!result.IsSuccessed)
            {
                return result;
            }
            wmsTransaction.Complete();
            return result;
        }
        [WebGet]
        public IQueryable<F02020109Data> GetF02020109Datas(string dcCode, string gupCode, string custCode, string stockNo, int stockSeq)
        {
            var repo = new F02020109Repository(Schemas.CoreSchema);
            return repo.GetF02020109Datas(dcCode, gupCode, custCode, stockNo, stockSeq);
        }

        [WebGet]
        public IQueryable<F0202Data> GetF0202Datas(string dcCode, string gupCode, string custCode, DateTime? begCrtDate, DateTime? endCrtDate,
             string orderNo, DateTime begCheckinDate, DateTime endCheckinDate, string custOrdNo, string empId, string empName, string itemCode,
             string selectedFastType)
        {
            var repo = new F0202Repository(Schemas.CoreSchema);
            //return repo.GetF0202Datas(dcCode, gupCode, custCode, begCrtDate, endCrtDate, orderNo, begCheckinDate, endCheckinDate, custOrdNo, empId, empName, itemCode, selectedFastType);
      return repo.GetF0202Datas_SQL(dcCode, gupCode, custCode, begCrtDate, endCrtDate, orderNo, begCheckinDate, endCheckinDate, custOrdNo, empId, empName, itemCode, selectedFastType);
    }

        [WebGet]
        public IQueryable<VW_F010301> GetALLVMF010301(string DcCode, DateTime? RecvDateS, DateTime? RecvDateE, string AllId, string EmpID, string CheckStatus, string ShipOrdNo)
        {
            var repo = new F010301Repository(Schemas.CoreSchema);
            return repo.GetALLVMF010301(DcCode, RecvDateS, RecvDateE, AllId, EmpID, CheckStatus, ShipOrdNo);
        }

        [WebGet]
        public IQueryable<ScanCargoData> GetF010301UncheckedScanCargoDatas(string dcCode, string LogisticCode, string RecvUser)
        {
            var repo = new F010301Repository(Schemas.CoreSchema);
            return repo.GetF010301UncheckedScanCargoDatas(dcCode, LogisticCode, RecvUser);
        }

        [WebGet]
        public IQueryable<ScanCargoStatistic> GetF010301ScanCargoStatistic(string dcCode, string LogisticCode)
        {
            var repo = new F010301Repository(Schemas.CoreSchema);
            return repo.GetF010301ScanCargoStatistic(dcCode, LogisticCode);
        }

        [WebGet]
        public IQueryable<ReceiptUnCheckData> GetF010301UncheckReceiptShipOrdNo(string dcCode, string LogisticCode)
        {
            var repo = new F010301Repository(Schemas.CoreSchema);
            return repo.GetF010301UncheckReceiptShipOrdNo(dcCode, LogisticCode);
        }

        [WebGet]
        public IQueryable<ScanReceiptData> GetF010302TodayReceiptData(string dcCode, string LogisticCode)
        {
            var repo = new F010302Repository(Schemas.CoreSchema);
            return repo.GetF010302TodayReceiptData(dcCode, LogisticCode);

        }

        [WebGet]
        public IQueryable<ContainerDetailData> GetContainerDetail(string dcCode, string gupCode, string custCode, string f020501Id)
        {
            var repo = new F020501Repository(Schemas.CoreSchema);
            return repo.GetContainerDetail(dcCode, gupCode, custCode, f020501Id);
        }

        [WebGet]
        public IQueryable<ItemBindContainerData> GetItemBindContainerData(string dcCode, string gupCode, string custCode, string RTNo, string RTSEQ)
        {
            var repo = new F0205Repository(Schemas.CoreSchema);
            return repo.GetItemBindContainerData(dcCode, gupCode, custCode, RTNo, RTSEQ);
        }

        [WebGet]
        public IQueryable<BindContainerData> GetBindContainerData(string dcCode, string gupCode, string custCode, string RTNo, string RTSEQ)
        {
            var repo = new F020501Repository(Schemas.CoreSchema);
            return repo.GetBindContainerData(dcCode, gupCode, custCode, RTNo, RTSEQ);
        }

        [WebGet]
        public ExecuteResult UpdataF0205StatusTo1(string dcCode, string gupCode, string custCode, string RTNo, string RTSeq)
        {
            var wmsTransaction = new WmsTransaction();
            var repo = new F0205Repository(Schemas.CoreSchema, wmsTransaction);
            var result = repo.UpdataF0205StatusTo1(dcCode, gupCode, custCode, RTNo, RTSeq);
            if (result.IsSuccessed)
                wmsTransaction.Complete();
            return result;
        }

        [WebGet]
        public ExecuteResult SetContainerComplete(string dcCode, string gupCode, string custCode, string RTNo, string RTSEQ)
        {
            var srv = new P020206Service();
            return srv.SetContainerComplete(dcCode, gupCode, custCode, RTNo, RTSEQ);
        }

        [WebGet]
        public IQueryable<AreaContainerData> GetAreaContainerData(string dcCode, string gupCode, string custCode, string typeCode, string RTNo, string RTSeq)
        {
            var repo = new F020501Repository(Schemas.CoreSchema);
            return repo.GetAreaContainerData(dcCode, gupCode, custCode, typeCode, RTNo, RTSeq);
        }
    

		/// <summary>
		/// 驗收單與上架容器查詢(查詢結果)
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="rtNo"></param>
		/// <param name="vnrCode"></param>
		/// <param name="custOrdNo"></param>
		/// <param name="allocationNo"></param>
		/// <param name="vnrNameConditon"></param>
		/// <param name="startDt"></param>
		/// <param name="endDt"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<P020206Data> GetP020207(string dcCode, string gupCode, string custCode, string purchaseNo
						, string rtNo, string vnrCode, string custOrdNo, string containerCode, string vnrNameConditon, string startDt = "", string endDt = "")
		{
			var srv = new P020206Service();

			var result = srv.Get(dcCode, gupCode, custCode, purchaseNo, rtNo, vnrCode, custOrdNo, containerCode, vnrNameConditon, startDt, endDt);
			return result;
		}

		[WebGet]
		public IQueryable<AcceptanceDetail> GetAcceptanceDetail(string dcCode, string gupCode, string custCode, string rtNo)
		{
			var srv = new P020206Service();
			var result = srv.GetAcceptanceDetail(dcCode, gupCode, custCode, rtNo);
			return result;
		}

		[WebGet]
		public IQueryable<AcceptanceContainerDetail> GetAcceptanceContainerDetail(string dcCode, string gupCode, string custCode, string rtNo)
		{
			var srv = new P020206Service();
			var result = srv.GetAcceptanceContainerDetail(dcCode, gupCode, custCode, rtNo);
			return result;
		}

		[WebGet]
		public IQueryable<DefectDetail> GetDefectDetail(string dcCode, string gupCode, string custCode, string rtNo)
		{
			var srv = new P020206Service();
			var result = srv.GetDefectDetail(dcCode, gupCode, custCode, rtNo);
			return result;
		}
	
		[WebGet]
		public IQueryable<DefectDetailReport> GetDefectDetailReportData(string dcCode,string gupCode,string custCode,string rtNo)
		{
			var repo = new F02020109Repository(Schemas.CoreSchema);
			return repo.GetDefectDetailReportData(dcCode, gupCode,custCode,rtNo);
		}

    [WebGet]
    public IQueryable<F020504ExData> GetF020504ExDatas(string dcCode, string gupCode, string custCode, DateTime? ProcDateStart, DateTime? ProcDateEnd, string StockNo, string RTNo, string ProcCode, string ContainerCode, string ItemCode, string Status)
    {
      var repo = new F020504Repository(Schemas.CoreSchema);
      return repo.GetF020504ExDatas(dcCode, gupCode, custCode, ProcDateStart, ProcDateEnd, StockNo, RTNo, ProcCode, ContainerCode, ItemCode, Status);
    }

    [WebGet]
    public IQueryable<UnnormalItemRecheckLog> GetUnnormalItemRecheckLog(string F020504ID)
    {
      var repo = new F02050401Repository(Schemas.CoreSchema);
      long longf020504ID;
      if (!long.TryParse(F020504ID, out longf020504ID))
        throw new Exception("無法轉換型別");
      return repo.GetUnnormalItemRecheckLog(longf020504ID);
    }

    [WebGet]
    public IQueryable<ContainerRecheckFaildItem> GetContainerRecheckFaildItem(string dcCode, string gupCode, string custCode, string ContainerCode)
    {
      var repo = new F020502Repository(Schemas.CoreSchema);
      return repo.GetContainerRecheckFaildItem(dcCode, gupCode, custCode, ContainerCode);
    }

    #region 複驗異常處理取得
    [WebGet]
    public P020203Data GetModifyRecheckNGDatas(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo, string stockSeq)
    {
      var srv = new P020208Service();
      return srv.GetModifyRecheckNGDatas(dcCode, gupCode, custCode, purchaseNo, rtNo, stockSeq);
    }
    #endregion

    /// <summary>
    /// 複驗異常處理-移出序號刷讀-檢查序號是否存在此驗收單中
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="rtNo"></param>
    /// <param name="serialNo"></param>
    /// <returns></returns>
    [WebGet]
    public ExecuteResult CheckSerialNo(string dcCode, string gupCode, string custCode, string rtNo, string serialNo)
    {
      var srv = new P020208Service();
      return srv.CheckSerialNo(dcCode, gupCode, custCode, rtNo, serialNo);
    }

    /// <summary>
    /// 商品標籤列印 - 進倉驗收檔
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="rtNo"></param>
    /// <param name="serialNo"></param>
    /// <returns></returns>
    [WebGet]
    public IQueryable<RecvRecords> GetF020209RecvRecord(string dcCode, string gupCode, string custCode, DateTime RecvDateBegin, DateTime RecvDateEnd, string PurchaseNo, 
      string CustOrdNo, string PrintMode, string PalletLocation, string ItemCode, string RecvStaff)
    {
      var srv = new P020209Service();
      return srv.GetF020209RecvRecord(dcCode, gupCode, custCode, RecvDateBegin, RecvDateEnd, PurchaseNo, CustOrdNo, PrintMode, PalletLocation, ItemCode, RecvStaff);
    }

    /// <summary>
    /// 商品標籤列印 - 商品標籤資料
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="rtNo"></param>
    /// <returns></returns>
    [WebGet]
    public IQueryable<ItemLabelData> GetF020209ItemLabelData(string dcCode, string gupCode, string custCode, string rtNos)
    {
      var srv = new P020209Service();
      return srv.GetF020209ItemLabelData(dcCode, gupCode, custCode, rtNos);
    }
  }
}
