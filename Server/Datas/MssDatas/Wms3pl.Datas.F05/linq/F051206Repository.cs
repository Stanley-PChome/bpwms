using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F051206Repository : RepositoryBase<F051206, Wms3plDbContext, F051206Repository>
	{
		public F051206Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public F051206 GetExistsNoApproveData(string dcCode, string gupCode, string custCode, string pickOrdNo, string pickOrdSeq)
		{
			List<string> status = new List<string> { "0", "1" };

			var result = _db.F051206s.Where(x => x.DC_CODE == dcCode &&
																					 x.GUP_CODE == gupCode &&
																					 x.CUST_CODE == custCode &&
																					 x.PICK_ORD_NO == pickOrdNo &&
																					 x.PICK_ORD_SEQ == pickOrdSeq &&
																					 x.ISDELETED == "0" &&
																					 status.Contains(x.STATUS)).FirstOrDefault();

			return result;
		}

		public IQueryable<F051206> GetApproveHasLackDatas(string dcCode, string gupCode, string custCode, string pickOrdNo, string pickOrdSeq)
		{
            var result = _db.F051206s.Where(x => x.DC_CODE == dcCode &&
                                                 x.GUP_CODE == gupCode &&
                                                 x.CUST_CODE == custCode &&
                                                 x.PICK_ORD_NO == pickOrdNo &&
                                                 x.PICK_ORD_SEQ == pickOrdSeq &&
                                                 x.ISDELETED == "0" &&
                                                 x.STATUS == "2" &&
                                                 x.RETURN_FLAG == "1");

            return result;
		}
	}
}
