using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Shared.Services
{
	/// <summary>
	/// 商品檢驗後容器綁定共用函數
	/// </summary>
	public class WarehouseInRecvBindBoxService
	{

		#region Service
		private CommonService _commonService;
		public CommonService CommonService
		{
			get { return _commonService == null ? _commonService = new CommonService() : _commonService; }
			set { _commonService = value; }
		}

		private ContainerService _containerService;
		public ContainerService ContainerService
		{
			get { return _containerService == null ? _containerService = new ContainerService(_wmsTransaction) : _containerService; }
			set { _containerService = value; }
		}

		private WarehouseInService _warehouseInService;
		public WarehouseInService WarehouseInService
		{
			get { return _warehouseInService == null ? _warehouseInService = new WarehouseInService(_wmsTransaction) : _warehouseInService; }
			set { _warehouseInService = value; }
		}

		private WarehouseInRecvService _warehouseInRecvService;
		public WarehouseInRecvService WarehouseInRecvService
		{
			get { return _warehouseInRecvService == null ? _warehouseInRecvService = new WarehouseInRecvService(_wmsTransaction) : _warehouseInRecvService; }
			set { _warehouseInRecvService = value; }
		}
		#endregion

		#region Repository

		#region F0701Repository
		private F0701Repository _F0701Repo;
		public F0701Repository F0701Repo
		{
			get { return _F0701Repo == null ? _F0701Repo = new F0701Repository(Schemas.CoreSchema, _wmsTransaction) : _F0701Repo; }
			set { _F0701Repo = value; }
		}
		#endregion

		#region F070101Repository
		private F070101Repository _F070101Repo;
		public F070101Repository F070101Repo
		{
			get { return _F070101Repo == null ? _F070101Repo = new F070101Repository(Schemas.CoreSchema, _wmsTransaction) : _F070101Repo; }
			set { _F070101Repo = value; }
		}
		#endregion

		#region F070102Repository
		private F070102Repository _F070102Repo;
		public F070102Repository F070102Repo
		{
			get { return _F070102Repo == null ? _F070102Repo = new F070102Repository(Schemas.CoreSchema, _wmsTransaction) : _F070102Repo; }
			set { _F070102Repo = value; }
		}
		#endregion

		#region F020201Repository
		private F020201Repository _F020201Repo;
		public F020201Repository F020201Repo
		{
			get { return _F020201Repo == null ? _F020201Repo = new F020201Repository(Schemas.CoreSchema, _wmsTransaction) : _F020201Repo; }
			set { _F020201Repo = value; }
		}
		#endregion

		#region F0205Repository
		private F0205Repository _F0205Repo;
		public F0205Repository F0205Repo
		{
			get { return _F0205Repo == null ? _F0205Repo = new F0205Repository(Schemas.CoreSchema, _wmsTransaction) : _F0205Repo; }
			set { _F0205Repo = value; }
		}
		#endregion

		#region F020501Repository
		private F020501Repository _F020501Repo;
		public F020501Repository F020501Repo
		{
			get { return _F020501Repo == null ? _F020501Repo = new F020501Repository(Schemas.CoreSchema, _wmsTransaction) : _F020501Repo; }
			set { _F020501Repo = value; }
		}
		#endregion

		#region F020502Repository
		private F020502Repository _F020502Repo;
		public F020502Repository F020502Repo
		{
			get { return _F020502Repo == null ? _F020502Repo = new F020502Repository(Schemas.CoreSchema, _wmsTransaction) : _F020502Repo; }
			set { _F020502Repo = value; }
		}
		#endregion

		#region F020603Repository
		private F020603Repository _F020603Repo;
		public F020603Repository F020603Repo
		{
			get { return _F020603Repo == null ? _F020603Repo = new F020603Repository(Schemas.CoreSchema, _wmsTransaction) : _F020603Repo; }
			set { _F020603Repo = value; }
		}
		#endregion

		#endregion Repository

		private WmsTransaction _wmsTransaction;
		public WarehouseInRecvBindBoxService(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 進貨容器綁定-驗收單操作人員鎖定
		/// </summary>
		/// <param name="req"></param>
		/// <param name="gupCode"></param>
		/// <returns></returns>
		public ApiResult LockBindContainerAcceptenceOrder(LockBindContainerAcceptenceOrderReq req)
		{
			var f075111Repo = new F075111Repository(Schemas.CoreSchema);
			#region 驗收單檢查&鎖定
			var f075111 = f075111Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
				new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
				() =>
				{
					var lockF0701 = f075111Repo.LockF075111();
					var chkF075111 = f075111Repo.Find(x => x.DC_CODE == req.DcCode && x.GUP_CODE == req.GupCode && x.CUST_CODE == req.CustCode && x.WMS_NO == req.RtNo && x.STATUS == "0");
					if (chkF075111 == null)
					{
						var newF075111 = new F075111()
						{
							DC_CODE = req.DcCode,
							GUP_CODE = req.GupCode,
							CUST_CODE = req.CustCode,
							WMS_NO = req.RtNo,
							STATUS = "0", //可以使用
							DEVICE_TOOL = req.DeviceTool,
							CRT_DATE = DateTime.Now,
							CRT_STAFF = Current.Staff,
							CRT_NAME = Current.StaffName
						};
						f075111Repo.Add(newF075111, true);
						return newF075111;
					}
					else //chkF075111 != null
					{
						if (chkF075111.DEVICE_TOOL != req.DeviceTool)
						{
							chkF075111.STATUS = "1";  //不可使用
							return chkF075111;
						}
						else //chkF075111.DEVICE_TOOL == "1"
						{
							if (chkF075111.CRT_STAFF == Current.Staff)
							{
								return chkF075111;
							}
							else //chkF075111.CRT_STAFF != req.AccNo
							{
								if (req.IsChangeUser)
								{
									chkF075111.STATUS = "2"; //更換人員
									f075111Repo.UpdateFields(new { chkF075111.STATUS }, x => x.ID == chkF075111.ID);

									var newF075111 = new F075111()
									{
										DC_CODE = req.DcCode,
										GUP_CODE = req.GupCode,
										CUST_CODE = req.CustCode,
										WMS_NO = req.RtNo,
										STATUS = "0", //可以使用
										DEVICE_TOOL = req.DeviceTool,
										CRT_DATE = DateTime.Now,
										CRT_STAFF = Current.Staff,
										CRT_NAME = Current.StaffName
									};
									f075111Repo.Add(newF075111, true);
									return newF075111;
								}
								else
								{
									chkF075111.STATUS = "2";  //等待人員確認
									return chkF075111;
								}
							}
						}
					}
				});

			if (f075111.STATUS == "2")      // 等待人員確認
				return new ApiResult { IsSuccessed = false, MsgCode = "30001", MsgContent = "該單據已有人員(%s)進行作業中，請問是否要更換作業人員?".Replace("(%s)", $"({f075111.CRT_NAME})") };
			else if (f075111.STATUS == "1") // 不可使用
																			//此驗收單已被{0}在電腦版/PDA作業，不可使用
				return new ApiResult { IsSuccessed = false, MsgCode = "22001", MsgContent = string.Format("此驗收單已被{0}使用{1}作業，不可使用", f075111.CRT_NAME, f075111.DEVICE_TOOL == "0" ? "電腦版" : "PDA") };

			return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = "執行成功" };
			#endregion
		}

		/// <summary>
		/// 釋放驗收單鎖定
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public void UnLockBindContainerAcceptenceOrder(UnLockBindContainerAcceptenceOrderReq req)
		{
			var f075111Repo = new F075111Repository(Schemas.CoreSchema, _wmsTransaction);
			f075111Repo.UpdateFields(
				new { STATUS = "1" },
				x => x.DC_CODE == req.DcCode &&
						 x.GUP_CODE == req.GupCode &&
						 x.CUST_CODE == req.CustCode &&
						 x.WMS_NO == req.RtNo &&
						 x.STATUS == "0");
		}

		/// <summary>
		/// 檢查該驗收單容器綁定的操作人員是否相符
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="rtNo"></param>
		/// <param name="rtSeq"></param>
		/// <param name="procStaff"></param>
		/// <param name="procStaffName"></param>
		/// <returns></returns>
		public ApiResult CheckIsOtherUserProc(LockBindContainerAcceptenceOrderReq req)
		{
			var f075111Repo = new F075111Repository(Schemas.CoreSchema);

			var f075111 = f075111Repo.UseTransationScope(new TransactionScope(TransactionScopeOption.Required,
				new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }),
				() =>
				{
					var lockF0701 = f075111Repo.LockF075111();
					var chkF075111 = f075111Repo.Find(x => x.DC_CODE == req.DcCode && x.GUP_CODE == req.GupCode && x.CUST_CODE == req.CustCode && x.WMS_NO == req.RtNo && x.STATUS == "0");
					if (chkF075111 == null)
					{
						var newF075111 = new F075111()
						{
							DC_CODE = req.DcCode,
							GUP_CODE = req.GupCode,
							CUST_CODE = req.CustCode,
							WMS_NO = req.RtNo,
							STATUS = "0", //可以使用
							DEVICE_TOOL = req.DeviceTool,
							CRT_DATE = DateTime.Now,
							CRT_STAFF = Current.Staff,
							CRT_NAME = Current.StaffName
						};
						f075111Repo.Add(newF075111, true);
						return newF075111;
					}
					else
					{
						if (chkF075111.DEVICE_TOOL == req.DeviceTool && chkF075111.CRT_STAFF == Current.Staff)
						{
							return chkF075111;
						}
						// 電腦版不同人作業允許直接更換不需要確認
						else if (chkF075111.DEVICE_TOOL == req.DeviceTool && req.DeviceTool == "0" && chkF075111.CRT_STAFF != Current.Staff)
						{
							chkF075111.STATUS = "2";
							f075111Repo.UpdateFields(new { chkF075111.STATUS }, x => x.ID == chkF075111.ID);
							var newF075111 = new F075111()
							{
								DC_CODE = req.DcCode,
								GUP_CODE = req.GupCode,
								CUST_CODE = req.CustCode,
								WMS_NO = req.RtNo,
								STATUS = "0", //可以使用
								DEVICE_TOOL = req.DeviceTool,
								CRT_DATE = DateTime.Now,
								CRT_STAFF = Current.Staff,
								CRT_NAME = Current.StaffName
							};
							f075111Repo.Add(newF075111, true);
							return newF075111;
						}
						else
						{
							chkF075111.STATUS = "1"; //不可以使用
							return chkF075111;
						}
					}
				});

			if (f075111.STATUS == "1")
				//此驗收單已被{0}在電腦版/PDA作業，不可使用
				return new ApiResult { IsSuccessed = false, MsgCode = "22001", MsgContent = string.Format("此驗收單已被{0}使用{1}作業，不可使用", f075111.CRT_NAME, f075111.DEVICE_TOOL == "0" ? "電腦版" : "PDA") };

			return new ApiResult { IsSuccessed = true };
		}

		/// <summary>
		/// 產生F020502資料時，判斷寫入的STATUS用
		/// </summary>
		/// <param name="NeedDoubleCheck"></param>
		/// <param name="TypeCode"></param>
		/// <returns></returns>
		public string Getf020502Status(int NeedDoubleCheck, string TypeCode)
		{
			return (NeedDoubleCheck == 1 && TypeCode.ToUpper() != "R") ? "0" : "1";
		}

		public ApiResult AddContainerBindData(AddContainerBindDataReq req)
		{
			F0701 f0701 = null;             //[F]
			F020501 f020501 = null;         //[G]
			List<F020502> f020502s = null;  //[H]
			F0205 f0205 = null;             //[J]

			#region 狀態檢核
			//檢查容器條碼格式是否正確
			//[E]
			var chkContainer = ContainerService.CheckContainer(req.ContainerCode);
			if (!chkContainer.IsSuccessed)
				return new ApiResult { IsSuccessed = chkContainer.IsSuccessed, MsgCode = "22003", MsgContent = chkContainer.Message };

			//只有揀區才允許使用分格容器
			if (req.TypeCode.ToUpper() != "A" && !String.IsNullOrWhiteSpace(chkContainer.BinCode))
				return new ApiResult { IsSuccessed = false, MsgCode = "22025", MsgContent = "只有揀區才允許使用分格容器" };

			//[J]
			f0205 = F0205Repo.Find(x => x.ID == req.AreaId);

			//檢查容器是否已綁定
			f0701 = F0701Repo.GetData(chkContainer.ContainerCode);
			if (f0701 != null)
			{
				if (f0701.CONTAINER_TYPE == "2")
					//原本的容器是混和型容器，並不允許使用
					return new ApiResult { IsSuccessed = false, MsgCode = "22004", MsgContent = "原本的容器是混和型容器，並不允許使用" };
				if (f0701.CUST_CODE != req.CustCode)
					//此容器以被其他貨主使用，不可重複使用
					return new ApiResult { IsSuccessed = false, MsgCode = "22005", MsgContent = "此容器以被其他貨主使用，不可重複使用" };

				f020501 = F020501Repo.GetDataByF0701Id(req.DcCode, req.GupCode, req.CustCode, f0701.ID);
				if (f020501 == null)
					//該容器已被其他作業使用，不可綁定
					return new ApiResult { IsSuccessed = false, MsgCode = "22006", MsgContent = "該容器已被其他作業使用，不可綁定" };

				if (!new[] { "0", "9" }.Contains(f020501.STATUS))
					//此容器已被使用，不可綁定
					return new ApiResult { IsSuccessed = false, MsgCode = "22007", MsgContent = "此容器已被使用，不可綁定" };

				if (f020501.TYPE_CODE != req.TypeCode)
				{
					var areaName = CommonService.GetF000904("F020501", "TYPE_CODE", f020501.TYPE_CODE).NAME;
					//此容器已綁定{0}，不可綁定
					return new ApiResult { IsSuccessed = false, MsgCode = "22008", MsgContent = string.Format("此容器已綁定{0}，不可綁定", areaName) };
				}

				if (f020501.PICK_WARE_ID != req.WarehouseId)
					//此容器上架倉別為{0}，與目前商品上架倉別{1}不同，不可綁定
					return new ApiResult
					{
						IsSuccessed = false,
						MsgCode = "22009",
						MsgContent = string.Format("此容器上架倉別為{0}，與目前商品上架倉別{1}不同，不可綁定", f020501.PICK_WARE_ID, req.WarehouseId)
					};

				if (f020501.TYPE_CODE == "R")
					//不良品容器不允許混放不同驗收不良品商品
					return new ApiResult { IsSuccessed = false, MsgCode = "22010", MsgContent = "不良品容器不允許混放不同驗收不良品商品" };

				f020502s = F020502Repo.GetDatasByF020501Id(f020501.ID).ToList();
				if (f020502s != null)
				{
					if (!string.IsNullOrWhiteSpace(chkContainer.BinCode))
					{
						if (f020502s.Any(x => x.BIN_CODE == req.ContainerCode))
							//此容器分格已綁定商品，不可綁定
							return new ApiResult { IsSuccessed = false, MsgCode = "22011", MsgContent = "此容器分格已綁定商品，不可綁定" };

						if (f020502s.Any(x => string.IsNullOrWhiteSpace(x.BIN_CODE)))
							//此容器為非分格容器，且已有綁定商品資料，不可刷入容器分格條碼
							return new ApiResult { IsSuccessed = false, MsgCode = "22012", MsgContent = "此容器為非分格容器，且已有綁定商品資料，不可刷入容器分格條碼" };
					}
					else //BinCode=null
					{
						if (f020502s.Any(x => !string.IsNullOrWhiteSpace(x.BIN_CODE)))
							//此容器為有分格容器，已有綁定商品資料，必須刷入分格條碼
							return new ApiResult { IsSuccessed = false, MsgCode = "22013", MsgContent = "此容器為有分格容器，已有綁定商品資料，必須刷入分格條碼" };

            //撈出這張單的F020502，檢查是否存在這容器
            //檢查沒有分格，但重複輸入容器
            if (string.IsNullOrWhiteSpace(chkContainer.BinCode) &&
              F020502Repo.CheckIsContainerExists(req.DcCode, req.GupCode, req.CustCode, req.PurchaseNo, req.RtNo, chkContainer.ContainerCode))
              return new ApiResult { IsSuccessed = false, MsgCode = "22013", MsgContent = "此容器已存在清單中" };

          }
        }
			}

			if (f0205.B_QTY < f0205.A_QTY + req.PutQty)
				//您輸入的放入數量{0}已綁定容器數量{1}超過該區預計分播數{2}
				return new ApiResult { IsSuccessed = false, MsgCode = "22014", MsgContent = string.Format("您輸入的放入數量{0}已綁定容器數量{1}超過該區預計分播數{2}", req.PutQty, f0205.A_QTY, f0205.B_QTY) };
			#endregion 狀態檢核

			long f0701Id;
			long f020501_ID;
			long f020502_ID;

			f0205.A_QTY += req.PutQty;
			F0205Repo.UpdateFields(new { f0205.A_QTY }, x => x.ID == req.AreaId);

			if (f0701 == null)
			{
				f0701Id = ContainerService.GetF0701NextId();
				F0701Repo.Add(new F0701
				{
					ID = f0701Id,
					DC_CODE = req.DcCode,
					CUST_CODE = req.CustCode,
					WAREHOUSE_ID = req.WarehouseId,
					CONTAINER_CODE = chkContainer.ContainerCode,
					CONTAINER_TYPE = "0"
				});
			}
			else
				f0701Id = f0701.ID;

			if (f020501 == null)
			{
				f020501_ID = WarehouseInService.GetF020501NextId();
				F020501Repo.Add(new F020501
				{
					ID = f020501_ID,
					DC_CODE = req.DcCode,
					GUP_CODE = req.GupCode,
					CUST_CODE = req.CustCode,
					CONTAINER_CODE = chkContainer.ContainerCode,
					F0701_ID = f0701Id,
					PICK_WARE_ID = req.WarehouseId,
					TYPE_CODE = req.TypeCode,
					STATUS = "0"
				});
			}
			else
				f020501_ID = f020501.ID;

			f020502_ID = WarehouseInService.GetF020502NextId();
			F020502Repo.Add(new F020502
			{
				ID = f020502_ID,
				F020501_ID = f020501_ID,
				DC_CODE = req.DcCode,
				GUP_CODE = req.GupCode,
				CUST_CODE = req.CustCode,
				STOCK_NO = req.PurchaseNo,
				STOCK_SEQ = req.PurchaseSeq,
				RT_NO = req.RtNo,
				RT_SEQ = req.RtSeq,
				ITEM_CODE = f0205.ITEM_CODE,
				QTY = req.PutQty,
				CONTAINER_CODE = chkContainer.ContainerCode,
				BIN_CODE = chkContainer.BinCode,
				STATUS = Getf020502Status(f0205.NEED_DOUBLE_CHECK, f0205.TYPE_CODE)
			});

			return new ApiResult
			{
				IsSuccessed = true,
				MsgCode = "10005",
				MsgContent = "執行成功",
				Data = new AddContainerBindDataRes
				{
					F020501_ID = f020501_ID,
					F020502_ID = f020502_ID
				}
			};
		}

		/// <summary>
		/// 刪除商品進貨容器綁定(F020502)
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult DeleteContainerBindData(DeleteContainerBindDataReq req)
		{
			//(2)更新F0205.A_QTY-=刪除的投入數量
			//var f0205data = F0205Repo.Find(x => x.DC_CODE == req.DcCode &&
			//     x.GUP_CODE == req.GupCode &&
			//     x.CUST_CODE == req.CustCode &&
			//     x.RT_NO == req.RtNo &&
			//     x.RT_SEQ == req.RtSeq);
			//x.TYPE_CODE == areaContainerData.TYPE_CODE);
			var f0205data = F0205Repo.Find(x => x.ID == req.AreaId);
			if (f0205data == null)
				return new ApiResult { IsSuccessed = false, MsgCode = "22015", MsgContent = "找不到驗收分播紀錄" };

			f0205data.A_QTY -= req.Qty;
			F0205Repo.UpdateFields(new { f0205data.A_QTY }, x => x.ID == f0205data.ID);

			//刪除F020502，若為最後一筆，刪除F020501,F0701
			var f020502data = F020502Repo.GetDatasByTrueAndCondition(
					x => x.F020501_ID == req.F020501_ID).ToList();
			if (f020502data != null)
			{
				if (f020502data.Count == 1)
				{
					var f020501data = F020501Repo.Find(x => x.ID == req.F020501_ID);
					if (f020501data == null)
						return new ApiResult { IsSuccessed = false, MsgCode = "22016", MsgContent = "找不到工作中容器紀錄" };
					F0701Repo.Delete(x => x.ID == f020501data.F0701_ID);
					F020501Repo.Delete(x => x.ID == req.F020501_ID);
					F020502Repo.Delete(x => x.ID == req.F020502_ID);

				}
				else
					F020502Repo.Delete(x => x.ID == req.F020502_ID);
			}
			return new ApiResult { IsSuccessed = true, MsgCode = "10005", MsgContent = "執行成功" };
		}


		/// <summary>
		/// 驗收單容器綁定完成
		/// </summary>
		/// <param name="req"></param>
		/// <returns></returns>
		public ApiResult RecvBindContainerFinished(ShareRecvBindContainerFinishedReq req)
		{

			var lockContainers = new List<string>();
			var allocationNoList = new List<string>();
			//檢查驗收單是否各區都完成容器綁定
			if (!F0205Repo.CheckAllContainerIsDone(req.DcCode, req.GupCode, req.CustCode, req.RtNo, req.RtSeq))
				return new ApiResult { IsSuccessed = false, MsgCode = "22019", MsgContent = "必須先完成此驗收單容器綁定後再進行綁定完成" };

			//檢查該筆驗收單是否已綁定完成
			if (!F020201Repo.IsAllOrdBindComplete(req.DcCode, req.GupCode, req.CustCode, req.RtNo, req.RtSeq))
				return new ApiResult { IsSuccessed = false, MsgCode = "22020", MsgContent = "此驗收單已綁定完成，不可重複綁定" };

			// 不良品容器關箱
					var f0205s = F0205Repo.GetDatas(req.DcCode, req.GupCode, req.CustCode, req.RtNo, req.RtSeq).ToList();
			//揀區＆補區的F0205
			var f0205Goods = f0205s.Where(x => x.TYPE_CODE != "R").ToList();
			//不良品的F0205
			var f0205NoGood = f0205s.FirstOrDefault(x => x.TYPE_CODE == "R");
			if (f0205NoGood != null)
			{
				var noGoodContainers = F020501Repo.GetNoGoodDataByRtNo(req.DcCode, req.GupCode, req.CustCode, req.RtNo, req.RtSeq).ToList();
				var finishedRtContainerStatusList = new List<RtNoContainerStatus>();
				var rtNoList = new List<string>();
				foreach (var item in noGoodContainers)
				{
					lockContainers.Add(item.CONTAINER_CODE);
					//不良品不用走複驗流程，因此直接傳False
					var closeBoxRes =  ContainerCloseBox(item.ID, req.RtNo, req.RtSeq, false);

					if (!closeBoxRes.IsSuccessed)
						return new ApiResult { IsSuccessed = false, MsgCode = "22023" ,MsgContent = closeBoxRes.Message };
					rtNoList.AddRange(closeBoxRes.f020502s.Select(a => a.RT_NO).ToList());
					finishedRtContainerStatusList.AddRange(closeBoxRes.f020502s.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.STOCK_NO, x.RT_NO })
						.Select(x => new RtNoContainerStatus
						{
							DC_CODE = x.Key.DC_CODE,
							GUP_CODE = x.Key.GUP_CODE,
							CUST_CODE = x.Key.CUST_CODE,
							STOCK_NO = x.Key.STOCK_NO,
							RT_NO = x.Key.RT_NO,
							F020501_ID = closeBoxRes.f020501.ID,
							F020501_STATUS = closeBoxRes.f020501.STATUS,
							ALLOCATION_NO = closeBoxRes.f020501.ALLOCATION_NO
						}).ToList());
					allocationNoList.Add(closeBoxRes.No);
				}
				var res = WarehouseInService.AfterConatinerTargetFinishedProcess(req.DcCode,req.GupCode,req.CustCode, rtNoList, finishedRtContainerStatusList);

				if (!res.IsSuccessed)
					return new ApiResult { IsSuccessed = false, MsgCode = "22024", MsgContent = res.Message };
				
			}

			//更新F0205.STATUS=1(分播完成)
			F0205Repo.UpdateFields(
				new { STATUS = "1" },
				x => x.DC_CODE == req.DcCode &&
						 x.GUP_CODE == req.GupCode &&
						 x.CUST_CODE == req.CustCode &&
						 x.RT_NO == req.RtNo &&
						 x.RT_SEQ == req.RtSeq &&
						 x.STATUS == "0");

			//更新F020201.STATUS=2(已上傳)
			F020201Repo.UpdateFields(
				new { STATUS = "2" },
				x => x.DC_CODE == req.DcCode &&
						 x.GUP_CODE == req.GupCode &&
						 x.CUST_CODE == req.CustCode &&
						 x.RT_NO == req.RtNo &&
						 x.RT_SEQ == req.RtSeq &&
						 x.STATUS == "3");

			//檢查驗收單是否都綁定完成
			if (F020201Repo.IsAcceptenceIsComplete(req.DcCode, req.GupCode, req.CustCode, req.RtNo, req.RtSeq))
			{
				UnLockBindContainerAcceptenceOrder(new UnLockBindContainerAcceptenceOrderReq
				{
					DcCode = req.DcCode,
					GupCode = req.GupCode,
					CustCode = req.CustCode,
					RtNo = req.RtNo
				});
				WarehouseInRecvService.UnLockAcceptenceOrder(new UnLockAcceptenceOrderReq
				{
					DcCode = req.DcCode,
					GupCode = req.GupCode,
					CustCode = req.CustCode,
					StockNo = req.PurchaseNo
				});
			}
			return new ApiResult
			{
				IsSuccessed = true,
				MsgCode = "10005",
				MsgContent = "執行成功",
				Data = new ShareRecvBindContainerFinishedRes
				{
					LockContainers = lockContainers,
					AllocationNoList = allocationNoList
				}
			};
		}

		public ContainerCloseBoxRes ContainerCloseBox(long f020501Id, string rtNo, string rtSeq, Boolean needCheck)
		{
			var f020501 = F020501Repo.Find(x => x.ID == f020501Id);
			#region 容器鎖定
			var lockRes = WarehouseInService.LockContainerProcess(f020501.CONTAINER_CODE);
			if (!lockRes.IsSuccessed)
				return new ContainerCloseBoxRes { IsSuccessed = false, Message = lockRes.Message };
			#endregion 容器鎖定

			#region F020501容器頭檔狀態檢查
			var chkF020501Status = WarehouseInService.CheckF020501Status(f020501, 0);
			if (!chkF020501Status.IsSuccessed)
				return new ContainerCloseBoxRes { IsSuccessed = false, Message = chkF020501Status.MsgContent };
			#endregion F020501容器頭檔狀態檢查

			var result = WarehouseInService.ContainerCloseBox(f020501Id, rtNo, rtSeq);
			return result;

		}

	}
}
