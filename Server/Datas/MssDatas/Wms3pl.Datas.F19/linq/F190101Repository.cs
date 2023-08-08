using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F190101Repository : RepositoryBase<F190101, Wms3plDbContext, F190101Repository>
    {
        public F190101Repository(string connName, WmsTransaction wmsTransaction = null)
          : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F190101> GetDcCustList(string dcCode, string gupCode, string custCode)
        {
            var data = _db.F190101s.AsQueryable();

            if (!string.IsNullOrWhiteSpace(dcCode))
            {
                data = data.Where(x => x.DC_CODE == dcCode);
            }

            if (!string.IsNullOrWhiteSpace(gupCode))
            {
                data = data.Where(x => x.GUP_CODE == gupCode);
            }

            if (!string.IsNullOrWhiteSpace(custCode))
            {
                data = data.Where(x => x.CUST_CODE == custCode);
            }

            return data;
        }
    }
}
