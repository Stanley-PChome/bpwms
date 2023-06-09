using System.Web.Security;
using Wms3pl.WpfClient.Common;

namespace Wms3pl.WpfClient.Services
{
  public interface ILoginService
  {
    bool ValidateUser(string account, string password);
    UserInfo GetUser(string account);
    bool ChangePassword(string account, string oldPassword, string newPassword);

    void ResetPassword(string username, string answer);
		string CheckAccountHasUserLogin(string account);
		void DeleteLogFiles();
		string GetF1924DataByAccount(string account,string password);
	}
}