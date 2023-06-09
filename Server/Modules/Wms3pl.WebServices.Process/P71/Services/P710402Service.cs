
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P71.Services
{
	public partial class P710402Service
	{
		private WmsTransaction _wmsTransaction;
		public P710402Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<DcWmsNoDateItem> GetDcWmsNoDateItemsByA(string dcCode, string gupCode, string custCode,
			DateTime begStockDate, DateTime endStockDate)
		{
			var f010201Repo = new F010201Repository(Schemas.CoreSchema);
			return f010201Repo.GetDcWmsNoDateItems(dcCode, gupCode, custCode, begStockDate, endStockDate);
		}

		public IQueryable<DcWmsNoDateItem> GetDcWmsNoDateItemsByS(string dcCode, string gupCode, string custCode,
			DateTime begOrdDate, DateTime endOrdDate)
		{
			var f050301Repo = new F050301Repository(Schemas.CoreSchema);
			return f050301Repo.GetDcWmsNoDateItems(dcCode, gupCode, custCode, begOrdDate, endOrdDate);
		}

		public IQueryable<DcWmsNoDateItem> GetDcWmsNoDateItemsByR(string dcCode, string gupCode, string custCode,
			DateTime begReturnDate, DateTime endReturnDate)
		{
			var f161201Repo = new F161201Repository(Schemas.CoreSchema);
			return f161201Repo.GetDcWmsNoDateItems(dcCode, gupCode, custCode, begReturnDate, endReturnDate);
		}

		public IQueryable<DcWmsNoDateItem> GetDcWmsNoDateItemsByW(string dcCode, string gupCode, string custCode,
			DateTime begFinishDate, DateTime endFinishDate)
		{
			var f910201Repo = new F910201Repository(Schemas.CoreSchema);
			return f910201Repo.GetDcWmsNoDateItems(dcCode, gupCode, custCode, begFinishDate, endFinishDate);
		}

		public IQueryable<DcWmsNoLocTypeItem> GetDcWmsNoLocTypeItems(string dcCode, string gupCode, string custCode)
		{
			var f1912Repo = new F1912Repository(Schemas.CoreSchema);
			return f1912Repo.GetDcWmsNoLocTypeItems(dcCode, gupCode, custCode);
		}
	}
}

