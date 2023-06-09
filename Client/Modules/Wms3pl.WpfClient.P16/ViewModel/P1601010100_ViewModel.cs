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

namespace Wms3pl.WpfClient.P16.ViewModel
{
  public partial class P1601010100_ViewModel : InputViewModelBase
  {
    public P1601010100_ViewModel()
    {
      if (!IsInDesignMode)
      {
        //初始化執行時所需的值及資料
      }

    }

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
    }
    #endregion Search

    #region Add
    public ICommand AddCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoAdd(), () => UserOperateMode == OperateMode.Query
          );
      }
    }

    private void DoAdd()
    {
      UserOperateMode = OperateMode.Add;
      //執行新增動作
    }
    #endregion Add

    #region Edit
    public ICommand EditCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoEdit(), () => UserOperateMode == OperateMode.Query
          );
      }
    }

    private void DoEdit()
    {
      UserOperateMode = OperateMode.Edit;
      //執行編輯動作
    }
    #endregion Edit

    #region Cancel
    public ICommand CancelCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoCancel(), () => UserOperateMode != OperateMode.Query
          );
      }
    }

    private void DoCancel()
    {
      //執行取消動作

      UserOperateMode = OperateMode.Query;
    }
    #endregion Cancel

    #region Delete
    public ICommand DeleteCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoDelete(), () => UserOperateMode == OperateMode.Query
          );
      }
    }

    private void DoDelete()
    {
      //執行刪除動作
    }
    #endregion Delete

    #region Save
    public ICommand SaveCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoSave(), () => UserOperateMode != OperateMode.Query
          );
      }
    }

    private void DoSave()
    {
      //執行確認儲存動作

      UserOperateMode = OperateMode.Query;
    }
    #endregion Save
    public List<DgDataClass> DgItemSource { get; set; }
    public class DgDataClass
    {
      public string Str1 { get; set; }
      public string Str2 { get; set; }
      public string Str3 { get; set; }
      public string Str4 { get; set; }
      public string Str5 { get; set; }
      public string Str6 { get; set; }
      public string Str7 { get; set; }
      public string Str8 { get; set; }
      public string Str9 { get; set; }
      public string Str10 { get; set; }
      public string Str11 { get; set; }
      public string Str12 { get; set; }
      public string Str13 { get; set; }
      public string Str14 { get; set; }
      public string Str15 { get; set; }
      public string Str16 { get; set; }
      public string Str17 { get; set; }
      public string Str18 { get; set; }
      public string Str19 { get; set; }
      public bool Bool1 { get; set; }
      public bool Bool2 { get; set; }
      public bool Bool3 { get; set; }
      public bool Bool4 { get; set; }
      public bool Bool5 { get; set; }
      public int Int1 { get; set; }
      public int Int2 { get; set; }
      public int Int3 { get; set; }
    }
  }
}
