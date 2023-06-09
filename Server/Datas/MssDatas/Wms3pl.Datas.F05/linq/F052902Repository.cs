using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Wms3pl.Datas.F05
{
    public partial class F052902Repository : RepositoryBase<F052902, Wms3plDbContext, F052902Repository>
    {
        public F052902Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }
        public IQueryable<F052902> GetF052902ItemByBoxId(string dcCode, string gupCode, string custCode, string itemCode, string boxIds, DateTime delvDate)
        {
					var f052901s = from m in _db.F052901s
												 where m.DC_CODE == dcCode && m.GUP_CODE == gupCode && m.CUST_CODE == custCode
												 select m;
					if (!string.IsNullOrEmpty(boxIds))
					{
						var boxIdList = boxIds.Split(',').ToList();
				    f052901s = f052901s.Where(x => boxIdList.Contains(x.MERGE_BOX_NO));
					}
			    if (delvDate != null)
				    f052901s = f052901s.Where(x => x.DELV_DATE == delvDate);

			    var datas = from d in _db.F052902s
									join m in f052901s
									on new { d.DC_CODE, d.GUP_CODE, d.CUST_CODE, d.WMS_ORD_NO } equals new { m.DC_CODE, m.GUP_CODE, m.CUST_CODE, m.WMS_ORD_NO }
									where d.DC_CODE == dcCode && d.GUP_CODE == gupCode && d.CUST_CODE == custCode &&
												d.ITEM_CODE == itemCode && m.STATUS == "0"
									select d;
					return datas.AsNoTracking();
        }
    }
}
