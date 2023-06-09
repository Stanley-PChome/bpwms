using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
  public partial class F0202Repository : RepositoryBase<F0202, Wms3plDbContext, F0202Repository>
  {
    public F0202Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
    {
    }

    public IQueryable<F0202Data> GetF0202Datas(string dcCode, string gupCode, string custCode, DateTime? begCrtDate, DateTime? endCrtDate,
            string orderNo, DateTime begCheckinDate, DateTime endCheckinDate, string custOrdNo, string empId, string empName, string itemCode,
            string selectedFastType)
    {
      var f0202 = _db.F0202s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                      x.GUP_CODE == gupCode &&
                                                                      x.CUST_CODE == custCode &&
                                                                      x.CHECKIN_DATE >= begCheckinDate.Date &&
                                                                      x.CHECKIN_DATE < endCheckinDate.Date);
      var f010201 = _db.F010201s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                              x.GUP_CODE == gupCode &&
                                                                              x.CUST_CODE == custCode);
      var f010202 = _db.F010202s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                              x.GUP_CODE == gupCode &&
                                                                              x.CUST_CODE == custCode);
      var f1903 = _db.F1903s.AsNoTracking();
      var f1908 = _db.F1908s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
                                                                                                       x.CUST_CODE == custCode);
      //20211216: 新增點收時間、開始驗收時間、等待驗收時間
      //join F010205
      var f010205Status1 = _db.F010205s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                              x.GUP_CODE == gupCode &&
                                                                              x.CUST_CODE == custCode && x.STATUS == "1");
      var f010205Status5 = _db.F010205s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                           x.GUP_CODE == gupCode &&
                                                                           x.CUST_CODE == custCode && x.STATUS == "5");

      if (begCrtDate.HasValue)
      {
        f0202 = f0202.Where(x => x.CRT_DATE >= begCrtDate.Value.Date);
      }

      if (endCrtDate.HasValue)
      {
        f0202 = f0202.Where(x => x.CRT_DATE < endCrtDate.Value.Date);
      }

      if (!string.IsNullOrWhiteSpace(orderNo))
      {
        f0202 = f0202.Where(x => x.ORDER_NO == orderNo);
        f010201 = f010201.Where(x => x.STOCK_NO == orderNo);
        f010202 = f010202.Where(x => x.STOCK_NO == orderNo);
        f010205Status1 = f010205Status1.Where(x => x.STOCK_NO == orderNo);
        f010205Status5 = f010205Status5.Where(x => x.STOCK_NO == orderNo);

      }

      if (!string.IsNullOrWhiteSpace(empId))
      {
        f0202 = f0202.Where(x => x.CRT_STAFF == empId);
      }

      if (!string.IsNullOrWhiteSpace(empName))
      {
        f0202 = f0202.Where(x => x.CRT_NAME == empName);
      }

      if (!string.IsNullOrWhiteSpace(custOrdNo))
      {
        f010201 = f010201.Where(x => x.CUST_ORD_NO == custOrdNo);
      }

      if (!string.IsNullOrWhiteSpace(itemCode))
      {
        f010202 = f010202.Where(x => x.ITEM_CODE == itemCode);
      }

      if (!string.IsNullOrWhiteSpace(selectedFastType))
      {
        f010201 = f010201.Where(x => x.FAST_PASS_TYPE == selectedFastType);
      }

      //排除f010205Status1重複內容
      var gupf010205Status1 = f010205Status1.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.STOCK_NO, x.RT_NO, x.ALLOCATION_NO })
                                            .Select(x => new
                                            {
                                              DC_CODE = x.Key.DC_CODE,
                                              GUP_CODE = x.Key.GUP_CODE,
                                              CUST_CODE = x.Key.CUST_CODE,
                                              STOCK_NO = x.Key.STOCK_NO,
                                              RT_NO = x.Key.RT_NO,
                                              ALLOCATION_NO = x.Key.ALLOCATION_NO,
                                              CRT_DATE = x.Max(a => a.CRT_DATE)
                                            });

      var result = from item in (from A in f0202
                                 join B in f010201
                                  on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.ORDER_NO } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, ORDER_NO = B.STOCK_NO }
                                 join C in f010202
                                  on new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.STOCK_NO } equals new { C.DC_CODE, C.GUP_CODE, C.CUST_CODE, C.STOCK_NO }
                                 join D in f1903
                                  on new { C.GUP_CODE, C.CUST_CODE, C.ITEM_CODE } equals new { D.GUP_CODE, D.CUST_CODE, D.ITEM_CODE }
                                 join E in f1908
                                  on new { A.GUP_CODE, A.CUST_CODE, A.VNR_CODE } equals new { E.GUP_CODE, E.CUST_CODE, E.VNR_CODE }
                                 join F in gupf010205Status1
                                  on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.ORDER_NO } equals new { F.DC_CODE, F.GUP_CODE, F.CUST_CODE, ORDER_NO = F.STOCK_NO }
                                 join H in f010205Status5
                                  on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.ORDER_NO } equals new { H.DC_CODE, H.GUP_CODE, H.CUST_CODE, ORDER_NO = H.STOCK_NO } into ps
                                 from I in ps.DefaultIfEmpty()
                                 orderby A.CHECKIN_DATE
                                 select new
                                 {
                                   A.CRT_DATE,
                                   A.CHECKIN_DATE,
                                   B.CUST_ORD_NO,
                                   A.ORDER_NO,
                                   C.ITEM_CODE,
                                   C.STOCK_QTY,
                                   D.ITEM_NAME,
                                   A.CRT_STAFF,
                                   A.CRT_NAME,
                                   A.VNR_CODE,
                                   E.VNR_NAME,
                                   B.STATUS,
                                   B.FAST_PASS_TYPE,
                                   B.DELIVER_DATE,
                                   B.STOCK_DATE,
                                   B.BOOKING_IN_PERIOD,
                                   CHECKACCEPT_TIME = F.CRT_DATE,
                                   BEGIN_CHECKACCEPT_TIME = (I != null) ? I.CRT_DATE : new Nullable<DateTime>()
                                 })
                   group item by new
                   {
                     item.CRT_DATE,
                     item.CHECKIN_DATE,
                     item.CUST_ORD_NO,
                     item.ORDER_NO,
                     item.ITEM_CODE,
                     item.ITEM_NAME,
                     item.CRT_STAFF,
                     item.CRT_NAME,
                     item.VNR_CODE,
                     item.VNR_NAME,
                     item.STATUS,
                     item.FAST_PASS_TYPE,
                     item.DELIVER_DATE,
                     item.STOCK_DATE,
                     item.BOOKING_IN_PERIOD
                   } into g
                   select new F0202Data
                   {
                     CRT_DATE = g.Key.CRT_DATE,
                     CHECKIN_DATE = g.Key.CHECKIN_DATE,
                     CUST_ORD_NO = g.Key.CUST_ORD_NO,
                     ORDER_NO = g.Key.ORDER_NO,
                     ITEM_CODE = g.Key.ITEM_CODE,
                     ITEM_NAME = g.Key.ITEM_NAME,
                     CRT_STAFF = g.Key.CRT_STAFF,
                     STOCK_QTY = g.Sum(x => x.STOCK_QTY),
                     CRT_NAME = g.Key.CRT_NAME,
                     VNR_CODE = g.Key.VNR_CODE,
                     VNR_NAME = g.Key.VNR_NAME,
                     STATUS = g.Key.STATUS,
                     FAST_PASS_TYPE = g.Key.FAST_PASS_TYPE,
                     DELIVER_DATE = g.Key.DELIVER_DATE,
                     STOCK_DATE = g.Key.STOCK_DATE,
                     BOOKING_IN_PERIOD = g.Key.BOOKING_IN_PERIOD,
                     //CHECKACCEPT_TIME = g.Min(o => o.CHECKACCEPT_TIME).ToString("yyyy/MM/dd HH:mm:ss"),
                     BEGIN_CHECKACCEPT_TIME = g.Max(o => o.BEGIN_CHECKACCEPT_TIME).HasValue ? g.Max(o => o.BEGIN_CHECKACCEPT_TIME).Value.ToString("yyyy/MM/dd HH:mm:ss") : "",
                     ACCEPTANCE_WAITTIME = ""
                   };
      return result.Distinct();


    }
  }
}
