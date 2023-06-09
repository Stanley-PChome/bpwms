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
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7109010100_ViewModel : InputViewModelBase
	{
		public Action DoExit = delegate { };

		public P7109010100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcList();
			}

		}

		#region 查詢條件

		private string _searchDcCode;

		public string SearchDcCode
		{
			get { return _searchDcCode; }
			set
			{
				_searchDcCode = value;
				//SetGupList(value);
				RaisePropertyChanged("SearchDcCode");
			}
		}

        //private string _searchGupCode;
        //public string SearchGupCode
        //{
        //    get { return _searchGupCode; }
        //    set
        //    {
        //        _searchGupCode = value;
        //        SetCustList(value);
        //        RaisePropertyChanged("SearchGupCode");
        //    }
        //}

        //private string _searchCustCode;
        //public string SearchCustCode
        //{
        //    get { return _searchCustCode; }
        //    set
        //    {
        //        _searchCustCode = value;
        //        RaisePropertyChanged("SearchCustCode");
        //    }
        //}

		private string _searchAllID;
		public string SearchAllID
		{
			get { return _searchAllID; }
			set
			{
				_searchAllID = value;
				RaisePropertyChanged("SearchAllID");
			}
		}

		private string _searchAllComp;
		public string SearchAllComp
		{
			get { return _searchAllComp; }
			set
			{
				_searchAllComp = value;
				RaisePropertyChanged("SearchAllComp");
			}
		}

		#endregion

		#region 物流中心 業主 貨主清單
		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				_dcList = value;
				RaisePropertyChanged("DcList");
			}
		}

		private List<NameValuePair<string>> _gupList;
		public List<NameValuePair<string>> GupList
		{
			get { return _gupList; }
			set
			{
				_gupList = value;
				RaisePropertyChanged("GupList");
			}
		}

		private List<NameValuePair<string>> _custList;
		public List<NameValuePair<string>> CustList
		{
			get { return _custList; }
			set
			{
				_custList = value;
				RaisePropertyChanged("CustList");
			}
		}

		#endregion

		#region 碼頭清單
		private List<NameValuePair<string>> _pierList;
		public List<NameValuePair<string>> PierList
		{
			get { return _pierList; }
			set
			{
				_pierList = value;
				RaisePropertyChanged("PierList");
			}
		}
		private string _selectedPierID = string.Empty;
		public string SelectedPierID
		{
			get { return _selectedPierID; }
			set
			{
				_selectedPierID = value;
				RaisePropertyChanged("SelectedPier");
			}
		}
		#endregion

		#region 下拉式選單資料來源

		#region 物流中心 業主 貨主
		/// <summary>
		/// 設定DC清單
		/// </summary>
		public void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
			if (data != null && data.Any())
			{
				// 查詢的時候，才有預設帶物流中心，新增畫面則不預設
				SearchDcCode = data.First().Value;
			}
		}

        ///// <summary>
        ///// 設定業主清單
        ///// </summary>
        //public void SetGupList(string dcCode)
        //{
        //    var gupList = Wms3plSession.Get<GlobalInfo>().GetGupDataList(dcCode);
        //    gupList.Insert(0, new NameValuePair<string> { Name = Properties.Resources.P7105010000_ViewModel_NONE_SPECIFY, Value = "0" });
        //    gupList.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "-1" });
        //    GupList = gupList;

        //    if (gupList.Any())
        //    {
        //        SearchGupCode = gupList.First().Value;
        //    }
        //}

        ///// <summary>
        ///// 設定貨主清單
        ///// </summary>
        //public void SetCustList(string gupCode)
        //{
        //    var custList = Wms3plSession.Get<GlobalInfo>().GetCustDataList(SearchDcCode, gupCode);

        //    custList.Insert(0, new NameValuePair<string> { Name = Properties.Resources.P7105010000_ViewModel_NONE_SPECIFY, Value = "0" });
        //    custList.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "-1" });
        //    CustList = custList;

        //    if (custList.Any())
        //    {
        //        SearchCustCode = custList.First().Value;
        //    }
        //}

		#endregion
		#endregion

		#region DataGrid 資料來源與屬性
		/// <summary>
		/// 配送商清單
		/// </summary>
		private List<F1947Ex> _itemList;
		public List<F1947Ex> ItemList
		{
			get { return _itemList; }
			set
			{
				_itemList = value;
				RaisePropertyChanged("ItemList");
			}
		}


		private F1947Ex _selectedItem;

		public F1947Ex SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				RaisePropertyChanged("SelectedItem");
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
            //var proxyF19 = ConfigurationHelper.GetProxy<F19Entities>(false);
            //var query = proxyF19.F1947s.AsQueryable();
            //query = query.Where(item => item.DC_CODE == SearchDcCode);

            //if (SearchGupCode != "-1")
            //{
            //	query = query.Where(item => item.GUP_CODE == SearchGupCode);
            //}

            //if (SearchCustCode != "-1")
            //{
            //	query = query.Where(item => item.CUST_CODE == SearchCustCode);
            //}

            //if (!string.IsNullOrWhiteSpace(SearchAllID))
            //{
            //	query = query.Where(item => item.ALL_ID == SearchAllID);
            //}

            //if (!string.IsNullOrWhiteSpace(SearchAllComp))
            //{
            //	query = query.Where(item => item.ALL_COMP.Contains(SearchAllComp));
            //}
            var globalInfo = Wms3plSession.Get<GlobalInfo>();
            var gupCode = globalInfo.GupCode;
            var custCode = globalInfo.CustCode;
            var proxy = GetExProxy<P71ExDataSource>();
            var query = proxy.CreateQuery<F1947Ex>("GetF1947ExQuery")
                .AddQueryExOption("dcCode", SearchDcCode)
                .AddQueryExOption("gupCode", gupCode)
                .AddQueryExOption("custCode", custCode)
                .AddQueryExOption("allID", SearchAllID)
                .AddQueryExOption("allComp", SearchAllComp);

            ItemList = query.ToList();

            if (!ItemList.Any())
            {
                ShowMessage(Messages.InfoNoData);
            }
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
