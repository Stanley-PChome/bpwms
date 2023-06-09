using System;

namespace Wms3pl.WpfClient.Common
{
	public class UserInfo
	{
		public string Account { get; set; }
		public string AccountName { get; set; }
		public string Password { get; set; }

		//public string GUP_CODE { get; set; }

		public bool IsLocked { get; set; }
		public DateTime? LastChangePasswordDate { get; set; }
		public DateTime? LastActivityDate { get; set; }
		public bool IsCommon { get; set; }
		public bool IsAccountFirstResetPassword { get; set; }
		public bool IsOverPasswordValidDays { get; set; }
		public string Message { get; set; }


		public System.Web.ClientServices.ClientFormsIdentity ClientFormsIdentity { get; set; }
	}
}
