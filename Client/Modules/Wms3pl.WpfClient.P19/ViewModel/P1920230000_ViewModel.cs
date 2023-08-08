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
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P19.ViewModel
{
  public partial class P1920230000_ViewModel : InputViewModelBase
  {
    private string _userId;
    private string _userName;
    public P1920230000_ViewModel()
    {
      if (!IsInDesignMode)
      {
        //初始化執行時所需的值及資料
        GetDcCodes();
        GetPriCode();
        GetDeviceType();
        InitControls();       
      }
    }

    private void InitControls()
    {
      _userId = Wms3plSession.Get<UserInfo>().Account;
      _userName = Wms3plSession.Get<UserInfo>().AccountName;
      SearchCommand.Execute(null);
    }

    #region 屬性

    private List<NameValuePair<string>> _dcCodes;

    public List<NameValuePair<string>> DcCodes
    {
      get { return _dcCodes; }
      set { Set(ref _dcCodes, value); }
    }

    private string _selectDcCode;
    public string SelectDcCode
    {
      get { return _selectDcCode; }
      set
      {
        Set(ref _selectDcCode, value); 
        SearchCommand.Execute(null);
      }
    }

    private List<NameValuePair<string>> _priCodeList;

    public List<NameValuePair<string>> PriCodeList
    {
      get { return _priCodeList; }
      set
      {
        Set(ref _priCodeList, value);
      }
    }

    private string _selectPriCode;
    public string SelectPriCode
    {
      get { return _selectPriCode; }
      set { Set(ref _selectPriCode, value); }
    }

    //PRIORITY_VALUE
     //PriorityValue
    private string _priorityValue;
    public string PriorityValue
    {
      get { return _priorityValue; }
      set { Set(ref _priorityValue, value); }
    }


    private List<NameValuePair<string>> _deviceTypeList;

    public List<NameValuePair<string>> DeviceTypeList
    {
      get { return _deviceTypeList; }
      set
      { Set(ref _deviceTypeList, value); }
    }

    private string _selectDeviceType;
    public string SelectDeviceType
    {
      get { return _selectDeviceType; }
      set { Set(ref _selectDeviceType, value); }
    }


    /// <summary>
    /// 檢驗項目清單
    /// </summary>
    private List<F195601> _checkItemList;
    public List<F195601> CheckItemList { get { return _checkItemList; } set { _checkItemList = value; RaisePropertyChanged("CheckItemList"); } }
    /// <summary>
    /// 存放選擇的檢驗項目資訊
    /// </summary>
    private F195601 _selectData;
    public F195601 SelectData { get { return _selectData; }
      set { _selectData = value;

        GetF1956IsShow();
     
      
      } }

    private bool _f1956IsShow;
    public bool F1956IsShow { get { return _f1956IsShow; }  set { _f1956IsShow = value; } }
    #endregion 屬性


    #region Method
    /// <summary>
    /// 取得 物流中心資料
    /// </summary>
    private void GetDcCodes()
    {
      DcCodes = Wms3plSession.Get<GlobalInfo>().DcCodeList;
      if (DcCodes.Any())
      {
        SelectDcCode = DcCodes.First().Value;
      }
    }



    /// <summary>
    /// 取得 出貨優先權代碼
    /// </summary>
    private void GetPriCode()
    {
      var proxy = GetProxy<F19Entities>();

      PriCodeList =
        (from p in proxy.F1956s
         
         select new NameValuePair<string>
         {
          Name= p.PRIORITY_NAME
          ,Value= p.PRIORITY_CODE
         }).ToList();
     
    }

    /// <summary>
    /// 取得 自動設備類型
    /// </summary>
    private void GetDeviceType()
    {
      DeviceTypeList = GetBaseTableService.GetF000904List(FunctionCode, "P1920230000", "DEVICE_TYPE");
      SelectDeviceType = DeviceTypeList.FirstOrDefault().Value;
    }

    public void GetF1956IsShow()
    {
      var proxy = GetProxy<F19Entities>();
      string val = "";
      if (SelectData != null && SelectData.PRIORITY_CODE != null)
        val = proxy.F1956s.Where(x => x.PRIORITY_CODE == SelectData.PRIORITY_CODE).Select(x => x.IS_SHOW).FirstOrDefault().ToString();
      if (!(val is string)) val = "";
      F1956IsShow = val == "1";
      RaisePropertyChanged("F1956IsShow");
      RaisePropertyChanged("SelectData");
    }

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
      CheckItemList = proxy.F195601s.Where(x=> x.DC_CODE == SelectDcCode).OrderBy(x => x.PRIORITY_CODE).ToList();
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
      SelectData = new F195601();
      SelectData.DC_CODE = SelectDcCode;
      PriorityValue = "";
    }
    #endregion Add

    #region Edit
    public ICommand EditCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoEdit(), () => UserOperateMode == OperateMode.Query && (SelectData != null && CheckItemList.Any()) 
          );
      }
    }

    private void DoEdit()
    {
      UserOperateMode = OperateMode.Edit;
      //執行編輯動作    
      PriorityValue = SelectData.PRIORITY_VALUE.ToString();
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
          o => DoDelete(), () => (UserOperateMode == OperateMode.Query) ,
          o => DoSaveComplete()
          );
      }
    }


    //  return false;
    //}
    private void DoDelete()
    {
      //執行刪除動作
      // 0.確認是否刪除
      if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;
      // 1.檢核資料是否存在
      var proxy = GetModifyQueryProxy<F19Entities>();    
      var f195601s = proxy.F195601s.Where(x => x.PRIORITY_CODE == SelectData.PRIORITY_CODE && x.DC_CODE == SelectData.DC_CODE && x.DEVICE_TYPE == SelectData.DEVICE_TYPE).AsQueryable().ToList();
      var isExist = (f195601s != null && f195601s.Count() > 0);
      // 1.1 不存在
      if (!isExist)
      {
        ShowMessage(Messages.WarningBeenDeleted);
        return;
      }
      // 2.判斷是否有關連到F1956s(不可刪除)
      var f1956s = proxy.F1956s.Where(x => x.PRIORITY_CODE == SelectData.PRIORITY_CODE ).AsQueryable().ToList();
      if (f1956s != null && f1956s.Any())
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
      var f195601s = proxy.F195601s.Where(x => x.PRIORITY_CODE == SelectData.PRIORITY_CODE && x.DC_CODE == SelectData.DC_CODE && x.DEVICE_TYPE== SelectData.DEVICE_TYPE).AsQueryable().ToList();
      var isExist = (f195601s != null && f195601s.Count() > 0);
    
      SelectData.PRIORITY_VALUE = int.Parse(PriorityValue);
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
        proxy.AddToF195601s(SelectData);
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
        var f195601 = f195601s.FirstOrDefault();

        f195601 = SetBasicInfo(f195601);
        f195601.DC_CODE = SelectData.DC_CODE;
        f195601.PRIORITY_CODE = SelectData.PRIORITY_CODE;
        f195601.DEVICE_TYPE = SelectData.DEVICE_TYPE;
        f195601.PRIORITY_VALUE = SelectData.PRIORITY_VALUE;

        //f195601.PRIORITY_NAME = SelectData.PRIORITY_NAME;
        //f195601.IS_SHOW = SelectData.IS_SHOW;

        proxy.UpdateObject(f195601);
      }
      // 4.存檔
      proxy.SaveChanges();
      // 5.重查
      DoSearch();
      // 6.顯示成功訊息
      ShowMessage(Messages.Success);
      UserOperateMode = OperateMode.Query;
    }

    private F195601 SetBasicInfo(F195601 obj)
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
    
      if (string.IsNullOrEmpty(SelectData.DC_CODE))
      {
        ShowMessage(Messages.WarningNoDcCode);
        return false;
      }

      if (string.IsNullOrEmpty(SelectData.PRIORITY_CODE))
      {
        var failedMessage = Messages.Failed;      
        failedMessage.Message= "出貨優先權代碼尚未輸入";
         ShowMessage(failedMessage);
        //ShowMessage("出貨優先權代碼 尚未輸入");      
        return false;
      }
      if (string.IsNullOrEmpty(SelectData.DEVICE_TYPE))
      {
        var failedMessage = Messages.Failed;
        failedMessage.Message = "自動設備類型尚未輸入";
        ShowMessage(failedMessage);
        //ShowMessage("自動設備類型 尚未輸入");
        return false;
      }
      if (!int.TryParse(PriorityValue, out int i))
      {
        var failedMessage = Messages.Failed;
        failedMessage.Message = "設備優先權值 未輸入";
        ShowMessage(failedMessage);
        //ShowMessage("出貨優先權代碼 尚未輸入");      
        return false;
      }
      return true;
    }
    private void DoSaveComplete()
    {
      //指定至該筆資料
      if (CheckItemList == null || !CheckItemList.Any()) return;
      if (SelectData == null || !CheckItemList.Any(x => x.PRIORITY_CODE == SelectData.PRIORITY_CODE && x.DC_CODE == SelectData.DC_CODE && x.DEVICE_TYPE == SelectData.DEVICE_TYPE))
        SelectData = CheckItemList.FirstOrDefault();
      else
        SelectData = CheckItemList.Find(x => x.PRIORITY_CODE == SelectData.PRIORITY_CODE && x.DC_CODE == SelectData.DC_CODE && x.DEVICE_TYPE == SelectData.DEVICE_TYPE);
    }
    #endregion Save
  }



}

