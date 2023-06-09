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
    public partial class F199006Repository : RepositoryBase<F199006, Wms3plDbContext, F199006Repository>
    {
        public F199006Repository(string connName, WmsTransaction wmsTransaction = null)
       : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F199006> GetF199006s(string dcCode, string accItemName, string status)
        {
            var result = _db.F199006s.Where(x=>x.DC_CODE ==dcCode);
            if (!string.IsNullOrWhiteSpace(accItemName) && accItemName != "")
            {
                result = result.Where(x => x.ACC_ITEM_NAME.Contains(accItemName));
            }
            if (status == "")
            {
                result = result.Where(x => x.STATUS != "9");
            }
            else
            {
                result = result.Where(x => x.STATUS == status);
            }

            return result;
        }
    }
}
