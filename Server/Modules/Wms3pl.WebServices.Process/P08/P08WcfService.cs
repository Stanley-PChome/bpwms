using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;
using System.Diagnostics;
using Wms3pl.Datas.F25;

namespace Wms3pl.WebServices.Process.P08.Services
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class P08WcfService
	{
		#region 範例用，以後移除

		[OperationContract]
		public IQueryable<ExecuteResult> GetF1929WithF1909Tests(string gupCode)
		{
			return new List<ExecuteResult>().AsQueryable();
		}
		#endregion 範例用，以後移除

		#region P080701 出貨包裝
		[OperationContract]
		public ExecuteResult CheckAndUpdatePacking(string dcCode, string gupCode, string custCode, string wmsOrdNo, List<string> serials)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P080701Service(wmsTransaction);
			var result = srv.CheckAndUpdatePacking(dcCode, gupCode, custCode, wmsOrdNo, serials);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public ExecuteResult CancelPacking(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P080701Service(wmsTransaction);
			var result = srv.CancelPacking(dcCode, gupCode, custCode, wmsOrdNo);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}

		/// <summary>
		/// 檢查刷讀序號是否在此出貨單指定序號出貨內
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <param name="serialStatus"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult CheckSerialsInOrder(string dcCode, string gupCode, string custCode, string wmsOrdNo, SerialDataEx serialStatus)
		{
			var f050802Repo = new F050802Repository(Schemas.CoreSchema);
			var f050802Data = f050802Repo.GetDatasByHasSerial(dcCode, gupCode, custCode, wmsOrdNo);

			var srv = new P080701Service();
			var result = srv.CheckSerialsInOrder(dcCode, gupCode, custCode, wmsOrdNo, serialStatus, f050802Data);
			return result;
		}

		/// <summary>
		/// 檢查序號狀態
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <param name="serials"></param>
		/// <returns></returns>
		[OperationContract]
		public IQueryable<SerialDataEx> CheckSerialStatus(string dcCode, string gupCode, string custCode, string wmsOrdNo, List<string> serials)
		{
			var srv = new P080701Service();
			var result = srv.CheckSerials(dcCode, gupCode, custCode, wmsOrdNo, serials).AsQueryable();
			return result;
		}
		[OperationContract]
		/// <summary>
		/// 刷讀條碼尋找出貨單
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="scanCode">刷讀條碼</param>
		/// <returns>出貨單 or Null</returns>
		public F050801 SearchWmsOrder(string dcCode, string gupCode, string custCode, string scanCode)
		{
			var srv = new P080701Service();
			return srv.SearchWmsOrder(dcCode, gupCode, custCode, scanCode);
		}

		/// <summary>
		/// 檢查F050801.SHIP_MODE是否在其他模式，可以包裝的話就將相關記錄寫檔
		/// </summary>
		/// <param name="f050801"></param>
		/// <param name="shipMode"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult StartPackageCheck(F050801 f050801, string shipMode)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P080701Service(wmsTransaction);
			var result = srv.StartPackageCheck(f050801, shipMode);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		/// <summary>
		/// 出貨包裝/缺貨包裝 刷讀紙箱、品號、序號
		/// </summary>
		/// <param name="f050801"></param>
		/// <param name="inputCode"></param>
		/// <param name="addQty"></param>
		/// <returns></returns>
		[OperationContract]
		public ScanPackageCodeResult ScanPackageCode(PackgeCode packgeCode)
		{
			ScanPackageCodeResult result = null;
			//var f2501Repo = new F2501Repository(Schemas.CoreSchema);
			// 避免多人同時刷讀品號/序號時，因同時產生F055001 PK 會重複，故自動重新嘗試一次刷讀
			try
			{
				result = CommitScanPackageCode(packgeCode);
			}
			catch
			{
				result = CommitScanPackageCode(packgeCode);
			}

			// 強制寫入Log，不包含在 Transaction 裡
			if (result != null && result.IsInsertLog)
			{
				var logService = new P080701Service();
				//var f2501 = f2501Repo.GetDataByBarCode(packgeCode.F050801Item.GUP_CODE, packgeCode.F050801Item.CUST_CODE, result.SerialNo);

        logService.LogF05500101(packgeCode.F050801Item.DC_CODE,
                                           packgeCode.F050801Item.GUP_CODE,
                                           packgeCode.F050801Item.CUST_CODE,
                                           packgeCode.F050801Item.WMS_ORD_NO,
                                           result.ItemCode,
                                           result.SerialNo,
                                           result.Status,
                                           result.IsPass ? "1" : "0",
                                           result.Message,
                                           packgeCode.PackageBoxNo,
                                           orgSerialWmsNo: result.OrgWmsNo,
                                           scanCode: packgeCode.InputCode);
      }

			return result;
		}

		private static ScanPackageCodeResult CommitScanPackageCode(PackgeCode packgeCode)
		{

			var wmsTransaction = new WmsTransaction();
			var srv = new P080701Service(wmsTransaction);
			var result = srv.ScanPackageCode(packgeCode);

			#region 塞入回傳結果需要的資訊，不用進DB

			result.SerialNo = result.SerialNo ?? string.Empty;
			result.Message = result.Message ?? string.Empty;

			// 若上次有刷過箱號，則與Client同步，避免刷讀品號失敗時，還能知道一開始刷讀的箱號
			if (string.IsNullOrEmpty(result.BoxNum) && !string.IsNullOrEmpty(packgeCode.BoxNum))
				result.BoxNum = packgeCode.BoxNum;

			// 但有刷讀NEWBOX，就將箱號清除
			if (packgeCode.InputCode != null && packgeCode.InputCode.Equals("NEWBOX", StringComparison.OrdinalIgnoreCase))
				result.BoxNum = string.Empty;
			#endregion 塞入回傳結果需要的資訊，不用進DB

			var stopWatch = new Stopwatch();
			stopWatch.Start();
			// 真的成功刷讀商品或新紙箱才寫入
			if (result.IsPass || result.IsCarton)
				wmsTransaction.Complete();
			stopWatch.Stop();
			TimeSpan ts = stopWatch.Elapsed;
			return result;
		}

		#endregion

		#region 退貨檢驗
		/// <summary>
		/// 匯入彙總表
		/// </summary>
		/// <param name="DataList"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult ImpoortP161502(List<F161502> DataList, string fileName = "")
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P080201Service(wmsTransaction);

			var result = srv.ImpoortP161502(DataList, fileName);

			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		/// <summary>
		/// 檢查序號狀態
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="returnNo"></param>
		/// <param name="serials"></param>
		/// <param name="validStatus"></param>
		/// <returns></returns>
		[OperationContract]
		public IQueryable<SerialDataEx> CheckSerials(string dcCode, string gupCode, string custCode, string returnNo, List<string> serials, string validStatus = "C1")
		{
			var srv = new P080201Service();
			var result = srv.CheckSerials(dcCode, gupCode, custCode, returnNo, serials, validStatus).AsQueryable();
			return result;
		}

		[OperationContract]
		public ExecuteResult InportUpdatePacking(string dcCode, string gupCode, string custCode, string returnNo, List<string> serials, string auditName, string auditStaff, string validStatus = "C1", string videoNo = null)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P080201Service(wmsTransaction);
			var result = srv.InportUpdatePacking(dcCode, gupCode, custCode, returnNo, serials, auditName, auditStaff, validStatus, videoNo);
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}
		/// <summary>
		/// 取得BarCode
		/// </summary>
		/// <param name="ordType"></param>
		/// <returns></returns>
		[OperationContract]
		public string GetPintBarCode(string ordType)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P080201Service(wmsTransaction);
			var result = srv.GetPintBarCode(ordType);
			if (result == null) return null;
			wmsTransaction.Complete();
			return result;
		}

		/// <summary>
		/// 新增 退貨檢驗刷驗紀錄檔
		/// </summary>
		/// <param name="f16140102"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult InsertF16140102(F16140102 f16140102)
		{
			var p080201Service = new P080201Service();
			return p080201Service.InsertF16140102(f16140102);
		}

		#endregion

		/// <summary>
		/// 更新派車單為結案
		/// </summary>
		/// <param name="f161201"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult UpdateF700101Status(F161201 f161201)
		{
			var wmsTransaction = new WmsTransaction();
			var p080201Service = new P080201Service(wmsTransaction);
			var result = p080201Service.UpdateF700101Status(f161201);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="f161201"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult UpdateF161201SourceNoStatus(F161201 f161201)
		{
			var wmsTransaction = new WmsTransaction();
			var sharedService = new SharedService(wmsTransaction);
			var result = sharedService.UpdateSourceNoStatus(SourceType.Return, f161201.DC_CODE, f161201.GUP_CODE, f161201.CUST_CODE, f161201.RETURN_NO, f161201.STATUS);
			if (result.IsSuccessed)
			{
				wmsTransaction.Complete();
			}
			return result;
		}

		/// <summary>
		/// 變更退貨檢驗的序號刷讀明細的ISPASS欄位
		/// </summary>
		/// <param name="f16140101Data"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult ChangeF16140101IsPass(F16140101 f16140101Data, string sourceNo)
		{
			var wmsTransaction = new WmsTransaction();
			var p080201Service = new P080201Service(wmsTransaction);
			var result = p080201Service.ChangeF16140101IsPass(f16140101Data, sourceNo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

    [OperationContract]
    public FinishCurrentBoxExecuteResult FinishCurrentBox(F050801 f050801, F055001 f055001, bool isCompletePackage, Boolean isManualCloseBox)
    {
      var wmsTransaction = new WmsTransaction();
      var p080701Service = new P080701Service(wmsTransaction);
      var result = p080701Service.FinishCurrentBox(ref f050801, ref f055001, isCompletePackage, isManualCloseBox);

      var finishCurrentBoxResult = AutoMapper.Mapper.DynamicMap<FinishCurrentBoxExecuteResult>(result);
      if (result.No == "LMS ERROR")
      {
        finishCurrentBoxResult.Message = "前一箱未正常關箱，請先手動執行關箱後再繼續作業";
        finishCurrentBoxResult.LMSMessage = result.Message;
      }
      if (finishCurrentBoxResult.IsSuccessed)
      {
        finishCurrentBoxResult.F050801Data = f050801;
        finishCurrentBoxResult.F055001Data = f055001;
        wmsTransaction.Complete();
      }

      return finishCurrentBoxResult;
    }

		[OperationContract]
		public ExecuteResult CheckF161401RepeatSerialNo(F161201 f161201, string barcode)
		{
			var p080201Service = new P080201Service();
			return p080201Service.CheckF161401RepeatSerialNo(f161201, barcode);
		}
		[OperationContract]
		public ExecuteResult LoadMasterDetailsData(string dcCode, string gupCode, string custCode, string returnNo)
		{
			var wmsTransaction = new WmsTransaction();
			var p080201Service = new P080201Service(wmsTransaction);
			var result = p080201Service.LoadMasterDetailsData(dcCode, gupCode, custCode, returnNo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		[OperationContract]
		public ExecuteResult CheckReturnBomItemNotFullReturn(string dcCode, string gupCode, string custCode, string returnNo)
		{
			var p080201Service = new P080201Service();
			var result = p080201Service.CheckReturnBomItemNotFullReturn(dcCode, gupCode, custCode, returnNo);
			return result;
		}

		[OperationContract]
		public ExecuteResult UpdateF16140201(string dcCode, string gupCode, string custCode, string returnNo, List<ReturnDetailSummary> datas)
		{
			var wmsTransaction = new WmsTransaction();
			var p080201Service = new P080201Service(wmsTransaction);
			var result = p080201Service.UpdateF16140201(dcCode, gupCode, custCode, returnNo, datas);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		[OperationContract]
		public SearchItemResult DoSearchItem(string dcCode, string gupCode, string custCode, string returnNo, string defaultLoc, string barCode, int? addAuditQty, string locCode, string cause, string memo)
		{
			var wmsTransaction = new WmsTransaction();
			var p080201Service = new P080201Service(wmsTransaction);
			var result = p080201Service.DoSearchItem(dcCode, gupCode, custCode, returnNo, defaultLoc, barCode, addAuditQty, locCode, cause, memo);
			if (result.IsPass == "1")
				wmsTransaction.Complete();
			return result;
		}
		[OperationContract]
		public ExecuteResult UpdateF161402(List<F161402Data> datas)
		{
			var wmsTransaction = new WmsTransaction();
			var p080201Service = new P080201Service(wmsTransaction);
			var result = p080201Service.UpdateF161402(datas);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		#region 裝車稽核
		[OperationContract]
		public ExecuteResult UploadDelvCheckCode(string dcCode, string gupCode, string custCode, List<UploadDelvCheckCode> delvCheckCodeList)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080802Service(wmsTransaction);
			var result = service.UploadDelvCheckCode(dcCode, gupCode, custCode, delvCheckCodeList);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		#endregion

		[OperationContract]
		public IQueryable<RetailDeliverReportDetail> GetRetailDeliverDetailReport(string dcCode, string gupCode, string custCode, string retailCode, List<string> wmsOrdNos)
		{
			var repo = new F1903Repository(Schemas.CoreSchema);
			return repo.GetRetailDeliverDetailReport(dcCode, gupCode, custCode, retailCode, wmsOrdNos);
		}

		[OperationContract]
		public IQueryable<RetailDeliverReportDetail> GetRetailDeliverDetailReportByRetail(string dcCode, string gupCode, string custCode, string retailCode, DateTime delvDate)
		{
			var repo = new F1903Repository(Schemas.CoreSchema);
			return repo.GetRetailDeliverDetailReportByRetail(dcCode, gupCode, custCode, retailCode, delvDate);
		}
		[OperationContract]
		public IQueryable<RetailDeliverReportDetail> GetRetailDeliverDetailReportByRetailIntact(string dcCode, string gupCode, string custCode, DateTime arrivalDate, string carPeriod, string delvNo, string retailCode)
		{
			var repo = new F1903Repository(Schemas.CoreSchema);
			return repo.GetRetailDeliverDetailReportByRetailIntact(dcCode, gupCode, custCode, arrivalDate, carPeriod, delvNo, retailCode);
		}




		#region P0806130000 單據工號綁定

		[OperationContract]
		public ExecuteResult UpdateP0806130000Data(string dcCode, string gupCode, string custCode, List<F0011BindData> f0011BD)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P080613Service(wmsTransaction);
			var result = srv.F0011Update(dcCode, gupCode, custCode, f0011BD.ToList());
			if (result.IsSuccessed == true) wmsTransaction.Complete();
			return result;
		}

        [OperationContract]
        public ExecuteResult CheckOrderNo(string dcCode, string gupCode, string custCode, string orderNo, string empId)
				{
					var wmsTransaction = new WmsTransaction();
					var p080613Service = new P080613Service(wmsTransaction);
					var result1 = p080613Service.CheckUserPermission(dcCode, gupCode, custCode, empId);
			    if (!result1.IsSuccessed)
				   return result1;

					var result2 = p080613Service.CheckOrderNo(dcCode, gupCode, custCode, orderNo, empId);
			    if(!result2.IsSuccessed)
							return result2;
					else
					{
				    var res = p080613Service.CheckIfAllOrdersCanceledByPickNo(dcCode,gupCode,custCode, orderNo);
						if (!res.IsSuccessed)
					      wmsTransaction.Complete();
								
						return res;
					}
        }

		[OperationContract]
		public ExecuteResult BindComplete(string dcCode, string gupCode, string custCode, List<F0011BindData> f0011BindDatas)
		{
			var wmsTransaction = new WmsTransaction();
			var p080613Service = new P080613Service(wmsTransaction);
			var result = p080613Service.BindComplete(dcCode, gupCode, custCode, f0011BindDatas);
			if (result.IsSuccessed || !string.IsNullOrEmpty(result.No))
				wmsTransaction.Complete();
			return result;
		}


		[OperationContract]
		public ExecuteResult AddF050305Data(string dcCode, string gupCode, string custCode, List<string> ordNos, string status)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new OrderService(wmsTransaction);
			srv.AddF050305(dcCode, gupCode, custCode, ordNos, status);
			wmsTransaction.Complete();
			return new ExecuteResult(true);
		}

		#endregion







		[OperationContract]
		public ExecuteResult P081301CheckTarLocCode(string dcCode, string locCode)
		{
			var service = new P081301Service();
			return service.CheckTarLocCode(dcCode, locCode);
		}
		[OperationContract]
		public ExecuteResult P081301CreateAllocation(string dcCode, string gupCode, string custCode, string tarLocCode, List<P08130101Stock> moveStocks)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P081301Service(wmsTransaction);
			var result = service.CreateAllocation(dcCode, gupCode, custCode, tarLocCode, moveStocks);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		[OperationContract]
		public ExecuteResult ApplyConsign(string dcCode, string gupCode, string custCode, string wmsOrdNo, short packageBoxNo)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new Shared.Lms.Services.ConsignService(wmsTransaction);
			var result = service.ApplyConsign(dcCode, gupCode, custCode, wmsOrdNo, packageBoxNo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		[OperationContract]
		public ExecuteResult CancelConsign(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new Shared.Lms.Services.ConsignService(wmsTransaction);
			var result = service.CancelConsign(dcCode, gupCode, custCode, wmsOrdNo);
			return result;
		}

		[OperationContract]
		public ExecuteResult UpdateF160204(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080701Service(wmsTransaction);
			var result = service.UpdateF160204(dcCode, gupCode, custCode, wmsOrdNo);
			return result;
		}



		[OperationContract]
		public ExecuteResult CheckPickAllotOrder(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			var service = new P080614Service();
			return service.CheckPickAllotOrder(dcCode, gupCode, custCode, pickOrdNo);
		}

		[OperationContract]
		public PickAllotData GetAndCreatePickAllotDatas(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080614Service(wmsTransaction);
			var data = service.GetAndCreatePickAllotDatas(dcCode, gupCode, custCode, pickOrdNo);
			wmsTransaction.Complete();
			return data;
		}

		[OperationContract]
		public ExecuteResult CheckContainer(string dcCode, string containerCode)
		{
			var service = new P080614Service();
			return service.CheckContainer(dcCode, containerCode);
		}

		[OperationContract]
		public List<ShipOrderPickAllot> BindContainerFinished(string dcCode, string gupCode, string custCode, string pickOrdNo, List<ShipOrderPickAllot> shipOrderPickAllots)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080614Service(wmsTransaction);
			var result = service.BindContainerFinished(dcCode, gupCode, custCode, pickOrdNo, shipOrderPickAllots);
			wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public PickAllotResult SowItem(string dcCode, string gupCode, string custCode, string pickOrdNo, string itemBarCode)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080614Service(wmsTransaction);
			var result = service.SowItem(dcCode, gupCode, custCode, pickOrdNo, itemBarCode);
			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}


		[OperationContract]
		public ExecuteResult InsertF050305(F050305 f050305)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080701Service(wmsTransaction);
			var result = service.InsertF050305(f050305);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		[OperationContract]
		public List<ShipOrderPickAllot> OutOfStocks(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080614Service(wmsTransaction);
			var result = service.OutOfStocks(dcCode, gupCode, custCode, pickOrdNo);
			wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public ScanContainerResult ScanContainerCode(string dcCode, string gupCode, string custCode, string containerCode)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080804Service(wmsTransaction);
			var result = service.ScanContainerCode(dcCode, gupCode, custCode, containerCode);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		[OperationContract]
		public BindBoxResult BindBox(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string moveOutTarget, string sowType, string newBoxNo, string orgBoxNo, bool isAddBox)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080804Service(wmsTransaction);
			var result = service.BindBox(dcCode, gupCode, custCode, delvDate, pickTime, moveOutTarget, sowType, newBoxNo, orgBoxNo, isAddBox);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		[OperationContract]
		public SowItemResult P080804SowItem(string dcCode, string gupCode, string custCode, string pickOrdNo, string containerCode, string itemBarcode, string normalBoxNo, string canelOrderBoxNo)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080804Service(wmsTransaction);
			var result = service.SowItem(dcCode, gupCode, custCode, pickOrdNo, containerCode, itemBarcode, normalBoxNo, canelOrderBoxNo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		[OperationContract]
		public LackItemResult GetPickLackItems(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			var service = new P080804Service();
			var result = service.GetPickLackItems(dcCode, gupCode, custCode, pickOrdNo);
			return result;
		}
		[OperationContract]
		public LackItemResult GetContainerLackItems(string dcCode, string gupCode, string custCode, string pickOrdNo, string containerCode)
		{
			var service = new P080804Service();
			var result = service.GetContainerLackItems(dcCode, gupCode, custCode, pickOrdNo, containerCode);
			return result;
		}
		[OperationContract]
		public ExecuteResult ContainerComplete(string dcCode, string gupCode, string custCode, string pickOrdNo, string containerCode)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080804Service(wmsTransaction);
			var result = service.ContainerComplete(dcCode, gupCode, custCode, pickOrdNo, containerCode);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		[OperationContract]
		public PickOutOfStockResult PickOutOfStockComfirm(string dcCode, string gupCode, string custCode, string pickOrdNo, string containerCode)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080804Service(wmsTransaction);
			var result = service.PickOutOfStockComfirm(dcCode, gupCode, custCode, pickOrdNo, containerCode);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public ExecuteResult DeleteEmpPickBind(int id)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080613Service(wmsTransaction);
			var result = service.Delete(id);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		#region P0814010000單人包裝站
		/// <summary>
		/// 包裝站開站/關站紀錄
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[OperationContract]
		public SetPackageStationStatusLogRes SetPackageStationStatusLog(SetPackageStationStatusLogReq req)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ShipPackageService(wmsTransaction);
			var result = service.SetPackageStationStatusLog(req);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		/// <summary>
		/// 出貨容器條碼檢核
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[OperationContract]
		public CheckShipContainerCodeRes CheckShipContainerCode(CheckShipContainerCodeReq req)
		{
			var service = new ShipPackageService();
			var result = service.CheckShipContainerCode(req);
			return result;
		}

		/// <summary>
		/// 查詢與檢核出貨單資訊
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[OperationContract]
		public SearchAndCheckWmsOrderInfoRes SearchAndCheckWmsOrderInfo(SearchAndCheckWmsOrderInfoReq req)
		{
			var service = new ShipPackageService(new WmsTransaction());
			return service.SearchAndCheckWmsOrderInfo(req);
		}

		/// <summary>
		/// 刷讀商品條碼
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[OperationContract]
		public ScanItemBarcodeRes ScanItemBarcode(ScanItemBarcodeReq req)
		{
			var service = new ShipPackageService(new WmsTransaction());
			return service.ScanItemBarcode(req);
		}

		/// <summary>
		/// 關箱處理
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[OperationContract]
		public CloseShipBoxRes CloseShipBox(CloseShipBoxReq req)
		{
			var service = new ShipPackageService(new WmsTransaction());
			return service.CloseShipBox(req);
		}

		/// <summary>
		/// 使用出貨單容器資料產生箱明細
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[OperationContract]
		public UseShipContainerToBoxDetailRes UseShipContainerToBoxDetail(UseShipContainerToBoxDetailReq req)
		{
			var service = new ShipPackageService(new WmsTransaction());
			return service.UseShipContainerToBoxDetail(req);
		}

		/// <summary>
		/// 取消包裝
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[OperationContract]
		public CancelShipOrderRes CancelShipOrder(CancelShipOrderReq req)
		{
			var service = new ShipPackageService(new WmsTransaction());
			return service.CancelShipOrder(req);
		}
		#endregion

		/// <summary>
		/// 呼叫[1.1.9 取得出貨單所有箱要列印報表清單]
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[OperationContract]
		public List<ShipPackageReportModel> SearchShipReportList(SearchShipReportListReq req)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ShipPackageService(wmsTransaction);
			var result = service.SearchShipReportList(req);
			wmsTransaction.Complete();
			return result;

		}

		/// <summary>
		/// 取得箱明細報表資料
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[OperationContract]
		public GetBoxDetailReportRes GetBoxDetailReport(GetBoxDetailReportReq req)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ShipPackageService();
			var result = service.GetBoxDetailReport(req);
			return result;
		}

		/// <summary>
		/// 取得一般出貨小白標報表資料
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[OperationContract]
		public GetShipLittleLabelReportRes GetShipLittleLabelReport(GetShipLittleLabelReportReq req)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ShipPackageService();
			var result = service.GetShipLittleLabelReport(req);
			return result;
		}

		/// <summary>
		/// 取得廠退出貨小白標報表資料
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[OperationContract]
		public GetRtnShipLittleLabelReportRes GetRtnShipLittleLabelReport(GetRtnShipLittleLabelReportReq req)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ShipPackageService();
			var result = service.GetRtnShipLittleLabelReport(req);
			return result;
		}

		/// <summary>
		/// 取得訂單出貨宅配單檔案
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[OperationContract]
		public GeShipFileRes GeShipFile(GeShipFileReq req)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ShipPackageService();
			var result = service.GeShipFile(req);
			return result;
		}

		/// <summary>
		/// 包裝線自動設備開站/暫停/關站
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[OperationContract]
		public SetPackageLineStationStatusRes SetPackageLineStationStatus(SetPackageLineStationStatusReq req)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ShipPackageService();
			var result = service.SetPackageLineStationStatus(req);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		/// <summary>
		/// 變更出貨單為所有商品都需過刷
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[OperationContract]
		public ChangeShipPackCheckRes ChangeShipPackCheck(ChangeShipPackCheckReq req)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ShipPackageService();
			var result = service.ChangeShipPackCheck(req);
			return result;
		}

		/// <summary>
		/// 取得包裝線工作站分配結果
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[OperationContract]
		public GetWorkStataionShipDataRes GetWorkStataionShipData(GetWorkStataionShipDataReq req)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ShipPackageService();
			var result = service.GetWorkStataionShipData(req);
			return result;
		}

		/// <summary>
		/// 取得出貨單容器資料
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		[OperationContract]
		public GetShipLogisticBoxRes GetShipLogisticBox(GetShipLogisticBoxReq req)
		{
			var service = new ShipPackageService();
			var result = service.GetShipLogisticBox(req);
			return result;
		}

		[OperationContract]
		public Boolean LogPausePacking(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var service = new ShipPackageService();
			service.LogF05500101(dcCode, gupCode, custCode, wmsOrdNo, null, null, null, "1", "暫停包裝", 0, null);
			return true;
		}

		[OperationContract]
		public Boolean UpdateShipReportList(string dcCode, string gupCode, string custCode, string wmsOrdNo, List<ShipPackageReportModel> shipPackageReportModels)
		{
			var service = new ShipPackageService();
			service.UpdateShipReportList(dcCode, gupCode, custCode, wmsOrdNo, shipPackageReportModels);
			return true;
		}

		#region 宅單扣帳
		[OperationContract]
    public HomeDeliveryOrderDebitResult HomeDeliveryOrderDebit(string dcCode, string gupCode, string custCode, string pastNo)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080803Service(wmsTransaction);
			var result = service.HomeDeliveryOrderDebit(dcCode, gupCode, custCode, pastNo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		#endregion

		/// <summary>
		/// 寫入訂單回檔記錄
		/// </summary>
		/// <param name="addF050305"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult InsertF050305Data(string dcCode, string gupCode, string custCode, string wmsOrdNo, string status, string procFlag, string WorkStationId)
		{
			var service = new ShipPackageService();
			service.InsertF050305Data(dcCode, gupCode, custCode, wmsOrdNo, status, procFlag, WorkStationId);
			return new ExecuteResult(true);
		}

		[OperationContract]
		public ExecuteResult CancelArrivalRecord(string dcCode, string gupCode, string custCode, string wmsNo, string containerCode = null)
		{
			var wmsTransation = new WmsTransaction();
			var service = new ShipPackageService(wmsTransation);
			var res = service.CancelArrivalRecord(dcCode, gupCode, custCode, wmsNo, containerCode);
			if (res.IsSuccessed)
				wmsTransation.Complete();
			return res;
		}

		#region P080805000
		[OperationContract]
		public PickContainerResult ScanPickContainerCode(string dcCode, string gupCode, string custCode, string containerCode)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080805Service(wmsTransaction);
			var result = service.ScanPickContainerCode(dcCode, gupCode, custCode, containerCode);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public OutContainerResult ScanOutContainerCode(string dcCode, string gupCode, string custCode, string containerCode)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080805Service(wmsTransaction);
			var result = service.ScanOutContainerCode(dcCode, gupCode, custCode, containerCode);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public PickContainerPutIntoOutContainerResult PickContainerPutIntoOutContainer(string dcCode, string gupCode, string custCode, PickContainerResult pickContainer, OutContainerResult outContainerResult)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080805Service(wmsTransaction);
			var result = service.PickContainerPutIntoOutContainer(dcCode, gupCode, custCode, pickContainer, outContainerResult);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public ExecuteResult RePackingBox(OutContainerInfo outContainerInfo)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080805Service(wmsTransaction);
			var result = service.RePackingBox(outContainerInfo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public ExecuteResult CloseBox(OutContainerInfo outContainerInfo)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080805Service(wmsTransaction);
			var result = service.CloseBox(outContainerInfo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		#endregion

		#region P080806000
		[OperationContract]
		public BindingPickContainerResult ScanBindingPickContainerCode(string dcCode, string gupCode, string custCode, string containerCode)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080806Service(wmsTransaction);
			var result = service.ScanBindingPickContainerCode(dcCode, gupCode, custCode, containerCode);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public OutContainerResult ScanNormalContainerCode(string dcCode, string gupCode, string custCode, string containerCode, BindingPickContainerInfo bindingPickContainerInfo)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080806Service(wmsTransaction);
			var result = service.ScanNormalContainerCode(dcCode, gupCode, custCode, containerCode, bindingPickContainerInfo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public OutContainerResult ScanCancelContainerCode(string dcCode, string gupCode, string custCode, string containerCode, BindingPickContainerInfo bindingPickContainerInfo)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080806Service(wmsTransaction);
			var result = service.ScanCancelContainerCode(dcCode, gupCode, custCode, containerCode, bindingPickContainerInfo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public ScanItemBarcodeResult ScanItemBarcodeFromP080806(string dcCode, string gupCode, string custCode, string itemBarcode,
														BindingPickContainerInfo bindingPickContainerInfo,
														OutContainerInfo normalContainer, OutContainerInfo cancelContainer)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080806Service(wmsTransaction);
			var result = service.ScanItemBarcodeFromP080806(dcCode, gupCode, custCode, itemBarcode, bindingPickContainerInfo, normalContainer, cancelContainer);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public IQueryable<F053601_NotAllotData> GetNotAllotDataInPickContainer(long f0701_Id)
		{
			var service = new P080806Service();
			var result = service.GetNotAllotDataInPickContainer(f0701_Id);
			return result;
		}

		[OperationContract]
		public ExecuteResult ManualContainerFinish(string dcCode, string gupCode, string custCode, BindingPickContainerInfo bindingPickContainerInfo)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080806Service(wmsTransaction);
			var result = service.ManualContainerFinish(dcCode, gupCode, custCode, bindingPickContainerInfo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public ExecuteResult CloseNormalContainer(OutContainerInfo containerInfo, BindingPickContainerInfo bindingPickContainerInfo)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080806Service(wmsTransaction);
			var result = service.CloseNormalContainer(containerInfo, bindingPickContainerInfo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public ExecuteResult CloseCancelContainer(OutContainerInfo containerInfo, BindingPickContainerInfo bindingPickContainerInfo)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080806Service(wmsTransaction);
			var result = service.CloseCancelContainer(containerInfo, bindingPickContainerInfo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public ExecuteResult RebindNormalContainer(string dcCode, string gupCode, string custCode, OutContainerInfo oriContainerInfo, string newContainerCode, BindingPickContainerInfo bindingPickContainerInfo)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080806Service(wmsTransaction);
			var result = service.RebindNormalContainer(dcCode, gupCode, custCode, oriContainerInfo, newContainerCode, bindingPickContainerInfo);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		[OperationContract]
		public ExecuteResult RebindCancelContainer(string dcCode, string gupCode, string custCode, OutContainerInfo oriContainerInfo, string newContainerCode)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080806Service(wmsTransaction);
			var result = service.RebindCancelContainer(dcCode, gupCode, custCode, oriContainerInfo, newContainerCode);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}
		[OperationContract]
		public ExecuteResult StopAllot(StopAllotParam param)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new P080806Service(wmsTransaction);
			var result = service.StopAllot(param);
			if (result.IsSuccessed)
				wmsTransaction.Complete();
			return result;
		}

		#endregion
	}
}
