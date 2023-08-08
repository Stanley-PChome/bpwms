using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Refind.R01.ExDataSources
{
	public partial class R01ExDataSource
	{
		public IQueryable<ExecuteResult> ExecuteResults
		{
			get { return new List<ExecuteResult>().AsQueryable(); }
		}

		public IQueryable<SchF700501Data> SchF700501Datas
		{
			get { return new List<SchF700501Data>().AsQueryable(); }
		}
	}
}
