using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
  public partial class F020504Repository : RepositoryBase<F020504, Wms3plDbContext, F020504Repository>
  {
    public IQueryable<F020504ExData> GetF020504ExDatas(string dcCode, string gupCode, string custCode, DateTime? ProcDateStart, DateTime? ProcDateEnd, string StockNo, string RTNo, string ProcCode, string ContainerCode, string ItemCode, string Status)
    {
      var para = new List<SqlParameter>()
      {
        new SqlParameter("@p0",dcCode){SqlDbType=SqlDbType.VarChar},
        new SqlParameter("@p1",gupCode){SqlDbType=SqlDbType.VarChar},
        new SqlParameter("@p2",custCode){SqlDbType=SqlDbType.VarChar},
      };
      var sql = $@"
SELECT
    a.ID,
    a.STOCK_NO,
    a.RT_NO,
    a.CONTAINER_CODE,
    a.BIN_CODE,
    a.ITEM_CODE,
    b.ITEM_NAME,
    a.PROC_CODE,
    -- c.NAME PROC_CODE,
    a.QTY,
    a.REMOVE_RECV_QTY,
    a.NOTGOOD_QTY,
    a.STATUS
    -- d.NAME STATUS,
    
FROM
    F020504 a
    LEFT JOIN F1903 b 
      ON a.GUP_CODE = b.GUP_CODE
        AND a.CUST_CODE = b.CUST_CODE
        AND a.ITEM_CODE = b.ITEM_CODE
    LEFT JOIN VW_F000904_LANG c
      ON c.TOPIC='F020504'
        AND c.SUBTOPIC='PROC_CODE'
        AND c.VALUE=a.PROC_CODE
        AND c.LANG='{Current.Lang}'
    LEFT JOIN VW_F000904_LANG d
      ON d.TOPIC='F020504'
        AND d.SUBTOPIC='STATUS'
        AND d.VALUE=a.STATUS
        AND d.LANG='{Current.Lang}'
WHERE
    a.DC_CODE = @p0
    AND a.GUP_CODE=@p1
    AND a.CUST_CODE=@p2";
      if (ProcDateStart.HasValue && ProcDateEnd.HasValue)
      {
        sql += $" AND a.PROC_DATE >= @p{para.Count} AND a.PROC_DATE <= @p{para.Count + 1}";
        para.Add(new SqlParameter($"@p{para.Count}", ProcDateStart.Value) { SqlDbType = SqlDbType.DateTime2 });
        para.Add(new SqlParameter($"@p{para.Count}", ProcDateEnd.Value) { SqlDbType = SqlDbType.DateTime2 });
      }
      if (!String.IsNullOrWhiteSpace(StockNo))
      {
        sql += $" AND a.STOCK_NO = @p{para.Count}";
        para.Add(new SqlParameter($"@p{para.Count}", StockNo) { SqlDbType = SqlDbType.VarChar });
      }
      if (!String.IsNullOrWhiteSpace(RTNo))
      {
        sql += $" AND a.RT_NO = @p{para.Count}";
        para.Add(new SqlParameter($"@p{para.Count}", RTNo) { SqlDbType = SqlDbType.VarChar });
      }
      if (!String.IsNullOrWhiteSpace(ProcCode))
      {
        sql += $" AND a.PROC_CODE = @p{para.Count}";
        para.Add(new SqlParameter($"@p{para.Count}", ProcCode) { SqlDbType = SqlDbType.Char });
      }
      if (!String.IsNullOrWhiteSpace(ContainerCode))
      {
        sql += $" AND (a.CONTAINER_CODE = @p{para.Count} OR a.BIN_CODE = @p{para.Count})";
        para.Add(new SqlParameter($"@p{para.Count}", ContainerCode) { SqlDbType = SqlDbType.VarChar });
      }
      if (!String.IsNullOrWhiteSpace(ItemCode))
      {
        sql += $" AND a.ITEM_CODE = @p{para.Count}";
        para.Add(new SqlParameter($"@p{para.Count}", ItemCode) { SqlDbType = SqlDbType.VarChar });
      }
      if (!String.IsNullOrWhiteSpace(Status))
      {
        sql += $" AND a.STATUS = @p{para.Count}";
        para.Add(new SqlParameter($"@p{para.Count}", Status) { SqlDbType = SqlDbType.VarChar });
      }

      //加上ROW_NUM
      sql = $@"
SELECT 
  ROW_NUMBER()OVER(ORDER BY aa.ID) AS ROW_NUM,
  aa.*
FROM
({sql}) aa";
      return SqlQuery<F020504ExData>(sql, para.ToArray());
    }

  }
}
