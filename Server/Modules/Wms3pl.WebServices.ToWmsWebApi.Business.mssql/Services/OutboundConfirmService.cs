using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.ToWmsWebApi.Business.mssql.Services
{
  public class OutboundConfirmService : BaseService
  {
    /// <summary>
    /// 出庫完成
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult OutboundConfirm(WcsImportReq req)
    {
      ApiResult res = new ApiResult { IsSuccessed = true };
      List<ApiResponse> data = new List<ApiResponse>();

      // 新增API Log
      res = ApiLogHelper.CreateApiLogInfo(req.DcCode, req.GupCode, req.CustCode, "ImportOutboundConfirmResults", req, () =>
      {
        // 取得物流中心服務貨主檔
        CommonService commonService = new CommonService();
        var dcCustList = commonService.GetDcCustList(req.DcCode, req.GupCode, req.CustCode);
        dcCustList.ForEach(item =>
        {
          var result = ImportOutboundConfirmResults(item.DC_CODE, item.GUP_CODE, item.CUST_CODE);
          data.Add(new ApiResponse { MsgCode = result.MsgCode, MsgContent = result.MsgContent, No = $"DC_CODE：{item.DC_CODE} GUP_CODE：{item.GUP_CODE} CUST_CODE：{ item.CUST_CODE}" });
        });
        res.Data = JsonConvert.SerializeObject(data);
        return res;
      }, true);

      return res;
    }

    /// <summary>
    /// 出庫完成
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    private ApiResult ImportOutboundConfirmResults(string dcCode, string gupCode, string custCode)
    {
      #region 變數設定
      ApiResult res = new ApiResult { IsSuccessed = true };
      var f060202Repo = new F060202Repository(Schemas.CoreSchema);
      var f060203Repo = new F060203Repository(Schemas.CoreSchema);
      var f060205Repo = new F060205Repository(Schemas.CoreSchema);
      var f060206Repo = new F060206Repository(Schemas.CoreSchema);
      var f1980Repo = new F1980Repository(Schemas.CoreSchema);
      var f151001Repo = new F151001Repository(Schemas.CoreSchema);
      int successCnt = 0;
      var executedDocIdList = new List<string>();
      #endregion

      #region 主要邏輯
      // 取得要執行出庫完成結果回傳資料
      var f060202s = f060202Repo.GetDatasForExecute(dcCode, gupCode, custCode).ToList();

      // 取得原任務單號
      var docIds = f060202s.Select(x => x.DOC_ID).ToList();

      // 取得出庫任務完成結果回傳明細資料
      var f060203s = f060203Repo.GetDatasForExecute(docIds).ToList();

      // 取得出貨容器紀錄
      var f060205s = f060205Repo.GetDatasForDocIds(docIds).ToList();

      // 取得出貨明細容器紀錄資料檔資料
      var f060206s = f060206Repo.GetDatasForDocIds(docIds).ToList();

      var executeDatas = f060202s.ToList();

      if (executeDatas.Any())
      {
        // Foreach #取得要執行出庫完成結果回傳資料
        foreach (var f060202 in executeDatas)
        {
          var wmsTransaction = new WmsTransaction();
          var sharedService = new SharedService(wmsTransaction);
          var stockService = new StockService(wmsTransaction);
          var containerService = new ContainerService(wmsTransaction);
          sharedService.StockService = stockService;

          var currF060203 = f060203s.Where(x => x.DOC_ID == f060202.DOC_ID);
          var currF060205 = f060205s.Where(x => x.DOC_ID == f060202.DOC_ID);
          var currF060206 = f060206s.Where(x => x.DOC_ID == f060202.DOC_ID);
          if (!currF060203.Any())
            continue;

          #region 更新 F060202 處理中狀態
          // 更新執行出庫完成結果回傳資料表處理中狀態
          f060202.STATUS = "1";
          f060202.PROC_DATE = DateTime.Now;
          f060202Repo.Update(f060202);
          #endregion

          #region 資料處理
          // 因為客戶端那邊F060202.DOC_ID有重複，所以先加已經執行過的再碰到重複的就F
          if (executedDocIdList.Contains(f060202.DOC_ID))
            f060202.STATUS = "F";
          else
          {
            // 取得倉庫類型
            var f1980 = f1980Repo.GetDatas(dcCode, new List<string> { f060202.WAREHOUSE_ID }).FirstOrDefault();
            var f151001 = f151001Repo.GetSingleData(dcCode, custCode, gupCode, f060202.WMS_NO);
            // 若回傳單據狀態 = 9 OR (f151001.ALLOCATION_NO = 5且倉庫類型=3(UnitLoad Shuttle)) 則處理下一張來源單
            if (f060202.M_STATUS == "9" || (f151001?.ALLOCATION_TYPE == "5" && f1980.DEVICE_TYPE == "3"))
            {
              f060202.STATUS = "2";
              successCnt++;
            }
            else
            {
              var result = new ApiResult { IsSuccessed = true };

              ApiLogHelper.CreateApiLogInfo(dcCode, gupCode, custCode, "ImportOutboundConfirmResults_Record", new { F060202 = f060202, F060203 = currF060203 }, () =>
              {
                List<ContainerExecuteResult> createResult = null;

                if (currF060205.Any() && currF060206.Any())
                {
                  createResult = containerService.CreateContainer(currF060206.GroupBy(x => new { x.CONTAINERCODE, x.SKUCODE, x.BIN_CODE }).Select(x => new CreateContainerParam
                  {
                    DC_CODE = dcCode,
                    GUP_CODE = gupCode,
                    CUST_CODE = custCode,
                    CONTAINER_CODE = x.Key.CONTAINERCODE,
                    CONTAINER_TYPE = "0",
                    WMS_NO = f060202.WMS_NO,
                    WMS_TYPE = f060202.WMS_NO.Substring(0, 1),
                    ITEM_CODE = x.Key.SKUCODE,
                    WAREHOUSE_ID = f060202.WAREHOUSE_ID,
                    QTY = x.Sum(x1 => x1.SKUQTY),
                    BIN_CODE = x.Key.BIN_CODE,
                    PICK_ORD_NO = f060202.PICK_NO,
                    SERIAL_NO_LIST = x.Any(x1 => string.IsNullOrWhiteSpace(x1.SERIAL_NO)) ? null : x.Select(x1 => x1.SERIAL_NO).ToList()
                  }).ToList());
                }

                if (f060202.WMS_NO.StartsWith("T")) // 調撥單
                {
                  var containerCode = currF060205.FirstOrDefault()?.CONTAINERCODE;
                  // 調撥單處理
                  sharedService.AllocationConfirm(new AllocationConfirmParam
                  {
                    DcCode = dcCode,
                    GupCode = gupCode,
                    CustCode = custCode,
                    AllocNo = f060202.WMS_NO,
                    StartTime = f060202.STARTTIME,
                    CompleteTime = f060202.COMPLETETIME,
                    Operator = f060202.OPERATOR,
                    ContainerCode = containerCode,
                    Details = currF060203.GroupBy(x => x.ROWNUM).Select(x => new AllocationConfirmDetail
                    {
                      Seq = Convert.ToInt16(x.Key),
                      Qty = x.Sum(x1 => x1.SKUQTY),
                      ContainerCode = containerCode
                    }).Distinct().ToList(),
                    ContainerResults = createResult
                  });

                  //上面的流程是下架處理，下面這功能應該是沒有作用
                  stockService.SaveChange();
                }
                else if (f060202.WMS_NO.StartsWith("O") || f060202.WMS_NO.StartsWith("P")) // 出貨單、揀貨單
                {
                  // 揀貨單處理
                  sharedService.PickConfirm(new PickConfirmParam
                  {
                    DcCode = dcCode,
                    GupCode = gupCode,
                    CustCode = custCode,
                    PickNo = f060202.PICK_NO,
                    StartTime = f060202.STARTTIME,
                    CompleteTime = f060202.COMPLETETIME,
                    ContainerData = currF060206.ToList(),
                    WmsNo = f060202.WMS_NO,
                    EmpId = f060202.OPERATOR,
                    DocID = f060202.DOC_ID,
                    IsAutoWHException = f060202.ISEXCEPTION == 2,
                    Details = currF060203.GroupBy(x => x.ROWNUM).Select(x => new PickConfirmDetail
                    {
                      Seq = x.Key.ToString().PadLeft(4, '0'),
                      Qty = x.Sum(x1 => x1.SKUQTY)
                    }).Distinct().ToList(),
                    ContainerResults = createResult
                  });
                }

                wmsTransaction.Complete();
                f060202.STATUS = "2";
                executedDocIdList.Add(f060202.DOC_ID);
                successCnt++;
                return result;
              }, false,
              (fResult) =>
              {
                if (!fResult.IsSuccessed)
                  f060202.STATUS = "0";
                return new ApiResult();
              });
            }
          }
          #endregion

          #region 更新 F060202 完成、錯誤、逾時狀態
          f060202Repo = new F060202Repository(Schemas.CoreSchema);
          f060202Repo.Update(f060202);
          #endregion
        }
      }

      int failCnt = executeDatas.Count - successCnt;
      res.MsgCode = "10005";
      res.MsgContent = string.Format(_tacService.GetMsg("10005"), "出庫完成結果回傳", successCnt, failCnt, executeDatas.Count);
      res.TotalCnt = executeDatas.Count;
      res.SuccessCnt = successCnt;
      res.FailureCnt = failCnt;
      #endregion

      return res;
    }
  }
}
