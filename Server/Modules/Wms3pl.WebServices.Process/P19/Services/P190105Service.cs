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
	public partial class P190105Service
	{
		private WmsTransaction _wmsTransaction;
		public P190105Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}
		public IQueryable<F190105> GetF190105Data(string dcCode)
		{
			var f190105Repo = new F190105Repository(Schemas.CoreSchema, _wmsTransaction);
			return f190105Repo.GetF190105Data(dcCode);
		}

		public ExecuteResult UpdateF190105AndF190106(F190105 f190105,List<F190106Data> addF190106, List<F190106Data> delF190106)
		{
			var f190105Repo = new F190105Repository(Schemas.CoreSchema, _wmsTransaction);
			var f190106Repo = new F190106Repository(Schemas.CoreSchema, _wmsTransaction);
			var updateData = f190105Repo.Find(item => item.DC_CODE == f190105.DC_CODE);
			f190105.B2B_PDA_PICK_PECENT = f190105.B2B_PDA_PICK_PECENT / (decimal)100.0;
			f190105.B2C_PDA_PICK_PERCENT = f190105.B2C_PDA_PICK_PERCENT / (decimal)100.0;
			f190105Repo.Update(f190105);
			List<int> ids = delF190106.Select(x => x.ID).ToList();
			f190106Repo.DeleteF190106ByIds(ids);
			if (addF190106.Any() && addF190106 != null)
			{
				foreach (var f190106Data in addF190106)
				{
					var f190106 = new F190106
					{
						DC_CODE = f190106Data.DC_CODE,
						SCHEDULE_TYPE = f190106Data.SCHEDULE_TYPE,
						START_TIME = f190106Data.START_TIME,
						END_TIME = f190106Data.END_TIME,
						PERIOD = f190106Data.PERIOD,
						CRT_DATE = DateTime.Now
					};
					f190106Repo.Add(f190106);
				}
			}
			return new ExecuteResult(true);
		}

	}
}
