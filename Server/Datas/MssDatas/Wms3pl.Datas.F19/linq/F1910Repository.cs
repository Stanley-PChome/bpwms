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
    public partial class F1910Repository : RepositoryBase<F1910, Wms3plDbContext, F1910Repository>
    {
        public F1910Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F1910> GetDatas(string gupCode, string custCode, List<string> retailCodes)
        {
            return _db.F1910s
                    .Where(x => x.GUP_CODE == gupCode)
                    .Where(x => x.CUST_CODE == custCode)
                    .Where(x => retailCodes.Contains(x.RETAIL_CODE))
                    .Select(x => x);
        }

        public IQueryable<F1910> GetDatasForRetail(string gupCode, string custCode, List<string> retailCodes)
        {
            return _db.F1910s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
                                                        x.CUST_CODE == custCode &&
                                                        retailCodes.Contains(x.RETAIL_CODE) &&
                                                        x.CHANNEL == "00");
        }
    }
}
