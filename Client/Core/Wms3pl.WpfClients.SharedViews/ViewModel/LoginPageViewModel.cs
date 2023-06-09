using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.ClientServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.LabelPrinter.Bartender;
using Wms3pl.WpfClient.ExDataServices;
using System.Collections.Generic;
using Wms3pl.WpfClient.UILib.Utility;
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Data.Services.Client;
using Wms3pl.WpfClient.DataServices;

namespace Wms3pl.WpfClients.SharedViews.ViewModel
{
	public class LoginPageViewModel : ViewModelBase
	{
		public Action LoginSuccessful = delegate { };

		protected WindowsIdentity _wi;
		protected WindowsImpersonationContext _wic;

		#region UserName

		/// <summary>
		/// The <see cref="UserName" /> property's name.
		/// </summary>

		public const string UserNamPropertyName = "UserName";

#if DEBUG
		private string _userName = "wms";
#else
    private string _userName = default(string);
#endif

		/// <summary>
		/// Gets the UserName property.
		/// </summary>
		public string UserName
		{
			get
			{
				return _userName;
			}

			set
			{
				if (_userName == value) return;
				_userName = value;
				RaisePropertyChanged(UserNamPropertyName);
			}
		}
		#endregion UserName

		#region UserAccounName

		/// <summary>
		/// The <see cref="UserAccounName" /> property's accountname.
		/// </summary>

		public const string UserAccountNamePropertyName = "UserAccountName";

#if DEBUG
		private string _userAccountName = default(string);
#else
    private string _userAccountName = default(string);
#endif

		/// <summary>
		/// Gets the UserAccountName property.
		/// </summary>
		public string UserAccountName
		{
			get
			{
				return _userAccountName;
			}

			set
			{
				if (_userAccountName == value) return;
				_userAccountName = value;
				RaisePropertyChanged(UserAccountNamePropertyName);
			}
		}
		#endregion UserAccountName

		#region ErrorMessage

		/// <summary>
		/// The <see cref="ErrorMessage" /> property's name.
		/// </summary>
		public const string ErrorMessagePropertyName = "ErrorMessage";

		private string _errorMessage = default(string);

		/// <summary>
		/// Gets the ErrorMessage property.
		/// </summary>
		public string ErrorMessage
		{
			get
			{
				return _errorMessage;
			}

			set
			{
				if (_errorMessage == value) return;
				_errorMessage = value;
				RaisePropertyChanged(ErrorMessagePropertyName);
			}
		}
		#endregion ErrorMessage

		#region IsBusy

		/// <summary>
		/// The <see cref="IsBusy" /> property's name.
		/// </summary>
		public const string IsBusyPropertyName = "IsBusy";

		private bool _isBusy = default(bool);

		/// <summary>
		/// Gets the IsBusy property.
		/// </summary>
		public bool IsBusy
		{
			get
			{
				return _isBusy;
			}

			set
			{
				if (_isBusy == value) return;
				_isBusy = value;
				RaisePropertyChanged(IsBusyPropertyName);
			}
		}
		#endregion IsBusy

		#region ExceptionMessage

		/// <summary>
		/// The <see cref="ExceptionMessage" /> property's name.
		/// </summary>
		public const string ExceptionMessagePropertyName = "ExceptionMessage";

		private string _exceptionMessage = default(string);

		/// <summary>
		/// Gets the ExceptionMessage property.
		/// </summary>
		public string ExceptionMessage
		{
			get
			{
				return _exceptionMessage;
			}

			set
			{
				if (_exceptionMessage == value) return;
				_exceptionMessage = value;
				RaisePropertyChanged(ExceptionMessagePropertyName);
			}
		}

		private Visibility _isShowMessage;
		public Visibility IsShowMessage
		{ 
			get { return _isShowMessage; }
			set {Set(ref _isShowMessage, value); }
		}
		#endregion ExceptionMessage		

		#region GupCode

		/// <summary>
		/// The <see cref="GupCode" /> property's name.
		/// </summary>
		public const string GupCodePropertyName = "GupCode";

		private string _gupCode = default(string);

		/// <summary>
		/// Gets the GupCode property.
		/// </summary>
		public string GupCode
		{
			get
			{
				return _gupCode;
			}

			set
			{
				if (_gupCode == value) return;
				_gupCode = value;
				RaisePropertyChanged(GupCodePropertyName);
			}
		}
		#endregion GupCode

		#region CustCode

		/// <summary>
		/// The <see cref="CustCode" /> property's name.
		/// </summary>
		public const string CustCodePropertyName = "CustCode";

		private string _custCode = default(string);

		/// <summary>
		/// Gets the CustCode property.
		/// </summary>
		public string CustCode
		{
			get
			{
				return _custCode;
			}

			set
			{
				if (_custCode == value) return;
				_custCode = value;
				RaisePropertyChanged(CustCodePropertyName);
			}
		}
		#endregion CustCode

		private Visibility _showLang;
		public Visibility ShowLang 
		{
			get { return _showLang; }
			set { Set(ref _showLang, value); }
		}

		private ICommand _loginTask;
		public ICommand LoginTask
		{
			get
			{
				return _loginTask ?? (_loginTask = new RelayCommand<PasswordBox>(async p =>
					{
						IsBusy = true;
						await Task.Run(() => DoLoginAsync(p.Password));
						IsBusy = false;
					},
						p => !IsBusy));
			}
		}

		private void DoLoginAsync(string password)
		{
			var schema = string.Empty;
			try
			{
				schema = SetGlobalSchema();
			}
			catch(Exception ex)
			{
				ErrorMessage = ex.Message;
				return;
			}
			
			var userInfo = new UserInfo
											 {
												 Account = UserName,
												 Password = password
											 };
			bool isValid;
			var message = string.Empty;
			using (var locator = new ServiceLocator())
			{
				var loginService = locator.GetService<ILoginService>("");

				IsBusy = true;
				try
				{
					userInfo.Account = loginService.GetF1924DataByAccount(userInfo.Account, userInfo.Password + "[Schema@]" + schema);
					
					isValid = loginService.ValidateUser(userInfo.Account, userInfo.Password + "[Schema@]" + schema);
					if(isValid && !string.IsNullOrWhiteSpace(userInfo.Account))
					{
						var workStationName = GetWorkStationName();
						var identity = Thread.CurrentPrincipal.Identity as ClientFormsIdentity;
						userInfo.ClientFormsIdentity = identity;
						Wms3plSession.Set(userInfo);
						var dcService = new ServiceLocator().GetService<IDcService>();
						userInfo.AccountName = (!string.IsNullOrEmpty(workStationName)) ? workStationName : dcService.GetAccountName(userInfo.Account);
						#region 登入檢核是否為共用帳號
						var checkService = locator.GetService<IDcService>();
						var isCommon = checkService.CheckIsCommon(userInfo.Account);
						if (!isCommon && !string.IsNullOrEmpty(UserAccountName))
						{
							ErrorMessage = string.Format("非共用帳號不可輸入共用帳號姓名。");
							return;
						}
						if (isCommon && string.IsNullOrEmpty(UserAccountName))
						{
							ErrorMessage = string.Format("請輸入共用帳號姓名。");
							return;
						}
						#endregion
						message = loginService.CheckAccountHasUserLogin(userInfo.Account);
						//依config檔設定存放天數刪除過期LogFile
						loginService.DeleteLogFiles();
					}
				}
				catch (System.Net.WebException exception)
				{
					if (exception.InnerException.Message.Contains("500"))
					{
						ErrorMessage = string.Format("請確認網路連線是否正常!");
					}
					else if (exception.InnerException.Message.Contains("503"))
					{
						ErrorMessage = string.Format("系統更新中，請稍後再試!");
					}
					else
					{
						ErrorMessage = string.Format("伺服器失敗。");
					}
					ExceptionMessage = exception.Message;
					IsShowMessage = Visibility.Visible;
					return;
				}
				catch (Exception exception)
				{
					ErrorMessage = string.Format("登入失敗。");
					ExceptionMessage = exception.Message;
					IsShowMessage = Visibility.Visible;
					return;
				}
			}
			IsShowMessage = Visibility.Hidden;

			try
			{
				if (isValid)
				{
					if (string.IsNullOrEmpty(message))
					{
						SetGlobalInfo();
						SystemSetting.ApplySetting(Lang);
						LoginSuccessful();

						var info = Wms3plSession.Get<GlobalInfo>();
						_wi = WindowsImpersonation.GetWindowsIdentity(info.ImpersonationAccount, info.ImpersonationDomain,
							info.ImpersonationPassword, false);
						if (_wi != null)
							_wic = _wi.Impersonate();
					}
					else
					{
						ErrorMessage = message;
					}
				}
				else
				{
					ErrorMessage = "帳號或密碼錯誤";
				}
			}
			catch (Exception e)
			{
				MessageBox.Show("Error!" + e.Message + e.StackTrace);
			}

			IsBusy = false;

		}

		private string GetWorkStationName()
		{
			var schema = AesCryptor.Current.Decode(Schemas.CoreSchema);
			if (schema.ToUpper().Contains("WORKSTATION="))
			{
				var schemas = schema.Split('#');
				foreach (var s in schemas)
				{
					if (s.ToUpper().Contains("WORKSTATION="))
					{
						return s.Split('=')[1];
					}
				}
			}
			return null;
		}

		private void SetGlobalInfo()
		{
			var sl = new ServiceLocator();
			var dcService = sl.GetService<IDcService>();
			var funService = sl.GetService<IFunctionService>();
			var user = Wms3plSession.Get<UserInfo>();
			var info = Wms3plSession.Get<GlobalInfo>();
			if (info == null) info = new GlobalInfo();

			info.DcGupCustDatas = dcService.GetF192402Data(user.Account);
			info.DcCodeList = (from a in info.DcGupCustDatas
				group a by new {a.DcCode, a.DcName}
				into g
				select new NameValuePair<string>
				{
					Name = g.First().DcName,
					Value = g.First().DcCode
				}).ToList();

			info.FunctionShowInfos = funService.GetFunctionShowInfos(user.Account).ToList();
			info.ItemPathDatas = dcService.GetItemImagePathDatas();

			info.SchemaName = SetGlobalSchema();
			info.ImpersonationDomain = dcService.GetFolderDomain();
			info.ImpersonationAccount = dcService.GetFolderUser();
			info.ImpersonationPassword = dcService.GetFolderPw();

			info.ClientIp = ReadRdpClientSessionInfo.GetRdpClientName();
			
			Wms3plSession.Set(info);
		}

		private string SetGlobalSchema()
		{
			return Schemas.CoreSchema;
		}

		public bool ShowLeng()
		{
			var schema = AesCryptor.Current.Decode(Schemas.CoreSchema);
			return schema.Contains("##TRANSON##");
		}

		#region Lang

		/// <summary>
		/// The <see cref="Lang" /> property's name.
		/// </summary>
		public const string LangPropertyName = "Lang";

		private string _lang = Thread.CurrentThread.CurrentUICulture.Name;

		/// <summary>
		/// Gets the Lang property.
		/// </summary>
		public string Lang
		{
			get
			{
				return _lang;
			}

			set
			{
				if (_lang == value) return;
				_lang = value;
				RaisePropertyChanged(LangPropertyName);
			}
		}
		#endregion Lang


		public List<NameValuePair<string>> Langs
		{
			get
			{
				return new List<NameValuePair<string>> {
										new NameValuePair<string> { Name = "繁體", Value = "zh-TW" },
										new NameValuePair<string> { Name = "简体", Value = "zh-CN" },
										new NameValuePair<string> { Name = "English", Value = "en-US" }};
			}
		}
	}
}