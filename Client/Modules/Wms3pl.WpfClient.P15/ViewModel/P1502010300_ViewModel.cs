using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using wcf = Wms3pl.WpfClient.ExDataServices.P15WcfService;
namespace Wms3pl.WpfClient.P15.ViewModel
{
	public partial class P1502010300_ViewModel : InputViewModelBase
	{
		public P1502010300_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				UserOperateMode = OperateMode.Edit;
			}

		}

		#region Property
		private  string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		private  string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }

		public Action ClosedWindow = delegate { };
		public Action OpenFileDialog = delegate { };
		public Action ScrollIntoView = delegate { };
		public Action SetDefaultFocus = delegate { };
		public List<string> ImportSerialList { get; set; }

		public Visibility IsShowQty { get { return MustScanSerialNoQty > 0 ? Visibility.Visible : Visibility.Collapsed; } }

		#region 物流中心
		private string _dcCode;

		public string DcCode
		{
			get { return _dcCode; }
			set
			{
				if (_dcCode == value)
					return;
				Set(() => DcCode, ref _dcCode, value);
			}
		}
		#endregion

		#region Grid選取的序號
		private SerialNoResult _selectedSerialNoResult;

		public SerialNoResult SelectedSerialNoResult
		{
			get { return _selectedSerialNoResult; }
			set
			{
				if (_selectedSerialNoResult == value)
					return;
				Set(() => SelectedSerialNoResult, ref _selectedSerialNoResult, value);
			}
		}
		#endregion

		#region Grid 序號資料來源
		private List<SerialNoResult> _serialNoResultList;

		public List<SerialNoResult> SerialNoResultList
		{
			get { return _serialNoResultList; }
			set
			{
				if (_serialNoResultList == value)
					return;
				Set(() => SerialNoResultList, ref _serialNoResultList, value);
			}
		}
		#endregion

		#region 應刷數
		private int _mustScanSerialNoQty;

		public int MustScanSerialNoQty
		{
			get { return _mustScanSerialNoQty; }
			set
			{
				if (_mustScanSerialNoQty == value)
					return;
				Set(() => MustScanSerialNoQty, ref _mustScanSerialNoQty, value);
			}
		}
		#endregion

		#region 已刷數
		private int _scanSerialNoQty;

		public int ScanSerialNoQty
		{
			get { return _scanSerialNoQty; }
			set
			{
				if (_scanSerialNoQty == value)
					return;
				Set(() => ScanSerialNoQty, ref _scanSerialNoQty, value);
			}
		}
		#endregion

		#region 品號
		private string _itemCode;

		public string ItemCode
		{
			get { return _itemCode; }
			set
			{
				if (_itemCode == value)
					return;
				Set(() => ItemCode, ref _itemCode, value);
			}
		}
		#endregion

		#region 品名
		private string _itemName;

		public string ItemName
		{
			get { return _itemName; }
			set
			{
				if (_itemName == value)
					return;
				Set(() => ItemName, ref _itemName, value);
			}
		}
		#endregion

		#region 尺寸
		private string _itemSize;

		public string ItemSize
		{
			get { return _itemSize; }
			set
			{
				if (_itemSize == value)
					return;
				Set(() => ItemSize, ref _itemSize, value);
			}
		}
		#endregion

		#region 規格
		private string _itemSpec;

		public string ItemSpec
		{
			get { return _itemSpec; }
			set
			{
				if (_itemSpec == value)
					return;
				Set(() => ItemSpec, ref _itemSpec, value);
			}
		}
		#endregion

		#region 顏色
		private string _itemColor;

		public string ItemColor
		{
			get { return _itemColor; }
			set
			{
				if (_itemColor == value)
					return;
				Set(() => ItemColor, ref _itemColor, value);
			}
		}
		#endregion

		#region 刷讀序號
		private string _scanSerialNo;

		public string ScanSerialNo
		{
			get { return _scanSerialNo; }
			set
			{
				if (_scanSerialNo == value)
					return;
				Set(() => ScanSerialNo, ref _scanSerialNo, value);
			}
		}
		#endregion

		#region 變更後狀態
		private string _changeStatus;

		public string ChangeStatus
		{
			get { return _changeStatus; }
			set
			{
				if (_scanSerialNo == value)
					return;
				Set(() => ChangeStatus, ref _changeStatus, value);
			}
		}
		#endregion

		#region 單據編號
		private string _wmsNo;

		public string WmsNo
		{
			get { return _wmsNo; }
			set
			{
				if (_wmsNo == value)
					return;
				Set(() => WmsNo, ref _wmsNo, value);
			}
		}
		#endregion

		#region 廠商
		private string _vnrCode;

		public string VnrCode
		{
			get { return _vnrCode; }
			set
			{
				if (_vnrCode == value)
					return;
				Set(() => VnrCode, ref _vnrCode, value);
			}
		}
		#endregion

		#region 效期
		private DateTime? _validDate;

		public DateTime? ValidDate
		{
			get { return _validDate; }
			set
			{
				if (_validDate == value)
					return;
				Set(() => ValidDate, ref _validDate, value);
			}
		}
		#endregion
		
		

		#endregion

		#region 初始化資料

		public void Init()
		{
			var proxy = GetProxy<F19Entities>();
			var itemF1903 = proxy.F1903s.Where(o => o.GUP_CODE == _gupCode && o.ITEM_CODE == ItemCode && o.CUST_CODE == _custCode).FirstOrDefault();
			if (itemF1903 != null)
			{
				ItemName = itemF1903.ITEM_NAME;
				ItemSpec = itemF1903.ITEM_SPEC;
				ItemSize = itemF1903.ITEM_SIZE;
				ItemColor = itemF1903.ITEM_COLOR;
			}
		}

		#endregion

		#region ImportExcel
		public ICommand ImportExcelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoImportExcel(), () => UserOperateMode == OperateMode.Edit, c => DoImportExcelComplete(),null, OpenFileDialog
					);
			}
		}

		private void DoImportExcel()
		{
			SerialNoResultList = SerialNoResultList ?? new List<SerialNoResult>();
			//執行查詢動
			if (ImportSerialList != null && ImportSerialList.Any())
			{
				int tmpCount;
				if (!int.TryParse(ImportSerialList[0], out tmpCount))
				{
					ShowWarningMessage(Properties.Resources.P1502010300_FirstCountShouldBeTotalCount);
					return;
				}
				var tmp = new List<SerialNoResult>();
				var isOkCount = 0;
				foreach (var p in ImportSerialList.Skip(1).Distinct())
				{
					var result = DoCheckSerialNo(p.Trim());
					if (!result.First().Checked) continue;

					result = result.Where(o => !SerialNoResultList.Select(c => c.SerialNo).Contains(o.SerialNo)).ToList();
					if (result.Any())
					{
						tmp.AddRange(result);
						isOkCount++;
					}
				}
				if (MustScanSerialNoQty > 0 && tmp.Count + SerialNoResultList.Count > MustScanSerialNoQty)
				{
					ShowWarningMessage(string.Format(Properties.Resources.P1502010300_ImportCount,tmp.Count,SerialNoResultList.Count,MustScanSerialNoQty));
					return;
				}
				var msg = Messages.InfoSerialNotAllImported;
				msg.Message += Environment.NewLine +
											 string.Format(Properties.Resources.P1502010300_SuccessCount, isOkCount, tmpCount - isOkCount, tmpCount);
				if (isOkCount != tmpCount) ShowMessage(msg);
				SerialNoResultList.AddRange(tmp);
				SerialNoResultList = SerialNoResultList.ToList();
			}
		}

		private void DoImportExcelComplete()
		{
			if(ImportSerialList != null)
				ImportSerialList.Clear();
			DoRefreshReadCount();
		}
		#endregion ImportExcel

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode == OperateMode.Edit,c=>DoCancelComplete()
					);
			}
		}

		public void DoCancel()
		{
			
		}

		public void DoCancelComplete()
		{
			ClosedWindow();
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Edit && SerialNoResultList != null && SerialNoResultList.Any(),
					c => DoDeleteComplete()
					);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
		}

		private void DoDeleteComplete()
		{
			if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
			{
				var list = SerialNoResultList;
				var items = (!string.IsNullOrEmpty(SelectedSerialNoResult.BatchNo)) ? list.Where(o => o.BatchNo == SelectedSerialNoResult.BatchNo).ToList() : null;
				items = items ?? ((!string.IsNullOrEmpty(SelectedSerialNoResult.BoxSerail)) ? list.Where(o => o.BoxSerail == SelectedSerialNoResult.BoxSerail).ToList() : null);
				items = items ?? ((!string.IsNullOrEmpty(SelectedSerialNoResult.CaseNo)) ? list.Where(o => o.BoxSerail == SelectedSerialNoResult.CaseNo).ToList() : null);
				if (items == null)
					list.Remove(SelectedSerialNoResult);
				else
				{
					for (int i = items.Count - 1; i >= 0; i--)
						list.Remove(items[i]);
				}
				SerialNoResultList = list.ToList();
				DoRefreshReadCount();
			}
		}
		#endregion Delete

		#region Save

		private bool _isSaveSuccess;
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => UserOperateMode == OperateMode.Edit && SerialNoResultList != null && SerialNoResultList.Any(), c => DoSaveComplete()
					);
			}
		}

		private void DoSave()
		{
			_isSaveSuccess = false;
			//執行確認儲存動作
			var wcfSerialNoResult = ExDataMapper.MapCollection<SerialNoResult, wcf.SerialNoResult>(SerialNoResultList.ToArray()).ToArray();

			var proxy = new wcf.P15WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.ImportSerialNo(DcCode, _gupCode, _custCode, ItemCode, wcfSerialNoResult,ChangeStatus,WmsNo,VnrCode,ValidDate?? new DateTime(9999/12/31)));
			if (result.IsSuccessed)
				_isSaveSuccess = true;
			else
				ShowWarningMessage(result.Message);
		}

		private void DoSaveComplete()
		{
			if (_isSaveSuccess)
				ClosedWindow();
		}
		#endregion Save

		#region ScanSerialNo

		private bool _isScanSuccess;
		public ICommand ScanSerialNoCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>  DoScanSerialNo(), () => UserOperateMode == OperateMode.Edit, c => DoScanSerialNoComplete()
					);
			}
		}

		private void DoScanSerialNo()
		{
		
			SerialNoResultList = SerialNoResultList ?? new List<SerialNoResult>();
			_isScanSuccess = false;
			if (string.IsNullOrWhiteSpace(ScanSerialNo))
				ShowWarningMessage(Properties.Resources.P1502010300_SerialNoEmpty);
			else if (SerialNoResultList!=null && SerialNoResultList.Any(o => o.SerialNo.Contains(ScanSerialNo)))
				ShowWarningMessage(Properties.Resources.P1502010300_SerialNoExist);
			else
			{
				var tmp = SerialNoResultList?? new List<SerialNoResult>().ToList();
				var result = DoCheckSerialNo(ScanSerialNo);
				if (result.First().Checked)
				{
					result = result.Where(o => !tmp.Select(c => c.SerialNo).Contains(o.SerialNo)).ToList();

					if (MustScanSerialNoQty>0 && result.Count + tmp.Count > MustScanSerialNoQty)
					{
						ShowWarningMessage(string.Format(Properties.Resources.P1502010300_ImportCount, result.Count, tmp.Count, MustScanSerialNoQty));
						return;
					}

					tmp.AddRange(result);
					SerialNoResultList = tmp.ToList();
					_isScanSuccess = true;
				}
				else
					ShowWarningMessage(result.First().Message);
			}
		}

		private void DoScanSerialNoComplete()
		{
			if (_isScanSuccess)
				DoRefreshReadCount();
		}
		#endregion ScanSerialNo

		private void DoRefreshReadCount()
		{
			ScanSerialNoQty = SerialNoResultList.Count;
			if (SerialNoResultList != null && SerialNoResultList.Any())
				SelectedSerialNoResult = SerialNoResultList.Last();
			ScrollIntoView();
			SetDefaultFocus();
		}

		private List<SerialNoResult> DoCheckSerialNo(string sn)
		{
			var proxyEx = GetExProxy<ShareExDataSource>();
			var result = proxyEx.CreateQuery<SerialNoResult>("CheckSerialNoFull")
				.AddQueryExOption("dcCode", DcCode)
				.AddQueryExOption("gupCode",_gupCode)
				.AddQueryExOption("custCode", _custCode)
				.AddQueryExOption("itemCode",ItemCode)
				.AddQueryExOption("serialNo", sn)
				.AddQueryExOption("status", ChangeStatus).ToList();

			return result;
		}
	}
}
