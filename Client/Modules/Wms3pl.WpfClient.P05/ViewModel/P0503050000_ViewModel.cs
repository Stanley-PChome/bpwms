using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P05.ViewModel
{
    public class P0503050000_ViewModel : InputViewModelBase
    {
        private string _gupCode;
        private string _custCode;

        public P0503050000_ViewModel()
        {
            if (!IsInDesignMode)
            {
                _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
                _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
                InitControls();
            }

        }

        private void InitControls()
        {
            GetDcList();
            SetDateTime();
        }

        private void SetDateTime()
        {
            CalDateBegin = DateTime.Today;
            CalDateEnd = DateTime.Today;
        }

        private void GetDcList()
        {
            DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
            if (DcList.Any())
                DcCode = DcList.First().Value;

        }

        #region 查詢條件

        private string _dcCode;
        public string DcCode
        {
            get { return _dcCode; }
            set
            {
                if (_dcCode == value) return;
                Set(() => DcCode, ref _dcCode, value);
            }
        }

        private List<NameValuePair<string>> _dcList;
        public List<NameValuePair<string>> DcList
        {
            get { return _dcList; }
            set
            {
                if (_dcList == value) return;
                Set(() => DcList, ref _dcList, value);
            }
        }

        private DateTime? _calDateBegin;
        public DateTime? CalDateBegin
        {
            get { return _calDateBegin; }
            set
            {
                if (_calDateBegin == value) return;
                Set(() => CalDateBegin, ref _calDateBegin, value);
            }
        }

        private DateTime? _calDateEnd;
        public DateTime? CalDateEnd
        {
            get { return _calDateEnd; }
            set
            {
                if (_calDateEnd == value) return;
                Set(() => CalDateEnd, ref _calDateEnd, value);
            }
        }

        private string _calNo;
        public string CalNo
        {
            get { return _calNo; }
            set
            {
                if (_calNo == value) return;
                Set(() => CalNo, ref _calNo, value);
            }
        }

        #endregion

        #region 查詢結果

        private ObservableCollection<P0503050000CalHead> _mainList;
        public ObservableCollection<P0503050000CalHead> MainList
        {
            get { return _mainList; }
            set
            {
                if (_mainList == value) return;
                Set(() => MainList, ref _mainList, value);
            }
        }

        private P0503050000CalHead _selectedMainItem;
        public P0503050000CalHead SelectedMainItem
        {
            get { return _selectedMainItem; }
            set
            {
                _selectedMainItem = value;
                RaisePropertyChanged("SelectedMainItem");
            }
        }

        #endregion

        #region Search
        public ICommand SearchCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoSearch(), () => UserOperateMode == OperateMode.Query
                    );
            }
        }

        private void DoSearch()
        {
            ClearData();
            var proxyEx = GetExProxy<P05ExDataSource>();
            MainList = proxyEx.CreateQuery<P0503050000CalHead>("GetCalHeadList")
                              .AddQueryExOption("gupCode", _gupCode)
                              .AddQueryExOption("custCode", _custCode)
                              .AddQueryExOption("dcCode",DcCode)
                              .AddQueryExOption("calDateBegin", CalDateBegin)
                              .AddQueryExOption("calDateEnd",  CalDateEnd.Value.AddDays(1))
                              .AddQueryExOption("calNo", CalNo)
                              .ToObservableCollection();

            if (MainList == null || MainList.Count == 0)
                ShowMessage(Messages.InfoNoData);
        }


        #endregion Search

        private void ClearData()
        {
            SelectedMainItem = null;
            MainList = null;
        }
    }
}
