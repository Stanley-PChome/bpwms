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
using Wms3pl.WpfClient.DataServices.F14DataService;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P71.ViewModel
{
  public partial class P7108100000_ViewModel : InputViewModelBase
  {
    public Action<PrintType> DoPrint = delegate { };
    public P7108100000_ViewModel()
    {
      if (!IsInDesignMode)
      {
        InitControls();
      }

    }

    private void InitControls()
    {
      SetDcList();
    }

    #region 查詢條件

    #region  物流中心 業主 貨主
    private List<NameValuePair<string>> _dcList;
    public List<NameValuePair<string>> DcList
    {
      get { return _dcList; }
      set
      {
        _dcList = value;
        RaisePropertyChanged("DcList");
      }
    }

    private string _selectedDcCode = "";
    public string SelectedDcCode
    {
      get { return _selectedDcCode; }
      set
      {
        _selectedDcCode = value;
        RaisePropertyChanged("SelectedDcCode");
        SetGupList();
      }
    }

    private List<NameValuePair<string>> _gupList;
    public List<NameValuePair<string>> GupList
    {
      get { return _gupList; }
      set
      {
        _gupList = value;
        RaisePropertyChanged("GupList");
      }
    }

    private string _selectedGupCode = "";
    public string SelectedGupCode
    {
      get { return _selectedGupCode; }
      set
      {
        _selectedGupCode = value;
        RaisePropertyChanged("SelectedGupCode");
        SetCustList();
      }
    }

    private List<NameValuePair<string>> _custList;
    public List<NameValuePair<string>> CustList
    {
      get { return _custList; }
      set
      {
        _custList = value;
        RaisePropertyChanged("CustList");
      }
    }

    private string _selectedCustCode = "";
    public string SelectedCustCode
    {
      get { return _selectedCustCode; }
      set
      {
        _selectedCustCode = value;
        RaisePropertyChanged("SelectedCustCode");
      }
    }

    #endregion

    //過帳日期起
    private DateTime _postingDateBegin = DateTime.Today;
    public DateTime PostingDateBegin
    {
      get { return _postingDateBegin; }
      set
      {
        if (_postingDateBegin == value) return;
        Set(() => PostingDateBegin, ref _postingDateBegin, value);
      }
    }

    //過帳日期迄
    private DateTime _postingDateEnd = DateTime.Today;
    public DateTime PostingDateEnd
    {
      get { return _postingDateEnd; }
      set
      {
        if (_postingDateEnd == value) return;
        Set(() => PostingDateEnd, ref _postingDateEnd, value);
      }
    }

    #endregion

    #region 下拉式選單資料來源

    #region  物流中心 業主 貨主
    /// <summary>
    /// 設定DC清單
    /// </summary>
    private void SetDcList()
    {
      var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
      DcList = data;
      if (DcList.Any())
        SelectedDcCode = DcList.First().Value;
    }

    /// <summary>
    /// 設定業主清單
    /// </summary>
    private void SetGupList()
    {
      GupList = Wms3plSession.Get<GlobalInfo>().GetGupDataList(_selectedDcCode);
      GupList.Insert(0, new NameValuePair<string>() { Name = Resources.Resources.All, Value = "" });
      SelectedGupCode = GupList.First().Value;
    }

    /// <summary>
    /// 設定貨主清單
    /// </summary>
    private void SetCustList()
    {
      CustList = Wms3plSession.Get<GlobalInfo>()
        .GetCustDataList(_selectedDcCode, string.IsNullOrWhiteSpace(_selectedGupCode) ? null : _selectedGupCode);
      CustList.Insert(0, new NameValuePair<string>() { Name = Resources.Resources.All, Value = "" });
      SelectedCustCode = CustList.First().Value;
    }

    #endregion

    #endregion

    //查詢結果
    private List<InventoryQueryData> _records;

    public List<InventoryQueryData> Records
    {
      get { return _records; }
      set
      {
        if (_records == value)
          return;
        Set(() => Records, ref _records, value);
      }
    }

    #region Search
    public ICommand SearchCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoSearch(), () => UserOperateMode == OperateMode.Query, o => DoSearchComplete()
          );
      }
    }

    private void DoSearchComplete()
    {
      if (UserOperateMode == OperateMode.Query && Records != null && !Records.Any())
        ShowMessage(Messages.InfoNoData);
    }

    private void DoSearch()
    {
      var proxyEx = GetExProxy<P71ExDataSource>();
      Records = proxyEx.CreateQuery<InventoryQueryData>("GetInventoryQueryDatas")
                   .AddQueryOption("dcCode", string.Format("'{0}'", SelectedDcCode))
                   .AddQueryOption("gupCode", string.Format("'{0}'", SelectedGupCode))
                   .AddQueryOption("custCode", string.Format("'{0}'", SelectedCustCode))
                   .AddQueryOption("postingDateBegin", string.Format("'{0}'", PostingDateBegin.ToString("yyyy/MM/dd")))
                   .AddQueryOption("postingDateEnd", string.Format("'{0}'", PostingDateEnd.ToString("yyyy/MM/dd")))
                   .ToList();

      if (!Records.Any())
        return;
    }
    #endregion Search

    #region Print
    public ICommand PrintCommand
    {
      get
      {
        return new RelayCommand<PrintType>(
          (t) =>
          {
            IsBusy = true;
            try
            {
              DoPrint(t);
            }
            catch (Exception ex)
            {
              Exception = ex;
              IsBusy = false;
            }
            IsBusy = false;
          },
        (t) => !IsBusy && UserOperateMode == OperateMode.Query && Records != null && Records.Count > 0);
      }
    }

    //private List<InventoryQueryData> _rptdata;
    //public List<InventoryQueryData> RptData { get { return _rptdata; } set { _rptdata = value; RaisePropertyChanged("RptData"); } }
    //private bool GetRptData()
    //{
    //  var proxy = GetExProxy<P93ExDataSource>();
    //  var result = proxy.CreateQuery<F930101Rpt>("GetP930101RptDatas")
    //  .AddQueryOption("gupCode", string.Format("'{0}'", F930101Data.GUP_CODE))
    //  .AddQueryOption("custCode", string.Format("'{0}'", F930101Data.CUST_CODE))
    //  .AddQueryOption("dcCode", string.Format("'{0}'", F930101Data.DC_CODE))
    //  .AddQueryOption("receiveNo", string.Format("'{0}'", F930101Data.RECEIVE_NO))
    //  .ToList();
    //  RptData = result;
    //  return true;
    //}
    #endregion Print
  }
}
