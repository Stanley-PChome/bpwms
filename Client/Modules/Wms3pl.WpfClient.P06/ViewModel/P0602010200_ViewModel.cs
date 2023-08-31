using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices.P06ExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P06WcfService;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.ExDataServices;
using System;

namespace Wms3pl.WpfClient.P06.ViewModel
{
  public class P0602010200_ViewModel : InputViewModelBase
  {
    /// <summary>
    /// 關閉畫面
    /// </summary>
    public Action<Boolean> DoClose = delegate { };
    public P0602010200_ViewModel()
    {
      if (!IsInDesignMode)
      {
        //初始化執行時所需的值及資料

      }
    }

    #region Propertys
    private List<F1951> _REASONList;
    public List<F1951> REASONList
    {
      get { return _REASONList; }
      set { Set(() => REASONList, ref _REASONList, value); }
    }

    private F051206Pick _PickSelected;
    public F051206Pick PickSelected
    {
      get { return _PickSelected; }
      set { Set(() => PickSelected, ref _PickSelected, value); }
    }

    private ObservableCollection<F051206LackList> _LackList;
    public ObservableCollection<F051206LackList> LackList
    {
      get { return _LackList; }
      set { Set(() => LackList, ref _LackList, value); }
    }

    private F051206LackList _LackSelected;
    public F051206LackList LackSelected
    {
      get { return _LackSelected; }
      set { Set(() => LackSelected, ref _LackSelected, value); }
    }


    private string _batchDate;
    /// <summary>
    /// 批次日期
    /// </summary>
    public string BatchDate
    {
      get { return _batchDate; }
      set { Set(() => BatchDate, ref _batchDate, value); }
    }

    private string _batchTime;
    /// <summary>
    /// 批次時段
    /// </summary>
    public string BatchTime
    {
      get { return _batchTime; }
      set { Set(() => BatchTime, ref _batchTime, value); }
    }

    private string _wmsOrdNo;
    /// <summary>
    /// 出貨單號
    /// </summary>
    public string WmsOrdNo
    {
      get { return _wmsOrdNo; }
      set { Set(() => WmsOrdNo, ref _wmsOrdNo, value); }
    }
    #endregion

    #region ICommmand

    #region SaveCommand
    public ICommand SaveCommand
    {
      get
      {
        var isSuccess = false;
        return CreateBusyAsyncCommand(
          o =>
          {
            var checkReason = LackList.Where(x => string.IsNullOrWhiteSpace(x.REASON));
            if (checkReason.Any())
            {
              ShowWarningMessage($"項次{string.Join(",", checkReason.Select(x => x.ROWNUM))}請選擇缺貨原因");
              return;
            }

            var checkMemo = LackList.Where(x => string.IsNullOrWhiteSpace(x.MEMO));
            if (checkMemo.Any())
            {
              ShowWarningMessage($"項次{string.Join(",", checkMemo.Select(x => x.ROWNUM))}請填寫備註");
              return;
            }

            var proxy = GetWcfProxy<wcf.P06WcfServiceClient>();
            var result = proxy.RunWcfMethod(w => w.ConfirmLackToShip(LackList.MapCollection<F051206LackList, wcf.F051206LackList>().ToArray()));

            if (!result.IsSuccessed)
            {
              isSuccess = false;
              ShowWarningMessage(result.Message);
              return;
            }

            isSuccess = true;
          },
          () => true,
          o =>
          {
            if (isSuccess)
              DoClose(true);
          });
      }
    }

    #endregion


    #endregion

  }
}
