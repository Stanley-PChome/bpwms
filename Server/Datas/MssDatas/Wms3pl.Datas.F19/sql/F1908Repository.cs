using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
  public partial class F1908Repository : RepositoryBase<F1908, Wms3plDbContext, F1908Repository>
  {

    public IQueryable<F1908> GetDatas(string gupCode, string custCode, List<string> vnrCodes)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", SqlDbType.VarChar) { Value = gupCode },
        new SqlParameter("@p1", SqlDbType.VarChar) { Value = custCode },
      };

      var sql = @"SELECT * FROM F1908 WHERE GUP_CODE=@p0 AND CUST_CODE=@p1";

      sql += para.CombineSqlInParameters(" AND VNR_CODE", vnrCodes, SqlDbType.VarChar);

      return SqlQuery<F1908>(sql, para.ToArray());

      #region 原LINQ語法
      /*
      var query = _db.F1908s
                  .Where(x => x.GUP_CODE == gupCode)
                  .Where(x => x.CUST_CODE == custCode)
                  .Where(x => vnrCodes.Contains(x.VNR_CODE));
      return query.Select(x => x);
      */
      #endregion
    }


		public bool IsVnrExisted(string gupCode, string custCode, string vnrCode)
		{
			var para = new List<SqlParameter>
			{
				new SqlParameter("@p0", SqlDbType.VarChar) { Value = gupCode },
				new SqlParameter("@p1", SqlDbType.VarChar) { Value = custCode },
				new SqlParameter("@p2", SqlDbType.VarChar) { Value = vnrCode },
			};

			var sql = @"SELECT TOP(1) 1 FROM F1908 WHERE GUP_CODE = @p0 AND CUST_CODE = @p1 AND VNR_CODE = @p2 ";

			return SqlQuery<int>(sql, para.ToArray()).Any();
		}
	
    public IQueryable<string> GetDatasReturnVnrCode(string gupCode, string custCode, List<string> vnrCodes)
    {
      var para = new List<SqlParameter>
      {
        new SqlParameter("@p0", SqlDbType.VarChar) { Value = gupCode },
        new SqlParameter("@p1", SqlDbType.VarChar) { Value = custCode },
      };

      if (vnrCodes == null || !vnrCodes.Any())
        return new List<string>().AsQueryable();

      var sql = $@"SELECT VNR_CODE FROM F1908 WHERE GUP_CODE=@p0 AND CUST_CODE=@p1 {para.CombineSqlInParameters("AND VNR_CODE", vnrCodes, SqlDbType.VarChar)}";
      return SqlQueryWithSqlParameterSetDbType<string>(sql, para.ToArray());
    }

    /// <summary>
		/// 取得已過濾人員權限的廠商主檔
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="vnrCode"></param>
		/// <param name="vnrName"></param>
		/// <returns></returns>
    public IQueryable<F1908> GetAllowedF1908s(string gupCode, string vnrCode, string vnrName, string custCode)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", custCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", Current.Staff) { SqlDbType = SqlDbType.VarChar },
      };

      var sql = @"
                SELECT
                  DISTINCT A.*
                FROM F1908 A
                JOIN F192402 B
                  ON A.GUP_CODE=B.GUP_CODE AND A.CUST_CODE=B.CUST_CODE
                JOIN F1909 C
                  ON A.GUP_CODE=C.GUP_CODE AND A.CUST_CODE=C.CUST_CODE
                WHERE
                  C. CUST_CODE = @p0
                  AND A.STATUS != '9'
                  AND B.EMP_ID = @p1
                ";

      if (!string.IsNullOrWhiteSpace(gupCode))
        sql += $" AND A.GUP_CODE = '{gupCode}'";

      if (!string.IsNullOrWhiteSpace(vnrCode))
        sql += $" AND A.VNR_CODE = '{vnrCode}'";

      if (!string.IsNullOrWhiteSpace(vnrName))
        sql += $" AND A.VNR_NAME LIKE '%{vnrName}%'";

      return SqlQuery<F1908>(sql, param.ToArray());
    }
  }
}
