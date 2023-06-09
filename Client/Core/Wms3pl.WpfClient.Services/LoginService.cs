using System.Linq;
using System.Web.Security;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.Services.MembershipProviderService;
using Wms3pl.WpfClient.ExDataServices.SignalRExDataService;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using System.IO;
using System.Configuration;
using System;

namespace Wms3pl.WpfClient.Services
{
	public partial class LoginService : ILoginService
	{
		public bool ValidateUser(string account, string password)
		{
			return Membership.ValidateUser(account, password);
		}

		public string CheckAccountHasUserLogin(string account)
		{
			var proxy = ConfigurationExHelper.GetExProxy<SignalRExDataSource>(false, "Login");
			var result = proxy.CheckAccountHasUserLogin(account).FirstOrDefault();
			return result == null ? string.Empty : result.Message;
		}

		public UserInfo GetUser(string account)
		{
			var proxy = ConfigurationExHelper.GetExProxy<P19ExDataSource>(false, "Login");
			var f1952Ex = proxy.ActivityGetF1952Ex(account);

			if (f1952Ex.EMP_ID.ToLower() == "wms")
				f1952Ex.IsAccountFirstResetPassword = false;

			return new UserInfo
			{
				Account = f1952Ex.EMP_ID,
				IsLocked = f1952Ex.STATUS == 1,
				LastChangePasswordDate = f1952Ex.LAST_PASSWORD_CHANGED_DATE,
				IsAccountFirstResetPassword = f1952Ex.IsAccountFirstResetPassword,
				IsCommon = f1952Ex.ISCOMMON == "1",
				IsOverPasswordValidDays = f1952Ex.IsOverPasswordValidDays,
				LastActivityDate = f1952Ex.LAST_ACTIVITY_DATE,
				Message = f1952Ex.Message,
			};
		}


		public bool ChangePassword(string account, string oldPassword, string newPassword)
		{
#if (Ph || Ph_A7)
			var service = new MembershipProviderClient("BasicHttpsBinding_MembershipProvider");
#else
			var service = new MembershipProviderClient("BasicHttpBinding_MembershipProvider");
#endif
			return service.ChangePassword(account, oldPassword, newPassword);
		}


		public void ResetPassword(string username, string answer)
		{
#if (Ph || Ph_A7)
			var service = new MembershipProviderClient("BasicHttpsBinding_MembershipProvider");
#else
			var service = new MembershipProviderClient("BasicHttpBinding_MembershipProvider");
#endif
			service.ResetPassword(username, answer);
		}

		public F1924 GetUserData(string account)
		{
			var proxy = ConfigurationHelper.GetProxy<F19Entities>(false, "Login");
			return proxy.F1924s.Where(x => x.EMP_ID == account).FirstOrDefault();
		}

		public void DeleteLogFiles()
		{
			string pathName = string.Format(ConfigurationManager.AppSettings["ClientExceptionLogFormatPath"], string.Empty);
			int days = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["LogFileStoreDays"]) ? 0 : Convert.ToInt32(ConfigurationManager.AppSettings["LogFileStoreDays"]);
			var directoryName = Path.GetDirectoryName(pathName);
			if (!Directory.Exists(directoryName))
				return;
			var fileList = Directory.GetFiles(directoryName);

			foreach (var file in fileList)
			{
				var createDate = File.GetLastWriteTime(file);
				TimeSpan ts = DateTime.Now - createDate;
				if (ts.Days > days)
					File.Delete(file);
			}
		}

		public string GetF1924DataByAccount(string account,string password)
		{
			var proxy = ConfigurationExHelper.GetExProxy<P19ExDataSource>(false, "Login");
			var result = proxy.GetF1924DataByAccount(account, password).FirstOrDefault();
			return result == null ? string.Empty : result.EMP_ID;
		}
	}
}