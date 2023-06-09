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
    public partial class F1933Repository : RepositoryBase<F1933, Wms3plDbContext, F1933Repository>
    {
        public F1933Repository(string connName, WmsTransaction wmsTransaction = null)
      : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F1933> GetF1933Datas(string COUDIV_ID)
        {

            var result = _db.F1933s.Where(x => x.COUDIV_ID == COUDIV_ID).OrderBy(x=>x.COUDIV_ID);
            return result;
        }
    }
}
