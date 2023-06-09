using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.TransApiServices;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Checks
{
	public class CheckInWarehouseReceiptService
	{
		private TransApiBaseService tacService = new TransApiBaseService();

		#region 入庫單檢核
		/// <summary>
		/// 檢查[入庫單狀態]是否設定錯誤
		/// </summary>
		/// <param name="res"></param>
		/// <param name="receipt"></param>
		public void CheckStatus(List<ApiResponse> res, InWarehouseReceiptModel receipt)
		{
			List<int> status = new List<int> { 0, 4, 9 };
			if (!status.Contains(Convert.ToInt32(receipt.Status)))
				res.Add(new ApiResponse { No = receipt.ReceiptCode, ErrorColumn = "Status", MsgCode = "23051", MsgContent = string.Format(tacService.GetMsg("23051"), receipt.ReceiptCode, "入庫單狀態") });
		}

		/// <summary>
		/// 檢查[是否異常]是否設定錯誤
		/// </summary>
		/// <param name="res"></param>
		/// <param name="receipt"></param>
		public void CheckIsException(List<ApiResponse> res, InWarehouseReceiptModel receipt)
		{
			List<int> isExceptions = new List<int> { 0, 1 };
			if (!isExceptions.Contains(Convert.ToInt32(receipt.IsException)))
				res.Add(new ApiResponse { No = receipt.ReceiptCode, ErrorColumn = "IsException", MsgCode = "23051", MsgContent = string.Format(tacService.GetMsg("23051"), receipt.ReceiptCode, "是否異常") });
		}

		/// <summary>
		/// 檢查品項數與明細筆數是否相同
		/// </summary>
		/// <param name="res"></param>
		/// <param name="receipt"></param>
		public void CheckSkuTotalEqualDetailCnt(List<ApiResponse> res, InWarehouseReceiptModel receipt)
		{
			if (receipt.SkuTotal != receipt.SkuList.Count)
				res.Add(new ApiResponse { No = receipt.ReceiptCode, ErrorColumn = "SkuTotal", MsgCode = "23058", MsgContent = string.Format(tacService.GetMsg("23058"), receipt.ReceiptCode, receipt.SkuTotal, receipt.SkuList.Count) });
		}

		/// <summary>
		/// 檢查單號狀態
		/// </summary>
		/// <param name="res"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		/// <param name="f151001s"></param>
		public void CheckReceiptCodeData(List<ApiResponse> res, string wmsNo, InWarehouseReceiptModel receipt, List<F151001> f151001s)
		{
			List<string> status = new List<string> { "3", "4" };
			var f151001 = f151001s.Where(x => x.ALLOCATION_NO == wmsNo).FirstOrDefault();

			if (f151001 == null)
				res.Add(new ApiResponse { No = receipt.ReceiptCode, MsgCode = "20962", MsgContent = string.Format(tacService.GetMsg("20962"), receipt.ReceiptCode) });
			else
			{
				if (f151001.STATUS == "9")
					res.Add(new ApiResponse { No = receipt.ReceiptCode, MsgCode = "23052", MsgContent = string.Format(tacService.GetMsg("23052"), receipt.ReceiptCode) });
				else if (f151001.STATUS == "5")
					res.Add(new ApiResponse { No = receipt.ReceiptCode, MsgCode = "23053", MsgContent = string.Format(tacService.GetMsg("23053"), receipt.ReceiptCode) });
				else if (!status.Contains(f151001.STATUS))
					res.Add(new ApiResponse { No = receipt.ReceiptCode, MsgCode = "23054", MsgContent = string.Format(tacService.GetMsg("23054"), receipt.ReceiptCode) });
			}
		}

		/// <summary>
		/// 檢查ROWNUM 跟F151002.SEQ是否一致
		/// </summary>
		/// <param name="res"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		/// <param name="f151002s"></param>
		public void CheckRowNumEqualSeq(List<ApiResponse> res, string wmsNo, InWarehouseReceiptModel receipt, List<F151002> f151002s)
		{
      var seqs = f151002s.Where(x => x.ALLOCATION_NO == wmsNo && x.STATUS == "1").Select(x => Convert.ToInt32(x.ALLOCATION_SEQ)).ToList();

      var rowNums = receipt.SkuList.Select(x => Convert.ToInt32(x.RowNum)).ToList();

			if (rowNums.Except(seqs).Any() || seqs.Except(rowNums).Any())
				res.Add(new ApiResponse { No = receipt.ReceiptCode, ErrorColumn = "RowNum", MsgCode = "23057", MsgContent = string.Format(tacService.GetMsg("23057"), receipt.ReceiptCode) });
		}

		/// <summary>
		/// 檢查SkuCode 跟F151002.ITEM_CODE是否一致
		/// </summary>
		/// <param name="res"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		/// <param name="f151002s"></param>
		public void CheckSkuCodeEqualItemCode(List<ApiResponse> res, string wmsNo, InWarehouseReceiptModel receipt, List<F151002> f151002s)
		{
      var seqs = f151002s.Where(x => x.ALLOCATION_NO == wmsNo && x.STATUS == "1").Select(x => new { RowNum = Convert.ToInt32(x.ALLOCATION_SEQ), ItemCode = x.ITEM_CODE }).ToList();

			var rowNums = receipt.SkuList.Select(x => new { RowNum = Convert.ToInt32(x.RowNum), ItemCode = x.SkuCode }).ToList();

			if (rowNums.Except(seqs).Any() || seqs.Except(rowNums).Any())
				res.Add(new ApiResponse { No = receipt.ReceiptCode, ErrorColumn = "SkuCode", MsgCode = "23059", MsgContent = string.Format(tacService.GetMsg("23059"), receipt.ReceiptCode) });
		}
		#endregion

		#region 入庫明細檢核
		/// <summary>
		/// 檢查[入庫標示]是否設定錯誤
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		public void CheckReceiptFlag(List<ApiResponse> res, InWarehouseReceiptSkuModel sku, string receiptCode, int index)
		{
			List<int> receiptFlags = new List<int> { 0, 1, 2 };
			if (!receiptFlags.Contains(Convert.ToInt32(sku.ReceiptFlag)))
				res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "ReceiptFlag", MsgCode = "23051", MsgContent = string.Format(tacService.GetMsg("23051"), $"{receiptCode}第{index + 1}筆明細", "入庫標示") });
		}

		/// <summary>
		/// 檢查[商品等級]是否設定錯誤
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		public void CheckSkuLevel(List<ApiResponse> res, InWarehouseReceiptSkuModel sku, string receiptCode, int index)
		{
			List<int> skuLeves = new List<int> { 0, 1 };
			if (!skuLeves.Contains(Convert.ToInt32(sku.SkuLevel)))
				res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SkuLevel", MsgCode = "23051", MsgContent = string.Format(tacService.GetMsg("23051"), $"{receiptCode}第{index + 1}筆明細", "商品等級") });
		}

		/// <summary>
		/// 檢查[實際入庫數量]是否有大於0
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		public void CheckSkuQty(List<ApiResponse> res, InWarehouseReceiptSkuModel sku, string receiptCode, int index)
		{
			if (sku.SkuQty <= 0)
				res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SkuQty", MsgCode = "23056", MsgContent = string.Format(tacService.GetMsg("23056"), $"{receiptCode}第{index + 1}筆明細") });
		}

		/// <summary>
		/// 檢查[實際入庫數量]是否大於[預計入庫量]
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		public void CheckSkuQtyAndSkuPlanQty(List<ApiResponse> res, InWarehouseReceiptSkuModel sku, string receiptCode, int index)
		{
			if (sku.SkuQty > sku.SkuPlanQty)
			{
				res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SkuQty", MsgCode = "23055", MsgContent = string.Format(tacService.GetMsg("23055"), $"{receiptCode}第{index + 1}筆明細", "實際入庫數量", "預計入庫數量") });
				res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SkuPlanQty", MsgCode = "23055", MsgContent = string.Format(tacService.GetMsg("23055"), $"{receiptCode}第{index + 1}筆明細", "實際入庫數量", "預計入庫數量") });
			}
		}

		/// <summary>
		/// 檢查[實際入庫數量]、[預計入庫數量]是否超過調撥單上架數
		/// </summary>
		/// <param name="res"></param>
		/// <param name="wmsNo"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		/// <param name="f151002s"></param>
		public void CheckSkuQtyAndSkuPlanQtyExceedTarQty(List<ApiResponse> res, string wmsNo, InWarehouseReceiptSkuModel sku, string receiptCode, int index, List<F151002> f151002s)
		{
			var f151002 = f151002s.Where(x => x.ALLOCATION_NO == wmsNo && x.ALLOCATION_SEQ == sku.RowNum).FirstOrDefault();

			if (f151002 != null)
			{
				if (sku.SkuQty > f151002.TAR_QTY)
					res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SkuQty", MsgCode = "23060", MsgContent = string.Format(tacService.GetMsg("23060"), $"{receiptCode}第{index + 1}筆明細", "實際入庫數量") });
				if (sku.SkuPlanQty > f151002.TAR_QTY)
					res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SkuPlanQty", MsgCode = "23060", MsgContent = string.Format(tacService.GetMsg("23060"), $"{receiptCode}第{index + 1}筆明細", "預計入庫數量") });
			}
		}
		#endregion

		public bool CheckDocExist(List<ApiResponse> res, InWarehouseReceiptModel receipt, string dcCode)
		{
			var isAddF075105 = false;
			var f075105Repo = new F075105Repository(Schemas.CoreSchema);
			#region 新增入庫任務單號回檔紀錄檔
			var f075105Res = f075105Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
				new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
			() =>
			{
				var lockF075105 = f075105Repo.LockF075105();
				var f075105 = f075105Repo.Find(o => o.DC_CODE == dcCode && o.DOC_ID == receipt.ReceiptCode, isForUpdate: true, isByCache: false);
				if (f075105 == null)
				{
					f075105 = new F075105 { DC_CODE = dcCode, DOC_ID = receipt.ReceiptCode };
					f075105Repo.Add(f075105);
					isAddF075105 = true;
				}
				else
				{
					f075105 = null; // 代表F075105已存在資料
				}
				return f075105;
			});
			if (f075105Res == null)// 代表F075105已存在資料
				res.Add(new ApiResponse { No = receipt.ReceiptCode, MsgCode = "20964", MsgContent = string.Format(tacService.GetMsg("20964"), receipt.ReceiptCode) });
			#endregion

			return isAddF075105;
		}
	}
}
