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
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.DataServices.F15DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices.P15ExDataService;
using P19EX = Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using P15Wcf = Wms3pl.WpfClient.ExDataServices.P15WcfService;
using Wms3pl.WpfClient.ExDataServices;
using System.Windows;

namespace Wms3pl.WpfClient.P15.ViewModel
{
	public partial class P1502010200_ViewModel : InputViewModelBase
	{
		#region 共用變數/資料連結/頁面參數
		public Action Closed = delegate { };
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

		#region Form - 查詢
		#region Form - 儲位起迄
		private string _locCodeS;
		public string LocCodeS
		{
			get { return _locCodeS; }
			set { _locCodeS = value; RaisePropertyChanged("LocCodeS"); }
		}
		private string _locCodeE;
		public string LocCodeE
		{
			get { return _locCodeE; }
			set { _locCodeE = value; RaisePropertyChanged("LocCodeE"); }
		}
		#endregion
		#region Form - 品號
		private string _searchItemCode;
		public string SearchItemCode
		{
			get { return _searchItemCode; }
			set
			{
				_searchItemCode = value;
				RaisePropertyChanged("SearchItemCode");
			}
		}
		#endregion
		#region Form - 品名
		private string _searchItemName;
		public string SearchItemName
		{
			get { return _searchItemName; }
			set
			{
				_searchItemName = value;
				RaisePropertyChanged("SearchItemName");
			}
		}
        #endregion
        #region 品號
        //品號
        private string _itemCode = string.Empty;

        public string ItemCode
        {
            get { return _itemCode; }
            set
            {
                _itemCode = value;
                RaisePropertyChanged("ItemCode");
            }
        }
		#endregion


		#region 批號
		private string _searchMakeNo;

		public string SearchMakeNo
		{
			get { return _searchMakeNo; }
			set
			{
				Set(() => SearchMakeNo, ref _searchMakeNo, value);
			}
		}
		#endregion



		#region Data - 調撥商品明細List
		private List<SelectionItem<F1913WithF1912Moved>> _dgItemSource;
		public List<SelectionItem<F1913WithF1912Moved>> DgItemSource
		{
			get { return _dgItemSource; }
			set
			{
				_dgItemSource = value;
				RaisePropertyChanged("DgItemSource");
			}
		}

		private bool _isSelectedAll = false;
		public bool IsSelectedAll
		{
			get { return _isSelectedAll; }
			set { _isSelectedAll = value; RaisePropertyChanged("IsSelectedAll"); }
		}
		#endregion
		#endregion
		#region ReturnData - 確認帶出資料
		private List<F151001DetailDatas> _returnData;
		public List<F151001DetailDatas> ReturnData
		{
			get { return _returnData; }
			set
			{
				_returnData = value;
				RaisePropertyChanged("ReturnData");
			}
		}
		#endregion
		#endregion

		#region 函式
		public P1502010200_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料

			}
		}

		public List<F1913WithF1912Moved> GetAddItemGroup()
		{
			if (DgItemSource != null)
			{
				var result = (from i in DgItemSource
							  where i.IsSelected
							  select i.Item).ToList();
				return result;
			}
			return new List<F1913WithF1912Moved>();
		}


        public void AppendItemCode(F1903 f1903)
        {
            if (!string.IsNullOrWhiteSpace(f1903.ITEM_CODE))
                SearchItemCode = f1903.ITEM_CODE;
            if (!string.IsNullOrWhiteSpace(f1903.ITEM_NAME))
                SearchItemName = f1903.ITEM_NAME;
        }

        private IEnumerable<string> GetSplitContent(string text)
        {
            return text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).Distinct();
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
			if (DgItemSource == null) return;
			foreach (var p in DgItemSource)
				p.IsSelected = IsSelectedAll;
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
			if (string.IsNullOrWhiteSpace(LocCodeS) && string.IsNullOrWhiteSpace(LocCodeE) && string.IsNullOrWhiteSpace(SearchItemCode) && string.IsNullOrWhiteSpace(SearchItemName) && string.IsNullOrWhiteSpace(SearchMakeNo))
				ShowWarningMessage(Properties.Resources.P1502010200_QueryConditionEmpty);
			else if ((!string.IsNullOrWhiteSpace(LocCodeS) && string.IsNullOrWhiteSpace(LocCodeE)) || (string.IsNullOrWhiteSpace(LocCodeS) && !string.IsNullOrWhiteSpace(LocCodeE)))
				ShowWarningMessage(Properties.Resources.P1502010200_LocCodeStartEndEmpty);
			else if (SourceData == null)
				ShowWarningMessage(Properties.Resources.P1502010200_SRCDataError);
			else
			{
				DgItemSource = new List<SelectionItem<F1913WithF1912Moved>>();
				var proxyEx = GetExProxy<P15ExDataSource>();
				var list = proxyEx.CreateQuery<F1913WithF1912Moved>("GetF1913WithF1912Moveds")
					.AddQueryExOption("dcCode", SourceData.SRC_DC_CODE)
					.AddQueryExOption("gupCode",SourceData.GUP_CODE)
					.AddQueryExOption("custCode",SourceData.CUST_CODE)
					.AddQueryExOption("srcLocCodeS",LocCodeHelper.LocCodeConverter9(LocCodeS))
					.AddQueryExOption("srcLocCodeE", LocCodeHelper.LocCodeConverter9(LocCodeE))
					.AddQueryExOption("itemCode",SearchItemCode)
					.AddQueryExOption("itemName",SearchItemName)
					.AddQueryExOption("srcWarehouseId",SourceData.SRC_WAREHOUSE_ID)
					.AddQueryExOption("isExpendDate",SourceData.ISEXPENDDATE)
					.AddQueryExOption("makeNoList",SearchMakeNo)
					.ToList();
                if (list.Any())
                    DgItemSource = list.ToSelectionList().ToList();
                else
                {
                    // 查無資料需清除查詢條件
                    LocCodeS = string.Empty;
                    LocCodeE = string.Empty;
                    SearchItemCode = string.Empty;
                    SearchItemName = string.Empty;
					          SearchMakeNo = string.Empty;
                    ShowMessage(Messages.InfoNoData);
                }
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

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
										o => DoDelete(), () => UserOperateMode == OperateMode.Query && DgItemSource != null && DgItemSource.Any()
					);
			}
		}

		private void DoDelete()
		{
			// 確認是否要刪除
			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;
			var tmpAddItemList = DgItemSource.Where(x => !x.IsSelected).ToList();
			DgItemSource = tmpAddItemList;
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
										o => DoSave(), () => DgItemSource != null && GetAddItemGroup().Any(), c => SaveComplete()
					);
			}
		}

		private bool _isSaveOk;
		
		private void DoSave()
		{
      string errmsg = "";
      _isSaveOk = true;
			var list = new List<F1913WithF1912Moved>();
      var addDetailList = GetAddItemGroup();
			if(!addDetailList.Any())
			{
				return;
			}
			else
			{
				foreach(var item in addDetailList)
				{
          list.Add(new F1913WithF1912Moved
          {
            ITEM_CODE = item.ITEM_CODE,
            LOC_CODE = item.LOC_CODE,
            MAKE_NO = item.MAKE_NO,
            MOVE_QTY = item.MOVE_QTY,
            VALID_DATE = item.VALID_DATE,
            WAREHOUSE_ID = item.WAREHOUSE_ID,
            WAREHOUSE_NAME = item.WAREHOUSE_NAME,
            ITEM_COLOR = item.ITEM_COLOR,
            ITEM_NAME = item.ITEM_NAME,
            ITEM_SIZE = item.ITEM_SIZE,
            ITEM_SPEC = item.ITEM_SPEC,
            ENTER_DATE = item.ENTER_DATE,
            SERIAL_NO = item.SERIAL_NO
					});
				}
				if (ReturnData!=null && ReturnData.Any())
				{
					foreach (var item in ReturnData)
					{
						var findItem = list.FirstOrDefault(o => o.LOC_CODE == item.SRC_LOC_CODE && o.ITEM_CODE == item.ITEM_CODE &&
									o.VALID_DATE == item.VALID_DATE && o.ENTER_DATE == item.ENTER_DATE && o.MAKE_NO == item.MAKE_NO);
						if (findItem != null)
						{
							findItem.MOVE_QTY = item.SRC_QTY + findItem.MOVE_QTY > findItem.QTY ? findItem.QTY : item.SRC_QTY + findItem.MOVE_QTY;
						}
						else
							list.Add(new F1913WithF1912Moved
							{
								ITEM_CODE = item.ITEM_CODE,
								LOC_CODE = item.SRC_LOC_CODE,
								MAKE_NO = item.MAKE_NO,
								MOVE_QTY = item.SRC_QTY,
								VALID_DATE = item.VALID_DATE,
								WAREHOUSE_ID = item.SRC_WAREHOUSE_ID,
								WAREHOUSE_NAME = item.SRC_WAREHOUSE_NAME,
								ITEM_COLOR = item.ITEM_COLOR,
								ITEM_NAME = item.ITEM_NAME,
								ITEM_SIZE = item.ITEM_SIZE,
								ITEM_SPEC = item.ITEM_SPEC,
								ENTER_DATE = item.ENTER_DATE,
                SERIAL_NO = item.SerialNo,
              });
					}
				}
				
			}
			ReturnData = new List<F151001DetailDatas>();
			var proxy = GetWcfProxy<P15Wcf.P15WcfServiceClient>();

			var isNoMoveQtyDatas = list.Where(x => x.MOVE_QTY == null || x.MOVE_QTY == 0 || x.MOVE_QTY > x.QTY).ToList();
			if(isNoMoveQtyDatas.Any())
			{
				_isSaveOk = false;
				errmsg = Properties.Resources.P1502010200_TransferDataIsNullOrEmpty;
			}
			else
			{
				var master = ExDataMapper.Map<F151001, P15Wcf.F151001>(SourceData);
				var data = ExDataMapper.MapCollection<F1913WithF1912Moved, P15Wcf.F1913WithF1912Moved>(list).ToArray();

				var res = proxy.RunWcfMethod(x => x.GetSuggestLocByF1913WithF1912MovedList(master, data));
				if(res.Result.IsSuccessed)
				{
					var data2 = ExDataMapper.MapCollection<P15Wcf.F151001DetailDatas, F151001DetailDatas>(res.F151001DetailDatas).ToList();

					ReturnData = data2;
				}
				else
				{
					_isSaveOk = false;
					errmsg = res.Result.Message;
				}
			}
			if (!_isSaveOk)
			{
				ShowWarningMessage(errmsg);
				ReturnData = null;
			}
		}

		private void SaveComplete()
		{
			if (_isSaveOk)
				Closed();
		}

		#endregion Save

		#region Paste

		public ICommand PasteCommand
		{
			get
			{
				return new RelayCommand(
					() =>
					{
						IsBusy = true;
						try
						{
							DoPaste();
						}
						catch (Exception ex)
						{
							Exception = ex;
							IsBusy = false;
						}
						IsBusy = false;
					},
				() => !IsBusy);
			}
		}

		private void DoPaste()
		{
			if (Clipboard.ContainsData(DataFormats.Text))
			{
				var pastData = Clipboard.GetDataObject();
				if (pastData != null && pastData.GetDataPresent(DataFormats.Text))
				{
					var content = pastData.GetData(DataFormats.Text).ToString();
					var arr = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
					SearchMakeNo = string.Join(",", arr.Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p)));
				}
			}
		}

		#endregion Paste
		#endregion
	}
}
