
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
using Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Checks;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Services
{
	/// <summary>
	/// 出庫完成結果回傳
	/// </summary>
	public class CommonOutWarehouseReceiptService
	{
    public CommonService CommonService;

		#region 定義需檢核欄位、必填、型態、長度
		// 出庫單檢核設定
		private List<ApiCkeckColumnModel> orderCheckColumnList = new List<ApiCkeckColumnModel>
				{
						new ApiCkeckColumnModel{  Name = "OrderCode",      Type = typeof(string),   MaxLength = 32, Nullable = false },
						new ApiCkeckColumnModel{  Name = "Status",         Type = typeof(int),      MaxLength = 0,  Nullable = false },
						new ApiCkeckColumnModel{  Name = "StartTime",      Type = typeof(string),   MaxLength = 19,  Nullable = false, IsDateTime = true },
						new ApiCkeckColumnModel{  Name = "CompleteTime",   Type = typeof(string),   MaxLength = 19,  Nullable = false, IsDateTime = true },
						new ApiCkeckColumnModel{  Name = "Operator",       Type = typeof(string),   MaxLength = 20, Nullable = false },
						new ApiCkeckColumnModel{  Name = "IsException",    Type = typeof(int),      MaxLength = 1 , Nullable = false },
						new ApiCkeckColumnModel{  Name = "SkuTotal",       Type = typeof(int),      MaxLength = 0 , Nullable = false }
				};

		// 出庫明細檢核設定
		private List<ApiCkeckColumnModel> skuCheckColumnList = new List<ApiCkeckColumnModel>
				{
						new ApiCkeckColumnModel{  Name = "RowNum",             Type = typeof(int),      MaxLength = 0,  Nullable = false },
						new ApiCkeckColumnModel{  Name = "SkuCode",            Type = typeof(string),   MaxLength = 20, Nullable = false },
						new ApiCkeckColumnModel{  Name = "SkuPlanQty",         Type = typeof(int),      MaxLength = 0 , Nullable = false },
						new ApiCkeckColumnModel{  Name = "SkuQty",             Type = typeof(int),      MaxLength = 0 , Nullable = false },
						new ApiCkeckColumnModel{  Name = "SkuLevel",           Type = typeof(int),      MaxLength = 0 , Nullable = false },
						new ApiCkeckColumnModel{  Name = "ExpiryDate",         Type = typeof(string),   MaxLength = 10, IsDate = true },
						new ApiCkeckColumnModel{  Name = "OutBatchCode",       Type = typeof(string),   MaxLength = 20 }
				};

		// 出庫明細檢核設定
		private List<ApiCkeckColumnModel> shelfBinCheckColumnList = new List<ApiCkeckColumnModel>
				{
						new ApiCkeckColumnModel{  Name = "ShelfCode",     Type = typeof(string),   MaxLength = 20 },
						new ApiCkeckColumnModel{  Name = "BinCode",       Type = typeof(string),   MaxLength = 20 },
						new ApiCkeckColumnModel{  Name = "SkuQty",        Type = typeof(int),      MaxLength = 0  },
						new ApiCkeckColumnModel{  Name = "Operator",      Type = typeof(string),   MaxLength = 20 }
				};

		// 出庫容器明細檢核設定
		private List<ApiCkeckColumnModel> containerCheckColumnList = new List<ApiCkeckColumnModel>
				{
						new ApiCkeckColumnModel{  Name = "ContainerCode",  Type = typeof(string),   MaxLength = 32 },
						new ApiCkeckColumnModel{  Name = "Operator",       Type = typeof(string),   MaxLength = 20 },
						new ApiCkeckColumnModel{  Name = "WorkStationNo",  Type = typeof(string),   MaxLength = 32 },
						new ApiCkeckColumnModel{  Name = "SeedBinCode",    Type = typeof(string),   MaxLength = 32 },
						new ApiCkeckColumnModel{  Name = "SkuTotal",       Type = typeof(int),      MaxLength = 0, Nullable = false }
				};

		// 出庫容器內品項檢核設定
		private List<ApiCkeckColumnModel> containerSkuCheckColumnList = new List<ApiCkeckColumnModel>
				{
						new ApiCkeckColumnModel{  Name = "SkuCode",      Type = typeof(string),   MaxLength = 20, Nullable = false },
						new ApiCkeckColumnModel{  Name = "SkuQty",       Type = typeof(string),   MaxLength = 0,  Nullable = false }
				};
		#endregion

		#region Private Property
		/// <summary>
		/// 出庫任務完成結果回傳清單
		/// </summary>
		private List<F060202> _f060202List;

		/// <summary>
		/// 出庫任務派發作業清單
		/// </summary>
		private List<F060201> _f060201List;

		/// <summary>
		/// 揀貨單明細By出貨單號
		/// </summary>
		private List<F051202> _f060202ListByO;

		/// <summary>
		/// 揀貨單明細By揀貨單號
		/// </summary>
		private List<F051203> _f060202ListByP;

		/// <summary>
		/// 調撥單主檔清單
		/// </summary>
		private List<F151001> _f151001List;

		/// <summary>
		/// 調撥單明細清單
		/// </summary>
		private List<F151002> _f151002List;

		/// <summary>
		/// 出貨單清單
		/// </summary>
		private List<F050801> _f050801List;

		/// <summary>
		/// 揀貨單清單
		/// </summary>
		private List<F051201> _f051201List;

		/// <summary>
		/// 序號資料清單
		/// </summary>
		private List<F2501> _serialNoList;

		/// <summary>
		/// 失敗出庫單數
		/// </summary>
		private int _failCnt = 0;

		/// <summary>
		/// 紀錄有新增過F075106的DOC_ID，用以若檢核失敗 找出是否有新增，用以刪除
		/// </summary>
		private List<string> _IsAddF075106DocIdList = new List<string>();
		#endregion

		#region Main Method
		/// <summary>
		/// Func1
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult RecevieApiDatas(OutWarehouseReceiptReq req)
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

			// 檢核倉庫編號 必填、是否存在
			ctaService.CheckZoneCode(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核出庫單列表
			if (req.OrderList == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "20098", MsgContent = string.Format(tacService.GetMsg("20098"), "出庫單") };

			// 檢核資料筆數
			int reqTotal = req.OrderTotal != null ? Convert.ToInt32(req.OrderTotal) : 0;
			if (req.OrderList == null || (req.OrderList != null && !tacService.CheckDataCount(reqTotal, req.OrderList.Count)))
				return new ApiResult { IsSuccessed = false, MsgCode = "20022", MsgContent = string.Format(tacService.GetMsg("20022"), "出庫單", reqTotal, req.OrderList.Count) };

			// 檢核出庫單筆數是否超過[排程每次執行最大單據數]筆
			int midApiMisMax = Convert.ToInt32(commonService.GetSysGlobalValue("MIDAPIMISMAX"));
			if (req.OrderList.Count > midApiMisMax)
				return new ApiResult { IsSuccessed = false, MsgCode = "20099", MsgContent = string.Format(tacService.GetMsg("20099"), "出庫單筆數", req.OrderList.Count, midApiMisMax) };
			#endregion

			// 取得業主編號
			string gupCode = commonService.GetGupCode(req.OwnerCode);

			// 資料處理1
			return ProcessApiDatas(req.DcCode, gupCode, req.OwnerCode, req.ZoneCode, req.OrderList);
		}

		/// <summary>
		/// 資料處理
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="warehouseId"></param>
		/// <param name="receiptList"></param>
		/// <returns></returns>
		public ApiResult ProcessApiDatas(string dcCode, string gupCode, string custCode, string warehouseId, List<OutWarehouseReceiptModel> receiptList)
		{
			#region 變數
			var tacService = new TransApiBaseService();
			WmsTransaction wmsTransation = new WmsTransaction();
			var f060201Repo = new F060201Repository(Schemas.CoreSchema, wmsTransation);
			var f060202Repo = new F060202Repository(Schemas.CoreSchema);
			var f151001Repo = new F151001Repository(Schemas.CoreSchema);
			var f151002Repo = new F151002Repository(Schemas.CoreSchema);
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			var f051201Repo = new F051201Repository(Schemas.CoreSchema);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema);
			var f051203Repo = new F051203Repository(Schemas.CoreSchema);
			var f2501Repo = new F2501Repository(Schemas.CoreSchema);
			var f075106Repo = new F075106Repository(Schemas.CoreSchema);
			#endregion

			#region 取得資料[以下清單設為目前服務的 private property]
			// 任務單號清單
			var docIds = receiptList.Select(x => x.OrderCode).ToList();

			// 取得出庫任務完成結果回傳清單
			_f060202List = f060202Repo.GetDatas(dcCode, gupCode, custCode, docIds).ToList();

			// 取得出庫任務派發作業清單
			_f060201List = f060201Repo.GetDatas(dcCode, gupCode, custCode, "1", docIds).ToList();

			// 取得出庫單所有單號
			var wmsNos = _f060201List.Select(x => x.WMS_NO).ToList();

			// 單號清單(調撥單號、訂單單號、揀貨單號)
			var allNos = wmsNos.Where(wmsNo => wmsNo.StartsWith("T")).ToList();
			var ordNos = wmsNos.Where(wmsNo => wmsNo.StartsWith("O")).ToList();
			var pickNos = wmsNos.Where(wmsNo => wmsNo.StartsWith("P")).ToList();

      _f151001List = new List<F151001>();
      _f151002List = new List<F151002>();
      _f050801List = new List<F050801>();
      _f060202ListByO = new List<F051202>();
      _f051201List = new List<F051201>();
      _f060202ListByP = new List<F051203>();

      if (allNos.Any())
      {
        // 取得調撥單資料
        _f151001List = f151001Repo.GetDatasNoTracking(dcCode, gupCode, custCode, allNos).ToList();
        // 取得調撥單明細資料
        _f151002List = f151002Repo.GetDatasNoTracking(dcCode, gupCode, custCode, allNos).ToList();
      }

      if (ordNos.Any())
      {
        // 取得出貨單資料
        _f050801List = f050801Repo.GetDatasNoTracking(dcCode, gupCode, custCode, ordNos).ToList();
				var pickNoList = _f060201List.Select(x => x.PICK_NO).ToList();
				// 取得揀貨明細
				_f060202ListByO = f051202Repo.GetDataByPickNos(dcCode, gupCode, custCode, pickNoList).ToList();
      }

      if (pickNos.Any())
      {
        // 取得揀貨單資料
        _f051201List = f051201Repo.GetDatasNoTracking(dcCode, gupCode, custCode, pickNos).ToList();
				// 取得揀貨彙總明細
				_f060202ListByP = f051203Repo.GetDatasByPickNos(dcCode, gupCode, custCode, pickNos).ToList();
      }

      // 取得參數序號資料
      var itemSerialNos = receiptList.SelectMany(x => x.SkuList).Where(x => x.SerialNumList != null).SelectMany(x => x.SerialNumList).ToList();

      if (CommonService == null)
        CommonService = new CommonService();

      _serialNoList = new List<F2501>();
      // 取得序號資料
      if (itemSerialNos.Any())
        _serialNoList = CommonService.GetItemSerialList(gupCode, custCode, itemSerialNos).ToList();
			#endregion

			#region Foreach [出庫單列表]
			var res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();
			List<string> successedReceiptCodes = new List<string>();

			// Foreach[出庫單列表] in <參數4> 
			receiptList.ForEach(receipt =>
			{
        var currRes = ApiLogHelper.CreateApiLogInfo(ApiLogType.WCSAPI_OW, dcCode, gupCode, custCode, "OutWarehouseReceipt_Record", receipt, () =>
        {
          if (!string.IsNullOrWhiteSpace(receipt.OrderCode) && successedReceiptCodes.Contains(receipt.OrderCode))
          {
            var sameWmsNoErrRes = new List<ApiResponse> { new ApiResponse { No = receipt.OrderCode, MsgCode = "20041", MsgContent = string.Format(tacService.GetMsg("20041"), receipt.OrderCode) } };
            data.AddRange(sameWmsNoErrRes);
            return new ApiResult { IsSuccessed = false, MsgCode = "20041", MsgContent = string.Format(tacService.GetMsg("20041"), receipt.OrderCode), Data = sameWmsNoErrRes };
          }
          else
          {
            var f060201 = _f060201List.Where(x => x.DOC_ID == receipt.OrderCode).FirstOrDefault();

            if (f060201 == null)
            {
              var wmsNoErrRes = new List<ApiResponse> { new ApiResponse { No = receipt.OrderCode, MsgCode = "20040", MsgContent = string.Format(tacService.GetMsg("20040"), receipt.OrderCode) } };
              data.AddRange(wmsNoErrRes);
              return new ApiResult { IsSuccessed = false, MsgCode = "20040", MsgContent = string.Format(tacService.GetMsg("20040"), receipt.OrderCode), Data = wmsNoErrRes };
            }
            else
            {
              var wmsNo = f060201.WMS_NO;

              if (_f060202List.Where(x => x.DOC_ID == receipt.OrderCode).Any())
              {
                var dataFinishRes = new List<ApiResponse> { new ApiResponse { No = receipt.OrderCode, MsgCode = "20049", MsgContent = string.Format(tacService.GetMsg("20049"), receipt.OrderCode) } };
                data.AddRange(dataFinishRes);
                return new ApiResult { IsSuccessed = false, MsgCode = "20040", MsgContent = string.Format(tacService.GetMsg("20049"), receipt.OrderCode), Data = dataFinishRes };
              }
              else
              {
                if (receipt.Status == 0)
                {
                  // 取得出庫任務資料
                  F060201 newF060201 = _f060201List.Where(x => x.DC_CODE == dcCode &&
                                                 x.GUP_CODE == gupCode &&
                                                 x.CUST_CODE == custCode &&
                                                 x.DOC_ID == receipt.OrderCode &&
                                                 x.STATUS == "2").FirstOrDefault();

                  if (newF060201 == null)
                  {
                    var dataNotExistRes = new List<ApiResponse> { new ApiResponse { No = receipt.OrderCode, MsgCode = "20962", MsgContent = string.Format(tacService.GetMsg("20962"), receipt.OrderCode) } };
                    data.AddRange(dataNotExistRes);
                    return new ApiResult { IsSuccessed = false, MsgCode = "20962", MsgContent = string.Format(tacService.GetMsg("20962"), receipt.OrderCode), Data = dataNotExistRes };
                  }
                  else
                  {
                    var sharedService = new SharedService(wmsTransation);
                    // 複製F060201 產生新的入庫任務資料(新增一筆新的F060201但DOC_ID產生新的編號，狀態為0)
                    sharedService.CreateF060201(dcCode, gupCode, custCode, wmsNo, f060201.PICK_NO, f060201.WAREHOUSE_ID);
                    // 將原F060101.STATUS 更新為4(中介回傳派發失敗)
                    f060201.STATUS = "4";
                    f060201Repo.Update(f060201);
                    wmsTransation.Complete();

                    // 記錄成功的單號，用以再有同單號的不處理
                    successedReceiptCodes.Add(receipt.OrderCode);
                  }
                }
                else if (receipt.Status == 3)
                {
                  //如果是系統報缺無法綁箱，就把ContainerList/SkuList清空，避免檢查＆寫入
                  if (receipt.IsException.HasValue && receipt.IsException.Value == 2)
                  {
                    receipt.ContainerList = new List<OutWarehouseReceiptContainerModel>();
                  }
                  // 出庫單檢核
                  var res1 = CheckOutWarehouseReceipt(dcCode, gupCode, custCode, wmsNo, receipt, f060201);

                  if (!res1.IsSuccessed)
                  {
                    var currDatas = (List<ApiResponse>)res1.Data;
                    data.AddRange(currDatas);

                    if (_IsAddF075106DocIdList.Contains(receipt.OrderCode))
                    {
                      f075106Repo.DelF075106ByKey(dcCode, receipt.OrderCode);
                      _IsAddF075106DocIdList = _IsAddF075106DocIdList.Where(x => x != receipt.OrderCode).ToList();
                    }
                    wmsTransation.Complete();
                    return new ApiResult { IsSuccessed = false, MsgCode = "99999", MsgContent = "檢核失敗", Data = currDatas };
                  }
                  else
                  {
                    f060202Repo = new F060202Repository(Schemas.CoreSchema, wmsTransation);
                    var f060203Repo = new F060203Repository(Schemas.CoreSchema, wmsTransation);
                    var f060204Repo = new F060204Repository(Schemas.CoreSchema, wmsTransation);
                    var f060205Repo = new F060205Repository(Schemas.CoreSchema, wmsTransation);
                    var f060206Repo = new F060206Repository(Schemas.CoreSchema, wmsTransation);
                    List<F060203> addF060203 = new List<F060203>();
                    List<F060206> addF060206 = new List<F060206>();

                    #region 新增F060202
                    f060202Repo.Add(new F060202
                    {
                      DC_CODE = dcCode,
                      GUP_CODE = gupCode,
                      CUST_CODE = custCode,
                      WAREHOUSE_ID = warehouseId,
                      DOC_ID = receipt.OrderCode,
                      WMS_NO = wmsNo,
                      PICK_NO = f060201.PICK_NO,
                      STATUS = "0",
                      M_STATUS = Convert.ToString(receipt.Status),
                      STARTTIME = receipt.StartTime,
                      COMPLETETIME = receipt.CompleteTime,
                      OPERATOR = receipt.Operator,
                      ISEXCEPTION = Convert.ToInt32(receipt.IsException),
                      SKUTOTAL = receipt.SkuList.Count
                    });
                    #endregion

														 #region 新增F060203、F060204
														 receipt.SkuList.ForEach(sku =>
														 {
                               //如果是有帶序號的就拆分成一筆一個序號
                               if (sku.SerialNumList != null && sku.SerialNumList.Any())
                                 addF060203.AddRange(sku.SerialNumList.Select(x => new F060203
                                 {
                                   DOC_ID = receipt.OrderCode,
                                   ROWNUM = Convert.ToInt32(sku.RowNum),
                                   SKUCODE = sku.SkuCode,
                                   SKUPLANQTY = 1,
                                   SKUQTY = 1,
                                   SKULEVEL = Convert.ToInt32(sku.SkuLevel),
                                   EXPIRYDATE = sku.ExpiryDate,
                                   OUTBATCHCODE = sku.OutBatchCode,
                                   SERIAL_NO = x
                                 }));
                               else
                                 addF060203.Add(new F060203
                                 {
                                   DOC_ID = receipt.OrderCode,
                                   ROWNUM = Convert.ToInt32(sku.RowNum),
                                   SKUCODE = sku.SkuCode,
                                   SKUPLANQTY = Convert.ToInt32(sku.SkuPlanQty),
                                   SKUQTY = Convert.ToInt32(sku.SkuQty),
                                   SKULEVEL = Convert.ToInt32(sku.SkuLevel),
                                   EXPIRYDATE = sku.ExpiryDate,
                                   OUTBATCHCODE = sku.OutBatchCode,
                                 });

                      if (sku.ShelfBinList != null)
                      {
                        sku.ShelfBinList.ForEach(shelfBin =>
                        {
                          f060204Repo.Add(new F060204
                          {
                            DOC_ID = receipt.OrderCode,
                            ORD_SEQ = Convert.ToInt32(sku.RowNum),
                            SHELFCODE = shelfBin.ShelfCode,
                            BINCODE = shelfBin.BinCode,
                            SKUQTY = shelfBin.SkuQty,
                            OPERATOR = shelfBin.Operator
                          });
                        });
                      }
                    });

                    #endregion

                    #region 新增F060205、F060206
                    receipt.ContainerList.ForEach(container =>
                    {
                      f060205Repo.Add(new F060205
                      {
                        DOC_ID = receipt.OrderCode,
                        CONTAINERCODE = container.ContainerCode,
                        OPERATOR = container.Operator,
                        WORKSTATIONNO = container.WorkStationNo,
                        SEEDBINCODE = container.SeedBinCode,
                        SKUTOTAL = Convert.ToInt32(container.SkuTotal)
                      });

                      container.SkuList.ForEach(sku =>
                       {
                         if (sku.SerialNumList != null && sku.SerialNumList.Any())
                           addF060206.AddRange(sku.SerialNumList.Select(x => new F060206
                           {
                             DOC_ID = receipt.OrderCode,
                             CONTAINERCODE = container.ContainerCode,
                             SKUCODE = sku.SkuCode,
                             SKUQTY = 1,
                             BIN_CODE = sku.BinCode,
                             SERIAL_NO = x
                           }));
                         else
                           addF060206.Add(new F060206
                           {
                             DOC_ID = receipt.OrderCode,
                             CONTAINERCODE = container.ContainerCode,
                             SKUCODE = sku.SkuCode,
                             SKUQTY = Convert.ToInt32(sku.SkuQty),
                             BIN_CODE = sku.BinCode
                           });

                       });
                    });
                    #endregion
                    f060203Repo.BulkInsert(addF060203);
                    f060206Repo.BulkInsert(addF060206);
                    wmsTransation.Complete();

                    // 記錄成功的單號，用以再有同單號的不處理
                    successedReceiptCodes.Add(receipt.OrderCode);
                  }
                }
              }
              return new ApiResult { IsSuccessed = true };
            }
          }
        }, false,
        (fResult)=>
        {
          if (fResult.MsgCode == "99999")
          {
            if (_IsAddF075106DocIdList.Contains(receipt.OrderCode))
            {
              f075106Repo.DelF075106ByKey(dcCode, receipt.OrderCode);
              _IsAddF075106DocIdList = _IsAddF075106DocIdList.Where(x => x != receipt.OrderCode).ToList();
            }
            wmsTransation.Complete();
          }
          return null;
        });
        if (!currRes.IsSuccessed && currRes.MsgCode == "99999") //執行時有例外
					data.Add(new ApiResponse { MsgCode = "99999", MsgContent = currRes.MsgContent, No = receipt.OrderCode });
			});
			#endregion

			#region 組回傳資料
			res.SuccessCnt = successedReceiptCodes.Count;
			res.FailureCnt = receiptList.Count - successedReceiptCodes.Count;
			res.TotalCnt = receiptList.Count;

			res.IsSuccessed = !data.Any();
			res.MsgCode = "10005";
			res.MsgContent = string.Format(tacService.GetMsg("10005"),
					"出庫完成結果回傳",
					res.SuccessCnt,
					res.FailureCnt,
					res.TotalCnt);

			res.Data = data.Any() ? data : null;
			#endregion

			return res;
		}


		/// <summary>
		/// 資料處理2
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		/// <returns></returns>
		private ApiResult CheckOutWarehouseReceipt(string dcCode, string gupCode, string custCode, string wmsNo, OutWarehouseReceiptModel receipt, F060201 f060201)
		{
			ApiResult result = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			// 預設值設定
			data.AddRange((List<ApiResponse>)CheckDefaultSetting(dcCode, gupCode, custCode, wmsNo, receipt, f060201).Data);

			// 共用欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(dcCode, gupCode, custCode, wmsNo, receipt, f060201).Data);

			// 貨主自訂欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckCustomColumnType(dcCode, gupCode, custCode, wmsNo, receipt, f060201).Data);

			// 如果以上檢核成功
			if (!data.Any())
			{
				// 共用欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCommonColumnData(dcCode, gupCode, custCode, wmsNo, receipt, f060201).Data);

				// 貨主自訂欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCustomColumnValue(dcCode, gupCode, custCode, wmsNo, receipt, f060201).Data);

				// 如果以上檢核失敗
				if (!data.Any())
					_failCnt++;
			}
			else
				_failCnt++;

			result.IsSuccessed = !data.Any();
			result.Data = data;

			return result;
		}
		#endregion

		#region Protected 檢核
		/// <summary>
		/// 預設值設定
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckDefaultSetting(string dcCode, string gupCode, string custCode, string wmsNo, OutWarehouseReceiptModel receipt, F060201 f060201)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}

		/// <summary>
		/// 共用欄位格式檢核
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckColumnNotNullAndMaxLength(string dcCode, string gupCode, string custCode, string wmsNo, OutWarehouseReceiptModel receipt, F060201 f060201)
		{
			TransApiBaseService tacService = new TransApiBaseService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();
			string nullErrorMsg = tacService.GetMsg("20058");
			string formatErrorMsg = tacService.GetMsg("20059");

			#region 檢查出庫單欄位必填、最大長度
			// 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
			orderCheckColumnList.ForEach(column =>
			{
				// 必填
				if (!column.Nullable)
					if (!DataCheckHelper.CheckRequireColumn(receipt, column.Name))
						data.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = column.Name, MsgCode = "20058", MsgContent = string.Format(nullErrorMsg, receipt.OrderCode, column.Name) });

				// 判斷是否為日期格式(yyyy/MM/dd HH:mm:ss)字串
				var value = Convert.ToString(DataCheckHelper.GetRequireColumnValue(receipt, column.Name));
				if (column.IsDateTime && !string.IsNullOrWhiteSpace(value))
					if (!DataCheckHelper.CheckDataIsDateTime(receipt, column.Name))
						data.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = column.Name, MsgCode = "20050", MsgContent = string.Format(tacService.GetMsg("20050"), receipt.OrderCode, column.Name) });

				// 最大長度
				if (column.MaxLength > 0)
					if (!DataCheckHelper.CheckDataMaxLength(receipt, column.Name, column.MaxLength))
						data.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = column.Name, MsgCode = "20059", MsgContent = string.Format(formatErrorMsg, receipt.OrderCode, column.Name) });
			});

      if (!receipt.SkuList.Any() && receipt.IsException != 2)
        data.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = "SkuList", MsgCode = "20058", MsgContent = string.Format(nullErrorMsg, receipt.OrderCode, "SkuList") });
			#endregion

			#region 檢查明細欄位必填、最大長度
			if (receipt.SkuList.Any())
			{
				for (int i = 0; i < receipt.SkuList.Count; i++)
				{
					var currSku = receipt.SkuList[i];

					skuCheckColumnList.ForEach(o =>
					{
						// 必填
						if (!o.Nullable)
							if (!DataCheckHelper.CheckRequireColumn(currSku, o.Name))
								data.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = o.Name, MsgCode = "20058", MsgContent = string.Format(nullErrorMsg, $"{receipt.OrderCode}第{i + 1}筆明細", o.Name) });

						// 判斷是否為日期格式(yyyy/MM/dd)字串
						var value = Convert.ToString(DataCheckHelper.GetRequireColumnValue(currSku, o.Name));
						if (o.IsDate && !string.IsNullOrWhiteSpace(value))
							if (!DataCheckHelper.CheckDataIsDate(currSku, o.Name))
								data.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = o.Name, MsgCode = "20075", MsgContent = string.Format(tacService.GetMsg("20075"), $"{receipt.OrderCode}第{i + 1}筆明細", o.Name) });

						// 最大長度
						if (o.MaxLength > 0)
							if (!DataCheckHelper.CheckDataMaxLength(currSku, o.Name, o.MaxLength))
								data.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = o.Name, MsgCode = "20059", MsgContent = string.Format(formatErrorMsg, $"{receipt.OrderCode}第{i + 1}筆明細", o.Name) });
					});

					#region 檢驗儲位
					if (currSku.ShelfBinList!=null)
					{
						for (int j = 0; j < currSku.ShelfBinList.Count; j++)
						{
							var currShelfBin = currSku.ShelfBinList[j];

							shelfBinCheckColumnList.ForEach(p =>
							{
								// 必填
								if (!p.Nullable)
									if (!DataCheckHelper.CheckRequireColumn(currShelfBin, p.Name))
										data.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = p.Name, MsgCode = "20058", MsgContent = string.Format(nullErrorMsg, $"{receipt.OrderCode}第{i + 1}筆明細第{j + 1}筆儲位", p.Name) });

								// 最大長度
								if (p.MaxLength > 0)
									if (!DataCheckHelper.CheckDataMaxLength(currShelfBin, p.Name, p.MaxLength))
										data.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = p.Name, MsgCode = "20059", MsgContent = string.Format(formatErrorMsg, $"{receipt.OrderCode}第{i + 1}筆明細第{j + 1}筆儲位", p.Name) });
							});
						}
					}
					#endregion
				}
			}
			#endregion

			#region 檢查容器明細欄位必填、最大長度
			if (receipt.ContainerList.Any())
			{
				for (int i = 0; i < receipt.ContainerList.Count; i++)
				{
					var currContainer = receipt.ContainerList[i];

					containerCheckColumnList.ForEach(o =>
					{
						// 必填
						if (!o.Nullable)
							if (!DataCheckHelper.CheckRequireColumn(currContainer, o.Name))
								data.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = o.Name, MsgCode = "20058", MsgContent = string.Format(nullErrorMsg, $"{receipt.OrderCode}第{i + 1}筆容器明細", o.Name) });

						// 最大長度
						if (o.MaxLength > 0)
							if (!DataCheckHelper.CheckDataMaxLength(currContainer, o.Name, o.MaxLength))
								data.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = o.Name, MsgCode = "20059", MsgContent = string.Format(formatErrorMsg, $"{receipt.OrderCode}第{i + 1}筆容器明細", o.Name) });
					});

					#region 檢驗容器內品項內容
					for (int j = 0; j < currContainer.SkuList.Count; j++)
					{
						var currSku = currContainer.SkuList[j];

						containerSkuCheckColumnList.ForEach(p =>
						{
							// 必填
							if (!p.Nullable)
								if (!DataCheckHelper.CheckRequireColumn(currSku, p.Name))
									data.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = p.Name, MsgCode = "20058", MsgContent = string.Format(nullErrorMsg, $"{receipt.OrderCode}第{i + 1}筆容器明細第{j + 1}筆品項", p.Name) });

							// 最大長度
							if (p.MaxLength > 0)
								if (!DataCheckHelper.CheckDataMaxLength(currSku, p.Name, p.MaxLength))
									data.Add(new ApiResponse { No = receipt.OrderCode, ErrorColumn = p.Name, MsgCode = "20059", MsgContent = string.Format(formatErrorMsg, $"{receipt.OrderCode}第{i + 1}筆容器明細第{j + 1}筆品項", p.Name) });
						});
					}
					#endregion
				}
			}
			#endregion

			res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位格式檢核
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnType(string dcCode, string gupCode, string custCode, string wmsNo, OutWarehouseReceiptModel receipt, F060201 f060201)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}

		/// <summary>
		/// 共用欄位資料檢核
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckCommonColumnData(string dcCode, string gupCode, string custCode, string wmsNo, OutWarehouseReceiptModel receipt, F060201 f060201)
		{
			CheckOutWarehouseReceiptService ciwService = new CheckOutWarehouseReceiptService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			#region 出庫單欄位資料檢核

			// 檢查資料庫任務單號是否存在
			var isAdd = ciwService.CheckDocExist(data, receipt, dcCode);
			if (isAdd)
				_IsAddF075106DocIdList.Add(receipt.OrderCode);

			// 檢查單號是否存在、是否已取消、是否已結案
			ciwService.CheckReceiptOrderExistAndStatus(data, wmsNo, receipt, _f151001List, _f050801List, _f051201List);

			// 檢查出庫單狀態
			ciwService.CheckStatus(data, receipt);

			// 檢查是否異常
			ciwService.CheckIsException(data, receipt);

			// 檢查品項數與明細筆數是否相同
			ciwService.CheckSkuTotalEqualDetailCnt(data, receipt);

			// 檢查明細資料是否有誤
			ciwService.CheckSkuCountEqualDatas(data, wmsNo, receipt, _f151002List, _f060202ListByO, _f060202ListByP);

      if (data.Any())
      {
        res.Data = data;
        return res;
      }
      #endregion

			#region 明細欄位資料檢核

			for (int index = 0; index < receipt.SkuList.Count; index++)
			{
				var sku = receipt.SkuList[index];
				var currF151002 = _f151002List.Where(x => x.ALLOCATION_NO == wmsNo && x.ALLOCATION_SEQ == sku.RowNum).FirstOrDefault();
				var currF051202ByO = _f060202ListByO.Where(x => x.PICK_ORD_NO == f060201.PICK_NO && x.WMS_ORD_NO == wmsNo && x.PICK_ORD_SEQ == Convert.ToString(sku.RowNum).PadLeft(4, '0')).FirstOrDefault();
				var currF051203ByP = _f060202ListByP.Where(x => x.PICK_ORD_NO == f060201.PICK_NO && x.TTL_PICK_SEQ == Convert.ToString(sku.RowNum).PadLeft(4, '0')).FirstOrDefault();

				// 檢查該ROWNUM是否與資料庫對應
				ciwService.CheckRowNumIsExist(data, wmsNo, sku, receipt.OrderCode, index, currF151002, currF051202ByO, currF051203ByP);

				// 檢查該ITEMCODE是否與資料庫對應
				ciwService.CheckSkuCodeIsExist(data, wmsNo, sku, receipt.OrderCode, index, currF151002, currF051202ByO, currF051203ByP);

				// 檢查[商品等級]是否設定錯誤
				ciwService.CheckSkuLevel(data, sku, receipt.OrderCode, index);

				// 檢查[實際出庫數量]是否有大於0
				ciwService.CheckSkuQty(data, sku, receipt.OrderCode, index);

				// 檢查[實際出庫數量]是否大於[預計出庫量]
				ciwService.CheckSkuQtyAndSkuPlanQty(data, sku, receipt.OrderCode, index);

				// 檢查[實際出庫數量]、[預計出庫數量]是否超過調撥單上架數
				ciwService.CheckSkuQtyAndSkuPlanQtyExceedTarQty(data, wmsNo, sku, receipt.OrderCode, index, currF151002, currF051202ByO, currF051203ByP);

				// 檢查該明細是否已完成
				ciwService.CheckSkuIsFinish(data, wmsNo, sku, receipt.OrderCode, index, currF151002, currF051202ByO, currF051203ByP);

				//// 檢查該明細效期
				//ciwService.CheckSkuExpiryDate(data, wmsNo, sku, receipt.OrderCode, index, currF151002, currF051202ByO, currF051202ByP);

				//// 檢查該明細批號
				//ciwService.CheckSkuOutBatchCode(data, wmsNo, sku, receipt.OrderCode, index, currF151002, currF051202ByO, currF051202ByP);

				// 檢查該明細序號長度是否與實際出庫數相同
				ciwService.CheckSkuSerialNumEquelSkuQty(data, sku, receipt.OrderCode, index);

				// 檢查該明細序號是否存在
				ciwService.CheckSkuSerialNumIsExist(data, sku, receipt.OrderCode, index, _serialNoList);

        // 序號重複檢查？
			}

      if (data.Any())
      {
        res.Data = data;
        return res;
      }
      #endregion

      #region 容器欄位資料檢核
      if (f060201.PICK_NO.StartsWith("P"))
      {
        var curF051201 = _f051201List.FirstOrDefault(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PICK_ORD_NO == wmsNo);
        if (curF051201 != null && curF051201.NEXT_STEP == ((int)NextStep.CrossAllotPier).ToString())
        {
          foreach (var container in receipt.ContainerList)
          {
            if (container.SkuList.SelectMany(x => x.SerialNumList).Any())
            { 
              //檢查商品序號是否存在
              //檢查商品序號是否為在庫序號(F2501.STATUS=A1)
              //稽核出庫不會走包裝作業流程，後面不會檢查到序號狀態，這邊要檢查
              ciwService.CheckContainerSerialNosExists(data, receipt.OrderCode, container, _serialNoList);

              //檢查序號數量(去重複)是否與容器商品裝箱數量相同
              ciwService.CheckContainerSerialNosQty(data, receipt.OrderCode, container);
            }
          }

          //檢查揀貨明細商品總實際揀貨數量是否等於加總各箱該商品裝箱數量
          ciwService.CheckContainerSkuQty(data, receipt.OrderCode, receipt.SkuList, receipt.ContainerList.SelectMany(x => x.SkuList).ToList());
        }
      }
      #endregion
      res.Data = data;

      return res;
		}

		/// <summary>
		/// 貨主自訂欄位資料檢核
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNo"></param>
		/// <param name="receipt"></param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnValue(string dcCode, string gupCode, string custCode, string wmsNo, OutWarehouseReceiptModel receipt, F060201 f060201)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}
    #endregion

  }
}
