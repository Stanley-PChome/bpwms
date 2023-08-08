using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Wms3pl.WpfClient.Common.Helpers
{
	public static class IPAddressHelper
	{

		public static bool Validate(string ipStr)
		{

			string validatedIP = string.Empty;

			//如果ip + Port的話，使用IPAddress.TryParse會無法解析成功
			//所以加入Uri來判斷看看
			Uri url;
			System.Net.IPAddress ip;
			if (Uri.TryCreate(string.Format("http://{0}", ipStr), UriKind.Absolute, out url))
			{
				var aryipStr = ipStr.Split('.');
				if (aryipStr.Length.Equals(4))
				{
					if (System.Net.IPAddress.TryParse(url.Host, out ip))
					{
						//合法的IP
						validatedIP = ip.ToString();
					}
				}

			}
			else
			{

				//可能是ipV6，所以用Uri.CheckHostName來處理
				var chkHostInfo = Uri.CheckHostName(ipStr);
				if (chkHostInfo == UriHostNameType.IPv6)
				{
					//V6才進來處理
					if (System.Net.IPAddress.TryParse(ipStr, out ip))
					{
						validatedIP = ip.ToString();
					}
					else
					{
						//後面有Port Num，所以再進行處理
						int colonPos = ipStr.LastIndexOf(":");
						if (colonPos > 0)
						{
							string tempIp = ipStr.Substring(0, colonPos - 1);
							if (System.Net.IPAddress.TryParse(tempIp, out ip))
							{
								validatedIP = ip.ToString();
							}
						}
					}
				}
			}

			return (!string.IsNullOrWhiteSpace(validatedIP));


		}
	}
}
