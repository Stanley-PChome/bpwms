using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.SignalR
{
	public class LoginHub : HubBase
	{
		public LoginHub()
		{
			GroupName = HubNames.LoginHub;
			HubRecordMode = HubRecordMode.DataBase;
		}

		public override Task OnConnected()
		{
			if(Context.GetStaff()=="wms" || Current.IsAllowSameAccountMulitLogin())
				return base.OnConnected();
			//取得同帳號其他使用者
			var items = HubConnectionStorage.HubConnectedRecord.FindNameByExcludeCurrentConnectId(GroupName, Context.GetStaff(), Context.ConnectionId,HubRecordMode,Context.GetSchema());
			if (items!=null && items.Any())
			{
				foreach (var item in items)
				{
					Groups.Remove(item.ConnectId, GroupName);
				}
				//移除同帳號其他使用者
				HubConnectionStorage.HubConnectedRecord.RemoveAllByName(GroupName, Context.GetStaff(), HubRecordMode,Context.GetSchema());
			}
			return base.OnConnected();
		}

		public override Task OnReconnected()
		{
			var item = HubConnectionStorage.HubConnectedRecord.Find(GroupName, Context.ConnectionId, HubRecordMode,Context.GetSchema());
			if(item == null)
			{
				var items = HubConnectionStorage.HubConnectedRecord.FindNameByExcludeCurrentConnectId(GroupName, Context.GetStaff(), Context.ConnectionId, HubRecordMode,Context.GetSchema());
				if(items != null && items.Any())
					Clients.Client(Context.ConnectionId).SetVaildate(items.First().HostName, false);
			}
			else
			{
				//更新登入資料 解鎖時間為null
				HubConnectionStorage.HubConnectedRecord.UpdateUnLockDate(GroupName, item.ConnectId, null,HubRecordMode,Context.GetSchema());
			}
			return base.OnBaseReconnected();	
		}

		public override Task OnDisconnected(bool stopCalled)
		{
			// 更新登入資料解鎖時間為系統時間 + 870秒
			HubConnectionStorage.HubConnectedRecord.UpdateUnLockDate(GroupName, Context.ConnectionId, DateTime.Now.AddSeconds(870), HubRecordMode,Context.GetSchema());
			return base.OnBaseDisconnected(stopCalled);
		}

		public void Exit()
		{
			var item = HubConnectionStorage.HubConnectedRecord.Find(GroupName, Context.ConnectionId,HubRecordMode,Context.GetSchema());
			if (item != null)
			{
				Groups.Remove(item.ConnectId, GroupName);
				//移除登入資料
				HubConnectionStorage.HubConnectedRecord.Remove(GroupName, item.ConnectId, HubRecordMode,Context.GetSchema());
			}
		}

	}
}
