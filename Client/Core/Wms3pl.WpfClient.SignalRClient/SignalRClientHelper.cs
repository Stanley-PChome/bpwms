using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Wms3pl.WpfClient.Common;

namespace Wms3pl.WpfClient.SignalRClient
{
	public class SignalRClientHelper
	{
		public static HubConnection CreateHubConnection(bool isSecretePersonalData = false, string functionCode = "", ServerSiteType serverSiteType = ServerSiteType.ApServer)
		{
			var url = string.Empty;
			url = ConfigurationManager.AppSettings["apServerUrl"];
			var connection = new HubConnection(url);
			connection.Headers["isSecrete"] = isSecretePersonalData ? "TRUE" : "FALSE";
			connection.Headers["functionCode"] = functionCode;
			connection.Headers["schema"] = Schemas.CoreSchema;
			connection.Headers["clientIp"] = Wms3plSession.Get<GlobalInfo>().ClientIp;
			connection.Headers["Lang"] = Wms3plSession.Get<GlobalInfo>().Lang;
			connection.Headers["userName"] = HttpUtility.UrlEncode(Wms3plSession.CurrentUserInfo.AccountName);
			connection.Headers["account"] = HttpUtility.UrlEncode(Wms3plSession.CurrentUserInfo.Account);
			connection.CookieContainer = Wms3plSession.CurrentUserInfo.ClientFormsIdentity.AuthenticationCookies;
			return connection;
		}
	}
}
