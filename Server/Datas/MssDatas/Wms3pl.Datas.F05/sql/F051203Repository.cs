using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F051203Repository : RepositoryBase<F051203, Wms3plDbContext, F051203Repository>
	{
		public IQueryable<F051203> GetDataByPickNo(string dcCode,string gupCode,string custCode,string pickOrdNo)
		{

			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",dcCode){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1",gupCode){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p2",custCode){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p3",pickOrdNo){ SqlDbType = SqlDbType.VarChar},
			};
			var sql = @" SELECT * FROM F051203
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND PICK_ORD_NO= @p3";
			return SqlQuery<F051203>(sql, parms.ToArray());
		}

		public IQueryable<GetPickDetailDetail> GetBatchPickDetail(
								string dcNo, string gupCode, string custNo, string pickOrdNo)
		{
			var parameters = new List<SqlParameter>
								{
										new SqlParameter("@p0", dcNo),
										new SqlParameter("@p1", gupCode),
										new SqlParameter("@p2", custNo),
										new SqlParameter("@p3",pickOrdNo)
								};

			var sql = $@"SELECT 
                        			A.PICK_ORD_NO AS WmsNo,
															A.PICK_ORD_NO AS PickNo,
                        			A.TTL_PICK_SEQ AS WmsSeq,
                        			A.ITEM_CODE AS ItemNo,
                        			C.WAREHOUSE_NAME AS WHName,
                        			A.PICK_LOC AS Loc,
                        			A.VALID_DATE AS ValidDate,
                        			A.B_PICK_QTY AS ShipQty,
                        			ISNULL(A.SERIAL_NO,'') AS Sn,
                        			ISNULL(A.MAKE_NO,'') AS MkNo,
                              '' AS PalletNo,
                              A.CRT_DATE AS CrtDate,
                              A.ROUTE_SEQ AS Route
														FROM F051203 A
														JOIN F1912 B ON A.DC_CODE = B.DC_CODE 
                        					AND A.PICK_LOC = B.LOC_CODE 
														JOIN F1980 C ON B.DC_CODE = C.DC_CODE 
                        					AND B.WAREHOUSE_ID = C.WAREHOUSE_ID 
														WHERE A.DC_CODE = @p0
															AND A.GUP_CODE = @p1
															AND A.CUST_CODE = @p2
                              AND A.PICK_ORD_NO = @p3
															AND A.PICK_STATUS = '0'
														ORDER BY A.ROUTE_SEQ,A.ITEM_CODE";

			var result = SqlQuery<GetPickDetailDetail>(sql, parameters.ToArray());
			return result;
		}

        public IQueryable<GetPickDetailDetail> GetBatchPickDetailAllCol(string dcNo, string gupCode, string custNo, string pickOrdNo)
        {
            var parameters = new List<SqlParameter>
                                {
                                        new SqlParameter("@p0", dcNo),
                                        new SqlParameter("@p1", gupCode),
                                        new SqlParameter("@p2", custNo),
                                        new SqlParameter("@p3",pickOrdNo)
                                };

            var sql = $@"SELECT 
                        A.PICK_ORD_NO AS WmsNo,
                        A.PICK_ORD_NO AS PickNo,
                        A.TTL_PICK_SEQ AS WmsSeq,
                        A.ITEM_CODE AS ItemNo,
                        C.WAREHOUSE_NAME AS WHName,
                        A.PICK_LOC AS Loc,
                        A.VALID_DATE AS ValidDate,
                        A.B_PICK_QTY AS ShipQty,
                        ISNULL(A.SERIAL_NO,'') AS Sn,
                        ISNULL(A.MAKE_NO,'') AS MkNo,
                        '' AS PalletNo,
                        A.CRT_DATE AS CrtDate,
                        A.ROUTE_SEQ AS Route,
                        D.ITEM_UNIT Unit,        
                        D.ITEM_NAME ProductName ,        
                        D.ITEM_SIZE ProductSize ,        
                        D.ITEM_COLOR ProductColor,        
                        D.ITEM_SPEC ProductSpec ,        
                        D.EAN_CODE1 Barcode1,        
                        D.EAN_CODE2 Barcode2 ,        
                        D.EAN_CODE3 Barcode3 ,        
                        E.PACK_WEIGHT Weight,        
                        D.CTNS BoxQty,        
                        A.CUST_CODE CustNo
                        FROM F051203 A
                        JOIN F1912 B ON A.DC_CODE = B.DC_CODE 
                        			AND A.PICK_LOC = B.LOC_CODE 
                        JOIN F1980 C ON B.DC_CODE = C.DC_CODE 
                        			AND B.WAREHOUSE_ID = C.WAREHOUSE_ID 
                        LEFT JOIN F1903 D ON A.GUP_CODE = D.GUP_CODE
                        			AND A.CUST_CODE = D.CUST_CODE
                        			AND A.ITEM_CODE = D.ITEM_CODE
                        LEFT JOIN F1905 E ON A.GUP_CODE = E.GUP_CODE
                        			AND A.CUST_CODE = E.CUST_CODE
                        			AND A.ITEM_CODE = E.ITEM_CODE
                        WHERE A.DC_CODE = @p0
                        	AND A.GUP_CODE = @p1
                        	AND A.CUST_CODE = @p2
                        	AND A.PICK_ORD_NO = @p3
                        	AND A.PICK_STATUS = '0'
                        ORDER BY A.ROUTE_SEQ,A.ITEM_CODE";

            var result = SqlQuery<GetPickDetailDetail>(sql, parameters.ToArray());
            return result;
        }

        public IQueryable<F051203> GetLackDataByPickNo(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			var sql = @" SELECT * FROM F051203
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND PICK_ORD_NO= @p3
                      AND B_PICK_QTY > A_PICK_QTY ";
			var parms = new object[] { dcCode, gupCode, custCode, pickOrdNo };
			return SqlQuery<F051203>(sql, parms);
		}

		public IQueryable<WcsOutboundSkuModel> GetWcsDetail(string dcCode, string gupCode, string custCode, string pickOrdNo, string warehouseId)
		{
			var parameters = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode),
								new SqlParameter("@p3", pickOrdNo),
								new SqlParameter("@p4", warehouseId)
						};

			var sql = $@"SELECT 
                         A.TTL_PICK_SEQ RowNum,
                         A.ITEM_CODE SkuCode,
                         A.B_PICK_QTY SkuQty,
                         1 SkuLevel,
                         convert(varchar, A.VALID_DATE, 111) ExpiryDate,
                         A.MAKE_NO OutBatchCode
                         FROM F051203 A
                         JOIN F1903 B
                         ON A.ITEM_CODE = B.ITEM_CODE
                         AND A.GUP_CODE = B.GUP_CODE
                         AND A.CUST_CODE = B.CUST_CODE
                         JOIN F051201 C
                           ON C.DC_CODE = A.DC_CODE
                          AND C.GUP_CODE = A.GUP_CODE
                          AND C.CUST_CODE = A.CUST_CODE
                          AND C.PICK_ORD_NO = A.PICK_ORD_NO
                         WHERE A.DC_CODE = @p0
                         AND A.GUP_CODE = @p1
                         AND A.CUST_CODE = @p2
                         AND A.PICK_ORD_NO = @p3
                         AND A.PICK_STATUS = '0'
												 AND C.SPLIT_CODE = @p4
                         ";

			var result = SqlQuery<WcsOutboundSkuModel>(sql, parameters.ToArray());
			return result;
		}

		public IQueryable<PickInfoWithLackItem> GetLackPickDtl(string dcCode, string gupCode, string custCode, string pickOrdNo)
		{
			var parms = new List<SqlParameter>();
			parms.Add(new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar });
			parms.Add(new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar });
			parms.Add(new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar });
			parms.Add(new SqlParameter("@p3", pickOrdNo) { SqlDbType = SqlDbType.VarChar });

			var sql = @" SELECT PICK_LOC, ITEM_CODE, VALID_DATE, MAKE_NO, SERIAL_NO, B_PICK_QTY - A_PICK_QTY AS LACK_QTY
                     FROM F051203
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND PICK_ORD_NO = @p3
                      AND PICK_STATUS <> 9
                      AND B_PICK_QTY > A_PICK_QTY
                      ORDER BY SERIAL_NO DESC,ITEM_CODE ";

			return SqlQuery<PickInfoWithLackItem>(sql, parms.ToArray());
		}
	}
}
