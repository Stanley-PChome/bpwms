using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebServices.WebApi.Common;

namespace WebServices.PdaWebApi
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API 設定和服務

			// Web API 路由
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
					name: "DefaultApi",
					routeTemplate: "api/{controller}/{id}",
					defaults: new { id = RouteParameter.Optional }
			);
			config.Filters.Add(new GlobalErrorHandleAttribute());

			//強制GET時也傳回JSON，不要傳回XML
			GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
		}
	}
}
