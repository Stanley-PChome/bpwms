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
			parm.Add(new SqlParameter("@p0", f0701Id));
			parm.Add(new SqlParameter("@p1", dcCode));
			parm.Add(new SqlParameter("@p2", custCode));
			parm.Add(new SqlParameter("@p3", warehouseId));
			parm.Add(new SqlParameter("@p4", string.IsNullOrWhiteSpace(containerCode) ? (object)DBNull.Value : containerCode));
			parm.Add(new SqlParameter("@p5", containerType));
			parm.Add(new SqlParameter("@p6", Current.Staff));
			parm.Add(new SqlParameter("@p7", Current.StaffName));
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
				new SqlParameter("@p0", id)
			};

			var sql = @"DELETE F0701 WHERE ID = @p0 ";
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
			var parms = new object[] { DateTime.Now, Current.Staff,Current.StaffName, id};
			var sql = @" SELECT B.DC_CODE,B.GUP_CODE,B.CUST_CODE,B.WMS_NO PICK_ORD_NO,B.CONTAINER_CODE,
                          C.ITEM_CODE,SUM(C.QTY) B_SET_QTY,0 A_SET_QTY,@p0 CRT_DATE, @p1 CRT_STAFF,@p2 CRT_NAME
										 FROM F0701 A
										 JOIN F070101 B
										   ON B.F0701_ID = A.ID
										 JOIN F070102 C
										   ON C.F070101_ID = B.ID
										WHERE A.ID = @p3
										GROUP BY B.DC_CODE,B.GUP_CODE,B.CUST_CODE,B.WMS_NO,B.CONTAINER_CODE, C.ITEM_CODE";
			return SqlQuery<F05290401>(sql, parms);
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

        public long Insert(string dcCode, string custCode, string warehouseId, string containerCode, string containerType)
        {
            var parm = new List<SqlParameter>();
            parm.Add(new SqlParameter("@p0", dcCode));
            parm.Add(new SqlParameter("@p1", custCode));
            parm.Add(new SqlParameter("@p2", warehouseId));
            parm.Add(new SqlParameter("@p3", containerCode));
            parm.Add(new SqlParameter("@p4", containerType));
            parm.Add(new SqlParameter("@p5", Current.Staff));
            parm.Add(new SqlParameter("@p6", Current.StaffName));
            parm.Add(new SqlParameter("@p7", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });

            var sql = @"DECLARE @a INT;
                        DECLARE @b varchar(20);
                        BEGIN TRAN
                        Select Top 1 @b =UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F0701';
                        INSERT INTO F0701
                        (
                        	DC_CODE,
                        	CUST_CODE,
                        	WAREHOUSE_ID,
                        	CONTAINER_CODE,
                        	CONTAINER_TYPE,
                        	CRT_DATE,
                        	CRT_STAFF,
                        	CRT_NAME) VALUES
                        (
                        @p0,
                        @p1,
                        @p2,
                        @p3,
                        @p4,
                        @p7,
                        @p5,
                        @p6);
                        SELECT @a=CAST(current_value as int)
                        FROM sys.sequences  
                        WHERE name = 'SEQ_F0701_ID' ; 
                        select @a ID
                        COMMIT TRAN;";

            return SqlQuery<long>(sql, parm.ToArray()).Single();
        }
    }
}
