using System;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F51;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Schedule.S00.Services
{
  public partial class S000201Service
  {
    private WmsTransaction _wmsTransaction;
    public S000201Service(WmsTransaction wmsTransaction = null)
    {
      _wmsTransaction = wmsTransaction;
    }

    public void DailyStockBackup(DateTime calDate)
    {
      var repoF190101 = new F190101Repository(Schemas.CoreSchema);
      var repoF510102 = new F510102Repository(Schemas.CoreSchema, _wmsTransaction);
      var repoF510104 = new F510104Repository(Schemas.CoreSchema, _wmsTransaction);
      //已備份過庫存資料不可再執行
      if (!repoF510102.Filter(n => n.CAL_DATE == calDate).Any())
      {
        repoF510102.InsertStockByDate(calDate);
      }
      if (!repoF510104.Filter(n => n.CAL_DATE == calDate).Any())
      {
        var f190101Datas = repoF190101.Filter(n => true).ToList();
        foreach (var f190101 in f190101Datas)
        {
          repoF510104.InsertVirtualByDate(f190101.DC_CODE, f190101.GUP_CODE, f190101.CUST_CODE, calDate);
        }
      }
    }

    public void InsertStockSettle(DateTime calDate)
    {
      var repoF190101 = new F190101Repository(Schemas.CoreSchema);
      var repoF1903 = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
      var repoF5101 = new F5101Repository(Schemas.CoreSchema, _wmsTransaction);
      var repoF510102 = new F510102Repository(Schemas.CoreSchema, _wmsTransaction);

      repoF5101.DeleteByDate(calDate);

      var f190101Datas = repoF190101.Filter(n => true).ToList();
      foreach (var f190101 in f190101Datas)
      {
        //期初庫存
        var lastLocDatas =
          repoF5101.GetLastLocQty(f190101.DC_CODE, f190101.GUP_CODE, f190101.CUST_CODE, calDate.AddDays(-1)).ToList();
        //庫存 
        var locDatas = repoF510102.GetLocSettleQty(f190101.DC_CODE, f190101.GUP_CODE, f190101.CUST_CODE, calDate).ToList();
        //驗收
        var recvDatas = repoF1903.GetRecvSettleQty(f190101.DC_CODE, f190101.GUP_CODE, f190101.CUST_CODE, calDate).ToList();
        //出貨
        var deliveryDatas =
          repoF1903.GetDeliverySettleQty(f190101.DC_CODE, f190101.GUP_CODE, f190101.CUST_CODE, calDate).ToList();
        //退貨上架
        var rtnDatas = repoF1903.GetReturnSettleQty(f190101.DC_CODE, f190101.GUP_CODE, f190101.CUST_CODE, calDate).ToList();
        //跨DC調出
        var moveOutDatas =
          repoF1903.GetMoveOutSettleQty(f190101.DC_CODE, f190101.GUP_CODE, f190101.CUST_CODE, calDate).ToList();
        //跨DC調入
        var moveInDatas =
          repoF1903.GetMoveInSettleQty(f190101.DC_CODE, f190101.GUP_CODE, f190101.CUST_CODE, calDate).ToList();

        var totalData = (from loc in locDatas
                         join last in lastLocDatas
                           on new { loc.DC_CODE, loc.GUP_CODE, loc.CUST_CODE, loc.ITEM_CODE } equals
                           new { last.DC_CODE, last.GUP_CODE, last.CUST_CODE, last.ITEM_CODE } into last1
                         from last in last1.DefaultIfEmpty()
                         join recv in recvDatas
                           on new { loc.DC_CODE, loc.GUP_CODE, loc.CUST_CODE, loc.ITEM_CODE } equals
                           new { recv.DC_CODE, recv.GUP_CODE, recv.CUST_CODE, recv.ITEM_CODE } into recv1
                         from recv in recv1.DefaultIfEmpty()
                         join delv in deliveryDatas
                           on new { loc.DC_CODE, loc.GUP_CODE, loc.CUST_CODE, loc.ITEM_CODE } equals
                           new { delv.DC_CODE, delv.GUP_CODE, delv.CUST_CODE, delv.ITEM_CODE } into delv1
                         from delv in delv1.DefaultIfEmpty()
                         join rtn in rtnDatas
                           on new { loc.DC_CODE, loc.GUP_CODE, loc.CUST_CODE, loc.ITEM_CODE } equals
                           new { rtn.DC_CODE, rtn.GUP_CODE, rtn.CUST_CODE, rtn.ITEM_CODE } into rtn1
                         from rtn in rtn1.DefaultIfEmpty()
                         join moveOut in moveOutDatas
                           on new { loc.DC_CODE, loc.GUP_CODE, loc.CUST_CODE, loc.ITEM_CODE } equals
                           new { moveOut.DC_CODE, moveOut.GUP_CODE, moveOut.CUST_CODE, moveOut.ITEM_CODE } into moveOut1
                         from moveOut in moveOut1.DefaultIfEmpty()
                         join moveIn in moveInDatas
                           on new { loc.DC_CODE, loc.GUP_CODE, loc.CUST_CODE, loc.ITEM_CODE } equals
                           new { moveIn.DC_CODE, moveIn.GUP_CODE, moveIn.CUST_CODE, moveIn.ITEM_CODE } into moveIn1
                         from moveIn in moveIn1.DefaultIfEmpty()
                         select new StockSettleData()
                         {
                           CAL_DATE = calDate,
                           DC_CODE = f190101.DC_CODE,
                           GUP_CODE = f190101.GUP_CODE,
                           CUST_CODE = f190101.CUST_CODE,
                           ITEM_CODE = loc.ITEM_CODE,
                           BEGIN_QTY = last.BEGIN_QTY ?? 0,
                           END_QTY = loc.END_QTY ?? 0,
                           RECV_QTY = recv.RECV_QTY ?? 0,
                           RTN_QTY = rtn.RTN_QTY ?? 0,
                           DELV_QTY = delv.DELV_QTY ?? 0,
                           SRC_QTY = moveOut.SRC_QTY ?? 0,
                           TAR_QTY = moveIn.TAR_QTY ?? 0,
                           LEND_QTY = 0
                         }).Select(AutoMapper.Mapper.DynamicMap<F5101>).ToList();

        repoF5101.BulkInsert(totalData);
      }
    }
  }
}