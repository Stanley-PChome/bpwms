using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F192404Repository : RepositoryBase<F192404, Wms3plDbContext, F192404Repository>
    {
        public F192404Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F192404> GetF192404Datas(string DC_CODE)
        {
            var result = _db.F192404s.Where(x => x.DC_CODE == DC_CODE);
            return result;
        }

    }
}
