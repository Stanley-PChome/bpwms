using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.UILib;
using wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public class P0813010100_ViewModel : InputViewModelBase
	{
		public P0813010100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				GupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				CustCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				//初始化執行時所需的值及資料
			}

		}

		#region Property
		public Action SetTxtScanUpLocCodeFocus = delegate { };
		public Action SetBtnMoveComplete = delegate { };
		#region 物流中心
		private string _dcCode;

		public string DcCode
		{
			get { return _dcCode; }
			set
			{
				Set(() => DcCode, ref _dcCode, value);
			}
		}
		#endregion

		#region 業主
		private string _gupCode;

		public string GupCode
		{
			get { return _gupCode; }
			set
			{
				Set(() => GupCode, ref _gupCode, value);
			}
		}
		#endregion

		#region 貨主
		private string _custCode;

		public string CustCode
		{
			get { return _custCode; }
			set
			{
				Set(() => CustCode, ref _custCode, value);
			}
		}
		#endregion
		#region 是否儲位搬移(true:儲位搬移;false:商品搬移)
		private bool _isMoveLoc;

		public bool IsMoveLoc
		{
			get { return _isMoveLoc; }
			set
			{
				Set(() => IsMoveLoc, ref _isMoveLoc, value);
			}
		}
		#endregion

		#region 搬移商品
		private string _moveItemCode;

		public string MoveItemCode
		{
			get { return _moveItemCode; }
			set
			{
				Set(() => MoveItemCode, ref _moveItemCode, value);
			}
		}
		#endregion



		#region 搬移儲位資訊
		private P08130101MoveLoc _moveLoc;

		public P08130101MoveLoc MoveLoc
		{
			get { return _moveLoc; }
			set
			{
				Set(() => MoveLoc, ref _moveLoc, value);
			}
		}
		#endregion

		#region 搬移儲位
		private string _moveLocCode;

		public string MoveLocCode
		{
			get { return _moveLocCode; }
			set
			{
				Set(() => MoveLocCode, ref _moveLocCode, value);
			}
		}
		#endregion

		#region 上架倉別
		private string _upWarehouseName;

		public string UpWarehouseName
		{
			get { return _upWarehouseName; }
			set
			{
				Set(() => UpWarehouseName, ref _upWarehouseName, value);
			}
		}
		#endregion

		#region 訊息
		private string _message;

		public string Message
		{
			get { return _message; }
			set
			{
				Set(() => Message, ref _message, value);
			}
		}
		#endregion

		#region 訊息顏色
		private SolidColorBrush _messageForeground;

		public SolidColorBrush MessageForeground
		{
			get { return _messageForeground; }
			set
			{
				Set(() => MessageForeground, ref _messageForeground, value);
			}
		}
		#endregion

		#region 是否可上架
		private bool _isCanSave;

		public bool IsCanSave
		{
			get { return _isCanSave; }
			set
			{
				Set(() => IsCanSave, ref _isCanSave, value);
			}
		}
		#endregion


		#region 儲位庫存資料
		private SelectionList<P08130101Stock> _dgList;

		public SelectionList<P08130101Stock> DgList
		{
			get { return _dgList; }
			set
			{
				Set(() => DgList, ref _dgList, value);
			}
		}
		#endregion


		#region 選取的儲位庫存
		private SelectionItem<P08130101Stock> _selectedItem;

		public SelectionItem<P08130101Stock> SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				Set(() => SelectedItem, ref _selectedItem, value);
			}
		}
		#endregion

		#region 是否全選
		private bool _isCheckAll;

		public bool IsCheckAll
		{
			get { return _isCheckAll; }
			set
			{
				Set(() => IsCheckAll, ref _isCheckAll, value);
				CheckAllChange(value);
			}
		}
		#endregion


		#region 刷讀的上架儲位
		private string _scanUpLocCode;

		public string ScanUpLocCode
		{
			get { return _scanUpLocCode; }
			set
			{
				Set(() => ScanUpLocCode, ref _scanUpLocCode, value);
			}
		}
		#endregion


		#endregion

		#region Method
		/// <summary>
		/// 顯示訊息
		/// </summary>
		/// <param name="isOk">是否成功</param>
		/// <param name="message">訊息內容</param>
		private void ShowMessage(bool isOk,string message)
		{
			Message = message;
			MessageForeground = isOk ? Brushes.Blue : Brushes.Red;
		}
		private void CheckAllChange(bool isChecked)
		{
			if(DgList!=null)
			{
				foreach (var item in DgList)
				{
					item.IsSelected = isChecked;
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
					o => DoSearch(true), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoSearch(bool isShowMessage)
		{
			//顯示搬移儲位資訊
			var proxy = GetExProxy<P08ExDataSource>();
			MoveLoc = proxy.CreateQuery<P08130101MoveLoc>("GetP08130101MoveLocs")
				.AddQueryExOption("dcCode", DcCode)
				.AddQueryExOption("locCode", MoveLocCode).ToList().FirstOrDefault();
			if(MoveLoc == null)
			{
				ShowMessage(false, Properties.Resources.P0813010100_MoveLocNotExists);
				IsCanSave = false;
				return;
			}
			if(MoveLoc.NOW_STATUS_ID == "03" || MoveLoc.NOW_STATUS_ID == "04")
			{
				ShowMessage(false, string.Format(Properties.Resources.P0813010100_MoveLocCanotMove,MoveLoc.LOC_STATUS_NAME));
				IsCanSave = false;
				return;
			}

			//取得搬移儲位庫存
			var datas = proxy.CreateQuery<P08130101Stock>("GetP08130101Stocks")
				.AddQueryExOption("dcCode", DcCode)
				.AddQueryExOption("gupCode", GupCode)
				.AddQueryExOption("custCode", CustCode)
				.AddQueryExOption("locCode", MoveLocCode)
				.AddQueryExOption("itemCode",MoveItemCode).ToList();
			if (datas.Any())
			{
				DgList = datas.ToSelectionList();
				IsCheckAll = true;
				SelectedItem = DgList.First();
				if(isShowMessage && string.IsNullOrWhiteSpace(ScanUpLocCode))
					ShowMessage(true, Properties.Resources.P0813010100_PleaseScanUpLoc);
				IsCanSave = true;
			}
			else
			{
				DgList = null;
				if(isShowMessage)
				{
					if (IsMoveLoc)
						ShowMessage(false, Properties.Resources.P0813010100_MoveLocNoStock);
					else
						ShowMessage(false,string.Format(Properties.Resources.P0813010100_MoveLocItemNoStock,MoveItemCode));
				}
				IsCanSave = false;
			}


		}
		#endregion Search

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>  DoSave(), () => UserOperateMode == OperateMode.Query && IsCanSave
					);
			}
		}

		private void DoSave()
		{
		
			//執行確認儲存動作
			if (!DoCheckTarLoc())
				return;
			if(!DgList.Where(x=> x.IsSelected).Any())
			{
				ShowMessage(false, Properties.Resources.P08130100_PleaseCheckMoveItem);
				return;
			}
			if(DgList.Where(x=> x.IsSelected && x.Item.MOVE_QTY <=0).Any())
			{
				ShowMessage(false, Properties.Resources.P0813010100_MoveQtyNotEquelOrLessZero);
				return;
			}
			if (ShowConfirmMessage(Properties.Resources.P0813010100_ConfirmMoveComplete) == UILib.Services.DialogResponse.No)
				return;

			var moveStocks = ExDataMapper.MapCollection<P08130101Stock, wcf.P08130101Stock>(DgList.Where(x => x.IsSelected).Select(x => x.Item).ToList());
			var proxy = new wcf.P08WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
												() => proxy.P081301CreateAllocation(DcCode,GupCode,CustCode,ScanUpLocCode, moveStocks.ToArray()));
			if (result.IsSuccessed)
			{
				ShowMessage(true, Properties.Resources.P0813010100_MoveComplete);
				DispatcherAction(() => { SetTxtScanUpLocCodeFocus(); });
			}
			else
			{
				ShowMessage(false, result.Message.Replace("\n",""));
			}
			DoSearch(false);

		}
		#endregion Save


		#region CheckTarLoc
		/// <summary>
		/// Gets the CheckTarLoc.
		/// </summary>
		public ICommand CheckTarLocCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => DoCheckTarLoc(), () => UserOperateMode == OperateMode.Query
);
			}
		}

		public bool DoCheckTarLoc()
		{
      if (string.IsNullOrWhiteSpace(ScanUpLocCode))
      {
        ShowMessage(false, "請刷入上架儲位條碼");
        return false;
      }
      ScanUpLocCode = LocCodeHelper.LocCodeConverter9((ScanUpLocCode.ToUpper() ?? "").Trim());
			if (ScanUpLocCode == MoveLocCode)
			{
				UpWarehouseName = string.Empty;
				ShowMessage(false, Properties.Resources.P0813010100_UpLocCanotMoveLoc);
				DispatcherAction(() => { SetTxtScanUpLocCodeFocus(); });
				return false;
			}
			var proxy = new wcf.P08WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
												() => proxy.P081301CheckTarLocCode(DcCode, ScanUpLocCode));
			if (!result.IsSuccessed)
			{
				UpWarehouseName = string.Empty;
				ShowMessage(result.IsSuccessed, result.Message);
				DispatcherAction(() => { SetTxtScanUpLocCodeFocus(); });
			}
			else
			{
				UpWarehouseName = result.Message;
				ShowMessage(true, Properties.Resources.P0813010100_ScanUpLocCodeCorrect);
				DispatcherAction(() => { SetBtnMoveComplete(); });
			}
			return result.IsSuccessed;
		}
		#endregion CheckTarLoc

	}
}
