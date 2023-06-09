using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices;
using System.Reflection;
using Wms3pl.WpfClient.P02.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.DataServices.F01DataService;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F25DataService;

//using Wms3pl.Datas.Shared.Entities;
using System.Data;
using Wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0202030400_ViewModel : InputViewModelBase
	{
		public Action OnCancelComplete = delegate { };
		private string _userId = Wms3plSession.Get<UserInfo>().Account;
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }

		public P0202030400_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}
		}

		private void InitControls()
		{
			UserOperateMode = OperateMode.Edit;
			DoSearchUccList();
		}

		#region 資料連結/ 頁面參數
		private void PageRaisePropertyChanged()
		{
		}
		#region Form - 可用的DC (物流中心)清單
		private string _selectedDc = string.Empty;
		/// <summary>
		/// 選取的物流中心
		/// </summary>
		public string SelectedDc
		{
			get { return _selectedDc; }
			set { _selectedDc = value; }
		}
		#endregion

		#region 進倉單號
		private string _purchaseNo;

		public string PurchaseNo
		{
			get { return _purchaseNo; }
			set
			{
				Set(() => PurchaseNo, ref _purchaseNo, value);
			}
		}
		#endregion

		#region Data - 基本資料
		private List<P020203Data> _baseData;
		public List<P020203Data> BaseData
		{
			get { return _baseData; }
			set { _baseData = value; RaisePropertyChanged("BaseData"); }
		}
		#endregion
		#region Data - 特殊採購原因

		private Dictionary<string, List<NameValuePair<string>>> _uccDict;

		public Dictionary<string, List<NameValuePair<string>>> UccDict
		{
			get { return _uccDict; }
			set
			{
				Set(() => UccDict, ref _uccDict, value);
			}
		}

		#endregion
		#endregion

		#region Command
		#region Search
		private void DoSearchUccList()
		{
			// 採用過濾 0 跟 1 的欄位是 是否檢驗 CHECK_ITEM，故沒檢驗時，只能選其他，沒序號時，只能選先進貨入帳(前面 canexecute 已經擋掉其他可能)
			var proxy = GetProxy<F19Entities>();
			var list = proxy.F1951s
							.Where(x => x.UCT_ID.Equals("CK"))
							.Select(x => new NameValuePair<string>(x.CAUSE, x.UCC_CODE))
							.ToList();
			var uccDict = new Dictionary<string, List<NameValuePair<string>>>();
			uccDict.Add("0", list.Where(x => x.Value == "999").ToList());	// 其他
			uccDict.Add("1", list.Where(x => x.Value == "201").ToList());	// 先進貨入帳
			UccDict = uccDict;
		}
		#endregion
		#region Cancel
		private bool _isOkToCancel = false;
		public ICommand ExitCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => _isOkToCancel = DoExit(),
					() =>
					{
						return UserOperateMode == OperateMode.Edit;
					},
					o =>
					{
						if (_isOkToCancel) OnCancelComplete();
						_isOkToCancel = false;
					}
				);
			}
		}
		public bool DoExit()
		{
			if (ShowMessage(Messages.WarningBeforeExit) == DialogResponse.OK)
			{
				UserOperateMode = OperateMode.Query;
				return true;
			}
			return false;
		}

		#endregion
		#region Save

		private bool _isSaveOk;
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => _isSaveOk = DoSave(),
					() =>
					{
						return UserOperateMode == OperateMode.Edit && BaseData != null;
					},
					c =>
					{
						if (_isSaveOk)
							OnCancelComplete();
					}
				);
			}
		}

		/// <summary>
		/// 更新資料
		/// ToDo: 先入帳進貨時, 此商品系統自動調撥至加工區做序號收集的串接
		/// </summary>
		/// <returns></returns>
		public bool DoSave()
		{
			var proxy = new Wcf.P02WcfServiceClient();
			var result = RunWcfMethod<Wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.UpdateP02020304(SelectedDc, this._gupCode, this._custCode, BaseData.Select(AutoMapper.Mapper.DynamicMap<Wcf.P020203Data>).ToArray(), this._userId));
			ShowMessage(new List<ExecuteResult>() { new ExecuteResult() { IsSuccessed = result.IsSuccessed, Message = result.Message } });
			return true;
		}
		#endregion Save

		//
		private RelayCommand<P020203Data> _specialCommand;

		/// <summary>
		/// Gets the SpecialCommand.
		/// </summary>
		public RelayCommand<P020203Data> SpecialCommand
		{
			get
			{
				return _specialCommand ?? (_specialCommand = new RelayCommand<P020203Data>(
					ExecuteSpecialCommand,
					CanExecuteSpecialCommand));
			}
		}

		private void ExecuteSpecialCommand(P020203Data parameter)
		{
			if (!SpecialCommand.CanExecute(parameter))
			{
				return;
			}
		}

		private PurchaseService _purchaseService = null;

		private bool CanExecuteSpecialCommand(P020203Data parameter)
		{
			return (_purchaseService ?? (_purchaseService = new PurchaseService())).CanExecuteSpecial(parameter);
		}

		private RelayCommand _disableCommand;

		/// <summary>
		/// Gets the DisableCommand.
		/// </summary>
		public RelayCommand DisableCommand
		{
			get
			{
				return _disableCommand
					?? (_disableCommand = new RelayCommand(
					() =>
					{
						if (!DisableCommand.CanExecute(null))
						{
							return;
						}


					},
					() => false));
			}
		}
		#endregion
	}


	public class CanExecuteSpecialConverter : System.Windows.Data.IValueConverter
	{
		private PurchaseService _purchaseService = null;

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var selectedData = value as P020203Data;
			if (selectedData == null)
			{
				return false;
			}

			return (_purchaseService ?? (_purchaseService = new PurchaseService())).CanExecuteSpecial(selectedData);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

}
