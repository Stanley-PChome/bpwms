using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;

namespace Wms3pl.WpfClient.P19.ViewModel
{
    public partial class P1901160000_ViewModel : InputViewModelBase
    {
        private string _userId;
        private string _userName;
        private bool _isDeleteOk;
        public Action AddAction = delegate { };
        F194716 _lastAddOrUpdateF194716 = null;

        #region 資料連結/ 頁面參數
        private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
        private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }

        #region UI 連動 binding
        private bool _searchResultIsExpanded = true;

        public bool SearchResultIsExpanded
        {
            get { return _searchResultIsExpanded; }
            set
            {
                _searchResultIsExpanded = value;
                RaisePropertyChanged("SearchResultIsExpanded");
            }
        }

        private bool _queryResultIsExpanded = true;

        public bool QueryResultIsExpanded
        {
            get { return _queryResultIsExpanded; }
            set
            {
                _queryResultIsExpanded = value;
                RaisePropertyChanged("QueryResultIsExpanded");
            }
        }

        private string _quoteHeaderText;

        public string QuoteHeaderText
        {
            get { return _quoteHeaderText; }
            set
            {
                _quoteHeaderText = value;
                RaisePropertyChanged("QuoteHeaderText");
            }
        }
        #endregion

        #region Form - 日期
        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { _selectedDate = value; RaisePropertyChanged("SelectedDate"); }
        }
        #endregion

        #region Form - 物流中心清單
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

        #region Data - 查詢資料List
        private List<F194716> _dgList;
        public List<F194716> DgList
        {
            get { return _dgList; }
            set
            {
                _dgList = value;
                RaisePropertyChanged("DgList");
            }
        }

        private F194716 _selectedData;

        public F194716 SelectedData
        {
            get { return _selectedData; }
            set
            {
                _selectedData = value;
                if (_selectedData != null)
                    SearchItemCommand.Execute(null); 
                RaisePropertyChanged("SelectedData");
            }
        }

        #region 車次明細
        private List<F19471601Data> _itemList;
        public List<F19471601Data> ItemList
        {
            get { return _itemList; }
            set
            {
                _itemList = value;
                RaisePropertyChanged("ItemList");
            }
        }

        private F19471601Data _selectItem;
        public F19471601Data SelectItem
        {
            get { return _selectItem; }
            set
            {
                _selectItem = value;
                RaisePropertyChanged("SelectItem");
            }
        }
        #endregion

        #endregion

        #region Form - 出車時段
        private List<NameValuePair<string>> _cAR_PERIODList;

        public List<NameValuePair<string>> CAR_PERIODList
        {
            get { return _cAR_PERIODList; }
            set
            {
                _cAR_PERIODList = value;
                RaisePropertyChanged("CAR_PERIODList");
            }
        }

        private List<NameValuePair<string>> _searchCAR_PERIODList;

        public List<NameValuePair<string>> SearchCAR_PERIODList
        {
            get { return _searchCAR_PERIODList; }
            set
            {
                _searchCAR_PERIODList = value;
                RaisePropertyChanged("SearchCAR_PERIODList");
            }
        }

        private string _selectedCarPeriod;
        public string SelectedCarPeriod
        {
            get { return _selectedCarPeriod; }
            set
            {
                _selectedCarPeriod = value;
                RaisePropertyChanged("SelectedCarPeriod");
            }
        }
        #endregion

        #region Form - 車次
        private string _searchDelvNo;
        public string SearchDelvNo
        {
            get { return _searchDelvNo; }
            set
            {
                _searchDelvNo = value;
                if (IsChinceString(SearchDelvNo))
                    SearchDelvNo = "";
                RaisePropertyChanged("SearchDelvNo");
            }
        }
        #endregion

        #region Form - 車行
        private List<NameValuePair<string>> _carGupList;

        public List<NameValuePair<string>> CarGupList
        {
            get { return _carGupList; }
            set
            {
                _carGupList = value;
                RaisePropertyChanged("CarGupList");
            }
        }

        private List<NameValuePair<string>> _searchCarGupList;

        public List<NameValuePair<string>> SearchCarGupList
        {
            get { return _searchCarGupList; }
            set
            {
                _searchCarGupList = value;
                RaisePropertyChanged("SearchCarGupList");
            }
        }

        private string _selectedCarGup;
        public string SelectedCarGup
        {
            get { return _selectedCarGup; }
            set
            {
                _selectedCarGup = value;
                RaisePropertyChanged("SelectedCarGup");
            }
        }
        #endregion

        #region Form - 門市編號
        private string _searchRetailCode;
        public string SearchRetailCode
        {
            get { return _searchRetailCode; }
            set
            {
                _searchRetailCode = value;
                RaisePropertyChanged("SearchRetailCode");
            }
        }
        #endregion

        #region Form - 路順
        private List<NameValuePair<string>> _delvWayList;

        public List<NameValuePair<string>> DelvWayList
        {
            get { return _delvWayList; }
            set
            {
                _delvWayList = value;
                RaisePropertyChanged("DelvWayList");
            }
        }

        private List<NameValuePair<string>> _searchDelvWayList;

        public List<NameValuePair<string>> SearchDelvWayList
        {
            get { return _searchDelvWayList; }
            set
            {
                _searchDelvWayList = value;
                RaisePropertyChanged("SearchDelvWayList");
            }
        }

        private string _selectedDelvWay;
        public string SelectedDelvWay
        {
            get { return _selectedDelvWay; }
            set
            {
                _selectedDelvWay = value;
                RaisePropertyChanged("SelectedDelvWay");
            }
        }
        #endregion

        #region Data - 車次明細
        private F19471601Data _editableF19471601Data = null;
        public F19471601Data EditableF19471601Data
        {
            get { return _editableF19471601Data; }
            set
            {
                _editableF19471601Data = value;
                RaisePropertyChanged("EditableF19471601Data");
            }
        }

        private DateTime _selectedArrivalTimeS;
        /// <summary>
        /// 給TimePicker使用的時間. 選定後更新EditableF19471601Data.ARRIVAL_TIME_S, 以便儲存時能取到資料.
        /// </summary>
        public DateTime SelectedArrivalTimeS
        {
            get { return _selectedArrivalTimeS; }
            set
            {
                _selectedArrivalTimeS = value;
                EditableF19471601Data.ARRIVAL_TIME_S = value.ToString("HHmm");
                RaisePropertyChanged("SelectedArrivalTimeS");
            }
        }
        private DateTime _selectedArrivalTimeE;
        /// <summary>
        /// 給TimePicker使用的時間. 選定後更新EditableF19471601Data.ARRIVAL_TIME_E, 以便儲存時能取到資料.
        /// </summary>
        public DateTime SelectedArrivalTimeE
        {
            get { return _selectedArrivalTimeE; }
            set
            {
                _selectedArrivalTimeE = value;
                EditableF19471601Data.ARRIVAL_TIME_E = value.ToString("HHmm");
                RaisePropertyChanged("SelectedArrivalTimeE");
            }
        }
        #endregion

        #endregion


        public P1901160000_ViewModel()
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
            QuoteHeaderText = GetQuoteHeaderText();
            DcList = GetDcCodeList();
            if (DcList.Any()) SelectedDc = DcList.FirstOrDefault().Value;
            CAR_PERIODList = GetCAR_PERIODList(false);
            CarGupList = GetCarGupList(false);
            DelvWayList = GetDelvWayList(false);
            SearchCAR_PERIODList = GetCAR_PERIODList(true);
            SearchCarGupList = GetCarGupList(true);
            SelectedCarPeriod = "";
            SelectedCarGup = "";
        }

        

        #region 函式
        /// <summary>
        /// 取得 F000904 的列舉值
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="subTopic"></param>
        /// <returns></returns>
        public List<NameValuePair<string>> GetF000904NameValueList(string topic, string subTopic,bool allItem)
        {
            return GetBaseTableService.GetF000904List(FunctionCode, topic, subTopic, allItem);
        }
        
        //取物流中心清單
        private List<NameValuePair<string>> GetDcCodeList()
        {
            return Wms3plSession.Get<GlobalInfo>().DcCodeList;
        }

        //出車時段
        public List<NameValuePair<string>> GetCAR_PERIODList(bool blAllItem=false)
        {
            return GetF000904NameValueList("F194716", "CAR_PERIOD", blAllItem);
        }

        //車行
        public List<NameValuePair<string>> GetCarGupList(bool blAllItem = false)
        {
            return GetF000904NameValueList("F194716", "CAR_GUP", blAllItem);
        }

        //路順
        public List<NameValuePair<string>> GetDelvWayList(bool blAllItem = false)
        {
            return GetF000904NameValueList("P190116", "DELV_WAY", blAllItem);
        }

        private string GetQuoteHeaderText()
        {
            switch (UserOperateMode)
            {
                case OperateMode.Edit:
                    return Properties.Resources.P1901160000_DELV_NO_Manage;
                case OperateMode.Add:
                    return Properties.Resources.P1901160000_DELV_NO_Insert;
                default:
                    return Properties.Resources.P1901160000_DELV_NO_Detail;
            }
        }

        public void GetF1910Data()
        {
            if(string.IsNullOrEmpty(EditableF19471601Data.RETAIL_CODE))
            {
                ShowWarningMessage(Properties.Resources.P1901160000_InputStoreNo);
                return;
            }
            var proxy = GetProxy<F19Entities>();
            //var result = proxy.F1910s.Where(x => x.GUP_CODE.Equals(_gupCode) && x.CUST_CODE.Equals(_custCode) && x.RETAIL_CODE.Equals(EditableF19471601Data.RETAIL_CODE)).ToList();
            var result = proxy.CreateQuery<F1910>("GetAllowedF1910s")
                                                            .AddQueryExOption("gupCode", _gupCode)
                                                            .AddQueryExOption("custCode", _custCode)
                                                            .AddQueryExOption("retailCode", EditableF19471601Data.RETAIL_CODE)
                                                            .AddQueryExOption("retailName", "")
                                                            .ToList();
            if (!result.Any())
            {
                ShowWarningMessage(Properties.Resources.P1901160000_NoStoreData);
                return;
            }
            EditableF19471601Data.RETAIL_NAME = result.FirstOrDefault().RETAIL_NAME;
        }

        public void ClearItemValue()
        {
            EditableF19471601Data = new F19471601Data();
            SelectedArrivalTimeS = System.DateTime.Now;
            SelectedArrivalTimeE = System.DateTime.Now;
        }

        public bool CheckDetailData()
        {
            if(SelectedData == null)
            {
                ShowWarningMessage(Properties.Resources.P1901160000_SelectedDELV_NODataNull);
                return false;
            }
            if (string.IsNullOrEmpty(SelectedData.DC_CODE))
            {
                ShowWarningMessage(Properties.Resources.P1901160000_SelectedDC);
                return false;
            }
            if (string.IsNullOrEmpty(SelectedData.DELV_NO))
            {
                ShowWarningMessage(Properties.Resources.P1901160000_SelectedDELV_NO);
                return false;
            }
            if (string.IsNullOrEmpty(EditableF19471601Data.RETAIL_CODE))
            {
                ShowWarningMessage(Properties.Resources.P1901160000_InputRETAIL_CODE);
                return false;
            }
            if (string.IsNullOrEmpty(EditableF19471601Data.RETAIL_NAME))
            {
                ShowWarningMessage(Properties.Resources.P1901160000_InputRETAIL_NAME);
                return false;
            }
            if (string.IsNullOrEmpty(EditableF19471601Data.DELV_WAY))
            {
                ShowWarningMessage(Properties.Resources.P1901160000_InputDELV_WAY);
                return false;
            }
            if (string.IsNullOrEmpty(EditableF19471601Data.ARRIVAL_TIME_S))
            {
                ShowWarningMessage(Properties.Resources.P1901160000_InputARRIVAL_TIME_S);
                return false;
            }
            if (string.IsNullOrEmpty(EditableF19471601Data.ARRIVAL_TIME_E))
            {
                ShowWarningMessage(Properties.Resources.P1901160000_InputARRIVAL_TIME_E);
                return false;
            }
            var proxy = GetProxy<F19Entities>();

            var data = proxy.F19471601s.Where(x => x.GUP_CODE.Equals(_gupCode) && x.CUST_CODE.Equals(_custCode) && x.DC_CODE.Equals(SelectedData.DC_CODE) && x.RETAIL_CODE.Equals(EditableF19471601Data.RETAIL_CODE) && x.DELV_NO != SelectedData.DELV_NO).ToList();
            if (data.Any())
            {
                ShowWarningMessage(Properties.Resources.P1901160000_RetailCodeExist);
                return false;
            }

            return true;

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
            //執行查詢動作
            DgList = new List<F194716>();
            //執行查詢動
            var proxy = GetProxy<F19Entities>();
            DgList = proxy.CreateQuery<F194716>("GetF194716Datas")
                .AddQueryExOption("gupCode", _gupCode)
                .AddQueryExOption("custCode", _custCode)
                .AddQueryExOption("dcCode", SelectedDc)
                .AddQueryExOption("carPeriod", SelectedCarPeriod)
                .AddQueryExOption("delvNo", SearchDelvNo)
                .AddQueryExOption("carGup", SelectedCarGup)
                .AddQueryExOption("retailCode", SearchRetailCode)
                .ToList();

            if (!DgList.Any())
            {
                SearchResultIsExpanded = !DgList.Any();
                SelectedData = null;
                DgList = new List<F194716>();
                ShowMessage(Messages.InfoNoData);
                return;
            }
            QueryResultIsExpanded = DgList.Count >= 1;
            SelectedData = DgList.FirstOrDefault();
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
            SelectedData = new F194716();
            SelectItem = new F19471601Data();
            EditableF19471601Data = new F19471601Data();
            SelectedArrivalTimeS = System.DateTime.Now;
            SelectedArrivalTimeE = System.DateTime.Now;
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
            if (SelectedData==null)
            {
                ShowWarningMessage(Properties.Resources.P1901160000_SelectedEditDataNull);
                return ;
            }
            UserOperateMode = OperateMode.Edit;
            //執行編輯動作
            EditableF19471601Data = new F19471601Data();
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
            ClearItemValue();
            //暫時改成取消時直接查詢該筆車次的資料
            SearchDelvNo = SelectedData.DELV_NO;
            DoSearch();
            SearchItemCommand.Execute(null);
        }
        #endregion Cancel

        #region Delete
        public ICommand DeleteCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedData != null, o => DoDeleteCompleted()
                    );
            }
        }

        private void DoDelete()
        {
            //執行刪除動作
            _isDeleteOk = false;
            if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
            {
                if (SelectedData == null)
                {
                    ShowMessage(Properties.Resources.P1901100000_DeleteFail);
                    return;
                }

                var proxy = GetExProxy<P19ExDataSource>();

                var query = proxy.CreateQuery<ExecuteResult>("DeleteP190116")
                                 .AddQueryExOption("gupCode", _gupCode)
                                 .AddQueryExOption("custCode", _custCode)
                                 .AddQueryExOption("dcCode", SelectedData.DC_CODE)
                                 .AddQueryExOption("delvNo", SelectedData.DELV_NO)
                                 ;

                var result = query.ToList().FirstOrDefault();
                if(result.IsSuccessed)
                {
                    _isDeleteOk = true;
                }
                ShowResultMessage(result);
            }
        }

        private void DoDeleteCompleted()
        {
            if (_isDeleteOk)
                SearchCommand.Execute(null);
        }
        #endregion Delete

        #region Save
        public ICommand SaveCommand
        {
            get
            {
                var isSuccess = false;
                return CreateBusyAsyncCommand(
                    o => isSuccess = DoSave(), () => UserOperateMode != OperateMode.Query && SelectedData != null,
                    o => DoSaveComplete(isSuccess)
                    );
            }
        }

        private void DoSaveComplete(bool isSuccess)
        {
            if (isSuccess)
                SearchItemCommand.Execute(null);
        }

        private bool DoSave()
        {
            bool isSuccess = false;

            if (ShowMessage(Messages.WarningBeforeUpdate) == DialogResponse.Yes)
            {
                //執行確認儲存動作
                if (UserOperateMode == OperateMode.Add)
                    isSuccess = DoSaveAdd();
                else if (UserOperateMode == OperateMode.Edit)
                    isSuccess = DoSaveEdit();
            }
            if (isSuccess)
            {
                UserOperateMode = OperateMode.Query;
            }
            return isSuccess;
        }

        private bool DoSaveAdd()
        {
            MessagesStruct alertMessage = new MessagesStruct();
            alertMessage.Button = DialogButton.OK;
            alertMessage.Title = Resources.Resources.Information;
            alertMessage.Image = DialogImage.Warning;
            // 檢查資料
            if (SelectedData == null)
            {
                alertMessage.Message = Properties.Resources.P1901090000_InsertFail;
                ShowMessage(alertMessage);
                return false;
            }

            var error = GetEditableError(SelectedData);
            if (!string.IsNullOrEmpty(error))
            {
                ShowWarningMessage(error);
                return false;
            }
            SelectedData.GUP_CODE = _gupCode;
            SelectedData.CUST_CODE = _custCode;
            var f194716 = ExDataMapper.Map<F194716, wcf.F194716>(SelectedData);
            var wcfF19471601Datas = ItemList.ToObservableCollection().MapCollection<F19471601Data, wcf.F19471601Data>().ToArray();

            var wcfProxy = GetWcfProxy<wcf.P19WcfServiceClient>();
            var result = wcfProxy.RunWcfMethod(w => w.InsertOrUpdateF194716(f194716, wcfF19471601Datas, true));
            if (!result.IsSuccessed)
            {
                ShowWarningMessage(result.Message);
                return false;
            }
            _lastAddOrUpdateF194716 = ExDataMapper.Clone(SelectedData);
            ShowMessage(Messages.Success);
            UserOperateMode = OperateMode.Query;
            DoSearch();
            return true;
        }

        private bool DoSaveEdit()
        {
            MessagesStruct alertMessage = new MessagesStruct();
            alertMessage.Button = DialogButton.OK;
            alertMessage.Title = Resources.Resources.Information;
            alertMessage.Image = DialogImage.Warning;
            // 檢查資料
            if (SelectedData == null)
            {
                alertMessage.Message = Properties.Resources.P1901090000_UpdateFail;
                ShowMessage(alertMessage);
                return false;
            }
            if (string.IsNullOrWhiteSpace(SelectedData.GUP_CODE))
            {
                ShowMessage(Messages.WarningNoGupCode);
                return false;
            }

            var error = GetEditableError(SelectedData);
            if (!string.IsNullOrEmpty(error))
            {
                ShowWarningMessage(error);
                return false;
            }

            SelectedData.GUP_CODE = _gupCode;
            SelectedData.CUST_CODE = _custCode;

            var f194716 = ExDataMapper.Map<F194716, wcf.F194716>(SelectedData);
            var wcfF19471601Datas = ItemList.ToObservableCollection().MapCollection<F19471601Data, wcf.F19471601Data>().ToArray();

            var wcfProxy = GetWcfProxy<wcf.P19WcfServiceClient>();
            var result = wcfProxy.RunWcfMethod(w => w.InsertOrUpdateF194716(f194716, wcfF19471601Datas, false));
            if (!result.IsSuccessed)
            {
                ShowWarningMessage(result.Message);
                return false;
            }
            _lastAddOrUpdateF194716 = ExDataMapper.Clone(SelectedData);
            ShowMessage(Messages.Success);
            UserOperateMode = OperateMode.Query;
            DoSearch();
            return true;
        }

        string GetEditableError(F194716 e)
        {
            StringBuilder ErrorMessage = new StringBuilder();

            if (string.IsNullOrEmpty(e.DC_CODE))
                ErrorMessage.Append(Properties.Resources.P1901160000_SelectedDC1);

            if (string.IsNullOrEmpty(e.DELV_NO))
                ErrorMessage.Append(Properties.Resources.P1901160000_SelectedDELV_NO1);

            if (string.IsNullOrEmpty(e.CAR_PERIOD))
                ErrorMessage.Append(Properties.Resources.P1901160000_SelectedCAR_PERIOD);

            if (string.IsNullOrEmpty(e.CAR_GUP))
                ErrorMessage.Append(Properties.Resources.P1901160000_SelectedCAR_GUP);

            if (string.IsNullOrEmpty(e.DRIVER_ID))
                ErrorMessage.Append(Properties.Resources.P1901160000_SelectedDRIVER_ID);

            if (string.IsNullOrEmpty(e.DRIVER_NAME))
                ErrorMessage.Append(Properties.Resources.P1901160000_SelectedDRIVER_NAME);

            if (e.EXTRA_FEE<0)
                ErrorMessage.Append(Properties.Resources.P1901160000_SelectedEXTRA_FEE);
            if (e.OIL_FEE < 0)
                ErrorMessage.Append(Properties.Resources.P1901160000_SelectedOIL_FEE);
            if (e.REGION_FEE < 0)
                ErrorMessage.Append(Properties.Resources.P1901160000_SelectedREGION_FEE);
            if (e.OVERTIME_FEE < 0)
                ErrorMessage.Append(Properties.Resources.P1901160000_SelectedOVERTIME_FEE);

      return ErrorMessage.ToString();
        }
        #endregion Save

        #region SearchItem
        public ICommand SearchItemCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoSearchItem(), () => UserOperateMode == OperateMode.Query,
                    o => DoSearchItemComplete()
                    );
            }
        }

        private void DoSearchItem()
        {
            ItemList = new List<F19471601Data>();
            SelectItem = new F19471601Data();
            if (SelectedData != null)
            {
                ItemList = GetListItems();
                //ShowMessage(new MessagesStruct() { Message = Properties.Resources.P1901160000_SelectedDELV_NO2, Title = Resources.Resources.Information });
                //return;
            }

           
            //if (ItemList == null || !ItemList.Any())
            //{
            //    OriItemList = null;
            //    return;
            //}
            //OriItemList = ItemList.Select(x => AutoMapper.Mapper.DynamicMap<F190301Data>(x)).ToList();
        }

        private void DoSearchItemComplete()
        {
            if (ItemList != null && ItemList.Any())
                SelectItem = ItemList.FirstOrDefault();
        }

        private List<F19471601Data> GetListItems()
        {
            var proxyEx = GetExProxy<P19ExDataSource>();
            var results = proxyEx.CreateQuery<F19471601Data>("GetF19471601Datas")
                .AddQueryOption("gupCode", string.Format("'{0}'", _gupCode))
                .AddQueryOption("custCode", string.Format("'{0}'", _custCode))
                .AddQueryOption("dcCode", string.Format("'{0}'", SelectedData.DC_CODE))
                .AddQueryOption("delvNo", string.Format("'{0}'", SelectedData.DELV_NO))
                .ToList();
            return results;
        }
        #endregion SearchItem

        #region AddDetail -Add Grid
        public ICommand AddDetailCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                o => DoAddDetail(), () => UserOperateMode != OperateMode.Query, o => DoAddDetailCompleted()
                );
            }
        }
        private void DoAddDetail()
        {
            
        }
        private void DoAddDetailCompleted()
        {
            var tmpList = ItemList ?? new List<F19471601Data>();

            //檢查是否為虛擬商品
            if (CheckDetailData())
            {
                var item = new F19471601Data();
                item.DC_CODE = SelectedData.DC_CODE;
                item.GUP_CODE = _gupCode;
                item.CUST_CODE = _custCode;
                item.DELV_NO = SelectedData.DELV_NO;
                item.RETAIL_CODE = EditableF19471601Data.RETAIL_CODE;
                item.RETAIL_NAME = EditableF19471601Data.RETAIL_NAME;
                item.DELV_WAY = DelvWayList.Where(o => o.Value == EditableF19471601Data.DELV_WAY).FirstOrDefault().Name;
                item.ARRIVAL_TIME_S = EditableF19471601Data.ARRIVAL_TIME_S;
                item.ARRIVAL_TIME_E = EditableF19471601Data.ARRIVAL_TIME_E;
                item.PACK_FIELD = EditableF19471601Data.PACK_FIELD;
                item.CRT_DATE = DateTime.Now;
                item.CRT_NAME = _userName;
                item.CRT_STAFF = _userId;

                tmpList.Add(item);
                SelectItem = item;
                ClearItemValue();
                ItemList = tmpList.ToList();
                //RaisePropertyChanged("ItemList");
                AddAction();
            }
        }

        #endregion

        #region DelDetail -Add Grid
        public ICommand DelDetailCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                o => DoDelDetail(), () => UserOperateMode != OperateMode.Query
                );
            }
        }
        private void DoDelDetail()
        {
            var list = ItemList ?? new List<F19471601Data>();
            var removeItem = SelectItem;

            list.Remove(removeItem);

            ItemList = list.ToList();

            if (ItemList.Any())
            {
                SelectItem = ItemList.FirstOrDefault();
            }
        }
        #endregion

        /// <summary>
        /// 檢驗是否含有中文字串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool IsChinceString(string str)
        {
            //防止無限堆疊
            if (string.IsNullOrEmpty(str))
                return false;

            string RegularExpressions = @"^[^\u4E00-\u9FA5]";

            Regex rgx = new Regex(RegularExpressions);
            foreach (var item in str)
            {
                if (!rgx.IsMatch(item.ToString()))
                    return true;
            }
            return false;
        }
    }
}
