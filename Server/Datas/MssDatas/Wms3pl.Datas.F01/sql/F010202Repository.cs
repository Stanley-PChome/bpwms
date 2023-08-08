using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F01
{
    public partial class F010202Repository : RepositoryBase<F010202, Wms3plDbContext, F010202Repository>
    {
        

        public void BulkDelete(string dcCode, string gupCode, string custCode, List<string> stockNos)
        {
            var parameters = new List<object>
            {
                dcCode,
                gupCode,
                custCode
            };
            
            var sql = $@" DELETE FROM F010202
                    WHERE DC_CODE = @p0
                      AND GUP_CODE = @p1
                      AND CUST_CODE = @p2 ";

            sql += parameters.CombineSqlInParameters(" AND STOCK_NO", stockNos);

            ExecuteSqlCommand(sql, parameters.ToArray());
        }

        public void DeleteF010202(string stockNo, string dcCode, string gupCode, string custCode)
        {
            string sql = @"
				delete from  F010202 Where STOCK_NO=@p0
											and DC_CODE =@p1
											and GUP_CODE =@p2
											and CUST_CODE =@p3
			";
            var sqlParams = new SqlParameter[]
            {
                new SqlParameter("@p0", stockNo),
                new SqlParameter("@p1", dcCode),
                new SqlParameter("@p2", gupCode),
                new SqlParameter("@p3", custCode)
            };
            ExecuteSqlCommand(sql, sqlParams);
        }

		public int GetInboundCnt(string dcCode, string gupCode, string custCode, List<string> stockNos)
		{
			var param = new List<SqlParameter>
												{
																new SqlParameter("@p0", dcCode),
																new SqlParameter("@p1", gupCode),
																new SqlParameter("@p2", custCode)
												};
			var sql = $@"  SELECT COUNT(DISTINCT ITEM_CODE) 
										FROM F010202
										WHERE DC_CODE = @p0
										AND GUP_CODE = @p1
										AND CUST_CODE = @p2
										AND STOCK_NO IN (
										'{ string.Join("','", stockNos) }' )
										 ";

			var result = SqlQuery<int>(sql, param.ToArray()).FirstOrDefault();

			return result;
    }

    public F010202 GetData(string dcCode, string gupCode, string custCode, string stockNo, int stockSeq)
    {
      var para = new List<SqlParameter>()
      {
        new SqlParameter("@p0",dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1",gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2",custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p3",stockNo) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p4",stockSeq) { SqlDbType = SqlDbType.Int }
      };
      var sql = @"SELECT TOP(1) * FROM F010202 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND STOCK_NO=@p3 AND STOCK_SEQ=@p4";
      return SqlQuery<F010202>(sql, para.ToArray()).FirstOrDefault();
    }
  }
}
