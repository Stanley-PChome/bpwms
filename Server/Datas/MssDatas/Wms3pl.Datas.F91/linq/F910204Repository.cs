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
    public partial class F910204Repository : RepositoryBase<F910204, Wms3plDbContext, F910204Repository>
    {
        public F910204Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<ProcessAction> GetProcessActions(string dcCode, string gupCode, string custCode, string processNo)
        {
            var q = (from a in _db.F910204s
                    where a.DC_CODE == dcCode
                    && a.GUP_CODE == gupCode
                    && a.CUST_CODE == custCode
                    && a.PROCESS_NO == processNo
                    select new ProcessAction
                    {
                        ACTION_NO = a.ACTION_NO,
                        SORT = (a.ACTION_NO == "A1" ? 1 :
                        a.ACTION_NO == "A8" ? 2 :
                        a.ACTION_NO == "A2" ? 3 :
                        a.ACTION_NO == "A3" ? 3 :
                        a.ACTION_NO == "A4" ? 4 :
                        a.ACTION_NO == "A6" ? 5 :
                        a.ACTION_NO == "A5" ? 6 : 7)
                    }).Distinct();
            return q;
        }
	

	}
}
