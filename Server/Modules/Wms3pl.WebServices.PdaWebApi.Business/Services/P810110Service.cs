using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
	public class P810110Service
	{
    P81Service _p81Service;
    private CommonService _commonService;
    public CommonService CommonService
    {
      get
      {
        if (_commonService == null)
          _commonService = new CommonService();
        return _commonService;
      }
      set { _commonService = value; }
    }

    private WmsTransaction _wmsTransation;
		public P810110Service(WmsTransaction wmsTransation)
		{
			_wmsTransation = wmsTransation;
      _p81Service = new P81Service();
    }

    /// <summary>
    /// 跨庫進倉驗收查詢
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult GetTransferStockReceivedData(GetTransferStockReceivedDataReq req, string gupCode)
		{
			var f010201Repo = new F010201Repository(Schemas.CoreSchema);
			var f000904Repo = new F000904Repository(Schemas.CoreSchema);
      var f07010401Repo = new F07010401Repository(Schemas.CoreSchema);
			#region 資料檢核

			// 帳號檢核
			var accData = _p81Service.CheckAcc(req.AccNo);

			// 檢核人員功能權限
			var accFunctionCount = _p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

			// 檢核人員貨主權限
			var accCustCount = _p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

			// 檢核人員物流中心權限
			var accDcCount =_p81Service.CheckAccDc(req.DcNo, req.AccNo);

			// 傳入的參數驗證
			if (string.IsNullOrWhiteSpace(req.FuncNo) ||
					string.IsNullOrWhiteSpace(req.AccNo) ||
					string.IsNullOrWhiteSpace(req.DcNo) ||
					string.IsNullOrWhiteSpace(req.CustNo) ||
					accData.Count() == 0 ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent =_p81Service.GetMsg("20069") };

			if (string.IsNullOrWhiteSpace(req.ContainerCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21001", MsgContent =_p81Service.GetMsg("21001") };
      #endregion

      #region 資料處理
      req.ContainerCode = req.ContainerCode?.ToUpper();

			var f070104s = new List<F070104>();
			var getF070104Res = GetF070104Data(ref f070104s, req.ContainerCode);
			if (!getF070104Res.IsSuccessed)
				return getF070104Res;

			if (f070104s.All(x => x.STATUS != "0"))
				return new ApiResult { IsSuccessed = false, MsgCode = "21013", MsgContent =_p81Service.GetMsg("21013") };

			var f010201s = f010201Repo.GetDatasByStockNos(req.DcNo, gupCode, req.CustNo, f070104s.Select(x => x.WMS_NO).Distinct().ToList());
			if (!f010201s.Any())
				return new ApiResult { IsSuccessed = false, MsgCode = "21004", MsgContent =_p81Service.GetMsg("21004") };

			var passStatus = new List<string> { "0", "1" };

			if (f010201s.All(x => passStatus.Contains(x.STATUS)))// 進倉單都為待處理或驗收中代表查詢成功
			{
        var itemDatas = CommonService.GetProductList(gupCode, req.CustNo, f070104s.Select(x => x.ITEM_CODE).Distinct().ToList());
        var f07010401s = f07010401Repo.GetDatasByF070104Ids(f070104s.Select(x => x.ID).ToList()).ToList();

        var data = (from A in f070104s
										join B in itemDatas
										on A.ITEM_CODE equals B.ITEM_CODE
										select new GetTransferStockReceivedDataRes
										{
											DcNo = A.DC_CODE,
											CustNo = A.CUST_CODE,
											WmsNo = A.WMS_NO,
											ItemSeq = A.ITEM_SEQ,
											ItemCode = A.ITEM_CODE,
											ItemName = B.ITEM_NAME,
											EanCode1 = B.EAN_CODE1,
											EanCode2 = B.EAN_CODE2,
											EanCode3 = B.EAN_CODE3,
                      SnList = 
                        f07010401s.Any(x => x.F070104_ID == A.ID)
                          ? string.Join(",", f07010401s.Where(x => x.F070104_ID == A.ID).Select(x => x.SERIAL_NO))
                          : A.SERIAL_NO_LIST, //f070104.SERIAL_NO_LIST確認刪除此欄位時，這判斷清除只留上面這行即可
                      ItemQty = A.QTY
										}).ToList();

				return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent =_p81Service.GetMsg("10001"), Data = data };
			}
			else // 找出待處理以外的進倉單組錯誤訊息
			{
				var stockStatus = f000904Repo.GetF000904Data("F010201", "STATUS");

				var errStock = f010201s.Where(x => x.STATUS != "0");

				var errDatas = (from A in errStock
												join B in stockStatus
												on A.STATUS equals B.VALUE
												select new
												{
													A.STOCK_NO,
													B.NAME
												}).GroupBy(x => x.NAME).Select(x => new { StatusName = x.Key, StockNos = x.Select(z => z.STOCK_NO).ToList() }).ToList();

				var msgList = new List<string>();
				errDatas.ForEach(errData =>
				{
					msgList.Add($"該進倉單{string.Join("、", errData.StockNos)}狀態為{errData.StatusName}");
				});

				return new ApiResult { IsSuccessed = false, MsgCode = "21005", MsgContent = string.Format(_p81Service.GetMsg("21005"), string.Join("；", msgList)) };
			}
			#endregion
		}

		/// <summary>
		/// 跨庫進倉驗收檢核
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult ConfirmTransferStockReceivedData(ConfirmTransferStockReceivedDataReq req, string gupCode)
		{
      #region 資料檢核

      // 帳號檢核
      var accData = _p81Service.CheckAcc(req.AccNo);

			// 檢核人員功能權限
			var accFunctionCount = _p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

			// 檢核人員貨主權限
			var accCustCount = _p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

			// 檢核人員物流中心權限
			var accDcCount = _p81Service.CheckAccDc(req.DcNo, req.AccNo);

			// 傳入的參數驗證
			if (string.IsNullOrWhiteSpace(req.FuncNo) ||
					string.IsNullOrWhiteSpace(req.AccNo) ||
					string.IsNullOrWhiteSpace(req.DcNo) ||
					string.IsNullOrWhiteSpace(req.CustNo) ||
					accData.Count() == 0 ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

			if (string.IsNullOrWhiteSpace(req.ContainerCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21001", MsgContent = _p81Service.GetMsg("21001") };

			if (string.IsNullOrWhiteSpace(req.ItemCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21006", MsgContent = _p81Service.GetMsg("21006") };
      #endregion

      #region 資料處理
      var res = ConfirmTransferStockReceivedDataProcess(req, gupCode);
      if (res.IsSuccessed)
        _wmsTransation.Complete();
      return res;
      #endregion
    }

    /// <summary>
    /// 跨庫進倉驗收檢核資料處理
    /// </summary>
    /// <param name="req"></param>
    /// <param name="gupCode"></param>
    /// <param name="f070104s">跨庫調撥驗收入自動倉-驗收完成前置作業會呼叫相同function去撈f070104，裡面不用重複呼叫，如果有傳值進來此fun不會呼叫DB資料更新</param>
    /// <returns></returns>
    public ApiResult ConfirmTransferStockReceivedDataProcess(ConfirmTransferStockReceivedDataReq req, string gupCode, List<F070104> f070104s = null)
    {
      #region 宣告
      var serialNoService = new SerialNoService(_wmsTransation);
      var f010204Repo2 = new F010204Repository(Schemas.CoreSchema);
      var f010204Repo = new F010204Repository(Schemas.CoreSchema, _wmsTransation);
      var f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransation);
      var f010202Repo = new F010202Repository(Schemas.CoreSchema, _wmsTransation);
      var f010205Repo = new F010205Repository(Schemas.CoreSchema, _wmsTransation);
      var f020301Repo = new F020301Repository(Schemas.CoreSchema, _wmsTransation);
      var f020302Repo = new F020302Repository(Schemas.CoreSchema, _wmsTransation);
      var f070104Repo = new F070104Repository(Schemas.CoreSchema, _wmsTransation);
      var f07010401Repo = new F07010401Repository(Schemas.CoreSchema, _wmsTransation);
      var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransation);
      var addF010205Datas = new List<F010205>();
      var updF010204Datas = new List<F010204>();
      var updF020302Datas = new List<F020302>();
      var updF010201Datas = new List<F010201>();
      var updF070104Datas = new List<F070104>();
      var serialnoCheckFailResult = new List<SerialNoResult>();
      var addF2501s = new List<F2501>();
      var updF2501s = new List<F2501>();
      var delSnLists = new List<string>();

      var isUpdateF070104 = f070104s == null;
      #endregion 宣告

      #region 資料處理
      // 檢核、取得F070104 By ContainerCode
      if (f070104s == null)
      {
        f070104s = new List<F070104>();
        var getF070104Res = GetF070104Data(ref f070104s, req.ContainerCode);
        if (!getF070104Res.IsSuccessed)
          return getF070104Res;
      }
			// 該容器品號的進倉單號清單
			var stockNos = f070104s.Select(x => x.SOURCE_NO).Distinct().ToList();

			#region 檢查商品條碼是否正確
			var isScanSn = false;
			var itemCodes = new List<string>();
			var itemService = new ItemService();
			var findSn = string.Empty;
			F2501 f2501 = null;
			var findItemCodes = itemService.FindItems(gupCode, req.CustNo, req.ItemCode, ref f2501);
      if (f2501 == null)
      {
        if (findItemCodes.Any())
          itemCodes.AddRange(findItemCodes);
        else
        {
          var findPurchaseItemSn = f020302Repo.FindPurchaseItemSn(req.DcNo, gupCode, req.CustNo, stockNos, req.ItemCode);
          if (findPurchaseItemSn != null)
          {
            itemCodes.Add(findPurchaseItemSn.ITEM_CODE);
            findSn = findPurchaseItemSn.SERIAL_NO;
          }
        }
      }
      else
      {
        isScanSn = true;
        if (f2501.STATUS == "A1")
        {
          itemCodes.AddRange(findItemCodes);
          findSn = f2501.SERIAL_NO;
        }
        else
        {
          var findPurchaseItemSn = f020302Repo.FindPurchaseItemSn(req.DcNo, gupCode, req.CustNo, stockNos, req.ItemCode);
          if (findPurchaseItemSn != null)
          {
            itemCodes.Add(findPurchaseItemSn.ITEM_CODE);
            findSn = findPurchaseItemSn.SERIAL_NO;
          }
        }
      }

			if (!itemCodes.Any())
				return new ApiResult { IsSuccessed = false, MsgCode = "21007", MsgContent = _p81Service.GetMsg("21007") };

			#endregion

			// 檢查商品是否在容器內商品
			var findItem = f070104s.FirstOrDefault(x => itemCodes.Contains(x.ITEM_CODE));
			if (findItem == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21008", MsgContent = _p81Service.GetMsg("21008") };

			var f07010401s = f07010401Repo.GetDatasByF070104Ids(f070104s.Select(x => x.ID).ToList()).ToList();
			var itemSnList = new List<string>();
			itemSnList.AddRange(f070104s.Where(x => !string.IsNullOrWhiteSpace(x.SERIAL_NO_LIST)).SelectMany(x => x.SERIAL_NO_LIST.Split(',').ToList()));
			itemSnList.AddRange(f07010401s.Select(x => x.SERIAL_NO).ToList());

			// 檢查商品序號是否在容器內
			if (isScanSn)
			{
			
				if(!itemSnList.Any(x=> x == req.ItemCode))
					return new ApiResult { IsSuccessed = false, MsgCode = "21012", MsgContent = _p81Service.GetMsg("21012") };
			}

      // 取得進倉單
      var f010201s = f010201Repo.GetDatasByStockNos(req.DcNo, gupCode, req.CustNo, stockNos).ToList();

      // 取得進倉單明細
      var f010202s = f010202Repo.GetDatasByStockNos(req.DcNo, gupCode, req.CustNo, stockNos).ToList();

      // 取得序號匯入頭檔
      var f020301s = f020301Repo.GetDatasByShopNos(req.DcNo, gupCode, req.CustNo, stockNos).ToList();

      // 取得序號匯入身檔
      var f020302s = f020302Repo.GetDatasByFileNames(req.DcNo, gupCode, req.CustNo, f020301s.Select(x => x.FILE_NAME).ToList()).ToList();

      // 取得進倉回檔歷程紀錄表
      var f010205s = f010205Repo.GetDatasByStockNos(req.DcNo, gupCode, req.CustNo, stockNos).ToList();

			#region 先新增所有進倉單明細至F010204，讓後面只做更新，所以獨立連線先寫入commit
			f010204Repo2.InsertNotExistDatas(req.DcNo, gupCode, req.CustNo, stockNos);

      #endregion

      // 取得進倉驗收上架結果表
      var f010204s = f010204Repo.GetDatasByStockNos(req.DcNo, gupCode, req.CustNo,stockNos).ToList();
			var addF2501List = new List<F2501>();
			var updF2501List = new List<F2501>();
			var delSnList = new List<string>();

			foreach (var f070104 in f070104s)
      {
        var itemSeq = Convert.ToInt32(f070104.ITEM_SEQ);

        var f010201 = f010201s.Where(o => o.STOCK_NO == f070104.SOURCE_NO).FirstOrDefault();
        var f010202 = f010202s.Where(o => o.STOCK_NO == f070104.SOURCE_NO && o.STOCK_SEQ == itemSeq).FirstOrDefault();
        var f010204 = f010204s.Where(o => o.STOCK_NO == f070104.SOURCE_NO && o.STOCK_SEQ == itemSeq).FirstOrDefault();

        // 更新F010204.TOTAL_REC_QTY += F070104.QTY
        if (f010204 != null)
        {
          var updF010204 = updF010204Datas.Where(x => x.STOCK_NO == f070104.SOURCE_NO && x.STOCK_SEQ == itemSeq).FirstOrDefault();

          if (updF010204 == null)
          {
            f010204.TOTAL_REC_QTY += f070104.QTY;
            updF010204Datas.Add(f010204);
          }
          else
          {
            updF010204.TOTAL_REC_QTY += f070104.QTY;
          }
        }

        // 檢查是否有序號資料，有的話將資料寫入F2501
        var f020301 = f020301s.Where(o => o.PURCHASE_NO == f010201.STOCK_NO).FirstOrDefault();
        if (f020301 != null)
        {
          var procSerialNos = new List<string>();
          if (!string.IsNullOrWhiteSpace(f070104.SERIAL_NO_LIST))
            procSerialNos.AddRange(f070104.SERIAL_NO_LIST.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

          if (f07010401s != null && f07010401s.Any())
            procSerialNos.AddRange(f07010401s.Where(x => f070104.ID == x.F070104_ID).Select(x => x.SERIAL_NO));

					if(procSerialNos.Any())
					{
						var currF020302s = f020302s.Where(o => o.FILE_NAME == f020301.FILE_NAME && procSerialNos.Contains(o.SERIAL_NO)).ToList();
						var checkResList = serialNoService.CheckItemLargeSerialWithBeforeInWarehouse(f070104.GUP_CODE, f070104.CUST_CODE,f070104.ITEM_CODE, procSerialNos);
						if (checkResList.All(x => x.Checked))
						{
							foreach(var sn in procSerialNos)
							{
								var checkRes = checkResList.First(x => x.SerialNo == sn);
								var currF020302 = currF020302s.FirstOrDefault(x => x.SERIAL_NO == sn);
								if(currF020302!=null)
								{
									// 新增、更新F2501
									serialNoService.UpdateSerialNoFull(ref addF2501s, ref updF2501s, ref delSnList, currF020302.DC_CODE, currF020302.GUP_CODE, currF020302.CUST_CODE, "A1", checkRes,
																		f010201.STOCK_NO, f010201.VNR_CODE, currF020302.VALID_DATE, null,
																		currF020302.PO_NO, f010201.ORD_PROP, null, currF020302.PUK,
																		currF020302.CELL_NUM, currF020302.BATCH_NO);
									currF020302.STATUS = "1";
									updF020302Datas.Add(currF020302);
								}
								else
									serialNoService.UpdateSerialNoFull(ref addF2501s, ref updF2501s, ref delSnList, f010201.DC_CODE, f010201.GUP_CODE, f010201.CUST_CODE, "A1", checkRes,
																	f010201.STOCK_NO, f010201.VNR_CODE, null, null,
																	f010201.SHOP_NO, f010201.ORD_PROP, null, null,
																	null, null);
							}
						}
						else
							serialnoCheckFailResult.AddRange(checkResList.Where(x => !x.Checked));

						if (serialnoCheckFailResult.Any())
              return new ApiResult()
              {
                IsSuccessed = false,
                MsgCode = "21011",
                MsgContent = string.Format(_p81Service.GetMsg("21011"), "\r\n" + string.Join("\r\n", serialnoCheckFailResult.Select(x => x.SerialNo + x.Message)))
              };
					}
        }

        // 若F010201.STATUS = 0，新增回檔資訊F010205
        if (f010201.STATUS == "0")
        {
          var thirdPartF010205 = f010205s.Where(x => x.STOCK_NO == f070104.SOURCE_NO && x.STATUS == "1").FirstOrDefault();
          var addF010205 = addF010205Datas.Where(x => x.STOCK_NO == f070104.SOURCE_NO && x.STATUS == "1").FirstOrDefault();
          if (thirdPartF010205 == null && addF010205 == null)
          {
            // 第一筆狀態為 1(收貨)
            addF010205Datas.Add(new F010205
            {
              DC_CODE = f070104.DC_CODE,
              GUP_CODE = f070104.GUP_CODE,
              CUST_CODE = f070104.CUST_CODE,
              STOCK_NO = f070104.SOURCE_NO,
              STATUS = "1",
              PROC_FLAG = "0"
            });
          }

          var thirdPartF010205Tmp = f010205s.Where(x => x.STOCK_NO == f070104.SOURCE_NO && x.STATUS == "5").FirstOrDefault();
          var addF010205Tmp = addF010205Datas.Where(x => x.STOCK_NO == f070104.SOURCE_NO && x.STATUS == "5").FirstOrDefault();
          if (thirdPartF010205Tmp == null && addF010205Tmp == null)
          {
            // 第二筆狀態為 5(開始驗收)
            addF010205Datas.Add(new F010205
            {
              DC_CODE = f070104.DC_CODE,
              GUP_CODE = f070104.GUP_CODE,
              CUST_CODE = f070104.CUST_CODE,
              STOCK_NO = f070104.SOURCE_NO,
              STATUS = "5",
              PROC_FLAG = "1",
              TRANS_DATE = DateTime.Now
            });
          }
        }

        // 確認是否全部驗收
        var currStockDatas = f010204s.Where(x => x.STOCK_NO == f070104.SOURCE_NO).ToList();
        if (currStockDatas.All(x => x.STOCK_QTY == x.TOTAL_REC_QTY))
        {
          var thirdPartF010205 = f010205s.Where(x => x.STOCK_NO == f070104.SOURCE_NO && x.STATUS == "4").FirstOrDefault();
          var addF010205 = addF010205Datas.Where(x => x.STOCK_NO == f070104.SOURCE_NO && x.STATUS == "4").FirstOrDefault();
          if (thirdPartF010205 == null && addF010205 == null)
          {
            var updF010201 = updF010201Datas.Where(x => x.STOCK_NO == f070104.SOURCE_NO).FirstOrDefault();
            if (updF010201 == null)
            {
              f010201.STATUS = "2";
              updF010201Datas.Add(f010201);
            }
            else
            {
              updF010201.STATUS = "2";
            }
          }
        }
        else
        {
          var updF010201 = updF010201Datas.Where(x => x.STOCK_NO == f070104.SOURCE_NO).FirstOrDefault();
          if (updF010201 == null)
          {
            f010201.STATUS = "1";
            updF010201Datas.Add(f010201);
          }
          else
          {
            updF010201.STATUS = "1";
          }
        }

        f070104.STATUS = "1";
        updF070104Datas.Add(f070104);
      }

      if (isUpdateF070104 && updF070104Datas.Any())
        f070104Repo.BulkUpdate(updF070104Datas);
      if (addF010205Datas.Any())
        f010205Repo.BulkInsert(addF010205Datas);
      if (updF010204Datas.Any())
        f010204Repo.BulkUpdate(updF010204Datas);
      if (updF020302Datas.Any())
        f020302Repo.BulkUpdate(updF020302Datas);
      if (updF010201Datas.Any())
        f010201Repo.BulkUpdate(updF010201Datas);

      if (delSnLists.Any())
        f2501Repo.DeleteBySnList(gupCode, req.CustNo, delSnLists);
      if (addF2501s.Any())
        f2501Repo.BulkInsert(addF2501s);
      if (updF2501s.Any())
        f2501Repo.BulkUpdate(updF2501s);

      return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = _p81Service.GetMsg("10005") };
      #endregion
    }

    /// <summary>
    /// 跨庫進倉上架查詢
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult GetTransferStockInData(GetTransferStockInDataReq req, string gupCode)
		{
			var f010201Repo = new F010201Repository(Schemas.CoreSchema);
			var f000904Repo = new F000904Repository(Schemas.CoreSchema);
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
			var f0003Repo = new F0003Repository(Schemas.CoreSchema);
			var f1980Repo = new F1980Repository(Schemas.CoreSchema);
			#region 資料檢核 資料處理

			// 帳號檢核
			var accData = _p81Service.CheckAcc(req.AccNo);

			// 檢核人員功能權限
			var accFunctionCount = _p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

			// 檢核人員貨主權限
			var accCustCount = _p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

			// 檢核人員物流中心權限
			var accDcCount = _p81Service.CheckAccDc(req.DcNo, req.AccNo);

			// 傳入的參數驗證
			if (string.IsNullOrWhiteSpace(req.FuncNo) ||
					string.IsNullOrWhiteSpace(req.AccNo) ||
					string.IsNullOrWhiteSpace(req.DcNo) ||
					string.IsNullOrWhiteSpace(req.CustNo) ||
					accData.Count() == 0 ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

			if (string.IsNullOrWhiteSpace(req.ContainerCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21001", MsgContent = _p81Service.GetMsg("21001") };

			var f070104s = new List<F070104>();
			var getF070104Res = GetF070104Data(ref f070104s, req.ContainerCode);
			if (!getF070104Res.IsSuccessed)
				return getF070104Res;

			if (f070104s.All(x => x.STATUS == "0"))
				return new ApiResult { IsSuccessed = false, MsgCode = "21020", MsgContent = _p81Service.GetMsg("21020") };

			if (f070104s.All(x => x.STATUS == "2"))
				return new ApiResult { IsSuccessed = false, MsgCode = "21019", MsgContent = _p81Service.GetMsg("21019") };

			var f010201s = f010201Repo.GetDatasByStockNos(req.DcNo, gupCode, req.CustNo, f070104s.Select(x => x.WMS_NO).Distinct().ToList());
			if (!f010201s.Any())
				return new ApiResult { IsSuccessed = false, MsgCode = "21004", MsgContent = _p81Service.GetMsg("21004") };

			var passStatus = new List<string> { "1", "2" };
			if (f010201s.All(x => passStatus.Contains(x.STATUS)))// 進倉單都為驗收中或結案代表查詢成功
			{
				// 尋找建議儲位
				//(1) 撈F1912.min(loc_code) by DC_CODE、NOW_CUST_CODE = 0、WAREHOUSE_ID = (撈F0003.SYS_PATH by DC_CODE、AP_NAME = MoveInWhID) 
				//(2) 若完全找不到空儲位，請在建議儲位上顯示”目前無空儲位，請自行尋找適當的儲位”
				var f0003 = f0003Repo.Find(o => o.DC_CODE == req.DcNo && o.AP_NAME == "MoveInWhID");
				if (f0003 == null)
					return new ApiResult { IsSuccessed = false, MsgCode = "21009", MsgContent = _p81Service.GetMsg("21009") };

				var f1980 = f1980Repo.Find(o => o.DC_CODE == req.DcNo && o.WAREHOUSE_ID == f0003.SYS_PATH);
				if (f1980 == null)
					return new ApiResult { IsSuccessed = false, MsgCode = "21010", MsgContent = _p81Service.GetMsg("21010") };

				var locCode = f1912Repo.GetMinLocCode(req.DcNo, f0003.SYS_PATH);

				return new ApiResult
				{
					IsSuccessed = true,
					MsgCode = "10001",
					MsgContent = _p81Service.GetMsg("10001"),
					Data = new GetTransferStockInDataRes
					{
						DcNo = req.DcNo,
						CustNo = req.CustNo,
						WarehouseName = f1980.WAREHOUSE_NAME,
						LocCode = locCode
					}
				};
			}
			else // 找出待處理以外的進倉單組錯誤訊息
			{
				var stockStatus = f000904Repo.GetF000904Data("F010201", "STATUS");

				var errStock = f010201s.Where(x => !passStatus.Contains(x.STATUS));

				var errDatas = (from A in errStock
												join B in stockStatus
												on A.STATUS equals B.VALUE
												select new
												{
													A.STOCK_NO,
													B.NAME
												}).GroupBy(x => x.NAME).Select(x => new { StatusName = x.Key, StockNos = x.Select(z => z.STOCK_NO).ToList() }).ToList();

				var msgList = new List<string>();
				errDatas.ForEach(errData =>
				{
					msgList.Add($"該進倉單{string.Join("、", errData.StockNos)}狀態為{errData.StatusName}");
				});

				return new ApiResult { IsSuccessed = false, MsgCode = "21005", MsgContent = string.Format(_p81Service.GetMsg("21005"), string.Join("；", msgList)) };
			}
			#endregion
		}

		/// <summary>
		/// 跨庫進倉上架檢核
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult ConfirmTransferStockInData(ConfirmTransferStockInDataReq req, string gupCode)
		{
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
			var f1980Repo = new F1980Repository(Schemas.CoreSchema);
			var f0003Repo = new F0003Repository(Schemas.CoreSchema);
			var sharedService = new SharedService(_wmsTransation);
			var f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransation);
			var f010202Repo = new F010202Repository(Schemas.CoreSchema, _wmsTransation);
			var f010204Repo = new F010204Repository(Schemas.CoreSchema, _wmsTransation);
			var f010205Repo = new F010205Repository(Schemas.CoreSchema, _wmsTransation);
			var f0701Repo = new F0701Repository(Schemas.CoreSchema, _wmsTransation);
			var f070104Repo = new F070104Repository(Schemas.CoreSchema, _wmsTransation);
			var f07010401Repo = new F07010401Repository(Schemas.CoreSchema, _wmsTransation);
			var f020202Repo = new F020202Repository(Schemas.CoreSchema, _wmsTransation);
			var f02020107Repo = new F02020107Repository(Schemas.CoreSchema, _wmsTransation);
			var f02020108Repo = new F02020108Repository(Schemas.CoreSchema, _wmsTransation);
			var addF010205Datas = new List<F010205>();
			var addF02020107Datas = new List<F02020107>();
			var addF02020108Datas = new List<F02020108>();
			var updF010204Datas = new List<F010204>();
			var addF020202Datas = new List<F020202>();
			var today = DateTime.Today;
			var stockList = new List<F1913>();
			var allocationList = new List<ReturnNewAllocation>();

      var updF070104Datas = new List<F070104>();
      #region 資料檢核

      // 帳號檢核
      var accData = _p81Service.CheckAcc(req.AccNo);

			// 檢核人員功能權限
			var accFunctionCount = _p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

			// 檢核人員貨主權限
			var accCustCount = _p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

			// 檢核人員物流中心權限
			var accDcCount = _p81Service.CheckAccDc(req.DcNo, req.AccNo);

			// 傳入的參數驗證
			if (string.IsNullOrWhiteSpace(req.FuncNo) ||
					string.IsNullOrWhiteSpace(req.AccNo) ||
					string.IsNullOrWhiteSpace(req.DcNo) ||
					string.IsNullOrWhiteSpace(req.CustNo) ||
					accData.Count() == 0 ||
					accFunctionCount == 0 ||
					accCustCount == 0 ||
					accDcCount == 0)
				return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = _p81Service.GetMsg("20069") };

			if (string.IsNullOrWhiteSpace(req.ContainerCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21001", MsgContent = _p81Service.GetMsg("21001") };

			if (string.IsNullOrWhiteSpace(req.LocCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "21014", MsgContent = _p81Service.GetMsg("21014") };
			#endregion

			// 容器條碼轉大寫
			req.ContainerCode = req.ContainerCode.ToUpper();
			var f076101Repo = new F076101Repository(Schemas.CoreSchema);
			var f076101 = f076101Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
				new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
				 () =>
				 {
					 var lockF0701 = f076101Repo.LockF076101();
					 var chkF076101 = f076101Repo.Find(x => x.CONTAINER_CODE == req.ContainerCode);
					 if (chkF076101 != null)
						 return null;
					 var newF076101 = new F076101()
					 {
						 CONTAINER_CODE = req.ContainerCode
					 };
					 f076101Repo.Add(newF076101);
					 return newF076101;
				 });

			if (f076101 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21023", MsgContent = string.Format(_p81Service.GetMsg("21023"), req.ContainerCode) };

			#region 進階檢核
			// 檢核、取得F070104 By ContainerCode
			var f070104s = new List<F070104>();
			var getF070104Res = GetF070104Data(ref f070104s, req.ContainerCode);
			if (!getF070104Res.IsSuccessed)
				return getF070104Res;

      var f07010401s = f07010401Repo.GetDatasByF070104Ids(f070104s.Select(x => x.ID).ToList()).ToList();

			// 尋找建議儲位
			//(1) 撈F1912.min(loc_code) by DC_CODE、NOW_CUST_CODE = 0、WAREHOUSE_ID = (撈F0003.SYS_PATH by DC_CODE、AP_NAME = MoveInWhID) 
			//(2) 若完全找不到空儲位，請在建議儲位上顯示”目前無空儲位，請自行尋找適當的儲位”
			var f0003 = f0003Repo.Find(o => o.DC_CODE == req.DcNo && o.AP_NAME == "MoveInWhID");
			if (f0003 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21009", MsgContent = _p81Service.GetMsg("21009") };

			var f1980 = f1980Repo.Find(o => o.DC_CODE == req.DcNo && o.WAREHOUSE_ID == f0003.SYS_PATH);
			if (f1980 == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "21010", MsgContent = _p81Service.GetMsg("21010") };

			var locCode = f1912Repo.GetMinLocCode(req.DcNo, f0003.SYS_PATH);

			F1912 f1912 = null;
			// 若輸入不同的儲位，先檢驗該儲位是否屬於在F0003.SYS_PATH的倉別中，
			// 撈F1912 by loc_code=刷入的儲位編號 + warehouse_id = F0003.SYS_PATH。
			// 若該儲位不屬於此區，錯誤訊息”此儲位並不允許進行跨庫調撥上架，請輸入正確儲位”
			if (locCode != req.LocCode)
			{
        f1912 = f1912Repo.Find(o => o.DC_CODE == req.DcNo && o.LOC_CODE == req.LocCode && o.WAREHOUSE_ID == f0003.SYS_PATH);
        if (f1912 == null)
					return new ApiResult { IsSuccessed = false, MsgCode = "21015", MsgContent = _p81Service.GetMsg("21015") };
			}
			else
			{
				f1912 = f1912Repo.Find(o => o.DC_CODE == req.DcNo && o.LOC_CODE == req.LocCode);
				if (f1912 == null)
					return new ApiResult { IsSuccessed = false, MsgCode = "21016", MsgContent = _p81Service.GetMsg("21016") };
			}

			// (1) 檢查F1912.now_cust_code = 0 或等於第一筆的(F070104.cust_code)，
			// 若不符合者，IsSuccessed = false，錯誤訊息”此儲位已經有其他貨主的商品，請尋找其他儲位”
			if (!(f1912.NOW_CUST_CODE == "0" || f1912.NOW_CUST_CODE == f070104s.FirstOrDefault().CUST_CODE))
				return new ApiResult { IsSuccessed = false, MsgCode = "21017", MsgContent = _p81Service.GetMsg("21017") };

			// (2) 檢查F1912.NOW_STATUS_ID = 01(使用中) 或 03(凍結出)，
			// 若不符合者，IsSuccessed = false，錯誤訊息”此儲位被凍結，不允許進貨上架”
			// 檢核儲位是否凍結，False代表檢核不通過
			if (!(f1912.NOW_STATUS_ID == "01" || f1912.NOW_STATUS_ID == "03"))
				return new ApiResult { IsSuccessed = false, MsgCode = "21018", MsgContent = _p81Service.GetMsg("21018") };
			#endregion

			#region 資料處理 在容器中的每一張進倉單都新增調撥單

			// 該容器品號的進倉單號清單
			var stockNos = f070104s.Select(x => x.SOURCE_NO).Distinct().ToList();

      // 取得進倉單
      //var f010201s = f010201Repo.GetDatasByStockNos(req.DcNo, gupCode, req.CustNo, stockNos).ToList();

      // 取貨主單號是TR開頭的進倉單號清單
      var startWithTr = f010201Repo.CheckAndGetCustOrdNoStartWithTr(req.DcNo, gupCode, req.CustNo, stockNos).ToList();

      // 取得進倉單明細
      var f010202s = f010202Repo.GetDatasByStockNos(req.DcNo, gupCode, req.CustNo, stockNos).ToList();

			// 取得進倉驗收上架結果表
			var f010204s = f010204Repo.GetDatasByStockNos(req.DcNo, gupCode, req.CustNo, stockNos).ToList();

			// 取得進倉回檔歷程紀錄表
			var f010205s = f010205Repo.GetDatasByStockNos(req.DcNo, gupCode, req.CustNo, stockNos).ToList();

			var stockDatas = f070104s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.SOURCE_NO }).Select(x => new { DcCode = x.Key.DC_CODE, GupCode = x.Key.GUP_CODE, CustCode = x.Key.CUST_CODE, StockNo = x.Key.SOURCE_NO, F070104s = x.ToList() }).ToList();

			var f1903s = CommonService.GetProductList(gupCode, req.CustNo, f070104s.Select(x => x.ITEM_CODE).Distinct().ToList());

			foreach (var obj in stockDatas)
			{
				#region 建立調撥單

				var details = new List<StockDetail>();

        var detailsTmp = from A in obj.F070104s
                         join B in f1903s
                         on A.ITEM_CODE equals B.ITEM_CODE
                         join C in f010202s
                         on new { A.SOURCE_NO, ITEM_SEQ = A.ITEM_SEQ.PadLeft(2, '0') } equals new { SOURCE_NO = C.STOCK_NO, ITEM_SEQ = C.STOCK_SEQ.ToString().PadLeft(2, '0') }
                         let SERIAL_NOs = f07010401s.Where(x => x.F070104_ID == A.ID).Select(x => x.SERIAL_NO)
                         select new
                         {
                           SEQ = A.ITEM_SEQ,
                           B.BUNDLE_SERIALLOC,
                           A.ITEM_CODE,
                           VALID_DATE = Convert.ToDateTime(A.VALID_DATE),
                           ENTER_DATE = ComposeEnterDate(A.MAKE_NO, A.SOURCE_NO, startWithTr),
                           A.MAKE_NO,
                           SN_LIST = B.BUNDLE_SERIALLOC == "1" && (!string.IsNullOrWhiteSpace(A.SERIAL_NO_LIST) || SERIAL_NOs.Any())
                            ? A.SERIAL_NO_LIST?.Split(',').ToList() ?? new List<string>().Union(SERIAL_NOs).ToList()
                            : new List<string>(),
                           A.QTY,
                         };

        // 序號綁儲位，需要帶入序號且要拆一個序號一筆明細
        var serialLocDatas = detailsTmp.Where(x => x.BUNDLE_SERIALLOC == "1").ToList();
				serialLocDatas.ForEach(serialLocData =>
				{
					for (int index = 0; index < serialLocData.QTY; index++)
					{
						details.Add(new StockDetail
						{
							CustCode = req.CustNo,
							GupCode = gupCode,
							SrcDcCode = req.DcNo,
							TarDcCode = req.DcNo,
							SrcWarehouseId = "",
							TarWarehouseId = f0003.SYS_PATH,
							TarLocCode = req.LocCode,
							ItemCode = serialLocData.ITEM_CODE,
							ValidDate = Convert.ToDateTime(serialLocData.VALID_DATE),
							EnterDate = today,
							Qty = 1,
							VnrCode = "000000",
							SerialNo = serialLocData.SN_LIST[index],
							BoxCtrlNo = "0",
							PalletCtrlNo = "0",
							MAKE_NO = string.IsNullOrWhiteSpace(serialLocData.MAKE_NO) ? "0" : serialLocData.MAKE_NO
						});
					}
				});

				// 非序號綁儲位商品
				var notSerialLocDatas = detailsTmp.Where(x => x.BUNDLE_SERIALLOC == "0").ToList();
				if (notSerialLocDatas.Any())
				{
					details.AddRange(notSerialLocDatas.Select(x => new StockDetail
					{
						CustCode = req.CustNo,
						GupCode = gupCode,
						SrcDcCode = req.DcNo,
						TarDcCode = req.DcNo,
						SrcWarehouseId = "",
						TarWarehouseId = f0003.SYS_PATH,
						TarLocCode = req.LocCode,
						ItemCode = x.ITEM_CODE,
						ValidDate = x.VALID_DATE,
						EnterDate = x.ENTER_DATE,
						Qty = x.QTY,
						VnrCode = "000000",
						SerialNo = "0",
						BoxCtrlNo = "0",
						PalletCtrlNo = "0",
						MAKE_NO = string.IsNullOrWhiteSpace(x.MAKE_NO) ? "0" : x.MAKE_NO
					}).ToList());
				}

				// 建立調撥單
				var newAllocationParam = new NewAllocationItemParam()
				{
					IsMoveOrder = true,
					SrcDcCode = req.DcNo,
					TarDcCode = req.DcNo,
					GupCode = gupCode,
					CustCode = req.CustNo,
					ReturnStocks = stockList,
					AllocationType = AllocationType.NoSource,
					SourceType = "30",
					SourceNo = obj.StockNo,
					StockDetails = details
				};

				var allloationResult = sharedService.CreateOrUpdateAllocation(newAllocationParam, false);
				if (!allloationResult.Result.IsSuccessed)
					return new ApiResult { IsSuccessed = false, MsgCode = "20756", MsgContent = string.Format(_p81Service.GetMsg("20756"), allloationResult.Result.Message) };

				stockList = allloationResult.StockList;
				allocationList.AddRange(allloationResult.AllocationList);
				#endregion

				#region 新增F02020108
				var currStockData = from A in obj.F070104s
														join B in f010202s.Where(x => x.STOCK_NO == obj.StockNo)
														on new { A.SOURCE_NO, ITEM_SEQ = A.ITEM_SEQ.PadLeft(2, '0') } equals new { SOURCE_NO = B.STOCK_NO, ITEM_SEQ = B.STOCK_SEQ.ToString().PadLeft(2, '0') }
														select new
														{
															F070104 = A,
															F010202 = B
														};


				var f02020108s = (from A in allloationResult.AllocationList.SelectMany(x => x.Details).ToList()
													join B in currStockData
													on new { A.ITEM_CODE, A.VALID_DATE, A.MAKE_NO } equals new { B.F070104.ITEM_CODE, VALID_DATE = Convert.ToDateTime(B.F070104.VALID_DATE), MAKE_NO = string.IsNullOrWhiteSpace(B.F070104.MAKE_NO) ? "0" : B.F070104.MAKE_NO } into subB
													from B in subB.DefaultIfEmpty()
													select new F02020108
													{
														DC_CODE = obj.DcCode,
														GUP_CODE = obj.GupCode,
														CUST_CODE = obj.CustCode,
														STOCK_NO = B.F010202.STOCK_NO,
														STOCK_SEQ = B.F010202.STOCK_SEQ,
														RT_NO = A.ALLOCATION_NO,
														RT_SEQ = A.ALLOCATION_SEQ.ToString(),
														ALLOCATION_NO = A.ALLOCATION_NO,
														ALLOCATION_SEQ = A.ALLOCATION_SEQ,
														REC_QTY = Convert.ToInt32(A.TAR_QTY),
														TAR_QTY = Convert.ToInt32(A.TAR_QTY)
													}).GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.STOCK_NO, x.STOCK_SEQ, x.RT_NO, x.RT_SEQ, x.ALLOCATION_NO, x.ALLOCATION_SEQ, x.REC_QTY, x.TAR_QTY }).Select(x => new F02020108
													{
														DC_CODE = x.Key.DC_CODE,
														GUP_CODE = x.Key.GUP_CODE,
														CUST_CODE = x.Key.CUST_CODE,
														STOCK_NO = x.Key.STOCK_NO,
														STOCK_SEQ = x.Key.STOCK_SEQ,
														RT_NO = x.Key.RT_NO,
														RT_SEQ = x.Key.RT_SEQ,
														ALLOCATION_NO = x.Key.ALLOCATION_NO,
														ALLOCATION_SEQ = x.Key.ALLOCATION_SEQ,
														REC_QTY = x.Key.REC_QTY,
														TAR_QTY = x.Key.TAR_QTY
													}).ToList();

				if (f02020108s.Any())
					addF02020108Datas.AddRange(f02020108s);
				#endregion

				allloationResult.AllocationList.ForEach(alloc =>
				{
					#region 新增F02020107 驗收調撥單對照表
					addF02020107Datas.Add(new F02020107
					{
						RT_NO = alloc.Master.ALLOCATION_NO,
						PURCHASE_NO = obj.StockNo,
						ALLOCATION_NO = alloc.Master.ALLOCATION_NO,
						DC_CODE = obj.DcCode,
						GUP_CODE = obj.GupCode,
						CUST_CODE = obj.CustCode
					});
					#endregion

					#region 新增F020202
					// 取得該調撥單明細對應表
					var currF02020108s = f02020108s.Where(x => x.ALLOCATION_NO == alloc.Master.ALLOCATION_NO && alloc.Details.Select(z => z.ALLOCATION_SEQ).Contains(x.ALLOCATION_SEQ));

					var currF010202s = f010202s.Where(x => x.STOCK_NO == obj.StockNo && currF02020108s.Select(z => z.STOCK_SEQ).Contains(x.STOCK_SEQ));

					var f020202s = (from A in alloc.Details
													join B in currF02020108s
													on new { A.ALLOCATION_NO, A.ALLOCATION_SEQ } equals new { B.ALLOCATION_NO, B.ALLOCATION_SEQ }
													join C in currF010202s
													on new { B.STOCK_NO, B.STOCK_SEQ } equals new { C.STOCK_NO, C.STOCK_SEQ }
													select new F020202
													{
														DC_CODE = req.DcNo,
														GUP_CODE = gupCode,
														CUST_CODE = req.CustNo,
														STOCK_NO = B.STOCK_NO,
														STOCK_SEQ = B.STOCK_SEQ,
														RT_NO = B.RT_NO,
														RT_SEQ = B.RT_SEQ,
														ALLOCATION_NO = B.ALLOCATION_NO,
														ALLOCATION_SEQ = B.ALLOCATION_SEQ,
														WAREHOUSE_ID = alloc.Master.TAR_WAREHOUSE_ID,
														LOC_CODE = A.TAR_LOC_CODE,
														ITEM_CODE = A.ITEM_CODE,
														VALID_DATE = A.VALID_DATE,
														ENTER_DATE = A.ENTER_DATE,
														MAKE_NO = A.MAKE_NO,
														VNR_CODE = A.VNR_CODE,
														SERIAL_NO = A.SERIAL_NO,
														BOX_CTRL_NO = A.BOX_CTRL_NO,
														PALLET_CTRL_NO = A.PALLET_CTRL_NO,
														TAR_QTY = A.TAR_QTY,
														STATUS = "0"
													}).ToList();

					if (f020202s.Any())
						addF020202Datas.AddRange(f020202s);
					#endregion

					#region 新增F010205
					var thirdPartF010205 = f010205s.Where(x => x.STOCK_NO == obj.StockNo && x.STATUS == "3").FirstOrDefault();
					var addF010205 = addF010205Datas.Where(x => x.STOCK_NO == obj.StockNo && x.STATUS == "3").FirstOrDefault();
					if (thirdPartF010205 == null && addF010205 == null)
					{
						// 第一筆狀態為 1(收貨)
						addF010205Datas.Add(new F010205
						{
							DC_CODE = obj.DcCode,
							GUP_CODE = obj.GupCode,
							CUST_CODE = obj.CustCode,
							STOCK_NO = obj.StockNo,
							RT_NO = alloc.Master.ALLOCATION_NO,
							ALLOCATION_NO = alloc.Master.ALLOCATION_NO,
							STATUS = "3",
							PROC_FLAG = "0"
						});
					}
					#endregion
				});

				#region
				obj.F070104s.ForEach(f070104 =>
				{
					var itemSeq = Convert.ToInt32(f070104.ITEM_SEQ);

					#region 撈進倉明細資料 F070104，累計明細的上架數
					var f010204 = f010204s.Where(o => o.STOCK_NO == f070104.SOURCE_NO && o.STOCK_SEQ == itemSeq).FirstOrDefault();

					// 更新F010204.TOTAL_TAR_QTY += F070104.QTY
					if (f010204 != null)
					{
						var updF010204 = updF010204Datas.Where(x => x.STOCK_NO == f070104.SOURCE_NO && x.STOCK_SEQ == itemSeq).FirstOrDefault();

						if (updF010204 == null)
						{
							f010204.TOTAL_TAR_QTY += f070104.QTY;
							updF010204Datas.Add(f010204);
						}
						else
						{
							updF010204.TOTAL_REC_QTY += f070104.QTY;
						}
					}
					#endregion

					#region 更新F070104
					f070104.STATUS = "2";
					updF070104Datas.Add(f070104);
					#endregion
				});
				#endregion
			}

			if (allocationList.Any())
			{
				// 批次上架
				var result = sharedService.BulkAllocationToAllUp(allocationList, stockList, false);
				if (!result.IsSuccessed)
					return new ApiResult { IsSuccessed = false, MsgCode = "21021", MsgContent = string.Format(_p81Service.GetMsg("21021"), result.Message) };

				// 新增調撥單
				var bulkInsertResult = sharedService.BulkInsertAllocation(allocationList, stockList, true);
				if (!bulkInsertResult.IsSuccessed)
					return new ApiResult { IsSuccessed = false, MsgCode = "21022", MsgContent = string.Format(_p81Service.GetMsg("21022"), bulkInsertResult.Message) };
			}

			// 容器釋放
			var f0701Ids = f070104s.Select(x => x.F0701_ID).Distinct().ToList();
			f0701Repo.DeleteF0701ByIds(f0701Ids);

			if (updF070104Datas.Any())
				f070104Repo.BulkUpdate(updF070104Datas);
			if (addF010205Datas.Any())
				f010205Repo.BulkInsert(addF010205Datas);
			if (updF010204Datas.Any())
				f010204Repo.BulkUpdate(updF010204Datas);
			if (addF02020107Datas.Any())
				f02020107Repo.BulkInsert(addF02020107Datas);
			if (addF02020108Datas.Any())
				f02020108Repo.BulkInsert(addF02020108Datas);
			if (addF020202Datas.Any())
				f020202Repo.BulkInsert(addF020202Datas);

			_wmsTransation.Complete();

			return new ApiResult { IsSuccessed = true, MsgCode = "20701", MsgContent = _p81Service.GetMsg("20701") };
			#endregion
		}

		/// <summary>
		/// 取得跨庫容器資料
		/// </summary>
		/// <param name="f070104s"></param>
		/// <param name="containerCode"></param>
		/// <returns></returns>
		public ApiResult GetF070104Data(ref List<F070104> f070104s, string containerCode)
		{
			var f0701Repo = new F0701Repository(Schemas.CoreSchema);
			var f070104Repo = new F070104Repository(Schemas.CoreSchema);
			var f0701s = f0701Repo.GetDatasByTrueAndCondition(o => o.CONTAINER_CODE == containerCode && o.CONTAINER_TYPE == "2");
			if (!f0701s.Any())
				return new ApiResult { IsSuccessed = false, MsgCode = "21002", MsgContent = _p81Service.GetMsg("21002") };

			f070104s = f070104Repo.GetDatasByF0701Ids(f0701s.Select(x => x.ID).ToList()).ToList();
			if (!f070104s.Any())
				return new ApiResult { IsSuccessed = false, MsgCode = "21003", MsgContent = _p81Service.GetMsg("21003") };

			return new ApiResult { IsSuccessed = true };
		}

    /// <summary>
		/// 組成跨庫進倉商品入庫日
		/// </summary>
		/// <param name="makeNo"></param>
		/// <returns></returns>
    public DateTime ComposeEnterDate(string makeNo, string stockNo, List<string> startWithTrNos)
    {
      if (startWithTrNos.Contains(stockNo) && makeNo.StartsWith("PR") && makeNo.Length >= 8)
      {
        DateTime result;
        var conposedDate = $"20{makeNo.Substring(2, 6)}";

        if (DateTime.TryParseExact(conposedDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out result))
        {
          return result;
        };
      }

      return DateTime.Today;
    }
  }
}
