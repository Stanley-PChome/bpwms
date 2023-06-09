using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F91;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.DataSevices
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class F91DataService : DataServiceBase<Wms3plDbContext>
	{
		// This method is called only once to initialize service-wide policies.
		public static void InitializeService(DataServiceConfiguration config)
        {
            DataServiceConfigHelper.SetEntitiesAccess(config, "F91");
            //config.SetEntitySetAccessRule("*", EntitySetRights.All);
            //config.SetServiceOperationAccessRule("GetUserFunctions", ServiceOperationRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}
		
		[WebGet]
		public IQueryable<F910003> GetF910003sByITEMTYPEIDANDITEMTYPE(string ITEM_TYPE_ID, string ITEM_TYPE)
		{
			var F910003Repository = new F910003Repository(Schemas.CoreSchema);
			return F910003Repository.GetF910003Datas(ITEM_TYPE_ID, ITEM_TYPE);
		}

		/// <summary>
		/// 以統編取得合約跟合約明細
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="uniForm"></param>
		/// <param name="enableDate"></param>
		/// <returns></returns>
		[WebGet]
		public IQueryable<F910401> GetF910301WithF910401(string dcCode, string gupCode, string uniForm, string enableDate)
		{
			var f910301Repo = new F910301Repository(Schemas.CoreSchema);
			return f910301Repo.GetF910301WithF910401(gupCode, dcCode, uniForm, enableDate);
		}
	}
}
