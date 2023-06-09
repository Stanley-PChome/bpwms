

using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.SignalR
{
	public static class HubConnectionStorage
	{
		public static HubConnectRecord HubConnectedRecord = new HubConnectRecord();

	}
	public abstract class HubBase: Hub 
	{
		/// <summary>
		/// 是否要寫入DB
		/// 是:由F0070紀錄
		/// 否:由Server記憶體紀錄
		/// </summary>
		protected HubRecordMode HubRecordMode;
		/// <summary>
		/// 設定群組名稱
		/// </summary>
		protected string GroupName { get; set; }
		protected Task OnBaseConnected()
		{
			return base.OnConnected();
		}
		protected Task OnBaseReconnected()
		{
			return base.OnReconnected();
		}
		protected Task OnBaseDisconnected(bool stopCalled)
		{
			return base.OnDisconnected(stopCalled);
		}

		public override Task OnConnected()
		{
			HubConnectionStorage.HubConnectedRecord.Add(GroupName, new HubConnect
			{
				ConnectId = Context.ConnectionId,
				UserName = Context.GetStaff(),
				HostName = Context.GetClientHostName(),
				CRT_STAFF = Context.GetStaff(),
				CRT_NAME = Context.GetStaffName()
			}, HubRecordMode,Context.GetSchema());
			Groups.Add(Context.ConnectionId, GroupName);
			return base.OnConnected();
		}

		public override Task OnReconnected()
		{
			if (!HubConnectionStorage.HubConnectedRecord.IsExist(GroupName, Context.ConnectionId, HubRecordMode,Context.GetSchema()))
			{
				HubConnectionStorage.HubConnectedRecord.Add(GroupName, new HubConnect
				{
					ConnectId = Context.ConnectionId,
					UserName = Context.GetStaff(),
					HostName = Context.GetClientHostName(),
					CRT_STAFF = Context.GetStaff(),
					CRT_NAME = Context.GetStaffName()
				}, HubRecordMode,Context.GetSchema());
				Groups.Add(Context.ConnectionId, GroupName);
			}
			return base.OnReconnected();
		}

		public override Task OnDisconnected(bool stopCalled)
		{
			HubConnectionStorage.HubConnectedRecord.Remove(GroupName, Context.ConnectionId, HubRecordMode,Context.GetSchema());
			Groups.Remove(Context.ConnectionId, Context.GetStaff());
			return base.OnDisconnected(stopCalled);
		}
	}
}
