using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F01DataService;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;

//using Wms3pl.Datas.Shared.Entities;

namespace Wms3pl.WpfClient.P02.ViewModel
{
    public partial class P0202010000_ViewModel : InputViewModelBase
    {

        public Action AfterSave = delegate { };
        public Action OnSearchPurchaseNoComplete = delegate { };
        public Action SearchPurchaseNoByCustOrdNoComplete = delegate { };

        public P0202010000_ViewModel()
        {
            if (!IsInDesignMode)
            {
                //初始化執行時所需的值及資料
                InitControls();
            }
        }

        private void InitControls()
        {
            EmpId = Wms3plSession.Get<UserInfo>().Account;
            EmpName = Wms3plSession.Get<UserInfo>().AccountName;
            DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
            if (DcList.Any())
            {
                SelectedDc = DcList.FirstOrDefault().Value;
                SelectedDcByQueryCondition = DcList.FirstOrDefault().Value;
            }
            SwipeCount = 0;
            SwipeList = new List<Wms3pl.WpfClient.ExDataServices.P01ExDataService.F010201MainData>();
            GetFastTypeList();

        }

        private void GetFastTypeList()
        {
            FastTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F010201", "FAST_PASS_TYPE", true);
        }

        private List<NameValuePair<string>> _fassTypeList;

        public List<NameValuePair<string>> FastTypeList
        {
            get { return _fassTypeList; }
            set
            {
                _fassTypeList = value;
                RaisePropertyChanged("FastTypeList");
            }
        }
        private string _selectedFastType = string.Empty;
        /// <summary>
        /// 選取的物流中心
        /// </summary>
        public string SelectedFastType
        {
            get { return _selectedFastType; }
            set { _selectedFastType = value; }
        }
        private void GetBookingPeriodList()
        {
            BookingPeriodList = GetBaseTableService.GetF000904List(FunctionCode, "F010201", "BOOKING_IN_PERIOD");
        }
        private List<NameValuePair<string>> _bookingPeriodList;

        public List<NameValuePair<string>> BookingPeriodList
        {
            get { return _bookingPeriodList; }
            set
            {
                _bookingPeriodList = value;
                RaisePropertyChanged("BookingPeriodList");
            }
        }

        #region 資料連結/ 頁面參數
        private void PageRaisePropertyChanged()
        {
        }
        private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
        private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }
        #region Form - 可用的DC (物流中心)清單
        private string _selectedDc = string.Empty;
        /// <summary>
        /// 選取的物流中心
        /// </summary>
        public string SelectedDc
        {
            get { return _selectedDc; }
            set { _selectedDc = value; }
        }
        private List<NameValuePair<string>> _dcList;
        /// <summary>
        /// 物流中心清單
        /// </summary>
        public List<NameValuePair<string>> DcList
        {
            get { return _dcList; }
            set { _dcList = value; RaisePropertyChanged("DcList"); }
        }
        #endregion
        #region
        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged("Status");
            }
        }

        private string _whName;
        public string WhName
        {
            get { return _whName; }
            set
            {
                _whName = value;
                RaisePropertyChanged("WhName");
            }
        }
        #endregion

        #region 是否顯示已報到
        private string _showReportCompleted;
        public string ShowReportCompleted
        {
            get { return _showReportCompleted; }
            set
            {
                _showReportCompleted = value;
                RaisePropertyChanged("ShowReportCompleted");
            }
        }
        #endregion
        //private ObservableCollection<Wms3pl.WpfClient.ExDataServices.P01ExDataService.F010202Data> _f010202Datas;
        //public ObservableCollection<Wms3pl.WpfClient.ExDataServices.P01ExDataService.F010202Data> F010202Datas
        //{
        //    get { return _f010202Datas; }
        //    set
        //    {
        //        _f010202Datas = value;
        //        RaisePropertyChanged("F010202Datas");
        //    }
        //}
        private ObservableCollection<Wms3pl.WpfClient.ExDataServices.P01ExDataService.F010201MainData> _f010202Datas;
        public ObservableCollection<Wms3pl.WpfClient.ExDataServices.P01ExDataService.F010201MainData> F010202Datas
        {
            get { return _f010202Datas; }
            set
            {
                _f010202Datas = value;
                RaisePropertyChanged("F010202Datas");
            }
        }

        private bool changeFlag = false;
        #region Form - 進倉單號
        private string _selectedPurchaseNo = string.Empty;
        /// <summary>
        /// 新增資料時需輸入進倉單號
        /// </summary>
        public string SelectedPurchaseNo
        {
            get { return _selectedPurchaseNo; }
            set
            {
                _selectedPurchaseNo = string.IsNullOrWhiteSpace(value) ? value : value.ToUpper();

                RaisePropertyChanged("SelectedPurchaseNo");
                //if (changeFlag == false)
                //{
                //	var f01Proxy = GetProxy<F01Entities>();
                //	//取得進倉單號(有可能刷的是 QRCODE 32碼)
                //	var item = f01Proxy.F010201s.Where(o => o.DC_CODE == SelectedDc && o.GUP_CODE == this._gupCode && o.CUST_CODE == this._custCode
                //					&& (o.STOCK_NO == value || o.CHECK_CODE == value)).FirstOrDefault();

                //	if (item != null)
                //	{
                //		value = item.STOCK_NO;
                //		var ItemStatus = GetBaseTableService.GetF000904List(FunctionCode, "F010201", "STATUS", true).FirstOrDefault(x => x.Value == item.STATUS);
                //		Status = ItemStatus.Name;
                //		ShowReportCompleted = ItemStatus.Value == "1" || ItemStatus.Value == "2" ? "1" : "0";
                //		if (CustOrdNo != item.CUST_ORD_NO)
                //			CustOrdNo = item.CUST_ORD_NO;
                //		//設定快速通關顯示狀態
                //		var sekectFastPassType = GetBaseTableService.GetF000904List(FunctionCode, "F010201", "FAST_PASS_TYPE", true).Where(x => x.Value == item.FAST_PASS_TYPE).FirstOrDefault();
                //		if (sekectFastPassType != null)
                //		{
                //			FastPassType = sekectFastPassType.Name;
                //			ShowFastPassType = sekectFastPassType.Value;
                //		}



                //		//取得進倉單明細
                //		var proxyEx = GetExProxy<Wms3pl.WpfClient.ExDataServices.P01ExDataService.P01ExDataSource>();
                //		F010202Datas = proxyEx.CreateQuery<Wms3pl.WpfClient.ExDataServices.P01ExDataService.F010202Data>("GetF010202DatasMargeValidate")
                //	 .AddQueryExOption("dcCode", SelectedDc)
                //	 .AddQueryExOption("gupCode", this._gupCode)
                //	 .AddQueryExOption("custCode", this._custCode)
                //	 .AddQueryExOption("stockNo", item.STOCK_NO).ToObservableCollection();

                //		// 進倉明細顯示
                //		ItemDetail = Properties.Resources.P0202010000_StockNo + item.STOCK_NO + "、" 
                //			+ Properties.Resources.P0202010000_CustOrdNo + item.CUST_ORD_NO + "  " 
                //			+ Properties.Resources.P0202010000_ItemDetailTotal + F010202Datas.Count;
                //	}
                //	else
                //	{
                //		CustOrdNo = null;
                //		changeFlag = false;
                //		F010202Datas = null;
                //		Status = null;
                //		ShowReportCompleted = "0";
                //		FastPassType = null;
                //		ItemDetail = null;
                //	}

                //}


            }
        }
        #region 顯示顏色調整
        private System.Windows.Media.Brush _foregroundColor = System.Windows.Media.Brushes.DarkSeaGreen;
        public System.Windows.Media.Brush ForegroundColor
        {
            get { return _foregroundColor; }
            set
            {
                _foregroundColor = value;
                RaisePropertyChanged("ForegroundColor");
            }
        }
        #endregion

        #region 累加刷讀筆數

        private int _swipeCount;
        public int SwipeCount
        {
            get { return _swipeCount; }
            set
            {
                _swipeCount = value;
                RaisePropertyChanged("SwipeCount");
            }
        }
        #endregion

        #region 清除按鈕
        public ICommand CleanCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                        o =>
                        {
                            SwipeCount = 0;
                            SwipeList = new List<Wms3pl.WpfClient.ExDataServices.P01ExDataService.F010201MainData>();
                            F010202Datas = new ObservableCollection<ExDataServices.P01ExDataService.F010201MainData>();
                            SelectedPurchaseNo = "";
                            CustOrdNo = "";
                            FastPassType = "";
                            WhName = "";
                        },
                     () => UserOperateMode == OperateMode.Query,
                     o =>
                     {
                         F010202Datas.Clear();
                         Status = "";
                     }
                );

            }
        }
        #endregion

        #region 累加刷讀清單
        private List<Wms3pl.WpfClient.ExDataServices.P01ExDataService.F010201MainData> _swipeList;
        public List<Wms3pl.WpfClient.ExDataServices.P01ExDataService.F010201MainData> SwipeList
        {
            get { return _swipeList; }
            set
            {
                _swipeList = value;
                RaisePropertyChanged("SwipeList");
            }
        }
    /// <summary>
    /// 新增刷讀明細至暫存list
    /// </summary>
    /// <param name="SwipeCountNeedAdd">0:增加清單內容＆增加數量 1:增加清單內容&不增加數量 2:不增加清單&不增加數量</param>
    /// <returns></returns>
    public bool AddSwipeList(int SwipeCountNeedAdd)
    {
      //查無此單號由另一個function 判斷,加上F010202Datas為空時只需回傳true
      if (F010202Datas != null)
      {
        var message = new MessagesStruct
        {
          Image = DialogImage.Warning,
          Title = Properties.Resources.Message
        };

        if (new[] { 0, 1 }.Contains(SwipeCountNeedAdd))
        {
          if (SwipeList.Any())
          {
            var checkList = SwipeList.Where(o => o.DC_CODE == SelectedDc && o.GUP_CODE == this._gupCode && o.CUST_CODE == this._custCode && (o.STOCK_NO == SelectedPurchaseNo && o.CUST_ORD_NO == CustOrdNo)).ToList();
            if (checkList.Any())
            {
              message.Message = Properties.Resources.P0202010000_RepeatSwipeList;
              ShowMessage(message);
              return false;
            }
          }
          SwipeList.AddRange(F010202Datas);
        }
        if (SwipeCountNeedAdd == 0)
          SwipeCount++;
      }
      else
      {
        return false;
      }

      return true;
    }
        #endregion
        private F010201 _f010201;
        public F010201 F010201
        {
            get { return _f010201; }
            set
            {
                _f010201 = value;
                RaisePropertyChanged("F010201");
            }
        }
        private bool _isUseful = true;
        public bool IsUseful
        {
            get { return _isUseful; }
            set
            {
                _isUseful = value;
                RaisePropertyChanged("IsUseful");
            }
        }

        public ICommand SearchPurchaseNoCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                        o =>
                        {

                            DoSearchPurchaseNo();
                            int saveResult = DoSave();
                            IsUseful = saveResult != -1;
                            if (IsUseful)
                                AddSwipeList(saveResult);
                        },
                        () => UserOperateMode == OperateMode.Query,
                        o => DoSearchPurchaseNoComplete()
                );
            }
        }

        public void DoSearchPurchaseNo()
        {
            if (changeFlag == false)
            {
                var f01Proxy = GetProxy<F01Entities>();
                //取得進倉單號(有可能刷的是 QRCODE 32碼)
                var item = f01Proxy.F010201s.Where(o => o.DC_CODE == SelectedDc && o.GUP_CODE == this._gupCode && o.CUST_CODE == this._custCode
                                                && (o.STOCK_NO == SelectedPurchaseNo || o.CHECK_CODE == SelectedPurchaseNo)).FirstOrDefault();
                FastPassType = "";
                if (item != null)
                {
                    SelectedPurchaseNo = item.STOCK_NO;
                    var ItemStatus = GetBaseTableService.GetF000904List(FunctionCode, "F010201", "STATUS", true).FirstOrDefault(x => x.Value == item.STATUS);
                    Status = ItemStatus.Name;
                    ShowReportCompleted = ItemStatus.Value == "1" || ItemStatus.Value == "2" ? "1" : "0";
                    if (CustOrdNo != item.CUST_ORD_NO)
                        CustOrdNo = item.CUST_ORD_NO;

                    //設定快速通關顯示狀態
                    var sekectFastPassType = GetBaseTableService.GetF000904List(FunctionCode, "F010201", "FAST_PASS_TYPE", true).Where(x => x.Value == item.FAST_PASS_TYPE).FirstOrDefault();
                    if (sekectFastPassType != null)
                    {
                        FastPassType = sekectFastPassType.Name;
                        ShowFastPassType = sekectFastPassType.Value;
                        //快速顯示橘色
                        if (ShowFastPassType == "2")
                        {
                            ForegroundColor = System.Windows.Media.Brushes.Orange;
                        }
                        else
                        {
                            ForegroundColor = System.Windows.Media.Brushes.Red;

                        }
                    }


                    F010201 = item;

                    //取得進倉單明細
                    //var proxyEx = GetExProxy<Wms3pl.WpfClient.ExDataServices.P01ExDataService.P01ExDataSource>();
                    //F010202Datas = proxyEx.CreateQuery<Wms3pl.WpfClient.ExDataServices.P01ExDataService.F010202Data>("GetF010202DatasMargeValidate")
                    //.AddQueryExOption("dcCode", SelectedDc)
                    //.AddQueryExOption("gupCode", this._gupCode)
                    //.AddQueryExOption("custCode", this._custCode)
                    //.AddQueryExOption("stockNo", item.STOCK_NO).ToObservableCollection();
                    //20211214_Change
                    var proxyEx = GetExProxy<Wms3pl.WpfClient.ExDataServices.P01ExDataService.P01ExDataSource>();
                    F010202Datas = proxyEx.CreateQuery<Wms3pl.WpfClient.ExDataServices.P01ExDataService.F010201MainData>("GetF010202DatasMargeValidateChange")
                                     .AddQueryExOption("dcCode", SelectedDc)
                                     .AddQueryExOption("gupCode", this._gupCode)
                                     .AddQueryExOption("custCode", this._custCode)
                                     .AddQueryExOption("stockNo", item.STOCK_NO).ToObservableCollection();

                    // 進倉明細顯示
                    ItemDetail = Properties.Resources.P0202010000_StockNo + item.STOCK_NO + " " + Properties.Resources.P0202010000_CustOrdNo + item.CUST_ORD_NO + " " + Properties.Resources.P0202010000_ItemDetailTotal + F010202Datas.Count;
                }
                else
                {
                    CustOrdNo = null;
                    changeFlag = false;
                    F010202Datas = null;
                    Status = null;
                    ShowReportCompleted = "0";
                    FastPassType = null;
                    ItemDetail = null;
                }
            }
        }

        private void DoSearchCustOrdNo()
        {
            var f01Proxy = GetProxy<F01Entities>();
            if (!string.IsNullOrWhiteSpace(CustOrdNo))
            {
                var item = f01Proxy.F010201s.Where(o => o.DC_CODE == SelectedDc && o.GUP_CODE == this._gupCode && o.CUST_CODE == this._custCode
                                        && (o.CUST_ORD_NO == CustOrdNo || o.CHECK_CODE == CustOrdNo)).FirstOrDefault();
                if (item != null && !string.IsNullOrWhiteSpace(item.CHECK_CODE))
                    CustOrdNo = item.STOCK_NO;

                if (item != null)
                {
                    if (SelectedPurchaseNo != item.STOCK_NO)
                    {
                        SelectedPurchaseNo = item.STOCK_NO;
                        RaisePropertyChanged("SelectedPurchaseNo");
                        DoSearchPurchaseNo();
                    }

                }
                else
                {
                    changeFlag = true;
                    SelectedPurchaseNo = null;
                    RaisePropertyChanged("SelectedPurchaseNo");
                    changeFlag = false;
                    Status = null;
                    ShowReportCompleted = "0";
                    F010202Datas = null;
                    FastPassType = null;
                    ItemDetail = null;
                }
            }
        }


        private void DoSearchPurchaseNoComplete()
        {
            RaisePropertyChanged("SelectedPurchaseNo");
            OnSearchPurchaseNoComplete();
        }
        #endregion
        #region Form - 車號
        private string _selectedCarNumber = string.Empty;
        public string SelectedCarNumber
        {
            get { return _selectedCarNumber; }
            set { _selectedCarNumber = value; }
        }
        #endregion

        #region From - 貨主單號

        private string _custOrdNo = string.Empty;
        /// <summary>
        /// 貨主單號
        /// </summary>
        public string CustOrdNo
        {
            get { return _custOrdNo; }
            set
            {
                _custOrdNo = string.IsNullOrWhiteSpace(value) ? value : value.ToUpper();
                RaisePropertyChanged("CustOrdNo");
                //var f01Proxy = GetProxy<F01Entities>();
                //if (!string.IsNullOrWhiteSpace(value))
                //{
                //	var item = f01Proxy.F010201s.Where(o => o.DC_CODE == SelectedDc && o.GUP_CODE == this._gupCode && o.CUST_CODE == this._custCode
                //				&& (o.CUST_ORD_NO == value || o.CHECK_CODE == value)).FirstOrDefault();
                //	if (item != null && !string.IsNullOrWhiteSpace(item.CHECK_CODE))
                //		value = item.STOCK_NO;

                //	if (item != null)
                //	{
                //		if (SelectedPurchaseNo != item.STOCK_NO)
                //		{
                //			SelectedPurchaseNo = item.STOCK_NO;
                //			RaisePropertyChanged("SelectedPurchaseNo");
                //		}

                //	}
                //	else
                //	{
                //		changeFlag = true;
                //		SelectedPurchaseNo = null;
                //		RaisePropertyChanged("SelectedPurchaseNo");
                //		changeFlag = false;
                //		Status = null;
                //		ShowReportCompleted = "0";
                //		F010202Datas = null;
                //		FastPassType = null;
                //		ItemDetail = null;
                //	}
                //}
            }
        }

        #endregion

        #region Data - 報表資料
        private List<P020201ReportData> _reportData = new List<P020201ReportData>();
        public List<P020201ReportData> ReportData
        {
            get { return _reportData; }
            set { _reportData = value; }
        }

        /// <summary>
        /// 給報表使用的VNR_CODE
        /// </summary>
        public string VnrCode
        {
            get
            {
                if (!ReportData.Any()) return string.Empty;
                return ReportData.FirstOrDefault().VNR_CODE;
            }
        }
        /// <summary>
        /// 給報表使用的VNR_NAME
        /// </summary>
        public string VnrName
        {
            get
            {
                if (!ReportData.Any()) return string.Empty;
                return ReportData.FirstOrDefault().VNR_NAME;
            }
        }
        /// <summary>
        /// 給報表使用的車號
        /// 當該進倉單沒有進場預排資料時, 此時以畫面上輸入的車號來顯示
        /// </summary>
        public string CarNumber
        {
            get
            {
                if (!ReportData.Any()) return string.Empty;
                var proxy = GetProxy<F02Entities>();
                var result = proxy.F020103s.Where(x => x.PURCHASE_NO.Equals(SelectedPurchaseNo)
                        && x.DC_CODE.Equals(SelectedDc) && x.GUP_CODE.Equals(this._gupCode) && x.CUST_CODE.Equals(this._custCode))
                        .FirstOrDefault();
                if (result == null) return SelectedCarNumber;
                return result.CAR_NUMBER;
            }
        }
        #endregion
        #endregion

        #region Command
        #region Print
        public ICommand PrintCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                        o => DoPrint(),
                        () => (F010202Datas?.Any() ?? false) && SelectedTabIndex == 0,
                        o => AfterSave()
                );
                //return new RelayCommand(() =>
                //{
                //    DispatcherAction(() =>
                //    {

                //    });
                //});

                //DoPrint();
                //AfterSave();
            }
        }
        #endregion
        #region Save
        public ICommand SaveCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                        o => DoSave(),
                        () =>
                        {
                            return UserOperateMode == OperateMode.Query && !string.IsNullOrWhiteSpace(SelectedPurchaseNo) && F010202Datas != null && SelectedTabIndex == 0 && !string.IsNullOrWhiteSpace(EmpId) && !string.IsNullOrWhiteSpace(EmpName);
                        }
                //o => { if (_doPrint) AfterSave(); }
                );
            }
        }

    //private bool _doPrint = false;
    //public bool DoSave()
    /// <summary>
    /// 
    /// </summary>
    /// <returns> -1 = 檢查失敗，不做後續動作 0=加入至SwipeList並且SwipeCount+1 1=加入至SwipeList並且SwipeCount+0</returns>
    public int DoSave()
    {
      //_doPrint = false;
      if (DoCheckData())
      {
        //if (ShowMessage(Messages.WarningBeforePrintWarehouseInReport) != DialogResponse.Yes) return false;

        // 當進倉單沒有填採購單號時，詢問是否繼續作業，是的話，採購單號自動填入進倉單號
        //if (!new PurchaseService().CheckShopNo(this, SelectedDc, _gupCode, _custCode, SelectedPurchaseNo))
        //	return false;

        // ToDo: LO更新串接
        var proxy = GetExProxy<P02ExDataSource>();

        // 呼叫Lms上架倉別指示、Wcssr收單驗貨上架
        var lmsRes = proxy.CreateQuery<ExecuteResult>("CallLmsApiWithWcssrApi")
                .AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
                .AddQueryOption("gupCode", string.Format("'{0}'", this._gupCode))
                .AddQueryOption("custCode", string.Format("'{0}'", this._custCode))
                .AddQueryOption("stockNo", string.Format("'{0}'", SelectedPurchaseNo))
                .ToList();

        WhName = string.IsNullOrWhiteSpace(lmsRes[0].No) ? string.Empty : lmsRes[0].No;

        var msgList = new List<string>();

        // 代表Wcssr收單驗貨上架有通過
        if (lmsRes[0].IsSuccessed)
        {
          var result = proxy.CreateQuery<ExecuteResult>("UpdateP020201")
              .AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
              .AddQueryOption("gupCode", string.Format("'{0}'", this._gupCode))
              .AddQueryOption("custCode", string.Format("'{0}'", this._custCode))
              .AddQueryOption("purchaseNo", string.Format("'{0}'", SelectedPurchaseNo))
              .AddQueryOption("carNumber", string.Format("'{0}'", SelectedCarNumber))
              .AddQueryOption("empId", string.Format("'{0}'", EmpId))
              .AddQueryOption("empName", string.Format("'{0}'", EmpName))
              .ToList();
          if (result[0].IsSuccessed == true)
          {
            //ShowMessage(Messages.Success2);
            //// 取得報表資料
            //DoPrint();

            //再次更新SelectedPurchaseNo
            var selectedPurchaseNo = SelectedPurchaseNo;
            SelectedPurchaseNo = SelectedPurchaseNo;

            if (!string.IsNullOrWhiteSpace(lmsRes[0].Message))
            {
              ShowMessage(new MessagesStruct()
              {
                Button = DialogButton.OK,
                Image = DialogImage.Warning,
                Message = lmsRes[0].Message,
                Title = Properties.Resources.Message
              });
            }

            if (result[0].Message == "StatusIs0to3") //判斷後端回傳的status值是不是0=>3
            {
              var checkList = SwipeList.Where(o => o.DC_CODE == SelectedDc && o.GUP_CODE == this._gupCode && o.CUST_CODE == this._custCode && (o.STOCK_NO == SelectedPurchaseNo && o.CUST_ORD_NO == CustOrdNo)).ToList();

              if (checkList.Any())
                return 3;//不加入至SwipeList並且SwipeCount+0
              else
                return 0; //加入至SwipeList並且SwipeCount+1
            }
            else
              return 1; //加入至SwipeList並且SwipeCount+0
          }
          else
          {
            msgList.Add(result[0].Message);
          }
        }

        if (!string.IsNullOrWhiteSpace(lmsRes[0].Message))
          msgList.Add(lmsRes[0].Message);

        if (msgList.Any())
        {
          IsUseful = false;
          F010202Datas = new ObservableCollection<ExDataServices.P01ExDataService.F010201MainData>();

          ShowMessage(new MessagesStruct()
          {
            Button = DialogButton.OK,
            Image = DialogImage.Warning,
            Message = string.Join("\r\n", msgList),
            Title = Properties.Resources.Message
          });
        }
      }
      else
      {
        F010202Datas = new ObservableCollection<ExDataServices.P01ExDataService.F010201MainData>();
      }
      return -1; //不做後續動作
    }

        /// <summary>
        /// 檢查必要資料是否都有填入
        /// </summary>
        /// <returns></returns>
        private bool DoCheckData()
        {
            string msg = string.Empty;
            if (string.IsNullOrWhiteSpace(SelectedPurchaseNo)) msg = Properties.Resources.P0201020100_PurchaseNoIsNull;
            if (!string.IsNullOrWhiteSpace(SelectedPurchaseNo) && !DoCheckPurchaseNo()) return false;
            if (string.IsNullOrWhiteSpace(EmpId) || string.IsNullOrWhiteSpace(EmpName)) msg = "查無點收工號";

            if (F010201.CUST_COST == "MoveIn")
            {
                msg = Properties.Resources.PurchaseNoCustCostError;

            }
            if (string.IsNullOrEmpty(msg))
            {
                return true;
            }
            else
            {
                ShowMessage(new MessagesStruct()
                {
                    Button = DialogButton.OK,
                    Image = DialogImage.Warning,
                    Message = msg,
                    Title = Properties.Resources.Message
                }
                );
                return false;
            }
        }

        /// <summary>
        /// 檢查進倉單是否正確
        /// </summary>
        /// <returns></returns>
        private bool DoCheckPurchaseNo()
        {
            var proxy = GetProxy<F01Entities>();
            var tmp = proxy.F010201s.Where(x => x.DC_CODE.Equals(SelectedDc) && x.STOCK_NO.Equals(SelectedPurchaseNo)).FirstOrDefault();
            if (tmp == null)
            {
                ShowMessage(new MessagesStruct()
                {
                    Image = DialogImage.Error,
                    Title = Properties.Resources.Message,
                    Message = Properties.Resources.P0202010000_PurchaseNotExist
                });
                return false;
            }
            if (tmp.STATUS == "8")
            {
                ShowWarningMessage(Properties.Resources.P0202010000_PurchaseNoStatusError);
                return false;
            }


            return true;
        }

        #endregion Save
        #region Print
        private void DoPrint()
        {
            var proxy = GetExProxy<P02ExDataSource>();
            var result = proxy.CreateQuery<P020201ReportData>("P020201Report")
                    .AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
                    .AddQueryOption("gupCode", string.Format("'{0}'", this._gupCode))
                    .AddQueryOption("custCode", string.Format("'{0}'", this._custCode))
                    .AddQueryOption("purchaseNo", string.Format("'{0}'", SelectedPurchaseNo))
                    .ToList();

            ReportData = result;
        }
        #endregion
        #region Search

        public bool DoSearchPurchaseNoByCustOrdNo()
        {
            bool result = false;
            var message = new MessagesStruct
            {
                Image = DialogImage.Error,
                Title = Properties.Resources.Message
            };

            if (string.IsNullOrEmpty(CustOrdNo))
            {
                message.Message = Properties.Resources.P0202010000_CustOrdNoEmpty;
                ShowMessage(message);
                result = false;
            }
            else
            {
                var proxy = GetProxy<F01Entities>();

                var f010201data = proxy.F010201s.Where(x => x.DC_CODE.Equals(_selectedDc)
                                                                                 && x.GUP_CODE.Equals(_gupCode)
                                                                                 && x.CUST_CODE.Equals(_custCode)
                                                                                 && x.CUST_ORD_NO.Equals(CustOrdNo)).OrderByDescending(o => o.CRT_DATE)
                                                        .FirstOrDefault();
                if (f010201data == null)
                {
                    message.Message = Properties.Resources.P0202010000_PURCHASENONull;
                    ShowMessage(message);
                    result = false;
                }
                else
                {
                    SelectedPurchaseNo = f010201data.STOCK_NO;
                    DoSearchPurchaseNo();
                    result = true;

                }
            }
            return result;
        }


        public ICommand SearchPurchaseNoByCustOrdNoCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                        o =>
                        {

                            if (DoSearchPurchaseNoByCustOrdNo())
                            {
                                int saveResult = DoSave();
                                IsUseful = saveResult != -1;
                                if (IsUseful)
                                    AddSwipeList(saveResult);
                            }
                            else
                            {
                                SelectedPurchaseNo = "";
                                RaisePropertyChanged("SelectedPurchaseNo");
                            };
                        },
                        () => UserOperateMode == OperateMode.Query,
                        o => SearchPurchaseNoByCustOrdNoComplete()
                );
            }
        }

        #endregion
        #endregion

        #region 點收工號
        private string _txtEmpId = Properties.Resources.P0202010000_EmpId;
        public string TxtEmpId
        {
            get { return _txtEmpId; }
            set
            {
                _txtEmpId = value;
                RaisePropertyChanged("TxtEmpId");
            }
        }
        #endregion

        #region 員工編號
        private string _empId;
        public string EmpId
        {
            get { return _empId; }
            set
            {
                _empId = value;
                RaisePropertyChanged("EmpId");
            }
        }

        #endregion

        #region 員工姓名
        private string _empName;
        public string EmpName
        {
            get { return _empName; }
            set
            {
                _empName = value;
                RaisePropertyChanged("EmpName");
            }
        }
        #endregion

        #region 顯示快速通關分類 
        private string _showFastPassType;
        public string ShowFastPassType
        {
            get { return _showFastPassType; }
            set
            {
                _showFastPassType = value;
                RaisePropertyChanged("ShowFastPassType");
            }
        }
        #endregion



        #region 取得員工姓名
        public string GetEmpName()
        {
            var proxy = GetProxy<F19Entities>();
            var UserName = proxy.F1924s.Where(o => o.EMP_ID == EmpId && o.ISDELETED == "0").FirstOrDefault()?.EMP_NAME;
            return UserName;
        }
        #endregion

        #region 快速通關分類
        private string _fastPassType;
        public string FastPassType
        {
            get { return _fastPassType; }
            set
            {
                _fastPassType = value;
                RaisePropertyChanged("FastPassType");
            }
        }
        #endregion

        #region 進倉明細廠商編號、廠商名稱及總品項數
        private string _itemDetail;
        public string ItemDetail
        {
            get { return _itemDetail; }
            set
            {
                _itemDetail = value;
                RaisePropertyChanged("ItemDetail");
            }
        }
        #endregion

        #region 查詢結果的單據筆數及總品項數
        private string _searchResultCount;
        public string SearchResultCount
        {
            get { return _searchResultCount; }
            set
            {
                _searchResultCount = value;
                RaisePropertyChanged("SearchResultCount");
            }
        }
        #endregion

        #region SearchCommand
        public ICommand SearchCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                        o => DoSearch(),
                        () => SelectedTabIndex == 1
                );
            }
        }
        #endregion

        #region 紀錄查尋查詢結果
        public void DoSearch()
        {
          if (!BegCheckinDate.HasValue)
          {
            ShowWarningMessage(Properties.Resources.P0202010000_BegCheckinDate);       
            return;
          }
          if (!EndCheckinDate.HasValue)
          {
            ShowWarningMessage(Properties.Resources.P0202010000_EndCheckinDate);
            return;
          }

      var proxy = GetExProxy<P02ExDataSource>();
            F0202Datas = proxy.CreateQuery<F0202Data>("GetF0202Datas")
                    .AddQueryExOption("dcCode", SelectedDcByQueryCondition)
                    .AddQueryExOption("gupCode", this._gupCode)
                    .AddQueryExOption("custCode", this._custCode)
                    .AddQueryExOption("begCrtDate", BegCrtDate)
                    .AddQueryExOption("endCrtDate", EndCrtDate?.Date.AddDays(1))
                    .AddQueryExOption("orderNo", SelectedOrderNoByQueryCondition)
                    .AddQueryExOption("begCheckinDate", BegCheckinDate)
                    .AddQueryExOption("endCheckinDate", EndCheckinDate?.Date.AddDays(1))
                    .AddQueryExOption("custOrdNo", CustOrdNoByQueryCondition)
                    .AddQueryExOption("empId", EmpIdByQueryCondition)
                     .AddQueryExOption("empName", EmpNameByQueryCondition)
                     .AddQueryExOption("itemCode", ItemCodeByQueryCondition)
                     .AddQueryExOption("selectedFastType", SelectedFastType)
                    .ToList();

            // 查無進倉單號
            if (!F0202Datas.Any())
            {
                ShowMessage(Messages.InfoNoData);
            }

            //計算單據筆數
            var orderNoCount = F0202Datas.Select(x => x.ORDER_NO).Distinct().Count();

            SearchResultCount = $"{Properties.Resources.P202010000_OrderNoCount}{orderNoCount}   {Properties.Resources.P202010000_ItemCount}{F0202Datas.Count}";

            //轉換狀態
            GetFastPassTypeList();
            GetStatusList();
            GetBookingPeriodList();
            F0202Datas.ForEach(x =>
            {
              x.STATUS_NAME = StatusList.Where(y => y.Value == x.STATUS).FirstOrDefault()?.Name;
              x.BOOKING_IN_PERIOD = BookingPeriodList.Where(y => y.Value == x.BOOKING_IN_PERIOD).FirstOrDefault()?.Name;
              x.FAST_PASS_TYPE = FastPassTypeList.Where(y => y.Value == x.FAST_PASS_TYPE).FirstOrDefault()?.Name;
              x.ACCEPTANCE_WAITTIME = x.CHECKACCEPT_TIME.HasValue 
                ? (!string.IsNullOrWhiteSpace(x.BEGIN_CHECKACCEPT_TIME)
                  ? (Convert.ToDateTime(x.BEGIN_CHECKACCEPT_TIME) - x.CHECKACCEPT_TIME.Value).ToString() 
                  : string.Empty)
                : string.Empty;
            });
    }

    #endregion

    #region 紀錄查詢-查詢條件
    #region 物流中心
    private string _selectedDcByQueryCondition;
        public string SelectedDcByQueryCondition
        {
            get { return _selectedDcByQueryCondition; }
            set { Set(() => SelectedDcByQueryCondition, ref _selectedDcByQueryCondition, value); }
        }
        #endregion

        #region 建立日期(起)
        private DateTime? _begCrtDate;
        public DateTime? BegCrtDate
        {
            get { return _begCrtDate; }
            set { Set(() => BegCrtDate, ref _begCrtDate, value); }
        }
        #endregion

        #region 建立日期(迄)
        private DateTime? _endCrtDate;
        public DateTime? EndCrtDate
        {
            get { return _endCrtDate; }
            set { Set(() => EndCrtDate, ref _endCrtDate, value); }
        }
        #endregion

        #region 進倉單號
        private string _selectedOrderNoByQueryCondition;
        public string SelectedOrderNoByQueryCondition
        {
            get { return _selectedOrderNoByQueryCondition; }
            set { Set(() => SelectedOrderNoByQueryCondition, ref _selectedOrderNoByQueryCondition, value); }
        }
        #endregion

        #region 點收日期(起)
        private DateTime? _begCheckinDate = DateTime.Today;
        public DateTime? BegCheckinDate
        {
            get { return _begCheckinDate; }
            set { Set(() => BegCheckinDate, ref _begCheckinDate, value); }
        }
        #endregion

        #region 點收日期(迄)
        private DateTime? _endCheckinDate = DateTime.Now ;
        public DateTime? EndCheckinDate
        {
            get { return _endCheckinDate; }
            set { Set(() => EndCheckinDate, ref _endCheckinDate, value); }
        }
        #endregion

        #region 貨主單號
        private string _custOrdNoByQueryCondition;
        public string CustOrdNoByQueryCondition
        {
            get { return _custOrdNoByQueryCondition; }
            set { Set(() => CustOrdNoByQueryCondition, ref _custOrdNoByQueryCondition, value); }
        }
        #endregion

        #region 點收工號
        private string _empIdByQueryCondition;
        public string EmpIdByQueryCondition
        {
            get { return _empIdByQueryCondition; }
            set { Set(() => EmpIdByQueryCondition, ref _empIdByQueryCondition, value); }
        }
        #endregion

        #region 點收姓名
        private string _empNameByQueryCondition;
        public string EmpNameByQueryCondition
        {
            get { return _empNameByQueryCondition; }
            set { Set(() => EmpNameByQueryCondition, ref _empNameByQueryCondition, value); }
        }
        #endregion

        #region 品號
        private string _itemCodeByQueryCondition;
        public string ItemCodeByQueryCondition
        {
            get { return _itemCodeByQueryCondition; }
            set { Set(() => ItemCodeByQueryCondition, ref _itemCodeByQueryCondition, value); }
        }
        #endregion
        #endregion

        #region 查詢結果
        private List<F0202Data> _f0202Datas;
        public List<F0202Data> F0202Datas
        {
            get { return _f0202Datas; }
            set
            {
                _f0202Datas = value;
                RaisePropertyChanged("F0202Datas");
            }
        }

		private F0202Data _f0202Data;
		public F0202Data F0202Data
		{
			get { return _f0202Data; }
			set
			{
				_f0202Data = value;
				RaisePropertyChanged("_f0202Data");
			}
		}
		#endregion

		#region SelectedTabIndex
		private int _selectedTabIndex;
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

        public ICommand SearchItemCodeCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                        o =>
                        {
                            var proxy = GetProxy<F19Entities>();
                            if (!string.IsNullOrWhiteSpace(ItemCodeByQueryCondition))
                            {
                                var itemCode = ItemCodeByQueryCondition = proxy.F1903s.Where(x => x.ITEM_CODE == ItemCodeByQueryCondition || x.EAN_CODE1 == ItemCodeByQueryCondition || x.EAN_CODE2 == ItemCodeByQueryCondition || x.EAN_CODE3 == ItemCodeByQueryCondition).FirstOrDefault()?.ITEM_CODE;
                                if (string.IsNullOrWhiteSpace(itemCode))
                                {
                                    ShowMessage(Messages.InfoNoData);
                                }
                            }

                        }
                );
            }
        }

		#region DelCommand
		public ICommand DelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
			   o => DoDelete(),
			   () => SelectedTabIndex == 1 && F0202Data != null && F0202Data.STATUS == "3",
			   O => DoDeleteComplete()
			   );
			}
		}

		public void DoDelete()
		{
			if (F0202Data.STATUS == "3")
			{
				var existF020201 = GetProxy<F02Entities>().F020201s.Where(x => x.DC_CODE == SelectedDcByQueryCondition
                            && x.GUP_CODE == _gupCode
														&& x.CUST_CODE == _custCode
														&& x.PURCHASE_NO == F0202Data.ORDER_NO).FirstOrDefault();
				if (existF020201 != null)
				{
					ShowWarningMessage("確認該進倉單沒有驗收紀錄 ");
					return;
				}
				GetWcfProxy<wcf.P02WcfServiceClient>().RunWcfMethod(w => w.UpdateF010201Status(SelectedDcByQueryCondition, _gupCode, _custCode, F0202Data.ORDER_NO));

        //清除清單資料
        var delSwipOrder = SwipeList.FirstOrDefault(x => x.DC_CODE == SelectedDcByQueryCondition && x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode && x.STOCK_NO == F0202Data.ORDER_NO);
        if (delSwipOrder != null)
        {
          SwipeList.Remove(delSwipOrder);
          SwipeCount--;
        }

        //清除畫面上清單資料
        if (F010202Datas != null)
        {
          var ordNo = F0202Data.ORDER_NO.ToString();
          var delF010202Datas = F010202Datas.Where(x => x.DC_CODE != SelectedDcByQueryCondition && x.GUP_CODE != _gupCode && x.CUST_CODE != _custCode && x.STOCK_NO != ordNo);
          if (delF010202Datas != null)
            F010202Datas = delF010202Datas.ToObservableCollection();
        }

        DoSearch();

			}
		}

		public void DoDeleteComplete()
		{

		}

		#endregion

		#region 
		private List<NameValuePair<string>> _statuslist;
        public List<NameValuePair<string>> StatusList
        {
            get { return _statuslist; }
            set
            {
                _statuslist = value;
                RaisePropertyChanged("StatusList");
            }
        }
        #endregion

        #region 
        private List<NameValuePair<string>> _fastPassTypeList;
        public List<NameValuePair<string>> FastPassTypeList
        {
            get { return _fastPassTypeList; }
            set
            {
                _fastPassTypeList = value;
                RaisePropertyChanged("FastPassTypeList");
            }
        }
        #endregion

        private void GetStatusList()
        {
            StatusList = GetBaseTableService.GetF000904List(FunctionCode, "F010201", "STATUS");
        }

        private void GetFastPassTypeList()
        {
            FastPassTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F010201", "FAST_PASS_TYPE");
        }


        #region 語音

        private bool _soundOn = true;
        public bool SoundOn
        {
            get { return _soundOn; }
            set { _soundOn = value; RaisePropertyChanged("SoundOn"); }
        }
        #endregion
    }
}
