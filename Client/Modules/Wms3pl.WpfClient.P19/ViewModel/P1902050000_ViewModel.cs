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
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.DataServices;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1902050000_ViewModel : InputViewModelBase
	{
		public Action<object> OnScrollIntoViewSelectedItem = (x) => { };

		public P1902050000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料

				TypeList = GetBaseTableService.GetF000904List(FunctionCode, "F1903", "TYPE");
				CheckItemTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F190206", "CHECK_TYPE");
				SearchType = TypeList.Select(x => x.Value).FirstOrDefault();
				if (!string.IsNullOrEmpty(SearchType))
					SearchData();
			}

		}

		private string _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
        private string _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;

        private string _searchType;

		public string SearchType
		{
			get { return _searchType; }
			set
			{
				Set(() => SearchType, ref _searchType, value);

				F190205List = new ObservableCollection<F190205>();
			}
		}

		private ObservableCollection<F190205> _f190205List;

		public ObservableCollection<F190205> F190205List
		{
			get { return _f190205List; }
			set
			{
				Set(() => F190205List, ref _f190205List, value);
			}
		}

		private F190205 _selectedF190205;

		public F190205 SelectedF190205
		{
			get { return _selectedF190205; }
			set
			{
				Set(() => SelectedF190205, ref _selectedF190205, value);
			}
		}

		private List<NameValuePair<string>> _typeList;

		public List<NameValuePair<string>> TypeList
		{
			get { return _typeList; }
			set
			{
				Set(() => TypeList, ref _typeList, value);
			}
		}


		private List<NameValuePair<string>> _checkItemTypeList;

		public List<NameValuePair<string>> CheckItemTypeList
		{
			get { return _checkItemTypeList; }
			set
			{
				Set(() => CheckItemTypeList, ref _checkItemTypeList, value);
			}
		}

		private List<NameValuePair<string>> _checkItemList;

		public List<NameValuePair<string>> CheckItemList
		{
			get { return _checkItemList; }
			set
			{
				Set(() => CheckItemList, ref _checkItemList, value);
			}
		}



		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoSearch()
		{
			//執行查詢動作

			SearchData();

			if (!F190205List.Any())
			{
				ShowMessage(Messages.InfoNoData);
			}
		}

		private void SearchData()
		{
			var proxy = GetProxy<F19Entities>();
			CheckItemList = proxy.F1983s.OrderBy(x => x.CHECK_NO)
										.Select(x => new NameValuePair<string>(x.CHECK_NAME, x.CHECK_NO))
										.ToList();

			F190205List = proxy.F190205s.Where(x => x.TYPE == SearchType && x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode)
										.OrderBy(x => x.CHECK_TYPE)
										.ToObservableCollection();
		}
		#endregion Search

		#region Add
		private RelayCommand _addCommand;

		/// <summary>
		/// Gets the AddCommand.
		/// </summary>
		public RelayCommand AddCommand
		{
			get
			{
				return _addCommand
					?? (_addCommand = new RelayCommand(
					DoAdd,
					() => UserOperateMode == OperateMode.Query && !string.IsNullOrEmpty(SearchType)));
			}
		}

		private void DoAdd()
		{
			if (!AddCommand.CanExecute(null))
			{
				return;
			}

			var f190205 = new F190205
			{
				TYPE = SearchType,
				GUP_CODE = _gupCode,
                CUST_CODE = _custCode
            };
			F190205List.Add(f190205);
			SelectedF190205 = f190205;
			OnScrollIntoViewSelectedItem(f190205);
			UserOperateMode = OperateMode.Add;
			//執行新增動作
		}
		#endregion Add

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
			if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.No)
				return;

			SearchData();
			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(),
					() => UserOperateMode == OperateMode.Query && SelectedF190205 != null
					);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
			if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.No)
				return;

			var proxy = GetProxy<F19Entities>();

			var f190205 = proxy.F190205s.FindByKey(SelectedF190205);
			if (f190205 == null)
			{
				ShowMessage(Messages.WarningBeenDeleted);
				return;
			}

			proxy.DeleteObject(f190205);
			proxy.SaveChanges();

			ShowMessage(Messages.DeleteSuccess);
			DoSearch();
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),
					() => UserOperateMode != OperateMode.Query
						&& SelectedF190205 != null
					);
			}
		}

		string GetEditableError()
		{
			if (string.IsNullOrEmpty(SelectedF190205.CHECK_NO))
				return Properties.Resources.P1902050000_InputCHECK_NO;

			if (string.IsNullOrEmpty(SelectedF190205.CHECK_TYPE))
				return Properties.Resources.P1902050000_Check_NoType_Required;

			return string.Empty;
		}

		private void DoSave()
		{
			//執行確認儲存動作

			var error = GetEditableError();
			if (!string.IsNullOrEmpty(error))
			{
				ShowWarningMessage(error);
				return;
			}

			var proxy = GetProxy<F19Entities>();

			var f190205 = proxy.F190205s.FindByKey(SelectedF190205);
			if (f190205 != null)
			{
				ShowMessage(Messages.WarningExist);
				return;
			}

			proxy.AddToF190205s(SelectedF190205);
			proxy.SaveChanges();

			ShowMessage(Messages.Success);

			DoSearch();
			UserOperateMode = OperateMode.Query;
		}
		#endregion Save
	}
}
