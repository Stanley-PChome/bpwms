using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F25;

namespace Wms3pl.WebServices.Process.P25.ExDataSources
{
	public partial class P25ExDataSource
	{
		public IQueryable<ExecuteResult> ExecuteResults
		{
			get { return new List<ExecuteResult>().AsQueryable(); }
		}


		public IQueryable<F2501ItemData> F2501ItemDatas
		{
			get { return new List<F2501ItemData>().AsQueryable(); }
		}


		public IQueryable<F250102Item> F250102Items
		{
			get { return new List<F250102Item>().AsQueryable(); }
		}

		public IQueryable<F2501QueryData> F2501QueryDatas
		{
			get { return new List<F2501QueryData>().AsQueryable(); }
		}

		public IQueryable<P2502QueryData> P2502QueryDatas
		{
			get { return new List<P2502QueryData>().AsQueryable(); }
		}

		public IQueryable<F2501WcfData> F2501WcfDatas
		{
			get { return new List<F2501WcfData>().AsQueryable(); }
		}

		public IQueryable<F250103Verification> F250103Verifications
		{
			get { return new List<F250103Verification>().AsQueryable(); }
		}

		public IQueryable<F2501SerialItemData> F2501SerialItemDatas
		{
			get { return new List<F2501SerialItemData>().AsQueryable(); }
		}

    public IQueryable<P250301QueryItem> P250301QueryItems
    {
      get { return new List<P250301QueryItem>().AsQueryable(); }
    }

    public IQueryable<P250302QueryItem> P250302QueryItems
    {
      get { return new List<P250302QueryItem>().AsQueryable(); }
    }
	}
}
