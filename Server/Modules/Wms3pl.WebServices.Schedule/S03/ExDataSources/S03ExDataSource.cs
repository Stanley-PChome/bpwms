using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.Schedule.S03.ExDataSources
{
	public partial class S03ExDataSource
	{
		public IQueryable<ExecuteResult> ExecuteResults
		{
			get { return new List<ExecuteResult>().AsQueryable(); }
		}

		public IQueryable<F055001QueryItem> F055001QueryItems
		{
			get { return new List<F055001QueryItem>().AsQueryable(); }
		}

		public IQueryable<F1924QueryData> F1924QueryDatas
		{
			get { return new List<F1924QueryData>().AsQueryable(); }
		}


	}
}
