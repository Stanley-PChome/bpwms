using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.ExDataServices.SignalRExDataService
{
	public partial class SignalRExDataSource : global::System.Data.Services.Client.DataServiceContext
	{

		public IQueryable<ExecuteResult> CheckAccountHasUserLogin(String userName)
		{
			return CreateQuery<ExecuteResult>("CheckAccountHasUserLogin")
						.AddQueryExOption("userName", userName);
		}
	}
}

