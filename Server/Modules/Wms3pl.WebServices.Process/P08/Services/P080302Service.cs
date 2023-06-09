
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F19;
using Wms3pl.Common.Enums;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;

namespace Wms3pl.WebServices.Process.P08.Services
{
	public partial class P080302Service
	{
		private WmsTransaction _wmsTransaction;

		public P080302Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F151002DataByTar> GetF151002DataByTars(string tarDcCode, string gupCode, string custCode,
			string allocationNo, string userId, string userName, bool isAllowStatus4, bool isDiffWareHouse)
		{
			var f151002Repo = new F151002Repository(Schemas.CoreSchema);
			var queryAllocationNo = allocationNo;
			if (string.IsNullOrEmpty(allocationNo))
			{
				var items = f151002Repo.GetF151002DataByTars(tarDcCode, gupCode, custCode, allocationNo, userId, isAllowStatus4, isDiffWareHouse);
				//優先給調撥單上架人員等於登入者 否則隨機給他一張調撥單
				var item = items.FirstOrDefault(o => o.TAR_MOVE_STAFF == userId && o.TAR_MOVE_NAME == userName) ?? items.FirstOrDefault();
				if (item != null)
					queryAllocationNo = item.ALLOCATION_NO;
			}
			if (string.IsNullOrEmpty(queryAllocationNo))
				return new List<F151002DataByTar>().AsQueryable();

			var data = f151002Repo.GetF151002DataByTars(tarDcCode, gupCode, custCode, queryAllocationNo, userId, isAllowStatus4, isDiffWareHouse);
			var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
			var combinItemData = data.Where(o => o.COMBIN_NO.HasValue).ToList();
			foreach (var f151002Data in combinItemData)
			{
				var list = f2501Repo.GetDatasByCombinNo(gupCode, custCode, f151002Data.COMBIN_NO.Value).ToList();
				f151002Data.SERIAL_NOByShow = string.Join("、", list.Where(o => o.SERIAL_NO != f151002Data.SERIAL_NO).Select(o => o.SERIAL_NO).ToArray());
			}
			return data;
		}

		public ExecuteResult OutOfStockByTar(string dcCode, string gupCode, string custCode, string allocationNo,
				string sugLocCode, string itemCode, string validDate, string serialNo, string makeNo, string boxCrtlNo, string palletNo, string stickerPalletNo = "")
		{
			var result = new ExecuteResult() { IsSuccessed = true };
			var dtmValidDate = DateTime.Parse(validDate);
			var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
			var f151002Data =
					f151002Repo.Filter(
							o =>
									o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo &&
									o.ITEM_CODE == itemCode && o.SUG_LOC_CODE == sugLocCode &&
									((o.VALID_DATE == dtmValidDate && !o.SRC_VALID_DATE.HasValue) || o.SRC_VALID_DATE == dtmValidDate) &&
									((o.MAKE_NO == makeNo && o.SRC_MAKE_NO == null) || o.SRC_MAKE_NO == makeNo) &&
									o.BOX_CTRL_NO == boxCrtlNo && o.PALLET_CTRL_NO == palletNo).ToList();
			f151002Data = string.IsNullOrEmpty(serialNo)
					? f151002Data.Where(o => string.IsNullOrEmpty(o.SERIAL_NO)).ToList()
					: f151002Data.Where(o => o.SERIAL_NO == serialNo).ToList();
			f151002Data = f151002Data.Where(o => o.STATUS == "1" && o.TAR_QTY > 0).ToList();
			var f151003Repo = new F151003Repository(Schemas.CoreSchema, _wmsTransaction);
			foreach (var f151002 in f151002Data)
			{
				var f151003 = new F151003
				{
					ALLOCATION_NO = f151002.ALLOCATION_NO,
					ALLOCATION_SEQ = f151002.ALLOCATION_SEQ,
					ITEM_CODE = f151002.ITEM_CODE,
					MOVE_QTY = (int)f151002.TAR_QTY,
					LACK_QTY = (int)(f151002.TAR_QTY - f151002.A_TAR_QTY),
					REASON = "101",
					STATUS = "0",
					DC_CODE = f151002.DC_CODE,
					GUP_CODE = f151002.GUP_CODE,
					CUST_CODE = f151002.CUST_CODE,
                    LACK_TYPE = "1"
				};
				f151003Repo.Add(f151003);
				f151002.STATUS = "2";
				f151002.STICKER_PALLET_NO = stickerPalletNo;
				f151002Repo.Update(f151002);
			}
			result.IsSuccessed = true;
			result.Message = "";

			return result;
		}

		public IQueryable<F151002ItemLocDataByTar> GetF151002ItemLocDataByTars(string dcCode, string gupCode, string custCode,
			string allocationNo, string itemCode, bool isDiffWareHouse)
		{
			var f151002Repo = new F151002Repository(Schemas.CoreSchema);
			return f151002Repo.GetF151002ItemLocDataByTars(dcCode, gupCode, custCode, allocationNo, itemCode, isDiffWareHouse);
		}

		public ExecuteResult CheckTarLocCode(string dcCode, string wareHouseId, string locCode, string userId, string itemCode)
		{
			var sharedService = new SharedService();
			return sharedService.CheckLocCode(locCode, dcCode, wareHouseId, userId, itemCode);
		}

		public ExecuteResult ScanTarLocItemCodeActualQty(string tarDcCode, string dcCode, string gupCode, string custCode, string allocationNo, string sugLocCode, string tarLocCode, string itemCode,
				string serialNo, string orginalValidDate, string newValidDate, int addActualQty, string userId, string wareHouseId, string scanCode,
				string orginalMakeNo, string newMakeNo, string palletCtrlNo, string boxCtrlNo, string stickerPalletNo = "")
		{
            var f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
            var warehouseInService = new WarehouseInService(_wmsTransaction);
            var serialNoService = new SerialNoService();
			var sn = string.IsNullOrWhiteSpace(scanCode) ? serialNo : scanCode;
			var serialNoList = new List<string>();
			bool hasSerial = false;
			if (!string.IsNullOrWhiteSpace(sn))
			{
				var f1903Repo = new F1903Repository(Schemas.CoreSchema);
				var f1903Item = f1903Repo.Find(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ITEM_CODE == itemCode);

				var combinF2501List = new List<F2501>();
				var combinItemCode = string.Empty;
				var isCombinItem = serialNoService.IsCombinItem(gupCode, custCode, sn, out combinF2501List, out combinItemCode);
				if (isCombinItem && combinItemCode == itemCode) //如果是組合商品 且目前商品=組合商品
				{
					if (f1903Item.BUNDLE_SERIALLOC == "1")
					{
						var item = combinF2501List.First(c => c.ITEM_CODE == combinItemCode);
						if (item.SERIAL_NO == serialNo) //序號=目前商品序號
							sn = item.SERIAL_NO;
					}
				}
				if (f1903Item.BUNDLE_SERIALNO == "0")
				{
					if (itemCode == sn)
						serialNoList.Add(string.Empty);
					else
						return new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P080301Service_ScanItemCodeError };
				}
				else
				{
					var checkSerialResult = serialNoService.CheckBarCode(gupCode, custCode, itemCode, sn, false);
					if (!checkSerialResult.IsSuccessed)
						return checkSerialResult;
					if (f1903Item.BUNDLE_SERIALLOC == "1")
					{
						var barCodeData = serialNoService.BarcodeInspection(gupCode, custCode, sn);
						//如果是barcode是儲值卡盒號規則 但是商品非儲值卡 就視為是序號
						if (barCodeData.Barcode == BarcodeType.BatchNo && !serialNoService.IsBatchNoItem(gupCode, custCode, itemCode))
							barCodeData.Barcode = BarcodeType.SerialNo;
						if (barCodeData.Barcode == BarcodeType.SerialNo && serialNo != sn)
							return new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P080301Service_ScanSerialNoError };

						serialNoList.AddRange(checkSerialResult.Message.Split(','));
						addActualQty = 1; //序號綁儲位 一律是1
						hasSerial = true;
					}
					else
						serialNoList.Add(string.Empty);
				}
			}
			else
				serialNoList.Add(string.Empty);

			//上架的調撥單明細，For LO
			var loF151002s = (from a in (new List<F151002>())
												select new { F151002 = new F151002(), AddTarQty = 0 }).ToList();


			var result = CheckTarLocCode(tarDcCode, wareHouseId, tarLocCode, userId, itemCode);
			if (result.IsSuccessed)
			{
				//檢查商品儲位溫層
				var sharedServie = new SharedService();
				var checkTmprMessage = sharedServie.CheckItemLocTmpr(tarDcCode, gupCode, itemCode, custCode, tarLocCode);
				if (!string.IsNullOrEmpty(checkTmprMessage))
					return new ExecuteResult { IsSuccessed = false, Message = checkTmprMessage };

				var dtmOrginalValidDate = DateTime.Parse(orginalValidDate); //原生效日
				var dtmNewValidDate = DateTime.Parse(newValidDate); //新生效日
				var dtmOrginalMakeNo = orginalMakeNo; //原批號
				var dtmNewMakeNo = newMakeNo; //新批號

				var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
				var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);

				var allDetails = f151002Repo.GetDatas(dcCode, gupCode, custCode, allocationNo).ToList();
				var seq = 1;
				if (allDetails.Any())
					seq += allDetails.Max(o => o.ALLOCATION_SEQ);

				var barcodeData = serialNoService.BarcodeInspection(gupCode, custCode, sn);
				if (barcodeData.Barcode == BarcodeType.BatchNo && !serialNoService.IsBatchNoItem(gupCode, custCode, itemCode))
					barcodeData.Barcode = BarcodeType.SerialNo;
				if (hasSerial)
				{
					if (serialNoList.Any(o => allDetails.All(c => c.SERIAL_NO != o)))
						return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.P080302Service_ScanSerialNoNotExist, barcodeData.BarcodeText) };
					if (!allDetails.Any(o => serialNoList.Any(c => c == o.SERIAL_NO) && o.STATUS == "1"))
						return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.P080301Service_ScanItemCodeIsScan, barcodeData.BarcodeText) };
					if (allDetails.Any(o => serialNoList.Any(c => c == o.SERIAL_NO) && o.STATUS == "2"))
						return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.P080301Service_ScanSerialIsScan, barcodeData.BarcodeText) };
				}
				var allocationDatas = allDetails.Where(o => o.SUG_LOC_CODE == sugLocCode && o.ITEM_CODE == itemCode).ToList();
				if (hasSerial)
				{
					if (serialNoList.Any(o => allocationDatas.All(c => c.SERIAL_NO != o)))
						return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.P080301Service_ScanSerialNoLocCodeNotExist, barcodeData.BarcodeText) };
				}
				var updateF151002List = new List<F151002>();
				var addF151002List = new List<F151002>();
				var removeF151002List = new List<F151002>();
				var addF1913List = new List<F1913>();
				var updateF1913List = new List<F1913>();

				bool isLocChange = sugLocCode != tarLocCode;
				bool isValidDateChange = dtmOrginalValidDate != dtmNewValidDate;
				bool isMakeNoChange = dtmOrginalMakeNo != dtmNewMakeNo;

				foreach (var serial in serialNoList)
				{
					var datas = string.IsNullOrEmpty(serialNo)
					? allocationDatas.Where(o => string.IsNullOrEmpty(o.SERIAL_NO)).ToList()
					: allocationDatas.Where(o => o.SERIAL_NO == serial).ToList();
					if (datas.Any())
					{
                        //取完全沒有改過效期、批號的資料回來，此處取多筆，主要讓不同的入庫日讓資料去扣算
                        var orginalDatas =
                                datas.Where(
                                        o =>
                                                ((o.VALID_DATE == dtmOrginalValidDate && !o.SRC_VALID_DATE.HasValue) || o.SRC_VALID_DATE == dtmOrginalValidDate) &&
                                                !o.TAR_VALID_DATE.HasValue &&
                                                (o.MAKE_NO == dtmOrginalMakeNo && string.IsNullOrWhiteSpace(o.SRC_MAKE_NO) || o.SRC_MAKE_NO == dtmOrginalMakeNo) &&
                                                string.IsNullOrWhiteSpace(o.TAR_MAKE_NO) &&
                                                o.SUG_LOC_CODE == sugLocCode &&
                                                o.SUG_LOC_CODE == o.TAR_LOC_CODE).ToList();

						List<F151002> newDatas = new List<F151002>() { };
						//拆出有異動儲位、效期、批號的資料
						newDatas = datas.ToList();
						if (isLocChange)
							newDatas = newDatas.Where(o => o.SUG_LOC_CODE != o.TAR_LOC_CODE).ToList();

						if (isValidDateChange)
							newDatas = newDatas.Where(
													o =>
													(o.VALID_DATE == dtmOrginalValidDate || o.SRC_VALID_DATE == dtmOrginalValidDate) && o.TAR_VALID_DATE == dtmNewValidDate)
													.ToList();
						if (isMakeNoChange)
							newDatas = newDatas.Where(
													o =>
														 (o.MAKE_NO == dtmOrginalMakeNo || o.SRC_MAKE_NO == dtmOrginalMakeNo) && o.TAR_MAKE_NO == dtmNewMakeNo)
													.ToList();

						if (!isLocChange && !isValidDateChange && !isMakeNoChange)
						{
							#region 沒改變儲位、效期、批號
							long tarQty = addActualQty;
							foreach (var orginalData in orginalDatas)
							{
								var loOrgiATarQty = orginalData.A_TAR_QTY; //記錄原上架數量，For LO
								var newTarQty = tarQty;
								var qty = orginalData.TAR_QTY - orginalData.A_TAR_QTY - tarQty;
								if (qty <= 0)
								{
									orginalData.A_TAR_QTY = orginalData.TAR_QTY;
									tarQty = -qty;
									newTarQty -= tarQty;
								}
								else
								{
									orginalData.A_TAR_QTY += tarQty;
									tarQty = 0;
								}
								if (orginalData.TAR_QTY == orginalData.A_TAR_QTY)
									orginalData.STATUS = "2";
								orginalData.STICKER_PALLET_NO = stickerPalletNo;
								updateF151002List.Add(orginalData);
								//增加LO上架的調撥單明細
								loF151002s.Add(new { F151002 = orginalData, AddTarQty = (int)(orginalData.A_TAR_QTY - loOrgiATarQty) });

								var orginalSerialNo = (string.IsNullOrEmpty(orginalData.SERIAL_NO)) ? "0" : orginalData.SERIAL_NO;
								SetF1913(ref addF1913List, ref updateF1913List, orginalData,
										(orginalData.SRC_VALID_DATE ?? orginalData.VALID_DATE),
										orginalData.SRC_MAKE_NO ?? orginalData.MAKE_NO,
										orginalSerialNo, newTarQty, tarDcCode);

								if (tarQty <= 0)
									break;
							}
							#endregion
						}
						else
						{
							#region 儲位、效期、批號其中一個改變的話
							long tarQty = addActualQty;
							foreach (var orginalData in orginalDatas)
							{
								var loOrgiATarQty = orginalData.A_TAR_QTY; //記錄原上架數量，For LO
								var newTarQty = tarQty;
								var qty = orginalData.TAR_QTY - orginalData.A_TAR_QTY - tarQty;
								if (qty <= 0)
								{
									orginalData.TAR_QTY = orginalData.A_TAR_QTY;
									tarQty = -qty;
									//扣掉超過的數量=這次的上架數
									newTarQty -= tarQty;
								}
								else
								{
									orginalData.TAR_QTY -= tarQty;
									tarQty = 0;
								}
								if (orginalData.TAR_QTY == orginalData.A_TAR_QTY)
									orginalData.STATUS = "2";
								orginalData.A_SRC_QTY -= newTarQty;
								orginalData.SRC_QTY -= newTarQty;
								orginalData.STICKER_PALLET_NO = stickerPalletNo;

								if (orginalData.TAR_QTY == 0)
									removeF151002List.Add(orginalData);
								else
									updateF151002List.Add(orginalData);
								//增加LO上架的調撥單明細
								loF151002s.Add(new { F151002 = orginalData, AddTarQty = (int)(orginalData.A_TAR_QTY - loOrgiATarQty) });

								loOrgiATarQty = 0; //記錄原上架數量，For LO
								var findNewItem = SetF151002(ref addF151002List, ref updateF151002List, ref newDatas, orginalData,
										isLocChange ? tarLocCode : orginalData.TAR_LOC_CODE,
										isValidDateChange ? dtmNewValidDate : (orginalData.SRC_VALID_DATE ?? orginalData.VALID_DATE),
										isMakeNoChange ? dtmNewMakeNo : (orginalData.SRC_MAKE_NO ?? orginalData.MAKE_NO),
										ref seq, ref loOrgiATarQty, newTarQty, stickerPalletNo);
								//增加LO上架的調撥單明細
								loF151002s.Add(new { F151002 = findNewItem, AddTarQty = (int)(findNewItem.A_TAR_QTY - loOrgiATarQty) });

								var orginalSerialNo = (string.IsNullOrEmpty(findNewItem.SERIAL_NO)) ? "0" : findNewItem.SERIAL_NO;
								SetF1913(ref addF1913List, ref updateF1913List, findNewItem,
										isValidDateChange ? dtmNewValidDate : (orginalData.SRC_VALID_DATE ?? orginalData.VALID_DATE),
										isMakeNoChange ? dtmNewMakeNo : (orginalData.SRC_MAKE_NO ?? orginalData.MAKE_NO),
										orginalSerialNo, newTarQty, tarDcCode);

								if (tarQty <= 0)
									break;
							}
							#endregion
						}
					}
				}
				//最後一次刪除 更新 新增
				foreach (var f151002 in removeF151002List)
					f151002Repo.DeleteData(f151002.DC_CODE, f151002.GUP_CODE, f151002.CUST_CODE, f151002.ALLOCATION_NO,
							  f151002.ALLOCATION_SEQ);

                foreach (var f151002 in addF151002List)
                    f151002Repo.Add(f151002);

                foreach (var f151002 in updateF151002List)
                    f151002Repo.UpdateData(f151002.DC_CODE, f151002.GUP_CODE, f151002.CUST_CODE, f151002.ALLOCATION_NO, f151002.ALLOCATION_SEQ, f151002.SRC_QTY, f151002.A_SRC_QTY, f151002.TAR_QTY, f151002.A_TAR_QTY, f151002.STATUS, Current.Staff, Current.StaffName, false, stickerPalletNo);
                
                foreach (var f1913 in updateF1913List)
					f1913Repo.UpdateQty(f1913.DC_CODE, f1913.GUP_CODE, f1913.CUST_CODE, f1913.ITEM_CODE, f1913.LOC_CODE, f1913.VALID_DATE, f1913.ENTER_DATE, f1913.VNR_CODE, f1913.SERIAL_NO, f1913.QTY, f1913.BOX_CTRL_NO, f1913.PALLET_CTRL_NO, f1913.MAKE_NO);
				foreach (var f1913 in addF1913List)
					f1913Repo.Add(f1913);

                // 純上架不需要 更新 新增F1511
                bool isPureTar = false;
                var f151001 = f151001Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo);
                if (f151001 != null && !string.IsNullOrWhiteSpace(f151001.TAR_WAREHOUSE_ID) && string.IsNullOrWhiteSpace(f151001.SRC_WAREHOUSE_ID))
                    isPureTar = true;

                if (!isPureTar)
                {
                    //最後一次刪除 更新 新增F1511
                    var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
				    foreach (var f151002 in removeF151002List)
				    	f1511Repo.Delete(x => x.DC_CODE == f151002.DC_CODE && x.GUP_CODE == f151002.GUP_CODE && x.CUST_CODE == f151002.CUST_CODE && x.ORDER_NO == f151002.ALLOCATION_NO && x.ORDER_SEQ == f151002.ALLOCATION_SEQ.ToString());
                    
                    if (updateF151002List.Any())
				    {
				    	var udpF1511List = f1511Repo.AsForUpdate().InWithTrueAndCondition("ORDER_SEQ", updateF151002List.Select(x => x.ALLOCATION_SEQ).ToList(), x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ORDER_NO == allocationNo);
				    	foreach (var f151002 in updateF151002List)
				    	{
				    		var f1511 = udpF1511List.Where(x => x.DC_CODE == f151002.DC_CODE && x.GUP_CODE == f151002.GUP_CODE && x.CUST_CODE == f151002.CUST_CODE && x.ORDER_NO == f151002.ALLOCATION_NO && x.ORDER_SEQ == f151002.ALLOCATION_SEQ.ToString()).FirstOrDefault();
                            if (f1511 != null)
                            {
                                f1511.B_PICK_QTY = (int)f151002.SRC_QTY;
                                f1511Repo.Update(f1511);
                            }
				    	}
				    }
				    foreach (var f151002 in addF151002List)
				    {
				    	f1511Repo.Add(new F1511
				    	{
				    		DC_CODE = f151002.DC_CODE,
				    		GUP_CODE = f151002.GUP_CODE,
				    		CUST_CODE = f151002.CUST_CODE,
				    		ORDER_NO = f151002.ALLOCATION_NO,
				    		ORDER_SEQ = f151002.ALLOCATION_SEQ.ToString(),
				    		STATUS = "2",
				    		B_PICK_QTY = (int)f151002.SRC_QTY,
				    		A_PICK_QTY = (int)f151002.SRC_QTY,
				    		ITEM_CODE = f151002.ITEM_CODE,
				    		VALID_DATE = f151002.VALID_DATE,
				    		ENTER_DATE = f151002.ENTER_DATE,
				    		SERIAL_NO = f151002.SERIAL_NO,
				    		LOC_CODE = f151002.SRC_LOC_CODE,
				    		MAKE_NO = f151002.MAKE_NO,
				    		BOX_CTRL_NO = f151002.BOX_CTRL_NO,
				    		PALLET_CTRL_NO = f151002.PALLET_CTRL_NO
				    	});
				    }
                }

                // 新增進倉上架完成歷程表
                var f020202s = warehouseInService.CreateF020202sForTar(dcCode, gupCode, custCode, allocationNo, updateF151002List, addF151002List, removeF151002List);

                // 更新進倉驗收結果上架表
                warehouseInService.UpdateF010204s(dcCode, gupCode, custCode, allocationNo, f020202s);
                
            }
            return result ?? (result = new ExecuteResult() { IsSuccessed = true });
		}

		private F151002 SetF151002(ref List<F151002> addF151002List, ref List<F151002> updateF151002List,
			ref List<F151002> newDatas, F151002 orginalData, string tarLocCode, DateTime dtmNewValidDate, string dtmNewMakeNo, ref int seq, ref long loOrgiATarQty, long newTarQty, string stickerPalletNo)
		{
            var findAddOrUpdateItem = (string.IsNullOrEmpty(orginalData.SERIAL_NO))
                ? addF151002List.FirstOrDefault(
                        o =>
                                o.DC_CODE == orginalData.DC_CODE && o.GUP_CODE == orginalData.GUP_CODE &&
                                o.CUST_CODE == orginalData.CUST_CODE && o.ITEM_CODE == orginalData.ITEM_CODE &&
                                o.VNR_CODE == orginalData.VNR_CODE && string.IsNullOrEmpty(o.SERIAL_NO) &&
                                o.ENTER_DATE == orginalData.ENTER_DATE &&
                                o.TAR_LOC_CODE == tarLocCode &&
                                (o.TAR_VALID_DATE.HasValue ? o.TAR_VALID_DATE : (o.SRC_VALID_DATE.HasValue ? o.SRC_VALID_DATE : o.VALID_DATE)) == dtmNewValidDate &&
                                o.PALLET_CTRL_NO == orginalData.PALLET_CTRL_NO &&
                                o.BOX_CTRL_NO == orginalData.BOX_CTRL_NO &&
                                (string.IsNullOrEmpty(o.TAR_MAKE_NO) ? (string.IsNullOrEmpty(o.SRC_MAKE_NO) ? o.MAKE_NO : o.SRC_MAKE_NO) : o.TAR_MAKE_NO) == dtmNewMakeNo &&
                                o.SRC_MAKE_NO == orginalData.SRC_MAKE_NO &&
                                o.SRC_VALID_DATE == orginalData.SRC_VALID_DATE)
                : addF151002List.FirstOrDefault(
                        o =>
                                o.DC_CODE == orginalData.DC_CODE && o.GUP_CODE == orginalData.GUP_CODE &&
                                o.CUST_CODE == orginalData.CUST_CODE && o.ITEM_CODE == orginalData.ITEM_CODE &&
                                o.VNR_CODE == orginalData.VNR_CODE && o.SERIAL_NO == orginalData.SERIAL_NO &&
                                o.ENTER_DATE == orginalData.ENTER_DATE &&
                                o.TAR_LOC_CODE == tarLocCode &&
                                (o.TAR_VALID_DATE.HasValue ? o.TAR_VALID_DATE : (o.SRC_VALID_DATE.HasValue ? o.SRC_VALID_DATE : o.VALID_DATE)) == dtmNewValidDate &&
                                o.PALLET_CTRL_NO == orginalData.PALLET_CTRL_NO &&
                                o.BOX_CTRL_NO == orginalData.BOX_CTRL_NO &&
                                (string.IsNullOrEmpty(o.TAR_MAKE_NO) ? (string.IsNullOrEmpty(o.SRC_MAKE_NO) ? o.MAKE_NO : o.SRC_MAKE_NO) : o.TAR_MAKE_NO) == dtmNewMakeNo &&
                                o.SRC_MAKE_NO == orginalData.SRC_MAKE_NO &&
                                o.SRC_VALID_DATE == orginalData.SRC_VALID_DATE);
            findAddOrUpdateItem = findAddOrUpdateItem ?? ((string.IsNullOrEmpty(orginalData.SERIAL_NO))
                ? updateF151002List.FirstOrDefault(
                    o =>
                        o.DC_CODE == orginalData.DC_CODE && o.GUP_CODE == orginalData.GUP_CODE &&
                        o.CUST_CODE == orginalData.CUST_CODE && o.ITEM_CODE == orginalData.ITEM_CODE &&
                        o.VNR_CODE == orginalData.VNR_CODE && string.IsNullOrEmpty(o.SERIAL_NO) &&
                        o.ENTER_DATE == orginalData.ENTER_DATE &&
                        o.TAR_LOC_CODE == tarLocCode &&
                        (o.TAR_VALID_DATE.HasValue ? o.TAR_VALID_DATE : (o.SRC_VALID_DATE.HasValue ? o.SRC_VALID_DATE : o.VALID_DATE)) == dtmNewValidDate &&
                        o.PALLET_CTRL_NO == orginalData.PALLET_CTRL_NO &&
                        o.BOX_CTRL_NO == orginalData.BOX_CTRL_NO &&
                        (string.IsNullOrEmpty(o.TAR_MAKE_NO) ? (string.IsNullOrEmpty(o.SRC_MAKE_NO) ? o.MAKE_NO : o.SRC_MAKE_NO) : o.TAR_MAKE_NO) == dtmNewMakeNo &&
                        o.SRC_MAKE_NO == orginalData.SRC_MAKE_NO &&
                        o.SRC_VALID_DATE == orginalData.SRC_VALID_DATE)
                : updateF151002List.FirstOrDefault(
                    o =>
                        o.DC_CODE == orginalData.DC_CODE && o.GUP_CODE == orginalData.GUP_CODE &&
                        o.CUST_CODE == orginalData.CUST_CODE && o.ITEM_CODE == orginalData.ITEM_CODE &&
                        o.VNR_CODE == orginalData.VNR_CODE && o.SERIAL_NO == orginalData.SERIAL_NO &&
                        o.ENTER_DATE == orginalData.ENTER_DATE &&
                        o.TAR_LOC_CODE == tarLocCode &&
                        (o.TAR_VALID_DATE.HasValue ? o.TAR_VALID_DATE : (o.SRC_VALID_DATE.HasValue ? o.SRC_VALID_DATE : o.VALID_DATE)) == dtmNewValidDate &&
                        o.PALLET_CTRL_NO == orginalData.PALLET_CTRL_NO &&
                        o.BOX_CTRL_NO == orginalData.BOX_CTRL_NO &&
                        (string.IsNullOrEmpty(o.TAR_MAKE_NO) ? (string.IsNullOrEmpty(o.SRC_MAKE_NO) ? o.MAKE_NO : o.SRC_MAKE_NO) : o.TAR_MAKE_NO) == dtmNewMakeNo &&
                        o.SRC_MAKE_NO == orginalData.SRC_MAKE_NO &&
                        o.SRC_VALID_DATE == orginalData.SRC_VALID_DATE));
            var findNewItem = findAddOrUpdateItem ?? ((string.IsNullOrEmpty(orginalData.SERIAL_NO))
                ? newDatas.FirstOrDefault(
                    o =>
                        o.DC_CODE == orginalData.DC_CODE && o.GUP_CODE == orginalData.GUP_CODE &&
                        o.CUST_CODE == orginalData.CUST_CODE && o.ITEM_CODE == orginalData.ITEM_CODE &&
                        o.VNR_CODE == orginalData.VNR_CODE && string.IsNullOrEmpty(o.SERIAL_NO) &&
                        o.ENTER_DATE == orginalData.ENTER_DATE &&
                        o.TAR_LOC_CODE == tarLocCode &&
                        (o.TAR_VALID_DATE.HasValue ? o.TAR_VALID_DATE : (o.SRC_VALID_DATE.HasValue ? o.SRC_VALID_DATE : o.VALID_DATE)) == dtmNewValidDate &&
                        o.PALLET_CTRL_NO == orginalData.PALLET_CTRL_NO &&
                        o.BOX_CTRL_NO == orginalData.BOX_CTRL_NO &&
                        (string.IsNullOrEmpty(o.TAR_MAKE_NO) ? (string.IsNullOrEmpty(o.SRC_MAKE_NO) ? o.MAKE_NO : o.SRC_MAKE_NO) : o.TAR_MAKE_NO) == dtmNewMakeNo &&
                        o.SRC_MAKE_NO == orginalData.SRC_MAKE_NO &&
                        o.SRC_VALID_DATE == orginalData.SRC_VALID_DATE)
                : newDatas.FirstOrDefault(
                    o =>
                        o.DC_CODE == orginalData.DC_CODE && o.GUP_CODE == orginalData.GUP_CODE &&
                        o.CUST_CODE == orginalData.CUST_CODE && o.ITEM_CODE == orginalData.ITEM_CODE &&
                        o.VNR_CODE == orginalData.VNR_CODE && o.SERIAL_NO == orginalData.SERIAL_NO &&
                        o.ENTER_DATE == orginalData.ENTER_DATE &&
                        o.TAR_LOC_CODE == tarLocCode &&
                        (o.TAR_VALID_DATE.HasValue ? o.TAR_VALID_DATE : (o.SRC_VALID_DATE.HasValue ? o.SRC_VALID_DATE : o.VALID_DATE)) == dtmNewValidDate &&
                        o.PALLET_CTRL_NO == orginalData.PALLET_CTRL_NO &&
                        o.BOX_CTRL_NO == orginalData.BOX_CTRL_NO &&
                        (string.IsNullOrEmpty(o.TAR_MAKE_NO) ? (string.IsNullOrEmpty(o.SRC_MAKE_NO) ? o.MAKE_NO : o.SRC_MAKE_NO) : o.TAR_MAKE_NO) == dtmNewMakeNo &&
                        o.SRC_MAKE_NO == orginalData.SRC_MAKE_NO &&
                        o.SRC_VALID_DATE == orginalData.SRC_VALID_DATE));
			loOrgiATarQty = 0; //記錄原上架數量，For LO
			if (findNewItem == null)
			{
				findNewItem = CreateF151002(orginalData, seq);
				findNewItem.SRC_QTY = newTarQty;
				//因為要移除舊的要把原本的SRC_QTY扣除本次減的差額加到目前新的SRC_QTY
				if (orginalData.TAR_QTY == 0 && orginalData.SRC_QTY > 0)
					findNewItem.SRC_QTY += orginalData.SRC_QTY - newTarQty;
				findNewItem.A_SRC_QTY = newTarQty;
				//因為要移除舊的要把原本的A_SRC_QTY扣除本次減的差額加到目前新的A_SRC_QTY
				if (orginalData.TAR_QTY == 0 && orginalData.A_SRC_QTY > 0)
					findNewItem.A_SRC_QTY += orginalData.A_SRC_QTY - newTarQty;
				findNewItem.TAR_QTY = newTarQty;
				findNewItem.A_TAR_QTY = newTarQty;
				findNewItem.STATUS = "2";
				findNewItem.TAR_LOC_CODE = tarLocCode;
				//先判斷有沒有修改下架效期，有的話拿下架效期來跟上架效期比，沒有的話拿原始效期跟上架效期比，來判斷是否有修改效期
				if (findNewItem.SRC_VALID_DATE != null ? findNewItem.SRC_VALID_DATE != dtmNewValidDate : findNewItem.VALID_DATE != dtmNewValidDate)
					findNewItem.TAR_VALID_DATE = dtmNewValidDate;
				else
					findNewItem.TAR_VALID_DATE = null;
				//先判斷有沒有修改下架批號，有的話拿下架批號來跟上架批號比，沒有的話拿原始批號跟上架批號比，來判斷是否有修改批號
				if (findNewItem.SRC_MAKE_NO != null ? findNewItem.SRC_MAKE_NO != dtmNewMakeNo : findNewItem.MAKE_NO != dtmNewMakeNo)
					findNewItem.TAR_MAKE_NO = dtmNewMakeNo;
				else
					findNewItem.TAR_MAKE_NO = null;
				findNewItem.STICKER_PALLET_NO = stickerPalletNo;
                findNewItem.SRC_MAKE_NO = orginalData.SRC_MAKE_NO;
                findNewItem.SRC_VALID_DATE = orginalData.SRC_VALID_DATE;
				addF151002List.Add(findNewItem);
				seq++;
			}
			else
			{
				loOrgiATarQty = findNewItem.A_TAR_QTY; //記錄原上架數量，For LO
				findNewItem.SRC_QTY += newTarQty;
				//因為要移除舊的要把原本的SRC_QTY扣除本次減的差額加到目前新的SRC_QTY
				if (orginalData.TAR_QTY == 0 && orginalData.SRC_QTY > 0)
					findNewItem.SRC_QTY += orginalData.SRC_QTY - newTarQty;
				findNewItem.A_SRC_QTY += newTarQty;
				if (orginalData.TAR_QTY == 0 && orginalData.A_SRC_QTY > 0)
					findNewItem.A_SRC_QTY += orginalData.A_SRC_QTY - newTarQty;
				findNewItem.TAR_QTY += newTarQty;
				findNewItem.A_TAR_QTY += newTarQty;
				findNewItem.STATUS = "2";
				findNewItem.STICKER_PALLET_NO = stickerPalletNo;
				if (findAddOrUpdateItem == null)
					updateF151002List.Add(findNewItem);
			}
			return findNewItem;
		}

		private void SetF1913(ref List<F1913> addF1913List, ref List<F1913> updateF1913List,
			 F151002 orginalData, DateTime validDate, string makeNo, string orginalSerialNo, long newTarQty, string tarDcCode)
		{

			var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			var addOrUpdateF1913Item =
									addF1913List.FirstOrDefault(
										o =>
											o.DC_CODE == tarDcCode && o.GUP_CODE == orginalData.GUP_CODE &&
											o.CUST_CODE == orginalData.CUST_CODE && o.ITEM_CODE == orginalData.ITEM_CODE &&
											o.LOC_CODE == orginalData.TAR_LOC_CODE && o.VALID_DATE == validDate &&
											o.ENTER_DATE == orginalData.ENTER_DATE && o.VNR_CODE == orginalData.VNR_CODE &&
											o.SERIAL_NO == orginalSerialNo && o.BOX_CTRL_NO == orginalData.BOX_CTRL_NO &&
																						o.PALLET_CTRL_NO == orginalData.PALLET_CTRL_NO && o.MAKE_NO == makeNo);
			addOrUpdateF1913Item = addOrUpdateF1913Item ??
																									 updateF1913List.FirstOrDefault(
																											 o =>
																													 o.DC_CODE == tarDcCode && o.GUP_CODE == orginalData.GUP_CODE &&
																													 o.CUST_CODE == orginalData.CUST_CODE && o.ITEM_CODE == orginalData.ITEM_CODE &&
																													 o.LOC_CODE == orginalData.TAR_LOC_CODE && o.VALID_DATE == validDate &&
																													 o.ENTER_DATE == orginalData.ENTER_DATE && o.VNR_CODE == orginalData.VNR_CODE &&
																													 o.SERIAL_NO == orginalSerialNo && o.BOX_CTRL_NO == orginalData.BOX_CTRL_NO &&
																			o.PALLET_CTRL_NO == orginalData.PALLET_CTRL_NO && o.MAKE_NO == makeNo);
			var findF1913Item = addOrUpdateF1913Item ??
													f1913Repo.GetData(tarDcCode, orginalData.GUP_CODE, orginalData.CUST_CODE,
														orginalData.ITEM_CODE, orginalData.TAR_LOC_CODE, validDate,
														orginalData.ENTER_DATE, orginalSerialNo, orginalData.VNR_CODE, orginalData.BOX_CTRL_NO, orginalData.PALLET_CTRL_NO, makeNo);
			if (findF1913Item != null)
			{
				findF1913Item.QTY += newTarQty;
				if (addOrUpdateF1913Item == null)
					updateF1913List.Add(findF1913Item);
			}
			else
			{
				findF1913Item = new F1913
				{
					DC_CODE = tarDcCode,
					GUP_CODE = orginalData.GUP_CODE,
					CUST_CODE = orginalData.CUST_CODE,
					LOC_CODE = orginalData.TAR_LOC_CODE,
					ITEM_CODE = orginalData.ITEM_CODE,
					VALID_DATE = validDate,
					ENTER_DATE = orginalData.ENTER_DATE,
					VNR_CODE = orginalData.VNR_CODE,
					SERIAL_NO = orginalSerialNo,
					QTY = newTarQty,
					BOX_CTRL_NO = orginalData.BOX_CTRL_NO,
					PALLET_CTRL_NO = orginalData.PALLET_CTRL_NO,
					MAKE_NO = makeNo
				};
				addF1913List.Add(findF1913Item);
			}
		}


		public ExecuteResult RemoveTarLocItemCodeActualQty(string dcCode, string gupCode, string custCode, string allocationNo, string tarLocCode, string itemCode,
		 string removeValidDate, string removeMakeNo, string stickerPalletNo = "")
		{
            var warehouseInService = new WarehouseInService(_wmsTransaction);
            ExecuteResult result = null;
			//上架的調撥單明細，For LO
			var loF151002s = (from a in (new List<F151002>())
												select new { F151002 = new F151002(), AddTarQty = 0 }).ToList();

			var dtmremoveValidDate = DateTime.Parse(removeValidDate); //移除的生效日
			var dtmremoveMakeNo = removeMakeNo; //移除的批號
			var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			var datas = f151002Repo.GetDatas(dcCode, gupCode, custCode, allocationNo).ToList();
			var seq = 1;
			if (datas.Any())
				seq += datas.Max(o => o.ALLOCATION_SEQ);
			datas = datas.Where(o => o.ITEM_CODE == itemCode).ToList();
			var orginalDatas = datas.Where(o => o.SUG_LOC_CODE == o.TAR_LOC_CODE && !o.TAR_VALID_DATE.HasValue && string.IsNullOrWhiteSpace(o.TAR_MAKE_NO)).ToList();
            var clearDatas = datas.Where(o =>
            o.TAR_LOC_CODE == tarLocCode && o.A_TAR_QTY > 0 &&
            ((o.TAR_VALID_DATE ?? o.SRC_VALID_DATE ?? o.VALID_DATE) == dtmremoveValidDate) &&
             ((o.TAR_MAKE_NO ?? o.SRC_MAKE_NO ?? o.MAKE_NO) == dtmremoveMakeNo)
            ).ToList();
			if (!string.IsNullOrWhiteSpace(stickerPalletNo))
			{
				orginalDatas = orginalDatas.Where(x => x.STICKER_PALLET_NO == stickerPalletNo).ToList();
				clearDatas = clearDatas.Where(x => x.STICKER_PALLET_NO == stickerPalletNo).ToList();
			}
			var updateF151002List = new List<F151002>();
			var addF151002List = new List<F151002>();
			var removeF151002List = new List<F151002>();
			var updateF1913List = new List<F1913>();
			var removeF1913List = new List<F1913>();
            List<string> errMessage = new List<string>();

            foreach (var clearData in clearDatas)
			{
				var loOrgiATarQty = clearData.A_TAR_QTY; //記錄原上架數量，For LO

				bool isLocChange = clearData.SUG_LOC_CODE != clearData.TAR_LOC_CODE;
				bool isValidDateChange = clearData.TAR_VALID_DATE.HasValue;
				bool isMakeNoChange = !string.IsNullOrWhiteSpace(clearData.TAR_MAKE_NO);

				if (!isLocChange && !isValidDateChange && !isMakeNoChange)
				{
                    #region 未修改儲位、效期、批號的部分

                    CheckF151003Data(clearData.DC_CODE, clearData.GUP_CODE, clearData.CUST_CODE, clearData.ALLOCATION_NO, clearData.ALLOCATION_SEQ,
                        clearData.ITEM_CODE, errMessage);

					SetF1913ByRemove(ref updateF1913List, ref removeF1913List, clearData,
						 clearData.SRC_VALID_DATE ?? clearData.VALID_DATE,
						 clearData.SRC_MAKE_NO ?? clearData.MAKE_NO,
						 f1913Repo);

                    clearData.A_TAR_QTY = 0;
					clearData.STATUS = "1";
					updateF151002List.Add(clearData);
					//增加LO上架的調撥單明細
					loF151002s.Add(new { F151002 = clearData, AddTarQty = (int)(clearData.A_TAR_QTY - loOrgiATarQty) });
                    #endregion
                }
				else
				{
					#region 儲位、效期、批號其中一個有修改

					SetF1913ByRemove(ref updateF1913List, ref removeF1913List, clearData,
						isValidDateChange ? clearData.TAR_VALID_DATE : (clearData.SRC_VALID_DATE ?? clearData.VALID_DATE),
						isMakeNoChange ? clearData.TAR_MAKE_NO : (clearData.SRC_MAKE_NO ?? clearData.MAKE_NO),
						f1913Repo);

					long loOrgiATarQty2 = 0; //記錄原上架數量，For LO
                    var orginalItem = SetF151002ByRemove(ref addF151002List, ref updateF151002List, ref orginalDatas, clearData, ref seq, ref loOrgiATarQty2, errMessage, stickerPalletNo);
                    
                    if (orginalItem != null)
                    {
                        removeF151002List.Add(clearData);

                        //增加LO上架的調撥單明細
                        loF151002s.Add(new { F151002 = orginalItem, AddTarQty = (int)(orginalItem.A_TAR_QTY - loOrgiATarQty2) });
                        //增加LO上架的調撥單明細
                        loF151002s.Add(new { F151002 = clearData, AddTarQty = (int)-loOrgiATarQty });
                    }
					#endregion
				}
                if (errMessage.Any())
                    return new ExecuteResult(false, errMessage.FirstOrDefault());
            }

			//最後一次刪除 更新 新增
			foreach (var f151002 in removeF151002List)
				f151002Repo.DeleteData(f151002.DC_CODE, f151002.GUP_CODE, f151002.CUST_CODE, f151002.ALLOCATION_NO,
						f151002.ALLOCATION_SEQ);

			foreach (var f151002 in updateF151002List)
				f151002Repo.UpdateData(f151002.DC_CODE, f151002.GUP_CODE, f151002.CUST_CODE, f151002.ALLOCATION_NO, f151002.ALLOCATION_SEQ, f151002.SRC_QTY, f151002.A_SRC_QTY, f151002.TAR_QTY, f151002.A_TAR_QTY, f151002.STATUS, Current.Staff, Current.StaffName, false, stickerPalletNo);

			foreach (var f151002 in addF151002List)
				f151002Repo.Add(f151002);

			foreach (var f1913 in removeF1913List)
				f1913Repo.DeleteDataByKey(f1913.DC_CODE, f1913.GUP_CODE, f1913.CUST_CODE, f1913.ITEM_CODE, f1913.LOC_CODE, f1913.VALID_DATE, f1913.ENTER_DATE, f1913.VNR_CODE, f1913.SERIAL_NO, f1913.BOX_CTRL_NO, f1913.PALLET_CTRL_NO, f1913.MAKE_NO);

			foreach (var f1913 in updateF1913List)
				f1913Repo.UpdateQty(f1913.DC_CODE, f1913.GUP_CODE, f1913.CUST_CODE, f1913.ITEM_CODE, f1913.LOC_CODE, f1913.VALID_DATE, f1913.ENTER_DATE, f1913.VNR_CODE, f1913.SERIAL_NO, f1913.QTY, f1913.BOX_CTRL_NO, f1913.PALLET_CTRL_NO, f1913.MAKE_NO);

			//最後一次刪除 更新 新增F1511
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			foreach (var f151002 in removeF151002List)
				f1511Repo.Delete(x => x.DC_CODE == f151002.DC_CODE && x.GUP_CODE == f151002.GUP_CODE && x.CUST_CODE == f151002.CUST_CODE && x.ORDER_NO == f151002.ALLOCATION_NO && x.ORDER_SEQ == f151002.ALLOCATION_SEQ.ToString());
			if (updateF151002List.Any())
			{
				var udpF1511List = f1511Repo.AsForUpdate().InWithTrueAndCondition("ORDER_SEQ", updateF151002List.Select(x => x.ALLOCATION_SEQ).ToList(), x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ORDER_NO == allocationNo);
				foreach (var f151002 in updateF151002List)
				{
					var f1511 = udpF1511List.First(x => x.DC_CODE == f151002.DC_CODE && x.GUP_CODE == f151002.GUP_CODE && x.CUST_CODE == f151002.CUST_CODE && x.ORDER_NO == f151002.ALLOCATION_NO && x.ORDER_SEQ == f151002.ALLOCATION_SEQ.ToString());
					f1511.B_PICK_QTY = (int)f151002.SRC_QTY;
					f1511Repo.Update(f1511);
				}
			}
			foreach (var f151002 in addF151002List)
			{
				f1511Repo.Add(new F1511
				{
					DC_CODE = f151002.DC_CODE,
					GUP_CODE = f151002.GUP_CODE,
					CUST_CODE = f151002.CUST_CODE,
					ORDER_NO = f151002.ALLOCATION_NO,
					ORDER_SEQ = f151002.ALLOCATION_SEQ.ToString(),
					STATUS = "1",
					B_PICK_QTY = (int)f151002.SRC_QTY,
					A_PICK_QTY = (int)f151002.SRC_QTY,
					ITEM_CODE = f151002.ITEM_CODE,
					VALID_DATE = f151002.VALID_DATE,
					ENTER_DATE = f151002.ENTER_DATE,
					SERIAL_NO = f151002.SERIAL_NO,
					LOC_CODE = f151002.SRC_LOC_CODE,
					MAKE_NO = f151002.MAKE_NO,
					BOX_CTRL_NO = f151002.BOX_CTRL_NO,
					PALLET_CTRL_NO = f151002.PALLET_CTRL_NO
				});
			}

            // 新增進倉上架完成歷程表
            var f020202s = warehouseInService.CreateF020202sForRemove(dcCode, gupCode, custCode, allocationNo, updateF151002List, addF151002List, removeF151002List);

            // 更新進倉驗收結果上架表
            warehouseInService.UpdateF010204s(dcCode, gupCode, custCode, allocationNo, f020202s);
            
            if (result == null)
				result = new ExecuteResult { IsSuccessed = true };

			return result;
		}

		private void SetF1913ByRemove(ref List<F1913> updateF1913List, ref List<F1913> removeF1913List, F151002 clearData, DateTime? validDate, string makeNo, F1913Repository f1913Repo)
		{
			var findUpdatef1913Item = updateF1913List.FirstOrDefault(o => o.DC_CODE == clearData.DC_CODE && o.GUP_CODE == clearData.GUP_CODE && o.CUST_CODE == clearData.CUST_CODE && o.ITEM_CODE == clearData.ITEM_CODE && o.LOC_CODE == clearData.TAR_LOC_CODE &&
								 o.VALID_DATE == validDate && //這邊有差異
								 o.ENTER_DATE == clearData.ENTER_DATE && o.SERIAL_NO == ((string.IsNullOrEmpty(clearData.SERIAL_NO)) ? "0" : clearData.SERIAL_NO) && o.VNR_CODE == clearData.VNR_CODE &&
								 o.PALLET_CTRL_NO == clearData.PALLET_CTRL_NO && o.BOX_CTRL_NO == clearData.BOX_CTRL_NO &&
								 o.MAKE_NO == makeNo); //ToDo 修改此處
			var f1913Item = findUpdatef1913Item ?? f1913Repo.GetData(clearData.DC_CODE, clearData.GUP_CODE, clearData.CUST_CODE, clearData.ITEM_CODE, clearData.TAR_LOC_CODE,
					validDate.Value, //這邊有差異
					clearData.ENTER_DATE, (string.IsNullOrEmpty(clearData.SERIAL_NO)) ? "0" : clearData.SERIAL_NO, clearData.VNR_CODE, clearData.BOX_CTRL_NO, clearData.PALLET_CTRL_NO,
					makeNo);//ToDo 修改此處
			f1913Item.QTY -= clearData.A_TAR_QTY;
			if (f1913Item.QTY <= 0)
			{
				removeF1913List.Add(f1913Item);
				if (findUpdatef1913Item != null)
					updateF1913List.Remove(findUpdatef1913Item);
			}
			else
			{
				if (findUpdatef1913Item == null)
					updateF1913List.Add(f1913Item);
			}
		}

		private F151002 SetF151002ByRemove(ref List<F151002> addF151002List, ref List<F151002> updateF151002List, ref List<F151002> orginalDatas, F151002 clearData, ref int seq, ref long loOrgiATarQty2,  List<string> errMessage, string stickerPalletNo = "")
		{
			//取得原始資料
			var addOrUpdateItem = (string.IsNullOrEmpty(clearData.SERIAL_NO))
					? addF151002List.FirstOrDefault(
							o => o.DC_CODE == clearData.DC_CODE && o.GUP_CODE == clearData.GUP_CODE && o.CUST_CODE == clearData.CUST_CODE && o.ITEM_CODE == clearData.ITEM_CODE &&
							o.SUG_LOC_CODE == clearData.SUG_LOC_CODE && o.SUG_LOC_CODE == o.TAR_LOC_CODE &&
							o.VALID_DATE == clearData.VALID_DATE && o.SRC_VALID_DATE == clearData.SRC_VALID_DATE && !o.TAR_VALID_DATE.HasValue &&
							o.ENTER_DATE == clearData.ENTER_DATE && o.VNR_CODE == clearData.VNR_CODE &&
							string.IsNullOrEmpty(o.SERIAL_NO) && o.PALLET_CTRL_NO == clearData.PALLET_CTRL_NO && o.BOX_CTRL_NO == clearData.BOX_CTRL_NO &&
							o.MAKE_NO == clearData.MAKE_NO && o.SRC_MAKE_NO == clearData.SRC_MAKE_NO && string.IsNullOrWhiteSpace(o.TAR_MAKE_NO))
					: addF151002List.FirstOrDefault(
							o => o.DC_CODE == clearData.DC_CODE && o.GUP_CODE == clearData.GUP_CODE && o.CUST_CODE == clearData.CUST_CODE && o.ITEM_CODE == clearData.ITEM_CODE &&
							o.SUG_LOC_CODE == clearData.SUG_LOC_CODE && o.SUG_LOC_CODE == o.TAR_LOC_CODE &&
							o.VALID_DATE == clearData.VALID_DATE && o.SRC_VALID_DATE == clearData.SRC_VALID_DATE && !o.TAR_VALID_DATE.HasValue &&
							o.ENTER_DATE == clearData.ENTER_DATE && o.VNR_CODE == clearData.VNR_CODE &&
							o.SERIAL_NO == clearData.SERIAL_NO && o.PALLET_CTRL_NO == clearData.PALLET_CTRL_NO && o.BOX_CTRL_NO == clearData.BOX_CTRL_NO &&
							o.MAKE_NO == clearData.MAKE_NO && o.SRC_MAKE_NO == clearData.SRC_MAKE_NO && string.IsNullOrWhiteSpace(o.TAR_MAKE_NO));

			addOrUpdateItem = addOrUpdateItem ?? ((string.IsNullOrEmpty(clearData.SERIAL_NO))
					? updateF151002List.FirstOrDefault(
							o => o.DC_CODE == clearData.DC_CODE && o.GUP_CODE == clearData.GUP_CODE && o.CUST_CODE == clearData.CUST_CODE && o.ITEM_CODE == clearData.ITEM_CODE &&
							o.SUG_LOC_CODE == clearData.SUG_LOC_CODE && o.SUG_LOC_CODE == o.TAR_LOC_CODE &&
							o.VALID_DATE == clearData.VALID_DATE && o.SRC_VALID_DATE == clearData.SRC_VALID_DATE && !o.TAR_VALID_DATE.HasValue &&
							o.ENTER_DATE == clearData.ENTER_DATE && o.VNR_CODE == clearData.VNR_CODE &&
							string.IsNullOrEmpty(o.SERIAL_NO) && o.PALLET_CTRL_NO == clearData.PALLET_CTRL_NO && o.BOX_CTRL_NO == clearData.BOX_CTRL_NO &&
							o.MAKE_NO == clearData.MAKE_NO && o.SRC_MAKE_NO == clearData.SRC_MAKE_NO && string.IsNullOrWhiteSpace(o.TAR_MAKE_NO))
					: updateF151002List.FirstOrDefault(
							o => o.DC_CODE == clearData.DC_CODE && o.GUP_CODE == clearData.GUP_CODE && o.CUST_CODE == clearData.CUST_CODE && o.ITEM_CODE == clearData.ITEM_CODE &&
							o.SUG_LOC_CODE == clearData.SUG_LOC_CODE && o.SUG_LOC_CODE == o.TAR_LOC_CODE &&
							o.VALID_DATE == clearData.VALID_DATE && o.SRC_VALID_DATE == clearData.SRC_VALID_DATE && !o.TAR_VALID_DATE.HasValue &&
							o.ENTER_DATE == clearData.ENTER_DATE && o.VNR_CODE == clearData.VNR_CODE &&
							o.SERIAL_NO == clearData.SERIAL_NO && o.PALLET_CTRL_NO == clearData.PALLET_CTRL_NO && o.BOX_CTRL_NO == clearData.BOX_CTRL_NO &&
							o.MAKE_NO == clearData.MAKE_NO && o.SRC_MAKE_NO == clearData.SRC_MAKE_NO && string.IsNullOrWhiteSpace(o.TAR_MAKE_NO)));

			var orginalItem = addOrUpdateItem ?? ((string.IsNullOrEmpty(clearData.SERIAL_NO))
									? orginalDatas.FirstOrDefault(
							o => o.DC_CODE == clearData.DC_CODE && o.GUP_CODE == clearData.GUP_CODE && o.CUST_CODE == clearData.CUST_CODE && o.ITEM_CODE == clearData.ITEM_CODE &&
							o.SUG_LOC_CODE == clearData.SUG_LOC_CODE && o.SUG_LOC_CODE == o.TAR_LOC_CODE &&
							o.VALID_DATE == clearData.VALID_DATE && o.SRC_VALID_DATE == clearData.SRC_VALID_DATE && !o.TAR_VALID_DATE.HasValue &&
							o.ENTER_DATE == clearData.ENTER_DATE && o.VNR_CODE == clearData.VNR_CODE &&
							string.IsNullOrEmpty(o.SERIAL_NO) && o.PALLET_CTRL_NO == clearData.PALLET_CTRL_NO && o.BOX_CTRL_NO == clearData.BOX_CTRL_NO &&
							o.MAKE_NO == clearData.MAKE_NO && o.SRC_MAKE_NO == clearData.SRC_MAKE_NO && string.IsNullOrWhiteSpace(o.TAR_MAKE_NO))
					: orginalDatas.FirstOrDefault(
							o => o.DC_CODE == clearData.DC_CODE && o.GUP_CODE == clearData.GUP_CODE && o.CUST_CODE == clearData.CUST_CODE && o.ITEM_CODE == clearData.ITEM_CODE &&
							o.SUG_LOC_CODE == clearData.SUG_LOC_CODE && o.SUG_LOC_CODE == o.TAR_LOC_CODE &&
							o.VALID_DATE == clearData.VALID_DATE && o.SRC_VALID_DATE == clearData.SRC_VALID_DATE && !o.TAR_VALID_DATE.HasValue &&
							o.ENTER_DATE == clearData.ENTER_DATE && o.VNR_CODE == clearData.VNR_CODE &&
							o.SERIAL_NO == clearData.SERIAL_NO && o.PALLET_CTRL_NO == clearData.PALLET_CTRL_NO && o.BOX_CTRL_NO == clearData.BOX_CTRL_NO &&
							o.MAKE_NO == clearData.MAKE_NO && o.SRC_MAKE_NO == clearData.SRC_MAKE_NO && string.IsNullOrWhiteSpace(o.TAR_MAKE_NO)));

			if (orginalItem != null)
			{
                CheckF151003Data(orginalItem.DC_CODE, orginalItem.GUP_CODE, orginalItem.CUST_CODE,
                        orginalItem.ALLOCATION_NO, orginalItem.ALLOCATION_SEQ, orginalItem.ITEM_CODE, errMessage);

                if (errMessage.Any())
                    return null;

                loOrgiATarQty2 = orginalItem.A_SRC_QTY; //記錄原上架數量，For LO
				orginalItem.SRC_QTY += clearData.SRC_QTY;
				orginalItem.A_SRC_QTY += clearData.A_SRC_QTY;
				orginalItem.TAR_QTY += clearData.TAR_QTY;
				orginalItem.STATUS = "1";
				orginalItem.STICKER_PALLET_NO = stickerPalletNo;
				if (addOrUpdateItem == null)
					updateF151002List.Add(orginalItem);
			}
			else
			{
                orginalItem = CreateF151002(clearData, seq);
				orginalItem.SRC_QTY = clearData.SRC_QTY;
				orginalItem.A_SRC_QTY = clearData.A_SRC_QTY;
				orginalItem.TAR_QTY = clearData.TAR_QTY;
				orginalItem.TAR_VALID_DATE = null;
				orginalItem.TAR_LOC_CODE = orginalItem.SUG_LOC_CODE;
				orginalItem.STATUS = "1";
				orginalItem.STICKER_PALLET_NO = stickerPalletNo;
				orginalItem.TAR_MAKE_NO = null;
				addF151002List.Add(orginalItem);
				seq++;
			}
			return orginalItem;
		}

        /// <summary>
        /// 缺貨資料驗證
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="allocationNo"></param>
        /// <param name="allocationSeq"></param>
        /// <param name="itemCode"></param>
        /// <param name="errMessage"></param>
        private void CheckF151003Data(string dcCode, string gupCode, string custCode, string allocationNo, short allocationSeq, string itemCode, List<string> errMessage)
        {
            var f151003Repo = new F151003Repository(Schemas.CoreSchema);
            //檢查是否有缺貨資料
            var isOutOfStockData = f151003Repo.GetF151003sByLackType(
                   dcCode, gupCode, custCode, allocationNo, allocationSeq, itemCode, "1").ToList();

            if (isOutOfStockData.Any())
                errMessage.Add(Properties.Resources.P080302_IsOutOfStockData);
        }

        private F151002 CreateF151002(F151002 orginalItem, int seq)
		{
			var item = new F151002
			{
				DC_CODE = orginalItem.DC_CODE,
				GUP_CODE = orginalItem.GUP_CODE,
				CUST_CODE = orginalItem.CUST_CODE,
				ALLOCATION_NO = orginalItem.ALLOCATION_NO,
				ALLOCATION_SEQ = short.Parse(seq.ToString()),
                ORG_SEQ = orginalItem.ALLOCATION_SEQ,
                ALLOCATION_DATE = orginalItem.ALLOCATION_DATE,
				ITEM_CODE = orginalItem.ITEM_CODE,
				SRC_LOC_CODE = orginalItem.SRC_LOC_CODE,
				SUG_LOC_CODE = orginalItem.SUG_LOC_CODE,
				TAR_LOC_CODE = orginalItem.TAR_LOC_CODE,
				SERIAL_NO = orginalItem.SERIAL_NO,
				VALID_DATE = orginalItem.VALID_DATE,
				CHECK_SERIALNO = orginalItem.CHECK_SERIALNO,
				SRC_STAFF = orginalItem.SRC_STAFF,
				SRC_NAME = orginalItem.SRC_NAME,
				SRC_DATE = orginalItem.SRC_DATE,
				TAR_STAFF = Current.Staff,
				TAR_NAME = Current.StaffName,
				TAR_DATE = DateTime.Now,
				SRC_VALID_DATE = orginalItem.SRC_VALID_DATE,
				TAR_VALID_DATE = orginalItem.TAR_VALID_DATE,
				ENTER_DATE = orginalItem.ENTER_DATE,
				VNR_CODE = orginalItem.VNR_CODE,
				PALLET_CTRL_NO = orginalItem.PALLET_CTRL_NO,
				BOX_CTRL_NO = orginalItem.BOX_CTRL_NO,
				MAKE_NO = orginalItem.MAKE_NO,
				SRC_MAKE_NO = orginalItem.SRC_MAKE_NO,
				TAR_MAKE_NO = orginalItem.TAR_MAKE_NO
			};
			return item;
		}
		public ExecuteResult StartUpItemChangeStatus(string tarDcCode, string gupCode, string custCode, string allocationNo)
		{
			var f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
			var item =
				f151001Repo.AsForUpdate().GetDatasByTrueAndCondition(
					o =>
						o.TAR_DC_CODE == tarDcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo).First();
			var orginalStatus = item.STATUS;
			item.STATUS = "4";
			item.LOCK_STATUS = "3";
			item.TAR_MOVE_STAFF = Current.Staff;
			item.TAR_MOVE_NAME = Current.StaffName;
			f151001Repo.Update(item);
			var sharedSrv = new SharedService(_wmsTransaction);
			sharedSrv.UpdateSourceNoStatus(SourceType.Allocation, item.DC_CODE, item.GUP_CODE, item.CUST_CODE, item.ALLOCATION_NO, item.STATUS);
			return new ExecuteResult { IsSuccessed = true, Message = orginalStatus };
		}

		public ExecuteResult UpdateF191204(string dcCode, string gupCode, string custCode,string itemCode, string allocationNo)
		{
			var f191204Repo = new F191204Repository(Schemas.CoreSchema,_wmsTransaction);
			var f151002Repo = new F151002Repository(Schemas.CoreSchema);
			var f191204s = f191204Repo.AsForUpdate().GetDatasByTrueAndCondition(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo && o.ITEM_CODE == itemCode && o.STATUS == "0").ToList();
			var f151002s = f151002Repo.GetDatasByTrueAndCondition(o=>o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo).ToList();
			bool isChange = false;
			foreach (var f191204 in f191204s)
			{
				if (!f151002s.Any(o => o.ALLOCATION_NO == allocationNo && o.ALLOCATION_SEQ == f191204.ALLOCATION_SEQ && o.STATUS == "1"))
				{
					f191204.STATUS = "1";
					isChange = true;
				}
			}
			if (isChange)
				f191204Repo.BulkUpdate(f191204s);
			return new ExecuteResult(true);
		}
	}
}

