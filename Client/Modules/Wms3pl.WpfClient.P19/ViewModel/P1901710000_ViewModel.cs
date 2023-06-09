using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
using Wms3pl.WpfClient.UILib;
using wcf = Wms3pl.WpfClient.ExDataServices.P19WcfService;

namespace Wms3pl.WpfClient.P19.ViewModel
{
	public partial class P1901710000_ViewModel : InputViewModelBase
	{
		public P1901710000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料							
				SetDcList();
			}
		}


		#region 物流中心
		// 物流中心清單
		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				Set(() => DcList, ref _dcList, value);
				if (_dcList != null && _dcList.Any())
				{
					SelectedDcCode = _dcList.First().Value;
					SearchCommand.Execute(null);
				}
			}
		}

		//選擇的物流中心
		private string _selectedDcCode;

		public string SelectedDcCode
		{
			get { return _selectedDcCode; }
			set
			{ Set(() => SelectedDcCode, ref _selectedDcCode, value); }
		}


		#endregion

		#region 下拉選單資料繫結
		private void SetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			DcList = data;
			if (data.Any())
				SelectedDcCode = DcList.First().Value;
		}
		#endregion

		#region  全選/取消全選

		private bool _isCheckAllAllocationStockSchedule;

		public bool IsCheckAllAllocationStockSchedule
		{
			get { return _isCheckAllAllocationStockSchedule; }
			set
			{
				Set(() => IsCheckAllAllocationStockSchedule, ref _isCheckAllAllocationStockSchedule, value);
				CheckAllAllocationStockSchedule(value);
			}
		}

		public void CheckAllAllocationStockSchedule(bool isChecked)
		{
			if (AllocationStockScheduleSetting != null)
				foreach (var item in AllocationStockScheduleSetting)
					item.IsSelected = isChecked;
		}

		private bool _isCheckAllBatchPickNoSchedule;

		public bool IsCheckAllBatchPickNoSchedule
		{
			get { return _isCheckAllBatchPickNoSchedule; }
			set
			{
				Set(() => IsCheckAllBatchPickNoSchedule, ref _isCheckAllBatchPickNoSchedule, value);
				CheckAllBatchPickNoSchedule(value);
			}
		}

		public void CheckAllBatchPickNoSchedule(bool isChecked)
		{
			if (PickingScheduleSetting != null)
				foreach (var item in PickingScheduleSetting)
					item.IsSelected = isChecked;
		}

    #endregion

    #region 查詢結果
    #region 周轉箱設定
    //直接把F190105Data丟到畫面上，輸入小數點會很不直覺打10.時，小數點會一直消失，因此多一層string去處理，要儲存時再丟回F190105Data

    private string _baseContainerMaxLength;
    /// <summary>
    /// 最長邊
    /// </summary>
    public string baseContainerMaxLength
    {
      get { return _baseContainerMaxLength; }
      set { Set(() => baseContainerMaxLength, ref _baseContainerMaxLength, value); }
    }

    private string _baseContainerMidLength;
    /// <summary>
    /// 次長邊
    /// </summary>
    public string baseContainerMidLength
    {
      get { return _baseContainerMidLength; }
      set { Set(() => baseContainerMidLength, ref _baseContainerMidLength, value); }
    }

    private string _baseContainerMinLength;
    /// <summary>
    /// 最短邊
    /// </summary>
    public string baseContainerMinLength
    {
      get { return _baseContainerMinLength; }
      set { Set(() => baseContainerMinLength, ref _baseContainerMinLength, value); }
    }

    private string _baseContainerVolumn;
    /// <summary>
    /// 可用容積
    /// </summary>
    public string baseContainerVolumn
    {
      get { return _baseContainerVolumn; }
      set { Set(() => baseContainerVolumn, ref _baseContainerVolumn, value); }
    }

    #endregion 周轉箱設定




    private F190105 _f190105Data = new F190105();
		public F190105 F190105Data
		{
			get { return _f190105Data; }
			set { Set(() => F190105Data, ref _f190105Data, value); }
		}

		private List<SelectionItem<F190106Data>> _allocationStockScheduleSetting = new List<SelectionItem<F190106Data>>();
		public List<SelectionItem<F190106Data>> AllocationStockScheduleSetting
		{

			get { return _allocationStockScheduleSetting; }
			set
			{
				Set(() => AllocationStockScheduleSetting, ref _allocationStockScheduleSetting, value);
			}

		}

		private List<SelectionItem<F190106Data>> _pickingScheduleSetting = new List<SelectionItem<F190106Data>>();
		public List<SelectionItem<F190106Data>> PickingScheduleSetting
		{
			get { return _pickingScheduleSetting; }
			set { Set(() => PickingScheduleSetting, ref _pickingScheduleSetting, value); }
		}

		//異動紀錄
		private List<F190106Data> _allSettingData = new List<F190106Data>();
		public List<F190106Data> AllSettingData
		{
			get { return _allSettingData; }
			set { Set(() => AllSettingData, ref _allSettingData, value); }
		}
		#endregion

		#region 配庫排程設定(開始時間)
		private string _allocationStockStartTime;
		public string AllocationStockStartTime
		{
			get { return _allocationStockStartTime; }
			set
			{
				//Regex reg = new Regex(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$");
				//Match match = reg.Match(value.ToString());

				//if (match.Success)
				//{

				//}


				Set(() => AllocationStockStartTime, ref _allocationStockStartTime, value);

			}
		}
		#endregion

		#region 配庫排程設定(結束時間)
		private string _allocationStockEndTime ;
		public string AllocationStockEndTime
		{
			get { return _allocationStockEndTime; }
			set { Set(() => AllocationStockEndTime, ref _allocationStockEndTime, value); }
		}
		#endregion

		#region 配庫排程設定(每幾分中執行一次)
		private string _allocationStockPeriod;
		public string AllocationStockPeriod
		{
			get { return _allocationStockPeriod; }
			set { Set(() => AllocationStockPeriod, ref _allocationStockPeriod, value); }
		}
		#endregion

		#region 揀貨排程設定(開始時間)
		private string _pickingStartTime;
		public string PickingStartTime
		{
			get { return _pickingStartTime; }
			set { Set(() => PickingStartTime, ref _pickingStartTime, value); }
		}
		#endregion

		#region 揀貨排程設定(結束時間)
		private string _pickingEndTime;
		public string PickingEndTime
		{
			get { return _pickingEndTime; }
			set { Set(() => PickingEndTime, ref _pickingEndTime, value); }
		}
		#endregion

		#region 揀貨排程設定(每幾分鐘執行一次)
		private string _pickingPeriod;
		public string PickingPeriod
		{
			get { return _pickingPeriod; }
			set { Set(() => PickingPeriod, ref _pickingPeriod, value); }
		}
		#endregion

		#region 所選擇頁籤

		private string _selectedIndex = "0";
		public string SelectedIndex
		{
			get { return _selectedIndex; }
			set { Set(() => SelectedIndex, ref _selectedIndex, value); }
		}
		#endregion


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
		#region
		// 編輯的暫存檔(配庫排程設定)
		private List<F190106> _addData = new List<F190106>();
		public List<F190106> AddData
		{
			get { return _addData; }
			set { Set(() => AddData, ref _addData, value); }
		}
		#endregion



		//編輯的暫存檔(揀貨排程設定)
		public List<F190106> _tempPickingScheduleSetting;

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
			var proxy = GetProxy<F19Entities>();
			F190105Data = proxy.F190105s.Where(o => o.DC_CODE == SelectedDcCode).FirstOrDefault();
			F190105Data.B2B_PDA_PICK_PECENT = F190105Data.B2B_PDA_PICK_PECENT * 100;
			F190105Data.B2C_PDA_PICK_PERCENT = F190105Data.B2C_PDA_PICK_PERCENT * 100;
      baseContainerMaxLength = F190105Data.BASE_CONTAINER_MAX_LENGTH.ToString();
      baseContainerMidLength= F190105Data.BASE_CONTAINER_MID_LENGTH.ToString();
      baseContainerMinLength = F190105Data.BASE_CONTAINER_MIN_LENGTH.ToString();
      baseContainerVolumn= F190105Data.BASE_CONTAINER_VOLUMN.ToString();

      var f190106s = proxy.F190106s.Where(o => o.DC_CODE == SelectedDcCode).ToList();
			var allocationStockScheduleSetting = f190106s.Where(x => x.SCHEDULE_TYPE == "01").Select(x => new F190106Data
			{
				ID = x.ID,
				DC_CODE = x.DC_CODE,
				SCHEDULE_TYPE = x.SCHEDULE_TYPE,
				START_TIME = x.START_TIME,
				END_TIME = x.END_TIME,
				PERIOD = x.PERIOD,
				CRT_STAFF = x.CRT_NAME,
				CRT_DATE = x.CRT_DATE,


			});
			AllocationStockScheduleSetting = allocationStockScheduleSetting.ToSelectionList().ToList();
			var pickingScheduleSetting = f190106s.Where(x => x.SCHEDULE_TYPE == "02").Select(x => new F190106Data
			{
				ID = x.ID,
				DC_CODE = x.DC_CODE,
				SCHEDULE_TYPE = x.SCHEDULE_TYPE,
				START_TIME = x.START_TIME,
				END_TIME = x.END_TIME,
				PERIOD = x.PERIOD,
				CRT_STAFF = x.CRT_NAME,
				CRT_DATE = x.CRT_DATE,

			});
			PickingScheduleSetting = pickingScheduleSetting.ToSelectionList().ToList();

			//異紀錄動
			AllSettingData = allocationStockScheduleSetting.Concat(pickingScheduleSetting).ToList();

			AllocationStockStartTime = null;
			AllocationStockEndTime = null;
			PickingStartTime = null;
			PickingEndTime = null;

		}

		#endregion

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
			DoSearch();
		}
		#endregion

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				var isSuccess = false;
				return CreateBusyAsyncCommand(
					o => isSuccess = DoSave(),
					() =>
					{
						return UserOperateMode == OperateMode.Edit &&
							   SelectedDcCode != null;
					},
					o => DoSaveComplete(isSuccess));
			}
		}

		private bool DoSave()
		{
			if (!CheckData())
			{
				return false;
			}
			var proxyWcf = new wcf.P19WcfServiceClient();
			var tempAddF190106 = AllSettingData.Where(x => x.ChangeFlag == "A").Select(x => new F190106Data
			{
				ID = x.ID,
				DC_CODE = x.DC_CODE,
				SCHEDULE_TYPE = x.SCHEDULE_TYPE,
				START_TIME = x.START_TIME,
				END_TIME = x.END_TIME,
				PERIOD = x.PERIOD,
				ChangeFlag = x.ChangeFlag
			});
			var tempDelF190106 = AllSettingData.Where(x => x.ChangeFlag == "D").Select(x => new F190106Data
			{
				ID = x.ID,
				DC_CODE = x.DC_CODE,
				SCHEDULE_TYPE = x.SCHEDULE_TYPE,
				START_TIME = x.START_TIME,
				END_TIME = x.END_TIME,
				PERIOD = x.PERIOD,
				ChangeFlag = x.ChangeFlag
			});
			var addF190106 = ExDataMapper.MapCollection<F190106Data, wcf.F190106Data>(tempAddF190106).ToArray();
			var delF190106 = ExDataMapper.MapCollection<F190106Data, wcf.F190106Data>(tempDelF190106).ToArray();
			var isSuccess = false;
			var f190105Data = ExDataMapper.Map<F190105, wcf.F190105>(F190105Data);
			var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
				() => proxyWcf.UpdateF190105AndF190106(f190105Data, addF190106, delF190106));
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
			//SearchCommand.Execute(null);
			//ShowResultMessage(result);
			//UserOperateMode = OperateMode.Query;
			//SearchCommand.Execute(null);
		}
		private void DoSaveComplete(bool isSuccess)
		{
			if (isSuccess)
			{
				IsCheckAllAllocationStockSchedule = false;
				IsCheckAllBatchPickNoSchedule = false;
				SearchCommand.Execute(null);
			}
				
		}

		public bool CheckData()
		{
			if (F190105Data.B2B_PDA_PICK_PECENT < 0 || F190105Data.B2B_PDA_PICK_PECENT > 100)
			{
				DialogService.ShowMessage(Properties.Resources.P1901710000_B2B_PDA_PICK_PECENT_ERROR);
				return false;
			}
			if (F190105Data.B2C_PDA_PICK_PERCENT < 0 || F190105Data.B2C_PDA_PICK_PERCENT > 100)
			{
				DialogService.ShowMessage(Properties.Resources.P1901710000_B2C_PDA_PICK_PERCENT_ERROR);
				return false;
			}
			if (F190105Data.PICKORDER_MAX_RECORD <= 0 || F190105Data.PICKORDER_MAX_RECORD > 999)
			{
				DialogService.ShowMessage(Properties.Resources.P1901710000_PICKORDER_MAX_RECORD_ERROR);
				return false;
			}
			if (F190105Data.ORDER_MAX_RECORD <= 0 || F190105Data.ORDER_MAX_RECORD > 999)
			{
				DialogService.ShowMessage(Properties.Resources.P1901710000_ORDER_MAX_RECORD_ERROR);
				return false;
			}
			if (F190105Data.ORDER_MAX_ITEMCNT <= 0 || F190105Data.ORDER_MAX_ITEMCNT > 999)
			{
				DialogService.ShowMessage(Properties.Resources.P1901710000_ORDER_MAX_ITEMCNT_ERROR);
				return false;
			}
      if (F190105Data.ORDER_MAX_ITEMCNT <= 0 || F190105Data.ORDER_MAX_ITEMCNT > 999)
      {
        DialogService.ShowMessage(Properties.Resources.P1901710000_ORDER_MAX_ITEMCNT_ERROR);
        return false;
      }

      var tmpDecimalValue = 0m;
      if (!decimal.TryParse(baseContainerMaxLength, out tmpDecimalValue))
      {
        DialogService.ShowMessage("最長邊(內徑)請輸入正確數值");
        return false;
      }
      F190105Data.BASE_CONTAINER_MAX_LENGTH = tmpDecimalValue;

      if (!decimal.TryParse(baseContainerMidLength, out tmpDecimalValue))
      {
        DialogService.ShowMessage("次長邊(內徑)請輸入正確數值");
        return false;
      }
      F190105Data.BASE_CONTAINER_MID_LENGTH = tmpDecimalValue;

      if (!decimal.TryParse(baseContainerMinLength, out tmpDecimalValue))
      {
        DialogService.ShowMessage("最短邊(內徑)請輸入正確數值");
        return false;
      }
      F190105Data.BASE_CONTAINER_MIN_LENGTH = tmpDecimalValue;

      if (!decimal.TryParse(baseContainerVolumn, out tmpDecimalValue))
      {
        DialogService.ShowMessage("可用容積請輸入正確數值");
        return false;
      }
      F190105Data.BASE_CONTAINER_VOLUMN = tmpDecimalValue;

      if (F190105Data.BASE_CONTAINER_MAX_LENGTH <= 0 || F190105Data.BASE_CONTAINER_MAX_LENGTH > 999)
      {
        DialogService.ShowMessage("最長邊(內徑)必須大於0且小於1000");
        return false;
      }
      if (F190105Data.BASE_CONTAINER_MID_LENGTH <= 0 || F190105Data.BASE_CONTAINER_MID_LENGTH > 999)
      {
        DialogService.ShowMessage("次長邊(內徑)必須大於0且小於1000");
        return false;
      }
      if (F190105Data.BASE_CONTAINER_MIN_LENGTH <= 0 || F190105Data.BASE_CONTAINER_MIN_LENGTH > 999)
      {
        DialogService.ShowMessage("最短邊(內徑)必須大於0且小於1000");
        return false;
      }
      if (F190105Data.BASE_CONTAINER_VOLUMN <= 0 || F190105Data.BASE_CONTAINER_VOLUMN > 9999999)
      {
        DialogService.ShowMessage("可用容積必須大於0且小於10000000");
        return false;
      }
      return true;
    }
		#endregion

		#region AddDetail
		//配庫排程設定
		public ICommand AddAllocationStockDetailCommand
		{
			get
			{

				return CreateBusyAsyncCommand(
					o => DoAdd(), () => UserOperateMode == OperateMode.Edit &&
					!string.IsNullOrWhiteSpace(AllocationStockStartTime) &&
					!string.IsNullOrWhiteSpace(AllocationStockEndTime) &&
					byte.TryParse(AllocationStockPeriod, out byte number) ? Convert.ToByte(AllocationStockPeriod) > 1 && Convert.ToByte(AllocationStockPeriod) <= 60 : false
					);
			}
		}
		// 揀貨排程設定
		public ICommand AddPickingDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(), () => UserOperateMode == OperateMode.Edit &&
					!string.IsNullOrWhiteSpace(PickingStartTime) &&
					!string.IsNullOrWhiteSpace(PickingEndTime) &&
					byte.TryParse(PickingPeriod, out byte number) ? Convert.ToByte(PickingPeriod) > 1 && Convert.ToByte(PickingPeriod) <= 60 : false
					);
			}
		}


		public void DoAdd()
		{
			if (SelectedIndex == "1")
			{
				if (CheckAddAndModifyData(SelectedIndex))
				{
					var tempAddData = AllSettingData.Where(x => x.DC_CODE == SelectedDcCode &&
																		  x.SCHEDULE_TYPE == SelectedIndex.PadLeft(2, '0') &&
																		  x.START_TIME == AllocationStockStartTime &&
																		  x.END_TIME == AllocationStockEndTime &&
																		  x.PERIOD == Convert.ToByte(AllocationStockPeriod)).FirstOrDefault();
					var addData = new F190106Data
					{
						DC_CODE = SelectedDcCode,
						SCHEDULE_TYPE = SelectedIndex.PadLeft(2, '0'),
						START_TIME = AllocationStockStartTime,
						END_TIME = AllocationStockEndTime,
						PERIOD = Convert.ToByte(AllocationStockPeriod),
						ChangeFlag = "A",
						CRT_STAFF = "",
						CRT_DATE = null,
						UPD_DATE = null
					};
					if (tempAddData == null)
					{

						var tempData = AllocationStockScheduleSetting.Select(x => x.Item).Select(x => x).ToList();
						tempData.Add(addData);

						AllocationStockScheduleSetting = tempData.ToSelectionList().ToList();
						AllSettingData.Add(addData);
					}
					else
					{
						if (tempAddData.ChangeFlag == "D")
						{
							tempAddData.ChangeFlag = "A";
						}
						else
						{
							DialogService.ShowMessage("新增資料重複");
						}
					}
					//AllocationStockStartTime = null;
					//AllocationStockEndTime = null;
					AllocationStockPeriod = null;
				}

			}
			else
			{
				if (CheckAddAndModifyData(SelectedIndex))
				{
					var tempAddData = AllSettingData.Where(x => x.DC_CODE == SelectedDcCode &&
																		  x.SCHEDULE_TYPE == SelectedIndex.PadLeft(2, '0') &&
																		  x.START_TIME == PickingStartTime &&
																		  x.END_TIME == PickingEndTime &&
																		  x.PERIOD == Convert.ToByte(PickingPeriod)).FirstOrDefault();
					var addData = new F190106Data
					{
						DC_CODE = SelectedDcCode,
						SCHEDULE_TYPE = SelectedIndex.PadLeft(2, '0'),
						START_TIME = PickingStartTime,
						END_TIME = _pickingEndTime,
						PERIOD = Convert.ToByte(_pickingPeriod),
						ChangeFlag = "A",
						CRT_STAFF = "",
						CRT_DATE = null,
						UPD_DATE = null
					};
					if (tempAddData == null)
					{
						var tempData = PickingScheduleSetting.Select(x => x.Item).Select(x => x).ToList();
						tempData.Add(addData);

						PickingScheduleSetting = tempData.ToSelectionList().ToList();
						AllSettingData.Add(addData);
					}
					else
					{
						if (tempAddData.ChangeFlag == "D")
						{
							tempAddData.ChangeFlag = "A";
						}
						else
						{
							DialogService.ShowMessage("新增資料重複");
						}
					}
					//PickingStartTime = null;
					//PickingEndTime = null;
					PickingPeriod = null;
				}
			}
		}

		public bool CheckAddAndModifyData(string selectedIndex)
		{
			switch (selectedIndex)
			{
				// 配庫排程設定
				case "1":
					// 驗證開始時間格式
					if (!ValidateHelper.IsMatchHHmm(AllocationStockStartTime))
					{
						DialogService.ShowMessage(Properties.Resources.P1901710000_StartTimeError);
						return false;
					}
					// 驗證結束時間格式
					if (!ValidateHelper.IsMatchHHmm(AllocationStockEndTime))
					{
						DialogService.ShowMessage(Properties.Resources.P1901710000_EndTimeError);
						return false;
					}
					// 驗證每幾分鐘執行一次的值
					if (Convert.ToByte(AllocationStockPeriod) < 1 ||
						Convert.ToByte(AllocationStockPeriod) > 60)
					{
						DialogService.ShowMessage(Properties.Resources.P1901710000_PeriodError);
						return false;
					}
					// 驗證開始時間和結束時間是否相同
					if (AllocationStockStartTime == AllocationStockEndTime)
					{
						DialogService.ShowMessage(Properties.Resources.P1901710000_StartTimeEndTimeSame);
						return false;
					}
					//驗證結束時間是否大於開始時間
					if (Convert.ToDateTime(AllocationStockStartTime) > Convert.ToDateTime(AllocationStockEndTime))
					{
						DialogService.ShowMessage(Properties.Resources.P1901710000_EndTimeIsGreaterThanStartTime);
						return false;
					}
					//驗證時間區間是否重複
					foreach (var itemData in AllocationStockScheduleSetting)
					{
						if (!(Convert.ToDateTime(itemData.Item.START_TIME) > Convert.ToDateTime(AllocationStockStartTime) && Convert.ToDateTime(itemData.Item.START_TIME)> Convert.ToDateTime(AllocationStockEndTime) ||
							Convert.ToDateTime(itemData.Item.END_TIME) < Convert.ToDateTime(AllocationStockStartTime) && Convert.ToDateTime(itemData.Item.END_TIME) < Convert.ToDateTime(AllocationStockEndTime)))
						{
							DialogService.ShowMessage(Properties.Resources.P1901710000_NewTimeIntervalOverlap);
							return false;
						}
					}
					// 驗證時間區間是否小於每幾分鐘執行一次
					if ((Convert.ToDateTime(AllocationStockEndTime) - Convert.ToDateTime(AllocationStockStartTime)).TotalMinutes <= Convert.ToByte(AllocationStockPeriod))
					{
						DialogService.ShowMessage(Properties.Resources.P1901710000_NewTimeIntervalIsLessThanOnceEveryFewMinutes);
						return false;
					}


					break;
				// 揀貨排程設定
				case "2":
					// 驗證開始時間格式
					if (!ValidateHelper.IsMatchHHmm(PickingStartTime))
					{
						DialogService.ShowMessage(Properties.Resources.P1901710000_StartTimeError);
						return false;
					}
					// 驗證結束時間格式
					if (!ValidateHelper.IsMatchHHmm(PickingEndTime))
					{
						DialogService.ShowMessage(Properties.Resources.P1901710000_EndTimeError);
						return false;
					}
					// 驗證每幾分鐘執行一次的值
					if (Convert.ToByte(PickingPeriod) < 1 ||
						Convert.ToByte(PickingPeriod) > 60)
					{
						DialogService.ShowMessage(Properties.Resources.P1901710000_PeriodError);
						return false;
					}
					// 驗證開始時間和結束時間是否相同
					if (PickingStartTime == PickingEndTime)
					{
						DialogService.ShowMessage(Properties.Resources.P1901710000_StartTimeEndTimeSame);
						return false;
					}
					//驗證結束時間是否大於開始時間
					if (Convert.ToDateTime(PickingStartTime) > Convert.ToDateTime(PickingEndTime))
					{
						DialogService.ShowMessage(Properties.Resources.P1901710000_EndTimeIsGreaterThanStartTime);
						return false;
					}
					//驗證時間區間是否重複
					foreach (var itemData in PickingScheduleSetting)
					{
						if (!(Convert.ToDateTime(itemData.Item.START_TIME) > Convert.ToDateTime(PickingStartTime) && Convert.ToDateTime(itemData.Item.START_TIME) > Convert.ToDateTime(PickingEndTime) ||
							Convert.ToDateTime(itemData.Item.END_TIME) < Convert.ToDateTime(PickingStartTime) && Convert.ToDateTime(itemData.Item.END_TIME) < Convert.ToDateTime(PickingEndTime)))
						{
							DialogService.ShowMessage(Properties.Resources.P1901710000_NewTimeIntervalOverlap);
							return false;
						}
					}
					// 驗證時間區間是否小於每幾分鐘執行一次
					if ((Convert.ToDateTime(PickingEndTime) - Convert.ToDateTime(PickingStartTime)).TotalMinutes <= Convert.ToByte(PickingPeriod))
					{
						DialogService.ShowMessage(Properties.Resources.P1901710000_NewTimeIntervalIsLessThanOnceEveryFewMinutes);
						return false;
					}
					break;
				default:
					break;
			}

			return true;
		}

		#endregion

		#region DelDetail
		// 配庫排程設定(刪除)
		public ICommand DelAllocationStockDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDel(), () => UserOperateMode == OperateMode.Edit &&
					AllocationStockScheduleSetting.Any()
					);
			}
		}
		// 揀貨排程設定(刪除)
		public ICommand DelPickingDetailCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDel(), () => UserOperateMode == OperateMode.Edit &&
					PickingScheduleSetting.Any()
					);
			}
		}
		public void DoDel()
		{
			switch (SelectedIndex)
			{
				case "1":
					foreach (var data in AllocationStockScheduleSetting.Where(si => si.IsSelected).Select(si => si.Item))
					{
						var existAllSettingData = AllSettingData.Where(x => x.DC_CODE == data.DC_CODE &&
												x.SCHEDULE_TYPE == data.SCHEDULE_TYPE &&
												x.START_TIME == data.START_TIME &&
												x.END_TIME == data.END_TIME && x.PERIOD == data.PERIOD).FirstOrDefault();
						existAllSettingData.ChangeFlag = "D";

					}

					AllocationStockScheduleSetting = AllocationStockScheduleSetting.Where(x => !x.IsSelected).ToList();
					break;
				case "2":
					foreach (var data in PickingScheduleSetting.Where(si => si.IsSelected).Select(si => si.Item))
					{
						var existAllSettingData = AllSettingData.Where(x => x.DC_CODE == data.DC_CODE &&
												x.SCHEDULE_TYPE == data.SCHEDULE_TYPE &&
												x.START_TIME == data.START_TIME &&
												x.END_TIME == data.END_TIME && x.PERIOD == data.PERIOD).FirstOrDefault();
						existAllSettingData.ChangeFlag = "D";

					}

					PickingScheduleSetting = PickingScheduleSetting.Where(x => !x.IsSelected).ToList();
					break;
				default:
					break;
			}


		}
		#endregion



	}
}
