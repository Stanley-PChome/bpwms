using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SendMessage.Test
{
	[TestClass]
	public class SendMessage
	{
		[TestMethod]
		public void TestSmsHelper()
		{
			var smsHelper = new SmsHelper();
			smsHelper.SendSms("測試簡訊", "0939309779");
		}

		[TestMethod]
		public void TestMailHelper()
		{
			var mailHelper = new MailHelper();
			mailHelper.SendMail("simon@bankpro.com.tw", "主旨", "內文");

		}
	}
}
