using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F0532Repository : RepositoryBase<F0532, Wms3plDbContext, F0532Repository>
	{
    public IQueryable<F0532Ex> GetF0532Ex(string dcCode, string gupCode, string custCode, DateTime startDate, DateTime endDate, string status, string containerSowType, string outContainerCode, string workType)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode)   { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode)   { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode)   { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3", startDate){ SqlDbType = SqlDbType.DateTime2 },
        new SqlParameter("@p4", endDate.AddDays(1))  { SqlDbType = SqlDbType.DateTime2 },
        new SqlParameter("@p5", workType)  { SqlDbType = SqlDbType.VarChar }
      };

      var sql = $@"
                   SELECT 
                     A.F0531_ID,
                     A.DC_CODE,
                     A.CRT_DATE,
                     A.CLOSE_DATE,
                     A.OUT_CONTAINER_CODE,
                     A.SOW_TYPE,
                     D.NAME AS SOW_TYPE_NAME,
                     C.CROSS_NAME AS MOVE_OUT_TARGET_NAME,
                     A.TOTAL AS TOTAL_PCS,
                     B.NAME AS STATUS_NAME,
                     A.STATUS
                   FROM F0532 A
                     LEFT JOIN VW_F000904_LANG B
                       ON B.TOPIC = 'F0532' AND B.SUBTOPIC='STATUS' AND B.VALUE = A.STATUS AND B.LANG = '{Current.Lang}'
                     LEFT JOIN VW_F000904_LANG D
                       ON D.TOPIC = 'F0532' AND D.SUBTOPIC='SOW_TYPE' AND D.VALUE = A.SOW_TYPE AND D.LANG = '{Current.Lang}'
                     LEFT JOIN F0001 C
                       ON A.MOVE_OUT_TARGET = C.CROSS_CODE
                   WHERE 
	                   A.DC_CODE = @p0
                     AND A.GUP_CODE = @p1
                     AND A.CUST_CODE = @p2
	                   AND A.CRT_DATE >= @p3
	                   AND A.CRT_DATE < @p4
                     AND A.WORK_TYPE = @p5
                  ";

      var sql2 = "";
      if (!string.IsNullOrWhiteSpace(status))
      {
        sql2 += $"\r\n	AND A.STATUS = @p{para.Count}";
        para.Add(new SqlParameter($"@p{para.Count}", status) { SqlDbType = SqlDbType.Char });
      }

      if (!string.IsNullOrWhiteSpace(outContainerCode))
      {
        sql2 += $"\r\n	AND A.OUT_CONTAINER_CODE = @p{para.Count}";
        para.Add(new SqlParameter($"@p{para.Count}", outContainerCode) { SqlDbType = SqlDbType.VarChar });
      }

      if (!string.IsNullOrWhiteSpace(containerSowType))
      {
        sql2 += $"\r\n	AND A.SOW_TYPE = @p{para.Count}";
        para.Add(new SqlParameter($"@p{para.Count}", containerSowType) { SqlDbType = SqlDbType.VarChar });
      }

      sql += sql2;
      sql += " ORDER BY A.CRT_DATE DESC;";

      return SqlQuery<F0532Ex>(sql, para.ToArray());
    }

		public F0532 GetDataByF0531Id(long f0531_ID)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", f0531_ID) { SqlDbType = SqlDbType.BigInt });

			var sql = @"SELECT * FROM F0532
						 WHERE F0531_ID = @p0
						   AND STATUS <> '0'
						";

			return SqlQuery<F0532>(sql, sqlParameter.ToArray()).FirstOrDefault();
		}

		public void UpdateTotalAndTargetById(long f0531_ID, int plusQty, string moveOutTarget = "")
		{
			var sqlPlus = string.Empty;
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", plusQty) { SqlDbType = SqlDbType.Int });
			sqlParameter.Add(new SqlParameter("@p1", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p2", Current.Staff) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });
			sqlParameter.Add(new SqlParameter("@p4", f0531_ID) { SqlDbType = SqlDbType.BigInt });
			if (!string.IsNullOrWhiteSpace(moveOutTarget))
			{
				sqlParameter.Add(new SqlParameter("@p5", moveOutTarget) { SqlDbType = SqlDbType.VarChar });
				sqlPlus = ", MOVE_OUT_TARGET = @p5 ";
			}

			var sql = $@" UPDATE F0532
							SET TOTAL = ISNULL(TOTAL, 0) + @p0, UPD_DATE = @p1, UPD_STAFF = @p2, UPD_NAME = @p3 {sqlPlus}
					  WHERE F0531_ID = @p4 ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

    public IQueryable<P0808050000_PrintData> GetPrintData(long f0531ID)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", f0531ID)  { SqlDbType = SqlDbType.BigInt }
      };

      var sql = @"
                  SELECT
	                ROW_NUMBER() OVER (ORDER BY B.ITEM_CODE) AS ROWNUM,
	                C.EAN_CODE1,
	                C.EAN_CODE2,
	                C.EAN_CODE3,
	                C.ITEM_CODE,
	                C.ITEM_NAME,
	                SUM(B.QTY) QTY
                FROM
	                F0532 A
                INNER JOIN F053202 B
	                ON
	                A.F0531_ID = B.F0531_ID
                INNER JOIN F1903 C WITH(NOLOCK)
	                ON B.GUP_CODE = C.GUP_CODE 
	                AND B.CUST_CODE = C.CUST_CODE 
	                AND B.ITEM_CODE = C.ITEM_CODE 
                WHERE 
                  A.F0531_ID = @p0
                GROUP BY 
                  B.ITEM_CODE, C.EAN_CODE1, C.EAN_CODE2, C.EAN_CODE3, C.ITEM_CODE, C.ITEM_NAME
                ";

      return SqlQuery<P0808050000_PrintData>(sql, para.ToArray());
    }

    public IQueryable<P0808050000_CancelPrintData> GetCancelPrintData(long f0531ID)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", f0531ID)  { SqlDbType = SqlDbType.BigInt }
      };

      var sql = @"
                  SELECT
	                  ROW_NUMBER() OVER (ORDER BY B.ITEM_CODE) AS ROWNUM,
	                  C.EAN_CODE1,
	                  C.EAN_CODE2,
	                  C.EAN_CODE3,
	                  C.ITEM_CODE,
	                  C.ITEM_NAME,
	                  SUM(B.QTY) QTY,
					          D.ORD_NO
                  FROM F0532 A
                    INNER JOIN F053202 B
	                    ON A.F0531_ID = B.F0531_ID
                    INNER JOIN F05030202 D WITH(NOLOCK)
	                    ON B.WMS_ORD_NO = D.WMS_ORD_NO
					            AND B.WMS_ORD_SEQ=D.WMS_ORD_SEQ
                    INNER JOIN F1903 C WITH(NOLOCK)
	                    ON B.GUP_CODE = C.GUP_CODE 
	                    AND B.CUST_CODE = C.CUST_CODE 
	                    AND B.ITEM_CODE = C.ITEM_CODE 
                  WHERE 
                    A.F0531_ID = @p0
                  GROUP BY 
                    B.ITEM_CODE, C.EAN_CODE1, C.EAN_CODE2, C.EAN_CODE3, C.ITEM_CODE, C.ITEM_NAME, D.ORD_NO
                  ";

      return SqlQuery<P0808050000_CancelPrintData>(sql, para.ToArray());
    }


    public void UpdateCloseInfoByF0531Id(long f0531_ID)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p1", Current.Staff) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p2", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });
			sqlParameter.Add(new SqlParameter("@p3", f0531_ID) { SqlDbType = SqlDbType.BigInt });

			var sql = @" UPDATE F0532
							SET STATUS = '1', UPD_DATE = @p0, UPD_STAFF = @p1, UPD_NAME = @p2, CLOSE_DATE = @p0, CLOSE_STAFF = @p1, CLOSE_NAME = @p2
					  WHERE F0531_ID = @p3 ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

		public F0532 GetCloseContainer(string dcCode, string containerCode)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", containerCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", dcCode) { SqlDbType = SqlDbType.VarChar });

			var sql = @"SELECT TOP(1) * FROM F0532
						 WHERE OUT_CONTAINER_CODE = @p0
						   AND DC_CODE = @p1
						   AND STATUS = '1'
						";

			var result = SqlQuery<F0532>(sql, sqlParameter.ToArray()).FirstOrDefault();
			return result;
		}

		public F0532 GetCloseContainer(string dcCode, string containerCode, string sowType)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", containerCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", dcCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p2", sowType) { SqlDbType = SqlDbType.Char });

			var sql = @"SELECT TOP(1) * FROM F0532
						 WHERE OUT_CONTAINER_CODE = @p0
						   AND DC_CODE = @p1
						   AND SOW_TYPE = @p2
						   AND STATUS = '1'
						";

			var result = SqlQuery<F0532>(sql, sqlParameter.ToArray()).FirstOrDefault();
			return result;
		}

		public IQueryable<long> GetCloseDebitContainerIds()
		{
			var sql = @"SELECT TOP(500) F0532.ID
                          FROM F0532
                          JOIN F053201 ON F0532.F0531_ID = F053201.F0531_ID
                          JOIN F0535 ON F0535.DC_CODE = F053201.DC_CODE
                                    AND F0535.GUP_CODE = F053201.GUP_CODE
                                    AND F0535.CUST_CODE = F053201.CUST_CODE
                                    AND F0535.PICK_ORD_NO = F053201.PICK_ORD_NO
						 WHERE F0532.STATUS = '1'
						 GROUP BY F0532.ID
						HAVING MIN(F0535.STATUS) IN('2', '9')
						";

			return SqlQuery<long>(sql);
		}

		public void UpdateStatusByIds(List<long> ids, string status)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", status) { SqlDbType = SqlDbType.Char });
			sqlParameter.Add(new SqlParameter("@p1", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p2", Current.Staff) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });

			var sql = @" UPDATE F0532
							SET STATUS = @p0, UPD_DATE = @p1, UPD_STAFF = @p2, UPD_NAME = @p3
					  WHERE 1 = 1 ";
			sql += sqlParameter.CombineSqlInParameters(" AND ID ", ids, SqlDbType.BigInt);

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

		public int GetNewContainerSeq(string sowType)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", DateTime.Today) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p1", DateTime.Today.AddDays(1)) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p2", sowType) { SqlDbType = SqlDbType.VarChar });

			var sql = @" SELECT MAX(OUT_CONTAINER_SEQ) + 1
						   FROM F0532
						  WHERE CRT_DATE >= @p0
							AND CRT_DATE < @p1
							AND SOW_TYPE = @p2 ";

			return SqlQuery<int>(sql, sqlParameter.ToArray()).FirstOrDefault();
		}

		public F0532 GetDataByF0701Id(long f0701_Id)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", f0701_Id) { SqlDbType = SqlDbType.BigInt });

			var sql = @"SELECT TOP(1) * FROM F0532
						 WHERE F0701_ID = @p0
						";

			return SqlQuery<F0532>(sql, sqlParameter.ToArray()).FirstOrDefault();
		}
		public F0532 GetCloseOrShipDataByF0531Id(long f0531_ID)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", f0531_ID) { SqlDbType = SqlDbType.BigInt });

			var sql = @"SELECT TOP(1) * FROM F0532
						 WHERE F0531_ID = @p0
						   AND STATUS IN ('1', '2')
						";

			return SqlQuery<F0532>(sql, sqlParameter.ToArray()).FirstOrDefault();
		}

		public void UpdateContainerCodeByF0531Id(long f0531_ID, string newContainerCode)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", newContainerCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p2", Current.Staff) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });
			sqlParameter.Add(new SqlParameter("@p4", f0531_ID) { SqlDbType = SqlDbType.BigInt });

			var sql = $@" UPDATE F0532
							SET OUT_CONTAINER_CODE = @p0, UPD_DATE = @p1, UPD_STAFF = @p2, UPD_NAME = @p3
					  WHERE F0531_ID = @p4 ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

		public bool CheckSnInCancelBoxData(string gupCode, string custCode, string serialNo)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", gupCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", custCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p2", serialNo) { SqlDbType = SqlDbType.VarChar });

			var sql = @"
                  SELECT TOP(1) 1
                    FROM F0532
                    JOIN F053202
                      ON F0532.F0531_ID = F053202.F0531_ID
                   WHERE F0532.STATUS IN ('0', '1')
                     AND F0532.SOW_TYPE = '1'
                     AND F053202.GUP_CODE = @p0
                     AND F053202.CUST_CODE = @p1
                     AND F053202.SERIAL_NO = @p2
                  ";

			return SqlQuery<int>(sql, sqlParameter.ToArray()).Any();
		}
	}
}
