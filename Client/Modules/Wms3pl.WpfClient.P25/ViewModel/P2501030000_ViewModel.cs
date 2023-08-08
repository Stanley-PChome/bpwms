using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P25ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using P01Wcf = Wms3pl.WpfClient.ExDataServices.P01WcfService;
using P25Wcf = Wms3pl.WpfClient.ExDataServices.P25WcfService;
using ExP25Wcf = Wms3pl.WpfClient.ExDataServices.P25ExDataService;
namespace Wms3pl.WpfClient.P25.ViewModel
{
  public partial class P2501030000_ViewModel : InputViewModelBase
  {

    #region Init Data
    public P2501030000_ViewModel()
    {
      if (!IsInDesignMode)
      {
        var globalInfo = Wms3plSession.Get<GlobalInfo>();

        DcList = globalInfo.DcCodeList;

        ReloadSnStatus();
      }

    }
    #endregion

    #region Property

    #region ExcelImport
    public Action ExcelImport = delegate { };
    #endregion

    #region ImportFilePath
    public string ImportFilePath { get; set; }
    #endregion

    #region DC
    private List<NameValuePair<string>> _dcList = new List<NameValuePair<string>>();

    public List<NameValuePair<string>> DcList
    {
      get { return _dcList; }
      set
      {
        _dcList = value;
        RaisePropertyChanged("DcList");
      }
    }

    #endregion

    #region WareHouse(倉別)
    /*
		 * 註解 WareHouse(倉別) :
		 * 當序號是綁儲位，倉別才需要紀錄
		 */
    private List<NameValuePair<string>> _wareHouseList = new List<NameValuePair<string>>();

    public List<NameValuePair<string>> WareHouseList
    {
      get { return _wareHouseList; }
      set
      {
        _wareHouseList = value;
        RaisePropertyChanged("WareHouseList");
      }
    }

    #endregion

    #region 序號狀態

    /// <summary>
    /// 可編輯的狀態只有調入
    /// </summary>
    private string[] CanEditStatus = new string[] { "A1" };

    private List<NameValuePair<string>> _snStatusList;

    public List<NameValuePair<string>> SnStatusList
    {
      get
      {
        if (UserOperateMode == OperateMode.Add)
        {
          // 新增模式時，狀態只顯示調入
          return _snStatusList.Where(x => x.Value == "A1").ToList();
        }
        else if (UserOperateMode == OperateMode.Edit)
        {
          // 編輯時，只顯示調入與
          return _snStatusList.Where(x => CanEditStatus.Contains(x.Value)).ToList();
        }
        else
        {
          // 查詢時，全部顯示作為顯示用
          return _snStatusList;
        }
      }
      set
      {
        _snStatusList = value;
        RaisePropertyChanged("SnStatusList");
      }
    }
    #endregion

    #region 供應商編號
    private string _vnrName;
    public string VnrName
    {
      get { return _vnrName; }
      set
      {
        _vnrName = value;
        RaisePropertyChanged("VnrName");
      }
    }
    #endregion

    private string _searchSerialNo = string.Empty;

    public string SearchSerialNo
    {
      get { return _searchSerialNo; }
      set
      {
        Set(() => SearchSerialNo, ref _searchSerialNo, value);
      }
    }

    private F2501WcfData _selectedF2501WcfData;

    public F2501WcfData SelectedF2501WcfData
    {
      get { return _selectedF2501WcfData; }
      set
      {
        Set(() => SelectedF2501WcfData, ref _selectedF2501WcfData, value);
      }
    }


    private F2501WcfData _editableF2501WcfData;
    public F2501WcfData EditableF2501WcfData
    {
      get { return _editableF2501WcfData; }
      set
      {
        Set(() => EditableF2501WcfData, ref _editableF2501WcfData, value);
      }
    }


    #region DataGrid

    private List<P25Wcf.F250103Verification> _f250103VerificationData;

    public List<P25Wcf.F250103Verification> F250103VerificationData
    {
      get { return _f250103VerificationData; }
      set
      {
        _f250103VerificationData = value;
        RaisePropertyChanged("F250103VerificationData");
      }
    }
    #endregion
    public string CustCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
    #endregion

    #region Search
    public ICommand SearchCommand
    {
      get
      {
        string errorMsg = string.Empty;
        return CreateBusyAsyncCommand(
          o => errorMsg = DoSearch(),
          () => UserOperateMode == OperateMode.Query && !string.IsNullOrWhiteSpace(SearchSerialNo),
          o =>
          {
            RaisePropertyChanged(() => WareHouseList);
            F250103VerificationData = null;
          }
          );
      }
    }

    string ValidateSearchCondition(string serialNo)
    {
      if (string.IsNullOrWhiteSpace(serialNo) || serialNo == string.Empty)
      {
        return Properties.Resources.P2501030000_ViewModel_serialNo_Required;
      }

      if (!CheckSerialNo(serialNo))
      {
        return Properties.Resources.P2501030000_ViewModel_serialNoError;
      }

      return string.Empty;
    }

    string ValidateSerachResult(F2501WcfData f2501WcfData)
    {
      if (f2501WcfData == null)
        return Properties.Resources.P2501030000_ViewModel_serialNoNotExist;

      return string.Empty;
    }

    private string DoSearch()
    {
      SelectedF2501WcfData = null;
      EditableF2501WcfData = null;

      var serialNo = SearchSerialNo = SearchSerialNo.Trim(); ;

      var errorMsg = ValidateSearchCondition(serialNo);
      if (!string.IsNullOrEmpty(errorMsg))
      {
        ShowWarningMessage(errorMsg);
        return errorMsg;
      }

      //執行查詢動作

      var globalInfo = Wms3plSession.Get<GlobalInfo>();
      var f2501WcfData = GetF2501Data( globalInfo.GupCode , globalInfo.CustCode, serialNo);
      errorMsg = ValidateSerachResult(f2501WcfData);
      if (!string.IsNullOrEmpty(errorMsg))
      {
        ShowWarningMessage(errorMsg);
        return errorMsg;
      }

      SelectedF2501WcfData = f2501WcfData;
      if (SelectedF2501WcfData != null)
      {
        SelectedF2501WcfData.DC_CODE = DcList.First().Value;
        EditableF2501WcfData = SelectedF2501WcfData.Clone();
      }

      return string.Empty;
    }
    #endregion Search

    #region Add
    public ICommand AddCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoAdd(),
          () => UserOperateMode == OperateMode.Query
          );
      }
    }

    private void DoAdd()
    {
      F250103VerificationData = null;

      UserOperateMode = OperateMode.Add;
      //執行新增動作
      var globalInfo = Wms3plSession.Get<GlobalInfo>();
      EditableF2501WcfData = new F2501WcfData
      {
        GUP_CODE = globalInfo.GupCode,
        CUST_CODE = globalInfo.CustCode,
        DC_CODE = DcList.Select(x => x.Value).FirstOrDefault(),
        VALID_DATE = new DateTime(9999,12,31),
        STATUS = SnStatusList.Select(x => x.Value).FirstOrDefault(),
        WAREHOUSE_ID = WareHouseList.Select(x => x.Value).FirstOrDefault()
      };
      VnrName = "";

      RaisePropertyChanged(() => SnStatusList);
    }
    #endregion Add

    #region Edit
    public ICommand EditCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoEdit(),
          () => UserOperateMode == OperateMode.Query && SelectedF2501WcfData != null,
          o =>
          {
            RaisePropertyChanged(() => SnStatusList);
          }
          );
      }
    }

    private void DoEdit()
    {
      if (SelectedF2501WcfData == null)
      {
        F250103VerificationData = null;
      }

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
          o => DoCancel(),
          () => UserOperateMode != OperateMode.Query,
          o =>
          {
            RaisePropertyChanged(() => WareHouseList);
          }
          );
      }
    }

    private void DoCancel()
    {
      if (ShowMessage(Messages.WarningBeforeCancel) != DialogResponse.Yes)
        return;

      UserOperateMode = OperateMode.Query;
      RaisePropertyChanged(() => SnStatusList);

      EditableF2501WcfData = null;
      if (SelectedF2501WcfData != null)
      {
        EditableF2501WcfData = SelectedF2501WcfData.Clone();
      }
      VnrName = string.IsNullOrWhiteSpace(EditableF2501WcfData?.VNR_CODE) ? "" : GetVnrName(EditableF2501WcfData.VNR_CODE, EditableF2501WcfData.GUP_CODE);

    }
    #endregion Cancel

    #region Delete
    public ICommand DeleteCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
          o => DoDelete(),
          () => UserOperateMode == OperateMode.Query
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
        bool isSaved = false;
        return CreateBusyAsyncCommand(
          o => isSaved = DoSave(),
          () => CanSave(),
          o =>
          {
            if (isSaved)
            {
              UserOperateMode = OperateMode.Query;
              if (SelectedF2501WcfData != null)
                SearchSerialNo = SelectedF2501WcfData.SERIAL_NO;
            }
            RaisePropertyChanged(() => SnStatusList);
          }
          );
      }
    }

    string ValidateSave()
    {
      if (EditableF2501WcfData.BUNDLE_SERIALLOC == "1")
        return "序號綁儲位商品，請用異動調整作業處理序號資料";

      if (!string.IsNullOrEmpty(EditableF2501WcfData.VNR_CODE))
      {
        var f1909 = GetProxy<F19Entities>().F1909s.Where(x => x.GUP_CODE == EditableF2501WcfData.GUP_CODE && x.CUST_CODE == EditableF2501WcfData.CUST_CODE).FirstOrDefault();
        var f1908s = GetProxy<F19Entities>().F1908s.Where(x => x.VNR_CODE == EditableF2501WcfData.VNR_CODE
                              && x.GUP_CODE == EditableF2501WcfData.GUP_CODE && x.CUST_CODE == EditableF2501WcfData.CUST_CODE).ToList();
        if (f1909.ALLOWGUP_VNRSHARE == "0")
          f1908s = f1908s.Where(x => x.CUST_CODE == EditableF2501WcfData.CUST_CODE).ToList();

        if (f1908s.Count() == 0)
          return Properties.Resources.P2501030000_ViewModel_VNRCodeNotFound;
      }

      return string.Empty;
    }

    private bool DoSave()
    {
      //執行確認儲存動作

      var errorMsg = ValidateSave();
      if (!string.IsNullOrEmpty(errorMsg))
      {
        ShowWarningMessage(errorMsg);
        return false;
      }

      var result = InsertF2501();
      F250103VerificationData = result;

      if (result.Where(x => x.SerialNo == EditableF2501WcfData.SERIAL_NO && string.IsNullOrEmpty(x.Message)).Any())
      {
        SelectedF2501WcfData = GetF2501Data(EditableF2501WcfData.CUST_CODE, EditableF2501WcfData.GUP_CODE, EditableF2501WcfData.SERIAL_NO);
        if (SelectedF2501WcfData != null)
        {
          SelectedF2501WcfData.DC_CODE = DcList.FirstOrDefault()?.Value;
          EditableF2501WcfData = SelectedF2501WcfData.Clone();
        }

        ShowMessage(Messages.Success);
        return true;
      }

      if (result.Any())
      {
        ShowWarningMessage(result.Select(x => x.Message).First());
      }
      else
      {
        ShowMessage(Messages.Failed);
      }
      return false;
    }
    #endregion Save

    #region Import
    public Action ActionAfterCheckSerialNo = delegate { };

    public ICommand ImportExcelCommand
    {
      get
      {
        return new RelayCommand(() =>
        {
          ExcelImport();
          if (string.IsNullOrWhiteSpace(ImportFilePath)) return;
          DoImportCommand.Execute(ImportFilePath);
        });
      }
    }

    public ICommand DoImportCommand
    {
      get
      {
        return CreateBusyAsyncCommand(o =>
        {
          string fullFilePath = o.ToString();
          DoImport(fullFilePath);
        });
      }
    }

    public void DoImport(string fullFilePath)
    {
      var msg = new MessagesStruct()
      {
        Button = DialogButton.OK,
        Image = DialogImage.Information,
        Message = Properties.Resources.P2501030000_ViewModel_SaveSuccess,
        Title = Resources.Resources.Information
      };
      DoAdd();
      //EditableF2501WcfData = new F2501WcfData { STATUS = SnStatusList.Select(x => x.Value).FirstOrDefault(), };
      var errorMeg = string.Empty;
      var excelTable = DataTableHelper.ReadExcelDataTable(fullFilePath, ref errorMeg);
      if (excelTable != null)
      {
        if (excelTable.Rows.Count > 100)
        {
          ShowWarningMessage("Excel匯入檔每次最多只能100筆資料");
          F250103VerificationData = null;   //#2149-1 
          return;
        }
        var dataTableColumnName = new List<string>
        {
          Properties.Resources.P2501030000_ViewModel_GUP_Code,
          Properties.Resources.P2501030000_ViewModel_Cust_Code,
          Properties.Resources.P2501030000_ViewModel_Item_Code,
          Properties.Resources.P2501030000_ViewModel_WMS_NO,
          Properties.Resources.P2501030000_ViewModel_SerialNo,
          Properties.Resources.P2501030000_ViewModel_SerialStatus,
          Properties.Resources.P2501030000_ViewModel_Supply,
          Properties.Resources.P2501030000_ViewModel_ValidDate,
        };//該EXCEL 應有的Table Column Name
        var check = excelTable.Columns.Cast<DataColumn>()
          .All(column => dataTableColumnName.Contains(column.ColumnName.Trim()));
        if (!check)
        {
          msg = new MessagesStruct() { Message = Properties.Resources.P2501030000_ViewModel_ImportTableFormatError, Button = DialogButton.OKCancel, Image = DialogImage.Warning, Title = Resources.Resources.Information };
          ShowMessage(msg);
          F250103VerificationData = null;   //#2149-1 
          return;
        }

        var list = (from col in excelTable.AsEnumerable()
                    let vnrCodeText = Convert.ToString(col[6])
                    let vnrCode = string.IsNullOrWhiteSpace(vnrCodeText) ? "" : vnrCodeText
                    select new P25Wcf.F2501WcfData
                    {
                      GUP_CODE = Convert.ToString(col[0]),
                      CUST_CODE = Convert.ToString(col[1]),
                      ITEM_CODE = Convert.ToString(col[2])?.ToUpper(),
                      WMS_NO = Convert.ToString(col[3]),
                      SERIAL_NO = Convert.ToString(col[4])?.ToUpper(),
                      STATUS = Convert.ToString(col[5]),
                      VNR_CODE = vnrCode,
                      VALID_DATE = new DateTime(9999, 12, 31)
                    }).ToList();

        var globalInfo = Wms3plSession.Get<GlobalInfo>();
        var gupCode = globalInfo.GupCode;
        if (list.Any(x => x.GUP_CODE != gupCode))
        {
          ShowWarningMessage(string.Format(Properties.Resources.P2501030000_ViewModel_ImportGUPCode_Different, gupCode));
          F250103VerificationData = null;   //#2149-1 

          return;
        }
        var custCode = globalInfo.CustCode;
        if (list.Any(x => x.CUST_CODE != custCode))
        {
          ShowWarningMessage(string.Format(Properties.Resources.P2501030000_ViewModel_ImportCUSTCode_Different, custCode));
          F250103VerificationData = null;   //#2149-1 
          return;
        }

        if (list.Any(x => !CanEditStatus.Contains(x.STATUS)))
        {
          ShowWarningMessage(Properties.Resources.P2501030000_ViewModel_ImportSTATUS_Invalid);
          F250103VerificationData = null;   //#2149-1 
          return;
        }


        var repeatSerails = new List<string>();
        //var OverLengthSerails = new List<string>();
        
        bool checkSnDuplicate;
        foreach (var item in list)
        {
          checkSnDuplicate = false;
   
          //自己檢查自己，如果>1才算是重複
          if (list.Count(x => x.SERIAL_NO.ToUpper() == item.SERIAL_NO.ToUpper()) > 1)
            checkSnDuplicate = true;

          if (checkSnDuplicate && !repeatSerails.Any(x => x.ToUpper() == item.SERIAL_NO.ToUpper()))
            repeatSerails.Add(item.SERIAL_NO);
          //if( item.SERIAL_NO.Length>50)
          //  OverLengthSerails.Add(item.SERIAL_NO);
        }     
        var repeatSerailNo = string.Join(Environment.NewLine, repeatSerails);
        if (!string.IsNullOrEmpty(repeatSerailNo))
        {
          ShowWarningMessage(string.Format("序號重複:\r\n{0}", repeatSerailNo));
          F250103VerificationData = null;   //#2149-1 若滙入重複則清除 上筆滙入結果.
       
          return ;
        }
        //var OverLengthSerailNo = string.Join(Environment.NewLine, OverLengthSerails);
        //if (!string.IsNullOrEmpty(OverLengthSerailNo))
        //{
        //  ShowWarningMessage(string.Format("序號長度過長:\r\n{0}", OverLengthSerailNo));
        //  F250103VerificationData = null;   //#2149-1 
        
        //  return;
        //}


        F250103VerificationData = InsertF2501S(list);

        SelectedF2501WcfData = null;
        //EditableF2501WcfData = null;
      }
      else if (string.IsNullOrWhiteSpace(errorMeg))
      {
        msg = new MessagesStruct() { Message = Properties.Resources.P2501030000_ViewModel_NoData, Button = DialogButton.OK, Image = DialogImage.Warning, Title = Resources.Resources.Information };
        ShowMessage(msg);
      }
      else
      {
        msg = new MessagesStruct() { Button = DialogButton.OKCancel, Image = DialogImage.Warning, Message = errorMeg, Title = Resources.Resources.Information };
        ShowMessage(msg);
      }
    }


    #endregion Save

    public ICommand CheckBundleSeriallocCommand
    {
      get
      {
        return CreateBusyAsyncCommand(o =>
        {
          if (EditableF2501WcfData.Trim() == null || string.IsNullOrEmpty(EditableF2501WcfData.ITEM_CODE))
            return;

          var f1903 = GetProxy<F19Entities>().F1903s.Where(x => x.ITEM_CODE == EditableF2501WcfData.ITEM_CODE
                                    && x.GUP_CODE == EditableF2501WcfData.GUP_CODE
                                    && x.CUST_CODE == EditableF2501WcfData.CUST_CODE)
                                .FirstOrDefault();
          if (f1903 == null)
          {
            ShowMessage(Messages.InfoNoData);
            return;
          }

          EditableF2501WcfData.BUNDLE_SERIALLOC = f1903.BUNDLE_SERIALLOC;
          EditableF2501WcfData.LOC_MIX_ITEM = f1903.LOC_MIX_ITEM;
        });
      }
    }

    #region Check(檢核)

    /// <summary>
    /// [儲位]不在[倉別]提示訊息
    /// </summary>
    public string StoreNotInWareHouse_MessageShow = Properties.Resources.P2501030000_ViewModel_StoreNotInWareHouse;

    /// <summary>
    /// 檢查[儲位]是否在[倉別]裡
    /// </summary>
    public bool CheckStoreInWareHouse(string dcCode, string locCode, string wareHouseId)
    {
      locCode = (locCode ?? string.Empty).Replace("-", string.Empty).Trim();

      var proxy = GetProxy<F19Entities>();
      var data = (from o in proxy.F1912s
                  where o.DC_CODE == dcCode &&
                        o.LOC_CODE == locCode &&
                        o.WAREHOUSE_ID == wareHouseId
                  select o).FirstOrDefault();
      //ShowMessage("此儲位不存在於目前物流中心與倉別");
      return data != null;
    }

    public ICommand CheckStoreInWareHouseCommand
    {
      get
      {
        return CreateBusyAsyncCommand(o =>
        {
          if (string.IsNullOrEmpty(EditableF2501WcfData.DC_CODE)
           || string.IsNullOrEmpty(EditableF2501WcfData.LOC_CODE)
           || string.IsNullOrEmpty(EditableF2501WcfData.WAREHOUSE_ID))
          {
            return;
          }

          var check = CheckStoreInWareHouse(EditableF2501WcfData.DC_CODE,
                          EditableF2501WcfData.LOC_CODE,
                          EditableF2501WcfData.WAREHOUSE_ID);
          if (!check)
          {
            ShowWarningMessage(StoreNotInWareHouse_MessageShow);
          }
        });
      }
    }

    /// <summary>
    /// 檢查[供應商]存在
    /// </summary>
    private string GetVnrName(string vnrCode, string gupCode)
    {
      var proxy = GetProxy<F19Entities>();
      var data = (from i in proxy.F1908s
                  where i.GUP_CODE == gupCode &&
                        i.VNR_CODE == vnrCode
                  select i
                  ).FirstOrDefault();
      if (data != null)
      {
        return data.VNR_NAME;
      }

      return string.Empty;
    }

    /// <summary>
    /// 檢查條碼是否是序號
    /// </summary>
    private bool CheckSerialNo(string barcode)
    {
      var code = GetBarcodeInspection(
        Wms3plSession.Get<GlobalInfo>().GupCode,
        Wms3plSession.Get<GlobalInfo>().CustCode,
        barcode);

      var check = false;
      switch (code)
      {
        case P01Wcf.BarcodeType.SerialNo://序號
          check = true;
          break;
        case P01Wcf.BarcodeType.None://非確認條碼
        case P01Wcf.BarcodeType.BatchNo://儲值卡盒號
        case P01Wcf.BarcodeType.BoxSerial://盒號
        case P01Wcf.BarcodeType.CaseNo://箱號
        default:/*非存在條碼、儲值卡盒號、盒號、箱號*/
          break;
      }
      return check;
    }

    //物流中心、業主、貨主、商品編號、序號、序號狀態
    private bool CanSave()
    {
      if (UserOperateMode == OperateMode.Query || EditableF2501WcfData == null)
        return false;

      return (!string.IsNullOrWhiteSpace(EditableF2501WcfData.ITEM_CODE)
          && !string.IsNullOrWhiteSpace(EditableF2501WcfData.SERIAL_NO)
          && !string.IsNullOrWhiteSpace(EditableF2501WcfData.STATUS));
    }

    private bool CanImport()
    {
      if (UserOperateMode == OperateMode.Add )
        return true;

      return false;
    }

    #endregion

    #region Data

    private NameValuePair<string> _selectedDcItem;

    public NameValuePair<string> SelectedDcItem
    {
      get { return _selectedDcItem; }
      set
      {
        Set(() => SelectedDcItem, ref _selectedDcItem, value);

        if (value == null)
          WareHouseList = new List<NameValuePair<string>>();
        else
          ReloadWareHouse(value.Value);
      }
    }


    /// <summary>
    /// 重新載入[倉別]資料 - 選單類別
    /// </summary>
    private void ReloadWareHouse(string dcCode)
    {
      var proxy = GetProxy<F19Entities>();
      WareHouseList = (from o in proxy.F1980s
                       where o.DC_CODE == dcCode
                       select new NameValuePair<string>
                       {
                         Name = o.WAREHOUSE_NAME,
                         Value = o.WAREHOUSE_ID
                       }).ToList();
    }
    /// <summary>
    /// 重新載入[序號狀態]資料 - 選單類別
    /// </summary>
    private void ReloadSnStatus()
    {
      SnStatusList = GetF000904("F2501", "STATUS");
    }

    private P01Wcf.BarcodeType GetBarcodeInspection(string gupCode, string custCode, string barcode)
    {
      var proxy = new P01Wcf.P01WcfServiceClient();
      var result = RunWcfMethod<P01Wcf.BarcodeData>(
        proxy.InnerChannel,
        () => proxy.BarcodeInspection(gupCode, custCode, barcode)
      );
      return result.Barcode;
    }

    private ExDataServices.P25ExDataService.F2501WcfData GetF2501Data(string custCode, string gupCode, string serialNo)
    {
      //var porxyEx = GetExProxy<P25ExDataSource>();
      //var f2501Data = porxyEx.CreateQuery<F2501WcfData>("GetF2501Data")
      //            .AddQueryOption("custCode", QueryFormat(custCode))
      //            .AddQueryOption("gupCode", QueryFormat(gupCode))
      //            .AddQueryOption("serialNo", QueryFormat(serialNo))
      //            .ToList()
      //            .FirstOrDefault();

      var wcfproxy = GetWcfProxy<P25Wcf.P25WcfServiceClient>();
      var f2501Data = wcfproxy.RunWcfMethod(w => w.GetF2501Data(custCode, gupCode, serialNo));

   
      VnrName = (f2501Data == null) ? string.Empty : GetVnrName(f2501Data.VNR_CODE, f2501Data.GUP_CODE);

      ExDataServices.P25ExDataService.F2501WcfData result = null;
      result= f2501Data==null? null : ExDataMapper.Map<ExDataServices.P25WcfService.F2501WcfData, F2501WcfData>(f2501Data);
      return result;
    }
    /// <summary>
    /// 取得[序號狀態] - 選單類別
    /// </summary>
    private List<NameValuePair<string>> GetF000904(string topic, string subtopic)
    {
      return GetBaseTableService.GetF000904List(FunctionCode, topic, subtopic);
    }

    public List<P25Wcf.F250103Verification> InsertF2501()
    {
      var globalInfo = Wms3plSession.Get<GlobalInfo>();
      EditableF2501WcfData.CUST_CODE = globalInfo.CustCode;
      EditableF2501WcfData.GUP_CODE = globalInfo.GupCode;
      return InsertF2501S(new List<P25Wcf.F2501WcfData> { EditableF2501WcfData.Map<F2501WcfData, P25Wcf.F2501WcfData>() });
    }

    public List<P25Wcf.F250103Verification> InsertF2501S(List<P25Wcf.F2501WcfData> data)
    {
      var sw = new Stopwatch();
      sw.Restart();

      var proxy = new P25Wcf.P25WcfServiceClient();

      var result = RunWcfMethod<List<P25Wcf.F250103Verification>>(
        proxy.InnerChannel,
        () => proxy.InsertUpdateF2501Data(data.Select(x => ExDataMapper.Trim(x)).ToArray()).ToList()
      );

      sw.Stop();
      System.Diagnostics.Debug.WriteLine($"花費時間:{sw.ElapsedMilliseconds / 1000m}");
      return result;
    }

    private string GetItemCodeBySerialNo(string dcCode, string gupCode, string custCode, string serialNo)
    {
      var proxy = new P01Wcf.P01WcfServiceClient();
      var result = RunWcfMethod<string>(
        proxy.InnerChannel,
        () => proxy.GetItemCodeBySerialNo(dcCode, gupCode, custCode, serialNo)
      );
      return result;
    }

    #endregion
  }
}


