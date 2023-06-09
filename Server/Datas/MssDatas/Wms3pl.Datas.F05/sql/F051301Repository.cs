using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F051301Repository : RepositoryBase<F051301, Wms3plDbContext, F051301Repository>
	{
		public IQueryable<F051301> GetF051301s(string dcCode,string gupCode,string custCode,DateTime delvDate,string pickTime,List<string> wmsOrdNos)
		{
			var parms = new List<object> { dcCode, gupCode, custCode, delvDate, pickTime };
			var sql = @" SELECT *
                     FROM F051301
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND DELV_DATE = @p3
                      AND PICK_TIME = @p4 ";
			 if(wmsOrdNos.Any())
				sql += parms.CombineNotNullOrEmptySqlInParameters("AND WMS_NO", wmsOrdNos);

			return SqlQuery<F051301>(sql, parms.ToArray());
		}

        public F051301 GetF051301(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string wmsNo)
        {
            var parms = new List<object> { dcCode, gupCode, custCode, delvDate, pickTime, wmsNo };
            var sql = @" SELECT *
                     FROM F051301
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND DELV_DATE = @p3
                      AND PICK_TIME = @p4
                      AND WMS_NO = @p5 ";

            return SqlQuery<F051301>(sql, parms.ToArray()).FirstOrDefault();
        }

        public void DeleteF051301(string gupCode, string custCode, string wmsOrdNo)
		{
			var parm = new List<SqlParameter>
			{
				new SqlParameter("@p0", gupCode),
				new SqlParameter("@p1", custCode),
				new SqlParameter("@p2", wmsOrdNo)
			};

			var sql = @"DELETE F051301 
									WHERE GUP_CODE = @p0 
									AND CUST_CODE = @p1 
									AND WMS_NO = @p2 ";

			ExecuteSqlCommand(sql, parm.ToArray());
		}

		public void UpdateNextStepByWmsOrdNos(string dcCode, string gupCode, string custCode, string nextStep, List<string> wmsOrdNos)
		{
			var parms = new List<object> { nextStep, DateTime.Now, Current.Staff, Current.StaffName, dcCode, gupCode, custCode };
			var sql = @" UPDATE F051301 SET NEXT_STEP = @p0,UPD_DATE = @p1 ,UPD_STAFF = @p2, UPD_NAME = @p3
                   WHERE DC_CODE = @p4 
                     AND GUP_CODE = @p5
                     AND CUST_CODE = @p6";
			sql += parms.CombineNotNullOrEmptySqlInParameters("AND WMS_NO", wmsOrdNos);
			ExecuteSqlCommand(sql, parms.ToArray());
		}

		public IQueryable<F051301> GetDatasByWmsOrdNos(string dcCode,string gupCode,string custCode,List<string> wmsOrdNos)
		{
			var parms = new List<object> { dcCode, gupCode, custCode};
			var sql = @" SELECT *
                     FROM F051301
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2 ";
			if (wmsOrdNos.Any())
				sql += parms.CombineNotNullOrEmptySqlInParameters("AND WMS_NO", wmsOrdNos);

			return SqlQuery<F051301>(sql, parms.ToArray());
		}

		public NextStepModel GetCollectionNameByWmsNo(string dcCode, string gupCode, string custCode, string wmsNo)
		{
			var parms = new List<object> { wmsNo, dcCode, gupCode, custCode };
			var sql = @" SELECT a.COLLECTION_NAME, c.NAME NEXT_STEP_NAME, b.NEXT_STEP FROM 
										F1945 a, F051301 b, F000904 c
										WHERE b.WMS_NO = @p0
										AND a.COLLECTION_CODE = b.COLLECTION_CODE 
										AND a.CELL_TYPE = b.CELL_TYPE
										AND b.NEXT_STEP = c.VALUE
										AND c.TOPIC = 'F051301' 
										AND c.SUBTOPIC = 'NEXT_STEP' 
										AND b.DC_CODE = @p1 
										AND b.GUP_CODE = @p2 
										AND b.CUST_CODE = @p3
									";

			return SqlQuery<NextStepModel>(sql, parms.ToArray()).FirstOrDefault();
		}

        public IQueryable<WcsOutboundCollectionData> GetWcsOutboundCollectionData(string dcCode, string gupCode, string custCode, List<string> wmsNos)
        {
            var parms = new List<object> { dcCode, gupCode, custCode };
            var sql = $@" SELECT 
                          A.WMS_NO WmsNo,
                          B.COLLECTION_NAME CollectionName
                          FROM F051301 A, F1945 B  
                          WHERE A.DC_CODE = B.DC_CODE
                          AND A.CELL_TYPE = B.CELL_TYPE
                          AND A.COLLECTION_CODE = B.COLLECTION_CODE
                          AND A.DC_CODE = @p0
                          AND A.GUP_CODE = @p1
                          AND A.CUST_CODE = @p2
                         ";
            sql += parms.CombineNotNullOrEmptySqlInParameters("AND A.WMS_NO", wmsNos);
            return SqlQuery<WcsOutboundCollectionData>(sql, parms.ToArray());
        }

        public ContainerSingleByOrd GetContainerSingleByOrd(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var parm = new List<SqlParameter>
            {
                new SqlParameter("@p0", Current.Lang),
                new SqlParameter("@p1", dcCode),
                new SqlParameter("@p2", gupCode),
                new SqlParameter("@p3", custCode),
                new SqlParameter("@p4", wmsOrdNo)
            };
            var sql = $@" SELECT 
                            K.COLLECTION_CODE,
                            K.NEXT_STEP,
                            L.NAME NEXT_STEP_NAME,
                            K.STATUS,
                            S.NAME STATUS_NAME
                            FROM F051301 K 
                            LEFT JOIN VW_F000904_LANG L
                            ON L.TOPIC='F051301' 
                            AND L.SUBTOPIC='NEXT_STEP'
                            AND L.LANG=@p0
                            AND L.VALUE = K.NEXT_STEP
                            LEFT JOIN VW_F000904_LANG S 
                            ON S.TOPIC='F051301' 
                            AND S.SUBTOPIC='STATUS'
                            AND S.LANG=@p0
                            AND S.VALUE = K.STATUS
                            WHERE K.DC_CODE =@p1
                            AND K.GUP_CODE =@p2
                            AND K.CUST_CODE =@p3
                            AND K.WMS_NO =@p4
                         ";

            return SqlQuery<ContainerSingleByOrd>(sql, parm.ToArray()).FirstOrDefault();
        }

        public void UpdateCollectionPosition(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string wmsNo, string collectionPosition)
        {
            var parm = new List<SqlParameter>
            {
                new SqlParameter("@p0", collectionPosition),
                new SqlParameter("@p1", Current.Staff),
                new SqlParameter("@p2", Current.StaffName),
                new SqlParameter("@p3", dcCode),
                new SqlParameter("@p4", gupCode),
                new SqlParameter("@p5", custCode),
                new SqlParameter("@p6", delvDate),
                new SqlParameter("@p7", pickTime),
                new SqlParameter("@p8", wmsNo)
            };

            var sql = @"
                        UPDATE F051301 SET COLLECTION_POSITION=@p0, UPD_STAFF=@p1, UPD_NAME=@p2,UPD_DATE = dbo.GetSysDate()
                        WHERE DC_CODE = @p3
                        AND GUP_CODE = @p4
                        AND CUST_CODE = @p5
                        AND DELV_DATE = @p6
                        AND PICK_TIME = @p7
                        AND WMS_NO = @p8
                        ";

            ExecuteSqlCommand(sql, parm.ToArray());
        }

        public IQueryable<F051301> GetDataByChkShip(string dcCode, string gupCode, string custCode, string wmsNo)
        {
            var parms = new List<object> { dcCode, gupCode, custCode, wmsNo };
            var sql = $@" SELECT * 
                          FROM F051301
                          WHERE DC_CODE = @p0
                          AND GUP_CODE = @p1
                          AND CUST_CODE = @p2
                          AND WMS_NO = @p3
                          AND COLLECTION_POSITION = '0' --人工集貨場
                          AND STATUS NOT IN(1)
                         ";

            return SqlQuery<F051301>(sql, parms.ToArray());
        }

        public void DeleteWmsNo(string dcCode, string gupCode, string custCode, string wmsNo)
        {
            var sql = @" DELETE F051301
										WHERE DC_CODE = @p0
										 AND GUP_CODE = @p1
										 AND CUST_CODE = @p2
                                         AND WMS_NO = @p3 
                        ";
            ExecuteSqlCommand(sql, new object[] { dcCode, gupCode, custCode, wmsNo });
        }
    }
}
