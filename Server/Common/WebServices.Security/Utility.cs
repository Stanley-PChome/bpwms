using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Wms3pl.WebServices.Security
{
  class Utility
  {
    public static void SendMail(string to, string password)
    {
      var client = new SmtpClient();
      var mail = new MailMessage();
      mail.To.Add(to);
      mail.Body = string.Format("您好，新的密碼為{0}", password);
      client.Send(mail);
    }
  }
}
