using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F25
{
    public partial class F250105Repository : RepositoryBase<F250105, Wms3plDbContext, F250105Repository>
    {
        public F250105Repository(string connName, WmsTransaction wmsTransaction = null)
      : base(connName, wmsTransaction)
        {
        }

        public IQueryable<P250301QueryItem> GetF250105Data(string gupCode, string custCode, string clientIp)
        {
            var q = from a in _db.F250105s
                    join b in _db.F1903s on new { a.ITEM_CODE, a.GUP_CODE, a.CUST_CODE } equals new { b.ITEM_CODE, b.GUP_CODE, b.CUST_CODE }
                    where a.STATUS == "0"
                    && a.GUP_CODE == gupCode
                    && a.CUST_CODE == custCode
                    && a.CLIENT_IP == clientIp
                    select new P250301QueryItem
                    {
                        LOG_SEQ = a.LOG_SEQ,
                        SERIAL_NO = a.SERIAL_NO,
                        ITEM_CODE = a.ITEM_CODE,
                        ORG_VALID_DATE = a.ORG_VALID_DATE,
                        VALID_DATE = a.VALID_DATE,
                        SERIAL_STATUS = a.SERIAL_STATUS,
                        ISPASS = a.ISPASS,
                        MESSAGE = a.MESSAGE,
                        STATUS = a.STATUS,
                        CLIENT_IP = a.CLIENT_IP,
                        GUP_CODE = a.GUP_CODE,
                        CUST_CODE = a.CUST_CODE,
                        CRT_STAFF = a.CRT_STAFF,
                        CRT_DATE = a.CRT_DATE,
                        UPD_STAFF = a.UPD_STAFF,
                        UPD_DATE = a.UPD_DATE,
                        CRT_NAME = a.CRT_NAME,
                        UPD_NAME = a.UPD_NAME,
                        ITEM_NAME = b.ITEM_NAME,
                        ISPASS_DESC = a.ISPASS == "1" ? "是" : "否",
                    };
            return q;
        }

        

        
    }
}
