using System;
using Wms3pl.WpfClient.Common;

namespace Wms3pl.WpfClient.Services
{
  public partial class FakeLoginService : ILoginService
  {
    public bool ValidateUser(string account, string password)
    {
      return true;
    }


    public UserInfo GetUser(string account)
    {
      return new UserInfo();
    }


    public bool ChangePassword(string account, string oldPassword, string newPassword)
    {
      return true;
    }

    public void ResetPassword(string username, string answer)
    {
      
    }

		public string CheckAccountHasUserLogin(string account)
		{
			return string.Empty;
		}

		public void DeleteLogFiles()
		{

		}
		public string GetF1924DataByAccount(string account,string password)
		{
			return string.Empty;
		}
	}
}