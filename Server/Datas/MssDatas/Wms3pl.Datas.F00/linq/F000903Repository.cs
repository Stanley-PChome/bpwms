using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
namespace Wms3pl.Datas.F00
{
    public partial class F000903Repository : RepositoryBase<F000903, Wms3plDbContext, F000903Repository>
    {
        public F000903Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F000903> GetDatasStartsWithOrdProp(string ordProp)
        {
            return _db.F000903s.Where(x => x.ORD_PROP.StartsWith(ordProp));
        }
    }
}
