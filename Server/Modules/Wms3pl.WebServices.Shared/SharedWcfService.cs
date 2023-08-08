using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Schedule;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Shared
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class SharedWcfService
	{
		/// <summary>
		/// 產生託運單號碼
		/// </summary>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult GenerateEgsConsign(AutoGenConsignParam param)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ConsignService(wmsTransaction);
			var result = service.GenerateEgsConsign(param);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		/// <summary>
		/// 超商取貨 產生託運單
		/// </summary>
		/// <param name="f050801"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult CreateConsignBySuperBU(F050801 f050801)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ConsignService(wmsTransaction);
			var result = service.CreateConsign(f050801);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		[OperationContract]
		public List<EgsReturnConsign> GetEgsReturnConsigns(EgsReturnConsignParam param)
		{
			var service = new ConsignService();
			return service.GetEgsReturnConsigns(param);
		}

		[OperationContract]
		public List<F194714> GetStatusList()
		{
			var service = new ConsignService();
			return service.GetStatusList();
		}

		[OperationContract]
		public List<F194714> GetStatusListByAllId(string allId)
		{
			var service = new ConsignService();
			return service.GetStatusList(allId);
		}
		[OperationContract]
		public List<F050901> GetUpdateF050901DataForSOD(string customerId, List<string> consignNos)
		{
			var service = new ConsignService();
			return service.GetUpdateF050901DataForSOD(customerId, consignNos);
		}

		[OperationContract]
		public List<F050901> GetUpdateF050901DataForLogId(string customerId, string logId, List<string> consignNos)
		{
			var service = new ConsignService();
			return service.GetUpdateF050901DataForLogId(customerId, logId, consignNos);
		}
		[OperationContract]
		public ExecuteResult UpdateStatusForTCAT(List<EgsReturnConsign> datas)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ConsignService(wmsTransaction);
			var result = service.UpdateStatusForTCAT(datas);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		[OperationContract]
		public ExecuteResult UpdateStatusForTCATSOD(List<F050901> datas)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ConsignService(wmsTransaction);
			var result = service.UpdateStatusForTCATSOD(datas);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public int InsertDbLog(string dcCode, string gupCode, string custCode, string scheduleName, string isSuccess, string message)
		{
			var repo = new SCHEDULE_JOB_RESULTRepository(Schemas.CoreSchema);
			var id = repo.InsertLog(dcCode, gupCode, custCode, scheduleName, isSuccess, message);
			return id ?? 0;
		}
		[OperationContract]
		public void UpdateDbLogIsSuccess(int id, string isSuccess, string message)
		{
			var repo = new SCHEDULE_JOB_RESULTRepository(Schemas.CoreSchema);
			repo.UpdateIsSuccess(id, isSuccess, message);
		}

		[OperationContract]
		public BoxPalletNoData GetBarCodePalletOrBoxNo()
		{
			var srv = new SerialNoService();
			return srv.GetBarCodePalletOrBoxNo();
		}

		[OperationContract]
		/// <summary>
		/// 取得EGS客代主檔設定
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns></returns>
		public List<F194715> GetEgsCustomerSetting(string customerId)
		{
			var service = new ConsignService();
			return service.GetEgsCustomerSetting(customerId).ToList();
		}

		[OperationContract]
		public List<HctShipReturn> GetHctShipReturns(HctShipReturnParam hctShipReturnParam)
		{
			var service = new ConsignService();
			return service.GetHctShipReturns(hctShipReturnParam);
		}
		[OperationContract]
		public ExecuteResult UpdateStatusForHCT(List<HctShipReturn> datas)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ConsignService(wmsTransaction);
			var result = service.UpdateStatusForHCT(datas);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;

		}
		[OperationContract]
		/// <summary>
		/// 取得EGS客代主檔設定
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns></returns>
		public List<F194715> GetHctCustomerSetting(string customerId)
		{
			var service = new ConsignService();
			return service.GetHctCustomerSetting(customerId).ToList();
		}
		/// <summary>
		/// 取得大榮貨運回檔資料
		/// </summary>
		/// <param name="hctShipReturnParam"></param>
		/// <returns></returns>
		[OperationContract]
		public List<KTJShipReturn> GetKTJShipReturns(HctShipReturnParam hctShipReturnParam)
		{
			var service = new ConsignService();
			return service.GetKTJShipReturns(hctShipReturnParam);
		}
		/// <summary>
		/// 取得範本檔案
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		[OperationContract]
		public ByteData GetSampleExcel(string filePath, string fileName = null)
		{
			var service = new SharedService();
			return service.GetSampleExcel(filePath, fileName);
		}

		[OperationContract]
		public ExecuteResult UpdateStatusSOD(List<F050901> datas)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ConsignService(wmsTransaction);
			var result = service.UpdateStatusForSOD(datas);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public List<F194715> GetKtjCustomerSetting(string customerId = null)
		{
			var service = new ConsignService();
			return service.GetKtjCustomerSetting(customerId).ToList();
		}

		[OperationContract]
		public ExecuteResult UpdateStatusForKTJ(List<KTJShipReturn> datas)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ConsignService(wmsTransaction);
			var result = service.UpdateStatusForKTJ(datas);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;

		}

		[OperationContract]
		public List<string> FindItems(string gupCode, string custCode, string barcode, ref F2501 f2501)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ItemService();
			var result = service.FindItems(gupCode, custCode, barcode, ref f2501);
			return result;
		}

    [OperationContract]
		public ExecuteResult ContainerCloseBoxWithRT(long f020501Id, string rtNo, string rtSeq)
		{
			var wmsTransaction = new WmsTransaction();
      var f020501Repo = new F020501Repository(Schemas.CoreSchema, wmsTransaction);
      var service = new WarehouseInService(wmsTransaction);
      var f020501 = f020501Repo.Find(x => x.ID == f020501Id);
      try
      {
        var lockRes = service.LockContainerProcess(f020501.CONTAINER_CODE);
        if (!lockRes.IsSuccessed)
          return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.ContainerIsProcessingTryLater, f020501.CONTAINER_CODE) };

        #region F020501容器頭檔狀態檢查
        var chkF020501Status = service.CheckF020501Status(f020501, 0);
        if (!chkF020501Status.IsSuccessed)
          return new ExecuteResult(chkF020501Status.IsSuccessed, chkF020501Status.MsgContent);
        #endregion F020501容器頭檔狀態檢查

        var result = service.ContainerCloseBox(f020501Id, rtNo, rtSeq);
        if (result.IsSuccessed)
        {
          if (result.f020501 != null && result.f020501.STATUS == "2")
          {
            var rtNoList = result.f020502s.Select(a => a.RT_NO).ToList();
            var finishedRtContainerStatusList = result.f020502s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.STOCK_NO, x.RT_NO })
              .Select(x => new RtNoContainerStatus
              {
                DC_CODE = x.Key.DC_CODE,
                GUP_CODE = x.Key.GUP_CODE,
                CUST_CODE = x.Key.CUST_CODE,
                STOCK_NO = x.Key.STOCK_NO,
                RT_NO = x.Key.RT_NO,
                F020501_ID = result.f020501.ID,
                F020501_STATUS = result.f020501.STATUS,
                ALLOCATION_NO = result.f020501.ALLOCATION_NO
              }).ToList();

            var res = service.AfterConatinerTargetFinishedProcess(result.f020501.DC_CODE, result.f020501.GUP_CODE, result.f020501.CUST_CODE, rtNoList, finishedRtContainerStatusList);

            if (!res.IsSuccessed)
              return res;

            wmsTransaction.Complete();
          }
          else
            wmsTransaction.Complete();

        }
        return new ExecuteResult { IsSuccessed = result.IsSuccessed, Message = result.Message, No = result.No };
      }
      catch (Exception ex)
      { return new ExecuteResult(false, ex.Message); }
      finally
      { service.UnlockContainerProcess(new[] { f020501.CONTAINER_CODE }.ToList()); }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="containerCode"></param>
    /// <returns></returns>
    [OperationContract]
		public ChkContainerResult CheckContainer(String containerCode)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ContainerService(wmsTransaction);
			var result = service.CheckContainer(containerCode);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="containerCode"></param>
		/// <returns></returns>
		[OperationContract]
		public GetItemAllDlnAndAllShpRes GetItemAllDlnAndAllShp(int? saveDay)
		{
			ItemService itemService = new ItemService();
			return itemService.GetItemAllDlnAndAllShp(saveDay);
		}


	}

}
