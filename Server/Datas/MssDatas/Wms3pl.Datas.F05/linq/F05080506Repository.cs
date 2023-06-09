using Microsoft.EntityFrameworkCore;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
  public partial class F05080506Repository : RepositoryBase<F05080506, Wms3plDbContext, F05080506Repository>
  {
    public F05080506Repository(string connName, WmsTransaction wmsTransaction = null) : base(connName, wmsTransaction)
    { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="calNo"></param>
    /// <param name="flag">查詢模式 0:手動挑單試算 1:配庫試算結果查詢</param>
    /// <returns></returns>
    public IQueryable<F05080506Data> GetF05080506Datas(string dcCode, string gupCode, string custCode, string calNo, string flag)
    {
      var result = from a in _db.F05080506s.AsNoTracking()
                   join b in _db.F1903s.AsNoTracking()
                    on new { a.GUP_CODE, a.CUST_CODE, a.ITEM_CODE } equals new { b.GUP_CODE, b.CUST_CODE, b.ITEM_CODE }
                   where a.DC_CODE == dcCode && a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.CAL_NO == calNo
                   select new F05080506Data
                   {
                     ORD_NO = a.ORD_NO,
                     ITEM_CODE = a.ITEM_CODE,
                     ITEM_NAME = b.ITEM_NAME,
                     A_QTY = a.A_QTY,
                     B_QTY = a.B_QTY,
                     IS_LACK = a.IS_LACK == "1" ? "是" : "",
                     WAREHOUSE_INFO = a.WAREHOUSE_INFO
                   };

      if (flag == "0")
        //有存在於訂單池才顯示
        result = result.Where(a => _db.F050001s.Any(b => b.DC_CODE == dcCode && b.GUP_CODE == gupCode && b.CUST_CODE == custCode && b.ORD_NO == a.ORD_NO));

      return result;
    }
  }
}
