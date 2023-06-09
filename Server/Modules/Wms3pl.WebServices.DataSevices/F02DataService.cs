using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F02;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.DataSevices
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class F02DataService : DataServiceBase<Wms3plDbContext>
	{
		// This method is called only once to initialize service-wide policies.
		public static void InitializeService(DataServiceConfiguration config)
        {
            DataServiceConfigHelper.SetEntitiesAccess(config, "F02");
            //config.SetEntitySetAccessRule("*", EntitySetRights.All);
            //config.SetServiceOperationAccessRule("GetUserFunctions", ServiceOperationRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

		#region  物流中心-看板-進貨狀況控管 P7104030000
		//[WebGet]
		//public IQueryable<F020201> GetDatasByWaitOrUpLoc(string dcCode, string receDate)
		//{
		//	var f020201Repo = new F020201Repository(Schemas.CoreSchema);
		//	return f020201Repo.GetDatasByWaitOrUpLoc(dcCode, DateTime.Parse(receDate));
		//}

		#endregion

		[WebGet]
		public IQueryable<F020201> GetAllocationData(string dcCode, string gupCode, string custCode, string allocationNo)
		{
			var f020201Repo = new F020201Repository(Schemas.CoreSchema);
			return f020201Repo.GetAllocationData(dcCode, gupCode, custCode, allocationNo);
		}
	}
}
