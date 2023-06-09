using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P08.Services
{
	public class P080804Service
	{
		private WmsTransaction _wmsTransaction;
    private CommonService _commonService;

		public P080804Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 刷讀容器條碼
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="containerCode"></param>
		/// <returns></returns>
		public ScanContainerResult ScanContainerCode(string dcCode,string gupCode,string custCode,string containerCode)
		{
			ContainerPickInfo containerPickInfo;
			var isFisrtAllot = false;
			F052904 f052904;
			List<F05290401> f05290401s;
		
			var f052904Repo = new F052904Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05290401Repo = new F05290401Repository(Schemas.CoreSchema, _wmsTransaction);
			var f052905Repo = new F052905Repository(Schemas.CoreSchema);
			var f0701Repo = new F0701Repository(Schemas.CoreSchema);



			// 取得未完成分貨的容器資訊
			containerPickInfo = f052904Repo.GetContainerPickInfo(dcCode, containerCode);
			var pickContainerNotAllotCnt = 0;
			// 如果不存在未完成分貨的容器資訊 從容器主檔產生分貨資料
			if (containerPickInfo == null)
			{
				isFisrtAllot = true;
			  containerPickInfo = f0701Repo.GetContainerPickInfo(dcCode, containerCode);
				if (containerPickInfo == null)
					return new ScanContainerResult { IsSuccessed = false, Message = Properties.Resources.P080804Service_ContianerNoNotExist };

				var f051201Repo = new F051201Repository(Schemas.CoreSchema);
				var isBatchPickNoFinished = f051201Repo.CheckIsBatchPickNotFinished(dcCode, gupCode, custCode, containerPickInfo.DelvDate, containerPickInfo.PickTime);
				if(isBatchPickNoFinished)
					return new ScanContainerResult { IsSuccessed = false, Message = string.Format(Properties.Resources.P080804Service_BatchPickNotFinished, containerPickInfo.DelvDate.ToString("yyyy/MM/dd"), containerPickInfo.PickTime, containerPickInfo.MoveOutTargetName)  };

				var findOtherContainerNotFinish = f052904Repo.FindOtherContainerNotFinish(dcCode, gupCode, custCode, containerPickInfo.DelvDate, containerPickInfo.PickTime, containerPickInfo.MoveOutTarget);
				if(findOtherContainerNotFinish != null)
					return new ScanContainerResult {
						IsSuccessed = false,
						Message = string.Format(Properties.Resources.P080804Service_BatchContainerNotFInished,
						containerPickInfo.DelvDate.ToString("yyyy/MM/dd"), containerPickInfo.PickTime, containerPickInfo.MoveOutTargetName, Environment.NewLine, findOtherContainerNotFinish.CONTAINER_CODE) };

				pickContainerNotAllotCnt = f0701Repo.GetPickContainerCnt(dcCode, gupCode, custCode, containerPickInfo.PickOrdNo);

				// 產生容器分貨頭檔
				f052904 = new F052904
				{
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					CONTAINER_CODE = containerCode,
					PICK_ORD_NO = containerPickInfo.PickOrdNo,
					MOVE_OUT_TARGET = containerPickInfo.MoveOutTarget,
					DELV_DATE = containerPickInfo.DelvDate,
					PICK_TIME = containerPickInfo.PickTime,
					STATUS = "0"
				};

				// 產生容器分貨明細檔
				f05290401s = f0701Repo.GetF05290401sByContainerId(containerPickInfo.Id).ToList();

				// 如果為揀貨單最後一箱容器
				if(pickContainerNotAllotCnt == 1)
				{
					var f051203Repo = new F051203Repository(Schemas.CoreSchema);
					var pickLackDetail = f051203Repo.GetLackDataByPickNo(dcCode, gupCode, custCode, containerPickInfo.PickOrdNo).ToList();

					// 若揀貨單有揀缺數，併到最後一箱分貨
					if (pickLackDetail.Any())
					{
						var groupPickLacks = pickLackDetail.GroupBy(x => x.ITEM_CODE).ToList();
						foreach (var item in groupPickLacks)
						{
							var f05290401 = f05290401s.FirstOrDefault(x => x.ITEM_CODE == item.Key);
							if (f05290401 == null)
							{
								f05290401 = new F05290401
								{
									DC_CODE = dcCode,
									GUP_CODE = gupCode,
									CUST_CODE = custCode,
									CONTAINER_CODE = containerCode,
									PICK_ORD_NO = containerPickInfo.PickOrdNo,
									ITEM_CODE = item.Key,
									B_SET_QTY = item.Sum(x => x.B_PICK_QTY - x.A_PICK_QTY),
									A_SET_QTY = 0
								};
								f05290401s.Add(f05290401);
							}
							else
								f05290401.B_SET_QTY += item.Sum(x => x.B_PICK_QTY - x.A_PICK_QTY);
						}
					}

					var sowLackDetail = f05290401Repo.GetLackDatasByPickOrdNo(dcCode, gupCode, custCode, containerPickInfo.PickOrdNo).ToList();

					// 若有前幾箱有播缺數，併到最後一箱分貨
					if (sowLackDetail.Any())
					{
						foreach (var item in sowLackDetail)
						{
							var f05290401 = f05290401s.FirstOrDefault(x => x.ITEM_CODE == item.ITEM_CODE);
							if (f05290401 == null)
							{
								f05290401 = new F05290401
								{
									DC_CODE = dcCode,
									GUP_CODE = gupCode,
									CUST_CODE = custCode,
									CONTAINER_CODE = containerCode,
									PICK_ORD_NO = containerPickInfo.PickOrdNo,
									ITEM_CODE = item.ITEM_CODE,
									B_SET_QTY = item.B_SET_QTY - item.A_SET_QTY,
									A_SET_QTY = 0
								};
								f05290401s.Add(f05290401);
							}
							else
								f05290401.B_SET_QTY += (item.B_SET_QTY - item.A_SET_QTY);
						}
					}

				}

				f052904Repo.Add(f052904);
				f05290401Repo.BulkInsert(f05290401s);

				var f051202Repo = new F051202Repository(Schemas.CoreSchema);
				// 正常出貨訂單要寫入回檔紀錄(包裝開始)
				var canShipWmsNos = f051202Repo.GetCanShipWmsNosByPick(dcCode, gupCode, custCode, containerPickInfo.PickOrdNo).ToList();
				if(canShipWmsNos.Any())
				{
					var orderService = new OrderService(_wmsTransaction);
					orderService.AddF050305(dcCode, gupCode, custCode, canShipWmsNos, "2");
				}

			}
			else
			{
				pickContainerNotAllotCnt = f0701Repo.GetPickContainerCnt(dcCode, gupCode, custCode, containerPickInfo.PickOrdNo);
				// 取得分貨明細檔
				f05290401s = f05290401Repo.GetDatas(dcCode, gupCode, custCode, containerPickInfo.PickOrdNo, containerCode).ToList();
			}

			var normalShipBoxInfo = f052905Repo.GetBoxInfo(dcCode, gupCode, custCode, containerPickInfo.DelvDate, containerPickInfo.PickTime, containerPickInfo.MoveOutTarget, "01");
			var cancelOrderBoxInfo = f052905Repo.GetBoxInfo(dcCode, gupCode, custCode, containerPickInfo.DelvDate, containerPickInfo.PickTime, containerPickInfo.MoveOutTarget, "02");

			var result = new ScanContainerResult
			{
				IsSuccessed = true,
				ContainerCode = containerCode,
				ContainerPickInfo = containerPickInfo,
				IsFisrtAllot = isFisrtAllot,
				IsPickLastBox = pickContainerNotAllotCnt == 1,
				NormalBox = normalShipBoxInfo,
				CancelBox = cancelOrderBoxInfo
			};

			return result;
		}

		/// <summary>
		/// 綁定箱號(加箱/重新綁定)
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="delvDate"></param>
		/// <param name="pickTime"></param>
		/// <param name="moveOutTarget"></param>
		/// <param name="sowType"></param>
		/// <param name="newBoxNo"></param>
		/// <param name="orgBoxNo"></param>
		/// <param name="isAddBox"></param>
		/// <returns></returns>
		public BindBoxResult BindBox(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string moveOutTarget, string sowType, string newBoxNo, string orgBoxNo, bool isAddBox)
		{
			var f052905Repo = new F052905Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05290501Repo = new F05290501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0003Repo = new F0003Repository(Schemas.CoreSchema, _wmsTransaction);

      if (_commonService == null)
      {
        _commonService = new CommonService();
      }

      F052905 orgF052905 = null;
			F052905 newF052905 = null;
			if (!string.IsNullOrWhiteSpace(orgBoxNo))
			{
				orgF052905 = f052905Repo.GetData(dcCode, gupCode, custCode, delvDate, pickTime, moveOutTarget, sowType, orgBoxNo);
				if (orgF052905 == null)
					return new BindBoxResult { IsSuccessed = false, Message = string.Format(Properties.Resources.P080804Service_OrgBoxNotExist, orgBoxNo) };
				if (orgF052905.STATUS == "1" && !isAddBox)
					return new BindBoxResult { IsSuccessed = false, Message = string.Format(Properties.Resources.P080804Service_OrgBoxIsClosed, orgBoxNo) };
				var hasDetail = f05290501Repo.GetDatasByRefId(orgF052905.ID).Any(x => x.A_SET_QTY > 0);
				if (hasDetail && !isAddBox)
					return new BindBoxResult { IsSuccessed = false, Message = string.Format(Properties.Resources.P080804Service_OrgBoxHasProduct, orgBoxNo) };
				if(!hasDetail && isAddBox)
					return new BindBoxResult { IsSuccessed = false, Message = string.Format(Properties.Resources.P080804Service_OrgAddBoxHasProduct, orgBoxNo) };
			}
			if (!string.IsNullOrWhiteSpace(newBoxNo))
			{
        if (_commonService == null)
        {
          _commonService = new CommonService();
        }

				newBoxNo = newBoxNo.ToUpper();
        // 在正常出貨檢核容器條碼的開頭必須是F0003.CrossDCContainer
        var crossDCContainer = _commonService.GetSysGlobalValue(dcCode, "CrossDCContainer");
				if (crossDCContainer != null && !newBoxNo.StartsWith(crossDCContainer) && sowType == "01")
				{
					return new BindBoxResult { IsSuccessed = false, Message = $"跨庫的容器條碼開頭必須是{crossDCContainer}" };
				}
				newF052905 = f052905Repo.GetData(dcCode, newBoxNo);
				if (newF052905 != null)
					return new BindBoxResult { IsSuccessed = false, Message = string.Format(Properties.Resources.P080804Service_NewBoxIsUsed, newBoxNo) };

				var f0701Repo = new F0701Repository(Schemas.CoreSchema);
				var isUsed = f0701Repo.CheckDcContainerIsUsed(dcCode, newBoxNo);
				if (isUsed.Any())
					return new BindBoxResult { IsSuccessed = false, Message = string.Format(Properties.Resources.P080804Service_NewBoxIsUsed, newBoxNo) };
				
			}
			var result = new BindBoxResult { IsSuccessed = true };
			if (isAddBox)
			{
				var containerSeq = 1;
				// 有原箱號資料 且未關箱時，將原箱號進行關箱
				if (orgF052905 != null && orgF052905.STATUS == "0")
				{
					orgF052905.STATUS = "1"; //關箱
					f052905Repo.Update(orgF052905);
				}

				if (orgF052905 != null)
					containerSeq = orgF052905.CONTAINER_SEQ + 1;

				//建立新箱號資料
				newF052905 = new F052905
				{
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					DELV_DATE = delvDate,
					PICK_TIME = pickTime,
					MOVE_OUT_TARGET = moveOutTarget,
					SOW_TYPE = sowType,
					CONTAINER_CODE = newBoxNo,
					CONTAINER_SEQ = containerSeq,
					STATUS = "0"
				};
				f052905Repo.Add(newF052905, "ID");
			}
			else
			{
				// 重新綁定更換成新箱號
				orgF052905.CONTAINER_CODE = newBoxNo;
				f052905Repo.Update(orgF052905);
			}
			result.BoxInfo = new BoxInfo
			{
				SowType = sowType,
				BoxNo = newBoxNo,
				SowQty = 0
			};
			return result;
		}

		/// <summary>
		/// 刷讀品號/國條/商品序號 進行分貨
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="pickOrdNo"></param>
		/// <param name="containerCode"></param>
		/// <param name="itemBarcode"></param>
		/// <param name="normalBoxNo"></param>
		/// <param name="canelOrderBoxNo"></param>
		/// <returns></returns>
		public SowItemResult SowItem(string dcCode, string gupCode, string custCode,string pickOrdNo,string containerCode,string itemBarcode,string normalBoxNo,string canelOrderBoxNo)
		{
			itemBarcode = itemBarcode.Trim().ToUpper();
			#region 商品/序號檢核
			var itemService = new ItemService();
			F2501 f2501 = null;
			if (string.IsNullOrWhiteSpace(itemBarcode))
			{
				return new SowItemResult { IsSuccessed = false, Message = "你刷讀的商品條碼不可為空" };
			}
			var findItemCodes = itemService.FindItems(gupCode, custCode, itemBarcode, ref f2501);
			if (!findItemCodes.Any())
				return new SowItemResult { IsSuccessed = false, Message = Properties.Resources.P080804Service_YouScanItemBarcodeNotExist };

			var f05290401Repo = new F05290401Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05290401s = f05290401Repo.AsForUpdate().GetDatasByItems(dcCode, gupCode, custCode, pickOrdNo, containerCode, findItemCodes).ToList();
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			F1903 f1903 = null;
			F05290401 f05290401 = null;
			if (f05290401s.Any())
			{
				f05290401 = f05290401s.FirstOrDefault(x => x.B_SET_QTY > x.A_SET_QTY);
				if (f05290401 == null)
				{
					f1903 = f1903Repo.Find(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ITEM_CODE == f05290401s.First().ITEM_CODE);
					return new SowItemResult { IsSuccessed = false, Message = Properties.Resources.P080804Service_ItemIsAllotFinish, ItemCode = f1903.ITEM_CODE, ItemName = f1903.ITEM_NAME };
				}
				else
					f1903 = f1903Repo.Find(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ITEM_CODE == f05290401.ITEM_CODE);
			}
			else
				return new SowItemResult { IsSuccessed = false, Message = Properties.Resources.P080804Service_ItemBarcodeNotInBox };

			// 若為序號商品 刷入的條碼非序號 要提示
			if (f1903.BUNDLE_SERIALNO == "1" && f2501 == null)
				return new SowItemResult { IsSuccessed = false, Message =Properties.Resources.P080804Service_SerialItemMustScanSeriral, ItemCode = f1903.ITEM_CODE, ItemName = f1903.ITEM_NAME };

			var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
			if (f2501 != null)
			{
				// 檢查序號是否為在庫序號
				if (f2501.STATUS == "C1" || f2501.STATUS == "D2")
					return new SowItemResult { IsSuccessed = false, Message = Properties.Resources.P080804Service_SerialIsNotInWarehouse, ItemCode = f1903.ITEM_CODE, ItemName = f1903.ITEM_NAME };

        // No.2091 若為不良品序號(F2501.ACTIVATED=1)，不可出貨
        if (f2501.ACTIVATED == "1")
          return new SowItemResult { IsSuccessed = false, Message = Properties.Resources.P080804Service_SerialIsActived, ItemCode = f1903.ITEM_CODE, ItemName = f1903.ITEM_NAME };

        // 檢查序號是否凍結
        var isSerialIsFreeze = f2501Repo.GetSerialIsFreeze(gupCode, custCode, "02", new List<string> { f2501.SERIAL_NO }).Any();
				if (isSerialIsFreeze)
					return new SowItemResult { IsSuccessed = false, Message = Properties.Resources.P080804Service_SerialIsLock, ItemCode = f1903.ITEM_CODE, ItemName = f1903.ITEM_NAME };
			}
			#endregion

			var result = new SowItemResult { IsSuccessed = true  ,ItemCode = f1903.ITEM_CODE, ItemName = f1903.ITEM_NAME };

			#region 更新揀貨明細，虛擬儲位庫存


			var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051202s = f051202Repo.AsForUpdate().GetNotCacnelDataByPickNo(dcCode, gupCode, custCode, pickOrdNo).ToList();
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var cancelWmsOrdNos = f050801Repo.GetOrderIsCancelByWmsOrdNos(dcCode, gupCode, custCode, f051202s.Select(x=> x.WMS_ORD_NO).Distinct().ToList()).ToList();
			// 先進先出
			var findDetails = f051202s.Where(x => x.ITEM_CODE == f1903.ITEM_CODE && x.PICK_STATUS == "0").OrderBy(x => x.VALID_DATE).ThenBy(x => x.ENTER_DATE).ToList();
			// 先找非取消訂單的明細
			var f051202 = findDetails.Where(x => !cancelWmsOrdNos.Contains(x.WMS_ORD_NO)).FirstOrDefault();
			// 若找不到就取第一筆取消訂單的明細
			if (f051202 == null)
				f051202 = findDetails.FirstOrDefault();

			if(f051202 == null)
				return new SowItemResult { IsSuccessed = false, Message = Properties.Resources.P080804Service_ItemIsAllotFinish, ItemCode = f1903.ITEM_CODE, ItemName = f1903.ITEM_NAME };

			f051202.A_PICK_QTY += 1;
			if (f051202.B_PICK_QTY == f051202.A_PICK_QTY)
				f051202.PICK_STATUS = "1";
			f051202Repo.Update(f051202);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511 = f1511Repo.Find(x => x.DC_CODE == f051202.DC_CODE && x.GUP_CODE == f051202.GUP_CODE && x.CUST_CODE == f051202.CUST_CODE && x.ORDER_NO == f051202.PICK_ORD_NO && x.ORDER_SEQ == f051202.PICK_ORD_SEQ);
			f1511.A_PICK_QTY += 1;
			if (f1511.B_PICK_QTY == f1511.A_PICK_QTY)
				f1511.STATUS = "1";
			f1511Repo.Update(f1511);

			#endregion

			#region 更新分貨頭檔、身擋 檢查是否容器完成

			var isContainerFinished = false;

			f05290401.A_SET_QTY += 1;
			f05290401Repo.Update(f05290401);

			var f052904Repo = new F052904Repository(Schemas.CoreSchema, _wmsTransaction);
			var f052904 = f052904Repo.GetDataByPickContianerCode(dcCode, gupCode, custCode, pickOrdNo, containerCode);

			if (f05290401.B_SET_QTY == f05290401.A_SET_QTY)
			{
				var datas = f05290401Repo.GetDatas(dcCode, gupCode, custCode, pickOrdNo, containerCode).ToList();
				isContainerFinished =  datas.Where(x => x.ITEM_CODE != f1903.ITEM_CODE).All(x => x.B_SET_QTY == x.A_SET_QTY);
			}
			result.IsContainerFinished = isContainerFinished;

			#endregion

			#region 新增/更新播種明細
			var f050801 = f050801Repo.GetData(f051202.WMS_ORD_NO, f051202.GUP_CODE, f051202.CUST_CODE, f051202.DC_CODE);
			var boxNo = f050801.STATUS == 9 ? canelOrderBoxNo : normalBoxNo;
			var sowType = f050801.STATUS == 9 ? "02" : "01";
			var f052905Repo = new F052905Repository(Schemas.CoreSchema,_wmsTransaction);
			var f052905 = f052905Repo.AsForUpdate().GetData(f052904.DC_CODE, f052904.GUP_CODE, f052904.CUST_CODE, f052904.DELV_DATE, f052904.PICK_TIME, f052904.MOVE_OUT_TARGET, sowType, boxNo);
			var f05290501Repo = new F05290501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05290501s = f05290501Repo.AsForUpdate().GetDatasByRefId(f052905.ID).ToList();

			var sowQty = f05290501s.Sum(x => x.A_SET_QTY);
			sowQty++;

			result.BoxInfo = new BoxInfo
			{
				BoxNo = boxNo,
				SowType = sowType,
				SowQty = sowQty
			};
			F05290501 addOrModifyF05290501; 
			var findF05290501 = f05290501s.FirstOrDefault(x => x.DC_CODE == f051202.DC_CODE && x.GUP_CODE == f051202.GUP_CODE && x.CUST_CODE == f051202.CUST_CODE
															&& x.PICK_ORD_NO == f051202.PICK_ORD_NO && x.PICK_ORD_SEQ == f051202.PICK_ORD_SEQ && x.WMS_ORD_NO == f051202.WMS_ORD_NO && 
															x.WMS_ORD_SEQ == f051202.WMS_ORD_SEQ && string.IsNullOrEmpty(x.SERIAL_NO));

			if (f2501 != null || findF05290501 == null)
			{
				var f05290501 = new F05290501
				{
					DC_CODE = f051202.DC_CODE,
					GUP_CODE = f051202.GUP_CODE,
					CUST_CODE = f051202.CUST_CODE,
					PICK_ORD_NO = f051202.PICK_ORD_NO,
					PICK_ORD_SEQ = f051202.PICK_ORD_SEQ,
					WMS_ORD_NO = f051202.WMS_ORD_NO,
					WMS_ORD_SEQ = f051202.WMS_ORD_SEQ,
					ITEM_CODE = f051202.ITEM_CODE,
					REF_ID = f052905.ID,
					A_SET_QTY = 1,
					SERIAL_NO = f2501 == null ? null : f2501.SERIAL_NO
				};
				f05290501Repo.Add(f05290501,"ID");
				addOrModifyF05290501 = f05290501;
			}
			else
			{
				findF05290501.A_SET_QTY += 1;
				f05290501Repo.Update(findF05290501);
				addOrModifyF05290501 = findF05290501;
			}

			#endregion

			#region 若是正常出貨(01)且為序號商品 更新序號狀態為C1(出庫)
			if (f2501 != null && sowType == "01")
			{
				// 更新序號為出庫C1
				f2501.STATUS = "C1";
        f2501.WMS_NO = f050801.WMS_ORD_NO;
        f2501.ORD_PROP = f050801.ORD_PROP;
				f2501Repo.Update(f2501);
			}

			#endregion

			result.IsBatchFinished = false;

			#region 容器分貨完成-更新分貨狀態、釋放容器、出貨扣帳、批次完成更新正常出貨箱子為扣帳、取消訂單箱子為關箱
			//如果容器分貨完成
			if (isContainerFinished)
			{
				// 更新分貨狀態
				f052904.STATUS = "2"; //分貨完成
				f052904Repo.Update(f052904);

				// 釋放容器
				var f0701Repo = new F0701Repository(Schemas.CoreSchema, _wmsTransaction);
				var f070101Repo = new F070101Repository(Schemas.CoreSchema, _wmsTransaction);
				var f0701 = f070101Repo.GetDatasByWmsNoAndContainerCode(dcCode, gupCode, custCode, pickOrdNo, containerCode);
				f0701Repo.DeleteF0701(f0701.F0701_ID);

				// 揀貨單明細都完成
				if (f051202s.All(x => x.PICK_STATUS == "1"))
				{
				

					var addOrUpdateWmsShipBoxDetail = new WmsShipBoxDetail
					{
					   ID = addOrModifyF05290501.ID,
						 DC_CODE = f052905.DC_CODE,
						 GUP_CODE = f052905.GUP_CODE,
						 CUST_CODE = f052905.CUST_CODE,
						 DELV_DATE = f052905.DELV_DATE,
						 PICK_TIME = f052905.PICK_TIME,
						 BOX_NUM = f052905.CONTAINER_CODE,
						 PACKAGE_BOX_NO = f052905.CONTAINER_SEQ,
						 PACKAGE_STAFF = f052905.CRT_STAFF,
						 PACKAGE_NAME = f052905.CRT_NAME,
						 WMS_ORD_NO = addOrModifyF05290501.WMS_ORD_NO,
						 WMS_ORD_SEQ = addOrModifyF05290501.WMS_ORD_SEQ,
						 ITEM_CODE = addOrModifyF05290501.ITEM_CODE,
						 SERIAL_NO = addOrModifyF05290501.SERIAL_NO,
						 PACKAGE_QTY = addOrModifyF05290501.A_SET_QTY,
						 PICK_ORD_NO = f051202.PICK_ORD_NO,
						 PICK_ORD_SEQ = f051202.PICK_ORD_SEQ
					};
					// 出貨扣帳
					var shipDebitResult = ShipDebit(dcCode,gupCode,custCode,f052904.DELV_DATE,f052904.PICK_TIME,f051202s.Select(x=> x.WMS_ORD_NO).Distinct().ToList(),f052904.PICK_ORD_NO, addOrUpdateWmsShipBoxDetail);
					if (!shipDebitResult.IsSuccessed)
						return new SowItemResult { IsSuccessed = shipDebitResult.IsSuccessed, Message = shipDebitResult.Message };

					// 檢查批次所有揀貨明細是否完成
					var isBatchFinished = f051202Repo.IsBatchFinished(dcCode, gupCode, custCode, f052904.DELV_DATE, f052904.PICK_TIME, f052904.PICK_ORD_NO);
					if (isBatchFinished)
						// 批次完成 將該批次播種箱更新為扣帳(2)
						f052905Repo.UpdateToDebit(dcCode, gupCode, custCode, f052904.DELV_DATE, f052904.PICK_TIME);

					result.IsBatchFinished = isBatchFinished;
				}
			}

			#endregion

			return result;
		}

		/// <summary>
		/// 出貨扣帳
		/// </summary>
		/// <param name="f051202s"></param>
		/// <returns></returns>
		private ExecuteResult ShipDebit(string dcCode,string gupCode,string custCode,DateTime delvDate,string pickTime,List<string> wmsOrdNos,string excludePickOrdNo,WmsShipBoxDetail addOrUpdateWmsShipBoxDetail)
		{
			var f051202Repo = new F051202Repository(Schemas.CoreSchema);
			var f051202s = f051202Repo.GetDatasByWmsOrdNos(dcCode, gupCode, custCode, wmsOrdNos).ToList();
			var pickFinishedWmsNos = new List<string>();
			foreach (var wmsOrdNo in wmsOrdNos)
			{
				//檢查出貨單是否揀貨完成
				var isWmsPickFinished = f051202s.Where(x => x.PICK_ORD_NO != excludePickOrdNo && x.WMS_ORD_NO == wmsOrdNo).All(x => x.PICK_STATUS == "1");
				if (isWmsPickFinished)
					pickFinishedWmsNos.Add(wmsOrdNo);
			}

			if(pickFinishedWmsNos.Any())
			{
				var f050801Repo = new F050801Repository(Schemas.CoreSchema,_wmsTransaction);
				var cancelWmsOrdNos = f050801Repo.GetOrderIsCancelByWmsOrdNos(dcCode, gupCode, custCode, pickFinishedWmsNos).ToList();
				pickFinishedWmsNos = pickFinishedWmsNos.Except(cancelWmsOrdNos).ToList();

				if(pickFinishedWmsNos.Any())
				{
					// 正常出貨訂單要寫入回檔紀錄(包裝完成)
					var orderService = new OrderService(_wmsTransaction);
					orderService.AddF050305(dcCode, gupCode, custCode, pickFinishedWmsNos, "2");
					orderService.AddF050305(dcCode, gupCode, custCode, pickFinishedWmsNos, "3");
					var addF055001List = new List<F055001>();
					var addF055002List = new List<F055002>();
					// 產生箱明細F055001,F055002
					var f05290501Repo = new F05290501Repository(Schemas.CoreSchema);
					var wmsShipBoxDetail = f05290501Repo.GetWmsShipBoxDetails(dcCode, gupCode, custCode, pickFinishedWmsNos).ToList();
					if(addOrUpdateWmsShipBoxDetail!= null &&  pickFinishedWmsNos.Contains(addOrUpdateWmsShipBoxDetail.WMS_ORD_NO))
					{
						// 如果是新增 產生一筆箱明細
						if(addOrUpdateWmsShipBoxDetail.ID ==0)
						{
							var findShipDetails = wmsShipBoxDetail.Where(x => x.BOX_NUM == addOrUpdateWmsShipBoxDetail.BOX_NUM &&  x.WMS_ORD_NO == addOrUpdateWmsShipBoxDetail.WMS_ORD_NO).ToList();
							if (findShipDetails.Any())
								addOrUpdateWmsShipBoxDetail.PACKAGE_BOX_SEQ = findShipDetails.Max(x => x.PACKAGE_BOX_SEQ) + 1;
							else
								addOrUpdateWmsShipBoxDetail.PACKAGE_BOX_SEQ = 1;

							var findShipDetail = findShipDetails.FirstOrDefault(x => x.ITEM_CODE == addOrUpdateWmsShipBoxDetail.ITEM_CODE && string.IsNullOrEmpty(x.SERIAL_NO));
							if (string.IsNullOrEmpty(addOrUpdateWmsShipBoxDetail.SERIAL_NO) && findShipDetail != null)
								findShipDetail.PACKAGE_QTY += addOrUpdateWmsShipBoxDetail.PACKAGE_QTY;
							else
								wmsShipBoxDetail.Add(addOrUpdateWmsShipBoxDetail);
						}
						else
						{
							// 如果是修改 更新數量
							var wmsShipBox = wmsShipBoxDetail.First(x => x.ID == addOrUpdateWmsShipBoxDetail.ID);
							wmsShipBox.PACKAGE_QTY = addOrUpdateWmsShipBoxDetail.PACKAGE_QTY;
						}
					}

					// 同一訂單不同項次會對應同一筆出貨項次，所以要進行分配訂單項次
					var f05030202Repo = new F05030202Repository(Schemas.CoreSchema,_wmsTransaction);
					var f05030202s = f05030202Repo.GetDatas(dcCode, gupCode, custCode, pickFinishedWmsNos).ToList();
					var addDetails = new List<WmsShipBoxDetail>();
					foreach(var wmsShipBox in wmsShipBoxDetail)
					{
						var packageQty = wmsShipBox.PACKAGE_QTY;
						var canAllotF05030202s = f05030202s.Where(x => x.WMS_ORD_NO == wmsShipBox.WMS_ORD_NO && x.WMS_ORD_SEQ == wmsShipBox.WMS_ORD_SEQ && x.B_DELV_QTY> x.A_DELV_QTY).ToList();
						foreach(var f05030202 in canAllotF05030202s)
						{
							var canAllotQty = f05030202.B_DELV_QTY - f05030202.A_DELV_QTY??0;
							if (canAllotQty >= packageQty)
							{
								wmsShipBox.ORD_NO = f05030202.ORD_NO;
								wmsShipBox.ORD_SEQ = f05030202.ORD_SEQ;
								f05030202.A_DELV_QTY += packageQty;
								break;
							}
							else
							{
								f05030202.A_DELV_QTY += canAllotQty;
								packageQty -= canAllotQty;
								wmsShipBox.PACKAGE_QTY -= canAllotQty;
								var newWmsShipBox = AutoMapper.Mapper.DynamicMap<WmsShipBoxDetail>(wmsShipBox);
								newWmsShipBox.ORD_NO = f05030202.ORD_NO;
								newWmsShipBox.ORD_SEQ = f05030202.ORD_SEQ;
								newWmsShipBox.PACKAGE_BOX_SEQ = wmsShipBoxDetail.Where(x => x.PACKAGE_BOX_NO == wmsShipBox.PACKAGE_BOX_NO).Max(x => x.PACKAGE_BOX_SEQ) + 1;
								newWmsShipBox.PACKAGE_QTY = canAllotQty;
								addDetails.Add(newWmsShipBox);
							}
						}
					}
					wmsShipBoxDetail.AddRange(addDetails);

					var groupWmsOrders = wmsShipBoxDetail.GroupBy(x => new { x.DC_CODE, x.GUP_CODE, x.CUST_CODE, x.DELV_DATE, x.PICK_TIME, x.BOX_NUM, x.PACKAGE_BOX_NO, x.PACKAGE_STAFF, x.PACKAGE_NAME, x.WMS_ORD_NO });
					foreach(var groupWmsOrder in groupWmsOrders)
					{
						var f055001 = new F055001
						{
							DC_CODE = groupWmsOrder.Key.DC_CODE,
							GUP_CODE = groupWmsOrder.Key.GUP_CODE,
							CUST_CODE = groupWmsOrder.Key.CUST_CODE,
							DELV_DATE = groupWmsOrder.Key.DELV_DATE,
							PICK_TIME = groupWmsOrder.Key.PICK_TIME,
							PACKAGE_BOX_NO = (short)groupWmsOrder.Key.PACKAGE_BOX_NO,
							BOX_NUM = groupWmsOrder.Key.BOX_NUM,
							WMS_ORD_NO = groupWmsOrder.Key.WMS_ORD_NO,
							PACKAGE_STAFF = groupWmsOrder.Key.PACKAGE_STAFF,
							PACKAGE_NAME = groupWmsOrder.Key.PACKAGE_NAME,
							STATUS = "0",
						};
						addF055001List.Add(f055001);
						foreach(var detail in groupWmsOrder)
						{
							
							var f055002 = new F055002
							{
								DC_CODE = detail.DC_CODE,
								GUP_CODE = detail.GUP_CODE,
								CUST_CODE = detail.CUST_CODE,
								WMS_ORD_NO = detail.WMS_ORD_NO,
								PACKAGE_BOX_NO = (short)detail.PACKAGE_BOX_NO,
								PACKAGE_BOX_SEQ = detail.PACKAGE_BOX_SEQ,
								ITEM_CODE = detail.ITEM_CODE,
								SERIAL_NO = detail.SERIAL_NO,
								PACKAGE_QTY = detail.PACKAGE_QTY,
								ORD_NO = detail.ORD_NO,
								ORD_SEQ = detail.ORD_SEQ
							};
							addF055002List.Add(f055002);
						}
					}
					var f055001Repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);
					var f055002Repo = new F055002Repository(Schemas.CoreSchema, _wmsTransaction);
					f055001Repo.BulkInsert(addF055001List);
					f055002Repo.BulkInsert(addF055002List);

					List<F0513> updF0513s;
					List<F05030202> updF05030202s;
					List<F050801> updF050801s;
					List<F050802> updF050802s;
					List<F1511> updF1511s;
					var f0513Repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
					var f050802Repo = new F050802Repository(Schemas.CoreSchema, _wmsTransaction);
					var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
					// 出貨單扣帳
					var result = orderService.MultiShipOrderDebit(dcCode, gupCode, custCode, pickFinishedWmsNos, out updF050801s, out updF050802s, out updF05030202s, out updF1511s, out updF0513s);
					if (!result.IsSuccessed)
						return result;

					if(addOrUpdateWmsShipBoxDetail!=null && pickFinishedWmsNos.Contains(addOrUpdateWmsShipBoxDetail.WMS_ORD_NO))
					{
						// 出貨扣帳時，還不會有這筆揀貨明細的數量，所以要把他+1進去
						var f1511 = updF1511s.First(x => x.ORDER_NO == addOrUpdateWmsShipBoxDetail.PICK_ORD_NO && x.ORDER_SEQ == addOrUpdateWmsShipBoxDetail.PICK_ORD_SEQ);
						f1511.A_PICK_QTY += 1;
						// 出貨扣帳時，還不會有這筆揀貨明細的數量，所以要把他+1進去
						var f050802 = updF050802s.First(x => x.WMS_ORD_NO == addOrUpdateWmsShipBoxDetail.WMS_ORD_NO && x.WMS_ORD_SEQ == addOrUpdateWmsShipBoxDetail.WMS_ORD_SEQ);
						f050802.A_DELV_QTY += 1;
						// 可能出貨項次對應到多張訂單項次，所以加到第一個實際出貨數<預計出貨數那一筆
						var f05030202 = updF05030202s.First(x => x.WMS_ORD_NO == addOrUpdateWmsShipBoxDetail.WMS_ORD_NO && x.WMS_ORD_SEQ == addOrUpdateWmsShipBoxDetail.WMS_ORD_SEQ && x.B_DELV_QTY > x.A_DELV_QTY);
						f05030202.A_DELV_QTY += 1;
					}

          if (updF050801s.Any())
          {
            updF050801s.ForEach(x => x.SHIP_MODE = "4");
            f050801Repo.BulkUpdate(updF050801s);
          }
					if (updF050802s.Any())
						f050802Repo.BulkUpdate(updF050802s);

					if (updF0513s.Any())
						f0513Repo.BulkUpdate(updF0513s);

					if (updF05030202s.Any())
						f05030202Repo.BulkUpdate(updF05030202s);

					if (updF1511s.Any())
						f1511Repo.BulkUpdate(updF1511s);
				}
			}
			return new ExecuteResult { IsSuccessed = true };
		}

		/// <summary>
		/// 取得揀缺商品清單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="pickOrdNo"></param>
		/// <returns></returns>
		public LackItemResult GetPickLackItems(string dcCode,string gupCode,string custCode,string pickOrdNo)
		{
			var f051202Repo = new F051202Repository(Schemas.CoreSchema);
			var f051202s = f051202Repo.AsForUpdate().GetNotFinishDataByPickNo(dcCode, gupCode, custCode, pickOrdNo).ToList();
			var containerService = new ContainerService();

      if (_commonService == null)
      {
        _commonService = new CommonService();
      }
			
			var itemCodes = f051202s.Select(x => x.ITEM_CODE).Distinct().ToList();
			var items = _commonService.GetProductList(gupCode, custCode, itemCodes);
			// 產生缺品清單
			var lackItemDetails = f051202s.GroupBy(x => x.ITEM_CODE)
				.Select(x => new LackItem
				{
					ItemCode = x.Key,
					ItemName = items.First(y => y.ITEM_CODE == x.Key).ITEM_NAME,
					LackQty = x.Sum(y => y.B_PICK_QTY - y.A_PICK_QTY)
				}).ToList();

			// 容器釋放
			containerService.DelContainer(dcCode, gupCode, custCode, pickOrdNo);

			return new LackItemResult
			{
				IsSuccessed = true,
				LackItemDetails = lackItemDetails
			};
		}

		

		/// <summary>
		/// 取得容器未分貨清單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="pickOrdNo"></param>
		/// <param name="containerCode"></param>
		/// <returns></returns>
		public LackItemResult GetContainerLackItems(string dcCode,string gupCode,string custCode,string pickOrdNo,string containerCode)
		{
			var f05290401Repo = new F05290401Repository(Schemas.CoreSchema, _wmsTransaction);

      if (_commonService == null)
      {
        _commonService = new CommonService();
      }

			var f05290401s = f05290401Repo.GetLackDatasByPickContainerCode(dcCode, gupCode, custCode, pickOrdNo, containerCode).ToList();
			var itemCodes = f05290401s.Select(x => x.ITEM_CODE).Distinct().ToList();
			var items = _commonService.GetProductList(gupCode, custCode, itemCodes);
			// 產生缺品清單
			var lackItemDetails = f05290401s.GroupBy(x => x.ITEM_CODE)
				.Select(x => new LackItem
				{
					ItemCode = x.Key,
					ItemName = items.First(y => y.ITEM_CODE == x.Key).ITEM_NAME,
					LackQty = x.Sum(y => y.B_SET_QTY - y.A_SET_QTY)
				}).ToList();

			return new LackItemResult
			{
				IsSuccessed = true,
				LackItemDetails = lackItemDetails
			};
		}

		/// <summary>
		/// 容器完成
		/// 揀貨單有多箱，不是最後一箱情境，若實際容器可分貨商品與預計該容器分貨數不同時，由人員手動按下容器完成
		/// 設定該筆分貨容器資料為缺貨
		/// 釋放容器綁定紀錄
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="pickOrdNo"></param>
		/// <param name="containerCode"></param>
		/// <returns></returns>
		public ExecuteResult ContainerComplete(string dcCode,string gupCode,string custCode,string pickOrdNo,string containerCode)
		{
			var f052904Repo = new F052904Repository(Schemas.CoreSchema, _wmsTransaction);
			var f052904 = f052904Repo.GetDataByPickContianerCode(dcCode, gupCode, custCode, pickOrdNo, containerCode);
			f052904.STATUS = "3"; //設為缺貨
			f052904Repo.Update(f052904);

			// 釋放容器
			var f0701Repo = new F0701Repository(Schemas.CoreSchema, _wmsTransaction);
			f0701Repo.DeleteByContainerCode(dcCode, custCode, containerCode);

			return new ExecuteResult(true);
		}

		/// <summary>
		/// 缺品出貨
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="pickOrdNo"></param>
		/// <returns></returns>
		public PickOutOfStockResult PickOutOfStockComfirm(string dcCode, string gupCode, string custCode, string pickOrdNo,string containerCode)
		{
			var f052904Repo = new F052904Repository(Schemas.CoreSchema, _wmsTransaction);
			var f052904 = f052904Repo.GetDataByPickContianerCode(dcCode, gupCode, custCode, pickOrdNo, containerCode);
			f052904.STATUS = "3"; //設為缺貨
			f052904Repo.Update(f052904);

			// 釋放容器
			var f0701Repo = new F0701Repository(Schemas.CoreSchema, _wmsTransaction);
			f0701Repo.DeleteByContainerCode(dcCode, custCode, containerCode);

			var f051202Repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051202s = f051202Repo.GetNotCacnelDataByPickNo(dcCode, gupCode, custCode, pickOrdNo).ToList();
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511s = f1511Repo.GetDatas(dcCode,gupCode,custCode,pickOrdNo).ToList();
			var sharedService = new SharedService(_wmsTransaction);
			var f191302Repo = new F191302Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051206Repo = new F051206Repository(Schemas.CoreSchema, _wmsTransaction);
			var returnStocks = new List<F1913>();
			var returnAllotList = new List<ReturnNewAllocation>();
			var updF051202List = new List<F051202>();
			var updF1511List = new List<F1511>();
			var addF191302List = new List<F191302>();
			var addF051206List = new List<F051206>();

			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			var pickWmsOrdNos = f051202s.Select(x => x.WMS_ORD_NO).Distinct().ToList();
			var cancelWmsOrdNos = f050801Repo.GetOrderIsCancelByWmsOrdNos(dcCode, gupCode, custCode, pickWmsOrdNos).ToList();


			// 取得疑似遺失倉-倉庫編號
			var pickLossWHId = sharedService.GetPickLossWarehouseId(dcCode, gupCode, custCode);
			if (string.IsNullOrWhiteSpace(pickLossWHId))
			{
				var msg = string.Format(Properties.Resources.P080804Service_DcNotSetLossWarehouse, dcCode);
				return new PickOutOfStockResult { IsSuccessed = false, Message = msg };
			}
			// 疑似遺失倉第一個儲位
			var pickLossLocCode = sharedService.GetPickLossLoc(dcCode, pickLossWHId);

			var lackPickDetail = f051202s.Where(x => x.PICK_STATUS == "0").ToList();
			var lackVirtualDetail = f1511s.Where(x => x.STATUS == "0").ToList();

			foreach (var f051202 in lackPickDetail)
			{
				var f1511 = lackVirtualDetail.First(x => x.ORDER_SEQ == f051202.PICK_ORD_SEQ);
				// 庫存異常處理(調撥到疑似遺失倉+庫存異常處理)
				var stockLack = new StockLack
				{
					DcCode = dcCode,
					GupCode = gupCode,
					CustCode =custCode,
					LackQty =f051202.B_PICK_QTY-f051202.A_PICK_QTY,
					PickLackWarehouseId = pickLossWHId,
					PickLackLocCode = pickLossLocCode,
					F051202 = f051202,
					F1511 = f1511,
					ReturnStocks = returnStocks
				};
				var result = sharedService.CreateStockLackProcess(stockLack);
				if (!result.IsSuccessed)
					return new PickOutOfStockResult { IsSuccessed = result.IsSuccessed, Message = result.Message };
				returnStocks = result.ReturnStocks;
				returnAllotList.AddRange(result.ReturnNewAllocations);
				updF051202List.Add(result.UpdF051202);
				if(!cancelWmsOrdNos.Contains(result.UpdF051202.WMS_ORD_NO))
					result.UpdF1511.STATUS = "2"; //狀態直接更新為扣帳
				updF1511List.Add(result.UpdF1511);
				addF191302List.AddRange(result.AddF191302List);
				// 寫入揀貨缺貨紀錄(狀態直接已確認  訂單取消押處理結果為2 缺品出貨押處理結果為1)
				var f051206 = new F051206
				{
					DC_CODE = f051202.DC_CODE,
					GUP_CODE = f051202.GUP_CODE,
					CUST_CODE = f051202.CUST_CODE,
					PICK_ORD_NO = f051202.PICK_ORD_NO,
					PICK_ORD_SEQ = f051202.PICK_ORD_SEQ,
					ITEM_CODE = f051202.ITEM_CODE,
					LOC_CODE = f051202.PICK_LOC,
					WMS_ORD_NO = f051202.WMS_ORD_NO,
					LACK_QTY = f051202.B_PICK_QTY - f051202.A_PICK_QTY,
					ISDELETED = "0",
					STATUS = "2",
					TRANS_FLAG = "0",
					RETURN_FLAG = cancelWmsOrdNos.Contains(f051202.WMS_ORD_NO) ? "2" : "1"
				};
				addF051206List.Add(f051206);
			}
			// 調撥單整批上架
			var result2 = sharedService.BulkAllocationToAllUp(returnAllotList, returnStocks, false, addF191302List);
			if (!result2.IsSuccessed)
				return new PickOutOfStockResult { IsSuccessed = result2.IsSuccessed, Message = result2.Message };
			// 調撥單整批寫入
			var result3 = sharedService.BulkInsertAllocation(returnAllotList, returnStocks, true);
			if (!result3.IsSuccessed)
				return new PickOutOfStockResult { IsSuccessed = result3.IsSuccessed, Message = result3.Message };

			f051202Repo.BulkUpdate(updF051202List);
			f1511Repo.BulkUpdate(updF1511List);
			f051206Repo.BulkInsert(addF051206List, "LACK_SEQ");
			f191302Repo.BulkInsert(addF191302List, true);

			// 出貨扣帳
			var canDebitWmsOrdNos = pickWmsOrdNos.Except(cancelWmsOrdNos).ToList();
			var shipDebitResult = ShipDebit(dcCode, gupCode, custCode, f052904.DELV_DATE, f052904.PICK_TIME, canDebitWmsOrdNos, f052904.PICK_ORD_NO, null);
			if (!shipDebitResult.IsSuccessed)
				return new PickOutOfStockResult { IsSuccessed = shipDebitResult.IsSuccessed, Message = shipDebitResult.Message };

			// 檢查批次所有揀貨明細是否完成
			var f052905Repo = new F052905Repository(Schemas.CoreSchema, _wmsTransaction);
			var isBatchFinished = f051202Repo.IsBatchFinished(dcCode, gupCode, custCode, f052904.DELV_DATE, f052904.PICK_TIME, f052904.PICK_ORD_NO);
			if (isBatchFinished)
				// 批次完成 將該批次播種箱更新為扣帳(2)
				f052905Repo.UpdateToDebit(dcCode, gupCode, custCode, f052904.DELV_DATE, f052904.PICK_TIME);

			return new PickOutOfStockResult { IsSuccessed = true, IsContainerFinished = true, IsBatchFinished = isBatchFinished };
		}

		#region 稽核出庫-箱內明細
		public IQueryable<P0808040100_BoxData> GetBoxData(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string sowType, string status)
		{
			var f052905Repo = new F052905Repository(Schemas.CoreSchema);
			return f052905Repo.GetBoxData(dcCode, gupCode, custCode, delvDate, pickTime, sowType, status);
		}

		public IQueryable<P0808040100_BoxDetailData> GetBoxDetailData(Int64 refId)
		{
			var f052905Repo = new F05290501Repository(Schemas.CoreSchema);
			return f052905Repo.GetBoxDetailData(refId);
		}

		public IQueryable<P0808040100_PrintData> GetPrintBoxData(string dcCode, string gupCode, string custCode, DateTime delvDate, string pickTime, string moveOutTarget, string containerCode, string sowType)
		{
			var f052905Repo = new F05290501Repository(Schemas.CoreSchema);
			return f052905Repo.GetPrintBoxData(dcCode, gupCode, custCode, delvDate, pickTime, moveOutTarget, containerCode, sowType);
		}
		#endregion


		#region 揀貨單批次查詢
		public IQueryable<BatchPickData> GetBatchPickData(string dcCode,string gupCode,string custCode,string containerBarcode)
		{
			var f0701Repo = new F0701Repository(Schemas.CoreSchema, _wmsTransaction);
			var f070101Repo = new F070101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051201Repo = new F051201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0513Repo = new F0513Repository(Schemas.CoreSchema, _wmsTransaction);
			var f0701 = f0701Repo.GetDatasByF0701ContainerCode(dcCode, containerBarcode);
			if (f0701 != null)
			{
				var f070101 = f070101Repo.GetDataByF0701Id(f0701.ID);
				var f051201 = f051201Repo.GetF051201(dcCode, gupCode, custCode, f070101?.WMS_NO);
				if (f051201 == null)
					return new List<BatchPickData>().AsQueryable(); ;
				return f0513Repo.GetNotCancelBatchPickData(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE,f051201.DELV_DATE, f051201.PICK_TIME);
			}
			else
			{
				var f051201 = f051201Repo.GetF051201(dcCode, gupCode, custCode, containerBarcode);
				if (f051201 == null)
					return new List<BatchPickData>().AsQueryable(); ;
				return  f0513Repo.GetNotCancelBatchPickData(f051201.DC_CODE, f051201.GUP_CODE, f051201.CUST_CODE,f051201.DELV_DATE, f051201.PICK_TIME);
			}
			
		}
		#endregion

	}
}
