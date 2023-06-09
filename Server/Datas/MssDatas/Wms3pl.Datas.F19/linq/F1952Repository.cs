using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1952Repository : RepositoryBase<F1952, Wms3plDbContext, F1952Repository>
    {
        public F1952Repository(string connName, WmsTransaction wmsTransaction = null)
         : base(connName, wmsTransaction)
        {
        }

        public F1952Ex GetF1952Ex(string empId)
        {
            var f1924s = _db.F1924s.AsNoTracking().Where(x => x.EMP_ID == empId);
            var f1952s = _db.F1952s.AsNoTracking();
            var result = (from A in f1924s
                          join B in f1952s on A.EMP_ID equals B.EMP_ID into BB
                          from B in BB.DefaultIfEmpty()
                          select new F1952Ex
                          {
                              EMP_ID = A.EMP_ID,
                              LAST_PASSWORD_CHANGED_DATE = B.LAST_PASSWORD_CHANGED_DATE,
                              LAST_ACTIVITY_DATE = B.LAST_ACTIVITY_DATE,
                              STATUS = B.STATUS,
                              ISCOMMON = A.ISCOMMON
                          }).Take(1);
            return result.SingleOrDefault();
        }

        public GetValidateUser ValidateUser(string AccNo)
        {
            var f1952s = _db.F1952s.AsNoTracking().Where(x => x.EMP_ID == AccNo);
            var f1924s = _db.F1924s.AsNoTracking().Where(x=>x.ISDELETED == "0");
            var result = from A in f1952s
                         join B in f1924s on A.EMP_ID equals B.EMP_ID
                         select new GetValidateUser
                         {
                             EmpId = A.EMP_ID,
                             Password = A.PASSWORD,
                             CrtDate = A.CRT_DATE,
                             UpdDate = A.UPD_DATE,
                             CrtStaff = A.CRT_STAFF,
                             UpdStaff = A.UPD_STAFF,
                             LastActivityDate = A.LAST_ACTIVITY_DATE,
                             LastLoginDate = A.LAST_LOGIN_DATE,
                             LastPasswordChangedDate = A.LAST_PASSWORD_CHANGED_DATE,
                             LastLockoutDate = A.LAST_LOCKOUT_DATE,
                             FailedPasswordAttemptCount = decimal.ToInt32(A.FAILED_PASSWORD_ATTEMPT_COUNT.Value),
                             Status = decimal.ToInt32(A.STATUS.Value),
                             CrtName = A.CRT_NAME,
                             UpdName = A.UPD_NAME,
                         };
            return result.SingleOrDefault();
        }
    }
}
