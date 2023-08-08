using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Wms3pl.Datas.F05
{
	public partial class F05030202Repository : RepositoryBase<F05030202, Wms3plDbContext, F05030202Repository>
	{
		public IQueryable<ShipPackageNoAllotOrder> GetItemShipPackageNoAllotOrder(string dcCode,string gupCode,string custCode,string wmsOrdNo,string itemCode)
		{
			var parms = new object[] { dcCode, gupCode, custCode, wmsOrdNo, itemCode };
			var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ORD_NO,A.ORD_SEQ,A.WMS_ORD_NO,B.ITEM_CODE,A.B_DELV_QTY,B.SERIAL_NO ASC) ROWNUM,
                         A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ORD_NO,A.ORD_SEQ,A.WMS_ORD_NO,B.ITEM_CODE,A.B_DELV_QTY,B.SERIAL_NO,ISNULL(C.PACKAGE_QTY,0) PACKAGE_QTY
										FROM F05030202 A
										JOIN F050802 B
										  ON B.DC_CODE = A.DC_CODE
										 AND B.GUP_CODE = A.GUP_CODE
										 AND B.CUST_CODE = A.CUST_CODE
										 AND B.WMS_ORD_NO = A.WMS_ORD_NO
										 AND B.WMS_ORD_SEQ = A.WMS_ORD_SEQ
										LEFT JOIN 
										(
										SELECT D.DC_CODE,D.GUP_CODE,D.CUST_CODE,D.WMS_ORD_NO,D.ITEM_CODE,
                           CASE WHEN E.BUNDLE_SERIALLOC ='1' THEN D.SERIAL_NO ELSE NULL END SERIAL_NO,
                            D.ORD_NO,D.ORD_SEQ,SUM(D.PACKAGE_QTY) PACKAGE_QTY
										FROM F055002 D
										JOIN F1903 E
										ON D.GUP_CODE = E.GUP_CODE
										AND D.CUST_CODE = E.CUST_CODE
										AND D.ITEM_CODE = E.ITEM_CODE
										GROUP BY D.DC_CODE,D.GUP_CODE,D.CUST_CODE,D.WMS_ORD_NO,D.ITEM_CODE,
                             CASE WHEN E.BUNDLE_SERIALLOC ='1' THEN D.SERIAL_NO ELSE NULL END,
                             D.ORD_NO,D.ORD_SEQ, E.BUNDLE_SERIALLOC) C
										 ON C.DC_CODE = A.DC_CODE
										AND C.GUP_CODE = A.GUP_CODE
										AND C.CUST_CODE = A.CUST_CODE
										AND C.WMS_ORD_NO = A.WMS_ORD_NO
										AND C.ITEM_CODE = B.ITEM_CODE
                    AND C.ORD_NO = A.ORD_NO
										AND C.ORD_SEQ = A.ORD_SEQ
                    AND ISNULL(C.SERIAL_NO,'') = ISNULL(B.SERIAL_NO,'')
									WHERE A.DC_CODE = @p0
										AND A.GUP_CODE = @p1
										AND A.CUST_CODE = @p2
										AND A.WMS_ORD_NO = @p3
                    AND B.ITEM_CODE = @p4
                    AND A.B_DELV_QTY > ISNULL(C.PACKAGE_QTY,0) ";
			return SqlQuery<ShipPackageNoAllotOrder>(sql, parms);
		}

        public IQueryable<ShipPackageNoAllotOrder> NewGetItemShipPackageNoAllotOrder(string dcCode, string gupCode, string custCode, string wmsOrdNo, string itemCode)
        {
            var parms = new object[] { dcCode, gupCode, custCode, wmsOrdNo, itemCode, dcCode, gupCode, custCode, wmsOrdNo, itemCode };
            var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ORD_NO,A.ORD_SEQ,A.WMS_ORD_NO,B.ITEM_CODE,A.B_DELV_QTY,B.SERIAL_NO ASC) ROWNUM,
                         A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ORD_NO,A.ORD_SEQ,A.WMS_ORD_NO,B.ITEM_CODE,A.B_DELV_QTY,B.SERIAL_NO,ISNULL(C.PACKAGE_QTY,0) PACKAGE_QTY
							FROM F05030202 A
							JOIN F050802 B
							  ON B.DC_CODE = A.DC_CODE
							 AND B.GUP_CODE = A.GUP_CODE
							 AND B.CUST_CODE = A.CUST_CODE
							 AND B.WMS_ORD_NO = A.WMS_ORD_NO
							 AND B.WMS_ORD_SEQ = A.WMS_ORD_SEQ
							LEFT JOIN 
							(
							SELECT D.DC_CODE,D.GUP_CODE,D.CUST_CODE,D.WMS_ORD_NO,D.ITEM_CODE,
                            D.ORD_NO,D.ORD_SEQ,SUM(D.PACKAGE_QTY) PACKAGE_QTY,D.SERIAL_NO
										FROM F055002 D
										GROUP BY D.DC_CODE,D.GUP_CODE,D.CUST_CODE,D.WMS_ORD_NO,D.ITEM_CODE,D.ORD_NO,D.ORD_SEQ,D.SERIAL_NO) C
										 ON C.DC_CODE = A.DC_CODE
										AND C.GUP_CODE = A.GUP_CODE
										AND C.CUST_CODE = A.CUST_CODE
										AND C.WMS_ORD_NO = A.WMS_ORD_NO
										AND C.ITEM_CODE = B.ITEM_CODE
										AND C.ORD_NO = A.ORD_NO
										AND C.ORD_SEQ = A.ORD_SEQ
                    AND C.SERIAL_NO = B.SERIAL_NO
									WHERE A.DC_CODE = @p0
										AND A.GUP_CODE = @p1
										AND A.CUST_CODE = @p2
										AND A.WMS_ORD_NO = @p3
										AND B.ITEM_CODE = @p4
										AND (SELECT SUM(B_DELV_QTY) 
                         FROM F050802 
                         WHERE A.DC_CODE = @p5
										       AND A.GUP_CODE = @p6
										       AND A.CUST_CODE = @p7
										       AND A.WMS_ORD_NO = @p8
										       AND B.ITEM_CODE = @p9)
                    > ISNULL(C.PACKAGE_QTY,0) ";

            return SqlQuery<ShipPackageNoAllotOrder>(sql, parms);
        }
		   
		    public IQueryable<ShipPackageNoAllotOrder> GetDatasByWmsOrdNo(string dcCode,string gupCode,string custCode,string wmsOrdNo)
				{
			     var parms = new List<SqlParameter>
					 {
						 new SqlParameter("@p0",dcCode){ SqlDbType = System.Data.SqlDbType.VarChar},
						 new SqlParameter("@p1",gupCode){ SqlDbType = System.Data.SqlDbType.VarChar},
						 new SqlParameter("@p2",custCode){ SqlDbType = System.Data.SqlDbType.VarChar},
						 new SqlParameter("@p3",wmsOrdNo){ SqlDbType = System.Data.SqlDbType.VarChar},
					 };
			     var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ORD_NO,A.ORD_SEQ,A.WMS_ORD_NO,B.ITEM_CODE,A.B_DELV_QTY,B.SERIAL_NO ASC) ROWNUM,
                         A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ORD_NO,A.ORD_SEQ,A.WMS_ORD_NO,B.ITEM_CODE,A.B_DELV_QTY,B.SERIAL_NO,0 PACKAGE_QTY
                          FROM F05030202 A
                          JOIN F050802 B
														ON B.DC_CODE = A.DC_CODE
													 AND B.GUP_CODE = A.GUP_CODE
													 AND B.CUST_CODE = A.CUST_CODE
													 AND B.WMS_ORD_NO = A.WMS_ORD_NO
													 AND B.WMS_ORD_SEQ = A.WMS_ORD_SEQ
                         WHERE A.DC_CODE = @p0 
                           AND A.GUP_CODE = @p1
                           AND A.CUST_CODE = @p2
                           AND A.WMS_ORD_NO = @p3 ";
			    return SqlQuery<ShipPackageNoAllotOrder>(sql,parms.ToArray());
				}



		public IQueryable<F05030202> GetByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p1", gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p2", custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p3", wmsOrdNo) { SqlDbType = System.Data.SqlDbType.VarChar },
			};
			var sql = @"SELECT *
                          FROM F05030202
                         WHERE DC_CODE = @p0
                           AND GUP_CODE = @p1
                           AND CUST_CODE = @p2
                           AND WMS_ORD_NO = @p3 ";
			return SqlQuery<F05030202>(sql, parms.ToArray());
		}
	}
}
