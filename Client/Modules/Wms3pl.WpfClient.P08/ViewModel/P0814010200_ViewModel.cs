using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.P08.Services;
using Wms3pl.WpfClient.UILib;
using Wcf08 = Wms3pl.WpfClient.ExDataServices.P08WcfService;

namespace Wms3pl.WpfClient.P08.ViewModel
{
    public partial class P0814010200_ViewModel : InputViewModelBase
    {
        private string _gupCode = null;
        private string _custCode = null;
        public F910501 f910501Data { get; set; }
        public Action OnCloseClick = delegate { };
        public Action OnPrintClick = delegate { };

        public P0814010200_ViewModel()
        {
            if (!IsInDesignMode)
            {
                ControlInit();
            }
        }

        private void ControlInit()
        {
            SetDcList();

        }
        private void SetDcList()
        {
            var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
            DcList = data;
        }

    /// <summary>
    /// 資料初始
    /// </summary>
    /// <param name="dcCode">物流中心編號</param>
    /// <param name="gupCode">業主編號</param>
    /// <param name="custCode">貨主編號</param>
    /// <param name="wmsOrdNo">出貨單號</param>
    public void SetInitValue(string dcCode, string gupCode, string custCode, string wmsOrdNo)
    {
      IsNotHasDefauleValue = true;

      if (!String.IsNullOrEmpty(wmsOrdNo))
      {
        SelectedDc = dcCode;
        _gupCode = gupCode;
        _custCode = custCode;
        WmsOrdNo = wmsOrdNo;
        WmsOrdNoDisplay = wmsOrdNo;
        SearchCommand.Execute(null);
      }
      else
      {
        _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
        _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
      }
    }

        #region UI屬性
        private List<NameValuePair<string>> _dcList;
        /// <summary>
        /// 物流中心清單
        /// </summary>
        public List<NameValuePair<string>> DcList
        {
            get { return _dcList; }
            set
            {
                _dcList = value;
                RaisePropertyChanged();
                if (_dcList != null && _dcList.Any())
                {
                    SelectedDc = _dcList.First().Value;
                }
            }
        }

        private string _SelectedDc;
        /// <summary>
        /// 選擇的物流中心清單
        /// </summary>
        public string SelectedDc
        {
            get { return _SelectedDc; }
            set
            {
                _SelectedDc = value;
                RaisePropertyChanged();
            }
        }

        private string _WmsOrdNo;
        /// <summary>
        /// 出貨單號
        /// </summary>
        public string WmsOrdNo
        {
            get { return _WmsOrdNo; }
            set { _WmsOrdNo = value; RaisePropertyChanged(); }
        }

        private string _WmsOrdNoDisplay;
        /// <summary>
        /// 出貨單號
        /// </summary>
        public string WmsOrdNoDisplay
        {
            get { return _WmsOrdNoDisplay; }
            set { _WmsOrdNoDisplay = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<String> _PackageBoxNoList;
        /// <summary>
        /// 出貨箱序
        /// </summary>
        public ObservableCollection<String> PackageBoxNoList
        {
            get { return _PackageBoxNoList; }
            set
            {
                _PackageBoxNoList = value;
                RaisePropertyChanged();
                SelectedPackageBoxNo = PackageBoxNoList.FirstOrDefault();
            }
        }

        private string _SelectedPackageBoxNo;
        /// <summary>
        /// 選擇的出貨箱序
        /// </summary>
        public string SelectedPackageBoxNo
        {
            get { return _SelectedPackageBoxNo; }
            set
            {
                _SelectedPackageBoxNo = value;
                RaisePropertyChanged();
                if (value != null)
                    SetReportNameListByPackageBoxNo(short.Parse(value));
            }
        }

        private Boolean _IsCheckAll;
        /// <summary>
        /// 要列印的報表名稱是否全選
        /// </summary>
        public Boolean IsCheckAll
        {
            get { return _IsCheckAll; }
            set
            {
                _IsCheckAll = value;
                RaisePropertyChanged();
                foreach (var item in ReportNameList)
                    item.IsSelected = value;
            }
        }

        List<Wcf08.ShipPackageReportModel> ReportNameListAllData;

        private SelectionList<Wcf08.ShipPackageReportModel> _ReportNameList;
        public SelectionList<Wcf08.ShipPackageReportModel> ReportNameList
        {
            get { return _ReportNameList; }
            set { _ReportNameList = value; RaisePropertyChanged(); }
        }

        private Boolean _IsNotHasDefauleValue;
        public Boolean IsNotHasDefauleValue
        {
            get { return _IsNotHasDefauleValue; }
            set { _IsNotHasDefauleValue = value; RaisePropertyChanged(); }
        }
        #endregion UI屬性

        #region ICommand

        /// <summary>
        /// 查詢按鈕
        /// </summary>
        public ICommand SearchCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                       o => { },
                       () => IsNotHasDefauleValue,
                       o => { DoSearch(); });
            }
        }

        private void DoSearch()
        {
            if (String.IsNullOrEmpty(WmsOrdNo))
            {
                ShowInfoMessage("請輸入出貨單號");
                return;
            }
            ClearUIValue();

            //(1) [A]=呼叫[1.1.9 取得出貨單所有箱要列印報表清單]
            var searchShipReportListReq = new Wcf08.SearchShipReportListReq()
            {
                DcCode = SelectedDc,
                CustCode = _custCode,
                GupCode = _gupCode,
                WmsOrdNo = WmsOrdNo
            };

            var wcfProxy = GetWcfProxy<Wcf08.P08WcfServiceClient>();
            var result = wcfProxy.RunWcfMethod(w => w.SearchShipReportList(searchShipReportListReq)).ToList();
            if (!result.Any())
            {
                ShowInfoMessage("查無資料");
                return;
            }
            WmsOrdNoDisplay = WmsOrdNoDisplay;
            //(2)[B] = 出貨箱序清單 = [A] group by PackageBoxNo
            //   顯示:轉三碼不足補往前補0
            //   值: PackageBoxNo
            var BoxSerialList = result.GroupBy(g => g.PackageBoxNo);
            PackageBoxNoList = BoxSerialList.Select(x => x.Key.ToString("000")).OrderBy(x => x).ToObservableCollection();

            //(3) [C]= [B]有資料，取第一筆箱序值
            //(4) [D] =從[A]資料篩選PackageBoxNo=[C]
            //(5) Grid的資料來源=[D]
            ReportNameListAllData = result;
            SetReportNameListByPackageBoxNo(BoxSerialList.First().Key);
            //var GetData = result.Where(x => x.PackageBoxNo == BoxSerialList.First().Key);

            //ReportNameList = GetData.ToSelectionList();
            WmsOrdNoDisplay = WmsOrdNo;
        }

        /// <summary>
        /// 更新畫面上報表清單
        /// </summary>
        /// <param name="PackageBoxNo">箱序值</param>
        private void SetReportNameListByPackageBoxNo(short PackageBoxNo)
        {
            if (ReportNameListAllData == null || !ReportNameListAllData.Any())
                return;
            var GetData = ReportNameListAllData.Where(x => x.PackageBoxNo == PackageBoxNo);
            ReportNameList = GetData.ToSelectionList();
        }

        /// <summary>
        /// 查詢前清除畫面顯示內容
        /// </summary>
        private void ClearUIValue()
        {
            WmsOrdNoDisplay = "";

            if (PackageBoxNoList != null)
            {
                PackageBoxNoList.Clear();
                PackageBoxNoList = PackageBoxNoList;
            }

            if (ReportNameList != null)
            {
                ReportNameList.Clear();
                ReportNameList = ReportNameList;
            }
        }

        /// <summary>
        /// 列印按鈕
        /// </summary>
        public ICommand PrintCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                       o => { },
                       () => ReportNameList?.Any(x => x.IsSelected) ?? false,
                       //() => true,
                       o => { DoPrint(); });
            }
        }

        private void DoPrint()
        {
            if (f910501Data == null)
            {
                var _proxy = ConfigurationHelper.GetProxy<F91Entities>(false, FunctionCode);
                f910501Data = _proxy.F910501s.Where(x => x.DEVICE_IP == Wms3plSession.Get<GlobalInfo>().ClientIp && x.DC_CODE == SelectedDc).FirstOrDefault();
                if (f910501Data == null || string.IsNullOrWhiteSpace(f910501Data?.PRINTER))
                {
                    DialogService.ShowMessage("尚未未設定印表機，請至裝置維護設定印表機");
                    return;
                }
            }

            ShipPackageService shipPackageService = new ShipPackageService(FunctionCode);
            var result = shipPackageService.PrintShipPackage(SelectedDc, _gupCode, _custCode, ReportNameList.Where(x => x.IsSelected).Select(x => x.Item).ToList(), f910501Data, WmsOrdNo, 1);
            if (!result.IsSuccessed)
            {
                ShowWarningMessage(result.Message);
            }
        }

        #endregion ICommand

    }
}
