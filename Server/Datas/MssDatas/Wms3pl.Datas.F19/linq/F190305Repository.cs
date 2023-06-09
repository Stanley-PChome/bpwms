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
	public partial class F190305Repository : RepositoryBase<F190305, Wms3plDbContext, F190305Repository>
	{
		public F190305Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}
        public IQueryable<F190305> GetDatasByItems(string gupCode, string custCode, List<string> itemCodes)
        {
            var query = _db.F190305s
                            .Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode);
            if (itemCodes.Any())
                query = query.Where(x => itemCodes.Contains(x.ITEM_CODE));
            return query.Select(x => new F190305
            {
                GUP_CODE = x.GUP_CODE,
                CUST_CODE = x.CUST_CODE,
                ITEM_CODE = x.ITEM_CODE,
                PALLET_LEVEL_CASEQTY = x.PALLET_LEVEL_CASEQTY,
                PALLET_LEVEL_CNT = x.PALLET_LEVEL_CNT,
                CRT_DATE = x.CRT_DATE,
                CRT_STAFF = x.CRT_STAFF,
                CRT_NAME = x.CRT_NAME,
                UPD_DATE = x.UPD_DATE,
                UPD_STAFF = x.UPD_STAFF,
                UPD_NAME = x.UPD_NAME,
            });
		}

        public IQueryable<F190305> GetDatas(string gupCode, string custCode, List<string> itemCodes)
        {
            return _db.F190305s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
                                                          x.CUST_CODE == custCode &&
                                                          itemCodes.Contains(x.ITEM_CODE));
        }
    }
}
