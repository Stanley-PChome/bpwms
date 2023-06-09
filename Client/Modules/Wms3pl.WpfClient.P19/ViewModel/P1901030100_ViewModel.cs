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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1901030100_ViewModel : InputViewModelBase
	{
		public Action DoExit = delegate { };
		private string _gupCode;
		private string _custCode;
		public P1901030100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}

		}

		private void InitControls()
		{
			_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
		}

		#region 查詢商品
		/// <summary>
		/// 查詢之商品編號
		/// </summary>
		private string _searchItemCode;
		public string SearchItemCode { get { return _searchItemCode; } set { _searchItemCode = value; RaisePropertyChanged("SearchItemCode"); } }

		private string _searchItemName;
		public string SearchItemName { get { return _searchItemName; } set { _searchItemName = value; RaisePropertyChanged("SearchItemName"); } }

		#endregion

		#region 商品項目材積清單
		/// <summary>
		/// 商品項目材積清單
		/// </summary>
		private List<F1905Data> _itemList;
		public List<F1905Data> ItemList { get { return _itemList; } set { _itemList = value; RaisePropertyChanged("ItemList"); } }
		private F1905Data _selectItem;
		public F1905Data SelectItem
		{
			get { return _selectItem; }
			set
			{
				_selectItem = value;
				RaisePropertyChanged("SelectItem");
				DoExit();
			}
		}
		#endregion

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
			//執行查詢動
			var proxyEx = GetExProxy<P19ExDataSource>();
			ItemList = proxyEx.CreateQuery<F1905Data>("GetPackCase")
				.AddQueryOption("gupCode", string.Format("'{0}'", _gupCode))
				.AddQueryOption("custCode", string.Format("'{0}'", _custCode))
				.AddQueryOption("itemCode", string.Format("'{0}'", SearchItemCode))
				.AddQueryOption("itemName", string.Format("'{0}'", SearchItemName))
				.ToList();
			if (ItemList == null || !ItemList.Any())
				ShowMessage(Messages.InfoNoData);
		}
		#endregion Search

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoAdd()
		{
			UserOperateMode = OperateMode.Add;
			//執行新增動作
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			//執行編輯動作
		}
		#endregion Edit

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

			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作

			UserOperateMode = OperateMode.Query;
		}
		#endregion Save
	}
}
