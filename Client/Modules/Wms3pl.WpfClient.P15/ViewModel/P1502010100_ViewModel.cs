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

using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F15DataService;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.ExDataServices.P15ExDataService;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P15WcfService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;

namespace Wms3pl.WpfClient.P15.ViewModel
{
	public partial class P1502010100_ViewModel : InputViewModelBase
	{
		#region 共用變數/資料連結/頁面參數
		public string _userId;
		private string _userName;
		public Func<bool?> OnShowP0203010200 = () => { return false; };

		#region PreData - 來源資料
		private F151001 _sourceData;
		public F151001 SourceData
		{
			get { return _sourceData; }
			set
			{
				_sourceData = value;
				RaisePropertyChanged("SourceData");
			}
		}
		#endregion
		#region Form - 調撥單號
		private string _txtALLOCATION_NO;
		public string TxtALLOCATION_NO
		{
			get { return _txtALLOCATION_NO; }
			set
			{
				_txtALLOCATION_NO = value;
				RaisePropertyChanged("TxtALLOCATION_NO");
			}
		}
		#endregion
		#region Form - 調撥日期
		private DateTime? _txtAllocation_Date;
		public DateTime? TxtAllocation_Date
		{
			get { return _txtAllocation_Date; }
			set
			{
				_txtAllocation_Date = value;
				RaisePropertyChanged("TxtAllocation_Date");
			}
		}
		#endregion
		#region Data - 調撥單明細List
		private List<SelectionItem<F151001DetailDatas>> _dgItemSource;
		public List<SelectionItem<F151001DetailDatas>> DgItemSource
		{
			get { return _dgItemSource; }
			set
			{
				_dgItemSource = value;
				RaisePropertyChanged("DgItemSource");
			}
		}

		private SelectionItem<F151001DetailDatas> _selectedDgItem;
		public SelectionItem<F151001DetailDatas> SelectedDgItem
		{
			get { return _selectedDgItem; }
			set
			{
				_selectedDgItem = value;
				RaisePropertyChanged("SelectedDgItem");
			}
		}

		private bool _isSelectedAll = false;
		public bool IsSelectedAll
		{
			get { return _isSelectedAll; }
			set { _isSelectedAll = value; RaisePropertyChanged("IsSelectedAll"); }
		}

		public bool IsSaveOk;
		#endregion


		#region 是否啟用修改上架儲位
		private bool _isEnabledTarLocCode;

		public bool IsEnabledTarLocCode
		{
			get { return _isEnabledTarLocCode; }
			set
			{
				if (_isEnabledTarLocCode == value)
					return;
				Set(() => IsEnabledTarLocCode, ref _isEnabledTarLocCode, value);
			}
		}
		#endregion


		#region 是可可修改上架數
		private bool _readOnlyTarQty;

		public bool ReadOnlyTarQty
		{
			get { return _readOnlyTarQty; }
			set
			{
				if (_readOnlyTarQty == value)
					return;
				Set(() => ReadOnlyTarQty, ref _readOnlyTarQty, value);
			}
		}
		#endregion

		#endregion

		#region 函式
		public P1502010100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				_userId = Wms3plSession.Get<UserInfo>().Account;
				_userName = Wms3plSession.Get<UserInfo>().AccountName;
				GetDcCodeList();
				SetWarehouseList();
			}
		}

		private List<NameValuePair<string>> _dcCodeList;

		public List<NameValuePair<string>> DcCodeList
		{
			get { return _dcCodeList; }
			set
			{
				if (_dcCodeList == value)
					return;
				Set(() => DcCodeList, ref _dcCodeList, value);
			}
		}

		public void GetDcCodeList()
		{
			DcCodeList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
		}

        private List<NameValuePair<string>> _sRC_WarehouseList;

        public List<NameValuePair<string>> SRC_WarehouseList
        {
            get { return _sRC_WarehouseList; }
            set
            {
                Set(() => SRC_WarehouseList, ref _sRC_WarehouseList, value);
            }
        }

        private List<NameValuePair<string>> tAR_WarehouseList;

        public List<NameValuePair<string>> TAR_WarehouseList
        {
            get { return tAR_WarehouseList; }
            set
            {
                Set(() => TAR_WarehouseList, ref tAR_WarehouseList, value);
            }
        }

        public void SetWarehouseList()
		{
			var proxy = GetProxy<F19Entities>();
            if (DgItemSource==null || !DgItemSource.Any())
            {
                SRC_WarehouseList = (from o in proxy.F1980s                       
                                 select new NameValuePair<string>
                                 {
                                     Name = o.WAREHOUSE_NAME,
                                     Value = o.WAREHOUSE_ID
                                 }).ToList();
                TAR_WarehouseList = SRC_WarehouseList;
            }
            else
            {
                SRC_WarehouseList = (from o in proxy.F1980s
                                 where o.DC_CODE == DgItemSource.FirstOrDefault().Item.SRC_DC_CODE
                                 select new NameValuePair<string>
                                 {
                                     Name = o.WAREHOUSE_NAME,
                                     Value = o.WAREHOUSE_ID
                                 }).ToList();
                TAR_WarehouseList = (from o in proxy.F1980s
                                     where o.DC_CODE == DgItemSource.FirstOrDefault().Item.TAR_DC_CODE
                                     select new NameValuePair<string>
                                     {
                                         Name = o.WAREHOUSE_NAME,
                                         Value = o.WAREHOUSE_ID
                                     }).ToList();
            }		
        }

        public List<F151001DetailDatas> GetCheckSearch()
		{
			var result = new List<F151001DetailDatas>();
			if (DgItemSource != null)
			{
				var sel = DgItemSource.Where(x => x.IsSelected == true).ToList();
				result = (from i in sel
						  select i.Item).ToList();
			}
			return result;
		}

		public F1510Data GetF1510Data(F151001DetailDatas selDgItem)
		{
			var proxyEx = GetExProxy<P02ExDataSource>();
			var F1510Datas = proxyEx.CreateQuery<F1510Data>("GetF1510Data")
				.AddQueryExOption("dcCode",selDgItem.DC_CODE)
				.AddQueryExOption("gupCode",selDgItem.GUP_CODE)
				.AddQueryExOption("custCode",selDgItem.CUST_CODE)
				.AddQueryExOption("allocationNo",selDgItem.ALLOCATION_NO)
				.AddQueryExOption("allocationDate",selDgItem.ALLOCATION_DATE.ToString("yyyy/MM/dd"))
				.AddQueryExOption("status",selDgItem.STATUS)
				.AddQueryExOption("userId",_userId)
				.AddQueryExOption("makeNo", selDgItem.MAKE_NO)
				.AddQueryExOption("enterDate",selDgItem.ENTER_DATE)
				.AddQueryExOption("srcLocCode",selDgItem.SRC_LOC_CODE);
			if (F1510Datas != null && F1510Datas.ToList().Any())
				return F1510Datas.ToList().FirstOrDefault(o=> o.ITEM_CODE == selDgItem.ITEM_CODE && o.TAR_LOC_CODE == selDgItem.TAR_LOC_CODE && o.VALID_DATE == selDgItem.VALID_DATE) ?? F1510Datas.ToList().FirstOrDefault();
			else
				return null;
		}
		#endregion

		#region Command
		#region CheckAll
		public ICommand CheckAllCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCheckAllItem()
				);
			}
		}

		public void DoCheckAllItem()
		{
			if (DgItemSource != null)
			{
				foreach (var p in DgItemSource)
				{
					p.IsSelected = IsSelectedAll;
				}
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
      if (SourceData != null)
      {
        //執行查詢動
        var proxyEx = GetExProxy<P15ExDataSource>();
        var detailqry = proxyEx.CreateQuery<F151001DetailDatas>("GetF151001DetailDatas")
                      .AddQueryExOption("dcCode", SourceData.DC_CODE)
                      .AddQueryExOption("gupCode", SourceData.GUP_CODE)
                      .AddQueryExOption("custCode", SourceData.CUST_CODE)
                      .AddQueryExOption("allocationNo", TxtALLOCATION_NO)
                      .AddQueryExOption("action", "03")
                      .AsQueryable();

        ReadOnlyTarQty = !string.IsNullOrWhiteSpace(SourceData.SOURCE_NO) || string.IsNullOrWhiteSpace(SourceData.SRC_WAREHOUSE_ID);
        IsEnabledTarLocCode = !string.IsNullOrWhiteSpace(SourceData.TAR_WAREHOUSE_ID);
        DgItemSource = detailqry.OrderBy(x => x.ROWNUM).ToSelectionList().ToList();
        //取得資料後刷新來源倉別、目的商別的顯示List
        SetWarehouseList();
      }
      else
      {
				ShowMessage(Messages.InfoNoData);
				DgItemSource = new List<SelectionItem<F151001DetailDatas>>();
			}
		}
		#endregion Search

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
		}
		#endregion Cancel

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => UserOperateMode == OperateMode.Query && GetCheckSearch().Count > 0
					);
			}
		}

		private bool DoSave()
		{
			//執行確認儲存動作
			var message = new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Error,
				Message = "",
				Title = Resources.Resources.Information
			};
			var f1510SelectedDatas = GetCheckSearch();
			if (!f1510SelectedDatas.Any())
				message.Message = Properties.Resources.P1502010100_ChooseCheckOutItems;
			else if (f1510SelectedDatas.Any(o => o.BUNDLE_SERIALLOC == "1" && o.CHECK_SERIALNO == "0"))
				message.Message = Properties.Resources.P1502010100_ScanBundleSerialNoBeforeOntheMarket;
			else
			{
				var proxyWcf = new wcf.P15WcfServiceClient();
				var datas = from o in f1510SelectedDatas 
							select new ExDataServices.P15WcfService.F1510Data
							{
								IsSelected = true,
								ALLOCATION_DATE = o.ALLOCATION_DATE,
								ALLOCATION_NO = o.ALLOCATION_NO,
								ALLOCATION_SEQ_LIST = o.ALLOCATION_SEQ_LIST,
								ITEM_CODE = o.ITEM_CODE,
								ITEM_NAME = o.ITEM_NAME,
								QTY = int.Parse((o.SRC_QTY ?? o.TAR_QTY ?? 0).ToString()),
								SUG_LOC_CODE = o.SUG_LOC_CODE,
								TAR_LOC_CODE = o.TAR_LOC_CODE,
								BUNDLE_SERIALLOC = o.BUNDLE_SERIALLOC,
								CHECK_SERIALNO = o.CHECK_SERIALNO,
								DC_CODE = o.DC_CODE,
								GUP_CODE = o.GUP_CODE,
								CUST_CODE = o.CUST_CODE,
								SRC_LOC_CODE = o.SRC_LOC_CODE,
								VALID_DATE = o.VALID_DATE,
								MAKE_NO = o.MAKE_NO
							};
				var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel, () => proxyWcf.UpdateF1510Data(SourceData.TAR_DC_CODE, SourceData.GUP_CODE, SourceData.CUST_CODE, SourceData.ALLOCATION_NO, datas.ToArray()));
				if (result.IsSuccessed)
				{

					ShowMessage(Messages.InfoUpdateSuccess);
					return true;
				}
				else
				{
					var error = Messages.ErrorUpdateFailed;
					error.Message += Environment.NewLine + result.Message;
					ShowMessage(error);
					return false;
				}
			}
			if (message.Message.Length > 0)
			{
				ShowMessage(message);
				return false;
			}
			else
				return true;
		}

		public bool SaveRuning()
		{
			if (DoSave())
				return true;
			else
				return false;
		}
		#endregion Save

		#region AddDetailCommand
		public ICommand AddDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAddDetail(), () => UserOperateMode == OperateMode.Query && SelectedDgItem != null
					);
			}
		}

		private void DoAddDetail()
		{

		}

		#endregion

		#region 開啟序號儲位刷讀
		private ICommand _serialLocCheckCommand;

		public ICommand SerialLocCheckCommand
		{
			get
			{
				return _serialLocCheckCommand = (_serialLocCheckCommand ?? new RelayCommand(
					 DoSerialLocCheck,
					 () => UserOperateMode == OperateMode.Query
						 && SelectedDgItem != null
						 && SelectedDgItem.Item.BUNDLE_SERIALLOC == "1"
					));
			}
		}

		private void DoSerialLocCheck()
		{
			if (!SerialLocCheckCommand.CanExecute(null))
				return;

			UserOperateMode = OperateMode.Add;

			//開啟序號儲位刷讀視窗
			if (OnShowP0203010200() == true)
			{
				SearchCommand.Execute(null);
			}

			UserOperateMode = OperateMode.Query;
		}
		#endregion
		#endregion
	}
}
