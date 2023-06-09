using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.Services
{
	public class WcfProxy<TWcfServiceClient> where TWcfServiceClient : ICommunicationObject
	{
		public bool IsSecretePersonalData { get; private set; }
		public string FunctionCode { get; private set; }
		public TWcfServiceClient WcfServiceClient { get; private set; }
		private IClientChannel _clientChannel = null;

		public WcfProxy(bool isSecretePersonalData, string functionCode)
		{
			this.WcfServiceClient = Activator.CreateInstance<TWcfServiceClient>();
			IsSecretePersonalData = isSecretePersonalData;
			FunctionCode = functionCode;

			var innerChannelProperty = typeof(TWcfServiceClient).GetProperty("InnerChannel", BindingFlags.Public | BindingFlags.Instance);
			if (innerChannelProperty == null)
				throw new Exception("找不到 InnerChannel 屬性，因 TWcfServiceClient 並不是 ClientBase<T> 的WCF服務類別!");

			_clientChannel = innerChannelProperty.GetValue(WcfServiceClient) as IClientChannel;
		}

		public T RunWcfMethod<T>(Func<TWcfServiceClient, T> doMethod)
		{
			return WcfServiceHelper.Execute<T>(_clientChannel,
											   () => doMethod.Invoke(this.WcfServiceClient),
											   IsSecretePersonalData, FunctionCode);
		}
	}
}
