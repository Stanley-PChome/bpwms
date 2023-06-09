using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
    public partial class F02020108Repository : RepositoryBase<F02020108, Wms3plDbContext, F02020108Repository>
	{
		public F02020108Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

        public IQueryable<F02020108> GetDatasForAllocationNo(string dcCode, string gupCode, string custCode, string allocationNo, List<short> orgSeqs)
        {
            return _db.F02020108s.Where(x => x.DC_CODE == dcCode &&
                                                            x.GUP_CODE == gupCode &&
                                                            x.CUST_CODE == custCode &&
                                                            x.ALLOCATION_NO == allocationNo &&
                                                            orgSeqs.Contains(x.ALLOCATION_SEQ));
        }

        public IQueryable<F02020108> GetDatasForStockNo(string dcCode, string gupCode, string custCode, string stockNo)
        {
            return _db.F02020108s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                            x.GUP_CODE == gupCode &&
                                                            x.CUST_CODE == custCode &&
                                                            x.STOCK_NO == stockNo);
        }
    }
}
