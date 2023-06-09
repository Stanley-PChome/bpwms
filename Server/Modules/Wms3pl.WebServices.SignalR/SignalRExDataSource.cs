using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WebServices.SignalR
{
	public class SignalRExDataSource
	{
		public IQueryable<ExecuteResult> ExecuteResults
		{
			get { return new List<ExecuteResult>().AsQueryable(); }
		}
	}


}
