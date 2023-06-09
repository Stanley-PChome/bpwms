using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Enums;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Enums;
using Wms3pl.WebServices.Shared.ServiceEntites;

namespace Wms3pl.WebServices.Shared.Services
{
    public partial class ItemService
    {
        /// <summary>
        /// 商品單位階層暫存資料
        /// </summary>
        private List<GetItemLevelRes> _itemLevelCacheList;

        /// <summary>
        /// 取得商品單位轉換資料
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="itemCodes"></param>
        /// <returns></returns>
        public List<ItemUnit> GetItemUnitList(string gupCode, List<string> itemCodes)
        {
            var f190301Repo = new F190301Repository(Schemas.CoreSchema);
            return f190301Repo.GetItemUnits(gupCode, itemCodes).ToList();
        }
        public string GetVolumeUnit(List<ItemUnit> data, int qty)
        {
            var volumeUnitResult = string.Empty;
            //排序(階層大到小)
            data = data.OrderByDescending(x => x.UNIT_LEVEL).ToList();
            //有最大階層開始計算
            foreach (var item in data)
            {
                //計算該階層最小單位數量
                var levelMinQty = item.UNIT_QTY;
                var data2 = data.Where(x => x.UNIT_LEVEL < item.UNIT_LEVEL).OrderByDescending(x => x.UNIT_LEVEL).ToList();
                foreach (var item2 in data2)
                    levelMinQty = levelMinQty * item2.UNIT_QTY;

                //如果數量超過該階層最小單位數量 
                if (qty >= levelMinQty)
                {
                    //該階層可分配單位數量
                    var unitQty = Math.Floor((decimal)qty / (decimal)levelMinQty);
                    //剩餘數量 在由下一階層分配
                    qty = qty % levelMinQty;
                    volumeUnitResult += string.Format("{0}{1}", unitQty, item.UNIT_NAME);
                }
                else
                {
                    volumeUnitResult += string.Format("{0}{1}", 0, item.UNIT_NAME);
                }
            }
            return volumeUnitResult;
        }

        /// <summary>
        /// 計算整數箱和零數箱
        /// </summary>
        /// <param name="data"></param>
        /// <param name="qty"></param>
        /// <param name="fullBoxQty">整數箱</param>
        /// <param name="bulkBoxQty">零數箱</param>
        public void GetBoxQty(List<ItemUnit> data, long qty, ref int fullBoxQty, ref int bulkBoxQty)
        {
            var volumeUnitResult = string.Empty;
            var boxLevelItemUnit = data.FirstOrDefault(x => x.UNIT_ID == "03"); //03:箱
            if (boxLevelItemUnit == null) //如果找不到就用最大階層來算
                boxLevelItemUnit = data.OrderByDescending(x => x.UNIT_LEVEL).First();

            //排序(階層大到小 從單位為箱往下計算)
            data = data.Where(x => x.UNIT_LEVEL <= boxLevelItemUnit.UNIT_LEVEL).OrderByDescending(x => x.UNIT_LEVEL).ToList();
            //計算該階層最小單位數量
            var levelMinQty = boxLevelItemUnit.UNIT_QTY;
            var data2 = data.Where(x => x.UNIT_LEVEL < boxLevelItemUnit.UNIT_LEVEL).OrderByDescending(x => x.UNIT_LEVEL).ToList();
            foreach (var item2 in data2)
                levelMinQty = levelMinQty * item2.UNIT_QTY;

            //如果數量超過該階層最小單位數量 
            if (qty >= levelMinQty)
            {
                //該階層可分配單位數量
                var unitQty = Math.Floor((decimal)qty / (decimal)levelMinQty);
                //剩餘數量
                qty = qty % levelMinQty;
                //整數箱
                fullBoxQty = (int)unitQty;
            }
            else
            {
                //整數箱
                fullBoxQty = 0;
            }
            //零數箱
            bulkBoxQty = (qty > 0) ? 1 : 0;
        }
        /// <summary>
        /// 最大階層的商品單位資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ItemUnit GetMaxLevelUnit(List<ItemUnit> data)
        {
            return data.OrderByDescending(x => x.UNIT_LEVEL).FirstOrDefault();
        }
        /// <summary>
        /// 最小階層的商品單位資料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ItemUnit GetMinLevelUnit(List<ItemUnit> data)
        {
            return data.OrderBy(x => x.UNIT_LEVEL).FirstOrDefault();
        }

        public decimal GetVolume(ItemUnit itemUnit, long cuftFactor)
        {
            return Math.Ceiling(((itemUnit.LENGTH ?? 0) * (itemUnit.WIDTH ?? 0) * (itemUnit.HEIGHT ?? 0)) / (decimal)cuftFactor);
        }

        /// <summary>
        /// 計算箱入數
        /// </summary>
        /// <param name="data"></param>
        /// <param name="packingMaxValue">F1909.包裝數量換算最大單位</param>
        /// <param name="inBoxQty">箱入數</param>
        public void GetInBoxQty(List<ItemUnit> data, int packingMaxValue, ref int inBoxQty)
        {
            var volumeUnitResult = string.Empty;
            var boxLevelItemUnit = data.FirstOrDefault(x => x.UNIT_LEVEL == packingMaxValue);
            if (boxLevelItemUnit == null) //如果找不到就用最大階層來算
                boxLevelItemUnit = data.OrderByDescending(x => x.UNIT_LEVEL).First();

            //排序(階層大到小 從單位為箱往下計算)
            data = data.Where(x => x.UNIT_LEVEL <= boxLevelItemUnit.UNIT_LEVEL).OrderByDescending(x => x.UNIT_LEVEL).ToList();
            //計算該階層最小單位數量
            var levelMinQty = boxLevelItemUnit.UNIT_QTY;
            var data2 = data.Where(x => x.UNIT_LEVEL < boxLevelItemUnit.UNIT_LEVEL && x.UNIT_LEVEL != 0).OrderByDescending(x => x.UNIT_LEVEL).ToList();
            foreach (var item2 in data2)
                levelMinQty = levelMinQty * item2.UNIT_QTY;

            inBoxQty = (int)levelMinQty;
        }

        /// <summary>
        /// 計算該單位總數量(如單位為箱=>計算箱入數)
        /// </summary>
        /// <param name="data">商品材積階層資料</param>
        /// <param name="unitName">單位名稱</param>
        /// <returns></returns>
        public int? GetUnitQty(List<ItemUnit> data, string unitName)
        {
            var findUnit = data.FirstOrDefault(x => x.UNIT_NAME == unitName);
            if (findUnit == null)
                return null;
            else
            {
                var unitQty = findUnit.UNIT_QTY;
                var data2 = data.Where(x => x.UNIT_LEVEL < findUnit.UNIT_LEVEL).OrderByDescending(x => x.UNIT_LEVEL);
                foreach (var item in data2)
                    unitQty *= item.UNIT_QTY;
                return unitQty;
            }
        }

        /// <summary>
        /// 取得商品單位階層資料
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="itemCodes">品號清單</param>
        /// <returns></returns>
        public List<GetItemLevelRes> GetItemLevelList(string gupCode, string custCode, List<string> itemCodeList)
        {
            var f190301Repo = new F190301Repository(Schemas.CoreSchema);
            var f91000302Repo = new F91000302Repository(Schemas.CoreSchema);
            var f91000302Datas = f91000302Repo.GetDatasByItemTypeId("001");

            var list = new List<GetItemLevelRes>();

            if (_itemLevelCacheList == null)
                _itemLevelCacheList = new List<GetItemLevelRes>();

            // 如果商品編號清單筆數超過1000 則批次1000筆取回資料
            int range = 1000;

            IEnumerable<string> itemCodes = itemCodeList.Distinct();

            int index = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(itemCodes.Count()) / range));

            for (int i = 0; i < index; i++)
            {
                var currItemCodes = itemCodes.Skip(i * range).Take(range).ToList();

                var datas = _itemLevelCacheList.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && currItemCodes.Contains(x.ITEM_CODE)).ToList();

                list.AddRange(datas);

                var existsItemCode = datas.Select(x => x.ITEM_CODE).ToList();

                var noExistsItemCode = currItemCodes.Except(existsItemCode).ToList();

                if (noExistsItemCode.Any())
                {
                    var currData = (from A in f190301Repo.GetDatasByItemCodes(gupCode, custCode, noExistsItemCode)
                                    join B in f91000302Datas
                                    on A.UNIT_ID equals B.ACC_UNIT into subB
                                    from B in subB.DefaultIfEmpty()
                                    select new GetItemLevelRes
                                    {
                                        GUP_CODE = A.GUP_CODE,
                                        CUST_CODE = A.CUST_CODE,
                                        ITEM_CODE = A.ITEM_CODE,
                                        UNIT_LEVEL = A.UNIT_LEVEL,
                                        UNIT_ID = A.UNIT_ID,
                                        UNIT_NAME = B.ACC_UNIT_NAME ?? null,
                                        UNIT_QTY = A.UNIT_QTY,
                                        LENGTH = A.LENGTH,
                                        WIDTH = A.WIDTH,
                                        HIGHT = A.HIGHT,
                                        WEIGHT = A.WEIGHT,
                                        SYS_UNIT = A.SYS_UNIT
                                    }).ToList();

                    _itemLevelCacheList.AddRange(currData);

                    list.AddRange(currData);
                }
            }

            return list;
        }

        /// <summary>
        /// 取得商品單位階層最小單位階層總數量
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="itemCode">品號</param>
        /// <param name="unitLevel">商品階層</param>
        /// <returns></returns>
        public int GetItemUnitLevelTotalQty(string gupCode, string custCode, string itemCode, short unitLevel)
        {
            // [商品階層清單] = &取得商品單位階層資料
            var itemLevelList = GetItemLevelList(gupCode, custCode, new List<string> { itemCode });

            var res = itemLevelList.Where(x => x.UNIT_LEVEL <= unitLevel)
                                   //.OrderByDescending(x => x.UNIT_LEVEL)
                                   .Select(x => x.UNIT_QTY)
                                   .Aggregate((m, n) => m * n);

            return res;
        }

        /// <summary>
        /// 取得商品系統單位階層總數量
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="itemCodes">品號清單</param>
        /// <param name="sysUnit"></param>
        /// <returns></returns>
        public List<ItemCodeQtyModel> GetSysItemUnitQtyList(string gupCode, string custCode, List<string> itemCodes, SysUnit sysUnit)
        {
            List<ItemCodeQtyModel> res = new List<ItemCodeQtyModel>();

            // [商品階層清單] = &取得商品單位階層資料
            var itemLevelList = GetItemLevelList(gupCode, custCode, itemCodes);

            itemCodes.ForEach(currItemCode =>
            {
                // [系統單位階層] = [商品階層清單]中ITEM_CODE = 品號 and SYS_UNIT =  ((int)SysUnit).PadLeft(2, '0') 取第一筆UNIT_LEVEL
                var firstUnitLevel = itemLevelList.Where(x => x.ITEM_CODE == currItemCode && x.SYS_UNIT == Convert.ToString((int)sysUnit).PadLeft(2, '0')).FirstOrDefault();

                if (firstUnitLevel != null)
                {
                    // [系統單位數量] =&取得商品單位階層最小單位階層總數量[<參數1>,<參數2>,品號,[系統單位階層]]
                    int qty = GetItemUnitLevelTotalQty(gupCode, custCode, currItemCode, firstUnitLevel.UNIT_LEVEL);
                    res.Add(new ItemCodeQtyModel { ItemCode = currItemCode, Qty = qty });
                }
            });

            return res;
        }

        /// <summary>
        /// 取得商品單位階層總數量
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="itemCodes">品號清單</param>
        /// <param name="unitName">商品單位名稱</param>
        /// <returns></returns>
        public List<ItemCodeQtyModel> GetItemUnitQtyByUnitNameList(string gupCode, string custCode, List<string> itemCodes, string unitName)
        {
            List<ItemCodeQtyModel> res = new List<ItemCodeQtyModel>();

            // [商品階層清單] = &取得商品單位階層資料
            var itemLevelList = GetItemLevelList(gupCode, custCode, itemCodes);

            itemCodes.ForEach(currItemCode =>
            {
                // [單位階層] = [商品階層清單]中ITEM_CODE = 品號 and UNIT_NAME =  <參數4> 取第一筆UNIT_LEVEL
                var firstUnitLevel = itemLevelList.Where(x => x.ITEM_CODE == currItemCode && x.UNIT_NAME == unitName).FirstOrDefault();

                if (firstUnitLevel != null)
                {
                    // [單位數量] =&取得商品單位階層最小單位階層總數量[<參數1>,<參數2>,品號,[單位階層]]
                    int qty = GetItemUnitLevelTotalQty(gupCode, custCode, currItemCode, firstUnitLevel.UNIT_LEVEL);
                    res.Add(new ItemCodeQtyModel { ItemCode = currItemCode, Qty = qty });
                }
            });

            return res;
        }

        /// <summary>
        /// 取得商品單位階層總數量 (取代原ItemService.GetVolumeUnit 方法，請調整原方法所有參考)
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="itemCodeQtys"></param>
        /// <returns></returns>
        public List<CountItemPackageRefRes> CountItemPackageRefList(string gupCode, string custCode, List<ItemCodeQtyModel> itemCodeQtys)
        {
            List<CountItemPackageRefRes> res = new List<CountItemPackageRefRes>();

            // [商品階層清單] = &取得商品單位階層資料
            var itemLevelList = GetItemLevelList(gupCode, custCode, itemCodeQtys.Select(x => x.ItemCode).Distinct().ToList());

            // Foreach[品號數量] in < 參數3 >
            itemCodeQtys.ForEach(item =>
            {
                // [計算商品數量] =[品號數量].Qty
                int calculateQty = item.Qty;

                // [商品包裝參考] = 空白
                string itemPackageRef = string.Empty;

                // [此商品階層清單] = [商品階層清單] where ITEM_CODE = [品號數量].ITEM_CODE 並降冪排序商品單位階層
                var currItemLevelList = itemLevelList.Where(x => x.ITEM_CODE == item.ItemCode).OrderByDescending(x => x.UNIT_LEVEL).ToList();

                // foreach 商品階層 in [此商品階層清單]
                currItemLevelList.ForEach(subItem =>
                {
                    // [該階層商品最小單位總數量] = &取得商品單位階層最小單位階層總數量[< 參數1 >,< 參數2 >,< 參數3 >, 商品階層.UNIT_LEVEL]
                    int currQty = GetItemUnitLevelTotalQty(gupCode, custCode, subItem.ITEM_CODE, subItem.UNIT_LEVEL);

                    // 如果[該階層商品最小單位總數量] <=[計算商品數量]
                    if (currQty <= calculateQty)
                    {
                        // [該階層數量] =[計算商品數量] 除以[該階層商品最小單位總數量]
                        var currLevelQty = Convert.ToInt32(Math.Floor((decimal)calculateQty / currQty));

                        // [計算商品數量] = [計算商品數量] Mod[該階層商品最小單位總數量]
                        calculateQty = calculateQty % currQty;

                        // [商品包裝參考] += [該階層數量] + 商品階層.UNIT_NAME
                        itemPackageRef += currLevelQty + subItem.UNIT_NAME;
                    }
                    else
                    {
                        // [商品包裝參考] += [計算商品數量] + 商品階層.UNIT_NAME
                        if (calculateQty > currQty)
                            itemPackageRef += calculateQty + subItem.UNIT_NAME;
                    }
                });

                // [商品包裝參考清單].Add({ ItemCode =[品號數量].ItemCode,PackageRef =[商品包裝參考]})
                res.Add(new CountItemPackageRefRes { ItemCode = item.ItemCode, PackageRef = itemPackageRef });

            });

            return res;
        }

        /// <summary>
        /// 計算商品整數箱與零散數 (取代原本 GetBoxQty 並調整所有參考)
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="itemCodeQtys"></param>
        /// <returns></returns>
        public List<CountItemFullAndBulkCaseQtyRes> CountItemFullAndBulkCaseQty(string gupCode, string custCode, List<ItemCodeQtyModel> itemCodeQtys)
        {
            List<CountItemFullAndBulkCaseQtyRes> res = new List<CountItemFullAndBulkCaseQtyRes>();

            // [各商品箱入數] = &取得商品系統單位階層總數量[< 參數1 >,< 參數2 >,< 參數3 >.所有品號, SysUnit.Case]
            var itemBoxCntList = GetSysItemUnitQtyList(gupCode, custCode, itemCodeQtys.Select(x => x.ItemCode).Distinct().ToList(), SysUnit.Case);

            // Foreach[品號數量] in < 參數3 >
            itemCodeQtys.ForEach(item =>
            {
                // [商品箱入數] = [各商品箱入數] where ItemCode = [品號數量].ItemCode
                var currItemBoxCnt = itemBoxCntList.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();

                if (currItemBoxCnt != null)
                {
                    // [整數箱] = [品號數量].Qty 除以[商品箱入數]
                    int fullCaseQty = Convert.ToInt32(Math.Floor((decimal)item.Qty / currItemBoxCnt.Qty));

                    // [零散數] = [品號數量].Qty  MOD[商品箱入數]
                    int bulkCaseQty = item.Qty % currItemBoxCnt.Qty;

                    // [商品整數箱、零散數清單].add({ ItemCode =[品號數量].ItemCode, FullCaseQty =[整數箱], BulkCaseQty =[零散數]})
                    res.Add(new CountItemFullAndBulkCaseQtyRes { ItemCode = item.ItemCode, FullCaseQty = fullCaseQty, BulkCaseQty = bulkCaseQty });
                }
            });

            return res;
        }

        /// <summary>
        /// 取得商品材積 (取代原GetVolume)
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="itemCode">品號</param>
        /// <param name="cuftFactor">整數箱材積係數</param>
        /// <returns></returns>
        public decimal GetVolume(string gupCode, string custCode, string itemCode, long cuftFactor)
        {
            // [箱單位階層] = &取得商品單位階層資料[< 參數1 >,< 參數2 >,< 參數3 >] where SYS_UNIT = ((int)SysUnit.Case).PadLeft(2, '0') 取第一筆 
            var firstItemLevel = GetItemLevelList(gupCode, custCode, new List<string> { itemCode }).Where(x => x.SYS_UNIT == Convert.ToString((int)SysUnit.Case).PadLeft(2, '0')).FirstOrDefault();

            if (firstItemLevel == null || cuftFactor <= 0)
            {
                // IF[箱單位階層] = NULL 或<參數4> <= 0 回傳 0
                return 0;
            }
            else
            {
                // ELSE 回傳[箱單位階層].LENGTH * [箱單位階層].WIDTH * [箱單位階層].HEIGHT 除以<參數4>
                return Math.Ceiling(((firstItemLevel.LENGTH ?? 0) * (firstItemLevel.WIDTH ?? 0) * (firstItemLevel.HIGHT ?? 0)) / (decimal)cuftFactor);
            }
        }

        /// <summary>
        /// 取得允收天數和警示天數
        /// </summary>
        /// <param name="saveDay">總保存天數</param>
        /// <returns></returns>
        public GetItemAllDlnAndAllShpRes GetItemAllDlnAndAllShp(int? saveDay)
        {
            GetItemAllDlnAndAllShpRes res = new GetItemAllDlnAndAllShpRes();
            if (saveDay > 30 && saveDay <= 90)
            {
                res = new GetItemAllDlnAndAllShpRes
                {
                    ALL_DLN = saveDay.Value * 2 / 3,
                    ALL_SHP = saveDay.Value * 1 / 3
                };
            }
            if (saveDay > 90 && saveDay <= 365)
            {
                res = new GetItemAllDlnAndAllShpRes
                {
                    ALL_DLN = saveDay.Value * 2 / 5,
                    ALL_SHP = saveDay.Value * 1 / 4
                };
            }
            if (saveDay > 365)
            {
                res = new GetItemAllDlnAndAllShpRes
                {
                    ALL_DLN = saveDay.Value * 1 / 3,
                    ALL_SHP = saveDay.Value * 1 / 5
                };
            }
            return res;
        }

		#region  使用商品條碼取得符合的商品清單共用方法
		
		public List<string> FindItems(string gupCode,string custCode,string barcode,ref F2501 f2501)
		{
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			var f1903s = f1903Repo.GetDatasByBarCode(gupCode, custCode, barcode).ToList();
			if (f1903s.Any())
				return f1903s.Select(x=> x.ITEM_CODE).ToList();

			var f2501Repo = new F2501Repository(Schemas.CoreSchema);
			f2501 = f2501Repo.GetDataByBarCode(gupCode, custCode, barcode);
			if (f2501 != null)
			{
				return new List<string> { f2501.ITEM_CODE };
			}
			return new List<string>();
		}


		#endregion
	}
}
