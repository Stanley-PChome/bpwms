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
using Wms3pl.WpfClient.ExDataServices.P06ExDataService;
using Wms3pl.WpfClient.UILib;

using System.Collections.ObjectModel;
using System.Reflection;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F15DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P06WcfService;

namespace Wms3pl.WpfClient.P06.ViewModel
{
	public partial class P0601010000_ViewModel : InputViewModelBase
	{
		#region 共用變數/資料連結/頁面參數
		private string _userId;
		private string _userName;

		#region Form - 物流中心
		private List<NameValuePair<string>> _dcCodes;

		public List<NameValuePair<string>> DcCodes
		{
			get { return _dcCodes; }
			set
			{
				_dcCodes = value;
				RaisePropertyChanged("DcCodes");
			}
		}

		private string _selectDcCode;
		public string SelectDcCode
		{
			get { return _selectDcCode; }
			set
			{
				_selectDcCode = value;
				ClearControl(1);
				RaisePropertyChanged("SelectDcCode");
			}
		}
		#endregion
		#region Data - 資料List
		private SelectionList<F0513WithF050801Batch> _dgList;
		public SelectionList<F0513WithF050801Batch> DgList
		{
			get { return _dgList; }
			set
			{
				_dgList = value;
				RaisePropertyChanged("DgList");
			}
		}

		private bool _isSelectedAllDL = false;
		public bool IsSelectedAllDL
		{
			get { return _isSelectedAllDL; }
			set { _isSelectedAllDL = value; RaisePropertyChanged("IsSelectedAllDL"); }
		}

		private SelectionItem<F0513WithF050801Batch> _selectedData;

		public SelectionItem<F0513WithF050801Batch> SelectedData
		{
			get { return _selectedData; }
			set
			{
				_selectedData = value;
				ClearControl(2);
				if (_selectedData != null)
				{
					TxtDELV_DATE = SelectedData.Item.DELV_DATE.ToString("yyyy/MM/dd");
					TxtPICK_TIME = SelectedData.Item.PICK_TIME;
					TxtCUST_ORD_NO = "";
					DoSearchCustOrdNo();
				}
				RaisePropertyChanged("SelectedData");
			}
		}

		private F0513WithF050801Batch _memItem;
		public F0513WithF050801Batch MemItem
		{
			get { return _memItem; }
			set
			{
				_memItem = value;
				RaisePropertyChanged("MemItem");
			}
		}
		#endregion

		#region Detail Data - 批次明細
		private string _txtDELV_DATE;
		/// <summary>
		/// 批次日期
		/// </summary>
		public string TxtDELV_DATE
		{
			get { return _txtDELV_DATE; }
			set
			{
				_txtDELV_DATE = value;
				RaisePropertyChanged("TxtDELV_DATE");
			}
		}

		private string _txtPICK_TIME;
		/// <summary>
		/// 批次時段
		/// </summary>
		public string TxtPICK_TIME
		{
			get { return _txtPICK_TIME; }
			set
			{
				_txtPICK_TIME = value;
				RaisePropertyChanged("TxtPICK_TIME");
			}
		}




		private string _txtCUST_ORD_NO;
		/// <summary>
		/// 貨主單號
		/// </summary>
		public string TxtCUST_ORD_NO
		{
			get { return _txtCUST_ORD_NO; }
			set
			{
				_txtCUST_ORD_NO = value;
				RaisePropertyChanged("TxtCUST_ORD_NO");
			}
		}
		#endregion

		#region Detail Data - 明細資料List
		private SelectionList<F050801WmsOrdNo> _dgItemList;
		public SelectionList<F050801WmsOrdNo> DgItemList
		{
			get { return _dgItemList; }
			set
			{
				_dgItemList = value;
				RaisePropertyChanged("DgItemList");
			}
		}

		private bool _isSelectedAllDI = false;
		public bool IsSelectedAllDI
		{
			get { return _isSelectedAllDI; }
			set { _isSelectedAllDI = value; RaisePropertyChanged("IsSelectedAllDI"); }
		}

		private SelectionItem<F050801WmsOrdNo> _selectedItemData;

		public SelectionItem<F050801WmsOrdNo> SelectedItemData
		{
			get { return _selectedItemData; }
			set
			{
				_selectedItemData = value;
				RaisePropertyChanged("SelectedItemData");
			}
		}
		#endregion
		#endregion

		#region 函式
		public P0601010000_ViewModel()
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
			GetDcCodeList();
		}

		private void ClearControl(int clearType)
		{
			if (clearType == 1)
				DgList = new SelectionList<F0513WithF050801Batch>(new List<F0513WithF050801Batch>());
			TxtDELV_DATE = "";
			TxtPICK_TIME = "";
			TxtCUST_ORD_NO = "";
			DgItemList = null;
		}

		private void GetDcCodeList()
		{
			DcCodes = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcCodes.Any()) SelectDcCode = DcCodes.FirstOrDefault().Value;
		}


		public List<F0513WithF050801Batch> GetDgListIsCheck()
		{
			var sel = DgList.Where(x => x.IsSelected == true).ToList();
			var result = (from i in sel
						  select i.Item).ToList();
			return result;
		}

		public List<F050801WmsOrdNo> GetDgItemListIsCheck()
		{
			var sel = DgItemList.Where(x => x.IsSelected == true).ToList();
			var result = (from i in sel
						  select i.Item).ToList();
			return result;
		}
		#endregion

		#region Command
		#region CheckAll
		public ICommand CheckAllCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCheckAllItem(),
					() => DgList != null
				);
			}
		}

		public void DoCheckAllItem()
		{
			foreach (var p in DgList)
			{
				p.IsSelected = IsSelectedAllDL;
			}
		}

		public ICommand CheckAllItemCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCheckAllDetailItem(),
					() => DgItemList != null
				);
			}
		}

		public void DoCheckAllDetailItem()
		{
			foreach (var p in DgItemList)
			{
				p.IsSelected = IsSelectedAllDI;
			}
		}
		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch()
					);
			}
		}

		private void DoSearch(bool isShowMsg = true,bool notOrder = false ,bool isB2c = false)
		{
			DgList = null;
			DgItemList = null;
			//執行查詢動作
			var gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			var custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			var proxyEx = GetExProxy<P05ExDataSource>();
			var qry = proxyEx.CreateQuery<F0513WithF050801Batch>("GetBatchDebitDatas")
										.AddQueryExOption("dcCode", SelectDcCode)
										.AddQueryExOption("gupCode", gupCode)
										.AddQueryExOption("custCode", custCode)
										.AddQueryExOption("notOrder", notOrder)
										.AddQueryExOption("isB2c", isB2c)
										.ToList();
			if (qry.Any())
				DgList = qry.ToSelectionList();
			else
			{
				if (isShowMsg)
					ShowMessage(Messages.InfoNoData);
				DgList = new SelectionList<F0513WithF050801Batch>(new List<F0513WithF050801Batch>());
			}
		}
		#endregion Search
		public ICommand SearchB2CCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(true,false,true)
					);
			}
		}

		public ICommand SearchNotOrderCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(true, true, false)
					);
			}
		}

		#region SearchCustOrdNo
		public ICommand SearchCustOrdNoCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearchCustOrdNo(),
					() => SelectedData != null
					);
			}
		}

		public void DoSearchCustOrdNo()
		{
			if (SelectedData != null)
			{
				var proxy = GetExProxy<P06ExDataSource>();
				var query = proxy.CreateQuery<F050801WmsOrdNo>("GetF050801ByDelvPickTime")
					.AddQueryExOption("dcCode", SelectedData.Item.DC_CODE)
					.AddQueryExOption("gupCode", SelectedData.Item.GUP_CODE)
					.AddQueryExOption("custCode", SelectedData.Item.CUST_CODE)
					.AddQueryExOption("delvDate", SelectedData.Item.DELV_DATE.ToString("yyyy/MM/dd"))
					.AddQueryExOption("pickTime", SelectedData.Item.PICK_TIME)
					.AddQueryExOption("custOrdNo", string.IsNullOrWhiteSpace(TxtCUST_ORD_NO) ? "" : TxtCUST_ORD_NO);

				var list = query.ToSelectionList(true); // 預設全選
				DgItemList = list;
			}
		}
		#endregion Search


		#region ConfirmPartialDebitCommand

		public ICommand ConfirmPartialDebitCommand
		{
			get
			{
				bool isSucceed = false;
				return CreateBusyAsyncCommand(
						o => isSucceed = DoConfirmBatchDebit(DgItemList.Where(si => si.IsSelected).Select(si => si.Item.WMS_ORD_NO).ToArray()),
						() => DgItemList != null && DgItemList.Any(si => si.IsSelected) && SelectedData != null,
						o => DoConfirmBatchDebitCompleted(isSucceed)
						);
			}
		}


		private void DoConfirmBatchDebitCompleted(bool isSucceed)
		{
			if (isSucceed && _lastSelectedF0513WithF050801Batch != null && DgList != null)
			{
				SelectedData = DgList.FirstOrDefault(si => si.Item.DELV_DATE == _lastSelectedF0513WithF050801Batch.DELV_DATE
																								&& si.Item.PICK_TIME == _lastSelectedF0513WithF050801Batch.PICK_TIME);

			}
		}

		#endregion

		#region ConfirmBatchDebitCommand
		private F0513WithF050801Batch _lastSelectedF0513WithF050801Batch = null;
		public ICommand ConfirmBatchDebitCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoConfirmBatchDebit(),
						() => DgList != null && DgList.Any(si => si.IsSelected)
						);
			}
		}

		private bool DoConfirmBatchDebit(string[] wmsOrdNos = null)
		{
			// 是否只針對部分出貨單做扣帳，或者針對勾選批次日期與批次時段來扣帳
			var f0513s = (wmsOrdNos != null && wmsOrdNos.Any())
								 ? new wcf.F0513[] { ExDataMapper.Map<F0513WithF050801Batch, wcf.F0513>(SelectedData.Item) }
								 : ExDataMapper.MapCollection<F0513WithF050801Batch, wcf.F0513>(DgList.Where(si => si.IsSelected).Select(si => si.Item)).ToArray();

			var proxyWcf = new wcf.P06WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
																									() => proxyWcf.ConfirmBatchDebit(f0513s, wmsOrdNos));

			ShowResultMessage(result);

			if (result.IsSuccessed)
			{
				if (SelectedData != null)
					_lastSelectedF0513WithF050801Batch = ExDataMapper.Clone(SelectedData.Item);

				DoSearch(isShowMsg: false);
			}

			return result.IsSuccessed;
		}

		#endregion
		#endregion
	}
}
