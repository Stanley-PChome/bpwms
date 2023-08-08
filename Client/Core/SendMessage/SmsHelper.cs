using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SendMessage
{
	public class SmsHelper
	{

		private bool IsMobile(string mobile)
		{
			return Regex.IsMatch(mobile, @"^0{0,1}13[0-9]{9}$");
		}


		/// <summary>
		/// 寄發簡訊
		/// </summary>
		/// <param name="msg">簡訊內容</param>
		/// <param name="phones">手機號碼</param>
		public void SendSms(string msg, params string[] phones)
		{
			var gc = new GlobalConfig();

			var smsService = gc.SmsServiceLink;
			using (var client = new WebClient())
			{
				foreach (var phone in phones)
				{
					var url = string.Format(smsService, phone, System.Web.HttpUtility.UrlEncode(msg));
					client.DownloadString(url);
					System.Threading.Thread.Sleep(3000);
				}
				client.Dispose();
			}
		}



	}
}
