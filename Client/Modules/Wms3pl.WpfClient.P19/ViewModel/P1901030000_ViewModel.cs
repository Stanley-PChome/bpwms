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
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.UILib.Services;
using AutoMapper;
using System.Reflection;
using Wms3pl.WpfClient.DataServices.F91DataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.Services;
using System.Collections.ObjectModel;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1901030000_ViewModel : InputViewModelBase
	{
		//public Action OpenSearchWin = delegate { };


		#region property,field

		public string _gupCode;
		public string _custCode;

		#region 查詢商品
		/// <summary>
		/// 查詢之商品編號
		/// </summary>
		private string _searchItemCode = string.Empty;
		public string SearchItemCode { get { return _searchItemCode; } set { _searchItemCode = value; RaisePropertyChanged("SearchItemCode"); } }

		private string _searchItemName = string.Empty;
		public string SearchItemName { get { return _searchItemName; } set { _searchItemName = value; RaisePropertyChanged("SearchItemName"); } }

		#endregion

		#region 商品清單
		/// <summary>
		/// 商品主檔清單
		/// </summary>
		private List<F1903> _records;
		public List<F1903> Records { get { return _records; } set { _records = value; RaisePropertyChanged(); } }

		private F1903 _selectedData;
		public F1903 SelectedData
		{
			get { return _selectedData; }
			set
			{
				_selectedData = value;
				if (_selectedData != null)
					SearchItemCommand.Execute(null);
				RaisePropertyChanged();
			}
		}

		#endregion

		#region 商品材積階層清單
		/// <summary>
		/// 商品材積階層清單
		/// </summary>
		private List<F190301Data> _itemList;
		public List<F190301Data> ItemList { get { return _itemList; } set { _itemList = value; RaisePropertyChanged("ItemList"); } }

		private List<F190301Data> _oriItemList;
		public List<F190301Data> OriItemList { get { return _oriItemList; } set { _oriItemList = value; RaisePropertyChanged("OriItemList"); } }

		private F190301Data _selectItem;
		public F190301Data SelectItem
		{
			get { return _selectItem; }
			set
			{
				_selectItem = value;
				//SetData(value);
				RaisePropertyChanged("SelectItem");
			}
		}
		#endregion

		#region 階層選單
		private List<NameValuePair<string>> _selectedUnitLevelList;

		public List<NameValuePair<string>> SelectedUnitLevelList
		{
			get { return _selectedUnitLevelList; }
			set
			{
				_selectedUnitLevelList = value;
				RaisePropertyChanged();
			}
		}


		private string _selectedUnitLevel;

		public string SelectedUnitLevel
		{
			get { return _selectedUnitLevel; }
			set
			{
				_selectedUnitLevel = value;
				RaisePropertyChanged();
			}
		}
        #endregion

        #region 系統單位選單
        /// <summary>
		///  系統單位代號下拉清單
		/// </summary>
		private ObservableCollection<NameValuePair<string>> _sysUnitList = null;
        public ObservableCollection<NameValuePair<string>> SysUnitList
        {
            get
            {
                if (_sysUnitList == null)
                    _sysUnitList = new ObservableCollection<NameValuePair<string>>();
                return _sysUnitList;
            }
            set
            {
                this._sysUnitList = value;
                RaisePropertyChanged("SysUnitList");
            }
        }
        #endregion

        #region 系統單位
        private List<NameValuePair<string>> _selectedSysUnitList;

        public List<NameValuePair<string>> SelectedSysUnitList
        {
            get { return _selectedSysUnitList; }
            set
            {
                _selectedSysUnitList = value;
                RaisePropertyChanged();
            }
        }


        private string _selectedSysUnit;

        public string SelectedSysUnit
        {
            get { return _selectedSysUnit; }
            set
            {
                _selectedSysUnit = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region 商品材積資料檔
        /// <summary>
        /// 商品材積清單
        /// </summary>
        private List<F1905> _volumes;
		public List<F1905> Volumes { get { return _volumes; } set { _volumes = value; } }
		/// <summary>
		/// 商品材積
		/// </summary>
		private F1905 _selectedVolume;
		public F1905 SelectedVolume
		{
			get { return _selectedVolume; }
			set { _selectedVolume = value; RaisePropertyChanged("SelectedVolume"); }
		}
		#endregion

		#endregion

		public P1901030000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
				//SetData(GetF1905Datas(string.Empty, string.Empty, string.Empty));
			}
		}

		public void SetSelectedUnitLevelList()
		{
			var proxy = GetProxy<F91Entities>();
			SelectedUnitLevelList = proxy.F91000302s.Where(x => x.ITEM_TYPE_ID == "001")
										.Select(x => new NameValuePair<string>(x.ACC_UNIT_NAME, x.ACC_UNIT))
										.ToList();
			if (SelectedUnitLevelList != null && SelectedUnitLevelList.Any())
				SelectedUnitLevel = SelectedUnitLevelList.FirstOrDefault().Value;
		}
		private void InitControls()
		{
			_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
			SetSelectedUnitLevelList();
            SetSelectedSysUnitList();
        }

        public void SetSelectedSysUnitList()
        {
            SelectedSysUnitList = GetBaseTableService.GetF000904List(FunctionCode, "F190301", "SYS_UNIT")
                                                     .Select(x => new NameValuePair<string>(x.Name, x.Value))
                                                     .ToList();
            SelectedSysUnitList.Insert(0, new NameValuePair<string>());
            
            if (SelectedSysUnitList != null && SelectedSysUnitList.Any())
                SelectedSysUnit = SelectedSysUnitList.FirstOrDefault().Value;
        }

        public F190301Data GetF190301Datas(string gupCode, string itemCode)
		{
			//執行查詢動
			_gupCode = (string.IsNullOrEmpty(gupCode)) ? Wms3plSession.Get<GlobalInfo>().GupCode : gupCode;
			var proxyEx = GetExProxy<P19ExDataSource>();
			var results = proxyEx.CreateQuery<F190301Data>("GetItemPack")
				.AddQueryOption("gupCode", string.Format("'{0}'", _gupCode))
				.AddQueryOption("itemCode", string.Format("'{0}'", itemCode))
				.ToList();
			if (results == null || !results.Any())
			{
				ShowMessage(Messages.InfoNoData);
				return null;
			}
			return results.FirstOrDefault();
		}



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
			if (Records != null && Records.Any())
				SelectedData = Records.FirstOrDefault();
			else
				ClearItem();
		}

		private void ClearItem()
		{
			SelectedData = null;
			ItemList = null;
			SelectItem = null;
		}

		private void DoSearch()
		{
			//執行查詢動
			//檢核是否至少有輸入一查詢條件
			if (string.IsNullOrEmpty(SearchItemCode) && string.IsNullOrEmpty(SearchItemName))
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P1901030000_NeedOneCondition, Title = Resources.Resources.Information });
				return;
			}

			var proxy = GetProxy<F19Entities>();
			Records = proxy.CreateQuery<F1903>("GetF1912s")
							.AddQueryExOption("gupCode", _gupCode)
							.AddQueryExOption("custCode", _custCode)
							.AddQueryExOption("itemCode", SearchItemCode)
							.AddQueryExOption("itemName", SearchItemName)
							.ToList();

			if (Records == null || !Records.Any())
			{
				ShowMessage(Messages.InfoNoData);
				return;
			}
		}


		#endregion Search

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
			if (SelectedData == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P1901030000_ChooseItem, Title = Resources.Resources.Information });
				return;
			}

			ItemList = GetListItems();
			if (ItemList == null || !ItemList.Any())
			{
				OriItemList = null;
				return;
			}
			OriItemList = ItemList.Select(x => AutoMapper.Mapper.DynamicMap<F190301Data>(x)).ToList();
		}

		private void DoSearchItemComplete()
		{
			if (ItemList != null && ItemList.Any())
				SelectItem = ItemList.FirstOrDefault();
		}

		private List<F190301Data> GetListItems()
		{
			var proxyEx = GetExProxy<P19ExDataSource>();
			var results = proxyEx.CreateQuery<F190301Data>("GetItemPack")
				.AddQueryOption("gupCode", string.Format("'{0}'", _gupCode))
				.AddQueryOption("custCode", string.Format("'{0}'", _custCode))
        .AddQueryOption("itemCode", string.Format("'{0}'", SelectedData.ITEM_CODE))
				.ToList();
			return results;
		}
		#endregion SearchItem

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(), () => UserOperateMode == OperateMode.Query && SelectedData != null,
					o => DoAddCompleted()
					);
			}
		}

		private void DoAddCompleted()
		{
			if (ItemList != null && ItemList.Any())
				SelectItem = ItemList.Last();
		}

		private void DoAdd()
		{
			if (SelectedData == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P1901030000_ChooseItem, Title = Resources.Resources.Information });
				return;
			}

			var proxy = GetProxy<F19Entities>();

			UserOperateMode = OperateMode.Add;
			//執行新增動作

			var tmpItemList = GetListItems();
			if (tmpItemList == null)
				tmpItemList = new List<F190301Data>();

			F190301Data newItem = new F190301Data();
			newItem.ITEM_CODE = SelectedData.ITEM_CODE;
			newItem.ITEM_NAME = SelectedData.ITEM_NAME;
			newItem.GUP_CODE = SelectedData.GUP_CODE;
            newItem.CUST_CODE = SelectedData.CUST_CODE;
            //如果查詢的商品F190301無資料，則點選新增時，自動帶此商品F1905的商品長寬高重量設定當作新增資料的預設值
            if (ItemList == null || !ItemList.Any())
			{
				var f1905 = proxy.F1905s.Where(x => x.GUP_CODE == SelectedData.GUP_CODE && x.CUST_CODE == SelectedData.CUST_CODE &&
																						x.ITEM_CODE == SelectedData.ITEM_CODE).FirstOrDefault();
				if (f1905 != null)
				{
					newItem.LENGTH = f1905.PACK_LENGTH;
					newItem.WIDTH = f1905.PACK_WIDTH;
					newItem.HIGHT = f1905.PACK_HIGHT;
					newItem.WEIGHT = f1905.PACK_WEIGHT;
				}
			}
			tmpItemList.Add(newItem);
			ItemList = tmpItemList;
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query && (ItemList != null && ItemList.Any())
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
					o => DoCancel(), () => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
			var tmpItem = (OriItemList != null) ? OriItemList.Select(x => AutoMapper.Mapper.DynamicMap<F190301Data>(x)).ToList() : null;
			ItemList = tmpItem;
			if (SelectItem == null && ItemList != null)
				SelectItem = ItemList.FirstOrDefault();
			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				var isSuccess = false;
				return CreateBusyAsyncCommand(
					o => isSuccess = DoDelete(), () => UserOperateMode == OperateMode.Query
						&& (ItemList != null && ItemList.Any()) && SelectItem != null,
					o => DoSaveComplete(isSuccess)
					);
			}
		}

		private bool DoDelete()
		{
			//執行刪除動作
			if (SelectItem == null)
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P1901030000_ChooseDeltePackSet, Title = Resources.Resources.Information });
				return false;
			}

			if (ShowMessage(Messages.WarningBeforeDelete) != DialogResponse.Yes) return false;

			// 刪除
			var proxy = GetProxy<F19Entities>();
			var f190301 = proxy.F190301s.Where(x => x.GUP_CODE == SelectItem.GUP_CODE &&
																							x.ITEM_CODE == SelectItem.ITEM_CODE &&
																							x.UNIT_ID == SelectItem.UNIT_ID).FirstOrDefault();
			if (f190301 == null)
			{
				ShowMessage(Messages.WarningCannotDelete);
				return false;
			}

			proxy.DeleteObject(f190301);
			proxy.SaveChanges();
			ShowMessage(Messages.Success);
			return true;
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				var isSuccess = false;
				return CreateBusyAsyncCommand(
					o => isSuccess = DoSave(), () => UserOperateMode != OperateMode.Query && (ItemList != null && ItemList.Any()),
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
			//執行確認儲存動作
			if (ShowMessage(Messages.WarningBeforeSave) != DialogResponse.Yes) return false;
			// 0.檢查必填欄位
			if (!isValid()) return false;

			// 1.分出該新增,更新的清單
			var delItems = GetDelItems();
			var addItems = GetAddItems();
			var editItems = GetEditItems();

			// 2.存檔
			bool isSuccess = false;
			wcf.ExecuteResult result = new wcf.ExecuteResult() { IsSuccessed = true };
			var proxy = new wcf.P19WcfServiceClient();
			result = RunWcfMethod<wcf.ExecuteResult>(
					proxy.InnerChannel,
					() => proxy.SaveItemPack(delItems, addItems, editItems));

			isSuccess = (result.IsSuccessed);

            // 3.顯示成功訊息
            if (isSuccess)
            {
                ShowMessage(Messages.Success);
                UserOperateMode = OperateMode.Query;
            }
            else
            {
                ShowResultMessage(isSuccess, result.Message);
            }
			
			return isSuccess;
		}

		private wcf.F190301[] GetEditItems()
		{
			if (OriItemList == null) return null;
			var editItems = new List<F190301Data>();
			var intersectedUnitIds = ItemList.Select(x => x.UNIT_ID).Intersect(OriItemList.Select(x => x.UNIT_ID)).ToList();
			var newItems = ItemList.Where(x => intersectedUnitIds.Contains(x.UNIT_ID)).ToList();
			foreach (var item in newItems)
			{
				var editItem = isModify(OriItemList.Where(x => x.UNIT_ID == item.UNIT_ID).FirstOrDefault(), item);
				if (editItem == null) continue;
				editItems.Add(editItem);
			}
			var results = ExDataMapper.MapCollection<F190301Data, wcf.F190301>(editItems).ToArray();
			return results;
		}

		private wcf.F190301[] GetAddItems()
		{
			var addUnitIds = new List<string>();
			if (OriItemList != null)
				addUnitIds = ItemList.Select(x => x.UNIT_ID).Except(OriItemList.Select(x => x.UNIT_ID)).ToList();
			else
				addUnitIds = ItemList.Select(x => x.UNIT_ID).ToList();
			var addItems = ItemList.Where(x => addUnitIds.Contains(x.UNIT_ID)).ToList();
			var results = ExDataMapper.MapCollection<F190301Data, wcf.F190301>(addItems).ToArray();
			return results;
		}

		private wcf.F190301[] GetDelItems()
		{
			if (OriItemList == null) return null;
			var delUnitIds = OriItemList.Select(x => x.UNIT_ID).Except(ItemList.Select(x => x.UNIT_ID)).ToList();
			var delItems = OriItemList.Where(x => delUnitIds.Contains(x.UNIT_ID)).ToList();
			var results = ExDataMapper.MapCollection<F190301Data, wcf.F190301>(delItems).ToArray();
			return results;
		}


		private F190301Data isModify(F190301Data OriRecord, F190301Data NewRecord)
		{
			var props = NewRecord.GetType().GetProperties();
			foreach (PropertyInfo prop in props)
			{
				if (prop.Name == "Item") continue;
				object propValue = prop.GetValue(NewRecord, null);
				object orgPropValue = prop.GetValue(OriRecord, null);
				if ((propValue != null) && propValue.Equals(orgPropValue)) continue;
				if ((propValue == null) && propValue == orgPropValue) continue;
				return NewRecord;
			}
			return null;
		}

		private bool isValid()
		{

			#region 檢核必填
			if (ItemList.Where(x => x.UNIT_ID == null || x.UNIT_LEVEL == null || x.UNIT_QTY == null).Any())
			{
				ShowMessage(new MessagesStruct() { Message = Properties.Resources.P1901030000_Hierarchy, Title = Resources.Resources.Information });
				return false;
			}

            //驗證商品長、寬、高、單位數字要>0
			if (ItemList.Where(o => o.LENGTH <= 0 || o.HIGHT <= 0 || o.WIDTH <= 0 || o.UNIT_QTY <= 0).Any())
			{
				ShowWarningMessage(Properties.Resources.P1901030000_ZeroError);
				return false;
			}

            //長、寬、高不能超過的判斷
            if (ItemList.Where(o => !isValidNumberMax(o.LENGTH, 8, 2) || !isValidNumberMax(o.HIGHT, 8, 2) || !isValidNumberMax(o.WIDTH, 8, 2)).Any())
            {
                ShowWarningMessage(Properties.Resources.P1901030000_SizeError);
                return false;
            }

           

            var duplicateKeys = ItemList.Select(x => x.UNIT_LEVEL).GroupBy(x => x)
				.Where(group => group.Count() > 1).Select(group => group.Key);

			if (duplicateKeys.Any())
			{
				ShowMessage(new MessagesStruct() { Message = string.Format(Properties.Resources.P1901030000_HierarchyRepeat, string.Join(",", duplicateKeys.ToArray())), Title = Resources.Resources.Information });
				return false;
			}

			var unitIdRepeatQuery = ItemList.GroupBy(x => x.UNIT_ID).Where(g => g.Count() > 1).Select(g => g.Key);
			if (unitIdRepeatQuery.Any())
			{
				var unitNames = unitIdRepeatQuery.Select(x => SelectedUnitLevelList.Where(item => item.Value == x).Select(item => item.Name).FirstOrDefault());
				ShowWarningMessage(string.Format(Properties.Resources.P1901030000_UnitRepeat, string.Join(",", unitNames)));
				return false;
			}
			#endregion

			#region 檢核格式
			/*
			MessagesStruct WarningOutOfNum;
			string msgTitle = string.Empty;
			decimal num1 = 1, num2 = 999999;
			//外箱體積長度、寬度及收縮體積長度、寬度、高度
			decimal packLength = 0, packWidth = 0, packHight = 0, caseLength = 0, caseWidth = 0, caseHight = 0;
			Decimal.TryParse(CurrentRecord.PACK_LENGTH.ToString(), out packLength);
			Decimal.TryParse(CurrentRecord.PACK_WIDTH.ToString(), out packWidth);
			Decimal.TryParse(CurrentRecord.PACK_HIGHT.ToString(), out packHight);

			if (!isValidFormate(packLength, num1, num2) || !isValidFormate(packWidth, num1, num2) || 
					!isValidFormate(packHight, num1, num2))
			{
				msgTitle = Properties.Resources.P1901030000_Volume;
				WarningOutOfNum = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = string.Format(Properties.Resources.P1901030000_Range_Error_Message, msgTitle, num1, num2), Title = Resources.Resources.Information };
				ShowMessage(WarningOutOfNum);
				return false;
			}
			
			//外箱重量及收縮重量
			double dNum1 = 0.0, dNum2 = 999999.9;
			double packWeight = -1.0;
			Double.TryParse(CurrentRecord.PACK_WEIGHT.ToString(), out packWeight);
			if (!isValidFormate(packWeight, dNum1, dNum2))
			{
				msgTitle = Properties.Resources.PACK_WEIGHT;
				WarningOutOfNum = new MessagesStruct() { Button = DialogButton.OK, Image = DialogImage.Warning, Message = string.Format(Properties.Resources.P1901030000_Range_Error_Message, msgTitle, dNum1, dNum2), Title = Resources.Resources.Information };
				ShowMessage(WarningOutOfNum);
				return false;
			}
			*/
			#endregion

			return true;
		}

        /// <summary>
        /// 判斷數字是否超過最大值
        /// </summary>
        /// <param name="num">數字</param>
        /// <param name="maxNum">最大位數</param>
        /// <param name="decimalNum">最大小數位數</param>
        /// <returns></returns>
        private bool isValidNumberMax(decimal? num,int maxNum,int decimalNum)
        {
            bool result = true;

            string[] tempNumArr = num.ToString().ToSplit('.');

            //判斷是否有限制小數位數
            if (decimalNum > 0)
            {
                //判斷幾個小數點
                if (tempNumArr.Count() == 2)
                {
                    //判斷整數
                    if (tempNumArr[0].Length > maxNum - decimalNum)
                    {
                        result = false;
                    }

                    //判斷小數
                    if (tempNumArr[1].Length > decimalNum)
                    {
                        result = false;
                    }
                }
                else if (tempNumArr.Count() == 1)
                {
                    if (tempNumArr[0].Length > maxNum - decimalNum)
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
            }
            else if (decimalNum == 0)
            {
                if (tempNumArr[0].Length > maxNum)
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        private bool isValidFormate(decimal inputValue, decimal num1, decimal num2)
		{
			return (inputValue >= num1 && inputValue <= num2);
		}
		private bool isValidFormate(double inputValue, double num1, double num2)
		{
			return (inputValue >= num1 && inputValue <= num2);
		}
		#endregion Save
	}
}
