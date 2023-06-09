using AutoMapper;
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

using System.Collections.ObjectModel;
using System.Reflection;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;
namespace Wms3pl.WpfClient.P19.ViewModel
{
    public partial class P1901100000_ViewModel : InputViewModelBase
    {
        #region 共用變數/資料連結/頁面參數
        private string _userId;
        private string _userName;
        public Action ToFirstTab = delegate { };
        public Action ScrollIntoView = () => { };
        public Action OnUpdateTab = () => { };
        public string _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
        public string _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;

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

        private bool _commonSetIsExpanded = true;

        public bool CommonSetIsExpanded
        {
            get { return _commonSetIsExpanded; }
            set
            {
                _commonSetIsExpanded = value;
                RaisePropertyChanged("CommonSetIsExpanded");
            }
        }

        private bool _delvdaysTypeIsExpanded = true;

        public bool DelvdaysTypeIsExpanded
        {
            get { return _delvdaysTypeIsExpanded; }
            set
            {
                _delvdaysTypeIsExpanded = value;
                RaisePropertyChanged("DelvdaysTypeIsExpanded");
            }
        }

        #endregion

        #region 查詢
        #region Form - 業主
        private List<NameValuePair<string>> _gupCodes;

        public List<NameValuePair<string>> GupCodes
        {
            get { return _gupCodes; }
            set
            {
                _gupCodes = value;
                RaisePropertyChanged("GupCodes");
            }
        }

        private string _selectedGupCode;
        public string SelectedGupCode
        {
            get { return _selectedGupCode; }
            set
            {
                _selectedGupCode = value;
                RaisePropertyChanged("SelectedGupCode");
                DgList = new List<F1910>();
            }
        }
        #endregion

        #region Form - 門市編號
        private string _txtRETAIL_CODE = string.Empty;

        public string txtRETAIL_CODE
        {
            get { return _txtRETAIL_CODE; }
            set
            {
                _txtRETAIL_CODE = value;
                RaisePropertyChanged("txtRETAIL_CODE");
            }
        }
        #endregion
        #region Form - 門市名稱
        private string _txtRETAIL_NAME = string.Empty;

        public string txtRETAIL_NAME
        {
            get { return _txtRETAIL_NAME; }
            set
            {
                _txtRETAIL_NAME = value;
                RaisePropertyChanged("txtRETAIL_NAME");
            }
        }
        #endregion
        #region Data - 資料List
        private List<F1910> _dgList;
        public List<F1910> DgList
        {
            get { return _dgList; }
            set
            {
                _dgList = value;
                RaisePropertyChanged("DgList");
            }
        }

        private F1910 _selectedData;

        public F1910 SelectedData
        {
            get { return _selectedData; }
            set
            {
                _selectedData = value;
                RaisePropertyChanged("SelectedData");

                if (value == null)
                    return;

                _lastAddOrUpdateF1910 = ExDataMapper.Clone(SelectedData);
            }
        }
        #endregion

        #endregion

        #region 新增/編輯模式
        #region 業主
        private List<NameValuePair<string>> _gupList;

        public List<NameValuePair<string>> GupList
        {
            get { return _gupList; }
            set
            {
                _gupList = value;
                RaisePropertyChanged("GupList");
            }
        }
        #endregion

        #region Form - 門市群組
        private List<NameValuePair<string>> _saleBaseGroup;

        public List<NameValuePair<string>> SaleBaseGroup
        {
            get { return _saleBaseGroup; }
            set
            {
                Set(() => SaleBaseGroup, ref _saleBaseGroup, value);
            }
        }
        #endregion
        #region Form - 客製允出類型
        private List<NameValuePair<string>> _customDelvdaysType;

        public List<NameValuePair<string>> CustomDelvdaysType
        {
            get { return _customDelvdaysType; }
            set
            {
                Set(() => CustomDelvdaysType, ref _customDelvdaysType, value);
            }
        }
        #endregion
       
        private string _selectedCustomDelvdaysType;
        public string SelectedCustomDelvdaysType
        {
            get { return _selectedCustomDelvdaysType; }
            set
            {
                _selectedCustomDelvdaysType = value;
                RaisePropertyChanged("SelectedCustomDelvdaysType");
            }
        }
        #endregion

        #region Data 原始的門市主檔
        private F1910 _orgData;
        public F1910 OrgData
        {
            get { return _orgData; }
            set { _orgData = value; RaisePropertyChanged("OrgData"); }
        }
        #endregion


        #region 門市類別
        private List<NameValuePair<string>> _channel;

        public List<NameValuePair<string>> Channel
        {
            get { return _channel; }
            set
            {
                Set(() => Channel, ref _channel, value);
            }
        }
        #endregion


        #endregion

        #region 函式
        public P1901100000_ViewModel()
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
            //SetVnrTypeList();
            GupCodes = GetGupList(true);
            if (GupCodes.Any())
                SelectedGupCode = GupCodes.FirstOrDefault().Value;
            #region 新增/編輯模式
            GupList = GetGupList();
            var list = GetBaseTableService.GetF000904List(FunctionCode, "F050101", "CHANNEL");
            list.AddRange(GetBaseTableService.GetF000904List(FunctionCode, "F1910", "TYPE"));
            Channel = list;
            var saleBaseGroupList = GetBaseTableService.GetF000904List(FunctionCode, "F1910", "SALES_BASE_GROUP");
            SaleBaseGroup = saleBaseGroupList;
            var customDelvdaystypeList = GetBaseTableService.GetF000904List(FunctionCode, "F1910", "CUSTOM_DELVDAYS_TYPE");
            CustomDelvdaysType = customDelvdaystypeList;
            //CurrencyList = GetCurrencyList();
            //INVO_TypeList = GetINVO_TypeList();
            //PAY_TYPEList = GetPAY_TYPEList();
            //PAY_FACTORList = GetPAY_FACTORList();
            //DELI_TYPEList = GetDELI_TYPEList();
            #endregion
        }

        private string GetQuoteHeaderText()
        {
            switch (UserOperateMode)
            {
                case OperateMode.Edit:
                    return Properties.Resources.P1901100000_SaleBaseManage;
                case OperateMode.Add:
                    return Properties.Resources.P1901100000_SaleBaseInsert;
                default:
                    return Properties.Resources.P1901100000_SaleBaseDetails;
            }
        }

        public List<NameValuePair<string>> GetGupList(bool canAll = false)
        {
            var query = from item in Wms3plSession.Get<GlobalInfo>().DcGupCustDatas
                        where item.CustCode == _custCode
                        group item by new { item.GupCode, item.GupName } into g
                        select new NameValuePair<string>()
                        {
                            Value = g.Key.GupCode,
                            Name = g.Key.GupName
                        };
            var list = query.ToList();
            if (canAll)
            {
                list.Insert(0, new NameValuePair<string>() { Value = string.Empty, Name = Resources.Resources.All });
            }
            return list;
        }

        public List<NameValuePair<string>> GetCustList(string gupCode, bool canAll = false)
        {
            var query = from item in Wms3plSession.Get<GlobalInfo>().DcGupCustDatas
                        where item.GupCode == gupCode
                        group item by new { item.CustCode, item.CustName } into g
                        select new NameValuePair<string>()
                        {
                            Value = g.Key.CustCode,
                            Name = g.Key.CustName
                        };
            var list = query.ToList();
            if (canAll)
            {
                list.Insert(0, new NameValuePair<string>() { Value = string.Empty, Name = Resources.Resources.All });
            }
            return list;
        }

        public void SetData(F1910 data)
        {
            if (data == null) return;
            // 0.先清空Select
            SelectedData = null;
            // 1.設定要顯示的資料
            SelectedData = Mapper.DynamicMap<F1910>(data);
            //SearchCOUDIVCode = SelectedData.COUDIV_ID;
            // 2.將資料備份起來
            OrgData = Mapper.DynamicMap<F1910>(SelectedData);
        }

        public string[] GetORD_DATEtoArray(string selectedORD_DATE)
        {
            string[] tmpArray = new string[8];
            if (selectedORD_DATE.Contains("1"))
                tmpArray[1] = "1";
            else
                tmpArray[1] = "0";
            if (selectedORD_DATE.Contains("2"))
                tmpArray[2] = "1";
            else
                tmpArray[2] = "0";
            if (selectedORD_DATE.Contains("3"))
                tmpArray[3] = "1";
            else
                tmpArray[3] = "0";
            if (selectedORD_DATE.Contains("4"))
                tmpArray[4] = "1";
            else
                tmpArray[4] = "0";
            if (selectedORD_DATE.Contains("5"))
                tmpArray[5] = "1";
            else
                tmpArray[5] = "0";
            if (selectedORD_DATE.Contains("6"))
                tmpArray[6] = "1";
            else
                tmpArray[6] = "0";
            if (selectedORD_DATE.Contains("7"))
                tmpArray[7] = "1";
            else
                tmpArray[7] = "0";

            return tmpArray;
        }

        public string GetArrayToORD_DATE(string[] arrayORD_DATE)
        {
            string tmpOrdDate = "";
            if (arrayORD_DATE != null)
            {
                for (int i = 1; i <= 7; i++)
                {
                    if (arrayORD_DATE[i] == "1")
                        tmpOrdDate = string.Format("{0}{1}", tmpOrdDate, i);
                }
            }
            return tmpOrdDate;
        }
        #endregion

        #region Command
        #region Search
        public ICommand SearchCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                        o => DoSearch(), () => UserOperateMode == OperateMode.Query,
                        o =>
                        {
                            SelectedData = DgList.FirstOrDefault();
                        }
                        );
            }
        }

        private void DoSearch()
        {
            //執行查詢動作
            var proxy = GetProxy<F19Entities>();
            DgList = proxy.CreateQuery<F1910>("GetAllowedF1910s")
                                                            .AddQueryExOption("gupCode", SelectedGupCode)
                                                            .AddQueryExOption("custCode", _custCode)
                                                            .AddQueryExOption("retailCode", txtRETAIL_CODE)
                                                            .AddQueryExOption("retailName", txtRETAIL_NAME)
                                                            .ToList();


            if (!DgList.Any())
            {
                SearchResultIsExpanded = !DgList.Any();
                SelectedData = null;
                DgList = new List<F1910>();
                ShowMessage(Messages.InfoNoData);
                return;
            }
            QueryResultIsExpanded = DgList.Count > 1;
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
            SelectedData = new F1910();
            SelectedData.GUP_CODE = _gupCode;
            SelectedData.CUST_CODE = _custCode;

            UserOperateMode = OperateMode.Add;
            QuoteHeaderText = GetQuoteHeaderText();
            OnUpdateTab();

        }
        #endregion Add

        #region Edit
        public ICommand EditCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                        o => DoEdit(), () => UserOperateMode == OperateMode.Query && SelectedData != null
                        );
            }
        }

        private void DoEdit()
        {
            //ORD_DATEArray = GetORD_DATEtoArray(SelectedData.ORD_DATE);

            UserOperateMode = OperateMode.Edit;
            QuoteHeaderText = GetQuoteHeaderText();
            OnUpdateTab();
            //執行編輯動作
        }
        #endregion Edit

        #region Cancel
        public ICommand CancelCommand
        {
            get
            {
                bool isCancel = false;
                return CreateBusyAsyncCommand(
                        o => isCancel = DoCancel(),
                        () => UserOperateMode != OperateMode.Query,
                        o =>
                        {
                            if (isCancel)
                            {
                                UserOperateMode = OperateMode.Query;
                                QuoteHeaderText = GetQuoteHeaderText();
                                if (_lastAddOrUpdateF1910 != null)
                                {
                                    SelectedData = DgList.Where(item => item.GUP_CODE == _lastAddOrUpdateF1910.GUP_CODE && (item.CUST_CODE == "0" || item.CUST_CODE == _lastAddOrUpdateF1910.CUST_CODE)
                                            && item.RETAIL_CODE == _lastAddOrUpdateF1910.RETAIL_CODE)
                                            .FirstOrDefault();

                                    //ToFirstTab();
                                    ScrollIntoView();
                                }
                            }
                        }
                        );
            }
        }

        private bool DoCancel()
        {
            if (ShowMessage(Messages.WarningBeforeCancel) == DialogResponse.Yes)
            {
                //執行取消動作
                //if (UserOperateMode == OperateMode.Add)
                DoSearch();
                return true;
            }
            return false;
        }
        #endregion Cancel

        #region Delete
        public ICommand DeleteCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                        o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedData != null
                        );
            }
        }

        private void DoDelete()
        {
            // 確認是否要刪除
            if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return;
            //執行刪除動作
            if (SelectedData == null)
            {
                ShowMessage(Properties.Resources.P1901100000_DeleteFail);
                return;
            }
            var proxy = GetModifyQueryProxy<F19Entities>();
            var f1909 = proxy.F1909s.Where(x => x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode).FirstOrDefault();
            var custCode = _custCode;
            if (f1909 != null && f1909.ALLOWGUP_RETAILSHARE == "1")
                custCode = "0";
            var f1910 = proxy.F1910s.Where(x => x.GUP_CODE == SelectedData.GUP_CODE && x.CUST_CODE == custCode
                                                   && x.RETAIL_CODE == SelectedData.RETAIL_CODE).SingleOrDefault();

            if (f1910 == null)
            {
                ShowMessage(Messages.WarningBeenDeleted);
            }
            else
            {
                proxy.DeleteObject(f1910);
                proxy.SaveChanges();
                ShowMessage(Messages.InfoDeleteSuccess);
            }
            // 刪除成功時重新載入資料
            DoSearch();

        }
        #endregion Delete

        F1910 _lastAddOrUpdateF1910 = null;

        #region Save
        public ICommand SaveCommand
        {
            get
            {
                bool isSaved = false;
                return CreateBusyAsyncCommand(
                        o => isSaved = DoSave(),
                        () => UserOperateMode != OperateMode.Query && SelectedData != null,
                        o =>
                        {
                            if (isSaved)
                            {
                                if (_lastAddOrUpdateF1910 != null)
                                {
                                    SelectedData = DgList.Where(item => item.GUP_CODE == _lastAddOrUpdateF1910.GUP_CODE && (item.CUST_CODE == _lastAddOrUpdateF1910.CUST_CODE || item.CUST_CODE == "0")
                                            && item.RETAIL_CODE == _lastAddOrUpdateF1910.RETAIL_CODE)
                                            .FirstOrDefault();

                                    //ToFirstTab();
                                    ScrollIntoView();
                                }
                            }
                        }
                        );
            }
        }

        private bool DoSave()
        {
            if (ShowMessage(Messages.WarningBeforeUpdate) == DialogResponse.Yes)
            {
                //執行確認儲存動作
                if (UserOperateMode == OperateMode.Add)
                    return DoSaveAdd();
                else if (UserOperateMode == OperateMode.Edit)
                    return DoSaveEdit();
            }

            return false;
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

            //新增客製允出類型
            SelectedData.CUSTOM_DELVDAYS_TYPE = _selectedCustomDelvdaysType;

            SelectedData.CUST_CODE = _custCode;
            var f1910 = ExDataMapper.Map<F1910, wcf.F1910>(SelectedData);

            var wcfProxy = GetWcfProxy<wcf.P19WcfServiceClient>();
            var result = wcfProxy.RunWcfMethod(w => w.InsertOrUpdateF1910(f1910, true));
            if (!result.IsSuccessed)
            {
                ShowWarningMessage(result.Message);
                return false;
            }
            _lastAddOrUpdateF1910 = ExDataMapper.Clone(SelectedData);
            ShowMessage(Messages.Success);
            UserOperateMode = OperateMode.Query;
            SelectedGupCode = string.Empty;
            txtRETAIL_CODE = SelectedData.RETAIL_CODE;
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

            //新增客製允出類型
            SelectedData.CUSTOM_DELVDAYS_TYPE = _selectedCustomDelvdaysType;

            SelectedData.CUST_CODE = _custCode;
            var f1910 = ExDataMapper.Map<F1910, wcf.F1910>(SelectedData);

            var wcfProxy = GetWcfProxy<wcf.P19WcfServiceClient>();
            var result = wcfProxy.RunWcfMethod(w => w.InsertOrUpdateF1910(f1910, false));
            if (!result.IsSuccessed)
            {
                ShowWarningMessage(result.Message);
                return false;
            }
            _lastAddOrUpdateF1910 = ExDataMapper.Clone(SelectedData);
            ShowMessage(Messages.Success);
            UserOperateMode = OperateMode.Query;
            SelectedGupCode = string.Empty;
            txtRETAIL_CODE = SelectedData.RETAIL_CODE;
            DoSearch();
            return true;
        }

        string GetEditableError(F1910 e)
        {
            //if (!string.IsNullOrEmpty(e.DELV_TIME))
            //{
            //    DateTime time;
            //    if (!DateTime.TryParseExact(e.DELV_TIME, "HH:mm", null, System.Globalization.DateTimeStyles.None, out time))
            //        return Properties.Resources.P1901090000_TimeFormat_Error;
            //}
            StringBuilder ErrorMessage = new StringBuilder();

            if (string.IsNullOrEmpty(e.RETAIL_CODE))
                ErrorMessage.Append(Properties.Resources.P1901100000_SaleBaseNoNeed);

            if (string.IsNullOrEmpty(e.RETAIL_NAME))
                ErrorMessage.Append(Properties.Resources.P1901100000_SaleBaseNameNeed);

            if (string.IsNullOrEmpty(e.CONTACT))
                ErrorMessage.Append(Properties.Resources.P1901100000_ContactNeed);

            if (string.IsNullOrEmpty(e.TEL))
                ErrorMessage.Append(Properties.Resources.P1901100000_PhoneNumberNeed);

            if (string.IsNullOrEmpty(e.ADDRESS))
                ErrorMessage.Append(Properties.Resources.P1901100000_AdrNeed);
            if (string.IsNullOrEmpty(e.CHANNEL))
                ErrorMessage.Append(Properties.Resources.P1901100000_SaleBaseTypeNeed);

            return ErrorMessage.ToString();
        }

        #endregion Save
        #endregion
    }
}
