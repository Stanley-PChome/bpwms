using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
	public partial class P192021Service
	{
		private WmsTransaction _wmsTransaction;
		public P192021Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult InsertF191201Datas(List<F191201> datas)
		{
			var f191201Repo = new F191201Repository(Schemas.CoreSchema, _wmsTransaction);
			f191201Repo.BulkInsert(datas);
			return new ExecuteResult(true);
		}
		public ExecuteResult DeletedF191201Datas(List<F191201> datas)
		{
			var f191201Repo = new F191201Repository(Schemas.CoreSchema, _wmsTransaction);
			var keys = from a in datas
					   select a.DC_CODE + a.TYPE + a.VALUE;
			f191201Repo.DeletedF191201Datas(keys.ToList());
			return new ExecuteResult(true);
		}
	}
}
