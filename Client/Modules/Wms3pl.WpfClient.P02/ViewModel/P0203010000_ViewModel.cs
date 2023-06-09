using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.DataServices.F15DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;
using Wms3pl.WpfClient.P02.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0203010000_ViewModel : InputViewModelBase
	{
		public P0203010000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				_userId = Wms3plSession.CurrentUserInfo.Account;
				SetDcList();
				AllocationDate = DateTime.Today;
				RabQuery1 = true;
			}

		}

		#region Property

		public string GupCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().GupCode; }
		}

		public string CustCode
		{
			get { return Wms3plSession.Get<GlobalInfo>().CustCode; }
		}

		private readonly string _userId;
		public string NowAllocationNo { get; set; }
		public Action OpenSerialLocCheckClick = delegate { };

		#region 物流中心

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
				if (RabQuery2)
					SetAllocationNoTree();
				F1510Datas = null;
			}
		}

		#endregion

		#region 調撥單日期

		private DateTime _allocationDate;

		public DateTime AllocationDate
		{
			get { return _allocationDate; }
			set
			{
				_allocationDate = value;
				RaisePropertyChanged("AllocationDate");
				if (RabQuery2)
					SetAllocationNoTree();
				F1510Datas = null;
				SelectedF1510Data = null;
			}
		}

		#endregion

		#region 調撥單查詢方式 

		private bool _rabQuery1;

		public bool RabQuery1
		{
			get { return _rabQuery1; }
			set
			{
				_rabQuery1 = value;
				RaisePropertyChanged("RabQuery1");
				if (_rabQuery1)
				{
					F1510Datas = null;
					SelectedF1510Data = null;
				}
			}
		}

		private bool _rabQuery2;

		public bool RabQuery2
		{
			get { return _rabQuery2; }
			set
			{
				_rabQuery2 = value;
				RaisePropertyChanged("RabQuery2");
				SetAllocationNoTree();
				if (_rabQuery2)
				{
					AllocationNo = "";
					F1510Datas = null;
					SelectedF1510Data = null;
				}

			}
		}

		#endregion

		#region 調撥單編號

		private string _allocationNo;

		public string AllocationNo
		{
			get { return _allocationNo; }
			set
			{
				_allocationNo = value;
				RaisePropertyChanged("AllocationNo");
			}
		}

		#endregion

		#region 調撥單Tree List

		private List<DelvNo> _allocationNoList;

		public List<DelvNo> AllocationNoList
		{
			get { return _allocationNoList; }
			set
			{
				_allocationNoList = value;
				RaisePropertyChanged("AllocationNoList");
			}
		}

		private DelvNo _selectedAllocationNo;

		public DelvNo SelectedAllocationNo
		{
			get { return _selectedAllocationNo; }
			set
			{
				_selectedAllocationNo = value;
				RaisePropertyChanged("SelectedAllocationNo");
				F1510Datas = null;
				SelectedF1510Data = null;
			}
		}

		#endregion

		#region 調撥單Grid List

		private List<F1510Data> _f1510Datas;

		public List<F1510Data> F1510Datas
		{
			get { return _f1510Datas; }
			set
			{
				_f1510Datas = value;
				RaisePropertyChanged("F1510Datas");
			}
		}

		private F1510Data _selectedF1510Data;

		public F1510Data SelectedF1510Data
		{
			get { return _selectedF1510Data; }
			set
			{
				_selectedF1510Data = value;
				RaisePropertyChanged("SelectedF1510Data");
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

		#region TreeView 資料來源
		public void SetAllocationNoTree()
		{
			var list = new List<DelvNo>();
			if (RabQuery2)
			{
				var proxy = GetExProxy<P02ExDataSource>();
				var datas = proxy.CreateQuery<F151001WithF02020107>("GetDatasByTar")
					.AddQueryExOption("tarDcCode", SelectedDcCode)
					.AddQueryExOption("gupCode", GupCode)
					.AddQueryExOption("custCode", CustCode)
					.AddQueryExOption("allocationDate", AllocationDate)
					.AddQueryExOption("status", "0,1,3") //只抓取Status = 0(待處理) 或 1 (已列印調撥單)
					.ToList();

				list = (from o in datas
						orderby o.ALLOCATION_NO
						select new DelvNo
						{
							Id = o.ALLOCATION_NO,
							Name = string.IsNullOrEmpty(o.SOURCE_NO) ? o.ALLOCATION_NO : string.Format("{0} ({1})", o.ALLOCATION_NO, o.RT_NO)
						}).ToList();
			}
			var service = new DelvNoService();
			var treeList = service.MakeTree(list.ToList()).ToList();
			treeList.First().Name = Properties.Resources.P0203010000_TreeListFirstName;
			AllocationNoList = treeList;
		}

		#endregion

		#region 下拉式選單資料來源

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
            if (F1510Datas != null)
            {
                foreach (var p in F1510Datas)
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
					o => DoSearch(), () => UserOperateMode == OperateMode.Query, c => DoSearchComplete()
					);
			}
		}

		private bool _isSearchOk;
		private void DoSearch()
		{
			_isSearchOk = false;
			var message = new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Warning,
				Message = "",
				Title = Properties.Resources.Message
			};
			//執行查詢動
			if (RabQuery1 && string.IsNullOrEmpty(AllocationNo))
				message.Message = Properties.Resources.P0203010000_AllocationNoIsNull;
			else if (RabQuery2 && (SelectedAllocationNo == null || SelectedAllocationNo != null && SelectedAllocationNo.Parent == null))
				message.Message = Properties.Resources.P0203010000_SelectAllocationNo;

			if (message.Message.Length > 0)
				ShowMessage(message);
			else
			{
				NowAllocationNo = RabQuery1 ? AllocationNo : SelectedAllocationNo.Id;
				var proxyEx = GetExProxy<P02ExDataSource>();
				F1510Datas = proxyEx.CreateQuery<F1510Data>("GetF1510DatasByTar")
					.AddQueryExOption("dcCode", _selectedDcCode)
					.AddQueryExOption("gupCode", GupCode)
					.AddQueryExOption("custCode", CustCode)
					.AddQueryExOption("allocationNo", NowAllocationNo)
					.AddQueryExOption("allocationDate", AllocationDate.ToString("yyyy/MM/dd")).ToList();
				_isSearchOk = true;
			}
		}

		private void DoSearchComplete()
		{
			if (_isSearchOk)
			{
				if (F1510Datas.Any())
					SelectedF1510Data = F1510Datas.First();
				else
					ShowMessage(Messages.InfoNoData);
			}
		}
		#endregion Search

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => UserOperateMode == OperateMode.Query && F1510Datas != null && F1510Datas.Any(o => o.IsSelected),
					c => DoSaveComplete()
					);
			}
		}
		private bool _isSaveOk;
		private void DoSave()
		{
			//執行確認儲存動作
			_isSaveOk = true;
			var message = new MessagesStruct
			{
				Button = DialogButton.OK,
				Image = DialogImage.Error,
				Message = "",
				Title = Properties.Resources.Message
			};
			UserOperateMode = OperateMode.Query;
			var f1510SelectedDatas = F1510Datas.Where(o => o.IsSelected);
			var f020201Datas = GetAllocationData();
			if (f1510SelectedDatas.Any(o => o.BUNDLE_SERIALLOC == "1" && o.CHECK_SERIALNO == "0" && f020201Datas.Any(n => n.ITEM_CODE == o.ITEM_CODE && string.IsNullOrEmpty(n.SPECIAL_CODE))))
			{
				message.Message = Properties.Resources.P0203010000_BundleSeriallocMessage;
			}
			else
			{
				var proxyWcf = new wcf.P02WcfServiceClient();
				var datas = from o in f1510SelectedDatas
							select new ExDataServices.P02WcfService.F1510Data
							{
								IsSelected = o.IsSelected,
								ALLOCATION_DATE = o.ALLOCATION_DATE,
								ALLOCATION_NO = o.ALLOCATION_NO,
								ITEM_CODE = o.ITEM_CODE,
								ITEM_NAME = o.ITEM_NAME,
								QTY = o.QTY,
								SRC_LOC_CODE = o.SRC_LOC_CODE,
								SUG_LOC_CODE = o.SUG_LOC_CODE,
								TAR_LOC_CODE = o.TAR_LOC_CODE.Replace("-",""),
								BUNDLE_SERIALLOC = o.BUNDLE_SERIALLOC,
								CHECK_SERIALNO = o.CHECK_SERIALNO,
								DC_CODE = o.DC_CODE,
								GUP_CODE = o.GUP_CODE,
								CUST_CODE = o.CUST_CODE,
								VALID_DATE = o.VALID_DATE
							};
				var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
					() => proxyWcf.UpdateF1510Data(_selectedDcCode, GupCode, CustCode, NowAllocationNo, datas.ToArray()));
				if (result.IsSuccessed)
					ShowMessage(Messages.InfoUpdateSuccess);
				else
				{
					_isSaveOk = false;
					var error = Messages.ErrorUpdateFailed;
					error.Message += Environment.NewLine + result.Message;
					ShowMessage(error);
				}
			}
			if (message.Message.Length > 0)
			{
				_isSaveOk = false;
				ShowMessage(message);
			}

		}
		private void DoSaveComplete()
		{
			if (_isSaveOk)
			{
				DoSearch();
				if (F1510Datas != null && F1510Datas.Any())
					DoSearchComplete();
				else
					SetAllocationNoTree();
			}
		}
		#endregion Save

		#region 開啟序號儲位刷讀
		public ICommand SerialLocCheckCommand
		{
			get
			{
				return new RelayCommand(
					 DoSerialLocCheck, () => UserOperateMode == OperateMode.Query && SelectedF1510Data != null && SelectedF1510Data.BUNDLE_SERIALLOC == "1"
					);
			}
		}

		private void DoSerialLocCheck()
		{
			var f020201Datas = GetAllocationData();
			if (f020201Datas.Any(n => n.ITEM_CODE == _selectedF1510Data.ITEM_CODE && n.SPECIAL_CODE == "201"))
			{
				ShowWarningMessage(Properties.Resources.P0203010000_SpecialCode201);
				return;
			}
			//開啟序號儲位刷讀視窗
			UserOperateMode = OperateMode.Add;
			OpenSerialLocCheckClick();

		}
		#endregion Search
		#endregion

		private IEnumerable<F020201> GetAllocationData()
		{
			var proxy = GetProxy<F02Entities>();
			return proxy.CreateQuery<F020201>("GetAllocationData")
				.AddQueryExOption("dcCode", _selectedDcCode)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.AddQueryExOption("allocationNo", NowAllocationNo).ToList();
		}
	}
}
