using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F1963Repository : RepositoryBase<F1963, Wms3plDbContext, F1963Repository>
	{
		public F1963Repository(string connName, WmsTransaction wmsTransaction = null)
		: base(connName, wmsTransaction)
		{
		}

		public decimal GetNewId()
		{
			var workIds = _db.F1963s.AsNoTracking();
			var result = workIds.ToList().Count == 0 ? 1 : workIds.Select(x => x.WORK_ID).Max() + 1;

			return result;
		}


		public bool CheckDuplicateByIdName(decimal workgroupId, string groupName)
		{
			var result = _db.F1963s.AsNoTracking().Where(x => x.ISDELETED == "0"
																									&& x.WORK_ID != workgroupId
																									&& x.WORK_NAME == groupName);
			return result.Any();
		}
	}
}
