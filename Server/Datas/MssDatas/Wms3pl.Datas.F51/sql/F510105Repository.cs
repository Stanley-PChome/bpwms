using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
namespace Wms3pl.Datas.F51
{
	public partial class F510105Repository : RepositoryBase<F510105, Wms3plDbContext, F510105Repository>
	{
        public void UpdateProcFlag(string dcCode, string calDate, string procFlag)
        {
            var parameters = new List<object>
            {
                procFlag,
                Current.Staff,
                DateTime.Now,
                Current.StaffName,
                dcCode,
                calDate
            };

      var sql = @"
				UPDATE  F510105  SET PROC_FLAG= @p0,
                                     DIFF_QTY=(WMS_QTY+BOOKING_QTY-WCS_QTY),
                                     UPD_STAFF = @p1,
						             UPD_DATE = @p2,
						             UPD_NAME = @p3
				 Where DC_CODE = @p4
                     And CAL_DATE = @p5
                     And PROC_FLAG = '0' ";

      ExecuteSqlCommand(sql, parameters.ToArray());
    }

    public IQueryable<F510105> GetDatasByCalDate(string dcCode, string gupCode, string custCode, DateTime calDate)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode)     { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode)    { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode)   { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3", calDate)    { SqlDbType = SqlDbType.DateTime2 },
      };
      var sql = @"SELECT * FROM F510105 WHERE DC_CODE = @p0 AND GUP_CODE = @p1 AND CUST_CODE = @p2 AND CONVERT(DATETIME2, CAL_DATE) < @p3";
      return SqlQuery<F510105>(sql, para.ToArray());
      //return _db.F510105s.Where(x => x.DC_CODE == dcCode &&
      //x.GUP_CODE == gupCode &&
      //x.CUST_CODE == custCode &&
      //Convert.ToDateTime(x.CAL_DATE) < calDate);
    }

    public IQueryable<F510105> GetDatasEqualCalDate(string dcCode, string gupCode, string custCode, string calDate)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode)     { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode)    { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode)   { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3", calDate)    { SqlDbType = SqlDbType.VarChar },
      };
      var sql = @"SELECT * FROM F510105 WHERE DC_CODE = @p0 AND GUP_CODE = @p1 AND CUST_CODE = @p2 AND CAL_DATE = @p3";
      return SqlQuery<F510105>(sql, para.ToArray());

      //return _db.F510105s.Where(x => x.DC_CODE == dcCode &&
      //x.GUP_CODE == gupCode &&
      //x.CUST_CODE == custCode &&
      //x.CAL_DATE == calDate);
    }

    public Boolean ChktDatasExists(string dcCode, string gupCode, string custCode, string calDate)
    {
      // 原GetDatasEqualCalDate將語法優化
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode)     { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode)    { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode)   { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3", calDate)    { SqlDbType = SqlDbType.VarChar },
      };
      var sql = @"SELECT TOP 1 1 FROM F510105 WHERE DC_CODE = @p0 AND GUP_CODE = @p1 AND CUST_CODE = @p2 AND CAL_DATE = @p3";
      return SqlQuery<int>(sql, para.ToArray()).Any();

      //return _db.F510105s.Where(x => x.DC_CODE == dcCode &&
      //x.GUP_CODE == gupCode &&
      //x.CUST_CODE == custCode &&
      //x.CAL_DATE == calDate);
    }


    public IQueryable<F510105> GetAddDatasByStockSnapshot(string dcCode, string gupCode, string custCode, DateTime now)
    {
      //(1) AA = 撈loc_code from F1912 by dc_code+warehouse_id
      //(2) 撈 F1913.*, sum(QTY) wms庫存
      //	條件: loc_code in (AA) + dc_code
      //	條件: group by GUP_CODE、CUST_CODE、ITEM_CODE、VALID_DATE、MAKE_NO
      //(3) 另外撈 sum(F1511.B_PICK_QTY) 虛擬帳庫存
      //	條件: status = 0 + loc_code in (AA) + dc_code
      //	條件: group by GUP_CODE、CUST_CODE、ITEM_CODE、VALID_DATE、MAKE_NO
      //	欄位請參考5.7.4
      //(4) 當(3)與(2)以下條件相同時，表示寫入同一筆資料
      //	DC_CODE、GUP_CODE、CUST_CODE、WAREHOUSE_ID、LOC_CODE、ITEM_CODE、VALID_DATE、MAKE_NO

      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode)                     { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode)                    { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode)                   { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3", now.ToString("yyyy/MM/dd")) { SqlDbType = SqlDbType.VarChar },
      };

      var sql = @"
SELECT 
  DC_CODE, 
  GUP_CODE, 
  CUST_CODE, 
  WAREHOUSE_ID, 
  @p3 AS CAL_DATE, 
  LOC_CODE, 
  ITEM_CODE, 
  VALID_DATE, 
  MAKE_NO, 
  SUM(QTY) AS WMS_QTY, 
  0 AS WCS_QTY, 
  SUM(B_PICK_QTY) AS BOOKING_QTY, 
  0 AS DIFF_QTY, 
  '0' AS PROC_FLAG 
FROM 
  (
    SELECT 
      A.DC_CODE, 
      A.GUP_CODE, 
      A.CUST_CODE, 
      B.WAREHOUSE_ID, 
      A.LOC_CODE, 
      A.ITEM_CODE, 
      A.VALID_DATE, 
      A.MAKE_NO, 
      A.QTY, 
      0 AS B_PICK_QTY 
    FROM 
      F1913 A 
      INNER JOIN(
        SELECT 
          F1912.DC_CODE, 
          F1912.LOC_CODE, 
          F1912.WAREHOUSE_ID 
        FROM 
          F1912 
          INNER JOIN F1980 ON F1912.DC_CODE = F1980.DC_CODE 
          AND F1912.WAREHOUSE_ID = F1980.WAREHOUSE_ID 
        WHERE 
          DEVICE_TYPE != '0'
          AND F1980.DC_CODE = @p0
      ) AS B ON A.DC_CODE = B.DC_CODE 
      AND A.LOC_CODE = B.LOC_CODE 
    WHERE 
      A.DC_CODE = @p0
      AND A.GUP_CODE = @p1 
      AND A.CUST_CODE = @p2 
    UNION ALL
    SELECT 
      A.DC_CODE, 
      A.GUP_CODE, 
      A.CUST_CODE, 
      B.WAREHOUSE_ID, 
      A.LOC_CODE, 
      A.ITEM_CODE, 
      A.VALID_DATE, 
      A.MAKE_NO, 
      0 AS QTY, 
      A.B_PICK_QTY 
    FROM 
      VW_VirtualStock  A 
      INNER JOIN(
        SELECT 
          F1912.DC_CODE, 
          F1912.LOC_CODE, 
          F1912.WAREHOUSE_ID 
        FROM 
          F1912 
          INNER JOIN F1980 ON F1912.DC_CODE = F1980.DC_CODE 
          AND F1912.WAREHOUSE_ID = F1980.WAREHOUSE_ID 
        WHERE 
          DEVICE_TYPE != '0'
          AND F1980.DC_CODE = @p0
      ) AS B ON A.DC_CODE = B.DC_CODE 
      AND A.LOC_CODE = B.LOC_CODE 
    WHERE 
      A.DC_CODE = @p0
      AND A.GUP_CODE = @p1 
      AND A.CUST_CODE = @p2 
      AND A.STATUS = '0'
  ) AS S 
GROUP BY 
  S.DC_CODE, 
  S.GUP_CODE, 
  S.CUST_CODE, 
  S.WAREHOUSE_ID, 
  S.LOC_CODE, 
  S.ITEM_CODE, 
  S.VALID_DATE, 
  S.MAKE_NO
";
      return SqlQuery<F510105>(sql, para.ToArray());
      #region 原有linq語法
      /*
      var f1980s = _db.F1980s.AsNoTracking().Where(o => o.DC_CODE == dcCode && o.DEVICE_TYPE != "0");

      var warehouseIds = f1980s.Select(x => x.WAREHOUSE_ID);

      var calDate = now.ToString("yyyy/MM/dd");

      var f1912s = _db.F1912s.AsNoTracking().Where(x =>
      x.DC_CODE == dcCode &&
      x.GUP_CODE == gupCode &&
      x.CUST_CODE == custCode &&
      warehouseIds.Contains(x.WAREHOUSE_ID));

      var locCodes = f1912s.Select(x => x.LOC_CODE);

      #region 實際庫存數
      var f1913s = _db.F1913s.AsNoTracking().Where(x =>
      x.DC_CODE == dcCode &&
      x.GUP_CODE == gupCode &&
      x.CUST_CODE == custCode &&
      locCodes.Contains(x.LOC_CODE));

      var stockDatas = (from A in f1913s
                        join B in f1912s
                        on new { A.DC_CODE, A.LOC_CODE } equals new { B.DC_CODE, B.LOC_CODE }
                        select new StockSnapshotData
                        {
                          WAREHOUSE_ID = B.WAREHOUSE_ID,
                          LOC_CODE = A.LOC_CODE,
                          ITEM_CODE = A.ITEM_CODE,
                          VALID_DATE = A.VALID_DATE,
                          MAKE_NO = A.MAKE_NO,
                          QTY = Convert.ToInt32(A.QTY)
                        }).ToList();
      #endregion

      #region 虛擬未搬動庫存數
      var f1511s = _db.F1511s.AsNoTracking().Where(x =>
      x.DC_CODE == dcCode &&
      x.GUP_CODE == gupCode &&
      x.CUST_CODE == custCode &&
      locCodes.Contains(x.LOC_CODE) &&
      x.STATUS == "0");

      var virtualDatas = (from A in f1511s
                          join B in f1912s
                          on new { A.DC_CODE, A.LOC_CODE } equals new { B.DC_CODE, B.LOC_CODE }
                          select new StockSnapshotData
                          {
                            WAREHOUSE_ID = B.WAREHOUSE_ID,
                            LOC_CODE = A.LOC_CODE,
                            ITEM_CODE = A.ITEM_CODE,
                            VALID_DATE = Convert.ToDateTime(A.VALID_DATE),
                            MAKE_NO = A.MAKE_NO,
                            B_PICK_QTY = A.B_PICK_QTY
                          }).ToList();
      #endregion

      stockDatas = stockDatas.Union(virtualDatas).ToList();

      var datas = stockDatas.GroupBy(x => new { x.WAREHOUSE_ID, x.LOC_CODE, x.ITEM_CODE, x.VALID_DATE, x.MAKE_NO })
        .Select(x => new F510105
        {
          DC_CODE = dcCode,
          GUP_CODE = gupCode,
          CUST_CODE = custCode,
          WAREHOUSE_ID = x.Key.WAREHOUSE_ID,
          CAL_DATE = calDate,
          LOC_CODE = x.Key.LOC_CODE,
          ITEM_CODE = x.Key.ITEM_CODE,
          VALID_DATE = x.Key.VALID_DATE,
          MAKE_NO = x.Key.MAKE_NO,
          WMS_QTY = x.Sum(z => z.QTY),
          WCS_QTY = 0,
          BOOKING_QTY = x.Sum(z => z.B_PICK_QTY),
          DIFF_QTY = 0,
          PROC_FLAG = "0"
        }).ToList();

      return datas;
      */
      #endregion

    }

  }
}
