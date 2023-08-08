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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.UILib.Services;
using System.Windows.Data;

namespace Wms3pl.WpfClient.P19.ViewModel
{
  public partial class P1920220000_ViewModel : InputViewModelBase
  {
    private string _userId;
    private string _userName;
    public P1920220000_ViewModel()
    {
      if (!IsInDesignMode)
      {
        //初始化執行時所需的值及資料
        InitControls();
      }
    }

    private void InitControls()
    {
      _userId = Wms3plSession.Get<UserInfo>().Account;
      _userName = Wms3plSession.Get<UserInfo>().AccountName;
      SearchCommand.Execute(null);
    }

    #region CheckItem
    /// <summary>
    /// 檢驗項目清單
    /// </summary>
    private List<F1956> _checkItemList;
    public List<F1956> CheckItemList { get { return _checkItemList; } set { _checkItemList = value; RaisePropertyChanged("CheckItemList"); } }
    /// <summary>
    /// 存放選擇的檢驗項目資訊
    /// </summary>
    private F1956 _selectData;
    public F1956 SelectData { get { return _selectData; } set { _selectData = value; RaisePropertyChanged("SelectData"); } }
    #endregion

    #region Search
    public ICommand SearchCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoSearch(), () => UserOperateMode == OperateMode.Query,
          o => DoSearchComplete()
          );
      }
    }

    private void DoSearchComplete()
    {
      if (CheckItemList == null || !CheckItemList.Any()) return;
      SelectData = CheckItemList.FirstOrDefault();
    }

    private void DoSearch()
    {
      //執行查詢動
      var proxy = GetProxy<F19Entities>();
      CheckItemList = proxy.F1956s.OrderBy(x => x.PRIORITY_CODE).ToList();
      if (CheckItemList == null || !CheckItemList.Any())
      {
        ShowMessage(Messages.InfoNoData);
        return;
      }
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
      SelectData = new F1956();
    }
    #endregion Add

    #region Edit
    public ICommand EditCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoEdit(), () => UserOperateMode == OperateMode.Query && (SelectData != null && CheckItemList.Any()) && !check_IsSystem()
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
          o => DoCancel(), () => UserOperateMode != OperateMode.Query,
          o => DoSaveComplete()
          );
      }
    }

    private void DoCancel()
    {
      //執行取消動作
      //重查
      DoSearch();
      UserOperateMode = OperateMode.Query;
    }
    #endregion Cancel

    #region Delete
    public ICommand DeleteCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoDelete(), () => (UserOperateMode == OperateMode.Query) &&(!check_IsSystem()),
          o => DoSaveComplete()
          );
      }
    }
    private bool check_IsSystem()
    {
      if (SelectData !=null)
      {
        string IsSystem = SelectData.IS_SYSTEM;
            if (IsSystem!=null)
            {
              return IsSystem == "1" ? true : false;
            }
      }
    
      return false;
    }
    private void DoDelete()
    {
      //執行刪除動作
      // 0.確認是否刪除
      if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;
      // 1.檢核資料是否存在
      var proxy = GetModifyQueryProxy<F19Entities>();
      var f1956s = proxy.F1956s.Where(x => x.PRIORITY_CODE == SelectData.PRIORITY_CODE).AsQueryable().ToList();
      var isExist = (f1956s != null && f1956s.Count() > 0);
      // 1.1 不存在
      if (!isExist)
      {
        ShowMessage(Messages.WarningBeenDeleted);
        return;
      }
      // 2.判斷是否有關連到F190206(不可刪除)
      var f195601s = proxy.F195601s.Where(x => x.PRIORITY_CODE == SelectData.PRIORITY_CODE).AsQueryable().ToList();
      if (f195601s != null && f195601s.Any())
      {
        ShowMessage(Messages.WarningCannotDeleteCheckItem);
        return;
      }
      // 3.刪除
      var f1956 = f1956s.FirstOrDefault();
      proxy.DeleteObject(f1956);
      // 4.存檔
      proxy.SaveChanges();
      // 5.重查
      DoSearch();
      // 6.顯示成功訊息
      ShowMessage(Messages.Success);
    }
    #endregion Delete

    #region Save
    public ICommand SaveCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoSave(), () => UserOperateMode != OperateMode.Query
          //,          o => DoSaveComplete()
          );
      }
    }

    private void DoSave()
    {
      //執行確認儲存動作
      // 0.檢核必填
      if (!isValid()) return;
      // 1.查看資料是否存在
      var proxy = GetModifyQueryProxy<F19Entities>();
      var f1956s = proxy.F1956s.Where(x => x.PRIORITY_CODE == SelectData.PRIORITY_CODE).AsQueryable().ToList();
      var isExist = (f1956s != null && f1956s.Count() > 0);
      SelectData.IS_SHOW =    SelectData.IS_SHOW    == null ? "0" : SelectData.IS_SHOW;
      SelectData.IS_SYSTEM =  SelectData.IS_SYSTEM  == null ? "0" : SelectData.IS_SYSTEM;
      if (UserOperateMode == OperateMode.Add)
      {
        // 2.新增
        // 2.1 已存在
        if (isExist)
        {
          ShowMessage(Messages.WarningExist);
          return;
        }
        SelectData = SetBasicInfo(SelectData);       
        proxy.AddToF1956s(SelectData);
      }
      else
      {
        // 3.更新
        // 3.1 不存在
        if (!isExist)
        {
          ShowMessage(Messages.WarningBeenDeleted);
          return;
        }
        var f1956 = f1956s.FirstOrDefault();
        f1956 = SetBasicInfo(f1956);
        f1956.PRIORITY_CODE = SelectData.PRIORITY_CODE;
        f1956.PRIORITY_NAME = SelectData.PRIORITY_NAME;
        f1956.IS_SHOW = SelectData.IS_SHOW;
        f1956.IS_SYSTEM = SelectData.IS_SYSTEM;

        proxy.UpdateObject(f1956);
      }
      // 4.存檔
      proxy.SaveChanges();
      // 5.重查
      DoSearch();
      // 6.顯示成功訊息
      ShowMessage(Messages.Success);
      UserOperateMode = OperateMode.Query;
    }

    private F1956 SetBasicInfo(F1956 obj)
    {
      if (UserOperateMode == OperateMode.Add)
      {      
        obj.CRT_STAFF = _userId;
        obj.CRT_NAME = _userName;
        obj.CRT_DATE = DateTime.Now;
      }
      obj.UPD_STAFF = _userId;
      obj.UPD_NAME = _userName;
      obj.UPD_DATE = DateTime.Now;
      return obj;
    }

    private bool isValid()
    {
      if (string.IsNullOrEmpty(SelectData.PRIORITY_CODE))
      {
        var failedMessage = Messages.Failed;
        failedMessage.Message = "出貨優先權代碼尚未輸入";
        ShowMessage(failedMessage);
        //ShowMessage("出貨優先權代碼 尚未輸入");      
        return false;
      }      

        if (string.IsNullOrEmpty(SelectData.PRIORITY_NAME))
        {
          var failedMessage = Messages.Failed;
          failedMessage.Message = "出貨優先權名稱尚未輸入";
          ShowMessage(failedMessage);         
          return false;
        }
      return true;
    }
    private void DoSaveComplete()
    {
      //指定至該筆資料
      if (CheckItemList == null || !CheckItemList.Any()) return;
      if (SelectData == null || !CheckItemList.Any(x => x.PRIORITY_CODE == SelectData.PRIORITY_CODE))
        SelectData = CheckItemList.FirstOrDefault();
      else
        SelectData = CheckItemList.Find(x => x.PRIORITY_CODE == SelectData.PRIORITY_CODE);
    }
    #endregion Save
  }


  public class IntBoolToYesNoConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      short int_Bool;
    
        Int16.TryParse((string)value, out int_Bool);        
        return int_Bool == 1 ? "是" : "否";
    
      //throw new Exception("Value is not Type of Boolean !");
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var visibility = value.ToString();
      if (visibility == "是")
        return "1";
      else if (visibility == "否")
        return "0";
      throw new Exception("Value is not Type of string !");
    }
  }


  /// <summary>
  /// 傳入字元布林回傳布林值
  /// </summary>
  public class IntBoolToTrueFalseConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {//value="0" or "1"  return false or true
      short int_Bool;
      if (value is string)
      {
        Int16.TryParse((string)value, out int_Bool);
        return int_Bool == 0 ? false : true;
      }

      throw new Exception("Value is not Type of Boolean !");
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var visibility = value.ToString();
      if (visibility == "是")
        return "1";
      else if (visibility == "否")
        return "0";
      throw new Exception("Value is not Type of string !");
    }
  }

}

