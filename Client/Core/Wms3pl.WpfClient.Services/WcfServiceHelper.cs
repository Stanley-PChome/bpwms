using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Web;
using System.Web.ClientServices;
using System.Web.Security;
using System.Xml.Linq;
using System.Xml.XPath;
using Wms3pl.WpfClient.Common;

namespace Wms3pl.WpfClient.Services
{
	public partial class WcfServiceHelper
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// <remarks>http://karlshifflett.wordpress.com/2011/04/25/various-clients-and-forms-authentication/</remarks>
		public static string GetTicketCookie()
		{
			var apServer = ConfigurationManager.AppSettings["APServerUrl"];
			var identity = Wms3plSession.CurrentUserInfo.ClientFormsIdentity;
			//if (identity == null || !identity.IsAuthenticated || identity.AuthenticationCookies.Count == 0)
			//{
			//	Membership.ValidateUser(Wms3plSession.CurrentUserInfo.Account, Wms3plSession.CurrentUserInfo.Password);
			//	identity = Thread.CurrentPrincipal.Identity as ClientFormsIdentity;
			//	Wms3plSession.CurrentUserInfo.ClientFormsIdentity = identity;
			//}

			var container = Wms3plSession.CurrentUserInfo.ClientFormsIdentity.AuthenticationCookies;
			return container.GetCookieHeader(new Uri(apServer));
		}

		public static T Execute<T>(Func<T> method, IContextChannel serviceInnerChannel)
		{
			if (method == null) throw new ArgumentNullException("method");
			if (serviceInnerChannel == null) throw new ArgumentNullException("serviceInnerChannel");
			if (String.IsNullOrWhiteSpace(GetTicketCookie()))
			{
				throw new InvalidOperationException(
						"Currently not logged in. Must Login before calling this method.");
			}

			using (new OperationContextScope(serviceInnerChannel))
			{
				var requestProperty = new HttpRequestMessageProperty();
				OperationContext.Current.OutgoingMessageProperties.Add(
						HttpRequestMessageProperty.Name, requestProperty);
				requestProperty.Headers.Add(HttpRequestHeader.Cookie, GetTicketCookie());
				return method();
			}
		}

		public static void Execute(Action method, IContextChannel serviceInnerChannel)
		{
			if (method == null) throw new ArgumentNullException("method");
			if (serviceInnerChannel == null) throw new ArgumentNullException("serviceInnerChannel");
			var cookie = GetTicketCookie();
			if (String.IsNullOrWhiteSpace(cookie))
			{
				throw new InvalidOperationException(
						"Currently not logged in. Must Login before calling this method.");
			}

			using (new OperationContextScope(serviceInnerChannel))
			{
				var requestProperty = new HttpRequestMessageProperty();
				OperationContext.Current.OutgoingMessageProperties.Add(
						HttpRequestMessageProperty.Name, requestProperty);
				requestProperty.Headers.Add(HttpRequestHeader.Cookie, cookie);
				method();
			}
		}

		public static T Execute<T>(System.ServiceModel.IClientChannel clientChannel, Func<T> doMethod, bool isSecretePersonalData, string functionCode)
		{
			if (doMethod == null) throw new ArgumentNullException("doMethod");
			if (clientChannel == null) throw new ArgumentNullException("clientChannel");
			var cookie = GetTicketCookie();
			if (String.IsNullOrWhiteSpace(cookie))
			{
				throw new InvalidOperationException(
						"Currently not logged in. Must Login before calling this method.");
			}

			T result;
			using (new System.ServiceModel.OperationContextScope((System.ServiceModel.IClientChannel)clientChannel))
			{
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("schema", Schemas.CoreSchema);
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("isSecrete", isSecretePersonalData.ToString());
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("functionCode", functionCode);
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("clientIp", ReadRdpClientSessionInfo.GetRdpClientName());
				var globalInfo = Wms3plSession.Get<GlobalInfo>();
				if (globalInfo != null)
					System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("Lang", globalInfo.Lang);
				else
					System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("Lang", Thread.CurrentThread.CurrentCulture.Name);
				if (Wms3plSession.CurrentUserInfo != null)
					System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("userName", HttpUtility.UrlEncode(Wms3plSession.CurrentUserInfo.AccountName));
				//System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("deviceName", System.Environment.MachineName);

				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add(HttpRequestHeader.Cookie, cookie);
				result = doMethod();
			}
			return result;
		}

		public static void Execute(System.ServiceModel.IClientChannel clientChannel, Action doMethod, bool isSecretePersonalData, string functionCode)
		{
			if (doMethod == null) throw new ArgumentNullException("doMethod");
			if (clientChannel == null) throw new ArgumentNullException("clientChannel");
			var cookie = GetTicketCookie();
			if (String.IsNullOrWhiteSpace(cookie))
			{
				throw new InvalidOperationException(
						"Currently not logged in. Must Login before calling this method.");
			}

			using (new System.ServiceModel.OperationContextScope((System.ServiceModel.IClientChannel)clientChannel))
			{
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("schema", Schemas.CoreSchema);
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("isSecrete", isSecretePersonalData.ToString());
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("functionCode", functionCode);
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("clientIp", ReadRdpClientSessionInfo.GetRdpClientName());
				var globalInfo = Wms3plSession.Get<GlobalInfo>();
				if (globalInfo != null)
					System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("Lang", globalInfo.Lang);
				else
					System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("Lang", Thread.CurrentThread.CurrentCulture.Name);
				if (Wms3plSession.CurrentUserInfo != null)
					System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("userName", HttpUtility.UrlEncode(Wms3plSession.CurrentUserInfo.AccountName));
				//System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("deviceName", System.Environment.MachineName);

				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add(HttpRequestHeader.Cookie, cookie);
				doMethod();
			}
		}

		public static T ExecuteForConsole<T>(System.ServiceModel.IClientChannel clientChannel, Func<T> doMethod, bool isSecretePersonalData, string functionCode = "", string dcCode = "", string gupCode = "")
		{
			if (doMethod == null) throw new ArgumentNullException("doMethod");
			if (clientChannel == null) throw new ArgumentNullException("clientChannel");

			T result;
			using (new System.ServiceModel.OperationContextScope((System.ServiceModel.IClientChannel)clientChannel))
			{
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("schema", AesCryptor.Current.Encode(GetSchema(dcCode, gupCode)));
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("isSecrete", isSecretePersonalData.ToString());
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("functionCode", functionCode);
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("clientIp", ReadRdpClientSessionInfo.GetRdpClientName());
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("IsSchedule", AesCryptor.Current.Encode("a1234567"));
				result = doMethod();
			}
			return result;
		}

		public static void ExecuteForConsole(System.ServiceModel.IClientChannel clientChannel, Action doMethod, bool isSecretePersonalData, string functionCode = "", string dcCode = "", string gupCode = "")
		{
			if (doMethod == null) throw new ArgumentNullException("doMethod");
			if (clientChannel == null) throw new ArgumentNullException("clientChannel");

			using (new System.ServiceModel.OperationContextScope((System.ServiceModel.IClientChannel)clientChannel))
			{
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("schema", AesCryptor.Current.Encode(GetSchema(dcCode, gupCode)));
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("isSecrete", isSecretePersonalData.ToString());
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("functionCode", functionCode);
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("clientIp", ReadRdpClientSessionInfo.GetRdpClientName());
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("IsSchedule", AesCryptor.Current.Encode("a1234567"));
				doMethod();
			}
		}
		private static string GetSchema(string dcCode, string gupCode)
		{
			string filePath = @"Schema.xml";
			string folderPath = string.Empty;
			if (ConfigurationManager.AppSettings["SchemaFolder"] != null)
				folderPath = ConfigurationManager.AppSettings["SchemaFolder"].ToString();
			else
				folderPath = System.AppDomain.CurrentDomain.BaseDirectory;

			var fileInfo = new FileInfo(Path.Combine(folderPath, filePath));
			if (fileInfo.Exists)
			{
				var doc = XDocument.Load(Path.Combine(folderPath, filePath));
				var selectPath = string.Format("/root/dc[@code={0}]/gup[@code={1}]", dcCode, gupCode);
				var resultElement = doc.XPathSelectElement(selectPath);
				if (resultElement != null)
					return (string)resultElement.Attribute("Schema");
				else
					throw new Exception("未設定資料連線。請參考 Schema.xml");
			}
			else
				throw new Exception("未設定資料連線。請參考 Schema.xml");
		}



		public static T ExecuteForConsoleBySchemaName<T>(System.ServiceModel.IClientChannel clientChannel, Func<T> doMethod, bool isSecretePersonalData, string schemaName, string functionCode = "")
		{
			if (doMethod == null) throw new ArgumentNullException("doMethod");
			if (clientChannel == null) throw new ArgumentNullException("clientChannel");

			T result;
			using (new System.ServiceModel.OperationContextScope((System.ServiceModel.IClientChannel)clientChannel))
			{
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("schema", AesCryptor.Current.Encode(GetSchema(schemaName)));
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("isSecrete", isSecretePersonalData.ToString());
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("functionCode", functionCode);
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("clientIp", ReadRdpClientSessionInfo.GetRdpClientName());
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("IsSchedule", AesCryptor.Current.Encode("a1234567"));
				result = doMethod();
			}
			return result;
		}

		public static void ExecuteForConsoleBySchemaName(System.ServiceModel.IClientChannel clientChannel, Action doMethod, bool isSecretePersonalData, string schemaName, string functionCode = "")
		{
			if (doMethod == null) throw new ArgumentNullException("doMethod");
			if (clientChannel == null) throw new ArgumentNullException("clientChannel");

			using (new System.ServiceModel.OperationContextScope((System.ServiceModel.IClientChannel)clientChannel))
			{
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("schema", AesCryptor.Current.Encode(GetSchema(schemaName)));
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("isSecrete", isSecretePersonalData.ToString());
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("functionCode", functionCode);
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("clientIp", ReadRdpClientSessionInfo.GetRdpClientName());
				System.ServiceModel.Web.WebOperationContext.Current.OutgoingRequest.Headers.Add("IsSchedule", AesCryptor.Current.Encode("a1234567"));
				doMethod();
			}
		}
		private static string GetSchema(string schemaName)
		{
			string filePath = @"Schema.xml";
			string folderPath = string.Empty;
			if (ConfigurationManager.AppSettings["SchemaFolder"] != null)
				folderPath = ConfigurationManager.AppSettings["SchemaFolder"].ToString();
			else
				folderPath = System.AppDomain.CurrentDomain.BaseDirectory;

			var fileInfo = new FileInfo(Path.Combine(folderPath, filePath));
			if (fileInfo.Exists)
			{
				var doc = XDocument.Load(Path.Combine(folderPath, filePath));
				var selectPath = string.Format("/root/schema[@name=\"{0}\"]", schemaName);
				var resultElement = doc.XPathSelectElement(selectPath);
				if (resultElement != null)
					return (string)resultElement.Attribute("value");
				else
					throw new Exception("未設定資料連線。請參考 Schema.xml");
			}
			else
				throw new Exception("未設定資料連線。請參考 Schema.xml");
		}

	}

}
