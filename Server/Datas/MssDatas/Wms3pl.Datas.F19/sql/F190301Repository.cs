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
	public partial class F190301Repository : RepositoryBase<F190301, Wms3plDbContext, F190301Repository>
	{
		public IQueryable<ItemUnit> GetItemUnits(string gupCode,List<string> itemCodes)
		{
			var sql = @" SELECT A.GUP_CODE,
							            A.ITEM_CODE,
							            A.UNIT_LEVEL,
							            A.UNIT_QTY,
                          A.UNIT_ID,
                          A.LENGTH,
                          A.WIDTH,
                          A.HIGHT HEIGHT,
                          A.WEIGHT,
							            B.ACC_UNIT_NAME UNIT_NAME
						         FROM F190301 A
								     JOIN F91000302 B ON A.UNIT_ID = B.ACC_UNIT AND B.ITEM_TYPE_ID = '001'
						        WHERE A.GUP_CODE = @p0 ";
			var paramList = new List<object> { gupCode };
			sql += paramList.CombineSqlInParameters("AND A.ITEM_CODE", itemCodes);
			return SqlQuery<ItemUnit>(sql, paramList.ToArray());

		}

		public IQueryable<F190301> GetF190301s(string gupCode, string custCode, string itemCode, string itemUnit)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", gupCode));
			sqlParameter.Add(new SqlParameter("@p1", custCode));
			sqlParameter.Add(new SqlParameter("@p2", itemCode));
			sqlParameter.Add(new SqlParameter("@p3", itemUnit));

			var sql = @" SELECT * FROM F190301
							WHERE GUP_CODE = @p0 
								AND CUST_CODE = @p1
								AND ITEM_CODE = @p2
								AND UNIT_ID = @p3 ";

			return SqlQuery<F190301>(sql, sqlParameter.ToArray());
		}

    public IQueryable<F190301> GetDatas(string gupCode, string custCode, List<string> itemCodes, List<string> unitIds)
    {
      var parameters = new List<SqlParameter>
      {
        new SqlParameter("@p0", gupCode) { SqlDbType = System.Data.SqlDbType.VarChar },
        new SqlParameter("@p1", custCode) { SqlDbType = System.Data.SqlDbType.VarChar },
      };
      var sql = @"SELECT * FROM F190301 WHERE GUP_CODE = @p0 AND CUST_CODE = @p1";
      sql += parameters.CombineSqlInParameters(" AND ITEM_CODE", itemCodes, SqlDbType.VarChar);
      sql += parameters.CombineSqlInParameters(" AND UNIT_ID", unitIds, SqlDbType.VarChar);
      return SqlQuery<F190301>(sql, parameters.ToArray());
      #region 原LINQ
      //return _db.F190301s.Where(x => x.GUP_CODE == gupCode
      //                             && x.CUST_CODE == custCode
      //                             &&
      //                               itemCodes.Contains(x.ITEM_CODE) &&
      //                               unitIds.Contains(x.UNIT_ID));
      #endregion
    }
  }
}
