using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F91
{
    public partial class F910201Repository : RepositoryBase<F910201, Wms3plDbContext, F910201Repository>
    {
        

        public IQueryable<ProcessItem> GetProcessItemsNonBom(string dcCode, string gupCode, string custCode, string processNo)
        {
            var parms = new List<object> { dcCode, gupCode, custCode, processNo };
            var sql = $@"Select A.ITEM_CODE, B.ITEM_SPEC, B.ITEM_COLOR, B.ITEM_SIZE, B.ITEM_NAME
				  From F910201 A
				  Join F1903 B On A.ITEM_CODE=B.ITEM_CODE And A.GUP_CODE=B.GUP_CODE And A.CUST_CODE=B.CUST_CODE
				 Where A.DC_CODE=@p0
				   And A.GUP_CODE=@p1
				   And A.CUST_CODE=@p2
				   And A.PROCESS_NO=@p3";

            return SqlQuery<ProcessItem>(sql, parms.ToArray());
        }
    }
}
