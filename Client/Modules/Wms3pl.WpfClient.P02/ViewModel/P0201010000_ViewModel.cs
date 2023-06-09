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
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.Services;
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
using wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;
using Wms3pl.WpfClient.UcLib.Views;
//using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0201010000_ViewModel : InputViewModelBase
	{
		public Action OnAddFocus = delegate { };
		public Action OnEditFocus = delegate { };
		public Action<string> OnSearchVnrCodeForAdd = x => { };


		public P0201010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}
		}

		private void InitControls()
		{
			SetTimeList();
			DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			QueryF020103.DC_CODE = DcList.Select(x => x.Value).FirstOrDefault();
			QueryF020103.ARRIVE_DATE = DateTime.Today;
			QueryF020103.ARRIVE_TIME = TimeList.Select(x => x.Value).FirstOrDefault();
		}

		#region 資料連結/ 頁面參數


		private string _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;

		private string _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;

		#region Form - 可用的DC (物流中心)清單

		private List<NameValuePair<string>> _dcList;

		/// <summary>
		/// 物流中心清單
		/// </summary>
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				_dcList = value;
				RaisePropertyChanged("DcList");
			}
		}

		#endregion

		#region Data - 查詢結果

		private ObservableCollection<F020103Detail> _purchaseList = new ObservableCollection<F020103Detail>();

		/// <summary>
		/// 進場主檔明細資料 - 依據日期/ 時段/ 物流中心/ 廠商編號查詢
		/// </summary>
		public ObservableCollection<F020103Detail> PurchaseList
		{
			get { return _purchaseList; }
			set
			{
				_purchaseList = value;
				RaisePropertyChanged("PurchaseList");
			}
		}

		private F020103Detail _selectedF020103 = null;

		/// <summary>
		/// 當前選取的進場主檔
		/// </summary>
		public F020103Detail SelectedF020103
		{
			get { return _selectedF020103; }
			set
			{
				Set(() => SelectedF020103, ref _selectedF020103, value);
			}
		}

		#endregion

		#region 是否有廠商
		private bool _hasVendor;

		public bool HasVendor
		{
			get { return _hasVendor; }
			set
			{
				Set(() => HasVendor, ref _hasVendor, value);
			}
		}

		#endregion

		#endregion

		private F020103Detail _queryF020103 = new F020103Detail();

		public F020103Detail QueryF020103
		{
			get { return _queryF020103; }
			set
			{
				Set(() => QueryF020103, ref _queryF020103, value);
			}
		}


		private F020103Detail _editF020103;

		public F020103Detail EditF020103
		{
			get { return _editF020103; }
			set
			{
				Set(() => EditF020103, ref _editF020103, value);
			}
		}

		private List<NameValuePair<string>> _pierList;

		/// <summary>
		/// 碼頭清單. 依據所選擇的物流中心而改變.
		/// </summary>
		public List<NameValuePair<string>> PierList
		{
			get { return _pierList; }
			set
			{
				Set(() => PierList, ref _pierList, value);
			}
		}

		/// <summary>
		/// 碼頭清單
		/// </summary>
		void SetPierList(string dcCode, DateTime? arriveDate)
		{
			if (dcCode != null && arriveDate.HasValue)
				PierList = PiersService.AllowInPiers(dcCode, arriveDate.Value, FunctionCode);
		}

		public void SetPierListByAddMode()
		{
			if (EditF020103 != null && UserOperateMode == OperateMode.Add)
				SetPierList(EditF020103.DC_CODE, EditF020103.ARRIVE_DATE);
		}

		private string _addCustOrdNo;

		public string AddCustOrdNo
		{
			get { return _addCustOrdNo; }
			set
			{
				Set(() => AddCustOrdNo, ref _addCustOrdNo, value);
			}
		}

		#region Command

		#region Search

		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(),
					() => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoSearch()
		{
			// 檢查日期有沒有輸入 - 查詢時的必要條件
			if (!QueryF020103.ARRIVE_DATE.HasValue)
			{
				ShowWarningMessage(Properties.Resources.P0201010000_SelectDate);
				return;
			}

			//執行查詢動作
			var proxy = GetExProxy<P02ExDataSource>();
			PurchaseList = proxy.GetF020103Detail(QueryF020103.ARRIVE_DATE,
													QueryF020103.ARRIVE_TIME,
													QueryF020103.DC_CODE,
													QueryF020103.VNR_CODE,
													_custCode,
													_gupCode).ToObservableCollection();

			if (!PurchaseList.Any())
				ShowMessage(Messages.InfoNoData);
		}

		#endregion Search

		#region Add

		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(),
					() => UserOperateMode == OperateMode.Query,
					o => OnAddFocus()
					);
			}
		}

		private void DoAdd()
		{
			EditF020103 = new F020103Detail
			{
				DC_CODE = DcList.Select(x => x.Value).FirstOrDefault(),
				GUP_CODE = _gupCode,
				CUST_CODE = _custCode,
				ARRIVE_DATE = DateTime.Today
			};

			AddCustOrdNo = string.Empty;

			UserOperateMode = OperateMode.Add;
        }

		#endregion Add

		#region Edit

		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(),
					() => UserOperateMode == OperateMode.Query && SelectedF020103 != null,
					o =>
					{
						// 列出碼頭資料
						SetPierList(SelectedF020103.DC_CODE, SelectedF020103.ARRIVE_DATE);
						EditF020103 = SelectedF020103.Clone();
						UserOperateMode = OperateMode.Edit;
						OnEditFocus();
					}
					);
			}
		}

		private void DoEdit()
		{
			EditF020103 = SelectedF020103.Clone();
			var f010201 = FindF010201byStockNo();
			if (f010201 != null)
				AddCustOrdNo = f010201.CUST_ORD_NO;

		}

		#endregion Edit

		#region Cancel

		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(),
					() => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
			if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.No)
				return;

			DoSearch();
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
					() => UserOperateMode == OperateMode.Query && SelectedF020103 != null
					);
			}
		}

		private void DoDelete()
		{
			// 確認是否要刪除
			var msg = Messages.WarningBeforeDelete;
			msg.Message = Properties.Resources.P0201010000_DelData;
			if (ShowMessage(msg) != DialogResponse.Yes) return;
			//執行刪除動作
			var proxy = GetExProxy<P02ExDataSource>();
			var result = proxy.CreateQuery<ExecuteResult>("Delete")
				.AddQueryOption("date", string.Format("'{0}'", SelectedF020103.ARRIVE_DATE.Value.ToString("yyyy/MM/dd")))
				.AddQueryOption("serialNo", string.Format("'{0}'", SelectedF020103.SERIAL_NO))
				.AddQueryOption("purchaseNo", string.Format("'{0}'", SelectedF020103.PURCHASE_NO))
				.AddQueryOption("dcCode", string.Format("'{0}'", SelectedF020103.DC_CODE))
				.AddQueryOption("gupCode", string.Format("'{0}'", SelectedF020103.GUP_CODE))
				.AddQueryOption("custCode", string.Format("'{0}'", SelectedF020103.CUST_CODE)).ToList();

			// 成功刪除後重查資料
			if (result != null && result.FirstOrDefault().IsSuccessed == true)
			{
				msg = Messages.Success;
				msg.Message = Properties.Resources.P0201010000_DelSuccess;
				ShowMessage(msg);
				DoSearch();
			}
			else
			{
				ShowMessage(result);
			}
		}

		#endregion Delete

		#region Save

		public ICommand SaveCommand
		{
			get
			{
				bool isSaved = false;
				return CreateBusyAsyncCommand(
					o => isSaved = DoSave(),
					CanExecuteSaveCommand,
					o =>
					{

					}
					);
			}
		}

		bool CanExecuteSaveCommand()
		{
			if (UserOperateMode == OperateMode.Query)
				return false;

			if (EditF020103 == null
			|| string.IsNullOrEmpty(EditF020103.PURCHASE_NO)
			|| string.IsNullOrEmpty(EditF020103.PIER_CODE))
				return false;

			if (UserOperateMode == OperateMode.Add && !HasVendor)
				return false;

            
            //if (string.IsNullOrWhiteSpace(AddCustOrdNo) && UserOperateMode == OperateMode.Add)
            //  return false;

			return true;
		}

		private bool DoSave()
		{
			if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes)
				return false;

			var errorMsg = DoCheckData();
			if (!string.IsNullOrEmpty(errorMsg))
			{
				ShowWarningMessage(errorMsg);
				return false;
			}

			var f020103 = EditF020103.Map<F020103Detail, wcf.F020103>();
			var proxyWcf = new wcf.P02WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
							() => proxyWcf.InsertOrUpdateF020103(f020103, UserOperateMode == OperateMode.Add));

			ShowResultMessage(result);
			if (result.IsSuccessed)
			{
				DoSearch();
				UserOperateMode = OperateMode.Query;
			}
			return result.IsSuccessed;
		}

		/// <summary>
		/// 檢查必要資料是否都有填入
		/// </summary>
		/// <returns></returns>
		private string DoCheckData()
		{
			if (String.IsNullOrEmpty(EditF020103.PIER_CODE))
			{
				return Properties.Resources.P0201010000_PierCodeIsNull;
			}

			if (String.IsNullOrEmpty(EditF020103.PURCHASE_NO))
			{
				return Properties.Resources.P0201010000_PurchaseNoIsNull;
			}

			if (String.IsNullOrEmpty(EditF020103.VNR_CODE))
			{
				return Properties.Resources.P0201010000_VnrCodeIsNull;
			}

			if (String.IsNullOrEmpty(EditF020103.ARRIVE_TIME))
			{
				return Properties.Resources.P0201010000_ArriveTimeIsNull;
			}

			if (!IsPurchaseNoValid())
			{
				return Properties.Resources.P0201010000_IsPurchaseNoValid;
			}

			if (UserOperateMode == OperateMode.Add)
			{
				if (!HasVendor)
				{
					return Properties.Resources.P0201010000_VnrCodeNotExist;
				}
			}

			return string.Empty;

		}

		#endregion Save

		#endregion

		private List<NameValuePair<string>> _timeList;

		public List<NameValuePair<string>> TimeList
		{
			get { return _timeList; }
			set
			{
				_timeList = value;
				RaisePropertyChanged("TimeList");
			}
		}

		private List<NameValuePair<string>> _editTimeList;

		public List<NameValuePair<string>> EditTimeList
		{
			get { return _editTimeList; }
			set
			{
				_editTimeList = value;
				RaisePropertyChanged("EditTimeList");
			}
		}

		private void SetTimeList()
		{
			var data = GetBaseTableService.GetF000904List(FunctionCode, "F020103", "ARRIVE_TIME");
			TimeList = (from o in data
						select new NameValuePair<string>
						{
							Name = o.Name,
							Value = o.Value
						}).ToList();
			TimeList.Insert(0, new NameValuePair<string> { Name = Properties.Resources.P0201010000_All, Value = "" });

			EditTimeList = (from o in data
							select new NameValuePair<string>
							{
								Name = o.Name,
								Value = o.Value
							}).ToList();

		}

		#region 進倉單號 Enter and LostFocus
		public ICommand SearchVendorCommand
		{
			get
			{
				return CreateBusyAsyncCommand(o => DoSearchVendorInfo());
			}
		}

		/// <summary>
		/// 查詢廠商資訊. 輸入進倉單號時自動查詢.
		/// 在新增資料的狀態下, 才做點選了物流中心後, 重新查詢進倉單的動作
		/// </summary>
		public void DoSearchVendorInfo()
		{
			if (string.IsNullOrEmpty(EditF020103.PURCHASE_NO))
				return;

			var f010201 = FindF010201byStockNo();
			if (f010201 == null)
			{
				ShowWarningMessage(Properties.Resources.P0201010000_IsPurchaseNoValid);
				return;
			}

			// 從進倉單搜尋，帶出貨主單號與廠商編號
			AddCustOrdNo = f010201.CUST_ORD_NO;

			DispatcherAction(() =>
			{
				OnSearchVnrCodeForAdd(f010201.VNR_CODE);
			});

		}

		public bool IsPurchaseNoValid()
		{
			return FindF010201byStockNo() != null;
		}

		F010201 FindF010201byStockNo()
		{
			var proxy = GetProxy<F01Entities>();

			return proxy.F010201s.Where(x => x.DC_CODE.Equals(EditF020103.DC_CODE)
											&& x.GUP_CODE.Equals(EditF020103.GUP_CODE)
											&& x.CUST_CODE.Equals(EditF020103.CUST_CODE)
											&& x.STOCK_NO.Equals(EditF020103.PURCHASE_NO))
								.FirstOrDefault();
		}
		#endregion

		public ICommand AddCustOrdNoCommand
		{
			get
			{
				return CreateBusyAsyncCommand(o =>
				{
					if (string.IsNullOrEmpty(AddCustOrdNo))
						return;

					var f010201 = FindF010201byCustOrdNo();
					if (f010201 == null)
					{
            EditF020103.PURCHASE_NO = string.Empty;
						ShowWarningMessage(Properties.Resources.P0201010000_CustOrdNoError);
						return;
					}

					EditF020103.PURCHASE_NO = f010201.STOCK_NO;

          #region 修正 賦予新的 PURCHASE_NO 後沒做檢查
          if (string.IsNullOrEmpty(EditF020103.PURCHASE_NO))
            return;

          var f010201_1 = FindF010201byStockNo();
          if (f010201_1 == null)
          {
            ShowWarningMessage(Properties.Resources.P0201010000_IsPurchaseNoValid);
            return;
          }
          #endregion 

          DispatcherAction(() =>
					{
						OnSearchVnrCodeForAdd(f010201.VNR_CODE);
					});
				});
			}
		}

		F010201 FindF010201byCustOrdNo()
		{
			var proxy = GetProxy<F01Entities>();

			return proxy.F010201s.Where(x => x.DC_CODE.Equals(EditF020103.DC_CODE)
											&& x.GUP_CODE.Equals(EditF020103.GUP_CODE)
											&& x.CUST_CODE.Equals(EditF020103.CUST_CODE)
											&& x.CUST_ORD_NO.Equals(AddCustOrdNo))
								.FirstOrDefault();
		}
	}
}
