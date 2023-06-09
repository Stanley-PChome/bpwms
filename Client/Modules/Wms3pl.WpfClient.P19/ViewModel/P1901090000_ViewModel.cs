using AutoMapper;
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

using System.Collections.ObjectModel;
using System.Reflection;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1901090000_ViewModel : InputViewModelBase
	{
		#region 共用變數/資料連結/頁面參數
		private string _userId;
		private string _userName;
		private string _custCode;
		public Action ToFirstTab = delegate { };
		public Action ScrollIntoView = () => { };
		public Action OnUpdateTab = () => { };

		#region UI 連動 binding
		private bool _searchResultIsExpanded = true;

		public bool SearchResultIsExpanded
		{
			get { return _searchResultIsExpanded; }
			set
			{
				_searchResultIsExpanded = value;
				RaisePropertyChanged("SearchResultIsExpanded");
			}
		}

		private string _quoteHeaderText;

		public string QuoteHeaderText
		{
			get { return _quoteHeaderText; }
			set
			{
				_quoteHeaderText = value;
				RaisePropertyChanged("QuoteHeaderText");
			}
		}

		private bool _queryResultIsExpanded = true;

		public bool QueryResultIsExpanded
		{
			get { return _queryResultIsExpanded; }
			set
			{
				_queryResultIsExpanded = value;
				RaisePropertyChanged("QueryResultIsExpanded");
			}
		}


		#endregion

		#region 查詢
		#region Form - 業主
		private List<NameValuePair<string>> _gupCodes;

		public List<NameValuePair<string>> GupCodes
		{
			get { return _gupCodes; }
			set
			{
				_gupCodes = value;
				RaisePropertyChanged("GupCodes");
			}
		}

		private string _selectedGupCode;
		public string SelectedGupCode
		{
			get { return _selectedGupCode; }
			set
			{
				_selectedGupCode = value;
				RaisePropertyChanged("SelectedGupCode");
				DgList = new List<F1908>();
			}
		}
		#endregion
		#region Form - 廠商編號
		private string _txtvrn_code = string.Empty;

		public string txtVRN_CODE
		{
			get { return _txtvrn_code; }
			set
			{
				_txtvrn_code = value;
				RaisePropertyChanged("txtVRN_CODE");
			}
		}
		#endregion
		#region Form - 廠商名稱
		private string _txtvrn_NAME = string.Empty;

		public string txtVRN_NAME
		{
			get { return _txtvrn_NAME; }
			set
			{
				_txtvrn_NAME = value;
				RaisePropertyChanged("txtVRN_NAME");
			}
		}
		#endregion
		#region Data - 資料List
		private List<F1908> _dgList;
		public List<F1908> DgList
		{
			get { return _dgList; }
			set
			{
				_dgList = value;
				RaisePropertyChanged("DgList");
			}
		}

		private F1908 _selectedData;

		public F1908 SelectedData
		{
			get { return _selectedData; }
			set
			{
				_selectedData = value;
				RaisePropertyChanged("SelectedData");
				var ordDate = (value == null) ? string.Empty : value.ORD_DATE;
				ORD_DATEArray = GetORD_DATEtoArray(ordDate);
				if (value == null)
					return;

				_lastAddOrUpdateF1908 = ExDataMapper.Clone(SelectedData);
			}
		}
		#endregion
		#endregion

		#region 新增/編輯模式
		#region 業主
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
		#endregion
		#region 發票樣式
		private List<NameValuePair<string>> _iNVO_TypeList;

		public List<NameValuePair<string>> INVO_TypeList
		{
			get { return _iNVO_TypeList; }
			set
			{
				_iNVO_TypeList = value;
				RaisePropertyChanged("INVO_TypeList");
			}
		}
		#endregion
		#region 幣別
		private List<NameValuePair<string>> _currencyList;

		public List<NameValuePair<string>> CurrencyList
		{
			get { return _currencyList; }
			set
			{
				_currencyList = value;
				RaisePropertyChanged("CurrencyList");
			}
		}
		#endregion
		#region 付款條件
		private List<NameValuePair<string>> _pAY_FACTORList;

		public List<NameValuePair<string>> PAY_FACTORList
		{
			get { return _pAY_FACTORList; }
			set
			{
				_pAY_FACTORList = value;
				RaisePropertyChanged("PAY_FACTORList");
			}
		}
		#endregion
		#region 付款方式
		private List<NameValuePair<string>> _pAY_TYPEList;

		public List<NameValuePair<string>> PAY_TYPEList
		{
			get { return _pAY_TYPEList; }
			set
			{
				_pAY_TYPEList = value;
				RaisePropertyChanged("PAY_TYPEList");
			}
		}
		#endregion
		#region 交貨模式清單
		private List<NameValuePair<string>> _dELI_TYPEList;

		public List<NameValuePair<string>> DELI_TYPEList
		{
			get { return _dELI_TYPEList; }
			set
			{
				_dELI_TYPEList = value;
				RaisePropertyChanged("DELI_TYPEList");
			}
		}
		#endregion
		#region 訂貨時間
		private string[] _oRD_DATEArray;

		public string[] ORD_DATEArray
		{
			get { return _oRD_DATEArray; }
			set
			{
				_oRD_DATEArray = value;
				RaisePropertyChanged("ORD_DATEArray");
			}
		}
		#endregion

		#region 進倉商品不檢驗
		private string _EXTENSION_A;

		public string EXTENSION_A
		{
			get { return _EXTENSION_A; }
			set
			{
				_EXTENSION_A = value;
				RaisePropertyChanged("EXTENSION_A");
			}
		}
		#endregion

		private List<NameValuePair<string>> _vnrTypeList;

		public List<NameValuePair<string>> VnrTypeList
		{
			get { return _vnrTypeList; }
			set
			{
				if (_vnrTypeList == value)
					return;
				Set(() => VnrTypeList, ref _vnrTypeList, value);
			}
		}

		public void SetVnrTypeList()
		{
			VnrTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F1908", "VNR_TYPE");
		}


		#region 配送方式
		private List<NameValuePair<string>> _deliveryWayList;

		public List<NameValuePair<string>> DeliveryWayList
		{
			get { return _deliveryWayList; }
			set
			{
				Set(() => DeliveryWayList, ref _deliveryWayList, value);
			}
		}
		#endregion


		#endregion

		#endregion

		#region 函式
		public P1901090000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}
		}

		private void InitControls()
		{
			_userId = Wms3plSession.Get<UserInfo>().Account;
			_userName = Wms3plSession.Get<UserInfo>().AccountName;
			_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			QuoteHeaderText = GetQuoteHeaderText();
			SetVnrTypeList();
			GupCodes = GetGupList(true);
			if (GupCodes.Any())
				SelectedGupCode = GupCodes.FirstOrDefault().Value;
			#region 新增/編輯模式
			GupList = GetGupList();
			CurrencyList = GetCurrencyList();
			INVO_TypeList = GetINVO_TypeList();
			PAY_TYPEList = GetPAY_TYPEList();
			PAY_FACTORList = GetPAY_FACTORList();
			DELI_TYPEList = GetDELI_TYPEList();
			DeliveryWayList = GetDeliveryWayList();
			#endregion
		}

		private string GetQuoteHeaderText()
		{
			switch (UserOperateMode)
			{
				case OperateMode.Edit:
					return Properties.Resources.P1901090000_VendorEdit;
				case OperateMode.Add:
					return Properties.Resources.P1901090000_VendorInsert;
				default:
					return Properties.Resources.P1901090000_VendorDetails;
			}
		}

		public List<NameValuePair<string>> GetGupList(bool canAll = false)
		{
			var query = from item in Wms3plSession.Get<GlobalInfo>().DcGupCustDatas
						group item by new { item.GupCode, item.GupName } into g
						select new NameValuePair<string>()
						{
							Value = g.Key.GupCode,
							Name = g.Key.GupName
						};
			var list = query.ToList();
			if (canAll)
			{
				list.Insert(0, new NameValuePair<string>() { Value = string.Empty, Name = Resources.Resources.All });
			}
			return list;
		}

		public List<NameValuePair<string>> GetINVO_TypeList()
		{
			return GetBaseTableService.GetF000904List(FunctionCode, "F1908", "INVO_TYPE");
		}

		public List<NameValuePair<string>> GetPAY_TYPEList()
		{
			return GetBaseTableService.GetF000904List(FunctionCode, "F1908", "PAY_TYPE");
		}

		public List<NameValuePair<string>> GetCurrencyList()
		{
			return GetBaseTableService.GetF000904List(FunctionCode, "F1909", "CURRENCY");
		}

		public List<NameValuePair<string>> GetPAY_FACTORList()
		{
			return GetBaseTableService.GetF000904List(FunctionCode, "F1909", "PAY_FACTOR");
		}

		public List<NameValuePair<string>> GetDELI_TYPEList()
		{
			return GetBaseTableService.GetF000904List(FunctionCode, "F1908", "DELI_TYPE");
		}
		public List<NameValuePair<string>> GetDeliveryWayList()
		{
			return GetBaseTableService.GetF000904List(FunctionCode, "F160201", "DELIVERY_WAY");
		}

		public string[] GetORD_DATEtoArray(string selectedORD_DATE)
		{
			string[] tmpArray = new string[8];
			if (selectedORD_DATE.Contains("1"))
				tmpArray[1] = "1";
			else
				tmpArray[1] = "0";
			if (selectedORD_DATE.Contains("2"))
				tmpArray[2] = "1";
			else
				tmpArray[2] = "0";
			if (selectedORD_DATE.Contains("3"))
				tmpArray[3] = "1";
			else
				tmpArray[3] = "0";
			if (selectedORD_DATE.Contains("4"))
				tmpArray[4] = "1";
			else
				tmpArray[4] = "0";
			if (selectedORD_DATE.Contains("5"))
				tmpArray[5] = "1";
			else
				tmpArray[5] = "0";
			if (selectedORD_DATE.Contains("6"))
				tmpArray[6] = "1";
			else
				tmpArray[6] = "0";
			if (selectedORD_DATE.Contains("7"))
				tmpArray[7] = "1";
			else
				tmpArray[7] = "0";

			return tmpArray;
		}

		public string GetArrayToORD_DATE(string[] arrayORD_DATE)
		{
			string tmpOrdDate = "";
			if (arrayORD_DATE != null)
			{
				for (int i = 1; i <= 7; i++)
				{
					if (arrayORD_DATE[i] == "1")
						tmpOrdDate = string.Format("{0}{1}", tmpOrdDate, i);
				}
			}
			return tmpOrdDate;
		}
		#endregion

		#region Command
		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoSearch(), () => UserOperateMode == OperateMode.Query,
						o =>
						{
							SelectedData = DgList.FirstOrDefault();
						}
						);
			}
		}

		private void DoSearch()
		{
			txtVRN_CODE = txtVRN_CODE.Trim();
			txtVRN_NAME = txtVRN_NAME.Trim();

			//執行查詢動作
			var proxy = GetProxy<F19Entities>();
			DgList = proxy.CreateQuery<F1908>("GetAllowedF1908s")
						.AddQueryExOption("custCode", _custCode)
												.AddQueryExOption("gupCode", SelectedGupCode)
						.AddQueryExOption("vnrCode", txtVRN_CODE)
												.AddQueryExOption("vnrName", txtVRN_NAME)
						.ToList();

			if (!DgList.Any())
			{
				SearchResultIsExpanded = !DgList.Any();
				SelectedData = null;
				DgList = new List<F1908>();
				ShowMessage(Messages.InfoNoData);
				return;
			}
			QueryResultIsExpanded = DgList.Count > 1;
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
			SelectedData = new F1908
			{
				STATUS = "0",
				PAY_FACTOR = "0",
				PAY_TYPE = "0",
				LEADTIME = 1,
				CHECKPERCENT = 0.00000000001m,
				ORD_DATE = "1234567",
				ORD_CIRCLE = 1,
				VNR_LIM_QTY = 1,
				ORD_STOCK_QTY = 0,
				INVO_TYPE = "0",
				VNR_TYPE = "0",
				CURRENCY = "NTD"
			};

			UserOperateMode = OperateMode.Add;
			QuoteHeaderText = GetQuoteHeaderText();
			OnUpdateTab();
			//執行新增動作
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedData != null
						);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			QuoteHeaderText = GetQuoteHeaderText();
			OnUpdateTab();
			//執行編輯動作
		}
		#endregion Edit

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
							{
								UserOperateMode = OperateMode.Query;
								QuoteHeaderText = GetQuoteHeaderText();
								if (_lastAddOrUpdateF1908 != null)
								{
									SelectedData = DgList.Where(item => item.GUP_CODE == _lastAddOrUpdateF1908.GUP_CODE
											&& item.VNR_CODE == _lastAddOrUpdateF1908.VNR_CODE)
											.FirstOrDefault();

									//ToFirstTab();
									ScrollIntoView();
								}
							}
						}
						);
			}
		}

		private bool DoCancel()
		{
			if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
			{
				//執行取消動作
				//if (UserOperateMode == OperateMode.Add)
				DoSearch();
				return true;
			}
			return false;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedData != null
						);
			}
		}

		private void DoDelete()
		{
			// 確認是否要刪除
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;
			//執行刪除動作
			var proxy = GetExProxy<P19ExDataSource>();
			var query = proxy.CreateQuery<Wms3pl.WpfClient.ExDataServices.P19ExDataService.ExecuteResult>("DeleteP190109")
							 .AddQueryExOption("gupCode", SelectedData.GUP_CODE)
							 .AddQueryExOption("vnrCode", SelectedData.VNR_CODE);

			var result = query.ToList().FirstOrDefault();
			ShowResultMessage(result);

			DoSearch();
		}
		#endregion Delete

		F1908 _lastAddOrUpdateF1908 = null;

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				bool isSaved = false;
				return CreateBusyAsyncCommand(
						o => isSaved = DoSave(),
						() => UserOperateMode != OperateMode.Query && SelectedData != null,
						o =>
						{
							if (isSaved)
							{
								if (_lastAddOrUpdateF1908 != null)
								{
									SelectedData = DgList.Where(item => item.GUP_CODE == _lastAddOrUpdateF1908.GUP_CODE
											&& item.VNR_CODE == _lastAddOrUpdateF1908.VNR_CODE)
											.FirstOrDefault();

									//ToFirstTab();
									ScrollIntoView();
								}
							}
						}
						);
			}
		}

		private bool DoSave()
		{
			if (ShowMessage(Messages.WarningBeforeUpdate) == DialogResponse.Yes)
			{
				//執行確認儲存動作
				if (UserOperateMode == OperateMode.Add)
					return DoSaveAdd();
				else if (UserOperateMode == OperateMode.Edit)
					return DoSaveEdit();
			}

			return false;
		}


		private bool DoSaveAdd()
		{
			MessagesStruct alertMessage = new MessagesStruct();
			alertMessage.Button = DialogButton.OK;
			alertMessage.Title = Resources.Resources.Information;
			alertMessage.Image = DialogImage.Warning;
			// 檢查資料
			if (SelectedData == null)
			{
				alertMessage.Message = Properties.Resources.P1901090000_InsertFail;
				ShowMessage(alertMessage);
				return false;
			}
			if (string.IsNullOrWhiteSpace(SelectedData.GUP_CODE))
			{
				ShowMessage(Messages.WarningNoGupCode);
				return false;
			}

			var error = GetEditableError(SelectedData);
			if (!string.IsNullOrEmpty(error))
			{
				ShowWarningMessage(error);
				return false;
			}

			var proxy = GetModifyQueryProxy<F19Entities>();
			var f1908s = proxy.F1908s.Where(x => x.GUP_CODE == SelectedData.GUP_CODE && x.VNR_CODE == SelectedData.VNR_CODE).AsQueryable().ToList();
			if (f1908s != null && f1908s.Count > 0)
			{
				// 資料已存在
				ShowWarningMessage(Properties.Resources.P1901090000_Ven_No_Duplicate);
				return false;
			}
			var f1909 = proxy.F1909s.Where(x => x.GUP_CODE == SelectedData.GUP_CODE && x.CUST_CODE == _custCode).AsQueryable().FirstOrDefault();
			SelectedData.ORD_DATE = GetArrayToORD_DATE(ORD_DATEArray);
			SelectedData.CUST_CODE = f1909.ALLOWGUP_VNRSHARE == "1" ? "0" : _custCode;
			proxy.AddToF1908s(SelectedData);
			proxy.SaveChanges();
			_lastAddOrUpdateF1908 = ExDataMapper.Clone(SelectedData);
			ShowMessage(Messages.Success);
			UserOperateMode = OperateMode.Query;
			SelectedGupCode = string.Empty;


			DoSearch();
			return true;
		}

		private bool DoSaveEdit()
		{
			MessagesStruct alertMessage = new MessagesStruct();
			alertMessage.Button = DialogButton.OK;
			alertMessage.Title = Resources.Resources.Information;
			alertMessage.Image = DialogImage.Warning;
			// 檢查資料
			if (SelectedData == null)
			{
				alertMessage.Message = Properties.Resources.P1901090000_UpdateFail;
				ShowMessage(alertMessage);
				return false;
			}
			if (string.IsNullOrWhiteSpace(SelectedData.GUP_CODE))
			{
				ShowMessage(Messages.WarningNoGupCode);
				return false;
			}

			var error = GetEditableError(SelectedData);
			if (!string.IsNullOrEmpty(error))
			{
				ShowWarningMessage(error);
				return false;
			}

			var proxy = GetModifyQueryProxy<F19Entities>();
			var f1908s = proxy.F1908s.Where(x => x.GUP_CODE == SelectedData.GUP_CODE && x.VNR_CODE == SelectedData.VNR_CODE).AsQueryable().ToList();
			if (f1908s == null || f1908s.Count == 0)
			{
				// 資料已刪除
				ShowMessage(Messages.WarningBeenDeleted);
				return false;
			}

			var f1908 = f1908s.FirstOrDefault();
			SelectedData.ORD_DATE = GetArrayToORD_DATE(ORD_DATEArray);
			var f1909 = proxy.F1909s.Where(x => x.GUP_CODE == SelectedData.GUP_CODE && x.CUST_CODE == _custCode).AsQueryable().FirstOrDefault();
			SelectedData.CUST_CODE = f1909.ALLOWGUP_VNRSHARE == "1" ? "0" : _custCode;

			ExDataMapper.SetProperties(SelectedData, f1908);

			//SetTargetData(ref f1908, SelectedData);
			proxy.UpdateObject(f1908);
			proxy.SaveChanges();
			_lastAddOrUpdateF1908 = ExDataMapper.Clone(SelectedData);
			ShowMessage(Messages.Success);
			UserOperateMode = OperateMode.Query;
			SelectedGupCode = string.Empty;
			DoSearch();

			return true;
		}

		string GetEditableError(F1908 e)
		{
			if (!string.IsNullOrEmpty(e.DELV_TIME))
			{
				DateTime time;
				if (!DateTime.TryParseExact(e.DELV_TIME, "HH:mm", null, System.Globalization.DateTimeStyles.None, out time))
					return Properties.Resources.P1901090000_TimeFormat_Error;
			}

			return string.Empty;
		}

		private void SetTargetData(ref F1908 TargetObj, F1908 cust)
		{
			foreach (var pro in typeof(F1908).GetProperties())
			{
				if (pro.Name.ToLower() == "error" || pro.Name.ToLower() == "item" || pro.Name.ToLower() == "f190101") continue;
				var value = pro.GetValue(cust);
				pro.SetValue(TargetObj, value);
			}
		}
		#endregion Save


		private ICommand _editableGupCodeSelectionChangedCommand;
		public ICommand EditableGupCodeSelectionChangedCommand
		{
			get
			{
				return _editableGupCodeSelectionChangedCommand ??
						(_editableGupCodeSelectionChangedCommand = new RelayCommand(
						() =>
						{
							if (SelectedData == null)
								return;
						}));
			}
		}
		#endregion
	}
}
