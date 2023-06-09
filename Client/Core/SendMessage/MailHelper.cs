using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SendMessage
{
	public class MailHelper
	{

		/// <summary>
		/// 寄EMail
		/// </summary>
		/// <param name="mailTo">收件者(可複數，以';'做分割)</param>
		/// <param name="mailSubject">Mail 主旨</param>
		/// <param name="mailBody">Mail 本文</param>
		/// <param name="attachment">附件(可複數，以';'做分割)</param>
		public void SendMail(string mailTo, string mailSubject, string mailBody, string attachment = null)
		{
			SendMail(null, mailTo, null, null, attachment, mailSubject, mailBody);
		}

		/// <summary>
		/// 寄EMail
		/// </summary>
		/// <param name="mailFrom">來源寄件者</param>
		/// <param name="mailTo">收件者(可複數，以';'做分割)</param>
		/// <param name="mailcc">收件副件者(可複數，以';'做分割)</param>
		/// <param name="mailBcc">密件(可複數，以';'做分割)</param>
		/// <param name="mailSubject">Mail 主旨</param>
		/// <param name="mailBody">Mail 本文</param>
		/// <param name="attachment">附件(可複數，以';'做分割)</param>
		/// <param name="displayName">寄件者DisplayName(可略，另作參數是因為不同信件可能名稱有所調整，寄件者名稱可再設config參數來控制與共用)</param>
		public void SendMail(string mailFrom, string mailTo, string mailcc, string mailBcc, string attachment, string mailSubject, string mailBody, string displayName = "")
		{
			var symbol = ';';//分割符號
			using (var mail = new MailMessage())
			{
				if (!string.IsNullOrWhiteSpace(mailFrom))
				{
					mail.From = string.IsNullOrEmpty(displayName)
						? new MailAddress(mailFrom)
						: new MailAddress(mailFrom, displayName);
				}

				if (!string.IsNullOrWhiteSpace(mailTo))
				{
					foreach (var sendTo in mailTo.Split(symbol).Where(x => !string.IsNullOrEmpty(x)))
					{
						mail.To.Add(new MailAddress(sendTo));
					}
				}
				mail.Subject = mailSubject;
				mail.Body = mailBody;
				mail.IsBodyHtml = true;
				mail.BodyEncoding = System.Text.Encoding.UTF8;//解決中文變亂碼

				if (!string.IsNullOrWhiteSpace(mailcc))
				{
					foreach (var cc in mailcc.Split(symbol).Where(x => !string.IsNullOrEmpty(x)))
					{
						mail.CC.Add(cc);
					}
				}
				if (!string.IsNullOrWhiteSpace(mailBcc))
				{
					foreach (var bcc in mailBcc.Split(symbol).Where(x => !string.IsNullOrEmpty(x)))
					{
						mail.Bcc.Add(bcc);
					}
				}
				if (!string.IsNullOrWhiteSpace(attachment))
				{
					foreach (var att in attachment.Split(symbol).Where(x => !string.IsNullOrEmpty(x)))
					{
						mail.Attachments.Add(new Attachment(att));
					}
				}
				var smtpClient = new SmtpClient();
				smtpClient.Send(mail);

			}
		}
	}
}
