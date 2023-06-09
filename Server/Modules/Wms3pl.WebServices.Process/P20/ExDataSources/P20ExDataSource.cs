using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Process.P20.ExDataSources
{
	public partial class P20ExDataSource
	{
		public IQueryable<ExecuteResult> ExecuteResults
		{
			get { return new List<ExecuteResult>().AsQueryable(); }
		}

		public IQueryable<F200101Data> F200101Datas
		{
			get { return new List<F200101Data>().AsQueryable(); }
		}

		public IQueryable<F200102Data> F200102Datas
		{
			get { return new List<F200102Data>().AsQueryable(); }
		}

		public IQueryable<F050301Data> F050301Datas
		{
			get { return new List<F050301Data>().AsQueryable(); }
		}

		public IQueryable<F0513Data> F0513Datas
		{
			get { return new List<F0513Data>().AsQueryable(); }
		}

		public IQueryable<F200103Data> F200103Datas
		{
			get { return new List<F200103Data>().AsQueryable(); }
		}

		public IQueryable<F1913Data> F1913Datas
		{
			get { return new List<F1913Data>().AsQueryable(); }
		}
		public IQueryable<SerialNoResult> SerialNoResults
		{
			get { return new List<SerialNoResult>().AsQueryable(); }
		}
	}
}
