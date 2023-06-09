using Microsoft.AspNet.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.SignalRClient
{
	public abstract class ClientHubBase : IDisposable
	{
		private int _hubRetry;
		public IHubProxy HubProxy { get; set; }
		public HubConnection Connection { get; set; }

		public ClientHubBase(bool isSecretePersonalData = false, string functionCode = "", ServerSiteType serverSiteType = ServerSiteType.ApServer)
		{
			Connection = SignalRClientHelper.CreateHubConnection(isSecretePersonalData, functionCode, serverSiteType);
			Connection.Closed += Connection_Closed;
		}
		public virtual void Connection_Closed()
		{
		}
		public void Dispose()
		{
			Connection?.Dispose();
		}


		protected Task StartHub()
		{
			if (_hubRetry > 5)
				return Connection.Start();
			return Connection.Start().ContinueWith(x =>
			{
				if (x.IsFaulted)
				{
					_hubRetry++;
					System.Threading.Thread.Sleep(1000);
					StartHub();
				}
			});
		}

		public abstract Task ConnectAsync();

		public void Stop()
		{
			if (Connection.State != ConnectionState.Disconnected)
			{
				HubProxy.Invoke("Exit").ContinueWith(x =>
				{
					if (x.IsCompleted)
					{
						Connection.Stop();
					}
				});
			}
		}
	}
}
