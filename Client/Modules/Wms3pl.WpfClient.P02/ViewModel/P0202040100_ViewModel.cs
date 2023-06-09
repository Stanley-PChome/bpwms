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
using Wms3pl.WpfClient.DataServices.F25DataService;

//using Wms3pl.Datas.Shared.Entities;
using System.Data;
using Wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;
using System.Windows.Media.Imaging;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0202040100_ViewModel : InputViewModelBase
	{
		public Action OnCancelComplete = delegate { };
		private string _userId = Wms3plSession.Get<UserInfo>().Account;
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }

		public P0202040100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}
		}

		private void InitControls()
		{
		}

		#region 資料連結/ 頁面參數
		private void PageRaisePropertyChanged()
		{
		}
		#region Form - 可用的DC (物流中心)清單
		private string _selectedDc = string.Empty;
		/// <summary>
		/// 選取的物流中心
		/// </summary>
		public string SelectedDc
		{
			get { return _selectedDc; }
			set { _selectedDc = value; }
		}
		#endregion
		#region Data - 基本資料
		private F020201WithF02020101 _baseData;
		public F020201WithF02020101 BaseData
		{
			get { return _baseData; }
			set { _baseData = value; RaisePropertyChanged("BaseData"); }
		}

		private string _rtNo;
		public string RtNo
		{
			get { return _rtNo; }
			set { _rtNo = value; RaisePropertyChanged("RtNo"); }
		}

		#endregion
		#region Data - 序號刷讀
		private ObservableCollection<SerialData> _dgSerialList = new ObservableCollection<SerialData>();
		/// <summary>
		/// 刷讀的結果集
		/// </summary>
		public ObservableCollection<SerialData> DgSerialList
		{
			get { return _dgSerialList; }
			set { _dgSerialList = value; RaisePropertyChanged("DgSerialList"); }
		}
		private SerialStatistic _serialCount = new SerialStatistic() { CurrentCount = 0, InvalidCount = 0, TotalValidCount = 0, ValidCount = 0 };
		public SerialStatistic SerialCount
		{
			get { return _serialCount; }
			set { _serialCount = value; RaisePropertyChanged("SerialCount"); }
		}
		#endregion
		#endregion

		#region Command
		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(),
					() => true
				);
			}
		}
		private void DoSearch()
		{
			var proxy = GetProxy<F02Entities>();
			var result = proxy.F02020104s.Where(x => x.DC_CODE == SelectedDc && x.GUP_CODE == this._gupCode
				&& x.CUST_CODE == this._custCode && x.ITEM_CODE == BaseData.ITEM_CODE
				&& x.PURCHASE_NO == BaseData.PURCHASE_NO && x.PURCHASE_SEQ == BaseData.PURCHASE_SEQ
				&& x.RT_NO == RtNo).ToList();

			DgSerialList = result.Select(x => new SerialData()
			{
				ISPASS = (x.ISPASS == "1" ? true : false),
				MESSAGE = SerialService.GetStatusMessageForInWarehouse(x.STATUS, FunctionCode),
				SERIAL_NO = x.SERIAL_NO
			}).ToObservableCollection();

			SerialCount.ValidCount = DgSerialList.Count(x => x.ISPASS == true);
			SerialCount.InvalidCount = DgSerialList.Count(x => x.ISPASS == false);
			SerialCount.CurrentCount = DgSerialList.Count();
			SerialCount.TotalValidCount = DgSerialList.Count(x => x.ISPASS == true);
			RaisePropertyChanged("SerialCount");
		}
		#endregion
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
