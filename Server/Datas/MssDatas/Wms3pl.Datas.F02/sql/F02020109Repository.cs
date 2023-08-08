using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
	public partial class F02020109Repository : RepositoryBase<F02020109, Wms3plDbContext, F02020109Repository>
	{
		/// <summary>
		/// 修改不良品數刪除RT_NO為NULL的資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="purchaseSeq"></param>
		public void Delete(string dcCode, string gupCode, string custCode, string purchaseNo, string purchaseSeq=null, List<string> serialNoResults = null)
		{
			var condition = string.Empty;
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p1", gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p2", custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p3", purchaseNo) { SqlDbType = System.Data.SqlDbType.VarChar },
			};
			if (!string.IsNullOrWhiteSpace(purchaseSeq))
				condition += parms.Combine(" AND STOCK_SEQ =@p"+parms.Count, int.Parse(purchaseSeq));
			if (serialNoResults != null)
			{
				condition += parms.CombineSqlInParameters(" AND SERIAL_NO", serialNoResults, SqlDbType.VarChar);
			}

			var sql = $@" DELETE F02020109 
                         WHERE DC_CODE = @p0 
                         AND GUP_CODE = @p1 
                         AND CUST_CODE = @p2 
                         AND STOCK_NO = @p3 
                         {condition}
                         AND RT_NO IS NULL ";
            ExecuteSqlCommand(sql, parms.ToArray());
        }
        

		public IQueryable<DefectDetail> GetDefectDetail(string dcCode,string gupCode,string custCode,string rtNo)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",dcCode),
				new SqlParameter("@p1",gupCode),
				new SqlParameter("@p2",custCode),
				new SqlParameter("@p3",rtNo)
			};

			var sql = @"SELECT B.RT_NO ,
						(SELECT WAREHOUSE_NAME FROM F1980 WHERE DC_CODE=B.DC_CODE AND WAREHOUSE_ID=B.WAREHOUSE_ID) WAREHOUSE_NAME,
						A.ITEM_CODE,
						SUM(B.DEFECT_QTY) QTY,
						(SELECT CAUSE FROM F1951 WHERE UCT_ID='IC' AND UCC_CODE=B.UCC_CODE) UCC_CODE_NAME,
						CAUSE,
						B.SERIAL_NO
						FROM F020201 A
						JOIN F02020109 B
						ON A.DC_CODE = B.DC_CODE 
						AND A.GUP_CODE = B.GUP_CODE 
						AND A.CUST_CODE = B.CUST_CODE 
						AND A.RT_NO =B.RT_NO 
						AND A.RT_SEQ = B.RT_SEQ 
						WHERE A.DC_CODE =@p0
						AND A.GUP_CODE = @p1
						AND A.CUST_CODE =@p2
						AND A.RT_NO =@p3
						GROUP BY B.RT_NO ,B.DC_CODE ,B.WAREHOUSE_ID ,A.ITEM_CODE ,B.UCC_CODE,CAUSE,B.SERIAL_NO";

			var result = SqlQuery<DefectDetail>(sql, param.ToArray());
			return result;
		}

        public IQueryable<F02020109> GetDatasByBarCode(string dcCode, string gupCode, string custCode, string rtNo, string inputItemCode)
        {
            var parms = new List<object> { dcCode, gupCode, custCode, rtNo, inputItemCode };
            var sql = @" SELECT *
                     FROM F02020109 
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2
                      AND RT_NO = @p3
                      AND SERIAL_NO = @p4";
            return SqlQuery<F02020109>(sql, parms.ToArray());
        }

    
		public IQueryable<DefectDetailReport> GetDefectDetailReportData(string dcCode,string gupCode,string custCode,string rtNo)
		{
			var param = new object[] { dcCode, gupCode, custCode, rtNo };
			var sql = @"SELECT A.ID, B.ITEM_CODE ,A.SERIAL_NO ,(SELECT TOP(1) CAUSE FROM F1951 WHERE UCT_ID ='IC' AND UCC_CODE = A.UCC_CODE) UCC_CODE_NAME,A.CAUSE 
						FROM F02020109 A
						JOIN F020201 B 
						ON A.STOCK_NO = B.PURCHASE_NO 
						AND A.STOCK_SEQ = B.PURCHASE_SEQ 
						AND A.RT_NO =B.RT_NO 
						AND A.RT_SEQ =B.RT_SEQ 
						WHERE A.DC_CODE = @p0
						AND A.GUP_CODE =@p1
						AND A.CUST_CODE =@p2
						AND A.RT_NO = @p3";
			return SqlQuery<DefectDetailReport>(sql, param.ToArray());
		}

		public IQueryable<F02020109> GetDatasByRtNo(string dcCode,string gupCode,string custCode,string rtNo)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",dcCode),
				new SqlParameter("@p1",gupCode),
				new SqlParameter("@p2",custCode),
				new SqlParameter("@p3",rtNo)
			};
			var sql = @" SELECT *
                    FROM F02020109
                   WHERE DC_CODE =@p0
                     AND GUP_CODE = @p1
                     AND CUST_CODE = @p2
                     AND RT_NO = @p3 ";
			return SqlQuery<F02020109>(sql, param.ToArray());
    }


    public IQueryable<F02020109> GetDatasByStockNo(string dcCode, string gupCode, string custCode, string stockNo)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p3", stockNo) { SqlDbType = System.Data.SqlDbType.VarChar },
      };
      var sql = @" SELECT *
                    FROM F02020109
                   WHERE DC_CODE = @p0
                     AND GUP_CODE = @p1
                     AND CUST_CODE = @p2
                     AND STOCK_NO = @p3
                     AND RT_NO IS NULL ";
      return SqlQuery<F02020109>(sql, param.ToArray());
    }
		public bool IsFindF02020109(string dcCode, string gupCode, string custCode, string stockNo, int stockSeq, string serialNo)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p1", gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p2", custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p3", stockNo) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p4", stockSeq) { SqlDbType = System.Data.SqlDbType.Int },
				new SqlParameter("@p5", serialNo) { SqlDbType = System.Data.SqlDbType.VarChar },
			};
			var sql = @" SELECT TOP(1) 1 
                     FROM F02020109
                   WHERE DC_CODE = @p0
                     AND GUP_CODE = @p1
                     AND CUST_CODE = @p2
                     AND STOCK_NO = @p3 
                     AND STOCK_SEQ = @p4
                     AND SERIAL_NO = @p5
                     AND RT_NO IS NULL ";
			return SqlQuery<int>(sql, parms.ToArray()).Any();
		}
		public void DeleteF02020109(string dcCode,string gupCode,string custCode,string stockNo,int stockSeq,string serialNo)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0", dcCode) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p1", gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p2", custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p3", stockNo) { SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p4", stockSeq) { SqlDbType = System.Data.SqlDbType.Int },
				new SqlParameter("@p5", serialNo) { SqlDbType = System.Data.SqlDbType.VarChar },
			};
			var sql = @" DELETE FROM F02020109
                   WHERE DC_CODE = @p0
                     AND GUP_CODE = @p1
                     AND CUST_CODE = @p2
                     AND STOCK_NO = @p3 
                     AND STOCK_SEQ = @p4
                     AND SERIAL_NO = @p5
                     AND RT_NO IS NULL ";
			ExecuteSqlCommand(sql, parms.ToArray());
		}

		public IQueryable<DefectItem> GetDefectInfoByUnRecvFinish(string dcCode, string gupCode, string custCode, string stockNo,int stockSeq)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",dcCode){ SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p1",gupCode){ SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p2",custCode){ SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p3",stockNo){ SqlDbType = System.Data.SqlDbType.VarChar },
				new SqlParameter("@p4",stockSeq){ SqlDbType = System.Data.SqlDbType.Int }
			};

			var sql = @"SELECT B.ID as Id, B.WAREHOUSE_ID WarehouseId,
						(SELECT TOP (1) WAREHOUSE_NAME FROM F1980 WHERE DC_CODE=B.DC_CODE AND WAREHOUSE_ID=B.WAREHOUSE_ID) WarehouseName,
						B.DEFECT_QTY Qty,
						(SELECT TOP(1) CAUSE FROM F1951 WHERE UCT_ID='IC' AND UCC_CODE=B.UCC_CODE) UccName,
						CAUSE as Cause,
						B.SERIAL_NO as SerialNo
						FROM F02020109 B
						WHERE B.DC_CODE =@p0
						AND B.GUP_CODE = @p1
						AND B.CUST_CODE =@p2
						AND B.STOCK_NO = @p3
            AND B.STOCK_SEQ = @p4
            AND B.RT_NO IS NULL ";

			var result = SqlQuery<DefectItem>(sql, param.ToArray());
			return result;
		}

		public F02020109 GetDataById(long defectId)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0", defectId) { SqlDbType = System.Data.SqlDbType.BigInt },
			};
			var sql = @" SELECT TOP(1) *
                     FROM F02020109
                   WHERE ID = @p0 ";
			return SqlQuery<F02020109>(sql, parms.ToArray()).FirstOrDefault();
		}
		public void UpdateWarehouse(string dcCode,string gupCode,string custCode,string stockNo,int stockSeq,string warehouseId)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",warehouseId) { SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1",DateTime.Now) { SqlDbType = SqlDbType.DateTime2},
				new SqlParameter("@p2",Current.Staff) { SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p3",Current.StaffName) { SqlDbType = SqlDbType.NVarChar},
				new SqlParameter("@p4",dcCode) { SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p5",gupCode) { SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p6",custCode) { SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p7",stockNo) { SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p8",stockSeq) { SqlDbType = SqlDbType.Int},

			};
			var sql = @"UPDATE F02020109 SET WAREHOUSE_ID=@p0,UPD_DATE=@p1,UPD_STAFF =@p2,UPD_NAME=@p3
                  WHERE DC_CODE = @p4 AND GUP_CODE =@p5 AND CUST_CODE = @p6 AND STOCK_NO =@p7 AND STOCK_SEQ = @p8  AND RT_NO IS NULL ";
			ExecuteSqlCommand(sql, parms.ToArray());

		}
	}
}
