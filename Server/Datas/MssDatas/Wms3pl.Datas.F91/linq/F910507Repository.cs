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
    public partial class F910507Repository : RepositoryBase<F910507, Wms3plDbContext, F910506Repository>
    {
        public F910507Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {

        }

      
        
        

        public IQueryable<SerialCheckData> GetF910507ScanLog(string dcCode, string gupCode, string custCode, string processNo, string clientIp)
        {
            var q = _db.F910507s.Where(c => c.DC_CODE == dcCode
            && c.CUST_CODE == custCode
            && c.GUP_CODE == gupCode
            && c.PROCESS_NO == processNo
            && c.PROCESS_IP == clientIp
            && c.STATUS == "0")
            .Select(s => new SerialCheckData
            {
                PROCESS_NO = s.PROCESS_NO,
                LOG_SEQ = s.LOG_SEQ,
                PROCESS_IP = s.PROCESS_IP,
                BOX_NO = s.BOX_NO,
                CASE_NO = s.CASE_NO,
                STATUS = s.STATUS,
                ISPASS = s.ISPASS,
                MESSAGE = s.MESSAGE,
                DC_CODE = s.DC_CODE,
                GUP_CODE = s.GUP_CODE,
                CUST_CODE = s.CUST_CODE,
                WEIGHT = Convert.ToDouble(s.WEIGHT)
            });

            return q;
        }
    }
}
