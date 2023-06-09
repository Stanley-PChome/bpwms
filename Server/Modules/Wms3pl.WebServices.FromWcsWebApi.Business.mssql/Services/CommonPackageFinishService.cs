using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.TransApiServices;
using Wms3pl.WebServices.Shared.TransApiServices.Check;

namespace Wms3pl.WebServices.FromWcsWebApi.Business.mssql.Services
{
  public class CommonPackageFinishService
  {
    #region 定義需檢核欄位、必填、型態、長度
    // 包裝完成回報檢核設定
    private List<ApiCkeckColumnModel> PackageFinishReqCheckColumnList = new List<ApiCkeckColumnModel>
    {
      new ApiCkeckColumnModel{  Name = "OwnerCode",       Type = typeof(string),   MaxLength = 12,  Nullable = false },
      new ApiCkeckColumnModel{  Name = "DcCode",          Type = typeof(string),   MaxLength = 10,  Nullable = false },
      new ApiCkeckColumnModel{  Name = "ZoneCode",        Type = typeof(string),   MaxLength = 5,   Nullable = false },
      new ApiCkeckColumnModel{  Name = "OrderCode",       Type = typeof(string),   MaxLength = 32,  Nullable = false },
      new ApiCkeckColumnModel{  Name = "Operator",        Type = typeof(string),   MaxLength = 20,  Nullable = false },
      new ApiCkeckColumnModel{  Name = "StartTime",       Type = typeof(string),   MaxLength = 19,  Nullable = false, IsDateTime = true },
      new ApiCkeckColumnModel{  Name = "CompleteTime",    Type = typeof(string),   MaxLength = 19,  Nullable = false, IsDateTime = true }
    };

    private List<ApiCkeckColumnModel> BoxListCheckColumnList = new List<ApiCkeckColumnModel>
    {
      new ApiCkeckColumnModel{  Name = "BoxSeq",          Type = typeof(int),      MaxLength = 0,   Nullable = false },
      new ApiCkeckColumnModel{  Name = "BoxNo",           Type = typeof(string),   MaxLength = 20,  Nullable = false },
      new ApiCkeckColumnModel{  Name = "PrintBoxTime",    Type = typeof(string),   MaxLength = 19,  Nullable = false, IsDateTime = true }
    };

    private List<ApiCkeckColumnModel> BoxDetailCheckColumnList = new List<ApiCkeckColumnModel>
    {
      new ApiCkeckColumnModel{  Name = "SkuCode",         Type = typeof(string),   MaxLength = 20,  Nullable = false },
      new ApiCkeckColumnModel{  Name = "SkuQty",          Type = typeof(int),      MaxLength = 0,   Nullable = false }
    };
    #endregion 定義需檢核欄位、必填、型態、長度

    private TransApiBaseService _tacService;
    private WmsTransaction _wmsTransaction;
		private CommonService _commonService;

		public CommonPackageFinishService()
    {
      _tacService = new TransApiBaseService();

    }

    /// <summary>
    /// 包裝完成回報(12)
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult PackageFinish(PackageFinishReq req)
    {
      List<ApiResponse> data = new List<ApiResponse>();
			_commonService = new CommonService();
      _wmsTransaction = new WmsTransaction();

      //基本資料檢核
      var CheckBasicResult = CheckBasicData(req);
      if (!CheckBasicResult.IsSuccessed)
        return CheckBasicResult;

      string dcCode = req.DcCode;
      string gupCode = _commonService.GetGupCode(req.OwnerCode);
      string custCode = req.OwnerCode;

			#region 強制將所有序號轉大寫
			foreach (var box in req.BoxList)
			{
				foreach (var detail in box.BoxDetail)
				{
					if(detail.SerialNoList!=null)
					{
						for(var i = 0;i<detail.SerialNoList.Count;i++)
						{
							if(!string.IsNullOrWhiteSpace(detail.SerialNoList[i]))
							{
								detail.SerialNoList[i] = detail.SerialNoList[i].ToUpper();
							}
						}
					}
				}
			}
			#endregion

			//資料檢核
			var checkRes = CheckPackageFinishContent(dcCode, gupCode, custCode, req);
      //因為失敗後需要把資料還原回來，所以不管成功失敗都要跑DataProcess
      var procRes = DataProcess(dcCode, gupCode, custCode, req, checkRes.IsSuccessed);
      if (procRes.IsSuccessed)
        _wmsTransaction.Complete();

      if (!checkRes.IsSuccessed)
        return new ApiResult
        {
          IsSuccessed = false,
          MsgCode = "10005",
          MsgContent = string.Format(_tacService.GetMsg("10005"), "包裝完成回報", "0", "1", "1"),
          Data = (List<ApiResponse>)checkRes.Data
        };
    
      else
        return new ApiResult
        {
          IsSuccessed = true,
          MsgCode = "10005",
          MsgContent = string.Format(_tacService.GetMsg("10005"), "包裝完成回報", "1", "0", "1")
        };
    }

    /// <summary>
    /// 基本資料檢核
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    private ApiResult CheckBasicData(PackageFinishReq req)
    {
      ApiResult res = new ApiResult() { IsSuccessed = true };
      CheckTransWcsApiService ctaService = new CheckTransWcsApiService();

      // 檢核參數
      if (req == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "20056", MsgContent = _tacService.GetMsg("20056") };

      // 檢核物流中心 必填、是否存在
      ctaService.CheckDcCode(ref res, req);
      if (!res.IsSuccessed)
        return res;

      // 檢核貨主編號 必填、是否存在
      ctaService.CheckOwnerCode(ref res, req);
      if (!res.IsSuccessed)
        return res;

      // 檢核異常類型是否存在
      ctaService.CheckZoneCode(ref res, req);
      if (!res.IsSuccessed)
        return res;

      return res;
    }

    /// <summary>
    /// 檢查傳入的參數內容
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="req"></param>
    /// <returns></returns>
    private ApiResult CheckPackageFinishContent(string dcCode, string gupCode, string custCode, PackageFinishReq req)
    {
      ApiResult res = new ApiResult();
      List<ApiResponse> data = new List<ApiResponse>();
      var f060201Repo = new F060201Repository(Schemas.CoreSchema);
      var f051202Repo = new F051202Repository(Schemas.CoreSchema);
      var f050801Repo = new F050801Repository(Schemas.CoreSchema);
      var f060209Repo = new F060209Repository(Schemas.CoreSchema);
      var f1903Repo = new F1903Repository(Schemas.CoreSchema);
      var f2501Repo = new F2501Repository(Schemas.CoreSchema);
      var f050802Repo = new F050802Repository(Schemas.CoreSchema);
      var shipPackageService = new ShipPackageService();

      try
      {
        #region 資料檢核
        // 檢查必填欄位、欄位型態、長度、格式
        data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(req.OrderCode, req, PackageFinishReqCheckColumnList).Data);

        foreach (var itemBoxList in req.BoxList)
        {
          data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(req.OrderCode, itemBoxList, BoxListCheckColumnList).Data);

          foreach (var itemBoxDetail in itemBoxList.BoxDetail)
            data.AddRange((List<ApiResponse>)CheckColumnNotNullAndMaxLength(req.OrderCode, itemBoxDetail, BoxDetailCheckColumnList).Data);
        }



        #region 檢查單號是否存在於控管資料表[F075109.DOC_ID]-請做Lock機制
        var F075109Repo = new F075109Repository(Schemas.CoreSchema);
        var f075109 = F075109Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
          new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
           () =>
           {
             var lockF0701 = F075109Repo.LockF075109();
             var chkF075109 = F075109Repo.Find(x => x.DC_CODE == req.DcCode && x.DOC_ID == req.OrderCode);
             if (chkF075109 != null)
               return null;
             var newF075109 = new F075109()
             {
               DC_CODE = req.DcCode,
               DOC_ID = req.OrderCode
             };
             F075109Repo.Add(newF075109);
             return newF075109;
           });
        if (f075109 == null)
        {
          data.Add(new ApiResponse { No = req.OrderCode, ErrorColumn = "OrderCode", MsgCode = "20964", MsgContent = string.Format(_tacService.GetMsg("20964"), req.OrderCode) });
          res.Data = data;
          return res;
        }

        #endregion 檢查單號是否存在於控管資料表[F075109.DOC_ID]-請做Lock機制

        #region 檢查是否存在任務單號[F060201.DOC_ID] 
        var f060201 = f060201Repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.DOC_ID == req.OrderCode && x.CMD_TYPE == "1");
        if (f060201 == null)
        {
          data.Add(new ApiResponse { No = req.OrderCode, ErrorColumn = "OrderCode", MsgCode = "20040", MsgContent = string.Format(_tacService.GetMsg("20040"), req.OrderCode) });
          res.Data = data;
          return res;
        }
        #endregion 檢查是否存在任務單號[F060201.DOC_ID] 

        #region 檢查相關單據狀態
        var WmsNo = f060201.WMS_NO;
        var f051202s = f051202Repo.GetDatasByWmsOrdNo(dcCode, gupCode, custCode, WmsNo).ToList();
        var f050801 = f050801Repo.GetData(WmsNo, gupCode, custCode, dcCode);

        switch (WmsNo.Substring(0, 1).ToUpper())
        {
          case "P":
            data.Add(new ApiResponse { No = req.OrderCode, ErrorColumn = "OrderCode", MsgCode = "20859", MsgContent = _tacService.GetMsg("20859") });
            break;
          case "T":
            data.Add(new ApiResponse { No = req.OrderCode, ErrorColumn = "OrderCode", MsgCode = "20860", MsgContent = _tacService.GetMsg("20860") });
            break;
          case "O":
            if (f051202s == null || !f051202s.Any())
              //如果有看到這錯誤代表前面資料處理可能哪邊有問題
              data.Add(new ApiResponse { No = req.OrderCode, ErrorColumn = "OrderCode", MsgContent = "找不到揀貨單" });
            else if (f050801 == null)
              //如果有看到這錯誤代表前面資料處理可能哪邊有問題
              data.Add(new ApiResponse { No = req.OrderCode, ErrorColumn = "OrderCode", MsgContent = "找不到出貨單" });
            else if (f051202s.Select(x => x.PICK_ORD_NO).Distinct().Count() > 1)
              data.Add(new ApiResponse { No = req.OrderCode, ErrorColumn = "OrderCode", MsgCode = "20861", MsgContent = _tacService.GetMsg("20861") });
            else if (f051202s.Any(x => x.PICK_STATUS == "1" && x.B_PICK_QTY > x.A_PICK_QTY))
              data.Add(new ApiResponse { No = req.OrderCode, ErrorColumn = "OrderCode", MsgCode = "20862", MsgContent = _tacService.GetMsg("20862") });
            else if (f050801.ORD_TYPE == "0")
              data.Add(new ApiResponse { No = req.OrderCode, ErrorColumn = "OrderCode", MsgCode = "20863", MsgContent = _tacService.GetMsg("20863") });
            else if (!string.IsNullOrWhiteSpace(f050801.MOVE_OUT_TARGET))
              data.Add(new ApiResponse { No = req.OrderCode, ErrorColumn = "OrderCode", MsgCode = "20864", MsgContent = _tacService.GetMsg("20864") });
            break;
        }
        if (data.Any())
        {
          res.Data = data;
          return res;
        }

        #endregion 檢查相關單據狀態

        //檢查是否存在外部出貨包裝單據主檔[F060209]
        if (f060209Repo.GetDataByDocId(dcCode, gupCode, custCode, req.OrderCode) != null)
        {
          data.Add(new ApiResponse { No = req.OrderCode, ErrorColumn = "OrderCode", MsgCode = "20049", MsgContent = string.Format(_tacService.GetMsg("20049"), req.OrderCode) });
          res.Data = data;
          return res;
        }

        //檢查包裝模式
        var chkPackageModeResult = shipPackageService.CheckPackageMode(f050801, "3");
        if (!chkPackageModeResult.IsSuccessed)
          data.Add(new ApiResponse { No = req.OrderCode, ErrorColumn = "OrderCode", MsgCode = chkPackageModeResult.MsgCode, MsgContent = chkPackageModeResult.MsgContent });

        //檢查商品是否存在，不存在，顯示[20076, SKU_CODE]
        var reqItemCodes = req.BoxList.SelectMany(a => a.BoxDetail.Select(b => b.SkuCode));
        var f1903s = _commonService.GetProductList(gupCode, custCode, reqItemCodes.ToList()).ToList();
        var checkItemCodeExists = reqItemCodes.Except(f1903s.Select(x => x.ITEM_CODE));
        if (checkItemCodeExists.Any())
          data.Add(new ApiResponse
          {
            No = req.OrderCode,
            ErrorColumn = "SKU_CODE",
            MsgCode = "20076",
            MsgContent = string.Format(_tacService.GetMsg("20076"), string.Join(",", checkItemCodeExists))
          });

				#region 商品為序號商品檢查
				var f1903SerialItems = f1903s.Where(x => x.BUNDLE_SERIALNO == "1" || x.BUNDLE_SERIALLOC == "1");
        if (f1903SerialItems.Any())
        {
          var getReqSerialList = req.BoxList.SelectMany(x => x.BoxDetail).Where(a => f1903SerialItems.Any(b => b.ITEM_CODE == a.SkuCode));
          //a.檢查是否有提供商品序號，若無提供回傳[20122]
          if (getReqSerialList.Any(x => !x.SerialNoList?.Any() ?? true))
            data.Add(new ApiResponse { No = req.OrderCode, ErrorColumn = "SerialNoList", MsgCode = "20122", MsgContent = _tacService.GetMsg("20122") });

          //b.檢查商品序號筆數是否等於商品數量，若不同回傳[商品xxx為序號商品，商品序號數量與出貨數不同]
          var checkSerialCount = getReqSerialList.Where(x => x.SkuQty != x.SerialNoList?.Count());
          if (checkSerialCount.Any())
            data.Add(new ApiResponse
            {
              No = req.OrderCode,
              ErrorColumn = "SerialNoList",
              MsgCode= "20865",
              MsgContent = string.Format(_tacService.GetMsg("20865"), string.Join(",", checkSerialCount.Select(x=>x.SkuCode)))
            });

          //c.檢查商品序號是重複，若重複回傳[20096,ORDER_CODE,SKU_CODE,商品序號]
          var checkSerialDuplicate = getReqSerialList
              .SelectMany(x => x.SerialNoList, (a, SerialNo) => new { a.SkuCode, SerialNo })
              .GroupBy(x => x.SerialNo)
              .Where(x => x.Count() > 1);
          if (checkSerialDuplicate.Any())
            foreach (var item in checkSerialDuplicate)
              data.Add(new ApiResponse
              {
                No = req.OrderCode,
                ErrorColumn = "SerialNoList",
                MsgCode = "20096",
                MsgContent = string.Format(_tacService.GetMsg("20096"), req.OrderCode, string.Join(",", item.Select(x => x.SkuCode).Distinct()), item.Key)
              });

          //d.檢查商品序號是否存在，若不存在回傳[商品序號xxx不存在]
          var reqSerials = getReqSerialList.SelectMany(x => x.SerialNoList).ToList();
          var f2501s = _commonService.GetItemSerialList(gupCode, custCode, reqSerials).ToList();
          var checkSerialExists = reqSerials.Except(f2501s.Select(x => x.SERIAL_NO));
          if (checkSerialExists.Any())
          {
            data.Add(new ApiResponse
            {
              No = req.OrderCode,
              ErrorColumn = "SerialNoList",
              MsgCode = "20866",
              MsgContent = string.Format(_tacService.GetMsg("20866"), string.Join(",", checkSerialExists))
            });
          }

          //e.檢查商品序號是否為在庫序號[F2501.STATUS=A1]，若非在庫序號，回傳[商品XXX，商品序號OOO非在庫序號]
          var checkA1Serial = from a in getReqSerialList.SelectMany(a => a.SerialNoList, (a, b) => new { a.SkuCode, SerialNo = b })
                              join b in f2501s
                              on a.SerialNo equals b.SERIAL_NO
                              where b.STATUS != "A1"
                              select new { a.SkuCode, b.SERIAL_NO };
          if (checkA1Serial.Any())
          {
            data.AddRange(checkA1Serial.GroupBy(x => x.SkuCode).Select(x => new ApiResponse()
            {
              No = req.OrderCode,
              ErrorColumn = "SerialNoList",
              MsgCode = "20867",
              MsgContent = string.Format(_tacService.GetMsg("20867"), x.Key, string.Join(",", x.Select(a => a.SERIAL_NO)))
            }));
          }

          //f. 檢查SeralNoList總長度是否超過資料庫長度(序號用逗點分隔)，若是回傳[20044]
          if (getReqSerialList.Any(a => string.Join(",", a.SerialNoList).Length > 8000))
            data.Add(new ApiResponse { No = req.OrderCode, ErrorColumn = "SerialNoList", MsgCode = "20044", MsgContent = string.Format(_tacService.GetMsg("20044"), req.OrderCode) });

          //g. 檢查商品序號的品號[F2501.ITEM_CODE]是否為該筆出貨品號[SKU_CODE]，若不是回傳[商品序號XXX非此出貨商品OOO的序號]
          var checkIsItemSerial = getReqSerialList.SelectMany(a => a.SerialNoList, (a, b) => new { a.SkuCode, SerialNo = b })
                                                  .GroupJoin(f2501s, a => a.SerialNo, b => b.SERIAL_NO, (a, b) => new { a.SkuCode, a.SerialNo, f2501ItemCode = b.FirstOrDefault().ITEM_CODE })
                                                  .Where(x => string.IsNullOrEmpty(x.f2501ItemCode) || x.SkuCode != x.f2501ItemCode);
          if (checkIsItemSerial.Any())
          {
            data.AddRange(checkIsItemSerial.Select(x => new ApiResponse()
            {
              No = req.OrderCode,
              ErrorColumn = "SerialNoList",
              MsgCode = "20877",
              MsgContent = string.Format(_tacService.GetMsg("20877"), x.SerialNo, x.SkuCode)
            }));
          }

        }
				#endregion 

				#region 商品為非序號商品檢查
				var notSerialItems = f1903s.Where(x => x.BUNDLE_SERIALNO =="0");
				if(notSerialItems.Any())
				{
					var getHasSerialList = req.BoxList.SelectMany(x => x.BoxDetail).Where(a => notSerialItems.Any(b => b.ITEM_CODE == a.SkuCode) && a.SerialNoList!=null && a.SerialNoList.Any());
					
					
					//a.檢查商品序號筆數是否等於商品數量，若不同回傳[商品xxx為非序號商品但有提供序號，商品序號數量與出貨數不同]
					var checkSerialCount = getHasSerialList.Where(x => x.SkuQty != x.SerialNoList.Count());
					if (checkSerialCount.Any())
						data.Add(new ApiResponse
						{
							No = req.OrderCode,
							ErrorColumn = "SerialNoList",
							MsgCode = "20878",
							MsgContent = string.Format(_tacService.GetMsg("20878"), string.Join(",", checkSerialCount.Select(x => x.SkuCode)))
						});

					//b.檢查商品序號是重複，若重複回傳[20096,ORDER_CODE,SKU_CODE,商品序號]
					var checkSerialDuplicate = getHasSerialList
							.SelectMany(x => x.SerialNoList, (a, SerialNo) => new { a.SkuCode, SerialNo })
							.GroupBy(x => x.SerialNo)
							.Where(x => x.Count() > 1);
					if (checkSerialDuplicate.Any())
						foreach (var item in checkSerialDuplicate)
							data.Add(new ApiResponse
							{
								No = req.OrderCode,
								ErrorColumn = "SerialNoList",
								MsgCode = "20096",
								MsgContent = string.Format(_tacService.GetMsg("20096"), req.OrderCode, string.Join(",", item.Select(x => x.SkuCode).Distinct()), item.Key)
							});

					//c.檢查商品序號是否存在，若不存在回傳[商品序號xxx不存在]
					var reqSerials = getHasSerialList.SelectMany(x => x.SerialNoList).ToList();
					var f2501s = _commonService.GetItemSerialList(gupCode, custCode, reqSerials).ToList();
					var checkSerialExists = reqSerials.Except(f2501s.Select(x => x.SERIAL_NO));
					if (checkSerialExists.Any())
					{
						data.Add(new ApiResponse
						{
							No = req.OrderCode,
							ErrorColumn = "SerialNoList",
							MsgCode = "20866",
							MsgContent = string.Format(_tacService.GetMsg("20866"), string.Join(",", checkSerialExists))
						});
					}

					//d.檢查商品序號是否為在庫序號[F2501.STATUS=A1]，若非在庫序號，回傳[商品XXX，商品序號OOO非在庫序號]
					var checkA1Serial = from a in getHasSerialList.SelectMany(a => a.SerialNoList, (a, b) => new { a.SkuCode, SerialNo = b })
															join b in f2501s
															on a.SerialNo equals b.SERIAL_NO
															where b.STATUS != "A1"
															select new { a.SkuCode, b.SERIAL_NO };
					if (checkA1Serial.Any())
					{
						data.AddRange(checkA1Serial.GroupBy(x => x.SkuCode).Select(x => new ApiResponse()
						{
							No = req.OrderCode,
							ErrorColumn = "SerialNoList",
							MsgCode = "20867",
							MsgContent = string.Format(_tacService.GetMsg("20867"), x.Key, string.Join(",", x.Select(a => a.SERIAL_NO)))
						}));
					}

					//e. 檢查SeralNoList總長度是否超過資料庫長度(序號用逗點分隔)，若是回傳[20044]
					if (getHasSerialList.Any(a => string.Join(",", a.SerialNoList).Length > 1000))
						data.Add(new ApiResponse { No = req.OrderCode, ErrorColumn = "SerialNoList", MsgCode = "20044", MsgContent = string.Format(_tacService.GetMsg("20044"), req.OrderCode) });

					//f. 檢查商品序號的品號[F2501.ITEM_CODE]是否為該筆出貨品號[SKU_CODE]，若不是回傳[商品序號XXX非此出貨商品OOO的序號]
					var checkIsItemSerial = getHasSerialList.SelectMany(a => a.SerialNoList, (a, b) => new { a.SkuCode, SerialNo = b })
																									.GroupJoin(f2501s, a => a.SerialNo, b => b.SERIAL_NO, (a, b) => new { a.SkuCode, a.SerialNo, f2501ItemCode = b.FirstOrDefault().ITEM_CODE })
																									.Where(x => string.IsNullOrEmpty(x.f2501ItemCode) || x.SkuCode != x.f2501ItemCode);
					if (checkIsItemSerial.Any())
					{
						data.AddRange(checkIsItemSerial.Select(x => new ApiResponse()
						{
							No = req.OrderCode,
							ErrorColumn = "SerialNoList",
							MsgCode = "20877",
							MsgContent = string.Format(_tacService.GetMsg("20877"), x.SerialNo, x.SkuCode)
						}));
					}
				}



				#endregion 


				//20220907會議上詢問Scott BoxSeq要檢查，不可重複
				var checkBoxSeq = req.BoxList.GroupBy(x => x.BoxSeq).Select(x => new { x.Key, Count = x.Count() }).Where(x => x.Count > 1);
        if (checkBoxSeq.Any())
        {
          data.Add(new ApiResponse
          {
            No = req.OrderCode,
            ErrorColumn = "BoxSeq",
            MsgCode = "20868",
            MsgContent = string.Format(_tacService.GetMsg("20868"), string.Join(",", checkBoxSeq.Select(x => x.Key)))
          });
        }
     
        //檢查紙箱編號[BOX_NO]是否存在商品主檔[F1903.ISCARTON=1 AND ITEM_CODE = SKU_CODE] OR BOX_NO=ORI 若不存在，回傳[紙箱編號XXX不存在]
        var f1903Cartons = f1903Repo.GetDatasByItems(gupCode, custCode, req.BoxList.Select(x => x.BoxNo).ToList()).Where(x => x.ISCARTON == "1").ToList();
        //排除掉參數的BoxNo!=ORI的內容後，再排除商品檔的紙箱內容，這樣就能取得不在商品主檔的紙箱編號
        var checkBoxNo = req.BoxList.Where(x => x.BoxNo.ToUpper() != "ORI").Select(x => x.BoxNo).Except(f1903Cartons.Select(x => x.ITEM_CODE));
        if (checkBoxNo.Any())
          data.AddRange(checkBoxNo.Select(x =>
            new ApiResponse()
            {
              No = req.OrderCode,
              ErrorColumn = "BoxNo",
              MsgCode = "20876",
              MsgContent = string.Format(_tacService.GetMsg("20876"), x)
            }));

        //檢查商品數量[SKU_QTY]<=0，回傳[20069]
        if (req.BoxList.Any(a => a.BoxDetail.Any(b => b.SkuQty <= 0)))
          data.Add(new ApiResponse
          {
            No = req.OrderCode,
            ErrorColumn = "SkuQty",
            MsgCode = "20069",
            MsgContent = _tacService.GetMsg("20069")
          });

        #region 檢查出貨單明細與包裝紀錄比較是否有差異
        //檢查包裝記錄品號是否不存在於出貨單出貨明細商品中，若不存在回傳[包裝紀錄中不可有非出貨單商品，品號如下: xxx, xxx]
        var f050802s = f050802Repo.GetDatas(dcCode, gupCode, custCode, req.OrderCode);
        var checkShipItemExists = req.BoxList.SelectMany(a => a.BoxDetail.Select(b => b.SkuCode)).Except(f050802s.Select(x => x.ITEM_CODE));
        if (checkShipItemExists.Any())
          data.Add(new ApiResponse
          {
            No = req.OrderCode,
            ErrorColumn = "SkuCode",
            MsgCode = "20869",
            MsgContent = string.Format(_tacService.GetMsg("20869"), string.Join(",", checkShipItemExists))
          });

        //檢查出貨單明細商品中，不存在於包裝紀錄品號，若不存在回傳[包裝紀錄缺少應出貨商品，品號如下: xxx, xxx]
        checkShipItemExists = f050802s.Select(x => x.ITEM_CODE).Except(req.BoxList.SelectMany(a => a.BoxDetail.Select(b => b.SkuCode)));
        if (checkShipItemExists.Any())
          data.Add(new ApiResponse
          {
            No = req.OrderCode,
            ErrorColumn = "SkuCode",
            MsgCode = "20870",
            MsgContent = string.Format(_tacService.GetMsg("20870"), string.Join(",", checkShipItemExists))
          });

        //檢查包裝紀錄商品數量是否與出貨單出貨明細商品數量一致 若不一致，回傳[商品xxx應出數量oo與實出數量不一致]
        var checkShipItemQty = from a in req.BoxList.SelectMany(x => x.BoxDetail).GroupBy(x => x.SkuCode).Select(x => new { SkuCode = x.Key, SkuQty = x.Sum(a => a.SkuQty) })
                               join b in f050802s
                               on a.SkuCode equals b.ITEM_CODE
                               where a.SkuQty != b.B_DELV_QTY
                               select new { a.SkuCode, a.SkuQty, b.B_DELV_QTY };
        if (checkShipItemQty.Any())
          data.AddRange(checkShipItemQty.Select(x =>
            new ApiResponse()
            {
              No = req.OrderCode,
              ErrorColumn = "SkuQty",
              MsgCode = "20871",
              MsgContent = string.Format(_tacService.GetMsg("20871"), x.SkuCode, x.B_DELV_QTY)
            }));

        #endregion 檢查出貨單明細與包裝紀錄比較是否有差異

        #endregion 資料檢核
      }
      catch (Exception ex)
      {
        data.Add(new ApiResponse
        {
          No = req.OrderCode,
          MsgContent = ex.Message
        });
        res.IsSuccessed = false;
        res.Data = data;
        return res;
      }

      res.IsSuccessed = !data.Any();
      res.Data = data;
      return res;
    }

    /// <summary>
    /// 檢查傳入的API參數內容是否正確
    /// </summary>
    /// <typeparam name="T1">API參數結構</typeparam>
    /// <typeparam name="T2">檢查結構</typeparam>
    /// <param name="OrdNo">單號</param>
    /// <param name="req">API參數</param>
    /// <param name="DefCheckColumnList">檢查定義內容</param>
    /// <returns></returns>
    private ApiResult CheckColumnNotNullAndMaxLength<T1, T2>(string OrdNo, T1 req, List<T2> DefCheckColumnList) where T2 : ApiCkeckColumnModel
    {
      ApiResult res = new ApiResult();
      List<ApiResponse> data = new List<ApiResponse>();

      string nullErrorMsg = _tacService.GetMsg("20034");
      string formatErrorMsg = _tacService.GetMsg("20035");
      string datetimeErrorMsg = _tacService.GetMsg("20050");

      List<string> notDateColumn = new List<string>();

      if (req == null)
      {
        data.Add(new ApiResponse { No = OrdNo, ErrorColumn = req.GetType().Name, MsgCode = "20058", MsgContent = string.Format(_tacService.GetMsg("20058"), OrdNo, "BoxList") });
        res.Data = data;
        return res;
      }

      // 找出欄位不符合需必填、超過最大長度的欄位後，寫入各List
      DefCheckColumnList.ForEach(column =>
      {
        var value = Convert.ToString(DataCheckHelper.GetRequireColumnValue(req, column.Name));

        // 必填
        if (!column.Nullable)
          if (!DataCheckHelper.CheckRequireColumn(req, column.Name))
            data.Add(new ApiResponse { No = OrdNo, ErrorColumn = column.Name, MsgCode = "20034", MsgContent = string.Format(nullErrorMsg, column.Name) });

        // 最大長度
        if (column.MaxLength > 0)
          if (!DataCheckHelper.CheckDataMaxLength(req, column.Name, column.MaxLength))
            data.Add(new ApiResponse { No = OrdNo, ErrorColumn = column.Name, MsgCode = "20035", MsgContent = string.Format(formatErrorMsg, column.Name) });

        // 判斷是否為日期格式(yyyy/MM/dd HH:mm:ss)字串
        if (column.IsDateTime && !string.IsNullOrWhiteSpace(value))
          if (!DataCheckHelper.CheckDataIsDateTime(req, column.Name))
          {
            string errDateValue = req.GetType().GetProperty(column.Name).GetValue(req).ToString();
            data.Add(new ApiResponse { No = OrdNo, ErrorColumn = column.Name, MsgCode = "20050", MsgContent = string.Format(datetimeErrorMsg, errDateValue, column.Name) });
          }
      });

      if (data.Any())
        res.IsSuccessed = false;
      res.Data = data;

      return res;
    }

    private ApiResult DataProcess(string dcCode, string gupCode, string custCode, PackageFinishReq req, Boolean IsCheckPass)
    {
      var failWmsTransaction = new WmsTransaction();
      var f075109Repo = new F075109Repository(Schemas.CoreSchema, _wmsTransaction);
      var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
      var f060201Repo = new F060201Repository(Schemas.CoreSchema);
      var f060209Repo = new F060209Repository(Schemas.CoreSchema, _wmsTransaction);
      var f06020901Repo = new F06020901Repository(Schemas.CoreSchema, _wmsTransaction);
      var f06020902Repo = new F06020902Repository(Schemas.CoreSchema, _wmsTransaction);
      var containerService = new ContainerService(_wmsTransaction);
      var addF06020901 = new List<F06020901>();
      var addF06020902 = new List<F06020902>();

      var f060201 = f060201Repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.DOC_ID == req.OrderCode && x.CMD_TYPE == "1");

      //若資料檢核失敗，或發生例外
      if (!IsCheckPass)
      {
        //發生例外的話，要用獨立的Transaction去跑，避免跟正常資料包在一起跑
        f060209Repo = new F060209Repository(Schemas.CoreSchema, failWmsTransaction);
        f075109Repo = new F075109Repository(Schemas.CoreSchema, failWmsTransaction);
        f050801Repo = new F050801Repository(Schemas.CoreSchema, failWmsTransaction);

        //檢查是否存在外部出貨包裝單據主檔[F060209],如果單據是已經存在的話，不可以把資料還原
        if (f060209Repo.GetDataByDocId(dcCode, gupCode, custCode, req.OrderCode) == null)
        {
          f075109Repo.Delete(x => x.DC_CODE == dcCode && x.DOC_ID == req.OrderCode);
          //檢核失敗，有可能是找不到出貨單的狀況
          if (f060201 != null)
            f050801Repo.UpdateFields(new { SHIP_MODE = "0" },
              x => x.DC_CODE == dcCode
              && x.GUP_CODE == gupCode
              && x.CUST_CODE == custCode
              && x.WMS_ORD_NO == f060201.WMS_NO
              && x.SHIP_MODE == "3");
        }

        failWmsTransaction.Complete();
      }
      else
      {
        containerService.DelContainer(dcCode, gupCode, custCode, f060201.WMS_NO);

        f060209Repo.Add(new F060209()
        {
          DC_CODE = dcCode,
          GUP_CODE = gupCode,
          CUST_CODE = custCode,
          WAREHOUSE_ID = req.ZoneCode,
          DOC_ID = req.OrderCode,
          WMS_NO = f060201.WMS_NO,
          PICK_NO = f060201.PICK_NO,
          OPERATOR = req.Operator,
          START_TIME = req.StartTime,
          COMPLETE_TIME = req.CompleteTime,
          PROC_FLAG = 0
        });

        addF06020901.AddRange(req.BoxList.Select(x => new F06020901()
        {
          DOC_ID = req.OrderCode,
          BOX_SEQ = x.BoxSeq,
          BOX_NO = x.BoxNo,
          PRINT_BOX_TIME = x.PrintBoxTime,
          PROC_FLAG = 0
        }));
        f06020901Repo.BulkInsert(addF06020901);

        addF06020902.AddRange(from a in req.BoxList
                              from b in a.BoxDetail
                              select new F06020902()
                              {
                                DOC_ID = req.OrderCode,
                                BOX_SEQ = a.BoxSeq,
                                SKU_CODE = b.SkuCode,
                                SKU_QTY = b.SkuQty,
                                SERIAL_NO_LIST = b.SerialNoList == null ? null : string.Join(",", b.SerialNoList)
                              });

        f06020902Repo.BulkInsert(addF06020902);
      }

      return new ApiResult() { IsSuccessed = true };
    }

  }
}
