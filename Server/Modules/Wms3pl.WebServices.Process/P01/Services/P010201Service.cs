
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P16.Services;
using Wms3pl.WebServices.Shared.Services;
namespace Wms3pl.WebServices.Process.P01.Services
{
	public partial class P010201Service
	{
		private WmsTransaction _wmsTransaction;
		public P010201Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public int GetInboundCnt(string dcCode, string gupCode, string custCode, List<string> stockNos)
		{
			var f010202Repo = new F010202Repository(Schemas.CoreSchema);
			return f010202Repo.GetInboundCnt(dcCode, gupCode, custCode, stockNos);
		}

		public IQueryable<F010201Data> GetF010201Datas(string dcCode, string gupCode, string custCode, string begStockDate,
			string endStockDate, string stockNo, string vnrCode, string vnrName, string custOrdNo, string sourceNo, string status)
		{
			var f010201Repo = new F010201Repository(Schemas.CoreSchema);
			return f010201Repo.GetF010201Datas(dcCode, gupCode, custCode, begStockDate, endStockDate, stockNo, vnrCode, vnrName,
				custOrdNo, sourceNo, status);
		}

		public IQueryable<F010202Data> GetF010202Datas(string dcCode, string gupCode, string custCode, string stockNo)
		{
			var f010202Repo = new F010202Repository(Schemas.CoreSchema);
			var f190301Repo = new F190301Repository(Schemas.CoreSchema);
			var itemService = new ItemService();
			var data = f010202Repo.GetF010202Datas(dcCode, gupCode, custCode, stockNo).ToList();

            var itemPackageRefs = itemService.CountItemPackageRefList(gupCode, custCode, data.Select(x => new ItemCodeQtyModel { ItemCode = x.ITEM_CODE, Qty = x.STOCK_QTY }).ToList());

            foreach (var d in data)
			{
                var currRef = itemPackageRefs.Where(x => x.ItemCode == d.ITEM_CODE).FirstOrDefault();
                d.UNIT_TRANS = currRef == null ? null : currRef.PackageRef;
			}

			return data.AsQueryable();
		}

        public ExecuteResult InsertOrUpdateP010201(F010201Data f010201Data, List<F010202Data> f010202Datas)
		{
			ExecuteResult result = null;

			bool isCreate = false;
			F010201 f010201 = null;
			var sharedService = new SharedService(_wmsTransaction);
			if (string.IsNullOrEmpty(f010201Data.STOCK_NO))
			{
				f010201Data.STOCK_NO = sharedService.GetNewOrdCode("A");
				isCreate = true;
				f010201 = new F010201();
			}

			//新增或更新主檔
			var f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransaction);
			f010201 = f010201 ?? f010201Repo.Find(o => o.STOCK_NO == f010201Data.STOCK_NO && o.DC_CODE == f010201Data.DC_CODE && o.GUP_CODE == f010201Data.GUP_CODE && o.CUST_CODE == f010201Data.CUST_CODE);

			//先檢查商品明細存不存在
			var checkItems = f010202Datas.Where(x => x.ChangeFlag != "D").Select(x => x.ITEM_CODE).ToList();
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			var items = f1903Repo.GetDatasByItems(f010201Data.GUP_CODE, f010201Data.CUST_CODE, checkItems).Select(x => x.ITEM_CODE);
			var noExistItems = checkItems.Except(items);
			if (noExistItems.Any())
			{
				return new ExecuteResult(false, string.Format("商品編號:{0}不存在", string.Join("、", noExistItems)));
			}

			f010201.SHOP_DATE = f010201Data.SHOP_DATE;
			f010201.DELIVER_DATE = f010201Data.DELIVER_DATE;
			f010201.CUST_ORD_NO = f010201Data.CUST_ORD_NO;
			f010201.CUST_COST = f010201Data.CUST_COST;
			f010201.MEMO = f010201Data.MEMO;
			f010201.ORD_PROP = f010201Data.ORD_PROP;
			f010201.SHOP_NO = f010201Data.SHOP_NO;
			f010201.FAST_PASS_TYPE = f010201Data.FAST_PASS_TYPE;
			f010201.BOOKING_IN_PERIOD = f010201Data.BOOKING_IN_PERIOD;
			if (isCreate)
			{
				f010201.STOCK_DATE = f010201Data.STOCK_DATE;
				f010201.STOCK_NO = f010201Data.STOCK_NO;
				f010201.DC_CODE = f010201Data.DC_CODE;
				f010201.GUP_CODE = f010201Data.GUP_CODE;
				f010201.CUST_CODE = f010201Data.CUST_CODE;
				f010201.VNR_CODE = f010201Data.VNR_CODE;
				f010201.SOURCE_TYPE = (string.IsNullOrEmpty(f010201Data.SOURCE_TYPE)) ? "04" : f010201Data.SOURCE_TYPE; //進倉
				f010201.SOURCE_NO = f010201Data.SOURCE_NO;
				f010201.STATUS = "0";
				f010201.FAST_PASS_TYPE = f010201Data.FAST_PASS_TYPE;
				f010201.BOOKING_IN_PERIOD = f010201Data.BOOKING_IN_PERIOD;
				f010201Repo.Add(f010201);
			}
			else
			{
				if (f010201.STATUS != "0" && f010201.STATUS != "8")
				{
					return new ExecuteResult(false, "此單據狀態已無法編輯");
				}
				//如果是異常單 轉成待處理
				if (f010201.STATUS == "8")
					f010201.STATUS = "0";
				f010201Repo.Update(f010201);
			}



			//新增或刪除明細
			var addDatas = f010202Datas.Where(o => o.ChangeFlag == "A").ToList();
			var f010202Repo = new F010202Repository(Schemas.CoreSchema, _wmsTransaction);
			if (!isCreate)
			{
				var delDatas = f010202Datas.Where(o => o.ChangeFlag == "D").ToList();
				foreach (var f010202Data in delDatas)
					f010202Repo.Delete(o => o.DC_CODE == f010201Data.DC_CODE && o.GUP_CODE == f010202Data.GUP_CODE && o.CUST_CODE == f010202Data.CUST_CODE && o.STOCK_NO == f010202Data.STOCK_NO && o.STOCK_SEQ == f010202Data.STOCK_SEQ);
			}
			foreach (var f010202Data in addDatas)
			{
                var f010202 = new F010202
                {
                    GUP_CODE = f010202Data.GUP_CODE,
                    CUST_CODE = f010202Data.CUST_CODE,
                    STOCK_NO = f010201.STOCK_NO,
                    STOCK_SEQ = short.Parse(f010202Data.STOCK_SEQ.ToString()),
                    ITEM_CODE = f010202Data.ITEM_CODE,
                    STOCK_QTY = f010202Data.STOCK_QTY,
                    VALI_DATE = f010202Data.VALI_DATE,
                    MAKE_NO = f010202Data.MAKE_NO,
                    DC_CODE = f010201.DC_CODE,
                    RECV_QTY = 0,
                    STATUS = "0"
                };
				f010202Repo.Add(f010202);
			}


			if (result == null)
				result = new ExecuteResult { IsSuccessed = true };

			return result;
		}

		public IQueryable<F010201QueryData> GetF010201SourceNo(string sourceNo)
		{
			var f010201Repo = new F010201Repository(Schemas.CoreSchema);
			return f010201Repo.GetF010201SourceNo(sourceNo);
		}

		public ExecuteResult DeleteP010201(string stockNo, string gupCode, string custCode, string dcCode)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f075101Repo = new F075101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f010201 = f010201Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode &&
							 o.CUST_CODE == custCode && o.STOCK_NO == stockNo);
			if (f010201 == null)
				return new ExecuteResult { IsSuccessed = false, Message = "該進倉單不存在" };
			//只能狀態為0的才可刪除
			if (f010201.STATUS != "0")
				return new ExecuteResult { IsSuccessed = false, Message = "狀態非為待處理，不可刪除" };

			f010201.STATUS = "9"; //更新為9取消
			f010201Repo.Update(f010201);

			//刪除F075101
			f075101Repo.DelF075101ByKey(f010201.CUST_CODE, f010201.CUST_ORD_NO);

			if (result == null)
				result = new ExecuteResult { IsSuccessed = true };
			return result;
		}

		public ExecuteResult DeleteP010201FromDB(string stockNo, string gupCode, string custCode, string dcCode)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f010202Repo = new F010202Repository(Schemas.CoreSchema, _wmsTransaction);
			f010201Repo.DeleteF010201(stockNo, dcCode, gupCode, custCode);
			f010202Repo.DeleteF010202(stockNo, dcCode, gupCode, custCode);

			if (result == null)
				result = new ExecuteResult { IsSuccessed = true };
			return result;
		}


		/// <summary>
		/// Import 進貨資料
		/// </summary>
		public ExecuteResult ImportF0201Data(string dcCode, string gupCode, string custCode
											, string fileName, List<F010201ImportData> importData)
		{
			//1. BY採購單號,廠商編號分進倉單 , 若存在(待處理. 則複蓋)
			//2. 檢查採購單號是否已存在
			//3. 檢查品號
			//4. 檢查廠商不存在則新增一廠商,新增方式參考FN_CREATE_F1908 
			//	 第2欄廠商代號-->對應商主檔的廠商代號欄，抓出相關資訊
			//5. 檢查採購單號資料是否重複 (作業類別：固定是 "A1" (預定進貨日=轉入當天日期)) 		

			ExecuteResult result = new ExecuteResult();
			string errorMessage = string.Empty;
			string dataContent = string.Empty;
			int successCtn = 0;
			int errorCtn = 0;
			int successTicketCtn = 0;
			result.IsSuccessed = true;

			var f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f010202Repo = new F010202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1908Repo = new F1908Repository(Schemas.CoreSchema, _wmsTransaction);
            var f1909Repo = new F1909Repository(Schemas.CoreSchema, _wmsTransaction);
            var p16srv = new P160101Service(_wmsTransaction);

			var itemCodes = importData.Select(x => x.ITEM_CODE).ToList();
			var itemCodePairs = f1903Repo.GetByItemCodeOrEanCode(gupCode, itemCodes);

			// 檢查商品編號是否為國際條碼，則設定為商品編號
			foreach (var item in importData)
			{
				var pair = itemCodePairs.Where(x => x.ITEM_CODE == item.ITEM_CODE ||
													x.EAN_CODE1 == item.ITEM_CODE ||
													x.EAN_CODE2 == item.ITEM_CODE ||
													x.EAN_CODE3 == item.ITEM_CODE)
										.FirstOrDefault();
				if (pair == null)
					continue;

				if (item.ITEM_CODE != pair.ITEM_CODE)
					item.ITEM_CODE = pair.ITEM_CODE;
			}

			//BY採購單號,廠商編號分進倉單
			var groupData = importData.GroupBy(o => new { o.PO_NO, o.VNR_CODE, o.VNR_NAME,o.FAST_PASS_TYPE,o.BOOKING_IN_PERIOD }).ToList();
			foreach (var groupItem in groupData)
			{
				List<F010202Data> f010202s = new List<F010202Data>();
				short detailCount = 0;
				bool addData = true;

				foreach (var item in importData.Where(o => o.PO_NO == groupItem.Key.PO_NO && o.VNR_CODE == groupItem.Key.VNR_CODE &&
				groupItem.Key.FAST_PASS_TYPE == o.FAST_PASS_TYPE && groupItem.Key.BOOKING_IN_PERIOD == o.BOOKING_IN_PERIOD)
														.GroupBy(o => new { o.ITEM_CODE, o.VALI_DATE,o.MAKE_NO}))
				{
					errorMessage = string.Empty;

					#region 檢查

					//檢查採購單號是否已存在
					//var f010201Item = new F010201();
					//if (!string.IsNullOrWhiteSpace(groupItem.Key.PO_NO))
					//{
					//	f010201Item = f010201Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode
					//				  && o.SHOP_NO == groupItem.Key.PO_NO);
					//	if (f010201Item != null && f010201Item.STATUS != "0" && f010201Item.STATUS != "9")
					//	{
					//		errorMessage += "採購單號已經存在 ;";
					//	}
					//}

					//檢查採購單號是否已存在
					var f010201Item = f010201Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode
										&& o.SHOP_NO == groupItem.Key.PO_NO);
					if (f010201Item != null && f010201Item.STATUS != "0" && f010201Item.STATUS != "9")
					{
						errorMessage += "採購單號已經存在 ;";
					}

					//檢查品號
					if (f1903Repo.Find(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.ITEM_CODE == item.Key.ITEM_CODE) == null)
					{
						errorMessage += "找不到品號 ;";
					}

					#endregion

					if (string.IsNullOrEmpty(errorMessage))
					{

						#region 取廠商編號;或新增廠商

						var vnrCode = groupItem.Key.VNR_CODE;

                        var f1909 = f1909Repo.GetAll().Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode).AsQueryable().FirstOrDefault();
                        var tempCust = f1909.ALLOWGUP_VNRSHARE == "1" ? "0" : custCode;
                        //檢查廠商不存在則新增一廠商 , (預定進貨日=轉入當天日期)
                        var vnrData = f1908Repo.Find(o => o.GUP_CODE == gupCode && o.VNR_CODE == vnrCode && o.CUST_CODE == tempCust);
                       
						if (vnrData == null)
						{
                            InsertF1908Data(gupCode, vnrCode, groupItem.Key.VNR_NAME, tempCust);
                        }

						#endregion

						//若單號存在 且為待處理狀態. 要先刪除先前單子
						if (f010201Item != null && (f010201Item.STATUS == "0" || f010201Item.STATUS == "9"))
						{
							DeleteP010201FromDB(f010201Item.STOCK_NO, f010201Item.GUP_CODE, f010201Item.CUST_CODE, f010201Item.DC_CODE);
						}
						successCtn += 1;

						#region Detail List
						detailCount += 1;

                        F010202Data f010202 = new F010202Data
                        {
                            STOCK_SEQ = detailCount,
                            ITEM_CODE = item.Key.ITEM_CODE,
                            STOCK_QTY = item.Sum(o => o.STOCK_QTY),
                            VALI_DATE = item.Key.VALI_DATE,
                            MAKE_NO = item.Key.MAKE_NO,
                            DC_CODE = dcCode,
                            GUP_CODE = gupCode,
                            CUST_CODE = custCode,
                            ChangeFlag = "A"
                        };

						f010202s.Add(f010202);
						#endregion
					}
					else
					{
						addData = false;
						errorCtn += 1;
					}

					//Log DataContent 欄位
					dataContent = string.Format("{0},{1},{2},{3},{4},{5}"
												, groupItem.Key.PO_NO, groupItem.Key.VNR_CODE, groupItem.Key.VNR_NAME, item.Key.ITEM_CODE, item.Sum(o => o.STOCK_QTY), item.Key.VALI_DATE);

					p16srv.UpdateF0060Log(fileName, "1", item.Key.ITEM_CODE, dataContent, errorMessage, dcCode, gupCode, custCode);
				}
				//同一張單若一個Item 有錯就全部不新增
				if (addData)
				{
					successTicketCtn += 1;
					InsertF010201Data(dcCode, gupCode, custCode, groupItem.Key.VNR_CODE, groupItem.Key.PO_NO, groupItem.Key.FAST_PASS_TYPE, groupItem.Key.BOOKING_IN_PERIOD, f010202s);
				}
			}
			result.Message = string.Format("匯入結果 :總筆數: {0} , 成功 {1} 筆 , 失敗 {2} 筆 \n預計匯入單據 {3} 張, 成功新增單據 {4} 張 , 失敗單據 {5} 張"
											, importData.Count, successCtn, errorCtn
											, groupData.Count, successTicketCtn, groupData.Count - successTicketCtn);

			return result;
		}
		#region 新增進倉主檔
		private void InsertF010201Data(string dcCode, string gupCode, string custCode, string vnrCode, string poNo,string fastPassType,string bookingInPeriod, List<F010202Data> f010202s)
		{
			F010201Data f010201Data = new F010201Data
			{
				STOCK_DATE = DateTime.Today,
				DELIVER_DATE = DateTime.Today,
				ORD_PROP = "A1",
				VNR_CODE = vnrCode,
				STATUS = "0",
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				SHOP_NO = poNo,
				FAST_PASS_TYPE = fastPassType,
				BOOKING_IN_PERIOD = bookingInPeriod
			};
			//使用共用 Insert Function
			var result = InsertOrUpdateP010201(f010201Data, f010202s);

		}

		#endregion

		#region 新增廠商編號
		private void InsertF1908Data(string gupCode, string vnrCode, string vnrName, string custCode)
		{

			var f1908Repo = new F1908Repository(Schemas.CoreSchema, _wmsTransaction);
			F1908 f1908 = new F1908
			{
				VNR_CODE = vnrCode,
                VNR_NAME = string.IsNullOrEmpty(vnrName) ? " " : vnrName,
                GUP_CODE = gupCode,
				CURRENCY = "NTD",
                CUST_CODE = custCode
            };
			f1908Repo.Add(f1908);
		}
		#endregion

		#region 檢查庫存量
		public IQueryable<F1913WithF1912Qty> GetF1913WithF1912Qty(string dcCode, string gupCode, string custCode,
			 string itemCode, string dataTable)
		{
			var f1913Repo = new F1913Repository(Schemas.CoreSchema);
			return f1913Repo.GetF1913WithF1912Qty(dcCode, gupCode, custCode, itemCode, dataTable);
		}

		#endregion

		#region 計算棧板標籤
		public ExecuteResult CountPallet(string dcCode, string gupCode, string custCode, string stockNo)
		{
			var f010201Repo = new F010201Repository(Schemas.CoreSchema);
			var f010201 = f010201Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.STOCK_NO == stockNo).FirstOrDefault();
			if (f010201 == null) //不存在
				return new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P010201Service_StockNoExist };
			else if (f010201.STATUS == "8") //異常
				return new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P010201Service_StockError };
			else if (f010201.STATUS == "9") //取消
				return new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P010201Service_StockCancel };
			else
			{
					return CallCountPallet(f010201);
			}
		}

		/// <summary>
		/// 計算棧板數
		/// </summary>
		/// <param name="f010201"></param>
		private ExecuteResult CallCountPallet(F010201 f010201)
		{
			var f010202Repo = new F010202Repository(Schemas.CoreSchema);
			var f010203Repo = new F010203Repository(Schemas.CoreSchema, _wmsTransaction);
			var f190305Repo = new F190305Repository(Schemas.CoreSchema);
			var f1903Repo = new F1903Repository(Schemas.CoreSchema);
			var itemService = new ItemService();
			//Step1 取得進倉單明細
			var f010202s = f010202Repo.GetDatas(f010201.DC_CODE, f010201.GUP_CODE, f010201.CUST_CODE, f010201.STOCK_NO).ToList();
			var itemCodes = f010202s.Select(x => x.ITEM_CODE).Distinct().ToList();

			//Step2 取得商品主檔、商品棧板疊法設定檔、商品材積階層檔
			var f1903s = f1903Repo.GetDatasByItems(f010201.GUP_CODE, f010201.CUST_CODE, itemCodes);
			var f190305s = f190305Repo.GetDatasByItems(f010201.GUP_CODE, f010201.CUST_CODE, itemCodes);
			var itemUnitList = itemService.GetItemUnitList(f010201.GUP_CODE, itemCodes);

			//Step3 檢查商品設定是否存在(顯示訊息 但不計算棧板貼紙)
			var noExistsItems = itemCodes.Except(f1903s.Select(y => y.ITEM_CODE)).ToList();
			if (noExistsItems.Any())
				return new ExecuteResult { IsSuccessed = false, Message = string.Format(Properties.Resources.P010201Service_NoExistItems, string.Join("、", noExistsItems)) };

			var messageList = new List<string>();
			//Step3.1 檢查商品堆疊設定(如果有會顯示訊息也會計算棧板貼紙)
			var noExistF190305s = itemCodes.Except(f190305s.Select(x => x.ITEM_CODE)).ToList();
			if (noExistF190305s.Any())
				messageList.Add(string.Format(Properties.Resources.P010201Service_NoSetItemPallect, string.Join("、", noExistF190305s)));

			//Step3.2 檢查商品階層設定(如果有會顯示訊息也會計算棧板貼紙)
			var noExistItemUnitCases = itemCodes.Except(itemUnitList.Where(x => x.UNIT_NAME == "箱").Select(x => x.ITEM_CODE));
			if (noExistItemUnitCases.Any())
				messageList.Add(string.Format(Properties.Resources.P010201Service_NoSetItemUnitCase, string.Join("、", noExistItemUnitCases)));


			//Step4 產生F010203(進倉棧板貼紙檔)
			var addF010203List = new List<F010203>();
			var seq = 1;
			foreach (var item in f010202s)
			{
				var f1902 = f1903s.First(x => x.GUP_CODE == item.GUP_CODE && x.ITEM_CODE == item.ITEM_CODE && x.CUST_CODE == item.CUST_CODE);
				var f190305 = f190305s.FirstOrDefault(x => x.GUP_CODE == item.GUP_CODE && x.CUST_CODE == item.CUST_CODE && x.ITEM_CODE == item.ITEM_CODE);
				var itemUnits = itemUnitList.Where(x => x.GUP_CODE == item.GUP_CODE && x.ITEM_CODE == item.ITEM_CODE).ToList();
				

				//箱入數
				var inCaseQty = itemService.GetUnitQty(itemUnits, "箱") ?? item.STOCK_QTY;
				//小包裝數
				var inPackageQty = itemService.GetUnitQty(itemUnits, "小包裝");
				//訂貨箱數
				var totalCaseQty = item.STOCK_QTY / inCaseQty;
				//訂貨零散數
				var otherQty = item.STOCK_QTY % inCaseQty;
                //棧板最多可放幾箱
                var palletMaxCaseQty = (f190305 == null || f190305.PALLET_LEVEL_CASEQTY < 1 || f190305.PALLET_LEVEL_CNT < 1) ? totalCaseQty : f190305.PALLET_LEVEL_CASEQTY * f190305.PALLET_LEVEL_CNT;
				//使用棧板數
				var palletQty = totalCaseQty / palletMaxCaseQty + (totalCaseQty % palletMaxCaseQty > 0 ? 1 : 0);
				if (palletQty == 0)
					palletQty = 1; //至少一板

				for (var i = 1; i <= palletQty; i++)
				{
					var orderCaseQty = 0;
					var orderOtherQty = 0;
					if (totalCaseQty > palletMaxCaseQty)
					{
						orderCaseQty = palletMaxCaseQty;
						orderOtherQty = 0;
						totalCaseQty -= palletMaxCaseQty;
					}
					else
					{
						orderCaseQty = totalCaseQty;
						orderOtherQty = otherQty;
					}

					addF010203List.Add(
                    new F010203
                    {
                        DC_CODE = item.DC_CODE,
                        GUP_CODE = item.GUP_CODE,
                        CUST_CODE = item.CUST_CODE,
                        STICKER_NO = item.STOCK_NO.Substring(1) + seq.ToString().PadLeft(4, '0'),
                        STOCK_NO = item.STOCK_NO,
                        PALLET_NO = seq.ToString().PadLeft(4, '0'),
                        ITEM_CODE = item.ITEM_CODE,
                        ENA_CODE1 = f1902.EAN_CODE1,
                        ENA_CODE3 = f1902.EAN_CODE3,
                        ITEM_CASE_QTY = inCaseQty,
                        ITEM_PACKAGE_QTY = inPackageQty,
                        PALLET_LEVEL_CASEQTY = (f190305 == null) ? 1 : f190305.PALLET_LEVEL_CASEQTY,
                        PALLET_LEVEL_CNT = (f190305 == null) ? 1 : f190305.PALLET_LEVEL_CNT,
                        VALID_DATE = item.VALI_DATE,
                        ORDER_CASE_QTY = orderCaseQty,
                        ORDER_OTHER_QTY = orderOtherQty,
                        STICKER_TYPE = "1"
                    });
                    seq++;
				}
			}

            //Step5 刪除舊F010203(進倉棧板貼紙檔)
            f010203Repo.DeleteByStockNo(f010201.DC_CODE, f010201.GUP_CODE, f010201.CUST_CODE, f010201.STOCK_NO, "1");
            //Step6 寫入F010203(進倉棧板貼紙檔)
            f010203Repo.BulkInsert(addF010203List);

            return new ExecuteResult { IsSuccessed = true ,Message = string.Join(Environment.NewLine, messageList)};
		}


		
		#endregion

		#region 列印棧板標籤資料
		/// <summary>
		/// 列印棧板標籤資料
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="stockNo"></param>
		/// <returns></returns>
		public IQueryable<P010201PalletData> GetP010201PalletDatas(string dcCode, string gupCode, string custCode, string stockNo)
		{
			var f010201Repo = new F010201Repository(Schemas.CoreSchema);
			return f010201Repo.GetPalletDatas(dcCode, gupCode, custCode, stockNo);
		}

        #endregion

        #region 廠商報到 Grid
        public IQueryable<F010202Data> GetF010202DatasMargeValidate(string dcCode, string gupCode, string custCode, string stockNo)
        {
            var f010202Repo = new F010202Repository(Schemas.CoreSchema);
            var f190301Repo = new F190301Repository(Schemas.CoreSchema);
            var itemService = new ItemService();
            var data = f010202Repo.GetF010202DatasMargeValidate(dcCode, gupCode, custCode, stockNo).ToList();
            return data.AsQueryable();
        }
				public IQueryable<F010201MainData> GetF010202DatasMargeValidateChange(string dcCode, string gupCode, string custCode, string stockNo)
				{
					var f010202Repo = new F010202Repository(Schemas.CoreSchema);
					var data = f010202Repo.GetF010202DatasMargeValidateChange(dcCode, gupCode, custCode, stockNo).ToList();
					return data.AsQueryable();
				}
		#endregion
	}
}

