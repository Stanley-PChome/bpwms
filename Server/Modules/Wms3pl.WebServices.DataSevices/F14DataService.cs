using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.DataSevices
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class F14DataService : DataServiceBase<Wms3plDbContext>
	{// This method is called only once to initialize service-wide policies.
		public static void InitializeService(DataServiceConfiguration config)
        {
            DataServiceConfigHelper.SetEntitiesAccess(config, "F14");
            //config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}
	
		#region 行動盤點
		[WebGet]
		public IQueryable<F140101> GetF140101ByUserCanInventory(string dcCode, string gupCode, string custCode, string inventoryNo)
		{
			var f140101Repo = new F140101Repository(Schemas.CoreSchema);
			return f140101Repo.GetDataByUserCanInventory(dcCode, gupCode, custCode, inventoryNo);
		}
		#endregion
	}
}
