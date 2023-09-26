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
    public partial class F1908Repository : RepositoryBase<F1908, Wms3plDbContext, F1908Repository>
	{
		public F1908Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public F1908 GetFirstData(string gupCode)
		{
            var query = _db.F1908s
                        .Where(x => x.GUP_CODE == gupCode);
            return query.OrderBy(x => x.GUP_CODE)
                        .ThenBy(x => x.VNR_CODE)
                        .FirstOrDefault();
		}

		public F1908 GetEnabledVnrData(string gupCode,string custCode,string vnrCode)
		{
            return _db.F1908s
                    .Where(x => x.GUP_CODE == gupCode
                    && x.CUST_CODE == custCode
                    && x.VNR_CODE == vnrCode
                    && x.STATUS != "*")
                    .Select(x=>x)
                    .FirstOrDefault();
        }
	}
}
