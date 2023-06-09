using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib.Services;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;
using System.Configuration;
using System.Security.Principal;
using Wms3pl.WpfClient.UILib.Utility;

namespace Wms3pl.WpfClients.SharedViews
{
	public abstract class Wms3plAppBase : Application
	{
		//public static WindowsIdentity WI { get; set; }
		//public static WindowsImpersonationContext WIC { get; set; }
		protected override void OnStartup(StartupEventArgs e)
		{
			var lang = SystemSetting.GetSystemLang();
			SystemSetting.ApplySetting(lang);

			EventManager.RegisterClassHandler(typeof(TextBox), TextBox.PreviewMouseDownEvent, new RoutedEventHandler(TextBox_PreviewMouseDown));
			EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotFocusEvent, new RoutedEventHandler((s, o) => { (s as TextBox).SelectAll(); }));
			base.OnStartup(e);
			//關閉所有動畫, see http://www.telerik.com/help/wpf/radexpander-howto-animation.html

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

			ShutdownMode = ShutdownMode.OnExplicitShutdown;
			DispatcherHelper.Initialize();
			
			LogonAndStartApplication();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);			
		}

		private void TextBox_PreviewMouseDown(object sender, RoutedEventArgs e)
		{
			var textBox = sender as TextBox;
			if (textBox != null && !textBox.IsFocused)
			{
				textBox.Focus();
				textBox.SelectAll();
				e.Handled = true;
			}
		}


		protected void LogonAndStartApplication()
		{
			try
			{
				bool validUser = DisplayLoginScreen();
				if (!validUser)
				{
					Shutdown();
					return;
				}

				using (var container = new ServiceLocator())
				{
					var loginService = container.GetService<ILoginService>(string.Empty);
					var user = loginService.GetUser(Wms3plSession.CurrentUserInfo.Account);
					//ConfigureLogFile(Wms3plSession.CurrentUserInfo.Account);

					if (user != null)
					{
						if (user.IsLocked)
						{
							DialogService.Current.ShowMessage("此帳號登入失敗過多次，已鎖定!\n請由系統管理員變更密碼解鎖。");
							Shutdown();
							return;
						}

						if (user.IsOverPasswordValidDays || user.IsAccountFirstResetPassword)
						{
							var result = ShowChangePasswordWindow(user);
							if (!result.HasValue || !result.Value)
							{
								// 不是共用帳號，密碼過期
								if (!user.IsCommon && user.IsOverPasswordValidDays)
								{
									DialogService.Current.ShowMessage("未變更密碼！\n非共用帳號超過密碼有效期間，需強制更新密碼！\n請重新登入！");
									Shutdown();
									return;
								}

								// 第一次須強制更變密碼
								if (user.IsAccountFirstResetPassword)
								{
									DialogService.Current.ShowMessage("未變更密碼！\n因帳號第一次啟用時，需強制更新密碼！\n請重新登入！");
									Shutdown();
									return;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				ErrorHandleHelper.HandleException(ex);
				Shutdown();
				throw;
			}


			//show splash screen

			try
			{
				SystemSetting.ApplySetting();
				if (ShowSplashScreen)
				{
					Assembly assmebly = Assembly.GetExecutingAssembly();
					var splashScreen = new SplashScreen(assmebly, "Assets/SplashScreen.png");
					splashScreen.Show(false);
					RunMainWindow();
					splashScreen.Close(TimeSpan.FromMilliseconds(0));
				}
				else
				{
					RunMainWindow();
				}
			}
			catch (Exception ex)
			{
				ErrorHandleHelper.HandleException(ex);
				DialogService.Current.ShowMessage("啟動程式時發生例外");
				Application.Current.Shutdown();
			}
		}

		protected virtual bool? ShowChangePasswordWindow(UserInfo userInfo)
		{
			var message = (userInfo.IsAccountFirstResetPassword) ? "第一次啟用帳號需變更密碼" : userInfo.Message;
			DialogService.Current.ShowMessage(message);
			var changePassword = new ChangePasswordWindow(false);
			var result = changePassword.ShowDialog();
			return result;
		}

		private bool _showSplashScreen = true;
		protected bool ShowSplashScreen
		{
			get { return _showSplashScreen; }
			set { _showSplashScreen = value; }
		}

		protected virtual bool DisplayLoginScreen()
		{
			var loginWindow = new LoginWindow();
			loginWindow.ShowDialog();
			bool isValid = (loginWindow.DialogResult.HasValue && loginWindow.DialogResult.Value);
			return isValid;
		}

		protected abstract void RunMainWindow();


		void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			ErrorHandleHelper.HandleException(e.Exception);
			string message = "系統發生錯誤!";
			DialogService.Current.ShowMessage(message, "錯誤", DialogButton.OK, DialogImage.Error);
			e.Handled = true;
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			ErrorHandleHelper.HandleException(e.ExceptionObject as Exception);
		}
	}
}
