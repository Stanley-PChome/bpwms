using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F194705Repository : RepositoryBase<F194705, Wms3plDbContext, F194705Repository>
    {
        public IQueryable<Route> GetRoutes(List<string> zipCodes, string allId, string dcCode)
        {
            var parameters = new List<object> {
                allId,
                dcCode
            };
            int paramStartIndex = parameters.Count;
            var inSql = parameters.CombineSqlInParameters("C.ZIP_CODE", zipCodes, ref paramStartIndex);
            string sql = @"
				Select Distinct A.ALL_ID AllId,A.ROUTE_CODE RouteCode, A.ROUTE RouteName, A.DELV_TIMES DelvTimes, C.ZIP_CODE ZipCode,
                   ROW_NUMBER () OVER(ORDER BY A.DC_CODE,A.ROUTE_NO) ROWNUM
				  From F194705 A
				  Join F1933 B On A.ADDRESS_A=B.COUDIV_NAME
				  Join F1934 C On (A.ADDRESS_B=C.ZIP_NAME Or A.ADDRESS_A=C.ZIP_NAME) And B.COUDIV_ID=C.COUDIV_ID
				 Where A.ALL_ID=@p0
				   And A.DC_CODE=@p1
				   And " + inSql;

            var result = SqlQuery<Route>(sql, parameters.ToArray()).ToList();
            return result.AsQueryable();
        }

        public IQueryable<Route> GetRoutes(string allId, string dcCode)
        {
            var parameters = new List<object>();
            parameters.Add(dcCode);
            if (!string.IsNullOrEmpty(allId))
                parameters.Add(allId);

            string sql = string.Format(@"
				Select Distinct A.ALL_ID AllId,A.ROUTE_CODE RouteCode, A.ROUTE RouteName, A.DELV_TIMES DelvTimes, C.ZIP_CODE ZipCode,
				  ROW_NUMBER () OVER(ORDER BY A.DC_CODE,A.ROUTE_NO) ROWNUM
                    From F194705 A
				  Join F1933 B On A.ADDRESS_A=B.COUDIV_NAME
				  Join F1934 C On (A.ADDRESS_B=C.ZIP_NAME Or A.ADDRESS_A=C.ZIP_NAME) And B.COUDIV_ID=C.COUDIV_ID
				 Where A.DC_CODE=@p0 {0} ", string.IsNullOrEmpty(allId) ? "" : "And A.ALL_ID=@p1");

            var result = SqlQuery<Route>(sql, parameters.ToArray()).ToList();
            return result.AsQueryable();
        }
    }
}
