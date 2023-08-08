using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Checks;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Services
{
	/// <summary>
	/// 補貨超揀申請
	/// </summary>
	public class CommonOverPickApplyReceiptService
	{
        #region 定義需檢核欄位、必填、型態、長度
        // 補貨超揀申請檢核設定
        private List<ApiCkeckColumnModel> receiptCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "OwnerCode",   Type = typeof(string),   MaxLength = 12,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "DcCode",      Type = typeof(string),   MaxLength = 16,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "ZoneCode",    Type = typeof(string),   MaxLength = 5,   Nullable = false },
			new ApiCkeckColumnModel{  Name = "OrderCode",   Type = typeof(string),   MaxLength = 32,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "RowNum",      Type = typeof(int),      MaxLength = 0 ,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "SkuCode",     Type = typeof(string),   MaxLength = 20 , Nullable = false },
            new ApiCkeckColumnModel{  Name = "SkuQty",      Type = typeof(int),      MaxLength = 0,   Nullable = false }
        };
		#endregion

		#region Main Method
		private WmsTransaction _wmsTransaction;
        /// <summary>
        /// 出庫任務派發作業
        /// </summary>
        private F060201 _f060201;
        /// <summary>
        /// 調撥單頭檔
        /// </summary>
        private F151001 _f151001;
        /// <summary>
        /// 調撥單身擋
        /// </summary>
        private F151002 _f151002;

		/// <summary>
		/// Func1
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult RecevieApiDatas(OverPickApplyReq req)
		{
			CheckTransWcsApiService ctaService = new CheckTransWcsApiService();
			TransApiBaseService tacService = new TransApiBaseService();
			CommonService commonService = new CommonService();
			ApiResult res = new ApiResult { IsSuccessed = true };

			#region 資料檢核
			// 檢核參數
			if (req == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };
            
            // 檢核物流中心 必填、是否存在
            ctaService.CheckDcCode(ref res, req);
			if (!res.IsSuccessed)
				return res;

            // 檢核貨主編號 必填、是否存在
            ctaService.CheckOwnerCode(ref res, req);
            if (!res.IsSuccessed)
                return res;
            #endregion

            // 取得業主編號
            string gupCode = commonService.GetGupCode(req.OwnerCode);

            // 資料處理1
            return ProcessApiDatas(req, gupCode);
		}

        /// <summary>
        /// 資料處理
        /// </summary>
        /// <param name="receipt"></param>
        /// <param name="gupCode"></param>
        /// <returns></returns>
		public ApiResult ProcessApiDatas(OverPickApplyReq receipt, string gupCode)
		{
            #region 變數
            if (_wmsTransaction == null)
                _wmsTransaction = new WmsTransaction();
            var f060201Repo = new F060201Repository(Schemas.CoreSchema);
			var f151001Repo = new F151001Repository(Schemas.CoreSchema);
            var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
            var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
            var f1913Repo = new F1913Repository(Schemas.CoreSchema);
            var f0093Repo = new F0093Repository(Schemas.CoreSchema, _wmsTransaction);
            var commonService = new CommonService();
			var tacService = new TransApiBaseService();
            var stockService = new StockService(_wmsTransaction);
            var res = new ApiResult { IsSuccessed = true };

            // 取得出庫任務派發作業
            _f060201 = f060201Repo.Find(o => o.DC_CODE == receipt.DcCode && o.GUP_CODE == gupCode && o.CUST_CODE == receipt.OwnerCode && o.DOC_ID == receipt.OrderCode && o.CMD_TYPE == "1");

            // 取得調撥單頭檔
            if(_f060201 != null)
                _f151001 = f151001Repo.Find(o => o.DC_CODE == receipt.DcCode && o.GUP_CODE == gupCode && o.CUST_CODE == receipt.OwnerCode && o.ALLOCATION_NO == _f060201.WMS_NO);

            // 取得調撥單身檔
            _f151002 = f151002Repo.AsForUpdate().GetDataByAllocSeq(receipt.DcCode, gupCode, receipt.OwnerCode, receipt.OrderCode, Convert.ToInt16(receipt.RowNum));
            #endregion

            #region 檢核
            var res1 = CheckOverPickApplyReceipt(receipt, gupCode);
			if (!res1.IsSuccessed)
                return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056"), Data = (List<ApiResponse>)res1.Data };
            #endregion

            #region 資料處理
			      
			      var isPass = false;
						string allotBatchNo=string.Empty;
            try
            {
				        var itemCodes = new List<ItemKey> { new ItemKey { DcCode = _f151002.DC_CODE,GupCode =_f151002.GUP_CODE,CustCode = _f151002.CUST_CODE,ItemCode=_f151002.ITEM_CODE } };
				        allotBatchNo = "BO" + DateTime.Now.ToString("yyyyMMddHHmmss");
				        isPass = stockService.CheckAllotStockStatus(false,allotBatchNo, itemCodes);

                // 檢查是否在配庫中
                if (!isPass)
                    res = new ApiResult { IsSuccessed = false, MsgCode = "23006", MsgContent = string.Format(tacService.GetMsg("23006"), receipt.OrderCode) };
                
                if (res.IsSuccessed)
                {
                    // 取得商品庫存
                    long qty = 0;
                    var serialNo = string.IsNullOrWhiteSpace(_f151002.SERIAL_NO) ? "0" : _f151002.SERIAL_NO;
                    var f1913 = f1913Repo.Find(o =>
                    o.DC_CODE == _f151001.SRC_DC_CODE &&
                    o.GUP_CODE == _f151002.GUP_CODE &&
                    o.CUST_CODE == _f151002.CUST_CODE &&
                    o.LOC_CODE == _f151002.SRC_LOC_CODE &&
                    o.ITEM_CODE == _f151002.ITEM_CODE &&
                    o.VALID_DATE == _f151002.VALID_DATE &&
                    o.ENTER_DATE == _f151002.ENTER_DATE &&
                    o.VNR_CODE == _f151002.VNR_CODE &&
                    o.MAKE_NO == _f151002.MAKE_NO &&
                    o.SERIAL_NO == serialNo &&
                    o.BOX_CTRL_NO == _f151002.BOX_CTRL_NO &&
                    o.PALLET_CTRL_NO == _f151002.PALLET_CTRL_NO);
                    if (f1913 != null)
                        qty = f1913.QTY;

                    var diffQty = receipt.SkuQty - _f151002.SRC_QTY;

                    // 如果商品庫存[F1913.QTY] < [SkuQty - F151002.SRC_QTY]，則
                    if (qty < diffQty)
                    {
                        // (1) 新增一筆[F0093]
                        // (2) 回傳錯誤訊息23007(補貨庫存數異常，不可超補[超補數 ={0}, 庫存數 ={1}],SkuQty,F1913.QTY)
                        var msgNo = "23007";
                        var msgContent = string.Format(tacService.GetMsg("23007"), diffQty, qty);

                        f0093Repo.Add(new F0093
                        {
                            DC_CODE = _f151002.DC_CODE,
                            GUP_CODE = _f151002.GUP_CODE,
                            CUST_CODE = _f151002.CUST_CODE,
                            WMS_NO = _f151002.ALLOCATION_NO,
                            WMS_TYPE = "T",
                            ITEM_CODE = _f151002.ITEM_CODE,
                            STATUS = "0",
                            MSG_NO = $"API{msgNo}" ,
                            MSG_CONTENT = msgContent
                        });

                        res = new ApiResult { IsSuccessed = false, MsgCode = msgNo, MsgContent = msgContent };
                    }
                    else // 如果商品庫存[F1913.QTY] >= [SkuQty - F151002.SRC_QTY]
                    {
                        // (1) 扣除庫存(StockService.DeductStock)
                        var deductStockRes = stockService.DeductStock(new List<Shared.ServiceEntites.OrderStockChange>
                        {
                            new Shared.ServiceEntites.OrderStockChange
                            {
                                DcCode = _f151002.DC_CODE,
                                GupCode = _f151002.GUP_CODE,
                                CustCode = _f151002.CUST_CODE,
                                WmsNo = _f151002.ALLOCATION_NO,
                                LocCode = _f151002.SRC_LOC_CODE,
                                ItemCode = _f151002.ITEM_CODE,
                                VaildDate = _f151002.VALID_DATE,
                                EnterDate = _f151002.ENTER_DATE,
                                MakeNo = _f151002.MAKE_NO,
                                SerialNo = serialNo,
                                BoxCtrlNo = _f151002.BOX_CTRL_NO,
                                PalletCtrlNo = _f151002.PALLET_CTRL_NO,
                                VnrCode = _f151002.VNR_CODE,
                                Qty = Convert.ToInt32(diffQty)
                            }
                        });

                        // (2) 更新庫存(StockService.SaveChange)
                        stockService.SaveChange();

						// (3) 更新F151002.SRC_QTY = SkuQty，如果F151001.ALLOCATION_TYPE<>5，才更新F151002.SRC_QTY = SkuQty,F151002.TAR_QTY = SkuQty
						var longSkuQty = Convert.ToInt64(receipt.SkuQty);
						_f151002.SRC_QTY = longSkuQty;
						if (_f151001.ALLOCATION_TYPE != "5")
						{
							_f151002.TAR_QTY = longSkuQty;
						}

						f151002Repo.Update(_f151002);

						// (4) 更新F1511.B_PICK_QTY = SkuQty
						var f1511 = f1511Repo.Find(o =>
                        o.DC_CODE == _f151002.DC_CODE &&
                        o.GUP_CODE == _f151002.GUP_CODE &&
                        o.CUST_CODE == _f151002.CUST_CODE &&
                        o.ORDER_NO == _f151002.ALLOCATION_NO &&
                        o.ORDER_SEQ == _f151002.ALLOCATION_SEQ.ToString());
                        if (f1511 != null)
                        {
                            f1511.B_PICK_QTY = Convert.ToInt32(receipt.SkuQty);
                            f1511Repo.Update(f1511);
                        }

                        res = new ApiResult { IsSuccessed = true, MsgCode = "10002", MsgContent = tacService.GetMsg("10002") };
                    }

                    _wmsTransaction.Complete();
                }
            }
            finally
            {
                // 更新為未配庫 StockService.UpdateAllotStockStatusToNotAllot();
                if(isPass)
                    stockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
            }
            #endregion

            return res;

        }

        /// <summary>
        /// 資料處理2
        /// </summary>
        /// <param name="req"></param>
        /// <param name="gupCode"></param>
        /// <returns></returns>
		private ApiResult CheckOverPickApplyReceipt(OverPickApplyReq req, string gupCode)
		{
			ApiResult result = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			// 預設值設定
			data.AddRange((List<ApiResponse>)CheckDefaultSetting(req).Data);

			// 共用欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(req).Data);

			// 貨主自訂欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckCustomColumnType(req).Data);

			// 如果以上檢核成功
			if (!data.Any())
			{
				// 共用欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCommonColumnData(req).Data);

				// 貨主自訂欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCustomColumnValue(req).Data);
			}

			result.IsSuccessed = !data.Any();
			result.Data = data;

			return result;
		}
		#endregion

		#region Protected 檢核
		/// <summary>
		/// 預設值設定
		/// </summary>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckDefaultSetting(OverPickApplyReq receipt)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}

		/// <summary>
		/// 共用欄位格式檢核
		/// </summary>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckColumnNotNullAndMaxLength(OverPickApplyReq receipt)
		{
			TransApiBaseService tacService = new TransApiBaseService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			string nullErrorMsg = tacService.GetMsg("20034");
			string formatErrorMsg = tacService.GetMsg("20035");

			#region 檢查盤點單欄位必填、最大長度
			List<string> notDateColumn = new List<string>();

			// 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
			receiptCheckColumnList.ForEach(column =>
			{
				var value = Convert.ToString(DataCheckHelper.GetRequireColumnValue(receipt, column.Name));

				// 必填
				if (!column.Nullable)
					if (!DataCheckHelper.CheckRequireColumn(receipt, column.Name))
						data.Add(new ApiResponse { ErrorColumn = column.Name, MsgCode = "20034", MsgContent = string.Format(nullErrorMsg, column.Name) });

				// 最大長度
				if (column.MaxLength > 0)
					if (!DataCheckHelper.CheckDataMaxLength(receipt, column.Name, column.MaxLength))
						data.Add(new ApiResponse { ErrorColumn = column.Name, MsgCode = "20035", MsgContent = string.Format(formatErrorMsg, column.Name) });
			});
			#endregion

			res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位格式檢核
		/// </summary>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnType(OverPickApplyReq receipt)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}

        /// <summary>
        /// 共用欄位資料檢核
        /// </summary>
        /// <param name="receipt"></param>
        /// <returns></returns>
		protected ApiResult CheckCommonColumnData(OverPickApplyReq receipt)
		{
			CheckOverPickApplyReceiptService copService = new CheckOverPickApplyReceiptService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

            #region 庫存快照單欄位資料檢核
            // 檢查該出庫單號是否存在於F060201
            copService.CheckOrderCodeExistByF060201(data, receipt, _f060201);

            if (!data.Any())
            {
                // 檢查該出庫單號是否存在於F151001
                copService.CheckOrderCodeExistByF151001(data, receipt, _f151001, _f060201.WMS_NO);

                // 檢查該調撥單狀態
                copService.CheckAllocNoStatus(data, receipt, _f151001, _f060201.WMS_NO);

                // 檢查該調撥單是否為下架單
                copService.CheckAllocNoIsSrc(data, receipt, _f151001, _f060201.WMS_NO);

                // 檢核明細項次是否有在調撥明細中
                copService.CheckRowNumIsExist(data, receipt, _f151002, _f060201.WMS_NO);

                // 檢核實際揀貨數量是否大於0、是否小於預計調撥下架數
                copService.CheckSkuQty(data, receipt, _f151002, _f060201.WMS_NO);
            }
            #endregion

            res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位資料檢核
		/// </summary>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnValue(OverPickApplyReq receipt)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}
		#endregion
	}
}
