using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib.Controls;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.UILib.Utility;
using Telerik.Windows.Controls;
using System.Reflection;
using System.Security.Principal;
using Wms3pl.WpfClient.DataServices.F91DataService;
using CrystalDecisions.CrystalReports.Engine;

namespace Wms3pl.WpfClient.UILib
{
	public class Wms3plUserControl : UserControl
	{
		/// <summary>
		///   關閉時的動作
		/// </summary>
		public Func<bool> OnClosing = delegate
		{
			return true;
		};
		private Wms3plContainer _Wms3plContainer;

		private bool _hasBeenLoaded;

		private Function _function;
		private ScaleTransform _scaleTransform;
		private List<AsyncDelegateCommand> _asyncCommands = new List<AsyncDelegateCommand>();



		public Wms3plUserControl()
		{
			//套用 User Control 的 style
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				Wms3plSession.Get<GlobalInfo>().IsSecretePersonalDataVm = IsSecretePersonalData;
				Wms3plSession.Get<GlobalInfo>().FunctionCode = this.GetType().Name;
				Style = FindResource("UserControlStyle") as Style;
				CustCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				Initialized += Wms3plUserControl_Initialized;
			}
			_scaleTransform = new ScaleTransform();
			this.LayoutTransform = _scaleTransform;
			Loaded += Wms3plUserControl_Loaded;
			this.PreviewMouseWheel += window_PreviewMouseWheel;
			this.Unloaded += (s, e) =>
			{
				Messenger.Default.Unregister(this);
			};
		}

		public bool CanClose()
		{
			if (!CheckIsBusy()) return false;
			if (!OnClosing()) return false;
			return true;
		}

		protected bool CheckIsBusy()
		{
			if (this.DataContext is InputViewModelBase)
			{
				if (((InputViewModelBase)this.DataContext).IsBusy)
					return false;
			}
			return true;
		}

		private void GCCollect()
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}

		public string CustCode { get; set; }

		#region FormStatus

		public static readonly DependencyProperty FormStatusProperty =
			DependencyProperty.Register("FormStatus", typeof(FormStatus), typeof(Wms3plUserControl),
																	new PropertyMetadata(default(FormStatus), OnFormStatusChanged));

		private static void OnFormStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var newStatus = (FormStatus)e.NewValue;
			var uc = d as Wms3plUserControl;
			if (d.CheckAccess())
			{
				UpdateFormStatus(uc, newStatus);
			}
			else
			{
				d.Dispatcher.Invoke(DispatcherPriority.Normal,
					new Action<Wms3plUserControl, FormStatus>(UpdateFormStatus), uc, newStatus);
			}
		}

		private static void UpdateFormStatus(Wms3plUserControl uc, FormStatus newStatus)
		{
			switch (newStatus)
			{
				case FormStatus.Normal:
					uc.ChangeHeaderTemplate("NormalHeaderTemplate", false);
					break;
				case FormStatus.Wrong:
					uc.ChangeHeaderTemplate("ErrorHeaderTemplate", false);
					break;
				case FormStatus.Busy:
					uc.ChangeHeaderTemplate("BusyHeaderTemplate", false);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public FormStatus FormStatus
		{
			get { return (FormStatus)GetValue(FormStatusProperty); }
			set
			{
				if (this.CheckAccess())
				{
					SetValue(FormStatusProperty, value);
				}
				else
				{
					this.Dispatcher.Invoke(DispatcherPriority.Normal,
					new Action(() => { SetValue(FormStatusProperty, value); }));
				}
			}
		}

		#endregion

		#region FormException

		public static readonly DependencyProperty FormExceptionProperty =
			DependencyProperty.Register("FormException", typeof(Exception), typeof(Wms3plUserControl),
																	new PropertyMetadata(default(Exception), OnFormExceptionChanged));

		private static void OnFormExceptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var Wms3plUserControl = d as Wms3plUserControl;
			var excepion = e.NewValue as Exception;
			Wms3plUserControl.ShowError(excepion);
			if (excepion != null)
			{
				Wms3plUserControl.FormStatus = FormStatus.Wrong;
			}
			else
			{
				if (Wms3plUserControl.FormStatus == FormStatus.Wrong)
					Wms3plUserControl.FormStatus = FormStatus.Normal;
			}
		}

		public Exception FormException
		{
			get { return (Exception)GetValue(FormExceptionProperty); }
			set { SetValue(FormExceptionProperty, value); }
		}

		#endregion


		private void window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			bool handle = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
			if (!handle) return;
			int count = e.Delta;
			var nowCount = _scaleTransform.ScaleX + count * 0.001;
			if (nowCount > 0)
			{
				_scaleTransform.ScaleX = _scaleTransform.ScaleY = nowCount;
			}
		}

		public Wms3plUserControl(Function function)
			: this()
		{
			Function = function;
		}

		public Function Function
		{
			get { return _function; }
			set
			{
				_function = value;
				_Wms3plContainer = new Wms3plContainer(_function);
				if (_function != null)
					SetFunctionAndTitle();
			}
		}

		private void SetFunctionAndTitle()
		{
			var master = FindName("Master") as BasicMaster;
			if (master != null && _function != null)
			{
				master.FunctionId = _function.Id;
				master.FunctionName = _function.Name;
				if (_isShowAccountInfo)
				{
					master.AccountName = Wms3plSession.Get<UserInfo>().AccountName;
					DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal,
																									delegate { master.NowDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm"); },
																									this.Dispatcher);
					master.AccountVisibility = Visibility.Visible;
				}
				master.PositionName = ConfigurationManager.AppSettings["PositionName"].ToString();
				if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
					master.VersionNo = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
				else
					master.VersionNo = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		public void ShowError(Exception exception, bool logError = false)
		{
			if (exception == null)
				ClearMessage();
			else
			{
				this.ShowExceptionMessage(exception);
				if (logError)
				{ ErrorHandleHelper.HandleException(exception); }
			}
		}

		public void ShowError(string message)
		{
			DialogService.ShowException(message);
		}

		public DialogResponse ShowMessage(MessagesStruct msg)
		{
			return DialogService.ShowMessage(msg.Message, msg.Title, msg.Button, msg.Image);
		}

		public void ShowMessage(string message)
		{
			//DialogService.ShowMessage(Window.GetWindow(this), message);
			DialogService.ShowMessage(message);
			//this.Activate();
		}

		private void Wms3plUserControl_Loaded(object sender, RoutedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				if (!_hasBeenLoaded)
				{
					_hasBeenLoaded = true;
					_Wms3plContainer = new Wms3plContainer(Function, enableLog: false);

					//使用者可以用的功能
					var needCheck = (bool)GetValue(PermissionService.CheckPermissionProperty);
					if (needCheck) CheckPermission();
					SetFunctionAndTitle();

					var container = new UnityContainer();
					var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
					section.Configure(container, "");
					_dialogService = container.Resolve<IDialogService>(
						new ParameterOverride("view", this)
						);

				}

			}
		}

		/// <summary>
		/// 檢查使用者是否可以使用該Window
		/// </summary>
		/// <returns>true: 可以使用</returns>
		private void CheckPermission()
		{
			bool userCanExecute = _Wms3plContainer.CheckPermission();
			if (!userCanExecute)
			{
				MessageBox.Show(Window.GetWindow(this), "您並沒有權限使用該功能，請向管理員連絡。");
				IsEnabled = false;
			}
		}


		public void LogUsage(string message)
		{
			if (_Wms3plContainer != null)
				_Wms3plContainer.LogUsage(message);
		}

		/// <summary>
		/// </summary>
		/// <param name = "action"></param>
		/// <param name = "clearMessage">是否清除訊息</param>
		protected void SmartAction(Action action, bool clearMessage = false)
		{
			try
			{
				ClearMessage();
				action();
				if (clearMessage) ClearMessage();
			}
			catch (Exception e)
			{
				ErrorHandleHelper.HandleException(e);
				ShowError(e);
			}
		}

		/// <summary>
		/// 執行非同步
		/// </summary>
		/// <param name="asyncAction">非同步的動作。注意到不能含有更新UI的指令。</param>
		/// <param name="completed">非同步動作成功完成後的動作。通常是更新 UI</param>
		/// <param name="failed">若非同步動作失敗後的動作。通常是更新 UI，清除IsBusy</param>
		protected void AsyncAction(Action<object> asyncAction, Action<object> completed,
			Action<Exception> failed)
		{
			ClearMessage();
			AsyncDelegateCommand asyncCommand = null;
			asyncCommand = new AsyncDelegateCommand(asyncAction,
						() => true,
						o =>
						{
							if (completed != null) completed(o);
							_asyncCommands.Remove(asyncCommand);
							SetHeaderBusyStatus();
							SendCompleteMessage();
						},
						e =>
						{
							ErrorHandleHelper.HandleException(e);
							ShowError(e);
							if (failed != null) failed(e);
							SendCompleteMessage();
						});
			_asyncCommands.Add(asyncCommand);
			this.FormStatus = FormStatus.Busy;
			SendBusyMessage();
			asyncCommand.Execute(null);
		}

		private void SetHeaderBusyStatus()
		{
			this.FormStatus = IsAsyncBusy ? FormStatus.Busy : FormStatus.Normal;
		}

		private void SendCompleteMessage()
		{
			Messenger.Default.Send<NotificationMessage<bool>>(new NotificationMessage<bool>(this, true, "InQuery"), "InQuery");
		}

		private void SendBusyMessage()
		{
			Messenger.Default.Send<NotificationMessage<bool>>(new NotificationMessage<bool>(this, false, "InQuery"), "InQuery");
		}

		public bool IsAsyncBusy
		{
			get { return _asyncCommands.Any(i => i.IsBusy); }
		}


		protected void ClearMessage()
		{
			this.ShowMessage("", false);
		}

		public void Close()
		{
			if (CanClose()) //是否可以關閉
			{
				//到這裡，就是可以關閉了
				//試試是不是在 WpfClient 中
				var group = VisualTreeHelperExtension.FindVisualParent<RadPaneGroup>(this); //這個寫法行不通，可能是 Telerik 的 bug
																																										//if (group == null ) group = this.ParentOfType<RadPaneGroup>(); //這個會造成 drag 的效果，不知道為什麼

				if (group != null)
				{
					var radPane = group.SelectedPane;
					if (radPane != null)
					{
						radPane.DataContext = null;
						radPane.Header = null;
						radPane.Content = null;
						radPane.RemoveFromParent();
						group.Items.Remove(radPane);
					}
				}
				else
				{
					var window = Window.GetWindow(this);
					if ((window != null) && (!window.Equals(App.Current.MainWindow)))
						window.Close();
				}
				GCCollect();
				Wms3plSession.Get<GlobalInfo>().IsNeedGCCollect = true;
			}
		}

		/// <summary>
		/// 只給 MainWindow 用，其它請不要用
		/// </summary>
		/// <param name="closeByMainWindow"></param>
		public void Close(Action closeByMainWindow)
		{
			if (CanClose()) //是否可以關閉
			{
				closeByMainWindow();
				GCCollect();
				Wms3plSession.Get<GlobalInfo>().IsNeedGCCollect = true;
			}
		}

		public void Wms3plUserControlClose()
		{
			foreach (var field in this.GetType().GetRuntimeFields())
			{
				if (field.FieldType.BaseType == typeof(InputViewModelBase))
				{
					var vm = (InputViewModelBase)field.GetValue(this);
					vm.UserOperateMode = OperateMode.Query;
					vm = null;
					break;
				}
			}
		}
		public void Activate()
		{
			//var radPane = Generic.FindVisualParent<RadDocumentPane>(this);
			var radPane = this.Parent as RadDocumentPane;
			if (radPane != null) radPane.IsSelected = true;
		}

		protected object OpenWindow(string functionId, Window window = null)
		{
			//var formService = new FormService();
			var formService = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IFormService>();
			return formService.AddFunctionForm(functionId);
		}

		protected object OpenWindow(string functionId, OpenType openType = OpenType.DockTab,
			Window owner = null, params object[] parameters)
		{
			var formService = new FormService();
			return formService.AddFunctionForm(functionId, owner, parameters);
		}

		protected void FocusElement(string name)
		{
			var control = (FindName(name) as Control);
			if (control != null) control.Focus();
		}

		private IDialogService _dialogService;
		public IDialogService DialogService
		{

			get
			{
				return _dialogService;
			}
		}

		public void Show(OpenType openType = OpenType.DockTab)
		{
			if (this.Parent != null)
			{
				this.Activate();
			}
			else
				FormService.AddFunctionForm(Function, this, openType);
		}

		private bool _isShowAccountInfo = true;
		public bool IsShowAccountInfo
		{
			get { return _isShowAccountInfo; }
			set { _isShowAccountInfo = value; }
		}

		public bool IsSecretePersonalData
		{
			get
			{
				var funCode = this.GetType().Name;
				return !Wms3plSession.Get<GlobalInfo>().FunctionShowInfos.Any(a => a.FUN_CODE == funCode && a.SHOWINFO == "1");
			}
		}

		public void ControlView(Action action)
		{
			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, action);
		}

		private bool _isSelectAll;
		private UIElement _focusingElement = null;
		public void SetFocusedElement(UIElement value, bool isSelectAll = false)
		{
			_isSelectAll = isSelectAll;
			if (Dispatcher.CheckAccess())
			{
				SetFocusedElementImpl(value, isSelectAll);
			}
			else
			{
				Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (Action)(() => { SetFocusedElementImpl(value, isSelectAll); }));
			}
		}

		private void SetFocusedElementImpl(UIElement value, bool isSelectAll)
		{
			if (value.IsEnabled)
			{
				if (value is TextBox && isSelectAll)
				{
					var txtBox = ((TextBox)value);
					if (txtBox.IsLoaded)
						txtBox.SelectAll();
					else
						Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (Action)(() => { txtBox.SelectAll(); }));
				}
				value.Focus();
			}
			else
			{
				_focusingElement = value;
				value.IsEnabledChanged += FWElemet_IsEnabledChanged;
			}
		}

		private void FWElemet_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var elem = ((UIElement)sender);
			if (_focusingElement.Equals(elem))
			{
				if (elem is TextBox && _isSelectAll)
				{
					var txtBox = ((TextBox)elem);
					txtBox.SelectAll();
				}
				elem.Focus();
				elem.IsEnabledChanged -= FWElemet_IsEnabledChanged;
			}
		}
		/// <summary>
		/// 該方法可確保從非同步回到同步也能將視野顯示到選擇的項目
		/// </summary>
		/// <param name="dataGrid"></param>
		/// <param name="selectedItem"></param>
		/// <param name="focusingElement">例如有多個TabItem, DataGrid不存在於該 TabItem時，可先用這個參數切換回 DataGrid 所在TabItem</param>
		public void ScrollIntoView(DataGrid dataGrid, object selectedItem, UIElement focusingElement = null)
		{
			if (focusingElement != null)
				_focusingElement = focusingElement;

			ControlView(() =>
					{
						if (focusingElement != null)
						{
							SetFocusedElement(focusingElement);
						}

						dataGrid.Focus();
						dataGrid.SelectedItem = selectedItem;
						if (dataGrid.SelectedItem != null)
						{
							dataGrid.ScrollIntoView(dataGrid.SelectedItem);
						}
					});
		}

		protected void CellElement_GotFocus(object sender, RoutedEventArgs e)
		{
			SetFocusedElement(((FrameworkElement)sender));
		}

		private void Wms3plUserControl_Initialized(object sender, EventArgs e)
		{
			if (this.DataContext is InputViewModelBase)
				((InputViewModelBase)this.DataContext).View = this;
		}

		public List<F910501> OpenDeviceWindow(string functionCode, string clientIp, string dcCode)
		{
			var deviceWindowService = new DeviceWindowService();
			return deviceWindowService.GetDeviceSettings(functionCode, clientIp, dcCode);

		}
		public void PrintReport(ReportClass report, F910501 device, PrintType printType, PrinterType printerType = PrinterType.A4)
		{
			CrystalReportService crystalReportService;
			if (printType == PrintType.Preview)
			{
				crystalReportService = new CrystalReportService(report);
				crystalReportService.ShowReport(null, PrintType.Preview);
			}
			else
			{
				crystalReportService = new CrystalReportService(report, device);
				crystalReportService.PrintToPrinter(printerType);
			}
		}
	}

}
