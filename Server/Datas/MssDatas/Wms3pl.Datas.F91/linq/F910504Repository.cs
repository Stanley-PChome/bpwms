using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F91
{
    public partial class F910504Repository : RepositoryBase<F910504, Wms3plDbContext, F910504Repository>
    {
        public F910504Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {

        }

        public IQueryable<SerialCheckData> GetF910504ScanLog(string dcCode, string gupCode, string custCode, string processNo, string clientIp)
        {
            var q = _db.F910504s.Where(c => c.DC_CODE == dcCode
            && c.GUP_CODE == gupCode
            && c.CUST_CODE == custCode
            && c.PROCESS_NO == processNo
            && c.PROCESS_IP == clientIp
            && c.STATUS == "0")
            .GroupJoin(_db.F1903s,
            a => new { a.GUP_CODE, a.CUST_CODE, a.ITEM_CODE},
            b => new { b.GUP_CODE, b.CUST_CODE, b.ITEM_CODE},
            (a,b) => new { a, b})
            .SelectMany(x => x.b.DefaultIfEmpty(),(x,b)=>new { f910504 = x.a, f1903 = b})
            .Select(s => new SerialCheckData
            {
               PROCESS_NO = s.f910504.PROCESS_NO,
               LOG_SEQ = s.f910504.LOG_SEQ,
               PROCESS_IP = s.f910504.PROCESS_IP,
               ITEM_CODE = s.f910504.ITEM_CODE,
               ITEM_NAME = s.f1903.ITEM_NAME,
               SERIAL_NO = s.f910504.SERIAL_NO,
               SERIAL_STATUS = s.f910504.SERIAL_STATUS,
               COMBIN_NO = Convert.ToInt32( s.f910504.COMBIN_NO),
               STATUS = s.f910504.STATUS,
               ISPASS = s.f910504.ISPASS,
               MESSAGE = s.f910504.MESSAGE,
               DC_CODE = s.f910504.DC_CODE,
               GUP_CODE = s.f910504.GUP_CODE,
               CUST_CODE = s.f910504.CUST_CODE
            });

            return q;
        }
    }
}
