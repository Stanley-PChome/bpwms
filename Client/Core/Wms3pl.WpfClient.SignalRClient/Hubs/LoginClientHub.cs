using Microsoft.AspNet.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.SignalRClient
{
	public class LoginClientHub : ClientHubBase
	{
		public Action<string, bool> SetVaildate = (string hostName, bool isValidate) => { };
		public Action<string> SendMessage = (message) => { };
		public LoginClientHub(bool isSecretePersonalData = false, string functionCode = "") : base(isSecretePersonalData, functionCode)
		{

		}
		public override Task ConnectAsync()
		{
			HubProxy = Connection.CreateHubProxy(HubNames.LoginHub);
			HubProxy.On<string, bool>("SetVaildate", (hostName, isValidate) => SetVaildate(hostName, isValidate));
			HubProxy.On<string>("SendMessage", (message) => SendMessage(message));
			return StartHub();
		}
	}
}
