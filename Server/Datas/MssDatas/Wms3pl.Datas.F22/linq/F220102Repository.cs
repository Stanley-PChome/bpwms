using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.DBCore;

namespace Wms3pl.Datas.F22
{
	public partial class F220102Repository : RepositoryBase<F220102, Wms3plDbContext, F220102Repository>
	{
		public F220102Repository(string connName, WmsTransaction wmsTransaction = null)
		: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F220102> GetDatas(string docNo, string wbCode, string poCode, List<string> statusList)
		{
            return _db.F220102s.Where(x => x.DOC_NO == docNo && x.WB_CODE == wbCode && x.POD_CODE == poCode).OrderByDescending(x=>x.BIN_CODE).Select(x => x);
		}

	}
}
