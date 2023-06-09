using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F70;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.DataSevices
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class F70DataService : DataServiceBase<Wms3plDbContext>
	{
		// This method is called only once to initialize service-wide policies.
		public static void InitializeService(DataServiceConfiguration config)
        {
            DataServiceConfigHelper.SetEntitiesAccess(config, "F70");
            //config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

		#region GetF700102
		[WebGet]
		public IQueryable<F700102> GetF700102(string dcCode, string distrCarNo)
		{
			var f700102Repo = new F700102Repository(Schemas.CoreSchema);
			return f700102Repo.GetF700102(dcCode, distrCarNo);
		}
		#endregion

		#region 依照DC 批次日期 取得F700102(出貨單) 出車時段(記得取回來要DISTINCT)
		[WebGet]
		public IQueryable<F700102> GetF700102List(string dcCode, string gupCode, string custCode, DateTime takeDate)
		{
			var f700102Repo = new F700102Repository(Schemas.CoreSchema);
			return f700102Repo.GetF700102List(dcCode, gupCode, custCode, takeDate);
		}

		#endregion

		[WebGet]
		public IQueryable<F700101> GetF700101ByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var repo = new F700101Repository(Schemas.CoreSchema);
			return repo.GetF700101ByWmsOrdNo(dcCode, gupCode, custCode, wmsOrdNo);
		}
	}
}
