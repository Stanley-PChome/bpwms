using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Web;

namespace Wms3pl.WpfClient.Common.WcfDataServices
{
	public class Wms3plClientInspector : IClientMessageInspector
	{
		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			var header = MessageHeader.CreateHeader("schema", "http://Wms3pl", "WMS");
			request.Headers.Add(header);
			if (Wms3plSession.CurrentUserInfo != null)
			{
				var header2 = MessageHeader.CreateHeader("userName", "http://Wms3pl", HttpUtility.UrlEncode(Wms3plSession.CurrentUserInfo.AccountName));
				request.Headers.Add(header2);
				//header2 = MessageHeader.CreateHeader("deviceName", "http://Wms3pl", System.Environment.MachineName);
				//request.Headers.Add(header2);
			}

			//var messageProperties = OperationContext.Current.OutgoingMessageProperties;
			//HttpRequestMessageProperty httpRequestMessage;
			//object httpRequestMessageObject;
			//if (messageProperties.TryGetValue(HttpRequestMessageProperty.Name, out httpRequestMessageObject))
			//{
			//    httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;
			//    if (string.IsNullOrEmpty(httpRequestMessage.Headers["schema"]))
			//    {
			//        httpRequestMessage.Headers["schema"] = "SMART" + Wms3plSession.Get<GlobalInfo>().Dc;
			//    }
			//}
			//else
			//{
			//    httpRequestMessage = new HttpRequestMessageProperty();
			//    httpRequestMessage.Headers.Add("schema", "SMART" + Wms3plSession.Get<GlobalInfo>().Dc);
			//    request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestMessage);
			//}
			return null;

		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
		}
	}
}

