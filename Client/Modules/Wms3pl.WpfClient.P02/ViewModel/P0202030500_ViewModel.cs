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
using Wms3pl.WpfClient.DataServices.F15DataService;
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
using wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;
using exshare = Wms3pl.WpfClient.ExDataServices.ShareExDataService;
//using Wms3pl.Datas.Shared.Entities;
using System.Data;
using Wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0202030500_ViewModel : InputViewModelBase
	{	
        public Action<PrintType> DoPrintReport = delegate { };

        private string _userId = Wms3plSession.Get<UserInfo>().Account;
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }

		public P0202030500_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
			}
		}

		public void InitControls()
		{
            //設定驗收單號的下拉式選單
            SetRt_NOeList();
        }

        /// <summary>
        /// 取得驗收單號到Combox中
        /// </summary>
        public void SetRt_NOeList()
        {
            var proxyF02 = GetProxy<F02Entities>();
            var data = proxyF02.F020201s.Where(o => o.PURCHASE_NO == SelectedPurchaseNo).ToList().Select(o => o.RT_NO).Distinct().OrderBy(o=>o);
            var rt_NOList = (from o in data
                             select new NameValuePair<string>()
                             {
                                 Name = o,
                                 Value = o
                             }).ToList();
            Rt_NOList = rt_NOList;
            if (Rt_NOList.Any())
                SelectedRt_NO = rt_NOList.First().Value;
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

		#region Form - 進倉單資料
		private P020203Data _baseData;
		public P020203Data BaseData
		{
			get { return _baseData; }
			set { _baseData = value; RaisePropertyChanged("BaseData"); }
		}
        #endregion

        #region Form - 進倉單號
        /// <summary>
        /// 選取的進倉單號
        /// </summary>
        private string _selectedPurchaseNo = string.Empty;
        /// <summary>
        /// 進倉單號
        /// </summary>
        public string SelectedPurchaseNo
        {
            get { return _selectedPurchaseNo; }
            set
            {
                Set(() => SelectedPurchaseNo, ref _selectedPurchaseNo, value);
            }
        }
        #endregion

        #region Form - 驗收單號

        /// <summary>
        /// 驗收單號
        /// </summary>
        private List<NameValuePair<string>> _rt_NOList;
        public List<NameValuePair<string>> Rt_NOList
		{
			get { return _rt_NOList; }
			set { _rt_NOList = value; RaisePropertyChanged("Rt_NO"); }
		}

        /// <summary>
        /// 選取的驗收單號
        /// </summary>
        private string _selectedRt_NO = "";
        public string SelectedRt_NO
        {
            get { return _selectedRt_NO; }
            set
            {
                _selectedRt_NO = value;
                RaisePropertyChanged("SelectedRt_NO");
            }
        }
        #endregion

        #region Data - 報表資料
        private List<wcf.P0202030500PalletData> _reportData = new List<wcf.P0202030500PalletData>();
		public List<wcf.P0202030500PalletData> ReportData
		{
			get { return _reportData; }
			set { _reportData = value; }
		}
		
		#endregion
		#endregion

		#region Command
		#region Print
		public ICommand PrintCommand
		{
			get
			{
                return new RelayCommand<PrintType>(
                    DoPrint,
                (t) => !IsBusy && _reportData != null);            
            }
		}
		
        private void DoPrint(PrintType printType)
        {
            //取得報表資料
            bool isHasData;
            var proxyEx = GetExProxy<exshare.ShareExDataSource>();
            var proxy = new wcf.P02WcfServiceClient();
            var reportData = RunWcfMethod<wcf.P0202030500PalletData[]>(proxy.InnerChannel,
                           () => proxy.GetP0202030500PalletDatas(_selectedDc , _gupCode, _custCode, SelectedRt_NO));
            isHasData = reportData.Any();
            
            if (isHasData)
            {
                foreach (var item in reportData)
                {
                    item.STICKER_BARCODE = BarcodeConverter128.StringToBarcode(item.STICKER_NO);
                }
                ReportData = reportData.ToList();
                DoPrintReport(printType);
            }              
        }
		#endregion
		#endregion		
	}
}
