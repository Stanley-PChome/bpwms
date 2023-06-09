using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F190001Repository : RepositoryBase<F190001, Wms3plDbContext, F190001Repository>
    {
        public F190001Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F190001> GetDatas(string dcCode, string gupCode, string custCode)
        {
            var query = _db.F190001s.Where(x =>
                               x.DC_CODE == dcCode
                            && x.GUP_CODE == gupCode
                            && x.CUST_CODE == custCode);
            return query.Select(x => new F190001
            {
                TICKET_ID = x.TICKET_ID,
                TICKET_TYPE = x.TICKET_TYPE,
                TICKET_NAME = x.TICKET_NAME,
                TICKET_CLASS = x.TICKET_CLASS,
                SHIPPING_ASSIGN = x.SHIPPING_ASSIGN,
                FAST_DELIVER = x.FAST_DELIVER,
                ASSIGN_DELIVER = x.ASSIGN_DELIVER,
                PRIORITY = x.PRIORITY,
                CUST_CODE = x.CUST_CODE,
                GUP_CODE = x.GUP_CODE,
                DC_CODE = x.DC_CODE,
                CRT_STAFF = x.CRT_STAFF,
                CRT_NAME = x.CRT_NAME,
                CRT_DATE = x.CRT_DATE,
                UPD_STAFF = x.UPD_STAFF,
                UPD_NAME = x.UPD_NAME,
                UPD_DATE = x.UPD_DATE,
            });
        }


        public IQueryable<F190001> GetTicketID(string dcCode, string gupCode, string custCode, string ticketClass)
        {
            var query = _db.F190001s.Where(x =>
                    x.DC_CODE == dcCode
                && x.GUP_CODE == gupCode
                && x.CUST_CODE == custCode
                && x.TICKET_CLASS == ticketClass
                );
            return query;
        }
    }
}
