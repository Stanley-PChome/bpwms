using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.Common
{
	public class GeneralErrorHandler : IErrorHandler
	{
		/// <summary>
		/// 處理異常的方法代理. 
		/// <para>通常情況下這個方法代理應該返回false, 以使其他處理異常的方法代理能繼續處理這個異常.</para>
		/// </summary>
		public readonly Func<Exception, bool> FnHandleError;

		/// <summary>
		/// 根據異常生成返回到用戶端的錯誤資訊的方法代理.
		/// </summary>
		public readonly Func<Exception, object> FnGetFaultDetails;

		/// <summary>
		/// 創建一個GeneralErrorHandler新實例.
		/// </summary>
		/// <param name="fnGetFaultDetails">根據異常生成返回到用戶端的錯誤資訊的方法代理.</param>
		/// <param name="fnHandleError">
		/// 處理異常的方法代理. 
		/// <para>通常情況下這個方法代理應該返回false, 以使其他處理異常的方法代理能繼續處理這個異常.</para>
		/// </param>
		public GeneralErrorHandler(Func<Exception, bool> fnHandleError, Func<Exception, object> fnGetFaultDetails)
		{
			FnHandleError = fnHandleError;
			FnGetFaultDetails = fnGetFaultDetails;
		}


		#region IErrorHandler Members

		public bool HandleError(Exception error)
		{
			if (FnHandleError == null)
				return false; //returns false so the other Error Handlers can do sth with the error.

			return FnHandleError(error);
		}

		public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
		{
			fault = null;
		}

		#endregion
	}

	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public class ErrorHandlerBehaviorAttribute : Attribute, IServiceBehavior
	{
		public readonly GeneralErrorHandler ErrorHandler;

		public ErrorHandlerBehaviorAttribute()
		{
			ErrorHandler = new GeneralErrorHandler(error =>
			{
				//...在這裡記錄並處理異常
				ExceptionPolicy.HandleException(error, "Default");
				return false; //返回false, 以使其他處理異常的方法代理能繼續處理這個異常.
			},

			error => error);
		}

		#region IServiceBehavior Members

		public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
		 Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{ }

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			//在這裡註冊我們的GeneralErrorHandler
			foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
				dispatcher.ErrorHandlers.Add(ErrorHandler);
		}

		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{ }


		#endregion
	}
}
