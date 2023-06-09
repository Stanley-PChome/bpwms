using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F00.Interfaces;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
  public partial class F0090Repository : RepositoryBase<F0090, Wms3plDbContext, F0090Repository>, IApiLogRepository<F0090>
  {
    public IQueryable<F0090> GetData(string name)
    {
      var param = new object[] { name };
      var sql = @"SELECT * FROM F0090 WHERE NAME = @p0 AND STATUS IS NULL";
      return SqlQuery<F0090>(sql, param);
    }

    public void UpdateLog(int id, string status, string errMsg, string retrunData)
    {
      if (!string.IsNullOrWhiteSpace(errMsg))
      {
        if (errMsg.Length > 200)
          errMsg = errMsg.Substring(0, 200);
      }
      if (!string.IsNullOrWhiteSpace(retrunData))
      {
        if (retrunData.Length > 4000)
          retrunData = retrunData.Substring(0, 4000);
      }
      var parm = new List<SqlParameter>();
      parm.Add(new SqlParameter("@p0", status));
      parm.Add(new SqlParameter("@p1", string.IsNullOrWhiteSpace(errMsg) ? (object)DBNull.Value : errMsg));
      parm.Add(new SqlParameter("@p2", string.IsNullOrWhiteSpace(retrunData) ? (object)DBNull.Value : retrunData));
      parm.Add(new SqlParameter("@p3", Current.Staff));
      parm.Add(new SqlParameter("@p4", Current.StaffName));
      parm.Add(new SqlParameter("@p5", id));
      parm.Add(new SqlParameter("@p6", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });

      var sql = @" UPDATE F0090
                     SET STATUS = @p0,
                         ERRMSG = @p1,
                         RETURN_DATA = @p2,
                         UPD_DATE = @p6,
                         UPD_STAFF = @p3,
                         UPD_NAME = @p4
                   WHERE ID = @p5 ";
      ExecuteSqlCommand(sql, parm.ToArray());
    }

    public void InsertLog(string dcCode, string gupCode, string custCode, string apiName, string sendData, string returnData, string errMsg, string status, DateTime startTime)
    {
      var parm = new List<SqlParameter>();
      parm.Add(new SqlParameter("@p0", dcCode));
      parm.Add(new SqlParameter("@p1", gupCode));
      parm.Add(new SqlParameter("@p2", custCode));
      parm.Add(new SqlParameter("@p3", apiName));
      parm.Add(new SqlParameter("@p4", sendData));
      parm.Add(new SqlParameter("@p5", string.IsNullOrWhiteSpace(returnData) ? (object)DBNull.Value : returnData));
      parm.Add(new SqlParameter("@p6", string.IsNullOrWhiteSpace(errMsg) ? (object)DBNull.Value : errMsg));
      parm.Add(new SqlParameter("@p7", status));
      parm.Add(new SqlParameter("@p8", startTime));
      parm.Add(new SqlParameter("@p9", Current.Staff));
      parm.Add(new SqlParameter("@p10", Current.StaffName));
      parm.Add(new SqlParameter("@p11", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });

      var sql = @" INSERT INTO F0090(DC_CODE,GUP_CODE,CUST_CODE,NAME,SEND_DATA,RETURN_DATA,ERRMSG,STATUS,
																			 CRT_DATE,CRT_STAFF,CRT_NAME,UPD_DATE,UPD_STAFF,UPD_NAME)
											VALUES(@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p9,@p10); ";

      ExecuteSqlCommand(sql, parm.ToArray());
    }

    /// <summary>
    /// 清除log檔紀錄，清除最舊的1000筆
    /// </summary>
    /// <param name="DeleteDate">要清除的日期</param>
    /// <param name="DeleteTable">要清除的Table</param>
    public void RemoveHistroyLog(DateTime DeleteDate, String DeleteTable)
    {
      var DeleteTables = new List<String>()
            {
                "F0090","F0091","F009001","F009002","F009003","F009004","F009005","F009006","F009007","F009008"
            };

      //禁止log用table以外的資料刪除
      if (!DeleteTables.Any(x => x == DeleteTable))
        return;

      var sql = $@"
                WITH T
                    AS (SELECT TOP 1000 * FROM {DeleteTable} WHERE CRT_DATE < @p0 ORDER BY ID)
                DELETE FROM T";
      var para = new List<SqlParameter>()
                    { new SqlParameter("@p0",System.Data.SqlDbType.DateTime2){Value=DeleteDate} };

      ExecuteSqlCommand(sql, para.ToArray());
    }

    /// <summary>
    /// 清除log檔紀錄，清除最舊的1000筆
    /// </summary>
    /// <param name="DeleteDate">要清除的日期</param>
    public void RemoveHistroyLog(DateTime DeleteDate)
    {
      var DeleteTables = new List<String>()
            {
                "F0090","F0091","F009001","F009002","F009003","F009004","F009005","F009006","F009007","F009008"
            };

      foreach (var DeleteTable in DeleteTables)
      {
        var sql = $@"
                    WITH T
                        AS (SELECT TOP 1000 * FROM {DeleteTable} WHERE CRT_DATE < @p0 ORDER BY ID)
                    DELETE FROM T";
        var para = new List<SqlParameter>()
                    { new SqlParameter("@p0",System.Data.SqlDbType.DateTime2){Value=DeleteDate} };

				ExecuteSqlCommand(sql, para.ToArray());
        //切成每個table各自刪除
				if (_wmsTransaction != null)
					_wmsTransaction.Complete();
			}
		}

    /// <summary>
    /// P2116010000查詢排程與API的執行歷程記錄功能
    /// </summary>
    /// <param name="DcCode"></param>
    /// <param name="QueryCount">查詢筆數，參照F000904.VALUE</param>
    /// <param name="IsSortDesc">排序方式，參照F000904.VALUE</param>
    /// <param name="ExternalSystem"></param>
    /// <param name="FunctionName"></param>
    /// <param name="SearchOrdNo"></param>
    /// <param name="ReturnMessage"></param>
    /// <returns></returns>
    public IQueryable<F0090x> GetF0090x(String DcCode, String QueryCount, Boolean IsSortDesc, F0006 FunctionName, String SearchOrdNo, String ReturnMessage, Boolean IsOnlyFailMessage)
    {
      int intQueryCount;
      String SortSQL, SendDataSQL = null, ReturnDataSQL = null, OnlyFailMessageSQL = null, sql = null, APISQL = null;
      var para = new List<SqlParameter>()
      {
        new SqlParameter("@p0", DcCode) { SqlDbType = SqlDbType.VarChar }
      };

      if (FunctionName == null)
        return null;
      if (String.IsNullOrWhiteSpace(FunctionName.PROG_LOG_TABLE) && String.IsNullOrEmpty(FunctionName.API_LOG_TABLE))
        return null;

      if (!int.TryParse(QueryCount, out intQueryCount))
        throw new Exception("無法辨識的QueryCount");
      intQueryCount = (intQueryCount + 1) * 100;

      if (!string.IsNullOrWhiteSpace(SearchOrdNo))
      {
        SendDataSQL = $" AND SEND_DATA LIKE @p{para.Count}";
        para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.NVarChar) { Value = $"%{SearchOrdNo}%" });
      }
      if (!string.IsNullOrWhiteSpace(ReturnMessage))
      {
        ReturnDataSQL = $" AND RETURN_DATA LIKE @p{para.Count}";
        para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.NVarChar) { Value = $"%{ReturnMessage}%" });
      }
      if (IsOnlyFailMessage)
      {
        OnlyFailMessageSQL = $" AND STATUS IN('0','9')";
      }

      if (IsSortDesc)
        SortSQL = " ORDER BY CRT_DATE DESC";
      else
        SortSQL = " ORDER BY CRT_DATE";

      if (!String.IsNullOrWhiteSpace(FunctionName.PROG_LOG_TABLE) && !String.IsNullOrWhiteSpace(FunctionName.PROG_NAME))
      {
        sql = $@"SELECT TOP {intQueryCount} *,'{FunctionName.PROG_LOG_TABLE}' LOG_TABLE FROM {FunctionName.PROG_LOG_TABLE} WHERE DC_CODE IN('0', @p0) AND NAME=@p{para.Count}";
        para.Add(new SqlParameter($"@p{para.Count}", FunctionName.PROG_NAME) { SqlDbType = SqlDbType.VarChar });

        //搜尋第一層LOG資料(PROG_LOG_TABLE)
        sql += SendDataSQL + ReturnDataSQL + OnlyFailMessageSQL + SortSQL;
      }

      //搜尋第二層LOG資料(API_LOG_TABLE)
      if (!string.IsNullOrWhiteSpace(FunctionName.API_LOG_TABLE))
      {
        string objName = "SEND_DATA";
        if (!string.IsNullOrWhiteSpace(FunctionName.FILTER_RULE))
          objName = $"JSON_QUERY(SEND_DATA, '$.{FunctionName.FILTER_RULE}'";

        APISQL = $@"SELECT TOP {intQueryCount} 
														ID,
														NAME,
														ISNULL(CASE WHEN LEN(SEND_DATA) >= 4000 THEN '內容太長不顯示' ELSE {objName}) END, SEND_DATA) SEND_DATA,
														RETURN_DATA,
														STATUS,
														ERRMSG,
														DC_CODE,
														GUP_CODE,
														CUST_CODE,
														CRT_DATE,
														CRT_STAFF,
														CRT_NAME,
														UPD_DATE,
														UPD_STAFF,
														UPD_NAME,
														'{FunctionName.API_LOG_TABLE}' LOG_TABLE FROM {FunctionName.API_LOG_TABLE} WHERE DC_CODE IN('0', @p0) AND NAME=@p{para.Count}";
        para.Add(new SqlParameter($"@p{para.Count}", SqlDbType.VarChar) { Value = FunctionName.API_NAME });

        APISQL += SendDataSQL + ReturnDataSQL + OnlyFailMessageSQL + SortSQL;

        if (!String.IsNullOrWhiteSpace(sql))
          sql = $@"SELECT TOP {intQueryCount} * FROM ({sql} UNION {APISQL}) A {SortSQL}";
        else
          sql = APISQL;
      }

      var result = SqlQuery<F0090x>(sql, para.ToArray()).ToList();

      return result.AsQueryable();
    }


    public IQueryable<string> GetBeginningWithF0090(string tableName)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0",SqlDbType.VarChar){ Value = tableName+"%"}
      };
      var sql = @"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE @p0 ORDER BY TABLE_NAME";

      return SqlQuery<string>(sql, param.ToArray());
    }

    public int GetBeginningWithF0090Count(string tableName)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 }
      };
      var sql = $" SELECT Count(*) FROM {tableName} WHERE ISNULL(STATUS,'') = '' AND CRT_DATE <= DATEADD(minute,-15,@p0)";

      return SqlQuery<int>(sql, param.ToArray()).FirstOrDefault();
    }

    public void UpdateAllBeginningWithF0090(string tableName)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0",SqlDbType.NVarChar){ Value = Current.StaffName},
        new SqlParameter("@p1",SqlDbType.VarChar){ Value = Current.Staff},
        new SqlParameter("@p2",SqlDbType.DateTime2){ Value = DateTime.Now }
      };
      var sql = $@" UPDATE {tableName} SET STATUS = '9', UPD_DATE = @p2, UPD_NAME = @p0, UPD_STAFF = @p1 WHERE ISNULL(STATUS,'') = '' AND CRT_DATE <= DATEADD(minute,-15,@p2)";
      ExecuteSqlCommand(sql, param.ToArray());
    }

		public IQueryable<F009X> GetDelF009XData(DateTime removeDate)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.DateTime){Value = removeDate}
			};
			var sql = @"SELECT A.* FROM (SELECT TOP(1000) 'F0090' TABLE_NAME ,NAME ,SEND_DATA ,RETURN_DATA ,STATUS ,CRT_DATE ,UPD_DATE ,ERRMSG  FROM F0090 WHERE CRT_DATE < @p0 ORDER BY F0090.ID ) A
						UNION ALL
						SELECT B.* FROM (SELECT TOP(1000) 'F0091' TABLE_NAME ,NAME ,SEND_DATA ,RETURN_DATA ,STATUS ,CRT_DATE ,UPD_DATE ,ERRMSG FROM F0091 WHERE CRT_DATE < @p0 ORDER BY F0091.ID ) B
						UNION ALL
						SELECT C.* FROM (SELECT TOP(1000) 'F009001' TABLE_NAME ,NAME ,SEND_DATA ,RETURN_DATA ,STATUS ,CRT_DATE ,UPD_DATE ,ERRMSG FROM F009001 WHERE CRT_DATE < @p0 ORDER BY F009001.ID  ) C
						UNION ALL
						SELECT D.* FROM (SELECT TOP(1000) 'F009002' TABLE_NAME ,NAME ,SEND_DATA ,RETURN_DATA ,STATUS ,CRT_DATE ,UPD_DATE ,ERRMSG FROM F009002 WHERE CRT_DATE < @p0 ORDER BY F009002.ID ) D
						UNION ALL
						SELECT E.* FROM (SELECT TOP(1000) 'F009003' TABLE_NAME ,NAME ,SEND_DATA ,RETURN_DATA ,STATUS ,CRT_DATE ,UPD_DATE ,ERRMSG FROM F009003 WHERE CRT_DATE < @p0 ORDER BY F009003.ID ) E
						UNION ALL
						SELECT F.* FROM (SELECT TOP(1000) 'F009004' TABLE_NAME ,NAME ,SEND_DATA ,RETURN_DATA ,STATUS ,CRT_DATE ,UPD_DATE ,ERRMSG FROM F009004 WHERE CRT_DATE < @p0 ORDER BY F009004.ID ) F
						UNION ALL
						SELECT G.* FROM (SELECT TOP(1000) 'F009005' TABLE_NAME ,NAME ,SEND_DATA ,RETURN_DATA ,STATUS ,CRT_DATE ,UPD_DATE ,ERRMSG FROM F009005 WHERE CRT_DATE < @p0 ORDER BY F009005.ID ) G
						UNION ALL
						SELECT H.* FROM (SELECT TOP(1000) 'F009006' TABLE_NAME ,NAME ,SEND_DATA ,RETURN_DATA ,STATUS ,CRT_DATE ,UPD_DATE ,ERRMSG FROM F009006 WHERE CRT_DATE < @p0 ORDER BY F009006.ID ) H
						UNION ALL
						SELECT I.* FROM (SELECT TOP(1000) 'F009007' TABLE_NAME ,NAME ,SEND_DATA ,RETURN_DATA ,STATUS ,CRT_DATE ,UPD_DATE ,ERRMSG FROM F009007 WHERE CRT_DATE < @p0 ORDER BY F009007.ID) I";

      return SqlQuery<F009X>(sql, param.ToArray());
    }
  }
}

