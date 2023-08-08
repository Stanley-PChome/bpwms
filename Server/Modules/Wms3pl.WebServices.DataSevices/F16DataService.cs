using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F16;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.DataSevices
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class F16DataService : DataServiceBase<Wms3plDbContext>
	{
		// This method is called only once to initialize service-wide policies.
		public static void InitializeService(DataServiceConfiguration config)
        {
            DataServiceConfigHelper.SetEntitiesAccess(config, "F16");
            //config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

        //[WebGet]
        //public IQueryable<F161201> GetF161201Datas(string dcCode, string gupCode, string custCode, string returnNo)
        //{
        //    var f161201Repo = new F161201Repository(Schemas.CoreSchema);
        //    return f161201Repo.GetF161201Datas(dcCode, gupCode, custCode, returnNo);
		//}

		#region 退貨狀況控管
		[WebGet]
		public IQueryable<F161601> GetUpLocDataByDc(string dcCode, string rtnApplyDate)
		{
			var repo = new F161601Repository(Schemas.CoreSchema);
			return repo.GetUpLocDataByDc(dcCode, DateTime.Parse(rtnApplyDate));
		}
		[WebGet]
		public IQueryable<F161601> GetWaitUpLocDataByDc(string dcCode, string rtnApplyDate)
		{
			var repo = new F161601Repository(Schemas.CoreSchema);
			return repo.GetWaitUpLocDataByDc(dcCode, DateTime.Parse(rtnApplyDate));
		}

		[WebGet]
		IQueryable<F161202> GetF161202ByDc(string dcCode, string ordProp, string returnDate)
		{
			var repo = new F161202Repository(Schemas.CoreSchema);
			return repo.GetDatasByDc(dcCode, ordProp, DateTime.Parse(returnDate));
		}
		#endregion

		#region 退貨點收維護

		[WebGet]
		public IQueryable<F161204> GetReurnItemByReturnNo(string dcCode, string gupCode, string custCode, string returnNo, string account)
		{
			var repo = new F161204Repository(Schemas.CoreSchema);
			return repo.GetReurnItem(dcCode, gupCode, custCode, returnNo, string.Empty, account);
		}
		#endregion
	}
}
