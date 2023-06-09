using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Process.P06.ExDataSources
{
	public partial class P06ExDataSource
	{
		public IQueryable<ExecuteResult> ExecuteResults
		{
			get { return new List<ExecuteResult>().AsQueryable(); }
		}

		public IQueryable<F05030101Ex> F05030101Exs
		{
			get { return new List<F05030101Ex>().AsQueryable(); }
		}
		public IQueryable<F051206Pick> F051206Picks
		{
			get { return new List<F051206Pick>().AsQueryable(); }
		}
		public IQueryable<F051206AllocationList> F051206AllocationLists
		{
			get { return new List<F051206AllocationList>().AsQueryable(); }
		}
		public IQueryable<F051206LackList> F051206LackLists
		{
			get { return new List<F051206LackList>().AsQueryable(); }
		}

		public IQueryable<F051206LackList_Allot> F051206LackLists_Allot
		{
			get { return new List<F051206LackList_Allot>().AsQueryable(); }
		}

		public IQueryable<F050801VirtualItem> F050801VirtualItems
		{
			get { return new List<F050801VirtualItem>().AsQueryable(); }
		}

		public IQueryable<SerialDataEx> SerialDataExs
		{
			get { return new List<SerialDataEx>().AsQueryable(); }
		}

		public IQueryable<SerialNoResult> SerialNoResults
		{
			get { return new List<SerialNoResult>().AsQueryable(); }
		}

		public IQueryable<F050801WmsOrdNo> F050801WmsOrdNos
		{
			get { return new List<F050801WmsOrdNo>().AsQueryable(); }
		}

		public IQueryable<P060202Data> P060202Datas
		{
			get { return new List<P060202Data>().AsQueryable(); }
		}

		//public IQueryable<F060401Data> F060401Datas
		//{
		//	get { return new List<F060401Data>().AsQueryable(); }
		//}
	}
}
