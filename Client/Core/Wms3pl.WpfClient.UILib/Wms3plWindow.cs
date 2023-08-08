using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
using System.Windows.Threading;
using System.Reflection;
using System.Windows.Input;
using System.Security.Principal;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.DataServices.F00DataService;

namespace Wms3pl.WpfClient.UILib
{
	public class Wms3plWindow : Window
	{
		private readonly bool _setTitle;
		private bool _hasBeenLoaded;

		private Function _function;
		private Wms3plContainer _Wms3plContainer;

		public Wms3plWindow()
			: this(true)
		{

		}

		public Wms3plWindow(bool setTitle = true)
		{
			_setTitle = setTitle;
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				Wms3plSession.Get<GlobalInfo>().IsSecretePersonalDataVm = IsSecretePersonalData;
				Wms3plSession.Get<GlobalInfo>().FunctionCode = this.GetType().Name;
				Style = FindResource("WindowStyle") as Style;
				Initialized += WindowInitialized;
			}

			Loaded += WindowLoaded;
			Closed += WindowClosed;
			Closing += WindowClosing;
			var classTypeName = this.GetType().Name.Substring(0);
			_function = FormService.GetFunctionFromSession(classTypeName);
			//_Wms3plContainer = new Wms3plContainer(_function);
			var container = new UnityContainer();
			var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
			section.Configure(container, "");
			_dialogService = container.Resolve<IDialogService>(new ParameterOverride("view", this));
		}

		private void WindowClosing(object sender, CancelEventArgs e)
		{
			if (this.DataContext is InputViewModelBase)
			{
				if (((InputViewModelBase)this.DataContext).IsBusy)
					e.Cancel = true;
			}
		}

		public Wms3plWindow(Function function, bool setTitle = true)
			: this()
		{
			Function = function;
			//_Wms3plContainer = new Wms3plContainer(_function);
			_setTitle = setTitle;

		}

		public Function Function
		{
			get { return _function; }
			set
			{
				_function = value;
				SetMaster();
			}
		}

		private void SetMaster()
		{
			if (_function != null)
			{
				var master = FindName("Master") as BasicMaster;
				if (master != null)
				{
					if (_setTitle)
					{
						master.FunctionId = _function.Id;
						master.FunctionName = _function.Name;
					}
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

				if (_setTitle) SetTitle();
			}
		}

		//由Function 製作 title 的路徑
		private void SetTitle()
		{
			var titles = new List<string>();
			var currentFuntion = _function;
			do
			{
				titles.Add(currentFuntion.Name);
				currentFuntion = currentFuntion.Parent;
			} while (currentFuntion != null);
			titles.Reverse();
			this.Title = _function.Id + " " + string.Join(" \\ ", titles.ToArray());
		}

		#region FormStatus

		public static readonly DependencyProperty FormStatusProperty =
			DependencyProperty.Register("FormStatus", typeof(FormStatus), typeof(Wms3plWindow),
																	new PropertyMetadata(default(FormStatus), OnFormStatusChanged));

		private static void OnFormStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var newStatus = (FormStatus)e.NewValue;
			var wd = d as Wms3plWindow;
			if (d.CheckAccess())
			{
				UpdateFormStatus(wd, newStatus);
			}
			else
			{
				d.Dispatcher.Invoke(DispatcherPriority.Normal,
					new Action<Wms3plWindow, FormStatus>(UpdateFormStatus), wd, newStatus);
			}
		}

		private static void UpdateFormStatus(Wms3plWindow wd, FormStatus newStatus)
		{
			switch (newStatus)
			{
				case FormStatus.Normal:
					wd.ChangeHeaderTemplate("NormalHeaderTemplate", false);
					break;
				case FormStatus.Wrong:
					wd.ChangeHeaderTemplate("ErrorHeaderTemplate", false);
					break;
				case FormStatus.Busy:
					wd.ChangeHeaderTemplate("BusyHeaderTemplate", false);
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
			DependencyProperty.Register("FormException", typeof(Exception), typeof(Wms3plWindow),
																	new PropertyMetadata(default(Exception), OnFormExceptionChanged));

		private static void OnFormExceptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var Wms3plWindow = d as Wms3plWindow;
			var excepion = e.NewValue as Exception;
			Wms3plWindow.ShowError(excepion);
			if (excepion != null)
			{
				Wms3plWindow.FormStatus = FormStatus.Wrong;
			}
			else
			{
				if (Wms3plWindow.FormStatus == FormStatus.Wrong)
					Wms3plWindow.FormStatus = FormStatus.Normal;
			}
		}

		public Exception FormException
		{
			get { return (Exception)GetValue(FormExceptionProperty); }
			set { SetValue(FormExceptionProperty, value); }
		}

		#endregion

		public void ShowError(Exception exception)
		{
			this.ShowExceptionMessage(exception);
		}
		public static void ShowError(string message)
		{
			Services.DialogService.Current.ShowException(message);
		}

		public static void ShowMessage(string message)
		{
			Services.DialogService.Current.ShowMessage(message);
		}

        private void WindowLoaded(object sender, RoutedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				if (!_hasBeenLoaded)
				{
					_hasBeenLoaded = true;
					_Wms3plContainer = new Wms3plContainer(Function);

					if (_function != null)
					{
						//check PermissionService.CheckPermission
						var needCheck = (bool)GetValue(PermissionService.CheckPermissionProperty);
						if (needCheck) CheckPermission();
						SetMaster();
					}
				}
				
				
			}
		}

		private void WindowClosed(object sender, EventArgs e)
		{
			foreach (var field in this.GetType().GetRuntimeFields())
			{
				if (field.FieldType.BaseType == typeof(InputViewModelBase))
				{
					var vm = (InputViewModelBase)field.GetValue(this);
					vm.UserOperateMode = OperateMode.Query;
					break;
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
				action();
				if (clearMessage) ClearMessage();
			}
			catch (Exception e)
			{
				ErrorHandleHelper.HandleException(e);
				//ShowError(e);
			}
		}

		protected void AsyncAction(Action<object> action, Action<object> completed, bool clearMessage = false)
		{
			var command = new AsyncDelegateCommand(action,
																						 () => true, (o) =>
																													 {
																														 completed(o);
																														 if (clearMessage) ClearMessage();
																													 }, e =>
																																{
																																	ErrorHandleHelper.HandleException(e);
																																	//ShowError(e);
																																});
			command.Execute(null);
		}


		protected void ClearMessage()
		{
			//this.ShowMessage("", false);
		}

		protected object OpenWindow(string functionId, params object[] parameters)
		{
			var formService = new FormService();
			return formService.AddFunctionForm(functionId, this, parameters);
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
				value.IsEnabledChanged -= FWElemet_IsEnabledChanged;
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

		private void WindowInitialized(object sender, EventArgs e)
		{
			if (this.DataContext is InputViewModelBase)
				((InputViewModelBase)this.DataContext).View = this;
		}
		private int _dataGridCurrentCellIndex = 0;
		private int _dataGridRowIndex = 0;
		protected void ReFocusDataGridCell(DataGrid grid)
		{
			var row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(grid.SelectedItem);
			_dataGridRowIndex = row.GetIndex();
			if (grid.CurrentCell.Column != null)
				_dataGridCurrentCellIndex = grid.Columns.IndexOf(grid.CurrentCell.Column);
			DispatcherAction(() =>
			{
				DataGridHelper.FocusCell(grid, _dataGridRowIndex, _dataGridCurrentCellIndex);
			});
		}
		public void DispatcherAction(Action action)
		{
			Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, action);
		}

        public List<F910501> OpenDeviceWindow(string functionCode, string clientIp, string dcCode)
        {
            var deviceWindowService = new DeviceWindowService();
            return deviceWindowService.GetDeviceSettings(functionCode, clientIp, dcCode);

        }

        public F0003 GetSysSetting(string functionCode, string dcCode, string apName)
        {
            var deviceWindowService = new DeviceWindowService();
            return deviceWindowService.GetSysSetting(functionCode, dcCode, apName);
        }
    }

	public static class Wms3plWindowExtension
	{
		public static void ShowStatusMessage(this Wms3plWindow userControl, string message)
		{
			Messenger.Default.Send<NotificationMessage<string>>(
				new NotificationMessage<string>(userControl, message, "Show"));
		}
	}
}