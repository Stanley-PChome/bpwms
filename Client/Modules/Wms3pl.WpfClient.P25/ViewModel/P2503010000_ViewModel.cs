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
using Wms3pl.WpfClient.DataServices.F25DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P25ExDataService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P25WcfService;

namespace Wms3pl.WpfClient.P25.ViewModel
{
  public partial class P2503010000_ViewModel : InputViewModelBase
  {
    private string _userId;
    private string _userName;
    private string _gupCode;
    private string _custCode;
    private string _clientIp;
    public P2503010000_ViewModel()
    {
      if (!IsInDesignMode)
      {
        _userId = Wms3plSession.Get<UserInfo>().Account;
        _userName = Wms3plSession.Get<UserInfo>().AccountName;
        _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
        _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
				_clientIp = Wms3plSession.Get<GlobalInfo>().ClientIp;
        InitControls();
      }
    }

    private void InitControls()
    {

    }

    #region 查詢條件

    //序號起
    private string _serialNoBegin;
    public string SerialNoBegin
    {
      get { return _serialNoBegin; }
      set
      {
        if (_serialNoBegin == value) return;
        Set(() => SerialNoBegin, ref _serialNoBegin, value);
      }
    }

    //序號迄
    private string _serialNoEnd;
    public string SerialNoEnd
    {
      get { return _serialNoEnd; }
      set
      {
        if (_serialNoEnd == value) return;
        Set(() => SerialNoEnd, ref _serialNoEnd, value);
      }
    }

    //效期起
    private DateTime? _validDateBegin;
    public DateTime? ValidDateBegin
    {
      get { return _validDateBegin; }
      set
      {
        if (_validDateBegin == value) return;
        Set(() => ValidDateBegin, ref _validDateBegin, value);
      }
    }

    //效期迄
    private DateTime? _validDateEnd;
    public DateTime? ValidDateEnd
    {
      get { return _validDateEnd; }
      set
      {
        if (_validDateEnd == value) return;
        Set(() => ValidDateEnd, ref _validDateEnd, value);
      }
    }
    #endregion

    #region 查詢結果
    private ObservableCollection<P250301QueryItem> _queryData;
    public ObservableCollection<P250301QueryItem> QueryData
    {
      get { return _queryData; }
      set
      {
        if (_queryData == value) return;
        Set(() => QueryData, ref _queryData, value);
      }
    }

    //選取的Data
    private P250301QueryItem _selectedQueryData;
    public P250301QueryItem SelectedQueryData
    {
      get { return _selectedQueryData; }
      set
      {
        _selectedQueryData = value;

        RaisePropertyChanged("SelectedQueryData");
      }
    }

    private string _extendCount = "0";
    public string ExtendCount
    {
      get { return _extendCount; }
      set
      {
        if (_extendCount == value) return;
        Set(() => ExtendCount, ref _extendCount, value);
      }
    }
    #endregion

    private DateTime? _extendValidDate;
    /// <summary>
    /// 展延效期
    /// </summary>
    public DateTime? ExtendValidDate
    {
      get { return _extendValidDate; }
      set
      {
        if (_extendValidDate == value) return;
        Set(() => ExtendValidDate, ref _extendValidDate, value);
      }
    }

    #region Search

    private bool _executeSearch;
    private bool _reGetFromF2501;
    public ICommand SearchCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoSearch(), () => UserOperateMode == OperateMode.Query && !string.IsNullOrWhiteSpace(SerialNoBegin),
          o => DoSearchCompleted(),
          null,
          SearchAlert
          );
      }
    }

    private void DoSearchCompleted()
    {
      if (!_executeSearch) return;
      if (QueryData != null && !QueryData.Any())
        ShowMessage(Messages.InfoNoData);
    }

    private void SearchAlert()
    {
      _executeSearch = true;
      if (QueryData != null && QueryData.Any())
      {
        if (
          ShowMessage(new MessagesStruct()
          {
            Button = DialogButton.YesNo,
            Image = DialogImage.Warning,
            Message = Properties.Resources.P2503010000_ViewModel_ClearDataAndReCheckSerialNo,
            Title = Resources.Resources.Information
          }) != DialogResponse.Yes)
          _executeSearch = false;
      }
      _reGetFromF2501 = true;
    }

    private void DoSearch()
    {
      if (!_executeSearch) return;

      SelectedQueryData = null;
      QueryData = null;

      var proxyEx = GetExProxy<P25ExDataSource>();
      QueryData = proxyEx.CreateQuery<P250301QueryItem>("GetP250301QueryData")
                .AddQueryExOption("gupCode", _gupCode)
                .AddQueryExOption("custCode", _custCode)
                .AddQueryExOption("serialBegin", SerialNoBegin)
                .AddQueryExOption("serialEnd", SerialNoEnd)
                .AddQueryExOption("validDateBegin", ValidDateBegin)
                .AddQueryExOption("validDateEnd", ValidDateEnd)
                .AddQueryExOption("clientIp", _clientIp)
                .AddQueryExOption("userId", _userId)
                .AddQueryExOption("userName", _userName)
                .AddQueryExOption("reGetFromF2501", _reGetFromF2501 ? "1" : "0")
                .ToObservableCollection();

      ExtendCount = QueryData.Count(p => p.ISPASS == "1").ToString();
    }
    #endregion Search

    #region Delete
    public ICommand DeleteCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedQueryData != null
          );
      }
    }

    private void DoDelete()
    {
      if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes)
        return;

      var proxy = GetProxy<F25Entities>();
      var f250105 = (from p in proxy.F250105s
                     where p.LOG_SEQ == SelectedQueryData.LOG_SEQ
                     select p).SingleOrDefault();
      if (f250105 != null)
      {
        f250105.STATUS = "9";
      }
      proxy.UpdateObject(f250105);
      proxy.SaveChanges();

      _executeSearch = true;
      _reGetFromF2501 = false;
      DoSearch();

      ShowMessage(Messages.DeleteSuccess);
    }
    #endregion Delete

    #region 序號展延
    public ICommand ExtendCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoExtend(),
          () =>
            UserOperateMode == OperateMode.Query && QueryData != null &&
            QueryData.Any(p => p.ISPASS == "1" && ExtendValidDate != null)
          );
      }
    }

    private void DoExtend()
    {
      if (
        ShowMessage(new MessagesStruct()
        {
          Button = DialogButton.YesNo,
          Image = DialogImage.Warning,
          Message = string.Format(Properties.Resources.P2503010000_ViewModel_ExtendCount, ExtendCount),
          Title = Resources.Resources.Information
        }) != DialogResponse.Yes)
        return;

      var proxy = new ExDataServices.P25WcfService.P25WcfServiceClient();
      var listSerialNo = QueryData.Where(p => p.ISPASS == "1").Select(p => p.SERIAL_NO).ToList();
      var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
        () =>
          proxy.ExtendValidDate(listSerialNo.ToArray(), ExtendValidDate.Value, _gupCode, _custCode, _userId, _userName));
      if (!result.IsSuccessed)
      {
        ShowMessage(result.Message);
        return;
      }

      _executeSearch = true;
      _reGetFromF2501 = false;
      DoSearch();

      DialogService.ShowMessage(Properties.Resources.P2503010000_ViewModel_ExtendSerial);
    }

    #endregion
  }
}
