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
    public partial class F190102Repository : RepositoryBase<F190102, Wms3plDbContext, F190102Repository>
    {
        public F190102Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

		public IQueryable<F190102> GetAllDatas()
		{
            var query = _db.F190102s.Select(x => new F190102
            {
                DC_CODE = x.DC_CODE,
                DELV_EFFIC = x.DELV_EFFIC,
                SORT = x.SORT,
                CRT_STAFF = x.CRT_STAFF,
                CRT_DATE = x.CRT_DATE,
                UPD_STAFF = x.UPD_STAFF,
                UPD_DATE = x.UPD_DATE,
                CRT_NAME = x.CRT_NAME,
                UPD_NAME = x.UPD_NAME,
            });
            return query;
		}
    }
}
