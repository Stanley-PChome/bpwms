using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.Shared.TransApiServices.Common
{
	/// <summary>
	/// 批次商品進倉資料
	/// </summary>
	public class CommonWarehouseInsService
	{

		public CommonWarehouseInsService(WmsTransaction wmsTransation)
		{
			_wmsTransation = wmsTransation;
		}

		#region 基本檢核欄位設定
		// 刪除
		private List<ApiCkeckColumnModel> delWarehouseInsCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "CustInNo",    Type = typeof(string),   MaxLength = 25, Nullable = false },
			new ApiCkeckColumnModel{  Name = "ProcFlag",    Type = typeof(string),   MaxLength = 1, Nullable = false }
		};

    /// <summary>
    /// 修改資料要檢查的欄位
    /// </summary>
    private List<ApiCkeckColumnModel> editWarehouseInsCheckColumnList = new List<ApiCkeckColumnModel>
    {
      new ApiCkeckColumnModel{  Name = "CustInNo",      Type = typeof(string),    MaxLength = 25,   Nullable = false },
      new ApiCkeckColumnModel{  Name = "ProcFlag",      Type = typeof(string),    MaxLength = 1,    Nullable = false },
      new ApiCkeckColumnModel{  Name = "FastPassType",  Type = typeof(string),    MaxLength = 1,    Nullable = false },
    };

    // 新增
    private List<ApiCkeckColumnModel> warehouseInsCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "CustInNo",        Type = typeof(string),   MaxLength = 25, Nullable = false },
			new ApiCkeckColumnModel{  Name = "CustCrtDate",     Type = typeof(DateTime), MaxLength = 0,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "InDate",          Type = typeof(DateTime), MaxLength = 0,  Nullable = false },
			new ApiCkeckColumnModel{  Name = "VnrCode",         Type = typeof(string),   MaxLength = 20, Nullable = false },
			new ApiCkeckColumnModel{  Name = "PoNo",            Type = typeof(string),   MaxLength = 20 },
			new ApiCkeckColumnModel{  Name = "InProp",          Type = typeof(string),   MaxLength = 2 , Nullable = false },
			new ApiCkeckColumnModel{  Name = "TranCode",        Type = typeof(string),   MaxLength = 10 },
			new ApiCkeckColumnModel{  Name = "Memo",            Type = typeof(string),   MaxLength = 200 },
			new ApiCkeckColumnModel{  Name = "ProcFlag",        Type = typeof(string),   MaxLength = 1, Nullable = false },
			new ApiCkeckColumnModel{  Name = "BatchNo",         Type = typeof(string),   MaxLength = 50 },
			new ApiCkeckColumnModel{  Name = "CustCost",        Type = typeof(string),   MaxLength = 10 },
			new ApiCkeckColumnModel{  Name = "FastPassType",    Type = typeof(string),   MaxLength = 1, Nullable = false },
			new ApiCkeckColumnModel{  Name = "BoookingInPeriod",Type = typeof(string),   MaxLength = 1, Nullable = false }
		};

		private List<ApiCkeckColumnModel> warehouseInDetailCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "ItemSeq",     Type = typeof(string),      MaxLength = 5, Nullable = false },
			new ApiCkeckColumnModel{  Name = "ItemCode",    Type = typeof(string),      MaxLength = 20, Nullable = false },
			new ApiCkeckColumnModel{  Name = "InQty",       Type = typeof(int),         MaxLength = 9, Nullable = false },
			new ApiCkeckColumnModel{  Name = "ValidDate",   Type = typeof(DateTime),    MaxLength = 0 },
			new ApiCkeckColumnModel{  Name = "MakeNo",      Type = typeof(string),      MaxLength = 20 },
			new ApiCkeckColumnModel{  Name = "SnList",      Type = typeof(string[])}
		};

		private List<ApiCkeckColumnModel> warehouseInContainerCheckColumnList = new List<ApiCkeckColumnModel>
		{
			new ApiCkeckColumnModel{  Name = "ContainerCode",    Type = typeof(string),      MaxLength = 32, Nullable = false },
			new ApiCkeckColumnModel{  Name = "InQty",            Type = typeof(int),         MaxLength = 9 , Nullable = false },
			new ApiCkeckColumnModel{  Name = "ValidDate",        Type = typeof(DateTime),    MaxLength = 0 , Nullable = false },
			new ApiCkeckColumnModel{  Name = "SnList",           Type = typeof(string[])}
		};
		#endregion

		#region Private Property
		private WmsTransaction _wmsTransation;
		private TransApiBaseService tacService = new TransApiBaseService();

		/// <summary>
		/// 商品資料清單
		/// </summary>
		private List<F1903> _f1903List;

		/// <summary>
		/// 廠商資料清單
		/// </summary>
		private List<F1908> _vnrList;

		/// <summary>
		/// 交易類型資料清單
		/// </summary>
		private List<F000903> _tranCodeList;

		/// <summary>
		/// 第三方系統單號已產生WMS訂單
		/// </summary>
		private List<ThirdPartOrders> _thirdPartOrdersList;

		/// <summary>
		/// 已存在的Sn (Container)
		/// </summary>
		private List<string> _thirdPartContainerSnList;

		/// <summary>
		/// 已存在的使用中容器清單
		/// </summary>
		private List<F0701> _thirdPartUsingContainerList;

		/// <summary>
		/// 工作中的容器
		/// </summary>
		private List<F0701> _f0701List = new List<F0701>();

		/// <summary>
		/// 混和型容器明細檔
		/// </summary>
		private List<F070104> _f070104List = new List<F070104>();

    /// <summary>
    /// 混和型容器內序號資料
    /// </summary>
    private List<F07010401> _f07010401List = new List<F07010401>();

    /// <summary>
    /// 進貨單主檔
    /// </summary>
    private List<F010201> _f010201List = new List<F010201>();

		/// <summary>
		/// 進貨單明細
		/// </summary>
		private List<F010202> _f010202List = new List<F010202>();

		/// <summary>
		/// 進倉序號主檔
		/// </summary>
		private List<F020301> _f020301List = new List<F020301>();
		/// <summary>
		/// 進倉序號明細檔
		/// </summary>
		private List<F020302> _f020302List = new List<F020302>();

		/// <summary>
		/// 失敗進倉單單數
		/// </summary>
		private int _failCnt = 0;

		/// <summary>
		/// 暫存訊息池清單
		/// </summary>
		private List<AddMessageReq> _addMessageTempList = new List<AddMessageReq>();

		/// <summary>
		/// 紀錄有新增過F075101的CUST_ORD_NO，用以若檢核失敗 找出是否有新增，用以刪除
		/// </summary>
		private List<string> _IsAddF075101CustOrdNoList = new List<string>();
		#endregion

		#region Main Method
		/// <summary>
		/// Func1
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult RecevieApiDatas(PostCreateWarehousesReq req)
		{
			CheckTransApiService ctaService = new CheckTransApiService();
			TransApiBaseService tacService = new TransApiBaseService();
			SharedService sharedService = new SharedService();
			CommonService commonService = new CommonService();
			ApiResult res = new ApiResult { IsSuccessed = true };

			#region 資料檢核1

			// 檢核物流中心 必填、是否存在
			ctaService.CheckDcCode(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核貨主編號 必填、是否存在
			ctaService.CheckCustCode(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核Result
			ctaService.CheckResult(ref res, req);
			if (!res.IsSuccessed)
				return res;

			// 檢核WarehouseIns
			if (req.Result.WarehouseIns == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = tacService.GetMsg("20056") };

			// 檢核資料筆數
			int reqTotal = req.Result.Total != null ? Convert.ToInt32(req.Result.Total) : 0;
			if (req.Result.WarehouseIns == null || (req.Result.WarehouseIns != null && !tacService.CheckDataCount(reqTotal, req.Result.WarehouseIns.Count)))
				return new ApiResult { IsSuccessed = false, MsgCode = "20054", MsgContent = string.Format(tacService.GetMsg("20054"), reqTotal, req.Result.WarehouseIns.Count) };

			// 檢核總明細筆數是否超過[進倉明細最大筆數]筆
			int whdMaxCnt = Convert.ToInt32(commonService.GetSysGlobalValue("WHDMaxCnt"));
			int detailTotalCnt = req.Result.WarehouseIns.Where(x => x.WarehouseInDetails != null).Sum(x => x.WarehouseInDetails.Count);
			if (detailTotalCnt > whdMaxCnt)
				return new ApiResult { IsSuccessed = false, MsgCode = "20055", MsgContent = string.Format(tacService.GetMsg("20055"), detailTotalCnt) };
			#endregion

			// 取得業主編號
			string gupCode = commonService.GetGupCode(req.CustCode);

      // 資料處理1
      return ProcessApiDatas(req.DcCode, gupCode, req.CustCode, req.Result.WarehouseIns);
		}

		/// <summary>
		/// 資料處理1
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="warehouseInsList">進倉單資料物件清單</param>
		/// <returns></returns>
		public ApiResult ProcessApiDatas(string dcCode, string gupCode, string custCode, List<PostCreateWarehouseInsModel> warehouseInsList)
		{
			F075101Repository f075101Repo = new F075101Repository(Schemas.CoreSchema);
			F010201Repository f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransation);
			F010202Repository f010202Repo = new F010202Repository(Schemas.CoreSchema, _wmsTransation);
			F020301Repository f020301Repo = new F020301Repository(Schemas.CoreSchema, _wmsTransation);
			F020302Repository f020302Repo = new F020302Repository(Schemas.CoreSchema, _wmsTransation);
			F0701Repository f0701Repo = new F0701Repository(Schemas.CoreSchema, _wmsTransation);
			F070104Repository f070104Repo = new F070104Repository(Schemas.CoreSchema, _wmsTransation);
			F07010401Repository f07010401Repo = new F07010401Repository(Schemas.CoreSchema, _wmsTransation);
			TransApiBaseService tacService = new TransApiBaseService();
			WarehouseInService wiService = new WarehouseInService(_wmsTransation);
			CommonService commonService = new CommonService();
			SharedService sharedService = new SharedService(_wmsTransation);
			int insertCnt = 0;

      #region 序號、容器、貨主單號、批號轉大寫
      warehouseInsList.ForEach(
      a =>
      {
        if (!string.IsNullOrWhiteSpace(a.CustInNo))
          a.CustInNo = a.CustInNo?.ToUpper();

        a.WarehouseInDetails.ForEach(
        b =>
        {
          if (b.SnList != null)
            for (int i = 0; i < b.SnList.Count(); i++)
              b.SnList[i] = b.SnList[i]?.ToUpper();

          if (b.MakeNo != null)
            b.MakeNo = b.MakeNo?.ToUpper();

          if (b.ContainerDatas != null)
          {
            b.ContainerDatas.ForEach(c =>
            {
              //序號轉大寫
              if (c.SnList != null)
                for (int i = 0; i < c.SnList.Count; i++)
                  c.SnList[i] = c.SnList[i]?.ToUpper();
              //容器轉大寫
              c.ContainerCode = c.ContainerCode?.ToUpper();

            });

          }
        });
      });
      #endregion 序號、容器轉大寫

      #region 取得資料[以下清單設為目前服務的 private property]
      // 容器內的SerialNo
      var containerSnList = warehouseInsList.Where(x => x.WarehouseInDetails != null).SelectMany(x => x.WarehouseInDetails
			.Where(z => z.ContainerDatas != null).SelectMany(z => z.ContainerDatas
			.Where(y => y.SnList != null).SelectMany(y => y.SnList))).Distinct().ToList();

      // 容器編號
      var containerCodeList = warehouseInsList.Where(x => x.WarehouseInDetails != null).SelectMany(x => x.WarehouseInDetails
			.Where(z => z.ContainerDatas != null).SelectMany(z => z.ContainerDatas
			.Where(y => !string.IsNullOrWhiteSpace(y.ContainerCode)).Select(y => y.ContainerCode))).Distinct().ToList();

			// 取得商品資料
			var itemCodes = warehouseInsList.Where(x => x.WarehouseInDetails != null).SelectMany(x => x.WarehouseInDetails.Select(z => z.ItemCode)).Distinct().ToList();
			_f1903List = commonService.GetProductList(gupCode, custCode, itemCodes);

			// 取得廠商資料
			_vnrList = commonService.GetVnrList(gupCode, custCode, warehouseInsList.Where(x => x.VnrCode != null).Select(x => x.VnrCode).Distinct().ToList());

			// 取得交易類型資料 [A]
			_tranCodeList = commonService.GetTranCodeList("A");

			// 貨主單號已產生WMS進倉單
			_thirdPartOrdersList = f010201Repo.GetThirdPartOrdersData(dcCode, gupCode, custCode, warehouseInsList);

			// 取出所有Container下的SnList找出已存在的Sn (Container)
			_thirdPartContainerSnList = f020302Repo.GetDatasBySns(dcCode, gupCode, custCode, "0", containerSnList);

			// 取得所有ContainerCode
			_thirdPartUsingContainerList = f0701Repo.GetDataByContainerCodes("In", "2", containerCodeList).ToList();
			
			#endregion

			#region Foreach [進倉單資料物件] 檢核
			var res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			// 避免若同一批參數有重複單號會把之前成功寫入的F075101的刪除，所以取得重複數
			var paramCustOrdNos = warehouseInsList.Select(x => x.CustInNo).GroupBy(x => x).Select(x => new { CustInNo = x.Key, Cnt = x.Count() }).ToList();

			// Foreach[進倉單資料物件] in <參數4> 
			warehouseInsList.ForEach(warehouseIns =>
			{
				// 資料處理2
				var res1 = CheckWarehouseIns(dcCode, gupCode, custCode, warehouseIns);

				if (!res1.IsSuccessed)
				{
					data.AddRange((List<ApiResponse>)res1.Data);

					// 若驗證失敗，不取消進倉單，所以將驗證失敗的進倉單剃除
					_thirdPartOrdersList = _thirdPartOrdersList.Where(x => x.CUST_ORD_NO != warehouseIns.CustInNo).ToList();
					
					var currCustOrdNo = paramCustOrdNos.Where(x => x.CustInNo == warehouseIns.CustInNo).FirstOrDefault();

					// 若驗證失敗 刪除新增的F075101
					if (_IsAddF075101CustOrdNoList.Contains(warehouseIns.CustInNo) && currCustOrdNo != null && currCustOrdNo.Cnt == 1)
					{
						f075101Repo.DelF075101ByKey(custCode, warehouseIns.CustInNo);
						_IsAddF075101CustOrdNoList = _IsAddF075101CustOrdNoList.Where(x => x != warehouseIns.CustInNo).ToList();
					}
				}
				else
				{
					// 將通過的容器內序號清單回寫到已存在序號清單內，讓接下來的進倉單資料不會寫入重複的序號
					var serialNoList = warehouseIns.WarehouseInDetails
            .Where(x => x.ContainerDatas != null)
            .SelectMany(x => x.ContainerDatas)
            .Where(x => x.SnList != null)
            .SelectMany(x => x.SnList)
            .ToList();
					if (serialNoList.Any())
						_thirdPartContainerSnList.AddRange(serialNoList);
				}
			});
			#endregion

			#region 處理純新增進倉單
			// 暫存新增的進倉單清單排除[貨主單號已產生WMS訂單]
			var addF010201List = _f010201List.Where(x => !_thirdPartOrdersList.Any(z => z.CUST_ORD_NO == x.CUST_ORD_NO)).ToList();

			if (addF010201List.Any())
			{
				// 暫存新增的進倉單明細清單.Contain(addF010201List.STOCK_NO)
				var addF010202List = _f010202List.Where(x => addF010201List.Select(z => z.STOCK_NO).Contains(x.STOCK_NO)).ToList();
				var addF070104List = _f070104List.Where(x => addF010201List.Select(y => y.STOCK_NO).Contains(x.WMS_NO)).ToList();
        var addF07010401List = _f07010401List.Where(x => addF070104List.Select(y => y.ID).Contains(x.F070104_ID)).ToList();
        var addF0701List = _f0701List.Where(x => addF070104List.Select(z => z.CONTAINER_CODE).Contains(x.CONTAINER_CODE)).ToList();
				var addF020301List = _f020301List.Where(x => addF010201List.Select(y => y.STOCK_NO).Contains(x.FILE_NAME)).ToList();
				var addF020302List = _f020302List.Where(x => _f020301List.Select(y => y.FILE_NAME).Contains(x.FILE_NAME)).ToList();

				addF010201List.ForEach(addF010201 =>
				{
					// [取得進倉單號]
					var stockNo = sharedService.GetNewOrdCode("A");

					// 寫入行事曆訊息池
					AddMessagePool(_wmsTransation, addF010201.STOCK_NO, stockNo);

					addF010202List.Where(x => x.STOCK_NO == addF010201.STOCK_NO).ToList().ForEach(addF010202 =>
					{
						addF010202.STOCK_NO = stockNo;
					});

					addF070104List.Where(x => x.WMS_NO == addF010201.STOCK_NO).ToList().ForEach(addF070104 =>
					{
						addF070104.WMS_NO = stockNo;
						addF070104.SOURCE_NO = stockNo;
					});

					if (addF020301List.Any() && addF020302List.Any())
					{
						// 建構FILE_NAME
						var fileName = $"SYSCHK99_{stockNo}01";

						addF020301List.Where(x => x.FILE_NAME == addF010201.STOCK_NO).ToList().ForEach(addF020301 =>
						{
							addF020301.FILE_NAME = fileName;
							addF020301.PURCHASE_NO = string.IsNullOrWhiteSpace(addF010201.SHOP_NO) ? stockNo : addF010201.SHOP_NO;
						});
						// 進倉單有提供採購單號SHOP_NO且有值F020302.PO_NO=SHOP_NO
						if (!string.IsNullOrWhiteSpace(addF010201.SHOP_NO))
						{
							addF020302List.Where(x => x.FILE_NAME == addF010201.STOCK_NO).ToList().ForEach(addF020302 =>
							{
								addF020302.FILE_NAME = fileName;
								addF020302.PO_NO = addF010201.SHOP_NO;
							});
						}
						// 若SHOP_NO無值且有序號清單，此時SHOP_NO = 進倉單號，F020301.PO_NO = 進倉單號
						if (string.IsNullOrWhiteSpace(addF010201.SHOP_NO) && addF020302List.Any())
						{
							addF020302List.Where(x => x.FILE_NAME == addF010201.STOCK_NO).ToList().ForEach(addF020302 =>
							{
								addF020302.FILE_NAME = fileName;
								addF020302.PO_NO = stockNo;
							});
						}
					}

					addF010201.STOCK_NO = stockNo;
					addF010201.SHOP_NO = string.IsNullOrWhiteSpace(addF010201.SHOP_NO) ? stockNo : addF010201.SHOP_NO;

				});

				if (addF010201List.Any())
				{
					f010201Repo.BulkInsert(addF010201List);
					insertCnt = addF010201List.Count;
				}

				if (addF010202List.Any())
					f010202Repo.BulkInsert(addF010202List);

				if (addF020301List.Any())
					f020301Repo.BulkInsert(addF020301List);

				if (addF020302List.Any())
					f020302Repo.BulkInsert(addF020302List);

				if (addF0701List.Any())
					f0701Repo.BulkInsert(addF0701List);

				if (addF070104List.Any())
					f070104Repo.BulkInsert(addF070104List);

        if (addF07010401List.Any())
          f07010401Repo.BulkInsert(addF07010401List);

        if (addF010201List.Any() || addF010202List.Any() || addF020301List.Any() || addF020302List.Any() || addF070104List.Any())
					_wmsTransation.Complete();
			}
			#endregion

			#region 已存在進倉單(by 每一筆進倉單commit)

			// 暫存新增的進倉單清單篩選含[貨主單號已產生WMS進倉單]
			var cancelF010201List = _f010201List.Where(x => _thirdPartOrdersList.Any(z => z.CUST_ORD_NO == x.CUST_ORD_NO)).ToList();

      // Foreach[找到的WMS單] in [貨主單號已產生WMS進倉單]
      _thirdPartOrdersList.ForEach(o =>
      {
        if (o.PROC_FLAG == "U")
        {
          var updWmsTransation = new WmsTransaction();
          var updf010201Repo = new F010201Repository(Schemas.CoreSchema, updWmsTransation);
          if (o.STATUS == "2" || o.STATUS == "9")
          {
            // 此來源單據{0}已在處理中或處理完成，無法刪除或更新單據。請聯絡現場人員。
            data.Add(new ApiResponse { MsgCode = "20073", MsgContent = string.Format(tacService.GetMsg("20073"), o.CUST_ORD_NO), No = o.CUST_ORD_NO });
            _failCnt++;
          }
          else
          {
            var fastPassType = warehouseInsList.FirstOrDefault(x => x.CustInNo == o.CUST_ORD_NO)?.FastPassType;
            if (!string.IsNullOrWhiteSpace(fastPassType))
              updf010201Repo.UpdateFastPassType(dcCode, gupCode, custCode, o.CUST_ORD_NO, fastPassType);
          }
          updWmsTransation.Complete();
        }
        else if (o.PROC_FLAG == "D")
        {
          var wmsTransation2 = new WmsTransaction();
          sharedService = new SharedService(wmsTransation2);
          wiService = new WarehouseInService(wmsTransation2);

          // 取消進倉單
          var isOk = wiService.CancelNotProcessWarehouseIn(dcCode, gupCode, custCode, o.STOCK_NO, o.PROC_FLAG, o.CUST_COST);

          // var cancelF010201 = CancelF010201List.Where(x => CUST_ORD_NO =[找到的WMS單].CUST_ORD_NO)
          var cancelF010201 = cancelF010201List.Where(x => x.CUST_ORD_NO == o.CUST_ORD_NO).SingleOrDefault();

          if (isOk)
          {
            if (cancelF010201 == null)
              // [轉入WMS取消單據]取消進倉單號[{0}]
              sharedService.AddMessagePool("9", dcCode, gupCode, custCode, "API20857", string.Format(tacService.GetMsg("20857"), o.STOCK_NO), "", "0", "SCH");
          }
          else
          {
            // 此來源單據{0}已在處理中或處理完成，無法刪除或更新單據。請聯絡現場人員。
            data.Add(new ApiResponse { MsgCode = "20073", MsgContent = string.Format(tacService.GetMsg("20073"), o.CUST_ORD_NO), No = o.CUST_ORD_NO });

            _failCnt++;

            if (cancelF010201 != null)
            {
              // 暫存新增的進倉單清單.Remove(cancelF010201)
              _f010201List.Remove(cancelF010201);

              // var cancelF010202 = 暫存新增的進倉單明細清單.Contain(cancelF010201.STOCK_NO)
              var cancelF010202 = _f010202List.Where(x => x.STOCK_NO == cancelF010201.STOCK_NO);

              // 暫存新增的進倉明細清單.Except(cancelF010202)
              _f010202List = _f010202List.Except(cancelF010202).ToList();
            }
          }

          wmsTransation2.Complete();
        }
			});
			#endregion

			#region 寫入行事曆訊息池
			var wmsTransation3 = new WmsTransaction();
			sharedService = new SharedService(wmsTransation3);

			// 取得訊息內容 20856
			int total = warehouseInsList.Count;
			int successedCnt = total - _failCnt;

			// [轉入WMS建立單據]進倉單成功{0}張;異常{1}張;失敗{2}張;共{3}張
			string msgContent = string.Format(tacService.GetMsg("20856"),
					successedCnt,
					_failCnt,
					total);

			// 寫入行事曆訊息池
			sharedService.AddMessagePool("9", dcCode, gupCode, custCode, "API20856", msgContent, "", "0", "SCH");

			wmsTransation3.Complete();

			#endregion

			res.IsSuccessed = !data.Any();
			res.MsgCode = "20856";
			res.MsgContent = msgContent;
			res.InsertCnt = insertCnt;
			res.UpdateCnt = total - insertCnt - _failCnt;
			res.FailureCnt = _failCnt;
			res.TotalCnt = total;
			res.Data = data.Any() ? data : null;

			return res;
		}

		/// <summary>
		/// 寫入行事曆訊息池
		/// </summary>
		/// <param name="guid"></param>
		/// <param name="stockNo"></param>
		private void AddMessagePool(WmsTransaction wmsTransaction, string guid, string stockNo)
		{
			SharedService sharedService = new SharedService(wmsTransaction);

			// 寫入行事曆訊息池
			var addMessageList = _addMessageTempList.Where(x => x.Guid == guid).ToList();
			addMessageList.ForEach(o =>
			{
				o.MessageContent = o.MessageContent.Replace(guid, stockNo);
				sharedService.AddMessagePool(o.TicketType, o.DcCode, o.GupCode, o.CustCode, o.MsgNo, o.MessageContent, o.NotifyOrdNo, o.TargetType, o.TargetCode);
			});
		}

		/// <summary>
		/// 資料處理2
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="warehouseIns">進倉單資料物件</param>
		/// <returns></returns>
		private ApiResult CheckWarehouseIns(string dcCode, string gupCode, string custCode, PostCreateWarehouseInsModel warehouseIns)
		{
			ApiResult result = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			// 預設值設定
			data.AddRange((List<ApiResponse>)CheckDefaultSetting(dcCode, gupCode, custCode, warehouseIns).Data);

			// 共用欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(dcCode, gupCode, custCode, warehouseIns).Data);

			// 貨主自訂欄位格式檢核
			data.AddRange((List<ApiResponse>)CheckCustomColumnType(dcCode, gupCode, custCode, warehouseIns).Data);

			// 如果以上檢核成功
			if (!data.Any())
			{
				// 共用欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCommonColumnData(dcCode, gupCode, custCode, warehouseIns).Data);

				// 貨主自訂欄位資料檢核
				data.AddRange((List<ApiResponse>)CheckCustomColumnValue(dcCode, gupCode, custCode, warehouseIns).Data);

				// 如果以上檢核成功
				if (!data.Any())
				{
					// 產生WMS資料
					CreateWarehouseIns(dcCode, gupCode, custCode, warehouseIns);
				}
				else
				{
					_failCnt++;
				}
			}
			else
			{
				_failCnt++;
			}

			result.IsSuccessed = !data.Any();
			result.Data = data;

			return result;
		}

		/// <summary>
		/// 產生WMS資料
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="warehouseIns">進倉單資料物件</param>
		private void CreateWarehouseIns(string dcCode, string gupCode, string custCode, PostCreateWarehouseInsModel warehouseIns)
		{
			// 如果ProcFlag<> D 才往下執行
			if (warehouseIns.ProcFlag != "D")
			{
				// 建立進倉單主檔F010201
				F010201 f010201 = CreateF010201(dcCode, gupCode, custCode, warehouseIns);

				// 建立進倉單明細檔F010202
				List<F010202> f010202List = CreateF010202List(warehouseIns, f010201);

				if (warehouseIns.WarehouseInDetails != null || (warehouseIns.WarehouseInDetails != null && warehouseIns.WarehouseInDetails.Any()))
				{
					foreach (var warehouseInDetail in warehouseIns.WarehouseInDetails)
					{
						if (warehouseInDetail.SnList != null && warehouseInDetail.SnList.Any())
						{
							List<F020302> f020302List = new List<F020302>();

							//建立進倉單序號主檔F020301
							F020301 f020301List = CreateF020301List(f010201);

							//建立進倉單序號明細檔F020302
							f020302List.AddRange(CreateF020302List(warehouseInDetail, f010201, warehouseInDetail.SnList));

							if (!_f020301List.Where(x => x.FILE_NAME == f020301List.FILE_NAME).Any())
								_f020301List.Add(f020301List);

							_f020302List.AddRange(f020302List);
						}

						// 建立F0701、F070104
						if (warehouseIns.CustCost == "MoveIn" && warehouseInDetail.ContainerDatas != null && warehouseInDetail.ContainerDatas.Any())
							CreateF0701WithF070104List(dcCode, gupCode, custCode, f010201.STOCK_NO, warehouseInDetail);
					}
				}

				_f010201List.Add(f010201);
				_f010202List.AddRange(f010202List);

			}
		}
		#endregion

		#region Protected 檢核
		/// <summary>
		/// 預設值設定
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="warehouseIns">進倉單資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckDefaultSetting(string dcCode, string gupCode, string custCode, PostCreateWarehouseInsModel warehouseIns)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}

		private void CheckWarehouseInsColumn(ref List<ApiResponse> data, PostCreateWarehouseInsModel warehouseIns, List<ApiCkeckColumnModel> columns)
		{
			List<string> warehouseInsIsNullList = new List<string>();
			List<ApiCkeckColumnModel> warehouseInsIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();

			// 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List

			columns.ForEach(column =>
			{
				// 必填
				if (!column.Nullable)
				{
					if (!DataCheckHelper.CheckRequireColumn(warehouseIns, column.Name))
						warehouseInsIsNullList.Add(column.Name);
				}

				// 最大長度
				if (column.MaxLength > 0)
				{
					if (!DataCheckHelper.CheckDataMaxLength(warehouseIns, column.Name, column.MaxLength))
						warehouseInsIsExceedMaxLenthList.Add(column);
				}
			});

			// 必填訊息
			if (warehouseInsIsNullList.Any())
				// 回傳訊息內容[20058, < 參數4 >.CustInNo, 必填欄位未填寫的欄位清單(格式:[欄位名稱1]、[欄位名稱2])]
				data.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20058", MsgContent = string.Format(tacService.GetMsg("20058"), warehouseIns.CustInNo, string.Join("、", warehouseInsIsNullList)) });

			// 最大長度訊息
			if (warehouseInsIsExceedMaxLenthList.Any())
			{
				List<string> errorMsgList = warehouseInsIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();

				string errorMsg = string.Join("、", errorMsgList);

				// 檢查進倉單欄位格式(參考2.8.2),如果檢核失敗，回傳 & 訊息內容[20059, < 參數4 >.CustInNo, 欄位格式錯誤清單(格式:[欄位名稱1]格式錯誤必須為X1、[欄位名稱2]格式錯誤必須為X2)]
				data.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20059", MsgContent = string.Format(tacService.GetMsg("20059"), warehouseIns.CustInNo, errorMsg) });
			}
		}

		/// <summary>
		/// 共用欄位格式檢核
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="warehouseIns">進倉單資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckColumnNotNullAndMaxLength(string dcCode, string gupCode, string custCode, PostCreateWarehouseInsModel warehouseIns)
		{
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();

			#region 定義需檢核欄位、必填、型態、長度
			if (warehouseIns.ProcFlag == "D")
			{
				CheckWarehouseInsColumn(ref data, warehouseIns, delWarehouseInsCheckColumnList);
			}
      else if (warehouseIns.ProcFlag == "U")
      {
        CheckWarehouseInsColumn(ref data, warehouseIns, editWarehouseInsCheckColumnList);
      }
      else
      {
        CheckWarehouseInsColumn(ref data, warehouseIns, warehouseInsCheckColumnList);

        if (warehouseIns.CustCost == "MoveIn")
          warehouseInDetailCheckColumnList.Where(x => x.Name == "MakeNo").First().Nullable = false;
        else
          warehouseInDetailCheckColumnList.Where(x => x.Name == "MakeNo").First().Nullable = true;

        #region 檢查進倉單明細欄位必填、最大長度
        List<string> warehouseInDetailIsNullList;
        List<ApiCkeckColumnModel> warehouseInDetailIsExceedMaxLenthList;

        if (warehouseIns.WarehouseInDetails != null && warehouseIns.WarehouseInDetails.Any())
        {
          for (int i = 0; i < warehouseIns.WarehouseInDetails.Count; i++)
          {
            var currDetail = warehouseIns.WarehouseInDetails[i];
            warehouseInDetailIsNullList = new List<string>();
            warehouseInDetailIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();
            warehouseInDetailCheckColumnList.ForEach(o =>
            {
              // 必填
              if (!o.Nullable)
              {
                if (!DataCheckHelper.CheckRequireColumn(currDetail, o.Name))
                  warehouseInDetailIsNullList.Add(o.Name);
              }

              // 最大長度
              if (o.MaxLength > 0)
              {
                if (!DataCheckHelper.CheckDataMaxLength(currDetail, o.Name, o.MaxLength))
                  warehouseInDetailIsExceedMaxLenthList.Add(o);
              }
            });

            // 必填訊息
            if (warehouseInDetailIsNullList.Any())
              // 檢查進倉單明細必填欄位(參考2.8.2) ,如果檢核失敗，回傳 & 訊息內容[20058, < 參數4 >.CustInNo, 必填欄位未填寫的欄位清單(格式:[欄位名稱1]、[欄位名稱2])]
              data.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20058", MsgContent = string.Format(tacService.GetMsg("20058"), $"{warehouseIns.CustInNo}第{i + 1}筆明細", string.Join("、", warehouseInDetailIsNullList)) });

            // 最大長度訊息
            if (warehouseInDetailIsExceedMaxLenthList.Any())
            {
              List<string> errorMsgList = warehouseInDetailIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();

              string errorMsg = string.Join("、", errorMsgList);

              // 檢查進倉單明細必填欄位(參考2.8.2) , 如果檢核失敗，回傳 & 訊息內容[20059, < 參數4 >.CustInNo, 欄位格式錯誤清單(格式:[欄位名稱1]格式錯誤必須為X1、[欄位名稱2]格式錯誤必須為X2)]
              data.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20059", MsgContent = string.Format(tacService.GetMsg("20059"), $"{warehouseIns.CustInNo}第{i + 1}筆明細", errorMsg) });
            }

            #region 檢查進倉單容器欄位必填、最大長度
            List<string> warehouseInContainerIsNullList;
            List<ApiCkeckColumnModel> warehouseInContainerIsExceedMaxLenthList;

            if (warehouseIns.CustCost == "MoveIn" && currDetail.ContainerDatas != null && currDetail.ContainerDatas.Any())
            {
              for (int j = 0; j < currDetail.ContainerDatas.Count; j++)
              {
                var currContainer = currDetail.ContainerDatas[j];

                warehouseInContainerIsNullList = new List<string>();
                warehouseInContainerIsExceedMaxLenthList = new List<ApiCkeckColumnModel>();

                warehouseInContainerCheckColumnList.ForEach(o =>
                {
                  // 必填
                  if (!o.Nullable)
                  {
                    if (!DataCheckHelper.CheckRequireColumn(currContainer, o.Name))
                      warehouseInContainerIsNullList.Add(o.Name);
                  }

                  // 最大長度
                  if (o.MaxLength > 0)
                  {
                    if (!DataCheckHelper.CheckDataMaxLength(currContainer, o.Name, o.MaxLength))
                      warehouseInContainerIsExceedMaxLenthList.Add(o);
                  }
                });

                // 必填訊息
                if (warehouseInContainerIsNullList.Any())
                  data.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20058", MsgContent = string.Format(tacService.GetMsg("20058"), $"{warehouseIns.CustInNo}第{i + 1}筆明細{j + 1 }筆容器", string.Join("、", warehouseInContainerIsNullList)) });

                // 最大長度訊息
                if (warehouseInContainerIsExceedMaxLenthList.Any())
                {
                  List<string> errorMsgList = warehouseInContainerIsExceedMaxLenthList.Select(x => $"{x.Name}格式錯誤必須為{x.Type.Name}{ (x.MaxLength > 0 ? $"({x.MaxLength})" : string.Empty)}").ToList();
                  string errorMsg = string.Join("、", errorMsgList);
                  data.Add(new ApiResponse { No = warehouseIns.CustInNo, MsgCode = "20059", MsgContent = string.Format(tacService.GetMsg("20059"), $"{warehouseIns.CustInNo}第{i + 1}筆容器{j + 1 }筆容器", errorMsg) });
                }
              }
            }
            #endregion
          }
        }
        #endregion
      }
      #endregion

      res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位格式檢核
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="warehouseIns">進倉單資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnType(string dcCode, string gupCode, string custCode, PostCreateWarehouseInsModel warehouseIns)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}

		/// <summary>
		/// 共用欄位資料檢核
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="warehouseIns">進倉單資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckCommonColumnData(string dcCode, string gupCode, string custCode, PostCreateWarehouseInsModel warehouseIns)
		{
			CheckWarehouseInService cwiService = new CheckWarehouseInService();
			ApiResult res = new ApiResult();
			List<ApiResponse> data = new List<ApiResponse>();
			var passSnList = new List<string>();

			#region 主檔欄位資料檢核
			// 檢查ProcFlag
			cwiService.CheckProcFlag(data, warehouseIns);

			// 檢查資料庫貨主單號是否存在
			var isAdd = cwiService.CheckCustExistForThirdPart(data, _thirdPartOrdersList, warehouseIns, custCode);
			if (isAdd)
				_IsAddF075101CustOrdNoList.Add(warehouseIns.CustInNo);

			// 檢查廠商編號
			cwiService.CheckVnrExist(data, _vnrList, gupCode, custCode, warehouseIns);

			// 檢查交易類型
			cwiService.CheckTranExist(data, _tranCodeList, warehouseIns);

			// 檢查貨主單號是否存在
			cwiService.CheckCustExist(ref _f010201List, ref _f010202List, dcCode, gupCode, custCode, warehouseIns);

			// 檢查貨主自訂義分類
			cwiService.CheckCustCost(data, warehouseIns);

			// 檢查快速通關分類
			cwiService.CheckFastPassType(data, warehouseIns);

			// 檢查預定進倉時段
			cwiService.CheckBoookingInPeriod(data, warehouseIns);
			#endregion

			#region 明細欄位資料檢核
			// 檢查明細筆數
			cwiService.CheckDetailCnt(data, warehouseIns);

			// 檢核項次必須大於0，且同一張單據內的序號不可重複
			cwiService.CheckDetailSeq(data, warehouseIns);

			// 檢查明細進倉數量
			cwiService.CheckDetailQty(data, warehouseIns);

			// 檢查採購單號
			cwiService.CheckDetailPoNoRepeat(data, warehouseIns);
			#endregion

			#region 容器欄位資料檢核
			if (warehouseIns.WarehouseInDetails != null && warehouseIns.WarehouseInDetails.Any()
				&& warehouseIns.ProcFlag == "0" && warehouseIns.CustCost == "MoveIn")
			{
				for (int i = 0; i < warehouseIns.WarehouseInDetails.Count; i++)
				{
					var currDetail = warehouseIns.WarehouseInDetails[i];

					// 檢查容器資料是否完整
					cwiService.CheckContainerDatas(data, warehouseIns, currDetail);

					// 檢查容器進倉數量
					cwiService.CheckContainerQty(data, warehouseIns, currDetail);

					#region 驗證序號清單
					var f1903 = _f1903List.Where(x => x.ITEM_CODE == currDetail.ItemCode).FirstOrDefault();
          if (f1903 != null && 
            (f1903.BUNDLE_SERIALNO == "1" || 
            (warehouseIns.CustCost == "MoveIn" && currDetail.ContainerDatas.Any(o => o.SnList?.Count > 0))))
          {
            //如果是跨庫入+有序號+但該品項非序號商品，就不檢查數量與序號是否相符
            var IsCheckSerialQty = !(warehouseIns.CustCost == "MoveIn" && warehouseIns.WarehouseInDetails.Any(a => a.ContainerDatas.Any(b => b.SnList?.Any() ?? false)) && f1903.BUNDLE_SERIALNO != "1");
            // 檢查容器序號清單
            cwiService.CheckContainerSnList(dcCode, gupCode, custCode, data, warehouseIns, currDetail, _thirdPartContainerSnList, IsCheckSerialQty);
					}
					#endregion
				}
			}
			#endregion

			#region 檢查資料是否完整
			cwiService.CheckWarehouseInsData(data, dcCode, gupCode, custCode, warehouseIns, _f1903List);
			#endregion

			res.Data = data;

			return res;
		}

		/// <summary>
		/// 貨主自訂欄位資料檢核
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="warehouseIns">進倉單資料物件</param>
		/// <returns></returns>
		protected ApiResult CheckCustomColumnValue(string dcCode, string gupCode, string custCode, PostCreateWarehouseInsModel warehouseIns)
		{
			// 請預留方法
			ApiResult res = new ApiResult();
			res.Data = new List<ApiResponse>();
			return res;
		}
		#endregion

		#region Protected 建立進倉單主檔、明細、檢核資料
		/// <summary>
		/// 建立進倉單主檔
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="warehouseIns"></param>
		/// <returns></returns>
		protected F010201 CreateF010201(string dcCode, string gupCode, string custCode, PostCreateWarehouseInsModel warehouseIns)
		{
			return new F010201
			{
				STOCK_NO = Guid.NewGuid().ToString(),
				STOCK_DATE = Convert.ToDateTime(warehouseIns.CustCrtDate),
				SHOP_DATE = warehouseIns.PoDate,
				DELIVER_DATE = Convert.ToDateTime(warehouseIns.InDate),
				SOURCE_TYPE = null,
				SOURCE_NO = null,
				ORD_PROP = warehouseIns.InProp,
				VNR_CODE = warehouseIns.VnrCode,
				CUST_ORD_NO = warehouseIns.CustInNo,
				CUST_COST = warehouseIns.CustCost,
				STATUS = "0",
				MEMO = warehouseIns.Memo,
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				SHOP_NO = warehouseIns.PoNo,
				EDI_FLAG = "1",
				CHECK_CODE = null,
				CHECKCODE_EDI_STATUS = "0",
				DELV_CHECKCODE = null,
				FOREIGN_WMSNO = null,
				FOREIGN_CUSTCODE = null,
				BATCH_NO = warehouseIns.BatchNo,
				IMPORT_FLAG = "0",
				FAST_PASS_TYPE = warehouseIns.FastPassType,
				BOOKING_IN_PERIOD = warehouseIns.BoookingInPeriod
			};
		}

		/// <summary>
		/// 建立進倉單明細
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="warehouseIns"></param>
		/// <param name="f010201"></param>
		/// <returns></returns>
		protected List<F010202> CreateF010202List(PostCreateWarehouseInsModel warehouseIns, F010201 f010201)
		{
			List<F010202> result = new List<F010202>();

			if (warehouseIns.WarehouseInDetails != null || (warehouseIns.WarehouseInDetails != null && warehouseIns.WarehouseInDetails.Any()))
			{
				result = warehouseIns.WarehouseInDetails.Select(x => new F010202
				{
					STOCK_NO = f010201.STOCK_NO,
					STOCK_SEQ = Convert.ToInt32(x.ItemSeq),
					ITEM_CODE = x.ItemCode,
					STOCK_QTY = x.InQty,
					RECV_QTY = 0,
					STATUS = "0",
					DC_CODE = f010201.DC_CODE,
					GUP_CODE = f010201.GUP_CODE,
					CUST_CODE = f010201.CUST_CODE,
					VALI_DATE = x.ValidDate,
					ORDER_QTY = x.InQty,
					MAKE_NO = x.MakeNo
				}).ToList();
			}

			return result;
		}

		protected F020301 CreateF020301List(F010201 f010201)
		{
			// 取得流水號
			F020301Repository f020301Repo = new F020301Repository(Schemas.CoreSchema, _wmsTransation);
			F020301 result = new F020301();

			return new F020301
			{
				FILE_NAME = f010201.STOCK_NO,
				PURCHASE_NO = f010201.STOCK_NO,
				STATUS = "0",
				DC_CODE = f010201.DC_CODE,
				GUP_CODE = f010201.GUP_CODE,
				CUST_CODE = f010201.CUST_CODE
			};
		}

		protected List<F020302> CreateF020302List(PostCreateWarehouseInDetailModel warehouseIns, F010201 f010201, string[] snList)
		{
			return snList.Select(x => new F020302
			{
				FILE_NAME = f010201.STOCK_NO,
				PO_NO = f010201.CUST_ORD_NO,
				ITEM_CODE = warehouseIns.ItemCode,
				SERIAL_NO = x,
				SERIAL_LEN = (short)x.Length,
				VALID_DATE = new DateTime(9999, 12, 31, 00, 00, 00),
				STATUS = "0",
				DC_CODE = f010201.DC_CODE,
				GUP_CODE = f010201.GUP_CODE,
				CUST_CODE = f010201.CUST_CODE
			}).ToList();
		}

		/// <summary>
		/// 建立F0701、F070104
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="stockNo"></param>
		/// <param name="warehouseIns"></param>
		protected void CreateF0701WithF070104List(string dcCode, string gupCode, string custCode, string stockNo, PostCreateWarehouseInDetailModel warehouseIns)
		{
			var containerService = new ContainerService(_wmsTransation);
			warehouseIns.ContainerDatas.ForEach(containerData =>
			{
				#region F0701
				long f0701Id = 0;
				var existThirdDatas = _thirdPartUsingContainerList.Where(x => x.CONTAINER_CODE == containerData.ContainerCode).FirstOrDefault();
				var existDatas = _f0701List.Where(x => x.CONTAINER_CODE == containerData.ContainerCode).FirstOrDefault();
				if (existThirdDatas == null && existDatas == null)
				{
					f0701Id = containerService.GetF0701NextId();
					_f0701List.Add(new F0701
					{
						ID = f0701Id,
						DC_CODE = dcCode,
						CUST_CODE = custCode,
						WAREHOUSE_ID = "In",
						CONTAINER_CODE = containerData.ContainerCode,
						CONTAINER_TYPE = "2"
					});
				}
				else
				{
					f0701Id = existThirdDatas == null ? existDatas.ID : existThirdDatas.ID;
				}
        #endregion
        #region F070104
        var f070104Id = containerService.GetF070104NextId();
        _f070104List.Add(new F070104
				{
          ID = f070104Id,
          F0701_ID = f0701Id,
					DC_CODE = dcCode,
					CONTAINER_CODE = containerData.ContainerCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					WMS_NO = stockNo,
					SOURCE_NO = stockNo,
					ITEM_SEQ = warehouseIns.ItemSeq,
					ITEM_CODE = warehouseIns.ItemCode,
					VALID_DATE = containerData.ValidDate,
					MAKE_NO = warehouseIns.MakeNo,
					QTY = Convert.ToInt32(containerData.InQty),
					//SERIAL_NO_LIST = containerData.SnList != null && containerData.SnList.Any() ? string.Join(",", containerData.SnList) : null,
					STATUS = "0"
				});
        #endregion

        #region F07010401
        if (containerData.SnList != null && containerData.SnList.Any())
        {
          foreach (var item in containerData.SnList)
          {
            _f07010401List.Add(new F07010401
            {
              F070104_ID = f070104Id,
              ITEM_CODE = warehouseIns.ItemCode,
              SERIAL_NO = item
            });
          }
        }
        #endregion
      });
		}
		#endregion
	}
}