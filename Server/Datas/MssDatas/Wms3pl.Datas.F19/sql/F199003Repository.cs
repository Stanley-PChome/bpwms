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
    public partial class F199003Repository : RepositoryBase<F199003, Wms3plDbContext, F199003Repository>
    {
        public IQueryable<F199003Data> GetShippingValuation(string dcCode, string accItemKindId, string accKind, string status)
        {
            var sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@p0", dcCode)
            };

            var sql = $@"SELECT a.*,
														CASE WHEN a.ACC_KIND = 'A' 
																			THEN CONCAT('費用' , a.FEE , '元' )
																			ELSE CONCAT('≦計價數量,費用' , a.BASIC_FEE , '元' ,  ',>計價數量,費用'  ,  a.OVER_FEE , '元' )
															END AS FEE_DESCRIBE,c.NAME AS DELV_ACC_TYPE_NAME
										FROM F199003 a 
							 LEFT JOIN F91000301 b ON a.ITEM_TYPE_ID = b.ITEM_TYPE_ID AND a.ACC_ITEM_KIND_ID = b.ACC_ITEM_KIND_ID AND a.DELV_ACC_TYPE = b.DELV_ACC_TYPE
							 LEFT JOIN VW_F000904_LANG c ON c.TOPIC = 'F91000301' AND c.SUBTOPIC = 'DELV_ACC_TYPE' AND b.DELV_ACC_TYPE = c.VALUE AND c.LANG = '{Current.Lang}'
									 WHERE a.DC_CODE = @p0 ";
            if (!string.IsNullOrEmpty(accItemKindId))
            {
                sql += string.Format("AND a.ACC_ITEM_KIND_ID = @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(string.Format("@p{0}", sqlParameters.Count), accItemKindId));
            }

            if (!string.IsNullOrEmpty(accKind))
            {
                sql += string.Format("AND a.ACC_KIND = @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(string.Format("@p{0}", sqlParameters.Count), accKind));
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

            var result = SqlQuery<F199003Data>(sql, sqlParameters.ToArray()).AsQueryable();
            return result;
        }
    }
}
