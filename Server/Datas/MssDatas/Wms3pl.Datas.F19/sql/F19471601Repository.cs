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
    public partial class F19471601Repository : RepositoryBase<F19471601, Wms3plDbContext, F19471601Repository>
    {
        public IQueryable<F19471601Data> GetF19471601Datas(string gupCode, string custCode, string dcCode, string delvNo)
        {
            var param = new List<object> { gupCode, custCode, dcCode, delvNo };

            var sql = @" 
                      SELECT A.*,B.RETAIL_NAME  FROM F19471601 A , F1910 B , F1909 C
                        WHERE A.GUP_CODE=B.GUP_CODE
                          AND A.GUP_CODE=C.GUP_CODE
                          AND A.CUST_CODE=C.CUST_CODE
                          AND CASE WHEN C.ALLOWGUP_RETAILSHARE = '1' THEN '0' ELSE C.CUST_CODE END = B.CUST_CODE
                          AND A.RETAIL_CODE = B.RETAIL_CODE
                          AND A.GUP_CODE=@p0
                          AND A.CUST_CODE=@p1
                          AND A.DC_CODE=@p2
                          AND A.DELV_NO=@p3
                        ";


            sql += " ORDER BY A.GUP_CODE,A.CUST_CODE,A.DC_CODE,A.DELV_NO,A.RETAIL_CODE ";
            return SqlQuery<F19471601Data>(sql, param.ToArray());
        }
    }
}
