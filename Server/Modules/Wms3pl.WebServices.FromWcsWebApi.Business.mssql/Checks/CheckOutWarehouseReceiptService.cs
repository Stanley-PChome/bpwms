
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.TransApiServices;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Checks
{
	public class CheckOutWarehouseReceiptService
	{
    private TransApiBaseService _TacService;
    public TransApiBaseService TacService
    {
      get { return _TacService == null ? _TacService = new TransApiBaseService() : _TacService; }
      set { _TacService = value; }
    }

		#region 出庫單檢核
		/// <summary>
		/// 檢查單號是否存在、是否已取消、是否已結案
		/// </summary>
		/// <param name="res"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		/// <param name="_f151001s"></param>
		/// <param name="_f050801s"></param>
		/// <param name="_f051201s"></param>
		public void CheckReceiptOrderExistAndStatus(List<ApiResponse> res, string wmsNo, OutWarehouseReceiptModel receipt, List<F151001> _f151001s, List<F050801> _f050801s, List<F051201> _f051201s)
		{
			bool isExist = false;   // 是否存在
			bool isCancel = false;  // 是否已取消
			bool isClose = false;   // 是否已結案

			if (wmsNo.StartsWith("T"))
			{
				var f151001 = _f151001s.Where(x => x.ALLOCATION_NO == wmsNo).FirstOrDefault();
				if (f151001 != null)
				{
					isExist = true;
					if (receipt.Status != 9)
					{
						List<string> f151001Status = new List<string> { "1", "2" };

						if (f151001.STATUS == "9")
							isCancel = true;
						else if (f151001.STATUS == "5")
							isClose = true;
						else if (!f151001Status.Contains(f151001.STATUS)) // 調撥單還需再檢核是否為下架單
							res.Add(new ApiResponse { No = receipt.OrderCode, MsgCode = "20048", MsgContent = string.Format(TacService.GetMsg("20048"), receipt.OrderCode) });
					}
				}
			}
			else if (wmsNo.StartsWith("O"))
			{
				var f050801 = _f050801s.Where(x => x.WMS_ORD_NO == wmsNo).FirstOrDefault();
				if (f050801 != null)
				{
					isExist = true;
					if (receipt.Status != 9)
					{
						if (f050801.STATUS == 5)
							isClose = true;
					}
				}
			}
			else if (wmsNo.StartsWith("P"))
			{
				var f051201 = _f051201s.Where(x => x.PICK_ORD_NO == wmsNo).FirstOrDefault();
				if (f051201 != null)
				{
					isExist = true;
					if (receipt.Status != 9)
					{
						if (f051201.PICK_STATUS == 2)
							isClose = true;
					}
				}
			}

			// 單據不存在
			if (!isExist)
				res.Add(new ApiResponse { No = receipt.OrderCode, MsgCode = "20962", MsgContent = string.Format(TacService.GetMsg("20962"), receipt.OrderCode) });

			// 單據已刪除
			if (isCancel)
				res.Add(new ApiResponse { No = receipt.OrderCode, MsgCode = "23052", MsgContent = string.Format(TacService.GetMsg("23052"), receipt.OrderCode) });

			// 單據已結案
			if (isClose)
				res.Add(new ApiResponse { No = receipt.OrderCode, MsgCode = "23053", MsgContent = string.Format(TacService.GetMsg("23053"), receipt.OrderCode) });
		}

		/// <summary>
		/// 檢查[出庫單狀態]是否設定錯誤
		/// </summary>
		/// <param name="res"></param>
		/// <param name="receipt"></param>
		public void CheckStatus(List<ApiResponse> res, OutWarehouseReceiptModel receipt)
		{
			List<int> status = new List<int> { 0, 3, 9 };
			if (!status.Contains(Convert.ToInt32(receipt.Status)))
				res.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = "Status", MsgCode = "23051", MsgContent = string.Format(TacService.GetMsg("23051"), receipt.OrderCode, "出庫單狀態") });
		}

		/// <summary>
		/// 檢查[是否異常]是否設定錯誤
		/// </summary>
		/// <param name="res"></param>
		/// <param name="receipt"></param>
		public void CheckIsException(List<ApiResponse> res, OutWarehouseReceiptModel receipt)
		{
			List<int> isExceptions = new List<int> { 0, 1, 2 };
			if (!isExceptions.Contains(Convert.ToInt32(receipt.IsException)))
				res.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = "IsException", MsgCode = "23051", MsgContent = string.Format(TacService.GetMsg("23051"), receipt.OrderCode, "是否異常") });
		}

		/// <summary>
		/// 檢查品項數與明細筆數是否相同
		/// </summary>
		/// <param name="res"></param>
		/// <param name="receipt"></param>
		public void CheckSkuTotalEqualDetailCnt(List<ApiResponse> res, OutWarehouseReceiptModel receipt)
		{
			if (receipt.SkuTotal != receipt.SkuList.Count)
				res.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = "SkuTotal", MsgCode = "23058", MsgContent = string.Format(TacService.GetMsg("23058"), receipt.OrderCode, receipt.SkuTotal, receipt.SkuList.Count) });
		}

		/// <summary>
		/// 檢查明細資料是否有誤
		/// </summary>
		/// <param name="res"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		/// <param name="f151002s"></param>
		/// <param name="f051202sByO"></param>
		/// <param name="f051203sByP"></param>
		public void CheckSkuCountEqualDatas(List<ApiResponse> res, string wmsNo, OutWarehouseReceiptModel receipt, List<F151002> f151002s, List<F051202> f051202sByO, List<F051203> f051203sByP)
		{
			bool isPass = true;
			if (receipt.SkuList.Any())
			{
				if (wmsNo.StartsWith("T"))
				{
					var currF151002s = f151002s.Where(x => x.ALLOCATION_NO == wmsNo);
					var rowNums = receipt.SkuList.Select(x => new { ItemCode = x.SkuCode, Seq = Convert.ToInt32(x.RowNum) }).ToList();
					var seqs = currF151002s.Select(x => new { ItemCode = x.ITEM_CODE, Seq = Convert.ToInt32(x.ALLOCATION_SEQ) }).ToList();
					if (rowNums.Except(seqs).Any())
						isPass = false;
				}
				else if (wmsNo.StartsWith("O") || wmsNo.StartsWith("P"))
				{
					var rowNums = receipt.SkuList.Select(x => new { ItemCode = x.SkuCode, Seq = Convert.ToString(x.RowNum).PadLeft(4, '0') }).ToList();
					if (wmsNo.StartsWith("O"))
					{
						var currF051202s = f051202sByO.Where(x => x.WMS_ORD_NO == wmsNo);
						var seqs = currF051202s.Select(x => new { ItemCode = x.ITEM_CODE, Seq = x.PICK_ORD_SEQ }).ToList();
						if (rowNums.Except(seqs).Any())
							isPass = false;
					}
					else if (wmsNo.StartsWith("P"))
					{
						var currF051203s = f051203sByP.Where(x => x.PICK_ORD_NO == wmsNo);
						var seqs = currF051203s.Select(x => new { ItemCode = x.ITEM_CODE, Seq = x.TTL_PICK_SEQ }).ToList();
						if (rowNums.Except(seqs).Any())
							isPass = false;
					}
				}

				if (!isPass)
					res.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = "SkuList", MsgCode = "20039", MsgContent = string.Format(TacService.GetMsg("20039"), receipt.OrderCode) });
			}
		}
		#endregion

		#region 出庫明細檢核
		/// <summary>
		/// 檢查該ROWNUM是否與資料庫對應
		/// </summary>
		/// <param name="res"></param>
		/// <param name="wmsNo"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		/// <param name="f151002"></param>
		/// <param name="f051202ByO"></param>
		/// <param name="f051202ByP"></param>
		public void CheckRowNumIsExist(List<ApiResponse> res, string wmsNo, OutWarehouseReceiptSkuModel sku, string receiptCode, int index, F151002 f151002, F051202 f051202ByO, F051203 f051203ByP)
		{
			bool rowNumIsExist = true;
			if (wmsNo.StartsWith("T") && f151002 == null)
				rowNumIsExist = false;
			else if (wmsNo.StartsWith("O") && f051202ByO == null)
				rowNumIsExist = false;
			else if (wmsNo.StartsWith("P") && f051203ByP == null)
				rowNumIsExist = false;

			if (!rowNumIsExist)
				res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "RowNum", MsgCode = "23057", MsgContent = string.Format(TacService.GetMsg("23057"), $"{receiptCode}第{index + 1}筆明細") });
		}

		/// <summary>
		/// 檢查該ITEMCODE是否與資料庫對應
		/// </summary>
		/// <param name="res"></param>
		/// <param name="wmsNo"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		/// <param name="f151002"></param>
		/// <param name="f051202ByO"></param>
		/// <param name="f051202ByP"></param>
		public void CheckSkuCodeIsExist(List<ApiResponse> res, string wmsNo, OutWarehouseReceiptSkuModel sku, string receiptCode, int index, F151002 f151002, F051202 f051202ByO, F051203 f051203ByP)
		{
			bool skuCodeIsExist = true;
			if (f151002 != null && f151002.ITEM_CODE != sku.SkuCode)
				skuCodeIsExist = false;
			else if (f051202ByO != null && f051202ByO.ITEM_CODE != sku.SkuCode)
				skuCodeIsExist = false;
			else if (f051203ByP != null && f051203ByP.ITEM_CODE != sku.SkuCode)
				skuCodeIsExist = false;

			if (!skuCodeIsExist)
				res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SkuCode", MsgCode = "23059", MsgContent = string.Format(TacService.GetMsg("23059"), $"{receiptCode}第{index + 1}筆明細") });
		}

		/// <summary>
		/// 檢查[商品等級]是否設定錯誤
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		public void CheckSkuLevel(List<ApiResponse> res, OutWarehouseReceiptSkuModel sku, string receiptCode, int index)
		{
			List<int> skuLeves = new List<int> { 0, 1 };
			if (!skuLeves.Contains(Convert.ToInt32(sku.SkuLevel)))
				res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SkuLevel", MsgCode = "23051", MsgContent = string.Format(TacService.GetMsg("23051"), $"{receiptCode}第{index + 1}筆明細", "商品等級") });
		}

		/// <summary>
		/// 檢查[實際揀貨數量]是否有大於0
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		public void CheckSkuQty(List<ApiResponse> res, OutWarehouseReceiptSkuModel sku, string receiptCode, int index)
		{
			if (sku.SkuQty < 0)
				res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SkuQty", MsgCode = "23056", MsgContent = string.Format(TacService.GetMsg("23056"), $"{receiptCode}第{index + 1}筆明細") });
		}

		/// <summary>
		/// 檢查[實際揀貨數量]是否大於[預計出庫量]
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		public void CheckSkuQtyAndSkuPlanQty(List<ApiResponse> res, OutWarehouseReceiptSkuModel sku, string receiptCode, int index)
		{
			if (sku.SkuQty > sku.SkuPlanQty)
			{
				res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SkuQty", MsgCode = "20047", MsgContent = string.Format(TacService.GetMsg("23055"), $"{receiptCode}第{index + 1}筆明細", "實際揀貨數量", "預計揀貨數量") });
				res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SkuPlanQty", MsgCode = "20047", MsgContent = string.Format(TacService.GetMsg("23055"), $"{receiptCode}第{index + 1}筆明細", "實際揀貨數量", "預計揀貨數量") });
			}
		}

		/// <summary>
		/// 檢查[實際揀貨數量]、[預計揀貨數量]是否超過調撥單下架數
		/// </summary>
		/// <param name="res"></param>
		/// <param name="wmsNo"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		/// <param name="f151002"></param>
		/// <param name="f051202ByO"></param>
		/// <param name="f051203ByP"></param>
		public void CheckSkuQtyAndSkuPlanQtyExceedTarQty(List<ApiResponse> res, string wmsNo, OutWarehouseReceiptSkuModel sku, string receiptCode, int index, F151002 f151002, F051202 f051202ByO, F051203 f051203ByP)
		{
			if (f151002 != null)
			{
				if (sku.SkuQty > f151002.SRC_QTY)
					res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SkuQty", MsgCode = "20047", MsgContent = string.Format(TacService.GetMsg("20047"), $"{receiptCode}第{index + 1}筆明細", "實際揀貨數量") });
				if (sku.SkuPlanQty > f151002.SRC_QTY)
					res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SkuPlanQty", MsgCode = "20047", MsgContent = string.Format(TacService.GetMsg("20047"), $"{receiptCode}第{index + 1}筆明細", "預計揀貨數量") });
			}
			else if (f051202ByO != null)
			{
				if (sku.SkuQty > f051202ByO.B_PICK_QTY)
					res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SkuQty", MsgCode = "20047", MsgContent = string.Format(TacService.GetMsg("20047"), $"{receiptCode}第{index + 1}筆明細", "實際揀貨數量") });
				if (sku.SkuPlanQty > f051202ByO.B_PICK_QTY)
					res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SkuPlanQty", MsgCode = "20047", MsgContent = string.Format(TacService.GetMsg("20047"), $"{receiptCode}第{index + 1}筆明細", "預計揀貨數量") });
			}
			else if (f051203ByP != null)
			{
				if (sku.SkuQty > f051203ByP.B_PICK_QTY)
					res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SkuQty", MsgCode = "20047", MsgContent = string.Format(TacService.GetMsg("20047"), $"{receiptCode}第{index + 1}筆明細", "實際揀貨數量") });
				if (sku.SkuPlanQty > f051203ByP.B_PICK_QTY)
					res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SkuPlanQty", MsgCode = "20047", MsgContent = string.Format(TacService.GetMsg("20047"), $"{receiptCode}第{index + 1}筆明細", "預計揀貨數量") });
			}
		}

		/// <summary>
		/// 檢查該明細是否已完成
		/// </summary>
		/// <param name="res"></param>
		/// <param name="wmsNo"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		/// <param name="f151002"></param>
		/// <param name="f051202ByO"></param>
		/// <param name="f051203ByP"></param>
		public void CheckSkuIsFinish(List<ApiResponse> res, string wmsNo, OutWarehouseReceiptSkuModel sku, string receiptCode, int index, F151002 f151002, F051202 f051202ByO, F051203 f051203ByP)
		{
			bool isFinish = false;
			if (f151002 != null && f151002.STATUS == "1")
				isFinish = true;
			else if (f051202ByO != null && f051202ByO.PICK_STATUS == "1")
				isFinish = true;
			else if (f051203ByP != null && f051203ByP.PICK_STATUS == "1")
				isFinish = true;

			if (isFinish)
				res.Add(new ApiResponse { No = receiptCode, MsgCode = "20046", MsgContent = string.Format(TacService.GetMsg("20046"), $"{receiptCode}第{index + 1}筆明細", "此單據明細") });
		}

		/// <summary>
		/// 檢查該明細效期
		/// </summary>
		/// <param name="res"></param>
		/// <param name="wmsNo"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		/// <param name="f151002"></param>
		/// <param name="f051202ByO"></param>
		/// <param name="f051202ByP"></param>
		public void CheckSkuExpiryDate(List<ApiResponse> res, string wmsNo, OutWarehouseReceiptSkuModel sku, string receiptCode, int index, F151002 f151002, F051202 f051202ByO, F051202 f051202ByP)
		{
			bool expiryDateIsEquel = false;
			if (f151002 != null && f151002.VALID_DATE.ToString("yyyy/MM/dd") == sku.ExpiryDate)
				expiryDateIsEquel = true;
			else if (f051202ByO != null && f051202ByO.VALID_DATE.ToString("yyyy/MM/dd") == sku.ExpiryDate)
				expiryDateIsEquel = true;
			else if (f051202ByP != null && f051202ByP.VALID_DATE.ToString("yyyy/MM/dd") == sku.ExpiryDate)
				expiryDateIsEquel = true;

			if (!expiryDateIsEquel && (f151002 != null || f051202ByO != null || f051202ByP != null))
				res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "ExpiryDate", MsgCode = "20045", MsgContent = string.Format(TacService.GetMsg("20045"), $"{receiptCode}第{index + 1}筆明細", "效期") });
		}

		/// <summary>
		/// 檢查該明細批號
		/// </summary>
		/// <param name="res"></param>
		/// <param name="wmsNo"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		/// <param name="f151002"></param>
		/// <param name="f051202ByO"></param>
		/// <param name="f051202ByP"></param>
		public void CheckSkuOutBatchCode(List<ApiResponse> res, string wmsNo, OutWarehouseReceiptSkuModel sku, string receiptCode, int index, F151002 f151002, F051202 f051202ByO, F051202 f051202ByP)
		{
			bool outBatchCodeIsEquel = false;
			if (f151002 != null && f151002.MAKE_NO == sku.OutBatchCode)
				outBatchCodeIsEquel = true;
			else if (f051202ByO != null && f051202ByO.MAKE_NO == sku.OutBatchCode)
				outBatchCodeIsEquel = true;
			else if (f051202ByP != null && f051202ByP.MAKE_NO == sku.OutBatchCode)
				outBatchCodeIsEquel = true;

			if (!outBatchCodeIsEquel && (f151002 != null || f051202ByO != null || f051202ByP != null))
				res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "OutBatchCode", MsgCode = "20045", MsgContent = string.Format(TacService.GetMsg("20045"), $"{receiptCode}第{index + 1}筆明細", "批號") });
		}

		/// <summary>
		/// 檢查該明細序號長度
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		public void CheckSkuSerialNum(List<ApiResponse> res, OutWarehouseReceiptSkuModel sku, string receiptCode, int index)
		{
			if (sku.SerialNumList != null && sku.SerialNumList.Any())
			{
				bool serialNumIsExceed = string.Join(",", sku.SerialNumList).Length > 8000;
				if (serialNumIsExceed)
					res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SerialNumList", MsgCode = "20044", MsgContent = string.Format(TacService.GetMsg("20044"), $"{receiptCode}第{index + 1}筆明細") });
			}
		}

		/// <summary>
		/// 檢查該明細序號長度是否與實際出庫數相同
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		public void CheckSkuSerialNumEquelSkuQty(List<ApiResponse> res, OutWarehouseReceiptSkuModel sku, string receiptCode, int index)
		{
			if (sku.SerialNumList != null && sku.SerialNumList.Any() && sku.SkuQty != sku.SerialNumList.Count)
				res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SerialNumList", MsgCode = "20043", MsgContent = string.Format(TacService.GetMsg("20043"), $"{receiptCode}第{index + 1}筆明細") });
		}

		/// <summary>
		/// 檢查該明細序號是否存在及是否為在庫序號
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sku"></param>
		/// <param name="receiptCode"></param>
		/// <param name="index"></param>
		/// <param name="_f2501"></param>
		public void CheckSkuSerialNumIsExist(List<ApiResponse> res, OutWarehouseReceiptSkuModel sku, string receiptCode, int index, List<F2501> serialNoList)
		{
			if (sku.SerialNumList != null && sku.SerialNumList.Any())
			{
        var data = serialNoList.Where(serialNo => sku.SerialNumList.Contains(serialNo.SERIAL_NO));
        if (data.Count() != sku.SerialNumList.Count)
					res.Add(new ApiResponse { No = receiptCode, ErrorColumn = "SerialNumList", MsgCode = "20042", MsgContent = string.Format(TacService.GetMsg("20042"), $"{receiptCode}第{index + 1}筆明細") });
			}
		}
		#endregion

		#region 出庫任務單號回檔紀錄檔檢核
		public bool CheckDocExist(List<ApiResponse> res, OutWarehouseReceiptModel receipt, string dcCode)
		{
			var isAddF075106 = false;
			var f075106Repo = new F075106Repository(Schemas.CoreSchema);
			#region 新增出庫任務單號回檔紀錄檔
			var f075106Res = f075106Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
				new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
			() =>
			{
				var lockF075106 = f075106Repo.LockF075106();
				var f075106 = f075106Repo.Find(o => o.DC_CODE == dcCode && o.DOC_ID == receipt.OrderCode, isForUpdate: true, isByCache: false);
				if (f075106 == null)
				{
					f075106 = new F075106 { DC_CODE = dcCode, DOC_ID = receipt.OrderCode };
					f075106Repo.Add(f075106);
					isAddF075106 = true;
				}
				else
				{
					f075106 = null; // 代表F075106已存在資料
				}
				return f075106;
			});
			if (f075106Res == null)// 代表F075106已存在資料
				res.Add(new ApiResponse { No = receipt.OrderCode, MsgCode = "20964", MsgContent = string.Format(TacService.GetMsg("20964"), receipt.OrderCode) });
			#endregion

			return isAddF075106;
		}

    /// <summary>
    /// 檢查序號數量(去重複)是否與容器商品裝箱數量相同
    /// </summary>
    /// <param name="res"></param>
    /// <param name="receiptCode"></param>
    /// <param name="containers"></param>
    public void CheckContainerSerialNosQty(List<ApiResponse> res, string receiptCode, OutWarehouseReceiptContainerModel container)
    {
      foreach (var containerSku in container.SkuList)
      {
        var SerialNumList = containerSku.SerialNumList.Distinct();
        if (SerialNumList.Count() > 0 && SerialNumList.Count() != containerSku.SkuQty)
        {
          res.Add(new ApiResponse
          {
            No = receiptCode,
            ErrorColumn = "SerialNumList",
            MsgCode = "20006",
            //20006, 容器{0}明細中商品{1}序號總數{2}與裝箱數量{3}不一致
            MsgContent =
              string.Format(TacService.GetMsg("20006"),
                container.ContainerCode,
                containerSku.SkuCode,
                SerialNumList.Count(),
                containerSku.SkuQty)
          });
        }
      }
    }

    /// <summary>
    /// 檢查揀貨明細商品總實際揀貨數量是否等於加總各箱該商品裝箱數量
    /// </summary>
    /// <param name="data"></param>
    /// <param name="orderCode"></param>
    /// <param name="skuList"></param>
    /// <param name="containerList"></param>
    public void CheckContainerSkuQty(List<ApiResponse> res, string receiptCode, List<OutWarehouseReceiptSkuModel> skuList, List<OutWarehouseReceiptContainerSkuModel> containerSkuList)
    {
      var checkQtyEquals = from container in containerSkuList.GroupBy(x => x.SkuCode)
                           join skus in skuList.GroupBy(x => x.SkuCode)
                           on container.Key equals skus.Key into e1
                           from j1 in e1.DefaultIfEmpty()
                           where container.Sum(x => x.SkuQty) != j1.Sum(x => x.SkuQty ?? 0)
                           select new { skuCode = container.Key, containerQty = container.Sum(x => x.SkuQty), skuQty = j1.Sum(x => x.SkuQty) };

      res.AddRange(checkQtyEquals.Select(x => new ApiResponse
      {
        No = receiptCode,
        ErrorColumn = "SkuQty",
        MsgCode = "20005",
        //20005, 容器明細中商品{0}總數量{1}與回傳揀貨明細商品實際揀貨數{2}不一致
        MsgContent =
                string.Format(TacService.GetMsg("20005"),
                  x.skuCode,
                  x.containerQty,
                  x.skuQty)
      }));
    }

    /// <summary>
    /// 檢查容器中的商品序號是否存在
    /// </summary>
    /// <param name="res"></param>
    /// <param name="receiptCode"></param>
    /// <param name="containers"></param>
    /// <param name="serialNoList"></param>
    public void CheckContainerSerialNosExists(List<ApiResponse> res, string receiptCode, OutWarehouseReceiptContainerModel containers, List<F2501> serialNoList)
    {
      var containerSerialNoList = containers.SkuList.SelectMany(x1 => x1.SerialNumList);

      var InSerialNoList = from x in containerSerialNoList
                           join y in serialNoList.Where(x => x.STATUS == "A1").Select(x => x.SERIAL_NO)
                           on x equals y
                           select x;

      containerSerialNoList = containerSerialNoList.Except(InSerialNoList);

      if (containerSerialNoList.Any())
        res.Add(new ApiResponse
        {
          No = receiptCode,
          ErrorColumn = "SerialNumList",
          MsgCode = "20007",
          //20007, 容器{0}明細中，商品序號{1}不存在
          MsgContent =
          string.Format(TacService.GetMsg("20007"),
            containers.ContainerCode,
            string.Join(",", containerSerialNoList))
        });

    }
    #endregion
  }
}
