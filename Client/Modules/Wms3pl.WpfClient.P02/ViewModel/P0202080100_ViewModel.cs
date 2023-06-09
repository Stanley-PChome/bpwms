using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P02.ViewModel
{
  public class P0202080100_ViewModel : InputViewModelBase
  {
    public Action DoFocusInputSerialNo = delegate { };
    public Action<Boolean> DoClose = delegate { };
    public P0202080100_ViewModel()
    {
      if (!IsInDesignMode)
      {

      }
    }

    #region 屬性

    #region DcCode
    public string DcCode { get; set; }
    #endregion DcCode

    #region GupCode
    public string GupCode { get; set; }
    #endregion GupCode

    #region CustCode
    public string CustCode { get; set; }
    #endregion CustCode

    #region 驗收單號
    /// <summary>
    /// 驗收單號
    /// </summary>
    public string rtNo { get; set; }
    #endregion 驗收單號

    #region 刷入的序號內容
    private string _inputSerialNo;
    /// <summary>
    /// 刷入的序號內容
    /// </summary>
    public string InputSerialNo
    {
      get { return _inputSerialNo; }
      set { Set(() => InputSerialNo, ref _inputSerialNo, value); }
    }
    #endregion 刷入的序號內容

    #region 要移除的序號清單
    private ObservableCollection<string> _removeSerialNos;
    /// <summary>
    /// 要移除的序號清單
    /// </summary>
    public ObservableCollection<string> RemoveSerialNos
    {
      get { return _removeSerialNos; }
      set { Set(() => RemoveSerialNos, ref _removeSerialNos, value); }
    }

    private string _selectedRemoveSerialNo;
    /// <summary>
    /// 要移除的序號清單-所選的序號
    /// </summary>
    public string SelectedRemoveSerialNo
    {
      get { return _selectedRemoveSerialNo; }
      set { Set(() => SelectedRemoveSerialNo, ref _selectedRemoveSerialNo, value); }
    }
    #endregion 要移除的序號清單

    #endregion 屬性

    #region ICommand

    #region SaveCommand
    public ICommand SaveCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => { },
          () => true,
          o => DoClose(true)
          );
      }
    }
    #endregion SaveCommand

    #region CancelCommand
    public ICommand CancelCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => { },
          ()=>true,
          o => DoClose(false)
          );
      }
    }
    #endregion CancelCommand

    #region DeleteCommand
    public ICommand DeleteCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => { },
          () => true,
          o =>
          {
            RemoveSerialNos.Remove(SelectedRemoveSerialNo);
            RemoveSerialNos = RemoveSerialNos;
          }
          );
      }
    }
    #endregion DeleteCommand

    #region 序號Textbox KeyDown
    public ICommand SerialNoKeyDownCommand
    {
      get
      {
        var IsCheck = false;
        return CreateBusyAsyncCommand(
          o =>
          {
            if(RemoveSerialNos.Any(x => x == InputSerialNo))
            {
              ShowWarningMessage("此序號已存在");
              IsCheck = false;
              return;
            }


            var proxy = GetExProxy<P02ExDataSource>();
            var result = proxy.CreateQuery<ExecuteResult>("CheckSerialNo")
                        .AddQueryExOption("dcCode", DcCode)
                        .AddQueryExOption("gupCode", GupCode)
                        .AddQueryExOption("custCode", CustCode)
                        .AddQueryExOption("rtNo", rtNo)
                        .AddQueryExOption("serialNo", InputSerialNo)
                        .ToList().First();
            if (!result.IsSuccessed)
            {
              ShowWarningMessage(result.Message);
              IsCheck = false;
              return;
            }
            IsCheck = true;
          },
          () => true,
          o =>
          {
            if (!IsCheck)
              return;

            RemoveSerialNos.Add(InputSerialNo);
            RemoveSerialNos = RemoveSerialNos;
            DoFocusInputSerialNo();
          }
          );
      }
    }
    #endregion 序號Textbox KeyDown

    #endregion ICommand
  }
}
