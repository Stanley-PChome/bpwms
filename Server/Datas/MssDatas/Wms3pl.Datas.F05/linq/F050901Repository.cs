using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F05
{
    public partial class F050901Repository : RepositoryBase<F050901, Wms3plDbContext, F050901Repository>
	{
		public F050901Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public List<F050901> GetUpDataForSOD(string customerId, List<string> consignNos)
		{
            var result = from A in _db.F050901s.Where(x => consignNos.Contains(x.CONSIGN_NO))
                         join B in _db.F19471201s.AsNoTracking().Where(x => x.CUSTOMER_ID == customerId)
                         on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.CONSIGN_NO } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.CONSIGN_NO }
                         select A;

            return result.ToList();
		}

		public List<F050901> GetUpDataForLogId(string customerId,string logId, List<string> consignNos)
		{
            var result = (from A in _db.F050901s.Where(x => x.DELIVID_SEQ_NAME == logId && consignNos.Contains(x.CONSIGN_NO))
                          join B in _db.F194710s.AsNoTracking().Where(x => x.LOGCENTER_ID == customerId)
                          on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, LOG_ID = A.DELIVID_SEQ_NAME } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.LOG_ID }
                          select A).Distinct().ToList();

            return result;
		}

		

		public IQueryable<F050901> GetDatasByConsignNo(string dcCode,string gupCode,string custCode,List<string> consignNos)
		{
            var result = _db.F050901s.Where(x => x.DC_CODE == dcCode &&
                                                 x.GUP_CODE == gupCode &&
                                                 x.CUST_CODE == custCode &&
                                                 consignNos.Contains(x.CONSIGN_NO));

            return result;
		}

		public IQueryable<F050901> GetDatasByWmsOrdNos(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			var result = _db.F050901s.Where(x => x.DC_CODE == dcCode &&
																					 x.GUP_CODE == gupCode &&
																					 x.CUST_CODE == custCode &&
																					 wmsOrdNos.Contains(x.WMS_NO));

			return result;
		}
	}
}
