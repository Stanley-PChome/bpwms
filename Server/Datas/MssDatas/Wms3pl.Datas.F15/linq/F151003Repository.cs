using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F15
{
	public partial class F151003Repository : RepositoryBase<F151003, Wms3plDbContext, F151003Repository>
	{
		public F151003Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

        public IQueryable<F151003> GetF151003sByLackType(string dcCode, string gupCode, string custCode, string allocationNo, short allocationSeq, string itemCode, string lackType)
        {
            var result = _db.F151003s.Where(x => x.DC_CODE == dcCode
                                              && x.GUP_CODE == gupCode
                                              && x.CUST_CODE == custCode
                                              && x.ALLOCATION_NO == allocationNo
                                              && x.ALLOCATION_SEQ == allocationSeq
                                              && x.ITEM_CODE == itemCode
                                              && x.LACK_TYPE == lackType
                                              && (x.STATUS == "0" || x.STATUS == "2"));
            return result;
        }

        public IQueryable<F151003> GetDatasByF051206LackListAllot(List<F051206LackList_Allot> data, List<string> status)
        {
            var result = _db.F151003s.AsNoTracking().Where(x => data.Any(z => z.ALLOCATION_NO == x.ALLOCATION_NO &&
            z.ALLOCATION_SEQ == x.ALLOCATION_SEQ) &&
            status.Contains(x.STATUS));

            return result;
        }
    }
}
