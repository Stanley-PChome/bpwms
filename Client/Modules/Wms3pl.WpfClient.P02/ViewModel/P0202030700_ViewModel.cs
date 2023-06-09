using AutoMapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;

namespace Wms3pl.WpfClient.P02.ViewModel
{
  public class P0202030700_ViewModel : InputViewModelBase
  {
    public Action OnAddScanInputComplete = delegate { };
    #region Property
    public Action ExitClick = delegate { };
    public Action CancelComplete = delegate { };
    public Action SaveComplete = delegate { };
    #region 商品編號
    private string _itemCode;

    public string ItemCode
    {
      get { return _itemCode; }
      set
      {
        _itemCode = value;
        RaisePropertyChanged("ItemCode");
        //StockList = GetUserWarehouse(BaseData.DC_CODE, true);
      }
    }
    #endregion

    #region 商品名稱
    private string _itemName;
    public string ItemName
    {
      get { return _itemName; }
      set
      {
        _itemName = value;
        RaisePropertyChanged("ItemName");
      }
    }
    #endregion

    #region 效期
    private DateTime? _valiDate;
    public DateTime? ValiDate
    {
      get { return _valiDate; }
      set
      {
        _valiDate = value;
        RaisePropertyChanged("ValiDate");
      }
    }
    #endregion




    #region 批號
    private string _makeNo;
    public string MakeNo
    {
      get { return _makeNo; }
      set
      {
        _makeNo = value;
        RaisePropertyChanged("MakeNo");
      }
    }
    #endregion

    #region 驗收總量
    public int? _recvQty;
    public int? RecvQty
    {
      get { return _recvQty; }
      set
      {
        _recvQty = value;
        RaisePropertyChanged("MakeNo");
      }
    }
    #endregion

    #region 系統模式
    /// <summary>
    /// 系統模式 1:商品檢驗 2:複驗異常處理
    /// </summary>
    public int SysMode;
    #endregion 系統模式

    #endregion
    #region 
    private string _userId = Wms3plSession.Get<UserInfo>().Account;
    #endregion

    #region 貨主編號
    private string _dcCode;
    public string DcCode
    {
      get { return _dcCode; }
      set
      {
        _dcCode = value;
        RaisePropertyChanged("DcCode");
      }

    }
    #endregion

    #region 貨主編號
    public string CustCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
    #endregion

    #region 業主編號
    public string GupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
    #endregion

    #region Data - 基本資料
    private P020203Data _baseData;
    public P020203Data BaseData
    {
      get { return _baseData; }
      set
      {
        _baseData = value;
        RaisePropertyChanged("BaseData");
      }
    }
    #endregion

    #region 倉庫清單
    private List<NameValuePair<string>> _warehouseList;

    public List<NameValuePair<string>> WarehouseList
    {
      get { return _warehouseList; }
      set
      {
        _warehouseList = value;
        RaisePropertyChanged("WarehouseList");
        //StockList = GetUserWarehouse(BaseData.DC_CODE, true);
      }
    }
    #endregion

    #region 選擇倉庫
    private string _selectedWarehouse;
    public string SelectedWarehouse
    {
      get { return _selectedWarehouse; }
      set
      {
        _selectedWarehouse = value;
        RaisePropertyChanged("SelectedWarehouse");
      }
    }
    #endregion

    #region 原因清單
    private List<NameValuePair<string>> _causeList;

    public List<NameValuePair<string>> CauseList
    {
      get { return _causeList; }
      set
      {
        _causeList = value;
        RaisePropertyChanged("CauseList");
      }
    }
    #endregion

    #region 選擇原因
    private string _selectedUccCode;
    public string SelectedUccCode
    {
      get { return _selectedUccCode; }
      set
      {
        _selectedUccCode = value;
        RaisePropertyChanged("SelectedUccCode");
        //if (SelectedUccCode == "999")
        //{
        //    EnableOtherCause = false;
        //}
        //else
        //{
        //    OtherCause = null;
        //    EnableOtherCause = true;
        //}
      }
    }
    #endregion

    #region Form - 語音
    private bool _playSound = true;
    public bool PlaySound
    {
      get { return _playSound; }
      set { _playSound = value; RaisePropertyChanged("PlaySound"); }
    }
    #endregion
    #region Form - 目前檢視的頁籤
    private int _selectedTabIndex = 0;
    public int SelectedTabIndex
    {
      get { return _selectedTabIndex; }
      set
      {
        _selectedTabIndex = value;

        RaisePropertyChanged("SelectedTabIndex");
      }
    }
    #endregion

    #region 不良品數量
    private int? _defectQty;
    public int? DefectQty
    {
      get { return _defectQty; }
      set
      {
        _defectQty = value;
        RaisePropertyChanged("DefectQty");
      }
    }
    #endregion

    #region 序號
    private string _serialNo = "";
    public string SerialNo
    {
      get { return _serialNo; }
      set
      {
        _serialNo = value;
        RaisePropertyChanged("SerialNo");
      }
    }
    #endregion
    #region 原因
    private string _cause;
    public string Cause
    {
      get { return _cause; }
      set
      {
        _cause = value;
        RaisePropertyChanged("Cause");
      }
    }
    #endregion

    #region 其他原因
    private string _otherCause;
    public string OtherCause
    {
      get { return _otherCause; }
      set
      {
        _otherCause = value;
        RaisePropertyChanged("OtherCause");
      }
    }
    #endregion

    #region
    private F02020109Data _selectedItem;
    public F02020109Data SelectedItem
    {
      get { return _selectedItem; }
      set
      {
        _selectedItem = value;
        RaisePropertyChanged("SelectedItem");
      }
    }
    #endregion

    #region Grid資料繫結資料 查詢

    private List<F02020109Data> _f02020109Datas;
    public List<F02020109Data> F02020109Datas
    {
      get { return _f02020109Datas; }
      set
      {
        _f02020109Datas = value;
        RaisePropertyChanged("F02020109Datas");
      }
    }

    private ObservableCollection<F02020109Data> _f02020109Data;

    public ObservableCollection<F02020109Data> F02020109Data
    {
      get { return _f02020109Data; }
      set
      {
        _f02020109Data = value;
        RaisePropertyChanged("F02020109Data");
      }
    }

    public List<F02020109Data> _tempF02020109Datas;

    #region Grid資料繫節結 新增
    private F02020109Data _addOrModifyF02020109Data;
    public F02020109Data AddOrModifyF02020109Data
    {
      get { return _addOrModifyF02020109Data; }
      set
      {
        _addOrModifyF02020109Data = value;
        RaisePropertyChanged("AddOrModifyF02020109Data");
      }
    }
    #endregion

    #region 進倉單號
    private string _stockNo;
    public string StockNo
    {
      get { return _stockNo; }
      set
      {
        _stockNo = value;
        RaisePropertyChanged("StockNo");
      }
    }
    #endregion
    #region 
    private int _stockSeq;
    public int StockSeq
    {
      get { return _stockSeq; }
      set
      {
        _stockSeq = value;
        RaisePropertyChanged("StockSeq");
      }
    }
    #endregion
    #endregion
    public P0202030700_ViewModel()
    {
      if (!IsInDesignMode)
      {
        //初始化執行時所需的值及資料
        InitControls();
      }

    }

    private void InitControls()
    {
      //UserOperateMode = OperateMode.Edit;
      //DoSearchUccList();
      //GetUserWarehouse(SelectedDc, true);
      DoSearchCauseList();
      SearchCommand.Execute(null);


    }



        #region 倉庫選單
        public void GetUserWarehouse(string dcCode)
        {
						var proxy = GetProxy<F19Entities>();
						var result = proxy.F1980s.Where(x => x.DC_CODE == dcCode && x.WAREHOUSE_TYPE =="R")
							.Select(x => new NameValuePair<string>
							{
								Name = string.Format("{0} {1}",x.WAREHOUSE_ID, x.WAREHOUSE_NAME),
								Value = x.WAREHOUSE_ID
							}).ToList();

            WarehouseList = result;
        }
        #endregion

    #region 取得原因清單
    public void DoSearchCauseList()
    {
      var result = new List<NameValuePair<string>>();
      var proxy = GetProxy<F19Entities>();
      var data = GetProxy<F19Entities>().F1951s.Where(o => o.UCT_ID == "IC").ToList();
      var list = (from o in data
                  select new NameValuePair<string>
                  {
                    Name = o.CAUSE,
                    Value = o.UCC_CODE
                  }).ToList();

      CauseList = list;
    }

    #endregion

    #region 驗證序號是否為序號蒐集內的序號
    bool VerificationSerialNo(string serialNo)
    {
      var proxy = GetProxy<F02Entities>();
      var query = from item in proxy.F020302s
                  where item.DC_CODE == BaseData.DC_CODE
                  && item.GUP_CODE == BaseData.GUP_CODE
                  && item.CUST_CODE == BaseData.CUST_CODE
                  && item.PO_NO == BaseData.SHOP_NO
                  && item.ITEM_CODE == BaseData.ITEM_CODE
                  && item.STATUS == "0"
									&& item.SERIAL_NO == serialNo
                  select item;
      return query.ToList().Any() ? true : false;
    }
		#endregion

		#region 驗證序號是否該進倉的序號
		bool VerificationSerialNo2(string serialNo)
		{
			var proxy = GetProxy<F02Entities>();
			var query = from item in proxy.F02020104s
									where item.DC_CODE == BaseData.DC_CODE
									&& item.GUP_CODE == BaseData.GUP_CODE
									&& item.CUST_CODE == BaseData.CUST_CODE
									&& item.PURCHASE_NO == BaseData.PURCHASE_NO
									&& item.PURCHASE_SEQ == BaseData.PURCHASE_SEQ
									&& item.RT_NO == BaseData.RT_NO
									&& item.ITEM_CODE == BaseData.ITEM_CODE
									&& item.SERIAL_NO == serialNo
									&& item.ISPASS == "1"
									select item;
			return query.ToList().Any() ? true : false;
		}
		#endregion

		#region 驗證序號是否重複輸入
		bool VerificationSerialNoIsRepeact(string serialNo)
    {
      return _tempF02020109Datas.Where(x => x.SERIAL_NO == serialNo && x.ChangeFlag != "D").Any() ? true : false;
    }
    #endregion
    #region
    void SetOperateMode(OperateMode operateMode)
    {
      UserOperateMode = operateMode;
    }
    #endregion

    //#region 故障原因控制
    //private bool _enableOtherCause = true;
    //public bool EnableOtherCause
    //{
    //    get
    //    {
    //        return _enableOtherCause;
    //    }
    //    set
    //    {
    //        _enableOtherCause = value;
    //        RaisePropertyChanged("EnableOtherCause");
    //    }
    //}
    //#endregion

    private F02020109Data _selectedAddOrModifyF02020109Data;
    public F02020109Data SelectedAddOrModifyF02020109Data
    {
      get { return _selectedAddOrModifyF02020109Data; }
      set
      {
        _selectedAddOrModifyF02020109Data = value;
        RaisePropertyChanged("SelectedAddOrModifyF02020109Data");
      }
    }

    #region Commond
    #region Search
    public ICommand SearchCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => DoSearch(),
            () => UserOperateMode == OperateMode.Query,
             o => DoSearchComplete()
            );
      }
    }

    private void DoSearch()
    {
      ///執行查詢動作

    }

    private void DoSearchComplete()
    {
      if (SysMode == 2)
        return;
      var porxyEx = GetExProxy<P02ExDataSource>();
      if (F02020109Datas == null)
        F02020109Datas = porxyEx.CreateQuery<F02020109Data>("GetF02020109Datas")
            .AddQueryExOption("dcCode", DcCode)
            .AddQueryExOption("gupCode", GupCode)
            .AddQueryExOption("custCode", CustCode)
            .AddQueryExOption("stockNo", StockNo)
            .AddQueryExOption("stockSeq", StockSeq)
            .ToList();
      if (F02020109Datas.Any())
      {
        SelectedWarehouse = F02020109Datas.FirstOrDefault().WAREHOUSE_ID;
        F02020109Datas.ForEach(x => x.CAUSE = CauseList.Where(y => y.Value == x.UCC_CODE).FirstOrDefault().Name);

      }

      _tempF02020109Datas = F02020109Datas;
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

    private bool _isAddSuccess;
    public ICommand AddDetailCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => DoAddDetail(),
            () => UserOperateMode == OperateMode.Query,
            o => DoAddDetailComplete()
            );
      }
    }
    public void DoAddDetail()
    {
      _isAddSuccess = false;
      var item = new F02020109Data
      {
        ChangeFlag = "A",
        DC_CODE = DcCode,
        GUP_CODE = GupCode,
        CUST_CODE = CustCode,
        STOCK_NO = StockNo,
        STOCK_SEQ = StockSeq,
        DEFECT_QTY = DefectQty,
        SERIAL_NO = SerialNo,
        UCC_CODE = SelectedUccCode,
        CAUSE = CauseList.Where(x => x.Value == SelectedUccCode).Select(x => x.Name).SingleOrDefault(),
        OTHER_CAUSE = string.IsNullOrWhiteSpace(OtherCause) ? null : OtherCause
      };

      // 先檢查一般商品是否有重複項目
      List<F02020109Data> repectItem = new List<F02020109Data>();
      //_tempF02020109Datas
      //.Where(x => x.DC_CODE == item.DC_CODE
      //					 && x.GUP_CODE == item.GUP_CODE
      //					 && x.CUST_CODE == item.CUST_CODE
      //					 && x.STOCK_NO == item.STOCK_NO
      //					 && x.UCC_CODE == item.UCC_CODE
      //					 && x.OTHER_CAUSE == item.OTHER_CAUSE
      //					 && x.ChangeFlag != "D").ToList();

      // 必須選擇原因
      if (string.IsNullOrEmpty(item.UCC_CODE))
      {
        DialogService.ShowMessage(Properties.Resources.P0202030700_UccCodeIsNull);
        return;
      }

      // 選擇其他，其他原因不得為空
      //if(SelectedUccCode == "999" && string.IsNullOrEmpty(item.OTHER_CAUSE))
      //{
      //    DialogService.ShowMessage(Properties.Resources.P0202030700_OtherCauseIsNull);
      //    return;
      //}
      if (BaseData.BUNDLE_SERIALNO == "0")
      {
        // 檢查不良品數量>0 
        if (item.DEFECT_QTY <= 0 || item.DEFECT_QTY == null)
        {
          DialogService.ShowMessage(Properties.Resources.P0202030700_DefectQtyIsNull);
          return;
        }

      }
      else
      {
        // 檢查序號是否有值
        if (string.IsNullOrWhiteSpace(item.SERIAL_NO))
        {
          DialogService.ShowMessage(Properties.Resources.P0202030700_SerialNoIsNull);
          return;
        }
        // 檢查序號是否為序號蒐集內的序號
        if (SysMode == 1 && !VerificationSerialNo(item.SERIAL_NO))
        {
          DialogService.ShowMessage(Properties.Resources.P0202030700_SerialNoNotExist);
          return;
        }
				// 檢查序號是否為序號蒐集內的序號
				if (SysMode == 2 && !VerificationSerialNo2(item.SERIAL_NO))
				{
					DialogService.ShowMessage("您輸入的序號非此進貨商品序號");
					return;
				}
				// 檢查序號是否重複
				if (VerificationSerialNoIsRepeact(item.SERIAL_NO))
        {
          DialogService.ShowMessage(Properties.Resources.P0202030700_SerialNoIsRepeact);
          return;
        }

        // 驗證序號商品是否有序號重複(過濾序號)
        repectItem = _tempF02020109Datas.Where(x => x.SERIAL_NO == item.SERIAL_NO && x.ChangeFlag == "A").ToList();

        // 序號商品將數量設定為1
        item.DEFECT_QTY = 1;
      }
      if (repectItem.Any())
      {
        DialogService.ShowMessage(Properties.Resources.P0202030700_DefectRepeat);
        return;
      }
      else
      {
        _tempF02020109Datas.Add(item);
        _isAddSuccess = true;
      }

    }

    private void DoAddDetailComplete()
    {
      if (_isAddSuccess == true)
      {
        F02020109Datas = Mapper.Map<List<F02020109Data>>(_tempF02020109Datas);
        //F02020109Datas = _tempF02020109Datas;
        F02020109Datas = _tempF02020109Datas.Where(o => o.ChangeFlag != "D").ToList();
        if (_tempF02020109Datas.Any())
          SelectedItem = F02020109Datas.First();

      }
      //OnAddScanInputComplete();
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
            o => DoCancel(),
            () => UserOperateMode == OperateMode.Query,
            o => CancelComplete()
            );
      }
    }

    private void DoCancel()
    {
      //執行取消動作
      if (ShowMessage(Messages.WarningBeforeExit) == DialogResponse.OK)
      {
        return;
      }
      _tempF02020109Datas = null;
      F02020109Data = null;

    }

    #endregion Cancel

    #region Delete

    public ICommand DeleteCommand
    {
      get
      {
        return CreateBusyAsyncCommand(
            o => DoDelete(),
            () => UserOperateMode == OperateMode.Query,
            o => DeleteComplete()
            );
      }
    }

    private void DoDelete()
    {
      //執行刪除動作
      var item = _tempF02020109Datas.Where(x => x.DC_CODE == SelectedItem.DC_CODE
                                              && x.GUP_CODE == SelectedItem.GUP_CODE
                                              && x.CUST_CODE == SelectedItem.CUST_CODE
                                              && x.STOCK_NO == SelectedItem.STOCK_NO
                                              && x.STOCK_SEQ == SelectedItem.STOCK_SEQ
                                              && x.ID == SelectedItem.ID
                                              && x.CAUSE == SelectedItem.CAUSE
                                              && x.ChangeFlag != "D").FirstOrDefault();
      if (item.ChangeFlag == "A")
        _tempF02020109Datas.Remove(item);
      else
        item.ChangeFlag = "D";
    }

    private void DeleteComplete()
    {
      F02020109Datas = _tempF02020109Datas.Where(o => o.ChangeFlag != "D").ToList();
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
            () => UserOperateMode == OperateMode.Query,
            o =>
            {
              if (isSaved)
                SaveComplete();
            }
            );
      }
    }

    private bool DoSave()
    {
      // 倉庫為必填
      if (string.IsNullOrEmpty(SelectedWarehouse))
      {
        DialogService.ShowMessage(Properties.Resources.P0202030700_WarehouseIsNull);
        return false;
      }

      // 良品數不可超過驗收總量
      if (_tempF02020109Datas.Where(x => x.ChangeFlag != "D").Select(x => x.DEFECT_QTY).Sum() > RecvQty)
      {
        DialogService.ShowMessage(Properties.Resources.P0202030700_OverRecvQty);
        return false;
      }

      // 新增項目一次把所有倉庫編號寫入
      foreach (var item in _tempF02020109Datas)
      {
        item.WAREHOUSE_ID = SelectedWarehouse;
      }

      // 檢查上架倉別溫層或設定不良品倉與該商品溫層是否一致
      var proxyF19 = GetProxy<F19Entities>();
      var itemData = proxyF19.F1903s.Where(x => x.GUP_CODE == BaseData.GUP_CODE &&
                  x.CUST_CODE == BaseData.CUST_CODE &&
                  x.ITEM_CODE == BaseData.ITEM_CODE).FirstOrDefault();
      var warehouseData = proxyF19.F1980s.Where(x => x.DC_CODE == BaseData.DC_CODE && x.WAREHOUSE_ID == SelectedWarehouse).FirstOrDefault();
      var tmprName = GetBaseTableService.GetF000904List(FunctionCode, "F1903", "TMPR_TYPE").Where(x => x.Value == itemData.TMPR_TYPE).FirstOrDefault()?.Name;
      if (itemData.TMPR_TYPE == "01" && !(warehouseData.TMPR_TYPE == "01" || warehouseData.TMPR_TYPE == "02"))
      {
        DialogService.ShowMessage($"品號{BaseData.ITEM_CODE}是{tmprName}溫層，故不可上架到{warehouseData.WAREHOUSE_NAME}倉");
        return false;
      }
      else if (itemData.TMPR_TYPE == "02" && warehouseData.TMPR_TYPE != "02")
      {
        DialogService.ShowMessage($"品號{BaseData.ITEM_CODE}是{tmprName}溫層，故不可上架到{warehouseData.WAREHOUSE_NAME}倉");
        return false;
      }
      else if (itemData.TMPR_TYPE == "03" && warehouseData.TMPR_TYPE != "02")
      {
        DialogService.ShowMessage($"品號{BaseData.ITEM_CODE}是{tmprName}溫層，故不可上架到{warehouseData.WAREHOUSE_NAME}倉");
        return false;
      }
      else if (itemData.TMPR_TYPE == "03" && warehouseData.TMPR_TYPE != "03")
      {
        DialogService.ShowMessage($"品號{BaseData.ITEM_CODE}是{tmprName}溫層，故不可上架到{warehouseData.WAREHOUSE_NAME}倉");
        return false;
      }


      if (ShowMessage(Messages.WarningBeforeUpdate) != DialogResponse.Yes)
      {
        return false;
      }

      if (SysMode == 1)
      {
        //驗證是否為品項序號
        var proxy = new wcf.P02WcfServiceClient();
        var f02020109Datas = ExDataMapper.MapCollection<F02020109Data, wcf.F02020109Data>(_tempF02020109Datas).ToArray();
        var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
                () => proxy.InsertOrUpdateP02020307(f02020109Datas));

        if (!result.IsSuccessed)
        {
          DialogService.ShowMessage(result.Message);
          return false;
        }
        else
        {
          ShowMessage(UserOperateMode == OperateMode.Add ? Messages.InfoAddSuccess : Messages.InfoUpdateSuccess);
          return true;
        }
      }
      else if (SysMode == 2)
      {
        //直接回傳F02020109Datas給呼叫端處理
      }
      return true;
    }
    #endregion Save
    #endregion


  }
}