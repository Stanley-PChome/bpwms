using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F02;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F01
{
  public partial class F010204Repository : RepositoryBase<F010204, Wms3plDbContext, F010204Repository>
  {

    public F010204 GetData(string dcCode,string gupCode,string custCode,string stockNo,int stockSeq)
    {
      var para = new List<SqlParameter>()
      {
        new SqlParameter("@p0",dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1",gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2",custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3",stockNo) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p4",stockSeq) { SqlDbType = SqlDbType.Int }
      };
      var sql = @"SELECT TOP(1) * FROM F010204 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND STOCK_NO=@p3 AND STOCK_SEQ=@p4";
      return SqlQuery<F010204>(sql, para.ToArray()).FirstOrDefault();
    }

		public IQueryable<F010204> GetDatasByStockNos(string dcCode, string gupCode, string custCode, List<string> stockNos)
		{
			var para = new List<SqlParameter>()
			{
				new SqlParameter("@p0",dcCode) { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p1",gupCode) { SqlDbType = SqlDbType.VarChar },
				new SqlParameter("@p2",custCode) { SqlDbType = SqlDbType.VarChar },
			};
			var sql = @"SELECT * FROM F010204 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 ";
			sql += para.CombineSqlInParameters(" AND STOCK_NO ", stockNos, SqlDbType.VarChar);
			return SqlQuery<F010204>(sql, para.ToArray());
		}
		public void InsertNotExistDatas(string dcCode,string gupCode,string custCode,List<string> stockNos)
		{
			var para = new List<SqlParameter>
			{
				new SqlParameter("@p0", DateTime.Now){ SqlDbType = SqlDbType.DateTime2},
				new SqlParameter("@p1",Current.Staff){ SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p2",Current.StaffName){SqlDbType = SqlDbType.NVarChar},
				new SqlParameter("@p3",dcCode){SqlDbType =  SqlDbType.VarChar},
				new SqlParameter("@p4",gupCode){SqlDbType =  SqlDbType.VarChar},
				new SqlParameter("@p5",custCode){SqlDbType =  SqlDbType.VarChar},

			};
			var sql = @" INSERT INTO F010204(DC_CODE,GUP_CODE,CUST_CODE,STOCK_NO,STOCK_SEQ,ITEM_CODE,STOCK_QTY,TOTAL_REC_QTY,TOTAL_DEFECT_RECV_QTY,TOTAL_TAR_QTY,TOTAL_DEFECT_TAR_QTY,CRT_DATE,CRT_STAFF,CRT_NAME)
										SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.STOCK_NO,A.STOCK_SEQ,A.ITEM_CODE,A.STOCK_QTY,0 TOTAL_REC_QTY,0 TOTAL_DEFECT_RECV_QTY, 0 TOTAL_TAR_QTY,0 TOTAL_DEFECT_TAR_QTY, @p0 CRT_DATE,@p1 CRT_STAFF,@p2 CRT_NAME
										FROM F010202 A
										LEFT JOIN F010204 B
										ON B.DC_CODE = A.DC_CODE
										AND B.GUP_CODE = A.GUP_CODE
										AND B.CUST_CODE = A.CUST_CODE
										AND B.STOCK_NO = A.STOCK_NO
										AND B.STOCK_SEQ = A.STOCK_SEQ
										WHERE A.DC_CODE =@p3
										AND A.GUP_CODE = @p4
										AND A.CUST_CODE = @p5
										AND B.DC_CODE IS NULL ";
			sql += para.CombineSqlInParameters(" AND A.STOCK_NO", stockNos, SqlDbType.VarChar);
			ExecuteSqlCommand(sql, para.ToArray());
		}
	}
}
