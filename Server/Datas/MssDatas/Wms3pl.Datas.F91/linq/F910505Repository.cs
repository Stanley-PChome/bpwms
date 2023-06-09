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
    public partial class F910505Repository : RepositoryBase<F910505, Wms3plDbContext, F910505Repository>
    {
        public F910505Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {

        }

        public IQueryable<SerialCheckData> GetF910505ScanLog(string dcCode, string gupCode, string custCode, string processNo, string clientIp)
        {

            var q = _db.F910505s.Where(c => c.DC_CODE == dcCode
            && c.GUP_CODE == gupCode
            && c.CUST_CODE == custCode
            && c.PROCESS_NO == processNo
            && c.PROCESS_IP == clientIp
            && c.STATUS == "0")
            .GroupJoin(_db.F1903s,
            a => new { a.GUP_CODE, a.CUST_CODE, a.ITEM_CODE },
            b => new { b.GUP_CODE, b.CUST_CODE, b.ITEM_CODE },
            (a, b) => new { a, b })
            .SelectMany(x => x.b.DefaultIfEmpty(), (x, b) => new { f910505 = x.a, f1903 = b })
            .Select(c => new SerialCheckData
            {
                PROCESS_NO = c.f910505.PROCESS_NO,
                LOG_SEQ = c.f910505.LOG_SEQ,
                PROCESS_IP = c.f910505.PROCESS_IP,
                ITEM_CODE = c.f910505.ITEM_CODE,
                ITEM_NAME = c.f1903.ITEM_NAME,
                SERIAL_NO = c.f910505.SERIAL_NO,
                SERIAL_STATUS = c.f910505.SERIAL_STATUS,
                COMBIN_NO = c.f910505.COMBIN_NO,
                STATUS = c.f910505.STATUS,
                ISPASS = c.f910505.ISPASS,
                MESSAGE = c.f910505.MESSAGE,
                DC_CODE = c.f910505.DC_CODE,
                GUP_CODE = c.f910505.GUP_CODE,
                CUST_CODE = c.f910505.CUST_CODE,
                ITEM_COLOR = c.f1903.ITEM_COLOR,
                ITEM_SIZE = c.f1903.ITEM_SIZE
            });
           
            return q;
        }

        //取得拆解作業同組內尚未刷讀的序號
        public IQueryable<SerialNoResult> GetDisassembleNonScan(string dcCode, string gupCode, string custCode, string processNo, string serialNo)
        {
            var f910505_SERIAL_NOs = _db.F910505s.Where(c => c.DC_CODE == dcCode
            && c.GUP_CODE == gupCode
            && c.CUST_CODE == custCode
            && c.PROCESS_NO == processNo
            && c.ISPASS == "1"
            && c.STATUS == "0"
            && !string.IsNullOrEmpty(c.SERIAL_NO)).Select(s => s.SERIAL_NO);

            var q = _db.F2501s.Where(c => c.GUP_CODE == gupCode
            && c.CUST_CODE == custCode
            && !c.SERIAL_NO.Equals(serialNo)
            && !f910505_SERIAL_NOs.Contains(c.SERIAL_NO))
            .Join(_db.F2501s,
            a => new { p0 = a.GUP_CODE, p1 = a.CUST_CODE, p2 = a.COMBIN_NO, p3 = serialNo },
            b => new { p0 = b.GUP_CODE, p1 = b.CUST_CODE, p2 = b.COMBIN_NO, p3 = b.SERIAL_NO },
            (a, b) => new { f2501A = a, f2501B = b })
            .Join(_db.F1903s,
            a => new { a.f2501A.GUP_CODE, a.f2501A.CUST_CODE, a.f2501A.ITEM_CODE },
            b => new { b.GUP_CODE, b.CUST_CODE, b.ITEM_CODE },
            (a, b) => new { a, f1903 = b })
            .Select(c => new SerialNoResult
            {
                ItemCode = c.a.f2501A.ITEM_CODE,
                ItemName = c.f1903.ITEM_NAME,
                SerialNo = c.a.f2501A.SERIAL_NO,
                CurrentlyStatus = c.a.f2501A.STATUS,
                CombinNo = c.a.f2501A.COMBIN_NO
            }).OrderBy(o => o.SerialNo);

            var result = q.ToList();
            var rI = 1;

            foreach (var item in result)
            {
                item.SEQ = rI;
                rI++;
            }
            return result.AsQueryable();
        }
    }
}
