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
    public partial class F197002Repository : RepositoryBase<F197002, Wms3plDbContext, F197002Repository>
    {
        public F197002Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
        {
        }

        public F197002 GetPalletOrBoxNo(string year, string lableType)
        {
            return _db.F197002s.Where(x => x.YEAR == year).FirstOrDefault();
        }
    }
}
