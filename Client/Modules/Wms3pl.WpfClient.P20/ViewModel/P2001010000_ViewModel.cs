using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Security.Permissions;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.Converters;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F20DataService;
using Wms3pl.WpfClient.DataServices.F25DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P20ExDataService;
using ex19 = Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P20WcfService;
using exShare = Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using ExecuteResult = Wms3pl.WpfClient.ExDataServices.P20ExDataService.ExecuteResult;
using System.Windows;
using System.IO;

namespace Wms3pl.WpfClient.P20.ViewModel
{
    public partial class P2001010000_ViewModel : InputViewModelBase
    {
        public P2001010000_ViewModel()
        {
            if (!IsInDesignMode)
            {
                //初始化執行時所需的值及資料
                _userId = Wms3plSession.Get<UserInfo>().Account;
                _userName = Wms3plSession.Get<UserInfo>().AccountName;
                SetDcList();
                SetAdjustTypeList();
                SetWorkTypeList();
                SetCauseList();
                SetCauseListByAdjustType1();
                BegAdjustDate = DateTime.Now;
                EndAdjustDate = DateTime.Now;
            }

        }

        #region Property

        private readonly string _userId;
        private readonly string _userName;

        public Action ItemAddClick = delegate { };
        public Action ItemEditClick = delegate { };
        public Action ResetAddDcCode1Selected = delegate { };
        public Action ExcelImport = delegate { };
        private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
        private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }

        #region 業主

        public string GupCode
        {
            get { return Wms3plSession.Get<GlobalInfo>().GupCode; }
        }

        #endregion

        #region 貨主

        public string CustCode
        {
            get { return Wms3plSession.Get<GlobalInfo>().CustCode; }
        }

        #endregion

        #region 品號
        //品號
        private string _itemCode = string.Empty;

        public string ItemCode
        {
            get { return _itemCode; }
            set
            {
                _itemCode = value;
                RaisePropertyChanged("ItemCode");
            }
        }
        #endregion

        #region 物流中心

        private List<NameValuePair<string>> _dcList;

        public List<NameValuePair<string>> DcList
        {
            get { return _dcList; }
            set
            {
                _dcList = value;
                RaisePropertyChanged("DcList");
            }
        }

        private string _selectedDcCode;

        public string SelectedDcCode
        {
            get { return _selectedDcCode; }
            set
            {
                _selectedDcCode = value;
                RaisePropertyChanged("SelectedDcCode");
                F200101Datas = null;
                SelectedF200101Data = null;
                F200102Datas = null;
                SelectedF200102Data = null;
            }
        }

        #endregion

        #region 調整單類別

        private List<NameValuePair<string>> _adjustTypeList;

        public List<NameValuePair<string>> AdjustTypeList
        {
            get { return _adjustTypeList; }
            set
            {
                _adjustTypeList = value;
                RaisePropertyChanged("AdjustTypeList");
            }
        }

        private string _selectedAdjustType;

        public string SelectedAdjustType
        {
            get { return _selectedAdjustType; }
            set
            {
                _selectedAdjustType = value;
                RaisePropertyChanged("SelectedAdjustType");
                RaisePropertyChanged("IsSelectedAdjustType3and5");
                F200101Datas = null;
                SelectedF200101Data = null;
                F200102Datas = null;
                SelectedF200102Data = null;
                F200103Datas = null;
                SelectedF200103Data = null;
                if (value == "0" || value == "1")
                    EditVisibility = Visibility.Visible;
                else
                    EditVisibility = Visibility.Collapsed;
				CauseMemoVisibility = new[] { "1", "2", "3", "4","5" }.Contains(SelectedAdjustType) ? Visibility.Visible : Visibility.Collapsed;
			}
        }

		private Visibility _causeMemoVisibility;
		public Visibility CauseMemoVisibility
		{
			get { return _causeMemoVisibility; }
			set
			{
				_causeMemoVisibility = value;
				RaisePropertyChanged("CauseMemoVisibility");
			}
		}
    #endregion

    


    public Visibility IsSelectedAdjustType3and5
    { get { return new[] { "3", "5" }.Contains(_selectedAdjustType) ? Visibility.Visible : Visibility.Collapsed; } }

    #endregion

    #region 作業類別

    private List<NameValuePair<string>> _workTypeList;

        public List<NameValuePair<string>> WorkTypeList
        {
            get { return _workTypeList; }
            set
            {
                _workTypeList = value;
                RaisePropertyChanged("WorkTypeList");
            }
        }

        private string _selectedWorkType;

        public string SelectedWorkType
        {
            get { return _selectedWorkType; }
            set
            {
                _selectedWorkType = value;
                RaisePropertyChanged("SelectedWorkType");
            }
        }

        private string _cause;
        public string CAUSE
        {
            get { return _cause; }
            set { Set(ref _cause, value); }
        }

        #endregion

        #region 調整單號

        private string _adjustNo;

        public string AdjustNo
        {
            get { return _adjustNo; }
            set
            {
                _adjustNo = value;
                RaisePropertyChanged("AdjustNo");
            }
        }

        #endregion

        #region 異動日期-起

        private DateTime? _begAdjustDate;

        public DateTime? BegAdjustDate
        {
            get { return _begAdjustDate; }
            set
            {
                _begAdjustDate = value;
                RaisePropertyChanged("BegAdjustDate");
            }
        }

        #endregion

        #region 異動日期-迄

        private DateTime? _endAdjustDate;

        public DateTime? EndAdjustDate
        {
            get { return _endAdjustDate; }
            set
            {
                _endAdjustDate = value;
                RaisePropertyChanged("EndAdjustDate");
            }
        }

        #endregion


        #region 是否顯示編輯明細按鈕
        private Visibility _editVisibility;

        public Visibility EditVisibility
        {
            get { return _editVisibility; }
            set
            {
                Set(() => EditVisibility, ref _editVisibility, value);
            }
        }
        #endregion


        #region 調整單主檔 Grid

        private List<F200101Data> _f200101Datas;

        public List<F200101Data> F200101Datas
        {
            get { return _f200101Datas; }
            set
            {
                _f200101Datas = value;
                RaisePropertyChanged("F200101Datas");
            }
        }

        private F200101Data _selectedF200101Data;

        public F200101Data SelectedF200101Data
        {
            get { return _selectedF200101Data; }
            set
            {
                _selectedF200101Data = value;
                RaisePropertyChanged("SelectedF200101Data");
                BindDetail();
                if (SelectedF200101Data != null && SelectedF200101Data.WORK_TYPE != oldWorkType)
                {
                    var proxy = GetProxy<F19Entities>();
                    string uctId = SelectedF200101Data.WORK_TYPE == "5" ? "AH" : "AJ";
                    var data = proxy.F1951s.Where(s => s.UCT_ID == uctId).ToList();
                    var list = (from s in data
                                select new NameValuePair<string>
                                {
                                    Name = s.CAUSE,
                                    Value = s.UCC_CODE
                                }).ToList();
                    EditCauseList = list;
                    oldWorkType = SelectedF200101Data.WORK_TYPE;
                }
            }
        }

        #endregion

        #region 調整單類別=訂單調整

        #region 調整單類別=訂單調整 調整單明細 Grid

        private List<F200102Data> _f200102Datas;

        public List<F200102Data> F200102Datas
        {
            get { return _f200102Datas; }
            set
            {
                _f200102Datas = value;
                RaisePropertyChanged("F200102Datas");
            }
        }

        private F200102Data _selectedF200102Data;

        public F200102Data SelectedF200102Data
        {
            get { return _selectedF200102Data; }
            set
            {
                _selectedF200102Data = value;
                RaisePropertyChanged("SelectedF200102Data");
            }
        }

        #endregion

        #region 調整單類別-新增

        private List<NameValuePair<string>> _addAdjustTypeList;

        public List<NameValuePair<string>> AddAdjustTypeList
        {
            get { return _addAdjustTypeList; }
            set
            {
                _addAdjustTypeList = value;
                RaisePropertyChanged("AddAdjustTypeList");
            }
        }

        private string _selectedAddAdjustType;

        public string SelectedAddAdjustType
        {
            get { return _selectedAddAdjustType; }
            set
            {
                _selectedAddAdjustType = value;
                RaisePropertyChanged("SelectedAddAdjustType");
            }
        }

        #endregion

        #region 作業類別-新增

        private List<NameValuePair<string>> _addWorkTypeList;

        public List<NameValuePair<string>> AddWorkTypeList
        {
            get { return _addWorkTypeList; }
            set
            {
                _addWorkTypeList = value;
                RaisePropertyChanged("AddWorkTypeList");
            }
        }

        private string _selectedAddWorkType;

        public string SelectedAddWorkType
        {
            get { return _selectedAddWorkType; }
            set
            {
                _selectedAddWorkType = value;
                RaisePropertyChanged("SelectedAddWorkType");
                if (value == "2")
                    IsCheckAll = false;
                SetCauseList();
                F050301Datas = null;
            }
        }
        #endregion

        #region 物流中心-新增查詢

        private List<NameValuePair<string>> _addDcList;

        public List<NameValuePair<string>> AddDcList
        {
            get { return _addDcList; }
            set
            {
                _addDcList = value;
                RaisePropertyChanged("AddDcList");
            }
        }

        private string _tempSelectedAddDcCode;
        private string _selectedAddDcCode;

        public string SelectedAddDcCode
        {
            get { return _selectedAddDcCode; }
            set
            {
                _selectedAddDcCode = value;
                RaisePropertyChanged("SelectedAddDcCode");
                F050301Datas = null;
                SelectedF050301Data = null;
                SetPickTime();
            }
        }

        #endregion

        #region 批次日期-新增查詢

        private DateTime _addDelvDate;

        public DateTime AddDelvDate
        {
            get { return _addDelvDate; }
            set
            {
                _addDelvDate = value;
                RaisePropertyChanged("AddDelvDate");
                SetPickTime();
            }
        }

        #endregion

        #region 批次時間-新增查詢

        private List<NameValuePair<string>> _addPickTimeList;

        public List<NameValuePair<string>> AddPickTimeList
        {
            get { return _addPickTimeList; }
            set
            {
                _addPickTimeList = value;
                RaisePropertyChanged("AddPickTimeList");
            }
        }

        private string _selectedAddPickTime;

        public string SelectedAddPickTime
        {
            get { return _selectedAddPickTime; }
            set
            {
                _selectedAddPickTime = value;
                RaisePropertyChanged("SelectedAddPickTime");
            }
        }

        #endregion

        #region 貨主單號-新增查詢

        private string _addCustOrdNo;

        public string AddCustOrdNo
        {
            get { return _addCustOrdNo; }
            set
            {
                _addCustOrdNo = value;
                RaisePropertyChanged("AddCustOrdNo");
            }
        }

        #endregion

        #region 商品編號-新增查詢

        private string _addItemCode;

        public string AddItemCode
        {
            get { return _addItemCode; }
            set
            {
                _addItemCode = value;
                RaisePropertyChanged("AddItemCode");
                AddItemName = "";
            }
        }

        #endregion

        #region 商品品名-新增查詢

        private string _addItemName;

        public string AddItemName
        {
            get { return _addItemName; }
            set
            {
                _addItemName = value;
                RaisePropertyChanged("AddItemName");
            }
        }

        #endregion

        #region 收件人-新增查詢

        private string _addConsignee;

        public string AddConsignee
        {
            get { return _addConsignee; }
            set
            {
                _addConsignee = value;
                RaisePropertyChanged("AddConsignee");
            }
        }

        #endregion

        #region 訂單單號

        private string _addOrdNo;
        public string AddOrdNo
        {
            get { return _addOrdNo; }
            set
            {
                _addOrdNo = value;
                RaisePropertyChanged("AddOrdNo");
            }
        }
        #endregion

        #region 配送商-新增 WorkType1

        private List<NameValuePair<string>> _addAllIdListByWorkType1;

        public List<NameValuePair<string>> AddAllIdListByWorkType1
        {
            get { return _addAllIdListByWorkType1; }
            set
            {
                _addAllIdListByWorkType1 = value;
                RaisePropertyChanged("AddAllIdListByWorkType1");
            }
        }

        private string _selectedAddAllIdByWorkType1;

        public string SelectedAddAllIdByWorkType1
        {
            get { return _selectedAddAllIdByWorkType1; }
            set
            {
                _selectedAddAllIdByWorkType1 = value;
                RaisePropertyChanged("SelectedAddAllIdByWorkType1");
                SetDelvTime();
            }
        }

        #endregion

        #region 出貨時間-新增 WorkType1

        private List<NameValuePair<string>> _addDelvTimeListByWorkType1;

        public List<NameValuePair<string>> AddDelvTimeListByWorkType1
        {
            get { return _addDelvTimeListByWorkType1; }
            set
            {
                _addDelvTimeListByWorkType1 = value;
                RaisePropertyChanged("AddDelvTimeListByWorkType1");
            }
        }

        private string _selectedAddDelvTimeByWorkType1;

        public string SelectedAddDelvTimeByWorkType1
        {
            get { return _selectedAddDelvTimeByWorkType1; }
            set
            {
                _selectedAddDelvTimeByWorkType1 = value;
                RaisePropertyChanged("SelectedAddDelvTimeByWorkType1");
            }
        }

        #endregion

        #region 配送商-新增 WorkType2

        private List<NameValuePair<string>> _addAllIdList;

        public List<NameValuePair<string>> AddAllIdList
        {
            get { return _addAllIdList; }
            set
            {
                _addAllIdList = value;
                RaisePropertyChanged("AddAllIdList");
            }
        }

        private NameValuePair<string> _selectedAddAll;
        public NameValuePair<string> SelectedAddAll
        {
            get { return _selectedAddAll; }
            set
            {
                _selectedAddAll = value;
                RaisePropertyChanged("SelectedAddAll");
            }
        }


        private string _selectedAddAllId;

        public string SelectedAddAllId
        {
            get { return _selectedAddAllId; }
            set
            {
                _selectedAddAllId = value;
                RaisePropertyChanged("SelectedAddAllId");
            }
        }

        #endregion

        #region 送貨地址

        private string _addAddress;

        public string AddAddress
        {
            get { return _addAddress; }
            set
            {
                _addAddress = value;
                RaisePropertyChanged("AddAddress");
            }
        }

        #endregion

        #region 選擇出貨物流中心-新增

        private List<NameValuePair<string>> _addNewDcList;

        public List<NameValuePair<string>> AddNewDcList
        {
            get { return _addNewDcList; }
            set
            {
                _addNewDcList = value;
                RaisePropertyChanged("AddNewDcList");
            }
        }

        private string _selectedAddNewDcCode;

        public string SelectedAddNewDcCode
        {
            get { return _selectedAddNewDcCode; }
            set
            {
                _selectedAddNewDcCode = value;
                RaisePropertyChanged("SelectedAddNewDcCode");
            }
        }

        #endregion

        #region 調整原因-新增

        private List<NameValuePair<string>> _addCauseList;

        public List<NameValuePair<string>> AddCauseList
        {
            get { return _addCauseList; }
            set
            {
                _addCauseList = value;
                RaisePropertyChanged("AddCauseList");
            }
        }

        private string _selectedAddCause;

        public string SelectedAddCause
        {
            get { return _selectedAddCause; }
            set
            {
                _selectedAddCause = value;
                RaisePropertyChanged("SelectedAddCause");
                if (SelectedAddCause != "999")
                    AddCauseMemo = "";
            }
        }

        #endregion

        #region 調整原因說明-新增

        private string _addCauseMemo;

        public string AddCauseMemo
        {
            get { return _addCauseMemo; }
            set
            {
                _addCauseMemo = value;
                RaisePropertyChanged("AddCauseMemo");
            }
        }

        #endregion

        #region 訂單搜尋結果 Grid

        private List<F050301Data> _f050301Datas;

        public List<F050301Data> F050301Datas
        {
            get { return _f050301Datas; }
            set
            {
                _f050301Datas = value;
                RaisePropertyChanged("F050301Datas");
            }
        }

        private string _oldZipCode;
        private F050301Data _selectedF050301Data;

        public F050301Data SelectedF050301Data
        {
            get { return _selectedF050301Data; }
            set
            {
                _selectedF050301Data = value;
                if (_selectedF050301Data != null)
                {
                    _selectedF050301Data.PropertyChanged -= SelectedF050301Data_PropertyChanged;
                    RaisePropertyChanged("SelectedF050301Data");
                    _oldZipCode = _selectedF050301Data.ZIP_CODE;
                    _selectedF050301Data.PropertyChanged += SelectedF050301Data_PropertyChanged;
                    SetAllIdList();
                }
            }
        }
        #endregion

        #region 配送商-編輯 WorkType1

        private string _editAllCompByWorkType1;

        public string EditAllCompByWorkType1
        {
            get { return _editAllCompByWorkType1; }
            set
            {
                _editAllCompByWorkType1 = value;
                RaisePropertyChanged("EditAllCompByWorkType1");
            }
        }

        #endregion

        #region 配送商出貨時間-編輯 WorkType1

        private string _editDelvTimeByWorkType1;

        public string EditDelvTimeByWorkType1
        {
            get { return _editDelvTimeByWorkType1; }
            set
            {
                _editDelvTimeByWorkType1 = value;
                RaisePropertyChanged("EditDelvTimeByWorkType1");
            }
        }

        #endregion

        #region 配送商-編輯

        private string _editAllComp;

        public string EditAllComp
        {
            get { return _editAllComp; }
            set
            {
                _editAllComp = value;
                RaisePropertyChanged("EditAllComp");
            }
        }

        #endregion

        #region 選擇出貨物流中心-編輯

        private List<NameValuePair<string>> _editNewDcList;

        public List<NameValuePair<string>> EditNewDcList
        {
            get { return _editNewDcList; }
            set
            {
                _editNewDcList = value;
                RaisePropertyChanged("EditNewDcList");
            }
        }

        private string _selectedEditNewDcCode;

        public string SelectedEditNewDcCode
        {
            get { return _selectedEditNewDcCode; }
            set
            {
                _selectedEditNewDcCode = value;
                RaisePropertyChanged("SelectedEditNewDcCode");
            }
        }

        #endregion

        #region 調整原因-編輯

        private List<NameValuePair<string>> _editCauseList;

        public List<NameValuePair<string>> EditCauseList
        {
            get { return _editCauseList; }
            set
            {
                _editCauseList = value;
                RaisePropertyChanged("EditCauseList");
            }
        }
        #endregion


        #region 是否全選

        private bool _isCheckAll;

        public bool IsCheckAll
        {
            get { return _isCheckAll; }
            set
            {
                _isCheckAll = value;
                CheckSelectedAll(_isCheckAll);
                RaisePropertyChanged("IsCheckAll");
            }
        }
        #endregion

        #endregion

        #region 調整單類別=商品庫存調整

        #region 調整單類別=商品庫存調整 調整單明細 Grid

        private List<F200103Data> _f200103Datas;

        public List<F200103Data> F200103Datas
        {
            get { return _f200103Datas; }
            set
            {
                _f200103Datas = value;
                RaisePropertyChanged("F200103Datas");
            }
        }

        private F200103Data _selectedF200103Data;

        public F200103Data SelectedF200103Data
        {
            get { return _selectedF200103Data; }
            set
            {
                _selectedF200103Data = value;
                RaisePropertyChanged("SelectedF200103Data");
            }
        }

        #endregion

        #region 物流中心-新增查詢

        private List<NameValuePair<string>> _addDcList1;

        public List<NameValuePair<string>> AddDcList1
        {
            get { return _addDcList1; }
            set
            {
                _addDcList1 = value;
                RaisePropertyChanged("AddDcList1");
            }
        }

        private string _tempSelectedAddDcCode1;
        private string _selectedAddDcCode1;

        public string SelectedAddDcCode1
        {
            get { return _selectedAddDcCode1; }
            set
            {
                if (F1913Datas != null && F1913Datas.Any(o => o.IsSelected) && _tempSelectedAddDcCode1 != value)
                {
                    var result = DialogService.ShowMessage(Properties.Resources.P2001010000_ViewModel_SureClearAllSet_To_ChangeDC, "", DialogButton.YesNo, DialogImage.Question);
                    if (result == DialogResponse.No)
                    {
                        ResetAddDcCode1Selected();
                        return;
                    }
                }
                _selectedAddDcCode1 = value;
                RaisePropertyChanged("SelectedAddDcCode1");
                if (_tempSelectedAddDcCode1 != value)
                {
                    F1913Datas = null;
                    SelectedF1913Data = null;
                    _serialNoResults = new Dictionary<int, List<exShare.SerialNoResult>>();
                    SetWareHouse();
                    _tempSelectedAddDcCode1 = value;
                    AddItemCode1 = "";
                    AddItemName1 = "";
                }
            }
        }

        #endregion

        private void SelectedF050301Data_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _selectedF050301Data.PropertyChanged -= SelectedF050301Data_PropertyChanged;
            switch (e.PropertyName)
            {
                case nameof(SelectedF050301Data.ZIP_CODE):
                    if (SelectedF050301Data.ZIP_CODE != null)
                    {
                        SelectedF050301Data.ZIP_CODE = CheckZipCode(SelectedF050301Data.ZIP_CODE, _oldZipCode);
                    }
                    break;
            }
            _selectedF050301Data.PropertyChanged += SelectedF050301Data_PropertyChanged;
        }

        #region 倉別-新增查詢

        private List<NameValuePair<string>> _addWareHouseList;

        public List<NameValuePair<string>> AddWareHouseList
        {
            get { return _addWareHouseList; }
            set
            {
                _addWareHouseList = value;
                RaisePropertyChanged("AddWareHouseList");
            }
        }

        private string _selectedAddWareHouseId;

        public string SelectedAddWareHouseId
        {
            get { return _selectedAddWareHouseId; }
            set
            {
                _selectedAddWareHouseId = value;
                RaisePropertyChanged("SelectedAddWareHouseId");
            }
        }

        #endregion

        #region 商品編號-新增查詢

        private string _addItemCode1;

        public string AddItemCode1
        {
            get { return _addItemCode1; }
            set
            {
                _addItemCode1 = value;
                RaisePropertyChanged("AddItemCode1");
            }
        }

        #endregion

        #region 商品品名-新增查詢

        private string _addItemName1;

        public string AddItemName1
        {
            get { return _addItemName1; }
            set
            {
                _addItemName1 = value;
                RaisePropertyChanged("AddItemName1");
            }
        }

        #endregion

        #region 是否全選

        private bool _isCheckAllByItem;

        public bool IsCheckAllByItem
        {
            get { return _isCheckAllByItem; }
            set
            {
                _isCheckAllByItem = value;
                CheckSelectedAllItem(_isCheckAllByItem);
                RaisePropertyChanged("IsCheckAllByItem");
            }
        }
        #endregion

        #region 商品搜尋結果Grid-新增查詢

        private List<F1913Data> _tempF1913Datas = new List<F1913Data>();
        private List<F1913Data> _f1913Datas;
        public List<F1913Data> F1913Datas
        {
            get { return _f1913Datas; }
            set
            {
                _f1913Datas = value;
                RaisePropertyChanged("F1913Datas");
            }
        }

        private F1913Data _selectedF1913Data;
        public F1913Data SelectedF1913Data
        {
            get { return _selectedF1913Data; }
            set
            {
                _selectedF1913Data = value;
                RaisePropertyChanged("SelectedF1913Data");
            }
        }

        #endregion

        #region 商品設定Data-新增查詢

        private Dictionary<int, List<exShare.SerialNoResult>> _serialNoResults;

        public Dictionary<int, List<exShare.SerialNoResult>> SerialNoResults
        {
            get { return _serialNoResults; }
            set
            {
                _serialNoResults = value;
                RaisePropertyChanged("SerialNoResults");
            }
        }

        #endregion

        #region 調整原因-編輯1

        private List<NameValuePair<string>> _editCauseList1;

        public List<NameValuePair<string>> EditCauseList1
        {
            get { return _editCauseList1; }
            set
            {
                _editCauseList1 = value;
                RaisePropertyChanged("EditCauseList1");
            }
        }
        #endregion

        #region 異動序號資料

        private List<F20010301> _serialNoList;

        public List<F20010301> SerialNoList
        {
            get { return _serialNoList; }
            set
            {
                _serialNoList = value;
                RaisePropertyChanged("SerialNoList");
            }
        }

        private F20010301 _selectedSerialNo;

        public F20010301 SelectedSerialNo
        {
            get { return _selectedSerialNo; }
            set
            {
                _selectedSerialNo = value;
                RaisePropertyChanged("SelectedSerialNo");
            }
        }
        #endregion

        #region 調入/調出WORKTYPE

        private string _workTypeNameByAdjustType1;
        public string WorkTypeNameByAdjustType1
        {
            get { return _workTypeNameByAdjustType1; }
            set
            {
                _workTypeNameByAdjustType1 = value;
                RaisePropertyChanged("WorkTypeNameByAdjustType1");
            }
        }

        #endregion

        #region 匯入庫存路徑

        private string _importFilePath;

        public string ImportFilePath
        {
            get { return _importFilePath; }
            set
            {
                _importFilePath = value;
                RaisePropertyChanged("ImportFilePath");
            }
        }

        #endregion

        #endregion

        #region 調整單類別=盤點庫存調整


        #endregion


        #region 下拉選單資料繫結

        private void SetDcList()
        {
            var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
            DcList = data;
            if (DcList.Any())
                SelectedDcCode = DcList.First().Value;
            AddDcList = data;
            if (AddDcList.Any())
                SelectedAddDcCode = AddDcList.First().Value;
            AddNewDcList = data;
            if (AddNewDcList.Any())
                SelectedAddNewDcCode = AddNewDcList.First().Value;
            EditNewDcList = data;
            if (EditNewDcList.Any())
                SelectedEditNewDcCode = EditNewDcList.First().Value;

            AddDcList1 = data;
            if (AddDcList1.Any())
                SelectedAddDcCode1 = AddDcList1.First().Value;

        }

        private void SetAdjustTypeList()
        {
            var list = GetBaseTableService.GetF000904List(FunctionCode, "F200101", "ADJUST_TYPE");
            AdjustTypeList = list;
            if (AdjustTypeList.Any())
                SelectedAdjustType = AdjustTypeList.First().Value;

            var addlist = (from o in list
                           where o.Value != "2" && o.Value != "3" && o.Value != "4" && o.Value !="5"
                           select new NameValuePair<string>
                           {
                               Name = o.Name,
                               Value = o.Value
                           }).ToList();

            AddAdjustTypeList = addlist;
            if (AddAdjustTypeList.Any())
                SelectedAddAdjustType = AddAdjustTypeList.First().Value;
        }

        private void SetWorkTypeList()
        {
            var data = GetBaseTableService.GetF000904List(FunctionCode, "F200101", "WORK_TYPE");
            var list = (from o in data
                        select new NameValuePair<string>
                        {
                            Name = o.Name,
                            Value = o.Value
                        }).ToList();
            list.Insert(0, new NameValuePair<string> { Name = Resources.Resources.All, Value = "" });
            WorkTypeList = list;
            if (WorkTypeList.Any())
                SelectedWorkType = WorkTypeList.First().Value;

            var addlist = (from o in data
                           select new NameValuePair<string>
                           {
                               Name = o.Name,
                               Value = o.Value
                           }).ToList();
            AddWorkTypeList = addlist;
            if (AddWorkTypeList.Any())
                SelectedAddWorkType = AddWorkTypeList.First().Value;
        }

        private void SetPickTime()
        {
            var procFlagList = new List<string> { "0", "1", "2" };
            var proxy = GetProxy<F05Entities>();
            var data =
                proxy.F0513s.Where(
                    o =>
                        o.DC_CODE == SelectedAddDcCode && o.GUP_CODE == GupCode && o.CUST_CODE == CustCode
                        ).ToList();
            data = data.Where(o => procFlagList.Contains(o.PROC_FLAG) && o.DELV_DATE == AddDelvDate).ToList();

            var list = (from o in data
                        orderby o.PICK_TIME
                        select new NameValuePair<string>
                        {
                            Name = o.PICK_TIME,
                            Value = o.PICK_TIME
                        }).ToList();
            AddPickTimeList = list;
            if (AddPickTimeList.Any())
                SelectedAddPickTime = AddPickTimeList.First().Value;
        }

        private void SetCauseList()
        {
            var proxy = GetProxy<F19Entities>();
            string uctId = SelectedAddWorkType == "5" ? "AH" : "AJ";
            var data = proxy.F1951s.Where(o => o.UCT_ID == uctId).ToList();
            var list = (from o in data
                        select new NameValuePair<string>
                        {
                            Name = o.CAUSE,
                            Value = o.UCC_CODE
                        }).ToList();
            AddCauseList = list;
            if (AddCauseList.Any())
                SelectedAddCause = AddCauseList.First().Value;
        }

        private void SetCauseListByAdjustType1()
        {
            var proxy = GetProxy<F19Entities>();
            var data = proxy.F1951s.Where(o => o.UCT_ID == "AI").ToList();
            var list = (from o in data
                        select new NameValuePair<string>
                        {
                            Name = o.CAUSE,
                            Value = o.UCC_CODE
                        }).ToList();
            EditCauseList1 = list;
        }
        private void SetAllIdList()
        {
            if (SelectedF050301Data != null)
            {
                var proxyEx = GetExProxy<P20ExDataSource>();
                var data = proxyEx.CreateQuery<F0513Data>("GetF0513Datas")
                    .AddQueryExOption("dcCode", SelectedF050301Data.DC_CODE)
                    .AddQueryExOption("gupCode", SelectedF050301Data.GUP_CODE)
                    .AddQueryExOption("custCode", SelectedF050301Data.CUST_CODE)
                    .AddQueryExOption("delvDate", SelectedF050301Data.DELV_DATE == null ? "" : ((DateTime)SelectedF050301Data.DELV_DATE).ToString("yyyy/MM/dd")).ToList();

                AddAllIdList = (from o in data
                                select new NameValuePair<string>
                                {
                                    Name = o.PICK_TIME + " " + o.ALL_COMP,
                                    Value = o.ALL_ID
                                }).ToList();
                SelectedAddAllId = AddAllIdList.Any() ? AddAllIdList.First().Value : null;


            }
            else
            {
                AddAllIdList = new List<NameValuePair<string>>();
                SelectedAddAllId = null;
            }
        }

        private void SetAllIdListByWorkType1()
        {
            if (SelectedF050301Data != null)
            {
                var proxy = GetProxy<F19Entities>();

                AddAllIdListByWorkType1 = proxy.CreateQuery<F1947>("GetAllowedF1947s")
                                    .AddQueryExOption("dcCode", _tempSelectedAddDcCode)
                                    .AddQueryExOption("gupCode", GupCode)
                                    .AddQueryExOption("custCode", CustCode)
                                    .Select(o => new NameValuePair<string>
                                    {
                                        Name = o.ALL_COMP,
                                        Value = o.ALL_ID
                                    }).ToList();

                SelectedAddAllIdByWorkType1 = AddAllIdListByWorkType1.Any() ? AddAllIdListByWorkType1.First().Value : null;
            }
            else
            {
                AddAllIdListByWorkType1 = new List<NameValuePair<string>>();
                SelectedAddAllIdByWorkType1 = null;
            }
        }

        private void SetDelvTime()
        {
            if (!string.IsNullOrEmpty(SelectedAddAllIdByWorkType1))
            {
                var proxy = GetProxy<F19Entities>();
                var delvTimeQuery = from o in proxy.F194701s
                                    where o.DC_CODE == _tempSelectedAddDcCode
                                    where o.ALL_ID == SelectedAddAllIdByWorkType1
                                    select o;

                AddDelvTimeListByWorkType1 = delvTimeQuery.ToList()
                                                          .GroupBy(o => o.DELV_TIME)
                                                          .Select(g => new NameValuePair<string>(g.Key, g.Key))
                                                          .ToList();
                SelectedAddDelvTimeByWorkType1 = AddDelvTimeListByWorkType1.Any() ? AddDelvTimeListByWorkType1.First().Value : null;

            }
            else
            {
                AddDelvTimeListByWorkType1 = new List<NameValuePair<string>>();
                SelectedAddDelvTimeByWorkType1 = null;
            }
        }

        private void SetWareHouse()
        {
            //var proxy = GetProxy<F19Entities>();
            //var data = proxy.F1980s.Where(o => o.DC_CODE == SelectedAddDcCode1).ToList();
            //AddWareHouseList = (from o in data
            //					select new NameValuePair<string>
            //					{
            //						Name = o.WAREHOUSE_NAME,
            //						Value = o.WAREHOUSE_ID
            //					}).ToList();

            var proxyP19Ex = GetExProxy<ex19.P19ExDataSource>();
            AddWareHouseList = proxyP19Ex.CreateQuery<ex19.F1912WareHouseData>("GetCustWarehouseDatas")
                                        .AddQueryExOption("dcCode", SelectedAddDcCode1)
                                        .AddQueryExOption("gupCode", _gupCode)
                                        .AddQueryExOption("custCode", _custCode)
																				.Where(x=>x.DEVICE_TYPE == "0")  // 只查詢出人工倉
                                        .Select(o => new NameValuePair<string>()
                                        {
                                            Name = o.WAREHOUSE_NAME,
                                            Value = o.WAREHOUSE_ID
                                        }).ToList();


            SelectedAddWareHouseId = AddWareHouseList.Any() ? AddWareHouseList.First().Value : null;
        }

        #endregion

        #region Search
        public ICommand SearchCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoSearch(), () => UserOperateMode == OperateMode.Query,
                    c => DoSearchComplete()
                    );
            }
        }

        private void DoSearch()
        {
            var proxyEx = GetExProxy<P20ExDataSource>();
            switch (SelectedAdjustType)
            {
                case "0": //訂單調整
                    F200101Datas = proxyEx.CreateQuery<F200101Data>("GetF200101Datas")
                    .AddQueryExOption("dcCode", SelectedDcCode)
                    .AddQueryExOption("gupCode", GupCode)
                    .AddQueryExOption("custCode", CustCode)
                    .AddQueryExOption("adjustNo", AdjustNo ?? "")
                    .AddQueryExOption("adjustType", SelectedAdjustType)
                    .AddQueryExOption("workType", SelectedWorkType)
                    .AddQueryExOption("begAdjustDate", BegAdjustDate.HasValue ? BegAdjustDate.Value.ToString("yyyy/MM/dd") : "")
                    .AddQueryExOption("endAdjustDate", EndAdjustDate.HasValue ? EndAdjustDate.Value.ToString("yyyy/MM/dd") : "").ToList();
                    break;
                case "1": //商品庫存盤點
                case "2": //盤點庫存調整
                case "3": //單據扣帳處理
								case "4": //快速移轉庫存調整
								case "5": //調撥缺庫存調整
                    F200101Datas = proxyEx.CreateQuery<F200101Data>("GetF200101DatasByAdjustType1Or2")
                    .AddQueryExOption("dcCode", SelectedDcCode)
                    .AddQueryExOption("gupCode", GupCode)
                    .AddQueryExOption("custCode", CustCode)
                    .AddQueryExOption("adjustNo", AdjustNo ?? "")
                    .AddQueryExOption("adjustType", SelectedAdjustType)
                    .AddQueryExOption("workType", SelectedWorkType)
                    .AddQueryExOption("begAdjustDate", BegAdjustDate.HasValue ? BegAdjustDate.Value.ToString("yyyy/MM/dd") : "")
                    .AddQueryExOption("endAdjustDate", EndAdjustDate.HasValue ? EndAdjustDate.Value.ToString("yyyy/MM/dd") : "").ToList();
                    break;
            }
        }

        private void DoSearchComplete()
        {
            if (F200101Datas != null && F200101Datas.Any())
                SelectedF200101Data = F200101Datas.First();
            else
            {
                F200102Datas = null;
                SelectedF200102Data = null;
                ShowMessage(Messages.InfoNoData);
            }

        }

        private void BindDetail()
        {
            if (SelectedF200101Data != null)
            {
                var proxyEx = GetExProxy<P20ExDataSource>();
                switch (SelectedAdjustType)
                {
                    case "0": //訂單調整
                        F200102Datas = proxyEx.CreateQuery<F200102Data>("GetF200102Datas")
                            .AddQueryExOption("dcCode", SelectedF200101Data.DC_CODE)
                            .AddQueryExOption("gupCode", SelectedF200101Data.GUP_CODE)
                            .AddQueryExOption("custCode", SelectedF200101Data.CUST_CODE)
                            .AddQueryExOption("adjustNo", SelectedF200101Data.ADJUST_NO)
                            .AddQueryExOption("workType", SelectedF200101Data.WORK_TYPE).ToList();

                        if (F200102Datas != null && F200102Datas.Any())
                            SelectedF200102Data = F200102Datas.First();
                        break;
                    case "1": //商品庫存盤點
                    case "2": //盤點庫存調整
                    case "3": //單據扣帳處理
										case "4": //快速移轉庫存調整
                    case "5": //調撥缺庫存調整
                            F200103Datas = proxyEx.CreateQuery<F200103Data>("GetF200103Datas")
                            .AddQueryExOption("dcCode", SelectedF200101Data.DC_CODE)
                            .AddQueryExOption("gupCode", SelectedF200101Data.GUP_CODE)
                            .AddQueryExOption("custCode", SelectedF200101Data.CUST_CODE)
                            .AddQueryExOption("adjustNo", SelectedF200101Data.ADJUST_NO).ToList();

                        if (F200103Datas != null && F200103Datas.Any())
                            SelectedF200103Data = F200103Datas.First();
                        break;
                }
            }
            else
            {
                F200102Datas = null;
                SelectedF200102Data = null;
                F200103Datas = null;
                SelectedF200103Data = null;
            }
        }
        #endregion Search

        #region Add
        public ICommand AddCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoAdd(), () => UserOperateMode == OperateMode.Query && SelectedAdjustType != "2" && SelectedAdjustType != "3" && SelectedAdjustType!="4" && SelectedAdjustType != "5"
                    );
            }
        }

        private void DoAdd()
        {

            if (AddAdjustTypeList.Any())
                SelectedAddAdjustType = AddAdjustTypeList.First().Value;
            if (AddWorkTypeList.Any())
                SelectedAddWorkType = AddWorkTypeList.First().Value;
            if (AddDcList.Any())
                SelectedAddDcCode = AddDcList.First().Value;
            AddDelvDate = DateTime.Now.Date;
            AddCustOrdNo = "";
            AddItemCode = "";
            AddItemName = "";
            AddConsignee = "";
            F050301Datas = null;
            SelectedF050301Data = null;
            AddAllIdList = null;
            SelectedAddAllId = null;
            AddAddress = "";
            if (AddNewDcList.Any())
                SelectedAddNewDcCode = AddNewDcList.First().Value;
            if (AddCauseList.Any())
                SelectedAddCause = AddCauseList.First().Value;
            AddCauseMemo = "";
            SelectedAddAdjustType = SelectedAdjustType;
            UserOperateMode = OperateMode.Add;

            SerialNoResults = new Dictionary<int, List<exShare.SerialNoResult>>();
            F1913Datas = null;
            AddItemCode1 = "";
            AddItemName1 = "";
            SelectedF1913Data = null;
            _tempF1913Datas = null;
            //執行新增動作
        }
        #endregion Add

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
                    o => DoDelete(), () => UserOperateMode == OperateMode.Query && SelectedF200101Data != null && SelectedF200101Data.ADJUST_TYPE == "0" && SelectedF200101Data.WORK_TYPE != "0" && SelectedF200101Data.WORK_TYPE != "1",
                    c => DoDeleteComplete()
                    );
            }
        }

        private void DoDelete()
        {
            //執行刪除動作
            _isDeleteOk = false;
            if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
            {
                var message = new MessagesStruct
                {
                    Button = DialogButton.OK,
                    Image = DialogImage.Warning,
                    Message = "",
                    Title = Resources.Resources.Information
                };

                //執行編輯動作
                var proxyEx = GetExProxy<P20ExDataSource>();
                var result = proxyEx.CreateQuery<ExecuteResult>("DeleteP200101ByAdjustType0")
                    .AddQueryExOption("dcCode", SelectedF200102Data.DC_CODE)
                    .AddQueryExOption("gupCode", SelectedF200102Data.GUP_CODE)
                    .AddQueryExOption("custCode", SelectedF200102Data.CUST_CODE)
                    .AddQueryExOption("adjustNo", SelectedF200102Data.ADJUST_NO)
                    .ToList();
                if (result.First().IsSuccessed)
                {
                    ShowMessage(Messages.Success);
                    _isDeleteOk = true;
                }
                else
                {
                    message.Message = result.First().Message;
                    ShowMessage(message);
                }
            }
        }
        private void DoDeleteComplete()
        {
            if (_isDeleteOk)
            {
                SearchCommand.Execute(null);
            }
        }
        #endregion Delete

        #region Save
        public ICommand SaveCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoSave(), () => UserOperateMode != OperateMode.Query &&
                        ((UserOperateMode == OperateMode.Add && ((SelectedAddAdjustType == "0" && SelectedF050301Data != null) || (SelectedAddAdjustType == "1" && F1913Datas != null && F1913Datas.Any(o => o.IsSelected)))) || UserOperateMode == OperateMode.Edit),
                        c => DoSaveComplete()
                    );
            }
        }

        private bool _isSaveSuccess;
        private void DoSave()
        {
            var adjustType = (UserOperateMode == OperateMode.Add) ? SelectedAddAdjustType : SelectedF200101Data.ADJUST_TYPE;
            switch (adjustType)
            {
                case "0":
                    SaveAdjustType0();
                    break;
                case "1":
                    SaveAdjustType1();
                    break;
            }

        }

        private void SaveAdjustType0()
        {
            _isSaveSuccess = false;
            var message = new MessagesStruct
            {
                Button = DialogButton.OK,
                Image = DialogImage.Warning,
                Message = "",
                Title = Resources.Resources.Information
            };

            //執行確認儲存動作
            if (UserOperateMode == OperateMode.Add)
            {
                if (SelectedAddCause != "999")
                    AddCauseMemo = string.Empty;

                message.Message = BeforeAddSaveCheck();
                if (message.Message.Length == 0)
                {
                    var allId = "";
                    if (SelectedAddWorkType == "1")
                        allId = SelectedAddAllIdByWorkType1 ?? ""; //配送商
                    if (SelectedAddWorkType == "2")
                        allId = SelectedAddAllId ?? ""; //配送商

                    var newDcCode = (SelectedAddWorkType == "3") ? SelectedAddNewDcCode : "";
                    string allTime = "";
                    if (SelectedAddWorkType == "1")
                        allTime = SelectedAddDelvTimeByWorkType1 ?? "";  //出車時段					

                    var selectedOrdDatas = new List<F050301Data>();
                    if (SelectedAddWorkType == "2")
                    {
                        selectedOrdDatas.Add(SelectedF050301Data);
                        allTime = SelectedF050301Data.PICK_TIME;
                    }
                    else
                        selectedOrdDatas.AddRange(F050301Datas.Where(o => o.IsSelected));

                    // 自取
                    if (SelectedAddWorkType == "4")
                    {
                        var proxyF05 = GetProxy<F05Entities>();

                        var query = selectedOrdDatas.Where(x =>
                        {
                            var f05030101s = proxyF05.CreateQuery<F05030101>("GetMergerOrders")
                                                    .AddQueryExOption("dcCode", x.DC_CODE)
                                                    .AddQueryExOption("gupCode", x.GUP_CODE)
                                                    .AddQueryExOption("custCode", x.CUST_CODE)
                                                    .AddQueryExOption("ordNo", x.ORD_NO)
                                                    .ToList();

                            return f05030101s.Count > 1;
                        });

                        if (query.Any())
                        {
                            var dialogResponse = ShowConfirmMessage(Properties.Resources.P2001010000_ViewModel_SureEditRelateORD_To_SelfTake);
                            if (dialogResponse == DialogResponse.No)
                                return;
                        }
                    }

                    var wcfSelectedOrdDatas = ExDataMapper.MapCollection<F050301Data, wcf.F050301Data>(selectedOrdDatas).ToArray();

                    var proxy = new wcf.P20WcfServiceClient();
                    var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
                        () => proxy.InsertP200101ByAdjustType0(wcfSelectedOrdDatas, SelectedAddWorkType, allId, allTime, AddAddress.Trim(), newDcCode, SelectedAddCause, AddCauseMemo));
                    if (result.IsSuccessed)
                    {
                        message.Message = string.Format(Properties.Resources.P2001010000_ViewModel_InsertSuccess, result.Message);
                        ShowMessage(message);
                        _isSaveSuccess = true;
                        AdjustNo = result.Message;
                    }
                    else
                    {
                        message.Message = result.Message;
                        ShowMessage(message);
                    }

                }
                else
                    ShowMessage(message);
            }
            else
            {
                if (SelectedF200102Data.CAUSE != "999")
                    SelectedF200102Data.CAUSE_MEMO = string.Empty;
                message.Message = BeforeEditCheck();
                if (message.Message.Length == 0)
                {
                    var proxyEx = GetExProxy<P20ExDataSource>();
                    var data = proxyEx.CreateQuery<ExecuteResult>("UpdateP200101ByAdjustTye0")
                        .AddQueryExOption("dcCode", SelectedF200102Data.DC_CODE)
                        .AddQueryExOption("gupCode", SelectedF200102Data.GUP_CODE)
                        .AddQueryExOption("custCode", SelectedF200102Data.CUST_CODE)
                        .AddQueryExOption("adjustNo", SelectedF200102Data.ADJUST_NO)
                        .AddQueryExOption("adjustSeq", SelectedF200102Data.ADJUST_SEQ)
                        .AddQueryExOption("workType", SelectedF200101Data.WORK_TYPE)
                        .AddQueryExOption("address", (SelectedF200101Data.WORK_TYPE == "2") ? SelectedF200102Data.ADDRESS.Trim() : "")
                        .AddQueryExOption("newDcCode", (SelectedF200101Data.WORK_TYPE == "3") ? SelectedF200102Data.NEW_DC_CODE : "")
                        .AddQueryExOption("cause", CAUSE)
                        .AddQueryExOption("causeMemo", SelectedF200102Data.CAUSE_MEMO).ToList();

                    if (data.First().IsSuccessed)
                    {
                        _isSaveSuccess = true;
                        ShowMessage(Messages.Success);
                    }
                    else
                    {
                        message.Message = data.First().Message;
                        ShowMessage(message);
                    }
                }
                else
                    ShowMessage(message);
            }
        }

        private F1913Data _checkErrorSelectedF1913Data;
        private void SaveAdjustType1()
        {
            _isSaveSuccess = false;
            var message = new MessagesStruct
            {
                Button = DialogButton.OK,
                Image = DialogImage.Warning,
                Message = "",
                Title = Resources.Resources.Information
            };
            if (UserOperateMode == OperateMode.Add)
            {
				var data = F1913Datas.Where(o => o.IsSelected).ToList();
                F1913Datas = data;
                // 取得只有通過的序號
                var dataDetail = SerialNoResults.Where(o => data.Select(a => a.ROWNUM).Contains(o.Key))
                                                .Select(o => new KeyValuePair<int, List<exShare.SerialNoResult>>(o.Key, o.Value.Where(x => x.Checked).ToList()))
                                                .ToList();
                var serialNoOkList = new List<string>();
				// 取得自動倉的倉別編號
				var autoWarehourse = GetProxy<F19Entities>().CreateQuery<F1980>("GetAutoWarehourse")
																								.AddQueryExOption("dcCode", SelectedAddDcCode1)
																								.ToList();
				foreach (var f1913Data in data)
                {
                    if (!f1913Data.ADJ_QTY_IN.HasValue && !f1913Data.ADJ_QTY_OUT.HasValue)
                    {
                        message.Message = Properties.Resources.P2001010000_ViewModel_NotSetAnyChange;
                    }
                    else if (f1913Data.BUNDLE_SERIALNO == "1")
                    {
                        var detail = dataDetail.FirstOrDefault(o => o.Key == f1913Data.ROWNUM);
                        if (detail.Key == 0)
                            message.Message = Properties.Resources.P2001010000_ViewModel_SERIALNO_SCANOK_Required;
                        else
                        {
                            foreach (var detailItem in detail.Value)
                            {
                                if (serialNoOkList.Contains(detailItem.SerialNo))
                                {
                                    message.Message = Properties.Resources.P2001010000_ViewModel_SerialNo + detailItem.SerialNo + Properties.Resources.P2001010000_ViewModel_Exist;
                                    break;
                                }
                                detailItem.Message = CheckSerialNoIsPass(f1913Data, detailItem.SerialNo);
                                detailItem.Checked = string.IsNullOrEmpty(detailItem.Message);
                                serialNoOkList.Add(detailItem.SerialNo);
                            }

                            SerialNoResults.Remove(detail.Key);
                            SerialNoResults.Add(detail.Key, detail.Value);
                            var qty = f1913Data.WORK_TYPE == "0" ? f1913Data.ADJ_QTY_IN : f1913Data.ADJ_QTY_OUT;
                            f1913Data.SERIALNO_SCANOK = detail.Value.Count(o => o.Checked) == qty ? "1" : "0";
                            if (f1913Data.SERIALNO_SCANOK == "0")
                            {
                                message.Message = Properties.Resources.P2001010000_ViewModel_SERIALNO_SCANOK_Required;
                            }
                        }
                    }
										else if (autoWarehourse.Any(x=>x.WAREHOUSE_ID == f1913Data.WAREHOUSE_ID))
					{

						message.Message = string.Format(Properties.Resources.P2001010000_IsAutoWarehourseCanNotChange, f1913Data.WAREHOUSE_NAME);
					}
                    if (message.Message.Length > 0)
                    {
                        ShowMessage(message);
                        F1913Datas = data;
                        _checkErrorSelectedF1913Data = f1913Data;
                        _isSaveSuccess = false;
                        break;
                    }
                }
                if (message.Message.Length == 0)
                {
                    //儲存資料
                    //傳data ,detailData過去寫入DB

                    var proxy = new wcf.P20WcfServiceClient();
                    var wcfData = ExDataMapper.MapCollection<F1913Data, wcf.F1913Data>(data).ToArray();
                    var wcfDataDetail = new List<KeyValuePair<int, wcf.SerialNoResult[]>>();
                    foreach (var keyValuePair in dataDetail)
                    {
                        var item = ExDataMapper.MapCollection<exShare.SerialNoResult, wcf.SerialNoResult>(keyValuePair.Value);
                        var itemKeyValue = new KeyValuePair<int, wcf.SerialNoResult[]>(keyValuePair.Key, item.ToArray());
                        wcfDataDetail.Add(itemKeyValue);
                    }
                    var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
                        () => proxy.InsertP200101ByAdjustType1(wcfData.ToArray(), wcfDataDetail.ToArray()));
                    if (result.IsSuccessed)
                    {
                        message.Message = string.Format(Properties.Resources.P2001010000_ViewModel_InsertSuccess, result.Message);
                        ShowMessage(message);
                        _isSaveSuccess = true;
                        AdjustNo = result.Message;
                    }
                    else
                    {
                        message.Message = result.Message;
                        ShowMessage(message);
                    }

                    _isSaveSuccess = true;
                }
            }
            else
            {
                if (SelectedF200103Data.CAUSE != "999")
                    SelectedF200103Data.CAUSE_MEMO = "";
                var proxyEx = GetExProxy<P20ExDataSource>();
                var data = proxyEx.CreateQuery<ExecuteResult>("UpdateP200101ByAdjustType1")
                        .AddQueryExOption("dcCode", SelectedF200103Data.DC_CODE)
                        .AddQueryExOption("gupCode", SelectedF200103Data.GUP_CODE)
                        .AddQueryExOption("custCode", SelectedF200103Data.CUST_CODE)
                        .AddQueryExOption("adjustNo", SelectedF200103Data.ADJUST_NO)
                        .AddQueryExOption("adjustSeq", SelectedF200103Data.ADJUST_SEQ)
                        .AddQueryExOption("cause", SelectedF200103Data.CAUSE)
                        .AddQueryExOption("causeMemo", SelectedF200103Data.CAUSE_MEMO).ToList();

                if (data.First().IsSuccessed)
                {
                    _isSaveSuccess = true;
                    ShowMessage(Messages.Success);
                }
                else
                {
                    message.Message = data.First().Message;
                    ShowMessage(message);
                }

            }
        }

        private void DoSaveComplete()
        {
            if (_isSaveSuccess)
            {
                if (UserOperateMode == OperateMode.Add)
                {
                    SelectedDcCode = (SelectedAddAdjustType == "0") ? _tempSelectedAddDcCode : _tempSelectedAddDcCode1;
                    SelectedAdjustType = SelectedAddAdjustType;
                    SelectedWorkType = SelectedAddAdjustType == "0" ? SelectedAddWorkType : "";
                    BegAdjustDate = DateTime.Now;
                    EndAdjustDate = DateTime.Now;
                }
                else
                {
                    var item = SelectedF200101Data;
                    SelectedDcCode = item.DC_CODE;
                    AdjustNo = item.ADJUST_NO;
                    SelectedAdjustType = item.ADJUST_TYPE;
                    SelectedWorkType = item.ADJUST_TYPE == "0" ? item.WORK_TYPE : "";
                    BegAdjustDate = item.ADJUST_DATE;
                    EndAdjustDate = item.ADJUST_DATE;
                }
                UserOperateMode = OperateMode.Query;
                SearchCommand.Execute(null);
            }
            else
            {
                if (UserOperateMode == OperateMode.Add && SelectedAddAdjustType == "1")
                    SelectedF1913Data = _checkErrorSelectedF1913Data;
            }
        }
        private string BeforeAddSaveCheck()
        {
            if ((SelectedAddCause == "999" && string.IsNullOrEmpty(AddCauseMemo)) || string.IsNullOrWhiteSpace(SelectedAddCause))
                return Properties.Resources.P2001010000_ViewModel_AddCause_Required;

            if (F050301Datas != null && F050301Datas.Any())
            {
                var checkDatas = new List<F050301Data>();
                if (SelectedAddWorkType == "2")
                    checkDatas.Add(SelectedF050301Data);
                else
                    checkDatas.AddRange(F050301Datas.Where(o => o.IsSelected));

                if (!checkDatas.Any())
                    return Properties.Resources.P2001010000_ViewModel_checkDatas_Required;

            }
            return string.Empty;

        }

        private string BeforeEditCheck()
        {
            if (SelectedAddCause == "999" && string.IsNullOrEmpty(AddCauseMemo))
                return Properties.Resources.P2001010000_ViewModel_AddCause_Required;
            return string.Empty;
        }


        #endregion Save

        #region EditDetail
        public ICommand EditDetailCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoEditDetail(), () => UserOperateMode == OperateMode.Query && (SelectedF200102Data != null || SelectedF200103Data != null), o =>
                    {
                        if (_selectedF200102Data != null)
                            CAUSE = SelectedF200102Data.CAUSE;
                    }
                    );
            }
        }

        private string oldWorkType;

        private void DoEditDetail()
        {
            UserOperateMode = OperateMode.Edit;

            if (_selectedF200102Data != null)
            {
                EditAllCompByWorkType1 = SelectedF200102Data.ALL_COMP ?? "";
                EditDelvTimeByWorkType1 = SelectedF200102Data.PICK_TIME ?? "";
                EditAllComp = (SelectedF200102Data.PICK_TIME ?? "") + " " + (SelectedF200102Data.ALL_COMP ?? "");
            }
            else if (SelectedF200103Data != null)
            {
                var proxy = GetProxy<F20Entities>();
                SerialNoList =
                    proxy.F20010301s.Where(
                        o =>
                            o.ADJUST_NO == SelectedF200103Data.ADJUST_NO && o.ADJUST_SEQ == SelectedF200103Data.ADJUST_SEQ && o.DC_CODE == SelectedF200103Data.DC_CODE &&
                            o.GUP_CODE == SelectedF200103Data.GUP_CODE && o.CUST_CODE == SelectedF200103Data.CUST_CODE).ToList();
                if (SerialNoList != null && SerialNoList.Any())
                    SelectedSerialNo = SerialNoList.First();
                WorkTypeNameByAdjustType1 = (SelectedF200103Data.WORK_TYPE == "0" ? Properties.Resources.P2001010000_ViewModel_PICK_IN : Properties.Resources.P2001010000_ViewModel_PICK_OUT) + Properties.Resources.P2001010000_ViewModel_SerailNo_Data;
            }

            //執行編輯動作
        }
        #endregion Edit

        #region DeleteDetail

        public ICommand DeleteDetailCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoDeleteDetail(), () => UserOperateMode == OperateMode.Query && SelectedF200101Data != null && SelectedF200101Data.WORK_TYPE != "0" && SelectedF200101Data.WORK_TYPE != "1" && SelectedF200102Data != null,
                    c => DoDeleteDetailComplete()
                    );
            }
        }

        private bool _isDeleteOk;
        private void DoDeleteDetail()
        {
            _isDeleteOk = false;
            if (ShowMessage(Messages.WarningBeforeDelete) == DialogResponse.Yes)
            {
                var message = new MessagesStruct
                {
                    Button = DialogButton.OK,
                    Image = DialogImage.Warning,
                    Message = "",
                    Title = Resources.Resources.Information
                };

                //執行編輯動作
                var proxyEx = GetExProxy<P20ExDataSource>();
                var result = proxyEx.CreateQuery<ExecuteResult>("DeleteP200101DetailByAdjustType0")
                    .AddQueryExOption("dcCode", SelectedF200102Data.DC_CODE)
                    .AddQueryExOption("gupCode", SelectedF200102Data.GUP_CODE)
                    .AddQueryExOption("custCode", SelectedF200102Data.CUST_CODE)
                    .AddQueryExOption("adjustNo", SelectedF200102Data.ADJUST_NO)
                    .AddQueryExOption("adjustSeq", SelectedF200102Data.ADJUST_SEQ).ToList();
                if (result.First().IsSuccessed)
                {
                    ShowMessage(Messages.Success);
                    _isDeleteOk = true;
                }
                else
                {
                    message.Message = result.First().Message;
                    ShowMessage(message);
                }
            }
        }

        private void DoDeleteDetailComplete()
        {
            if (_isDeleteOk)
            {
                BindDetail();
                if (!F200102Datas.Any())
                    SearchCommand.Execute(null);
            }
        }

        #endregion

        #region OrdSearch

        public ICommand OrdSearchCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoOrdSearch(), () => UserOperateMode == OperateMode.Add,
                    c => DoOrdSearchComplete()
                    );
            }
        }

        private void DoOrdSearch()
        {
            _tempSelectedAddDcCode = SelectedAddDcCode;
            var proxyEx = GetExProxy<P20ExDataSource>();
            F050301Datas = proxyEx.CreateQuery<F050301Data>("GetF050301Datas")
                .AddQueryExOption("dcCode", SelectedAddDcCode)
                .AddQueryExOption("gupCode", GupCode)
                .AddQueryExOption("custCode", CustCode)
                .AddQueryExOption("delvDate", AddDelvDate.ToString("yyyy/MM/dd"))
                .AddQueryExOption("pickTime", SelectedAddPickTime ?? "")
                .AddQueryExOption("custOrdNo", AddCustOrdNo ?? "")
                .AddQueryExOption("itemCode", AddItemCode ?? "")
                .AddQueryExOption("consignee", AddConsignee ?? "")
                .AddQueryExOption("ordNo", AddOrdNo)
                .AddQueryExOption("workType", SelectedAddWorkType).ToList();
        }

        private void DoOrdSearchComplete()
        {
            if (F050301Datas.Any())
                SelectedF050301Data = F050301Datas.First();
            else
            {
                AddPickTimeList = null;
                SelectedAddPickTime = "";
                // 查無資料清除條件
                if (AddDcList.Any())
                    SelectedAddDcCode = AddDcList.First().Value;
                AddDelvDate = DateTime.Now.Date;
                AddCustOrdNo = "";
                AddOrdNo = "";
                AddItemCode = "";
                AddItemName = "";
                AddConsignee = "";

                ShowMessage(Messages.InfoNoData);
            }
                
            IsCheckAll = false;
            SetAllIdListByWorkType1();
        }


        #endregion

        #region ItemSearch

        public ICommand ItemSearchCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => DoItemSearch(), () => UserOperateMode == OperateMode.Add,
                    c => DoItemSearchComplete()
                    );
            }
        }

        private bool _isItemSearchOk;
        private void DoItemSearch()
        {
            _isItemSearchOk = true;
            if (string.IsNullOrEmpty((AddItemCode1 ?? "").Trim()) && string.IsNullOrEmpty((AddItemName1 ?? "").Trim()))
                _isItemSearchOk = false;
            //移除品號至少6碼限制
            //else if (!string.IsNullOrEmpty(AddItemCode1) && AddItemCode1.Trim().Length < 6)
            //	_isItemSearchOk = false;
            else
            {
                if (_tempF1913Datas == null)
                    _tempF1913Datas = new List<F1913Data>();
                if (F1913Datas != null && F1913Datas.Any(o => o.IsSelected))
                    _tempF1913Datas.AddRange(F1913Datas.Where(o => o.IsSelected));

                var proxyEx = GetExProxy<P20ExDataSource>();
                F1913Datas = proxyEx.CreateQuery<F1913Data>("GetF1913Datas")
                    .AddQueryExOption("dcCode", SelectedAddDcCode1)
                    .AddQueryExOption("gupCode", GupCode)
                    .AddQueryExOption("custCode", CustCode)
                    .AddQueryExOption("warehouseId", SelectedAddWareHouseId ?? "")
                    .AddQueryExOption("itemCode", AddItemCode1 ?? "")
                    .AddQueryExOption("itemName", AddItemName1 ?? "").ToList();
                _tempSelectedAddDcCode1 = SelectedAddDcCode1;
            }

        }

        private void DoItemSearchComplete()
        {
            if (_isItemSearchOk)
            {
                if (_tempF1913Datas != null && _tempF1913Datas.Any())
                {
                    if (F1913Datas == null)
                        F1913Datas = new List<F1913Data>();
                    foreach (var f1913Data in _tempF1913Datas)
                    {
                        var item =
                            F1913Datas.FirstOrDefault(
                                o =>
                                    o.DC_CODE == f1913Data.DC_CODE && o.GUP_CODE == f1913Data.GUP_CODE && o.CUST_CODE == f1913Data.CUST_CODE &&
                                    o.ITEM_CODE == f1913Data.ITEM_CODE && o.LOC_CODE == f1913Data.LOC_CODE &&
                                    o.WAREHOUSE_ID == f1913Data.WAREHOUSE_ID && o.VALID_DATE == f1913Data.VALID_DATE && o.ENTER_DATE == f1913Data.ENTER_DATE && o.VNR_CODE == f1913Data.VNR_CODE);
                        if (item != null)
                            F1913Datas.Remove(item);
                    }
                    foreach (var f1913Data in _tempF1913Datas)
                    {
                        var item = SerialNoResults.FirstOrDefault(o => o.Key == f1913Data.ROWNUM);
                        if (F1913Datas != null && F1913Datas.Any())
                            f1913Data.ROWNUM = F1913Datas.Max(o => o.ROWNUM) + 1;
                        else
                            f1913Data.ROWNUM = 1;
                        if (item.Key != 0)
                        {
                            var data = item.Value;
                            SerialNoResults.Remove(item.Key);
                            SerialNoResults.Add(f1913Data.ROWNUM, data);
                        }

                        F1913Datas.Add(f1913Data);
                    }
                    _tempF1913Datas = new List<F1913Data>();
                }
                F1913Datas = F1913Datas.OrderByDescending(o => o.IsSelected).ToList();

                if (F1913Datas.Any(o => !o.IsSelected))
                    SelectedF1913Data = F1913Datas.First();
                else
                {
                    // 查無資料清空條件
                    if (AddDcList1.Any())
                        SelectedAddDcCode1 = AddDcList1.First().Value;
                    if (AddWareHouseList.Any())
                        SelectedAddWareHouseId = AddWareHouseList.First().Value;
                    AddItemCode1 = "";
                    AddItemName1 = "";

                    ShowMessage(Messages.InfoNoData);
                } 
            }
            else
            {
                var message = new MessagesStruct
                {
                    Button = DialogButton.OK,
                    Image = DialogImage.Warning,
                    Title = Resources.Resources.Information
                };
                if (string.IsNullOrEmpty((AddItemCode1 ?? "").Trim()) && string.IsNullOrEmpty((AddItemName1 ?? "").Trim()))
                    message.Message = Properties.Resources.P2001010000_ViewModel_AddItemCode_AddItemName_Required;
                //else if (!string.IsNullOrEmpty(AddItemCode1) && AddItemCode1.Length < 6)
                //	message.Message = Properties.Resources.P2001010000_ViewModel_AddItemCode_Required;
                ShowMessage(message);
            }
        }


        #endregion

        #region ItemAdd

        public ICommand ItemAddCommand
        {
            get
            {
                return new RelayCommand(
                     DoItemAdd, () => UserOperateMode == OperateMode.Add
                    );
            }
        }

        private void DoItemAdd()
        {
            ItemAddClick();
        }

        #endregion

        #region ItemEdit

        public ICommand ItemEditCommand
        {
            get
            {
                return new RelayCommand(
                     DoItemEdit, () => UserOperateMode == OperateMode.Add && SelectedF1913Data != null
                    );
            }
        }

        private void DoItemEdit()
        {
            ItemEditClick();
        }




        #endregion

        #region Import

        public ICommand ExcelImportCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    DispatcherAction(() =>
                    {
                        ExcelImport();
                        if (string.IsNullOrEmpty(ImportFilePath)) return;
                        DoImportCommand.Execute(null);
                    });
                });
            }
        }
        public ICommand DoImportCommand
        {
            get
            {
                return CreateBusyAsyncCommand(
                    o => { Import(); },
                    () => UserOperateMode == OperateMode.Add
                    );
            }
        }

        public void Import()
        {
            string fullFilePath = ImportFilePath;
            var errorMeg = string.Empty;

            var excelTable = DataTableHelper.ReadExcelDataTable(fullFilePath, ref errorMeg);

            // 請檢查匯入檔案是否正確或開啟中!
            if (excelTable == null)
            {
                if (!string.IsNullOrWhiteSpace(errorMeg))
                {
                    DialogService.ShowMessage(errorMeg);
                    return;
                }
                DialogService.ShowMessage(Properties.Resources.P2001010000_CSVisnotCorrectOrOpening);
                return;
            }

            // 檢查欄位數是否小於10欄
            if (excelTable.Columns.Count < 10)
            {
                DialogService.ShowMessage(Properties.Resources.P2001010000_ColumnCountNotSufficient);
                return;
            }

            // 檢查匯入筆數一次以1000為限
            if (excelTable.Rows.Count > 1000)
            {
                DialogService.ShowMessage(Properties.Resources.P2001010000_WarningOver);
                return;
            }

            if (F1913Datas == null)
                F1913Datas = new List<F1913Data>();

            //清空資料
            if (F1913Datas.Any())
            {
                F1913Datas.Clear();
                SerialNoResults.Clear();
            }

            var importDatas = (from col in excelTable.Rows.Cast<DataRow>().Select((r, i) => new { Row = r, Index = i })
                               select new wcf.F1913DataImport
                               {
                                   ROWNUMk__BackingField = col.Index,
                                   DC_CODEk__BackingField = col.Row[0]?.ToString().Trim(),
                                   LOC_CODEk__BackingField = LocCodeHelper.LocCodeConverter9(col.Row[1]?.ToString().Trim().ToUpper()),
                                   ITEM_CODEk__BackingField = col.Row[2]?.ToString().Trim().ToUpper(),
                                   VALID_DATEk__BackingField = col.Row[3]?.ToString().Trim(),
                                   ENTER_DATEk__BackingField = col.Row[4]?.ToString().Trim(),
                                   MAKE_NOk__BackingField = col.Row[5]?.ToString().Trim(),
                                   ADJ_QTY_INk__BackingField = col.Row[6]?.ToString().Trim(),
                                   ADJ_QTY_OUTk__BackingField = col.Row[7]?.ToString().Trim(),
                                   CAUSEk__BackingField = col.Row[8]?.ToString().Trim(),
                                   SERIAL_NOk__BackingField = col.Row[9]?.ToString().Trim().ToUpper()
                               }).ToArray();

            
            var proxyWcf = new wcf.P20WcfServiceClient();
            var result = RunWcfMethod<wcf.ImportF1913DataResult>(proxyWcf.InnerChannel,
                () => proxyWcf.ImportF1913DataItems(SelectedAddDcCode1, _gupCode, _custCode, importDatas));

            if (result != null)
            {
                if (result.F1913DataItems == null)
                    result.F1913DataItems = new List<wcf.F1913Data>().ToArray();

                if (!result.Result.IsSuccessed)
                    ShowWarningMessage(result.Result.Message);
                else
                {
                    
                    var data = ExDataMapper.MapCollection<wcf.F1913Data, F1913Data>(result.F1913DataItems);
                    var totalCnt = importDatas.Length;
                    var successCnt = data.Count();
                    var failureCnt = totalCnt - successCnt;
                   
                    F1913Datas = data.ToList();

                    for (int i = 0; i < F1913Datas.Count(); i++)
                    {
                        F1913Datas[i].ROWNUM = i + 1;
                        if(F1913Datas[i].BUNDLE_SERIALNO == "1" || F1913Datas[i].BUNDLE_SERIALLOC == "1")
                        {
                            var serialNoList = F1913Datas[i].SERIAL_NO.Split('|').Select(x => new exShare.SerialNoResult { SerialNo = x, ItemCode = F1913Datas[i].ITEM_CODE, Checked = true }).ToList();

                            SerialNoResults.Add(F1913Datas[i].ROWNUM, serialNoList);
                            F1913Datas[i].SERIALNO_SCANOK = "1";
                        }
                    }
                    

                    ShowInfoMessage(string.Format(Properties.Resources.P2001010000_ImportSuccessDetail, successCnt, failureCnt, totalCnt));

                    //匯出失敗的Excel
                    if (result.F1913DataFailItems.Any())
                    {
                        var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                        saveFileDialog.DefaultExt = ".xlsx";
                        saveFileDialog.Filter = "Excel (.xlsx)|*.xlsx";
                        saveFileDialog.RestoreDirectory = true;
                        saveFileDialog.OverwritePrompt = true;
                        saveFileDialog.Title = Properties.Resources.P2001010000_ExportFailDataDetail;
                        saveFileDialog.FileName = string.Format("{0}.xlsx", Properties.Resources.P2001010000_ExcelName);
                        bool? isShowOk;

                        isShowOk = saveFileDialog.ShowDialog();
                        if (isShowOk ?? false)
                        {
                            var excelExportService = new ExcelExportService();
                            excelExportService.CreateNewSheet(Properties.Resources.P2001010000_StockChangeDetail);

                            var failData = result.F1913DataFailItems.ToDataTable();

                            var showColumnName = new List<string>
                            {
                                "DC_CODEk__BackingField",
                                "LOC_CODEk__BackingField",
                                "ITEM_CODEk__BackingField",
                                "VALID_DATEk__BackingField",
                                "ENTER_DATEk__BackingField",
                                "MAKE_NOk__BackingField",
                                "ADJ_QTY_INk__BackingField",
                                "ADJ_QTY_OUTk__BackingField",
                                "CAUSEk__BackingField",
                                "SERIAL_NOk__BackingField",
                                "FailMessagek__BackingField"
                            };

                            var delColumnList = (from DataColumn column in failData.Columns where !showColumnName.Contains(column.ColumnName) select column.ColumnName).ToList();
                            foreach (var columnName in delColumnList)
                                failData.Columns.Remove(columnName);
                            failData.Columns["DC_CODEk__BackingField"].SetOrdinal(0);
                            failData.Columns["DC_CODEk__BackingField"].ColumnName = Properties.Resources.P2001010000_DC_CODE;
                            failData.Columns["LOC_CODEk__BackingField"].SetOrdinal(1);
                            failData.Columns["LOC_CODEk__BackingField"].ColumnName = Properties.Resources.P2001010000_LOC_CODE;
                            failData.Columns["ITEM_CODEk__BackingField"].SetOrdinal(2);
                            failData.Columns["ITEM_CODEk__BackingField"].ColumnName = Properties.Resources.P2001010000_ITEM_CODE;
                            failData.Columns["VALID_DATEk__BackingField"].SetOrdinal(3);
                            failData.Columns["VALID_DATEk__BackingField"].ColumnName = Properties.Resources.P2001010000_VALID_DATE;
                            failData.Columns["ENTER_DATEk__BackingField"].SetOrdinal(4);
                            failData.Columns["ENTER_DATEk__BackingField"].ColumnName = Properties.Resources.P2001010000_ENTER_DATE;
                            failData.Columns["MAKE_NOk__BackingField"].SetOrdinal(5);
                            failData.Columns["MAKE_NOk__BackingField"].ColumnName = Properties.Resources.P2001010000_MAKE_NO;
                            failData.Columns["ADJ_QTY_INk__BackingField"].SetOrdinal(6);
                            failData.Columns["ADJ_QTY_INk__BackingField"].ColumnName = Properties.Resources.P2001010000_ADJ_QTY_IN;
                            failData.Columns["ADJ_QTY_OUTk__BackingField"].SetOrdinal(7);
                            failData.Columns["ADJ_QTY_OUTk__BackingField"].ColumnName = Properties.Resources.P2001010000_ADJ_QTY_OUT;
                            failData.Columns["CAUSEk__BackingField"].SetOrdinal(8);
                            failData.Columns["CAUSEk__BackingField"].ColumnName = Properties.Resources.P2001010000_CAUSE;
                            failData.Columns["SERIAL_NOk__BackingField"].SetOrdinal(9);
                            failData.Columns["SERIAL_NOk__BackingField"].ColumnName = Properties.Resources.P2001010000_SERIAL_NO;
                            failData.Columns["FailMessagek__BackingField"].SetOrdinal(10);
                            failData.Columns["FailMessagek__BackingField"].ColumnName = Properties.Resources.P2001010000_FailMessage;
                            var excelExportSource = new ExcelExportReportSource
                            {
                                Data = failData,
                                DataFormatList =
                                    new List<NameValuePair<string>>
                                    {
                            new NameValuePair<string>() {Name = Properties.Resources.P2001010000_VALID_DATE, Value = "yyyy/m/d"},
                            new NameValuePair<string>() {Name = Properties.Resources.P2001010000_ENTER_DATE, Value = "yyyy/m/d"}
                                    }
                            };
                            excelExportService.AddExportReportSource(excelExportSource);
                            bool isExportSuccess = excelExportService.Export(Path.GetDirectoryName(saveFileDialog.FileName),
                                Path.GetFileName(saveFileDialog.FileName));
                            var message = new MessagesStruct
                            {
                                Button = DialogButton.OK,
                                Image = DialogImage.Information,
                                Message = "",
                                Title = Resources.Resources.Information
                            };
                            message.Message = isExportSuccess ? Properties.Resources.P2001010000_ExportInventoryDetailFailDataSuccess : Properties.Resources.P2001010000_ExportInventoryDetailFailDataFail;
                            if (!isExportSuccess)
                                message.Image = DialogImage.Warning;
                            ShowMessage(message);
                        }
                    }
                }
            }
        }

        #endregion

        public void GetItemName()
        {
            var proxy = GetProxy<F19Entities>();
            var item = proxy.F1903s.Where(o => o.GUP_CODE == GupCode && o.ITEM_CODE == AddItemCode && o.CUST_CODE == CustCode).ToList().FirstOrDefault();
            AddItemName = item == null ? "" : item.ITEM_NAME;
        }

        public void CheckSelectedAll(bool isChecked)
        {
            if (F050301Datas != null)
            {
                foreach (var f050301Data in F050301Datas)
                    f050301Data.IsSelected = isChecked;
            }
        }

        public void CheckSelectedAllItem(bool isChecked)
        {
            if (F1913Datas != null)
            {
                foreach (var f1913Data in F1913Datas)
                    f1913Data.IsSelected = isChecked;
            }
        }

        #region 檢查序號

        private string CheckSerialNoIsPass(F1913Data f1913Data, string serialNo)
        {
            var proxyEx = GetExProxy<ShareExDataSource>();
            var serialNoResultList = proxyEx.CreateQuery<exShare.SerialNoResult>("CheckSerialNoFull")
                .AddQueryExOption("dcCode", f1913Data.DC_CODE)
                .AddQueryExOption("gupCode", f1913Data.GUP_CODE)
                .AddQueryExOption("custCode", f1913Data.CUST_CODE)
                .AddQueryExOption("itemCode", f1913Data.ITEM_CODE)
                .AddQueryExOption("serialNo", serialNo)
                .AddQueryExOption("status", f1913Data.WORK_TYPE == "0" ? "A1" : "C1").ToList();
            return serialNoResultList.First().Message;
        }
        #endregion

        public string CheckZipCode(string zipCode, string oldZipCode)
        {
            if (string.IsNullOrWhiteSpace(zipCode))
            {
                ShowWarningMessage(Properties.Resources.P2001010000_ViewModel_ZIPCode_Required);
                return oldZipCode;
            }

            if (zipCode.Length < 3)
            {
                ShowWarningMessage(Properties.Resources.P2001010000_ViewModel_ZIPCode_Wrong);
                return oldZipCode;
            }
            var proxy = GetProxy<F19Entities>();
            if (!proxy.F1934s.Where(o => o.ZIP_CODE == zipCode).ToList().Any())
            {
                ShowWarningMessage(Properties.Resources.P2001010000_ViewModel_ZIPCode_NotFound);
                return oldZipCode;
            }
            return zipCode;
        }

        public void AppendItemCode(F1903 f1903)
        {
            if (!string.IsNullOrWhiteSpace(f1903.ITEM_CODE))
                AddItemCode1 = f1903.ITEM_CODE;
            if (!string.IsNullOrWhiteSpace(f1903.ITEM_NAME))
                AddItemName1 = f1903.ITEM_NAME;
        }

        private IEnumerable<string> GetSplitContent(string text)
        {
            return text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).Distinct();
        }

    }
}
