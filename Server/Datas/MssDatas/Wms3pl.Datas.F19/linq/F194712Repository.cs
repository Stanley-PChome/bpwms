using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System.Data.SqlClient;

namespace Wms3pl.Datas.F19
{
    public partial class F194712Repository : RepositoryBase<F194712, Wms3plDbContext, F194712Repository>
    {
        public F194712Repository(string connName, WmsTransaction wmsTransaction = null)
         : base(connName, wmsTransaction)
        {
        }

        public F194712 Get(string dcCode, string gupCode, string custCode, string channel, string allId, string consignType = null)
        {
            var result = _db.F194712s.Where(x => x.DC_CODE == dcCode
                                            && x.GUP_CODE == gupCode
                                            && x.CUST_CODE == custCode
                                            && x.CHANNEL == channel
                                            && x.ALL_ID == allId);
            if (!string.IsNullOrWhiteSpace(consignType))
            {
                result = result.Where(x => x.CONSIGN_TYPE == consignType);
            }

#if DEBUG
            result = result.Where(x => x.ISTEST == "1");
#else
             result = result.Where(x => x.ISTEST == "0");
#endif
            return result.FirstOrDefault();
        }
    }
}
