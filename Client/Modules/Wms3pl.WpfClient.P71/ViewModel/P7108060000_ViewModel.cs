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

namespace Wms3pl.WpfClient.P71.ViewModel
{
  public partial class P7108060000_ViewModel : InputViewModelBase
  {
    public P7108060000_ViewModel()
    {
      if (!IsInDesignMode)
      {
        //初始化執行時所需的值及資料
          _searchWidth = 30;
          _searchLength = 30;
          _searchHeight = 30;
      }

    }

    #region 查詢條件

    private decimal _searchWidth = default(decimal);

    public decimal SearchWidth
    {
        get { return _searchWidth; }
        set
        {
            if (value == 0) return;
            _searchWidth = value;
            RaisePropertyChanged("SearchWidth");
        }
    }

    private decimal _searchLength = default(decimal);

    public decimal SearchLength
    {
        get { return _searchLength; }
        set
        {
            if (value == 0) return;
            _searchLength = value;
            RaisePropertyChanged("SearchLength");
        }
    }

    private decimal _searchHeight = default(decimal);

    public decimal SearchHeight
    {
        get { return _searchHeight; }
        set
        {
            if (value == 0) return;
            _searchHeight = value;
            RaisePropertyChanged("SearchHeight");
        }
    }

    private string _calCuft;

    public string CalCuft
    {
        get { return _calCuft; }
        set
        {
            _calCuft = value;
            RaisePropertyChanged("CalCuft");
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
      //執行查詢動
        CalCuft = Math.Round((_searchLength * _searchWidth * _searchHeight / 28317), 2).ToString();
    }
    #endregion Search

  }
}
