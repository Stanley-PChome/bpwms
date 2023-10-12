using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Common.Enums;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F25;
using System.Data.Objects;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.F16;
using System.Diagnostics;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F06;

namespace Wms3pl.WebServices.Process.P08.Services
{
	public partial class P080701Service
	{
		#region Property
		private WmsTransaction _wmsTransaction;

		private F055002Repository _f055002Repo = null;
		public F055002Repository F055002Repo
		{
			get
			{
				if (_f055002Repo == null)
					_f055002Repo = new F055002Repository(Schemas.CoreSchema, _wmsTransaction);

				return _f055002Repo;
			}
		}


		private F2501Repository _f2501Repo = null;
		public F2501Repository F2501Repo
		{
			get
			{
				if (_f2501Repo == null)
					_f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);

				return _f2501Repo;
			}
		}

    private ShipPackageService _shipPackageService = null;
    public ShipPackageService ShipPackageService
    {
      get
      {
        if (_shipPackageService == null)
          _shipPackageService = new ShipPackageService(_wmsTransaction);
        return _shipPackageService;
      }
    }
    #endregion

    public P080701Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}


		#region 刷讀條碼尋找出貨單
		/// <summary>
		/// 刷讀條碼尋找出貨單
		/// </summary>
		/// <param name="dcCode">物流中心編號</param>
		/// <param name="gupCode">業主編號</param>
		/// <param name="custCode">貨主編號</param>
		/// <param name="scanCode">刷讀條碼</param>
		/// <returns>出貨單 or Null</returns>
		public F050801 SearchWmsOrder(string dcCode, string gupCode, string custCode, string scanCode)
		{
			F050801 f050801 = null;
			if (scanCode.ToUpper().StartsWith("O")) //刷讀出貨單號/出貨貼紙
			{
				//找出貨單號
				f050801 = GetWmsOrder(dcCode, gupCode, custCode, scanCode);
				if (f050801 != null)
					return f050801;

				//找出貨貼紙
				var f050804Repo = new F050804Repository(Schemas.CoreSchema);
				var f050804 = f050804Repo.GetF050804(dcCode, gupCode, custCode, scanCode);
				if (f050804 != null)
					return GetWmsOrder(f050804.DC_CODE, f050804.GUP_CODE, f050804.CUST_CODE, f050804.WMS_ORD_NO);

			}
			else //刷讀揀貨箱號/合流箱號
			{
				// 找容器編號的流程
				var f070101Repo = new F070101Repository(Schemas.CoreSchema);

        var f070101 = f070101Repo.GetDataWithF0701(dcCode, gupCode, custCode, scanCode);
        if (f070101 != null)
          return GetWmsOrder(f070101.DC_CODE, f070101.GUP_CODE, f070101.CUST_CODE, f070101.WMS_NO);

      }

      return f050801;
		}

        /// <summary>
        /// 取得出貨單&檢查訂單，如果訂單狀態為取消，STATUS回傳-9999
        /// </summary>
        /// <param name="dcCode">物流中心編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="wmsOrdNo">出貨單號</param>
        /// <returns></returns>
        private F050801 GetWmsOrder(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            //找出貨單號
            var f050801Repo = new F050801Repository(Schemas.CoreSchema);
            var f050801 = f050801Repo.GetData(wmsOrdNo, gupCode, custCode, dcCode);


			var f050101Repo = new F050101Repository(Schemas.CoreSchema);
			var f050101s = f050101Repo.GetOrdNoByWmsOrdNo(dcCode, gupCode, custCode, wmsOrdNo);

			if (f050101s.Any(x => x.STATUS == "9"))
				f050801.STATUS = -9999;
			return f050801;
		}

    #endregion

    #region 刷讀 商品條碼/商品序號/紙箱條碼/NEWBOX/刷讀Log

    /// <summary>
    /// 刷讀 商品條碼/商品序號/紙箱條碼/NEWBOX
    /// </summary>
    /// <param name="packgeCode">刷讀包裝資訊</param>
    /// <returns></returns>
    public ScanPackageCodeResult ScanPackageCode(PackgeCode packgeCode)
    {
      // 取得最後一次包裝頭檔資訊
      var f055001 = GetMyLastF055001(packgeCode.F050801Item);

      // 檢查上一箱是否為原箱商品且未關箱 - 需提示使用者手動關箱 且來源單據非廠退出貨
      if (packgeCode.F050801Item.ALLOWORDITEM == "1" && f055001 != null && f055001.PRINT_FLAG == 0 && packgeCode.F050801Item.SOURCE_TYPE != "13")
      {
        var f055002Repo = new F055002Repository(Schemas.CoreSchema);
        var f055002 = F055002Repo.GetDatasByTrueAndCondition(
          x => x.DC_CODE == f055001.DC_CODE && x.GUP_CODE == f055001.GUP_CODE && x.CUST_CODE == f055001.CUST_CODE &&
          x.WMS_ORD_NO == f055001.WMS_ORD_NO && x.PACKAGE_BOX_NO == f055001.PACKAGE_BOX_NO).FirstOrDefault();
        if (f055002 != null)
        {
          var item = packgeCode.F1903s.FirstOrDefault(x => x.ITEM_CODE == f055002.ITEM_CODE);
          if (item != null && item.ALLOWORDITEM == "1")
            return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_PreBoxHasOrginalItemNotClose);
        }
      }

      // 是否還有原箱商品尚未刷讀完成
      var hasOrginalItemNotPackFinish = HasOrginalItemNotPack(packgeCode);

      // 檢查 刷讀條碼
      var check = CheckScanPackageCode(packgeCode, f055001, hasOrginalItemNotPackFinish);
      // 檢核不通過 或 紙箱 直接回傳 不往下做
      if (!check.IsPass || check.IsCarton || check.IsFinishCurrentBox)
        return check;

      // 取得商品
      var f1903s = packgeCode.F1903s.Where(
        x => x.ITEM_CODE == packgeCode.InputCode ||
            (!string.IsNullOrEmpty(x.EAN_CODE1) && x.EAN_CODE1 == packgeCode.InputCode) ||
            (!string.IsNullOrEmpty(x.EAN_CODE2) && x.EAN_CODE2 == packgeCode.InputCode) ||
            (!string.IsNullOrEmpty(x.EAN_CODE3) && x.EAN_CODE3 == packgeCode.InputCode)).ToList();

      if (!f1903s.Any())
      {
        var f1903Repo = new F1903Repository(Schemas.CoreSchema);
        f1903s = f1903Repo.GetItemByCondition(packgeCode.F050801Item.GUP_CODE, packgeCode.F050801Item.CUST_CODE, packgeCode.InputCode).ToList();
      }

      if (f1903s.Any())
        return ScanItem(packgeCode, f1903s, f055001, hasOrginalItemNotPackFinish);
      else
        // 刷讀序號時, 將序號寫入F055002, 並更新序號狀態為C1
        return ScanSerial(packgeCode, f055001, hasOrginalItemNotPackFinish);

    }



    #region 加箱(NEWBOX)
    /// <summary>
    /// 建立新的箱子，邏輯按原本的改寫搬到 Service, 另外加入建立新箱子時，將 F050901 託運單號帶入
    /// </summary>
    /// <param name="f050801"></param>
    /// <param name="boxNo"></param>
    /// <param name="f055001"></param>
    /// <returns></returns>
    private F055001 NewBox(F050801 f050801, string boxNo, F055001 f055001, ref string errorMessage)
		{
			var f055001Repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);

			if (f055001 == null)
			{
				// 取得新箱子的箱號
				var newPackageBoxNo = f055001Repo.GetNewPackageBoxNo(f050801.WMS_ORD_NO, f050801.GUP_CODE, f050801.CUST_CODE, f050801.DC_CODE);

				var consignNo = string.Empty;
				// 需要列印托單，才取托單號
				if (f050801.PRINT_PASS == "1")
				{
					if (newPackageBoxNo == 1)
					{

						// 派庫時，就已經建立一張託運單，直接從F050901取得即可，
						consignNo = GetFirstConsignNo(f050801.WMS_ORD_NO, f050801.GUP_CODE, f050801.CUST_CODE, f050801.DC_CODE);

					}
					else
					{

						// 當相同出貨單要建立第二個箱子後，都要重新建立並取得託運單號
						var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
						var f700102Repo = new F700102Repository(Schemas.CoreSchema, _wmsTransaction);
						var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
						var consignService = new ConsignService(_wmsTransaction);
						var sharedService = new SharedService(_wmsTransaction);
						var allId = f050801Repo.GetAllIdByWmsOrdNo(f050801.WMS_ORD_NO, f050801.GUP_CODE, f050801.CUST_CODE, f050801.DC_CODE);
						if (!string.IsNullOrEmpty(allId))
						{
							var f700102 = f700102Repo.GetDataByWmsNo(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, f050801.WMS_ORD_NO);
							var ordAddress = new OrdAddress
							{
								WmsNo = f050801.WMS_ORD_NO,
								ZipCode = f050801.ZIP_CODE,
								Address = f700102.ADDRESS,
								DelvTimes = f700102.DELV_TIMES,
								DistrUse = "01" // 送件:01
							};
							consignService.CreateConsignForF055001(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, allId, ordAddress, newPackageBoxNo, ref errorMessage);
							if (ordAddress.AddBoxGetConsignNo == "0")
								f050901Repo.Add(ordAddress.F050901, "CONSIGN_ID");
							if (ordAddress.AddBoxGetConsignNo == "1")
								f050901Repo.Update(ordAddress.F050901);

							consignNo = ordAddress.F050901.CONSIGN_NO;
						}

					}
				}

				f055001 = new F055001()
				{
					WMS_ORD_NO = f050801.WMS_ORD_NO,
					PACKAGE_BOX_NO = newPackageBoxNo,
					CUST_CODE = f050801.CUST_CODE,
					GUP_CODE = f050801.GUP_CODE,
					DC_CODE = f050801.DC_CODE,
					DELV_DATE = f050801.DELV_DATE,
					PICK_TIME = f050801.PICK_TIME,
					PACKAGE_NAME = Current.StaffName,
					PACKAGE_STAFF = Current.Staff,
					BOX_NUM = boxNo,
          ORG_BOX_NUM = boxNo,
          STATUS = "0",
					PAST_NO = consignNo,
          IS_ORIBOX = boxNo == "ORI" ? "1" : "0"
        };

				f055001Repo.Add(f055001);
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(boxNo) && f055001.BOX_NUM != boxNo)
				{
					f055001.BOX_NUM = boxNo;
					f055001Repo.Update(f055001);
				}
			}

			return f055001;
		}

		/// <summary>
		/// 如果為出貨單第一個箱子，才可以取得在派庫時，就已經產生的託運單號
		/// </summary>
		/// <param name="wmsOrdNo"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <returns></returns>
		private string GetFirstConsignNo(string wmsOrdNo, string gupCode, string custCode, string dcCode)
		{
			var f050901Repo = new F050901Repository(Schemas.CoreSchema);
			var consignNo = f050901Repo.Filter(x => x.WMS_NO == EntityFunctions.AsNonUnicode(wmsOrdNo)
																					&& x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
																					&& x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
																					&& x.DC_CODE == EntityFunctions.AsNonUnicode(dcCode))
																	.OrderBy(x => x.CONSIGN_ID)
																	.Select(x => x.CONSIGN_NO)
																	.FirstOrDefault();
			return consignNo;
		}


		#endregion

		#region 刷讀品號相關方法

		/// <summary>
		/// 刷讀條碼 = 品號條碼
		/// </summary>
		/// <param name="packgeCode">刷讀包裝資訊</param>
		/// <param name="f1903">商品資訊</param>
		/// <param name="f055001">取得最後一次包裝頭檔資訊</param>
		/// <param name="hasOrginalItemNotPackFinish">是否還有原箱商品尚未刷讀完成</param>
		/// <returns></returns>
		private ScanPackageCodeResult ScanItem(PackgeCode packgeCode, List<F1903> f1903s, F055001 f055001, bool hasOrginalItemNotPackFinish)
		{
			var f055002Repo = new F055002Repository(Schemas.CoreSchema);
			var f05030202Repo = new F05030202Repository(Schemas.CoreSchema);

			var f055002s = F055002Repo.GetDatasByTrueAndCondition(x =>
			x.DC_CODE == packgeCode.F050801Item.DC_CODE &&
			x.GUP_CODE == packgeCode.F050801Item.GUP_CODE &&
			x.CUST_CODE == packgeCode.F050801Item.CUST_CODE &&
			x.WMS_ORD_NO == packgeCode.F050801Item.WMS_ORD_NO).ToList();

			var f05030202s = f05030202Repo.GetDatasByTrueAndCondition(x =>
			x.DC_CODE == packgeCode.F050801Item.DC_CODE &&
			x.GUP_CODE == packgeCode.F050801Item.GUP_CODE &&
			x.CUST_CODE == packgeCode.F050801Item.CUST_CODE &&
			x.WMS_ORD_NO == packgeCode.F050801Item.WMS_ORD_NO).ToList();

			var data = (from A in packgeCode.F050802s
									join B in f05030202s
									on A.WMS_ORD_SEQ equals B.WMS_ORD_SEQ
									join C in f1903s
									on A.ITEM_CODE equals C.ITEM_CODE
									join D in f055002s
									on B.ORD_SEQ equals D.ORD_SEQ into subD
									from D in subD.DefaultIfEmpty()
									select new
									{
										F050802 = A,
										F1903 = C,
										F055002s = D
									}).GroupBy(x => new { x.F050802, x.F1903 })
									.Select(x => new
									{
										x.Key.F050802,
										ItemType = x.Key.F1903.BUNDLE_SERIALLOC == "1" ? "2" : (x.Key.F1903.BUNDLE_SERIALNO == "1" ? "1" : "0"),
										PageQty = x.Where(z => z.F055002s != null).Sum(z => z.F055002s.PACKAGE_QTY)
									}).Where(x => x.F050802.B_DELV_QTY != x.PageQty)
									.OrderBy(x => x.F050802.ITEM_CODE)
									.ThenBy(x => x.ItemType).ToList();

			F1903 f1903 = null;

			if (data.Any())
			{
				foreach (var item in data)
				{
					if (item.PageQty + packgeCode.AddQty <= item.F050802.B_DELV_QTY)
					{
						packgeCode.InputCode = item.F050802.ITEM_CODE;
						f1903 = packgeCode.F1903s.Where(x => x.ITEM_CODE == packgeCode.InputCode).FirstOrDefault();
						break;
					}
				}

				if (f1903 == null)
				{
					packgeCode.InputCode = data.First().F050802.ITEM_CODE;
					f1903 = packgeCode.F1903s.Where(x => x.ITEM_CODE == packgeCode.InputCode).FirstOrDefault();
				}
			}
			else
			{
				f1903 = f1903s.FirstOrDefault();
				packgeCode.InputCode = f1903.ITEM_CODE;
			}

			if (f1903.ALLOWORDITEM == "0" && hasOrginalItemNotPackFinish)
				// 出貨明細中含有原箱商品，請刷讀原箱品號/序號/箱號
				return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_ScanOrginalItem) { ItemCode = packgeCode.InputCode };

			// 判斷商品是否為序號商品
			if (f1903.BUNDLE_SERIALNO == "1")
				// 必須刷讀序號
				return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_SerialNoScan) { ItemCode = packgeCode.InputCode };

			// 判斷商品有沒有在此次出貨裡
			if (!IsInWmsOrder(packgeCode, packgeCode.InputCode))
				return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_F051202QueryNotExist) { ItemCode = packgeCode.InputCode };

			// 判斷商品是否為原箱 如果是原箱商品 數量必須為1
			if (f1903.ALLOWORDITEM == "1" && packgeCode.AddQty > 1)
				return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_OrginalItemQtyMustOne);

			// 如果商品為原箱商品 則 包裝箱號固定為ORI
			if (f1903.ALLOWORDITEM == "1")
				packgeCode.BoxNum = "ORI";

			// 已經關箱的，就只能刷箱號
			if (f055001 != null && f055001.PRINT_FLAG == 1)
				f055001 = null;

			var errorMessage = string.Empty;
			// 無包裝頭檔紀錄或上一箱已關箱 若有輸入箱號，則開箱
			if (f055001 == null && !string.IsNullOrEmpty(packgeCode.BoxNum))
				f055001 = NewBox(packgeCode.F050801Item, packgeCode.BoxNum, f055001, ref errorMessage);

			if (!string.IsNullOrWhiteSpace(errorMessage))
				return new ScanPackageCodeResult(false, errorMessage);

			// 檢核必須要有出貨包裝頭檔，這行有問題，表示上面檢核邏輯有問題
			if (f055001 == null)
				return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_ScanBoxNo);

			// 每次回傳都記錄箱號
			packgeCode.BoxNum = f055001.BOX_NUM;
			// 包裝箱號只為了記錄LOG用
			packgeCode.PackageBoxNo = f055001.PACKAGE_BOX_NO;

			var deliveryData = GetQuantityOfDeliveryInfo(packgeCode.F050801Item, packgeCode.InputCode);
			// 品號的部分可直接修改數量，但數量必須要大於目前已刷讀的量
			var addQty = packgeCode.AddQty;
			if (addQty > 1)
			{
				if (addQty <= deliveryData.TotalPackQty)
					return new ScanPackageCodeResult(false, string.Format(Properties.Resources.P080701Service_TotalPackQtyError, addQty, deliveryData.TotalPackQty));

				addQty -= deliveryData.TotalPackQty;
			}


			var result = InsertOrUpdateF055002Qty(packgeCode.F050801Item, f055001, f1903, deliveryData, packgeCode.InputCode, addQty);
			// 如果商品為原箱商品，則需設定為關箱
			if (f1903.ALLOWORDITEM == "1")
			{
				result.IsFinishCurrentBox = true;
				result.Message = f1903.ALLOWORDITEM == "1" ? Properties.Resources.P080701Service_OrginalItem : string.Empty;
			}
			return result;

		}


		private ScanPackageCodeResult GetCartonResult(F050801 f050801, string inputCode)
		{
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);

			var cartonItem = f1903Repo.GetCartonItem(f050801.GUP_CODE, f050801.CUST_CODE, inputCode).FirstOrDefault();
			if (cartonItem != null)
			{
				return new ScanPackageCodeResult(true, Properties.Resources.P080701Service_BoxNoExist)
				{
					IsCarton = true,
					ItemCode = cartonItem.ITEM_CODE,
					BoxNum = cartonItem.ITEM_CODE,
          ScanCode = inputCode
				};
			}

			return new ScanPackageCodeResult();
		}

        #endregion

        #region 刷讀序號相關方法

        /// <summary>
        /// 刷讀條碼 = 序號/組合商品
        /// </summary>
        /// <param name="packgeCode">刷讀包裝資訊</param>
        /// <param name="f055001">取得最後一次包裝頭檔資訊</param>
        /// <param name="hasOrginalItemNotPackFinish">是否還有原箱商品尚未刷讀完成</param>
        /// <returns></returns>
        private ScanPackageCodeResult ScanSerial(PackgeCode packgeCode, F055001 f055001, bool hasOrginalItemNotPackFinish)
        {
            var f050801 = packgeCode.F050801Item;
            var itemService = new ItemService();

            F2501 f2501 = null;

            //取得序號/組合商品 的商品品號
            var serialItem = itemService.FindItems(f050801.GUP_CODE, f050801.CUST_CODE, packgeCode.InputCode, ref f2501).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(serialItem))
                return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_ItemNotFound) { SerialNo = packgeCode.InputCode };

            // 檢查序號是否存在
            if (f2501 == null)
                return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_SerialNoIsExist) { SerialNo = packgeCode.InputCode };

            // 檢查序號 的商品品號 是否為此出貨單商品
            if (!IsInWmsOrder(packgeCode, serialItem))
                return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_F051202QueryNotExist) { SerialNo = packgeCode.InputCode };

            var f1903 = packgeCode.F1903s.FirstOrDefault(x => x.GUP_CODE == f050801.GUP_CODE && x.CUST_CODE == f050801.CUST_CODE && x.ITEM_CODE == (serialItem));
            // 檢查商品是否為此貨主商品
            if (f1903 == null)
                return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_CustItemNotExist) { SerialNo = packgeCode.InputCode };

            // 刷讀的商品非原箱 但還有原箱商品未刷讀 必須先刷讀原箱商品
            if (f1903.ALLOWORDITEM == "0" && hasOrginalItemNotPackFinish)
                // 出貨明細中含有原箱商品，請刷讀原箱品號/序號/箱號
                return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_ScanOrginalItem) { SerialNo = packgeCode.InputCode };

            // 判斷商品是否為原箱 如果是原箱商品 數量必須為1
            if (f1903.ALLOWORDITEM == "1" && packgeCode.AddQty > 1)
                return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_OrginalItemQtyMustOne);

            // 判斷商品是否為序號商品 如果是序號商品 包裝數必須為1
            if (f1903.BUNDLE_SERIALNO == "1" && packgeCode.AddQty > 1)
                return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_AddQtyError);

            var result = new ScanPackageCodeResult();

            // 如果商品為原箱商品 則 包裝箱號固定為ORI
            if (f1903.ALLOWORDITEM == "1")
                packgeCode.BoxNum = "ORI";

            // 已經關箱的，就只能刷箱號
            if (f055001 != null && f055001.PRINT_FLAG == 1)
                f055001 = null;

            var errorMessage = string.Empty;
            // 無包裝頭檔紀錄或上一箱已關箱 若有輸入箱號，則開箱
            if (f055001 == null && !string.IsNullOrEmpty(packgeCode.BoxNum))
                f055001 = NewBox(packgeCode.F050801Item, packgeCode.BoxNum, f055001, ref errorMessage);

            if (!string.IsNullOrWhiteSpace(errorMessage))
                return new ScanPackageCodeResult(false, errorMessage);

            // 檢核必須要有出貨包裝頭檔，這行有問題，表示上面檢核邏輯有問題
            if (f055001 == null)
                return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_ScanBoxNo);

            // 每次回傳都記錄箱號
            packgeCode.BoxNum = f055001.BOX_NUM;
            // 包裝箱號只為了記錄LOG用
            packgeCode.PackageBoxNo = f055001.PACKAGE_BOX_NO;

            DeliveryData deliveryData = null;

                result.ItemCode = f2501.ITEM_CODE;
                result.SerialNo = f2501.SERIAL_NO;
                // 紀錄刷讀序號狀態
                result.Status = f2501.STATUS;

                // 判斷商品有沒有在此次出貨裡
                if (!IsInWmsOrder(packgeCode, f2501.ITEM_CODE))
                {
                    result.Message = Properties.Resources.P080701Service_F051202QueryNotExist;
                    return result;
                }

                // 判斷是不是合法的序號
                if (f2501.STATUS != "A1")
                    return new ScanPackageCodeResult(false, Properties.Resources.P080804Service_SerialIsNotInWarehouse);

                // No.2091 若為不良品序號(F2501.ACTIVATED=1) & 為客戶訂單(F050801.SOURCE_TYPE is NULL 或空白)，不可出貨
                if (f2501.ACTIVATED == "1" && string.IsNullOrWhiteSpace(f050801.SOURCE_TYPE))
                    return new ScanPackageCodeResult(false, Properties.Resources.P080804Service_SerialIsActived, f2501.SERIAL_NO);

                var serialF1903 = packgeCode.F1903s.FirstOrDefault(o => o.GUP_CODE == f050801.GUP_CODE && o.CUST_CODE == f050801.CUST_CODE && o.ITEM_CODE == f2501.ITEM_CODE);

                if (serialF1903 == null)
                    return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_CustItemNotExist);

                if (serialF1903.BUNDLE_SERIALLOC == "1" && !packgeCode.F050802s.Any(o => o.SERIAL_NO == f2501.SERIAL_NO))
                    return new ScanPackageCodeResult(false, string.Format(Properties.Resources.P080701Service_ItemSerialError, f2501.SERIAL_NO));


                // 非組合當有多個序號時，只撈一次出貨數量資訊，用於差異數檢查。組合商品的話上面已經檢查過了，就不用在檢查組合的物料
                if (deliveryData == null)
                    deliveryData = GetQuantityOfDeliveryInfo(f050801, f2501.ITEM_CODE);

                // 重複刷讀檢核
                if (IsRepeatSerialNo(f055001, f2501.SERIAL_NO))
                {
                    return new ScanPackageCodeResult
                    {
                        ItemCode = f2501.ITEM_CODE,
                        SerialNo = f2501.SERIAL_NO,
                        Status = f2501.STATUS,
                        Message = Properties.Resources.P080201Service_SerialNoScanRepeat
                    }; ;
                }

                result = InsertOrUpdateF055002Qty(f050801, f055001, serialF1903, deliveryData, f2501.ITEM_CODE, 1, f2501.SERIAL_NO, f2501.STATUS);

                if (!result.IsPass)
                    return result;

            // 紀錄F2501原WMS_NO
            var orgWmsNo = f2501.WMS_NO;

                SetF2501ScanInfo(f050801, packgeCode, f2501);
                F2501Repo.Update(f2501);

            // 刷讀成功，有門號就將門號放在訊息中
            return new ScanPackageCodeResult(true, packgeCode.CellNum)
            {
                SerialNo = packgeCode.InputCode,
                ItemCode = serialItem,
                Status = result.Status,
                // 如果商品為原箱商品，則需設定為關箱
                IsFinishCurrentBox = f1903.ALLOWORDITEM == "1" ? true : false,
                Message = f1903.ALLOWORDITEM == "1" ? Properties.Resources.P080701Service_OrginalItem : string.Empty,
                OrgWmsNo = orgWmsNo
            };
        }

        /// <summary>
        /// 處理組合商品的原料序號檢查
        /// </summary>
        /// <param name="serialNoService"></param>
        /// <param name="packgeCode"></param>
        /// <param name="serialNoList"></param>
        /// <returns></returns>
        private ScanPackageCodeResult HandleCombinItem(SerialNoService serialNoService, PackgeCode packgeCode, List<string> serialNoList)
		{
			var f050801 = packgeCode.F050801Item;

			foreach (var serialNo in serialNoList)
			{
				var f2501 = F2501Repo.Find(x => x.GUP_CODE == packgeCode.F050801Item.GUP_CODE && x.CUST_CODE == packgeCode.F050801Item.CUST_CODE && x.SERIAL_NO == serialNo);
				if (f2501 == null)
					return new ScanPackageCodeResult(false, string.Format(Properties.Resources.P080701Service_F2501IsNull, serialNo)) { SerialNo = packgeCode.InputCode };

				// 判斷是不是合法的序號
				var serialResult = serialNoService.SerialNoStatusCheck(packgeCode.F050801Item.GUP_CODE, packgeCode.F050801Item.CUST_CODE, serialNo, "C1");
				if (!serialResult.Checked)
				{
					return new ScanPackageCodeResult
					{
						ItemCode = f2501.ITEM_CODE,
						SerialNo = f2501.SERIAL_NO,
						Status = f2501.STATUS,
						Message = serialResult.Message
					};
				}

				SetF2501ScanInfo(f050801, packgeCode, f2501);
				F2501Repo.Update(f2501);
			}

			return new ScanPackageCodeResult { IsPass = true };
		}

		private static void SetF2501ScanInfo(F050801 f050801, PackgeCode packgeCode, F2501 f2501)
		{
			//銷毀的出貨單,序號狀態更新為報廢D2
			//廠退的出貨單,序號狀態更新為廠退C1
			//借出/外送的出貨單,序號狀態更新為報廢C1
			//其它  C1
			string sourceType = f050801.SOURCE_TYPE;

			f2501.ORD_PROP = f050801.ORD_PROP;
			f2501.RETAIL_CODE = packgeCode.F050301s.Select(x => x.RETAIL_CODE).FirstOrDefault();
			f2501.WMS_NO = f050801.WMS_ORD_NO;
			f2501.STATUS = sourceType == "08" ? "D2"  //銷毀
											: (sourceType == "02" ? "C1" //廠退
											: (sourceType == "05" || sourceType == "06" ? "C1"  //借出/外送
											: "C1"));

			// 商品類別是易通卡，無條件Update門號欄位，不正確則要求重新刷讀序號。
			var itemCode = (f2501.BOUNDLE_ITEM_CODE ?? f2501.ITEM_CODE);
			if (packgeCode.Type == "05" || packgeCode.F1903s.Any(x => x.ITEM_CODE == itemCode && x.TYPE == "05"))
			{
				if (!string.IsNullOrEmpty(packgeCode.CellNum))
					f2501.CELL_NUM = packgeCode.CellNum;
			}
		}

		/// <summary>
		/// 檢查出貨量，每次檢查也會更新 deliveryData 的刷讀量，在記憶體中暫存計算數量
		/// </summary>
		/// <param name="f1903"></param>
		/// <param name="deliveryData"></param>
		/// <param name="itemCode"></param>
		/// <param name="addQty"></param>
		/// <param name="serialNo"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		private ScanPackageCodeResult CheckDeliveryQty(F1903 f1903, DeliveryData deliveryData, string itemCode, int addQty, string serialNo, string status)
		{
			var result = new ScanPackageCodeResult
			{
				ItemCode = itemCode,
				SerialNo = serialNo,
				Status = status
			};

			// 判斷要增加的量是否超過出貨量
			if (deliveryData == null)
			{
				result.Message = Properties.Resources.P080701Service_DeliveryDataIsNull;
				return result;
			}


			if (deliveryData.DiffQty - addQty < 0)
			{
				result.Message = Properties.Resources.P080701Service_AddQtyOver;
				return result;
			}

			// 當刷盒號時，有可能會會有多個序號，故要同步已出貨數量計算
			deliveryData.DiffQty -= addQty;
			deliveryData.TotalPackQty += addQty;


			result = new ScanPackageCodeResult
			{
				IsPass = true,
				SPILL = f1903.SPILL,
				FRAGILE = f1903.FRAGILE,
				ItemCode = itemCode,
				SerialNo = serialNo,
				Status = status,
			};

			return result;
		}

		/// <summary>
		/// 是否重複序號在出貨包裝裡
		/// </summary>
		/// <param name="f055001"></param>
		/// <param name="f055002Repo"></param>
		/// <param name="serialNo"></param>
		/// <returns></returns>
		private bool IsRepeatSerialNo(F055001 f055001, string serialNo)
		{

			return F055002Repo.GetDatasByTrueAndCondition(x => x.WMS_ORD_NO == EntityFunctions.AsNonUnicode(f055001.WMS_ORD_NO)
										&& x.DC_CODE == EntityFunctions.AsNonUnicode(f055001.DC_CODE)
										&& x.GUP_CODE == EntityFunctions.AsNonUnicode(f055001.GUP_CODE)
										&& x.CUST_CODE == EntityFunctions.AsNonUnicode(f055001.CUST_CODE)
										&& x.SERIAL_NO == EntityFunctions.AsNonUnicode(serialNo))
								.Any();
		}


		#endregion

		#region 刷讀共用方法

		/// <summary>
		/// 檢查 刷讀條碼
		/// </summary>
		/// <param name="packgeCode">刷讀包裝資訊</param>
		/// <param name="f055001">取得最後一次包裝頭檔資訊</param>
		/// <param name="hasOrginalItemNotPack">是否還有原箱商品尚未刷讀完成</param>
		/// <returns></returns>
		private ScanPackageCodeResult CheckScanPackageCode(PackgeCode packgeCode, F055001 f055001, bool hasOrginalItemNotPackFinish)
		{
			var f050801 = packgeCode.F050801Item;

			// 檢核刷讀條碼未輸入或空白
			if (string.IsNullOrWhiteSpace(packgeCode.InputCode))
			{
				// 出貨單含有原箱，且原箱商品尚未刷讀
				if (packgeCode.F050801Item.ALLOWORDITEM == "1" && hasOrginalItemNotPackFinish)
					// 出貨明細中含有原箱商品，請刷讀原箱品號/序號/箱號
					return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_ScanOrginalItem);
				else if (f055001 == null || f055001.PRINT_FLAG == 1)
					// 請刷讀箱號
					return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_ScanBoxNo);
				else
					// 請刷讀品號/序號/盒號/箱號
					return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_ScanItemSerialBoxNo);
			}
			else
			{
				// 檢查刷讀條碼為加箱條碼
				if (packgeCode.InputCode.Equals("NEWBOX", StringComparison.OrdinalIgnoreCase))
				{
					// 出貨單含有原箱，且原箱商品尚未刷讀
					if (packgeCode.F050801Item.ALLOWORDITEM == "1" && hasOrginalItemNotPackFinish)
						// 出貨明細中含有原箱商品，請刷讀原箱品號/序號/箱號
						return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_ScanOrginalItem) { ItemCode = packgeCode.InputCode };
					else
					{
						// 自取加箱無須取託運單
						if (f050801.SELF_TAKE != "1")
						{
							var f194704Repo = new F194704Repository(Schemas.CoreSchema);
							var f194704 = f194704Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == f050801.DC_CODE && x.GUP_CODE == f050801.GUP_CODE && x.CUST_CODE == f050801.CUST_CODE && x.ALL_ID == f050801.ALL_ID).FirstOrDefault();
							//非超取訂單 且取號方式為外部取號時不可加箱
							if (f050801.CVS_TAKE == "0" && f194704 != null && f194704.GET_CONSIGN_NO == "3")
								return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_CvsTake0);

							if (f050801.CVS_TAKE == "1")
								return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_CvsTake1);
						}
						if (f055001 == null)
							return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_ScanBoxNo);
						return new ScanPackageCodeResult { IsPass = true, IsFinishCurrentBox = true, ItemCode = packgeCode.InputCode, Message = "人員按下加箱" };
					}
				}

				// 檢查刷讀條碼是否為紙箱
				var result = GetCartonResult(f050801, packgeCode.InputCode);
				if (result.IsCarton)
				{
					// 出貨單含有原箱，且原箱商品尚未刷讀
					if (packgeCode.F050801Item.ALLOWORDITEM == "1" && hasOrginalItemNotPackFinish)
						// 出貨明細中含有原箱商品，請刷讀原箱品號/序號/箱號
						return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_ScanOrginalItem) { ItemCode = packgeCode.InputCode };

					// 只有無包裝頭檔 或 最後一次包裝頭檔為已列印託運單(此箱包裝完成) 才允許刷讀箱號
					if (f055001 == null || f055001.PRINT_FLAG == 1)
						return result;
					// 有包裝頭檔 只是要換紙箱
					else if (f055001 != null && f055001.BOX_NUM != result.ItemCode)
					{
						// 更新包裝
						F055001Repository f055001Repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);
						f055001.BOX_NUM = result.ItemCode;
            f055001Repo.Update(f055001);

						return new ScanPackageCodeResult
						{
							IsPass = true,
							Message = Properties.Resources.P0807010000_ChangeBoxNum,
							IsCarton = true,
							ItemCode = result.ItemCode,
							BoxNum = result.BoxNum
						};
					}
				}
				else
				{
					// 出貨單含有原箱，且原箱商品都已刷讀完成，且無包裝頭檔或最後一次包裝頭檔為已列印託運單(此箱包裝完成) 刷讀不是箱號 
					if (!hasOrginalItemNotPackFinish && (f055001 == null || f055001.PRINT_FLAG == 1) && string.IsNullOrEmpty(packgeCode.BoxNum))
						// 請刷讀箱號
						return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_ErrorBoxItemCode);
				}

			}

			// 檢核數量
			if (packgeCode.AddQty <= 0)
				// 刷讀數量必須大於等於1
				return new ScanPackageCodeResult(false, Properties.Resources.P080701Service_ScanCountError);



			// 檢核通過
			return new ScanPackageCodeResult(true, string.Empty);
		}


		/// <summary>
		/// 是否還有原箱商品尚未刷讀完成
		/// </summary>
		/// <param name="packgeCode">刷讀包裝資訊</param>
		/// <returns></returns>
		private bool HasOrginalItemNotPack(PackgeCode packgeCode)
		{
			if (packgeCode.F050801Item.ALLOWORDITEM == "0")
				return false;

			var f055002Repo = new F055002Repository(Schemas.CoreSchema);
			var data = f055002Repo.GetQuantityOfDeliveryInfo(packgeCode.F050801Item.DC_CODE, packgeCode.F050801Item.GUP_CODE, packgeCode.F050801Item.CUST_CODE, packgeCode.F050801Item.WMS_ORD_NO, itemCode: null, packageBoxNo: 0);

			// 是否還有原箱商品尚未刷讀完成
			var hasOrginalItemNotPack = (from o in data
																	 where o.AllowOrdItem == "1" && o.DiffQty > 0
																	 select o).Any();
			return hasOrginalItemNotPack;
		}

		/// <summary>
		/// 檢查商品是否為出貨單商品
		/// </summary>
		/// <param name="packgeCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		private bool IsInWmsOrder(PackgeCode packgeCode, string itemCode)
		{
			return packgeCode.F050802s.Any(x => x.ITEM_CODE == itemCode);
		}

		/// <summary>
		/// 新增或更新出貨包裝身檔的商品數量
		/// </summary>
		/// <param name="f050801"></param>
		/// <param name="f055001"></param>
		/// <param name="f1903"></param>
		/// <param name="itemCode"></param>
		/// <param name="addQty"></param>
		/// <param name="serialNo"></param>
		/// <returns></returns>
		private ScanPackageCodeResult InsertOrUpdateF055002Qty(F050801 f050801, F055001 f055001, F1903 f1903, DeliveryData deliveryData, string itemCode, int addQty = 1, string serialNo = "", string status = "")
		{
			var result = CheckDeliveryQty(f1903, deliveryData, itemCode, addQty, serialNo, status);

			if (!result.IsPass)
				return result;

			var addF055002List = new List<F055002>();
			var updF055002List = new List<F055002>();
			var f055002Repo = new F055002Repository(Schemas.CoreSchema, _wmsTransaction);
			// 若是組合商品，前面傳來的可能是 0，避免將0 也新增 F055002資料。
			var executeResult = new ExecuteResult(true);
			if (addQty > 0)
				InsertOrUpdateF055002(f055001, itemCode, addQty, serialNo, ref addF055002List, ref updF055002List);

			if (addF055002List.Any())
				f055002Repo.BulkInsert(addF055002List);
			if (updF055002List.Any())
				f055002Repo.BulkUpdate(updF055002List);

			if (executeResult != null && !executeResult.IsSuccessed)
			{
				result.IsPass = executeResult.IsSuccessed;
				result.Message = executeResult.Message;
			}

			return result;
		}


		/// <summary>
		/// 產生新增或更新出貨包裝身擋明細
		/// </summary>
		/// <param name="f055001"></param>
		/// <param name="itemCode"></param>
		/// <param name="addQty"></param>
		/// <param name="serialNo"></param>
		/// <param name="addF055002List"></param>
		/// <param name="updF055002List"></param>
		private void InsertOrUpdateF055002(F055001 f055001, string itemCode, int addQty, string serialNo, ref List<F055002> addF055002List, ref List<F055002> updF055002List)
		{
			var f05030202Repo = new F05030202Repository(Schemas.CoreSchema);

			var itemShipPackageNoAllotOrders = f05030202Repo.GetItemShipPackageNoAllotOrder(f055001.DC_CODE, f055001.GUP_CODE, f055001.CUST_CODE, f055001.WMS_ORD_NO, itemCode).ToList();

			if (!string.IsNullOrEmpty(serialNo) && itemShipPackageNoAllotOrders.Any(x => x.SERIAL_NO == serialNo))
				itemShipPackageNoAllotOrders = itemShipPackageNoAllotOrders.Where(x => x.SERIAL_NO == serialNo).ToList();

			var f055002s = F055002Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == f055001.DC_CODE && x.GUP_CODE == f055001.GUP_CODE && x.CUST_CODE == f055001.CUST_CODE && x.WMS_ORD_NO == f055001.WMS_ORD_NO && x.PACKAGE_BOX_NO == f055001.PACKAGE_BOX_NO && x.ITEM_CODE == itemCode).ToList();
      
			do
			{
        var item = itemShipPackageNoAllotOrders.FirstOrDefault(o => !string.IsNullOrWhiteSpace(o.SERIAL_NO) && o.SERIAL_NO == serialNo);

        if (item == null)
          item = itemShipPackageNoAllotOrders.First(x => x.B_DELV_QTY - x.PACKAGE_QTY > 0);

				var itemQty = item.B_DELV_QTY - item.PACKAGE_QTY;
				var allotQty = addQty;

				if (itemQty >= addQty)
				{
					item.PACKAGE_QTY += addQty;
					addQty = 0;
				}
				else
				{
					item.PACKAGE_QTY += itemQty;
					allotQty = itemQty;
					addQty -= itemQty;
				}

				if (string.IsNullOrEmpty(serialNo))
				{
					var findF055002 = f055002s.FirstOrDefault(x => x.ORD_NO == item.ORD_NO && x.ORD_SEQ == item.ORD_SEQ && string.IsNullOrWhiteSpace(x.SERIAL_NO));
					if (findF055002 != null)
					{
						findF055002.PACKAGE_QTY += allotQty;
						updF055002List.Add(findF055002);
						continue;
					}
				}

        addF055002List.Add(new F055002
				{
					WMS_ORD_NO = f055001.WMS_ORD_NO,
					DC_CODE = f055001.DC_CODE,
					GUP_CODE = f055001.GUP_CODE,
					CUST_CODE = f055001.CUST_CODE,
					PACKAGE_BOX_NO = f055001.PACKAGE_BOX_NO,
					PACKAGE_BOX_SEQ = GetF055002NextSeq(f055001),
					PACKAGE_QTY = allotQty,
					CLIENT_PC = Current.DeviceIp,
					ITEM_CODE = itemCode,
					SERIAL_NO = serialNo,
					ORD_NO = item.ORD_NO,
					ORD_SEQ = item.ORD_SEQ
				});
			}
			while (addQty > 0);
		}

		private int? f055002MaxSeq = null;
		/// <summary>
		/// 取得包裝身擋明細新的SEQ
		/// </summary>
		/// <param name="f055001"></param>
		/// <returns></returns>
		private int GetF055002NextSeq(F055001 f055001)
		{
			if (f055002MaxSeq == null)
			{
				var maxSeq = F055002Repo.Filter(x => x.WMS_ORD_NO == EntityFunctions.AsNonUnicode(f055001.WMS_ORD_NO)
													 && x.PACKAGE_BOX_NO == f055001.PACKAGE_BOX_NO
													 && x.DC_CODE == EntityFunctions.AsNonUnicode(f055001.DC_CODE)
													 && x.GUP_CODE == EntityFunctions.AsNonUnicode(f055001.GUP_CODE)
													 && x.CUST_CODE == EntityFunctions.AsNonUnicode(f055001.CUST_CODE))
					.Max(x => (int?)x.PACKAGE_BOX_SEQ);
				f055002MaxSeq = maxSeq.HasValue ? maxSeq.Value + 1 : 1;
				return f055002MaxSeq.Value;
			}
			f055002MaxSeq++;
			return f055002MaxSeq.Value;

		}

		/// <summary>
		/// 取得出貨資料尚未包裝完商品與差異數
		/// </summary>
		/// <param name="f050801"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		private DeliveryData GetQuantityOfDeliveryInfo(F050801 f050801, string itemCode)
		{
			var f055002Repo = new F055002Repository(Schemas.CoreSchema, _wmsTransaction);
			return f055002Repo.GetQuantityOfDeliveryInfo(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, f050801.WMS_ORD_NO, itemCode).FirstOrDefault();
		}

		/// <summary>
		/// 取得最後一箱包裝頭檔
		/// </summary>
		/// <param name="f050801"></param>
		/// <returns></returns>
		public F055001 GetMyLastF055001(F050801 f050801)
		{
			var f055001Repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);
			var f055001 = f055001Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == EntityFunctions.AsNonUnicode(f050801.DC_CODE)
												&& x.GUP_CODE == EntityFunctions.AsNonUnicode(f050801.GUP_CODE)
												&& x.CUST_CODE == EntityFunctions.AsNonUnicode(f050801.CUST_CODE)
												&& x.WMS_ORD_NO == EntityFunctions.AsNonUnicode(f050801.WMS_ORD_NO))
								.OrderByDescending(x => x.PACKAGE_BOX_NO)
								.FirstOrDefault();

			return f055001;
		}



    #endregion

    #region 刷讀Log
    /// <summary>
    /// 刷讀Log
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="wmsOrdNo"></param>
    /// <param name="itemCode"></param>
    /// <param name="serialNo"></param>
    /// <param name="status"></param>
    /// <param name="isPass"></param>
    /// <param name="message"></param>
    /// <param name="packageBoxNo"></param>
    /// <param name="logSeq"></param>
    /// <param name="flag">紀錄狀態(0:正常包裝 9:取消包裝)</param>
    /// <returns></returns>
    public ExecuteResult LogF05500101(string dcCode, string gupCode, string custCode, string wmsOrdNo, string itemCode, string serialNo, string status, string isPass, string message, short packageBoxNo = 0, int? logSeq = null, string flag = "0", string orgSerialWmsNo = null, string scanCode = null)
    {
      ShipPackageService.LogF05500101(dcCode, gupCode, custCode, wmsOrdNo, itemCode, serialNo, status, isPass, message, packageBoxNo, null, logSeq, flag, orgSerialWmsNo);

			return new ExecuteResult(true);
		}

    #endregion

    #region 更新刷讀紀錄Flag
    public ExecuteResult UpdateF05500101Flag(string dcCode,string gupCode,string custCode,string WmsOrdNo,string Flag)
    {
      var f05500101Repo = new F05500101Repository(Schemas.CoreSchema, _wmsTransaction);
      f05500101Repo.UpdateFields(new { FLAG = Flag }, x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.WMS_ORD_NO == WmsOrdNo);
      return new ExecuteResult(true);
    }
    #endregion 更新刷讀紀錄Flag

    #endregion

    #region 關箱 / 出貨單包裝完成更新 / 更新紙箱庫存

    /// <summary>
    /// 當每次NEWBOX或者全部已刷讀後，則開始更新出貨單與箱子的資料與狀態
    /// </summary>
    /// <param name="f050801"></param>
    /// <param name="f055001"></param>
    /// <param name="isCompletePackage"></param>
    /// <returns></returns>
    public ExecuteResult FinishCurrentBox(ref F050801 f050801, ref F055001 f055001, bool isCompletePackage, Boolean isManualCloseBox)
    {
      var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
      var f055001Repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);
      var f055002Repo = new F055002Repository(Schemas.CoreSchema);
      var consignService = new Shared.Lms.Services.ConsignService(_wmsTransaction);

      //因為下方Find不能用ref傳入的物件傳入參數，因此複製一份出來查詢用
      var f581Data = AutoMapper.Mapper.DynamicMap<F050801>(f050801);
			var f551Data = AutoMapper.Mapper.DynamicMap<F055001>(f055001);

      f055001 = f055001Repo.Find(x => x.DC_CODE == f551Data.DC_CODE && x.GUP_CODE == f551Data.GUP_CODE && x.CUST_CODE == f551Data.CUST_CODE && x.WMS_ORD_NO == f551Data.WMS_ORD_NO && x.PACKAGE_BOX_NO == f551Data.PACKAGE_BOX_NO);

      f055001.PACK_CLIENT_PC = Current.DeviceIp;
      f055001.CLOSEBOX_TIME = DateTime.Now;

        //廠退出貨不需要申請宅單
        if (f050801.SOURCE_TYPE != "13")
      {
        //申請宅配單號
        var lmsApiRes = consignService.ApplyConsign(f055001.DC_CODE, f055001.GUP_CODE, f055001.CUST_CODE, f055001.WMS_ORD_NO, f055001.PACKAGE_BOX_NO, f055001: f055001);
        if (!lmsApiRes.IsSuccessed)
          return new ExecuteResult { IsSuccessed = false, Message = $"{lmsApiRes.MsgContent}\r\n呼叫LMS申請宅配單失敗，請執行<手動關箱>", No = "LMS ERROR" };
      }
      else
			{
				f055001.PRINT_FLAG = 1;
				f055001.PRINT_DATE = DateTime.Now;
				f055001.IS_CLOSED = "1";
				f055001Repo.Update(f055001);
			}

      f050801 = f050801Repo.Find(x => x.DC_CODE == f581Data.DC_CODE
										&& x.GUP_CODE == f581Data.GUP_CODE
										&& x.CUST_CODE == f581Data.CUST_CODE
										&& x.WMS_ORD_NO == f581Data.WMS_ORD_NO);

			// 全部刷讀完後，更新狀態、清除盒箱號、建立LO
			var result = FinishCurrentBoxByCompletePackage(f050801, isCompletePackage);
			if (!result.IsSuccessed)
				return result;

			// 使用過的箱子數量要從F1913扣掉
			UpdateBoxStock(f055001.DC_CODE, f055001.GUP_CODE, f055001.CUST_CODE, f055001.BOX_NUM);

      if (isCompletePackage)
        f050801.PACK_FINISH_TIME = DateTime.Now;
      // 標記已列印過
      f050801.PRINT_FLAG = "1";
			f050801Repo.Update(f050801);

      if (isCompletePackage)
        return GetFinishPackingMessage(f050801);

      return new ExecuteResult(true);
		}

		/// <summary>
		/// 出貨單完成包裝後更新資料
		/// </summary>
		/// <param name="f050801Entity"></param>
		/// <param name="isCompletePackage"></param>
		/// <returns></returns>
		public ExecuteResult FinishCurrentBoxByCompletePackage(F050801 f050801Entity, bool isCompletePackage)
		{
			var f1909Repo = new F1909Repository(Schemas.CoreSchema);
			var f1909 = f1909Repo.Find(x => x.GUP_CODE == f050801Entity.GUP_CODE && x.CUST_CODE == f050801Entity.CUST_CODE);
			if (f050801Entity.STATUS != 0 || !isCompletePackage)
			{
				return new ExecuteResult(true);
			}

			if (f050801Entity.SELF_TAKE == "1")
			{
				f050801Entity.STATUS = 2; //自取單:2:已稽核待出貨
			}
			else if (f1909.IS_SINGLEBOXCHECK == "1" && (IsSingleBox(f050801Entity) && !IsSplitOrder(f050801Entity))) //是否單箱稽核(0否 1是) 且 單箱且無拆單
			{
				f050801Entity.STATUS = 1; //已包裝待稽核
			}
			else if (f050801Entity.NO_AUDIT == "1" || (IsSingleBox(f050801Entity) && !IsSplitOrder(f050801Entity)) || f1909.ALLOW_ADDBOXNOCHECK == "1")
			{
				f050801Entity.STATUS = 2; //不稽核(0否 1是) 或 單箱且無拆單 或貨主允許加箱號後不做稽核
			}
			else if (f050801Entity.NO_LOADING == "1")
			{
				f050801Entity.STATUS = 2; //不裝車(0否 1是)
			}
			else
			{
				f050801Entity.STATUS = 1;
			}

			// 更新來源單據狀態
			if (!string.IsNullOrEmpty(f050801Entity.SOURCE_TYPE))
			{
				// 目前只有進倉->內部交易->結案在這邊會用到，詳細請查看 F00090201
				var sharedService = new SharedService(_wmsTransaction);
				sharedService.UpdateSourceNoStatus(SourceType.Order,
													f050801Entity.DC_CODE,
													f050801Entity.GUP_CODE,
													f050801Entity.CUST_CODE,
													f050801Entity.WMS_ORD_NO,
													f050801Entity.STATUS.ToString());
			}

			// 清除已拆開序號的箱號/盒號/儲值卡盒號
			var serialNoSerivce = new SerialNoService(_wmsTransaction);
			serialNoSerivce.ClearSerialByBoxOrCaseNo(f050801Entity.DC_CODE, f050801Entity.GUP_CODE, f050801Entity.CUST_CODE, f050801Entity.WMS_ORD_NO, "O");

			// 設定LO為完成包裝
			//var result = SetLoStatusFinishPack(f050801Entity.WMS_ORD_NO);
			//if (result == null)
			//{
			//	// 建立稽核LO
			//	result = CreateLoMainAudit(f050801Entity.WMS_ORD_NO, f050801Entity.GUP_CODE, f050801Entity.CUST_CODE, f050801Entity.DC_CODE);
			//	if (result == null)
			//		result = new ExecuteResult { IsSuccessed = true };
			//}

			//return result;
			return new ExecuteResult(true);
		}

		/// <summary>
		/// 取得出貨單完成包裝後下一步動作訊息
		/// </summary>
		/// <param name="f050801"></param>
		/// <returns></returns>
		public ExecuteResult GetFinishPackingMessage(F050801 f050801)
		{
			var f05500102Repo = new F05500102Repository(Schemas.CoreSchema, _wmsTransaction);
			var containerService = new ContainerService(_wmsTransaction);

			var msgType = GetMsgType(f050801);
			var sourceType = string.IsNullOrEmpty(f050801.SOURCE_TYPE) ? "00" : f050801.SOURCE_TYPE;
			var f05500102 = f05500102Repo.Find(x => x.SOURCE_TYPE == sourceType && x.MSG_TYPE == msgType);

			if (f05500102 == null)
				return new ExecuteResult(true, Properties.Resources.P080701Service_ItemPackingComplete); // 出貨包裝完成，不為內部交易同DC，也不用自取、稽核、裝車，無法判斷訊息

			if (msgType == "2")
			{
				var f700101Repo = new F700101Repository(Schemas.CoreSchema, _wmsTransaction);
				var distrDatas = f700101Repo.GetF700101ByWmsOrdNo(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, f050801.WMS_ORD_NO).FirstOrDefault(); ;
				var pierCode = distrDatas == null ? string.Empty : distrDatas.PIER_CODE;
				return new ExecuteResult(true, string.Format(f05500102.MESSAGE, pierCode));
			}

			// 容器釋放任務觸發
			containerService.DelContainer(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, f050801.WMS_ORD_NO);

			return new ExecuteResult(true, f05500102.MESSAGE);
		}

		/// <summary>
		/// 更新紙箱庫存
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="boxNum"></param>
		/// <returns></returns>
		public ExecuteResult UpdateBoxStock(string dcCode, string gupCode, string custCode, string boxNum)
		{
			if (boxNum == "ORI") // 原箱不更新紙箱庫存
				return new ExecuteResult { IsSuccessed = true };

			var f1913Repo = new F1913Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1913 = f1913Repo.Filter(x => x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
											&& x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
											&& x.ITEM_CODE == EntityFunctions.AsNonUnicode(boxNum))
								 .FirstOrDefault();

			if (f1913 == null)
			{
				var f1912repo = new F1912Repository(Schemas.CoreSchema, _wmsTransaction); ;
				var f1912 = f1912repo.GetDatas(dcCode, "S", gupCode, custCode).FirstOrDefault();
				if (f1912 == null)
					f1912 = f1912repo.GetDatas(dcCode, "S", gupCode).FirstOrDefault();
				if (f1912 == null)
					f1912 = f1912repo.GetDatas(dcCode, "S").FirstOrDefault();

				f1913 = new F1913
				{
					CUST_CODE = custCode,
					DC_CODE = dcCode,
					ENTER_DATE = DateTime.Today,
					GUP_CODE = gupCode,
					ITEM_CODE = boxNum,
					LOC_CODE = f1912.LOC_CODE,
					QTY = -1,
					VALID_DATE = Convert.ToDateTime("9999/12/31"),
					VNR_CODE = "000000",
					BOX_CTRL_NO = "0",
					PALLET_CTRL_NO = "0"
				};
				f1913Repo.Add(f1913);
			}
			else
			{
				f1913.QTY -= 1;
				f1913Repo.Update(f1913);
			}

			return new ExecuteResult { IsSuccessed = true };
		}

		#region 關箱相關方法

		/// <summary>
		/// 依出貨單取得下一步動作訊息代號
		/// </summary>
		/// <param name="f050801"></param>
		/// <returns></returns>
		private string GetMsgType(F050801 f050801)
		{
			//是否為超取
			if (f050801.CVS_TAKE == "1")
				return "4";
			// 是否為同DC的內部交易單
			if (IsSameDCInternalTrading(f050801))
				return "0"; // 不封箱，請移至收貨碼頭區

			if (f050801.SELF_TAKE == "1")
			{
				return "3"; // 客戶自取請移至出貨碼頭暫存區
			}
			var f1909Repo = new F1909Repository(Schemas.CoreSchema);
			var f1909 = f1909Repo.Find(x => x.GUP_CODE == f050801.GUP_CODE && x.CUST_CODE == f050801.CUST_CODE);
			if (f050801.NO_AUDIT == "0")
			{
				if (f1909.ALLOW_ADDBOXNOCHECK == "0")
				{
					if (!IsSingleBox(f050801))
						return "1"; // 請移至稽核線

					if (IsSplitOrder(f050801))
						return "1"; // 請移至稽核線
				}
			}

			if (f050801.NO_LOADING == "0")
			{
				return "2"; // 請移至{0}出貨碼頭區裝車
			}

			// 出貨包裝完成，不為內部交易同DC，也不用自取、稽核、裝車，無法判斷訊息
			return string.Empty;
		}

		/// <summary>
		/// 是否為同DC的內部交易單
		/// </summary>
		/// <param name="f050801"></param>
		/// <returns></returns>
		private static bool IsSameDCInternalTrading(F050801 f050801)
		{
			return f050801.SOURCE_TYPE == "09" && f050801.PRINT_PASS == "0";
		}

		/// <summary>
		/// 是否為單箱，可能落在兩個不會一起處理的部分，為了不重複讀取，讀取過就快取起來。
		/// </summary>
		private bool? _singleBox = null;

		/// <summary>
		/// 是否為單箱
		/// </summary>
		/// <param name="f050801"></param>
		/// <returns></returns>
		private bool IsSingleBox(F050801 f050801)
		{
			if (!_singleBox.HasValue)
			{
				var f055001Repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);
				var count = f055001Repo.Filter(x => x.DC_CODE == EntityFunctions.AsNonUnicode(f050801.DC_CODE)
													&& x.GUP_CODE == EntityFunctions.AsNonUnicode(f050801.GUP_CODE)
													&& x.CUST_CODE == EntityFunctions.AsNonUnicode(f050801.CUST_CODE)
													&& x.WMS_ORD_NO == EntityFunctions.AsNonUnicode(f050801.WMS_ORD_NO)).Count();

				_singleBox = (count == 1);
			}
			return _singleBox.Value;
		}

		/// <summary>
		/// 是否為拆單，可能落在兩個不會一起處理的部分，為了不重複讀取，讀取過就快取起來。
		/// </summary>
		private bool? _isSplitOrder = null;

		/// <summary>
		/// 是否為拆單
		/// </summary>
		/// <param name="f050801"></param>
		/// <returns></returns>
		private bool IsSplitOrder(F050801 f050801)
		{
			if (!_isSplitOrder.HasValue)
			{
				var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
				_isSplitOrder = f050801Repo.GetF050801SeparateBillData(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, f050801.WMS_ORD_NO).Any();
			}

			return _isSplitOrder.Value;
		}




		#endregion

		#endregion

		#region 取消包裝

		/// <summary>
		/// 取消包裝
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdCode"></param>
		/// <returns></returns>
		public ExecuteResult CancelPacking(string dcCode, string gupCode, string custCode, string wmsOrdCode)
		{
			DeletedF055002(dcCode, gupCode, custCode, wmsOrdCode);
			var sharedService = new SharedService(_wmsTransaction);
      var f050801Repo = new F050801Repository(Schemas.CoreSchema);

      UpdateF05500101Flag(dcCode, gupCode, custCode, wmsOrdCode, "9");
      LogF05500101(dcCode, gupCode, custCode, wmsOrdCode, null, null, null, "1", "取消包裝", 0, flag: "9");

      var f050801 = f050801Repo.GetData(wmsOrdCode, gupCode, custCode, dcCode);
      var shipModeCheckResult = ShipPackageService.CheckPackageMode(f050801, "0");
      if (!shipModeCheckResult.IsSuccessed)
        return new ExecuteResult(shipModeCheckResult.IsSuccessed, shipModeCheckResult.MsgContent);

      var shareResult = sharedService.ReturnBoxQty(dcCode, gupCode, custCode, wmsOrdCode);
			if (!shareResult.IsSuccessed)
			{
				return shareResult;
			}
			DeleteF050901AndResetF19471201IsUsed(dcCode, gupCode, custCode, wmsOrdCode);
			DeletedF055001(dcCode, gupCode, custCode, wmsOrdCode);
			ShipPackageService.UpdateF2501(dcCode, gupCode, custCode, wmsOrdCode);
			//DeletedF05500101(dcCode, gupCode, custCode, wmsOrdCode);
			UpdateF050801(dcCode, gupCode, custCode, wmsOrdCode);
			UpdateF160204ForCancelPacking(dcCode, gupCode, custCode, wmsOrdCode);
			return new ExecuteResult(true);
		}

		#region 取消包裝相關方法

		/// <summary>
		/// 取消包裝
		/// 要刪除加箱的託運單
		/// 要復原加箱託運單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		private void DeleteF050901AndResetF19471201IsUsed(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			var f050901Repo = new F050901Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050801 = f050801Repo.GetData(wmsOrdNo, gupCode, custCode, dcCode);

			// 自取無託運單 不需要回復託運單資料
			if (f050801 != null && f050801.SELF_TAKE != "1")
			{

				var f055001Repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);
				var f19471201Repo = new F19471201Repository(Schemas.CoreSchema, _wmsTransaction);

				var f055001Data = f055001Repo.GetDatas(wmsOrdNo, gupCode, custCode, dcCode);
				var f050301Repo = new F050301Repository(Schemas.CoreSchema);
				var f194712Repo = new F194712Repository(Schemas.CoreSchema);
				var f194704Repo = new F194704Repository(Schemas.CoreSchema);
				var f194704 = f194704Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == f050801.DC_CODE && x.GUP_CODE == f050801.GUP_CODE && x.CUST_CODE == f050801.CUST_CODE && x.ALL_ID == f050801.ALL_ID).FirstOrDefault();
				//如果是系統取號且加箱取同一託運單號(一單多件)
				if (f194704 != null && f194704.GET_CONSIGN_NO == "1" && f194704.ADDBOX_GET_CONSIGN_NO == "1")
				{
					var f050901 = f050901Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == f050801.DC_CODE && x.GUP_CODE == f050801.GUP_CODE && x.CUST_CODE == f050801.CUST_CODE && x.WMS_NO == f050801.WMS_ORD_NO).FirstOrDefault();
					if (f050901 != null)
					{
						switch (f050801.ALL_ID)
						{
							case "HCT"://新竹貨運
								f050901.DELIVID_SEQ_NAME = f050901.WMS_NO.StartsWith("ZC") ? "NOHAVEORDELIVID" : (f050901.DELV_TIMES == "12" ? "TODAYDELIVID" : "TOMORROWDELIVID");
								f050901.BOXQTY = 1;
								f050901Repo.Update(f050901);
								break;
						}
					}
				}
				else
				{
					var consignNoList = new List<string>();
					foreach (var item in f055001Data)
					{
						if (item.PACKAGE_BOX_NO == 1 && (f050801.CVS_TAKE == "0" && f050801.SELF_TAKE == "0")) //宅配保留配庫產生的第一張託運單
							continue;
						if (!string.IsNullOrWhiteSpace(item.PAST_NO))
							consignNoList.Add(item.PAST_NO);
					}
					if (consignNoList.Any())
					{
						f050901Repo.DeleteInWithTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.WMS_NO == wmsOrdNo, x => x.CONSIGN_NO, consignNoList);
						if (f050801.ALL_ID == "TCAT") //速達要釋放託運單號
						{
							var f050301 = f050301Repo.GetDataByWmsOrdNo(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, f050801.WMS_ORD_NO);
							var channel = "00";
							if (f050301 != null)
								channel = f050301.CHANNEL;
							var f194712 = f194712Repo.Get(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, channel, f050801.ALL_ID);
							if (f194712 == null)
								channel = "00";
#if DEBUG
							var isTest = "1";
#else
					var isTest = "0";
#endif
							f19471201Repo.UpdateFieldsInWithTrueAndCondition(SET: new { ISUSED = "0" }, WHERE: x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ALL_ID == f050801.ALL_ID && x.ISTEST == isTest && x.CHANNEL == channel, InFieldName: x => x.CONSIGN_NO, InValues: consignNoList);
						}
					}
				}
			}

			if (f050801.SELF_TAKE == "1")
			{
				var f050901s = f050901Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.WMS_NO == wmsOrdNo).ToList();
        if (f050901s != null && f050901s.Any())
				  f050901Repo.SqlBulkDeleteForAnyCondition(f050901s, "F050901", new List<string> { "DC_CODE", "GUP_CODE", "CUST_CODE", "WMS_NO" });
			}

		}

		/// <summary>
		/// 取消包裝-刪除F055002
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdCode"></param>
		private void DeletedF055002(string dcCode, string gupCode, string custCode, string wmsOrdCode)
		{
			var f055002repo = new F055002Repository(Schemas.CoreSchema, _wmsTransaction);
			f055002repo.Delete(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.WMS_ORD_NO == wmsOrdCode);

		}

		/// <summary>
		/// 取消包裝-刪除F055001
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdCode"></param>
		private void DeletedF055001(string dcCode, string gupCode, string custCode, string wmsOrdCode)
		{
			var f055001repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);
			f055001repo.Delete(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.WMS_ORD_NO == wmsOrdCode);
		}

		/// <summary>
		/// 取消包裝-刪除F05500101
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdCode"></param>
		private void DeletedF05500101(string dcCode, string gupCode, string custCode, string wmsOrdCode)
		{
			var f05500101repo = new F05500101Repository(Schemas.CoreSchema, _wmsTransaction);
			f05500101repo.Delete(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.WMS_ORD_NO == wmsOrdCode);
		}

		/// <summary>
		/// 取消包裝-更新F050801狀態
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdCode"></param>
		private void UpdateF050801(string dcCode, string gupCode, string custCode, string wmsOrdCode)
		{
			var f050801repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050801 = f050801repo.GetData(wmsOrdCode, gupCode, custCode, dcCode);
      // 若出貨單狀態為已取消則不可更新為待處理(0)
      if (f050801 != null && f050801.STATUS != 9)
      {
        f050801.STATUS = 0;
        f050801.PACK_CANCEL_TIME = DateTime.Now;
        f050801.PACK_START_TIME = null;
        f050801.PACK_FINISH_TIME = null;
        f050801repo.Update(f050801);
        //f050801repo.UpdateStatus(dcCode, gupCode, custCode, wmsOrdCode, 0);
      }

		}

		/// <summary>
		/// 出貨包裝_取消廠退單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdCode"></param>
		public void UpdateF160204ForCancelPacking(string dcCode, string gupCode, string custCode, string wmsOrdCode)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			var f050301Repo = new F050301Repository(Schemas.CoreSchema);
			var f160204Repo = new F160204Repository(Schemas.CoreSchema);
			var f050801 = f050801Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.WMS_ORD_NO == wmsOrdCode).FirstOrDefault();
			if (f050801.SOURCE_TYPE == "13")
			{
				var f050301s = f050301Repo.GetF050301ForWmsOrdNo(dcCode, gupCode, custCode, wmsOrdCode);
				// 取得廠退單出貨單資料
				var f160204s = f160204Repo.GetDatasByTrueAndCondition(x => x.RTN_WMS_NO == f050301s.First().SOURCE_NO).OrderBy(x => x.RTN_WMS_NO);
				// 更新f160204.PROC_FLAG
				foreach (var item in f160204s)
				{
					item.PROC_FLAG = "1";
				}
				f160204Repo.BulkUpdate(f160204s);
			}
		}

		#endregion

		#endregion

		#region 取得箱明細

		/// <summary>
		/// 取得箱明細設定
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <returns></returns>
		public IQueryable<DelvdtlInfo> GetDelvdtlInfo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var f050801repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			return f050801repo.GetDelvdtlInfo(dcCode, gupCode, custCode, wmsOrdNo).AsQueryable();
		}

		/// <summary>
		/// 取得PcHome箱明細頭檔資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <returns></returns>
		public IQueryable<PcHomeDeliveryReport> GetPcHomeDelivery(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
      var res = new PcHomeDeliveryReport();
      var f055001Repo = new F055001Repository(Schemas.CoreSchema);
      var f05030101Repo = new F05030101Repository(Schemas.CoreSchema, _wmsTransaction);
      res.ROWNUM = 1;

      res = f05030101Repo.GetBoxHeaderData(dcCode, gupCode, custCode, wmsOrdNo);


      var f055001 = f055001Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.WMS_ORD_NO == wmsOrdNo).OrderByDescending(x => x.PACKAGE_BOX_NO).FirstOrDefault();

      return (new List<PcHomeDeliveryReport> { res }).AsQueryable();
    }

    /// <summary>
    /// 取得箱明細身擋
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="wmsOrdNo"></param>
    /// <param name="packageBoxNo"></param>
    /// <returns></returns>
    public IQueryable<DeliveryReport> GetDeliveryReport(string dcCode, string gupCode, string custCode, string wmsOrdNo, short? packageBoxNo = null)
		{
			var list = new List<DeliveryReport>();

			var repo = new F055002Repository(Schemas.CoreSchema);

			var data = repo.GetDeliveryReport(dcCode, gupCode, custCode, wmsOrdNo, packageBoxNo).ToList();

			//取得服務型商品 以下將明細插入服務項目
			var f050104Repo = new F050104Repository(Schemas.CoreSchema);
			var f050104s = f050104Repo.GetDatas(dcCode, gupCode, custCode, data.Select(x => x.OrdNo).ToList())
					.GroupBy(x => x.ITEM_CODE)
					.Select(x => new { ItemCode = x.Key, Services = x.Select(z => new { Code = z.SERVICE_ITEM_CODE, Name = z.SERVICE_ITEM_NAME }).ToList() });
			if (!f050104s.Any())
			{
				list = data;
				for (int i = 0; i < list.Count; i++) { list[i].ROWNUM = i + 1; }
			}
			else
			{
				int rowNum = 0;
				data.ForEach(item =>
				{
					rowNum++;
					item.ROWNUM = rowNum;
					list.Add(item);
					var currF050104 = f050104s.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
					if (currF050104 != null)
					{
						currF050104.Services.ForEach(service =>
										{
									rowNum++;
									list.Add(new DeliveryReport
									{
										ROWNUM = rowNum,
										PackageBoxNo = item.PackageBoxNo,
										CUST_ITEM_CODE = service.Code,
										ItemName = service.Name
									});
								});
					}
				});
			}

			// 2023/02/13 Scott 目前客戶無非加工組合商品，先暫時註解，等客戶有需要再來解開註解
			//以下為將明細商品轉換成非加工組合商品
			//var f05030201Repo = new F05030201Repository(Schemas.CoreSchema);
			//var BomOrderDetail = f05030201Repo.GetDeliveryReportByBomItem(dcCode, gupCode, custCode, wmsOrdNo, packageBoxNo).ToList();
			//if (BomOrderDetail.Any())
			//{
			//	var detail = f05030201Repo.GetDatasByWmsOrdNo(dcCode, gupCode, custCode, new List<string> { wmsOrdNo }).ToList();
			//	foreach (var item in BomOrderDetail)
			//	{
			//		var bomItemDetail = detail.Where(o => o.BOM_ITEM_CODE == item.ItemCode).ToList();
			//		var resultDetail = list.Where(x => bomItemDetail.Any(y => y.ITEM_CODE == x.ItemCode) && x.PackageBoxNo == item.PackageBoxNo).ToList();
			//		//組合品項數必須存在出貨單中
			//		if (resultDetail.Count == bomItemDetail.Count)
			//		{
			//			//計算每個商品訂貨數可以組合成幾個c
			//			var countItemC = new List<int>();
			//			foreach (var item3 in bomItemDetail)
			//			{
			//				var resultItem = resultDetail.FirstOrDefault(o => o.ItemCode == item3.ITEM_CODE);
			//				var orderQty = resultItem.PackQty;
			//				//如果訂貨數>= 組合C該商品數量 則訂貨數等於組合C該商品數量
			//				if (orderQty - item3.ORD_QTY >= 0)
			//					orderQty = item3.ORD_QTY;
			//				//此商品可產生幾個組合C
			//				countItemC.Add(orderQty / item3.BOM_QTY);
			//			}
			//			//取最小可組合c的數量
			//			var minC = countItemC.Min();
			//			if (minC >= item.PackQty)
			//				minC = item.PackQty;
			//			if (minC > 0) //有組合c數量
			//			{
			//				foreach (var item2 in bomItemDetail)
			//				{
			//					var resultItem = resultDetail.FirstOrDefault(o => o.ItemCode == item2.ITEM_CODE);
			//					resultItem.PackQty -= item2.BOM_QTY * minC;
			//					if (resultItem.PackQty == 0)
			//						list.Remove(resultItem);
			//				}
			//				item.PackQty = minC;
			//				item.ROWNUM = list.Count + 1;
			//				list.Add(item);
			//			}
			//		}
			//	}
			//}
			return list.AsQueryable();
		}


		#endregion

		#region 託運單

		/// <summary>
		/// 取得出貨配送商通路設定
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <param name="allId"></param>
		/// <returns></returns>
		public F190905 GetF190905(string dcCode, string gupCode, string custCode, string wmsOrdNo, string allId)
		{
			var f050301Repo = new F050301Repository(Schemas.CoreSchema);
			var f050301 = f050301Repo.GetDataByWmsOrdNo(dcCode, gupCode, custCode, wmsOrdNo);
			if (f050301 == null)
				return null;
			var f190905Repo = new F190905Repository(Schemas.CoreSchema);
			var datas = f190905Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.ALL_ID == allId).ToList();
			var item = datas.FirstOrDefault(x => x.CHANNEL == f050301.CHANNEL && x.SUBCHANNEL == f050301.SUBCHANNEL);
			if (item == null)
				item = datas.FirstOrDefault(x => x.CHANNEL == "00" && x.SUBCHANNEL == f050301.SUBCHANNEL);
			if (item == null)
				item = datas.FirstOrDefault(x => x.CHANNEL == f050301.CHANNEL && x.SUBCHANNEL == "00");
			if (item == null)
				item = datas.FirstOrDefault(x => x.CHANNEL == "00" && x.SUBCHANNEL == "00");
			return item;
		}

		/// <summary>
		/// 取得託運單資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <param name="packageBoxNo"></param>
		/// <returns></returns>
		public IQueryable<F055001Data> GetConsignData(string dcCode, string gupCode, string custCode, string wmsOrdNo, string packageBoxNo)
		{
			var f055001repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);
			var data = f055001repo.GetConsignData(dcCode, gupCode, custCode, wmsOrdNo, packageBoxNo);
			if (data.Any(o => o.ALL_ID == "TCAT"))
			{
				var service = new ConsignService();
				var version = service.GetEgsInfo();
				var result = service.GetEgsSuda7DashList(data.Select(o => o.ADDRESS).ToList());
				foreach (var item in data)
				{
					item.VERSION_NUMBER = version.address_db_version;
					var suda7DashItem = result.FirstOrDefault(o => o.address == item.ADDRESS);
					if (suda7DashItem != null)
					{
						item.EGS_SUDA7_DASH = suda7DashItem.suda7_1;
						item.EGS_SUDA7 = suda7DashItem.suda7_1.Replace("-", "");
					}
				}
			}
			else if (data.Any(x => x.ALL_ID == "HCT"))
			{
				var service = new ConsignService();
				foreach (var item in data)
				{
					item.PACK_WEIGHT = "";
					item.HCT_STATION = service.GetHctStation(item.LOGCENTER_ID, item.ADDRESS).PutData_s;
				}
			}
			else if (data.Any(x => x.ALL_ID == "KTJ"))
			{
				var service = new ConsignService();
				List<KtjPostData> ktjPostDatas = new List<KtjPostData>();
				List<KtjPostData> ktjPostChennelDatas = new List<KtjPostData>();
				foreach (var item in data)
				{
					ktjPostDatas.Add(new KtjPostData { ID = item.ROWNUM.ToString(), Address = item.ADDRESS });
					ktjPostChennelDatas.Add(new KtjPostData { ID = item.ROWNUM.ToString(), Address = item.CHANNEL_ADDRESS });
				}

				var ktjStations = service.GetKtjData(ktjPostDatas);
				var ktjChennelStations = service.GetKtjData(ktjPostChennelDatas);
				foreach (var item in data)
				{
					var ktjStation = ktjStations.FirstOrDefault(o => o.ID == item.ROWNUM.ToString());
					var ktjChennelStation = ktjChennelStations.FirstOrDefault(o => o.ID == item.ROWNUM.ToString());
					item.KTJ_STATION = ktjStation.SNO;
					item.KTJ_STATION_NAME = ktjStation.SNA;
					item.KTJ_STATION_S = ktjChennelStation.SNO;
				}

			}
			return data;
		}

		/// <summary>
		/// 取得託運單明細資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <param name="pastNo"></param>
		/// <returns></returns>
		public IQueryable<F055002Data> GetConsignItemData(string dcCode, string gupCode, string custCode, string wmsOrdNo, string pastNo)
		{
			var f055001repo = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);
			return f055001repo.GetConsignItemData(dcCode, gupCode, custCode, wmsOrdNo, pastNo);
		}

		#endregion

		#region 序號匯入
		/// <summary>
		/// 序號匯入的整包更新
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <param name="packageBoxNo"></param>
		/// <param name="serials"></param>
		/// <returns></returns>
		public ExecuteResult CheckAndUpdatePacking(string dcCode, string gupCode, string custCode, string wmsOrdNo, List<string> serials)
		{
			var f050801Repo = new F050801Repository(Schemas.CoreSchema);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema);
			var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);
			var sharedService = new SharedService(_wmsTransaction);
			var f055001Repo = new F055001Repository(Schemas.CoreSchema);
			var f055002Repo = new F055002Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05500101Repo = new F05500101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);

			// 檢查出貨主檔狀態(是否被Lock住, 是否允許包裝)			
			var f050801 = f050801Repo.AsForUpdate()
									 .Find(x => x.WMS_ORD_NO == wmsOrdNo && x.DC_CODE == dcCode
											 && x.GUP_CODE == gupCode && x.CUST_CODE == custCode);

			if (f050801 == null || f050801.STATUS > 0)
				return new ExecuteResult(false, Properties.Resources.P080701Service_F050801StatusError);

			// 判斷是否已經刷讀箱號
			var f055001 = f055001Repo.FindSelfF055001ByNoPrint(f050801.WMS_ORD_NO, f050801.GUP_CODE, f050801.CUST_CODE, f050801.DC_CODE);
			if (f055001 == null || string.IsNullOrEmpty(f055001.BOX_NUM))
				return new ExecuteResult(false, Properties.Resources.P080701Service_NotScanBoxNo);

			// 取得出貨量統計
			var deliveryDatas = f055002Repo.GetDeliveryData(dcCode, gupCode, custCode, wmsOrdNo).ToList();

			// 驗證序號狀態
			var serialStatus = CheckSerials(dcCode, gupCode, custCode, wmsOrdNo, serials);
			var passSerials = serialStatus.Where(x => x.ISPASS).ToList();

			var f051202Query = f051202Repo.Filter(x => x.WMS_ORD_NO == EntityFunctions.AsNonUnicode(wmsOrdNo)
													&& x.DC_CODE == EntityFunctions.AsNonUnicode(dcCode)
													&& x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
													&& x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode));

			// 檢查所有序號
			foreach (var g in passSerials.GroupBy(x => x.ITEMCODE))
			{
				var items = deliveryDatas.Where(x => x.ItemCode == g.Key).ToList();

				if (!items.Any())
					return new ExecuteResult() { IsSuccessed = false, Message = string.Format(Properties.Resources.P080701Service_ItemDeliveryDatasError, g.Key) };

				// 差異數不可小於實收數
				var diffQtySum = items.Sum(x => x.DiffQty);
				var passCount = g.Count();
				if (diffQtySum < passCount)
					return new ExecuteResult(false, string.Format(Properties.Resources.P080701Service_diffQtySumError, g.Key, diffQtySum, passCount));

				// 如果有指定序號出貨,檢查匯入序號是否包含指定出貨序號
				// 檢查該品項是否存在於指定序號?
				if (items.Any(x => x.BUNDLE_SERIALLOC == "1"))
				{
					var bundleSerials = f051202Query.Where(x => x.ITEM_CODE == EntityFunctions.AsNonUnicode(g.Key)).Select(x => x.SERIAL_NO).ToList();
					foreach (var serial in g.Select(x => x.SERIAL_NO))
						if (!bundleSerials.Contains(serial))
							return new ExecuteResult(false, string.Format(Properties.Resources.P080701Service_BundleSerialsNotExist, serial, g.Key));
				}
			}

			// 鎖住出貨主檔, 不允許其它人編輯
			decimal oldStatus = f050801.STATUS;
			f050801.STATUS = 99;
			f050801Repo.Update(f050801);
			try
			{
				// 取得Log檔序號

				var logSeq = f05500101Repo.GetNextLogSeq(dcCode, gupCode, custCode, wmsOrdNo, f055001.PACKAGE_BOX_NO);
				// 不管通過或失敗都皆寫入Log
				foreach (var p in serialStatus)
				{
					// 寫入Log
					LogF05500101(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, f050801.WMS_ORD_NO, p.ITEMCODE, p.SERIAL_NO, p.STATUS, (p.ISPASS ? "1" : "0"), p.MESSAGE, 0, logSeq);
					logSeq++;
				}

				// 依出貨單號/ 包裝箱號取得最大一筆包裝箱序號

				var itemCodes = passSerials.Select(a => a.ITEMCODE).Distinct().ToList();
				var f1903s = f1903Repo.InWithTrueAndCondition("ITEM_CODE", itemCodes, a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode).ToList();

				var addF055002List = new List<F055002>();
				var updF055002List = new List<F055002>();
				// 更新通過的序號狀態及更新出貨數
				foreach (var p in passSerials)
				{
					// 更新F2501，已經在上面檢核 C1，這邊可直接更新狀態
					var f2501 = f2501Repo.Find(x => x.SERIAL_NO == p.SERIAL_NO && x.GUP_CODE == gupCode && x.CUST_CODE == custCode);
					f2501.STATUS = "C1";
					f2501Repo.Update(f2501);

					// 寫入F055002
					InsertOrUpdateF055002(f055001, p.ITEMCODE, 1, p.SERIAL_NO, ref addF055002List, ref updF055002List);
				}
				if (addF055002List.Any())
					f055002Repo.BulkInsert(addF055002List);
				if (updF055002List.Any())
					f055002Repo.BulkUpdate(updF055002List);
			}
			finally
			{
				// 解鎖
				var unLockf050801 = f050801Repo.AsForUpdate()
												 .Find(x => x.WMS_ORD_NO == wmsOrdNo && x.DC_CODE == dcCode
														 && x.GUP_CODE == gupCode && x.CUST_CODE == custCode);
				unLockf050801.STATUS = oldStatus;
				f050801Repo.Update(unLockf050801);
			}
			return new ExecuteResult(true);
		}

		/// <summary>
		/// 檢查序號狀態正不正確
		/// </summary>
		/// <param name="src"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <returns></returns>
		public List<SerialDataEx> CheckSerials(string dcCode, string gupCode, string custCode, string wmsOrdNo, List<string> src, bool isCombinItem = false)
		{
			var tmp = new List<SerialDataEx>();
			var f055002Repo = new F055002Repository(Schemas.CoreSchema);
			var f051202Repo = new F051202Repository(Schemas.CoreSchema);
			var serialNoService = new SerialNoService(_wmsTransaction);

			var f051202Query = f051202Repo.Filter(x => x.WMS_ORD_NO == EntityFunctions.AsNonUnicode(wmsOrdNo)
													&& x.DC_CODE == EntityFunctions.AsNonUnicode(dcCode)
													&& x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
													&& x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode));

			var f055002Query = f055002Repo.Filter(x => x.WMS_ORD_NO == EntityFunctions.AsNonUnicode(wmsOrdNo)
													&& x.DC_CODE == EntityFunctions.AsNonUnicode(dcCode)
													&& x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
													&& x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode));

			src = src.Select(s => s.Trim()).ToList();

			foreach (var p in src)
			{
				var itemCode = serialNoService.GetSerialItem(gupCode, custCode, p).ItemCode;
				var check = serialNoService.CheckBarCode(gupCode, custCode, itemCode, p, false);
				if (!check.IsSuccessed)
				{
					tmp.Add(new SerialDataEx { ITEMCODE = itemCode, MESSAGE = check.Message, ISPASS = false, SERIAL_NO = p, BOX_NO = "" });
					continue;
				}
				var serialList = check.Message.Split(',').ToList();
				var barcodeData = serialNoService.BarcodeInspection(gupCode, custCode, p);
				if (barcodeData.Barcode == BarcodeType.BatchNo && !serialNoService.IsBatchNoItem(gupCode, custCode, itemCode))
					barcodeData.Barcode = BarcodeType.SerialNo;

				foreach (var serialNo in serialList)
				{
					if (tmp.Any(x => x.SERIAL_NO == serialNo))
					{
						tmp.Add(new SerialDataEx() { ITEMCODE = itemCode, MESSAGE = Properties.Resources.P080701Service_SerialNoRepeat, ISPASS = false, SERIAL_NO = serialNo, BOX_NO = "" });
						continue;
					}
					// 序號狀態檢核
					var serialResult = serialNoService.SerialNoStatusCheck(gupCode, custCode, serialNo, "C1");
					if (!serialResult.Checked)
					{
						tmp.Add(new SerialDataEx() { ITEMCODE = itemCode, MESSAGE = serialResult.Message, ISPASS = false, SERIAL_NO = serialNo, STATUS = serialResult.CurrentlyStatus, BOX_NO = "" });
						continue;
					}
					if (!isCombinItem && !f051202Query.Any(x => x.ITEM_CODE == EntityFunctions.AsNonUnicode(itemCode)))
					{
						tmp.Add(new SerialDataEx { ITEMCODE = itemCode, MESSAGE = Properties.Resources.P080701Service_F051202QueryNotExist, ISPASS = false, SERIAL_NO = serialNo, STATUS = serialResult.CurrentlyStatus, BOX_NO = "" });
						continue;
					}
					// 檢查有無重複刷讀
					if (f055002Query.Any(x => x.SERIAL_NO == EntityFunctions.AsNonUnicode(serialNo)))
					{
						tmp.Add(new SerialDataEx() { ITEMCODE = itemCode, MESSAGE = Properties.Resources.P080201Service_SerialNoScanRepeat, ISPASS = false, SERIAL_NO = serialNo, STATUS = serialResult.CurrentlyStatus, BOX_NO = "" });
						continue;
					}
					tmp.Add(new SerialDataEx() { ITEMCODE = itemCode, MESSAGE = string.Empty, ISPASS = true, SERIAL_NO = serialNo, STATUS = serialResult.CurrentlyStatus, BOX_NO = p });
				}
			}
			return tmp;
		}

		/// <summary>
		/// 如果有指定序號出貨,檢查刷讀序號是否包含指定出貨序號
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <param name="serialStatus"></param>
		/// <returns></returns>
		public ExecuteResult CheckSerialsInOrder(string dcCode, string gupCode, string custCode, string wmsOrdNo, SerialDataEx serialStatus, IQueryable<F050802> f050802Data)
		{
			var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1903 = f1903Repo.Find(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ITEM_CODE == serialStatus.ITEMCODE);
			if (f1903 == null)
				return new ExecuteResult(false, Properties.Resources.P080701Service_CustItemNotExist);

			if (f1903.BUNDLE_SERIALLOC == "1" && !f050802Data.Any(o => o.SERIAL_NO == serialStatus.SERIAL_NO))
				return new ExecuteResult(false, string.Format(Properties.Resources.P080701Service_ItemSerialError, serialStatus.SERIAL_NO));

			return new ExecuteResult(true);
		}


		#endregion

		#region 取得小白單資料
		public IQueryable<LittleWhiteReport> GetLittleWhiteReport(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var f05030101Repo = new F05030101Repository(Schemas.CoreSchema);
			var f050301Repo = new F050301Repository(Schemas.CoreSchema);
			var f160204Repo = new F160204Repository(Schemas.CoreSchema);
			var f160202Repo = new F160202Repository(Schemas.CoreSchema);
			var f1951Repo = new F1951Repository(Schemas.CoreSchema);
			var f1908Repo = new F1908Repository(Schemas.CoreSchema);
			var f055001Repo = new F055001Repository(Schemas.CoreSchema);
			// 取得訂單編號
			var ordNo = f05030101Repo.GetDatasByTrueAndCondition(x => x.WMS_ORD_NO == wmsOrdNo).FirstOrDefault()?.ORD_NO;
			// 取得貨主單據頭檔
			var f050301 = f050301Repo.GetDatasByTrueAndCondition(x => x.ORD_NO == ordNo).FirstOrDefault();
			// 取得廠退單出貨單資料
			var f160204s = f160204Repo.GetDatasByTrueAndCondition(x => x.RTN_WMS_NO == f050301.SOURCE_NO).OrderBy(x => x.RTN_WMS_NO);
			// 取得廠退原因
			var rtnVnrCause = f160202Repo.GetDatasByTrueAndCondition(x => x.RTN_VNR_NO == f160204s.First().RTN_VNR_NO).FirstOrDefault()?.RTN_VNR_CAUSE;
			var cause = f1951Repo.GetDatasByTrueAndCondition(x => x.UCC_CODE == rtnVnrCause && x.UCT_ID == "RV").FirstOrDefault().CAUSE;
			var littleWhiteReport = new LittleWhiteReport
			{
				VNR_CODE = f160204s.FirstOrDefault().VNR_CODE,
				VNR_NAME = f1908Repo.GetDatasByTrueAndCondition(x => x.VNR_CODE == f160204s.FirstOrDefault().VNR_CODE).FirstOrDefault()?.VNR_NAME,
				SOURCE_NO = f050301.SOURCE_NO,
				CAUSE = cause
			};

			// 更新f055001.BOX_DOC
			var f055001 = f055001Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.WMS_ORD_NO == wmsOrdNo).OrderByDescending(x => x.PACKAGE_BOX_NO).FirstOrDefault();
			f055001.BOX_DOC = f050301.SOURCE_NO;

			f055001Repo.Update(f055001);
			var result = new List<LittleWhiteReport>();
			result.Add(littleWhiteReport);

      LogF05500101(dcCode, gupCode, custCode, wmsOrdNo, null, null, null, "1", "人員列印廠退出貨小白標", f055001.PACKAGE_BOX_NO);

      return result.AsQueryable();

		}
		#endregion

		#region 更新F160204
		public ExecuteResult UpdateF160204(string dcCode, string gupCode, string custCode, string wmsOrdNo)
		{
			var f05030101Repo = new F05030101Repository(Schemas.CoreSchema);
			var f050301Repo = new F050301Repository(Schemas.CoreSchema);
			var f160204Repo = new F160204Repository(Schemas.CoreSchema);

			// 取得訂單編號
			var ordNo = f05030101Repo.GetDatasByTrueAndCondition(x => x.WMS_ORD_NO == wmsOrdNo).FirstOrDefault()?.ORD_NO;
			// 取得貨主單據頭檔
			var f050301 = f050301Repo.GetDatasByTrueAndCondition(x => x.ORD_NO == ordNo).FirstOrDefault();
			// 取得廠退單出貨單資料
			var f160204s = f160204Repo.GetDatasByTrueAndCondition(x => x.RTN_WMS_NO == f050301.SOURCE_NO).OrderBy(x => x.RTN_WMS_NO);
			// 更新f160204.PROC_FLAG
			foreach (var item in f160204s)
			{
				item.PROC_FLAG = "2";
			}
			f160204Repo.BulkUpdate(f160204s);
			return new ExecuteResult { IsSuccessed = true };
		}

		#endregion

		#region
		public ExecuteResult InsertF050305(F050305 f050305)
		{

			var f050305Repo = new F050305Repository(Schemas.CoreSchema, _wmsTransaction);
			f050305Repo.Add(f050305);
			return new ExecuteResult { IsSuccessed = true };
		}
    #endregion

    #region 開始包裝前檢查
    public ExecuteResult StartPackageCheck(F050801 f050801, string shipMode)
    {
      var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
      var checkPackageMode = ShipPackageService.CheckPackageMode(f050801, shipMode);
      if (checkPackageMode.IsSuccessed)
      {
        LogF05500101(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, f050801.WMS_ORD_NO, null, null, null, "1", "開始包裝", 0);

        //var dbf050801 = f050801Repo.Find(x => x.DC_CODE == f050801.DC_CODE && x.GUP_CODE == f050801.GUP_CODE && x.CUST_CODE == f050801.CUST_CODE && x.WMS_ORD_NO == f050801.WMS_ORD_NO);
        if (!f050801.PACK_START_TIME.HasValue)
          f050801Repo.UpdateFields(new { PACK_START_TIME = DateTime.Now },
            x => x.DC_CODE == f050801.DC_CODE && x.GUP_CODE == f050801.GUP_CODE && x.CUST_CODE == f050801.CUST_CODE && x.WMS_ORD_NO == f050801.WMS_ORD_NO);

        // 新增訂單回檔歷程紀錄表
        var orderService = new OrderService(_wmsTransaction);
        orderService.AddF050305(f050801.DC_CODE, f050801.GUP_CODE, f050801.CUST_CODE, new[] { f050801.WMS_ORD_NO }.ToList(), "2");
      }
      return new ExecuteResult(checkPackageMode.IsSuccessed, checkPackageMode.MsgContent);
    }
    #endregion 開始包裝前檢查
  }
}
