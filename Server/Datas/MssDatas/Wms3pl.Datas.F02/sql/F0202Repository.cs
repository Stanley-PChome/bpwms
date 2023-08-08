using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;


using Wms3pl.Datas.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F02
{
  public partial class F0202Repository : RepositoryBase<F0202, Wms3plDbContext, F0202Repository>
  {
    /// <summary>
    /// 刪除F0202
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCoed"></param>
    /// <param name="orderNo"></param>
    public void DelF0202(string dcCode, string gupCode, string custCoed, string orderNo)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0",SqlDbType.VarChar){ Value = dcCode},
        new SqlParameter("@p1",SqlDbType.VarChar) { Value = gupCode},
        new SqlParameter("@p2",SqlDbType.VarChar){ Value = custCoed},
        new SqlParameter("@p3",SqlDbType.VarChar){ Value = orderNo}
      };

      var sql = @"DELETE FROM F0202 WHERE DC_CODE = @p0
							AND GUP_CODE = @p1
							AND CUST_CODE = @p2
							AND ORDER_NO = @p3";

      ExecuteSqlCommand(sql, param.ToArray());
    }


    public IQueryable<F0202Data> GetF0202Datas_SQL(string dcCode, string gupCode, string custCode, DateTime? begCrtDate, DateTime? endCrtDate,
            string orderNo, DateTime begCheckinDate, DateTime endCheckinDate, string custOrdNo, string empId, string empName, string itemCode,
            string selectedFastType)
    {
      var sql_Where = @" ";
      var sqlParamers = new List<SqlParameter>();
      sqlParamers.Add(new SqlParameter("@dcCode", dcCode)     { SqlDbType = SqlDbType.VarChar });
      sqlParamers.Add(new SqlParameter("@gupCode", gupCode)   { SqlDbType = SqlDbType.VarChar });
      sqlParamers.Add(new SqlParameter("@custcode", custCode) { SqlDbType = SqlDbType.VarChar });
      sqlParamers.Add(new SqlParameter("@begCheckinDate", begCheckinDate) { SqlDbType = SqlDbType.DateTime2 });
      sqlParamers.Add(new SqlParameter("@endCheckinDate", endCheckinDate) { SqlDbType = SqlDbType.DateTime2 });

      #region sql_Where Add
      if (begCrtDate.HasValue)
      {
        sqlParamers.Add(new SqlParameter("@begCrtDate", begCrtDate) { SqlDbType = SqlDbType.DateTime2 });
        sql_Where += " and a.CRT_DATE>= @begCrtDate ";
      }

      if (endCrtDate.HasValue)
      {
        sqlParamers.Add(new SqlParameter("@endCrtDate", endCrtDate) { SqlDbType = SqlDbType.DateTime2 });
        sql_Where += " and a.CRT_DATE < @endCrtDate ";
      }
      if (!string.IsNullOrWhiteSpace(orderNo))
      {
        sqlParamers.Add(new SqlParameter("@orderNo", orderNo) { SqlDbType = SqlDbType.VarChar });
        sql_Where += " and a.ORDER_NO= @orderNo ";
      }

      if (!string.IsNullOrWhiteSpace(empId))
      {
        sqlParamers.Add(new SqlParameter("@empId", empId) { SqlDbType = SqlDbType.VarChar });
        sql_Where += " and a.CRT_STAFF=@empId ";
      }
      if (!string.IsNullOrWhiteSpace(empName))
      {
        sqlParamers.Add(new SqlParameter("@empName", empName) { SqlDbType = SqlDbType.VarChar });
        sql_Where += " and a.CRT_NAME=@empName ";
      }

      if (!string.IsNullOrWhiteSpace(custOrdNo))
      {
        sqlParamers.Add(new SqlParameter("@custOrdNo", custOrdNo) { SqlDbType = SqlDbType.VarChar });
        sql_Where += " and b.CUST_ORD_NO = @custOrdNo ";
      }

      if (!string.IsNullOrWhiteSpace(itemCode))
      {
        sqlParamers.Add(new SqlParameter("@itemCode", itemCode) { SqlDbType = SqlDbType.VarChar });
        sql_Where += " and c.ITEM_CODE = @itemCode ";
      }

      if (!string.IsNullOrWhiteSpace(selectedFastType))
      {
        sqlParamers.Add(new SqlParameter("@selectedFastType", selectedFastType) { SqlDbType = SqlDbType.VarChar });
        sql_Where += " and b.FAST_PASS_TYPE = @selectedFastType ";
      }
      #endregion sql_Where Add

      var sql = @"
        Select Distinct
	        A.CRT_DATE                      ,	A.CHECKIN_DATE  , B.CUST_ORD_NO   ,	A.ORDER_NO      ,	C.ITEM_CODE
	        , sum(C.STOCK_QTY) 'STOCK_QTY'	, D.ITEM_NAME     ,	A.CRT_STAFF     ,	A.CRT_NAME      ,	A.VNR_CODE
	        , E.VNR_NAME                    , B.STATUS        , B.FAST_PASS_TYPE, B.DELIVER_DATE  , B.STOCK_DATE
	        , B.BOOKING_IN_PERIOD           , min(F.CRT_DATE) 'CHECKACCEPT_TIME'                                   
	        , convert(varchar,max(h.CRT_DATE) ,120) 'BEGIN_CHECKACCEPT_TIME'	
        from f0202 a 
        join f010201 b on A.DC_CODE = b.DC_CODE and   A.GUP_CODE= b.gup_code and  A.CUST_CODE =b.cust_code and   A.order_no= B.STOCK_NO 
        join f010202 c on A.DC_CODE = c.DC_CODE and   A.GUP_CODE= c.gup_code and  A.CUST_CODE =c.cust_code and   A.order_no= c.STOCK_NO 
        join f1903   d on A.gup_code=  d.gup_code and A.cust_code= d.cust_code and C.item_code= d.item_code
        join f1908   e on A.gup_code=  e.gup_code and A.cust_code= e.cust_code and A.vnr_code = e.vnr_code
        join  (select  DC_CODE, GUP_CODE, CUST_CODE, STOCK_NO, RT_NO, ALLOCATION_NO, max(CRT_DATE) 'CRT_DATE' From f010205 
          Where status ='1'  group by DC_CODE, GUP_CODE, CUST_CODE, STOCK_NO, RT_NO, ALLOCATION_NO ) f  on  a.DC_CODE =F.DC_CODE and  a.GUP_CODE = F.GUP_CODE and  a.CUST_CODE =F.CUST_CODE and  A.ORDER_NO =F.STOCK_NO
        left join  f010205 h on h.status ='5' and  A.DC_CODE =H.DC_CODE and  A.GUP_CODE = H.GUP_CODE and  A.CUST_CODE =H.CUST_CODE and  A.ORDER_NO =H.STOCK_NO
        Where a.dc_code= @dcCode and a.gup_code= @gupCode and a.cust_code= @custcode and  a.checkin_date >= @begCheckinDate and a.checkin_date < @endCheckinDate     ";

      var sql_GroupBy = @"  Group by 
        A.CRT_DATE    , A.CHECKIN_DATE    ,	B.CUST_ORD_NO   ,	A.ORDER_NO    , C.ITEM_CODE
	      , D.ITEM_NAME , A.CRT_STAFF       ,	A.CRT_NAME      ,	A.VNR_CODE    , E.VNR_NAME
        , B.STATUS    , B.FAST_PASS_TYPE  , B.DELIVER_DATE  , B.STOCK_DATE	, B.BOOKING_IN_PERIOD ";
      sql += sql_Where + sql_GroupBy;
      return SqlQuery<F0202Data>(sql, sqlParamers.ToArray());

    }
  }
}
