
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Wms3pl.Common.Enums;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.Wcssr.Services;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.F25;

namespace Wms3pl.WebServices.Process.P02.Services
{
  /// <summary>
  /// 系統功能
  /// </summary>
  [ServiceContract]
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
  public partial class P02WcfService
  {
    /// <summary>
    /// 更新商品檢驗的檢驗項目 (P02020301)
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="purchaseSeq"></param>
    /// <param name="data"></param>
    /// <param name="purchaseNo"></param>
    /// <param name="validDate"></param>
    /// <param name="rtNo"></param>
    /// <param name="checkItem"></param>
    /// <returns></returns>
    [OperationContract]
    public ExecuteResult UpdateF02020102(string dcCode, string gupCode, string custCode
        , string purchaseNo, string purchaseSeq, List<F190206CheckName> data, string validDate, string rtNo, string checkItem, F1905 item05, string makeNo)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P020203Service(wmsTransaction);
      DateTime dtmValidDate = DateTime.Parse(validDate);
      var result = srv.UpdateF02020102(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, data, dtmValidDate, rtNo, checkItem, item05, makeNo);
      if (result.IsSuccessed) wmsTransaction.Complete();
      return result;
    }

    [OperationContract]
    public ExecuteResult QuickUpdateF02020102(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P020203Service(wmsTransaction);
      var result = srv.QuickUpdateF02020102(dcCode, gupCode, custCode, purchaseNo, rtNo);
      if (result.IsSuccessed) wmsTransaction.Complete();
      return result;
    }

        /// <summary>
        /// 隨機抽驗序號(商品檢驗->序號刷讀)
        /// </summary>
        /// <param name="p020203Data"></param>
        /// <param name="rtNo"></param>
        /// <param name="newSerialNo"></param>
        /// <returns></returns>
        [OperationContract]
        public ExecuteResult RandomCheckSerial(P020203Data p020203Data, string rtNo, string newSerialNo)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P020203Service(wmsTransaction);
            newSerialNo = newSerialNo.ToUpper();
            var result = srv.RandomCheckSerial(p020203Data, rtNo, newSerialNo);

      if (result.IsSuccessed)
        wmsTransaction.Complete();

      return result;
    }

        /// <summary>
        /// 寫入序號收集
        /// </summary>>
        /// <param name="isScanSerial"></param>
        /// <returns></returns>
        [OperationContract]
        public ExecuteResult InsertF2501AndF02020104(string dcCode, string gupCode, string custCode
            , string purchaseNo, string purchaseSeq, string itemCode, List<SerialNoResult> serialNoResults, bool isScanSerial, string rtNo)
        {
            serialNoResults.ForEach(x => x.SerialNo = x.SerialNo.ToUpper());
            // 1.先取得所有的要匯入的序號
            var f020302Data = serialNoResults.Select(x => new F020302Data { SERIAL_NO = x.SerialNo, BATCH_NO = x.BatchNo }).ToList();

      // 2.在過濾出此次刷讀的序號
      serialNoResults = serialNoResults.Where(x => x.Checked).ToList();

      var wmsTransaction = new WmsTransaction();
      var srv = new P020203Service(wmsTransaction);
      var result = srv.InsertF2501(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, itemCode, serialNoResults, isScanSerial, rtNo, f020302Data);
      if (result.IsSuccessed)
        wmsTransaction.Complete();
      return result;
    }

    /// <summary>
    /// 更新特殊採購資訊
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="data"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [OperationContract]
    public ExecuteResult UpdateP02020304(string dcCode, string gupCode, string custCode, List<P020203Data> data, string userId)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P020203Service(wmsTransaction);
      var result = srv.UpdateP02020304(dcCode, gupCode, custCode, data, userId);
      if (result.IsSuccessed) wmsTransaction.Complete();
      return result;
    }

    

    #region P020301 - 調入上架

    [OperationContract]
    public ExecuteResult InsertOrUpdateF1510LocItemData(F1510Data f1510Data, List<F1510ItemLocData> datas)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P020301Service(wmsTransaction);
      var result = srv.InsertOrUpdateF1510LocItemData(f1510Data, datas);
      if (result.IsSuccessed)
        wmsTransaction.Complete();
      return result;
    }

    #region P020301 - 調入上架


        [OperationContract]
        public ExecuteResult UpdateF1510Data(string tarDcCode, string gupCode, string custCode, string allocationNo, List<F1510Data> datas)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P020301Service(wmsTransaction);
            var result = srv.UpdateF1510Data(tarDcCode, gupCode, custCode, allocationNo, datas, IsAllocationPosting:true);
            if (result.IsSuccessed)
                wmsTransaction.Complete();
            return result;
        }

    [OperationContract]
    public ExecuteResult ImportSerialNoLoc(string dcCode, string gupCode, string custCode, string allocationNo,
        List<ImportBundleSerialLoc> importBundleSerialLocList)
    {
      var wmsTransaction = new WmsTransaction();
      var p020301Service = new P020301Service(wmsTransaction);
      var result = p020301Service.ImportSerialNoLoc(dcCode, gupCode, custCode, allocationNo, importBundleSerialLocList);
      if (result.IsSuccessed)
        wmsTransaction.Complete();
      return result;
    }
    #endregion


    #endregion

    /// <summary>
    /// 新版-驗收確認
    /// </summary>
    /// <param name="acp"></param>
    /// <returns></returns>
    [OperationContract]
    public AcceptanceReturnData NewAcceptancePurchase(AcceptanceConfirmParam acp)
    {
      var logSvc = new LogService("商品檢驗_"+DateTime.Now.ToString("yyyyMMdd"));
      logSvc.Log($"******************驗收確認NewAcceptancePurchase ({acp.PurchaseNo}) Start******************");
      var acceptanceConfirmDt = DateTime.Now;
      AcceptanceReturnData result;
      var wmsTransaction = new WmsTransaction();
      var srv = new P020203Service(wmsTransaction);
      var p020206Srv = new P020206Service(wmsTransaction);

      var checkRTMode = srv.CheckRTModeValue(acp.RT_MODE);
      if (!checkRTMode.IsSuccessed)
        return new AcceptanceReturnData() { RT_NO = "", OrderNo = "", ExecuteResult = checkRTMode };

      if (acp.RT_MODE == "0")
        result = srv.AcceptanceConfirm(acp);
      else
        result = p020206Srv.AcceptanceConfirm(acp);

      if (result.ExecuteResult.IsSuccessed)
      {
        logSvc.Log($"NewAcceptancePurchase {logSvc.DateDiff(acceptanceConfirmDt, DateTime.Now)}");
        var acceptanceConfirmCompleteDt = DateTime.Now;
        wmsTransaction.Complete();
        logSvc.Log($"NewAcceptancePurchaseComplete {logSvc.DateDiff(acceptanceConfirmCompleteDt, DateTime.Now)}");

        var RecvCompleteCustNoUploadDt = DateTime.Now;
        wmsTransaction = new WmsTransaction();
        // 貨主設定不須上傳檔案結案此驗收單
        srv = new P020203Service(wmsTransaction);
        var result2 = srv.RecvCompleteCustNoUpload(acp.DcCode, acp.GupCode, acp.CustCode, acp.PurchaseNo, acp.RTNo, acp.RT_MODE);
        logSvc.Log($"RecvCompleteCustNoUpload {logSvc.DateDiff(RecvCompleteCustNoUploadDt, DateTime.Now)}");
        if (result2.IsSuccessed)
        {
          var recvCompleteCustNoUploadCompleteDt = DateTime.Now;
					wmsTransaction.Complete();
					logSvc.Log($"RecvCompleteCustNoUploadComplete {logSvc.DateDiff(recvCompleteCustNoUploadCompleteDt, DateTime.Now)}");
        }
      }
      logSvc.Log($"******************驗收確認NewAcceptancePurchase ({acp.PurchaseNo}) End******************");
      logSvc.Log(string.Empty, false);
      return result;
    }

    #region 列印棧板標籤資料   
    /// <summary>
    /// 商品檢驗_列印驗收後棧板貼紙
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="rtNO">驗收單號</param>
    /// <returns></returns>
    [OperationContract]
    public IQueryable<P0202030500PalletData> GetP0202030500PalletDatas(string dcCode, string gupCode, string custCode, string rtNO)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P020203Service(wmsTransaction);
      return srv.GetP0202030500PalletDatas(dcCode, gupCode, custCode, rtNO);
    }

    #endregion

    [OperationContract]
    public string GetRtNo(string dcCode, string gupCode, string custCode, string userId)
    {
      //ExecuteResult result = new ExecuteResult();
      var wmsTransaction = new WmsTransaction();
      var srv = new SharedService(wmsTransaction);
      var result = srv.GetRtNo(dcCode, gupCode, custCode, userId);
      wmsTransaction.Complete();
      //result.IsSuccessed =true;
      return result;
    }


        [OperationContract]
        public ExecuteResult InserF020301AndF020302(string dcCode, string gupCode, string custCode, List<P020205Detail> f020205Data)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P020203Service(wmsTransaction);
            //先把序號強制轉大寫
            f020205Data.ForEach(x => x.SERIAL_NO = x.SERIAL_NO.ToUpper());

            var result = srv.InserF020301AndF020302(dcCode, gupCode, custCode, f020205Data);
            if (result.IsSuccessed)
                wmsTransaction.Complete();
            return result;
        }

    [OperationContract]
    public ExecuteResult DeleteP020205(P020205Main p020205Main)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P020205Service(wmsTransaction);
      var result = srv.DeleteP020205(p020205Main);
      if (result.IsSuccessed)
        wmsTransaction.Complete();
      return result;
    }

    /// <summary>
    /// 檢核大量序號，使用完整檢核規則
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="itemCode"></param>
    /// <param name="largeSerialNo"></param>
    /// <param name="status"></param>
    /// <param name="ignoreCheckOfStatus"></param>
    /// <returns></returns>
    [OperationContract]
    public IQueryable<SerialNoResult> CheckLargeSerialNoFull(string dcCode, string gupCode, string custCode,
                                                             string itemCode, string[] largeSerialNo,
                                                             string status, string ignoreCheckOfStatus = "")
    {
      var srv = new SerialNoService();
      return srv.CheckLargeSerialNoFull(dcCode, gupCode, custCode,
                                          itemCode, largeSerialNo, status,
                                          ProcessWork.ScanSerial, ignoreCheckOfStatus)
                  .AsQueryable();
    }

    [OperationContract]
    public ExecuteResult CheckRepeatSerails(string dcCode, string gupCode, string custCode, string poNo, string itemCode, string[] largeSerialNo)
    {
      var srv = new P020203Service();
      return srv.CheckRepeatSerails(dcCode, gupCode, custCode, poNo, itemCode, largeSerialNo);
    }

    [OperationContract]
    public ExecuteResult InsertOrUpdateF020103(F020103 editF020103, bool isAdd)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P020101Service(wmsTransaction);

      var result = srv.InsertOrUpdateF020103(editF020103, isAdd);

      if (result.IsSuccessed) wmsTransaction.Complete();
      return result;
    }
    [OperationContract]
    public ExecuteResult OutOfStock(string dcCode, string gupCode, string custCode, string stockNo, string itemCode, string rtNo)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P020203Service(wmsTransaction);
      var result = srv.OutOfStock(dcCode, gupCode, custCode, stockNo, itemCode, rtNo);
      if (result.IsSuccessed)
        wmsTransaction.Complete();
      return result;
    }

    [OperationContract]
    public ExecuteResult CancelAcceptance(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P020203Service(wmsTransaction);
      var result = srv.CancelAcceptance(dcCode, gupCode, custCode, purchaseNo, rtNo);
      if (result.IsSuccessed)
        wmsTransaction.Complete();
      return result;
    }

        [OperationContract]
        public ExecuteResult InsertOrUpdateP02020307(F02020109Data[] f02020109Datas)
        {
            var wmsTransaction = new WmsTransaction();
            var srv = new P020203Service(wmsTransaction);

            //先把序號強制轉大寫
            foreach (var item in f02020109Datas)
              item.SERIAL_NO = item.SERIAL_NO.ToUpper();

            var result = srv.InsertOrUpdateP02020307(f02020109Datas.ToList());
            if (result.IsSuccessed)
                wmsTransaction.Complete();
            return result;
        }

    [OperationContract]
    public ExecuteResult UpdateF1903(string gupCode, string custCode
        , string itemCode, string needExpired, DateTime? firstInDate, int? saveDay, string eanCode1, string eanCode2, string eanCode3, short? allDln, int? allShp, string isPrecious, string fragile,
        string isEasyLos, string spill, string isMagnetic, string isPerishable, string tmprType, string IsTempControl, F1905 updateVolumn)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P020203Service(wmsTransaction);
			var result = srv.UpdateF1903(gupCode, custCode, itemCode, needExpired, firstInDate, saveDay, eanCode1, eanCode2, eanCode3, allDln, allShp, isPrecious, fragile, isEasyLos, spill, isMagnetic, isPerishable, tmprType, IsTempControl, updateVolumn);
      if (result.IsSuccessed) wmsTransaction.Complete();
      return result;
    }

    /// <summary>
    /// 按下查詢時, 自動產生F02020101資料
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="purchaseNo"></param>
    /// <param name="warehouseList"></param>
    /// <returns></returns>
    [OperationContract]
    public ExecuteResult UpdateP020203(string dcCode, string gupCode, string custCode, string purchaseNo, string[] warehouseList, string RT_MODE)
    {
      var logSvc = new LogService("商品檢驗_" + DateTime.Now.ToString("yyyyMMdd"));
      logSvc.Log($"******************查詢單號UpdateP020203 ({purchaseNo}) Start******************");
      var UpdateP020203Dt = DateTime.Now;
      var wmsTransaction = new WmsTransaction();
      var srv = new P020203Service(wmsTransaction);

      var result = srv.Update(dcCode, gupCode, custCode, purchaseNo, warehouseList, RT_MODE);
      if (!result.IsSuccessed)
        return result;
      logSvc.Log($"UpdateP020203 {logSvc.DateDiff(UpdateP020203Dt, DateTime.Now)}");

      var UpdateP020203CompleteDt = DateTime.Now;
      wmsTransaction.Complete();
      logSvc.Log($"UpdateP020203Complete {logSvc.DateDiff(UpdateP020203CompleteDt, DateTime.Now)}");

      var UpdateCheckSerialByVirtualItemDt = DateTime.Now;
      // 更新暫存驗收檔中的虛擬商品，若驗收數已等於進倉驗收數，則更新已刷讀序號欄位
      var serialRes = srv.UpdateCheckSerialByVirtualItem(dcCode, gupCode, custCode, purchaseNo, result.No);
      logSvc.Log($"UpdateCheckSerialByVirtualItem {logSvc.DateDiff(UpdateCheckSerialByVirtualItemDt, DateTime.Now)}");

      var UpdateCheckSerialByVirtualItemCompleteDt = DateTime.Now;
      if (serialRes)
        wmsTransaction.Complete();
      logSvc.Log($"UpdateCheckSerialByVirtualItemComplete {logSvc.DateDiff(UpdateCheckSerialByVirtualItemCompleteDt, DateTime.Now)}");
      logSvc.Log($"******************查詢單號UpdateP020203 ({purchaseNo}) End******************");
      logSvc.Log(string.Empty, false);
      return result;
    }

    [OperationContract]
    public ScanCargoData InsertF010301AndGetNewID(ScanCargoData f010301Data)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P020105Service(wmsTransaction);
      return srv.InsertF010301AndGetNewID(f010301Data);
    }

    [OperationContract]
    public ExecuteResult DeleteF010301ScanCargoDatas(ScanCargoData[] front_f010301s)
    {
      //var wmsTransaction = new WmsTransaction();
      //var repo = new F010301Repository(Schemas.CoreSchema, wmsTransaction);
      //return repo.DeleteF010301ScanCargoDatas(front_f010301s);

      var wmsTransaction = new WmsTransaction();
      var srv = new P020105Service(wmsTransaction);
      return srv.DeleteF010301ScanCargoDatas(front_f010301s);
    }

    [OperationContract]
    public ExecuteResult UpdateF010301BoxCount(ScanCargoData front_f010301s)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P020105Service(wmsTransaction);
      return srv.UpdateF010301BoxCount(front_f010301s);
    }

    [OperationContract]
    public ExecuteResult UpdateF010301ScanCargoMemo(ScanCargoData front_f010301s)
    {
      var wmsTransaction = new WmsTransaction();
      var repo = new F010301Repository(Schemas.CoreSchema, wmsTransaction);
      return repo.UpdateF010301ScanCargoMemo(front_f010301s);
    }

    [OperationContract]
    public ScanReceiptData InsertF010302AndReturnValue(ScanReceiptData f010302Data)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P020105Service(wmsTransaction);
      return srv.InsertF010302AndReturnValue(f010302Data);
    }

    [OperationContract]
    public ExecuteResult UpdateF010302ShipBoxCnt(ScanReceiptData f010302Data)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P020105Service(wmsTransaction);
      return srv.UpdateF010302ShipBoxCnt(f010302Data);
    }

    [OperationContract]
    public AddF020501Result AddF020501(String ContainerCode, int ItemQty, F0205 f0205data)
    {
      var srv = new P020206Service();
      return srv.AddF020501(ContainerCode, ItemQty, f0205data);
    }

    [OperationContract]
    public ExecuteResult DeleteContainerBindData(AreaContainerData areaContainerData)
    {
      var srv = new P020206Service();
      return srv.DeleteContainerBindData(areaContainerData);
    }

    [OperationContract]
    public ExecuteResult UpdateContainerBindData(List<AreaContainerData> areaContainerData)
    {
      var srv = new P020206Service();
      return srv.UpdateContainerBindData(areaContainerData);
    }

    [OperationContract]
    public Boolean CheckAllContainerIsDone(string dcCode, string gupCode, string custCode, string RTNo, string RTSEQ)
    {
      var repo = new F0205Repository(Schemas.CoreSchema);
      return repo.CheckAllContainerIsDone(dcCode, gupCode, custCode, RTNo, RTSEQ);
    }

    [OperationContract]
    public List<String> GetUnnormalAllocDatas(string dcCode, string gupCode, string custCode, List<string> allocNos)
    {
      var repo = new F151001Repository(Schemas.CoreSchema);
      return repo.GetUnnormalAllocDatas(dcCode, gupCode, custCode, allocNos);
    }

    [OperationContract]
		public ExecuteResult CallRecvVedio(string dcCode, string gupCode, string custCode, string stockNo, List<string> itemCodeList = null)
		{
			var srv = new P020203Service();
			return srv.CallRecvVedio(dcCode, gupCode, custCode, stockNo, itemCodeList);
    }

    [OperationContract]
    public ExecuteResult ReGetLmsApiStowShelfAreaGuide(string dcCode, string gupCode, string custCode, string purchaseNo, string[] warehouseList)
    {
      var wmsTransaction = new WmsTransaction();
      var srv = new P020203Service(wmsTransaction);
      var result = srv.ReGetLmsApiStowShelfAreaGuide(dcCode, gupCode, custCode, purchaseNo, warehouseList);
      if (result.IsSuccessed)
        wmsTransaction.Complete();
      return result;
    }

    [OperationContract]
    public int GetTodayRecvQty(string dcCode, string gupCode, string custCode, string purchaseNo, DateTime reveDate)
    {
      var srv = new P020203Service();
      return srv.GetTodayRecvQty(dcCode, gupCode, custCode, purchaseNo, reveDate);
    }

		/// <summary>
		///  複驗異常處理-手動排除異常資料新增
		/// </summary>
		/// <param name="f020501Id"></param>
		/// <param name="f020502Id"></param>
		/// <param name="Memo"></param>
		/// <returns></returns>
		[OperationContract]
    public ExecuteResult InsertManualProcessData(long f020501Id,long f020502Id, string Memo)
    {
      var wmsTransaction = new WmsTransaction();
      var f020501Repo = new F020501Repository(Schemas.CoreSchema, wmsTransaction);
      var warehouseInService = new WarehouseInService(wmsTransaction);
      var srv = new P020208Service(wmsTransaction);
      var f020501 = f020501Repo.Find(x => x.ID == f020501Id);
      if (f020501 == null)
        return new ExecuteResult { IsSuccessed = false, Message = "查無此驗收容器上架頭檔" };

      try
      {
        var lockRes = warehouseInService.LockContainerProcess(f020501);
        if (!lockRes.IsSuccessed)
          return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.ContainerIsProcessingTryLater, f020501.CONTAINER_CODE) };

        #region F020501容器頭檔狀態檢查
        var chkF020501Status = warehouseInService.CheckF020501Status(f020501, 1);
        if (!chkF020501Status.IsSuccessed)
          return new ExecuteResult(chkF020501Status.IsSuccessed, chkF020501Status.MsgContent);
        #endregion F020501容器頭檔狀態檢查

        var result = srv.InsertManualProcessData(f020501Id, f020502Id, Memo);
        if (result.IsSuccessed)
          wmsTransaction.Complete();
        return result;
      }
      catch (Exception ex)
      { return new ExecuteResult(false, ex.Message); }
      finally
      { warehouseInService.UnlockContainerProcess(new[] { f020501.CONTAINER_CODE }.ToList()); }
    }

    /// <summary>
    /// 複驗異常處理-修改驗收數資料新增
    /// </summary>
    /// <param name="f020501Id"></param>
    /// <param name="f020502Id"></param>
    /// <param name="removeRecvQty"></param>
    /// <param name="memo"></param>
    /// <returns></returns>
    [OperationContract]
    public ExecuteResult InsertModifyRecvQtyData(long f020501Id, long f020502Id, int removeRecvQty, string memo, List<string> RemoveSerialNos)
    {
      var wmsTransaction = new WmsTransaction();
      var f020501Repo = new F020501Repository(Schemas.CoreSchema, wmsTransaction);
      var warehouseInService = new WarehouseInService(wmsTransaction);
      var srv = new P020208Service(wmsTransaction);
      var f020501 = f020501Repo.Find(x => x.ID == f020501Id);
      if (f020501 == null)
        return new ExecuteResult { IsSuccessed = false, Message = "查無此驗收容器上架頭檔" };

      try
      {
        var lockRes = warehouseInService.LockContainerProcess(f020501);
        if (!lockRes.IsSuccessed)
          return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.ContainerIsProcessingTryLater, f020501.CONTAINER_CODE) };

        #region F020501容器頭檔狀態檢查
        var chkF020501Status = warehouseInService.CheckF020501Status(f020501, 1);
        if (!chkF020501Status.IsSuccessed)
          return new ExecuteResult(chkF020501Status.IsSuccessed, chkF020501Status.MsgContent);
        #endregion F020501容器頭檔狀態檢查

        var result = srv.InsertModifyRecvQtyData(f020501Id, f020502Id, removeRecvQty, memo, RemoveSerialNos);
        if (result.IsSuccessed)
          wmsTransaction.Complete();
        return result;
      }
      catch (Exception ex)
      { return new ExecuteResult(false, ex.Message); }
      finally
      { warehouseInService.UnlockContainerProcess(new[] { f020501.CONTAINER_CODE }.ToList()); }
    }

    /// <summary>
    /// 複驗異常處理-修改不良品數
    /// </summary>
    /// <param name="f020501Id"></param>
    /// <param name="f020502Id"></param>
    /// <param name="ngItem"></param>
    /// <param name="memo"></param>
    /// <param name="ngContainerCode"></param>
    /// <returns></returns>
    [OperationContract]
    public ExecuteResult InsertModifyNGQtyData(long f020501Id, long f020502Id, List<F02020109Data> ngItem, string memo, string ngContainerCode)
    {
      var wmsTransaction = new WmsTransaction();
      var f020501Repo = new F020501Repository(Schemas.CoreSchema, wmsTransaction);
      var f2501Repo = new F2501Repository(Schemas.CoreSchema, wmsTransaction);
      var warehouseInService = new WarehouseInService(wmsTransaction);
      var srv = new P020208Service(wmsTransaction);
      var lockContainers = new List<F020501>();
      var f020501 = f020501Repo.Find(x => x.ID == f020501Id);

      if (f020501 == null)
        return new ExecuteResult { IsSuccessed = false, Message = "查無此驗收容器上架頭檔" };

      try
      {
        lockContainers.Add(f020501);
        var lockRes = warehouseInService.LockContainerProcess(f020501);
        if (!lockRes.IsSuccessed)
          return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.ContainerIsProcessingTryLater, f020501.CONTAINER_CODE) };

        #region F020501容器頭檔狀態檢查
        var chkF020501Status = warehouseInService.CheckF020501Status(f020501, 1);
        if (!chkF020501Status.IsSuccessed)
          return new ExecuteResult(chkF020501Status.IsSuccessed, chkF020501Status.MsgContent);
        #endregion F020501容器頭檔狀態檢查

        var ngF020501 = new F020501 { CONTAINER_CODE = ngContainerCode };
        lockContainers.Add(ngF020501);
        lockRes = warehouseInService.LockContainerProcess(ngF020501);
        if (!lockRes.IsSuccessed)
          return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.ContainerIsProcessingTryLater, f020501.CONTAINER_CODE) };

        // No.2091 序號商品在複驗異常處理設為不良品時，要標註序號為不良品序號(F2501.ACTIVATED = 1)
        var ngSerialItem = ngItem.Where(o => !string.IsNullOrWhiteSpace(o.SERIAL_NO));
        if (ngSerialItem.Any())
        {
          var tarItems = new List<string>();
          foreach (var ngSerial in ngSerialItem)
          {
            var f2501 = f2501Repo.Find(o => o.GUP_CODE == ngSerial.GUP_CODE && o.CUST_CODE == ngSerial.CUST_CODE && o.SERIAL_NO == ngSerial.SERIAL_NO);
            if (f2501 == null)
            {
              return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.SerialDataNotFound, f020501.CONTAINER_CODE) };
            }

            tarItems.Add(f2501.SERIAL_NO);
          }

          f2501Repo.UpdateSerialActivated(f020501.GUP_CODE, f020501.CUST_CODE, tarItems, "1");
        }

        var result = srv.InsertModifyNGQtyData(f020501Id, f020502Id, ngItem, memo, ngContainerCode);
        if (result.IsSuccessed)
          wmsTransaction.Complete();
        return result;
      }
      catch (Exception ex)
      { return new ExecuteResult(false, ex.Message); }
      finally
      { warehouseInService.UnlockContainerProcess(lockContainers.Select(x => x.CONTAINER_CODE).ToList()); }
    }


    //進貨收發作更新F010201
    [OperationContract]
		public ExecuteResult UpdateF010201Status(string dcCode, string gupCode, string custCode, string stockNo)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P020201Service(wmsTransaction);
			var result = srv.UpdateF010201Status(dcCode, gupCode, custCode, stockNo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
	}
}

