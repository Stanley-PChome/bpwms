using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;

namespace Wms3pl.Datas.F22
{
	public partial class F220101Repository : RepositoryBase<F220101, Wms3plDbContext, F220101Repository>
	{
		public F220101Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F220101> GetDatas(string reqCode)
		{
            return _db.F220101s.Where(x => x.REQ_CODE == reqCode)
                        .Select(x=>x);
		}

	
	}
}
