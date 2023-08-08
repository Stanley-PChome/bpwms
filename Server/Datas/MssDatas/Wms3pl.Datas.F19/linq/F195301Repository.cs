using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F195301Repository : RepositoryBase<F195301, Wms3plDbContext, F195301Repository>
	{
		public F195301Repository(string connName, WmsTransaction wmsTransaction = null)
		 : base(connName, wmsTransaction)
		{
		}

		public List<F195301> AddP190504Detail(List<string> funCodeList, decimal grpId)
		{
			var funCodes = _db.F195301s.AsNoTracking().Where(x => x.GRP_ID == grpId).Select(x => x.FUN_CODE).ToList();

			return funCodeList.Except(funCodes).Select(x => new F195301
			{
				FUN_CODE = x,
				GRP_ID = grpId
			}).ToList();
		}

		public IQueryable<F195301> GetDelDatas(List<string> funCodeList, decimal grpId)
		{
			return _db.F195301s.Where(x => x.GRP_ID == grpId && !funCodeList.Contains(x.FUN_CODE));
		}
	}
}
