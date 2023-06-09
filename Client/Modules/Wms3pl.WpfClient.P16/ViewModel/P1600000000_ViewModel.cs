using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F16DataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P16.ViewModel
{
  public partial class P1600000000_ViewModel:InputViewModelBase
  {
    public P1600000000_ViewModel()
		{
      if (!IsInDesignMode)
      {
        //初始化執行時所需的值及資料
      }
		}

		

		#region Function
		#endregion

		#region Command

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

		#endregion
	}
}
