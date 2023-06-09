using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F16
{
	public partial class F160402Repository : RepositoryBase<F160402, Wms3plDbContext, F160402Repository>
	{
        
		public IQueryable<F160402Data> GetF160402ScrapDetails(string dcCode, string gupCode, string custCode, string scrapNo)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", scrapNo)
			};
			var sql = @"SELECT a.*,
							   b.ITEM_NAME,
							   b.ITEM_SIZE,
							   b.ITEM_SPEC,
							   b.ITEM_COLOR,
							   c.QTY,
							   d.All_Qty
						  FROM F160402 a
							   LEFT JOIN F1903 b
								  ON a.GUP_CODE = b.GUP_CODE AND a.ITEM_CODE = b.ITEM_CODE AND a.CUST_CODE = b.CUST_CODE
							   LEFT JOIN
							   (  SELECT aa.DC_CODE,
										 aa.GUP_CODE,
										 aa.CUST_CODE,
										 aa.ITEM_CODE,
										 aa.LOC_CODE,
										 bb.WAREHOUSE_ID,
										 aa.VALID_DATE,
										 aa.BOX_CTRL_NO,
										 aa.PALLET_CTRL_NO,
                                         aa.MAKE_NO,
										 SUM (aa.QTY) AS QTY
									FROM F1913 aa
										 LEFT JOIN F1912 bb
											ON aa.DC_CODE = bb.DC_CODE AND aa.LOC_CODE = bb.LOC_CODE
								GROUP BY aa.DC_CODE,
										 aa.GUP_CODE,
										 aa.CUST_CODE,
										 aa.ITEM_CODE,
										 aa.LOC_CODE,
										 bb.WAREHOUSE_ID,
										 aa.VALID_DATE,
										 aa.BOX_CTRL_NO,
										 aa.PALLET_CTRL_NO,
                                         aa.MAKE_NO) c
								  ON     a.DC_CODE = c.DC_CODE
									 AND a.GUP_CODE = c.GUP_CODE
									 AND a.CUST_CODE = c.CUST_CODE
									 AND a.ITEM_CODE = c.ITEM_CODE
									 AND a.LOC_CODE = c.LOC_CODE
									 AND a.WAREHOUSE_ID = c.WAREHOUSE_ID
									 AND a.VALID_DATE = c.VALID_DATE
									 AND a.BOX_CTRL_NO = c.BOX_CTRL_NO
									 AND a.PALLET_CTRL_NO = c.PALLET_CTRL_NO
                                     AND a.MAKE_NO = c.MAKE_NO
							   LEFT JOIN
							   (  SELECT aa.DC_CODE,
										 aa.GUP_CODE,
										 aa.CUST_CODE,
										 aa.ITEM_CODE,
										 aa.LOC_CODE,
										 bb.WAREHOUSE_ID,
										 SUM (aa.QTY) AS All_Qty
									FROM F1913 aa
										 LEFT JOIN F1912 bb
											ON aa.DC_CODE = bb.DC_CODE AND aa.LOC_CODE = bb.LOC_CODE
								GROUP BY aa.DC_CODE,
										 aa.GUP_CODE,
										 aa.CUST_CODE,
										 aa.ITEM_CODE,
										 aa.LOC_CODE,
										 bb.WAREHOUSE_ID) d
								  ON     a.DC_CODE = d.DC_CODE
									 AND a.GUP_CODE = d.GUP_CODE
									 AND a.CUST_CODE = d.CUST_CODE
									 AND a.ITEM_CODE = d.ITEM_CODE
									 AND a.LOC_CODE = d.LOC_CODE
									 AND a.WAREHOUSE_ID = d.WAREHOUSE_ID
						 WHERE     a.DC_CODE = @p0
							   AND a.GUP_CODE = @p1
							   AND a.CUST_CODE = @p2
							   AND a.SCRAP_NO = @p3 ";

			var result = SqlQuery<F160402Data>(sql, parameters.ToArray()).AsQueryable();
			return result;
		}

		public IQueryable<F160402AddData> GetF160402AddScrapDetails(string dcCode, string gupCode, string custCode, string wareHouseId, string itemCode, string locCode, string itemName, DateTime? validDateStart, DateTime? validDateEnd)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", wareHouseId)
			};
            var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY a.ITEM_CODE, a.GUP_CODE),
							   a.ITEM_CODE,
							   b.DC_CODE,
							   a.GUP_CODE,
							   b.CUST_CODE,
							   b.LOC_CODE,
							   b.WAREHOUSE_ID,
							   b.BOX_CTRL_NO,
							   b.PALLET_CTRL_NO,
							   a.ITEM_NAME,
							   a.ITEM_SIZE,
							   a.ITEM_SPEC,
							   a.ITEM_COLOR,
							   b.VALID_DATE,
							   b.QTY,
							   c.All_Qty,
                               b.MAKE_NO
						  FROM F1903 a
							   LEFT JOIN
							   (  SELECT aa.DC_CODE,
										 aa.GUP_CODE,
										 aa.CUST_CODE,
										 aa.ITEM_CODE,
										 aa.LOC_CODE,
										 bb.WAREHOUSE_ID,
										 aa.VALID_DATE,
										 aa.BOX_CTRL_NO,
										 aa.PALLET_CTRL_NO,
										 SUM (aa.QTY) AS QTY,
                                         aa.MAKE_NO
									FROM F1913 aa
										 LEFT JOIN F1912 bb
											ON aa.DC_CODE = bb.DC_CODE AND aa.LOC_CODE = bb.LOC_CODE
								GROUP BY aa.DC_CODE,
										 aa.GUP_CODE,
										 aa.CUST_CODE,
										 aa.ITEM_CODE,
										 aa.LOC_CODE,
										 bb.WAREHOUSE_ID,
										 aa.VALID_DATE,
										 aa.BOX_CTRL_NO,
										 aa.PALLET_CTRL_NO,
                                         aa.MAKE_NO) b
								  ON a.GUP_CODE = b.GUP_CODE AND a.ITEM_CODE = b.ITEM_CODE AND a.CUST_CODE = b.CUST_CODE
							   LEFT JOIN
							   (  SELECT aa.DC_CODE,
										 aa.GUP_CODE,
										 aa.CUST_CODE,
										 aa.ITEM_CODE,
										 aa.LOC_CODE,
										 bb.WAREHOUSE_ID,
										 SUM (aa.QTY) AS All_Qty
									FROM F1913 aa
										 LEFT JOIN F1912 bb
											ON aa.DC_CODE = bb.DC_CODE AND aa.LOC_CODE = bb.LOC_CODE
								GROUP BY aa.DC_CODE,
										 aa.GUP_CODE,
										 aa.CUST_CODE,
										 aa.ITEM_CODE,
										 aa.LOC_CODE,
										 bb.WAREHOUSE_ID) c
								  ON     b.DC_CODE = c.DC_CODE
									 AND b.GUP_CODE = c.GUP_CODE
									 AND b.CUST_CODE = c.CUST_CODE
									 AND b.ITEM_CODE = c.ITEM_CODE
									 AND b.LOC_CODE = c.LOC_CODE
									 AND b.WAREHOUSE_ID = c.WAREHOUSE_ID
						 WHERE     b.QTY != 0
							   AND b.DC_CODE = @p0
							   AND b.GUP_CODE = @p1
							   AND b.CUST_CODE = @p2
							   AND b.WAREHOUSE_ID = @p3 ";
							   //--箱板管理新增箱號板號之後調整應該將'0'改為變數由UI帶入參數值
							   //AND b.BOX_CTRL_NO = '0'
							   //AND b.PALLET_CTRL_NO = '0' ";

			if (!string.IsNullOrEmpty(itemCode))
			{
				sql += string.Format("AND a.ITEM_CODE = @p{0} ", parameters.Count);
				parameters.Add(new SqlParameter(string.Format("@p{0}", parameters.Count), itemCode));
			}
			if (!string.IsNullOrEmpty(locCode))
			{
				sql += string.Format("AND b.LOC_CODE = @p{0} ", parameters.Count);
				parameters.Add(new SqlParameter(string.Format("@p{0}", parameters.Count), locCode));
			}
			if (!string.IsNullOrEmpty(itemName))
			{
				sql += string.Format("AND a.ITEM_NAME LIKE @p{0} + '%' ", parameters.Count);
				parameters.Add(new SqlParameter(string.Format("@p{0}",parameters.Count), itemName));
			}
			if (validDateStart != null)
			{
				sql += string.Format("AND b.VALID_DATE >= @p{0} ", parameters.Count);
				parameters.Add(new SqlParameter(string.Format("@p{0}", parameters.Count), validDateStart.Value.Date));
			}
			if (validDateEnd != null)
			{
				sql += string.Format("AND b.VALID_DATE <= @p{0} ", parameters.Count);
				parameters.Add(new SqlParameter(string.Format("@p{0}", parameters.Count), validDateEnd.Value.Date));
			}

			var result = SqlQuery<F160402AddData>(sql, parameters.ToArray()).AsQueryable();
			return result;
		}

		public IQueryable<F160402StockSum> GetF160402StockSum(string dcCode, string gupCode, string custCode, string itemCode, string locCode, string wareHouseId)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", itemCode),
				new SqlParameter("@p4", locCode),
				new SqlParameter("@p5", wareHouseId)
			};
			var sql = @"SELECT aa.DC_CODE,aa.GUP_CODE,aa.CUST_CODE,aa.ITEM_CODE,aa.LOC_CODE,bb.WAREHOUSE_ID,SUM(aa.QTY) as All_Qty
									  FROM F1913 aa LEFT JOIN F1912 bb ON aa.DC_CODE = bb.DC_CODE AND aa.LOC_CODE = bb.LOC_CODE
							     WHERE aa.DC_CODE = @p0
                     AND aa.GUP_CODE = @p1
                     AND aa.CUST_CODE = @p2
                     AND aa.ITEM_CODE = @p3  
                     AND aa.LOC_CODE = @p4
										 AND bb.WAREHOUSE_ID = @p5 
								GROUP BY aa.DC_CODE,aa.GUP_CODE,aa.CUST_CODE,aa.ITEM_CODE,aa.LOC_CODE,bb.WAREHOUSE_ID ";

			var result = SqlQuery<F160402StockSum>(sql, parameters.ToArray()).AsQueryable();
			return result;
		}

		public IQueryable<F160402> GetF160402(string dcCode, string gupCode, string custCode, string scrapNo)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", scrapNo)
			};
			var sql = @"SELECT a.*
									  FROM F160402 a
									 WHERE a.DC_CODE = @p0
									 	 AND a.GUP_CODE = @p1
									 	 AND a.CUST_CODE = @p2
									 	 AND a.SCRAP_NO = @p3 ";

			var result = SqlQuery<F160402>(sql, parameters.ToArray()).AsQueryable();
			return result;
		}

		public decimal GetMaxSeq(string dcCode, string gupCode, string custCode, string scrapNo)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", scrapNo)
			};
			var sql = @"SELECT Max(a.SCRAP_SEQ) as SCRAP_SEQ
									  FROM F160402 a
									 WHERE a.DC_CODE = @p0
									 	 AND a.GUP_CODE = @p1
									 	 AND a.CUST_CODE = @p2
									 	 AND a.SCRAP_NO = @p3 ";

			var result = SqlQuery<decimal?>(sql, parameters.ToArray()).FirstOrDefault();
			
			return result ?? 0;
		}

		public decimal GetF160402ScrapSum(string dcCode, string gupCode, string custCode, string scrapNo, string itemCode, string locCode, string wareHouseId)
		{
			var parameters = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode),
				new SqlParameter("@p1", gupCode),
				new SqlParameter("@p2", custCode),
				new SqlParameter("@p3", itemCode),
				new SqlParameter("@p4", locCode),
				new SqlParameter("@p5", wareHouseId)
			};

            var sql = @"SELECT SUM(b.SCRAP_QTY) as All_SCRAP_QTY
									  FROM F160401 a
							 LEFT JOIN F160402 b ON a.DC_CODE = b.DC_CODE AND a.GUP_CODE = b.GUP_CODE AND a.CUST_CODE = b.CUST_CODE AND a.SCRAP_NO = b.SCRAP_NO
							     WHERE a.DC_CODE = @p0
                     AND a.GUP_CODE = @p1
                     AND a.CUST_CODE = @p2
                     AND b.ITEM_CODE = @p3  
                     AND b.LOC_CODE = @p4
										 AND b.WAREHOUSE_ID = @p5 
										 AND a.STATUS = '0' ";
										 //--箱板管理新增箱號板號之後調整應該將'0'改為變數由UI帶入參數值
										 //AND b.BOX_CTRL_NO = '0'
										 //AND b.PALLET_CTRL_NO = '0' ";                                          

            if(!string.IsNullOrWhiteSpace(scrapNo))
            {
                parameters.Add(new SqlParameter("@p6", scrapNo));
                sql += " AND a.SCRAP_NO <> @p6 ";
            }
			var result = SqlQuery<decimal?>(sql, parameters.ToArray()).FirstOrDefault();

			return result ?? 0;
		}

        public IQueryable<F160402> GetF160402ScrapData(string dcCode, string gupCode, string custCode, string scrapNo, string itemCode, string locCode, string wareHouseId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", itemCode),
                new SqlParameter("@p4", locCode),
                new SqlParameter("@p5", wareHouseId)
            };

            var sql = @"SELECT b.*
									  FROM F160401 a
							 LEFT JOIN F160402 b ON a.DC_CODE = b.DC_CODE AND a.GUP_CODE = b.GUP_CODE AND a.CUST_CODE = b.CUST_CODE AND a.SCRAP_NO = b.SCRAP_NO
							     WHERE a.DC_CODE = @p0
                     AND a.GUP_CODE = @p1
                     AND a.CUST_CODE = @p2
                     AND b.ITEM_CODE = @p3  
                     AND b.LOC_CODE = @p4
										 AND b.WAREHOUSE_ID = @p5 
										 AND a.STATUS = '0' ";
            //--箱板管理新增箱號板號之後調整應該將'0'改為變數由UI帶入參數值
            //AND b.BOX_CTRL_NO = '0'
            //AND b.PALLET_CTRL_NO = '0' ";                                          

            if (!string.IsNullOrWhiteSpace(scrapNo))
            {
                parameters.Add(new SqlParameter("@p6", scrapNo));
                sql += " AND a.SCRAP_NO <> @p6 ";
            }

            return SqlQuery<F160402>(sql, parameters.ToArray());
        }
    }
}
