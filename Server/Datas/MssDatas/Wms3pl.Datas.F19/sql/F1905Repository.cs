using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F1905Repository : RepositoryBase<F1905, Wms3plDbContext, F1905Repository>
	{

        public IQueryable<F1905Data> GetPackCase(string gupCode,string custCode, string itemCode, string itemName = null)
        {
            var paramers = new List<SqlParameter>();

            string gupCodeCondition = string.Empty;
            string custCodeCondition = string.Empty;
            string itemCodeCondition = (string.IsNullOrEmpty(itemCode)) ? string.Empty : "AND a.ITEM_CODE LIKE '" + itemCode + "%' ";
            string itemNameCondition = (string.IsNullOrEmpty(itemName)) ? string.Empty : "AND d.ITEM_NAME LIKE '" + itemName + "%' ";
            if (!string.IsNullOrEmpty(gupCode) && gupCode != "0")
            {
                gupCodeCondition = string.Format("AND a.GUP_CODE = @p{0} ", paramers.Count());
                paramers.Add(new SqlParameter(string.Format("@p{0}", paramers.Count()), gupCode));
            }
						if (!string.IsNullOrEmpty(custCode) && custCode != "0")
						{
							gupCodeCondition = string.Format("AND a.CUST_CODE = @p{0} ", paramers.Count());
							paramers.Add(new SqlParameter(string.Format("@p{0}", paramers.Count()), custCode));
						}
						var strSQL = @"SELECT TOP 50
                                a.ITEM_CODE,d.ITEM_NAME,a.GUP_CODE,b.GUP_NAME,a.PACK_WEIGHT
        									 FROM F1905 a 
        							LEFT JOIN F1929 b on a.GUP_CODE = b.GUP_CODE
        							LEFT JOIN F1903 d on a.GUP_CODE = d.GUP_CODE AND a.ITEM_CODE = d.ITEM_CODE AND a.CUST_CODE = d.CUST_CODE
                                WHERE 1=1 ";
            strSQL += gupCodeCondition;
            strSQL += custCodeCondition;
            strSQL += itemCodeCondition;
            strSQL += itemNameCondition;
            return SqlQuery<F1905Data>(strSQL, paramers.ToArray());
        }

    public IQueryable<F1905> GetF1905ByItemCodes(string gupCode, string custCode, List<string> itemCodes)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", custCode) { SqlDbType = SqlDbType.VarChar }
      };

      var sql = $@"
                SELECT 
                  * 
                FROM 
                  F1905 
                WHERE 
                  GUP_CODE = @p0 
                  AND CUST_CODE = @p1
                ";

      if (itemCodes.Any())
        sql += param.CombineSqlInParameters("AND ITEM_CODE", itemCodes, SqlDbType.VarChar);
      else
        return null;

      return SqlQuery<F1905>(sql, param.ToArray());
    }
		public F1905 GetData(string gupCode, string custCode, string itemCode)
		{
			var para = new List<SqlParameter>
			{
				new SqlParameter("@p0", SqlDbType.VarChar) { Value = gupCode },
				new SqlParameter("@p1", SqlDbType.VarChar) { Value = custCode },
				new SqlParameter("@p2", SqlDbType.VarChar) { Value = itemCode },
			};

			var sql = @"SELECT * FROM F1905 WHERE GUP_CODE = @p0 AND CUST_CODE = @p1 AND ITEM_CODE = @p2";
			return SqlQuery<F1905>(sql, para.ToArray()).FirstOrDefault();
		}

    public IQueryable<F1905> GetF1905ByItems(string gupCode, string custCode, List<string> itemCodes)
    {
      var sql = @"SELECT * FROM F1905 WHERE GUP_CODE = @p0 AND CUST_CODE = @p1";
      var parameters = new List<SqlParameter>
      {
        new SqlParameter("@p0", gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", custCode) { SqlDbType = SqlDbType.VarChar },
      };
      sql += parameters.CombineSqlInParameters(" AND ITEM_CODE", itemCodes, SqlDbType.VarChar);
      return SqlQuery<F1905>(sql, parameters.ToArray());
      #region 原LINQ
      //return _db.F1905s.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && itemCodes.Contains(x.ITEM_CODE));
      #endregion
    }

  }
}
