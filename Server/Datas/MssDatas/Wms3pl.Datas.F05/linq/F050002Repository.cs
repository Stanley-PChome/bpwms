using System.Collections.Generic;
using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F050002Repository : RepositoryBase<F050002, Wms3plDbContext, F050002Repository>
	{
		public F050002Repository(string connName, WmsTransaction wmsTransaction = null)
				: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F050002> GetNotAllotedDatas()
		{
			var result = from A in _db.F050002s
									 join B in _db.F050001s.Where(x => x.PROC_FLAG != "9")
									 on new { A.ORD_NO, A.GUP_CODE, A.CUST_CODE, A.DC_CODE } equals new { B.ORD_NO, B.GUP_CODE, B.CUST_CODE, B.DC_CODE }
									 select A;

			return result;
		}



		public IQueryable<F050002> GetNotAllotedDatas(List<string> ordNos)
		{
			var result = from A in _db.F050002s.Where(x => ordNos.Contains(x.ORD_NO))
									 join B in _db.F050001s.Where(x => ordNos.Contains(x.ORD_NO))
									 on new { A.ORD_NO, A.GUP_CODE, A.CUST_CODE, A.DC_CODE } equals new { B.ORD_NO, B.GUP_CODE, B.CUST_CODE, B.DC_CODE }
									 select A;

			return result;
		}
	}
}
