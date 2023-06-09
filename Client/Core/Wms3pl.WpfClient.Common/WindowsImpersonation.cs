using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Security.Permissions;
using System.Runtime.InteropServices;

// Make sure we have permission to execute unmanged code
[assembly: SecurityPermission(SecurityAction.RequestMinimum, UnmanagedCode = true)]

namespace Wms3pl.WpfClient.Common
{
	public class WindowsImpersonation
	{
		// Define the external LogonUser method from advapi32.dll.
		[DllImport("advapi32.dll", SetLastError = true)]
		static extern int LogonUser(String UserName, String Domain, String Password, int LogonType, int LogonProvider,
		ref IntPtr Token);

		public static WindowsIdentity GetWindowsIdentity(string userName, string domain, string password,
			bool skipError = true)
		{
			// Create a new initialized IntPtr to hold the access token
			// of the user to impersonate.
			IntPtr token = IntPtr.Zero;
			try
			{
				if (userName == "NoSetting")
				{
					return null;
				}
				// Call LogonUser to obtain an access token for the user
				// "Bob" with the password "treasure". We authenticate against
				// the local accounts database by specifying a "." as the Domain
				// argument.
				//int ret = LogonUser(userName, domain, password, 9, 0, ref token);
				int ret = LogonUser(userName, domain, password, 2, 0, ref token);
				// If the LogonUser return code is zero an error has occured.
				// Display it and exit.
				if (ret == 0)
				{
					throw new Exception(string.Format("{0}登入失敗! 錯誤代碼 {1}", domain, Marshal.GetLastWin32Error().ToString()));
				}
				// Create a new WindowsIdentity from Bob's access token			
				var wi = new WindowsIdentity(token);
				return wi;
			}
			catch (Exception)
			{
				if (!skipError)
					throw;
			}
			
			return null;

		}
	}
}