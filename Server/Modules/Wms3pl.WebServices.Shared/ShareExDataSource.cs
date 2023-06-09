using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F00;

namespace Wms3pl.WebServices.Shared
{
	public partial class ShareExDataSource
	{
		public IQueryable<F050801WmsOrdNo> F050801Datas
		{
			get { return new List<F050801WmsOrdNo>().AsQueryable(); }
		}

		#region P910304000 標籤 Print
		public IQueryable<LableItem> LableItems
		{
			get { return new List<LableItem>().AsQueryable(); }
		}
		#endregion

		public IQueryable<SerialNoResult> SerialNoResults
		{
			get { return new List<SerialNoResult>().AsQueryable(); }
		}

		public IQueryable<F1903Plus> F1903Pluss
		{
			get { return new List<F1903Plus>().AsQueryable(); }
		}

		public IQueryable<Route> Routes
		{
			get { return new List<Route>().AsQueryable(); }
		}

		public IQueryable<F197002> GetPalletOrBoxNos
		{
			get { return new List<F197002>().AsQueryable(); }
		}

		public IQueryable<F000904> GetF000904Lists
		{
			get { return new List<F000904>().AsQueryable(); }
		}
	}
}
