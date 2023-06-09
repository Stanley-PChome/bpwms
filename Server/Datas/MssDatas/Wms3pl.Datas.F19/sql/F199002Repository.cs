using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F199002Repository : RepositoryBase<F199002, Wms3plDbContext, F199002Repository>
    {
        public IQueryable<F199002Data> GetJobValuation(string dcCode, string accItemKindId, string OrdType, string accKind, string accUnit, string status)
        {
            var sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@p0", dcCode)
            };

            var sql = @"SELECT a.*,
												 CASE WHEN a.ACC_KIND = 'A' 
															THEN CONCAT('費用' , a.FEE , '元' )
															ELSE CONCAT('≦計價數量,費用' , a.BASIC_FEE , '元' ,  ',>計價數量,費用'  ,  a.OVER_FEE , '元' )
													END AS FEE_DESCRIBE 
										FROM F199002 a 
									 WHERE a.DC_CODE = @p0 ";
            if (!string.IsNullOrEmpty(accItemKindId))
            {
                sql += string.Format("AND a.ACC_ITEM_KIND_ID = @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(string.Format("@p{0}", sqlParameters.Count), accItemKindId));
            }

            if (!string.IsNullOrEmpty(OrdType))
            {
                sql += string.Format("AND a.ORD_TYPE = @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(string.Format("@p{0}", sqlParameters.Count), OrdType));
            }

            if (!string.IsNullOrEmpty(accKind))
            {
                sql += string.Format("AND a.ACC_KIND = @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(string.Format("@p{0}", sqlParameters.Count), accKind));
            }

            if (!string.IsNullOrEmpty(accUnit))
            {
                sql += string.Format("AND a.ACC_UNIT = @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(string.Format("@p{0}", sqlParameters.Count), accUnit));
            }

            if (!string.IsNullOrEmpty(status))
            {
                sql += string.Format("AND a.STATUS = @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(string.Format("@p{0}", sqlParameters.Count), status));
            }
            else
            {
                sql += "AND a.STATUS <> '9' ";
            }
            var result = SqlQuery<F199002Data>(sql, sqlParameters.ToArray()).AsQueryable();
            return result;
        }
    }
}
