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
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;

namespace Wms3pl.WpfClient.P19.ViewModel
{


  public partial class P1920020000_ViewModel : InputViewModelBase
  {
    #region 共用變數/資料連結/頁面參數
    private readonly F91Entities _proxy;
    private string _userId;
    private string _userName;
    private bool isValid;
    public Action AddAction = delegate { };
    public Action EditAction = delegate { };
    public Action SearchAction = delegate { };

    #endregion

    public P1920020000_ViewModel()
    {
      if (!IsInDesignMode)
      {
        _proxy = GetProxy<F91Entities>();
        _userId = Wms3plSession.Get<UserInfo>().Account;
        _userName = Wms3plSession.Get<UserInfo>().AccountName;
        InitControls();
      }
    }

    private void InitControls()
    {
      GetItemTypeList();
      if (ItemTypeList.Any())
        ItemTypeId = ItemTypeList[0].Value;
    }

    #region 項目類別
    private List<NameValuePair<string>> _itemTypeList;

    public List<NameValuePair<string>> ItemTypeList
    {
      get { return _itemTypeList; }
      set
      {
        if (_itemTypeList == value) return;
        Set(() => ItemTypeList, ref _itemTypeList, value);
      }
    }

    private string _itemTypeId;

    public string ItemTypeId
    {
      get { return _itemTypeId; }
      set
      {
        if (_itemTypeId == value) return;
        Set(() => ItemTypeId, ref _itemTypeId, value);
      }
    }

    private List<NameValuePair<string>> _itemTypeListForEdit;

    public List<NameValuePair<string>> ItemTypeListForEdit
    {
      get { return _itemTypeListForEdit; }
      set
      {
        if (_itemTypeListForEdit == value) return;
        Set(() => ItemTypeListForEdit, ref _itemTypeListForEdit, value);
      }
    }

    private void GetItemTypeList()
    {
      var q = (from p in _proxy.F910003s
               where p.ISVISABLE == "1"
               select new NameValuePair<string>()
               {
                 Value = p.ITEM_TYPE_ID,
                 Name = p.ITEM_TYPE
               }).AsEnumerable().OrderBy(p => p.Value);
      ItemTypeList = q.ToList();
      ItemTypeListForEdit = ItemTypeList.ToList();
      ItemTypeList.Insert(0, new NameValuePair<string>(Resources.Resources.All, ""));
    }
    #endregion

    private string _accUnit;

    public string AccUnit
    {
      get { return _accUnit; }
      set
      {
        if (_accUnit == value) return;
        Set(() => AccUnit, ref _accUnit, value);
      }
    }

    private string _accUnitName;

    public string AccUnitName
    {
      get { return _accUnitName; }
      set
      {
        if (_accUnitName == value) return;
        Set(() => AccUnitName, ref _accUnitName, value);
      }
    }


    private List<F91000302SearchData> _dataList;
    public List<F91000302SearchData> DataList
    {
      get
      {
        if (_dataList == null) _dataList = new List<F91000302SearchData>();
        return _dataList;
      }
      set
      {
        if (_dataList == value) return;
        Set(() => DataList, ref _dataList, value);
      }
    }

    private F91000302SearchData _selectedData;

    public F91000302SearchData SelectedData
    {
      get { return _selectedData; }
      set
      {
        if (_selectedData != null && (UserOperateMode == OperateMode.Edit))
        {
          //ShowMessage(Properties.Resources.P1901090100_UnSelectableStatus);
          return;
        }
        else
        {
          _selectedData = value;
          RaisePropertyChanged("SelectedData");
        }
      }
    }

    #region Search
    public ICommand SearchCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => DoSearch(), () => UserOperateMode == OperateMode.Query,
            o => DoSearchCompleted()
            );
      }
    }

    private void DoSearch()
    {
      var proxyEx = GetExProxy<P19ExDataSource>();
      DataList = proxyEx.CreateQuery<F91000302SearchData>("GetF91000302Data")
           .AddQueryOption("itemTypeId", string.Format("'{0}'", ItemTypeId))
           .AddQueryOption("accUnit", string.Format("'{0}'", AccUnit))
           .AddQueryOption("accUnitName", string.Format("'{0}'", AccUnitName)).ToList();

      if (DataList == null || DataList.Count.Equals(0))
      {
        ShowMessage(Messages.InfoNoData);
        return;
      }
    }

    private void DoSearchCompleted()
    {
      if (DataList == null || !DataList.Any()) return;
      SelectedData = DataList.FirstOrDefault();
      SearchAction();
    }

    #endregion Search

    #region Add
    public ICommand AddCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => DoAdd(), () => UserOperateMode == OperateMode.Query, o => DoAddCompleted()
            );
      }
    }

    private void DoAdd()
    {

    }

    private void DoAddCompleted()
    {
      var newItem = new F91000302SearchData();
      newItem.CRT_DATE = DateTime.Now;
      DataList.Add(newItem);
      DataList = DataList.ToList();
      SelectedData = newItem;
      AddAction();
      UserOperateMode = OperateMode.Add;
    }
    #endregion Add

    #region Edit
    public ICommand EditCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => DoEdit(), () => UserOperateMode == OperateMode.Query && (SelectedData != null && DataList.Any()), o => DoEditCompleted()
            );
      }
    }

    private void DoEdit()
    {
      //執行編輯動作
    }

    private void DoEditCompleted()
    {
      EditAction();
      UserOperateMode = OperateMode.Edit;
    }
    #endregion Edit

    #region Cancel
    public ICommand CancelCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
                            o => DoCancel(), () => UserOperateMode != OperateMode.Query, p => DoCancelCompleted()
                            );
      }
    }

    private void DoCancel()
    {

    }

    private void DoCancelCompleted()
    {
      if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
      {
        var resetSelectedData = false;
        if (UserOperateMode == OperateMode.Add && DataList.Any())
        {
          //DataList.RemoveAt(DataList.Count - 1);
          resetSelectedData = true;
        }

        UserOperateMode = OperateMode.Query;
        if (resetSelectedData && DataList.Any())
          SelectedData = DataList.First();
        SearchAction();

        DoSearch();
      }
      else
      {
        if (UserOperateMode == OperateMode.Edit)
          DoEditCompleted();
        else
        {
          //UserOperateMode = OperateMode.Add;                  
          //DoAddCompleted();
        }

      }
    }
    #endregion Cancel

    #region Delete
    public ICommand DeleteCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedData != null,
            o => DoDeleteCompleted()
            );
      }
    }

    private void DoDelete()
    {
      if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;


      var pier = (from a in _proxy.F91000302s
                  where a.ACC_UNIT.Equals(SelectedData.ACC_UNIT) && a.ITEM_TYPE_ID.Equals(SelectedData.ITEM_TYPE_ID)
                  select a).FirstOrDefault();
      if (pier == null)
      {
        DialogService.ShowMessage(Properties.Resources.P1920020000_ACC_UNIT_NoExist);
        return;
      }
      else
      {
        _proxy.DeleteObject(pier);
      }
      _proxy.SaveChanges();
      ShowMessage(Messages.DeleteSuccess);
    }

    private void DoDeleteCompleted()
    {
      UserOperateMode = OperateMode.Query;
      SearchCommand.Execute(null);
    }
    #endregion Delete

    #region Save
    public ICommand SaveCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => DoSave(), () => UserOperateMode != OperateMode.Query, p => DoSaveCompleted()
            );
      }
    }

    private void DoSave()
    {
      isValid = true;

      if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes)
      {
        isValid = false;
        return;
      }

      if (string.IsNullOrEmpty(SelectedData.ITEM_TYPE_ID) || string.IsNullOrWhiteSpace(SelectedData.ITEM_TYPE_ID))
      {
        isValid = false;
        DialogService.ShowMessage(Properties.Resources.P1920020000_InputItemType);
        return;
      }

      if (string.IsNullOrEmpty(SelectedData.ACC_UNIT) || string.IsNullOrWhiteSpace(SelectedData.ACC_UNIT))
      {
        isValid = false;
        DialogService.ShowMessage(Properties.Resources.P1920020000_IntpuAccUnit);
        return;
      }

      if (string.IsNullOrEmpty(SelectedData.ACC_UNIT_NAME) || string.IsNullOrWhiteSpace(SelectedData.ACC_UNIT_NAME))
      {
        isValid = false;
        DialogService.ShowMessage(Properties.Resources.P1920020000_InputAccUnit);
        return;
      }

      if (UserOperateMode == OperateMode.Add)
      {
        var pier =
          _proxy.F91000302s.Where(
            x =>
              x.ACC_UNIT.ToLower().Equals(SelectedData.ACC_UNIT.ToLower()) &&
              x.ITEM_TYPE_ID.ToLower().Equals(SelectedData.ITEM_TYPE_ID.ToLower()))
            .ToList().Count();
        if (pier != 0)
        {
          isValid = false;
          DialogService.ShowMessage(Properties.Resources.P1920020000_ACC_UNIT_Duplicate);
          return;
        }
      }

      //執行確認儲存動作
      if (UserOperateMode == OperateMode.Add)
        DoSaveAdd();
      else if (UserOperateMode == OperateMode.Edit)
        DoSaveEdit();
    }

    private void DoSaveAdd()
    {
      var f91000302 = new F91000302();

      f91000302.ITEM_TYPE_ID = SelectedData.ITEM_TYPE_ID;
      f91000302.ACC_UNIT = SelectedData.ACC_UNIT;
      f91000302.ACC_UNIT_NAME = SelectedData.ACC_UNIT_NAME.Trim();
      _proxy.AddToF91000302s(f91000302);
      _proxy.SaveChanges();
      ShowMessage(Messages.Success);
    }

    private void DoSaveEdit()
    {
      var f91000302s =
        _proxy.F91000302s.Where(x => x.ACC_UNIT == SelectedData.ACC_UNIT && x.ITEM_TYPE_ID == SelectedData.ITEM_TYPE_ID)
          .ToList();
      var f91000302 = f91000302s.FirstOrDefault();

      if (f91000302 != null)
      {
        f91000302.ACC_UNIT_NAME = SelectedData.ACC_UNIT_NAME;
        f91000302.UPD_STAFF = _userId;
        f91000302.UPD_NAME = _userName;
        f91000302.UPD_DATE = DateTime.Now;
        _proxy.UpdateObject(f91000302);
        _proxy.SaveChanges();
        ShowMessage(Messages.Success);
      }
      else
      {
        ShowMessage(Messages.Failed);
      }
    }

    private void DoSaveCompleted()
    {
      if (isValid == true)
      {
        UserOperateMode = OperateMode.Query;
        SearchCommand.Execute(null);
      }
    }
    #endregion Save

    public bool IsValidPireCode(String pireCode)
    {
      System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$");
      return reg1.IsMatch(pireCode);
    }
    public bool IsValidTempArea(String tempArea)
    {
      System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^\+?[1-9][0-9]*$");
      return reg1.IsMatch(tempArea);
    }

  }

}
