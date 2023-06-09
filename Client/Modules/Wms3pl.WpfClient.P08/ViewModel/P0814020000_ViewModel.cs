using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F06DataService;
using Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.P08.Services;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public class P0814020000_ViewModel : P0814010000_ViewModel
	{
		#region Constructor
		public P0814020000_ViewModel()
		{
			if (!IsInDesignMode)
			{
			}
		}
		#endregion

		#region Public Variable
		/// <summary>
		/// 開始排程
		/// </summary>
		public Action OnStartDispatcher = () => { };
		/// <summary>
		/// 停止排程
		/// </summary>
		public Action OnStopDispatcher = () => { };

		public wcf.GetWorkStataionShipDataRes _workStationShipData;
		#endregion

		#region Property
		/// <summary>
		/// 已分配訂單數
		/// </summary>
		private int _waitWmsOrderCnt;
		public int WaitWmsOrderCnt
		{
			get { return _waitWmsOrderCnt; }
			set { Set(() => WaitWmsOrderCnt, ref _waitWmsOrderCnt, value); }
		}
		#endregion

		#region Public Method
		/// <summary>
		/// 取得排程執行頻率(秒)
		/// </summary>
		/// <returns></returns>
		public int GetAutoFindContainerPeriod()
		{
			var proxyF00 = GetProxy<F00Entities>();
			var f0003 = proxyF00.F0003s.Where(x => x.DC_CODE == "00" && x.GUP_CODE == "00" && x.CUST_CODE == "00" && x.AP_NAME == "AutoFindContainerPeriod").FirstOrDefault();
			return f0003 == null ? 0 : Convert.ToInt32(f0003.SYS_PATH);
		}

		/// <summary>
		/// 複寫關箱
		/// </summary>
		/// <param name="closeRes"></param>
		public override void DoCloseBoxComplete(CloseShipBoxRes closeRes)
		{
			if (closeRes.IsSuccessed)
			{
				//  (1) 呼叫[1.2.14 列印報表]([H].ReportList)
				if (closeRes.ReportList.Any())
				{
					LogHelper.Log(FunctionCode, "列印報表 開始 出貨單號" + WmsOrdData.WmsOrdNo);

					IsBusy = true;
					var result = _shipPackageService.PrintShipPackage(SelectedDc, _gupCode, _custCode, closeRes.ReportList.ToList(), SelectedF910501, WmsOrdData.WmsOrdNo);

					if (!result.IsSuccessed)
					{
						ShowWarningMessage(result.Message);
					}
					IsBusy = false;
					LogHelper.Log(FunctionCode, "列印報表 結束 出貨單號" + WmsOrdData.WmsOrdNo);
				}
				if (DetailData.All(x => x.DiffQty == 0))//a.如果所有[G].DiffQty = 0(所有商品都沒差異代表整張單包裝完成)
				{
					if (closeRes.LastPackageBoxNo == null || closeRes.LastPackageBoxNo == 0)//  (a - 2)如果[H].LastPackageBoxNo無值
					{
						_shipPackageService.InsertF050305Data(SelectedDc, _gupCode, _custCode, WmsOrdData.WmsOrdNo, "3", "0", WorkStationCode);
						LogHelper.Log(FunctionCode, "寫入包裝完成 結束 出貨單號" + WmsOrdData.WmsOrdNo);
						// 清除所有出貨資訊，回到可刷讀容器條碼畫面
						// 容器條碼Focus
						InitialData();
						ChangeInitMode();
						if (_f1946.STATUS == "1" || _f1946.STATUS == "2")
							OnStartDispatcher();
						else if (_f1946.STATUS == "0" || _f1946.STATUS == "3")
						{
							Message = "您可以按下[離開]，關閉包裝站";
							MessageForeground = Brushes.Blue;
						}
					}
					else// 如果[H].LastPackageBoxNo有值
					{
						// 呼叫[1.2.12 手動關箱 / 系統自動關箱]([H].LastPackageBoxNo)
						_closeBoxParam = new CloseBoxParam { PackageBoxNo = closeRes.LastPackageBoxNo };
						CloseBoxCommand.Execute(_closeBoxParam);
					}
				}
				else//b.如果所有[G].DiffQty != 0(還有差異)
				{
					FocusSelectAllSerial();// 游標Focus商品條碼
				}
				
			}
		}

		/// <summary>
		/// 複寫取消包裝
		/// </summary>
		public override string DoCancelPacking()
		{
			// 人員按下取消包裝:要跳出詢問視窗 "您是否要重新包裝此單?"
			if (ShowConfirmMessage(string.Format("請確認是否取消包裝 ? {0} - 按下< 是 >，取消包裝並刪除容器到站資料，並取得下一箱資料 {0} - 按下< 否 >，取消包裝並重包此單", Environment.NewLine)) == UILib.Services.DialogResponse.No)
			{
				// 選[是] =>執行取消包裝後 => 重新載入此單
				//(1)[B] = 呼叫[1.1.8 取消包裝]([SS].DcCode,[SS].GupCode,[SS].CustCode,[SS].WmsOrdNo)
				var cancelRes = _shipPackageService.CancelShipOrder(new CancelShipOrderReq
				{
					DcCode = WmsOrdData.DcCode,
					GupCode = WmsOrdData.GupCode,
					CustCode = WmsOrdData.CustCode,
					WmsOrdNo = WmsOrdData.WmsOrdNo
				});

				if (!cancelRes.IsSuccessed)
				{
					//a.[訊息]=[B].Message
					//b.跳出視窗顯示[B].Message
					//c.更新前端出貨單資訊Status=[B].Status
					Message = cancelRes.Message;
					MessageForeground = Brushes.Red;
					ShowWarningMessage(cancelRes.Message);
					WmsOrdData.Status = cancelRes.Status;
					return null;
				}
				else
				{
					ContainerCode = _workStationShipData.ArrivalContainerCode;
					return "01";// 重新載入此單
				}
			}
			else
			{
				//(1)[B] = 呼叫[1.1.8 取消包裝]([SS].DcCode,[SS].GupCode,[SS].CustCode,[SS].WmsOrdNo)
				var cancelRes = _shipPackageService.CancelShipOrder(new CancelShipOrderReq
				{
					DcCode = WmsOrdData.DcCode,
					GupCode = WmsOrdData.GupCode,
					CustCode = WmsOrdData.CustCode,
					WmsOrdNo = WmsOrdData.WmsOrdNo
				});

				if (!cancelRes.IsSuccessed)
				{
					//a.[訊息]=[B].Message
					//b.跳出視窗顯示[B].Message
					//c.更新前端出貨單資訊Status=[B].Status
					Message = cancelRes.Message;
					MessageForeground = Brushes.Red;
					ShowWarningMessage(cancelRes.Message);
					WmsOrdData.Status = cancelRes.Status;
					return null;
				}
				else
				{
					// 選[否]  =>執行取消包裝後 => 顯示訊息"請將此單商品放回箱內，並將此箱移至異常區"
					if (ShowInfoMessage("請將此單商品放回箱內，並將此箱移至異常區") == UILib.Services.DialogResponse.OK)
					{
						var proxy = new wcf.P08WcfServiceClient();
						var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.CancelArrivalRecord(SelectedDc, _gupCode, _custCode, _workStationShipData.ArrivalWmsNo, null));
						if (result.IsSuccessed)
						{

							InitialData();
							ChangeInitMode();

							// 按下確定後，啟動排程(工作站狀態 = 開站、暫停] ，若工作站狀態 = 關站中，不啟動排程，並顯示您可以離開此包裝站
							if (_f1946.STATUS == "1" || _f1946.STATUS == "2")
								return "02";
							else if (_f1946.STATUS == "3")
								ShowInfoMessage("您可以離開此包裝站");
						}
						else
						{
							ShowWarningMessage(result.Message);
						}
					}
				}
			}

			return null;
		}

    /// <summary>
    /// 複寫取消包裝_Complete
    /// </summary>
    /// <param name="action"></param>
    public override void DoCancelPackingComplete(string action)
    {
      if (action == "01") //重新載入此單
        SearchContainerCommand.Execute(null);
      else if (action == "02") //啟動排程
        OnStartDispatcher();

		}

		/// <summary>
		/// 複寫包裝完成_Complete
		/// </summary>
		public override void DoEndPackingComplete()
		{
			if (WmsOrdData.IsPackCheck == "否")// 是否過刷=否
			{
				//(1) 如果[[SS].BoxCnt > 1 and 此功能為單人包裝站，
				if (WmsOrdData.BoxCnt > 1 && ContainerRes == false)
					return;

				_shipPackageService.InsertF050305Data(SelectedDc, _gupCode, _custCode, WmsOrdData.WmsOrdNo, "6", "1", WorkStationCode);


				// [A]=呼叫[1.1.7 使用出貨單容器資料產生箱明細]([SS].DcCode,[SS].GupCode,[SS].CustCode,[SS].WmsOrdNo)
				var res = _shipPackageService.UseShipContainerToBoxDetail(new UseShipContainerToBoxDetailReq
				{
					DcCode = SelectedDc,
					GupCode = _gupCode,
					CustCode = _custCode,
					WmsOrdNo = WmsOrdData.WmsOrdNo,
					DelvDate = WmsOrdData.DelvDate,
					PickTime = WmsOrdData.PickTime,
					SubBoxNo = WmsOrdData.SugBoxNo,
					ShipMode = ShipMode
				});

				// 如果[A].IsSuccessed = false
				if (!res.IsSuccessed)
				{
					Message = res.Message;
					MessageForeground = Brushes.Red;
					FocusSelectAllSerial();// 游標Focus商品條碼
				}
				else// 如果[A].IsSuccessed = true
				{
					//  a. [B]=呼叫[1.1.3 查詢出貨商品包裝明細]
					DetailData = _shipPackageService.SearchWmsOrderPackingDetail(SelectedDc, _gupCode, _custCode, WmsOrdData.WmsOrdNo);

					if (DetailData.Any(x => x.DiffQty > 0)) // 如果[B].DiffQty 有一筆 > 0
					{
						var msg = "本出貨單需要的商品未全部到齊，無法出貨，請將商品放入最後一個周轉箱中，並送至異常區處理。(系統自動進行取消包裝)";
						Message = msg;
						MessageForeground = Brushes.Red;
						if (ShowWarningMessage(msg) == UILib.Services.DialogResponse.OK)
						{
							// (c - 3)當人員按下確定後
							// (c - 3 - 1)[C] = 呼叫[1.1.8 取消包裝]([SS].DcCode,[SS].GupCode,[SS].CustCode,[SS].WmsOrdNo)
							var cancelRes = _shipPackageService.CancelShipOrder(new CancelShipOrderReq
							{
								DcCode = WmsOrdData.DcCode,
								GupCode = WmsOrdData.GupCode,
								CustCode = WmsOrdData.CustCode,
								WmsOrdNo = WmsOrdData.WmsOrdNo
							});

							if (!cancelRes.IsSuccessed)// (c-3-2)如果[C].IsSuccessed=false
							{
								// (c-3-2-1) [訊息]=C.Message =>紅字
								// (c-3-2-2)彈出視窗顯示[C.Message]
								Message = cancelRes.Message;
								MessageForeground = Brushes.Red;
							}
							else // (c-3-3)如果[C].IsSuccessed = true
							{
								var proxy = GetModifyQueryProxy<F06Entities>();
								var result = proxy.F060208s.Where(x =>
								x.DC_CODE == SelectedDc &&
								x.GUP_CODE == _gupCode &&
								x.CUST_CODE == _custCode &&
								x.PROC_FLAG == 1 &&
								x.CONTAINER_CODE == _workStationShipData.ArrivalContainerCode).AsQueryable().FirstOrDefault();

								if (result != null)
								{
									result.PROC_FLAG = 3;
									proxy.UpdateObject(result);
									proxy.SaveChanges();
								}

								InitialData();
								ChangeInitMode();

								// 按下確定後，啟動排程(工作站狀態 = 開站、暫停] ，若工作站狀態 = 關站中，不啟動排程，並顯示您可以離開此包裝站
								if (_f1946.STATUS == "1" || _f1946.STATUS == "2")
									ExecuteScheduleCommand.Execute(null);
								else if (_f1946.STATUS == "3")
									ShowInfoMessage("您可以離開此包裝站");
							}
						}
					}
					else if (DetailData.All(x => x.DiffQty == 0)) // 如果所有[B].DiffQty=0
					{
						//  (d-1) 如果是需要刷讀紙箱，訊息顯示[請刷讀紙箱進行關箱]
						if (CloseByBoxno == "1")
						{
							Message = "請刷讀紙箱進行關箱";
							MessageForeground = Brushes.Red;
						}
						else
						{
							// (d-1) 呼叫[1.2.12 手動關箱/系統自動關箱]([A].PackageBoxNo)
							_closeBoxParam = new CloseBoxParam { PackageBoxNo = res.PackageBoxNo };
							CloseBoxCommand.Execute(_closeBoxParam);
						}
					}
				}
			}
			else// 是否過刷=是
			{
				//(1) 如果[B].DiffQty 有一筆 > 0
				//     a. [訊息] = 尚有商品未完成刷讀，不可包裝完成，請刷讀商品條碼
				//     b.彈出視窗[尚有商品未完成刷讀，不可包裝完成，請刷讀商品條碼]，
				//       按下確定後，商品條碼Focus
				if (DetailData.Any(x => x.DiffQty > 0))
				{
					var msg = "尚有商品未完成刷讀，不可包裝完成，請刷讀商品條碼";
					Message = msg;
					MessageForeground = Brushes.Red;
					if (ShowWarningMessage(msg) == UILib.Services.DialogResponse.OK)
						FocusSelectAllSerial();// 游標Focus商品條碼
				}
				else if (DetailData.All(x => x.DiffQty == 0))//(2) 如果所有[B].DiffQty = 0 呼叫[1.2.12 手動關箱 / 系統自動關箱](null)
				{
					if (CloseByBoxno == "1")
					{
						Message = "請刷讀紙箱進行關箱";
						MessageForeground = Brushes.Red;
					}
					else
					{
						_closeBoxParam = new CloseBoxParam();
						CloseBoxCommand.Execute(_closeBoxParam);
					}
				}
			}
		}

		#endregion

		#region ExecuteSchedule 啟動排程
		public ICommand ExecuteScheduleCommand
		{
			get
			{
				bool isSearchContainer = false;
				return CreateBusyAsyncCommand(
						o => isSearchContainer = ExecuteSchedule(),
						() => true,
						o => ExecuteScheduleComplete(isSearchContainer)
				);
			}
		}
		/// <summary>
		/// 啟動排程
		/// </summary>
		private bool ExecuteSchedule()
		{
			LogHelper.Log(FunctionCode, "啟動排程");
			IsBusy = true;
			//Thread.Sleep(10);

			// 訊息顯示[等待容器抵達工作站，請稍候]
			Message = "等待容器抵達工作站，請稍候";
			MessageForeground = Brushes.White;

			// [A]= 呼叫1.1.18 取得包裝線工作站分配結果(目前的物流中心，工作站編號)
			var proxy = new wcf.P08WcfServiceClient();
			_workStationShipData = RunWcfMethod<wcf.GetWorkStataionShipDataRes>(proxy.InnerChannel, () => proxy.GetWorkStataionShipData(new wcf.GetWorkStataionShipDataReq
			{
				DcCode = SelectedDc,
				workstationCode = WorkStationCode
			}));

			// 更新畫面已分配訂單數=[A].WaitWmsOrderCnt
			WaitWmsOrderCnt = _workStationShipData.WaitContainerCnt;

			// [A].IsArrialContainer = false，不處理
			if (!_workStationShipData.IsArrialContainer)
			{
				IsBusy = false;
				LogHelper.Log(FunctionCode, "讀不到容器資料");
				return false;
			}
			LogHelper.Log(FunctionCode, "讀到容器資料"+ _workStationShipData.ArrivalContainerCode);
			//(5)[A].IsArrialContainer = true，
			//a.停止排程
			OnStopDispatcher();
			LogHelper.Log(FunctionCode, "停止排程");
			//b.容器條碼 =[A].ArrivalContainerCode
			ContainerCode = _workStationShipData.ArrivalContainerCode;

			IsBusy = false;
			return true;
		}

		private void ExecuteScheduleComplete(bool isSearchContainer)
		{
			if (isSearchContainer)
			{
				//c.呼叫[1.2.11 刷讀容器條碼]
				//SearchContainerCommand.Execute(true);
				DoSearchContainer();
				if (WmsOrdData != null && WmsOrdData.IsCancelOrder)
				{
					ShowWarningMessage(WmsOrdData.Result.Message);
					InitialData();
					ChangeInitMode();

					if (_f1946.STATUS == "0")
						ShowWarningMessage("您可以離開包裝站");
					else
						OnStartDispatcher();
				}
				DoSearchContainerComplete();
			}
		}
		#endregion


		#region CancelArriveRecord
		/// <summary>
		/// Gets the CancelArriveRecord.
		/// </summary>
		public ICommand CancelArriveRecordCommand
		{
			get
			{
				var action = string.Empty;
				return CreateBusyAsyncCommand(
						o => action = DoCancelArriveRecord(),
						() => _workStationShipData != null && _workStationShipData.IsArrialContainer,
						o => DoCancelArriveRecordComplete(action)
);
			}
		}

		public string DoCancelArriveRecord()
		{
			LogHelper.Log(FunctionCode, "人員按下取消到站資料 開始");

			if (ShowConfirmMessage(string.Format("本箱的容器編號{0}、出貨單號{1}，是否跳過，{2} - 按下< 是 >，押掉目前這筆的容器到站資料，並取得下一箱資料 {2} - 按下< 否 >，回到原本畫面",
				_workStationShipData.ArrivalContainerCode, _workStationShipData.ArrivalWmsNo, Environment.NewLine)) == DialogResponse.Yes)
			{
				var proxy = new wcf.P08WcfServiceClient();
				var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel, () => proxy.CancelArrivalRecord(SelectedDc, _gupCode, _custCode, _workStationShipData.ArrivalWmsNo, _workStationShipData.ArrivalContainerCode));
				if (result.IsSuccessed)
				{
					InitialData();
					ChangeInitMode();

					// 按下確定後，啟動排程(工作站狀態 = 開站、暫停] ，若工作站狀態 = 關站中，不啟動排程，並顯示您可以離開此包裝站
					if (_f1946.STATUS == "1" || _f1946.STATUS == "2")
					{
						LogHelper.Log(FunctionCode, "目前工作站狀態為開站、暫停時，啟動排程");
						return "02";

					}
						
					else if (_f1946.STATUS == "3")
					{
						LogHelper.Log(FunctionCode, "目前工作站狀態為關站中時，不啟動排程");
						ShowInfoMessage("您可以離開此包裝站");
					}
				}
				else
				{
					ShowWarningMessage(result.Message);
					return null;
				}
			}
			return null;
		}

		public void DoCancelArriveRecordComplete(string action)
		{
			if (action == "02") //啟動排程
				OnStartDispatcher();
		}
		#endregion CancelArriveRecord

	}
}
