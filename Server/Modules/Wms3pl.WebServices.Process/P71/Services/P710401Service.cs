
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P71.Services
{
	public partial class P710401Service
	{
		private WmsTransaction _wmsTransaction;
		public P710401Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<DcWmsNoOrdPropItem> GeDcWmsNoOrdPropItemsByA(string dcCode, DateTime stockDate)
		{
			var f010201Repo = new F010201Repository(Schemas.CoreSchema);
			return f010201Repo.GetDcWmsNoOrdPropItems(dcCode, stockDate);
		}

		public IQueryable<DcWmsNoOrdPropItem> GeDcWmsNoOrdPropItemsByR(string dcCode, DateTime returnDate)
		{
			var f161201Repo = new F161201Repository(Schemas.CoreSchema);
			return f161201Repo.GetDcWmsNoOrdPropItems(dcCode, returnDate);
		}

		public IQueryable<DcWmsNoOrdPropItem> GeDcWmsNoOrdPropItemsByT(string dcCode, DateTime allocationDate)
		{
			var f151001Repo = new F151001Repository(Schemas.CoreSchema);
			return f151001Repo.GetDcWmsNoOrdPropItems(dcCode, allocationDate);
		}

		public IQueryable<DcWmsNoOrdPropItem> GeDcWmsNoOrdPropItemsByO(string dcCode, DateTime delvDate)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			return f050801Repo.GetDcWmsNoOrdPropItems(dcCode, delvDate);
		}
	}
}

