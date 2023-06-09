using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.LabelPrinter.Bartender;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.Utility;
using Wms3pl.WpfClients.SharedViews;
using Telerik.Windows.Controls;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using System.Threading.Tasks;
using Wms3pl.WpfClient.SignalRClient;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;

namespace Wms3pl.WpfClient
{
	/// <summary>
	///   Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public static RoutedCommand ShowFunctionCommand = new RoutedCommand();
		public static RoutedCommand CloseCurrentDocumentCommand = new RoutedCommand();
		public static RoutedCommand FindDocumentCommand = new RoutedCommand();
		public static RoutedCommand ExitCommand = new RoutedCommand();
		public static RoutedCommand FilterFocusCommand = new RoutedCommand();
		private LoginClientHub _loginHub;
		public MainWindow()
		{
			var sl = new ServiceLocator();
			var settingsService = sl.GetService<ISettingStorage>("");
			var settings = settingsService.Load(Wms3plSession.CurrentUserInfo.Account);
			Wms3plSession.Set(settings);
			if (settings != null) settings.ApplySettings();

			InitializeComponent();
			Application.Current.MainWindow.FontSize = 18;
			ShowFunctionCommand.InputGestures.Add(new KeyGesture(Key.F8));
			CloseCurrentDocumentCommand.InputGestures.Add(new KeyGesture(Key.F4, ModifierKeys.Control));
			//FindDocumentCommand.InputGestures.Add(new KeyGesture(Key.D9, ModifierKeys.Alt));

			SetTimer();

			//rotator.SetValue(AdRotatorExtensions.AdRotatorExtensions.ItemsSourceProperty, _ads);

			Messenger.Default.Register<NotificationMessage<string>>(this, nm => statusBarMessage.Content = nm.Content);
			Messenger.Default.Register<NotificationMessage<bool>>(this, "IsPinned",
			  nm => { Panel1.IsHidden = Panel1.IsPinned = nm.Content; });

			Messenger.Default.Register<NotificationMessage<bool>>(this, "InQuery",
				nm =>
				{
					this.Dispatcher.Invoke((Action)(() =>
					{
						cbCustCode.IsEnabled = cbGupCode.IsEnabled = nm.Content;
						var modifyingFormCount = Wms3plSession.Get<GlobalInfo>().ModifyingFormCount;
						if ((!nm.Content) && modifyingFormCount > 0)
						{
							NonChangeMsg.Text = string.Format(Properties.Resources.MainWindowxamlcs_CantChangeGupOrCust, modifyingFormCount);
							NonChangeMsg.Visibility = Visibility.Visible;
						}
						else
							NonChangeMsg.Visibility = Visibility.Hidden;
					}));

				});

			SetGlobalInfo();

			if ((settings != null) && (settings.NeedContinueUnClosedFunctions))
			{
				var formService = new FormService();
				settings.FunctionIds.ForEach(s => formService.AddFunctionForm(s));
				settings.ApplySettings();
			}

			_loginHub = new LoginClientHub(false, "MainWindow");
			_loginHub.SetVaildate += SetVaildate;
			_loginHub.SendMessage += SendMessage;
			_loginHub.ConnectAsync().Wait();

		}

		private void SetVaildate(string hostName, bool isValidate)
		{
			if (!isValidate)
			{
				DispatcherAction(() =>
				{
					var result = DialogService.Current.ShowMessage(this, string.Format(Properties.Resources.MainWindowxamlcs_ApplicationClose, hostName), Properties.Resources.MainWindow_Window_Warnning, DialogButton.OK, DialogImage.Warning, MessageBoxResult.OK);
					if (result == DialogResponse.OK)
					{
						_loginHub.Stop();
						Window_Closed(this, null);
						Environment.Exit(0);
					}
				});
			}
		}
		private void SendMessage(string message)
		{
			DispatcherAction(() =>
			{
				DialogService.Current.ShowMessage(this, message?.Replace("\r\n", Environment.NewLine), Properties.Resources.MainWindow_Window_Warnning, DialogButton.OK, DialogImage.Warning, MessageBoxResult.OK);
			});
		}
		public void DispatcherAction(Action action)
		{
			Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, action);
		}

		[DebuggerStepThrough]
		private void SetTimer()
		{
			DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal,
														delegate { this.statusBarItemTime.Content = DateTime.Now; },
														this.Dispatcher);
		}

		//private DispatcherTimer _gcTimer;
		//[DebuggerStepThrough]
		//private void SetGcTimer()
		//{
		//	if (_gcTimer == null)
		//		_gcTimer = new DispatcherTimer(new TimeSpan(0, 0, 30), DispatcherPriority.Normal,
		//																								delegate { GCCollecct(null, null); },
		//																								this.Dispatcher);
		//	else
		//	{
		//		_gcTimer.Stop();
		//		_gcTimer.Start();
		//	}
		//}

		private void ShowFunctionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Panel1.IsPinned = !Panel1.IsPinned;
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			var settings = new SettingsWindow();
			settings.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			settings.ShowDialog();
		}

		private void ChangeResovlution(object sender, RoutedEventArgs e)
		{
			var menuItem = sender as MenuItem;
			string res = menuItem.Tag as string;
			var ary = res.Split('*');
			Application.Current.MainWindow.Height = int.Parse(ary[1]);
			Application.Current.MainWindow.Width = int.Parse(ary[0]);
			Application.Current.MainWindow.ResizeMode = ResizeMode.CanResize;
			Application.Current.MainWindow.WindowState = WindowState.Normal;

			Application.Current.MainWindow.Left = 0;
			Application.Current.MainWindow.Top = 0;
			menuItem.IsChecked = true;
		}

		private bool _wantLogout = false;
		private bool _secondAsking = false;
		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (_wantLogout)
			{
				if (_secondAsking) return;
				var settings = Wms3plSession.Get<Wms3plSettings>();
				bool leave = false;

				if (settings.NeedLeavingConfirmation)
				{
					var result = DialogService.Current.ShowMessage(this, Properties.Resources.MainWindow_Window_Closing, Properties.Resources.MainWindow_Window_Warnning, DialogButton.OKCancel, DialogImage.Warning);
					leave = result == DialogResponse.OK;
					e.Cancel = (!leave);
					if (_wantLogout && leave && !_secondAsking)
					{
						if (_loginHub != null)
							_loginHub.Stop();
						_secondAsking = true;
						Application.Current.Shutdown(999);
					}
				}
				else
				{
					if (_loginHub != null)
						_loginHub.Stop();
					//不要要詢問，直接直出
					Application.Current.Shutdown(999);
				}
			}
			else
			{
				//正常的關閉
				var settings = Wms3plSession.Get<Wms3plSettings>();
				if (settings.NeedLeavingConfirmation)
				{
					var result = DialogService.Current.ShowMessage(this, Properties.Resources.MainWindow_Window_Closing, Properties.Resources.MainWindow_Window_Warnning, DialogButton.OKCancel, DialogImage.Warning, MessageBoxResult.OK);
					bool leave = result == DialogResponse.OK;
					e.Cancel = (!leave);
					if (leave)
					{
						if (_loginHub != null)
							_loginHub.Stop();
					}
				}
			}

		}

		private void FindDocumentCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var documentsRadPaneGroup = FindName("DocumentsRadPaneGroup") as RadPaneGroup;
			if (documentsRadPaneGroup != null)
			{
				var count = documentsRadPaneGroup.Items.Count;
				int index;
				if (int.TryParse(e.Parameter as string, out index))
					if (index <= count) documentsRadPaneGroup.SelectedIndex = index - 1;
			}
		}

		private void LogUsage(string message)
		{
			var account = Wms3plSession.Get<UserInfo>().Account;
			var entry = new LogEntry
			{
				Message = message,
				Categories = new[] { "Usage" }
			};
			entry.ExtendedProperties.Add("Account", account);
			Logger.Write(entry);

			Task.Run(() =>
			{
				try
				{
					var proxy = ConfigurationExHelper.GetExProxy<P19ExDataSource>(false, "LogUsage");
					proxy.InsertF0050(message, null, null);
				}
				catch { }
			});


		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			LogUsage(Properties.Resources.MainWindowxamlcs_Login);
			ShowGupCodes();
			ShowCustCodes();

			//今日工作指示查詢
			//var formService = new FormService();
			//formService.AddFunctionForm("P2115010000");
			if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
				statusBarVer.Content = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
			else
				statusBarVer.Content = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

			var proxy = ConfigurationExHelper.GetExProxy<ShareExDataSource>(false, "LogUsage");
			var db = proxy.CreateQuery<string>("GetConnectName").AddQueryExOption("key", AesCryptor.Current.Encode("a1234567")).ToList().FirstOrDefault();
			if (db != null)
			{
				statusBarVer.Content = AesCryptor.Current.Decode(db).ToLower() + " " + statusBarVer.Content;
			}
		}

		private void ShowCustCodes()
		{
			//cbCustCode.Items.Clear();
			cbCustCode.ItemsSource = (from a in Wms3plSession.Get<GlobalInfo>().DcGupCustDatas
									  where a.GupCode.Equals(cbGupCode.SelectedValue)
									  group a by new { a.CustCode, a.CustName }
										  into g
									  select g.First()).ToList();

			cbCustCode.SelectedIndex = 0;
			Wms3plSession.Get<GlobalInfo>().ItemImagePath = (from a in Wms3plSession.Get<GlobalInfo>().ItemPathDatas
															where a.GUP_CODE.Equals(cbGupCode.SelectedValue) && a.CUST_CODE.Equals(cbCustCode.SelectedValue)
															select a.PATH_ROOT)?.FirstOrDefault();
		}

		private void ShowGupCodes()
		{
			cbGupCode.Items.Clear();
			cbGupCode.ItemsSource = (from a in Wms3plSession.Get<GlobalInfo>().DcGupCustDatas
									 group a by new { a.GupCode, a.GupName }
										 into g
									 select g.First()).ToList();

			cbGupCode.SelectedIndex = 0;
		}

		private void CbGupCode_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cbGupCode.SelectedIndex < 0)
				return;

			Wms3plSession.Get<GlobalInfo>().GupCode = cbGupCode.SelectedValue.ToString();
			Wms3plSession.Get<GlobalInfo>().GupName = ((dynamic)cbGupCode.SelectedItem).GupName;
			ShowCustCodes();
		}

		private void CbCustCode_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (cbCustCode.SelectedIndex < 0)
				return;

			Wms3plSession.Get<GlobalInfo>().CustCode = cbCustCode.SelectedValue.ToString();
			Wms3plSession.Get<GlobalInfo>().CustName = ((dynamic)cbCustCode.SelectedItem).CustName;

			FormService.ResetAllForm();
		}

		private void SetGlobalInfo()
		{
			var userInfo = Wms3plSession.Get<UserInfo>();
			statusBarItemUserAccount.Content = userInfo.AccountName;
		}

		private void CloseCurrentDocumentCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var documentsRadPaneGroup = FindName("DocumentsRadPaneGroup") as RadPaneGroup;
			if (documentsRadPaneGroup != null)
			{
				var selectedDocument = (RadDocumentPane)documentsRadPaneGroup.SelectedItem;
				if (selectedDocument != null)
				{
					documentsRadPaneGroup.RemovePane(selectedDocument);
				}
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			var settings = Wms3plSession.Get<Wms3plSettings>();
			if (settings != null)
			{
				var q = (from RadPane radPane in DocumentsRadPaneGroup.Items
						 let Wms3plUserControl = radPane.Content as Wms3plUserControl
						 where Wms3plUserControl != null
						 select Wms3plUserControl.Function.Id).ToList();

				var functions = Wms3plSession.Get<IEnumerable<Function>>();
				var sl = new ServiceLocator();
				var settingsService = sl.GetService<ISettingStorage>("");

				settings.FunctionIds = settings.NeedContinueUnClosedFunctions ? q : null;
				settings.PreferredFunctionIds = (from f in functions
												 where f.IsChecked
												 select f.Id).ToList();
				settingsService.Save(settings, Wms3plSession.CurrentUserInfo.Account);
			}
		}

		private void ChangePassword_Click(object sender, RoutedEventArgs e)
		{
			var win = new ChangePasswordWindow(true);
			win.Show();
		}

		private void ParentDocking_PreviewClose(object sender, Telerik.Windows.Controls.Docking.StateChangeEventArgs e)
		{
			//RadPane 中 SmartControl 的為 CanClose 時才可關閉
			var current = e.Panes.Where(p => p.IsSelected).SingleOrDefault();
			if (current != null)
			{
				var userControl = current.Content as Wms3plUserControl;
				if (userControl != null)
				{
					var canClose = userControl.CanClose();
					e.Handled = !canClose;
				}
			}
			foreach (var p in e.Panes)
			{
				var closingUserControl = p.Content as Wms3plUserControl;
				if (closingUserControl != null)
				{
					closingUserControl.Wms3plUserControlClose();
				}
			}
			//SetGcTimer();
		}

		private void ExitCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
		}

		private void Logout(object sender, RoutedEventArgs e)
		{
			//Application.Current.MainWindow.Close();
			_wantLogout = true;
			Window_Closing(this, new CancelEventArgs());
			//if (_wantLogout)
			Window_Closed(this, null);
			//Application.Current.Exit()
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			LogUsage(Properties.Resources.MainWindowxamlcs_Logout);
		}

		private void ShowSystemInfo(object sender, RoutedEventArgs e)
		{
			var m1 = string.Format("{0} * {1}", System.Windows.SystemParameters.PrimaryScreenWidth, System.Windows.SystemParameters.PrimaryScreenHeight);
			var p = Process.GetCurrentProcess();
			var m2 = string.Format("\n" + Properties.Resources.MainWindowxamlcs_TotalMemorySet + "\n" + Properties.Resources.MainWindowxamlcs_PrivateMemorySet + "\n" + Properties.Resources.MainWindowxamlcs_GCTotalMemorySet,
								   p.WorkingSet64 / 1024 / 1024, p.PagedMemorySize64 / 1024 / 1024, GC.GetTotalMemory(true) / 1024 / 1024);

			MessageBox.Show(this, m1 + m2);
		}

		private void Window_Unloaded(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Unregister(this);
		}

		private void FilterFocusCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Messenger.Default.Send<NotificationMessage>(new NotificationMessage(this, "Filter"));
		}

		private void GCCollecct(object sender, RoutedEventArgs e)
		{
			LabelPrintHelper.Dispose();
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			//if (_gcTimer != null)
			//	_gcTimer.Stop();
		}

		private void cbDc_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SetGlobalInfo();
		}

	}
}