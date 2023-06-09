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
using System.Windows.Controls;
using Wms3pl.Common.Security;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F25DataService;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.DataServices.F05DataService;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.P08.Services;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.ExDataServices;
using Wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wcf2 = Wms3pl.WpfClient.ExDataServices.P02WcfService;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public partial class P0807010100_ViewModel : InputViewModelBase
	{
		public Action ActionAfterCheckSerialNo = delegate { };
		public Action ActionForRequireUnlock = delegate { };
		//public Action ActionBeforeImportData = delegate { };
		public Action OnSaveComplete = delegate { };
        public Action ExcelImport = delegate { };
        private string _userId = Wms3plSession.Get<UserInfo>().Account;
		public P0807010100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				UserOperateMode = OperateMode.Edit;
			}

		}

		#region 資料連結
		#region Form - 物流中心, 批次日期
		public string DC_NAME
		{
			get
			{
				if (BaseData == null) return string.Empty;
				var tmp = Wms3plSession.Get<GlobalInfo>().DcCodeList.FirstOrDefault(x => x.Value == BaseData.DC_CODE);
				return (tmp == null) ? string.Empty : tmp.Name;
			}
		}
        #endregion

        #region 匯入檔案路徑參數

        private string _importFilePath;

        public string ImportFilePath
        {
            get { return _importFilePath; }
            set
            {
                _importFilePath = value;
                RaisePropertyChanged("ImportFilePath");
            }
        }
        #endregion
        #region Data - 基本資料
        private F050801 _baseData = null;
		public F050801 BaseData
		{
			get { return _baseData; }
			set { _baseData = value; RaisePropertyChanged("BaseData"); }
		}

		public List<F1903> F1903s { get; set; }

		private List<Wms3pl.WpfClient.ExDataServices.P08ExDataService.DeliveryData> _dlvData = null;
		public List<Wms3pl.WpfClient.ExDataServices.P08ExDataService.DeliveryData> DlvData
		{
			get { return _dlvData; }
			set
			{
				Set(() => DlvData, ref _dlvData, value);

				// 計算序號目前已刷數與應刷數
				if (F1903s != null)
				{
					var query = from f0552 in DlvData
								join f1903 in F1903s
								on f0552.ItemCode equals f1903.ITEM_CODE
								where f1903.BUNDLE_SERIALNO == "1"
								select f0552;

					TotalPackQtySum = query.Sum(x => x.TotalPackQty);
					OrderQtySum = query.Sum(x => x.OrderQty);
				}

				SerialCount.ValidCount = DgSerialList.Count(x => x.ISPASS == true);
				SerialCount.InvalidCount = DgSerialList.Count(x => x.ISPASS == false);
				SerialCount.CurrentCount = DgSerialList.Count();
				RaisePropertyChanged(() => SerialCount);
			}
		}

		#endregion
		#region Data - 序號刷讀
		private SerialDataEx _dgSelectedItem;
		/// <summary>
		/// 選擇的項目
		/// </summary>
		public SerialDataEx DgSelectedItem
		{
			get { return _dgSelectedItem; }
			set { _dgSelectedItem = value; RaisePropertyChanged("DgSelectedItem"); }
		}
		private ObservableCollection<SerialDataEx> _dgSerialList = new ObservableCollection<SerialDataEx>();
		/// <summary>
		/// 刷讀的結果集
		/// </summary>
		public ObservableCollection<SerialDataEx> DgSerialList
		{
			get { return _dgSerialList; }
			set { _dgSerialList = value; RaisePropertyChanged("DgSerialList"); }
		}
		private string _newSerialNo = string.Empty;
		/// <summary>
		/// 新序號
		/// </summary>
		[Required(AllowEmptyStrings = false)]
		[Display(Name = "Required_ErrorMessage", ResourceType = typeof(Resources.Resources))]
		public string NewSerialNo
		{
			get { return _newSerialNo; }
			set { _newSerialNo = value; }
		}
		private SerialStatistic _serialCount = new SerialStatistic() { CurrentCount = 0, InvalidCount = 0, ValidCount = 0 };
		public SerialStatistic SerialCount
		{
			get { return _serialCount; }
			set { _serialCount = value; RaisePropertyChanged("SerialCount"); }
		}

		private int _orderQtySum;
		/// <summary>
		/// 應刷數
		/// </summary>
		public int OrderQtySum
		{
			get { return _orderQtySum; }
			set
			{
				Set(() => OrderQtySum, ref _orderQtySum, value);
			}
		}

		private int _totalPackQtySum;
		/// <summary>
		/// 實收總數
		/// </summary>
		public int TotalPackQtySum
		{
			get { return _totalPackQtySum; }
			set
			{
				Set(() => TotalPackQtySum, ref _totalPackQtySum, value);
			}
		}

        private F1909 _f1909Data;
        public F1909 F1909Data
        {
            get { return _f1909Data; }
            set
            {
                _f1909Data = value;
                RaisePropertyChanged("F1909Data");
            }
        }
		#endregion
		#endregion

		#region Command
		#region ImportExcelCommand
		private List<SerialDataEx> _serialDataExList = null;
		private bool _isImported = true;
		public ICommand ImportExcelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{

						_isImported = true;
						_serialDataExList = null;
                        DispatcherAction(() =>
                        {
                            ExcelImport();
                            if (string.IsNullOrEmpty(ImportFilePath)) return;
                            DoImportData();
                        });
					},
					() =>
					{
						return UserOperateMode == OperateMode.Edit && BaseData != null;
					},
					o =>
					{
						RaisePropertyChanged("DgSerialList");
						// Set Focus and ScrollIntoView
						ActionAfterCheckSerialNo();
						if (!_isImported && _serialDataExList != null)
							ActionForRequireUnlock();
					}
				);
			}
		}

        /// <summary>
        /// 匯入資料的檢查
        /// </summary>
        /// <param name="data"></param>
        public void DoImportData()
		{
            //var data = System.IO.File.ReadAllLines(ImportFilePath);
            List<string> data = System.IO.File.ReadAllLines(ImportFilePath).Select(x => x.Replace(",", "")).ToList();
            // 沒有資料時直接跳出
            if (!data.Any()) return;

			// 如果第一列不是數字, 直接跳出
			int tmpCount;
			if (!int.TryParse(data[0], out tmpCount))
			{
				ShowMessage(Messages.WarningInvalidSerialFile);
				return;
			}

			_serialDataExList = DoCheckSerialNo(data.Skip(1).ToList());

			// 重複刷讀檢核
			foreach (var importSerial in _serialDataExList)
			{
				if (DgSerialList.Any(x => x.SERIAL_NO == importSerial.SERIAL_NO))
				{
					importSerial.ISPASS = false;
					importSerial.MESSAGE = Properties.Resources.P0807010100_ImportSerialRepeat;
				}
			}

			if (_serialDataExList.Where(x => x.ISPASS == true).Count() != tmpCount)
			{
				ShowMessage(Messages.InfoSerialNotAllImported);
				_isImported = false;
			}

			DgSerialList = new ObservableCollection<SerialDataEx>(DgSerialList.ToList().Concat(_serialDataExList));
			DoRefreshReadCount();
		}
		#endregion
		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
					},
					() =>
					{
						return UserOperateMode == OperateMode.Edit && DgSelectedItem != null && DgSelectedItem.ISPASS;
					},
					o =>
					{
						// 刪除動作放這裡, 才會在刪除後更新畫面
						DoDelete();
						RaisePropertyChanged("DgSerialList");
					}
				);
			}
		}
		public void DoDelete()
		{
			if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
			{
				var delList = DgSerialList.Where(o => o.BOX_NO == DgSelectedItem.BOX_NO && o.ISPASS).ToList();
				foreach (var serialDataEx in delList)
					DgSerialList.Remove(serialDataEx);
				DoRefreshReadCount();
			}

			DgSelectedItem = DgSerialList.FirstOrDefault();
		}
		#endregion
		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(),
					() =>
					{
						return UserOperateMode == OperateMode.Edit;
					}
				);
			}
		}
		public void DoCancel()
		{
		}

		#endregion
		#region Check Serial No

		private bool _isCheckOk;
		public ICommand CheckSerialNoCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => _isCheckOk = DoCheckSerialNo(),
					() => true,
					o =>
					{
						RaisePropertyChanged("DgSerialList");
						ActionAfterCheckSerialNo(); // Focus到新項目必須在Command完成後才做
                        if(!_isCheckOk && F1909Data.MANAGER_LOCK == "3")
                        {
                            ActionForRequireUnlock();
                        }
					}
				);
			}
		}
		#endregion
		#region 序號刷讀作業
		/// <summary>
		/// 單筆刷讀的檢查
		/// Memo: 與P02不同, 刷讀有問題的一樣列出來
		/// </summary>
		/// <returns></returns>
		public bool DoCheckSerialNo()
		{
			var proxy = new Wcf.P08WcfServiceClient();
			var tmp = RunWcfMethod<List<Wcf.SerialDataEx>>(proxy.InnerChannel, () => proxy.CheckSerialStatus(
				BaseData.DC_CODE, BaseData.GUP_CODE, BaseData.CUST_CODE, BaseData.WMS_ORD_NO, new string[] { NewSerialNo }).ToList());

			if (DgSerialList == null) DgSerialList = new ObservableCollection<SerialDataEx>();

			var tmpResult = tmp.Select(AutoMapper.Mapper.DynamicMap<SerialDataEx>).ToList();

			// 檢查該序號是否已在此次刷讀過
			var list = tmpResult.Where(x => DgSerialList.Any(o => o.SERIAL_NO == x.SERIAL_NO)).ToList();
			if (list.Any())
			{
				foreach (var serialDataEx in list)
				{
					serialDataEx.MESSAGE = Properties.Resources.P0807010100_serialDataEx;
					serialDataEx.ISPASS = false;
				}
			}

			var tmpData = DgSerialList.ToList();
			tmpData.AddRange(tmpResult);

			DgSerialList = tmpData.ToObservableCollection();
			DoRefreshReadCount();
			//if (!tmp.First().ISPASS)
			//	return false;
			return tmp.First().ISPASS;
		}
		/// <summary>
		/// 匯入時的檢查, 拿掉UI Binding的部份以免效能變差
		/// Memo: 與P02不同, 刷讀有問題的一樣列出來
		/// </summary>
		public List<SerialDataEx> DoCheckSerialNo(List<string> sn)
		{
			var proxy = new Wcf.P08WcfServiceClient();
			var tmp = RunWcfMethod<List<Wcf.SerialDataEx>>(proxy.InnerChannel, () => proxy.CheckSerialStatus(
				BaseData.DC_CODE, BaseData.GUP_CODE, BaseData.CUST_CODE, BaseData.WMS_ORD_NO, sn.ToArray()).ToList());

			return tmp.Select(AutoMapper.Mapper.DynamicMap<SerialDataEx>).ToList();
		}
		/// <summary>
		/// 刷讀後更新統計數
		/// </summary>
		public void DoRefreshReadCount()
		{
			if (BaseData == null)
				return;

			// PACKAGE_BOX_NO 填 0 也OK，因為這邊不用看到自己的包裝數，故填 0
			// 當設定 DlvData 時，也會更新所有畫面要顯示的數量
			DlvData = PackingService.RefreshReadCount(DlvData, BaseData.DC_CODE, BaseData.GUP_CODE, BaseData.CUST_CODE, BaseData.WMS_ORD_NO, 0, FunctionCode);
		}
		#endregion
		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),
					() =>
					{
						return UserOperateMode == OperateMode.Edit
							&& BaseData != null
							&& SerialCount.CurrentCount > 0
							&& DgSerialList != null && DgSerialList.Any()
							&& (SerialCount.ValidCount + TotalPackQtySum) <= OrderQtySum;
					},
					o => DoSaveComplete()
				);
			}
		}

		private bool _saved = false;
		/// <summary>
		/// 刷讀後寫入資料
		/// 0. 檢查序號狀態, 以及是否重複刷讀
		/// 1. 更新F2501
		/// 2. 寫入F05500101, F055002
		/// </summary>
		/// <returns></returns>
		public bool DoSave()
		{
			_saved = false;

			// 批次寫入資料 - 只寫入ISPASS的序號
			var proxy = new Wcf.P08WcfServiceClient();
			var result = RunWcfMethod<Wcf.ExecuteResult>(proxy.InnerChannel,
												() => proxy.CheckAndUpdatePacking(
															BaseData.DC_CODE,
															BaseData.GUP_CODE,
															BaseData.CUST_CODE,
															BaseData.WMS_ORD_NO,
															DgSerialList.Select(x => x.SERIAL_NO).ToArray()));

			if (result.IsSuccessed)
				_saved = true;
			else
				ShowResultMessage(result);
			return _saved;
		}
		private void DoSaveComplete()
		{
            if (_saved) OnSaveComplete();
            else {
                if (F1909Data.MANAGER_LOCK == "3")
                    ActionForRequireUnlock();
            }
           
			_saved = false;
		}
		#endregion Save
		#endregion

		#region 序號刷讀資料結構
		public class SerialStatistic
		{
			public int CurrentCount { get; set; }
			public int ValidCount { get; set; }
			public int InvalidCount { get; set; }
		}
		#endregion

	}
}
