using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
  public partial class F020502Repository : RepositoryBase<F020502, Wms3plDbContext, F020502Repository>
  {
    public IQueryable<F020502> GetDataByF020501Id(string dcCode, string gupCode, string custCode, long f020501Id, string binCode)
    {
      var parms = new List<SqlParameter> {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", f020501Id)
            };

      string insql = string.Empty;
      if (!string.IsNullOrWhiteSpace(binCode))
      {
        insql = " AND BIN_CODE = @p4 ";
        parms.Add(new SqlParameter("@p4", binCode));
      }

      string sql = $@"
        					SELECT * 
                            FROM F020502
                            WHERE DC_CODE = @p0
                            AND GUP_CODE = @p1
                            AND CUST_CODE = @p2
                            AND F020501_ID = @p3
        					{insql} ";

      return SqlQuery<F020502>(sql, parms.ToArray());
    }

    /// <summary>
    /// 取得最新一筆F020502
    /// </summary>
    /// <returns></returns>
    public F020502 GetNewestData()
    {
      var sql = @"SELECT TOP 1 * FROM F020502 ORDER BY ID DESC";
      return SqlQuery<F020502>(sql).FirstOrDefault();
    }

    public IQueryable<ContainerRecheckFaildItem> GetContainerRecheckFaildItem(string dcCode, string gupCode, string custCode, string ContainerCode)
    {
      var para = new List<SqlParameter>() {
        new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3", ContainerCode) { SqlDbType = SqlDbType.VarChar },
      };
      var sql = $@"
								SELECT C.ID F020502_ID,C.STOCK_NO,C.STOCK_SEQ,C.RT_NO,C.RT_SEQ,B.CONTAINER_CODE,C.BIN_CODE,C.ITEM_CODE,D.ITEM_NAME,C.QTY,E.NAME ACCE_STATUS,C.STATUS,C.F020501_ID
								FROM F0701 A
								JOIN F020501 B
								ON B.F0701_ID = A.ID
								JOIN F020502 C
								ON C.F020501_ID = B.ID
								LEFT JOIN F1903 D 
									ON D.GUP_CODE =C.GUP_CODE AND D.CUST_CODE =C.CUST_CODE AND D.ITEM_CODE =C.ITEM_CODE 
								LEFT JOIN VW_F000904_LANG E
									ON E.TOPIC='F020502' AND E.SUBTOPIC='STATUS' AND C.STATUS=E.VALUE AND E.LANG='{Current.Lang}'
								WHERE B.DC_CODE =@p0 
									AND B.GUP_CODE =@p1 
									AND B.CUST_CODE=@p2 
									AND (B.CONTAINER_CODE=@p3 OR C.BIN_CODE=@p3) ";

      //加上ROW_NUM
      sql = $@"
SELECT 
  ROW_NUMBER()OVER(ORDER BY aa.F020502_ID) AS ROW_NUM,
  aa.*
FROM
({sql}) aa";

      return SqlQuery<ContainerRecheckFaildItem>(sql, para.ToArray());
    }
    public IQueryable<F020502> GetDatasByF020501Id(long f020501Id)
    {
      var parms = new List<SqlParameter>
      {
        new SqlParameter("@p0",f020501Id){SqlDbType= SqlDbType.BigInt}
      };
      var sql = @" SELECT *
                     FROM F020502
                    WHERE F020501_ID= @p0";
      return SqlQuery<F020502>(sql, parms.ToArray());
    }

    public IQueryable<F020502> GetDatasByRtNos(List<string> rtNos)
    {
      var parms = new List<object>
      {
      };

      var sql = @" SELECT *
                     FROM F020502
                    WHERE 1 = 1 ";
      sql += parms.CombineSqlInParameters("AND RT_NO", rtNos);
      return SqlQuery<F020502>(sql, parms.ToArray());
    }

    public string LockF020502()
    {
      var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F020502';";
      return SqlQuery<string>(sql).FirstOrDefault();
    }

    public long GetF020502NextId()
    {
      var sql = @"SELECT NEXT VALUE FOR SEQ_F020502_ID";

      return SqlQuery<long>(sql).Single();
    }

    public IQueryable<RecvBindContainerWaitClosedQueryRes> GetRecvBindContainerWaitClosedQueryRes(string dcCode, string gupCode, string custCode, string rtNo, string rtSeq)
    {
      var para = new List<SqlParameter>()
      {
        new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcCode },
        new SqlParameter("@p1", SqlDbType.VarChar) { Value = gupCode },
        new SqlParameter("@p2", SqlDbType.VarChar) { Value = custCode },
        new SqlParameter("@p3", SqlDbType.VarChar) { Value = rtNo },
        new SqlParameter("@p4", SqlDbType.VarChar) { Value = rtSeq }
      };

      var sql = $@"
SELECT 
	B.ID F020501_ID,
	B.F0701_ID,
	B.TYPE_CODE TypeCode,
	D.NAME TypeName,
	B.CONTAINER_CODE ContainerCode,
	B.PICK_WARE_ID PickWareId,
	C.WAREHOUSE_NAME PickWareName
FROM 
	F020502 A
	INNER JOIN F020501 B
		ON A.F020501_ID = B.ID 
	LEFT JOIN F1980 C WITH(NOLOCK)
		ON B.DC_CODE = C.DC_CODE 
		AND B.PICK_WARE_ID = C.WAREHOUSE_ID 
	LEFT JOIN VW_F000904_LANG D 
		ON D.TOPIC = 'F0205'
		AND D.SUBTOPIC = 'TYPE_CODE'
    AND D.LANG='{Current.Lang}'
		AND D.VALUE = B.TYPE_CODE 
WHERE
	A.DC_CODE = @p0
	AND A.GUP_CODE = @p1
	AND A.CUST_CODE = @p2
	AND A.RT_NO = @p3
	AND A.RT_SEQ = @p4
  AND B.STATUS = '0'
GROUP BY 
	B.ID, B.F0701_ID, B.CONTAINER_CODE, B.TYPE_CODE, D.NAME, B.PICK_WARE_ID, C.WAREHOUSE_NAME";
      return SqlQuery<RecvBindContainerWaitClosedQueryRes>(sql, para.ToArray());
    }

    /// <summary>
    /// 檢查該容器是否有其他單尚未分播完成
    /// </summary>
    /// <param name="f020501Id"></param>
    /// <param name="excludeSockNo">目前操作的驗收單號</param>
    /// <returns></returns>
    public bool CheckAcceptanceAllSow(long f020501Id, string excludeRtNo)
    {
      var para = new List<SqlParameter>()
      {
        new SqlParameter("@p0", SqlDbType.BigInt) { Value = f020501Id },
        new SqlParameter("@p1", SqlDbType.VarChar) { Value = excludeRtNo },
      };

      var sql = $@"
SELECT 
TOP 1 1
FROM 
  F020502 A
  INNER JOIN F0205 B
  	ON A.DC_CODE = B.DC_CODE 
	  AND A.GUP_CODE = B.GUP_CODE 
	  AND A.CUST_CODE = B.CUST_CODE 
	  AND A.STOCK_NO = B.STOCK_NO 
	  AND A.STOCK_SEQ = B.STOCK_SEQ 
	  AND A.RT_NO = B.RT_NO 
	  AND A.RT_SEQ = B.RT_SEQ 
WHERE 
	A.F020501_ID = @p0
  AND A.RT_NO != @p1
	AND B.STATUS = '0'";

      return SqlQuery<int>(sql, para.ToArray()).Any();
    }

  }
}
