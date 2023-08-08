using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.SignalR
{
	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public partial class SignalRExDataService : DataService<SignalRExDataSource>
	{
		public static void InitializeService(DataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("*", EntitySetRights.All);
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
			config.UseVerboseErrors = true;
		}
		[WebGet]
		public IQueryable<ExecuteResult> CheckAccountHasUserLogin(string userName)
		{
			if(userName.ToLower()== "wms" || Current.IsAllowSameAccountMulitLogin())
				return new List<ExecuteResult> { new ExecuteResult(true) }.AsQueryable();
			var item = HubConnectionStorage.HubConnectedRecord.FindNameByExcludeCurrentConnectId(HubNames.LoginHub, userName, null,HubRecordMode.DataBase,Schemas.CoreSchema);
			if (item != null && item.Any()  && (item.First().UnLockTime == null || (item.First().HostName != Current.DeviceIp && item.First().UnLockTime.Value >= DateTime.Now)))
				return new List<ExecuteResult> { new ExecuteResult(false, string.Format("該帳號已由{0}使用中", item.First().HostName)) }.AsQueryable();
			if(item!=null && item.Any() && item.First().HostName == Current.DeviceIp && item.First().UnLockTime!=null)
			{
				var service = new LoginService();
				service.SetVaildate(item.First().ConnectId, item.First().HostName, false);
			}
			return new List<ExecuteResult> { new ExecuteResult(true) }.AsQueryable() ;
		}
	}
}
