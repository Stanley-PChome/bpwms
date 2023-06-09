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
    public partial class F199005Repository : RepositoryBase<F199005, Wms3plDbContext, F199005Repository>
    {
        public IQueryable<F199005Data> GetF199005(string dcCode, string accItemKindId, string logiType, string taxType,
           string accKind, string isSpecialCar, string status)
        {
            var sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@p0", dcCode)
            };

            var sql = @"SELECT    A.*,B.ACC_UNIT_NAME ACC_UNIT_TEXT
								FROM    F199005 A
								LEFT JOIN F91000302 B ON A.ACC_UNIT = B.ACC_UNIT AND B.ITEM_TYPE_ID = '005'
								WHERE	DC_CODE = @p0 ";

            if (!String.IsNullOrEmpty(accItemKindId))
            {
                sql += String.Format("AND A.ACC_ITEM_KIND_ID = @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(String.Format("@p{0}", sqlParameters.Count), accItemKindId));
            }

            if (!String.IsNullOrEmpty(logiType))
            {
                sql += String.Format("AND A.LOGI_TYPE = @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(String.Format("@p{0}", sqlParameters.Count), logiType));
            }

            if (!String.IsNullOrEmpty(taxType))
            {
                sql += String.Format("AND A.IN_TAX = @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(String.Format("@p{0}", sqlParameters.Count), taxType));
            }

            if (!String.IsNullOrEmpty(accKind))
            {
                sql += String.Format("AND A.ACC_KIND = @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(String.Format("@p{0}", sqlParameters.Count), accKind));
            }

            if (!String.IsNullOrEmpty(isSpecialCar) && isSpecialCar != "-1")
            {
                sql += String.Format("AND A.IS_SPECIAL_CAR = @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(String.Format("@p{0}", sqlParameters.Count), isSpecialCar));
            }

            if (!String.IsNullOrEmpty(status))
            {
                sql += String.Format("AND A.STATUS = @p{0} ", sqlParameters.Count);
                sqlParameters.Add(new SqlParameter(String.Format("@p{0}", sqlParameters.Count), status));
            }

            sql += "ORDER BY A.CRT_DATE ASC";
            var result = SqlQuery<F199005Data>(sql, sqlParameters.ToArray()).AsQueryable();
            return result;
        }
    }
}
