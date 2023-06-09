using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F06
{
    public partial class F060301Repository : RepositoryBase<F060301, Wms3plDbContext, F060301Repository>
    {
        public F060301Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<WcsUserExecuteModel> GetWcsExecuteDatas(string dcCode, List<string> statusList, int midApiRelmt)
        {
            var f060301s = _db.F060301s.Where(x => x.DC_CODE == dcCode &&
            statusList.Contains(x.STATUS) &&
            x.RESENT_CNT < midApiRelmt);

            var f1924s = _db.F1924s.AsNoTracking().Where(x => f060301s.Select(z => z.EMP_ID).Contains(x.EMP_ID));

            var result = from A in f060301s
                         join B in f1924s
                         on A.EMP_ID equals B.EMP_ID into subB
                         from B in subB.DefaultIfEmpty()
                         select new WcsUserExecuteModel
                         {
                             F060301 = A,
                             UserData = new WcsUserModel
                             {
                                 UserId = A.EMP_ID,
                                 UserName = B.EMP_NAME,
                                 UserPw = "a12345",
                                 Status = A.CMD_TYPE == "1" ? 1 : 0
                             }
                         };

            return result;
        }

        public IQueryable<F060301> GetDatas(string empId)
        {
            return _db.F060301s.Where(x => x.EMP_ID == empId && x.STATUS == "0");
        }
    }
}
