using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
    public partial class F0205Repository : RepositoryBase<F0205, Wms3plDbContext, F0205Repository>
    {
        public F0205Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F0205> GetDatasByF020502s(string dcCode, string gupCode, string custCode, string typeCode, List<F020502> f020502List)
        {
            return _db.F0205s.AsNoTracking().Where(x =>
            x.DC_CODE == dcCode &&
            x.GUP_CODE == gupCode &&
            x.CUST_CODE == custCode &&
            x.TYPE_CODE == typeCode &&
            x.STATUS == "0" &&
            f020502List.Any(z => z.RT_NO == x.RT_NO && z.RT_SEQ == x.RT_SEQ));
        }

        public ExecuteResult UpdataF0205StatusTo1(string dcCode, string gupCode, string custCode, string RTNo, string RTSeq)
        {
            UpdateFields(new { STATUS = "1" }, x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.RT_NO == RTNo && x.RT_SEQ == RTSeq);
            return new ExecuteResult(true);
        }

    }
}
