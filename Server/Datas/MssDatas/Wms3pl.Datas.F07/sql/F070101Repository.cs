using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F07
{
	public partial class F070101Repository
	{
        public void InsertF070101(long f070101Id, long f0701Id, string dcCode, string gupCode, string custCode, string containerCode, string wmsNo, string wmsType, string pickOrdNo)
        {
            var parm = new List<SqlParameter>();
						parm.Add(new SqlParameter("@p0", f070101Id) { SqlDbType = SqlDbType.BigInt });
						parm.Add(new SqlParameter("@p1", f0701Id) { SqlDbType = SqlDbType.BigInt });
						parm.Add(new SqlParameter("@p2", dcCode) { SqlDbType = SqlDbType.VarChar });
						parm.Add(new SqlParameter("@p3", string.IsNullOrWhiteSpace(containerCode) ? (object)DBNull.Value : containerCode) { SqlDbType = SqlDbType.VarChar });
						parm.Add(new SqlParameter("@p4", string.IsNullOrWhiteSpace(gupCode) ? (object)DBNull.Value : gupCode) { SqlDbType = SqlDbType.VarChar });
						parm.Add(new SqlParameter("@p5", string.IsNullOrWhiteSpace(custCode) ? (object)DBNull.Value : custCode) { SqlDbType = SqlDbType.VarChar });
						parm.Add(new SqlParameter("@p6", wmsNo) { SqlDbType = SqlDbType.VarChar });
						parm.Add(new SqlParameter("@p7", wmsType) { SqlDbType = SqlDbType.VarChar });
						parm.Add(new SqlParameter("@p8", Current.Staff) { SqlDbType = SqlDbType.VarChar });
						parm.Add(new SqlParameter("@p9", Current.StaffName) { SqlDbType = SqlDbType.NVarChar });
            parm.Add(new SqlParameter("@p10", string.IsNullOrWhiteSpace(pickOrdNo) ? (object)DBNull.Value : pickOrdNo) { SqlDbType = SqlDbType.VarChar});
            parm.Add(new SqlParameter("@p11", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });

            var sql = @"INSERT INTO F070101(
													ID,
													F0701_ID,
													DC_CODE,
													CONTAINER_CODE,
													GUP_CODE,
													CUST_CODE,
													WMS_NO,
													WMS_TYPE,
													CRT_DATE,
													CRT_STAFF,
													CRT_NAME,
                                                    PICK_ORD_NO
													) VALUES (
													@p0,
													@p1,
													@p2,
													@p3,
													@p4,
													@p5,
													@p6,
													@p7,
													@p11,
													@p8,
													@p9,
                                                    @p10
													);";

			ExecuteSqlCommand(sql, parm.ToArray());
		}

		public long GetF070101NextId()
		{
			var sql = @"SELECT NEXT VALUE FOR SEQ_F070101_ID";

			return SqlQuery<long>(sql).Single();
		}

		public IQueryable<F070101> GetCollectionOutboundDatas()
		{
			var parms = new object[] { };

			var sql = $@"SELECT * FROM F070101 A WHERE EXISTS ( 
									  SELECT 1 
									   FROM F051301 B
									  WHERE B.STATUS = '2' --集貨中
									  AND A.WMS_NO = B.WMS_NO 
									  AND A.CUST_CODE = B.CUST_CODE
									  AND A.GUP_CODE = B.GUP_CODE
									)  
									";

			var result = SqlQuery<F070101>(sql, parms.ToArray());
			return result;
		}

		public string LockF070101()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK) Where UPD_LOCK_TABLE_NAME='F070101';";
			return SqlQuery<string>(sql).FirstOrDefault();
		}

		public IQueryable<F070101> GetDatasByWmsOrdNos(string dcCode,string gupCode,string custCode,List<string> wmsOrdNos)
		{
			var parms = new List<object> { dcCode, gupCode, custCode };
			var sql = @" SELECT * 
                     FROM F070101 
                    WHERE DC_CODE =  @p0 
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2 ";
			sql += parms.CombineNotNullOrEmptySqlInParameters(" AND WMS_NO ", wmsOrdNos);
			return SqlQuery<F070101>(sql, parms.ToArray());
		}

		public IQueryable<F070101> GetDatasByContainerCodes(string dcCode, string gupCode, string custCode, List<string> containerCodes,List<string> wmsOrdNos)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
			};
			var sql = @" SELECT * 
                     FROM F070101 
                    WHERE DC_CODE =  @p0 
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2 ";
			sql += parms.CombineSqlInParameters(" AND CONTAINER_CODE ", containerCodes, SqlDbType.VarChar);
			sql += parms.CombineSqlInParameters(" AND WMS_NO ", wmsOrdNos, SqlDbType.VarChar);
			return SqlQuery<F070101>(sql, parms.ToArray());
		}

        public long Insert(long f0701Id, string dcCode, string containerCode, string gupCode, string custCode, string wmsNo, string wmsType)
        {
            var parm = new List<SqlParameter>();
            parm.Add(new SqlParameter("@p0", f0701Id));
            parm.Add(new SqlParameter("@p1", dcCode));
            parm.Add(new SqlParameter("@p2", containerCode));
            parm.Add(new SqlParameter("@p3", gupCode));
            parm.Add(new SqlParameter("@p4", custCode));
            parm.Add(new SqlParameter("@p5", wmsNo));
            parm.Add(new SqlParameter("@p6", wmsType));
            parm.Add(new SqlParameter("@p7", Current.Staff));
            parm.Add(new SqlParameter("@p8", Current.StaffName));
            parm.Add(new SqlParameter("@p9", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });

            var sql = @"DECLARE @a INT;
                        BEGIN TRAN
                        INSERT INTO F070101
                        (
							F0701_ID,
                        	DC_CODE,
                        	CONTAINER_CODE,
                        	GUP_CODE,
                        	CUST_CODE,
                        	WMS_NO,
                        	WMS_TYPE,
                        	CRT_DATE,
                        	CRT_STAFF,
                        	CRT_NAME) VALUES
                        (
                        @p0,
                        @p1,
                        @p2,
                        @p3,
                        @p4,
						            @p5,
                        @p6,
                        @p9,
                        @p7,
                        @p8);
                        SELECT @a=CAST(current_value as int)
                        FROM sys.sequences  
                        WHERE name = 'SEQ_F070101_ID' ; 
                        select @a ID
                        COMMIT TRAN;";

            return SqlQuery<long>(sql, parm.ToArray()).Single();
        }

        public void UpdateWmsNo(long f0701Id, string wmsNo)
        {
            var sql = @" UPDATE F070101 SET WMS_NO = @p0, WMS_TYPE = @p1 WHERE F0701_ID =@p2 ";
            var param = new object[] { wmsNo, wmsNo.Substring(0, 1), f0701Id };
            ExecuteSqlCommand(sql, param);
        }

        public IQueryable<String> GetShipLogisticBox(String dcCode,String gupCode,String custCode,String WmsOrdNo)
        {
            var sql = @"SELECT b.CONTAINER_CODE FROM F070101 a INNER JOIN F0701 b ON a.F0701_ID=b.ID WHERE a.DC_CODE=@p0 AND a.GUP_CODE=@p1 AND a.CUST_CODE=@p2 AND a.WMS_NO=@p3";
            var para = new List<SqlParameter>()
            {
                new SqlParameter("@p0",SqlDbType.VarChar){Value=dcCode},
                new SqlParameter("@p1",SqlDbType.VarChar){Value=gupCode},
                new SqlParameter("@p2",SqlDbType.VarChar){Value=custCode},
                new SqlParameter("@p3",SqlDbType.VarChar){Value=WmsOrdNo}
            };
            return SqlQuery<String>(sql, para.ToArray());
        }

        public IQueryable<F070101> GetData(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var sql = @"SELECT A.*
                        FROM F070101 A
                        JOIN F0701 B ON 
                        A.F0701_ID = B.ID 
                        WHERE A.DC_CODE=@p0 
                        AND A.GUP_CODE=@p1 
                        AND A.CUST_CODE=@p2 
                        AND A.WMS_NO=@p3";
            var parm = new List<SqlParameter>();
            parm.Add(new SqlParameter("@p0", dcCode));
            parm.Add(new SqlParameter("@p1", gupCode));
            parm.Add(new SqlParameter("@p2", custCode));
            parm.Add(new SqlParameter("@p3", wmsOrdNo));
            return SqlQuery<F070101>(sql, parm.ToArray());
        }


		public F070101 GetDatasByWmsNoAndContainerCode(string dcCode,string gupCode,string custCode,string wmsNo, string containerCode)
		{
			var param = new object[] { dcCode, gupCode, custCode, wmsNo, containerCode };
			var sql = @"SELECT * FROM F070101 
						WHERE DC_CODE = @p0
						AND GUP_CODE = @p1
						AND CUST_CODE = @p2
						AND WMS_NO = @p3
						AND CONTAINER_CODE = @p4";
			return SqlQuery<F070101>(sql, param).FirstOrDefault(); 
		}

    public F070101 GetDataWithF0701(string dcCode,string gupCode,string custCode,string containerCode)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3", containerCode) { SqlDbType = SqlDbType.VarChar },
      };
      var sql = @"
SELECT A.* 
FROM F070101 A 
INNER JOIN F0701 B 
	ON A.F0701_ID = B.ID
WHERE 
  A.DC_CODE = @p0 
  AND A.GUP_CODE = @p1
  AND A.CUST_CODE = @p2
  AND B.CONTAINER_CODE = @p3";
      return SqlQuery<F070101>(sql, para.ToArray()).SingleOrDefault();
    }



		public IQueryable<F070101> GetContainerInfoByF0701Id(List<long> f0701_IDs)
		{
			var sqlParameter = new List<SqlParameter>();

			var sql = @"
						SELECT A.F0701_ID, A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.WMS_NO AS PICK_ORD_NO, A.CONTAINER_CODE, B.ITEM_CODE, B.SERIAL_NO, B.QTY

						  FROM F070101 A
						  JOIN F070102 B ON B.F070101_ID = A.ID
						 WHERE 1 = 1
						";
			sql += sqlParameter.CombineSqlInParameters(" AND A.F0701_ID ", f0701_IDs, SqlDbType.BigInt);

			return SqlQuery<F070101>(sql, sqlParameter.ToArray());
		}

		public IQueryable<BindingPickContainerDetail> GetContainerDetailByF0701Id(long f0701_ID)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", f0701_ID) { SqlDbType = SqlDbType.BigInt });

			var sql = @"
						SELECT F070102.ITEM_CODE, 
							   F1903.ITEM_NAME, 
                               F1903.EAN_CODE1, 
                               F1903.EAN_CODE2, 
                               F1903.EAN_CODE3, 
                               F1903.BUNDLE_SERIALNO, 
                               SUM(F070102.QTY) B_SET_QTY, 
                               0 A_SET_QTY 
						  FROM F070101
						  JOIN F070102 ON F070102.F070101_ID = F070101.ID
						  JOIN F1903 ON F1903.GUP_CODE = F070101.GUP_CODE AND F1903.CUST_CODE = F070101.CUST_CODE AND F1903.ITEM_CODE = F070102.ITEM_CODE
						 WHERE F0701_ID = @p0 
						 GROUP BY F070102.ITEM_CODE, 
							   F1903.ITEM_NAME, 
                               F1903.EAN_CODE1, 
                               F1903.EAN_CODE2, 
                               F1903.EAN_CODE3, 
                               F1903.BUNDLE_SERIALNO 
						";

			return SqlQuery<BindingPickContainerDetail>(sql, sqlParameter.ToArray());
		}

    public String GetContainerInCollection(string dcCode, string gupCode, string custCode, string wmsOrdNo, string pickOrdNo)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0",SqlDbType.VarChar) { Value = dcCode },
        new SqlParameter("@p1",SqlDbType.VarChar) { Value = gupCode },
        new SqlParameter("@p2",SqlDbType.VarChar) { Value = custCode },
        new SqlParameter("@p3",SqlDbType.VarChar) { Value = wmsOrdNo },
        new SqlParameter("@p4",SqlDbType.VarChar) { Value = pickOrdNo },
      };
      var sql = @"
SELECT
	TOP 1 A.CONTAINER_CODE
FROM
	F070101 A
INNER JOIN F051402 B ON
	A.DC_CODE = B.DC_CODE 
	AND A.GUP_CODE = B.GUP_CODE 
	AND A.CUST_CODE = B.CUST_CODE 
	AND A.WMS_NO = B.WMS_ORD_NO 
	AND A.CONTAINER_CODE = B.CONTAINER_CODE 
WHERE 
	B.DC_CODE = @p0
	AND B.GUP_CODE = @p1
	AND B.CUST_CODE = @p2
	AND B.WMS_ORD_NO = @p3
  AND A.PICK_ORD_NO = @p4";
      return SqlQuery<string>(sql, para.ToArray()).FirstOrDefault();
    }

  }
}
