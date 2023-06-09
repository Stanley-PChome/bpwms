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
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P05WcfService;
namespace Wms3pl.WpfClient.P08.ViewModel
{
	public partial class P0808010000_ViewModel : InputViewModelBase
	{
		#region 共用變數/資料連結/頁面參數
		private string _userId;
		private string _userName;
		private bool _isInit = true;

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
				RaisePropertyChanged("SelectDcCode");
                GetPickTimeList();
            }
		}
		#endregion
		#region Form - 批次日期
		private DateTime? _delvDate = DateTime.Today;
		public DateTime? DelvDate
		{
			get { return _delvDate; }
			set { _delvDate = value; RaisePropertyChanged("DelvDate"); GetPickTimeList(); }
		}
		#endregion
		#region Form - 批次時段
		private List<NameValuePair<string>> _pickTimeList;
		public List<NameValuePair<string>> PickTimeList
		{
			get { return _pickTimeList; }
			set { _pickTimeList = value; RaisePropertyChanged("PickTimeList"); }
		}

		private string _selectedPickTime;

		public string SelectedPickTime
		{
			get { return _selectedPickTime; }
			set { _selectedPickTime = value; RaisePropertyChanged("SelectedPickTime"); }
		}
		#endregion
		#region Form - 出貨單號
		private string _selectWMS_ORD_NO;
		public string SelectWMS_ORD_NO
		{
			get { return _selectWMS_ORD_NO; }
			set
			{
				_selectWMS_ORD_NO = value;
				RaisePropertyChanged("SelectWMS_ORD_NO");
			}
		}
		#endregion
		#region Form - 託運單號
		private string _selectPAST_NO;
		public string SelectPAST_NO
		{
			get { return _selectPAST_NO; }
			set
			{
				_selectPAST_NO = value;
				RaisePropertyChanged("SelectPAST_NO");
			}
		}
		#endregion
		#region Form - 商品品號
		private string _selectITEM_CODE;
		public string SelectITEM_CODE
		{
			get { return _selectITEM_CODE; }
			set
			{
				_selectITEM_CODE = value;
				RaisePropertyChanged("SelectITEM_CODE");
			}
		}
		#endregion
		#region Form - 訂單編號
		private string _searchOrdNo = string.Empty;

		public string SearchOrdNo
		{
			get { return _searchOrdNo; }
			set
			{
				_searchOrdNo = value;
				RaisePropertyChanged("SearchOrdNo");
			}
		}

		#endregion
		#region Data - 資料List
		private ObservableCollection<SelectionItem<F050801WithF055001>> _dgList;
		public ObservableCollection<SelectionItem<F050801WithF055001>> DgList
		{
			get { return _dgList; }
			set
			{
				_dgList = value;
				RaisePropertyChanged("DgList");
			}
		}

		private List<F050801WithF055001> _selectedData;

		public List<F050801WithF055001> SelectedData
		{
			get { return _selectedData; }
			set { _selectedData = value; RaisePropertyChanged("SelectedData"); }
		}
		#endregion
		#region Form - 勾選所有
		private bool _isCheckAll = false;
		public bool IsCheckAll
		{
			get { return _isCheckAll; }
			set { _isCheckAll = value; RaisePropertyChanged("IsCheckAll"); }
		}
		#endregion
		#endregion

		#region 函式
		public P0808010000_ViewModel()
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
			GetDcCodes();
			GetPickTimeList();
		}

		private void GetDcCodes()
		{
			DcCodes = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			if (DcCodes.Any())
				SelectDcCode = DcCodes.First().Value;
		}

		private void GetPickTimeList()
		{
			if (DelvDate != null)
			{
				var gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				var custCode = Wms3plSession.Get<GlobalInfo>().CustCode;

				var proxy = GetExProxy<Wms3pl.WpfClient.ExDataServices.P08ExDataService.P08ExDataSource>();
				var query = proxy.CreateQuery<string>("GetPickTimeSeparator")
									.AddQueryExOption("dcCode", SelectDcCode)
									.AddQueryExOption("gupCode", gupCode)
									.AddQueryExOption("custCode", custCode)
									.AddQueryExOption("delvDate", DelvDate);

				PickTimeList = query.ToList()
									.First()
									.Split(',')
									.Select(pickTime => new NameValuePair<string>(pickTime, pickTime))
									.ToList();

				SelectedPickTime = PickTimeList.Select(x => x.Value).FirstOrDefault();
			}
		}

		private List<F050801WithF055001> GetSelectedDataList()
		{
			var sel = DgList.Where(x => x.IsSelected == true).ToList();
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
					o => DoCheckAllItem()
				);
			}
		}

		public void DoCheckAllItem()
		{
			foreach (var p in DgList.Where(x => x.Item.ISMERGE == 0))
			{
				p.IsSelected = IsCheckAll;
			}
		}
		#endregion

		private RelayCommand<SelectionItem<F050801WithF055001>> _isSelectedCommand;

		/// <summary>
		/// Gets the IsSelectedCommand.
		/// </summary>
		public RelayCommand<SelectionItem<F050801WithF055001>> IsSelectedCommand
		{
			get
			{
				return _isSelectedCommand
					?? (_isSelectedCommand = new RelayCommand<SelectionItem<F050801WithF055001>>(
					p =>
					{
						if (!IsSelectedCommand.CanExecute(p))
						{
							return;
						}


					},
					p => p.Item.ISMERGE == 0));
			}
		}

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query && PickTimeList != null, o => SearchComplate()
					);
			}
		}

		private void DoSearch()
		{
			if (DelvDate != null)
			{
				SearchOrdNo = SearchOrdNo.Trim();

				//執行查詢動作
				var gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				var custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				var proxyEx = GetExProxy<P05ExDataSource>();
				var qry = proxyEx.CreateQuery<F050801WithF055001>("GetF050801WithF055001Datas")
								.AddQueryExOption("dcCode", SelectDcCode)
								.AddQueryExOption("gupCode", gupCode)
								.AddQueryExOption("custCode", custCode)
								.AddQueryExOption("delvDate", DelvDate)
								.AddQueryExOption("pickTime", SelectedPickTime)
								.AddQueryExOption("wmsOrdNo", (string.IsNullOrEmpty(SelectWMS_ORD_NO) ? "" : SelectWMS_ORD_NO))
								.AddQueryExOption("pastNo", (string.IsNullOrEmpty(SelectPAST_NO) ? "" : SelectPAST_NO))
								.AddQueryExOption("itemCode", (string.IsNullOrEmpty(SelectITEM_CODE) ? "" : SelectITEM_CODE))
								.AddQueryExOption("ordNo", SearchOrdNo);
				var list = qry.ToList();
				DgList = list.ToSelectionList();
			}
		}

		private void SearchComplate()
		{
			IsCheckAll = false;
		}
		#endregion Search

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel()
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
					o => DoSave(),
					() => DgList != null
						&& DgList.Where(p => p.IsSelected == true).Any()
						&& !DgList.Where(p => p.IsSelected && p.Item.ISMERGE > 0).Any()	// 如果併單的就不能設設定不出貨
					, o => SaveComplate()
					);
			}
		}

		private void DoSave()
		{
			if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes) return;
			var gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			var custCode = Wms3plSession.Get<GlobalInfo>().CustCode;

			// 設定不出貨，改成用訂單編號判斷
			var ordNoList = DgList.Where(x => x.IsSelected).Select(si => si.Item.ORD_NO).Distinct().ToArray();
			var wmsOrdNoList = DgList.Where(x => x.IsSelected).Select(si => si.Item.WMS_ORD_NO).Distinct().ToArray();

			var proxy = new wcf.P05WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
														() => proxy.UpdateStatusCancelByWmsOrdNo(SelectDcCode, gupCode, custCode, ordNoList, wmsOrdNoList));

			ShowResultMessage(result);
			if (result.IsSuccessed)
			{
				DoSearch();
			}
			//執行確認儲存動作
			//var proxy = GetModifyQueryProxy<F05Entities>();
			//SelectedData = GetSelectedDataList();
			//List<string> chkItem=new List<string>();
			//foreach(var item in SelectedData)
			//{
			//	chkItem.Add(item.CUST_ORD_NO);
			//}

			//var f050801s = proxy.F050801s.Where(x => x.CUST_CODE == custCode && x.GUP_CODE == gupCode && x.DC_CODE == SelectDcCode)
			//										.AsQueryable().ToList();

			//if (f050801s == null || f050801s.Count == 0)
			//{
			//	// 資料已刪除
			//	ShowMessage(Messages.WarningBeenDeleted);
			//	return;
			//}

			//foreach (var item in f050801s)
			//{
			//	if (chkItem.Contains(item.CUST_ORD_NO))
			//	{
			//		item.STATUS = 9;
			//		item.UPD_STAFF = _userId;
			//		item.UPD_NAME = _userName;
			//		item.UPD_DATE = DateTime.Now;
			//		proxy.UpdateObject(item);
			//		proxy.SaveChanges();
			//	}
			//}
			//ShowMessage(Messages.InfoUpdateSuccess);
		}

		private void SaveComplate()
		{
			//UserOperateMode = OperateMode.Query;
			IsCheckAll = false;
		}
		#endregion Save
		#endregion
	}
}
