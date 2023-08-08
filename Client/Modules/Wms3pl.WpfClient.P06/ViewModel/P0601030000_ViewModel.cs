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
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.DataServices.F15DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices.P06ExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P06WcfService;
using Wms3pl.WpfClient.DataServices.F06DataService;

namespace Wms3pl.WpfClient.P06.ViewModel
{
	public partial class P0601030000_ViewModel : InputViewModelBase
	{
		static readonly string AllSelectText = Resources.Resources.All;
		public P0601030000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				SetDcList();
				if (DcList != null && DcList.Any())
				{
					SearchDcCode = DcList.FirstOrDefault().Value;
				}
				SearchDelvDate = DateTime.Now.Date;
			}
		}

		#region 查詢條件屬性

		#region 物流中心
		private string _searchDcCode;

		public string SearchDcCode
		{
			get { return _searchDcCode; }
			set
			{
				ClearSearrchResults();

				_searchDcCode = value;
				RaisePropertyChanged("SearchDcCode");
			}
		}

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

		/// <summary>
		/// 設定DC清單
		/// </summary>
		public void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
		}
		#endregion

		#region
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
		#endregion

		#region
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		#endregion

		#region 批次日期
		private DateTime _searchDelvDate;

		public DateTime SearchDelvDate
		{
			get { return _searchDelvDate; }
			set
			{
				_searchDelvDate = value;
				// 設定揀貨時段，並預設全部
				SetPickTimeList(value);
				SearchPickTime = AllSelectText;
				RaisePropertyChanged("SearchDelvDate");
			}
		}
		#endregion

		#region 批次時段
		private string _searchPickTime;

		public string SearchPickTime
		{
			get { return _searchPickTime; }
			set
			{
				_searchPickTime = value;
				RaisePropertyChanged("SearchPickTime");
			}
		}

		private List<string> _pickTimeList;

		public List<string> PickTimeList
		{
			get { return _pickTimeList; }
			set
			{
				_pickTimeList = value;
				RaisePropertyChanged("PickTimeList");
			}
		}

		/// <summary>
		/// 設定批次時段清單
		/// </summary>
		public void SetPickTimeList(DateTime delvDate)
		{
			var globalInfo = Wms3plSession.Get<GlobalInfo>();

			var proxy = GetProxy<F05Entities>();
			var query = from item in proxy.F0513s
						where item.GUP_CODE == globalInfo.GupCode
						where item.CUST_CODE == globalInfo.CustCode 
						where item.DC_CODE == SearchDcCode
						where item.DELV_DATE == SearchDelvDate
						orderby item.PICK_TIME
						select item;

			var pickTimeList = query.ToList().Select(item => item.PICK_TIME).ToList();
			pickTimeList.Insert(0, AllSelectText);
			PickTimeList = pickTimeList;
		}


		#endregion

		#region 貨主單號
		private string _searchCustOrdNo = string.Empty;

		public string SearchCustOrdNo 
		{
			get { return _searchCustOrdNo.Trim(); }
			set
			{
				_searchCustOrdNo = value;
				RaisePropertyChanged("SearchCustOrdNo");
			}
		}
		#endregion

		#region 訂單編號
		private string _searchOrdNo = string.Empty;

		public string SearchOrdNo
		{
			get { return _searchOrdNo.Trim(); }
			set
			{
				_searchOrdNo = value;
				RaisePropertyChanged("SearchOrdNo");
			}
		}
		#endregion

		#region 品號
		private string _searchItemCode = string.Empty;

		public string SearchItemCode
		{
			get { return _searchItemCode.Trim(); }
			set
			{
				_searchItemCode = value;
				RaisePropertyChanged("SearchItemCode");
			}
		}

		#endregion

		#region 訂單單號清單(查詢容器條碼用)
		private List<string> _ordNos;
		public List<string> OrdNos
		{
			get { return _ordNos; }
			set { Set(() => OrdNos, ref _ordNos, value); }
		}
		#endregion

		public List<string> _containerBarcodeResult;
		public List<string> ContainerBarcodeResult
		{
			get { return _containerBarcodeResult; }
			set { Set(() => ContainerBarcodeResult, ref _containerBarcodeResult, value); }
		}

		#endregion

		#region 查詢結果屬性

		private SelectionList<F05030101Ex> _searchResults;

		public SelectionList<F05030101Ex> SearchResults
		{
			get { return _searchResults; }
			set
			{
				_searchResults = value;
				RaisePropertyChanged("SearchResults");
			}
		}
					

		public void ClearSearrchResults()
		{
			SearchResults = null;
		}

		private bool _isFuncSelectedAll;

		public bool IsFuncSelectedAll
		{
			get { return _isFuncSelectedAll; }
			set
			{
				_isFuncSelectedAll = value;
				RaisePropertyChanged("IsFuncSelectedAll");
			}
		}


		public ICommand CheckAllTask
		{
			get
			{
				return new RelayCommand(() => SelectedAllCheckedBox(), () => { return true; });
			}
		}

		void SelectedAllCheckedBox()
		{
			if (SearchResults != null)
			{
				foreach (var item in SearchResults)
				{
					item.IsSelected = IsFuncSelectedAll;
				}
			}
		}

		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				//List<F050801> F050801List = null;
				return CreateBusyAsyncCommand(
					o => DoSearch(),
					() => UserOperateMode == OperateMode.Query,
					o =>
					{
						if (SearchResults.Any() == false)
							ShowMessage(Messages.InfoNoData);
					}
					);
			}
		}

		private void DoSearch()
		{
			//執行查詢動
			SearchCustOrdNo = SearchCustOrdNo.Trim();
			SearchOrdNo = SearchOrdNo.Trim();
			SearchItemCode = SearchItemCode.Trim();

			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			
			var proxy = GetExProxy<P06ExDataSource>();
			var query = proxy.CreateQuery<F05030101Ex>("GetP060103Data")
							 .AddQueryExOption("gupCode", globalInfo.GupCode)
							 .AddQueryExOption("custCode", globalInfo.CustCode)
							 .AddQueryExOption("dcCode", SearchDcCode)
							 .AddQueryExOption("delvDate", SearchDelvDate.ToString("yyyy/MM/dd"))
							 .AddQueryExOption("pickTime", SearchPickTime == AllSelectText ? null : SearchPickTime)
							 .AddQueryExOption("custOrdNo", SearchCustOrdNo)
							 .AddQueryExOption("ordNo", SearchOrdNo)
							 .AddQueryExOption("itemCode", SearchItemCode);
							 

			var results = query.ToList();

			SearchResults = new SelectionList<F05030101Ex>(results);
			IsFuncSelectedAll = false;
			ContainerBarcodeResult = null;
		}

		private bool TryValidateSearchConditions(out string errorMessage)
		{
			errorMessage = null;

			if (string.IsNullOrWhiteSpace(SearchPickTime))
				errorMessage = Properties.Resources.P0601030000_ViewModel_SelectBatchTime;

			if (SearchDelvDate == default(DateTime))
				errorMessage = Properties.Resources.P0601030000_ViewModel_SelectBathcDate;

			if (string.IsNullOrWhiteSpace(SearchDcCode))
				errorMessage = Properties.Resources.P0601030000_ViewModel_SelectDC;

			return errorMessage == null;
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
					o =>
					{
						if (ShowMessage(new MessagesStruct() { Message = Properties.Resources.P0601030000_ViewModel_ChkRecoverVirtualLoc, 
																Title = Properties.Resources.P0601030000_ViewModel_VirtualLocRecoverConfirm, 
																Image = UILib.Services.DialogImage.Question, 
																Button = UILib.Services.DialogButton.YesNo }) == UILib.Services.DialogResponse.Yes)
						{
							DoSave();
						}
					},
					() => UserOperateMode == OperateMode.Query && SearchResults != null && SearchResults.Any(item => item.IsSelected)
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作
			var selectedOrdNoList = (from selectionItem in SearchResults
									 where selectionItem.IsSelected
									 select selectionItem.Item.ORD_NO).ToArray();
			if (!selectedOrdNoList.Any())
			{
				DialogService.ShowMessage(Properties.Resources.P0601030000_ViewModel_selectedOrdNoList);
				return;
			}

			var first = SearchResults.First().Item;
			var gupCode = first.GUP_CODE;
			var custCode = first.CUST_CODE;
			var dcCode = first.DC_CODE;

			var proxy = new wcf.P06WcfServiceClient();

			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
														() => proxy.ConfirmP060103(gupCode, custCode, dcCode, selectedOrdNoList));

			DialogService.ShowMessage(result.Message);

			if ( result.IsSuccessed)
			{
				DoSearch();

				UserOperateMode = OperateMode.Query;
			}
		}
		#endregion Save

		private SelectionList<F05030101Ex> _selectedData;
		public SelectionList<F05030101Ex> SelectedData
		{
			get { return _selectedData; }
			set
			{
				_selectedData = value;
				RaisePropertyChanged("SelectedData");
			}
		}


		public void GetContainerBarcode()
		{
			ContainerBarcodeResult = null;
			var proxyWcf = new wcf.P06WcfServiceClient();
			//var proxy = GetExProxy<P06ExDataSource>();
			//var proxy = GetProxy<F07Entities>();
			var selectedOrdNoList = (from selectionItem in SearchResults
									 where selectionItem.IsSelected
									 select selectionItem.Item.ORD_NO).ToArray();
			if (selectedOrdNoList.Count() > 0)
			{
				var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
				() => proxyWcf.GetContainerBarcode(SearchDcCode, _gupCode, _custCode, selectedOrdNoList));

				if (result.IsSuccessed == false)
				{
					DialogService.ShowMessage(result.Message);
				}
				else
				{
					ContainerBarcodeResult = result.No.Split(',').ToList();
				}
			}
		}
	}
}
