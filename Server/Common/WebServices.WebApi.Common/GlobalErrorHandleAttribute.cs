using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace WebServices.WebApi.Common
{
	public class GlobalErrorHandleAttribute : ExceptionFilterAttribute
	{
		public override void OnException(HttpActionExecutedContext filterContext)
		{
			ExceptionPolicy.HandleException(filterContext.Exception, "Default");
		}
	}
}
