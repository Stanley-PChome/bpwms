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
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.ExDataServices;
using System.Collections.ObjectModel;
using Wms3pl.Common;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7109020000_ViewModel : InputViewModelBase
	{
		public Action AddMode = () => { };
		public Action EditMode = () => { };
		public Action QueryMode = () => { };

		#region DataGrid Columns Properties

		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				Set(() => DcList, ref _dcList, value);
			}
		}

		/// <summary>
		/// 設定DC清單
		/// </summary>
		public void SetDcList()
		{
			DcList = new List<NameValuePair<string>>(Wms3plSession.Get<GlobalInfo>().DcCodeList);
		}

		private string _searchDcCode;

		public string SearchDcCode
		{
			get { return _searchDcCode; }
			set
			{
				Set(() => SearchDcCode, ref _searchDcCode, value);

				if (value == null)
					return;
				SetGupList(value);

			}
		}

		private string _selectGupCode;
		public string SelectGupCode
		{
			get { return _selectGupCode; }
			set
			{
				Set(() => SelectGupCode, ref _selectGupCode, value);
				if (value == null)
					return;
				SetCustListCombox(SearchDcCode, value);
			}
		}

		private string _selectCustCode;

		public string SelectCustCode
		{
			get { return _selectCustCode; }
			set
			{
				Set(() => SelectCustCode, ref _selectCustCode, value);

				if (value == null)
					return;
				DcCodeSelectionChangedCommand.Execute(null);
			}
		}


		private List<NameValuePair<string>> _allList;

		public List<NameValuePair<string>> AllList
		{
			get { return _allList; }
			set
			{
				Set(() => AllList, ref _allList, value);
			}
		}

		private List<NameValuePair<string>> _consignFormat;

		public List<NameValuePair<string>> ConsignFormat
		{
			get { return _consignFormat; }
			set
			{
				Set(ref _consignFormat, value);
			}
		}

		private List<NameValuePair<string>> _printerType;

		public List<NameValuePair<string>> PrinterType
		{
			get { return _printerType; }
			set
			{
				Set(ref _printerType, value);
			}
		}

		private List<NameValuePair<string>> _getConsingNo;

		public List<NameValuePair<string>> GetConsingNo
		{
			get { return _getConsingNo; }
			set
			{
				Set(ref _getConsingNo, value);
			}
		}

		private List<NameValuePair<string>> _addBoxGetConsignNo;

		public List<NameValuePair<string>> AddBoxGetConsignNo
		{
			get { return _addBoxGetConsignNo; }
			set
			{
				Set(ref _addBoxGetConsignNo, value);
			}
		}

		private List<NameValuePair<string>> _printConsign;

		public List<NameValuePair<string>> PrintConsign
		{
			get { return _printConsign; }
			set
			{
				Set(ref _printConsign, value);
			}
		}

		private List<NameValuePair<string>> _autoPrintConsign;

		public List<NameValuePair<string>> AutoPrintConsign
		{
			get { return _autoPrintConsign; }
			set
			{
				Set(ref _autoPrintConsign, value);
			}
		}

		public void SetAllList(string dcCode)
		{
			var proxy = GetProxy<F19Entities>();
			var query = from item in proxy.F1947s
						where item.DC_CODE == dcCode
						orderby item.ALL_ID
						select new NameValuePair<string>(item.ALL_COMP, item.ALL_ID);

			AllList = query.ToList();
		}

		public void SetConsignFormatList()
		{
			ConsignFormat = GetBaseTableService.GetF000904List(FunctionCode, "F194704", "CONSIGN_FORMAT");
		}
		/// <summary>
		/// 取得取號方式清單
		/// </summary>
		public void SetConsignNo()
		{
			GetConsingNo = GetBaseTableService.GetF000904List(FunctionCode, "F194704", "GET_CONSIGN_NO");
		}
		/// <summary>
		/// 取得加箱取號方式清單
		/// </summary>
		public void SetAddBoxConsignNo()
		{
			AddBoxGetConsignNo = GetBaseTableService.GetF000904List(FunctionCode,"F194704", "ADDBOX_GET_CONSIGN_NO");
		}


		public void SetYesOrNoList()
		{
			NameValuePair<string> yes = new NameValuePair<string>(Resources.Resources.Yes, "1");
			NameValuePair<string> no = new NameValuePair<string>(Resources.Resources.No, "0");
			PrintConsign = new List<NameValuePair<string>>();
			PrintConsign.Add(yes);
			PrintConsign.Add(no);
			AutoPrintConsign = new List<NameValuePair<string>>();
			AutoPrintConsign.Add(yes);
			AutoPrintConsign.Add(no);
		}


		public void SetPrinterTypeList()
		{
			PrinterType = GetBaseTableService.GetF000904List(FunctionCode, "F194704", "PRINTER_TYPE");
		}

		private List<NameValuePair<string>> _gupList;

		public List<NameValuePair<string>> GupList
		{
			get { return _gupList; }
			set
			{
				Set(() => GupList, ref _gupList, value);
			}
		}

		private List<NameValuePair<string>> _custList;

		public List<NameValuePair<string>> CustList
		{
			get { return _custList; }
			set
			{
				Set(() => CustList, ref _custList, value);
			}
		}

		/// <summary>
		/// 設定Gup清單
		/// </summary>	

		public void SetGupList(string dcCode)
		{
			GupList = Wms3plSession.Get<GlobalInfo>().GetGupDataList(dcCode);
		}

		public void SetCustListCombox(string dcCode, string gupCode)
		{
			if (gupCode == null)
				return;
			CustList = Wms3plSession.Get<GlobalInfo>().GetCustDataList(dcCode, gupCode);
		}


		/// <summary>
		/// 設定Cust清單
		/// </summary>
		public void SetCustList(string dcCode, string gupCode)
		{
			if (string.IsNullOrEmpty(dcCode) || string.IsNullOrEmpty(gupCode))
				return;

			if (SelectedF194704Data != null && SelectedF194704Data.Item.GUP_CODE != gupCode)
			{
				SelectedF194704Data.ItemSource = Wms3plSession.Get<GlobalInfo>().GetCustDataList(dcCode, gupCode);
			}
		}
		#endregion


		private SelectionNameValuePiarList<F194704Data, string> _f194704DataList
			= new SelectionNameValuePiarList<F194704Data, string>();

		public SelectionNameValuePiarList<F194704Data, string> F194704DataList
		{
			get { return _f194704DataList; }
			set
			{
				Set(() => F194704DataList, ref _f194704DataList, value);
			}
		}

		private SelectionItem<F194704Data, List<NameValuePair<string>>> _selectedF194704Data;

		public SelectionItem<F194704Data, List<NameValuePair<string>>> SelectedF194704Data
		{
			get { return _selectedF194704Data; }
			set
			{
				Set(() => SelectedF194704Data, ref _selectedF194704Data, value);
			}
		}


		public P7109020000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcList();
				SearchDcCode = DcList.Select(item => item.Value).FirstOrDefault();
				//UpdateAllItemSource();
			}

		}

		SelectionNameValuePiarList<F194704Data, string> GetF194704Datas(string dcCode, string gupCode, string custCode)
		{
			var proxy = GetExProxy<P71ExDataSource>();
			var list = proxy.CreateQuery<F194704Data>("GetF194704Datas")
									.AddQueryExOption("dcCode", SearchDcCode)
									.AddQueryExOption("gupCode", gupCode)
									.AddQueryExOption("custCode", custCode)
									.ToList();

			var f194704DataSelectionList = new SelectionNameValuePiarList<F194704Data, string>(list, x => x.CUST_NAME, x => x.CUST_CODE);
			return f194704DataSelectionList;
		}

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return new RelayCommand(
					() => DoSearch(),
					() => UserOperateMode == OperateMode.Query && !string.IsNullOrEmpty(SearchDcCode)
					);
			}
		}

		void DoSearch()
		{
			if (!string.IsNullOrEmpty(SearchDcCode))
			{
				F194704DataList = GetF194704Datas(SearchDcCode, SelectGupCode, SelectCustCode);

				if (!F194704DataList.Any())
				{
					ShowMessage(Messages.InfoNoData);
				}
				SelectedF194704Data = F194704DataList.FirstOrDefault();
				AllSelectF194704DataListCheckBox = false;
			}
		}

		public void SetUserOperateMode(OperateMode operateMode)
		{
			UserOperateMode = operateMode;

			if (operateMode == OperateMode.Query)
			{
				QueryMode();
			}
			else if (operateMode == OperateMode.Add)
			{
				AddMode();
			}
			else if (operateMode == OperateMode.Edit)
			{
				EditMode();
			}
		}
		#endregion Search

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return new RelayCommand(
					() => DoAdd(),
					() => UserOperateMode == OperateMode.Query && !string.IsNullOrEmpty(SearchDcCode)
					);
			}
		}

		private void DoAdd()
		{
			var proxy = GetProxy<F19Entities>();
			var dcName = proxy.F1901s.Where(o => o.DC_CODE == SearchDcCode).First();
			//執行新增動作
			var newItem = new SelectionItem<F194704Data, List<NameValuePair<string>>>(new F194704Data
			{
				DC_CODE = SearchDcCode,
				DC_NAME = dcName != null ? dcName.DC_NAME : "",
				GUP_CODE = SelectGupCode,
				CUST_CODE = SelectCustCode,
				CRT_STAFF = Wms3plSession.CurrentUserInfo.Account,
				CRT_DATE = DateTime.Now
			});
			F194704DataList.Add(newItem);
			SelectedF194704Data = newItem;
			SetUserOperateMode(OperateMode.Add);
		}
		#endregion Add

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				bool isCancel = false;
				return CreateBusyAsyncCommand(
					o => isCancel = DoCancel(),
					() => UserOperateMode != OperateMode.Query,
					o =>
					{
						if (isCancel)
							SetUserOperateMode(OperateMode.Query);
					}
					);
			}
		}

		private bool DoCancel()
		{
			//執行取消動作
			if (ShowMessage(Messages.WarningBeforeCancel) != UILib.Services.DialogResponse.Yes)
				return false;
			if (!string.IsNullOrWhiteSpace(SelectCustCode))
				DoSearch();
			else
				F194704DataList = new SelectionNameValuePiarList<F194704Data, string>();
			return true;
		}
		#endregion Cancel

		private bool _allSelectF194704DataListCheckBox;

		public bool AllSelectF194704DataListCheckBox
		{
			get { return _allSelectF194704DataListCheckBox; }
			set
			{
				Set(() => AllSelectF194704DataListCheckBox, ref _allSelectF194704DataListCheckBox, value);
			}
		}


		public ICommand F194704DataListCheckBoxCommand
		{
			get
			{
				return new RelayCommand(
					() => SelectedAllCheckedBox(),
					() => UserOperateMode == OperateMode.Query && F194704DataList != null);
			}
		}

		void SelectedAllCheckedBox()
		{
			foreach (var si in F194704DataList)
			{
				si.IsSelected = AllSelectF194704DataListCheckBox;
			}
		}

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(),
					() => UserOperateMode == OperateMode.Query && F194704DataList.Any(si => si.IsSelected)
					);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
			if (ShowMessage(Messages.WarningBeforeDelete) == UILib.Services.DialogResponse.Yes)
			{
				var proxy = GetProxy<F19Entities>();
				foreach (var item in F194704DataList.Where(si => si.IsSelected).Select(si => ExDataMapper.Map<F194704Data, F194704>(si.Item)))
				{
					var f194704Entity = GetF194704(proxy, item);
					if (f194704Entity != null)
						proxy.DeleteObject(f194704Entity);
				}
				proxy.SaveChanges();
				ShowMessage(Messages.DeleteSuccess);
				DoSearch();
			}

		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				bool isSave = false;
				return CreateBusyAsyncCommand(
					o => isSave = DoSave(), () => UserOperateMode != OperateMode.Query && SelectedF194704Data != null,
					o =>
					{
						if (isSave)
						{
							SetUserOperateMode(OperateMode.Query);
							SearchCommand.Execute(null);
						}
					}
					);
			}
		}

		private bool DoSave()
		{
			//執行確認儲存動作
			var proxy = GetProxy<F19Entities>();
			var f194704 = ExDataMapper.Map<F194704Data, F194704>(SelectedF194704Data.Item);

			var error = GetEditableError(f194704);
			if (!string.IsNullOrEmpty(error))
			{
				ShowWarningMessage(error);
				return false;
			}

			if (UserOperateMode == OperateMode.Add)
			{
				var f194704Entity = GetF194704(proxy, f194704);
				if (f194704Entity != null)
				{
					ShowWarningMessage(Properties.Resources.P7109020000_ViewModel_Deliver_GUP_CUST_Duplicate);
					return false;
				}

				proxy.AddToF194704s(f194704);
				proxy.SaveChanges();

			}
			else if (UserOperateMode == OperateMode.Edit)
			{
				var updata = GetF194704(proxy, ExDataMapper.Map<F194704Data, F194704>(SelectedF194704Data.Item));
				updata.PRINTER_TYPE = SelectedF194704Data.Item.PRINTER_TYPE;
				updata.GET_CONSIGN_NO = SelectedF194704Data.Item.GET_CONSIGN_NO;
				updata.CONSIGN_FORMAT = SelectedF194704Data.Item.CONSIGN_FORMAT;
				updata.AUTO_PRINT_CONSIGN = SelectedF194704Data.Item.AUTO_PRINT_CONSIGN;
				updata.PRINT_CONSIGN = SelectedF194704Data.Item.PRINT_CONSIGN;
				updata.ADDBOX_GET_CONSIGN_NO = SelectedF194704Data.Item.ADDBOX_GET_CONSIGN_NO;
				updata.ZIP_CODE = SelectedF194704Data.Item.ZIP_CODE;
				updata.UPD_DATE = DateTime.Now;
				updata.UPD_STAFF = Wms3plSession.Get<UserInfo>().Account;
				updata.UPD_NAME = Wms3plSession.Get<UserInfo>().AccountName;
				proxy.UpdateObject(updata);
				proxy.SaveChanges();
			}

			ShowMessage(Messages.Success);
			return true;
		}

		public string GetEditableError(F194704 e)
		{
			if (e == null)
				return Properties.Resources.P7109020000_ViewModel_Empty_Value;

			if (string.IsNullOrEmpty(e.DC_CODE))
				return Properties.Resources.P7109020000_ViewModel_DC_Required;

			if (string.IsNullOrEmpty(e.ALL_ID))
				return Properties.Resources.P7109020000_ViewModel_Deliver_Required;

			if (string.IsNullOrEmpty(e.GUP_CODE))
				return Properties.Resources.P7109020000_ViewModel_GUP_Required;

			if (string.IsNullOrEmpty(e.CUST_CODE))
				return Properties.Resources.P7109020000_ViewModel_CUST_Required;

			return string.Empty;
		}

		F194704 GetF194704(F19Entities proxy, F194704 e)
		{
			var query = from item in proxy.F194704s
						where item.DC_CODE == e.DC_CODE
						where item.ALL_ID == e.ALL_ID
						where item.GUP_CODE == e.GUP_CODE
						where item.CUST_CODE == e.CUST_CODE
						select item;

			return query.FirstOrDefault();
		}
		#endregion Save

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedF194704Data != null, o => SetUserOperateMode(OperateMode.Edit)
					);
			}
		}

		private void DoEdit()
		{

		}
		#endregion Edit

		private ICommand _dcCodeSelectionChangedCommand;

		public ICommand DcCodeSelectionChangedCommand
		{
			get
			{
				return _dcCodeSelectionChangedCommand ??
					(_dcCodeSelectionChangedCommand = CreateBusyAsyncCommand(
					(o) =>
					{
						UpdateAllItemSource();
					}));
			}
		}

		private void UpdateAllItemSource()
		{
			if (SearchDcCode == null)
				return;

			SetAllList(SearchDcCode);
			SetConsignFormatList();
			SetConsignNo();
			SetAddBoxConsignNo();
			SetPrinterTypeList();
			SetYesOrNoList();
			DoSearch();
		}
	}
}
