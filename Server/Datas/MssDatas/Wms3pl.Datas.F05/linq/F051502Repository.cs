using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Microsoft.EntityFrameworkCore;
using Wms3pl.Datas.Shared.Pda.Entitues;
using System.Collections.Generic;
using System;

namespace Wms3pl.Datas.F05
{
    public partial class F051502Repository : RepositoryBase<F051502, Wms3plDbContext, F051502Repository>
	{
		public F051502Repository(string connName, WmsTransaction wmsTransaction = null)
					 : base(connName, wmsTransaction)
		{
		}
			   

        public IQueryable<GetPickDetailDetail> GetPickDetail(
           string dcNo, string gupCode, string custNo, List<string> statusList, string batchPickNo)
        {

            var f051502s = _db.F051502s.AsNoTracking();
            var f1912s = _db.F1912s.AsNoTracking();
            var f1980s = _db.F1980s.AsNoTracking();

            if (statusList != null)
            {
                f051502s = f051502s.Where(x => statusList.Contains(x.STATUS));
            }



            var result = f051502s.Where(w => w.DC_CODE == dcNo
                                                        && w.GUP_CODE == gupCode
                                                        && w.CUST_CODE == custNo
                                                        && w.BATCH_PICK_NO == batchPickNo)
                                                    .Join(f1912s,
                                                          A => new { A.DC_CODE, A.LOC_CODE },
                                                          B => new { B.DC_CODE, B.LOC_CODE },
                                                          (A, B) => new { A, B })
                                                    .Join(f1980s,
                                                          O => new { O.B.DC_CODE, O.B.WAREHOUSE_ID },
                                                          C => new { C.DC_CODE, C.WAREHOUSE_ID },
                                                          (O, C) => new
                                                          {O,C})
                                                    .Select(x=> new GetPickDetailDetail {
                                                        WmsNo = x.O.A.BATCH_PICK_NO,
                                                        WmsSeq = x.O.A.BATCH_PICK_SEQ.ToString(),
                                                        ItemNo = x.O.A.ITEM_CODE,
                                                        WHName = x.C.WAREHOUSE_NAME,
                                                        Loc = x.O.A.LOC_CODE,
                                                        ShipQty = Convert.ToInt32(x.O.A.B_PICK_QTY)
                                                    });
           

            return result;
        }
    }
}
