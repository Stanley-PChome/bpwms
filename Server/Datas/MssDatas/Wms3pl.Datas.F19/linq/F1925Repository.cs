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
    public partial class F1925Repository : RepositoryBase<F1925, Wms3plDbContext, F1925Repository>
    {
        public F1925Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F1925> GetF1925Datas(string DEP_ID, string DEP_NAME)
        {
            var result = _db.F1925s.AsQueryable();
            if (!string.IsNullOrWhiteSpace(DEP_ID))
            {
                result = result.Where(x => x.DEP_ID == DEP_ID);
            }

            if (!string.IsNullOrWhiteSpace(DEP_NAME))
            {
                result = result.Where(x => x.DEP_NAME.Contains(DEP_NAME));
            }

            return result;
        }
    }
}
