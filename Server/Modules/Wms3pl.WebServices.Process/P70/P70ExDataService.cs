using System;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P70.ExDataSources;
using Wms3pl.WebServices.Process.P70.Services;
using Wms3pl.Datas.F70;

namespace Wms3pl.WebServices.Process.P70
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class P70ExDataService : DataService<P70ExDataSource>
	{
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("*", EntitySetRights.All);
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}

		#region GetF700101ByDistrCarNo
		[WebGet]
		public IQueryable<F700101EX> GetF700101ByDistrCarNo(string distrCarNo, string dcCode)
		{
			var p700104Service = new P700104Service();
			return p700104Service.GetF700101ByDistrCarNo(distrCarNo, dcCode);
		}
		#endregion

		#region 行事曆

		[WebGet]
		public IQueryable<F700501Ex> GetF700501Ex(string dcCode, string dateBegin,
			string dateEnd, string scheduleType)
		{
			var srv = new P700105Service();
			var result = srv.GetF700501Ex(dcCode, dateBegin, dateEnd, scheduleType);
			return result;
		}

		#endregion

		#region P7001080000無單派車

		[WebGet]
		public IQueryable<F700101Data> GetF700101Data(string dcCode, string allId, string delvTmpr, DateTime? takeDateFrom, DateTime? takeDateTo, string distrUse, string consignNos, string detailNos)
		{
			var f700101Repo = new F700101Repository(Schemas.CoreSchema);
			var consignNoItems = (consignNos ?? "").Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
			var detailNoItems = (detailNos ?? "").Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
			return f700101Repo.GetF700101Datas(dcCode, allId, delvTmpr, takeDateFrom, takeDateTo, distrUse, consignNoItems, detailNoItems);
		}
		#endregion
	}
}
