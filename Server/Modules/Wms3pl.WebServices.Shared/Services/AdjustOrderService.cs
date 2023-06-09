using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F20;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;

namespace Wms3pl.WebServices.Shared.Services
{
	public partial class AdjustOrderService
	{
		private WmsTransaction _wmsTransaction;
		private List<F1951> _causeList;
		private CommonService _commonSerivce;

		public CommonService CommonService
		{
			get
			{
				if (_commonSerivce == null)
					_commonSerivce = new CommonService();
				return _commonSerivce;
			}
			set
			{
				_commonSerivce = value;
			}
		}

		public SharedService SharedService { get; set; }

		public Stack<string> NewOrderNoList { get; set; }

		public AdjustOrderService(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
			SharedService = new SharedService(_wmsTransaction);
		}


		private void LoadCauseMemo()
		{
			if(_causeList == null)
			{
				var f1951Repo = new F1951Repository(Schemas.CoreSchema);
				_causeList = f1951Repo.GetDatasByTrueAndCondition(o => o.UCT_ID == "AI").ToList();
			}
		}

		public string GetNewOrderNo()
		{
			if (NewOrderNoList != null && NewOrderNoList.Any())
				return NewOrderNoList.Pop();
			else
				return SharedService.GetNewOrdCode("J");
		}

		/// <summary>
		/// 建立調整單(單筆)，若有一筆商品庫存要調出(worktype=1)要在呼叫時進行配庫商品鎖定
		/// </summary>
		/// <param name="adjustOrderParam"></param>
		/// <returns></returns>
		public ExecuteResult CreateAdjustOrder(AdjustOrderParam adjustOrderParam)
		{
			return CreateAdjustOrder(new List<AdjustOrderParam> { adjustOrderParam });
		}

		/// <summary>
		/// 建立調整單(多筆)，若有一筆商品庫存要調出(worktype=1)要在呼叫時進行配庫商品鎖定
		/// </summary>
		/// <param name="adjustOrderParam"></param>
		/// <returns></returns>
		public ExecuteResult CreateAdjustOrder(List<AdjustOrderParam> adjustOrderParamList)
		{
			var stockService = new StockService(_wmsTransaction);
			var ajdustNoList = new List<string>();
			foreach (var adjustOrderParam in adjustOrderParamList)
			{
				if (adjustOrderParam.AdjustStockDetails == null)
					return new ExecuteResult(false, "無調整明細，不可建立調整單");

				var f200101Repo = new F200101Repository(Schemas.CoreSchema, _wmsTransaction);
				var f200103Repo = new F200103Repository(Schemas.CoreSchema, _wmsTransaction);
				var f20010301Repo = new F20010301Repository(Schemas.CoreSchema, _wmsTransaction);
				var f191303Repo = new F191303Repository(Schemas.CoreSchema, _wmsTransaction);
				var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
				

				var addF2501List = new List<F2501>();
				var updF2501List = new List<F2501>();
				var addF200103List = new List<F200103>();
				var addF20010301List = new List<F20010301>();
				var addF191303List = new List<F191303>();
				var addStockList = new List<OrderStockChange>();
				var rmStockList = new List<OrderStockChange>();


				var itemCodes = adjustOrderParam.AdjustStockDetails.Select(x => x.ItemCode).Distinct().ToList();
				var f1903s = CommonService.GetProductList(adjustOrderParam.GupCode, adjustOrderParam.CustCode, itemCodes);

				LoadCauseMemo();
				var logSeq = 1;
				var adjustNo = GetNewOrderNo();

				#region 建立調整單頭檔
				var f200101 = new F200101
				{
					DC_CODE = adjustOrderParam.DcCode,
					GUP_CODE = adjustOrderParam.GupCode,
					CUST_CODE = adjustOrderParam.CustCode,
					ADJUST_DATE = DateTime.Now.Date,
					ADJUST_NO = adjustNo,
					ADJUST_TYPE = ((int)adjustOrderParam.AdjustType).ToString(),
					SOURCE_TYPE = adjustOrderParam.SourceType,
					SOURCE_NO = adjustOrderParam.SourceNo,
					STATUS = "0",
					WORK_TYPE = adjustOrderParam.WorkType
				};
				#endregion

				#region 建立調整單明細檔、序號調整、F20010301 調整單序號刷入紀錄、庫存調整

				for (var index = 0; index < adjustOrderParam.AdjustStockDetails.Count; index++)
				{
					var x = adjustOrderParam.AdjustStockDetails[index];
					var f1903 = f1903s.FirstOrDefault(y => y.ITEM_CODE == x.ItemCode);
					if (f1903 == null)
						return new ExecuteResult(false, "商品主檔不存在，無法建立調整單");
					if ((adjustOrderParam.CheckSerialItem && (f1903.BUNDLE_SERIALNO == "1" || f1903.BUNDLE_SERIALLOC == "1") && (x.SerialNoList == null || !x.SerialNoList.Any())))
						return new ExecuteResult(false, string.Format("商品{0}為序號商品，必須提供序號清單", f1903.ITEM_CODE));

					var casueMemo = string.Empty;
					if (!string.IsNullOrWhiteSpace(x.Cause) && x.Cause != "999")
						casueMemo = _causeList.FirstOrDefault(y => y.UCC_CODE == x.Cause)?.CAUSE;
					casueMemo += x.CasueMemo;

					#region 新增調整單明細
					addF200103List.Add(new F200103
					{
						DC_CODE = adjustOrderParam.DcCode,
						GUP_CODE = adjustOrderParam.GupCode,
						CUST_CODE = adjustOrderParam.CustCode,
						ADJUST_NO = adjustNo,
						ADJUST_SEQ = index + 1,
						CAUSE = x.Cause,
						CAUSE_MEMO = casueMemo,
						WAREHOUSE_ID = x.WarehouseId,
						LOC_CODE = x.LocCode,
						ITEM_CODE = x.ItemCode,
						VALID_DATE = x.ValidDate,
						ENTER_DATE = x.EnterDate,
						MAKE_NO = x.MakeNo,
						VNR_CODE = "000000",
						BOX_CTRL_NO = x.BoxCtrlNo,
						PALLET_CTRL_NO = x.PalletCtrlNo,
						WORK_TYPE = x.WORK_TYPE,
						ADJ_QTY = x.AdjQty,
						STATUS = "0"
					});
					#endregion

					#region 序號調整、F20010301 調整單序號刷入紀錄
					if (x.SerialNoList != null)
					{
						var dbSnList = CommonService.GetItemSerialList(adjustOrderParam.GupCode, adjustOrderParam.CustCode, x.SerialNoList);

						foreach (var sn in x.SerialNoList)
						{
							if (x.WORK_TYPE == "0")
							{
								var dbSn = dbSnList.FirstOrDefault(y => y.SERIAL_NO == sn);
								if (dbSn == null)
								{
									addF2501List.Add(new F2501
									{
										GUP_CODE = adjustOrderParam.GupCode,
										CUST_CODE = adjustOrderParam.CustCode,
										SERIAL_NO = sn,
										ITEM_CODE = x.ItemCode,
										IN_DATE = DateTime.Today,
										VALID_DATE = x.ValidDate,
										WMS_NO = adjustNo,
										PROCESS_NO = adjustNo,
										STATUS = "A1", //調入序號
										ORD_PROP = "J1",//內部調整
										ACTIVATED = "0",
										SEND_CUST = "0",                   
                    CLIENT_IP = Current.DeviceIp
                    ,IS_ASYNC = "N"     //
                  });
								}
								else
								{
									if (dbSn.STATUS != "A1")  
									{
										dbSn.ITEM_CODE = x.ItemCode;
                    dbSn.IN_DATE = DateTime.Today;
                    dbSn.VALID_DATE = x.ValidDate;
										dbSn.WMS_NO = adjustNo;
										dbSn.PROCESS_NO = adjustNo;
										dbSn.PO_NO = null;
										dbSn.STATUS = "A1";//調入序號
										dbSn.ORD_PROP = "J1";//內部調整
										dbSn.VNR_CODE = null;
										dbSn.CLIENT_IP = Current.DeviceIp;
                    dbSn.IS_ASYNC = "N";
										updF2501List.Add(dbSn);
									}
									else
									{
										return new ExecuteResult(false, string.Format("商品序號{0}已存在庫內，不可調入此序號", sn));
									}
								}
							}
							else
							{

								var dbSn = dbSnList.FirstOrDefault(y => y.SERIAL_NO == sn);
								if (dbSn == null)
								{
									return new ExecuteResult(false, string.Format("商品序號{0}不存在庫內，不可調出此序號", sn));
								}
								else
								{
									//dbSn.IN_DATE = DateTime.Today;
									dbSn.WMS_NO = adjustNo;
									dbSn.STATUS = "C1"; //調出序號
                  dbSn.ORD_PROP = "J1";//內部調整  //#2149
                  dbSn.IS_ASYNC = "N";
									updF2501List.Add(dbSn);
								}
							}


							//快速移轉庫存 增加寫入F20010301 調整單序號刷入紀錄
							if (adjustOrderParam.AdjustType == AdjustType.FastStockTransfer)
							{
								addF20010301List.Add(new F20010301
								{
									ADJUST_NO = adjustNo,
									ADJUST_SEQ = index + 1,
									LOG_SEQ = logSeq,
									SERIAL_NO = sn,
									SERIAL_STATUS = x.WORK_TYPE == "0" ? "A1" : "C1",
									ISPASS = "1",
									DC_CODE = adjustOrderParam.DcCode,
									GUP_CODE = adjustOrderParam.GupCode,
									CUST_CODE = adjustOrderParam.CustCode
								});
								logSeq++;
							}
						}
					}
					#endregion

					#region 庫存異動紀錄


					if (x.WORK_TYPE == "0") // 調入
					{
						if (f1903.BUNDLE_SERIALLOC == "1" && x.SerialNoList != null) //序號綁儲位 依序號拆庫存
						{
							addStockList.AddRange(x.SerialNoList.Select(y => new OrderStockChange
							{
								DcCode = adjustOrderParam.DcCode,
								GupCode = adjustOrderParam.GupCode,
								CustCode = adjustOrderParam.CustCode,
								ItemCode = x.ItemCode,
								LocCode = x.LocCode,
								VaildDate = x.ValidDate,
								EnterDate = x.EnterDate,
								BoxCtrlNo = x.BoxCtrlNo,
								PalletCtrlNo = x.PalletCtrlNo,
								VnrCode = "000000",
								MakeNo = x.MakeNo,
								WmsNo = adjustNo,
								SerialNo = y,
								Qty = 1
							}));
						}
						else
						{
							addStockList.Add(new OrderStockChange
							{
								DcCode = adjustOrderParam.DcCode,
								GupCode = adjustOrderParam.GupCode,
								CustCode = adjustOrderParam.CustCode,
								ItemCode = x.ItemCode,
								LocCode = x.LocCode,
								VaildDate = x.ValidDate,
								EnterDate = x.EnterDate,
								BoxCtrlNo = x.BoxCtrlNo,
								PalletCtrlNo = x.PalletCtrlNo,
								VnrCode = "000000",
								MakeNo = x.MakeNo,
								WmsNo = adjustNo,
								SerialNo = "0",
								Qty = x.AdjQty
							});
						}
					}
					else
					{
						if (f1903.BUNDLE_SERIALLOC == "1" && x.SerialNoList != null) //序號綁儲位 依序號拆庫存
						{
							rmStockList.AddRange(x.SerialNoList.Select(y => new OrderStockChange
							{
								DcCode = adjustOrderParam.DcCode,
								GupCode = adjustOrderParam.GupCode,
								CustCode = adjustOrderParam.CustCode,
								ItemCode = x.ItemCode,
								LocCode = x.LocCode,
								VaildDate = x.ValidDate,
								EnterDate = x.EnterDate,
								BoxCtrlNo = x.BoxCtrlNo,
								PalletCtrlNo = x.PalletCtrlNo,
								VnrCode = "000000",
								MakeNo = x.MakeNo,
								WmsNo = adjustNo,
								SerialNo = y,
								Qty = 1
							}));
						}
						else
						{
							rmStockList.Add(new OrderStockChange
							{
								DcCode = adjustOrderParam.DcCode,
								GupCode = adjustOrderParam.GupCode,
								CustCode = adjustOrderParam.CustCode,
								ItemCode = x.ItemCode,
								LocCode = x.LocCode,
								VaildDate = x.ValidDate,
								EnterDate = x.EnterDate,
								BoxCtrlNo = x.BoxCtrlNo,
								PalletCtrlNo = x.PalletCtrlNo,
								VnrCode = "000000",
								MakeNo = x.MakeNo,
								WmsNo = adjustNo,
								SerialNo = "0",
								Qty = x.AdjQty
							});
						}
					}

					#endregion

				}

				#endregion

				#region 庫存調整

				if (addStockList.Any())
				{
					stockService.AddStock(addStockList);
				}
				if (rmStockList.Any())
				{
					var res = stockService.DeductStock(rmStockList);
					if (!res.IsSuccessed)
						return res;
				}
				#endregion

				#region 倉庫搬動紀錄
				var group = addF200103List.GroupBy(x => new { x.WAREHOUSE_ID, x.WORK_TYPE, x.ITEM_CODE, x.MAKE_NO, x.CAUSE, x.CAUSE_MEMO }).ToList();
				foreach (var x in group)
				{
					//var casueMemo = string.Empty;
					//if (!string.IsNullOrWhiteSpace(x.Key.CAUSE) && (x.Key.CAUSE != "999"))
					//	casueMemo = _causeList.FirstOrDefault(y => y.UCC_CODE == (x.Key.CAUSE))?.CAUSE;

					addF191303List.Add(new F191303
					{
						DC_CODE = f200101.DC_CODE,
						GUP_CODE = f200101.GUP_CODE,
						CUST_CODE = f200101.CUST_CODE,
						SHIFT_WMS_NO = f200101.ADJUST_NO,
						SHIFT_TYPE = "1",
						SRC_WAREHOUSE_TYPE = x.Key.WAREHOUSE_ID.Substring(0, 1),
						SRC_WAREHOUSE_ID = x.Key.WAREHOUSE_ID,
						TAR_WAREHOUSE_TYPE = x.Key.WAREHOUSE_ID.Substring(0, 1),
						TAR_WAREHOUSE_ID = x.Key.WAREHOUSE_ID,
						ITEM_CODE = x.Key.ITEM_CODE,
						SHIFT_CAUSE = x.Key.CAUSE,
						SHIFT_CAUSE_MEMO = x.Key.CAUSE_MEMO.Length > 20 ? x.Key.CAUSE_MEMO.Substring(0, 20) : x.Key.CAUSE_MEMO,
						SHIFT_TIME = DateTime.Now,
						SHIFT_QTY = x.Sum(z => z.ADJ_QTY),
						PROC_FLAG = "0",
						MAKE_NO = x.Key.MAKE_NO,
					});
				}
				#endregion


				// 更新儲位容積
				SharedService.UpdateUsedVolumnByLocCodes(f200101.DC_CODE, f200101.GUP_CODE, f200101.CUST_CODE, addF200103List.Select(x => x.LOC_CODE).Distinct());


				#region db commit

				f200101Repo.Add(f200101);

				if (addF2501List.Any())
					f2501Repo.BulkInsert(addF2501List);

				if (updF2501List.Any())
					f2501Repo.BulkUpdate(updF2501List);

				if (addF200103List.Any())
					f200103Repo.BulkInsert(addF200103List);

				if (addF20010301List.Any())
					f20010301Repo.BulkInsert(addF20010301List);

				if (addF191303List.Any())
					f191303Repo.BulkInsert(addF191303List);

				#endregion
				ajdustNoList.Add(adjustNo);
			}

			stockService.SaveChange();
			return new ExecuteResult(true, "", string.Join("、", ajdustNoList));
		}

		public AdjustType GetAdjustTypeByValue(string value)
		{
			AdjustType curAdjustType;
			switch (value)
			{
				case "0":
					curAdjustType = AdjustType.CancelOrder;
					break;
				case "1":
					curAdjustType = AdjustType.ItemStock;
					break;
				case "2":
					curAdjustType = AdjustType.InventoryStock;
					break;
				case "3":
					curAdjustType = AdjustType.LackStock;
					break;
				case "4":
					curAdjustType = AdjustType.FastStockTransfer;
					break;
				case "5":
					curAdjustType = AdjustType.AllocationLack;
					break;
				default:
					curAdjustType = AdjustType.ItemStock;
					break;
			}
			return curAdjustType;
		}
	}
}
