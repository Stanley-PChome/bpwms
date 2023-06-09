using System;
using System.Configuration.Provider;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Security;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Wms3pl.Common.Security;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Security;
using Wms3pl.WebServices.Process.P19.Services;
using Wms3pl.DBCore;

namespace Wms3pl.WebServices
{
	public class ECMembershipProvider : System.Web.Security.MembershipProvider
	{
		private const int PASSWORD_SIZE = 6;
		private bool _EnablePasswordRetrieval;
		private bool _EnablePasswordReset;
		//private bool _RequiresQuestionAndAnswer;
		//private bool _RequiresUniqueEmail;
		/// <summary>
		/// 鎖定成員資格使用者以前，所允許的無效密碼或密碼解答嘗試次數。
		/// </summary>
		private int _MaxInvalidPasswordAttempts;
		//private int _PasswordAttemptWindow;
		private int _MinRequiredPasswordLength;
		private int _MinRequiredNonalphanumericCharacters;
		private string _PasswordStrengthRegularExpression;
		//private MembershipPasswordFormat _PasswordFormat;

		/// <summary>
		/// The name of the application using the custom membership provider.
		/// </summary>
		/// <returns>
		/// The name of the application using the custom membership provider.
		/// </returns>
		public override string ApplicationName { get; set; }

		private string[] spliter = new string[] { "[Schema@]" };
		private string CurrentSchema { get; set; }
		public override bool ChangePassword(string username, string oldPassword, string newPassword)
		{
			var pwd = oldPassword.Split(spliter, StringSplitOptions.None).First();
			var schema = oldPassword.Split(spliter, StringSplitOptions.None).Last();
			CurrentSchema = DbSchemaHelper.ChangeRealSchema(schema);
			if (!ValidateUser(username, oldPassword)) return false;

			var args = new ValidatePasswordEventArgs(username, newPassword, false);
			OnValidatingPassword(args);

			if (args.Cancel)
				if (args.FailureInformation != null)
					throw args.FailureInformation;
				else
					throw new MembershipPasswordException("Change password canceled due to new password validation failure.");

			using (var context = SecurityUtil.GetContext(CurrentSchema))
			{
				var user = (from i in context.F1952s
							where i.EMP_ID == username
							select i).Single();
				user.PASSWORD = CryptoUtility.GetHashString(newPassword);
				user.LAST_ACTIVITY_DATE = DateTime.Now;
				user.LAST_LOGIN_DATE = DateTime.Now;
				user.LAST_PASSWORD_CHANGED_DATE = DateTime.Now;
				user.FAILED_PASSWORD_ATTEMPT_COUNT = 0;

				//add to history
				AddPasswordHistory(context, user.EMP_ID, user.PASSWORD);

				context.SaveChanges();
				return true;
			}
		}

		private static void AddPasswordHistory(Wms3plDbContext context, string empId, string password)
		{
			var history = new F1952_HISTORY { EMP_ID = empId, CRT_DATE = DateTime.Now, PASSWORD = password };
			context.F1952_HISTORYs.Add(history);
		}

		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			if (config == null)
				throw new ArgumentNullException("config");
			if (String.IsNullOrEmpty(name))
				name = "Wms3plMembershipProvider";
			if (string.IsNullOrEmpty(config["description"]))
			{
				config.Remove("description");
				config.Add("description", "Wms3pl MembershipSqlProvider");
			}

			base.Initialize(name, config);

			ApplicationName = SecurityUtil.GetStringValue(config, "applicationName", System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);

			_EnablePasswordRetrieval = SecurityUtil.GetBooleanValue(config, "enablePasswordRetrieval", false);
			_EnablePasswordReset = SecurityUtil.GetBooleanValue(config, "enablePasswordReset", true);
			//_RequiresQuestionAndAnswer = SecurityUtil.GetBooleanValue(config, "requiresQuestionAndAnswer", true);
			//_RequiresUniqueEmail = SecurityUtil.GetBooleanValue(config, "requiresUniqueEmail", true);
			_MaxInvalidPasswordAttempts = SecurityUtil.GetIntValue(config, "maxInvalidPasswordAttempts", 3, false, 0);
			//_PasswordAttemptWindow = SecurityUtil.GetIntValue(config, "passwordAttemptWindow", 10, false, 0);
			_MinRequiredPasswordLength = SecurityUtil.GetIntValue(config, "minRequiredPasswordLength", 7, false, 128);
			_MinRequiredNonalphanumericCharacters = SecurityUtil.GetIntValue(config, "minRequiredNonalphanumericCharacters", defaultValue: 0, zeroAllowed: true, maxValueAllowed: 128);

			_PasswordStrengthRegularExpression = config["passwordStrengthRegularExpression"];
			if (_PasswordStrengthRegularExpression != null)
			{
				_PasswordStrengthRegularExpression = _PasswordStrengthRegularExpression.Trim();
				if (_PasswordStrengthRegularExpression.Length != 0)
				{
					try
					{
						Regex regex = new Regex(_PasswordStrengthRegularExpression);
					}
					catch (ArgumentException e)
					{
						throw new ProviderException(e.Message, e);
					}
				}
			}
			else
			{
				_PasswordStrengthRegularExpression = string.Empty;
			}


			string temp_format = config["passwordFormat"];
			if (temp_format == null)
			{
				temp_format = "Hashed";
			}

			switch (temp_format)
			{
				case "Hashed":
					//_PasswordFormat = MembershipPasswordFormat.Hashed;
					break;
				case "Encrypted":
					//_PasswordFormat = MembershipPasswordFormat.Encrypted;
					break;
				case "Clear":
					//_PasswordFormat = MembershipPasswordFormat.Clear;
					break;
				default:
					throw new ProviderException("Password format not supported.");
			}
		}

		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
		{
			throw new NotImplementedException();
		}

		public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
		{
			throw new NotImplementedException();
		}

		public override bool DeleteUser(string username, bool deleteAllRelatedData)
		{
			throw new NotImplementedException();
		}

		public override bool EnablePasswordReset
		{
			get
			{
				return _EnablePasswordReset;
			}
		}

		public override bool EnablePasswordRetrieval
		{
			get { return _EnablePasswordRetrieval; }
		}

		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
			throw new NotImplementedException();
		}

		public override int GetNumberOfUsersOnline()
		{
			throw new NotImplementedException();
		}

		public override string GetPassword(string username, string answer)
		{
			throw new NotImplementedException();
		}

		public override MembershipUser GetUser(string username, bool userIsOnline)
		{
			var name = username.Split(spliter, StringSplitOptions.None).First();
			var schema = username.Split(spliter, StringSplitOptions.None).Last();

			bool isAdmin = (name.ToLower() == "wms");
			if (isAdmin)
			{
				var member = new MembershipUser("xxxprovider", name, name, "aaa@bbb.ccc.ddd",
				"hello", "comment", true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
				return member;
			}
			else
			{
				using (var context = SecurityUtil.GetContext(schema))
				{
					var q = (from i in context.F1952s
							 where i.EMP_ID == name
							 select i).Single();
					var member = new MembershipUser("xxxprovider", name, name, "aaa@bbb.ccc.ddd",
													"hello", "comment", true, false, DateTime.Now, DateTime.Now, DateTime.Now,
													DateTime.Now, DateTime.Now);
					return member;
				}
			}

		}

		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		{
			return GetUser(providerUserKey.ToString(), userIsOnline);
		}

		public override string GetUserNameByEmail(string email)
		{
			throw new NotImplementedException();
		}

		public override int MaxInvalidPasswordAttempts
		{
			get { return _MaxInvalidPasswordAttempts; }
		}

		public override int MinRequiredNonAlphanumericCharacters
		{
			get { return _MinRequiredNonalphanumericCharacters; }
		}

		public override int MinRequiredPasswordLength
		{
			get { return _MinRequiredPasswordLength; }
		}

		public override int PasswordAttemptWindow
		{
			get { throw new NotImplementedException(); }
		}

		public override MembershipPasswordFormat PasswordFormat
		{
			get { throw new NotImplementedException(); }
		}

		public override string PasswordStrengthRegularExpression
		{
			get { return _PasswordStrengthRegularExpression; }
		}

		public override bool RequiresQuestionAndAnswer
		{
			get { throw new NotImplementedException(); }
		}

		public override bool RequiresUniqueEmail
		{
			get { throw new NotImplementedException(); }
		}

		public override string ResetPassword(string username, string answer)
		{
			var name = username.Split(spliter, StringSplitOptions.None).First();
			var schema = username.Split(spliter, StringSplitOptions.None).Last();
			CurrentSchema = DbSchemaHelper.ChangeRealSchema(schema);

			if (!EnablePasswordReset)
			{
				throw new NotSupportedException("Not_configured_to_support_password_resets");
			}

			SecurityUtil.CheckParameter(ref name, true, true, true, 256, "username");
			using (var context = SecurityUtil.GetContext(CurrentSchema))
			{
				var q = (from i in context.F1924s
						 where i.EMP_ID == name
						 select i).Single();
				if (q.EMAIL == answer)
				{
					string newPassword = GeneratePassword();
					ValidatePasswordEventArgs e = new ValidatePasswordEventArgs(name, newPassword, false);
					OnValidatingPassword(e);
					if (e.Cancel)
					{
						if (e.FailureInformation != null)
						{
							throw e.FailureInformation;
						}
						else
						{
							throw new ProviderException("Membership_Custom_Password_Validation_Failure");
						}
					}

					var f1952 = (from i in context.F1952s
								 where i.EMP_ID == name
								 select i).Single();

					f1952.FAILED_PASSWORD_ATTEMPT_COUNT = 0;
					f1952.PASSWORD = CryptoUtility.GetHashString(newPassword);

					//AddPasswordHistory(context, username, f1952.PASSWORD);
					context.SaveChanges();
					return newPassword;
				}
				else
				{
					return "";
				}
			}

		}

		public virtual string GeneratePassword()
		{
			return Membership.GeneratePassword(
					  MinRequiredPasswordLength < PASSWORD_SIZE ? PASSWORD_SIZE : MinRequiredPasswordLength,
					  MinRequiredNonAlphanumericCharacters);
		}

		public override bool UnlockUser(string userName)
		{
			throw new NotImplementedException();
		}

		public override void UpdateUser(MembershipUser user)
		{
			throw new NotImplementedException();
		}

		public override bool ValidateUser(string username, string password)
		{
			var pwd = password.Split(spliter, StringSplitOptions.None).First();
			var schema = password.Split(spliter, StringSplitOptions.None).Last();
			CurrentSchema = DbSchemaHelper.ChangeRealSchema(schema);
			var adminHelper = new AdminHelper();
			var saIdFromReg = adminHelper.SaId;

			bool loginUsingAdminAccount = false;

			if (saIdFromReg != null)
				loginUsingAdminAccount = (username.ToLower() == saIdFromReg.ToLower());
			else
				loginUsingAdminAccount = (username.ToLower() == "wms");

			if (loginUsingAdminAccount)
			{
				if (saIdFromReg != null)
				{
					bool isOk = ((!string.IsNullOrEmpty(pwd)) &&
								 CryptoUtility.CompareHash(pwd, adminHelper.SaPwdHash));
					return isOk;
				}
				else
				{
					// F1924 需存在 wms 才能做管理員身分登入
					bool existsAccount = ExistsAccount(username.ToLower(), CurrentSchema);
					bool isAdmin = ((!string.IsNullOrEmpty(pwd)) && (pwd.ToLower().Trim() == "a1234567"));
					return existsAccount && isAdmin;
				}
			}
			else
			{
				var isOk = CheckPassword(username, pwd, true, CurrentSchema);
				if (!isOk)
					Logger.Write(string.Format("登入失敗！userName={0}", username));
				return isOk;
			}
		}

		private bool ExistsAccount(string username, string schema)
		{
			using (var context = SecurityUtil.GetContext(schema))
			{
				return context.F1924s.Any(x => x.EMP_ID == username);
			}
		}

		private bool CheckPassword(string username, string password, bool updateLastLoginActivityDate, string schema)
		{
			using (var context = SecurityUtil.GetContext(schema))
			{
				var user =
										  (from u in context.F1952s
										   from emp in context.F1924s
										   where u.EMP_ID == emp.EMP_ID && emp.ISDELETED == "0" && u.EMP_ID == username
										   select u).SingleOrDefault();
				if (user == null) return false;

				var isOk = CryptoUtility.CompareHash(password, user.PASSWORD);
				if (isOk)
				{
					if (updateLastLoginActivityDate)
					{
						// 通過密碼只更新最後登入時間，啟用時間由後續驗證使用者是否需要更換密碼那，要取得使用者時來判斷第一次是否強制更新密碼用，參考 ActivityGetF1952Ex
						user.LAST_LOGIN_DATE = DateTime.Now;					
					}
					user.STATUS = 0;
					user.FAILED_PASSWORD_ATTEMPT_COUNT = 0;					
				}
				else
				{
					// 更新最多可允許密碼輸入錯誤次數
					user.FAILED_PASSWORD_ATTEMPT_COUNT = (user.FAILED_PASSWORD_ATTEMPT_COUNT ?? 0) + 1;
					if (user.FAILED_PASSWORD_ATTEMPT_COUNT >= MaxInvalidPasswordAttempts)
					{
						user.STATUS = 1;
					}		
				}
				Current.DefaultStaff = username;
				context.SaveChanges();
				Current.DefaultStaff = "System";
				return isOk;
			}
		}

		protected override void OnValidatingPassword(ValidatePasswordEventArgs args)
		{
			var p190506Service = new P190506Service();
			var checkResult = p190506Service.CheckPasswordByRule(args.UserName, args.Password, CurrentSchema);
			if (!checkResult.IsSuccessed)
				throw new ArgumentException(checkResult.Message);

			if (PasswordStrengthRegularExpression.Length > 0)
			{
				if (!Regex.IsMatch(args.Password, PasswordStrengthRegularExpression))
				{
					throw new ArgumentException("不符合 RegularExpression");
				}
			}

			//Calling base 
			base.OnValidatingPassword(args);

			if (args.Cancel)
			{
				if (args.FailureInformation == null)
					args.FailureInformation = new ArgumentException(String.Format("密碼規則失敗 '{0}'", args.Password));

				throw args.FailureInformation;
			}
		}


	}
}