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
    public partial class F910506Repository : RepositoryBase<F910506, Wms3plDbContext, F910506Repository>
    {
        public F910506Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {

        }
        
        

        public IQueryable<SerialCheckData> GetF910506ScanLog(string dcCode, string gupCode, string custCode, string processNo, string clientIp)
        {
            var q = _db.F910506s.Where(c => c.DC_CODE == dcCode
            && c.GUP_CODE == gupCode
            && c.CUST_CODE == custCode
            && c.PROCESS_NO == processNo
            && c.PROCESS_IP == clientIp
            && c.STATUS == "0")
            .Select(s => new SerialCheckData
            {
                PROCESS_NO = s.PROCESS_NO,
                LOG_SEQ = s.LOG_SEQ,
                PROCESS_IP = s.PROCESS_IP,
                SERIAL_NO = s.SERIAL_NO,
                SERIAL_STATUS = s.SERIAL_STATUS,
                BOX_NO = s.BOX_NO,
                STATUS = s.STATUS,
                ISPASS = s.ISPASS,
                MESSAGE = s.MESSAGE,
                DC_CODE = s.DC_CODE,
                GUP_CODE = s.GUP_CODE,
                CUST_CODE = s.CUST_CODE,
                WEIGHT = s.WEIGHT.HasValue ? Convert.ToDouble(s.WEIGHT.Value) : default(double?)
            });
            
            return q;
        }
    }
}
