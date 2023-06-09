using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;

namespace Wms3pl.WpfClient.P19.ViewModel
{
    public class P1901920000_ViewModel : InputViewModelBase
    {
        private Boolean IsCheckDCChange = true;
        private P19ExDataSource _exproxy;
        private F19Entities _proxy;

        public P1901920000_ViewModel()
        {
            if (!IsInDesignMode)
            {
                InitControls();
            }
        }

        private void InitControls()
        {
            CollectionList = new ObservableCollection<F1945CollectionList>();
            CollectionCellList = new ObservableCollection<F1945CellList>();
            SetDcCode();
            GetCollectionTypeList();
            _exproxy = GetExProxy<P19ExDataSource>();
            _proxy = GetProxy<F19Entities>();
        }

        private void SetDcCode()
        {
            DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
            if (DcList.Any())
            {
                SelectedDc = DcList.First().Value;
                SelectedEditDc = DcList.First().Value;
            }
        }
        private void GetCollectionTypeList()
        {
            CollectionTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F1945", "COLLECTION_TYPE", true);
            SelectedCollectionType = CollectionTypeList.FirstOrDefault().Value;

            EditCollectionTypeList = CollectionTypeList.ToList();
            EditCollectionTypeList.RemoveAt(0); //編輯的清單不用全部的項目
            SelectedEditCollectionType = EditCollectionTypeList.FirstOrDefault().Value;
        }

        private void GetEditCollectionCellTypeList()
        {
            if (_proxy == null)
                return;
            EditCollectionCellTypeList = _proxy.CreateQuery<F194501>("GetF194501ByDcCode")
               .AddQueryExOption("dcCode", SelectedEditDc)
               .ToObservableCollection();
            SelectedEditCollectionCellType = EditCollectionCellTypeList.FirstOrDefault();
        }

        public void ChangeModifyModeToC()
        {
            var UpdateModifyMode = EditCollectionCellList.Where(x => x.CELL_TYPE == SelectedEditCollectionCell.CELL_TYPE).FirstOrDefault();
            if (UpdateModifyMode != null)
            {
                if (UpdateModifyMode.MODIFY_MODE == "N")
                {
                    UpdateModifyMode.MODIFY_MODE = "C";
                    EditCollectionCellList = EditCollectionCellList;
                }
            }
        }

        #region UI屬性

        #region 物流中心清單
        private List<NameValuePair<string>> _dcList;
        /// <summary>
        /// 物流中心清單
        /// </summary>
        public List<NameValuePair<string>> DcList
        {
            get { return _dcList; }
            set { Set(() => DcList, ref _dcList, value); }
        }
        #endregion 物流中心清單

        #region 所選的物流中心
        private string _selectedDc;
        /// <summary>
        /// 所選的物流中心
        /// </summary>
        public string SelectedDc
        {
            get { return _selectedDc; }
            set { Set(() => SelectedDc, ref _selectedDc, value); }
        }

        private string _OriSelectedEditDc;
        private string _SelectedEditDc;
        /// <summary>
        /// 所選的物流中心
        /// </summary>
        public string SelectedEditDc
        {
            get { return _SelectedEditDc; }
            set
            {
                _SelectedEditDc = value;
                RaisePropertyChanged();
                CheckDCChangeCommand.Execute(null);
                GetEditCollectionCellTypeList();
            }
        }

        private ICommand CheckDCChangeCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                o => { },
                () => true,
                o =>
                {
                    if (!IsCheckDCChange)
                        return;
                    if (SelectedEditDc != _OriSelectedEditDc)
                    {
                        if (EditCollectionCellList?.Any(x => x.MODIFY_MODE != "N") ?? false)
                        {
                            if (ShowConfirmMessage("物流中心有異動，是否要清除集貨格清單") == UILib.Services.DialogResponse.Yes)
                            {
                                EditCollectionCellList = new ObservableCollection<F1945CellList>();
                                _OriSelectedEditDc = SelectedEditDc;

                                return;
                            }
                            else
                            {
                                IsCheckDCChange = false;
                                SelectedEditDc = _OriSelectedEditDc;
                                IsCheckDCChange = true;
                            }
                        }
                    }
                });
            }
        }
        #endregion 所選的物流中心

        #region Selectedtabindex
        private int _Selectedtabindex = 0;
        public int Selectedtabindex
        {
            get { return _Selectedtabindex; }
            set { _Selectedtabindex = value; RaisePropertyChanged(); }
        }
        #endregion Selectedtabindex

        #region 集貨場編號
        private string _CollectionCode;
        public string CollectionCode
        {
            get { return _CollectionCode; }
            set { _CollectionCode = value; RaisePropertyChanged(); }
        }

        #endregion 集貨場編號

        #region 集貨場類型
        private List<NameValuePair<string>> _CollectionTypeList;
        /// <summary>
        /// 集貨場類型清單
        /// </summary>
        public List<NameValuePair<string>> CollectionTypeList
        {
            get { return _CollectionTypeList; }
            set { _CollectionTypeList = value; RaisePropertyChanged(); }
        }

        private string _SelectedCollectionType;
        /// <summary>
        /// 所選的集貨場類型
        /// </summary>
        public string SelectedCollectionType
        {
            get { return _SelectedCollectionType; }
            set { _SelectedCollectionType = value; RaisePropertyChanged(); }
        }

        private List<NameValuePair<string>> _EditCollectionTypeList;
        /// <summary>
        /// 集貨場類型清單
        /// </summary>
        public List<NameValuePair<string>> EditCollectionTypeList
        {
            get { return _EditCollectionTypeList; }
            set { _EditCollectionTypeList = value; RaisePropertyChanged(); }
        }


        private string _SelectedEditCollectionType;
        /// <summary>
        /// 所選的集貨場類型
        /// </summary>
        public string SelectedEditCollectionType
        {
            get { return _SelectedEditCollectionType; }
            set { _SelectedEditCollectionType = value; RaisePropertyChanged(); }
        }

        #endregion 集貨場類型

        #region 集貨場清單
        private ObservableCollection<F1945CollectionList> _CollectionList;
        public ObservableCollection<F1945CollectionList> CollectionList
        {
            get { return _CollectionList; }
            set
            {
                _CollectionList = value; RaisePropertyChanged();
            }
        }

        private F1945CollectionList _SelectedCollectionList;
        public F1945CollectionList SelectedCollectionList
        {
            get { return _SelectedCollectionList; }
            set
            {
                _SelectedCollectionList = value;
                RaisePropertyChanged();

                if (SelectedCollectionList != null)
                {
                    CollectionCellList = _exproxy.CreateQuery<F1945CellList>("GetF1945CellList")
                                              .AddQueryExOption("dcCode", SelectedCollectionList.DC_CODE)
                                              .AddQueryExOption("CollectionCode", SelectedCollectionList.COLLECTION_CODE)
                                              .ToObservableCollection();
                    SelectedCollectionCell = CollectionCellList.FirstOrDefault();
                }
            }
        }
        #endregion 集貨場清單

        #region 集貨格清單
        private ObservableCollection<F1945CellList> _CollectionCellList;
        public ObservableCollection<F1945CellList> CollectionCellList
        {
            get { return _CollectionCellList; }
            set
            {
                _CollectionCellList = value; RaisePropertyChanged();
            }
        }

        private F1945CellList _SelectedCollectionCell;
        public F1945CellList SelectedCollectionCell
        {
            get { return _SelectedCollectionCell; }
            set
            {
                _SelectedCollectionCell = value; RaisePropertyChanged();
            }
        }


        private ObservableCollection<F1945CellList> _OriEditCollectionCellList;
        private ObservableCollection<F1945CellList> _EditCollectionCellList;
        public ObservableCollection<F1945CellList> EditCollectionCellList
        {
            get { return _EditCollectionCellList; }
            set
            {
                _EditCollectionCellList = value; RaisePropertyChanged();
            }
        }

        private F1945CellList _SelectedEditCollectionCell;
        public F1945CellList SelectedEditCollectionCell
        {
            get { return _SelectedEditCollectionCell; }
            set
            {
                _SelectedEditCollectionCell = value; RaisePropertyChanged();
            }
        }
        #endregion 集貨格清單

        #region 集貨格類型
        private ObservableCollection<F194501> _EditCollectionCellTypeList;
        public ObservableCollection<F194501> EditCollectionCellTypeList
        {
            get { return _EditCollectionCellTypeList; }
            set
            {
                _EditCollectionCellTypeList = value;
                RaisePropertyChanged();
                if (EditCollectionCellTypeList?.Any() ?? false)
                {
                    SelectedEditCollectionCellType = EditCollectionCellTypeList.FirstOrDefault();
                }
            }
        }

        private F194501 _SelectedEditCollectionCellType;
        public F194501 SelectedEditCollectionCellType
        {
            get { return _SelectedEditCollectionCellType; }
            set
            {
                _SelectedEditCollectionCellType = value; RaisePropertyChanged();
            }
        }

        #endregion 集貨格類型

        #region 集貨場編號
        private string _EditCollectionCode = "";
        /// <summary>
        /// 集貨場編號
        /// </summary>
        public string EditCollectionCode
        {
            get { return _EditCollectionCode; }
            set { _EditCollectionCode = value; RaisePropertyChanged(); }
        }
        #endregion 集貨場編號

        #region 集貨場名稱
        private string _OriEditCollectionName = "";
        private string _EditCollectionName = "";
        /// <summary>
        /// 集貨場名稱
        /// </summary>
        public string EditCollectionName
        {
            get { return _EditCollectionName; }
            set { _EditCollectionName = value; RaisePropertyChanged(); }
        }
        #endregion 集貨場名稱

        #region 集貨格開頭編號
        private string _EditCellStartCode;
        /// <summary>
        /// 集貨格開頭編號
        /// </summary>
        public string EditCellStartCode
        {
            get { return _EditCellStartCode; }
            set { _EditCellStartCode = value; RaisePropertyChanged(); }
        }
        #endregion 集貨格開頭編號

        #region 集貨格數量
        private int _EditCellNum = 1;
        /// <summary>
        /// 集貨格數量
        /// </summary>
        public int EditCellNum
        {
            get { return _EditCellNum; }
            set { _EditCellNum = value; RaisePropertyChanged(); }
        }
        #endregion 集貨格數量

        private Boolean _IsReadOnly = true;
        public Boolean IsReadOnly
        {
            get { return _IsReadOnly; }
            set { Set(() => IsReadOnly, ref _IsReadOnly, value); }
        }
        #endregion UI屬性

        #region ICommad
        public ICommand SearchCommand
        {
            get
            {
                var tmpCollectionList = CollectionList;
                return CreateBusyAsyncCommand(
                       o => { tmpCollectionList = DoSearch(); },
                       () => UserOperateMode == OperateMode.Query,
                       o =>
                       {
                           CollectionList = tmpCollectionList;
                           SelectedCollectionList = CollectionList.FirstOrDefault();
                       },
                       o => { },
                       () =>
                       {
                           CollectionList.Clear();
                           CollectionList = CollectionList;
                           CollectionCellList.Clear();
                           CollectionCellList = CollectionCellList;
                       });
            }
        }

        public ObservableCollection<F1945CollectionList> DoSearch()
        {
            var result = _exproxy.CreateQuery<F1945CollectionList>("GetF1945CollectionList")
                                        .AddQueryExOption("dcCode", SelectedDc)
                                        .AddQueryExOption("CollectionCode", CollectionCode)
                                        .AddQueryExOption("CollectionType", SelectedCollectionType)
                                        .ToObservableCollection();
            if (!result.Any())
                ShowInfoMessage("查無資料");
            return result;
        }

        public ICommand AddCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                       o =>
                       {
                           UserOperateMode = OperateMode.Add;
                           Selectedtabindex = 1;
                       },
                       () => UserOperateMode == OperateMode.Query,
                       o =>
                       {
                           GetEditCollectionCellTypeList();
                           EditCollectionCellList = new ObservableCollection<F1945CellList>();
                       }
              );
            }
        }

        public ICommand EditCommand
        {
            get
            {
                ObservableCollection<F1945CellList> queryResult = null;
                return CreateBusyAsyncCommand(
                       o =>
                       {
                           queryResult = LoadEditData();
                       },
                       () => UserOperateMode == OperateMode.Query && SelectedCollectionCell != null,
                       o =>
                       {
                           UserOperateMode = OperateMode.Edit;
                           Selectedtabindex = 1;
                           SelectedEditDc = SelectedCollectionList.DC_CODE;
                           SelectedEditCollectionType = SelectedCollectionList.COLLECTION_TYPE;
                           EditCollectionCode = SelectedCollectionList.COLLECTION_CODE;
                           EditCollectionName = SelectedCollectionList.COLLECTION_NAME;
                           _OriEditCollectionName = SelectedCollectionList.COLLECTION_NAME;
                           GetEditCollectionCellTypeList();
                           if (queryResult?.Any() ?? false)
                           {
                               EditCollectionCellList = queryResult;
                               _OriEditCollectionCellList = queryResult;
                           }
                       }
              );
            }
        }

        private ObservableCollection<F1945CellList> LoadEditData()
        {
            return _exproxy.CreateQuery<F1945CellList>("GetF1945CellList")
                           .AddQueryExOption("dcCode", SelectedCollectionList.DC_CODE)
                           .AddQueryExOption("CollectionCode", SelectedCollectionList.COLLECTION_CODE)
                           .ToObservableCollection();
        }

        public void UpdateModifyMode()
        {

        }

        public ICommand CancelCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                       o =>
                       {
                       },
                       () => UserOperateMode != OperateMode.Query,
                       o =>
                       {
                           UserOperateMode = OperateMode.Query;
                           Selectedtabindex = 0;

                           ClearEditTabItem();
                       }
              );
            }
        }

        private void ClearEditTabItem()
        {
            EditCollectionCellList = new ObservableCollection<F1945CellList>();
            SelectedEditDc = DcList.First().Value;
            SelectedEditCollectionType = EditCollectionTypeList.First().Value;
            EditCollectionCode = "";
            EditCollectionName = "";
            EditCellStartCode = "";
            EditCellNum = 1;
            SelectedEditCollectionCellType = EditCollectionCellTypeList.First();
        }

        public ICommand DeleteCommand
        {
            get
            {
                ExecuteResult result = null;
                return CreateBusyAsyncCommand(
                       o =>
                       {
                           result = DoDelete();
                       },
                       () => UserOperateMode == OperateMode.Query,
                       o =>
                       {
                           if (result.IsSuccessed)
                               SearchCommand.Execute(null);
                       }
              );
            }
        }

        private ExecuteResult DoDelete()
        {
            if (ShowConfirmMessage($"您確定要刪除{SelectedCollectionList.COLLECTION_NAME}集貨場?") != UILib.Services.DialogResponse.Yes)
                return new ExecuteResult() { IsSuccessed = false };
            var result = _exproxy.CreateQuery<ExecuteResult>("DeleteF1945Collection")
                               .AddQueryExOption("dcCode", SelectedCollectionList.DC_CODE)
                               .AddQueryExOption("CollectionCode", SelectedCollectionList.COLLECTION_CODE)
                               .ToList()
                               .FirstOrDefault();
            ShowResultMessage(result);
            return result;
        }

        public ICommand SaveCommand
        {
            get
            {
                Boolean Result = false;
                return CreateBusyAsyncCommand(
                       o =>
                       {
                           Result = false;
                           if (DoVerify())
                               if (UserOperateMode == OperateMode.Add)
                                   Result = DoAdd();
                               else
                                   Result = DoUpdate();
                       },
                       () => UserOperateMode != OperateMode.Query,
                       o =>
                       {
                           if (Result)
                           {
                               UserOperateMode = OperateMode.Query;
                               Selectedtabindex = 0;
                               ClearEditTabItem();
                               SearchCommand.Execute(null);
                           }
                       }
              );
            }
        }

        private Boolean DoVerify()
        {
            String msg = null;
            var f19Proxy = GetProxy<F19Entities>();
            var wcfproxy = GetWcfProxy<wcf.P19WcfServiceClient>();

            var tmpEditCollectionCode = EditCollectionCode;
            var tmpEditCollectionName = EditCollectionName;
            CollectionDataTrimToUpper(ref tmpEditCollectionCode, ref tmpEditCollectionName);

            foreach (var item in EditCollectionCellList)
                item.CELL_START_CODE = item.CELL_START_CODE.Trim().ToUpper();

            var a = f19Proxy.F1945s.Where(x => x.DC_CODE == SelectedEditDc).ToList();

            if (String.IsNullOrEmpty(tmpEditCollectionCode))
                msg = "集貨場編號必填";
            else if (String.IsNullOrEmpty(EditCollectionName))
                msg = "集貨場名稱必填";
            else if (_OriEditCollectionName != tmpEditCollectionName && f19Proxy.F1945s.Where(x => x.DC_CODE == SelectedEditDc && x.COLLECTION_NAME == tmpEditCollectionName).ToList().Any())
                msg = "集貨場名稱已存在";
            else if (!EditCollectionCellList.Any(x => x.MODIFY_MODE != "D"))
                msg = "至少設定一筆集貨格類型資料";
            else if (EditCollectionCellList.Any(x => String.IsNullOrEmpty(x.CELL_START_CODE)))
                msg = $"集貨格類型:{EditCollectionCellList.First(x => String.IsNullOrEmpty(x.CELL_START_CODE)).CELL_TYPE} 集貨格編號必填";
            else if (EditCollectionCellList.Any(x => x.CELL_NUM <= 0))
                msg = "集貨格數量必須大於0";
            else if (UserOperateMode == OperateMode.Add)
            {
                if (f19Proxy.F1945s.Where(x => x.DC_CODE == SelectedEditDc && x.COLLECTION_CODE == tmpEditCollectionCode).ToList().Any())
                    msg = "集貨場編號已存在";
                if (string.IsNullOrWhiteSpace(msg))
                {
                    var GrpCellStartCode = EditCollectionCellList.GroupBy(x => x.CELL_START_CODE).Where(x => x.Count() > 1).Select(x => x.Key);
                    if (GrpCellStartCode.Any())
                        msg = $"集貨格開頭編號{String.Join(",", GrpCellStartCode)}重複";
                }
            }
            else if (UserOperateMode == OperateMode.Edit)
            {
                foreach (var item in EditCollectionCellList.Where(x => new[] { "C", "D" }.Contains(x.MODIFY_MODE)))
                {
                    int tmpCellNum;
                    if (item.MODIFY_MODE == "D")
                        tmpCellNum = 0;
                    else
                        tmpCellNum = item.CELL_NUM;

                    if (!wcfproxy.RunWcfMethod(w => w.IsCellNumCanReduce(SelectedEditDc, tmpEditCollectionCode, item.CELL_TYPE, tmpCellNum)))
                    {
                        if (item.MODIFY_MODE == "D")
                            msg = $"此集貨格類型{item.CELL_TYPE}上有集貨格正在集貨中，不可刪除";
                        else
                            msg = $"此集貨格類型{item.CELL_TYPE}上，調整移除集貨格範圍區間內正在集貨中，不可調整";
                    }
                }

            }

            if (!string.IsNullOrEmpty(msg))
            {
                ShowWarningMessage(msg);
                return false;
            }

            return true;
        }

        private Boolean DoAdd()
        {
            var tmpEditCollectionCode = EditCollectionCode;
            var tmpEditCollectionName = EditCollectionName;
            CollectionDataTrimToUpper(ref tmpEditCollectionCode, ref tmpEditCollectionName);

            var wcfProxy = GetWcfProxy<wcf.P19WcfServiceClient>();
            var wcfEditCollectionCellList = EditCollectionCellList.MapCollection<F1945CellList, wcf.F1945CellList>().ToArray();

            var result = wcfProxy.RunWcfMethod(w => w.InsertF1945(SelectedEditDc, Wms3plSession.Get<GlobalInfo>().GupCode, Wms3plSession.Get<GlobalInfo>().CustCode, SelectedEditCollectionType, tmpEditCollectionCode, tmpEditCollectionName, wcfEditCollectionCellList));

            return result.IsSuccessed;
        }

        private Boolean DoUpdate()
        {
            var tmpEditCollectionCode = EditCollectionCode;
            var tmpEditCollectionName = EditCollectionName;
            CollectionDataTrimToUpper(ref tmpEditCollectionCode, ref tmpEditCollectionName);

            var wcfProxy = GetWcfProxy<wcf.P19WcfServiceClient>();
            var wcfEditCollectionCellList = EditCollectionCellList.MapCollection<F1945CellList, wcf.F1945CellList>().ToArray();

            var result = wcfProxy.RunWcfMethod(w => w.UpdateF051401(SelectedEditDc, Wms3plSession.Get<GlobalInfo>().GupCode, Wms3plSession.Get<GlobalInfo>().CustCode, SelectedEditCollectionType, tmpEditCollectionCode, tmpEditCollectionName, wcfEditCollectionCellList));

            return result.IsSuccessed;

        }


        private void CollectionDataTrimToUpper(ref String fCollectionCode, ref String fCollectionName)
        {
            fCollectionCode = fCollectionCode.Trim().ToUpper();
            fCollectionName = fCollectionName.Trim();

            if (!fCollectionName.StartsWith(fCollectionCode))
                fCollectionName = fCollectionCode + " " + fCollectionName;
        }


        public ICommand AddItemCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                       o =>
                       { },
                       () => true,
                       o =>
                       {
                           var msg = "";
                           if (String.IsNullOrEmpty(EditCellStartCode))
                               msg = "集貨格開頭編號必填";
                           else if (EditCellNum <= 0)
                               msg = "集貨格數量必須大於0";
                           if (CheckCellTypeDuplicate(SelectedEditCollectionCellType.CELL_TYPE))
                               msg = "已有重複的集貨格";
                           if (CheckCellStartCodeDuplicate(EditCellStartCode))
                               msg = "已有重複的集貨格開頭編號";

                           if (!String.IsNullOrEmpty(msg))
                           {
                               ShowWarningMessage(msg);
                               return;
                           }

                           _OriSelectedEditDc = SelectedEditDc;
                           EditCollectionCellList.Add(
                               new F1945CellList()
                               {
                                   DC_CODE = SelectedEditDc,
                                   CELL_TYPE = SelectedEditCollectionCellType.CELL_TYPE,
                                   CELL_NAME = SelectedEditCollectionCellType.CELL_NAME,
                                   CELL_START_CODE = EditCellStartCode,
                                   CELL_NUM = EditCellNum,
                                   MODIFY_MODE = "A",
                                   IS_SHOW_DELETE_BUTTON = true
                               });

                           EditCellStartCode = "";
                           EditCellNum = 1;
                       });
            }
        }

        /// <summary>
        /// 檢查輸入的CELL_TYPE項目是否已存在於EditCollectionCellList
        /// </summary>
        /// <param name="InputValue"></param>
        /// <returns></returns>
        private Boolean CheckCellTypeDuplicate(String CellType)
        {
            if (UserOperateMode == OperateMode.Add)
                return EditCollectionCellList.Any(x => x.CELL_TYPE == CellType);
            else
                return EditCollectionCellList.Where(x => x.MODIFY_MODE != "D").Any(x => x.CELL_TYPE == CellType);
        }

        /// <summary>
        /// 檢查輸入的CELL_START_CODE項目是否已存在於EditCollectionCellList
        /// </summary>
        /// <param name="CellStartCode"></param>
        /// <returns></returns>
        public Boolean CheckCellStartCodeDuplicate(String CellStartCode)
        {
            if (UserOperateMode == OperateMode.Add)
                return EditCollectionCellList.Any(x => x.CELL_START_CODE == CellStartCode);
            else
                return EditCollectionCellList.Where(x => x.MODIFY_MODE != "D").Any(x => x.CELL_START_CODE == CellStartCode);
        }

        public ICommand DeleteItmeCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => { },
                    () => true,
                    o =>
                    {
                        if (UserOperateMode == OperateMode.Add)
                        {
                            EditCollectionCellList.Remove(SelectedEditCollectionCell);
                            EditCollectionCellList = EditCollectionCellList;
                        }
                        else
                        {
                            if (SelectedEditCollectionCell.MODIFY_MODE == "A")
                            {
                                EditCollectionCellList.Remove(SelectedEditCollectionCell);
                                EditCollectionCellList = EditCollectionCellList;
                            }
                            else if (new[] { "N", "C" }.Contains(SelectedEditCollectionCell.MODIFY_MODE))
                            {
                                SelectedEditCollectionCell.IS_SHOW_DELETE_BUTTON = false;
                                var tmpCollectionCellList = EditCollectionCellList.First(x => x.CELL_TYPE == SelectedEditCollectionCell.CELL_TYPE);
                                tmpCollectionCellList.IS_SHOW_DELETE_BUTTON = false;
                                tmpCollectionCellList.MODIFY_MODE = "D";
                                EditCollectionCellList = EditCollectionCellList;
                            }
                        }
                    });
            }
        }
        #endregion ICommad

    }

    //如果有新增類別，記得要去畫面新增對應的datatrigger
    public class ModifyModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                var b = value.ToString();
                switch (b)
                {
                    case "A":
                        return "新增";
                    case "N":
                        return "未異動";
                    case "C":
                        return "異動";
                    case "D":
                        return "待刪除";
                    default:
                        throw new Exception("無法識別的MODIFY_MODE");
                }
            }
            throw new Exception("無法識別的MODIFY_MODE");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                var b = value.ToString();
                switch (b)
                {
                    case "新增":
                        return "A";
                    case "未異動":
                        return "N";
                    case "異動":
                        return "C";
                    case "待刪除":
                        return "D";
                    default:
                        throw new Exception("無法識別的MODIFY_MODE");
                }
            }
            throw new Exception("無法識別的MODIFY_MODE");
        }
    }

    public class ModifyModeForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                var b = value.ToString();
                switch (b)
                {
                    case "A":
                        return new SolidColorBrush(Colors.Green);
                    case "N":
                        return new SolidColorBrush(Colors.Black);
                    case "C":
                        return new SolidColorBrush(Colors.Blue);
                    case "D":
                        return new SolidColorBrush(Colors.Red);
                    default:
                        return new SolidColorBrush(Colors.Yellow);
                }
            }
            throw new Exception("無法識別的MODIFY_MODE");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
