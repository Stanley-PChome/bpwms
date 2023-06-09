using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System;

namespace Wms3pl.Datas.F01
{
    public partial class F010203Repository : RepositoryBase<F010203, Wms3plDbContext, F010203Repository>
    {
        public void DeleteByStockNo(string dcCode, string gupCode, string custCode, string stockNo, string stockType)
        {
            var sql = @" DELETE
                     FROM F010203 
                    WHERE DC_CODE = @p0 
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND STOCK_NO = @p3 
                      AND STICKER_TYPE = @p4 ";

            ExecuteSqlCommand(sql, new object[] { dcCode, gupCode, custCode, stockNo, stockType });
        }

        /// <summary>
        /// 商品檢驗_列印驗收後棧板貼紙
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="rtNO">驗收單號</param>
        /// <returns></returns>
        public IQueryable<P0202030500PalletData> GetPalletDatas(string dcCode, string gupCode, string custCode, string rtNO)
        {
            var param = new List<SqlParameter>
                        {
                                new SqlParameter("@p0", dcCode),
                                new SqlParameter("@p1", gupCode),
                                new SqlParameter("@p2", custCode),
                                new SqlParameter("@p3", rtNO),
                                new SqlParameter("@p4", DateTime.Now)
                        };

            string sql = $@"SELECT ROW_NUMBER()OVER(ORDER BY B.STICKER_NO ASC) ROWNUM, B.LOC_CODE, B.STOCK_NO,C.VNR_NAME,B.ITEM_CODE,E.ITEM_NAME,
                                CONVERT(varchar, B.PALLET_LEVEL_CASEQTY) + '*' + CONVERT(varchar, B.PALLET_LEVEL_CNT) PALLET_LEVEL,
                        		CONVERT(varchar, B.ITEM_CASE_QTY) +
                        		CASE WHEN H.ITEM_CODE IS NOT NULL AND I.ITEM_CODE IS NOT NULL 
                        		THEN N'' ELSE G.ACC_UNIT_NAME END ITEM_CASE_QTY,
                        		CONVERT(varchar, B.ITEM_PACKAGE_QTY) ITEM_PACKAGE_QTY,  
                        		B.STICKER_REF,                          
                                B.ENA_CODE1,B.ENA_CODE3,
                                B.TAR_QTY_DESC,
                                B.RECV_QTY_DESC,
                                B.RECV_QTY,
                        		CASE 
                        		WHEN B.ENTER_DATE IS NULL 
                        		THEN '' ELSE CONVERT(varchar, B.ENTER_DATE, 111) END ENTER_DATE,
                                CASE 
                                WHEN B.VALID_DATE IS NULL 
                                THEN '' ELSE CONVERT(varchar, B.VALID_DATE, 111) END VALID_DATE,
                                B.STICKER_NO,
                        		CONVERT(varchar, @p4, 120) PRINT_DATE
                        FROM F010203 B
                         JOIN F010201 A
                        ON B.GUP_CODE = A.GUP_CODE
                         AND B.CUST_CODE = A.CUST_CODE 
                         AND B.DC_CODE=A.DC_CODE
                         AND B.STOCK_NO = A.STOCK_NO
                        
                        JOIN F1908 C
                          ON C.GUP_CODE = A.GUP_CODE
                         AND C.CUST_CODE = A.CUST_CODE
                         AND C.VNR_CODE = A.VNR_CODE
                         													
                        JOIN F1903 E
                          ON E.GUP_CODE = B.GUP_CODE
                         AND E.ITEM_CODE = B.ITEM_CODE
                         AND E.CUST_CODE = B.CUST_CODE
                         																								
                        JOIN F91000302 G
                          ON G.ITEM_TYPE_ID ='001' AND G.ACC_UNIT = E.ITEM_UNIT
                        	 LEFT JOIN 
                        	 (
                        	 SELECT A1.GUP_CODE,
                                       A1.ITEM_CODE
                                  FROM F190301 A1
                                  
                                  JOIN F190301 A2 
                                  ON A2.GUP_CODE = A1.GUP_CODE AND A1.UNIT_ID = A2.UNIT_ID AND A1.ITEM_CODE = A2.ITEM_CODE AND A2.SYS_UNIT ='02'
                        									 ) H
                        									 ON H.GUP_CODE = B.GUP_CODE
                        									 AND H.ITEM_CODE = B.ITEM_CODE
                        									 
                        									 LEFT JOIN F190305 I
                        									 ON I.GUP_CODE = B.GUP_CODE
                        									 AND I.CUST_CODE = B.CUST_CODE
                        									 AND I.ITEM_CODE = B.ITEM_CODE
                        									WHERE B.DC_CODE = @p0
                        									  AND B.GUP_CODE = @p1
                        									  AND B.CUST_CODE = @p2
                        									  AND B.RT_NO = @p3
                        									  AND B.STICKER_TYPE='2'
                        									ORDER BY B.STICKER_NO";

            var result = SqlQuery<P0202030500PalletData>(sql, param.ToArray());

            return result;
        }
    }
}
