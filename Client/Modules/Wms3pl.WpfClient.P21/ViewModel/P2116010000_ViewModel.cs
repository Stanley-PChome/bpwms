using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P21ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using wcf = Wms3pl.WpfClient.ExDataServices.P21WcfService;

namespace Wms3pl.WpfClient.P21.ViewModel
{
    public class P2116010000_ViewModel : InputViewModelBase
    {

        public P2116010000_ViewModel()
        {
            if (!IsInDesignMode)
                InitControls();
        }

        private void InitControls()
        {
            DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
            if (DcList.Any()) SelectedDc = DcList.FirstOrDefault().Value;

            QueryCount = GetBaseTableService.GetF000904List(FunctionCode, "F0006", "COUNT");
            SelectedQueryCount = QueryCount.First().Value;

            SortMode = GetBaseTableService.GetF000904List(FunctionCode, "F0006", "SORTER");
            SelectedSortMode = SortMode.Last().Value; //預設改遞減，方便查詢

            ExtSystem = GetBaseTableService.GetF000904List(FunctionCode, "F0006", "EXT_SYSTEM");
            SelectedExtSystem = ExtSystem.First().Value;
        }

        private void ReflashFunctionShowNameList()
        {
            var proxy = GetProxy<F00Entities>();
            FunctionShowNameList = proxy.F0006s.Where(x => x.DC_CODE == SelectedDc && x.EXTERNAL_SYSTEM == SelectedExtSystem).OrderBy(x=>x.SHOW_NAME).ToObservableCollection();
        }

        #region UI屬性
        #region 物流中心
        private List<NameValuePair<string>> _dcList;
        /// <summary>
        /// 物流中心清單
        /// </summary>
        public List<NameValuePair<string>> DcList
        {
            get
            {
                return _dcList;
            }
            set { _dcList = value; RaisePropertyChanged(); }
        }
        private string _selectedDc = string.Empty;
        public string SelectedDc
        {
            get { return _selectedDc; }
            set
            {
                _selectedDc = value;
                RaisePropertyChanged();
                ReflashFunctionShowNameList();
            }
        }
        #endregion 物流中心

        #region 查詢筆數
        private List<NameValuePair<string>> _QueryCount;
        /// <summary>
        /// 查詢筆數
        /// </summary>
        public List<NameValuePair<string>> QueryCount
        {
            get
            {
                return _QueryCount;
            }
            set { _QueryCount = value; RaisePropertyChanged(); }
        }

        private string _selectedQueryCount = string.Empty;
        public string SelectedQueryCount
        {
            get { return _selectedQueryCount; }
            set
            {
                _selectedQueryCount = value;
                RaisePropertyChanged();
            }
        }

        #endregion 查詢筆數

        #region 排序方式
        private List<NameValuePair<string>> _SortMode;
        /// <summary>
        /// 排序方式
        /// </summary>
        public List<NameValuePair<string>> SortMode
        {
            get
            {
                return _SortMode;
            }
            set { _SortMode = value; RaisePropertyChanged(); }
        }

        private string _selectedSortMode = string.Empty;
        public string SelectedSortMode
        {
            get { return _selectedSortMode; }
            set
            {
                _selectedSortMode = value;
                RaisePropertyChanged();
            }
        }

        #endregion 排序方式

        #region 介接系統
        private List<NameValuePair<string>> _ExtSystem;
        /// <summary>
        /// 介接系統
        /// </summary>
        public List<NameValuePair<string>> ExtSystem
        {
            get
            {
                return _ExtSystem;
            }
            set { _ExtSystem = value; RaisePropertyChanged(); }
        }

        private string _selectedExtSystem = string.Empty;
        public string SelectedExtSystem
        {
            get { return _selectedExtSystem; }
            set
            {
                _selectedExtSystem = value;
                RaisePropertyChanged();

                ReflashFunctionShowNameList();
            }
        }

        #endregion 介接系統

        #region 功能名稱
        private ObservableCollection<F0006> _FunctionShowNameList;
        /// <summary>
        /// 功能名稱
        /// </summary>
        public ObservableCollection<F0006> FunctionShowNameList
        {
            get { return _FunctionShowNameList; }
            set
            {
                _FunctionShowNameList = value;
                RaisePropertyChanged();
                if (FunctionShowNameList?.Any() ?? false) SelectedFunctionShowName = FunctionShowNameList.First();
            }
        }

        private F0006 _SelectedFunctionShowName;
        public F0006 SelectedFunctionShowName
        {
            get { return _SelectedFunctionShowName; }
            set
            {
                _SelectedFunctionShowName = value;
                RaisePropertyChanged();
            }
        }

        #endregion 功能名稱

        #region 查詢單號
        private string _SearchOrdNo;
        public string SearchOrdNo
        {
            get { return _SearchOrdNo; }
            set { _SearchOrdNo = value; RaisePropertyChanged(); }
        }
        #endregion 查詢單號

        #region 回傳訊息
        private string _ReturnMessage;
        public string ReturnMessage
        {
            get { return _ReturnMessage; }
            set { _ReturnMessage = value; RaisePropertyChanged(); }
        }
        #endregion 回傳訊息

        #region 只顯示失敗的紀錄
        private Boolean _IsOnlyFailMessage = false;
        public Boolean IsOnlyFailMessage
        {
            get { return _IsOnlyFailMessage; }
            set { _IsOnlyFailMessage = value; RaisePropertyChanged(); }
        }
        #endregion 只顯示失敗的紀錄

        #region Log紀錄清單
        private ObservableCollection<F0090x> _LogList;
        public ObservableCollection<F0090x> LogList
        {
            get { return _LogList; }
            set { _LogList = value; RaisePropertyChanged(); }
        }
        #endregion Log紀錄清單

        #endregion UI屬性

        #region ICommand
        public ICommand SearchCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o =>
                    {
                        LogList = new ObservableCollection<F0090x>();
                        if (SelectedFunctionShowName == null)
                        {
                            ShowWarningMessage("請選擇功能名稱");
                            return;
                        }

                        var proxy = GetWcfProxy<wcf.P21WcfServiceClient>();
                        var result = proxy.RunWcfMethod(w => w.GetF0090x(SelectedDc, SelectedQueryCount, SelectedSortMode == "1", SelectedFunctionShowName.Map<F0006, wcf.F0006>(), SearchOrdNo, ReturnMessage, IsOnlyFailMessage));
                        if (!result?.Any() ?? true)
                        {
                            ShowInfoMessage("查無資料");
                            return;
                        }
                        LogList = result.MapCollection<wcf.F0090x, F0090x>().ToObservableCollection();
                        LogList = LogList;
                    });
            }
        }
        #endregion ICommand
    }
}
