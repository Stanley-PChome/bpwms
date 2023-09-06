using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F06;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F20;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;

namespace Wms3pl.WebServices.Shared.Services
{
	public class OrderService
	{
		private WmsTransaction _wmsTransaction;
		private SharedService _sharedService;
		public SharedService SharedService {
			get {
				if (_sharedService == null)
					_sharedService = new SharedService(_wmsTransaction);
				return _sharedService;   
			}
			set
			{
				_sharedService = value;
			}
		}
		private StockService _stockService;
		public StockService StockService
		{
			get
			{
				if (_stockService == null)
					_stockService = new StockService(_wmsTransaction);
				return _stockService;
			}
			set
			{
				_stockService = value;
			}
		}
		private SerialNoCancelService _serialNoCancelService;
		public SerialNoCancelService SerialNoCancelService
		{
			get
			{
				if (_serialNoCancelService == null)
					_serialNoCancelService = new SerialNoCancelService(_wmsTransaction);
				return _serialNoCancelService;
			}
			set
			{
				_serialNoCancelService = value;
			}
		}

		private CommonService _commonService;
		public CommonService CommonService
		{
			get
			{
				if (_commonService == null)
					_commonService = new CommonService();
				return _commonService;
			}
			set
			{
				_commonService = value;
			}
		}



		public OrderService(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 取消未配庫訂單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="ordNo"></param>
		/// <param name="procFlag"></param>
		public void CancelNotAllocStockOrder(string dcCode, string gupCode, string custCode, string ordNo, string procFlag)
		{
			#region 修改 F050101
			var f050101Repo = new F050101Repository(Schemas.CoreSchema, _wmsTransaction);

			var f050101Data = f050101Repo.AsForUpdate().GetDataByOrdNo(dcCode, gupCode, custCode, ordNo);

			if (f050101Data != null)
			{
				f050101Data.IMPORT_FLAG = procFlag == "D" ? "2" : "1";
				f050101Data.STATUS = "9";
				f050101Data.UPD_DATE = DateTime.Now;
				f050101Data.UPD_STAFF = Current.Staff;
				f050101Data.UPD_NAME = Current.StaffName;

				f050101Repo.Update(f050101Data);
			}
			#endregion

			#region 修改 F050104
			var f050104Repo = new F050104Repository(Schemas.CoreSchema, _wmsTransaction);

			var f050104Data = f050104Repo.AsForUpdate().GetDatas(dcCode, gupCode, custCode, ordNo).ToList();

			if (f050104Data.Any())
			{
				f050104Data.ForEach(f050104 => { f050104.STATUS = "9"; });
				f050104Repo.BulkUpdate(f050104Data);
			}
			#endregion

			#region 刪除 F050001
			var f050001Repo = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);

			f050001Repo.Delete(new List<string> { ordNo }, gupCode, custCode, dcCode);
			#endregion

			#region 刪除 F050002
			var f050002Repo = new F050002Repository(Schemas.CoreSchema, _wmsTransaction);

			f050002Repo.Delete(new List<string> { ordNo }, gupCode, custCode, dcCode);
			#endregion
		}

		public ExecuteResult CancelAllocStockOrder(string dcCode, string gupCode, string custCode, List<string> ordNos, string workType, string allId, string allTime, string address, string newDcCode, string cause, string causeMemo)
		{
			var f050301Repo = new F050301Repository(Schemas.CoreSchema, _wmsTransaction);

			var f050301Datas = f050301Repo.GetF050301Datas(dcCode, gupCode, custCode, ordNos, workType).ToList();

			var result = new ExecuteResult { IsSuccessed = true };
			var newDelvTime = "";
			var f0513Repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
			var delvDate = f050301Datas.First().DELV_DATE;
			var pickTime = f050301Datas.First().PICK_TIME;
			F0513 orginalF0513 =
					f0513Repo.Find(
							o =>
									o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.DELV_DATE == delvDate &&
									o.PICK_TIME == pickTime);
			if (workType == "1")
			{
				var dtmAllTime = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd") + " " + allTime);
				if (dtmAllTime < DateTime.Now.AddHours(1))
				{
					result.IsSuccessed = false;
					result.Message = Properties.Resources.DtmAllTimeGreaterThanOneHour;
					return result;
				}
				var dtmBegAllTime = (dtmAllTime.AddHours(-1) < DateTime.Now.Date) ? DateTime.Now.Date : dtmAllTime.AddHours(-1);


				for (var dtime = dtmBegAllTime; dtime <= dtmAllTime; dtime = dtime.AddMinutes(5))
				{
					var time = dtime.ToString("HH:mm");

					var item = f0513Repo.Filter(
							o =>
									o.DC_CODE == dcCode && o.GUP_CODE == gupCode &&
									o.CUST_CODE == custCode && o.DELV_DATE == delvDate &&
									o.PICK_TIME == time).FirstOrDefault();
					if (item == null)
					{
						newDelvTime = time;
						break;
					}
				}
				if (string.IsNullOrEmpty(newDelvTime))
				{
					result.IsSuccessed = false;
					result.Message = Properties.Resources.HaveNoNewDelvTime;
					return result;
				}
			}

			var adjustNo = SharedService.GetNewOrdCode("J");
			var f200101Repo = new F200101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f200101 = new F200101
			{
				DC_CODE = f050301Datas.First().DC_CODE,
				GUP_CODE = f050301Datas.First().GUP_CODE,
				CUST_CODE = f050301Datas.First().CUST_CODE,
				ADJUST_NO = adjustNo,
				ADJUST_TYPE = "0",
				WORK_TYPE = workType,
				ADJUST_DATE = DateTime.Now.Date,
				STATUS = "0",
				CRT_DATE = DateTime.Now
			};
			f200101Repo.Add(f200101);
			var f200102Repo = new F200102Repository(Schemas.CoreSchema, _wmsTransaction);
			short seq = 1;
			var f050101Repo = new F050101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05030101Rep = new F05030101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			var f19471201Repo = new F19471201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f700101Repo = new F700101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f700102Repo = new F700102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050001Repo = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f075102Repo = new F075102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f194712Repo = new F194712Repository(Schemas.CoreSchema);
			var f1951Repo = new F1951Repository(Schemas.CoreSchema);
			//紀錄此訂單取消且有關聯的訂單紀錄並不存在此次勾選訂單名單
			var relationOrdList = new List<F050301Data>();
			var f1951List = f1951Repo.GetDatasByTrueAndCondition(o => o.UCT_ID == "AJ").ToList();
			//需求調整: 若 取消訂單 或 修改出貨物流中心 會取消訂單狀態. 
			//必需檢查是否有相同出貨單(wmsOrdNo)相關的訂單(ordNo) 也一併做取消動作. 並重新產生訂單池
			//以下做檢查動作.若有相關 OrdNo 加入 f050301Datas 裡			
			if (workType == "0" || workType == "3" || workType == "4")
			{
				var tmpf050301Datas = f050301Datas.Select(AutoMapper.Mapper.DynamicMap<F050301Data>).ToList();
				foreach (var f050301Data in tmpf050301Datas)
				{
					var f050301WmsNoData = f050301Repo.GetF050301WmsNoData(f050301Data.DC_CODE, f050301Data.GUP_CODE, f050301Data.CUST_CODE, f050301Data.ORD_NO).ToList();
					foreach (var item in f050301WmsNoData)
					{
						var f050301NewData = f050301Datas.Where(o => o.DC_CODE == item.DC_CODE && o.GUP_CODE == item.GUP_CODE && o.CUST_CODE == item.CUST_CODE
																										&& o.ORD_NO == item.ORD_NO).FirstOrDefault();
						//未在原本 f050301Datas 時, 加入item 置 f050301Datas
						if (f050301NewData == null)
						{
							f050301Datas.Add(item);
							if (!tmpf050301Datas.Any(o => o.DC_CODE == item.DC_CODE && o.GUP_CODE == item.GUP_CODE && o.CUST_CODE == item.CUST_CODE && o.ORD_NO == item.ORD_NO))
								relationOrdList.Add(item);
						}
					}
				}

				var f050801s = f050801Repo.AsForUpdate().GetF050801sByF050301s(dcCode, gupCode, custCode, f050301Datas.Select(x => x.ORD_NO).Distinct().ToArray());
				if (workType == "0" || workType == "3")
				{
					// 若是取消訂單，則將出貨單設為不出貨
					foreach (var f050801 in f050801s)
					{
						f050801.STATUS = 9;
						f050801Repo.Update(f050801);
						if (workType == "0")
						{
							//有託運單才更新
							var consignNos = f050901Repo.Filter(o => o.WMS_NO == f050801.WMS_ORD_NO).Select(o => o.CONSIGN_NO).ToList();
							if (consignNos.Any())
							{
								f050901Repo.Delete(o => o.WMS_NO == f050801.WMS_ORD_NO);
								//速達才釋放託運單號
								if (f050801.ALL_ID == "TCAT")
								{
									var f050301 = f050301Repo.GetDataByWmsOrdNo(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, f050801.WMS_ORD_NO);
									var channel = "00";
									if (f050301 != null)
										channel = f050301.CHANNEL;
									var f194712 = f194712Repo.Get(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, channel, f050801.ALL_ID);
									if (f194712 == null)
										channel = "00";
									f19471201Repo.UpDataForIsused(dcCode, gupCode, custCode, channel, f050801.ALL_ID, null, consignNos);
								}

							}

						}
					}
					if (workType == "0")
					{
						//訂單取消 也要將派車單取消
						if (f050801s.Any())
							f700101Repo.UpdateF700101StatusToCancel(dcCode, f050801s.Select(x => x.WMS_ORD_NO).ToList());
					}
				}
				else if (workType == "4")
				{
					DeleteF700102WithF050901(dcCode, gupCode, custCode, f050801s.Select(x => x.WMS_ORD_NO).ToList());
				}
			}
			List<string> addWmsOrdNo = new List<string>();

			foreach (var f050301Data in f050301Datas)
			{
				var f050301Item = f050301Repo.Find(o => o.DC_CODE == f050301Data.DC_CODE && o.GUP_CODE == f050301Data.GUP_CODE && o.CUST_CODE == f050301Data.CUST_CODE && o.ORD_NO == f050301Data.ORD_NO);
				var f200102 = new F200102
				{
					DC_CODE = f050301Data.DC_CODE,
					GUP_CODE = f050301Data.GUP_CODE,
					CUST_CODE = f050301Data.CUST_CODE,
					ADJUST_NO = adjustNo,
					ADJUST_SEQ = seq,
					WORK_TYPE = workType,
					ORD_NO = f050301Data.ORD_NO,
					STATUS = "0"
				};
				var f050101 = f050101Repo.Find(o => o.DC_CODE == f050301Data.DC_CODE && o.GUP_CODE == f050301Data.GUP_CODE && o.CUST_CODE == f050301Data.CUST_CODE && o.ORD_NO == f050301Data.ORD_NO);

				switch (workType)
				{
					case "0": //訂單取消
						f200102.CAUSE = cause;
						f200102.CAUSE_MEMO = causeMemo;
						f200102.ORG_STATUS = f050301Item.PROC_FLAG;
						//只有關聯訂單且不再此次勾選訂單取消名單內 要替他重新產生訂單 否則不產生
						if (relationOrdList.Any(o => o.DC_CODE == f050301Data.DC_CODE && o.GUP_CODE == f050301Data.GUP_CODE && o.CUST_CODE == f050301Data.CUST_CODE && o.ORD_NO == f050301Data.ORD_NO))
						{ //需求調整: 新增訂單池 
							f200102.NEW_ORD_NO = UpdateWorkType3(f050301Data.DC_CODE, f050301Data.GUP_CODE, f050301Data.CUST_CODE, f050301Data.ORD_NO, f050301Data.DC_CODE, f050301Item);
						}
						f050301Item.PROC_FLAG = "9";
						f050301Repo.Update(f050301Item);
						if (f050101 != null)
						{
							f050101.STATUS = "9";
							f050101Repo.Update(f050101);
						}
						break;
					case "1": //產生新批次
						f200102.CAUSE = cause;
						f200102.CAUSE_MEMO = causeMemo;
						f200102.ORG_PICK_TIME = f050301Data.PICK_TIME;
						f200102.ALL_ID = allId;
						var f0513 = new F0513
						{
							DC_CODE = orginalF0513.DC_CODE,
							GUP_CODE = orginalF0513.GUP_CODE,
							CUST_CODE = orginalF0513.CUST_CODE,
							DELV_DATE = orginalF0513.DELV_DATE,
							PICK_TIME = newDelvTime,
							ORD_TYPE = orginalF0513.ORD_TYPE,
							PROC_FLAG = orginalF0513.PROC_FLAG,
							CHECKOUT_TIME = allTime,
							ALL_ID = allId,
							PIER_CODE = orginalF0513.PIER_CODE,
							CAR_NO_A = orginalF0513.CAR_NO_A,
							CAR_NO_B = orginalF0513.CAR_NO_B,
							CAR_NO_C = orginalF0513.CAR_NO_C,
							ISSEAL = orginalF0513.ISSEAL
						};
						f0513Repo.Add(f0513);
						UpdateWorkType1Or2(f050301Data.DC_CODE, f050301Data.GUP_CODE, f050301Data.CUST_CODE, f050301Data.ORD_NO, newDelvTime);
						break;
					case "2": //修改配送資訊
						f200102.ORG_ADDRESS = f050301Item.ADDRESS;
						f200102.ADDRESS = address;
						f200102.CAUSE = cause;
						f200102.CAUSE_MEMO = causeMemo;
						f200102.ALL_ID = allId;
						f200102.ORG_PICK_TIME = f050301Data.PICK_TIME;
						UpdateWorkType1Or2(f050301Data.DC_CODE, f050301Data.GUP_CODE, f050301Data.CUST_CODE, f050301Data.ORD_NO, allTime);
						f050301Item.ADDRESS = address;
						f050301Repo.Update(f050301Item);
						break;
					case "3": //修改出貨物流中心
						f200102.CAUSE = cause;
						f200102.CAUSE_MEMO = causeMemo;
						f200102.NEW_DC_CODE = newDcCode;
						f200102.NEW_ORD_NO = UpdateWorkType3(f050301Data.DC_CODE, f050301Data.GUP_CODE, f050301Data.CUST_CODE, f050301Data.ORD_NO, newDcCode, f050301Item);
						f050301Item.PROC_FLAG = "9";
						f050301Repo.Update(f050301Item);
						f050101.STATUS = "9";
						f050101Repo.Update(f050101);
						break;
					case "4": //自取
						f200102.CAUSE = cause;
						f200102.CAUSE_MEMO = causeMemo;
						UpdateWorkType4(f050301Data.DC_CODE, f050301Data.GUP_CODE, f050301Data.CUST_CODE, f050301Data.ORD_NO, selfTake: "1", printPass: "0");
						f050301Item.SELF_TAKE = "1";
						f050301Repo.Update(f050301Item);
						break;
					case "5": //修改出貨地址
						f200102.CAUSE = cause;
						f200102.CAUSE_MEMO = causeMemo;
						ChangeAddress(true, f050301Data.ADDRESS, f050301Data.ZIP_CODE, f050301Item, f200102, f050301Repo);
						break;
				}
				seq++;
				f200102Repo.Add(f200102);

			}

			// 訂單取消
			if (workType == "0")
			{
				var f05030101s = f05030101Rep.GetDatasForOrdNoList(dcCode, gupCode, custCode, f050301Datas.Select(x => x.ORD_NO).ToList());

        // 取消出貨單揀貨資料
        var cancelRes = CancelWmsOrderPick(dcCode, gupCode, custCode, f05030101s.Select(x => x.WMS_ORD_NO).Distinct().ToList());
        if (!cancelRes.IsSuccessed)
          return cancelRes;
        #region 刪除F750102
        var delCustOrdNos = (from A in f050301Datas
														 join B in f05030101s
														 on A.ORD_NO equals B.ORD_NO
														 select A.CUST_ORD_NO).Distinct().ToList();
				if (delCustOrdNos.Any())
					f075102Repo.DelF075102ByCustOrdNos(custCode, delCustOrdNos);
				#endregion
			}

			if (result.IsSuccessed == true)
				result.Message = adjustNo;

			return result;
		}

		/// <summary>
		/// 更新地址
		/// </summary>
		/// <param name="isAdd">是否為新增調整單</param>
		/// <param name="address">更新的地址</param>
		/// <param name="zipCode">更新的郵遞區號</param>
		/// <param name="f050301Item">原始F050301資料</param>
		/// <param name="item">調整單明細資料</param>
		/// <param name="f050301Repo">F050301Repository</param>
		private void ChangeAddress(bool isAdd, string address, string zipCode, F050301 f050301Item, F200102 item, F050301Repository f050301Repo)
		{
			var f700101Repo = new F700101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f700102Repo = new F700102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05030101Rep = new F05030101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050001Repo = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);
			if (f050301Item != null)
			{
				//已配庫未出貨包裝
				var f05030101 = f05030101Rep.Find(o => o.DC_CODE == item.DC_CODE && o.GUP_CODE == item.GUP_CODE && o.CUST_CODE == item.CUST_CODE && o.ORD_NO == item.ORD_NO);
				if (f05030101 != null)
				{
					//已配庫未出貨包裝資料還須更新派車單明細地址郵遞區號
					var f700102 = f700102Repo.Find(o => o.DC_CODE == f05030101.DC_CODE && o.GUP_CODE == f05030101.GUP_CODE && o.CUST_CODE == f05030101.CUST_CODE && o.WMS_NO == f05030101.WMS_ORD_NO && o.ZIP_CODE == f050301Item.ZIP_CODE && o.ADDRESS == f050301Item.ADDRESS);
					if (f700102 != null)
					{
						f700102.ADDRESS = address;
						f700102.ZIP_CODE = zipCode;
						f700102Repo.Update(f700102);
					}
				}
				//新增F200102異動調整單明細+更新F050301
				if (isAdd)
				{
					item.ORG_ADDRESS = f050301Item.ZIP_CODE + "," + f050301Item.ADDRESS;
					item.ADDRESS = zipCode + "," + address;
				}
				f050301Item.ADDRESS = address;
				f050301Item.ZIP_CODE = zipCode;
				f050301Repo.Update(f050301Item);
			}
			else
			{
				//未配庫已核准
				var f050001Item = f050001Repo.Find(o => o.DC_CODE == item.DC_CODE && o.GUP_CODE == item.GUP_CODE && o.CUST_CODE == item.CUST_CODE && o.ORD_NO == item.ORD_NO);
				if (f050001Item != null)
				{
					//F200102 調整單明細 沒有郵遞區號所以郵遞區號串在地址欄位用，分隔
					if (isAdd)
					{
						item.ORG_ADDRESS = f050001Item.ZIP_CODE + "," + f050001Item.ADDRESS;
						item.ADDRESS = zipCode + "," + address;
					}
					f050001Item.ADDRESS = address;
					f050001Item.ZIP_CODE = zipCode;
					f050001Repo.Update(f050001Item);
				}
			}
		}

		private void UpdateWorkType1Or2(string dcCode, string gupCode, string custCode, string ordNo, string newPickTime)
		{
			var f05030101Repo = new F05030101Repository(Schemas.CoreSchema);
			var items =
					f05030101Repo.Filter(
							o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ORD_NO == ordNo).ToList();
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var f052901Repo = new F052901Repository(Schemas.CoreSchema, _wmsTransaction);
			var f055001Repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051201Repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema);
			foreach (var f05030101 in items)
			{
				var f050801Item = f050801Repo.Filter(
						o =>
								o.DC_CODE == f05030101.DC_CODE && o.GUP_CODE == f05030101.GUP_CODE && o.CUST_CODE == f05030101.CUST_CODE &&
								o.WMS_ORD_NO == f05030101.WMS_ORD_NO).FirstOrDefault();
				if (f050801Item != null)
				{
					f050801Item.PICK_TIME = newPickTime;
					f050801Repo.Update(f050801Item);
				}
				var f052901Item =
						f052901Repo.Filter(
								o =>
										o.DC_CODE == f05030101.DC_CODE && o.GUP_CODE == f05030101.GUP_CODE && o.CUST_CODE == f05030101.CUST_CODE &&
										o.WMS_ORD_NO == f05030101.WMS_ORD_NO).FirstOrDefault();
				if (f052901Item != null)
				{
					f052901Item.PICK_TIME = newPickTime;
					f052901Repo.Update(f052901Item);
				}

				var f055001Item =
						f055001Repo.Filter(
								o =>
										o.DC_CODE == f05030101.DC_CODE && o.GUP_CODE == f05030101.GUP_CODE && o.CUST_CODE == f05030101.CUST_CODE &&
										o.WMS_ORD_NO == f05030101.WMS_ORD_NO).FirstOrDefault();
				if (f055001Item != null)
				{
					f055001Item.PICK_TIME = newPickTime;
					f055001Repo.Update(f055001Item);
				}

				var pickOrdNoList =
						f051202Repo.Filter(
								o =>
										o.DC_CODE == f05030101.DC_CODE && o.GUP_CODE == f05030101.GUP_CODE && o.CUST_CODE == f05030101.CUST_CODE &&
										o.WMS_ORD_NO == f05030101.WMS_ORD_NO).Select(o => o.PICK_ORD_NO).Distinct().ToList();
				foreach (var pickOrdNo in pickOrdNoList)
				{
					var f051201Item =
							f051201Repo.Filter(
									o =>
											o.DC_CODE == f05030101.DC_CODE && o.GUP_CODE == f05030101.GUP_CODE && o.CUST_CODE == f05030101.CUST_CODE &&
											o.PICK_ORD_NO == pickOrdNo).FirstOrDefault();
					if (f051201Item != null)
					{
						f051201Item.PICK_TIME = newPickTime;
						f051201Repo.Update(f051201Item);
					}
				}

			}

		}

		private string UpdateWorkType3(string dcCode, string gupCode, string custCode, string ordNo, string newDcCode, F050301 f050301)
		{
			var newOrdNo = SharedService.GetNewOrdCode("S");
			var f050001Repo = new F050001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050001 = new F050001();
			f050001.GUP_CODE = f050301.GUP_CODE;
			f050001.CUST_CODE = f050301.CUST_CODE;
			f050001.CUST_ORD_NO = f050301.CUST_ORD_NO;
			f050001.ORD_TYPE = f050301.ORD_TYPE;
			f050001.RETAIL_CODE = f050301.RETAIL_CODE;
			f050001.ORD_DATE = f050301.ORD_DATE;
			f050001.PROC_FLAG = "0";
			f050001.CUST_NAME = f050301.CUST_NAME;
			f050001.SELF_TAKE = f050301.SELF_TAKE;
			f050001.FRAGILE_LABEL = f050301.FRAGILE_LABEL;
			f050001.GUARANTEE = f050301.GUARANTEE;
			f050001.SA = f050301.SA;
			f050001.GENDER = f050301.GENDER;
			f050001.AGE = f050301.AGE;
			f050001.SA_QTY = f050301.SA_QTY;
			f050001.SA_CHECK_QTY = f050301.SA_CHECK_QTY;
			f050001.TEL = f050301.TEL;
			f050001.ADDRESS = f050301.ADDRESS;
			f050001.ORDER_BY = f050301.ORDER_BY;
			f050001.CONSIGNEE = f050301.CONSIGNEE;
			f050001.ARRIVAL_DATE = f050301.ARRIVAL_DATE;
			f050001.TRAN_CODE = f050301.TRAN_CODE;
			f050001.SP_DELV = f050301.SP_DELV;
			f050001.CUST_COST = f050301.CUST_COST;
			f050001.BATCH_NO = f050301.BATCH_NO;
			f050001.CHANNEL = f050301.CHANNEL;
			f050001.POSM = f050301.POSM;
			f050001.CONTACT = f050301.CONTACT;
			f050001.CONTACT_TEL = f050301.CONTACT_TEL;
			f050001.TEL_2 = f050301.TEL_2;
			f050001.SPECIAL_BUS = f050301.SPECIAL_BUS;
			f050001.ALL_ID = f050301.ALL_ID;
			f050001.COLLECT = f050301.COLLECT;
			f050001.COLLECT_AMT = f050301.COLLECT_AMT;
			f050001.MEMO = f050301.MEMO;
			f050001.SAP_MODULE = f050301.SAP_MODULE;
			f050001.SOURCE_TYPE = f050301.SOURCE_TYPE;
			f050001.SOURCE_NO = f050301.SOURCE_NO;
			f050001.TYPE_ID = f050301.TYPE_ID;
			f050001.CAN_FAST = f050301.CAN_FAST;
			f050001.TEL_1 = f050301.TEL_1;
			f050001.TEL_AREA = f050301.TEL_AREA;
			f050001.PRINT_RECEIPT = f050301.PRINT_RECEIPT;
			f050001.RECEIPT_NO = f050301.RECEIPT_NO;
			f050001.ZIP_CODE = f050301.ZIP_CODE;
			f050001.TICKET_ID = f050301.TICKET_ID;
			f050001.ORD_NO = newOrdNo;
			f050001.DC_CODE = newDcCode;
			f050001Repo.Add(f050001);
			var f050101Repo = new F050101Repository(Schemas.CoreSchema, _wmsTransaction);
			f050101Repo.Add(CreateF050101(f050301, newOrdNo));
			var f050302Repo = new F050302Repository(Schemas.CoreSchema);
			var items =
					f050302Repo.Filter(
							o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ORD_NO == ordNo).ToList();
			var f050002Repo = new F050002Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050102Repo = new F050102Repository(Schemas.CoreSchema, _wmsTransaction);
			foreach (var f050302 in items)
			{
				var f050002 = new F050002();
				f050002.ORD_SEQ = f050302.ORD_SEQ;
				f050002.CUST_CODE = f050302.CUST_CODE;
				f050002.GUP_CODE = f050302.GUP_CODE;
				f050002.ITEM_CODE = f050302.ITEM_CODE;
				f050002.ORD_QTY = f050302.ORD_QTY;
				f050002.SERIAL_NO = f050302.SERIAL_NO;
				f050002.DC_CODE = newDcCode;
				f050002.ORD_NO = newOrdNo;
				f050002Repo.Add(f050002);
				f050102Repo.Add(CreateF050102(f050302, newOrdNo));
			}

			return newOrdNo;
		}

		private List<F050801> UpdateWorkType4(string dcCode, string gupCode, string custCode, string ordNo, string selfTake, string printPass)
		{
			var f050101Repo = new F050101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050101 =
					f050101Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ORD_NO == ordNo);
			f050101.SELF_TAKE = selfTake;
			f050101Repo.Update(f050101);
			var f050801s = new List<F050801>();

			var f05030101Repo = new F05030101Repository(Schemas.CoreSchema);
			var items =
					f05030101Repo.Filter(
							o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ORD_NO == ordNo).ToList();
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			foreach (var f05030101 in items)
			{
				var f050801Item = f050801Repo.Filter(
						o =>
								o.DC_CODE == f05030101.DC_CODE && o.GUP_CODE == f05030101.GUP_CODE && o.CUST_CODE == f05030101.CUST_CODE &&
								o.WMS_ORD_NO == f05030101.WMS_ORD_NO).FirstOrDefault();
				if (f050801Item != null)
				{
					f050801Item.SELF_TAKE = selfTake;
					f050801Item.PRINT_PASS = printPass;
					f050801Item.NO_LOADING = selfTake;
					f050801Repo.Update(f050801Item);
					f050801s.Add(f050801Item);
				}
			}

			return f050801s;
		}
		private F050101 CreateF050101(F050301 f050301, string newOrdNo)
		{
			var f050101 = new F050101
			{
				ORD_NO = newOrdNo,
				CUST_ORD_NO = f050301.CUST_ORD_NO,
				ORD_TYPE = f050301.ORD_TYPE,
				RETAIL_CODE = f050301.RETAIL_CODE,
				ORD_DATE = f050301.ORD_DATE,
				STATUS = "1",
				CUST_NAME = f050301.CUST_NAME,
				SELF_TAKE = f050301.SELF_TAKE,
				FRAGILE_LABEL = f050301.FRAGILE_LABEL,
				GUARANTEE = f050301.GUARANTEE,
				SA = f050301.SA,
				GENDER = f050301.GENDER,
				AGE = f050301.AGE,
				SA_QTY = f050301.SA_QTY,
				SA_CHECK_QTY = f050301.SA_CHECK_QTY,
				TEL = f050301.TEL,
				ADDRESS = f050301.ADDRESS,
				CONSIGNEE = f050301.CONSIGNEE,
				ARRIVAL_DATE = f050301.ARRIVAL_DATE ?? DateTime.Now,
				TRAN_CODE = f050301.TRAN_CODE,
				SP_DELV = f050301.SP_DELV,
				CUST_COST = f050301.CUST_COST,
				BATCH_NO = f050301.BATCH_NO,
				CHANNEL = f050301.CHANNEL,
				POSM = f050301.POSM,
				CONTACT = f050301.CONTACT,
				CONTACT_TEL = f050301.CONTACT_TEL,
				TEL_2 = f050301.TEL_2,
				SPECIAL_BUS = f050301.SPECIAL_BUS,
				ALL_ID = f050301.ALL_ID,
				COLLECT = f050301.COLLECT,
				COLLECT_AMT = f050301.COLLECT_AMT,
				MEMO = f050301.MEMO,
				GUP_CODE = f050301.GUP_CODE,
				CUST_CODE = f050301.CUST_CODE,
				DC_CODE = f050301.DC_CODE,
				TYPE_ID = f050301.TYPE_ID,
				CAN_FAST = f050301.CAN_FAST,
				TEL_1 = f050301.TEL_1,
				TEL_AREA = f050301.TEL_AREA,
				PRINT_RECEIPT = f050301.PRINT_RECEIPT,
				RECEIPT_NO = f050301.RECEIPT_NO,
				RECEIPT_NO_HELP = f050301.RECEIPT_NO_HELP,
				//RECEIPT_TITLE="",
				//RECEIPT_ADDRESS ="",
				//BUSINESS_NO = "",
				//DISTR_CAR_NO = "",
				HAVE_ITEM_INVO = f050301.HAVE_ITEM_INVO,
				NP_FLAG = f050301.NP_FLAG,
				EXTENSION_A = f050301.EXTENSION_A,
				EXTENSION_B = f050301.EXTENSION_B,
				EXTENSION_C = f050301.EXTENSION_C,
				EXTENSION_D = f050301.EXTENSION_D,
				EXTENSION_E = f050301.EXTENSION_E,
				ROUND_PIECE = "0"
			};
			return f050101;
		}
		private F050102 CreateF050102(F050302 f050302, string newOrdNo)
		{
			var f050102 = new F050102
			{
				ORD_NO = newOrdNo,
				ORD_SEQ = f050302.ORD_SEQ,
				ITEM_CODE = f050302.ITEM_CODE,
				ORD_QTY = f050302.ORD_QTY,
				SERIAL_NO = f050302.SERIAL_NO,
				DC_CODE = f050302.DC_CODE,
				GUP_CODE = f050302.GUP_CODE,
				CUST_CODE = f050302.CUST_CODE,
				NO_DELV = f050302.NO_DELV
			};
			return f050102;
		}

		/// <summary>
		/// 若為自取的話，就將派車單與託運單暫時備份起來
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNos"></param>
		void DeleteF700102WithF050901(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			var f700102Repo = new F700102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			var f20010201Repo = new F20010201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f20010202Repo = new F20010202Repository(Schemas.CoreSchema, _wmsTransaction);

			var f700102s = f700102Repo.InWithTrueAndCondition("WMS_NO", wmsOrdNos, x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode);
			var f050901s = f050901Repo.InWithTrueAndCondition("WMS_NO", wmsOrdNos, x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode);

			f20010201Repo.BulkInsert(f700102s.Select(AutoMapper.Mapper.DynamicMap<F20010201>));
			f20010202Repo.BulkInsert(f050901s.Select(AutoMapper.Mapper.DynamicMap<F20010202>));

			foreach (var f700102 in f700102s)
			{
				f700102Repo.Delete(x => x.WMS_NO == f700102.WMS_NO && x.DC_CODE == f700102.DC_CODE && x.GUP_CODE == f700102.GUP_CODE && x.CUST_CODE == f700102.CUST_CODE);
			}

			foreach (var f050901 in f050901s)
			{
				f050901Repo.Delete(x => x.WMS_NO == f050901.WMS_NO && x.DC_CODE == f050901.DC_CODE && x.GUP_CODE == f050901.GUP_CODE && x.CUST_CODE == f050901.CUST_CODE);
			}
		}

    /// <summary>
    /// 新增訂單回檔歷程紀錄表
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="ordNos"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    public void AddF050305(string dcCode, string gupCode, string custCode, List<string> ordNos, string status, DateTime? crtDate = null)
    {
      var f050305Repo = new F050305Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05030101Repo = new F05030101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);

			if (ordNos != null && ordNos.Any())
			{
				List<string> wmsOrdNos;
				if (ordNos.First().StartsWith("P"))
				{
					#region P單

					var pickWmsNoList = f051202Repo.GetWmsOrdNoListByPickOrdNos(dcCode, gupCode, custCode, ordNos);
					wmsOrdNos = pickWmsNoList.Select(x => x.WMS_ORD_NO).Distinct().ToList();
					#endregion
				}
				else
				{
					wmsOrdNos = ordNos;
				}

				var insertF050305s = f05030101Repo.GetOrderRtnInsertDatas(dcCode, gupCode, custCode, status, wmsOrdNos).ToList();
        #region 新增訂單回檔歷程紀錄表
        if (insertF050305s.Any())
        {
          if (crtDate.HasValue)
          {
            insertF050305s.ForEach(x =>
            {
              x.CRT_DATE = crtDate.Value;
              x.CRT_STAFF = Current.Staff;
              x.CRT_NAME = Current.StaffName;
            });
            f050305Repo.BulkInsert(insertF050305s, true);
          }
          else
            f050305Repo.BulkInsert(insertF050305s);
        }
				#endregion

			}
		}

		#region 取消出貨單揀貨資料
		/// <summary>
		/// 取消出貨單揀貨資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNos"></param>
		public ExecuteResult CancelWmsOrderPick(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			#region 定義變數
			var f051201Repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051203Repo = new F051203Repository(Schemas.CoreSchema, _wmsTransaction);
			var f060201Repo = new F060201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051206Repo = new F051206Repository(Schemas.CoreSchema, _wmsTransaction);
			var f191302Repo = new F191302Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0513Repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
      var f051301Repo = new F051301Repository(Schemas.CoreSchema, _wmsTransaction);
      var f060702Repo = new F060702Repository(Schemas.CoreSchema, _wmsTransaction);
			var updF051201List = new List<F051201>();
			var updF051202List = new List<F051202>();
			var updF051203List = new List<F051203>();
			var updF1511List = new List<F1511>();
			var updF060201List = new List<F060201>();
			var addF060201List = new List<F060201>();
			var autoCanCancelStatusList = new List<string> { "0","3" };

			var returnStocks = new List<F1913>();
			var returnNewAllocationList = new List<ReturnNewAllocation>();
			var updF051202Datas = new List<F051202>();
			var updF1511Datas = new List<F1511>();
			var addF191302Datas = new List<F191302>();
			var updF051206Datas = new List<F051206>();
			var updF0513Datas = new List<F0513>();
			var addF060702List = new List<F060702>();
      var updF051301s = new List<F051301>();
			#endregion

			#region 取得要處理的資料
			// 取得出貨單揀缺紀錄[A] = 揀缺紀錄[F051206]有此出貨單紀錄，且揀缺紀錄狀態[F051206.STATUS]為0(揀缺待確認)或1(貨主待確認) 且F051206.ISDELETED = 0(未刪除)
			var f051206s = f051206Repo.AsForUpdate().GetDatasByOrderCancel(dcCode, gupCode, custCode, wmsOrdNos).ToList();

			// 取得揀貨單
			var f051201s = f051201Repo.AsForUpdate().GetDatasByWmsOrdNos(dcCode, gupCode, custCode, wmsOrdNos).ToList();

			// 揀貨單號清單
			var pickOrdNos = f051201s.Select(x => x.PICK_ORD_NO).ToList();

			// 找出揀貨單明細資料
			var f051202s = f051202Repo.AsForUpdate().GetDatasByPickOrdNos(dcCode, gupCode, custCode, pickOrdNos).ToList();

			// 取得虛擬儲位
			var f1511s = f1511Repo.AsForUpdate().GetDatas(dcCode, gupCode, custCode, pickOrdNos).ToList();

			var f0513s = f0513Repo.AsForUpdate().GetDatasByPickOrdNos(dcCode, gupCode, custCode, pickOrdNos).ToList();

			// 取得疑似遺失倉倉庫編號[B] = sharedService.GetPickLossWarehouseId
			var doubtLackWhId = SharedService.GetPickLossWarehouseId(dcCode, gupCode, custCode);

			// 取得疑似遺失倉第一個儲位[C] = sharedService.GetPickLossLoc
			var doubtLackFstLoc = SharedService.GetPickLossLoc(dcCode, doubtLackWhId);

      // 找出揀貨單明細未全部取消的出貨單
      var notAllCanceled = f051202Repo.GetNotAllCanceledOrders(dcCode, gupCode, custCode, wmsOrdNos);
      #endregion

      // 取消訂單若揀貨單還沒開始揀貨可將揀貨單明細壓成9，並回傳要做虛擬儲位回復的F1511資料，再跑虛擬儲位自動回庫存
      var groupBatchPicker = f051201s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.DELV_DATE, x.PICK_TIME }).ToList();
			foreach (var batch in groupBatchPicker)
			{
				var batchCancelQty = 0;
				var batchCancelWmsNos = new List<string>();
				foreach (var f051201 in batch)
				{
					// 是否整張揀貨單所有出貨單都取消
					var isPickAllWmsCancel = !f051202s.Where(x => x.DC_CODE == f051201.DC_CODE && x.GUP_CODE == f051201.GUP_CODE && 
					x.CUST_CODE == f051201.CUST_CODE && x.PICK_ORD_NO == f051201.PICK_ORD_NO &&  x.PICK_STATUS != "9")
						.Select(x => x.WMS_ORD_NO).Distinct().Except(wmsOrdNos).Any();

					var f060201 = f060201Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == f051201.DC_CODE &&
													x.GUP_CODE == f051201.GUP_CODE && x.CUST_CODE == f051201.CUST_CODE &&
													x.PICK_NO == f051201.PICK_ORD_NO && x.CMD_TYPE == "1").FirstOrDefault();

					if (f051201.PICK_STATUS == 0 || f051201.PICK_STATUS == 1)
					{
						var canCancel = false;
						// 找出揀貨單明細資料
						var currF051202s = f051202s.Where(x => x.PICK_ORD_NO == f051201.PICK_ORD_NO).ToList();
						#region 取消下發
						// 人工倉還沒揀貨，可以取消
						if (f051201.DISP_SYSTEM == "0" && f051201.PICK_STATUS == 0)
							canCancel = true;
						// 自動倉是否還沒揀，需判斷出庫任務是否已下發，若未下發可以取消，若已下發不可取消並新增取消出庫任務
						if (f051201.DISP_SYSTEM != "0")
						{
							if (f060201 == null)
								canCancel = true;
							else
							{
								if (autoCanCancelStatusList.Contains(f060201.STATUS))
								{
									canCancel = true;
									if (isPickAllWmsCancel)
									{
										f060201.STATUS = "9";
										f060201.MESSAGE = "尚未發送，先行取消";
										updF060201List.Add(f060201);
									}
								}
								else
								{
									if (isPickAllWmsCancel)
									{
										addF060201List.Add(new F060201
										{
											DOC_ID = f060201.DOC_ID,
											DC_CODE = f060201.DC_CODE,
											GUP_CODE = f060201.GUP_CODE,
											CUST_CODE = f060201.CUST_CODE,
											WAREHOUSE_ID = f060201.WAREHOUSE_ID,
											PICK_NO = f060201.PICK_NO,
											CMD_TYPE = "2",
											WMS_NO = f060201.WMS_NO,
											STATUS = "0"
										});
									}
								}
							}
								
						}
						
						#endregion

						// 可以取消，調整揀貨單、揀貨單明細、揀貨總量明細、並將虛擬儲位庫存進行回復
						if (canCancel)
						{
							#region 調整揀貨單、揀貨單明細、揀貨總量明細、並將虛擬儲位庫存進行回復
							

							// 找出虛擬儲位檔資料
							var currF1511s = f1511s.Where(x => x.ORDER_NO == f051201.PICK_ORD_NO).ToList();

							foreach (var f051202 in currF051202s)
							{
								if (wmsOrdNos.Contains(f051202.WMS_ORD_NO))
								{
									var f1511 = currF1511s.First(x => x.ORDER_SEQ == f051202.PICK_ORD_SEQ);
									updF1511List.Add(f1511);
									f051202.PICK_STATUS = "9";
									batchCancelQty += f051202.B_PICK_QTY;

									if (!batchCancelWmsNos.Contains(f051202.WMS_ORD_NO))
										batchCancelWmsNos.Add(f051202.WMS_ORD_NO);

									updF051202List.Add(f051202);
								}
							}
							if (currF051202s.All(x => x.PICK_STATUS == "9"))
							{
								f051201.PICK_STATUS = 9;
								updF051201List.Add(f051201);
							}
							// 找出揀貨單總揀明細資料
							var f051203s = f051203Repo.GetDataByPickNo(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, f051201.PICK_ORD_NO).ToList();
							foreach (var f051203 in f051203s)
							{
								var qty = currF051202s.Where(x => x.PICK_LOC == f051203.PICK_LOC && x.ITEM_CODE == f051203.ITEM_CODE &&
								x.MAKE_NO == f051203.MAKE_NO && x.SERIAL_NO == f051203.SERIAL_NO && x.PICK_STATUS == "0").Sum(x => x.B_PICK_QTY);
								if (qty == 0)
									f051203.PICK_STATUS = "9";
								f051203.B_PICK_QTY = qty;
								updF051203List.Add(f051203);
							}
							#endregion							
						}					
					}

					#region 未確認揀缺紀錄要自動結案，並將缺貨記錄搬到疑似遺失倉且寫入庫存異常紀錄
					var currF051206s = f051206s.Where(x => x.PICK_ORD_NO == f051201.PICK_ORD_NO).ToList();

          foreach (var f051206 in currF051206s)
					{
						// 取得揀貨明細
						var currF051202 = f051202s.Where(x => x.PICK_ORD_NO == f051206.PICK_ORD_NO && x.PICK_ORD_SEQ == f051206.PICK_ORD_SEQ).FirstOrDefault();

						// 取得虛擬庫存明細
						var currF1511 = f1511s.Where(x => x.ORDER_NO == f051206.PICK_ORD_NO && x.ORDER_SEQ == f051206.PICK_ORD_SEQ).FirstOrDefault();

						// 執行庫存異常處理
						var crtStockLackProRes = SharedService.CreateStockLackProcess(new StockLack
						{
							DcCode = f051206.DC_CODE,
							GupCode = f051206.GUP_CODE,
							CustCode = f051206.CUST_CODE,
							LackQty = Convert.ToInt32(f051206.LACK_QTY),
							PickLackWarehouseId = doubtLackWhId,
							PickLackLocCode = doubtLackFstLoc,
							F051202 = currF051202,
							F1511 = currF1511,
							ReturnStocks = returnStocks
						});

            if (!crtStockLackProRes.IsSuccessed)
              return crtStockLackProRes;

            returnStocks = crtStockLackProRes.ReturnStocks;
						returnNewAllocationList.AddRange(crtStockLackProRes.ReturnNewAllocations);
						updF051202Datas.Add(crtStockLackProRes.UpdF051202);
						updF1511Datas.Add(crtStockLackProRes.UpdF1511);
						addF191302Datas.AddRange(crtStockLackProRes.AddF191302List);

						//	F051206.STATUS=2(已確認)、F051206.PROC_FLAG = 2(訂單被取消)
						f051206.STATUS = "2";
						f051206.RETURN_FLAG = "2";
						updF051206Datas.Add(f051206);
					}
          #endregion

          #region 集貨等待通知(異常處理)
          // 如果自動倉[F051201.DISP_SYSTEM=1]揀貨單狀態為揀貨中、揀貨完成[F051201.STATUS IN (1,2)]且出庫任務已經派發成功[F060201.STATUS=2]且整張揀貨單所有出貨單都取消
          // 跨庫揀貨單不需要集貨，所以不需要發送集貨等待通知
          // 產生F060702 集貨等待通知 集貨場狀態設為2(異常處理)
          if (f051201.DISP_SYSTEM == "1")
          {
            var f051301 = f051301Repo.Find(x => x.DC_CODE == f051201.DC_CODE && x.GUP_CODE == f051201.GUP_CODE && x.CUST_CODE == f051201.CUST_CODE && x.DELV_DATE == f051201.DELV_DATE && x.PICK_TIME == f051201.PICK_TIME && x.WMS_NO == f060201.WMS_NO);

            if (f051301 != null)
            {
              if ((f051201.PICK_STATUS == 1 || f051201.PICK_STATUS == 2) && f051201.DISP_SYSTEM == "1" && f051201.ORD_TYPE == "1" && f051201.NEXT_STEP != ((int)NextStep.CrossAllotPier).ToString() && isPickAllWmsCancel && f060201 != null && f060201.STATUS == "2")
              {
                addF060702List.Add(new F060702
                {
                  DC_CODE = f060201.DC_CODE,
                  GUP_CODE = f060201.GUP_CODE,
                  CUST_CODE = f060201.CUST_CODE,
                  ORDER_CODE = f060201.DOC_ID,
                  ORI_ORDER_CODE = f060201.WMS_NO,
                  STATUS = 2, // 集貨場狀態=2(異常處理)
                  PROC_FLAG = "0" //待處理
                });

                //如果集貨位置是自動集貨場，要改成人工集貨場
                if (f051301.COLLECTION_POSITION == "1")
                  f051301Repo.UpdateCollectionPosition(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE, f051201.DELV_DATE.Value, f051201.PICK_TIME, f060201.WMS_NO, "0");
              }
            }
          }
          #endregion
        }

        #region 更新F0513 批次統計
        var f0513 = f0513s.First(x => x.DC_CODE == batch.Key.DC_CODE && x.GUP_CODE == batch.Key.GUP_CODE && x.CUST_CODE == batch.Key.CUST_CODE &&
											x.DELV_DATE == batch.Key.DELV_DATE && x.PICK_TIME == batch.Key.PICK_TIME);
				
				// 更新批次總PCS數
				f0513.PICK_CNT -= batchCancelQty;
				// 更新批次總出貨單數
				f0513.SHIP_CNT -= batchCancelWmsNos.Count;

				// 如果批次總出貨單數=0 該批次狀態更新為9(取消)
				if (f0513.SHIP_CNT == 0)
					f0513.PROC_FLAG = "9";

				var batchCancelPickers = updF051201List.Where(x => x.DC_CODE == batch.Key.DC_CODE && x.GUP_CODE == batch.Key.GUP_CODE && x.CUST_CODE == batch.Key.CUST_CODE &&
											x.DELV_DATE == batch.Key.DELV_DATE && x.PICK_TIME == batch.Key.PICK_TIME && x.PICK_STATUS == 9);
				if (batchCancelPickers.Any())
				{
					foreach(var f051201 in batchCancelPickers)
					{
						if (f051201.DISP_SYSTEM == "0")
						{
							if (f051201.PICK_TOOL == "1")
							{
								if (f051201.SPLIT_TYPE == "03")
									f0513.ATFL_N_PICK_CNT -= 1;
								else if (f051201.PICK_TYPE == "4")
									f0513.ATFL_S_PICK_CNT -= 1;
								else
									f0513.ATFL_B_PICK_CNT -= 1;
							}
							else
							{
								if (f051201.SPLIT_TYPE == "03")
									f0513.ATFL_NP_PICK_CNT -= 1;
								else if (f051201.PICK_TYPE == "4")
									f0513.ATFL_SP_PICK_CNT -= 1;
								else
									f0513.ATFL_BP_PICK_CNT -= 1;
							}
						}
						else
						{
							if (f051201.PICK_TYPE == "4")
								f0513.AUTO_S_PICK_CNT -= 1;
							else
								f0513.AUTO_N_PICK_CNT -= 1;
						}
					}
				}
				updF0513Datas.Add(f0513);
				#endregion
			}


			#region 資料異動
			if (returnNewAllocationList.Any())
			{
				// 調撥單整批上架
				SharedService.BulkAllocationToAllUp(returnNewAllocationList, returnStocks, false, addF191302Datas);
				// 調撥單整批寫入
				SharedService.BulkInsertAllocation(returnNewAllocationList, returnStocks, true);
			}

			if (updF1511List.Any())
				RestoreVirtualStock(updF1511List);
			if (updF051201List.Any())
				f051201Repo.BulkUpdate(updF051201List);
			if (updF051202List.Any())
				f051202Repo.BulkUpdate(updF051202List);
			if (updF051203List.Any())
				f051203Repo.BulkUpdate(updF051203List);
			if (updF051202Datas.Any())
				f051202Repo.BulkUpdate(updF051202Datas);
			if (updF1511Datas.Any())
				f1511Repo.BulkUpdate(updF1511Datas);
			if (updF051206Datas.Any())
				f051206Repo.BulkUpdate(updF051206Datas);
			if (addF191302Datas.Any())
				f191302Repo.BulkInsert(addF191302Datas);
			if (updF0513Datas.Any())
				f0513Repo.BulkUpdate(updF0513Datas);
			if (addF060201List.Any())
				f060201Repo.BulkInsert(addF060201List);
			if (updF060201List.Any())
				f060201Repo.BulkUpdate(updF060201List);
			if (addF060702List.Any())
				f060702Repo.BulkInsert(addF060702List, "ID");
      if (updF051301s.Any())
        f051301Repo.BulkUpdate(updF051301s);

      return new ExecuteResult(true);
			#endregion

		}


		#endregion

		#region 虛擬庫存直接回庫
		/// <summary>
		/// 虛擬庫存直接回庫
		/// </summary>
		/// <param name="f1511s"></param>
		public void RestoreVirtualStock(List<F1511> f1511s)
		{
			if (f1511s.Any())
			{
				var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
				var group = f1511s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE }).ToList();

				foreach (var custVirtualStock in group)
				{
					StockService.AddStock(f1511s.Select(x => new OrderStockChange
					{
						DcCode = x.DC_CODE,
						GupCode = x.GUP_CODE,
						CustCode = x.CUST_CODE,
						LocCode = x.LOC_CODE,
						ItemCode = x.ITEM_CODE,
						MakeNo = x.MAKE_NO,
						EnterDate = x.ENTER_DATE.Value,
						VnrCode = "000000",
						VaildDate = x.VALID_DATE.Value,
						SerialNo = x.SERIAL_NO,
						BoxCtrlNo = x.BOX_CTRL_NO,
						PalletCtrlNo = x.PALLET_CTRL_NO,
						Qty = x.B_PICK_QTY,
						WmsNo = x.ORDER_NO
					}).ToList());
					// 異動庫存要更新儲位容積
					SharedService.UpdateUsedVolumnByLocCodes(custVirtualStock.Key.DC_CODE, custVirtualStock.Key.GUP_CODE, custVirtualStock.Key.CUST_CODE, custVirtualStock.Select(x => x.LOC_CODE).Distinct().ToList());
					custVirtualStock.ToList().ForEach(x =>
					{
						x.STATUS = "9";
					});
				}
				StockService.SaveChange();
				f1511Repo.BulkUpdate(f1511s);
			}
		}

    #endregion




    /// <summary>
    /// 出貨箱宅單扣帳
    /// </summary>
    /// <param name="f050801"></param>
    /// <param name="packageBoxNo"></param>
    /// <param name="auditDate"></param>
    /// <param name="boxNum"></param>
    /// <returns></returns>
    public ExecuteResult PackageBoxDebit(F050801 f050801, short packageBoxNo, string pastNo, string workStationCode, string boxNum,
      DateTime? auditDate = null, string clientPc = null, string sorterCode = null)
    {
      pastNo = pastNo?.ToUpper();
      boxNum = boxNum?.ToUpper();
      workStationCode = workStationCode?.ToUpper();
      var f055001Repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);
      var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
      var f055005Repo = new F055005Repository(Schemas.CoreSchema, _wmsTransaction);
      if (f050801.STATUS == 2)
      {
        f050801.STATUS = 6;//更新出貨單狀態為已扣帳
        f050801Repo.Update(f050801);
      }
      if (string.IsNullOrWhiteSpace(clientPc))
        clientPc = Current.DeviceIp;
      // 更新出貨箱已扣帳
      f055001Repo.UpdateToAudit(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, f050801.WMS_ORD_NO, packageBoxNo, "1", auditDate ?? DateTime.Now, Current.Staff, Current.StaffName, boxNum, clientPc, sorterCode);

      f055005Repo.Add(new F055005
			{
				DC_CODE = f050801.DC_CODE,
				GUP_CODE = f050801.GUP_CODE,
				CUST_CODE = f050801.CUST_CODE,
				PAST_NO = pastNo,
				WMS_NO = f050801.WMS_ORD_NO,
				PROC_FLAG = "0"
			}, "ID");

			
			return new ExecuteResult(true);
		}

		/// 批次扣帳
		/// </summary>
		/// <param name="batchDelvs">批次扣帳設定清單</param>
		public ExecuteResult DoBatchDebit(List<BatchDelv> batchDelvs, out List<F050801> updF050801s, out List<F050802> updF050802s, out List<F0513> updF0513s, out List<F05030202> updF05030202s, out List<F1511> updF1511s)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050802Repo = new F050802Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0513Repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
			updF0513s = new List<F0513>();
			updF05030202s = new List<F05030202>();
			updF050801s = new List<F050801>();
			updF050802s = new List<F050802>();
			updF1511s = new List<F1511>();

			var f05030101Repo = new F05030101Repository(Schemas.CoreSchema);
			var f05030202Repo = new F05030202Repository(Schemas.CoreSchema, _wmsTransaction);
			var serialNoCancelService = new SerialNoCancelService(_wmsTransaction);

			foreach (var batchDelv in batchDelvs)
			{
				//取得此批次所有未扣帳未出貨的出貨單
				var batchCanDebitWmsOrdNos = f050801Repo.AsForUpdate()
					.GetNotDebitAndNotCancelShipOrdersByBatch(batchDelv.DC_CODE, batchDelv.GUP_CODE, batchDelv.CUST_CODE, batchDelv.DELV_DATE, batchDelv.PICK_TIME)
					.Where(x => x.STATUS == 2).ToList();

				if (!batchCanDebitWmsOrdNos.Any())
					continue;

				var result = MultiShipOrderDebit(batchDelv.DC_CODE, batchDelv.GUP_CODE, batchDelv.CUST_CODE, batchCanDebitWmsOrdNos, out updF050801s, out updF050802s, out updF05030202s, out updF1511s, out updF0513s);
				if (!result.IsSuccessed)
					return result;
			}
			return new ExecuteResult(true, Properties.Resources.DebitComplete);
		}

			/// <summary>
			/// 單筆出貨單扣帳
			/// </summary>
			/// <param name="dcCode"></param>
			/// <param name="gupCode"></param>
			/// <param name="custCode"></param>
			/// <param name="wmsOrdNo"></param>
			/// <returns></returns>
			public ExecuteResult OneShipOrderDebit(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			return MultiShipOrderDebit(dcCode, gupCode, custCode, new List<string> { wmsOrdNo });
		}
		/// <summary>
		/// 多筆出貨單進行扣帳
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNos"></param>
		/// <returns></returns>
		public ExecuteResult MultiShipOrderDebit(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050802Repo = new F050802Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05030202Repo = new F05030202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0513Repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
			var updF050801s = new List<F050801>();
			var updF050802s = new List<F050802>();
			var updF05030202s = new List<F05030202>();
			var updF1511s = new List<F1511>();
			var updF0513s = new List<F0513>();
			var canDebitWmsOrders = f050801Repo.GetDatasForWmsOrdNos(dcCode, gupCode, custCode, wmsOrdNos).ToList();
			var res = MultiShipOrderDebit(dcCode, gupCode, custCode, canDebitWmsOrders, out updF050801s, out updF050802s, out updF05030202s, out updF1511s,out updF0513s);
			if (!res.IsSuccessed)
				return res;

			f050801Repo.BulkUpdate(updF050801s);
			f050802Repo.BulkUpdate(updF050802s);
			f1511Repo.BulkUpdate(updF1511s);
			f05030202Repo.BulkUpdate(updF05030202s);
			f0513Repo.BulkUpdate(updF0513s);
			return new ExecuteResult { IsSuccessed = true, Message = "" };
		}
		/// <summary>
		/// 多筆出貨單進行扣帳
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNos"></param>
		public ExecuteResult MultiShipOrderDebit(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos,
			out List<F050801> updF050801s, out List<F050802> updF050802s, out List<F05030202> updF05030202s, out List<F1511> updF1511s, out List<F0513> updF0513s)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var canDebitWmsOrders = f050801Repo.GetDatasForWmsOrdNos(dcCode, gupCode, custCode, wmsOrdNos).ToList();
			var res = MultiShipOrderDebit(dcCode, gupCode, custCode, canDebitWmsOrders, out updF050801s, out updF050802s, out updF05030202s, out updF1511s, out updF0513s);
			if (!res.IsSuccessed)
				return res;
			return new ExecuteResult { IsSuccessed = true, Message = "" };
		}
		/// <summary>
		/// 單筆出貨單進行扣帳
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNos"></param>
		public ExecuteResult MultiShipOrderDebit(string dcCode, string gupCode, string custCode, F050801 canDebitWmsOrder,
			out List<F050801> updF050801s, out List<F050802> updF050802s, out List<F05030202> updF05030202s, out List<F1511> updF1511s, out List<F0513> updF0513s)
		{
			var res = MultiShipOrderDebit(dcCode, gupCode, custCode, new List<F050801> { canDebitWmsOrder }
										, out updF050801s, out updF050802s, out updF05030202s, out updF1511s, out updF0513s);
			if (!res.IsSuccessed)
				return res;
			return new ExecuteResult { IsSuccessed = true, Message = "" };
		}

		/// <summary>
		/// 多筆出貨單進行扣帳共用
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNos"></param>
		/// <returns></returns>
		private ExecuteResult MultiShipOrderDebit(string dcCode, string gupCode, string custCode, List<F050801> canDebitWmsOrders,
			out List<F050801> updF050801s, out List<F050802> updF050802s, out List<F05030202> updF05030202s, out List<F1511> updF1511s,out List<F0513> updF0513s)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050802Repo = new F050802Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05030202Repo = new F05030202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0513Repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
			var f056001Repo = new F056001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f056002Repo = new F056002Repository(Schemas.CoreSchema, _wmsTransaction);
			var f055001Repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);
			var canDebitWmsOrdNos = canDebitWmsOrders.Select(x => x.WMS_ORD_NO).ToList();
			updF050801s = new List<F050801>();
			updF050802s = new List<F050802>();
			updF05030202s = new List<F05030202>();
			updF1511s = new List<F1511>();
			updF0513s = new List<F0513>();
			var updF056001List = new List<F056001>();
			var addF056002List = new List<F056002>();
			// 取得出貨箱資訊
			var f055001Datas = f055001Repo.GetDatasByWmsOrdNos(dcCode, gupCode, custCode, canDebitWmsOrdNos).ToList();
			if(f055001Datas.Any())
			{
				// 檢查出貨單所有宅配單是否都扣帳
				if (f055001Datas.Where(x => !string.IsNullOrEmpty(x.PAST_NO)).Min(x => x.STATUS) == "0")
				{
					var noDebitWmsNos = f055001Datas.Where(x => !string.IsNullOrEmpty(x.PAST_NO)).Select(x => x.WMS_ORD_NO).Distinct().ToList();
					return new ExecuteResult(false, string.Format("出貨單{0}還有宅配單尚未宅單扣帳，不可出貨",string.Join("、", noDebitWmsNos)));
				}
					

				#region 由包裝工作站包裝，進行工作站紙箱扣帳與補紙箱補貨通知處理
				var datas = f055001Datas.Where(x => !string.IsNullOrWhiteSpace(x.WORKSTATION_CODE))
				.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.WORKSTATION_CODE, x.BOX_NUM });
				foreach (var item in datas)
				{
					var f056001 = f056001Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == item.Key.DC_CODE && x.GUP_CODE == item.Key.GUP_CODE &&
					x.CUST_CODE == item.Key.CUST_CODE && x.WORKSTATION_CODE == item.Key.WORKSTATION_CODE && x.BOX_CODE == item.Key.BOX_NUM).FirstOrDefault();
					if (f056001 != null)
					{
						f056001.QTY -= item.Count();
						updF056001List.Add(f056001);
						if (f056001.QTY <= f056001.SAVE_QTY)
						{
							var f056002 = f056002Repo.GetF056002(f056001.DC_CODE, f056001.GUP_CODE, f056001.CUST_CODE, f056001.WORKSTATION_CODE, f056001.BOX_CODE);
							if (f056002 == null)
							{
								addF056002List.Add(new F056002
								{
									DC_CODE = f056001.DC_CODE,
									GUP_CODE = f056001.GUP_CODE,
									CUST_CODE = f056001.CUST_CODE,
									WORKSTATION_CODE = f056001.WORKSTATION_CODE,
									BOX_CODE = f056001.BOX_CODE,
									FLOOR = f056001.FLOOR,
									STATUS = "0"
								});
							}
						}
					}
				}
				if (updF056001List.Any())
					f056001Repo.BulkUpdate(updF056001List);
				if (addF056002List.Any())
					f056002Repo.BulkInsert(addF056002List);
				#endregion
			}


			// 出貨單扣帳
			var res = ShipOrderDebitProcess(dcCode, gupCode, custCode, canDebitWmsOrders, out updF050801s, out updF050802s, out updF05030202s, out updF1511s);
			if (!res.IsSuccessed)
				return res;


			var group = canDebitWmsOrders.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.DELV_DATE, x.PICK_TIME }).ToList();
			foreach (var g in group)
			{
				//取得此批次所有未扣帳未出貨的出貨單
				var batchDelvAllWmsOrders = f050801Repo.AsForUpdate()
					.GetNotDebitAndNotCancelShipOrdersByBatch(g.Key.DC_CODE, g.Key.GUP_CODE, g.Key.CUST_CODE, g.Key.DELV_DATE, g.Key.PICK_TIME)
					.Select(x => x.WMS_ORD_NO).ToList();
				if (!batchDelvAllWmsOrders.Except(canDebitWmsOrdNos).Any())
				{
					var f0513 = f0513Repo.Find(x => x.DC_CODE == g.Key.DC_CODE && x.GUP_CODE == g.Key.GUP_CODE && x.CUST_CODE == g.Key.CUST_CODE 
					&& x.DELV_DATE == g.Key.DELV_DATE && x.PICK_TIME == g.Key.PICK_TIME);
					f0513.PROC_FLAG = "6";
					updF0513s.Add(f0513);
				}
			}

			return new ExecuteResult { IsSuccessed = true, Message = "" };
		}


		/// <summary>
		/// 出貨單扣帳共用(不含批次扣帳更新、工作站紙箱扣帳與補紙箱補貨通知處理)
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="canDebitWmsOrders"></param>
		/// <param name="updF050801s"></param>
		/// <param name="updF050802s"></param>
		/// <param name="updF05030202s"></param>
		/// <returns></returns>
		private ExecuteResult ShipOrderDebitProcess(string dcCode,string gupCode,string custCode,List<F050801> canDebitWmsOrders,
			out List<F050801> updF050801s,out List<F050802> updF050802s,out List<F05030202> updF05030202s,out List<F1511> updF1511s)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050802Repo = new F050802Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05030202Repo = new F05030202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05030101Repo = new F05030101Repository(Schemas.CoreSchema);

			updF05030202s = new List<F05030202>();
			updF050801s = new List<F050801>();
			updF050802s = new List<F050802>();
			updF1511s = new List<F1511>();

			var canDebitWmsOrdNos = canDebitWmsOrders.Select(x => x.WMS_ORD_NO).ToList();
			var f05030101s = f05030101Repo.GetDatas(dcCode,gupCode,custCode, canDebitWmsOrdNos).ToList();
			var f05030202s = f05030202Repo.AsForUpdate().GetDatas(dcCode, gupCode, custCode, canDebitWmsOrdNos).ToList();
			var f1511WithF051202s = f1511Repo.GetF1511sByWmsOrdNo(dcCode, gupCode, custCode, canDebitWmsOrdNos).ToList();
			var f050802s = f050802Repo.AsForUpdate().GetDatas(dcCode, gupCode, custCode, canDebitWmsOrdNos).ToList();

			// 扣帳前，回填實際出貨量
			foreach (var f050802 in f050802s)
			{
				var f1511WithF051202 = f1511WithF051202s.Find(x => x.DC_CODE == f050802.DC_CODE && x.GUP_CODE == f050802.GUP_CODE && x.CUST_CODE == f050802.CUST_CODE && x.WMS_ORD_NO == f050802.WMS_ORD_NO && x.ITEM_CODE == f050802.ITEM_CODE && x.SERIAL_NO == f050802.SERIAL_NO);
				if (f1511WithF051202 == null)
					return new ExecuteResult(false, string.Format(Properties.Resources.SERIAL_NO_NotFound, f050802.ITEM_CODE, f050802.SERIAL_NO));

				f050802.A_DELV_QTY = f1511WithF051202.A_PICK_QTY_SUM;
				updF050802s.Add(f050802);

				//更新訂單明細實際出貨數
				var wmsF05030202s = f05030202s.Where(x => x.DC_CODE == f050802.DC_CODE && x.GUP_CODE == f050802.GUP_CODE && x.CUST_CODE == f050802.CUST_CODE && x.WMS_ORD_NO == f050802.WMS_ORD_NO && x.WMS_ORD_SEQ == f050802.WMS_ORD_SEQ).OrderBy(x => x.ORD_NO).ThenBy(x => x.ORD_SEQ).ToList();
				var aDelvQty = f050802.A_DELV_QTY;
				foreach (var wmsF05030202 in wmsF05030202s)
				{
					if (aDelvQty == 0)
					{
						wmsF05030202.A_DELV_QTY = 0;
						updF05030202s.Add(wmsF05030202);
					}
					else if (wmsF05030202.B_DELV_QTY >= aDelvQty)
					{
						wmsF05030202.A_DELV_QTY = aDelvQty;
						aDelvQty = 0;
						updF05030202s.Add(wmsF05030202);
					}
					else
					{
						wmsF05030202.A_DELV_QTY = wmsF05030202.B_DELV_QTY;
						aDelvQty -= wmsF05030202.B_DELV_QTY;
						updF05030202s.Add(wmsF05030202);
					}
				}
			}
			// by出貨單據更新F1511虛擬儲位狀態為已扣帳
			var f1511s = f1511Repo.GetDatasByWmsOrdNos(dcCode, gupCode, custCode, canDebitWmsOrdNos).ToList();
			f1511s.ForEach(x =>
			{
				x.STATUS = "2";
			});
			updF1511s.AddRange(f1511s);

			foreach (var f050801 in canDebitWmsOrders)
			{
				if (f050801.NO_LOADING == "1")
				{
					f050801.STATUS = 5; // 已出貨
					f050801.INCAR_DATE = DateTime.Now;
					f050801.INCAR_NAME = Current.StaffName;
					f050801.INCAR_STAFF = Current.Staff;
				}
				else
					f050801.STATUS = 6; //已扣帳
				f050801.APPROVE_DATE = DateTime.Now;

				updF050801s.Add(f050801);

			}

			// 上面已經將出貨單狀態都改為扣帳，接著這邊就能一次將所有出貨單押來源單據狀態
			var groupStatusList = canDebitWmsOrders.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.STATUS });
			foreach (var status in groupStatusList)
			{
				var updateSourceResult = SharedService.UpdateSourceNoStatus(SourceType.Order, status.Key.DC_CODE, status.Key.GUP_CODE, status.Key.CUST_CODE, status.Select(x => x.WMS_ORD_NO), status.Key.STATUS.ToString());
				if (!updateSourceResult.IsSuccessed)
					return updateSourceResult;
			}


			// 新增訂單回檔歷程紀錄表
			AddF050305(dcCode, gupCode, custCode, canDebitWmsOrdNos, "5");

			// 序號刪除任務觸發
			SerialNoCancelService.CreateSerialNoCancel(dcCode, gupCode, custCode, canDebitWmsOrdNos);
			return new ExecuteResult(true);
		}
	}
}
