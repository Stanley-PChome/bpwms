using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ApiService;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Schedule.WmsSchedule
{
  class ImmediateItemScheduleService
  {
    private WmsTransaction _wmsTransaction;

    #region Services

    #region CommonService
    private CommonService _CommonService;
    public CommonService CommonService
    {
      get { return _CommonService == null ? _CommonService = new CommonService() : _CommonService; }
      set { _CommonService = value; }
    }
    #endregion CommonService

    #region StockService
    private StockService _StockService;
    public StockService StockService
    {
      get { return _StockService == null ? _StockService = new StockService(_wmsTransaction) : _StockService; }
      set { _StockService = value; }
    }
    #endregion

    #region SharedService
    private SharedService _SharedService;
    public SharedService SharedService
    {
      get { return _SharedService == null ? _SharedService = new SharedService(_wmsTransaction) : _SharedService; }
      set { _SharedService = value; }
    }
    #endregion SharedService

    #endregion

    #region Repositorys

    #region F0003Repository
    private F0003Repository _F0003Repo;
    public F0003Repository F0003Repo
    {
      get { return _F0003Repo == null ? _F0003Repo = new F0003Repository(Schemas.CoreSchema, _wmsTransaction) : _F0003Repo; }
      set { _F0003Repo = value; }
    }
    #endregion F0003Repository

    #region F190101Repository
    private F190101Repository _F190101Repo;
    public F190101Repository F190101Repo
    {
      get { return _F190101Repo == null ? _F190101Repo = new F190101Repository(Schemas.CoreSchema, _wmsTransaction) : _F190101Repo; }
      set { _F190101Repo = value; }
    }
    #endregion F190101Repository

    #region F1913Repository
    private F1913Repository _F1913Repo;
    public F1913Repository F1913Repo
    {
      get { return _F1913Repo == null ? _F1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction) : _F1913Repo; }
      set { _F1913Repo = value; }
    }
    #endregion F1913Repository


    #endregion

    public ImmediateItemScheduleService()
    {
      _wmsTransaction = new WmsTransaction();
    }

    public ApiResult ExecImmediateItemSchedule()
    {
      var resList = new List<ApiResult>();
      //共處理幾筆庫存資料
      var procStockTotalCnt = 0;
      //共產生的調撥單單號
      var crtAllocTotalOrds = new List<string>();

      var MsgContent1 = "共{0}筆庫存符合即期品條件，產生{1}張調撥單，調撥單號:{2}";

      var f190101s = F190101Repo.GetImmediateItemSchedule().ToList();
      if (f190101s == null || !f190101s.Any())
        return new ApiResult { IsSuccessed = false, MsgContent = "無貨主需要執行即期品調撥" };

      var immediateItemWhIds = F0003Repo.GetImmediateItemScheduleDatas(f190101s.Select(x => x.CUST_CODE).ToList()).ToList();

      foreach (var curf190101 in f190101s)
      {
        var res = ApiLogHelper.CreateApiLogInfo(curf190101.DC_CODE, curf190101.GUP_CODE, curf190101.CUST_CODE, "ImmediateItemSchedule",
          new { curf190101.DC_CODE, curf190101.GUP_CODE, curf190101.CUST_CODE }, () =>
          {
            //處理幾筆庫存資料
            var procStockCnt = 0;
            //產生的調撥單單號
            var crtAllocOrds = new List<string>();

            var detResults = new List<ApiResult>();

            //[C] = 取得貨主設定即期品商品預設上架的不良品倉庫編號
            var immediateItemWhId = immediateItemWhIds.FirstOrDefault(x =>
                x.DC_CODE == curf190101.DC_CODE &&
                x.GUP_CODE == curf190101.GUP_CODE &&
                x.CUST_CODE == curf190101.CUST_CODE &&
                x.AP_NAME == "ImmediateItemWhId").SYS_PATH;

            //未設定不良品倉庫編號，進行下個貨主處理
            if (string.IsNullOrWhiteSpace(immediateItemWhId))
              return new ApiResult
              {
                IsSuccessed = false,
                MsgContent = $"未設定即期品商品預設上架的不良品倉庫編號",
              };

            //取得貨主盤點遺失倉與疑似遺失倉倉別代號
            //不要包含的倉別
            var withoutWhId = immediateItemWhIds.Where(x =>
                  x.DC_CODE == curf190101.DC_CODE &&
                  x.GUP_CODE == curf190101.GUP_CODE &&
                  x.CUST_CODE == curf190101.CUST_CODE)
                 .Select(x => x.SYS_PATH)
                 .ToList();

            var procImmediateItemsByWh = F1913Repo.GetProcImmediateItem(curf190101.DC_CODE, curf190101.GUP_CODE, curf190101.CUST_CODE, withoutWhId).ToList()
                .GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.WAREHOUSE_ID });

            foreach (var item in procImmediateItemsByWh)
            {
              _wmsTransaction = new WmsTransaction();
              SharedService = new SharedService(_wmsTransaction);

              var allotBatchNo = "BT" + DateTime.Now.ToString("yyyyMMddHHmmss");
              var returnStocks = new List<F1913>();
              var IsLockStock = false;

              procStockCnt += item.Count();
              procStockTotalCnt += item.Count();

              try
              {
                //商品鎖定
                IsLockStock = StockService.CheckAllotStockStatus(
                  true,
                  allotBatchNo,
                  item.Select(x => new ItemKey { DcCode = x.DC_CODE, GupCode = x.GUP_CODE, CustCode = x.CUST_CODE, ItemCode = x.ITEM_CODE }).ToList());

                if (!IsLockStock)
                {
                  detResults.Add(new ApiResult { IsSuccessed = false, MsgContent = $"仍有程序正在配庫中 品號:{string.Join(",", item.Select(x => x.ITEM_CODE).Distinct())}" });
                  continue;
                }

                var allocRes = SharedService.CreateOrUpdateAllocation(new NewAllocationItemParam
                {
                  GupCode = item.Key.GUP_CODE,
                  CustCode = item.Key.CUST_CODE,
                  AllocationDate = DateTime.Today,
                  IsExpendDate = true,
                  SrcDcCode = item.Key.DC_CODE,
                  SrcWarehouseId = item.Key.WAREHOUSE_ID,
                  TarDcCode = item.Key.DC_CODE,
                  TarWarehouseId = immediateItemWhId,
                  AllocationType = Shared.Enums.AllocationType.Both,
                  ReturnStocks = returnStocks,
                  isIncludeResupply = true,
                  AllocationTypeCode = "8",
                  Memo = "即期品調撥",
                  SrcStockFilterDetails =
                    item.Select(x => new StockFilter { LocCode = x.LOC_CODE, ItemCode = x.ITEM_CODE, ValidDates = new List<DateTime> { x.VALID_DATE }, Qty = x.QTY }).ToList()
                });

                if (!allocRes.Result.IsSuccessed)
                {
                  detResults.Add(new ApiResult { IsSuccessed = false, MsgContent = allocRes.Result.Message });
                  continue;
                }

                crtAllocOrds.AddRange(allocRes.AllocationList.Select(x => x.Master.ALLOCATION_NO));
                crtAllocTotalOrds.AddRange(allocRes.AllocationList.Select(x => x.Master.ALLOCATION_NO));

                var bulkInsertResult = SharedService.BulkInsertAllocation(allocRes.AllocationList, allocRes.StockList);
                if (!bulkInsertResult.IsSuccessed)
                {
                  detResults.Add(new ApiResult { IsSuccessed = false, MsgContent = bulkInsertResult.Message });
                  continue;
                }

                  _wmsTransaction.Complete();
              }
              catch (Exception ex)  //給APiLogHelper去處理例外
              { throw new Exception(null, ex); }
              finally
              {
                //配庫鎖定解鎖
                //上方有鎖定成功才解鎖
                if (IsLockStock)
                  StockService.UpdateAllotStockStatusToNotAllot(allotBatchNo);
              }

            }

            return new ApiResult
            {
              IsSuccessed = true,
              MsgContent = string.Format(MsgContent1, procStockCnt, crtAllocOrds.Count, string.Join(",", crtAllocOrds)) +
              (detResults.Any() ? $" ，失敗訊息：{ string.Join(Environment.NewLine, detResults.Select(x => x.MsgContent)) }" : "")
            };
          },
        true);

        //給外面排程寫入文字log時可以清楚看出是哪個dc,gup,cust
        res.MsgContent = $"物流中心:{curf190101.DC_CODE} 業主:{curf190101.GUP_CODE} 貨主:{curf190101.CUST_CODE} {res.MsgContent}";
        resList.Add(res);
      }

      return new ApiResult
      {
        IsSuccessed = true,
        MsgContent = string.Join(Environment.NewLine, resList.Select(x => x.MsgContent))
                     + Environment.NewLine
                     + string.Format(MsgContent1, procStockTotalCnt, crtAllocTotalOrds.Count, string.Join(",", crtAllocTotalOrds))
      };
    }

  }
}
