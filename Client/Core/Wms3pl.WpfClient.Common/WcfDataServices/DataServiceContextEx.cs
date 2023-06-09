using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using Wms3pl.WpfClient.DataServices.WcfDataServices;
using System.Web.Security;
using System.Web.ClientServices.Providers;
using System.Threading;
using System.Web.ClientServices;
using System.Web;
using System.Configuration;

namespace Wms3pl.WpfClient.Common.WcfDataServices
{
	public partial class DataServiceContextEx
	{
		public static void DataServiceContextEx_WritingEntity(object sender,
			System.Data.Services.Client.ReadingWritingEntityEventArgs e)
		{
			XName entityProperties = XName.Get("properties", e.Data.GetNamespaceOfPrefix("m").NamespaceName);
			XElement payload = null;
			foreach (PropertyInfo propertyInfo in e.Entity.GetType().GetProperties())
			{
				object[] doNotSerializeAttributes = propertyInfo.GetCustomAttributes(typeof(DoNotSerializeAttribute), false);
				if (doNotSerializeAttributes.Length > 0)
				{
					if (payload == null)
					{
						payload = e.Data.Descendants().Where<XElement>(xe => xe.Name == entityProperties).First<XElement>();
					}
					XName property = XName.Get(propertyInfo.Name, e.Data.GetNamespaceOfPrefix("d").NamespaceName);
					XElement xeRemoveThisProperty = payload
						.Descendants()
						.Where<XElement>(xe => xe.Name == property)
						.First<XElement>();
					xeRemoveThisProperty.Remove();
				}
			}
		}

		public static void OnSendingRequestEx(object sender, SendingRequest2EventArgs e)
		{
			var webRequest = (e.RequestMessage as HttpWebRequestMessage).HttpWebRequest;

			webRequest.Timeout = 1000 * 60 * 10;

			if (Wms3plSession.CurrentUserInfo != null)
			{
				var identity = Wms3plSession.CurrentUserInfo.ClientFormsIdentity;
		
				if (identity != null)
				{
					webRequest.CookieContainer = identity.AuthenticationCookies;
				}
				
			}
			webRequest.AutomaticDecompression = DecompressionMethods.GZip;
		}

		public static void OnSendingRequest(object sender, SendingRequest2EventArgs e)
		{
			OnSendingRequestEx(sender, e);
		}

		public static void OnSendingRequestLongTermSchema(object sender, SendingRequest2EventArgs e)
		{
			OnSendingRequestEx(sender, e);
		}

		public static void BuildingRequest(object sender, BuildingRequestEventArgs e)
		{
			var apServer = ConfigurationManager.AppSettings["APServerUrl"];
			if(apServer.StartsWith("https"))
			{
				var httpsUri = new Uri(e.RequestUri.AbsoluteUri.Replace("http:", "https:"));
				e.RequestUri = httpsUri;
			}
			OnBuildingRequestEx(sender, e);
		}

		public static void BuildingRequestLongTermSchema(object sender, BuildingRequestEventArgs e)
		{
			var apServer = ConfigurationManager.AppSettings["APServerUrl"];
			var baseUri = new Uri(apServer);
			e.RequestUri = DataServiceContextExtensions.BuildUri(baseUri, e.RequestUri);
			OnBuildingRequestEx(sender, e);
			e.Headers["schema"] = Schemas.LongTermSchema;
		}


		public static void OnBuildingRequestEx(object sender, BuildingRequestEventArgs e)
		{
			if (Wms3plSession.CurrentUserInfo != null)
			{
				var identity = Wms3plSession.CurrentUserInfo.ClientFormsIdentity;
				//if (identity == null || !identity.IsAuthenticated || identity.AuthenticationCookies.Count == 0)
				//{
				//	Membership.ValidateUser(Wms3plSession.CurrentUserInfo.Account, Wms3plSession.CurrentUserInfo.Password);
				//	identity = Thread.CurrentPrincipal.Identity as ClientFormsIdentity;
				//	Wms3plSession.CurrentUserInfo.ClientFormsIdentity = identity;
				//}

			
				e.Headers["userName"] = HttpUtility.UrlEncode(Wms3plSession.CurrentUserInfo.AccountName);
				//e.RequestHeaders["deviceName"] = System.Environment.MachineName;
			}
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			if (globalInfo != null)
			{
				e.Headers["isSecrete"] = globalInfo.IsSecretePersonalData.ToString();
				e.Headers["functionCode"] = globalInfo.FunctionCode;
				e.Headers["ClientIp"] = globalInfo.ClientIp;
				e.Headers["Lang"] = globalInfo.Lang;
			}
			else
			{
				e.Headers["isSecrete"] = "FALSE"; ;
				e.Headers["functionCode"] = "SystemFunction";
				e.Headers["clientIp"] = ReadRdpClientSessionInfo.GetRdpClientName();
				e.Headers["Lang"] = Thread.CurrentThread.CurrentCulture.Name;
			}
			e.Headers["schema"] = Schemas.CoreSchema;
		}

		private void OnSendingRequest2(object sender, SendingRequest2EventArgs e)
		{
			object o = sender;
			//var identity = Wms3plSession.CurrentUserInfo.ClientFormsIdentity;
			//var webRequest = e.RequestMessage...Request as HttpWebRequest;
			//if (identity != null)
			//  webRequest.CookieContainer = identity.AuthenticationCookies;
			//webRequest.AutomaticDecompression = DecompressionMethods.GZip;
			//var globalInfo = Wms3plSession.Get<GlobalInfo>();
			//if (globalInfo != null)
			//  e.RequestMessage.SetHeader("dc", globalInfo.Dc);
		}

		public static void AddHeaderToRequest(HttpWebRequest webRequest)
		{
			var identity = Wms3plSession.CurrentUserInfo.ClientFormsIdentity;
			if (identity != null)
				webRequest.CookieContainer = identity.AuthenticationCookies;
			webRequest.AutomaticDecompression = DecompressionMethods.GZip;
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			if (globalInfo != null)
			{
				//webRequest.Headers["dc"] = globalInfo.Dc;
				//webRequest.Headers["schema"] = "SMART"+ globalInfo.Dc;
				webRequest.Headers["schema"] = globalInfo.SchemaName;
			}
		}
	}

	public static class DataServiceContextExtensions
	{
		public static string ExecutePlainText(this DataServiceContext ctx, Uri uri)
		{
			uri = BuildUri(ctx.BaseUri, uri);

			// Build the request to make our specific request
			var request = (HttpWebRequest)WebRequest.Create(uri);
			DataServiceContextEx.AddHeaderToRequest(request);
			request.Method = "GET";
			request.Accept = "*/*";
			//request.Headers[HttpRequestHeader.AcceptCharset] = "UTF-8";
			var response = request.GetResponse();
			using (var sr = new StreamReader(response.GetResponseStream()))
			{
				string result = sr.ReadToEnd();
				return result;
			}
		}

		public static IEnumerable<T> ExecuteNonEntityOperation<T>(this DataServiceContext ctx, Uri uri)
		{
			uri = BuildUri(ctx.BaseUri, uri);

			// Build the request to make our specific request
			//var request = (HttpWebRequest)WebRequest.Create(uri);
			var request = (HttpWebRequest)WebRequest.Create(uri);
			DataServiceContextEx.AddHeaderToRequest(request);
			request.Method = "GET";
			request.Accept = "application/atom+xml,application/xml";
			//request.Headers[HttpRequestHeader.AcceptCharset] = "UTF-8";
			request.Headers["DataServiceVersion"] = "2.0;NetFx";
			request.Headers["MaxDataServiceVersion"] = "2.0;NetFx";



			var response = request.GetResponse();

			XDocument doc = XDocument.Load(response.GetResponseStream());
			return ConvertDocToNonEntity<T>(doc);
		}

		public static IEnumerable<T> ExecuteNonEntityPostOperation<T>(this DataServiceContext ctx, Uri uri,
			Dictionary<string, string> forms)
		{
			uri = BuildUri(ctx.BaseUri, uri);

			var req = (HttpWebRequest)WebRequest.Create(uri);
			DataServiceContextEx.AddHeaderToRequest(req);
			req.Method = "POST";
			req.ContentType = "application/x-www-form-urlencoded";
			req.Accept = "application/atom+xml,application/xml";
			req.Headers["DataServiceVersion"] = "1.0;Silverlight";
			req.Headers["MaxDataServiceVersion"] = "1.0;Silverlight";

			var builder = new StringBuilder();
			foreach (var form in forms)
			{
				builder.AppendFormat("{0}={1}", form.Key, form.Value);
			}
			string body = builder.ToString();
			req.ContentLength = body.Length;
			byte[] bytes = Encoding.ASCII.GetBytes(body);

			using (var reqStream = req.GetRequestStream())
				reqStream.Write(bytes, 0, body.Length);

			using (var resp = (HttpWebResponse)req.GetResponse())
			{
				XDocument doc = XDocument.Load(resp.GetResponseStream());
				return ConvertDocToNonEntity<T>(doc);
			}
		}


		public static IAsyncResult BeginExecuteNonEntityOperation<T>(this DataServiceContext ctx, Uri uri, AsyncCallback callback, object state)
		{
			// Convert the Uri to use the Context's URI Base
			uri = BuildUri(ctx.BaseUri, uri);

			// Build the request to make our specific request
			var request = (HttpWebRequest)WebRequest.Create(uri);
			DataServiceContextEx.AddHeaderToRequest(request);
			request.Method = "GET";
			request.Accept = "application/atom+xml,application/xml";
			//request.Headers[HttpRequestHeader.AcceptCharset] = "UTF-8";
			request.Headers["DataServiceVersion"] = "1.0;Silverlight";
			request.Headers["MaxDataServiceVersion"] = "1.0;Silverlight";

			NonEntityOperationResult result = new NonEntityOperationResult(ctx, request, callback, state);
			result.BeginExecute();

			return result;
		}

		public static IEnumerable<T> EndExecuteNonEntityOperation<T>(this DataServiceContext ctx, IAsyncResult asyncResult)
		{
			var result = (NonEntityOperationResult)asyncResult;

			// Mark it as complete
			result.userCompleted = true;

			// Load the results as an XML Doc
			XDocument doc = XDocument.Load(result.resultStream);

			return ConvertDocToNonEntity<T>(doc);
		}

		private static IEnumerable<T> ConvertDocToNonEntity<T>(XDocument doc)
		{
			// Convert to a Enumerable list of values
			if (!typeof(T).IsPrimitive && typeof(T) != typeof(string))
			{
				return doc.ToNonEntities<T>();
			}
			else
			{
				//Primitive types
				try
				{
					if (doc.Root.Elements().Count() == 0)
					{
						var returnValue = (T)Convert.ChangeType(doc.Root.Value, typeof(T), null);
						return new T[] { returnValue };
					}

					var qry = from x in doc.Root.Descendants()
										select Convert.ChangeType(x.Value, typeof(T), null);

					return qry.Cast<T>();
				}
				catch (Exception ex)
				{
					throw new InvalidCastException("Could not cast results into expected type", ex);
				}
			}
		}

		public static XDocument EndExecuteNonEntityOperation(this DataServiceContext ctx, IAsyncResult asyncResult)
		{
			NonEntityOperationResult result = (NonEntityOperationResult)asyncResult;
			result.userCompleted = true;
			XDocument doc = XDocument.Load(result.resultStream);
			return doc;
		}

		internal static Uri BuildUri(Uri baseUri, Uri requestUri)
		{
			const char ForwardSlash = '/';

			if (!requestUri.IsAbsoluteUri)
			{
				if (baseUri.OriginalString.EndsWith("/", StringComparison.Ordinal))
				{
					if (requestUri.OriginalString.StartsWith("/", StringComparison.Ordinal))
					{
						requestUri = new Uri(baseUri, new Uri(requestUri.OriginalString.TrimStart(ForwardSlash), UriKind.Relative));
					}
					else
					{
						requestUri = new Uri(baseUri, requestUri);
					}
				}
				else
				{
					requestUri = new Uri(baseUri.OriginalString + "/" + requestUri.OriginalString.TrimStart(ForwardSlash), UriKind.Absolute);
				}
			}

			return requestUri;
		}
	}
}
