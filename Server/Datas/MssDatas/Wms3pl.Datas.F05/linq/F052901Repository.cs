using System.Linq;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F052901Repository : RepositoryBase<F052901, Wms3plDbContext, F052901Repository>
	{
		public F052901Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public F052901 GetData(string dcCode,string gupCode,string custCode,string mergeBoxNo)
		{
			var f052901 = (from o in _db.F052901s
										 where o.DC_CODE == dcCode &&
													 o.GUP_CODE == gupCode &&
													 o.CUST_CODE == custCode &&
													 o.MERGE_BOX_NO == mergeBoxNo
										 orderby o.CRT_DATE descending
										 select o
										).FirstOrDefault();
			return f052901;
		}
	}
}
