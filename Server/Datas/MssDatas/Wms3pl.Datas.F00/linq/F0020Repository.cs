using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F00
{
    public partial class F0020Repository : RepositoryBase<F0020, Wms3plDbContext, F0020Repository>
    {
        public F0020Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }
        public IQueryable<F0020> GetDatasBymsgNoKeyword(string msgNoKeyword)
        {
            var query = _db.F0020s.Where(x => x.MSG_NO.Contains(msgNoKeyword));
            return query.Select(x => new F0020()
            {
                MSG_NO = x.MSG_NO,
                MSG_SUBJECT = x.MSG_SUBJECT,
                MSG_LEVEL = x.MSG_LEVEL,
                MSG_CONTENT = x.MSG_CONTENT,
                CRT_STAFF = x.CRT_STAFF,
                CRT_DATE = x.CRT_DATE,
                UPD_STAFF = x.UPD_STAFF,
                UPD_DATE = x.UPD_DATE,
                CRT_NAME = x.CRT_NAME,
                UPD_NAME = x.UPD_NAME,
            });
        }
    }
}

