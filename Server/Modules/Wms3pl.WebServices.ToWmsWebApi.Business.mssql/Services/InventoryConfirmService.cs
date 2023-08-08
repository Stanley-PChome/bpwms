using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.ToWmsWebApi.Business.mssql.Services
{
  public class InventoryConfirmService : BaseService
  {
    /// <summary>
    /// 盤點完成
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult InventoryConfirm(WcsImportReq req)
    {
      ApiResult res = new ApiResult { IsSuccessed = true };
      List<ApiResponse> data = new List<ApiResponse>();

      // 新增API Log
      res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ImportInventoryConfirmResults", req, () =>
      {
        // 取得物流中心服務貨主檔
        CommonService commonService = new CommonService();
        var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);
        dcCustList.ForEach(item =>
              {
                var result = ImportInventoryConfirmResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);
                data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
              });
        res.Data = JsonConvert.SerializeObject(data);
        return res;
      }, true);

      return res;
    }

    /// <summary>
    /// 盤點完成
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    private ApiResult ImportInventoryConfirmResults(string dcCode, string gupCode, string custCode)
    {
      #region 變數設定
      ApiResult res = new ApiResult { IsSuccessed = true };
      var wmsTransaction = new WmsTransaction();
      var f140101Repo = new F140101Repository(Schemas.CoreSchema, wmsTransaction);
      var f140104Repo = new F140104Repository(Schemas.CoreSchema, wmsTransaction);
      var f140105Repo = new F140105Repository(Schemas.CoreSchema, wmsTransaction);
      var f060402Repo = new F060402Repository(Schemas.CoreSchema);
      var f060403Repo = new F060403Repository(Schemas.CoreSchema, wmsTransaction);
      var f1909Repo = new F1909Repository(Schemas.CoreSchema);
      var f1924Repo = new F1924Repository(Schemas.CoreSchema);
      var f1913Repo = new F1913Repository(Schemas.CoreSchema, wmsTransaction);
      var f020203Repo = new F020203Repository(Schemas.CoreSchema);
      var today = DateTime.Today;
      var validDate = new DateTime(9999, 12, 31);
      int successCnt = 0;
      #endregion

      #region 主要邏輯
      // 取得要執行盤點完成結果回傳資料
      var f060402s = f060402Repo.GetDatasForExecute(dcCode, gupCode, custCode).ToList();

      // 取得原任務單號
      var docIds = f060402s.Select(x => x.DOC_ID).ToList();

      var executeDatas = f060402s.OrderBy(x => x.DOC_ID).ToList();

      if (executeDatas.Any())
      {
        var inventoryNos = f060402s.Select(x => x.WMS_NO).ToList();

        // 取得盤點單主檔資料
        var f140101s = f140101Repo.GetDatas(dcCode, gupCode, custCode, inventoryNos).ToList();

        // 取得盤點完成結果回傳明細資料
        var f060403s = f060403Repo.GetDatasForExecute(dcCode, gupCode, custCode, docIds).ToList();

        // 取得盤點單初盤明細資料
        var f140104s = f140104Repo.AsForUpdate().GetDatasByWcsInventoryNos(dcCode, gupCode, custCode, inventoryNos).ToList();

        // 取得盤點單複盤明細資料
        var f140105s = f140105Repo.AsForUpdate().GetDatasByWcsInventoryNos(dcCode, gupCode, custCode, inventoryNos).ToList();

        // 取得人員資料
        var f1924s = f1924Repo.GetDatasForEmpIds(f060402s.Select(x => x.OPERATOR).ToList()).ToList();

        // 取得盤盈是否回沖(0否1是)
        var f1909Data = f1909Repo.GetDatasByTrueAndCondition(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode).ToList();
        string flushbackByF1909 = "0";
        if (f1909Data.Any())
          flushbackByF1909 = f1909Data.FirstOrDefault().FLUSHBACK;

        // Foreach #取得要執行盤點完成結果回傳資料
        executeDatas.ForEach(f060402 =>
        {
          #region 更新 F060402 處理中狀態
          // 更新執行盤點完成結果回傳資料表處理中狀態
          f060402.STATUS = "1";
          f060402.PROC_DATE = DateTime.Now;
          f060402Repo.Update(f060402);
          #endregion

          #region 資料處理
          var f140101 = f140101s.Where(x => x.INVENTORY_NO == f060402.WMS_NO).FirstOrDefault();

          if (f140101 == null)
            f060402.STATUS = "F";
          else
          {
            // 取得當前盤點人員資料
            var currF1924 = f1924s.Where(x => x.EMP_ID == f060402.OPERATOR).FirstOrDefault();
            var inventoryName = currF1924 == null ? "支援人員" : currF1924.EMP_NAME;

            // 取得當前盤點完成結果回傳明細
            var currF060403s = f060403s.Where(x => x.DOC_ID == f060402.DOC_ID)
            .GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.WMS_NO, x.WAREHOUSE_ID, x.SKUCODE, x.EXPIRYDATE, x.OUTBATCHCODE })
            .Select(x => new InventoryConfirmResultModel
            {
              DC_CODE = x.Key.DC_CODE,
              GUP_CODE = x.Key.GUP_CODE,
              CUST_CODE = x.Key.CUST_CODE,
              WMS_NO = x.Key.WMS_NO,
              WAREHOUSE_ID = x.Key.WAREHOUSE_ID,
              SKUCODE = x.Key.SKUCODE,
              EXPIRYDATE = Convert.ToDateTime(x.Key.EXPIRYDATE),
              OUTBATCHCODE = x.Key.OUTBATCHCODE,
              SKUSYSQTY = x.Sum(z => z.SKUSYSQTY),
              SKUQTY = x.Sum(z => z.SKUQTY),
              OPERATORTIME = Convert.ToDateTime(x.Max(z => z.OPERATORTIME))
            }).ToList();

            var processedAfterKeys = new List<StockDataByInventoryParam>();

            if (f140101.ISSECOND == "0")
            {
              #region 初盤

              #region (1) 從F140104中WMS庫存數(QTY+UNMOVE_STOCK_QTY) 取得>0的庫存明細 
              var currF140104s = f140104s.Where(x => x.INVENTORY_NO == f060402.WMS_NO && x.WAREHOUSE_ID == f060402.WAREHOUSE_ID)
              .Select(x => new FirstInventoryModel { F140104 = x, AllQty = Convert.ToInt32(x.QTY) + Convert.ToInt32(x.UNMOVE_STOCK_QTY) })
              .ToList();
              #endregion

              // 取得實際庫存數
              var stockDatas = new List<StockDataByInventory>();
              foreach (var F140104 in currF140104s)
              {
                var resData = f1913Repo.GetStockQtyByInventory(dcCode, gupCode, custCode, f060402.WAREHOUSE_ID,
                   new StockDataByInventoryParam
                   {
                     ITEM_CODE = F140104.F140104.ITEM_CODE,
                     VALID_DATE = F140104.F140104.VALID_DATE,
                     MAKE_NO = F140104.F140104.MAKE_NO,
                     LOC_CODE = F140104.F140104.LOC_CODE,
                     ENTER_DATE = F140104.F140104.ENTER_DATE,
                     BOX_CTRL_NO = F140104.F140104.BOX_CTRL_NO,
                     PALLET_CTRL_NO = F140104.F140104.PALLET_CTRL_NO
                   }).ToList();

                if (resData != null && resData.Any())
                  stockDatas.AddRange(resData);
                else
                  stockDatas.Add(new StockDataByInventory()
                  {
                    ITEM_CODE = F140104.F140104.ITEM_CODE,
                    VALID_DATE = F140104.F140104.VALID_DATE,
                    MAKE_NO = F140104.F140104.MAKE_NO,
                    LOC_CODE = F140104.F140104.LOC_CODE,
                    ENTER_DATE = F140104.F140104.ENTER_DATE,
                    BOX_CTRL_NO = F140104.F140104.BOX_CTRL_NO,
                    PALLET_CTRL_NO = F140104.F140104.PALLET_CTRL_NO,
                    QTY = 0,
                    UNMOVE_STOCK_QTY = 0
                  });
              }

              // 結果回傳資料若對應不到明細需要取第一筆盤點明細的儲位來填入新增的明細
              var locCode = currF140104s.Any() ? currF140104s.First().F140104.LOC_CODE : string.Empty;

              // Foreach結果回傳 逐筆去找對應的盤點明細來做分配盤點數及自動倉庫存數
              currF060403s.ForEach(f060403 =>
              {
                // 取得該結果回傳對應的盤點明細
                var currItemF140104s = currF140104s.Where(x => x.F140104.ITEM_CODE == f060403.SKUCODE &&
                x.F140104.VALID_DATE == f060403.EXPIRYDATE &&
                x.F140104.MAKE_NO == f060403.OUTBATCHCODE);

                if (currItemF140104s.Any())
                {
                  #region (2) 先找WMS庫存數=自動倉實際盤點數 (排序QTY DESC)，若有先分配給這筆的實際盤點數，自動倉系統庫存數也分配給這筆但不能超過WMS庫存數
                  var sameQtyF140104s = currItemF140104s.Where(x => x.AllQty > 0 && x.AllQty == f060403.SKUQTY)
.OrderByDescending(x => x.F140104.QTY)
.OrderByDescending(x => x.F140104.ENTER_DATE).ToList();
                  sameQtyF140104s.ForEach(obj =>
                  {
                    if (obj.AllQty > 0)
                    {
                      var currStock = GetStockData(stockDatas, obj.F140104.ITEM_CODE, obj.F140104.MAKE_NO, obj.F140104.VALID_DATE, obj.F140104.LOC_CODE, obj.F140104.ENTER_DATE, obj.F140104.BOX_CTRL_NO, obj.F140104.PALLET_CTRL_NO);

                      obj.F140104.DEVICE_STOCK_QTY = f060403.SKUSYSQTY > f060403.SKUQTY ? f060403.SKUQTY : f060403.SKUSYSQTY;
                      obj.F140104.FIRST_QTY = f060403.SKUQTY;
                      obj.F140104.FST_INVENTORY_STAFF = f060402.OPERATOR;
                      obj.F140104.FST_INVENTORY_NAME = inventoryName;
                      obj.F140104.FST_INVENTORY_DATE = f060403.OPERATORTIME;
                      obj.F140104.FST_INVENTORY_PC = "自動倉PC";
                      obj.F140104.QTY = currStock.Sum(x => x.QTY);
                      obj.F140104.UNMOVE_STOCK_QTY = currStock.Sum(x => x.UNMOVE_STOCK_QTY);

                      f140104Repo.Update(obj.F140104);

                      f060403.SKUSYSQTY -= Convert.ToInt32(obj.F140104.DEVICE_STOCK_QTY);
                      f060403.SKUQTY = 0;
                      obj.AllQty = 0;
                    }
                  });
                  #endregion

                  #region (3) 再依找WMS庫存數有小到大開始分配(排序QTY + UNMOVE_STOCK_QTY ASC, QTY DESC)實際盤點數與自動倉系統庫存數，自動倉系統庫存數分配也不能超過WMS庫存數
                  var differentQtyF140104s = currItemF140104s.Where(x => x.AllQty > 0 && x.AllQty != f060403.SKUQTY)
                  .OrderBy(x => x.AllQty)
                  .OrderByDescending(x => x.F140104.QTY)
                  .OrderByDescending(x => x.F140104.ENTER_DATE)
                  .ToList();
                  differentQtyF140104s.ForEach(obj =>
                  {
                    if (obj.AllQty > 0)
                    {
                      var currStock = GetStockData(stockDatas, obj.F140104.ITEM_CODE, obj.F140104.MAKE_NO, obj.F140104.VALID_DATE, obj.F140104.LOC_CODE, obj.F140104.ENTER_DATE, obj.F140104.BOX_CTRL_NO, obj.F140104.PALLET_CTRL_NO);

                      obj.F140104.FIRST_QTY = f060403.SKUQTY > obj.AllQty ? obj.AllQty : f060403.SKUQTY;
                      obj.F140104.DEVICE_STOCK_QTY = f060403.SKUSYSQTY > obj.AllQty ? obj.AllQty : f060403.SKUSYSQTY;
                      obj.F140104.FST_INVENTORY_STAFF = f060402.OPERATOR;
                      obj.F140104.FST_INVENTORY_NAME = inventoryName;
                      obj.F140104.FST_INVENTORY_DATE = f060403.OPERATORTIME;
                      obj.F140104.FST_INVENTORY_PC = "自動倉PC";
                      obj.F140104.QTY = currStock.Sum(x => x.QTY);
                      obj.F140104.UNMOVE_STOCK_QTY = currStock.Sum(x => x.UNMOVE_STOCK_QTY);

                      f140104Repo.Update(obj.F140104);

                      f060403.SKUQTY -= Convert.ToInt32(obj.F140104.FIRST_QTY);
                      f060403.SKUSYSQTY -= Convert.ToInt32(obj.F140104.DEVICE_STOCK_QTY);
                      obj.AllQty -= Convert.ToInt32(obj.F140104.FIRST_QTY);
                    }
                  });
                  #endregion

                  if (f060403.SKUQTY > 0 || f060403.SKUSYSQTY > 0)
                  {
                    // (4) 若配完還有自動倉實際盤點數或系統庫存數，取得F140104中WMS庫存數 = 0的庫存明細且入庫日從大到小排序，取得第一筆庫存明細，將剩餘的自動倉實際盤點數與自動倉系統庫存數壓在這筆上
                    var firstF140104 = currItemF140104s.Where(x => x.AllQty == 0).OrderByDescending(x => x.F140104.ENTER_DATE).FirstOrDefault();
                    if (firstF140104 != null)
                    {
                      var currStock = GetStockData(stockDatas, firstF140104.F140104.ITEM_CODE, firstF140104.F140104.MAKE_NO, firstF140104.F140104.VALID_DATE, firstF140104.F140104.LOC_CODE, firstF140104.F140104.ENTER_DATE, firstF140104.F140104.BOX_CTRL_NO, firstF140104.F140104.PALLET_CTRL_NO);

                      firstF140104.F140104.DEVICE_STOCK_QTY = Convert.ToInt32(firstF140104.F140104.DEVICE_STOCK_QTY) + f060403.SKUSYSQTY;
                      firstF140104.F140104.FIRST_QTY = Convert.ToInt32(firstF140104.F140104.FIRST_QTY) + f060403.SKUQTY;
                      f060403.SKUSYSQTY = 0;
                      f060403.SKUQTY = 0;
                      f140104Repo.Update(firstF140104.F140104);
                    }
                  }

                  if (f060403.SKUQTY > 0 || f060403.SKUSYSQTY > 0)
                  {
                    // (5) 若配完還有自動倉實際盤點數或系統庫存數，取得F140104中WMS庫存數>0的庫存明細且入庫日從大到小排序，取得第一筆庫存明細，將剩餘的自動倉實際盤點數與自動倉系統庫存數壓在這筆上
                    var firstF140104 = currItemF140104s.Where(x => x.AllQty > 0).OrderByDescending(x => x.F140104.ENTER_DATE).FirstOrDefault();
                    if (firstF140104 != null)
                    {
                      var currStock = GetStockData(stockDatas, firstF140104.F140104.ITEM_CODE, firstF140104.F140104.MAKE_NO, firstF140104.F140104.VALID_DATE, firstF140104.F140104.LOC_CODE, firstF140104.F140104.ENTER_DATE, firstF140104.F140104.BOX_CTRL_NO, firstF140104.F140104.PALLET_CTRL_NO);

                      firstF140104.F140104.DEVICE_STOCK_QTY = Convert.ToInt32(firstF140104.F140104.DEVICE_STOCK_QTY) + f060403.SKUSYSQTY;
                      firstF140104.F140104.FIRST_QTY = Convert.ToInt32(firstF140104.F140104.FIRST_QTY) + f060403.SKUQTY;
                      f060403.SKUSYSQTY = 0;
                      f140104Repo.Update(firstF140104.F140104);
                    }
                  }
                }
                else // 對不到盤點明細則新增
                {
                  #region 即時新增/更新 驗收批號的流水號紀錄檔(F020203)，用以回填F140104批號
                  var currRtSeq = 1;
                  var currF020203 = f020203Repo.GetDataByKey(dcCode, gupCode, custCode, f060403.SKUCODE, today);
                  if (currF020203 == null)
                  {
                    // 若沒有在資料庫則新增 目前已使用流水號
                    f020203Repo.Add(
                      new F020203
                      {
                        DC_CODE = dcCode,
                        GUP_CODE = gupCode,
                        CUST_CODE = custCode,
                        ITEM_CODE = f060403.SKUCODE,
                        RT_DATE = today,
                        RT_SEQ = currRtSeq
                      });
                  }
                  else
                  {
                    currF020203.RT_SEQ++;
                    currRtSeq = currF020203.RT_SEQ;
                    f020203Repo.Update(currF020203);
                  }
                  #endregion

                  #region 新增F140104
                  var currStock = stockDatas.Where(x => x.ITEM_CODE == f060403.SKUCODE && x.MAKE_NO == f060403.OUTBATCHCODE && x.VALID_DATE == f060403.EXPIRYDATE);
                  f140104Repo.Add(new F140104
                  {
                    INVENTORY_NO = f060402.WMS_NO,
                    WAREHOUSE_ID = f060402.WAREHOUSE_ID,
                    LOC_CODE = locCode,
                    ITEM_CODE = f060403.SKUCODE,
                    VALID_DATE = f060403.EXPIRYDATE == null ? validDate : f060403.EXPIRYDATE,
                    ENTER_DATE = today,
                    QTY = currStock.Sum(x => x.QTY),
                    FIRST_QTY = f060403.SKUQTY,
                    FLUSHBACK = flushbackByF1909,
                    DC_CODE = dcCode,
                    GUP_CODE = gupCode,
                    CUST_CODE = custCode,
                    FST_INVENTORY_STAFF = f060402.OPERATOR,
                    FST_INVENTORY_NAME = currF1924 == null ? "支援人員" : currF1924.EMP_NAME,
                    FST_INVENTORY_DATE = f060403.OPERATORTIME,
                    FST_INVENTORY_PC = "自動倉PC",
                    BOX_CTRL_NO = "0",
                    PALLET_CTRL_NO = "0",
                    MAKE_NO = string.IsNullOrWhiteSpace(f060403.OUTBATCHCODE) ? $"{today.ToString("yyMMdd")}{Convert.ToString(currRtSeq).PadLeft(3, '0') }" : f060403.OUTBATCHCODE,
                    DEVICE_STOCK_QTY = f060403.SKUSYSQTY,
                    UNMOVE_STOCK_QTY = currStock.Sum(x => x.UNMOVE_STOCK_QTY)
                  });
                  #endregion
                }
              });

              #region (6) 若還有未分配到的WMS庫存明細，這些庫存明細的系統庫存數與實際盤點數都設為0
              var notCombineF140104s = currF140104s.Where(x => x.F140104.FIRST_QTY == null).ToList();
              notCombineF140104s.ForEach(obj =>
              {
                obj.F140104.FIRST_QTY = 0;
                obj.F140104.DEVICE_STOCK_QTY = 0;
                f140104Repo.Update(obj.F140104);
              });
              #endregion

              #endregion
            }
            else
            {
              #region 複盤

              #region (1) 從F140105中WMS庫存數(QTY+UNMOVE_STOCK_QTY) 取得>0的庫存明細 
              var currF140105s = f140105s.Where(x => x.INVENTORY_NO == f060402.WMS_NO && x.WAREHOUSE_ID == f060402.WAREHOUSE_ID)
              .Select(x => new SecondInventoryModel { F140105 = x, AllQty = Convert.ToInt32(x.QTY) + Convert.ToInt32(x.UNMOVE_STOCK_QTY) })
              .ToList();
              #endregion

              // 取得實際庫存數
              var stockDatas = new List<StockDataByInventory>();
              foreach (var F140105 in currF140105s)
              {
                var resData = f1913Repo.GetStockQtyByInventory(dcCode, gupCode, custCode, f060402.WAREHOUSE_ID,
                  new StockDataByInventoryParam
                  {
                    ITEM_CODE = F140105.F140105.ITEM_CODE,
                    VALID_DATE = F140105.F140105.VALID_DATE,
                    MAKE_NO = F140105.F140105.MAKE_NO,
                    LOC_CODE = F140105.F140105.LOC_CODE,
                    ENTER_DATE = F140105.F140105.ENTER_DATE,
                    BOX_CTRL_NO = F140105.F140105.BOX_CTRL_NO,
                    PALLET_CTRL_NO = F140105.F140105.PALLET_CTRL_NO
                  }).ToList();

                if (resData != null && resData.Any())
                  stockDatas.AddRange(resData);
                else
                  stockDatas.Add(new StockDataByInventory()
                  {
                    ITEM_CODE = F140105.F140105.ITEM_CODE,
                    VALID_DATE = F140105.F140105.VALID_DATE,
                    MAKE_NO = F140105.F140105.MAKE_NO,
                    LOC_CODE = F140105.F140105.LOC_CODE,
                    ENTER_DATE = F140105.F140105.ENTER_DATE,
                    BOX_CTRL_NO = F140105.F140105.BOX_CTRL_NO,
                    PALLET_CTRL_NO = F140105.F140105.PALLET_CTRL_NO,
                    QTY = 0,
                    UNMOVE_STOCK_QTY = 0
                  });
              }

              // 結果回傳資料若對應不到明細需要取第一筆盤點明細的儲位來填入新增的明細
              var locCode = currF140105s.Any() ? currF140105s.First().F140105.LOC_CODE : string.Empty;

              // 結果回傳逐筆去找對應的盤點明細來做分配盤點數及自動倉庫存數
              currF060403s.ForEach(f060403 =>
              {
                // 取得該結果回傳對應的盤點明細
                var currItemF140105s = currF140105s.Where(x => x.F140105.ITEM_CODE == f060403.SKUCODE &&
                x.F140105.VALID_DATE == f060403.EXPIRYDATE &&
                x.F140105.MAKE_NO == f060403.OUTBATCHCODE);

                if (currItemF140105s.Any())
                {
                  #region (2) 先找WMS庫存數=自動倉實際盤點數 (排序QTY DESC)，若有先分配給這筆的實際盤點數，自動倉系統庫存數也分配給這筆但不能超過WMS庫存數
                  var sameQtyF140105s = currItemF140105s.Where(x => x.AllQty > 0 && x.AllQty == f060403.SKUQTY)
                  .OrderByDescending(x => x.F140105.QTY)
                  .OrderByDescending(x => x.F140105.ENTER_DATE)
                  .ToList();
                  sameQtyF140105s.ForEach(obj =>
                  {
                    if (obj.AllQty > 0)
                    {
                      var currStock = GetStockData(stockDatas, obj.F140105.ITEM_CODE, obj.F140105.MAKE_NO, obj.F140105.VALID_DATE, obj.F140105.LOC_CODE, obj.F140105.ENTER_DATE, obj.F140105.BOX_CTRL_NO, obj.F140105.PALLET_CTRL_NO);

                      obj.F140105.DEVICE_STOCK_QTY = f060403.SKUSYSQTY > f060403.SKUQTY ? f060403.SKUQTY : f060403.SKUSYSQTY;
                      obj.F140105.SECOND_QTY = f060403.SKUQTY;
                      obj.F140105.SEC_INVENTORY_STAFF = f060402.OPERATOR;
                      obj.F140105.SEC_INVENTORY_NAME = inventoryName;
                      obj.F140105.SEC_INVENTORY_DATE = f060403.OPERATORTIME;
                      obj.F140105.SEC_INVENTORY_PC = "自動倉PC";
                      obj.F140105.QTY = currStock.Sum(x => x.QTY);
                      obj.F140105.UNMOVE_STOCK_QTY = currStock.Sum(x => x.UNMOVE_STOCK_QTY);

                      f140105Repo.Update(obj.F140105);

                      f060403.SKUSYSQTY -= Convert.ToInt32(obj.F140105.DEVICE_STOCK_QTY);
                      f060403.SKUQTY = 0;
                      obj.AllQty = 0;
                    }
                  });
                  #endregion

                  #region (3) 再依找WMS庫存數有小到大開始分配(排序QTY + UNMOVE_STOCK_QTY ASC, QTY DESC)實際盤點數與自動倉系統庫存數，自動倉系統庫存數分配也不能超過WMS庫存數
                  var differentQtyF140105s = currItemF140105s.Where(x => x.AllQty > 0 && x.AllQty != f060403.SKUQTY)
                  .OrderBy(x => x.AllQty)
                  .OrderByDescending(x => x.F140105.QTY)
.OrderByDescending(x => x.F140105.ENTER_DATE)
.ToList();
                  differentQtyF140105s.ForEach(obj =>
                  {
                    if (obj.AllQty > 0)
                    {
                      var currStock = GetStockData(stockDatas, obj.F140105.ITEM_CODE, obj.F140105.MAKE_NO, obj.F140105.VALID_DATE, obj.F140105.LOC_CODE, obj.F140105.ENTER_DATE, obj.F140105.BOX_CTRL_NO, obj.F140105.PALLET_CTRL_NO);

                      obj.F140105.SECOND_QTY = f060403.SKUQTY > obj.AllQty ? obj.AllQty : f060403.SKUQTY;
                      obj.F140105.DEVICE_STOCK_QTY = f060403.SKUSYSQTY > obj.AllQty ? obj.AllQty : f060403.SKUSYSQTY;
                      obj.F140105.SEC_INVENTORY_STAFF = f060402.OPERATOR;
                      obj.F140105.SEC_INVENTORY_NAME = inventoryName;
                      obj.F140105.SEC_INVENTORY_DATE = f060403.OPERATORTIME;
                      obj.F140105.SEC_INVENTORY_PC = "自動倉PC";
                      obj.F140105.QTY = currStock.Sum(x => x.QTY);
                      obj.F140105.UNMOVE_STOCK_QTY = currStock.Sum(x => x.UNMOVE_STOCK_QTY);

                      f140105Repo.Update(obj.F140105);

                      f060403.SKUQTY -= Convert.ToInt32(obj.F140105.SECOND_QTY);
                      f060403.SKUSYSQTY -= Convert.ToInt32(obj.F140105.DEVICE_STOCK_QTY);
                      obj.AllQty -= Convert.ToInt32(obj.F140105.SECOND_QTY);
                    }
                  });
                  #endregion

                  if (f060403.SKUQTY > 0 || f060403.SKUSYSQTY > 0)
                  {
                    // (4) 若配完還有自動倉實際盤點數或系統庫存數，取得F140105中WMS庫存數 = 0的庫存明細且入庫日從大到小排序，取得第一筆庫存明細，將剩餘的自動倉實際盤點數與自動倉系統庫存數壓在這筆上
                    var firstF140105 = currItemF140105s.Where(x => x.AllQty == 0).OrderByDescending(x => x.F140105.ENTER_DATE).FirstOrDefault();
                    if (firstF140105 != null)
                    {
                      var currStock = GetStockData(stockDatas, firstF140105.F140105.ITEM_CODE, firstF140105.F140105.MAKE_NO, firstF140105.F140105.VALID_DATE, firstF140105.F140105.LOC_CODE, firstF140105.F140105.ENTER_DATE, firstF140105.F140105.BOX_CTRL_NO, firstF140105.F140105.PALLET_CTRL_NO);

                      firstF140105.F140105.DEVICE_STOCK_QTY = Convert.ToInt32(firstF140105.F140105.DEVICE_STOCK_QTY) + f060403.SKUSYSQTY;
                      firstF140105.F140105.SECOND_QTY = Convert.ToInt32(firstF140105.F140105.SECOND_QTY) + f060403.SKUQTY;
                      f060403.SKUSYSQTY = 0;
                      f060403.SKUQTY = 0;
                      f140105Repo.Update(firstF140105.F140105);
                    }
                  }

                  if (f060403.SKUQTY > 0 || f060403.SKUSYSQTY > 0)
                  {
                    // (5) 若配完還有自動倉實際盤點數或系統庫存數，取得F140104中WMS庫存數>0的庫存明細且入庫日從大到小排序，取得第一筆庫存明細，將剩餘的自動倉實際盤點數與自動倉系統庫存數壓在這筆上
                    var firstF140105 = currItemF140105s.Where(x => x.AllQty > 0).OrderByDescending(x => x.F140105.ENTER_DATE).FirstOrDefault();
                    if (firstF140105 != null)
                    {
                      var currStock = GetStockData(stockDatas, firstF140105.F140105.ITEM_CODE, firstF140105.F140105.MAKE_NO, firstF140105.F140105.VALID_DATE, firstF140105.F140105.LOC_CODE, firstF140105.F140105.ENTER_DATE, firstF140105.F140105.BOX_CTRL_NO, firstF140105.F140105.PALLET_CTRL_NO);

                      firstF140105.F140105.DEVICE_STOCK_QTY = Convert.ToInt32(firstF140105.F140105.DEVICE_STOCK_QTY) + f060403.SKUSYSQTY;
                      firstF140105.F140105.SECOND_QTY = Convert.ToInt32(firstF140105.F140105.SECOND_QTY) + f060403.SKUQTY;
                      f060403.SKUSYSQTY = 0;
                      f060403.SKUQTY = 0;
                      f140105Repo.Update(firstF140105.F140105);
                    }
                  }

                }
                else // 對不到盤點明細
                {
                  #region 即時新增/更新 驗收批號的流水號紀錄檔(F020203)，用以回填F140105批號
                  var currRtSeq = 1;
                  var currF020203 = f020203Repo.GetDataByKey(dcCode, gupCode, custCode, f060403.SKUCODE, today);
                  if (currF020203 == null)
                  {
                    // 若沒有在資料庫則新增 目前已使用流水號
                    f020203Repo.Add(
                        new F020203
                        {
                          DC_CODE = dcCode,
                          GUP_CODE = gupCode,
                          CUST_CODE = custCode,
                          ITEM_CODE = f060403.SKUCODE,
                          RT_DATE = today,
                          RT_SEQ = currRtSeq
                        });
                  }
                  else
                  {
                    currF020203.RT_SEQ++;
                    currRtSeq = currF020203.RT_SEQ;
                    f020203Repo.Update(currF020203);
                  }
                  #endregion

                  #region 新增F140105
                  var currStock = stockDatas.Where(x => x.ITEM_CODE == f060403.SKUCODE && x.MAKE_NO == f060403.OUTBATCHCODE && x.VALID_DATE == f060403.EXPIRYDATE);
                  f140105Repo.Add(new F140105
                  {
                    INVENTORY_NO = f060402.WMS_NO,
                    WAREHOUSE_ID = f060402.WAREHOUSE_ID,
                    LOC_CODE = locCode,
                    ITEM_CODE = f060403.SKUCODE,
                    VALID_DATE = f060403.EXPIRYDATE == null ? validDate : f060403.EXPIRYDATE,
                    ENTER_DATE = today,
                    QTY = currStock.Sum(x => x.QTY),
                    SECOND_QTY = f060403.SKUQTY,
                    FLUSHBACK = flushbackByF1909,
                    DC_CODE = dcCode,
                    GUP_CODE = gupCode,
                    CUST_CODE = custCode,
                    SEC_INVENTORY_STAFF = f060402.OPERATOR,
                    SEC_INVENTORY_NAME = currF1924 == null ? "支援人員" : currF1924.EMP_NAME,
                    SEC_INVENTORY_DATE = f060403.OPERATORTIME,
                    SEC_INVENTORY_PC = "自動倉PC",
                    BOX_CTRL_NO = "0",
                    PALLET_CTRL_NO = "0",
                    MAKE_NO = string.IsNullOrWhiteSpace(f060403.OUTBATCHCODE) ? $"{today.ToString("yyMMdd")}{Convert.ToString(currRtSeq).PadLeft(3, '0') }" : f060403.OUTBATCHCODE,
                    DEVICE_STOCK_QTY = f060403.SKUSYSQTY,
                    UNMOVE_STOCK_QTY = currStock.Sum(x => x.UNMOVE_STOCK_QTY)
                  });
                  #endregion
                }
              });

              #region (6) 若還有未分配到的WMS庫存明細，這些庫存明細的系統庫存數與實際盤點數都設為0
              var notCombineF140105s = currF140105s.Where(x => x.F140105.SECOND_QTY == null).ToList();
              notCombineF140105s.ForEach(obj =>
              {
                obj.F140105.SECOND_QTY = 0;
                obj.F140105.DEVICE_STOCK_QTY = 0;
                f140105Repo.Update(obj.F140105);
              });
              #endregion

              #endregion
            }

            wmsTransaction.Complete();
            f060402.STATUS = "2";
            successCnt++;
          }
          #endregion

          #region 更新 F060402 完成、錯誤、逾時狀態
          f060402Repo = new F060402Repository(Schemas.CoreSchema);
          f060402Repo.Update(f060402);
          #endregion
        });
      }

      int failCnt = executeDatas.Count - successCnt;
      res.MsgCode = "10005";
      res.MsgContent = string.Format(_tacService.GetMsg("10005"), "盤點完成結果回傳", successCnt, failCnt, executeDatas.Count);
      res.TotalCnt = executeDatas.Count;
      res.SuccessCnt = successCnt;
      res.FailureCnt = failCnt;
      #endregion

      return res;
    }

    private List<StockDataByInventory> GetStockData(List<StockDataByInventory> data, string itemCode, string makeNo, DateTime validDate, string locCode, DateTime enterDate, string boxCtrlNo, string palletCtrlNo)
    {
      return data.Where(x => x.ITEM_CODE == itemCode &&
                             x.MAKE_NO == makeNo &&
                             x.VALID_DATE == validDate &&
                             x.LOC_CODE == locCode &&
                             x.ENTER_DATE == enterDate &&
                             x.BOX_CTRL_NO == boxCtrlNo &&
                             x.PALLET_CTRL_NO == palletCtrlNo).ToList();
    }
  }
}
