using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F0701Repository
	{
		public void InsertF0701(long f0701Id, string dcCode, string custCode, string warehouseId, string containerCode, string containerType)
		{
			var parm = new List<SqlParameter>();
			parm.Add(new SqlParameter("@p0", f0701Id) {SqlDbType= SqlDbType.BigInt});
			parm.Add(new SqlParameter("@p1", dcCode) { SqlDbType = SqlDbType.VarChar });
			parm.Add(new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar });
			parm.Add(new SqlParameter("@p3", warehouseId) { SqlDbType = SqlDbType.VarChar });
			parm.Add(new SqlParameter("@p4", string.IsNullOrWhiteSpace(containerCode) ? (object)DBNull.Value : containerCode) {  SqlDbType = SqlDbType.VarChar});
			parm.Add(new SqlParameter("@p5", containerType) { SqlDbType = SqlDbType.VarChar});
			parm.Add(new SqlParameter("@p6", Current.Staff) { SqlDbType = SqlDbType.VarChar });
			parm.Add(new SqlParameter("@p7", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });
      parm.Add(new SqlParameter("@p8", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });

      var sql = @"INSERT INTO F0701(
													ID,
													DC_CODE,
													CUST_CODE,
													WAREHOUSE_ID,
													CONTAINER_CODE,
													CONTAINER_TYPE,
													CRT_DATE,
													CRT_STAFF,
													CRT_NAME
													) VALUES (
													@p0,
													@p1,
													@p2,
													@p3,
													@p4,
													@p5,
													@p8,
													@p6,
													@p7
													);";
			ExecuteSqlCommand(sql, parm.ToArray());
		}

		public long GetF0701NextId()
		{
			var sql = @"SELECT NEXT VALUE FOR SEQ_F0701_ID";

			return SqlQuery<long>(sql).Single();
		}

		public void DeleteF0701(long id)
		{
			var parm = new List<SqlParameter>
			{
				new SqlParameter("@p0", id) { SqlDbType = SqlDbType.BigInt }
			};

			var sql = @"DELETE F0701 WHERE ID = @p0 ";
			ExecuteSqlCommand(sql, parm.ToArray());
		}

    public void DeleteF0701(string dcCode, string gupCode, string custCode, string wmsNo, long? f0701Id)
    {
      var parm = new List<SqlParameter>
      {
        new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcCode },
        new SqlParameter("@p1", SqlDbType.VarChar) { Value = gupCode },
        new SqlParameter("@p2", SqlDbType.VarChar) { Value = custCode },
        new SqlParameter("@p3", SqlDbType.VarChar) { Value = wmsNo }
      };

      var sql = @"DELETE F0701
                  WHERE 
                    ID IN(
                      SELECT 
                        F0701_ID 
                      FROM F070101 A
                      WHERE 
                        A.DC_CODE = @p0
                        AND A.GUP_CODE = @p1
                        AND A.CUST_CODE = @p2
                        AND A.WMS_NO =  @p3
                        {0})
                  ";

      var sql2 = "";

      if(!string.IsNullOrWhiteSpace(f0701Id.ToString()))
      {
        sql2 += $" AND A.F0701_ID = @p{parm.Count}";
        parm.Add(new SqlParameter($"@p{parm.Count}", SqlDbType.BigInt) { Value = f0701Id });
      }

      sql = string.Format(sql, sql2);

      ExecuteSqlCommand(sql, parm.ToArray());
    }

    public void DeleteF0701ByIds(List<long> ids)
		{
			var parameters = new List<object>{};
			var sql = $"DELETE F0701 WHERE {parameters.CombineSqlInParameters(" ID ", ids)}";
			ExecuteSqlCommand(sql, parameters.ToArray());
		}

		public void DeleteF0701ByContainerCodes(string dcCode, string custCode, string warehouseId, string containerType, List<string> containerCodes)
		{
			var parameters = new List<object>
						{
								dcCode,
								custCode,
								warehouseId,
								containerType
						};

			var sql = @"DELETE F0701 
									WHERE DC_CODE = @p0
									AND CUST_CODE = @p1
									AND WAREHOUSE_ID = @p2
									AND CONTAINER_TYPE = @p3 ";

			sql += parameters.CombineSqlInParameters(" AND CONTAINER_CODE ", containerCodes);
			ExecuteSqlCommand(sql, parameters.ToArray());
		}

		public IQueryable<F0701> GetCollectionOutboundDatas()
		{
			var parms = new object[] { };

			var sql = $@"SELECT * FROM F0701 WHERE ID IN (
									SELECT F0701_ID FROM F070101 A WHERE EXISTS ( 
									  SELECT 1 
									   FROM F051301 B
									  WHERE B.STATUS = '2' --集貨中
									  AND A.WMS_NO = B.WMS_NO 
									  AND A.CUST_CODE = B.CUST_CODE
									  AND A.GUP_CODE = B.GUP_CODE
									) 
									)
									";

			var result = SqlQuery<F0701>(sql, parms.ToArray());
			return result;
		}

		public IQueryable<F0701> CheckDcContainerIsUsed(string dcCode, string containerCode)
		{
			var parms = new object[] { dcCode, containerCode };
			var sql = @" SELECT *
                     FROM F0701
                    WHERE DC_CODE = @p0
                      AND CONTAINER_CODE = @p1";
			return SqlQuery<F0701>(sql, parms.ToArray());
		}

		public IQueryable<F0701> GetDatasByContainers(string dcCode,List<string> containerCodes)
		{
			var parms = new List<object> { dcCode, "0" };
			var sql = @" SELECT *
                     FROM F0701
                    WHERE DC_CODE = @p0
                      AND CONTAINER_TYPE = @p1 ";

			sql += parms.CombineNotNullOrEmptySqlInParameters("AND CONTAINER_CODE", containerCodes);
			return SqlQuery<F0701>(sql, parms.ToArray());
		}

		public string LockF0701()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F0701';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}

		public IQueryable<F0701> GetDatasByF0701Ids(string dcCode,string gupCode,string custCode, List<long> f0701Ids)
		{
			var parms = new List<object>();
			var sql = @" SELECT * 
                     FROM F0701
                    WHERE 1=1";
			if (f0701Ids.Count > 0)
			{
				sql += parms.CombineNotNullOrEmptySqlInParameters("AND ID ", f0701Ids);
			}
			
			return SqlQuery<F0701>(sql, parms.ToArray());
		}

		public void DeleteByContainerCode(string dcCode,string custCode,string containerCode)
		{
			var parms = new List<object> { dcCode, custCode, containerCode };
			var sql = @" DELETE FROM F0701 WHERE DC_CODE = @p0 AND CUST_CODE = @p1 AND CONTAINER_CODE = @p2";
			ExecuteSqlCommand(sql, parms.ToArray());
		}

		public ContainerPickInfo GetContainerPickInfo(string dcCode,string containerCode)
		{
			var parms = new object[] { dcCode, containerCode };
			var sql = @" SELECT A.ID Id,C.PICK_ORD_NO PickOrdNo,D.DELV_DATE DelvDate,D.PICK_TIME PickTime, 
													D.ATFL_N_PICK_CNT+D.ATFL_B_PICK_CNT+D.ATFL_S_PICK_CNT+
													D.ATFL_NP_PICK_CNT+D.ATFL_BP_PICK_CNT+D.ATFL_SP_PICK_CNT+
													D.AUTO_N_PICK_CNT+D.AUTO_S_PICK_CNT+D.REPICK_CNT  BatchPickCnt,
													D.PICK_CNT BatchPickQty,
													E.B_PICK_QTY PickQty,
                          C.MOVE_OUT_TARGET MoveOutTarget,
													G.CROSS_NAME MoveOutTargetName,
                          ISNULL(F.CancelOrderCnt,0) CancelOrderCnt,
                          H.AllOrderCnt - ISNULL(F.CancelOrderCnt,0) NormalOrderCnt
										 FROM F0701 A
										 JOIN F070101 B
										   ON B.F0701_ID = A.ID
										 JOIN F051201 C
										   ON C.DC_CODE = B.DC_CODE
										  AND C.GUP_CODE = B.GUP_CODE
										  AND C.CUST_CODE = B.CUST_CODE
										  AND C.PICK_ORD_NO = B.WMS_NO
										JOIN F0513 D
										  ON D.DC_CODE = C.DC_CODE
										 AND D.GUP_CODE = C.GUP_CODE
										 AND D.CUST_CODE = C.CUST_CODE
										 AND D.DELV_DATE = C.DELV_DATE
										 AND D.PICK_TIME = C.PICK_TIME
										JOIN (
											SELECT DC_CODE,GUP_CODE,CUST_CODE,PICK_ORD_NO,SUM(B_PICK_QTY) B_PICK_QTY
											FROM F051202
                      WHERE PICK_STATUS <> '9'
											GROUP BY DC_CODE,GUP_CODE,CUST_CODE,PICK_ORD_NO
										) E
										  ON E.DC_CODE = C.DC_CODE
										 AND E.GUP_CODE = C.GUP_CODE
										 AND E.CUST_CODE = C.CUST_CODE
										 AND E.PICK_ORD_NO = C.PICK_ORD_NO
										 LEFT JOIN (SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.PICK_ORD_NO,COUNT(DISTINCT A.WMS_ORD_NO) CancelOrderCnt
														FROM F051202 A
														JOIN F050801 B
													  	ON B.DC_CODE = A.DC_CODE
														 AND B.GUP_CODE = A.GUP_CODE
													 	 AND B.CUST_CODE = A.CUST_CODE
														 AND B.WMS_ORD_NO = A.WMS_ORD_NO
														WHERE B.STATUS ='9' AND A.PICK_STATUS <>'9'
														GROUP BY  A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.PICK_ORD_NO) F
                       ON F.DC_CODE = C.DC_CODE
                      AND F.GUP_CODE = C.GUP_CODE
                      AND F.CUST_CODE = C.CUST_CODE
                      AND F.PICK_ORD_NO = C.PICK_ORD_NO
										 LEFT JOIN F0001 G
                       ON G.CROSS_CODE = C.MOVE_OUT_TARGET
                     JOIN (SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.PICK_ORD_NO,COUNT(DISTINCT A.WMS_ORD_NO) AllOrderCnt
														FROM F051202 A
														JOIN F050801 B
													  	ON B.DC_CODE = A.DC_CODE
														 AND B.GUP_CODE = A.GUP_CODE
													 	 AND B.CUST_CODE = A.CUST_CODE
														 AND B.WMS_ORD_NO = A.WMS_ORD_NO
                            WHERE A.PICK_STATUS <>'9'
														GROUP BY  A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.PICK_ORD_NO) H
                       ON H.DC_CODE = C.DC_CODE
                      AND H.GUP_CODE = C.GUP_CODE
                      AND H.CUST_CODE = C.CUST_CODE
                      AND H.PICK_ORD_NO = C.PICK_ORD_NO
									 WHERE A.DC_CODE = @p0
										AND A.CONTAINER_CODE = @p1
										AND C.NEXT_STEP = '6'
										ORDER BY A.ID DESC ";

			return SqlQuery<ContainerPickInfo>(sql, parms).FirstOrDefault();
		}

		public IQueryable<F05290401> GetF05290401sByContainerId(Int64 id)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.DateTime2){ Value = DateTime.Now },
				new SqlParameter("@p1",SqlDbType.VarChar){ Value = Current.Staff},
				new SqlParameter("@p2",SqlDbType.NVarChar){ Value = Current.StaffName},
				new SqlParameter("@p3",SqlDbType.BigInt){ Value = id}
			};
			var sql = @" SELECT B.DC_CODE,B.GUP_CODE,B.CUST_CODE,B.WMS_NO PICK_ORD_NO,B.CONTAINER_CODE,
                          C.ITEM_CODE,SUM(C.QTY) B_SET_QTY,0 A_SET_QTY,@p0 CRT_DATE, @p1 CRT_STAFF,@p2 CRT_NAME
										 FROM F0701 A
										 JOIN F070101 B
										   ON B.F0701_ID = A.ID
										 JOIN F070102 C
										   ON C.F070101_ID = B.ID
										WHERE A.ID = @p3
										GROUP BY B.DC_CODE,B.GUP_CODE,B.CUST_CODE,B.WMS_NO,B.CONTAINER_CODE, C.ITEM_CODE";
			return SqlQueryWithSqlParameterSetDbType<F05290401>(sql, param.ToArray());
		}

		public int GetPickContainerCnt(string dcCode,string gupCode,string custCode,string pickOrdNo)
		{
			var parms = new object[] { dcCode, gupCode, custCode, pickOrdNo };
			var sql = @" SELECT COUNT(*)
                      FROM F0701 A
                      JOIN F070101 B
                        ON B.F0701_ID = A.ID
                     WHERE B.DC_CODE = @p0
                       AND B.GUP_CODE = @p1
                       AND B.CUST_CODE = @p2
                       AND B.WMS_NO = @p3 ";
			return SqlQuery<int>(sql, parms).FirstOrDefault();
		}

        public F0701 GetDataByContainerCode(string containerType, string containerCodes)
        {
            var parms = new List<SqlParameter>
            {
                new SqlParameter("@p0", containerCodes),
            };

            var insql = string.Empty;
            if (!string.IsNullOrWhiteSpace(containerType))
            {
                insql += " AND CONTAINER_TYPE = @p1 ";
                parms.Add(new SqlParameter("@p1", containerType));
            }

            var sql = $@" SELECT * FROM F0701 
                         WHERE UPPER(CONTAINER_CODE) = @p0
                         {insql} ";
            
            return SqlQuery<F0701>(sql, parms.ToArray()).FirstOrDefault();
        }

		public IQueryable<PickContainerInfo> GetPickContainerInfo(string dcCode, string containerCode)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", Current.Lang) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", dcCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p2", containerCode) { SqlDbType = SqlDbType.VarChar });

			var sql = @"
SELECT
	F0534.F0701_ID,
	F0534.DC_CODE,
	F0534.GUP_CODE,
	F0534.CUST_CODE,
	F0534.PICK_ORD_NO, 
	F0534.CONTAINER_CODE, 
	F0534.DEVICE_TYPE,
	VW_F000904_LANG.NAME DEVICE_TYPE_NAME, 
	F0534.MOVE_OUT_TARGET,
	F0001.CROSS_NAME, 
	F0534.TOTAL
FROM F0701
JOIN F0534 ON F0534.F0701_ID = F0701.ID
LEFT JOIN VW_F000904_LANG ON VW_F000904_LANG.TOPIC = 'F1980'
							AND VW_F000904_LANG.SUBTOPIC = 'DEVICE_TYPE'
							AND VW_F000904_LANG.VALUE = F0534.DEVICE_TYPE
							AND VW_F000904_LANG.LANG = @p0
LEFT JOIN F0001 ON F0001.CROSS_CODE = F0534.MOVE_OUT_TARGET
WHERE F0701.DC_CODE = @p1
	AND F0701.CONTAINER_CODE = @p2
	AND F0701.CONTAINER_TYPE = '0'
ORDER BY
	F0534.PICK_ORD_NO ASC
";

			return SqlQuery<PickContainerInfo>(sql, sqlParameter.ToArray());
		}

		public void DeleteByF0531Id(long f0531_ID)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", f0531_ID) { SqlDbType = SqlDbType.BigInt });

			var sql = @" DELETE F0701
					  WHERE ID IN (SELECT F0701_ID FROM F053201 WHERE F0531_ID = @p0) ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

		public IQueryable<F0701> GetDatasByF0701Ids(List<long> ids)
		{
			var sqlParameter = new List<SqlParameter>();

			var sql = @"SELECT * FROM F0701
						 WHERE 1 = 1
						";

			sql += sqlParameter.CombineSqlInParameters(" AND ID ", ids, SqlDbType.BigInt);

			return SqlQuery<F0701>(sql, sqlParameter.ToArray());
		}

		public BindingPickContainerInfo GetBindingPickContainerInfo(string dcCode, string gupCode, string custCode, string containerCode)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", containerCode) { SqlDbType = SqlDbType.VarChar });

			var sql = @"
SELECT TOP(1)
	F0534.ID AS F0534_ID,
	F0534.F0701_ID,
	F0534.CONTAINER_CODE,
	F0534.DEVICE_TYPE,
	F0534.TOTAL,
	F0534.DC_CODE,
	F0534.GUP_CODE,
	F0534.CUST_CODE,
	F0534.PICK_ORD_NO,
	F0534.MOVE_OUT_TARGET,
	F0001.CROSS_NAME,
	'0' AS HAS_CP_ITEM
FROM
	F0701
JOIN F0534 ON F0534.F0701_ID = F0701.ID
LEFT JOIN F0001 ON F0001.CROSS_CODE = F0534.MOVE_OUT_TARGET
WHERE
	F0534.DC_CODE = @p0
	AND F0534.GUP_CODE = @p1
	AND F0534.CUST_CODE = @p2
	AND F0534.CONTAINER_CODE = @p3
ORDER BY
	F0534.PICK_ORD_NO
";

			return SqlQuery<BindingPickContainerInfo>(sql, sqlParameter.ToArray()).FirstOrDefault();
		}

		public void DeleteByF0532Id(List<long> f0532_ID)
		{
			var sqlParameter = new List<SqlParameter>();
			var sqlCondition = sqlParameter.CombineSqlInParameters(" AND  ID ", f0532_ID, SqlDbType.BigInt);

			var sql = $@" DELETE F0701
					  WHERE ID IN (SELECT F0701_ID FROM F0532
									WHERE  F0701_ID IS NOT NULL 
                  {sqlCondition}
								  ) ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

		public F0701 GetDataByTypeAndContainer(string dcCode, string containerType, string containerCode)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", containerCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p2", containerType) { SqlDbType = SqlDbType.Char });

			var sql = @" SELECT *
						   FROM F0701
						  WHERE DC_CODE = @p0
							AND CONTAINER_CODE = @p1
							AND CONTAINER_TYPE = @p2 ";

			return SqlQuery<F0701>(sql, sqlParameter.ToArray()).FirstOrDefault();
		}

		public void DeleteById(long ids)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", ids) { SqlDbType = SqlDbType.BigInt });

			var sql = $"DELETE F0701 WHERE ID = @p0 ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

		public int GetContainerCntExceptId(string dcCode, string gupCode, string custCode, string pickOrdNo, long exceptF0701Id)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", pickOrdNo) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p4", exceptF0701Id) { SqlDbType = SqlDbType.BigInt });

			var sql = @" SELECT COUNT(*)
                      FROM F0701 A
                      JOIN F070101 B
                        ON B.F0701_ID = A.ID
                     WHERE B.DC_CODE = @p0
                       AND B.GUP_CODE = @p1
                       AND B.CUST_CODE = @p2
                       AND B.PICK_ORD_NO = @p3
                       AND B.F0701_ID <> @p4 ";
			return SqlQuery<int>(sql, sqlParameter.ToArray()).FirstOrDefault();
		}

		public void UpdateContainerCodeById(long id, string newContainerCode)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", newContainerCode) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });
			sqlParameter.Add(new SqlParameter("@p2", Current.Staff) { SqlDbType = SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p3", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });
			sqlParameter.Add(new SqlParameter("@p4", id) { SqlDbType = SqlDbType.BigInt });

			var sql = $@" UPDATE F0701
							SET CONTAINER_CODE = @p0, UPD_DATE = @p1, UPD_STAFF = @p2, UPD_NAME = @p3
					  WHERE ID = @p4 ";

			ExecuteSqlCommand(sql, sqlParameter.ToArray());
		}

    public IQueryable<MoveOutPickOrders> GetMoveOutPickOrders(string dcCode, string gupCode, string custCode, DateTime startDate, DateTime endDate, string moveOutTarget, string pickContainerCode)
    {
      var param = new List<SqlParameter>();

      param.Add(new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar });
      param.Add(new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar });
      param.Add(new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar });
      param.Add(new SqlParameter("@p3", startDate) { SqlDbType = SqlDbType.DateTime2 });
      param.Add(new SqlParameter("@p4", endDate.AddDays(1)) { SqlDbType = SqlDbType.DateTime2 });

      var sql = $@"
                SELECT 
                  C.DELV_DATE,
                  C.PICK_TIME,
                  C.PICK_ORD_NO,
                  D.CROSS_NAME 'MOVE_OUT_TARGET_NAME',
                  E.NAME 'PICK_STATUS_NAME',
                  F.NAME 'PICK_TOOL_NAME',
                  C.PK_AREA_NAME
                FROM F0701 A
                  JOIN F070101 B 
                    ON A.ID=B.F0701_ID
                  JOIN F051201 C 
                    ON B.DC_CODE=C.DC_CODE AND B.GUP_CODE=C.GUP_CODE AND B.CUST_CODE=C.CUST_CODE AND B.PICK_ORD_NO=C.PICK_ORD_NO
                  JOIN F0001 D 
                    ON C.MOVE_OUT_TARGET=D.CROSS_CODE
                  JOIN VW_F000904_LANG E 
                    ON E.TOPIC='F051201' AND E.SUBTOPIC='PICK_STATUS' AND E.LANG='{Current.Lang}' AND E.VALUE=C.PICK_STATUS
                  JOIN VW_F000904_LANG F 
                    ON F.TOPIC='F051201' AND F.SUBTOPIC='PICK_TOOL' AND F.LANG='{Current.Lang}' AND F.VALUE=C.PICK_TOOL
                WHERE C.DC_CODE = @p0
                  AND C.GUP_CODE = @p1
                  AND C.CUST_CODE = @p2
                  AND C.NEXT_STEP = '6'
                  AND C.DELV_DATE >= @p3
                  AND C.DELV_DATE < @p4
                ";

      var sql2 = "";

      if (!string.IsNullOrWhiteSpace(moveOutTarget))
      {
        sql2 += $" AND C.MOVE_OUT_TARGET = @p{param.Count}";
        param.Add(new SqlParameter($"@p{param.Count}", moveOutTarget) { SqlDbType = SqlDbType.VarChar });
      }

      if (!string.IsNullOrWhiteSpace(pickContainerCode))
      {
        sql2 += $" AND A.CONTAINER_CODE = @p{param.Count}";
        param.Add(new SqlParameter($"@p{param.Count}", pickContainerCode) { SqlDbType = SqlDbType.VarChar });
      }

      sql += sql2;
      sql += " ORDER BY C.DELV_DATE DESC, C.PICK_TIME DESC";

      return SqlQuery<MoveOutPickOrders>(sql, param.ToArray());
    }
		public F0701 GetData(string containerCode)
		{
			var para = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.VarChar){ Value = containerCode }
			};

			var sql = @"SELECT TOP 1 * FROM F0701 WHERE CONTAINER_CODE=@p0";

			return SqlQuery<F0701>(sql, para.ToArray()).FirstOrDefault();
		}
	}
}
