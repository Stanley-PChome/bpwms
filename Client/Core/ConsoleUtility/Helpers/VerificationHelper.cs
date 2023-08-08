using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleUtility.Helpers
{
	public static class VerificationHelper
	{
		private const string EmailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";


		/// <summary>
		/// 檢查Email格式
		/// </summary>
		/// <param name="email">Email Address</param>
		/// <returns>true:格式正確; false:格式錯誤</returns>
		public static bool CheckEmailFormat(string email)
		{
			var emailExpression = new Regex(EmailRegex, RegexOptions.Compiled | RegexOptions.Singleline);
			return !string.IsNullOrEmpty(email) && emailExpression.IsMatch(email);
		}
	}
}
