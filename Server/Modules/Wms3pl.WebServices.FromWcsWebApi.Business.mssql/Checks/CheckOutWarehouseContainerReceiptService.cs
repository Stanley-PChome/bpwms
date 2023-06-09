
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.TransApiServices;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Checks
{
	public class CheckOutWarehouseContainerReceiptService
	{
		private TransApiBaseService tacService = new TransApiBaseService();

		/// <summary>
		/// 檢查明細數是否與裝箱明細數相同
		/// </summary>
		/// <param name="res"></param>
		/// <param name="receipt"></param>
		public void CheckSkuTotalEqualSkuListCount(List<ApiResponse> res, OutWarehouseContainerReceiptContainerModel receipt)
		{
			int reqTotal = receipt.SkuTotal != null ? Convert.ToInt32(receipt.SkuTotal) : 0;
			if (receipt.SkuTotal != receipt.SkuList.Count)
				res.Add(new ApiResponse { No = receipt.ContainerCode, MsgCode = "20022", MsgContent = string.Format(tacService.GetMsg("20022"), "裝箱明細", reqTotal, receipt.SkuList.Count) });
		}


		/// <summary>
		/// 檢查裝箱數量
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="containerCode"></param>
		/// <param name="index"></param>
		public void CheckSkuQty(List<ApiResponse> res, OutWarehouseContainerReceiptSkuModel sku, string containerCode, int index)
		{
			if (sku.SkuQty <= 0)
				res.Add(new ApiResponse { No = containerCode, ErrorColumn = "SkuQty", MsgCode = "20008", MsgContent = string.Format(tacService.GetMsg("20008"), containerCode,sku.OrderCode, index) });
		}

		/// <summary>
		/// 檢查出貨單號是否存在
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="receipt"></param>
		/// <param name="index"></param>
		/// <param name="f060201s"></param>
		/// <param name="f151002s"></param>
		/// <param name="f051202sByO"></param>
		public void CheckOrderCodeExist(List<ApiResponse> res, OutWarehouseContainerReceiptSkuModel sku, string dcCode,string gupCode, string custCode,string receipt, int index, List<F060201> f060201s, List<F151002> f151002s, List<F051202> f051202sByO,List<F051203> f051203ListByP)
		{
			// 檢查單號[ORDERCODE]是否存在F060201，若不存在則回傳失敗訊息23001(單據不存在)
			if (!f060201s.Any(x=>x.WMS_NO == sku.OrderCode))
			{
				// 不存在F060201
				res.Add(new ApiResponse { No = receipt,ErrorColumn = "OrderCode", MsgCode = "23001", MsgContent = string.Format(tacService.GetMsg("23001"), sku.OrderCode) });
			}
			else
			{
				// 存在F060201
				var f060201 = f060201s.Where(x => x.DOC_ID == sku.OrderCode).FirstOrDefault();
				var wmsNo = f060201.WMS_NO;
				var currF151002 = f151002s.Where(x => x.ALLOCATION_NO == wmsNo && x.ALLOCATION_SEQ == sku.RowNum).FirstOrDefault();
				var currF051202ByO = f051202sByO.Where(x => x.PICK_ORD_NO == f060201.PICK_NO && x.WMS_ORD_NO == wmsNo && x.PICK_ORD_SEQ == Convert.ToString(sku.RowNum).PadLeft(4, '0')).FirstOrDefault();
				var currF051203ByP = f051203ListByP.Where(x => x.PICK_ORD_NO == wmsNo && x.TTL_PICK_SEQ == Convert.ToString(sku.RowNum).PadLeft(4, '0')).FirstOrDefault();
				// 檢查ROWNUM 跟SEQ是否一致，若不同則回傳失敗訊息23005(單據項次有誤
				CheckRowNumIsExist(res, sku, wmsNo, receipt, index, currF151002, currF051202ByO, currF051203ByP);
			}
		}

		/// <summary>
		/// 檢查ROWNUM 跟SEQ是否一致
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		/// <param name="f151002"></param>
		/// <param name="f051202ByO"></param>
		public void CheckRowNumIsExist(List<ApiResponse> res, OutWarehouseContainerReceiptSkuModel sku, string wmsNo, string receipt, int index, F151002 f151002, F051202 f051202ByO,F051203 f051203ByP)
		{
			bool rowNumIsExist = true;
			if (wmsNo.StartsWith("T") && f151002 == null)
				rowNumIsExist = false;
			else if (wmsNo.StartsWith("O") && f051202ByO == null)
				rowNumIsExist = false;
			else if (wmsNo.StartsWith("P") && f051203ByP == null)
				rowNumIsExist = false;


			if (!rowNumIsExist)
				res.Add(new ApiResponse { No = receipt, ErrorColumn = "RowNum", MsgCode = "23057", MsgContent = string.Format(tacService.GetMsg("23057"), sku.OrderCode) });
		}

		/// <summary>
		/// 檢查商品序號清單數量是否大於8000
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		public void CheckSkuSerialNum(List<ApiResponse> res, OutWarehouseContainerReceiptSkuModel sku, string receiptCode, int index)
		{
			if (sku.SerialNumList != null && sku.SerialNumList.Any())
			{
				bool serialNumIsExceed = string.Join(",", sku.SerialNumList).Length > 8000;
				if (serialNumIsExceed)
					res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SerialNumList", MsgCode = "20044", MsgContent = string.Format(tacService.GetMsg("20044"), sku.OrderCode) });
			}
		}

		/// <summary>
		/// 檢查商品序號清單是否存在F2501
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		/// <param name="serialNoList"></param>
		public void CheckSkuSerialNumIsExist(List<ApiResponse> res, OutWarehouseContainerReceiptSkuModel sku, string receiptCode, int index, List<string> serialNoList)
		{
			if (sku.SerialNumList != null && sku.SerialNumList.Any())
			{
				var data = serialNoList.Where(serialNo => sku.SerialNumList.Contains(serialNo));
				if (data.Count() != sku.SerialNumList.Count)
					res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SerialNumList", MsgCode = "20042", MsgContent = string.Format(tacService.GetMsg("20042"), $"{receiptCode}第{index + 1}筆明細") });
			}
		}

		/// <summary>
		/// 檢查該明細序號長度是否與裝箱數量相同
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		public void CheckSkuSerialNumEquelSkuQty(List<ApiResponse> res, OutWarehouseContainerReceiptSkuModel sku, string receiptCode, int index, List<string> serialNoList)
		{
			if (sku.SerialNumList != null && sku.SerialNumList.Any() && sku.SkuQty != sku.SerialNumList.Count)
				res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SerialNumList", MsgCode = "20043", MsgContent = string.Format(tacService.GetMsg("20043"), $"{receiptCode}第{index + 1}筆明細") });
		}
	}
}