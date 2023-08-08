using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.Wcssr.Services;

namespace Wms3pl.WebServices.Process.P16.Services
{
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public partial class P16WcfService
	{
		[OperationContract]
		public ExecuteResult InsertP160101(F161201 addF161201, F161202[] addF161202s)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P160101Service(wmsTransaction);
			var result = srv.InsertP160101(addF161201, addF161202s);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult UpdateP160101(F161201 editF161201, F161202[] editF161202s)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P160101Service(wmsTransaction);
			var result = srv.UpdateP160101(editF161201, editF161202s, isP160101: true);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult InsertF160201(F160201 addF160201, F160202[] addF161202s)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P160201Service(wmsTransaction);
			var result = srv.InsertF160201(addF160201, addF161202s);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult UpdateF160201(F160201 editF160201, F160202[] editF160202s)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P160201Service(wmsTransaction);
			var result = srv.UpdateF160201(editF160201, editF160202s);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult InsertF160204( F160204[] f160204s)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P160202Service(wmsTransaction);
			var result = srv.InsertF160204(f160204s);
			string resultString1 = String.Empty;

			if (result.IsSuccessed)
			{
				resultString1 = result.Message;
				result = srv.UpdateF160202ByF160204(resultString1, f160204s);

				if (result.IsSuccessed)
				{
					wmsTransaction.Complete();
				}
			}

			return result;
		}

		[OperationContract]
		public ExecuteResult InsertP160102(F161601 addF161601, F161602[] addF161602s)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P160102Service(wmsTransaction);
			var result = srv.InsertP160102(addF161601, addF161602s);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult UpdateP160102(F161601 editF161601, F161602[] editF161602s)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P160102Service(wmsTransaction);
			var result = srv.UpdateP160102(editF161601, editF161602s);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}


		//新增銷毀單據
		[OperationContract]
		public ExecuteResult InsertF160501s(F160501 f160501, List<F160502Data> detailData, List<F160502Data> serialData)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P160501Service(wmsTransaction);
			var result = srv.InsertF160501s(f160501, detailData, serialData);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		//編輯銷毀單據
		[OperationContract]
		public ExecuteResult UpdateF160501s(F160501 f160501, List<F160502Data> detailData, List<F160502Data> serialData)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P160501Service(wmsTransaction);
			var result = srv.UpdateF160501s(f160501, detailData, serialData);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		//刪除銷毀單據
		[OperationContract]
		public ExecuteResult DeleteF160501s(F160501 f160501)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P160501Service(wmsTransaction);
			var result = srv.DeleteF160501s(f160501);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public ExecuteResult P160501Shipment(F160501Data f160501Data)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P160501Service(wmsTransaction);
			var result = srv.P160501Shipment(f160501Data);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		#region 退貨單
		[OperationContract]
		public ExecuteResult InsertF161201(F161201 addF161201, F161202[] addF161202s)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P160101Service(wmsTransaction);
			var result = srv.InsertP160101(addF161201, addF161202s);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion

		#region 報廢單維護
		[OperationContract]
		public ExecuteResult SaveScrapDetails(F160401 f160401, List<F160402> f160402s)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P160402Service(wmsTransaction);
			var result = srv.SaveScrapDetails(f160401, f160402s);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public decimal GetF160402ScrapSum(string dcCode, string gupCode, string custCode, string scrapNo, string itemCode, string locCode, string wareHouseId)
		{
			var srv = new P160402Service();
			var result = srv.GetF160402ScrapSum(dcCode, gupCode, custCode, scrapNo, itemCode, locCode, wareHouseId);
			return result;
		}

        [OperationContract]
        public IQueryable<F160402> GetF160402ScrapData(string dcCode, string gupCode, string custCode, string scrapNo, string itemCode, string locCode, string wareHouseId)
        {
            var srv = new P160402Service();
            return srv.GetF160402ScrapData(dcCode, gupCode, custCode, scrapNo, itemCode, locCode, wareHouseId);
        }

        [OperationContract]
		public ExecuteResult ApproveScrapDetails(F160401 f160401, List<F160402> f160402s, List<SrcItemLocQtyItem> srcItemLocQtyItems)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P160402Service(wmsTransaction);
			var result = srv.ApproveScrapDetails(f160401, f160402s, srcItemLocQtyItems);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}
		#endregion


		//銷毀上傳
		[OperationContract]
		public ExecuteResult UpdateF160503(List<F160501FileData> serialData, List<F160501FileData> fileData, List<F160501FileData> fileDeleteData)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P160503Service(wmsTransaction);
			var result = srv.UpdateF160503(serialData, fileData, fileDeleteData);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}


		//Import 退貨資料
		[OperationContract]
		public ExecuteResult ImportF1612Data(string dcCode, string gupCode, string custCode
											, string fileName, List<F1612ImportData> importData)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P160101Service(wmsTransaction);
			var result = srv.ImportF1612Data(dcCode, gupCode, custCode, fileName, importData);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}

		[OperationContract]
		public IQueryable<ExecuteResult> CheckItemLocTmpr(string dcCode, string gupCode, string custCode, string warehouseId, List<F161601DetailDatas> checkDatas)
		{
			var sharedService = new SharedService();
			var itemDatas = checkDatas.Select(o => o.ITEM_CODE).ToList();
			var message = sharedService.CheckItemLocTmprByWareHouse(dcCode, gupCode, custCode, itemDatas, warehouseId);

			if (!string.IsNullOrWhiteSpace(message))
				return new List<ExecuteResult> { new ExecuteResult { IsSuccessed = false, Message = message } }.AsQueryable();
			return new List<ExecuteResult> { new ExecuteResult { IsSuccessed = true, Message = string.Empty } }.AsQueryable();
		}

		[OperationContract]
		public ExecuteResult ApplyVendorReturnOrder(string dcCode, string gupCode, string custCode, string rtnWmsNo,string deliveryWay,string allId,string sheetNum,string memo)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ForeignWebApi.Business.LmsServices.VnrReturnConsignService(wmsTransaction);
			var result = service.ApplyVendorReturnOrder(dcCode, gupCode, custCode, rtnWmsNo, deliveryWay,allId, sheetNum, memo);
			return result;
		}

		[OperationContract]
		public ExecuteLmsPdfApiResult GetVendorReturnOrderFile(string dcCode, string gupCode, string custCode, string order_no, int rePrint, int? sno)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ForeignWebApi.Business.LmsServices.VnrReturnConsignService(wmsTransaction);
			var result = service.GetVendorReturnOrderFile(dcCode, gupCode, custCode, order_no, rePrint, sno);
			return result;
		}

		[OperationContract]
		public ExecuteLmsPdfApiResult GetVendorReturnOrderDetailFile(string dcCode, string gupCode, string custCode, string order_no, int rePrint, int? sno)
		{
			var wmsTransaction = new WmsTransaction();
			var service = new ForeignWebApi.Business.LmsServices.VnrReturnConsignService(wmsTransaction);
			var result = service.GetVendorReturnOrderDetailFile(dcCode, gupCode, custCode, order_no, rePrint, sno);
			return result;
		}

		[OperationContract]
		public ExecuteResult DelF075103s(string custCode,string custOrdCode)
		{
			var wmsTransaction = new WmsTransaction();
			var srv = new P160201Service(wmsTransaction);
			var result = srv.DelF075103s(custCode, custOrdCode);

			if (result.IsSuccessed)
				wmsTransaction.Complete();

			return result;
		}



		/// <summary>
		/// 配箱資訊同步
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="req"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult DistibuteInfoAsync(string dcCode, string gupCode, string custCode, DistibuteInfoAsyncReq req)
		{
			var result = new ExecuteResult(true);
			var wmsTransaction = new WmsTransaction();
			var service = new DistibuteService(wmsTransaction);
			var apiResult = service.DistibuteInfoAsync(dcCode, gupCode, custCode, req);

			var resultData = JsonConvert.DeserializeObject<DistibuteInfoAsyncRes>(JsonConvert.SerializeObject(apiResult.Data));

			if (apiResult.IsSuccessed)
				wmsTransaction.Complete();
			else
			{
				result.IsSuccessed = apiResult.IsSuccessed;
				result.Message = apiResult.MsgContent; // 連線失敗錯誤訊息

				//若連線成功但有錯誤顯示錯誤訊息
				var errorMsg = "";
				var errorColumn = "";
				if (resultData?.ErrorData != null)
				{
					resultData.ErrorData.ForEach(x => {
						errorMsg += string.IsNullOrWhiteSpace(errorMsg) ? x.ErrorMsg : $",{x.ErrorMsg}";
						if(x.ErrorColumn != null)
						{
							x.ErrorColumn = x.ErrorColumn.Where(y => !string.IsNullOrWhiteSpace(y)).ToList(); // 排除List<string>內有null或空白
							errorColumn += string.IsNullOrWhiteSpace(errorColumn) ? String.Join(",", x.ErrorColumn) : $",{ String.Join(",", x.ErrorColumn)}";
						}
					});
					result.Message = $"WCSSR回覆:{errorMsg} {errorColumn}";
				}
			}
			return result;
		}

		/// <summary>
		/// 封箱資訊同步
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="req"></param>
		/// <returns></returns>
		[OperationContract]
		public ExecuteResult SealingInfoAsync(string dcCode, string gupCode, string custCode, SealingInfoAsyncReq req)
		{
			var result = new ExecuteResult(true);
			var wmsTransaction = new WmsTransaction();
			var service = new DistibuteService(wmsTransaction);
			var apiResult = service.SealingInfoAsync(dcCode, gupCode, custCode, req);


			var resultData = JsonConvert.DeserializeObject<SealingInfoAsyncRes>(JsonConvert.SerializeObject(apiResult.Data));
			if (apiResult.IsSuccessed)
			{
				wmsTransaction.Complete();
			}
			else
			{
				result.IsSuccessed = apiResult.IsSuccessed;
				result.Message = apiResult.MsgContent; // 連線失敗錯誤訊息

				//若連線成功但有錯誤顯示錯誤訊息
				var errorMsg = "";
				var errorColumn = "";
				if (resultData?.ErrorData != null)
				{
					resultData.ErrorData.ForEach(x => {
						errorMsg += string.IsNullOrWhiteSpace(errorMsg) ? x.ErrorMsg : $",{x.ErrorMsg}";
						if (x.ErrorColumn != null)
						{
							x.ErrorColumn = x.ErrorColumn.Where(y => !string.IsNullOrWhiteSpace(y)).ToList(); // 排除List<string>內有null或空白
							errorColumn += string.IsNullOrWhiteSpace(errorColumn) ? String.Join(",", x.ErrorColumn) : $",{ String.Join(",", x.ErrorColumn)}";
						}
					});
					result.Message = $"WCSSR回覆:{errorMsg} {errorColumn}";
				}
			}

			return result;
		}
	}
}
