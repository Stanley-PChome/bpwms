using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P06.Services
{
	public partial class P060202Service
	{
		private WmsTransaction _wmsTransaction;
		public P060202Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<P060202Data> GetP060202Datas(string dcCode,string gupCode,string custCode,DateTime pickSDate,DateTime pickEDate,string warehouseId,string itemCode)
		{
			var f051206Repo = new F051206Repository(Schemas.CoreSchema);
			return f051206Repo.GetP060202Datas(dcCode, gupCode, custCode, pickSDate, pickEDate, warehouseId, itemCode);
		}

		private List<F1912> _manageLocs;
		public F1912 GetManageLoc(string dcCode,string gupCode,string custCode)
		{
			if (_manageLocs == null)
				_manageLocs = new List<F1912>();
			var manageLoc = _manageLocs.FirstOrDefault(x => x.DC_CODE == dcCode);
			if(manageLoc == null)
			{
				var f0003Repo = new F0003Repository(Schemas.CoreSchema);
				var f0003 = f0003Repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.AP_NAME == "TransLostToLocCode");
				if (f0003 == null)
					return null;
				var f1912Repo = new F1912Repository(Schemas.CoreSchema);
				manageLoc = f1912Repo.Find(x => x.DC_CODE == dcCode && x.LOC_CODE == f0003.SYS_PATH);
				if(manageLoc != null)
					_manageLocs.Add(manageLoc);
			}
			return manageLoc;
		}

	}
}
