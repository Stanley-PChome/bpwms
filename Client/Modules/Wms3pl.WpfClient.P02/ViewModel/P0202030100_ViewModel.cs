using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
//using Wms3pl.Datas.Shared.Entities;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F01DataService;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F25DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using Wms3pl.WpfClient.P02.Services;
using Wms3pl.WpfClient.P08.Services;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using ExecuteResult = Wms3pl.WpfClient.ExDataServices.P02ExDataService.ExecuteResult;
using exShare = Wms3pl.WpfClient.ExDataServices.ShareExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P02WcfService;
using wcfShare = Wms3pl.WpfClient.ExDataServices.SharedWcfService;
using wcf19 = Wms3pl.WpfClient.ExDataServices.P19WcfService;
using Wms3pl.Common.Helper;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0202030100_ViewModel : InputViewModelBase
	{
		public Action ActionAfterCheckSerialNo = delegate { };
		public Action OnCancelComplete = delegate { };
		public Action DoOpenP1901030000 = delegate { };
    public Action DoFoucusValidDate = delegate { };
    public Action DoClose = delegate { };
		private string _userId = Wms3plSession.Get<UserInfo>().Account;
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }

		public P0202030100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}
		}

		private void InitControls()
		{
			UserOperateMode = OperateMode.Edit;
			DoSearchUccList();
			TmprTypeList = GetBaseTableService.GetF000904List(FunctionCode, "F1903", "TMPR_TYPE");
		}

    

		#region 資料連結/ 頁面參數
		private void PageRaisePropertyChanged()
		{
		}
		#region Form - 商品溫層

		private List<NameValuePair<string>> _tmprTypeList;

		public List<NameValuePair<string>> TmprTypeList
		{
			get { return _tmprTypeList; }
			set
			{
				Set(() => TmprTypeList, ref _tmprTypeList, value);
			}
		}
		private string _selectedTmprType = string.Empty;
		public string SelectedTmprType
		{
			get { return _selectedTmprType; }
			set
			{
				_selectedTmprType = value;
				RaisePropertyChanged("SelectedTmprType");
			}
		}
		#endregion

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
		#endregion
		#region Form - 目前檢視的頁籤
		private int _selectedTabIndex = 0;
		public int SelectedTabIndex
		{
			get { return _selectedTabIndex; }
			set
			{
				_selectedTabIndex = value;
				DoRefreshReadCount();
				if (value == 0) DoSearchCheckItem();
				RaisePropertyChanged("SelectedTabIndex");
			}
		}
		#endregion
		#region Form - 上傳圖檔名稱/ ShareFolder路徑
		/// <summary>
		/// 圖檔是否已上傳, 給Upload Button要不要顯示
		/// </summary>
		public bool IsImageUploaded
		{
			get { return BaseData.ISUPLOAD == "1"; }
		}
		private string _imagePathForDisplay = string.Empty;
		/// <summary>
		/// 取得圖檔路徑, 用來呈現在Image上
		/// 這裡不取資料庫裡的IMAGE_PATH, 因為位置可能變更, 所以還是抓APP SETTING的路徑為主
		/// </summary>
		public string ImagePathForDisplay
		{
			get { return _imagePathForDisplay; }
			set { _imagePathForDisplay = value; RaisePropertyChanged("ImagePathForDisplay"); }
		}

		private string _fileName = string.Empty;
		public string FileName
		{
			get { return _fileName; }
			set { _fileName = value; }
		}
		private BitmapImage _itemImageSource = null;
		/// <summary>
		/// 顯示圖片
		/// Memo: 可用此方式來避免圖檔被程式咬住而無法刪除或移動
		/// </summary>
		public BitmapImage ItemImageSource
		{
			get
			{
				return _itemImageSource;
			}
			set
			{
				if (BaseData == null || string.IsNullOrWhiteSpace(BaseData.ITEM_CODE)) _itemImageSource = null;
				else
					_itemImageSource = FileService.GetItemImage(_gupCode, _custCode, BaseData.ITEM_CODE);
				RaisePropertyChanged("ItemImageSource");
			}
		}
    #endregion
    #region Data - 基本資料

    /// <summary>
    /// 是否為首次開啟畫面
    /// </summary>
    public Boolean IsFirstLoad;

    /// <summary>
    /// 最原始傳進來的資料，使用在判斷保存天數異動時，警示天數、若允收天數是否為空值用（因為BaseData的這兩個記錄會異動）
    /// </summary>
    private P020203Data _baseDataOriginal;
		private P020203Data _baseData;
		public P020203Data BaseData
		{
			get { return _baseData; }
			set
			{
				if (_baseData == null)
					_baseDataOriginal = value.Clone();
				_baseData = value;
				ItemImageSource = null;
				RefreshImage();
				RaisePropertyChanged("BaseData");
			}
		}
		private DateTime _deliverDate;
		/// <summary>
		/// 預計進貨日
		/// </summary>
		public DateTime DeliverDate
		{
			get { return _deliverDate; }
			set { _deliverDate = value; RaisePropertyChanged("DeliverDate"); }
		}

    #region 商品效期
    private string _validDateStr;
    public string ValidDateStr
    {
      get { return _validDateStr; }
      set
      {
        Set(() => ValidDateStr, ref _validDateStr, value);
      }
    }

    private DateTime? _validDate;

		public DateTime? ValidDate
		{
			get { return _validDate; }
			set
			{
				if (value == null)
        {
          //value = new DateTime(9999, 12, 31);
          Set(() => ValidDate, ref _validDate, value);
          ValidDateStr = null;
        }
        else
				{
          if (value.HasValue)
            ValidDateStr = value.Value.ToString("yyyy/MM/dd");
          else
            ValidDateStr = null;

          if (value == _validDate)
						return;

          var checkValidDate = CheckValidDate(value);
					if (!checkValidDate.IsSuccessed)
					{
						DispatcherAction(() =>
						{
							ShowWarningMessage(checkValidDate.Message);
              DoFoucusValidDate();
						});
					}

					Set(() => ValidDate, ref _validDate, value);

          //跑兩次原因是因為有可能效期檢查不通過，要還原成原本的值
          if (ValidDate.HasValue)
            ValidDateStr = ValidDate.Value.ToString("yyyy/MM/dd");
          else
            ValidDateStr = null;
        }

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

		#region 商品長寬高重量
		private decimal? _itemHight;
		[Range(typeof(Decimal), "0", "9999999999")]
		[Display(Name = "Range_ErrorMessage", ResourceType = typeof(Resources.Resources))]
		public decimal? ItemHight
		{
			get { return _itemHight; }
			set
			{
				var hightValue = value.ToString();
				Regex reg = new Regex(@"^[0-9]+(.[0-9]{1,2})?$");
				Match match = reg.Match(hightValue);

				if (string.IsNullOrWhiteSpace(hightValue) || match.Success)
				{
					_itemHight = value; RaisePropertyChanged("ItemHight");
				}
			}
		}

		private decimal? _itemLenght;
		[Range(typeof(Decimal), "0", "9999999999")]
		[Display(Name = "Range_ErrorMessage", ResourceType = typeof(Resources.Resources))]
		public decimal? ItemLenght
		{
			get { return _itemLenght; }
			set
			{
				var lenghtValue = value.ToString();
				Regex reg = new Regex(@"^[0-9]+(.[0-9]{1,2})?$");
				Match match = reg.Match(lenghtValue);

				if (string.IsNullOrWhiteSpace(lenghtValue) || match.Success)
				{
					_itemLenght = value; RaisePropertyChanged();
				}
			}
		}

		private decimal? _itemWidth;
		[Range(typeof(Decimal), "0", "9999999999")]
		[Display(Name = "Range_ErrorMessage", ResourceType = typeof(Resources.Resources))]
		public decimal? ItemWidth
		{
			get { return _itemWidth; }
			set
			{
				var widthValue = value.ToString();
				Regex reg = new Regex(@"^[0-9]+(.[0-9]{1,2})?$");
				Match match = reg.Match(widthValue);

				if (string.IsNullOrWhiteSpace(widthValue) || match.Success)
				{
					_itemWidth = value; RaisePropertyChanged("ItemWidth");
				}
			}
		}

		private decimal? _itemWeight;
		[Range(typeof(Decimal), "0", "9999999999")]
		[Display(Name = "Range_ErrorMessage", ResourceType = typeof(Resources.Resources))]
		public decimal? ItemWeight
		{
			get { return _itemWeight; }
			set
			{
				var weightValue = value.ToString();
				Regex reg = new Regex(@"^[0-9]+(.[0-9]{1,2})?$");
				Match match = reg.Match(weightValue);

				if (string.IsNullOrWhiteSpace(weightValue) || match.Success)
				{
					_itemWeight = value; RaisePropertyChanged("ItemWeight");
				}
			}
		}

		// 
		private bool _enableItemLenght;
		public bool EnableItemLenght
		{
			get { return _enableItemLenght; }
			set { Set(() => EnableItemLenght, ref _enableItemLenght, value); }
		}

		private bool _enableItemWidth;
		public bool EnableItemWidth
		{
			get { return _enableItemWidth; }
			set { Set(() => EnableItemWidth, ref _enableItemWidth, value); }
		}

		private bool _enableItemHight;
		public bool EnableItemHight
		{
			get { return _enableItemHight; }
			set { Set(() => EnableItemHight, ref _enableItemHight, value); }
		}

		private bool _enableItemWeight;
		public bool EnableItemWeight
		{
			get { return _enableItemWeight; }
			set { Set(() => EnableItemWeight, ref _enableItemWeight, value); }
		}
		#endregion

		/// <summary>
		/// 商品材積檔
		/// </summary>
		private F1905 _itemRecordVolume;
		public F1905 ItemRecordVolume
		{
			get { return _itemRecordVolume; }
			set { _itemRecordVolume = value; RaisePropertyChanged("ItemRecordVolume"); }
		}

		#endregion
		#region Data - 序號刷讀
		private int _dgSelectedIndex = 0;
		public int DgSelectedIndex
		{
			get { return _dgSelectedIndex; }
			set { _dgSelectedIndex = value; RaisePropertyChanged("DgSelectedIndex"); }
		}
		private ObservableCollection<exShare.SerialNoResult> _dgSerialList = new ObservableCollection<exShare.SerialNoResult>();
		/// <summary>
		/// 刷讀的結果集
		/// </summary>
		public ObservableCollection<exShare.SerialNoResult> DgSerialList
		{
			get { return _dgSerialList; }
			set { _dgSerialList = value; RaisePropertyChanged("DgSerialList"); }
		}
		private string _newSerialNo = string.Empty;
		/// <summary>
		/// 新序號
		/// </summary>
		public string NewSerialNo
		{
			get { return _newSerialNo; }
			set
			{
				_newSerialNo = value;
				RaisePropertyChanged("NewSerialNo");
			}
		}
		private SerialStatistic _serialCount = new SerialStatistic() { CurrentCount = 0, InvalidCount = 0, TotalValidCount = 0, ValidCount = 0 };
		public SerialStatistic SerialCount
		{
			get { return _serialCount; }
			set { _serialCount = value; RaisePropertyChanged("SerialCount"); }
		}
		#endregion
		#region Data - 商品檢驗項目
		private ObservableCollection<F190206CheckName> _checkList = new ObservableCollection<F190206CheckName>();
		public ObservableCollection<F190206CheckName> CheckList
		{
			get { return _checkList; }
			set { _checkList = value; RaisePropertyChanged("CheckList"); }
		}
		#endregion
		#region Data - 檢驗未通過原因
		private List<F1951> _uccList = new List<F1951>();
		public List<F1951> UccList
		{
			get { return _uccList; }
			set { _uccList = value; RaisePropertyChanged("UccList"); }
		}
		#endregion
		#region Form - 驗收單號
		private string _rtNo = string.Empty;
		/// <summary>
		/// 新增資料時需輸入進倉單號
		/// </summary>
		public string RtNo
		{
			get { return _rtNo; }
			set { _rtNo = value; }
		}
		#endregion
		#region Data - 已抽驗商品狀態
		private string _checkItem = string.Empty;
		/// <summary>
		/// 已抽驗商品狀態
		/// </summary>
		public string CheckItem
		{
			get { return _checkItem; }
			set { _checkItem = value; }
		}
		#endregion
		#region
		private string _eanCode1;
		public string EanCode1
		{
			get { return _eanCode1; }
			set
			{
				_eanCode1 = value;
				RaisePropertyChanged("EanCode1");
			}
		}
		#endregion
		#region
		private string _eanCode2;
		public string EanCode2
		{
			get { return _eanCode2; }
			set
			{
				_eanCode2 = value;
				RaisePropertyChanged("EanCode2");
			}
		}
		#endregion
		#region
		private string _eanCode3;
		public string EanCode3
		{
			get { return _eanCode3; }
			set
			{
				_eanCode3 = value;
				RaisePropertyChanged("EanCode3");
			}
		}
		#endregion

		#region 
		private bool _enableEanCode1;
		public bool EnableEanCode1
		{
			get { return _enableEanCode1; }
			set
			{
				_enableEanCode1 = value;
				//_enableEanCode1 = EanCode1 == null ? true : false;
				RaisePropertyChanged("EnableEanCode1");
			}
		}
		#endregion
		#region 
		private bool _enableEanCode2;
		public bool EnableEanCode2
		{
			get { return _enableEanCode2; }
			set
			{
				_enableEanCode2 = value;
				//_enableEanCode2 = EanCode2 == null ? true : false;
				RaisePropertyChanged("EnableEanCode2");
			}
		}
		#endregion
		#region 
		private bool _enableEanCode3;
		public bool EnableEanCode3
		{
			get { return _enableEanCode3; }
			set
			{
				_enableEanCode3 = value;
				//_enableEanCode3 = EanCode3 == null ? true : false;
				RaisePropertyChanged("EnableEanCode3");
			}
		}
		#endregion

		#region 
		private string _needExpired;
		public string NeedExpired
		{
			get { return _needExpired; }
			set
			{
				//if(string.IsNullOrWhiteSpace(_needExpired))
				//{
				//    EnableNeedExpired = true;
				//}
				Set(() => NeedExpired, ref _needExpired, value);
				if (string.IsNullOrEmpty(value) || value == "0")
				{
					BaseData.ALL_DLN = null;
					BaseData.ALL_SHP = null;
					SaveDay = null;
					ShowSaveDayError = "0";
					ValidDate = new DateTime(9999, 12, 31);
				}
				else
				{
					if(_baseDataOriginal.ALL_DLN.HasValue)
						BaseData.ALL_DLN = _baseDataOriginal.ALL_DLN;
					if(_baseDataOriginal.ALL_SHP.HasValue)
						BaseData.ALL_SHP = _baseDataOriginal.ALL_SHP;
          ValidDate = null;

        }

				if (FirstInDate == null)
					EnabledSaveDay = value == "1";

				
			}
		}
		#endregion
		#region 
		private bool _enabledSaveDay;
		public bool EnabledSaveDay
		{
			get { return _enabledSaveDay; }
			set
			{
				_enabledSaveDay = value;
				RaisePropertyChanged("EnabledSaveDay");
			}
		}
		#endregion

		#region 允許使用效期商品
		private bool _enableNeedExpired;
		public bool EnableNeedExpired
		{
			get { return _enableNeedExpired; }
			set
			{
				_enableNeedExpired = value;
				RaisePropertyChanged("EnableNeedExpired");
			}
		}
		#endregion
		#region 保存天數
		private int? _saveDay;
		public int? SaveDay
		{
			get { return _saveDay; }
			set
			{
				_saveDay = value;
				RaisePropertyChanged("SaveDay");
			}
		}
		#endregion
		#region  首次進倉日
		private DateTime? _firstInDate;
		public DateTime? FirstInDate
		{
			get { return _firstInDate; }
			set
			{
				_firstInDate = value;
				RaisePropertyChanged("FirstInDate");

			}
		}
		#endregion
		#region 貴重品標示
		private string _isPrecious;
		public string IsPrecious
		{
			get { return _isPrecious; }
			set
			{
				Set(() => IsPrecious, ref _isPrecious, value);
			}
		}

		//允許使用貴重品標示
		private bool _enableIsPrecious;
		public bool EnableIsPrecious
		{
			get { return _enableIsPrecious; }
			set
			{
				Set(() => EnableIsPrecious, ref _enableIsPrecious, value);
			}
		}
		#endregion
		#region 易遺失品
		private string _isEasyLose;
		public string IsEasyLose
		{
			get { return _isEasyLose; }
			set
			{
				Set(() => IsEasyLose, ref _isEasyLose, value);
			}
		}

		//允許使用易遺失品
		private bool _enableIsEasyLose;
		public bool EnableIsEasyLose
		{
			get { return _enableIsEasyLose; }
			set
			{
				Set(() => EnableIsEasyLose, ref _enableIsEasyLose, value);
			}
		}
		#endregion
		#region 強磁標示
		private string _isMagnetic;
		public string IsMagnetic
		{
			get { return _isMagnetic; }
			set
			{
				Set(() => IsMagnetic, ref _isMagnetic, value);
			}
		}

		//允許使用強磁標示
		private bool _enableIsMagnetic;
		public bool EnableIsMagnetic
		{
			get { return _enableIsMagnetic; }
			set
			{
				Set(() => EnableIsMagnetic, ref _enableIsMagnetic, value);
			}
		}
		#endregion
		#region 易碎品
		private string _fragile;
		public string Fragile
		{
			get { return _fragile; }
			set
			{
				Set(() => Fragile, ref _fragile, value);
			}
		}

		//允許使用易碎品
		private bool _enableFragile;
		public bool EnableFragile
		{
			get { return _enableFragile; }
			set
			{
				Set(() => EnableFragile, ref _enableFragile, value);
			}
		}
		#endregion
		#region 液體
		private string _spill;
		public string Spill
		{
			get { return _spill; }
			set
			{
				Set(() => Spill, ref _spill, value);
			}
		}

		//允許使用液體
		private bool _enableSpill;
		public bool EnableSpill
		{
			get { return _enableSpill; }
			set
			{
				Set(() => EnableSpill, ref _enableSpill, value);
			}
		}
		#endregion
		#region 易變質
		private string _isPerishable;
		public string IsPerishable
		{
			get { return _isPerishable; }
			set
			{
				Set(() => IsPerishable, ref _isPerishable, value);
			}
		}

		private bool _enableIsPerishable;
		public bool EnableIsPerishable
		{
			get { return _enableIsPerishable; }
			set
			{
				Set(() => EnableIsPerishable, ref _enableIsPerishable, value);
			}
		}
		#endregion

		#region 需溫控
		private string _isTempControl;
		public string IsTempControl
		{
			get { return _isTempControl; }
			set
			{
				Set(() => IsTempControl, ref _isTempControl, value);
			}
		}

		private bool _enableIsTempControl;
		public bool EnableIsTempControl
		{
			get { return _enableIsTempControl; }
			set
			{
				Set(() => EnableIsTempControl, ref _enableIsTempControl, value);
			}
		}
		#endregion
		#region 顯示總保存天數錯誤
		private string _showSaveDayError = "0";
		public string ShowSaveDayError
		{
			get { return _showSaveDayError; }
			set
			{
				_showSaveDayError = value;
				RaisePropertyChanged("ShowSaveDayError");
			}
		}
		#endregion

		#endregion

		#region Command
		#region Search
		private void DoSearchUccList()
		{
			var proxy = GetProxy<F19Entities>();
			var result = proxy.F1951s.Where(x => x.UCT_ID.Equals("CC")).ToList();
			UccList = result;
		}
		/// <summary>
		/// 取得商品檢驗項目
		/// </summary>
		public void DoSearchCheckItem()
		{
			var proxy = GetExProxy<P02ExDataSource>();
			var result = proxy.CreateQuery<F190206CheckName>("GetItemCheckList")
				.AddQueryOption("dcCode", string.Format("'{0}'", SelectedDc))
				.AddQueryOption("gupCode", string.Format("'{0}'", this._gupCode))
				.AddQueryOption("custCOde", string.Format("'{0}'", this._custCode))
				.AddQueryOption("purchaseNO", string.Format("'{0}'", BaseData.PURCHASE_NO))
				.AddQueryOption("purchaseSeq", string.Format("'{0}'", BaseData.PURCHASE_SEQ))
				.AddQueryOption("itemCode", string.Format("'{0}'", BaseData.ITEM_CODE))
				.AddQueryOption("rtNo", string.Format("'{0}'", RtNo))
				.AddQueryOption("checkType", string.Format("'{0}'", "00")) // 進貨檢驗

				.ToList();
			CheckList = result.ToObservableCollection();
		}
		#endregion
		#region Cancel
		private bool _okToCancel = false;
    public ICommand CancelCommand
		{
			get
			{
        _okToCancel = false;

        return CreateBusyAsyncCommand(
					o => _okToCancel = DoCancel(),
					() =>
					{
						return UserOperateMode == OperateMode.Edit;
					},
					o =>
					{
						if (_okToCancel) OnCancelComplete();
					}
				);
			}
		}
		public bool DoCancel()
		{
			if (ShowMessage(Messages.WarningBeforeExit) == DialogResponse.OK)
			{
				UserOperateMode = OperateMode.Query;
				return true;
			}
			return false;
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
						return UserOperateMode == OperateMode.Edit && BaseData != null && ShowSaveDayError == "0";
					},
					c =>
					{
						if (_isSaveOk)
						{
							var proxyF19 = GetProxy<F19Entities>();
							var itemCount =
								proxyF19.F1905s.Where(o => o.GUP_CODE == _gupCode && o.ITEM_CODE == BaseData.ITEM_CODE)
									.Count();
							if (itemCount == 0)
							{
								var msg = new MessagesStruct()
								{
									Button = DialogButton.YesNo,
									Image = DialogImage.Question,
									Message = Properties.Resources.P0202030100_ItemCountZero,
									Title = Properties.Resources.P0202030100_Ask
								};
								if (ShowMessage(msg) == DialogResponse.Yes)
									DoOpenP1901030000();
								else
									ChangeSelectedTab();
							}
							else
								ChangeSelectedTab();
						}
						else
						{
							if (_checkItem == "F")
							{
								UserOperateMode = OperateMode.Query;
								OnCancelComplete();
							}
						}
					}
				);
			}
		}

		private bool _isSaveOk;
		/// <summary>
		/// 寫入商品檢驗結果
		/// </summary>
		/// <returns></returns>
		public bool DoSave()
		{
			_isSaveOk = false;

			if (CheckList == null || !CheckList.Any())
			{
				ShowMessage(new List<ExecuteResult>() { new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P0202030100_CheckListIsNull } });
				return false;
			}

      if (!DoConvertToValidDate(true))
        return false;

      if (!DoCheck()) return false;

			var proxyEntities = GetProxy<F19Entities>();
			var tmpVolume = proxyEntities.F1905s.Where(x => x.ITEM_CODE == BaseData.ITEM_CODE && x.GUP_CODE == BaseData.GUP_CODE && x.CUST_CODE == BaseData.CUST_CODE)
				.ToList().FirstOrDefault();
			if (tmpVolume != null)
				ItemRecordVolume = Mapper.DynamicMap<F1905>(tmpVolume);
			var f1903 = proxyEntities.F1903s.Where(x => x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode && x.ITEM_CODE == BaseData.ITEM_CODE).FirstOrDefault();
			if (f1903 != null && f1903.MAKENO_REQU == "1")
			{
				if (string.IsNullOrWhiteSpace(MakeNo) || MakeNo == "0")
				{
					ShowWarningMessage(Properties.Resources.P02020301_MakeNoCheckError);
					return false;
				}

				var f1909 = proxyEntities.F1909s.Where(o => o.GUP_CODE == _gupCode && o.CUST_CODE == _custCode).FirstOrDefault();
				if (f1909 != null && f1909.VALID_DATE_CHKYEAR != null)
				{
					var checkYear = (int)f1909.VALID_DATE_CHKYEAR;
					if (DateTime.Today.AddYears(checkYear) < ValidDate)
					{
						ShowWarningMessage(string.Format(Properties.Resources.P02020301_ValidDateCheckOver, checkYear));
						return false;
					}
				}
			}

			//允收天數檢核
			//_checkItem = string.Empty;
			//var resultMsg = DoCheckAll_Dln();
			//if (!string.IsNullOrEmpty(resultMsg))
			//{
			//    var msg = new MessagesStruct()
			//    {
			//        Button = DialogButton.YesNo,
			//        Image = DialogImage.Question,
			//        Message = resultMsg,
			//        Title = Properties.Resources.P0202030100_Ask
			//    };

			//    if (ShowMessage(msg) == DialogResponse.No)
			//    {
			//        _checkItem = "F";
			//        return false;
			//    }
			//    else
			//    {
			//        //儲存時,商品允收天數不足，仍確認儲存，CHECK_ITEM改為0
			//        _checkItem = "0";
			//    }
			//}

			var proxy = new wcf.P02WcfServiceClient();
			wcf.ExecuteResult result = new wcf.ExecuteResult() { IsSuccessed = true };
			wcf.F1905 volumeItem = null;

			//商品長寬高、重量有異動時，才進行更新
			if (ItemHight != BaseData.PACK_HIGHT ||
				ItemLenght != BaseData.PACK_LENGTH ||
				ItemWidth != BaseData.PACK_WIDTH ||
				ItemWeight != BaseData.PACK_WEIGHT)
			{
				volumeItem = new wcf.F1905();
				ItemRecordVolume.PACK_LENGTH = ItemLenght.Value;
				ItemRecordVolume.PACK_WIDTH = ItemWidth.Value;
				ItemRecordVolume.PACK_HIGHT = ItemHight.Value;
				ItemRecordVolume.PACK_WEIGHT = ItemWeight.Value;
				volumeItem = SetObject(ItemRecordVolume, volumeItem) as wcf.F1905;
			}

			// 若未選擇是否為效期商品，將是將該商品視為非效期商品
			if (string.IsNullOrWhiteSpace(NeedExpired))
			{
				NeedExpired = "0";
			}

			//首次進倉日若為空值，將首次進倉日設定為今日
			FirstInDate = FirstInDate == null ? DateTime.Today : FirstInDate;


			var tmpData = RunWcfMethod(proxy.InnerChannel, () => CheckList.Select(AutoMapper.Mapper.DynamicMap<wcf.F190206CheckName>).ToList());
			result = RunWcfMethod(proxy.InnerChannel, () => proxy.UpdateF02020102(SelectedDc, this._gupCode, this._custCode, BaseData.PURCHASE_NO, BaseData.PURCHASE_SEQ
				, tmpData.ToArray(), ValidDate?.ToString("yyyy/MM/dd"), RtNo, _checkItem, volumeItem, MakeNo));
			ShowMessage(new List<ExecuteResult>() { new ExecuteResult() { IsSuccessed = result.IsSuccessed, Message = result.Message } });

      //丟到後端要異動的欄位都要檢查有沒有異動
      if (_baseDataOriginal.NEED_EXPIRED != NeedExpired ||
          _baseDataOriginal.FIRST_IN_DATE != FirstInDate ||
          _baseDataOriginal.SAVE_DAY != SaveDay ||
          _baseDataOriginal.EAN_CODE1 != EanCode1 ||
          _baseDataOriginal.EAN_CODE2 != EanCode2 ||
          _baseDataOriginal.EAN_CODE3 != EanCode3 ||
          _baseDataOriginal.ALL_DLN != BaseData.ALL_DLN ||
          _baseDataOriginal.ALL_SHP != BaseData.ALL_SHP ||
          _baseDataOriginal.IS_PRECIOUS != IsPrecious ||
          _baseDataOriginal.FRAGILE != Fragile ||
          _baseDataOriginal.IS_EASY_LOSE != IsEasyLose ||
          _baseDataOriginal.SPILL != Spill ||
          _baseDataOriginal.IS_MAGNETIC != IsMagnetic ||
          _baseDataOriginal.IS_PERISHABLE != IsPerishable ||
          _baseDataOriginal.TMPR_TYPE != SelectedTmprType ||
          _baseDataOriginal.IS_TEMP_CONTROL != IsTempControl ||
		  volumeItem != null)
        RunWcfMethod(proxy.InnerChannel, () => proxy.UpdateF1903(this._gupCode, this._custCode, BaseData.ITEM_CODE, NeedExpired, FirstInDate, SaveDay, EanCode1, EanCode2, EanCode3, BaseData.ALL_DLN, BaseData.ALL_SHP, IsPrecious, Fragile,
          IsEasyLose, Spill, IsMagnetic, IsPerishable, SelectedTmprType, IsTempControl, volumeItem));


      if (result.IsSuccessed)
				_isSaveOk = true;
			return true;
		}
		private object SetObject(object sourceItem, object targetItem)
		{
			var props = sourceItem.GetType().GetProperties();
			foreach (PropertyInfo prop in props)
			{
				if (prop.Name.ToLower() == "item" || prop.Name.ToLower() == "error") continue;
				string propName = string.Format("{0}k__BackingField", prop.Name);
				var sourceValue = prop.GetValue(sourceItem, null);
				var targetProp = targetItem.GetType().GetProperty(propName);
				if (targetProp == null) continue;
				targetProp.SetValue(targetItem, sourceValue);
			}
			return targetItem;
		}

		/// <summary>
		///系統檢查是否已完成此商品檢驗, 且是否需要做序號刷讀(F02020101.BUNDLE_SERIALNO), 是則自動跳至商品序號刷讀頁籤
		/// </summary>
		public void ChangeSelectedTab()
		{
			if (BaseData.BUNDLE_SERIALNO == "1" && DoCheckIsAllPass())
			{
				if (DoCheckIsAllPass())
				{
					//若序號收集以刷讀續序號，不再做序號刷讀
					if (!DoSerialAllPass())
					{
						SelectedTabIndex = 1;
						RaisePropertyChanged("SelectedTabIndex");
					}
					else
					{
						DoClose();
					}
				}
				else
					DoClose();

				//SelectedTabIndex = 1;
				//RaisePropertyChanged("SelectedTabIndex");
			}
			else
				DoClose();
		}

		private bool DoCheck()
		{
      ExecuteResult checkResult=new ExecuteResult();


      if (CheckList.Any(x => (x.ISPASS ?? "0") == "0" && string.IsNullOrWhiteSpace(x.UCC_CODE)))
			{
				// 有檢查項目未勾選PASS, 且未選擇原因
				ShowMessage(Messages.WarningNotUccSelected);
				return false;
			}

			var maxDate = DateTime.MaxValue.Date;

			//允收效期的檢查,若F1909.ISLATEST_VALID_DATE(是否只允許最新的效期進倉)=1,則目前進倉的效期要比之前驗收單中的長
			var proxy = GetProxy<F19Entities>();
			var tmp = proxy.F1909s.Where(x => x.GUP_CODE.Equals(this._gupCode) && x.CUST_CODE.Equals(this._custCode)).FirstOrDefault();
			//if (tmp.ISLATEST_VALID_DATE == "1")
			//{
			//	var proxyF02 = GetProxy<F02Entities>();
			//	var tmpF02 = proxyF02.F020201s.Where(x => x.GUP_CODE.Equals(this._gupCode) && x.CUST_CODE.Equals(this._custCode) && x.DC_CODE.Equals(SelectedDc) && x.ITEM_CODE.Equals(BaseData.ITEM_CODE)).ToList();

			//	if (tmpF02.Any())
			//	{
			//		var f02ValiDate = tmpF02.Max(x => x.VALI_DATE);
			//		if (f02ValiDate != null)
			//		{
			//			if ((ValidDate <= f02ValiDate.Value && f02ValiDate.Value.Date != maxDate.Date) || (ValidDate < f02ValiDate.Value && f02ValiDate.Value.Date == maxDate))
			//			{
			//				ShowWarningMessage(Properties.Resources.P0202030100_ValidDateError + f02ValiDate.Value.ToString("yyyy/MM/dd"));
			//				return false;
			//			}
			//		}
			//	}
			//}

			if (tmp.NEED_ITEMSPEC == "1")
			{
				//if (ItemLenght == 0 || ItemWidth == 0 || ItemHight == 0 || ItemWeight == 0)
				//{
				//	//商品的長寬高重量皆不可設定為0，提示要輸入正確的長寬高資料
				//	ShowMessage(new List<ExecuteResult>() { new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P0202030100_ViewModel_ItemLengthWidthHeightWeightInvalid } });
				//	return false;
				//}

				if (ItemHight == 1 && ItemLenght == 1 && ItemWidth == 1)
				{
					//商品 的長寬高都為1，請提示要輸入正確的長寬高資料
					ShowMessage(new List<ExecuteResult>() { new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P0202030100_ViewModel_InputItemLengthWidthHight } });
					return false;
				}
			}
			else
			{
				if (ItemLenght == 0 || ItemWidth == 0 || ItemHight == 0 || ItemWeight == 0 || ItemLenght == null || ItemWidth == null || ItemHight == null || ItemWeight == null)
				{
					//商品的長寬高重量皆不可設定為0，提示要輸入正確的長寬高資料
					ShowMessage(new List<ExecuteResult>() { new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P0202030100_ViewModel_ItemLengthWidthHeightWeightInvalid } });
					return false;
				}
			}

			// 若為商品首次驗收
			if (!BaseData.FIRST_IN_DATE.HasValue)
			{
				// 若為效期商品，總保存天數必填且大於0
				if (NeedExpired == "1" && (SaveDay == null || SaveDay <= 0))
				{
					DialogService.ShowMessage(Properties.Resources.P0202030100_SaveDayRequired);
					return false;
				}

        if (ItemLenght == null || ItemWidth == null || ItemHight == null || ItemWeight == null ||
										ItemLenght == 0 || ItemWidth == 0 || ItemHight == 0 || ItemWeight == 0)
				{
					DialogService.ShowMessage(Properties.Resources.P0202030100_LenghtWidthHightWeightRequired);
					return false;
				}
			}

			if (NeedExpired == "1" && ValidDate == null)
			{
        DispatcherAction(() =>
        {
          DialogService.ShowMessage(Properties.Resources.P0202030100_ValidDateRequired);
          DoFoucusValidDate();
        });
        return false;
			}
			checkResult = CheckValidDate(ValidDate);
			if (!checkResult.IsSuccessed)
			{
        DispatcherAction(() =>
        {
          DialogService.ShowMessage(checkResult.Message);
          DoFoucusValidDate();
        });

        return false;
			}

			// 當非效期商品，商品效期必須為9999/12/31
			if ((NeedExpired == "0" || string.IsNullOrWhiteSpace(NeedExpired)) && ValidDate != new DateTime(9999, 12, 31))
			{
				DialogService.ShowMessage(Properties.Resources.P0202030100_ValidDateConfirm);
				return false;
			}

			//
			if (ValidDate == null)
			{
				DialogService.ShowMessage(Properties.Resources.P0202030100_ValidDateNotNull);
				return false;
			}

			if (!CheckEANCodeIsMatch(true))
				return false;

			if (NeedExpired == "1")
			{
				if (ShowConfirmMessage($"{checkResult.Message}{(string.IsNullOrWhiteSpace(checkResult.Message) ? string.Empty : "\r\n")}商品效期一旦驗收完成，將無法修改。請確認商品效期是否輸入正確。") == DialogResponse.No)
				{
					return false;
				}
			}
			if (StringHelper.CheckStringIncludeWhiteSpace(EanCode1))
			{
				DialogService.ShowMessage(string.Format("刷入的國際條碼一[{0}]中有空白，請重新刷入", EanCode1));
				return false;
			}
			if (StringHelper.CheckStringIncludeWhiteSpace(EanCode2))
			{
				DialogService.ShowMessage(string.Format("刷入的國際條碼二[{0}]中有空白，請重新刷入", EanCode2));
				return false;
			}
			if (StringHelper.CheckStringIncludeWhiteSpace(EanCode3))
			{
				DialogService.ShowMessage(string.Format("刷入的國際條碼三[{0}]中有空白，請重新刷入", EanCode3));
				return false;
			}

			return true;
		}

		/// <summary>
		/// 檢查是否所有檢驗項目均通過
		/// </summary>
		/// <returns>true: 全部通過, false: 任一項未通過</returns>
		private bool DoCheckIsAllPass()
		{
			var proxy = GetProxy<F02Entities>();
			var tmp = proxy.F02020102s.Where(x => x.DC_CODE.Equals(SelectedDc)
				&& x.GUP_CODE.Equals(this._gupCode) && x.CUST_CODE.Equals(this._custCode)
				&& x.PURCHASE_NO.Equals(BaseData.PURCHASE_NO) && x.PURCHASE_SEQ.Equals(BaseData.PURCHASE_SEQ)
				&& x.ITEM_CODE.Equals(BaseData.ITEM_CODE) && x.ISPASS.Equals("0")).Count() == 0;
			return tmp;
		}

		private bool DoSerialAllPass()
		{
			var proxy = GetProxy<F02Entities>();
			var serialNoList = proxy.F020302s.Where(x => x.DC_CODE.Equals(SelectedDc) &&
																				x.GUP_CODE.Equals(_gupCode) &&
																				x.CUST_CODE.Equals(_custCode) &&
																				x.PO_NO.Equals(BaseData.SHOP_NO) &&
																				x.ITEM_CODE.Equals(BaseData.ITEM_CODE)).FirstOrDefault();
			// 當F020302.PO_NO = BaseData.SHOP_NO 代表是事由進倉序號檔匯入序號，不進入序號刷讀，且當應刷數=實收總數代表以刷讀完成
			return serialNoList != null && SerialCount.TotalValidCount > 0 ? true : false;
		}

		/// <summary>
		/// 允收天數檢核
		/// </summary>
		/// <returns></returns>
		private string DoCheckAll_Dln()
		{
			var msg = string.Empty;
			var proxy = GetProxy<F19Entities>();
			var tmp = proxy.F1903s.Where(x => x.GUP_CODE.Equals(this._gupCode) && x.CUST_CODE.Equals(this._custCode)
				&& x.ITEM_CODE.Equals(BaseData.ITEM_CODE)).FirstOrDefault();

			//該商品的允收天數(F1903.all_dln) 非空值或 大於 0
			if (tmp.ALL_DLN != null && tmp.ALL_DLN.Value > 0)
			{
				//可接受日=效期(畫面上Valid_date)-允收天數(F1903.all_dln)，不可小於系統日(sysdate)
				var all_dln = tmp.ALL_DLN.Value;
				if (ValidDate?.AddDays(-all_dln) < System.DateTime.Today)
				{
					msg = Properties.Resources.P0202030100_AllDlnInsufficient;
					return msg;
				}
			}

			return msg;
		}

		#endregion Save
		#region Check Serial No
		ICommand _checkSerialNoCommand;
		public ICommand CheckSerialNoCommand
		{
			get
			{
				return _checkSerialNoCommand ?? (_checkSerialNoCommand = CreateBusyAsyncCommand(
					o => DoCheckSerialNo(),
					() => !string.IsNullOrWhiteSpace(NewSerialNo) && BaseData != null,
					o => ActionAfterCheckSerialNo() // Focus到新項目必須在Command完成後才做
				));
			}
		}
		#endregion
		#region 序號刷讀作業

		/// <summary>
		/// 刷讀後判斷該序號在F2501裡的狀態
		/// 只要Status非空即顯示其訊息
		/// </summary>
		/// <returns></returns>
		public void DoCheckSerialNo()
		{
			var wcfP020203Data = BaseData.Map<P020203Data, wcf.P020203Data>();

			var proxy = new wcf.P02WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(
							proxy.InnerChannel,
							() => proxy.RandomCheckSerial(wcfP020203Data, RtNo, NewSerialNo));

			if (!result.IsSuccessed)
			{
				ShowResultMessage(result);
				return;
			}

			var list = (DgSerialList ?? new ObservableCollection<exShare.SerialNoResult>()).ToList();
			var serialNoResult = GetLastSerialLog(NewSerialNo);
			if (serialNoResult != null)
			{
				list.Add(serialNoResult);
			}
			DgSerialList = list.ToObservableCollection();
			DoRefreshReadCount();
		}

		SerialNoResult GetLastSerialLog(string serial)
		{
			var proxy = GetProxy<F02Entities>();
			var query = from item in proxy.F02020104s
									where item.PURCHASE_NO == BaseData.PURCHASE_NO
									where item.PURCHASE_SEQ == BaseData.PURCHASE_SEQ
									where item.RT_NO == BaseData.RT_NO
									where item.DC_CODE == BaseData.DC_CODE
									where item.GUP_CODE == BaseData.GUP_CODE
									where item.CUST_CODE == BaseData.CUST_CODE
									where item.ITEM_CODE == BaseData.ITEM_CODE
									where item.SERIAL_NO == serial
									orderby item.LOG_SEQ descending
									select item;

			return query.ToList()
						.Select(item => new SerialNoResult
						{
							SerialNo = item.SERIAL_NO,
							Checked = item.ISPASS == "1",
							Message = item.ISPASS == "1" ? string.Empty : Properties.Resources.P0202030100_QuerySerialIsNull
						})
						.FirstOrDefault();
		}

		/// <summary>
		/// 刷讀後更新統計數
		/// </summary>
		private void DoRefreshReadCount()
		{
			SerialCount.ValidCount = DgSerialList.Count(x => x.Checked);
			SerialCount.InvalidCount = DgSerialList.Count(x => !x.Checked);
			SerialCount.CurrentCount = DgSerialList.Count();
			// 每讀一次就從資料庫讀一次實數總數
			var proxy = GetProxy<F02Entities>();
			var query = from item in proxy.F02020104s
									where item.PURCHASE_NO == BaseData.PURCHASE_NO
									where item.PURCHASE_SEQ == BaseData.PURCHASE_SEQ
									where item.RT_NO == BaseData.RT_NO
									where item.DC_CODE == BaseData.DC_CODE
									where item.GUP_CODE == BaseData.GUP_CODE
									where item.CUST_CODE == BaseData.CUST_CODE
									where item.ITEM_CODE == BaseData.ITEM_CODE
									where item.ISPASS == "1"
									select item;
			SerialCount.TotalValidCount = query.Count();
			RaisePropertyChanged("SerialCount");
		}

		#endregion
		#region 商品圖檔上傳

		private bool _isUploadOk = false;
		public ICommand UploadCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSaveImage(),
					() =>
					{
						// 如果ISUPLOAD = 0, 表示未上傳
						return UserOperateMode == OperateMode.Edit && !IsImageUploaded;
					},
					o =>
					{
						if (_isUploadOk)
						{
							ItemImageSource = FileService.GetItemImage(_gupCode, _custCode, BaseData.ITEM_CODE);
						}
					}
				);
			}
		}
		/// <summary>
		/// 圖檔上傳, 並回傳檔案名稱 (ITEMCODE + NO)
		/// 圖檔上傳流程是先選擇檔案, 然後寫入資料庫, 再複製檔案到目的地
		/// </summary>
		/// <returns></returns>
		public void DoSaveImage()
		{
			_isUploadOk = false;
			var proxyF19 = GetProxy<F19Entities>();
			var proxyF02 = GetProxy<F02Entities>();
			var message = string.Empty;
			if (FileService.UpLoadItemImage(_gupCode, _custCode, BaseData.ITEM_CODE, FileName, out message))
			{
				// 更新F02020101的檔案上傳狀態
				var tmp2 = proxyF02.F02020101s.Where(x => x.DC_CODE == SelectedDc && x.GUP_CODE == _gupCode && x.CUST_CODE == _custCode
					&& x.PURCHASE_NO == BaseData.PURCHASE_NO && x.ITEM_CODE == BaseData.ITEM_CODE).ToList();
				tmp2.ForEach(x =>
				{
					x.ISUPLOAD = "1";
					proxyF02.UpdateObject(x);
				});

				// 更新F020201的檔案上傳狀態
				var tmp3 = proxyF02.F020201s.Where(x => x.GUP_CODE == _gupCode && x.ITEM_CODE == BaseData.ITEM_CODE).ToList();
				tmp3.ForEach(x =>
				{
					x.ISUPLOAD = "1";
					proxyF02.UpdateObject(x);
				});
				proxyF02.SaveChanges();
				FileName = string.Empty;
				BaseData.ISUPLOAD = "1";
				RefreshImage();
				_isUploadOk = true;
			}
			else
			{
				ShowWarningMessage(message);
				BaseData.ISUPLOAD = "0";
			}
		}

		/// <summary>
		/// 上傳後, 或是載入時, 更新圖檔的顯示
		/// </summary>
		public void RefreshImage()
		{

			RaisePropertyChanged("IsImageUploaded");
			RaisePropertyChanged("ImagePathForDisplay");
			RaisePropertyChanged("ItemImageSource");
		}
    #endregion
    #region 效期驗證&資料轉換
    public ICommand ConvertToValidDateCommand
    {
      get
      {
        return CreateBusyAsyncCommand(o => { }, () => true, o =>
        {
          DoConvertToValidDate();
        });
      }
    }

    public Boolean DoConvertToValidDate(Boolean IsSaveAction = false)
    {
      if (string.IsNullOrWhiteSpace(ValidDateStr))
      {
        ValidDate = null;
        return true;
      }

      //驗證是否為8碼的數字 ex:20230101
      if (!Regex.IsMatch(ValidDateStr, @"\d{8}") && !Regex.IsMatch(ValidDateStr, @"\d{4}/\d{2}/\d{2}"))
      {
        ShowWarningMessage("效期不符合日期格式(範例：2023/01/01、20230101)");
        return false;
      }
      if (Regex.IsMatch(ValidDateStr, @"\d{8}"))
      {
        int year, month, day;
        if (int.TryParse(ValidDateStr.Substring(0, 4), out year) && int.TryParse(ValidDateStr.Substring(4, 2), out month) && int.TryParse(ValidDateStr.Substring(6, 2), out day))
        {
          DateTime tmpDate;
          if (DateTime.TryParse($"{year}/{month}/{day}", out tmpDate))
          {
            if (IsSaveAction)
              _validDate = tmpDate;
            else
              ValidDate = tmpDate;
          }
          else
          {
            ShowWarningMessage("效期不符合日期格式(範例：2023/01/01、20230101)");
          }
        }
      }
      else if (Regex.IsMatch(ValidDateStr, @"\d{4}/\d{2}/\d{2}"))
      {
        DateTime tmpDate;
        if (DateTime.TryParse(ValidDateStr, out tmpDate))
        {
          if (IsSaveAction)
            _validDate = tmpDate;
          else
            ValidDate = tmpDate;
        }
        else
        {
          ShowWarningMessage("效期不符合日期格式(範例：2023/01/01、20230101)");
        }
      }
      return true;
    }
    #endregion 效期驗證&資料轉換
    #endregion

    #region 序號刷讀資料結構
    public class SerialStatistic
		{
			public int TotalValidCount { get; set; }
			public int CurrentCount { get; set; }
			public int ValidCount { get; set; }
			public int InvalidCount { get; set; }
		}
		#endregion


		#region 國際條碼一重複
		public void LostFocusEanCode1()
		{
			if (StringHelper.CheckStringIncludeWhiteSpace(EanCode1))
			{
				DialogService.ShowMessage(string.Format("刷入的國際條碼一[{0}]中有空白，請重新刷入", EanCode1));
			}
			//if (!string.IsNullOrWhiteSpace(EanCode1) && (EanCode1 == EanCode2 || EanCode1 == EanCode3))
			//{
			//	EanCode1 = BaseData.EAN_CODE1;
			//	DialogService.ShowMessage(Properties.Resources.P0202030100_RepeatEanCode);
			//}
		}
		#endregion
		#region 國際條碼二重複
		public void LostFocusEanCode2()
		{
			if (StringHelper.CheckStringIncludeWhiteSpace(EanCode2))
			{
				DialogService.ShowMessage(string.Format("刷入的國際條碼二[{0}]中有空白，請重新刷入", EanCode2));
			}
			//if (!string.IsNullOrWhiteSpace(EanCode2) && (EanCode2 == EanCode1 || EanCode2 == EanCode3))
			//{
			//	EanCode2 = BaseData.EAN_CODE2;
			//	DialogService.ShowMessage(Properties.Resources.P0202030100_RepeatEanCode);
			//}
		}
		#endregion
		#region 國際條碼三重複
		public void LostFocusEanCode3()
		{
			if (StringHelper.CheckStringIncludeWhiteSpace(EanCode3))
			{
				DialogService.ShowMessage(string.Format("刷入的國際條碼三[{0}]中有空白，請重新刷入", EanCode3));
			}
			//if (!string.IsNullOrWhiteSpace(EanCode3) && (EanCode3 == EanCode2 || EanCode3 == EanCode1))
			//{
			//	EanCode3 = BaseData.EAN_CODE3;
			//	DialogService.ShowMessage(Properties.Resources.P0202030100_RepeatEanCode);
			//}
		}
		#endregion

		#region 商品條碼檢驗

		#region 商品條碼檢驗Textbox
		private string _CheckEANCode;
		/// <summary>
		/// 商品條碼檢驗Textbox
		/// </summary>
		public string CheckEANCode
		{
			get { return _CheckEANCode; }
			set { _CheckEANCode = value; RaisePropertyChanged(); }
		}
		#endregion 商品條碼檢驗Textbox

		#region 商品條碼檢驗Command


		public ICommand CheckEANCodeIsMatchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
								(o) => { },
								() => true,
								(o) => CheckEANCodeIsMatch(false)
						);
			}
		}

		public Boolean CheckEANCodeIsMatch(Boolean DontShowOKMessage = true)
		{
			Boolean CheckEanCodeExist = false;
			if (!String.IsNullOrWhiteSpace(EanCode1))
				CheckEanCodeExist |= CheckEANCode == EanCode1;
			if (!String.IsNullOrWhiteSpace(EanCode2))
				CheckEanCodeExist |= CheckEANCode == EanCode2;
			if (!String.IsNullOrWhiteSpace(EanCode3))
				CheckEanCodeExist |= CheckEANCode == EanCode3;

			if (StringHelper.CheckStringIncludeWhiteSpace(CheckEANCode))
			{
				DialogService.ShowMessage(string.Format("刷入的條碼檢驗[{0}]中有空白，請重新刷入", CheckEANCode));
				CheckEANCode = string.Empty;
				return false;
			}

			if (String.IsNullOrWhiteSpace(CheckEANCode))
			{
				ShowWarningMessage("請刷入該商品的條碼");
				return false;
			}
			if (!CheckEanCodeExist)
			{
				ShowWarningMessage("刷入的條碼不屬於此商品，不能繼續驗收");
				return false;
			}
			if (!DontShowOKMessage)
				ShowInfoMessage("商品條碼檢核通過");
			return true;
		}


		private RelayCommand<object> _keyEnterCommand;

		public ICommand KeyEnterCommand
		{
			get
			{
				if (_keyEnterCommand == null)
				{
					_keyEnterCommand = new RelayCommand<object>(ExecuteKeyEnterCommand);
				}

				return _keyEnterCommand;
			}
		}

		public void ExecuteKeyEnterCommand(object sender)
		{
			ShowInfoMessage("aa");
		}
		#endregion 商品條碼檢驗Command

		#endregion 商品條碼檢驗

		/// <summary>
		/// 判斷保存天數是否有異動用
		/// </summary>
		public Boolean SaveDayIsCheange = false;

		/// <summary>
		/// 是否為首次驗收 and 有設定警示天數 and 有設定警示天數
		/// </summary>
		public Boolean IsFirstInDate
		{
			get { return !FirstInDate.HasValue && (_baseDataOriginal?.ALL_SHP ?? 0) != 0 && ((_baseDataOriginal?.ALL_DLN ?? 0) != 0); }
		}

		#region 保存天數變更事件
		public ICommand CheckSaveDayCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
						o => CheckSaveDay());
			}
		}

		// 保存天數變更時，進行檢核
		public void CheckSaveDay()
		{
			// 如果為效期商品，才進行計算允收天數與警示天數
			if (NeedExpired == "1")
			{
				if (_baseDataOriginal.ALL_DLN.HasValue && _baseDataOriginal.ALL_DLN.Value > 0 && _baseDataOriginal.ALL_SHP.HasValue && _baseDataOriginal.ALL_SHP > 0)
					ShowInfoMessage("本商品的允收天數與警示天數已由LMS設定，不會因總保存天數的調整而重新計算");
				else
				{
					if(SaveDay>30)
					{
						var proxy = new wcfShare.SharedWcfServiceClient();
						var result = RunWcfMethod<wcfShare.GetItemAllDlnAndAllShpRes>(
						proxy.InnerChannel,
						() => proxy.GetItemAllDlnAndAllShp(SaveDay));
						BaseData.ALL_DLN = (short)result.ALL_DLN;
						BaseData.ALL_SHP = result.ALL_SHP;
					}
					else
					{
						BaseData.ALL_DLN = null;
						BaseData.ALL_SHP = null;
					}
					ShowSaveDayError = SaveDay > 30 ? "0" : "1";
				}
			}
			else
				ShowSaveDayError = "0";
		}
		#endregion


		/// <summary>
		/// 檢查有效日期 IsSuccess=false一定有Message，IsSuccess=true，可能也會有Message
		/// </summary>
		/// <param name="validDate"></param>
		/// <returns></returns>
		private ExecuteResult CheckValidDate(DateTime? validDate)
    {
      if (IsFirstLoad)
        return new ExecuteResult { IsSuccessed = true };
      // 效期商品
      if (NeedExpired == "1")
      {
        var maxValidDate = DateTime.Today.AddDays(SaveDay ?? 0);

        // 保存天數未設定或是保存天數設定小於等於30 顯示訊息
        if (!SaveDay.HasValue || SaveDay <= 30)
          return new ExecuteResult { IsSuccessed = false, Message = "效期商品，保存天數必需設定且必需設定大於30" };
        else if (!validDate.HasValue)
          return new ExecuteResult() { IsSuccessed = false, Message = "請輸入效期" };
        else if (validDate <= DateTime.Now.Date)
          return new ExecuteResult() { IsSuccessed = false, Message = "效期必須大於今日" };
        else if (validDate.Value.AddDays(-(double)(BaseData.ALL_DLN ?? 0)) < DateTime.Today && BaseData.RECV_QTY != BaseData.DEFECT_QTY)
          return new ExecuteResult()
          {
            IsSuccessed = false,
            Message = string.Format("該商品效期必須大於允收日期{0}，不符合正常商品允收規則，請利用設定不良品驗退",
                DateTime.Today.AddDays(BaseData.ALL_DLN.Value).ToString("yyyy/MM/dd"))
          };
        else if (validDate.Value > maxValidDate)
          return new ExecuteResult() { IsSuccessed = false, Message = $"本商品的總保存天數為{SaveDay.Value}，商品效期不可以大於{maxValidDate.ToString("yyyy/MM/dd")}" };
      }
      else
      {
        if (validDate != new DateTime(9999, 12, 31))
          return new ExecuteResult { IsSuccessed = false, Message = "非效期商品，不可調整效期，請設定為9999/12/31" };
			}
      return new ExecuteResult() { IsSuccessed = true };
    }
  }

}
