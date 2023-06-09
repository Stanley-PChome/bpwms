using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.SignalR
{
	public class LoginService
	{
		private Lazy<IHubContext> _loginHub = new Lazy<IHubContext>(() => GlobalHost.ConnectionManager.GetHubContext<LoginHub>());

	  public void SetVaildate(string connectId,string hostName,bool isVaildate)
		{
			if(!isVaildate)
				_loginHub.Value.Clients.Client(connectId).SetVaildate(hostName, false);
		}
		public void SendMessageByAllUser(string message)
		{
			_loginHub.Value.Clients.All.SendMessage(message);
		}
		public void SendMessageByDBUser(string message)
		{
			var datas = HubConnectionStorage.HubConnectedRecord.GetAllHubConnect(HubNames.LoginHub, HubRecordMode.DataBase, Schemas.CoreSchema);
			_loginHub.Value.Clients.Clients(datas.Select(x => x.ConnectId).ToList()).SendMessage(message);
		}
	}
}
