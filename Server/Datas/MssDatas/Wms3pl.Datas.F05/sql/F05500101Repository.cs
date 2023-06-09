using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F05500101Repository : RepositoryBase<F05500101, Wms3plDbContext, F05500101Repository>
	{
        public IQueryable<F05500101> GetDatas(string dcCode, string gupCode, string custCode, string stockNo)
        {
            var parameter = new object[] { dcCode, gupCode, custCode, stockNo, stockNo };
            var sql = @" SELECT A.* 
                           FROM F05500101 A
                          WHERE A.DC_CODE = @p0 
                            AND A.GUP_CODE = @p1 
                            AND A.CUST_CODE = @p2 
                            AND (EXISTS ( 
                            SELECT 1 
                              FROM F010201 B
                             INNER JOIN F05030101 C 
                                ON C.DC_CODE = B.DC_CODE 
                               AND C.GUP_CODE = B.GUP_CODE 
                               AND C.CUST_CODE = B.CUST_CODE 
                               AND C.ORD_NO = B.SOURCE_NO 
                             WHERE B.DC_CODE = A.DC_CODE 
                        				AND B.GUP_CODE = A.GUP_CODE 
                               AND B.CUST_CODE = A.CUST_CODE 
                               AND C.WMS_ORD_NO = A.WMS_ORD_NO 
                               AND B.STOCK_NO = @p3 
                            ) OR EXISTS( 
                              SELECT 1 
                                FROM F010201 B 
                               INNER JOIN F050301 C 
                                  ON C.DC_CODE = B.DC_CODE 
                                 AND C.GUP_CODE = B.GUP_CODE 
                                 AND C.CUST_CODE = B.CUST_CODE 
                                 AND C.SOURCE_NO = B.SOURCE_NO 
                               INNER JOIN F05030101 D 
                                          ON D.DC_CODE = C.DC_CODE 
                                       AND D.GUP_CODE = C.GUP_CODE 
                                       AND D.CUST_CODE = C.CUST_CODE 
                                       AND D.ORD_NO = C.ORD_NO 
                                     WHERE B.DC_CODE = A.DC_CODE 
                                				AND B.GUP_CODE = A.GUP_CODE 
                                       AND B.CUST_CODE = A.CUST_CODE 
                                       AND D.WMS_ORD_NO = A.WMS_ORD_NO 
                                       AND B.STOCK_NO = @p4 
                                    ))
                            AND A.ISPASS = '1' 
                            AND A.SERIAL_NO IS NOT NULL ";

            var result = SqlQuery<F05500101>(sql, parameter);

            return result;
        }

        public IQueryable<F05500101Data> GetF05500101Datas(string dcCode, string gupCode, string custCode, string stockNo, string itemCode)
        {
            var parameter = new object[] { dcCode, gupCode, custCode, itemCode, stockNo, stockNo };
            var sql = @" SELECT DISTINCT ROW_NUMBER()OVER(ORDER BY A.WMS_ORD_NO, A.LOG_SEQ, A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.PACKAGE_BOX_NO ASC) ROWNUM,A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.WMS_ORD_NO,A.ITEM_CODE,B.VALID_DATE 
                         FROM F05500101 A 
                        INNER JOIN F051202 B 
                           ON  A.DC_CODE = B.DC_CODE 
                          AND A.GUP_CODE = B.GUP_CODE 
                          AND A.CUST_CODE = B.CUST_CODE 
                          AND A.ITEM_CODE = B.ITEM_CODE 
                          AND A.WMS_ORD_NO = B.WMS_ORD_NO 
                        WHERE A.ISPASS ='1' 
                          AND A.DC_CODE =@p0 
                          AND A.GUP_CODE =@p1 
                          AND A.CUST_CODE =@p2 
                          AND A.ITEM_CODE = @p3 
                          AND (EXISTS ( 
                          SELECT 1 
                            FROM F010201 B
                           INNER JOIN F05030101 C 
                              ON C.DC_CODE = B.DC_CODE 
                             AND C.GUP_CODE = B.GUP_CODE 
                             AND C.CUST_CODE = B.CUST_CODE 
                             AND C.ORD_NO = B.SOURCE_NO 
                           WHERE B.DC_CODE = A.DC_CODE 
                      				AND B.GUP_CODE = A.GUP_CODE 
                             AND B.CUST_CODE = A.CUST_CODE 
                             AND C.WMS_ORD_NO = A.WMS_ORD_NO 
                             AND B.STOCK_NO = @p4 
                          ) OR EXISTS( 
                            SELECT 1 
                              FROM F010201 B 
                             INNER JOIN F050301 C 
                                ON C.DC_CODE = B.DC_CODE 
                               AND C.GUP_CODE = B.GUP_CODE 
                               AND C.CUST_CODE = B.CUST_CODE 
                               AND C.SOURCE_NO = B.SOURCE_NO 
                             INNER JOIN F05030101 D 
                                ON D.DC_CODE = C.DC_CODE 
                             AND D.GUP_CODE = C.GUP_CODE 
                             AND D.CUST_CODE = C.CUST_CODE 
                             AND D.ORD_NO = C.ORD_NO 
                           WHERE B.DC_CODE = A.DC_CODE 
                      				AND B.GUP_CODE = A.GUP_CODE 
                             AND B.CUST_CODE = A.CUST_CODE 
                             AND D.WMS_ORD_NO = A.WMS_ORD_NO 
                             AND B.STOCK_NO = @p5 
                          ))
                    ";

            var result = SqlQuery<F05500101Data>(sql, parameter);

            return result;
        }

		public IQueryable<F05500101> GetItemCodeAndSerialNo(string dcCode, string gupCode, string custCode, IEnumerable<string> wmsOdrNoList)
		{
			var parameter = new List<object> { dcCode, gupCode, custCode };

			int paramStartIndex = parameter.Count;
			
			var sql = @"SELECT * FROM F05500101 
						WHERE ISPASS = '1'
            AND FLAG = '0'
						AND SERIAL_NO<> '0'
						AND ISNULL(SERIAL_NO,'') <> ''
						AND DC_CODE = @p0
						AND GUP_CODE = @p1
						AND CUST_CODE = @p2
              AND FLAG='0'"; 
			sql+=parameter.CombineSqlInParameters(" AND WMS_ORD_NO", wmsOdrNoList, ref paramStartIndex);
			var result = SqlQuery<F05500101>(sql, parameter.ToArray());

			return result;
		}

	}
}