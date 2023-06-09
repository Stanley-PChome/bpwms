using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.DataSevices
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class F00DataService : DataServiceBase<Wms3plDbContext>
	{
		// This method is called only once to initialize service-wide policies.
		public static void InitializeService(DataServiceConfiguration config)
		{
            DataServiceConfigHelper.SetEntitiesAccess(config, "F00");
            //config.SetEntitySetAccessRule("*", EntitySetRights.All);
            //config.SetServiceOperationAccessRule("GetUserFunctions", ServiceOperationRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

        [WebGet]
        public IQueryable<F0002> getLogisticList(string dcCode)
        {
            var f0002Repo = new F0002Repository(Schemas.CoreSchema);
            return f0002Repo.getLogisticList(dcCode);
        }

    }
}
