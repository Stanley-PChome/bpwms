
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Common.Enums;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P02.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P08.Services
{
	public partial class P080301Service
	{
		private WmsTransaction _wmsTransaction;
		public P080301Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F151002Data> GetF151002Datas(string srcDcCode, string gupCode, string custCode, string allocationNo,
			string userId, string userName, bool isAllowStatus2, bool isDiffWareHouse)
		{
			var f151002Repo = new F151002Repository(Schemas.CoreSchema);
			var queryAllocationNo = allocationNo;
			if (string.IsNullOrEmpty(allocationNo))
			{
				var items = f151002Repo.GetF151002Datas(srcDcCode, gupCode, custCode, allocationNo, userId, isAllowStatus2, isDiffWareHouse);
				//優先給調撥單下架人員等於登入者 否則隨機給他一張調撥單
				var item = items.FirstOrDefault(o => o.SRC_MOVE_STAFF == userId && o.SRC_MOVE_NAME == userName) ?? items.FirstOrDefault();
				if (item != null)
					queryAllocationNo = item.ALLOCATION_NO;
			}
			if (string.IsNullOrEmpty(queryAllocationNo))
				return new List<F151002Data>().AsQueryable();
			var data = f151002Repo.GetF151002Datas(srcDcCode, gupCode, custCode, queryAllocationNo, userId, isAllowStatus2, isDiffWareHouse);
			var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
			var combinItemData = data.Where(o => o.COMBIN_NO.HasValue).ToList();
			foreach (var f151002Data in combinItemData)
			{
				var list = f2501Repo.GetDatasByCombinNo(gupCode, custCode, f151002Data.COMBIN_NO.Value).ToList();
				f151002Data.SERIAL_NOByShow = string.Join("、", list.Where(o => o.SERIAL_NO != f151002Data.SERIAL_NO).Select(o => o.SERIAL_NO).ToArray());
			}
			return data;
		}

		public ExecuteResult OutOfStock(string dcCode, string gupCode, string custCode, string allocationNo,
		string srcLocCode, string itemCode, string validDate, string serialNo, string makeNo, string boxCrtlNo, string palletNo)
		{
			var result = new ExecuteResult() { IsSuccessed = true };
			var dtmValidDate = DateTime.Parse(validDate);
			var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
            var f151002Data =
                f151002Repo.Filter(
                    o =>
                        o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo &&
                        o.ITEM_CODE == itemCode && o.SRC_LOC_CODE == srcLocCode && o.VALID_DATE == dtmValidDate &&
                        o.MAKE_NO == makeNo && o.BOX_CTRL_NO == boxCrtlNo && o.PALLET_CTRL_NO == palletNo).ToList();
			f151002Data = string.IsNullOrEmpty(serialNo) ? f151002Data.Where(o => string.IsNullOrEmpty(o.SERIAL_NO)).ToList() : f151002Data.Where(o => o.SERIAL_NO == serialNo).ToList();
			f151002Data = f151002Data.Where(o => o.STATUS == "0" && o.SRC_QTY > 0).ToList();
			var f151003Repo = new F151003Repository(Schemas.CoreSchema, _wmsTransaction);
			foreach (var f151002 in f151002Data)
			{
				var f151003 = new F151003
				{
					ALLOCATION_NO = f151002.ALLOCATION_NO,
					ALLOCATION_SEQ = f151002.ALLOCATION_SEQ,
					ITEM_CODE = f151002.ITEM_CODE,
					MOVE_QTY = (int)f151002.SRC_QTY,
					LACK_QTY = (int)(f151002.SRC_QTY - f151002.A_SRC_QTY),
					REASON = "101",
					STATUS = "0",
					DC_CODE = f151002.DC_CODE,
					GUP_CODE = f151002.GUP_CODE,
					CUST_CODE = f151002.CUST_CODE,
                    LACK_TYPE = "0"
				};
				f151003Repo.Add(f151003);
				f151002.TAR_QTY = f151002.A_SRC_QTY;
				f151002.STATUS = "1";
				f151002Repo.Update(f151002);
			}
			result.IsSuccessed = true;
			result.Message = "";

			return result;
		}

		public IQueryable<F151002ItemLocData> GetF151002ItemLocDatas(string dcCode, string gupCode, string custCode,
			string allocationNo, string itemCode, bool isDiffWareHouse)
		{
			var f151002Repo = new F151002Repository(Schemas.CoreSchema);
			return f151002Repo.GetF151002ItemLocDatas(dcCode, gupCode, custCode, allocationNo, itemCode, isDiffWareHouse);
		}

		public ExecuteResult ScanSrcLocItemCodeActualQty(string dcCode, string gupCode, string custCode, string allocationNo, string srcLocCode, string itemCode,
			string serialNo, string orginalValidDate, string newValidDate, int addActualQty, string scanCode,
            string orginalMakeNo, string newMakeNo, string palletCtrlNo, string boxCtrlNo)
		{
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
					var checkSerialResult = serialNoService.CheckBarCode(gupCode, custCode, itemCode, sn);
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

			ExecuteResult result = null;
			var dtmOrginalValidDate = DateTime.Parse(orginalValidDate); //原生效日
			var dtmNewValidDate = DateTime.Parse(newValidDate); //新生效日
            var dtmOrginalMakeNo = orginalMakeNo; //原批號
            var dtmNewMakeNo = newMakeNo; //新批號
            var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);

            // 是否為加工單，如果是加工單 代表是純下架
            bool isPureSrc = false;
            var f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
            var f151001 = f151001Repo.GetSingleData(dcCode, custCode, gupCode, allocationNo);
            if (f151001 != null && string.IsNullOrWhiteSpace(f151001.TAR_WAREHOUSE_ID) && f151001.SOURCE_NO.StartsWith("W"))
                isPureSrc = true;
            
            var allocationDatas = f151002Repo.GetDatas(dcCode, gupCode, custCode, allocationNo).ToList();
			var seq = 1;
			if (allocationDatas.Any())
				seq += allocationDatas.Max(o => o.ALLOCATION_SEQ);

			var barcodeData = serialNoService.BarcodeInspection(gupCode, custCode, sn);
			if (barcodeData.Barcode == BarcodeType.BatchNo && !serialNoService.IsBatchNoItem(gupCode, custCode, itemCode))
				barcodeData.Barcode = BarcodeType.SerialNo;
			if (hasSerial)
			{
				if (serialNoList.Any(o => allocationDatas.All(c => c.SERIAL_NO != o)))
					return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.P080301Service_ScanSerialNoNotExist, barcodeData.BarcodeText) };
				if (!allocationDatas.Any(o => serialNoList.Any(c => c == o.SERIAL_NO) && o.STATUS == "0"))
					return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.P080301Service_ScanItemCodeIsScan, barcodeData.BarcodeText) };
				if (allocationDatas.Any(o => serialNoList.Any(c => c == o.SERIAL_NO) && o.STATUS == "1"))
					return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.P080301Service_ScanSerialIsScan, barcodeData.BarcodeText) };

			}

			allocationDatas = allocationDatas.Where(o => o.ITEM_CODE == itemCode && o.SRC_LOC_CODE == srcLocCode).ToList();

			if (hasSerial)
			{
				if (serialNoList.Any(o => allocationDatas.All(c => c.SERIAL_NO != o)))
					return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.P080301Service_ScanSerialNoLocCodeNotExist, barcodeData.BarcodeText) };
			}

			var updateF151002List = new List<F151002>();
			var addF151002List = new List<F151002>();
			var removeF151002List = new List<F151002>();
            bool isValidDateChange = dtmOrginalValidDate != dtmNewValidDate;
            bool isMakeNoChange = dtmOrginalMakeNo != dtmNewMakeNo;

            foreach (var serial in serialNoList)
			{
				var datas = string.IsNullOrEmpty(serial)
					? allocationDatas.Where(o => string.IsNullOrEmpty(o.SERIAL_NO)).ToList()
					: allocationDatas.Where(o => o.SERIAL_NO == serial).ToList();
				if (datas.Any())
				{
                    var orginalDatas = datas.Where(o => o.VALID_DATE == dtmOrginalValidDate && !o.SRC_VALID_DATE.HasValue &&
                    o.MAKE_NO == dtmOrginalMakeNo && string.IsNullOrWhiteSpace(o.SRC_MAKE_NO)).ToList();

                    List<F151002> newDatas = new List<F151002>() { };
                    newDatas = datas.ToList();
                    if (isValidDateChange)
                    {
                        newDatas = newDatas.Where(o => 
                        o.VALID_DATE == dtmOrginalValidDate && o.SRC_VALID_DATE == dtmNewValidDate).ToList();
                    }
                    if (isMakeNoChange)
                    {
                        newDatas = newDatas.Where(o =>
                       o.MAKE_NO == dtmOrginalMakeNo && o.SRC_MAKE_NO == dtmNewMakeNo).ToList();
                    }

                    if(!isValidDateChange && !isMakeNoChange)
                    {
                        #region 沒改變效期、批號
                        long srcQty = addActualQty;
                        long newSrcQty = addActualQty;
                        foreach (var orginalData in orginalDatas) //逐筆更新實際下架數
                        {
                            var item = orginalData;
                            AdjustOrginalData(isPureSrc, ref item, ref srcQty, ref newSrcQty);
                            updateF151002List.Add(item);
                            if (srcQty <= 0)
                                break;
                        }

                        #endregion
                    }
                    else
                    {
                        #region 效期、批號，其中一個改變的話
                      
                        long srcQty = addActualQty;
                        foreach (var item in orginalDatas) //逐筆扣除原下架數 並建立或更新 新生效日下架數、實際下架數與上架數
                        {
                            var newSrcQty = srcQty;
                            var orginalData = item;
                            AdjustNewData(isPureSrc, ref orginalData, ref srcQty, ref newSrcQty);
                            orginalData.TAR_QTY = item.SRC_QTY;
                            if (orginalData.SRC_QTY == 0)
                                removeF151002List.Add(orginalData);
                            else
                                updateF151002List.Add(orginalData);

                            var findNewItem = SetF151002(ref addF151002List, ref updateF151002List, ref newDatas, orginalData,
                                    isValidDateChange ? dtmNewValidDate : (orginalData.SRC_VALID_DATE ?? orginalData.VALID_DATE),
                                    isMakeNoChange ? dtmNewMakeNo : (orginalData.SRC_MAKE_NO ?? orginalData.MAKE_NO),
                                    ref seq, newSrcQty);

                            if (srcQty <= 0)
                                break;
                        }
                        #endregion
                    }
                }
            }
			//最後一次刪除 更新 新增
			foreach (var f151002 in removeF151002List)
			{
				f151002Repo.DeleteData(f151002.DC_CODE, f151002.GUP_CODE, f151002.CUST_CODE, f151002.ALLOCATION_NO,
					f151002.ALLOCATION_SEQ);
				f1511Repo.DeleteData(f151002.DC_CODE, f151002.GUP_CODE, f151002.CUST_CODE, f151002.ALLOCATION_NO,
					 f151002.ALLOCATION_SEQ.ToString());
			}
			foreach (var f151002 in updateF151002List)
			{
				f151002Repo.UpdateData(f151002.DC_CODE, f151002.GUP_CODE, f151002.CUST_CODE, f151002.ALLOCATION_NO, f151002.ALLOCATION_SEQ, f151002.SRC_QTY, f151002.A_SRC_QTY, f151002.TAR_QTY, f151002.A_TAR_QTY, f151002.STATUS, Current.Staff, Current.StaffName, true);
				f1511Repo.UpdateData(f151002.DC_CODE, f151002.GUP_CODE, f151002.CUST_CODE, f151002.ALLOCATION_NO, f151002.ALLOCATION_SEQ.ToString(), f151002.SRC_QTY, f151002.A_SRC_QTY, f151002.STATUS);
			}
			foreach (var f151002 in addF151002List)
			{
				f151002Repo.Add(f151002);
				var f1511Item = CreateF1511(f151002);
				f1511Repo.Add(f1511Item);
			}
            return result ?? (result = new ExecuteResult() { IsSuccessed = true });
		}

        /// <summary>
        /// 沒有異動生效期、批號的下架數量計算
        /// </summary>
        /// <param name="isPureSrc">是否為純下架</param>
        /// <param name="orginalData">原始明細資料</param>
        /// <param name="srcQty">下架數</param>
        /// <param name="newSrcQty">實際要下架的數量</param>
		private void AdjustOrginalData(bool isPureSrc, ref F151002 orginalData, ref long srcQty, ref long newSrcQty)
		{
			var qty = orginalData.SRC_QTY - orginalData.A_SRC_QTY - srcQty;
			if (qty <= 0)
			{
				orginalData.A_SRC_QTY = orginalData.SRC_QTY;
				srcQty = -qty;
				newSrcQty -= srcQty;
			}
			else
			{
				orginalData.A_SRC_QTY += srcQty;
				srcQty = 0;
			}
			if (orginalData.SRC_QTY == orginalData.A_SRC_QTY)
				orginalData.STATUS = isPureSrc ? "2" : "1";
		}

        /// <summary>
        /// 異動生效期、批號的下架數量計算
        /// </summary>
        /// <param name="isPureSrc">是否為純下架</param>
        /// <param name="orginalData"></param>
        /// <param name="srcQty"></param>
        /// <param name="newSrcQty"></param>
        private void AdjustNewData(bool isPureSrc, ref F151002 orginalData, ref long srcQty, ref long newSrcQty)
        {
            var qty = orginalData.SRC_QTY - orginalData.A_SRC_QTY - srcQty;
            if (qty <= 0)
            {
                orginalData.SRC_QTY = orginalData.A_SRC_QTY;
                srcQty = -qty;
                //扣掉超過的數量=這次的上架數
                newSrcQty -= srcQty;
            }
            else
            {
                orginalData.SRC_QTY -= srcQty;
                //因為是新增，所以原本的沒扣，設為0
                srcQty = 0;
            }
            if (orginalData.SRC_QTY == orginalData.A_SRC_QTY)
                orginalData.STATUS = isPureSrc ? "2" : "1";
        }

        private F151002 SetF151002(ref List<F151002> addF151002List, ref List<F151002> updateF151002List,
            ref List<F151002> newDatas, F151002 orginalData, DateTime dtmNewValidDate, string dtmNewMakeNo, ref int seq, long newSrcQty)
        {
            var findAddOrUpdateItem = (string.IsNullOrEmpty(orginalData.SERIAL_NO))
                               ? addF151002List.FirstOrDefault(
                                   o =>
                                       o.DC_CODE == orginalData.DC_CODE && o.GUP_CODE == orginalData.GUP_CODE && o.CUST_CODE == orginalData.CUST_CODE &&
                                       o.ITEM_CODE == orginalData.ITEM_CODE && o.SRC_LOC_CODE == orginalData.SRC_LOC_CODE &&
                                       o.VNR_CODE == orginalData.VNR_CODE && string.IsNullOrEmpty(o.SERIAL_NO) &&
                                       o.ENTER_DATE == orginalData.ENTER_DATE &&
                                        (o.SRC_VALID_DATE.HasValue ? o.SRC_VALID_DATE : o.VALID_DATE) == dtmNewValidDate &&
                                       o.BOX_CTRL_NO == orginalData.BOX_CTRL_NO &&
                                       o.PALLET_CTRL_NO == orginalData.PALLET_CTRL_NO &&
                                       (string.IsNullOrEmpty(o.SRC_MAKE_NO) ? o.MAKE_NO : o.SRC_MAKE_NO) == dtmNewMakeNo)
                               : addF151002List.FirstOrDefault(
                                   o =>
                                       o.DC_CODE == orginalData.DC_CODE && o.GUP_CODE == orginalData.GUP_CODE && o.CUST_CODE == orginalData.CUST_CODE &&
                                       o.ITEM_CODE == orginalData.ITEM_CODE && o.SRC_LOC_CODE == orginalData.SRC_LOC_CODE &&
                                       o.VNR_CODE == orginalData.VNR_CODE && o.SERIAL_NO == orginalData.SERIAL_NO &&
                                       o.ENTER_DATE == orginalData.ENTER_DATE &&
                                        (o.SRC_VALID_DATE.HasValue ? o.SRC_VALID_DATE : o.VALID_DATE) == dtmNewValidDate &&
                                       o.BOX_CTRL_NO == orginalData.BOX_CTRL_NO && 
                                       o.PALLET_CTRL_NO == orginalData.PALLET_CTRL_NO &&
                                       (string.IsNullOrEmpty(o.SRC_MAKE_NO) ? o.MAKE_NO : o.SRC_MAKE_NO) == dtmNewMakeNo);
            findAddOrUpdateItem = findAddOrUpdateItem ?? ((string.IsNullOrEmpty(orginalData.SERIAL_NO))
                ? updateF151002List.FirstOrDefault(
                    o =>
                        o.DC_CODE == orginalData.DC_CODE && o.GUP_CODE == orginalData.GUP_CODE && o.CUST_CODE == orginalData.CUST_CODE &&
                        o.ITEM_CODE == orginalData.ITEM_CODE && o.SRC_LOC_CODE == orginalData.SRC_LOC_CODE &&
                        o.VNR_CODE == orginalData.VNR_CODE && string.IsNullOrEmpty(o.SERIAL_NO) &&
                        o.ENTER_DATE == orginalData.ENTER_DATE &&
                        (o.SRC_VALID_DATE.HasValue ? o.SRC_VALID_DATE : o.VALID_DATE) == dtmNewValidDate &&
                        o.BOX_CTRL_NO == orginalData.BOX_CTRL_NO &&
                        o.PALLET_CTRL_NO == orginalData.PALLET_CTRL_NO &&
                        (string.IsNullOrEmpty(o.SRC_MAKE_NO) ? o.MAKE_NO : o.SRC_MAKE_NO) == dtmNewMakeNo)
                : updateF151002List.FirstOrDefault(
                    o =>
                        o.DC_CODE == orginalData.DC_CODE && o.GUP_CODE == orginalData.GUP_CODE && o.CUST_CODE == orginalData.CUST_CODE &&
                        o.ITEM_CODE == orginalData.ITEM_CODE && o.SRC_LOC_CODE == orginalData.SRC_LOC_CODE &&
                        o.VNR_CODE == orginalData.VNR_CODE && o.SERIAL_NO == orginalData.SERIAL_NO &&
                        o.ENTER_DATE == orginalData.ENTER_DATE &&
                        (o.SRC_VALID_DATE.HasValue ? o.SRC_VALID_DATE : o.VALID_DATE) == dtmNewValidDate &&
                        o.BOX_CTRL_NO == orginalData.BOX_CTRL_NO && 
                        o.PALLET_CTRL_NO == orginalData.PALLET_CTRL_NO &&
                        (string.IsNullOrEmpty(o.SRC_MAKE_NO) ? o.MAKE_NO : o.SRC_MAKE_NO) == dtmNewMakeNo));
            var findNewItem = findAddOrUpdateItem ?? ((string.IsNullOrEmpty(orginalData.SERIAL_NO))
                ? newDatas.FirstOrDefault(
                    o =>
                        o.DC_CODE == orginalData.DC_CODE && o.GUP_CODE == orginalData.GUP_CODE && o.CUST_CODE == orginalData.CUST_CODE &&
                        o.ITEM_CODE == orginalData.ITEM_CODE && o.SRC_LOC_CODE == orginalData.SRC_LOC_CODE &&
                        o.VNR_CODE == orginalData.VNR_CODE && string.IsNullOrEmpty(o.SERIAL_NO) &&
                        o.ENTER_DATE == orginalData.ENTER_DATE &&
                        (o.SRC_VALID_DATE.HasValue ? o.SRC_VALID_DATE : o.VALID_DATE) == dtmNewValidDate &&
                        o.BOX_CTRL_NO == orginalData.BOX_CTRL_NO &&
                        o.PALLET_CTRL_NO == orginalData.PALLET_CTRL_NO &&
                        (string.IsNullOrEmpty(o.SRC_MAKE_NO) ? o.MAKE_NO : o.SRC_MAKE_NO) == dtmNewMakeNo)
                : newDatas.FirstOrDefault(
                    o =>
                        o.DC_CODE == orginalData.DC_CODE && o.GUP_CODE == orginalData.GUP_CODE && o.CUST_CODE == orginalData.CUST_CODE &&
                        o.ITEM_CODE == orginalData.ITEM_CODE && o.SRC_LOC_CODE == orginalData.SRC_LOC_CODE &&
                        o.VNR_CODE == orginalData.VNR_CODE && o.SERIAL_NO == orginalData.SERIAL_NO &&
                        o.ENTER_DATE == orginalData.ENTER_DATE &&
                         (o.SRC_VALID_DATE.HasValue ? o.SRC_VALID_DATE : o.VALID_DATE) == dtmNewValidDate &&
                        o.BOX_CTRL_NO == orginalData.BOX_CTRL_NO && 
                        o.PALLET_CTRL_NO == orginalData.PALLET_CTRL_NO &&
                        (string.IsNullOrEmpty(o.SRC_MAKE_NO) ? o.MAKE_NO : o.SRC_MAKE_NO) == dtmNewMakeNo));

            if (findNewItem == null)
            {
                findNewItem = CreateF151002(orginalData, seq);
                findNewItem.SRC_QTY = newSrcQty;
                findNewItem.A_SRC_QTY = newSrcQty;
                findNewItem.TAR_QTY = newSrcQty;
                findNewItem.STATUS = "1";
                findNewItem.SRC_VALID_DATE = dtmNewValidDate;
                findNewItem.SRC_MAKE_NO = dtmNewMakeNo;
                addF151002List.Add(findNewItem);
                seq++;
            }
            else
            {
                findNewItem.SRC_QTY += newSrcQty;
                findNewItem.A_SRC_QTY += newSrcQty;
                findNewItem.TAR_QTY += newSrcQty;
                findNewItem.STATUS = "1";
                if (findAddOrUpdateItem == null)
                    updateF151002List.Add(findNewItem);
            }
            return findNewItem;
        }

        public ExecuteResult RemoveSrcLocItemCodeActualQty(string dcCode, string gupCode, string custCode, string allocationNo, string srcLocCode, string itemCode,
			string removeValidDate,string removeMakeNo)
		{
			ExecuteResult result = null;
            
			long loMoveQty = 0; //移除數量 For LO
			var dtmNewValidDate = DateTime.Parse(removeValidDate); //移除的生效日
            var dtmNewMakeNo = removeMakeNo; //移除的批號
            var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			var datas = f151002Repo.GetDatas(dcCode, gupCode, custCode, allocationNo).ToList();
			var seq = 1;
            List<string> errMessage = new List<string>(); //存取錯誤訊息用
			if (datas.Any())
				seq += datas.Max(o => o.ALLOCATION_SEQ);
			datas = datas.Where(o => o.SRC_LOC_CODE == srcLocCode && o.ITEM_CODE == itemCode).ToList();

            var orginalDatas = datas.Where(o => !o.SRC_VALID_DATE.HasValue && string.IsNullOrWhiteSpace(o.SRC_MAKE_NO)).ToList();
            var clearDatas = datas.Where(o => ((o.VALID_DATE == dtmNewValidDate && !o.SRC_VALID_DATE.HasValue) || o.SRC_VALID_DATE == dtmNewValidDate) &&
                                                ((o.MAKE_NO == dtmNewMakeNo && string.IsNullOrWhiteSpace(o.SRC_MAKE_NO)) || o.SRC_MAKE_NO == dtmNewMakeNo) &&
                                                o.A_SRC_QTY > 0).ToList();

			var updateF151002List = new List<F151002>();
			var addF151002List = new List<F151002>();
			var removeF151002List = new List<F151002>();

			foreach (var clearData in clearDatas)
			{
				loMoveQty += clearData.A_SRC_QTY;

                bool isValidDateChange = clearData.SRC_VALID_DATE.HasValue;
                bool isMakeNoChange = !string.IsNullOrWhiteSpace(clearData.SRC_MAKE_NO);

                if(!isValidDateChange && !isMakeNoChange)
                {
                    #region 效期、批號都未修改的部分

                    CheckF151003Data(clearData.DC_CODE, clearData.GUP_CODE, clearData.CUST_CODE,
                        clearData.ALLOCATION_NO, clearData.ALLOCATION_SEQ, clearData.ITEM_CODE, errMessage);

                    clearData.A_SRC_QTY = 0;
                    clearData.STATUS = "0";
                    updateF151002List.Add(clearData);

                    #endregion
                }
                else
                {
                    #region 效期、批號其中一個有修改
                    SetF151002ByRemove(ref addF151002List, ref updateF151002List, ref orginalDatas, clearData, ref seq, errMessage);
                 
                    removeF151002List.Add(clearData);
                    #endregion
                }
                if (errMessage.Any())
                    return new ExecuteResult(false, errMessage.FirstOrDefault());
            }

            //最後一次刪除 更新 新增
            foreach (var f151002 in removeF151002List)
			{
				f151002Repo.DeleteData(f151002.DC_CODE, f151002.GUP_CODE, f151002.CUST_CODE, f151002.ALLOCATION_NO, f151002.ALLOCATION_SEQ);
				f1511Repo.DeleteData(f151002.DC_CODE, f151002.GUP_CODE, f151002.CUST_CODE, f151002.ALLOCATION_NO,
				 f151002.ALLOCATION_SEQ.ToString());
			}
			foreach (var f151002 in updateF151002List)
			{
				f151002Repo.UpdateData(f151002.DC_CODE, f151002.GUP_CODE, f151002.CUST_CODE, f151002.ALLOCATION_NO, f151002.ALLOCATION_SEQ, f151002.SRC_QTY, f151002.A_SRC_QTY, f151002.TAR_QTY, f151002.A_TAR_QTY, f151002.STATUS, Current.Staff, Current.StaffName, true);
				f1511Repo.UpdateData(f151002.DC_CODE, f151002.GUP_CODE, f151002.CUST_CODE, f151002.ALLOCATION_NO, f151002.ALLOCATION_SEQ.ToString(), f151002.SRC_QTY, f151002.A_SRC_QTY, f151002.STATUS);
			}
			foreach (var f151002 in addF151002List)
			{
				f151002Repo.Add(f151002);
				var f1511Item = CreateF1511(f151002);
				f1511Repo.Add(f1511Item);
			}
            
			if (result == null)
				result = new ExecuteResult() { IsSuccessed = true };
			return result;
		}

        private void SetF151002ByRemove(ref List<F151002> addF151002List, ref List<F151002> updateF151002List, ref List<F151002> orginalDatas, F151002 clearData, ref int seq,List<string> errMessage)
        {
            //取得原始資料
            var addOrUpdateItem = (string.IsNullOrEmpty(clearData.SERIAL_NO))
                ? addF151002List.FirstOrDefault(
                    o => o.DC_CODE == clearData.DC_CODE && o.GUP_CODE == clearData.GUP_CODE && o.CUST_CODE == clearData.CUST_CODE && o.ITEM_CODE == clearData.ITEM_CODE &&
                    o.SRC_LOC_CODE == clearData.SRC_LOC_CODE &&
                    o.VALID_DATE == clearData.VALID_DATE &&
                    o.ENTER_DATE == clearData.ENTER_DATE && o.VNR_CODE == clearData.VNR_CODE &&
                    string.IsNullOrEmpty(o.SERIAL_NO) && o.BOX_CTRL_NO == clearData.BOX_CTRL_NO && o.PALLET_CTRL_NO == clearData.PALLET_CTRL_NO &&
                    o.MAKE_NO == clearData.MAKE_NO)
                : addF151002List.FirstOrDefault(
                    o => o.DC_CODE == clearData.DC_CODE && o.GUP_CODE == clearData.GUP_CODE && o.CUST_CODE == clearData.CUST_CODE && o.ITEM_CODE == clearData.ITEM_CODE &&
                    o.SRC_LOC_CODE == clearData.SRC_LOC_CODE &&
                    o.VALID_DATE == clearData.VALID_DATE &&
                    o.ENTER_DATE == clearData.ENTER_DATE && o.VNR_CODE == clearData.VNR_CODE &&
                    o.SERIAL_NO == clearData.SERIAL_NO && o.BOX_CTRL_NO == clearData.BOX_CTRL_NO && o.PALLET_CTRL_NO == clearData.PALLET_CTRL_NO &&
                    o.MAKE_NO == clearData.MAKE_NO);

            addOrUpdateItem = addOrUpdateItem ?? ((string.IsNullOrEmpty(clearData.SERIAL_NO))
                ? updateF151002List.FirstOrDefault(
                    o => o.DC_CODE == clearData.DC_CODE && o.GUP_CODE == clearData.GUP_CODE && o.CUST_CODE == clearData.CUST_CODE && o.ITEM_CODE == clearData.ITEM_CODE &&
                    o.SRC_LOC_CODE == clearData.SRC_LOC_CODE &&
                        o.VALID_DATE == clearData.VALID_DATE &&
                        o.ENTER_DATE == clearData.ENTER_DATE && o.VNR_CODE == clearData.VNR_CODE &&
                        string.IsNullOrEmpty(o.SERIAL_NO) && o.BOX_CTRL_NO == clearData.BOX_CTRL_NO && o.PALLET_CTRL_NO == clearData.PALLET_CTRL_NO &&
                        o.MAKE_NO == clearData.MAKE_NO)
                : updateF151002List.FirstOrDefault(
                    o => o.DC_CODE == clearData.DC_CODE && o.GUP_CODE == clearData.GUP_CODE && o.CUST_CODE == clearData.CUST_CODE && o.ITEM_CODE == clearData.ITEM_CODE &&
                    o.SRC_LOC_CODE == clearData.SRC_LOC_CODE &&
                        o.VALID_DATE == clearData.VALID_DATE &&
                        o.ENTER_DATE == clearData.ENTER_DATE && o.VNR_CODE == clearData.VNR_CODE &&
                        o.SERIAL_NO == clearData.SERIAL_NO && o.BOX_CTRL_NO == clearData.BOX_CTRL_NO && o.PALLET_CTRL_NO == clearData.PALLET_CTRL_NO &&
                        o.MAKE_NO == clearData.MAKE_NO));

            var orginalItem = addOrUpdateItem ?? ((string.IsNullOrEmpty(clearData.SERIAL_NO))
                ? orginalDatas.FirstOrDefault(
                    o => o.DC_CODE == clearData.DC_CODE && o.GUP_CODE == clearData.GUP_CODE && o.CUST_CODE == clearData.CUST_CODE && o.ITEM_CODE == clearData.ITEM_CODE &&
                    o.SRC_LOC_CODE == clearData.SRC_LOC_CODE &&
                        o.VALID_DATE == clearData.VALID_DATE &&
                        o.ENTER_DATE == clearData.ENTER_DATE && o.VNR_CODE == clearData.VNR_CODE &&
                        string.IsNullOrEmpty(o.SERIAL_NO) && o.BOX_CTRL_NO == clearData.BOX_CTRL_NO && o.PALLET_CTRL_NO == clearData.PALLET_CTRL_NO &&
                        o.MAKE_NO == clearData.MAKE_NO)
                : orginalDatas.FirstOrDefault(
                    o => o.DC_CODE == clearData.DC_CODE && o.GUP_CODE == clearData.GUP_CODE && o.CUST_CODE == clearData.CUST_CODE && o.ITEM_CODE == clearData.ITEM_CODE &&
                    o.SRC_LOC_CODE == clearData.SRC_LOC_CODE &&
                        o.VALID_DATE == clearData.VALID_DATE &&
                        o.ENTER_DATE == clearData.ENTER_DATE && o.VNR_CODE == clearData.VNR_CODE &&
                        o.SERIAL_NO == clearData.SERIAL_NO && o.BOX_CTRL_NO == clearData.BOX_CTRL_NO && o.PALLET_CTRL_NO == clearData.PALLET_CTRL_NO &&
                        o.MAKE_NO == clearData.MAKE_NO));
            if (orginalItem == null) //沒找到原始資料 則建立新一筆
            {
                orginalItem = CreateF151002(clearData, seq);
                orginalItem.SRC_QTY = clearData.SRC_QTY;
                orginalItem.TAR_QTY = clearData.TAR_QTY;
                orginalItem.STATUS = "0";
                orginalItem.A_SRC_QTY = 0;
                orginalItem.VALID_DATE = clearData.VALID_DATE;
                orginalItem.MAKE_NO = clearData.MAKE_NO;
                addF151002List.Add(orginalItem);
                seq++;
            }
            else //找到原始資料 則更新下架數 上架數
            {
                CheckF151003Data(
                    orginalItem.DC_CODE, orginalItem.GUP_CODE, orginalItem.CUST_CODE,
                    orginalItem.ALLOCATION_NO, orginalItem.ALLOCATION_SEQ, orginalItem.ITEM_CODE, errMessage);

                if (errMessage.Any())
                    return;
                
                orginalItem.SRC_QTY += clearData.SRC_QTY;
                orginalItem.TAR_QTY += clearData.TAR_QTY;
                orginalItem.STATUS = "0";
                if (addOrUpdateItem == null)
                    updateF151002List.Add(orginalItem);
            }
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
                   dcCode, gupCode, custCode, allocationNo, allocationSeq, itemCode, "0").ToList();

            if (isOutOfStockData.Any())
                errMessage.Add(Properties.Resources.P080301_IsOutOfStockData);
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
        ALLOCATION_DATE = orginalItem.ALLOCATION_DATE,
        ITEM_CODE = orginalItem.ITEM_CODE,
        SRC_LOC_CODE = orginalItem.SRC_LOC_CODE,
        SUG_LOC_CODE = orginalItem.SUG_LOC_CODE,
        TAR_LOC_CODE = orginalItem.TAR_LOC_CODE,
        A_TAR_QTY = orginalItem.A_TAR_QTY,
        SERIAL_NO = orginalItem.SERIAL_NO,
        VALID_DATE = orginalItem.VALID_DATE,
        CHECK_SERIALNO = orginalItem.CHECK_SERIALNO,
        SRC_STAFF = Current.Staff,
        SRC_NAME = Current.StaffName,
        SRC_DATE = DateTime.Now,
        TAR_STAFF = orginalItem.TAR_STAFF,
        TAR_NAME = orginalItem.TAR_NAME,
        TAR_DATE = orginalItem.TAR_DATE,
        TAR_VALID_DATE = orginalItem.TAR_VALID_DATE,
        ENTER_DATE = orginalItem.ENTER_DATE,
        VNR_CODE = orginalItem.VNR_CODE,
        BOX_CTRL_NO = orginalItem.BOX_CTRL_NO,
        PALLET_CTRL_NO = orginalItem.PALLET_CTRL_NO,
        MAKE_NO = orginalItem.MAKE_NO,
        BIN_CODE = orginalItem.BIN_CODE,
        SOURCE_TYPE = orginalItem.SOURCE_TYPE,
        SOURCE_NO = orginalItem.SOURCE_NO,
        REFENCE_NO = orginalItem.REFENCE_NO,
        REFENCE_SEQ = orginalItem.REFENCE_SEQ,
        CONTAINER_CODE = orginalItem.CONTAINER_CODE,
        ORG_SEQ = orginalItem.ORG_SEQ,
        A_SRC_QTY = orginalItem.A_SRC_QTY,
        BOX_NO = orginalItem.BOX_NO,
        SRC_MAKE_NO = orginalItem.SRC_MAKE_NO,
        SRC_QTY = orginalItem.SRC_QTY,
        RECEIPTFLAG = orginalItem.RECEIPTFLAG,
        SRC_VALID_DATE = orginalItem.SRC_VALID_DATE,
        TAR_MAKE_NO = orginalItem.TAR_MAKE_NO,
        STICKER_PALLET_NO = orginalItem.STICKER_PALLET_NO,
        TAR_QTY = orginalItem.TAR_QTY,
        STATUS = orginalItem.STATUS
      };
      return item;
    }

    private F1511 CreateF1511(F151002 f151002)
		{
			var f1511 = new F1511
			{
				DC_CODE = f151002.DC_CODE,
				GUP_CODE = f151002.GUP_CODE,
				CUST_CODE = f151002.CUST_CODE,
				ORDER_NO = f151002.ALLOCATION_NO,
				ORDER_SEQ = f151002.ALLOCATION_SEQ.ToString(),
				STATUS = (f151002.STATUS == "1") ? "1" : "0",   // 當下架完成時，將虛擬儲位狀態改為已搬動
				B_PICK_QTY = (int)f151002.SRC_QTY,
				A_PICK_QTY = (int)f151002.A_SRC_QTY,
			};
			return f1511;
		}

		public ExecuteResult StartDownOrUpItemChangeStatus(string dcCode, string gupCode, string custCode, string allocationNo, bool isUp = false)
		{
			var f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
			var item =
				f151001Repo.Find(
					o =>
						o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo);
			var orginalStatus = item.STATUS;
			item.STATUS = (isUp) ? "4" : "2";
			item.LOCK_STATUS = (isUp) ? "3" : "1";
			if (!isUp)
			{
				item.SRC_MOVE_STAFF = Current.Staff;
				item.SRC_MOVE_NAME = Current.StaffName;
			}
			else
			{
				item.TAR_MOVE_STAFF = Current.Staff;
				item.TAR_MOVE_NAME = Current.StaffName;
			}
			f151001Repo.Update(item);
			var sharedSrv = new SharedService(_wmsTransaction);
			sharedSrv.UpdateSourceNoStatus(SourceType.Allocation, item.DC_CODE, item.GUP_CODE, item.CUST_CODE, item.ALLOCATION_NO, item.STATUS);
			return new ExecuteResult { IsSuccessed = true, Message = orginalStatus };
		}

		public ExecuteResult ChangeAllocationDownOrUpStatusToOrginal(string dcCode, string gupCode, string custCode,
			string allocationNo, string status, bool isUp, bool lackType)
		{
			var f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
			var item =
				f151001Repo.Find(
					o =>
						o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo);
			item.STATUS = status;

			if (!lackType) //缺貨 ,  不更新 LOCK_STATUS
				item.LOCK_STATUS = (isUp) ? "2" : "0";

			var f151002Repo = new F151002Repository(Schemas.CoreSchema);
			var datas = f151002Repo.GetDatas(item.DC_CODE, item.GUP_CODE, item.CUST_CODE, item.ALLOCATION_NO);
			if (!datas.Any(o => o.STATUS == ((isUp) ? "2" : "1")))
			{
				if (!isUp)
				{
					item.SRC_MOVE_STAFF = "";
					item.SRC_MOVE_NAME = "";
				}
				else
				{
					item.TAR_MOVE_STAFF = "";
					item.TAR_MOVE_NAME = "";
				}
			}
			f151001Repo.Update(item);
			var sharedSrv = new SharedService(_wmsTransaction);
			sharedSrv.UpdateSourceNoStatus(SourceType.Allocation, item.DC_CODE, item.GUP_CODE, item.CUST_CODE, item.ALLOCATION_NO, item.STATUS);
			return new ExecuteResult { IsSuccessed = true, Message = "" };
		}

		public ExecuteResult ChangeAllocationDownOrUpFinish(string dcCode, string gupCode, string custCode,
			string allocationNo, bool isUp)
		{
			List<F1912LocVolumn> locVolumn = new List<F1912LocVolumn>();
			var f151001Repo = new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f151002Repo = new F151002Repository(Schemas.CoreSchema, _wmsTransaction);
            var sharedService = new SharedService();

            var item =
				f151001Repo.Find(
					o =>
						o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo);
			var orginalStatus = item.STATUS;
			if (!isUp)
			{
				item.SRC_MOVE_STAFF = Current.Staff;
				item.SRC_MOVE_NAME = Current.StaffName;
			}
			else
			{
				item.TAR_MOVE_STAFF = Current.Staff;
				item.TAR_MOVE_NAME = Current.StaffName;
			}

			//若改為在刷讀時，在Server端直接呼叫此Method，可改為由參數中傳入，不需再由資料庫取出
			var f151002s = f151002Repo.GetDatasByTrueAndCondition(a => a.DC_CODE == dcCode && a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.ALLOCATION_NO == allocationNo).ToList();


			if (!isUp && string.IsNullOrEmpty(item.TAR_WAREHOUSE_ID)) //沒有目的倉，要直接結案
			{
				isUp = true;
				var p020301Service = new P020301Service(_wmsTransaction);

				//var f91020502Repo = new F91020502Repository(Schemas.CoreSchema, _wmsTransaction);
				//var f91020502 = f91020502Repo.GetDatasByTrueAndCondition(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.ALLOCATION_NO == allocationNo).FirstOrDefault();

				//修改調撥單的LO為已完成
				//var result = p020301Service.FinishAllocationLo(item, f151002s, AllocationType.NoTarget);
			}

			item.STATUS = (isUp) ? "5" : "3";
			item.LOCK_STATUS = (isUp) ? "4" : "2";
			if (isUp)
				item.POSTING_DATE = DateTime.Now;
			f151001Repo.Update(item);
			var sharedSrv = new SharedService(_wmsTransaction);

			//更新 來源單據狀態
			sharedSrv.UpdateSourceNoStatus(SourceType.Allocation, item.DC_CODE, item.GUP_CODE, item.CUST_CODE, item.ALLOCATION_NO, item.STATUS);

			//清除此調撥單已拆開序號的箱號/盒號/儲值卡盒號
			var serialNoService = new SerialNoService(_wmsTransaction);
			serialNoService.ClearSerialByBoxOrCaseNo(dcCode, gupCode, custCode, item.ALLOCATION_NO, isUp ? "TU" : "TD");
			//上架完成後 清除F1913 此調撥單商品 QTY<0 
			if (isUp)
			{
				var itemList =
					f151002Repo.GetDatasByTrueAndCondition(
						o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == allocationNo)
						.Select(o => o.ITEM_CODE).Distinct().ToList();
				var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
				foreach (var itemCode in itemList)
					f1913Repo.DeleteDataByItemZeroQty(dcCode, gupCode, custCode, itemCode);

				#region 更新F1511
				var f1511Repo = new F1511Repository(Schemas.CoreSchema);
				foreach (var f151002 in f151002s)
				{
					var f1511 = f1511Repo.Find(
						o =>
							o.DC_CODE == f151002.DC_CODE && o.GUP_CODE == f151002.GUP_CODE && o.CUST_CODE == f151002.CUST_CODE &&
							o.ORDER_NO == f151002.ALLOCATION_NO && o.ORDER_SEQ == f151002.ALLOCATION_SEQ.ToString());
					if (f1511 != null)
					{
						f1511.STATUS = "2";
						f1511.A_PICK_QTY = f1511.B_PICK_QTY;
						f1511Repo.Update(f1511);
					}
				}
				#endregion

			}

			//跨倉DC調撥時-> 要將序號狀態改為(下架)C1  , 上架(A1)
			if (item.SRC_DC_CODE != item.TAR_DC_CODE)
			{
				var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
				var serialItemData = f151002s.Where(o => !string.IsNullOrEmpty(o.SERIAL_NO)).ToList();
				foreach (var sItem in serialItemData)
				{
					var f2501Item = f2501Repo.Find(o => o.GUP_CODE == sItem.GUP_CODE && o.CUST_CODE == sItem.CUST_CODE
										&& o.SERIAL_NO == sItem.SERIAL_NO);
					if (f2501Item != null)
					{
						f2501Item.STATUS = (isUp) ? "A1" : "C1";
						f2501Repo.Update(f2501Item);
						if (f2501Item.COMBIN_NO.HasValue) //組合商品 要一併更新被組合商品序號
						{
							var data = f2501Repo.AsForUpdate().GetDatasByCombinNo(f2501Item.GUP_CODE, f2501Item.CUST_CODE, f2501Item.COMBIN_NO.Value).Where(o => o.SERIAL_NO != sItem.SERIAL_NO);
							foreach (var f2501 in data)
							{
								f2501.STATUS = (isUp) ? "A1" : "C1";
								f2501Repo.Update(f2501);
							}
						}
					}
				}
			}

			//更新儲位容量
			sharedSrv.UpdateAllocationLocVolumn(item, f151002s);

            // 調撥下架針對一張調撥單完成下架後呼叫調撥函數以發送入庫任務。
            if (item.STATUS == "3")
                sharedService.CreateInBoundJob(item.DC_CODE, item.GUP_CODE, item.CUST_CODE, item.ALLOCATION_NO, item.TAR_WAREHOUSE_ID);

            return new ExecuteResult { IsSuccessed = true, Message = orginalStatus };
		}

		public ExecuteResult InsertOrUpdateF151004(string dcCode, string gupCode, string custCode, string allocationNo,
			string caseNo)
		{
			var f151004Repo = new F151004Repository(Schemas.CoreSchema, _wmsTransaction);
			var item =
				f151004Repo.AsForUpdate().GetDatasByTrueAndCondition(
					o =>
						o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode &&
						o.MOVE_BOX_NO == caseNo).OrderByDescending(o=>o.CRT_DATE).FirstOrDefault();
			if (item != null)
			{
                //不同調撥單更新箱號
                if (item.ALLOCATION_NO != allocationNo)
                {
                    var f151001Repo=new F151001Repository(Schemas.CoreSchema, _wmsTransaction);
                    var f151001s = f151001Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ALLOCATION_NO == item.ALLOCATION_NO && o.STATUS == "5");
                    if (f151001s != null)
                    {
                        item.ALLOCATION_NO = allocationNo;
                        item.CRT_DATE = DateTime.Now;
                        item.UPD_DATE = null;
                        item.UPD_STAFF = "";
                        item.UPD_NAME = "";
                        f151004Repo.Add(item);
                      
                        return new ExecuteResult { IsSuccessed = true, Message = "" };
                    }
                    else
                    {
                        return new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P080301Service_BoxUsed };
                    }
                }
                //同調撥單更新
				item.UPD_DATE = DateTime.Now;
				item.UPD_STAFF = Current.Staff;
				item.UPD_NAME = Current.StaffName;
				f151004Repo.Update(item);
			}
			else
			{
				item = new F151004
				{
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					ALLOCATION_NO = allocationNo,
					MOVE_BOX_NO = caseNo
				};
				f151004Repo.Add(item);
			}
			return new ExecuteResult { IsSuccessed = true, Message = "" };
		}
	}
}

