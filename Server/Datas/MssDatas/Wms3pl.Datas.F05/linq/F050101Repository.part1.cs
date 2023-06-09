using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F05
{
    public partial class F050101Repository : RepositoryBase<F050101, Wms3plDbContext, F050101Repository>
	{
		public IQueryable<F050101CustOrdNo> GetF050101ByCustOrdNo(string dcCode,  string gupCode, string custCode, string CustordNos)
		{
            var result = _db.F050101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                x.GUP_CODE == gupCode &&
                                                                x.CUST_CODE == custCode &&
                                                                x.CUST_ORD_NO == CustordNos &&
                                                                x.STATUS != "9")
                                                    .Select(x=>new F050101CustOrdNo
                                                    {
                                                        CUST_ORD_NO = x.CUST_ORD_NO,
                                                        STATUS = x.STATUS,
                                                        BATCH_NO = x.BATCH_NO
                                                    });

            return result;
		}
	}
}