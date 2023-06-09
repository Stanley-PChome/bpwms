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
//using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WpfClient.DataServices.F02DataService;
using System.Data;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0202030200_ViewModel : InputViewModelBase
	{
		public Action AfterCheckRtNo = delegate { };
		private string _userId = Wms3plSession.Get<UserInfo>().Account;
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }

		public P0202030200_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}
		}

		private void InitControls()
		{
			UserOperateMode = OperateMode.Edit;
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
		#region Form - 單號
		private string _purchaseNo = string.Empty;
		public string PurchaseNo
		{
			get { return _purchaseNo; }
			set { _purchaseNo = value; }
		}
		private string _rtNo = string.Empty;
		public string RtNo
		{
			get { return _rtNo; }
			set { _rtNo = value; }
		}
        private ObservableCollection<string> _rtNoList = new ObservableCollection<string>();
        public ObservableCollection<string> RtNoList
        {
            get { return _rtNoList; }
            set { _rtNoList = value; RaisePropertyChanged("RtNoList"); }
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
					o =>
					{
					},
					() => !string.IsNullOrWhiteSpace(RtNo),
					o =>
					{
						AfterCheckRtNo();
					}
				);
			}
		}

		/// <summary>
		/// 列出該進倉單所有的驗收單號
		/// </summary>
		public void DoSearchRtNoList()
		{
			var proxy = GetProxy<F02Entities>();
            var result = proxy.F020201s.Where(x => x.DC_CODE == SelectedDc && x.GUP_CODE == this._gupCode && x.CUST_CODE == this._custCode
                && x.PURCHASE_NO == PurchaseNo && x.RT_NO == RtNo).ToList();
            RtNoList = result.Select(x => x.RT_NO).Distinct().ToObservableCollection();
		}
		#endregion
		#endregion
	}
}
