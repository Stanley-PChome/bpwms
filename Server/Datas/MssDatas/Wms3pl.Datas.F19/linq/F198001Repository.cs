using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F198001Repository : RepositoryBase<F198001, Wms3plDbContext, F198001Repository>
	{
		public F198001Repository(string connName, WmsTransaction wmsTransaction = null)
	: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F198001> GetDatasByItemPickWare(string itemPickWare)
		{
			return _db.F198001s.AsNoTracking().Where(x => x.ITEM_PICK_WARE == itemPickWare);
		}

		public IQueryable<F198001> GetDatasByTypeIds(List<string> typeIds)
		{
			return _db.F198001s.AsNoTracking().Where(x => typeIds.Contains(x.TYPE_ID));
		}
	}
}
