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
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.UILib;
using System.Windows.Controls;
using Wms3pl.Common.Security;
using Wms3pl.WpfClient.DataServices.F25DataService;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.P08.Services;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.ExDataServices;
using Wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.DataServices.F16DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public partial class P0802010200_ViewModel : InputViewModelBase
	{
		public Action ActionAfterCheckSerialNo = delegate { };
		public Action ActionForRequireUnlock = delegate { };
		//public Action ActionBeforeImportData = delegate { };
		public Action OnSaveComplete = delegate { };
        public Action ExcelImport = delegate { };

        private string _userId = Wms3plSession.Get<UserInfo>().Account;
        #region 貨主
        public string CustCode= Wms3plSession.Get<GlobalInfo>().CustCode;
        #endregion
        public P0802010200_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				UserOperateMode = OperateMode.Edit;
			}

		}
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
        #region 資料連結
        #region Form - 物流中心
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
		#region Data - 退貨檢驗序號刷驗紀錄檔
		private F161201 _baseData = null;
		public F161201 BaseData { get { return _baseData; } set { _baseData = value; RaisePropertyChanged("BaseData"); } }
		private F16140101 _selectData = null;
		public F16140101 SelectData
		{
			get { return _selectData; }
			set { _selectData = value; RaisePropertyChanged("SelectData"); }
		}
		private List<F16140101> _returnSerialDatas = null;
		public List<F16140101> ReturnSerialDatas
		{
			get { return _returnSerialDatas; }
			set { _returnSerialDatas = value; RaisePropertyChanged("ReturnSerialDatas"); }
		}
		private F910501 _selectedF910501;

		public F910501 SelectedF910501
		{
			get { return _selectedF910501; }
			set
			{
				Set(ref _selectedF910501, value);
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
		private SerialStatistic _serialCount = new SerialStatistic() { CurrentCount = 0, InvalidCount = 0, TotalValidCount = 0, ValidCount = 0 };
		public SerialStatistic SerialCount
		{
			get { return _serialCount; }
			set { _serialCount = value; RaisePropertyChanged("SerialCount"); }
		}
		#endregion
		#region Data - 序號匯入
		private int _importNotPassCount = 0;
		public int ImportNotPassCount
		{
			get { return _importNotPassCount; }
			set
			{
				_importNotPassCount = value;
				RaisePropertyChanged("ImportNotPassCount");
				RaisePropertyChanged("NotPassCountString");
			}
		}
		public string NotPassCountString
		{ get { return (ImportNotPassCount == 0) ? string.Empty : string.Format(Properties.Resources.P0802010200_ImportNotPassCount, ImportNotPassCount); } }
        #endregion
        #endregion

        #region Command

        public ICommand ImportExcelCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    DispatcherAction(() =>
                    {
                        ExcelImport();
                        if (string.IsNullOrEmpty(ImportFilePath)) return;
                        DoImportExcelCommand.Execute(null);
                    });
                });
            }
        }

        #region ImportExcelCommand
        private bool _isImported = true;
		public ICommand DoImportExcelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{
                        DoImportData();
                       
                    },
					() =>
					{
						return UserOperateMode == OperateMode.Edit && true;
					},
                    o =>
                    {
                        RaisePropertyChanged("DgSerialList");
                        ActionAfterCheckSerialNo();
                        if (!_isImported) ActionForRequireUnlock();
                        _isImported = true;
                    }
				);
			}
		}
		/// <summary>
		/// 匯入資料的檢查
		/// </summary>
		/// <param name="data"></param>
		public bool DoImportData()
		{
            var data = System.IO.File.ReadAllLines(ImportFilePath);

            bool result = true;
			// 沒有資料時直接跳出
			if (!data.Any()) return result;

			if (data.GroupBy(x => x).Any(g => g.Count() > 1))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P0802010200_ItemSerialIsRepeat, Title = WpfClient.Resources.Resources.Information });
				return result;
			}
            //DoCheckSerialNo();
            var tmpResult = DoCheckSerialNo(data.ToList());

            // 檢查該序號是否已在此次刷讀過
            var list = tmpResult.Where(x => DgSerialList.Any(o => o.SERIAL_NO == x.SERIAL_NO)).ToList();
            if (list.Any())
            {
                foreach (var serialDataEx in list)
                {
                    serialDataEx.MESSAGE = Properties.Resources.P0802010200_ItemSerialIsRepeat;
                    serialDataEx.ISPASS = false;
                }
            }
            var tmp2 = DgSerialList.ToList();
			tmp2.AddRange(tmpResult);
			DgSerialList = tmp2.ToObservableCollection();

			// 設定匯入不通過筆數
			DoRefreshReadCount();

			_isImported = result;
			return result;
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
		public ICommand CheckSerialNoCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCheckSerialNo(),
					() => true,
					o =>
					{
						RaisePropertyChanged("DgSerialList");
						ActionAfterCheckSerialNo(); // Focus到新項目必須在Command完成後才做
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
		public void DoCheckSerialNo()
		{
			var proxy = GetWcfProxy<Wcf.P08WcfServiceClient>();
			var tmpResult = proxy.RunWcfMethod(w => w.CheckSerials(BaseData.DC_CODE,
													BaseData.GUP_CODE,
													BaseData.CUST_CODE,
													BaseData.RETURN_NO,
													new string[] { NewSerialNo },
													"C1")).Select(AutoMapper.Mapper.DynamicMap<SerialDataEx>).ToList();

			if (DgSerialList == null)
				DgSerialList = new ObservableCollection<SerialDataEx>();

			// 檢查該序號是否已在此次刷讀過
			var list = tmpResult.Where(x => DgSerialList.Any(o => o.SERIAL_NO == x.SERIAL_NO)).ToList();
			if (list.Any())
			{
				foreach (var serialDataEx in list)
				{
					serialDataEx.MESSAGE = Properties.Resources.P0802010200_ItemSerialIsRepeat;
					serialDataEx.ISPASS = false;
				}
			}

			DgSerialList = DgSerialList.Concat(tmpResult).ToObservableCollection();
			DoRefreshReadCount();
		}
		/// <summary>
		/// 匯入時的檢查, 拿掉UI Binding的部份以免效能變差
		/// Memo: 與P02不同, 刷讀有問題的一樣列出來
		/// </summary>
		public List<SerialDataEx> DoCheckSerialNo(List<string> sn)
		{
			var proxy = new Wcf.P08WcfServiceClient();
			var tmp = RunWcfMethod<List<Wcf.SerialDataEx>>(proxy.InnerChannel, () => proxy.CheckSerials(
				BaseData.DC_CODE, BaseData.GUP_CODE, BaseData.CUST_CODE, BaseData.RETURN_NO, sn.ToArray(), "C1").ToList());
			var tmpResult = tmp.Select(AutoMapper.Mapper.DynamicMap<SerialDataEx>).ToList();
			return tmpResult.ToList();

		}
		/// <summary>
		/// 刷讀後更新統計數
		/// </summary>
		public void DoRefreshReadCount()
		{
			ImportNotPassCount = DgSerialList.Count(x => !x.ISPASS);
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
						return UserOperateMode == OperateMode.Edit && DgSerialList.Any();
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

			// 批次寫入資料
			var proxy = new Wcf.P08WcfServiceClient();
			var tmp = RunWcfMethod<Wcf.ExecuteResult>(proxy.InnerChannel,
					() => proxy.InportUpdatePacking(BaseData.DC_CODE, BaseData.GUP_CODE, BaseData.CUST_CODE, BaseData.RETURN_NO,
																					DgSerialList.Select(x => x.SERIAL_NO).Distinct().ToArray(),
																					Wms3plSession.CurrentUserInfo.AccountName, Wms3plSession.CurrentUserInfo.Account,
																					"C1", SelectedF910501?.VIDEO_NO));
			if (tmp.IsSuccessed == true) _saved = true;
			else ShowMessage(new MessagesStruct() { Button = DialogButton.OK, Title = WpfClient.Resources.Resources.Information, Message = tmp.Message, Image = DialogImage.Information });

			return _saved;
		}
		private void DoSaveComplete()
		{
			if (_saved) OnSaveComplete();
			_saved = false;
		}
		#endregion Save
		#endregion

		#region 序號刷讀資料結構
		public class SerialStatistic
		{
			public int TotalValidCount { get; set; }
			public int CurrentCount { get; set; }
			public int ValidCount { get; set; }
			public int InvalidCount { get; set; }
		}
		#endregion
	}
}
