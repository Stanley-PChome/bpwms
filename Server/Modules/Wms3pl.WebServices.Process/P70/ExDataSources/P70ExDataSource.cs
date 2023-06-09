using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Management;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Process.P70.ExDataSources
{
	public partial class P70ExDataSource
	{
		public IQueryable<ExecuteResult> ExecuteResults
		{
			get { return new List<ExecuteResult>().AsQueryable(); }
		}

		public IQueryable<F700101EX> F700101EXs
		{
			get { return new List<F700101EX>().AsQueryable(); }
		}

		public IQueryable<F700102DirstCarNo> F700102DirstCarNos
		{
			get { return new List<F700102DirstCarNo>().AsQueryable(); }
		}

		public IQueryable<F700201Ex> F700201Exs
		{
			get { return new List<F700201Ex>().AsQueryable(); }
		}

		public IQueryable<F700102Data> F700102Datas
		{
			get { return new List<F700102Data>().AsQueryable(); }
		}

		public IQueryable<F700102DataReport> F700102DataReports
		{
			get { return new List<F700102DataReport>().AsQueryable(); }
		}
		public IQueryable<F700501Ex> F700501Exs
		{
			get { return new List<F700501Ex>().AsQueryable(); }
		}

		public IQueryable<P700104WmsNoDetialData> P700104WmsNoDetialDatas
		{
			get { return new List<P700104WmsNoDetialData>().AsQueryable(); }
		}

		public IQueryable<TakeTimeItem> TakeTimeItems
		{
			get { return new List<TakeTimeItem>().AsQueryable(); }
		}

		public IQueryable<WmsDistrCarItem> WmsDistrCarItems
		{
			get { return new List<WmsDistrCarItem>().AsQueryable(); }
		}

		public IQueryable<P700104ExportData> P700104ExportDatas
		{
			get { return new List<P700104ExportData>().AsQueryable(); }
		}
		public IQueryable<F700101Data> F700101Datas
		{
			get { return new List<F700101Data>().AsQueryable(); }
		}
		public IQueryable<F055001Data> F055001Datas
		{
			get { return new List<F055001Data>().AsQueryable(); }
		}
	}
}
