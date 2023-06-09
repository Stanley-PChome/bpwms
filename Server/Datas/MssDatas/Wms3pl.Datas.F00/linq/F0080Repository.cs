using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
    public partial class F0080Repository : RepositoryBase<F0080, Wms3plDbContext, F0080Repository>
    {
        public F0080Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<WMSMessage> GetWMSMessages()
        {
            var query = _db.F0080s
                .Join(_db.F0020s, f0080 => f0080.MSG_NO, f0020 => f0020.MSG_NO, (f0080, f0020) => new { f0080, f0020 });
            query = query.Where(x =>
                           x.f0080.STATUS == null
                        || x.f0080.STATUS == ""
                        || (x.f0080.TARGET_TYPE == "0" && x.f0080.STATUS == "01" && ((DateTime.Now - x.f0080.SEND_TIME.Value).Days) >= 7)
                        || (x.f0080.TARGET_TYPE == "0" && x.f0080.STATUS == "02" && ((DateTime.Now - x.f0080.SEND_TIME.Value).Days) >= 14));
            return query.Select(x => new WMSMessage()
            {
                MESSAGE_ID = x.f0080.MESSAGE_ID,
                MSG_SUBJECT = x.f0020.MSG_SUBJECT,
                MESSAGE_CONTENT = x.f0080.MEAAGE_CONTENT,
                STATUS = x.f0080.STATUS,
                TARGET_TYPE = x.f0080.TARGET_TYPE,
                TARGET_CODE = x.f0080.TARGET_CODE,
                DC_CODE = x.f0080.DC_CODE,
                GUP_CODE = x.f0080.GUP_CODE,
                CUST_CODE = x.f0080.CUST_CODE,
                CRT_DATE = x.f0080.CRT_DATE,
                DAYS = (DateTime.Now - x.f0080.SEND_TIME.Value).Days
            });
        }


        public void UpdatetStatus(List<decimal> messageIds, string status)
        {
            var query = _db.F0080s.Where(x => messageIds.Contains(x.MESSAGE_ID)).ToList();
            query.ForEach(x => x.STATUS = status);
            if (status == "01")
               query.ForEach(x => x.SEND_TIME = DateTime.Now);
            _db.UpdateRange(query);
            _db.SaveChanges();
        }
    }
}

