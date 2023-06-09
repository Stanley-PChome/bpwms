using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7101020000_ViewModel : InputViewModelBase
	{
		public P7101020000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcList();
			}

		}

		#region Property
		public Action OpenAddClick = delegate { };
		public Action OpenEditClick = delegate { };


		private UseModelType _displayUseModelType;
		public UseModelType DisplayUseModelType
		{
			get { return _displayUseModelType; }
			set
			{
				_displayUseModelType = value;
				ChangeUseModeTypeDisplay();
			}
		}

		private Visibility _visibilityGupAndCust = Visibility.Hidden;
		public Visibility VisibilityGupAndCust
		{
			get { return _visibilityGupAndCust; }
			set
			{
				_visibilityGupAndCust = value;
				RaisePropertyChanged("VisibilityGupAndCust");
			}
		}

		#region 物流中心 業主 雇主

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

		private string _selectedDcCode = "";
		public string SelectedDcCode
		{
			get { return _selectedDcCode; }
			set
			{
				_selectedDcCode = value;
				RaisePropertyChanged("SelectedDcCode");
				SetGupList();
				F1919Datas = null;
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

		private string _selectedGupCode = "";
		public string SelectedGupCode
		{
			get { return _selectedGupCode; }
			set
			{
				_selectedGupCode = value;
				RaisePropertyChanged("SelectedGupCode");
				SetCustList();
				F1919Datas = null;
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

		private string _selectedCustCode = "";
		public string SelectedCustCode
		{
			get { return _selectedCustCode; }
			set
			{
				_selectedCustCode = value;
				RaisePropertyChanged("SelectedCustCode");
				SetWarehouseList();
				F1919Datas = null;
			}
		}
		#endregion

		#region 倉別屬性

		private List<NameValuePair<string>> _warehouseList;
		public List<NameValuePair<string>> WarehouseList
		{
			get { return _warehouseList; }
			set
			{
				_warehouseList = value;
				RaisePropertyChanged("WarehouseList");
			}
		}

		private string _selectedwarehouseId = "";
		public string SelectedwarehouseId
		{
			get { return _selectedwarehouseId; }
			set
			{
				_selectedwarehouseId = value;
				RaisePropertyChanged("SelectedwarehouseId");
				F1919Datas = null;
			}
		}
		#endregion

		#region Grid 
		private List<F1919Data> _f1919Datas;
		public List<F1919Data> F1919Datas
		{
			get { return _f1919Datas; }
			set
			{
				_f1919Datas = value;
				RaisePropertyChanged("F1919Datas");
			}
		}

		private F1919Data _selectedF1919Data;
		public F1919Data SelectedF1919Data
		{
			get { return _selectedF1919Data; }
			set
			{
				_selectedF1919Data = value;
				RaisePropertyChanged("SelectedF1919Data");
			}
		}

		#endregion

		
		#endregion

		#region 下拉式選單資料來源

		#region 物流中心 業主 貨主
		/// <summary>
		/// 設定DC清單
		/// </summary>
		private void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
			if (DcList.Any())
				SelectedDcCode = DcList.First().Value;
		}

		/// <summary>
		/// 設定業主清單
		/// </summary>
		private void SetGupList()
		{
			var gupList = Wms3plSession.Get<GlobalInfo>().GetGupDataList(_selectedDcCode);
			gupList.Insert(0, new NameValuePair<string>() { Name = Properties.Resources.P7101010000_ViewModel_share, Value = "0" });
			gupList.Insert(0, new NameValuePair<string>() { Name = Resources.Resources.All, Value = "" });
			GupList = gupList;
			SelectedGupCode = gupList.First().Value;
		}

		/// <summary>
		/// 設定貨主清單
		/// </summary>
		private void SetCustList()
		{
			var custList = new List<NameValuePair<string>>();
			if (_selectedGupCode != "0")
			{
				custList = Wms3plSession.Get<GlobalInfo>().GetCustDataList(_selectedDcCode, _selectedGupCode);
				custList.Insert(0, new NameValuePair<string>() { Name = Properties.Resources.P7101010000_ViewModel_share, Value = "0" });
				custList.Insert(0, new NameValuePair<string>() { Name = Resources.Resources.All, Value = "" });
			}
			else
				custList.Add(new NameValuePair<string>() { Name = Properties.Resources.P7101010000_ViewModel_share, Value = "0" });
			CustList = custList;
			SelectedCustCode = custList.First().Value;
		}
		#endregion

		#region 倉別屬性

		/// <summary>
		/// 設定倉別清單
		/// </summary>
		public void SetWarehouseList()
		{
			var proxyEx = GetExProxy<P71ExDataSource>();
			var data = proxyEx.CreateQuery<F1980Data>("GetF1980Datas")
				.AddQueryOption("dcCode", string.Format("'{0}'", _selectedDcCode))
				.AddQueryOption("gupCode", string.Format("'{0}'", _selectedGupCode))
				.AddQueryOption("custCode", string.Format("'{0}'", _selectedCustCode))
				.AddQueryOption("warehourseId", string.Format("'{0}'", ""))
				.AddQueryOption("account", string.Format("'{0}'", Wms3plSession.CurrentUserInfo.Account))
				.ToList();
			var warehouseList = (from o in
														 (from c in data
															select new { c.WAREHOUSE_ID, c.WAREHOUSE_Name }).Distinct()
													 orderby o.WAREHOUSE_ID
													 select new NameValuePair<string>()
													 {
														 Name = o.WAREHOUSE_Name,
														 Value = o.WAREHOUSE_ID
													 }).ToList();
			WarehouseList = warehouseList;
			if (warehouseList.Any())
				SelectedwarehouseId = warehouseList.First().Value;
		}

		#endregion

		#endregion

		#region ChangeUseMode

		private void ChangeUseModeTypeDisplay()
		{
			VisibilityGupAndCust = (_displayUseModelType == UseModelType.Headquarters) ? Visibility.Visible : Visibility.Hidden;
		}

		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query, c => DoSearchComplete()
					);
			}
		}

		private void DoSearch()
		{
			//執行查詢動
			var proxyEx = GetExProxy<P71ExDataSource>();
			F1919Datas = proxyEx.CreateQuery<F1919Data>("GetF1919Datas")
				.AddQueryOption("dcCode", string.Format("'{0}'", _selectedDcCode))
				.AddQueryOption("gupCode", string.Format("'{0}'", _selectedGupCode))
				.AddQueryOption("custCode", string.Format("'{0}'", _selectedCustCode))
				.AddQueryOption("warehourseId", string.Format("'{0}'", string.IsNullOrEmpty(_selectedwarehouseId) ? "-1" : _selectedwarehouseId))
				.ToList();
		}
		private void DoSearchComplete()
		{
			if (F1919Datas.Any())
				SelectedF1919Data = F1919Datas.First();
			else
				ShowMessage(Messages.InfoNoData);
		}
		#endregion Search

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return new RelayCommand(
					 DoAdd, () => UserOperateMode == OperateMode.Query && (SelectedwarehouseId!=null)
					);
			}
		}

		private void DoAdd()
		{
			UserOperateMode = OperateMode.Add;
			//執行新增動作
			OpenAddClick();

		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return new RelayCommand(
					 DoEdit, () => UserOperateMode == OperateMode.Query && (SelectedF1919Data!=null)
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			//執行編輯動作
			OpenEditClick();
		}
		#endregion Edit
	}
}
