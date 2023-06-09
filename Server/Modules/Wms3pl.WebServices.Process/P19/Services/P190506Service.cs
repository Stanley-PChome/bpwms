using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Common.Security;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Collections.Specialized;

namespace Wms3pl.WebServices.Process.P19.Services
{
	/// <summary>
	/// 系統功能
	/// </summary>
	public partial class P190506Service
	{
		private WmsTransaction _wmsTransaction;
		public P190506Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		///  取得User's Password
		/// </summary>
		/// <param name="empId"></param>
		public IQueryable<GetUserPassword> GetUserPassword(string empId)
		{
			var f1924repo = new F1924Repository(Schemas.CoreSchema, _wmsTransaction);

			var result = f1924repo.GetUserPassword(empId);

			return result;
		}
		
		/// <summary>
		/// 取得使用者登入時，各種設定的資料，並且若帳號第一次啟用，會依照設定來更新啟用
		/// </summary>
		/// <param name="empId"></param>
		/// <returns></returns>
		public F1952Ex ActivityGetF1952Ex(string empId)
		{
			var repo = new F1952Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1952ex = repo.GetF1952Ex(empId);
			if (f1952ex == null)
				return null;

			// 載入 config
			var appSettings = ConfigurationManager.AppSettings;
			// 密碼有效期間
			var passwordValidDays = GetConfigIntValue(appSettings, "PasswordValidDays", 90);
			// 帳號第一次啟用時要強制更新密碼
			var isAccountFirstResetPassword = GetConfigBoolValue(appSettings, "IsAccountFirstResetPassword", true);


			// 密碼有效期間: x 天 (default), 若未更新則不能登入 =>共用帳號不用強制改密碼
			if (f1952ex.ISCOMMON == "0" && f1952ex.LAST_PASSWORD_CHANGED_DATE.HasValue)
			{
				// 上次修改密碼日期 + x天，若大於今天的話，表示尚未過期，若小於的話表示密碼已過期
				if (f1952ex.LAST_PASSWORD_CHANGED_DATE.Value.Date.AddDays(passwordValidDays) < DateTime.Today)
				{
					f1952ex.IsOverPasswordValidDays = true;
					f1952ex.Message = string.Format(Properties.Resources.P190506Service_IsOverPasswordValidDays, passwordValidDays);
				}
			}

			// 尚未啟用
			if (!f1952ex.LAST_ACTIVITY_DATE.HasValue)
			{
				// 且要求第一次必須更改密碼
				if (isAccountFirstResetPassword)
				{
					f1952ex.IsAccountFirstResetPassword = true;
				}
				else
				{
					// 若沒有要求第一次要更改密碼，則算直接啟用帳號
					var f1952 = repo.Find(x => x.EMP_ID == empId);
					if (f1952 != null)
					{
						f1952.LAST_ACTIVITY_DATE = DateTime.Now;
						repo.Update(f1952);
					}
					
				}
			}

			return f1952ex;
		}

		/// <summary>
		/// 檢核密碼規則
		/// </summary>
		/// <param name="empId"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public ExecuteResult CheckPasswordByRule(string empId, string password,string schema = null)
		{
			// 載入 config
			var appSettings = ConfigurationManager.AppSettings;

			// 最小密碼長度
			var minRequiredPasswordLength = GetConfigIntValue(appSettings, "MinRequiredPasswordLength", 6);
			// 密碼至少要有幾個非數字字元
			var minRequiredNonalphanumericCharacters = GetConfigIntValue(appSettings, "MinRequiredNonalphanumericCharacters", 1);
			// 密碼不可重複次數
			var unrepeatableFrequency = GetConfigIntValue(appSettings, "UnrepeatableFrequency", 3);

			if (empId == password)
				return new ExecuteResult(false, Properties.Resources.P190506Service_AccountNeedDifferenceToPassWord);

			// pattern: 所有字串必須符合至少有一個數字，與至少有一個英文字母，且長度為 minRequiredPasswordLength ~ 30
			var pattern = string.Format(@"^(?=.*\d)(?=.*[A-Za-z]).{{{0},30}}$", minRequiredPasswordLength);
			if (!Regex.IsMatch(password, pattern))
				return new ExecuteResult(false, string.Format(Properties.Resources.P190506Service_PasswordInvalid, minRequiredPasswordLength));


			// pattern: 非數字
			pattern = @"[^\d]";
			if (Regex.Matches(password, pattern).Count < minRequiredNonalphanumericCharacters)
				return new ExecuteResult(false, string.Format(Properties.Resources.P190506Service_PasswordLimitCharInvalid, minRequiredNonalphanumericCharacters));


			// 檢查不能重複修改為最後幾次的密碼
			if (!CheckPasswordHistory(empId, password, unrepeatableFrequency, schema))
				return new ExecuteResult(false, string.Format(Properties.Resources.P190506Service_PasswordHistory_Duplicate, unrepeatableFrequency));

			return new ExecuteResult(true);
		}

		/// <summary>
		/// 取得設定檔的整數，若小於等於0也用預設值做防呆
		/// </summary>
		/// <param name="appSettings"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		int GetConfigIntValue(NameValueCollection appSettings, string name, int defaultValue)
		{
			string value = appSettings[name];
			int result;
			if (!int.TryParse(value, out result) || result <= 0)
				return defaultValue;
			return result;
		}
		/// <summary>
		/// 取得設定檔的整數
		/// </summary>
		/// <param name="appSettings"></param>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		bool GetConfigBoolValue(NameValueCollection appSettings, string name, bool defaultValue)
		{
			string value = appSettings[name];
			bool result;
			if (!bool.TryParse(value, out result))
				return defaultValue;
			return result;
		}

		public ExecuteResult UpdateP190506(string empId, string password, string confirmPassword, List<decimal> addgroups, List<decimal> removegroups, List<decimal> addworkgroups, List<decimal> removeworkgroups, List<string> scheduleList, string checkpackage)
		{
			//0.
			var f1952Repo = new F1952Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF1924 = new F1924Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF192401 = new F192401Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF192403 = new F192403Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF192405 = new F192405Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1952HistoryRepo = new F1952_HISTORYRepository(Schemas.CoreSchema, _wmsTransaction);

			//1.變更儲存密碼
			if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(confirmPassword))
			{
				if (password != confirmPassword)
					return new ExecuteResult(false, Properties.Resources.P190506Service_PasswordDifferenceToConfirmPassword);

				var checkResult = CheckPasswordByRule(empId, password);
				if (!checkResult.IsSuccessed)
					return checkResult;

				// 修改密碼與新增密碼修改歷史紀錄
				var hashPassword = CryptoUtility.GetHashString(password);
				var f1952 = f1952Repo.Find(x => x.EMP_ID.Equals(empId));
				if (f1952 != null)
				{
					f1952.PASSWORD = hashPassword;
					f1952.LAST_PASSWORD_CHANGED_DATE = DateTime.Now;
					f1952.FAILED_PASSWORD_ATTEMPT_COUNT = 0;
					f1952.STATUS = 0;
					f1952Repo.Update(f1952);
				}
				else
				{
					f1952Repo.Add(new F1952() { EMP_ID = empId, PASSWORD = hashPassword });
				}

				f1952HistoryRepo.Add(new F1952_HISTORY { EMP_ID = empId, PASSWORD = hashPassword });
			}

			//2.設定工作群組
			// 2.1. 更新F192401 - 刪除不在選取範圍裡的資料
			foreach (var p in removegroups)
				repoF192401.Delete(x => x.EMP_ID == empId && x.GRP_ID == p);

			// 2.2.更新F192401 - 寫入新資料
			foreach (var p in addgroups)
			{
				if (repoF192401.Find(x => x.EMP_ID.Equals(empId) && x.GRP_ID.Equals(p)) != null) continue;
				repoF192401.Add(new F192401() { EMP_ID = empId, GRP_ID = p });
			}

			//3.設定作業群組
			// 3.1. 更新F192403 - 刪除不在選取範圍裡的資料
			foreach (var p in removeworkgroups)
			{
				repoF192403.Delete(x => x.EMP_ID == empId && x.WORK_ID == p);
			}

			// 3.2 .更新F192403 - 寫入新資料
			foreach (var p in addworkgroups)
			{
				if (repoF192403.Find(x => x.EMP_ID.Equals(empId) && x.WORK_ID.Equals(p)) != null) continue;
				repoF192403.Add(new F192403() { EMP_ID = empId, WORK_ID = p });
			}
			//4.設定排程權限
			repoF192405.Delete(empId);
			foreach (var p in scheduleList)
			{
				if (!string.IsNullOrWhiteSpace(p))
				{
					repoF192405.Add(new F192405() { EMP_ID = empId, SCHEDULE_ID = p });
				}
			}

			//5.包裝刷驗解鎖權限設定

			var tmp = repoF1924.Find(x => x.EMP_ID.Equals(empId));
			if (tmp == null) return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.DataDelete};

			tmp.PACKAGE_UNLOCK = checkpackage;
			repoF1924.Update(tmp);

			return new ExecuteResult(true);
		}
		/// <summary>
		/// 檢查X次內密碼不能重複
		/// </summary>
		/// <param name="empId"></param>
		/// <param name="passWord"></param>
		/// <returns></returns>
		private bool CheckPasswordHistory(string empId, string passWord, int unrepeatableFrequency,string schema)
		{
			var f1952HistoryRepo = new F1952_HISTORYRepository(schema??Schemas.CoreSchema, _wmsTransaction);

			// 取得最後幾次的密碼修改紀錄
			var f1952Historys = f1952HistoryRepo.GetLastFrequencyF1952History(empId, unrepeatableFrequency).ToList();

			// 都沒有重複使用歷史密碼
			return (f1952Historys.All(x => !CryptoUtility.CompareHash(passWord, x.PASSWORD)));
		}

		/// <summary>
		///  傳回F1924(人員資料) + F192401(工作群組和人員關係) + F1953(工作群組)+F195301(群組與功能對應) List
		/// </summary>
		/// <param name="empId"></param>
		/// <param name="grpId"></param>
		/// <param name="grpName"></param>
		/// <param name="funCode"></param>
		/// <param name="funName"></param>
		/// <returns></returns>
		public IQueryable<EmpWithFuncionName> EmpWithFuncionName(string EmpId)
		{
			var f1924repo = new F1924Repository(Schemas.CoreSchema, _wmsTransaction);

			var result = f1924repo.EmpWithFuncionName(EmpId);

			return result;
		}

	}
}

