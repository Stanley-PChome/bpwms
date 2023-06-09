using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib.Services;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System.Collections.Generic;
using Wms3pl.WpfClient.DataServices;
using System.Data.Services.Client;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.Services;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Media;

namespace Wms3pl.WpfClient.UILib
{
	public class InputViewModelBase : ViewModelBase
	{
		public InputViewModelBase()
		{
			if (!IsInDesignMode)
			{
				IsSecretePersonalData = Wms3plSession.Get<GlobalInfo>().IsSecretePersonalDataVm;
				
				FunctionCode = Wms3plSession.Get<GlobalInfo>().FunctionCode;
				var container = new UnityContainer();
				var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
				section.Configure(container, "");
				_dialogService = container.Resolve<IDialogService>(
					new ParameterOverride("View", this)
					);
			}
		}
		public Action<string, string> OnCheckError = delegate(string message, string elementName) { };

		public Action<string> OnFocusElement = delegate(string elementName) { };

		public IDialogService DialogService
		{
			get { return _dialogService; }
		}

		#region Exception

		/// <summary>
		/// The <see cref="Exception" /> property's name.
		/// </summary>
		public const string ExceptionPropertyName = "Exception";

		private Exception _exception = default(Exception);

		/// <summary>
		/// Gets the Exception property.
		/// </summary>
		public Exception Exception
		{
			get
			{
				return _exception;
			}

			set
			{
				if (_exception == value) return;
				_exception = value;
				RaisePropertyChanged(ExceptionPropertyName);
			}
		}
		#endregion Exception

		#region IsBusy

		/// <summary>
		///   The <see cref = "IsBusy" /> property's name.
		/// </summary>
		public const string IsBusyPropertyName = "IsBusy";

		private bool _isBusy;

		/// <summary>
		///   Gets the IsBusy property.
		/// </summary>
		public bool IsBusy
		{
			get { return _isBusy; }

			set
			{
				if (_isBusy == value)
				{
					return;
				}

				_isBusy = value;
				DoSendMessage();

				RaisePropertyChanged(IsBusyPropertyName);
			}
		}

		#endregion

		private OperateMode _userOperateMode = OperateMode.Query;
		public OperateMode UserOperateMode
		{
			get { return _userOperateMode; }
			set
			{
				if (_userOperateMode != OperateMode.Add && _userOperateMode != OperateMode.Edit && (value == OperateMode.Add || value == OperateMode.Edit))
					Wms3plSession.Get<GlobalInfo>().ModifyingFormCount++;
				else if ((_userOperateMode == OperateMode.Add || _userOperateMode == OperateMode.Edit) && value != OperateMode.Add && value != OperateMode.Edit)
					Wms3plSession.Get<GlobalInfo>().ModifyingFormCount--;

				_userOperateMode = value;

				DoSendMessage();
				RaisePropertyChanged("UserOperateMode");
			}
		}

		public bool IsSecretePersonalData { get; set; }

		public string FunctionCode { get; set; }

		private void DoSendMessage()
		{
			if (IsBusy || Wms3plSession.Get<GlobalInfo>().ModifyingFormCount > 0) SendBusyMessage(); else SendCompleteMessage();
		}

		private FrameworkElement _view;
		private IDialogService _dialogService;

		public FrameworkElement View
		{
			get { return _view; }
			set
			{
				_view = value;
				DialogService.View = _view;
			}
		}

		protected AsyncDelegateCommand CreateBusyAsyncCommand(Action<object> action,
			Func<bool> canExecute = null,
			Action<object> completed = null,
			Action<Exception> error = null,
			Action preAction = null)
		{
			return new AsyncDelegateCommand(
					o =>
					{
						action(o);
					},
					() => ((canExecute == null) ? true : canExecute()) && (!IsBusy),
					o =>
					{
						IsBusy = false;
						if (completed != null) completed(o);
					},
					ex =>
					{
						IsBusy = false;
						if (error != null) error(ex);
						Exception = ex;
					},
					() =>
					{
						if (preAction != null) preAction();
						IsBusy = true;
					}
				);
		}

		protected AsyncRelayCommand<T> CreateBusyAsyncCommand<T>(Action<T> action,
				Func<T, bool> canExecute = null,
				Action<T> completed = null,
				Func<T, bool> preFunc = null)
		{
			return new AsyncRelayCommand<T>(
					o => action(o),
					o => !IsBusy && ((canExecute == null) ? true : canExecute(o)),
					o =>
					{
						IsBusy = false;
						CommandManager.InvalidateRequerySuggested();
					},
					o =>
					{
						if (completed != null)
							completed(o);
					},
					o =>
					{
						IsBusy = true;
						if (preFunc != null)
							return preFunc(o);
						return true;
					}
				);
		}

		/// <summary>
		/// 非同步執行
		/// </summary>
		/// <param name="asyncAction"></param>
		/// <param name="complete"></param>
		/// <param name="failed"></param>
		public void AsyncAction(Action asyncAction, Action complete = null, Action<Exception> failed = null)
		{
			Exception = null;
			Wms3plFunction.AsyncAction(
				asyncAction: () =>
											 {
												 Exception = null;
												 IsBusy = true; asyncAction();
											 },
				completed: () =>
				{
					if (complete != null) complete();
					IsBusy = false;
				},
				failed: e =>
				{
					ErrorHandleHelper.HandleException(e);
					if (failed != null) failed(e);
					Exception = e;
					IsBusy = false;
				});
		}

		/// <summary>
		/// 非同步執行
		/// </summary>
		/// <param name="asyncAction"></param>
		/// <param name="cancellationToken"> </param>
		/// <param name="complete"></param>
		/// <param name="failed"></param>
		public Task AsyncAction(Action asyncAction, CancellationToken cancellationToken,
			Action complete = null, Action<Exception> failed = null)
		{
			Exception = null;
			return Wms3plFunction.AsyncAction(
				asyncAction: () =>
				{
					Exception = null;
					IsBusy = true;
					cancellationToken.ThrowIfCancellationRequested();
					asyncAction();
				},
				cancellationToken: cancellationToken,
				completed: () =>
				{
					if (complete != null) complete();
					IsBusy = false;
				},
				failed: e =>
				{
					ErrorHandleHelper.HandleException(e);
					if (failed != null) failed(e);
					Exception = e;
					IsBusy = false;
				},
				cancelAction: () => { IsBusy = false; });
		}

		/// <summary>
		/// 非同步執行
		/// </summary>
		/// <param name="action"></param>
		/// <param name="complete"></param>
		/// <param name="failed"></param>
		public void SmartAction(Action action, Action complete = null, Action<Exception> failed = null)
		{
			Exception = null;
			try
			{
				action();
			}
			catch (Exception e)
			{
				ErrorHandleHelper.HandleException(e);
				Exception = e;
			}
		}

		public void DoActionNotShowException(Action action)
		{
			try
			{
				action();
			}
			catch (Exception e)
			{
				ErrorHandleHelper.HandleException(e);
			}
		}

		private void SendCompleteMessage()
		{
			Messenger.Default.Send<NotificationMessage<bool>>(new NotificationMessage<bool>(this, true, "InQuery"), "InQuery");
		}

		private void SendBusyMessage()
		{
			Messenger.Default.Send<NotificationMessage<bool>>(new NotificationMessage<bool>(this, false, "InQuery"), "InQuery");
		}


		#region 訊息定義
		/// <summary>
		/// 跳出提示訊息. 傳入MessageStruct.
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		public DialogResponse ShowMessage(MessagesStruct msg)
		{
			return DialogService.ShowMessage(msg.Message, msg.Title, msg.Button, msg.Image);
		}
		/// <summary>
		/// 跳出提示訊息.
		/// 從DataService接到IEnumable ExecuteResult時, 直接傳入此function, 會取第一個ExecureResult顯示
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public DialogResponse ShowMessage(dynamic result)
		{
			var msg = Messages.Success;
			if (result != null && result.Count != 0)
			{
				if (result[0].IsSuccessed == true)
				{
					msg = Messages.Success;
				}
				else
				{
					msg = Messages.Failed;
					msg.Message = msg.Message + Environment.NewLine + result[0].Message;
				}
			}
			else
			{
				msg = Messages.Failed;
			}
			return ShowMessage(msg);
		}
		public DialogResponse ShowWarningMessage(string message)
		{
			var msg = new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Warning,
				Message = message,
				Title = "警告"
			};
			return DialogService.ShowMessage(msg.Message, msg.Title, msg.Button, msg.Image);
		}

		public DialogResponse ShowInfoMessage(string message)
		{
			var msg = new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Information,
				Message = message,
				Title = "訊息"
			};
			return DialogService.ShowMessage(msg.Message, msg.Title, msg.Button, msg.Image);
		}

		public DialogResponse ShowConfirmMessage(string message)
		{
			var msg = new MessagesStruct
			{
				Button = DialogButton.YesNo,
				Image = DialogImage.Question,
				Message = message,
				Title = "訊息"
			};
			return DialogService.ShowMessage(msg.Message, msg.Title, msg.Button, msg.Image, MessageBoxResult.No);
		}

		public DialogResponse ShowConfirmMessage(string message, string yesButtonText, string noButtonText, string cancelButtonText)
		{
			return DialogService.ShowMessage(message, "訊息", yesButtonText, noButtonText, cancelButtonText, DialogImage.Question);
		}

		/// <summary>
		/// 可依照操作結果是否成功，顯示提示或警告的訊息視窗
		/// </summary>
		/// <param name="isSucceeded"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public DialogResponse ShowResultMessage(bool isSucceeded, string message)
		{
			if (isSucceeded)
			{
				if (string.IsNullOrWhiteSpace(message))
				{
					return ShowMessage(Messages.Success);
				}

				return ShowMessage(new MessagesStruct()
				{
					Button = DialogButton.OK,
					Image = DialogImage.Information,
					Message = message,
					Title = "訊息"
				});
			}
			else
			{
				var failedMessage = Messages.Failed;
				if (!string.IsNullOrWhiteSpace(message))
					failedMessage.Message += Environment.NewLine + message;
				return ShowMessage(failedMessage);
			}
		}
		/// <summary>
		/// 直接丟 ExecuteResult 進來就會依照操作結果是否成功，顯示提示或警告的訊息視窗
		/// </summary>
		/// <typeparam name="TExecuteResult"></typeparam>
		/// <param name="executeResult"></param>
		/// <returns></returns>
		public DialogResponse ShowResultMessage<TExecuteResult>(TExecuteResult executeResult)
				where TExecuteResult : new()    // 不要懷疑，直接傳 ExecuteResult 給我即可，不要再給我 message string 拉~
		{
			var isSuccessedProp = typeof(TExecuteResult).GetProperty("IsSuccessed");
			if (isSuccessedProp != null)
			{
				bool isSuccessed = Convert.ToBoolean(isSuccessedProp.GetValue(executeResult));
				var message = Convert.ToString(typeof(TExecuteResult).GetProperty("Message").GetValue(executeResult));
				return ShowResultMessage(isSuccessed, message);
			}

			isSuccessedProp = typeof(TExecuteResult).GetProperty("IsSuccessedk__BackingField");
			if (isSuccessedProp != null)
			{
				bool isSuccessed = Convert.ToBoolean(isSuccessedProp.GetValue(executeResult));
				var message = Convert.ToString(typeof(TExecuteResult).GetProperty("Messagek__BackingField").GetValue(executeResult));
				return ShowResultMessage(isSuccessed, message);
			}

			throw new ArgumentException("executeResult 參數不是有效的回傳 ExecuteResult 結果類型");
		}
		#endregion

		public T GetProxy<T>() where T : DataServiceContext
		{			
			return ConfigurationHelper.GetProxy<T>(IsSecretePersonalData, FunctionCode);
		}
		public T GetProxyLongTermSchema<T>() where T : DataServiceContext
		{
			return ConfigurationHelper.GetProxyLongTermSchema<T>(IsSecretePersonalData, FunctionCode);
		}
		public T GetModifyQueryProxy<T>() where T : DataServiceContext
		{
			return ConfigurationHelper.GetProxy<T>(false, FunctionCode);
		}

		public void SetGlobalInfoFunctionCode()
		{
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			if (globalInfo != null)
			{
				globalInfo.IsSecretePersonalData = IsSecretePersonalData;
				globalInfo.FunctionCode = FunctionCode;
			}
		}

		public T GetExProxy<T>() where T : DataServiceContext
		{
			return ConfigurationExHelper.GetExProxy<T>(IsSecretePersonalData, FunctionCode);
		}

		/// <summary>
		/// 依照傳入的 keyParamterObject 與要取得的 TEntity Key 組為查詢條件，並回傳第一筆符合的項目。
		/// </summary>
		/// <typeparam name="TEntity">要回傳的 Entity 類型</typeparam>
		/// <param name="keyParamterObject">相同Key屬性的參數物件</param>
		/// <returns></returns>
		public TEntity FindByKey<TEntity>(object keyParamterObject) where TEntity : System.ComponentModel.INotifyPropertyChanged
		{
			return ConfigurationHelper.FindByKey<TEntity>(keyParamterObject, IsSecretePersonalData, FunctionCode);
		}

		public T RunWcfMethod<T>(System.ServiceModel.IClientChannel clientChannel, Func<T> doMethod)
		{
			return WcfServiceHelper.Execute<T>(clientChannel, doMethod, IsSecretePersonalData, FunctionCode);
		}
		public void RunWcfMethod(System.ServiceModel.IClientChannel clientChannel, Action doMethod)
		{
			WcfServiceHelper.Execute(clientChannel, doMethod, IsSecretePersonalData, FunctionCode);
		}

		public WcfProxy<TWcfServiceClient> GetWcfProxy<TWcfServiceClient>() where TWcfServiceClient : System.ServiceModel.ICommunicationObject
		{
			return new WcfProxy<TWcfServiceClient>(IsSecretePersonalData, FunctionCode);
		}

		public void DispatcherAction(Action action)
		{
			Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, action);
		}

		public void DispatcherThreadStartAction(Action action)
		{
			Application.Current.Dispatcher.Invoke((ThreadStart)delegate { action(); });
		}

		#region Common
		/// <summary>
		/// 查詢字串格式化
		/// </summary>
		protected virtual string QueryFormat(string text)
		{
			return string.Format(@"'{0}'", text ?? string.Empty);
		}
		#endregion

		private DataTable DataGridTable { get; set; }
		public virtual ICommand DataGridExportExcelCommand
		{
			get
			{
				return CreateBusyAsyncCommand<MenuItem>(
					o => DataGridExportExcel(),
					o => true,
					null,
					o => PreDataGridExportExcel(o)
					);
			}
		}

		private string exportDataGridFileName;
		private bool PreDataGridExportExcel(MenuItem sender)
		{
			var dataGrid = (DataGrid)((ContextMenu)sender.Parent).PlacementTarget;

			var saveFileDialog = new SaveFileDialog
			{
				DefaultExt = ".xls",
				Filter = "excel files |*.xls"
			};
			if (!saveFileDialog.ShowDialog().Value)
				return false;
			exportDataGridFileName = saveFileDialog.FileName;

			ScrollViewer scrollview = dataGrid.FindChildrenByType<ScrollViewer>().FirstOrDefault();
			var scrollIndex = (int)scrollview.ContentVerticalOffset;
			DataGridTable = DataGridHelper.ConvertDataGridToDataTable(dataGrid);
			if (dataGrid.Items.Count > 0) dataGrid.ScrollIntoView(dataGrid.Items[scrollIndex]);

			return true;
		}

		private void DataGridExportExcel()
		{
			DataGridTable.RenderDataTableToExcel(exportDataGridFileName);
			DataGridTable = null;
			ShowWarningMessage("Excel匯出完成");
		}
		public virtual ICommand DataGridCopyCellCommand
		{
			get
			{
				return new RelayCommand<MenuItem>(
					o => CopyDataCellContent(o),
					o => true
					);
			}
		}

		private void CopyDataCellContent(MenuItem sender)
		{
			var dataGrid = (DataGrid)((ContextMenu)sender.Parent).PlacementTarget;
			if (dataGrid.CurrentCell == null || dataGrid.CurrentCell.Column == null)
				return;
			var data = dataGrid.CurrentCell.Item;
			var cellContent = dataGrid.CurrentCell.Column.GetCellContent(data);
			if (cellContent == null)
			{
				var item = dataGrid.Items[dataGrid.SelectedIndex - 1];
				Type type = item.GetType();
				var pi = type.GetProperty(dataGrid.CurrentCell.Column.SortMemberPath);
				object priorValue = pi.GetValue(item);
				Clipboard.SetText(priorValue.ToString());
			}
			else if (cellContent is TextBlock)
				Clipboard.SetText(((TextBlock)cellContent).Text);
      else if(cellContent is TextBox)
        Clipboard.SetText(((TextBox)cellContent).Text);
			else
			{
				var textBlock = DataGridHelper.GetVisualChild<TextBlock>((Visual)cellContent);
				Clipboard.SetText(textBlock.Text);
			}
		}

	}
}
