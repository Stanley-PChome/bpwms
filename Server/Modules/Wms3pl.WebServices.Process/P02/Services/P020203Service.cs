using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.Datas.F00;
using Newtonsoft.Json;
using Wms3pl.WebServices.Shared.Lms.Services;
using System.IO;
using System.Text;

namespace Wms3pl.WebServices.Process.P02.Services
{
	public partial class P020203Service
	{
		protected WmsTransaction _wmsTransaction;
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
		private SerialNoService _serialNoService;
		public SerialNoService SerialNoService
		{
			get
			{
				if (_serialNoService == null)
					_serialNoService = new SerialNoService(_wmsTransaction);
				return _serialNoService;
			}
			set
			{
				_serialNoService = value;
			}
		}

		#region UpdateItemInfoService
		private UpdateItemInfoService _updatdeItemInfoService;
		public UpdateItemInfoService UpdatdeItemInfoService
		{
			get { return _updatdeItemInfoService == null ? _updatdeItemInfoService = new UpdateItemInfoService() : _updatdeItemInfoService; }
			set { _updatdeItemInfoService = value; }
		}
		#endregion

		public P020203Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		#region 取得資料
		/// <summary>
		/// 商品檢驗
		/// 檢查F02020101, 不存在則寫入, 存在則更新
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="deliveryDate"></param>
		/// <param name="RT_MODE">舊商品檢驗=0 商品檢驗與容器綁定=1</param>
		/// <returns></returns>
		public ExecuteResult Update(string dcCode, string gupCode, string custCode, string purchaseNo, string[] warehouseList, string RT_MODE)
		{
			// 進倉單鎖定
			var warehouseInRecvService = new WarehouseInRecvService(_wmsTransaction);
			var res = warehouseInRecvService.LockAcceptenceOrder(new LockAcceptenceOrderReq
			{
				DcCode = dcCode,
				GupCode = gupCode,
				CustCode = custCode,
				StockNo = purchaseNo,
				IsChangeUser = false,
				DeviceTool = "0"
			});
			if (!res.IsSuccessed)
				return new ExecuteResult(false, res.MsgContent);

			var repoF010201 = new F010201Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF010202 = new F010202Repository(Schemas.CoreSchema);
			var repoF190207 = new F190207Repository(Schemas.CoreSchema);
			var repoF020201 = new F020201Repository(Schemas.CoreSchema);
			var repoF02020101 = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05500101Repo = new F05500101Repository(Schemas.CoreSchema);
			var f1908Repo = new F1908Repository(Schemas.CoreSchema);
			var sharedService = new SharedService(_wmsTransaction);

			// 0. 檢查進倉單是否存在
			var order = repoF010201.AsForUpdate().Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.STOCK_NO == purchaseNo);
			if (order == null || order.STATUS == "9")
				return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.DataDelete };


			//No1130-3 (1)[AA] = 增加傳入參數RT_MODE，如果從舊商品檢驗請設為0，從商品檢驗與容器綁定請設為1)
			var checkRTmode = CheckRTModeValue(RT_MODE);
			if (!checkRTmode.IsSuccessed)
				return checkRTmode;


			//No1130-3 (2)	[BB]=用進倉單取得最新一筆驗收資料[F020201] AND STATUS=3
			var LastWarehouseOrder = repoF020201.GetLastOrder(dcCode, gupCode, custCode, purchaseNo);
			//如果[BB] IS NULL OR [BB.STAUTS=2(已上傳)]，跳至(3)
			//如果[BB] IS NOT NULL AND [BB].STATUS==3(綁容器) && [AA]== 0，回傳訊息”請改使用[商品檢驗與容器綁定]功能，完成上一次驗收未完成容器綁定”。
			if (LastWarehouseOrder != null)
			{
				if (RT_MODE == "0")
					return new ExecuteResult() { IsSuccessed = false, Message = "請改使用[商品檢驗與容器綁定]功能，完成上一次驗收未完成容器綁定" };

				//如果驗收內容中還有待綁定容器的就不繼續進行（不產生驗收單暫存檔）
				return new ExecuteResult() { IsSuccessed = true, No = LastWarehouseOrder.RT_NO };
			}


			//取得進倉單明細
			var orderDetail = repoF010202.GetDatas(dcCode, gupCode, custCode, purchaseNo).ToList();

			//取得驗收檔資料
			var f020201s = repoF020201.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PURCHASE_NO == purchaseNo).ToList();
			//取得驗收暫存檔資料
			var f02020101s = repoF02020101.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PURCHASE_NO == purchaseNo).ToList();
			var rtNo = string.Empty;
			if (f02020101s.Any())
				rtNo = f02020101s.First().RT_NO;

			// 1. 寫入F02020101
			bool canUpdateF010201Status = false;
			var inDate = DateTime.Now;
			var addF02020101List = new List<F02020101>();
			foreach (var p in orderDetail)
			{
				// 如果已驗收數 >= 進貨數, 就不做新增
				var totalRecv = f020201s.Where(x => x.PURCHASE_SEQ == p.STOCK_SEQ.ToString()).Sum(x => x.RECV_QTY) ?? 0;
				if (totalRecv >= p.STOCK_QTY) continue;

				// 如果已驗收數 < 進貨數, 就新增資料
				var tmp = f02020101s.Find(x => x.DC_CODE == p.DC_CODE && x.GUP_CODE == p.GUP_CODE && x.CUST_CODE == p.CUST_CODE && x.PURCHASE_NO == p.STOCK_NO && x.PURCHASE_SEQ == p.STOCK_SEQ.ToString());
				if (tmp != null) continue;

				var imageIsExist = repoF190207.GetDataIsExist(p.GUP_CODE, p.ITEM_CODE, p.CUST_CODE);

				var item = GetF1903(p.GUP_CODE, p.CUST_CODE, p.ITEM_CODE);

				//是否為虛擬商品
				bool isVirtualItem = !string.IsNullOrEmpty(item.VIRTUAL_TYPE);
				// 找出抽驗比例
				var recvQty = p.STOCK_QTY - totalRecv;
				//只有無驗收暫存資料且要產生新的驗收資料才取新的驗收單號
				if (string.IsNullOrWhiteSpace(rtNo))
					rtNo = sharedService.GetRtNo(dcCode, gupCode, custCode, Current.Staff);

				tmp = new F02020101()
				{
					RT_NO = rtNo,
					DC_CODE = p.DC_CODE,
					GUP_CODE = p.GUP_CODE,
					CUST_CODE = p.CUST_CODE,
					PURCHASE_NO = p.STOCK_NO,
					PURCHASE_SEQ = p.STOCK_SEQ.ToString(),
					VNR_CODE = order.VNR_CODE,
					ITEM_CODE = p.ITEM_CODE,
					ORDER_QTY = p.STOCK_QTY,
					RECV_QTY = recvQty, // 初始化時就先寫入驗收總量
					CHECK_QTY = sharedService.GetQtyByRatio(recvQty, p.ITEM_CODE, p.GUP_CODE, p.CUST_CODE, p.STOCK_NO),
					STATUS = "0", // 狀態初始為"待驗收"
					ISUPLOAD = imageIsExist ? "1" : "0", // 如果該商品已有圖檔就寫入1
					ISSPECIAL = "0",
					CHECK_ITEM = (isVirtualItem) ? "1" : "0",
					CHECK_SERIAL = "0",
					IN_DATE = inDate,
					//批號
					MAKE_NO = p.MAKE_NO,
					ISPRINT = "0",
					QUICK_CHECK = "0",
					DEVICE_MODE ="1",
					CHECK_DETAIL = "0",
					DEFECT_QTY = 0,
					IS_PRINT_ITEM_ID= "0"
				};

				// 沒有匯入的效期，才預設 9999/12/31
				var valiDate = p.VALI_DATE.HasValue ? p.VALI_DATE.Value : new DateTime(9999, 12, 31);
				if (isVirtualItem)
				{
					// 優先採用匯入的效期，沒有才是虛擬商品
					tmp.VALI_DATE = valiDate;
				}
				else
				{
					tmp.VALI_DATE = p.VALI_DATE;
				}
				addF02020101List.Add(tmp);
				canUpdateF010201Status = true;
			}

			// Insert廠商報到F0202,進場預排F020103,Update F010201 STATUS = 驗收中(1)
			if (canUpdateF010201Status)
			{
				if (order.STATUS == "3")
				{
					order.STATUS = "1";
					repoF010201.Update(order);
				}
			}

			#region 呼叫LmsApi上架倉別指示，檢核是否呼叫成功
			if (addF02020101List.Any())
			{
				// 呼叫LmsApi上架倉別指示，檢核是否呼叫成功
				var custInNo = order.CUST_ORD_NO;
#if (DEBUG || TEST)
				if (string.IsNullOrWhiteSpace(custInNo))
					custInNo = order.STOCK_NO;
#endif
				var lmsRes = CallLmsApiStowShelfAreaGuide(dcCode, gupCode, custCode, custInNo, addF02020101List.Select(x => x.ITEM_CODE).Distinct().ToList());
				var diffWhId = new List<LmsStowShelfAreaGuideRespense>();

				if (lmsRes.Data.Any())
				{
					addF02020101List.ForEach(addF02020101 =>
					{
						var lmsItemData = lmsRes.Data.Where(x => x.ItemCode == addF02020101.ITEM_CODE).FirstOrDefault();
						if (lmsItemData != null)
						{
							if (warehouseList.Contains(lmsItemData.WhId))
								addF02020101.TARWAREHOUSE_ID = lmsItemData.WhId;
							else
								diffWhId.Add(lmsItemData);
						}
					});
				}

				repoF02020101.BulkInsert(addF02020101List);

				if (!lmsRes.IsSucessed)
					return new ExecuteResult() { IsSuccessed = true, No = rtNo, Message = lmsRes.Msg };
				else if (diffWhId.Any())
					return new ExecuteResult() { IsSuccessed = true, No = rtNo, Message = string.Join("\r", diffWhId.Select(x => $"品號{x.ItemCode}上架倉別代碼為「{x.WhId}」，不在倉別清單內。").ToList()) };
			}
			#endregion

			return new ExecuteResult() { IsSuccessed = true, No = rtNo };
		}

		public int GetTodayRecvQty(string dcCode, string gupCode, string custCode, string purchaseNo, DateTime receDate)
		{
			var repo = new F020201Repository(Schemas.CoreSchema);
			return repo.GetTodayRecvQty(dcCode, gupCode, custCode, purchaseNo, receDate);
		}


		/// <summary>
		/// 取得要顯示的驗收單資料集 (驗收單主檔)
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="rtNo"></param>
		/// <param name="vnrCode"></param>
		/// <param name="startDt"></param>
		/// <param name="endDt"></param>
		/// <returns></returns>
		public IQueryable<P020203Data> Get(string dcCode, string gupCode, string custCode, string purchaseNo
			, string rtNo, string vnrCode, string custOrdNo, string allocationNo, string vnrNameConditon, string startDt = null, string endDt = null)
		{
			var repo = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction);
			var repo1 = new F1909Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1909 = repo1.GetAll().Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode).AsQueryable().FirstOrDefault();
			var result = repo.FindEx(dcCode, gupCode, custCode, purchaseNo, f1909.ALLOWGUP_VNRSHARE == "1" ? "0" : custCode, rtNo, vnrCode, custOrdNo, allocationNo, vnrNameConditon, startDt, endDt);
			return result;
		}

		#endregion

		#region 更新驗收數
		/// <summary>
		/// 更新驗收數, 同時取消勾選已檢驗與序號LOG檔
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="purchaseSeq"></param>
		/// <param name="recvQty"></param>
		/// <param name="userId"></param>
		/// <param name="rtNo"></param>
		/// <returns></returns>
		public ExecuteResult UpdateRecvQty(string dcCode, string gupCode, string custCode
	, string purchaseNo, string purchaseSeq, int recvQty, string userId, string rtNo)
		{
			var repo = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction);
			var result = repo.Find(x => x.DC_CODE.Equals(EntityFunctions.AsNonUnicode(dcCode))
				&& x.GUP_CODE.Equals(EntityFunctions.AsNonUnicode(gupCode))
				&& x.CUST_CODE.Equals(EntityFunctions.AsNonUnicode(custCode))
				&& x.PURCHASE_NO.Equals(EntityFunctions.AsNonUnicode(purchaseNo))
				&& x.PURCHASE_SEQ.Equals(EntityFunctions.AsNonUnicode(purchaseSeq)));
			if (result == null) return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.DataDelete };

			result.RECV_QTY = recvQty;
			var sharedService = new SharedService(_wmsTransaction);
			result.CHECK_QTY = sharedService.GetQtyByRatio(recvQty, result.ITEM_CODE, result.GUP_CODE, result.CUST_CODE,
				result.PURCHASE_NO);
			result.CHECK_ITEM = "0";
			result.CHECK_SERIAL = "0";
			result.ISSPECIAL = "0";
			repo.Update(result);

			// 2. 重設已檢驗項目
			var f02020102Repo = new F02020102Repository(Schemas.CoreSchema, _wmsTransaction);
			var f02020102s = f02020102Repo.Filter(x => x.GUP_CODE.Equals(EntityFunctions.AsNonUnicode(gupCode))
													&& x.CUST_CODE.Equals(EntityFunctions.AsNonUnicode(custCode))
													&& x.PURCHASE_NO.Equals(EntityFunctions.AsNonUnicode(purchaseNo))
													&& x.PURCHASE_SEQ.Equals(EntityFunctions.AsNonUnicode(purchaseSeq))
													&& x.ISPASS.Equals(EntityFunctions.AsNonUnicode("1")))
											.ToList();
			foreach (var f02020102 in f02020102s)
			{
				f02020102.ISPASS = "0";
				f02020102Repo.Update(f02020102);
			}

			// 3. 重設進倉商品序號刷讀紀錄項目
			var f02020104Repo = new F02020104Repository(Schemas.CoreSchema, _wmsTransaction);
			//var f02020104s = f02020104Repo.Filter(x => x.GUP_CODE.Equals(EntityFunctions.AsNonUnicode(gupCode))
			//										&& x.CUST_CODE.Equals(EntityFunctions.AsNonUnicode(custCode))
			//										&& x.PURCHASE_NO.Equals(EntityFunctions.AsNonUnicode(purchaseNo))
			//										&& x.PURCHASE_SEQ.Equals(EntityFunctions.AsNonUnicode(purchaseSeq))
			//										&& x.ISPASS.Equals(EntityFunctions.AsNonUnicode("1")))
			//								.ToList();
			//foreach (var f02020104 in f02020104s)
			//{
			//	f02020104.ISPASS = "0";
			//	f02020104Repo.Update(f02020104);
			//}

			//刪除進倉商品序號刷讀紀錄項目
			f02020104Repo.DeleteF02020104(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, rtNo);

			// 4. 重設不良品暫存檔
			var f02020109Repo = new F02020109Repository(Schemas.CoreSchema, _wmsTransaction);

			f02020109Repo.Delete(dcCode, gupCode, custCode, purchaseNo, purchaseSeq);

			return new ExecuteResult() { IsSuccessed = true };
		}

		#endregion

		#region 取得與更新商品檢驗項目、快速檢驗
		public IQueryable<F190206CheckName> GetItemCheckList(string dcCode, string gupCode, string custCode, string itemCode
			, string purchaseNo, string purchaseSeq, string rtNo, string checkType = "")
		{
			var repo = new F190206Repository(Schemas.CoreSchema);
			var result = repo.GetItemCheckList(dcCode, gupCode, custCode, itemCode, purchaseNo, purchaseSeq, rtNo, checkType);
			return result;
		}

		/// <summary>
		/// 更新商品檢驗的檢驗項目
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="purchaseSeq"></param>
		/// <param name="data"></param>
		/// <param name="dtmValidDate"></param>
		/// <returns></returns>
		public ExecuteResult UpdateF02020102(string dcCode, string gupCode, string custCode
			, string purchaseNo, string purchaseSeq, List<F190206CheckName> data, DateTime dtmValidDate, string rtNo, string checkItem, F1905 searchItem, string makeNo)
		{
			var repo = new F02020102Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF02020101 = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction);
			foreach (var p in data)
			{
				var tmp = repo.Find(x => x.DC_CODE.Equals(EntityFunctions.AsNonUnicode(dcCode))
					&& x.GUP_CODE.Equals(EntityFunctions.AsNonUnicode(gupCode))
					&& x.CUST_CODE.Equals(EntityFunctions.AsNonUnicode(custCode))
					&& x.PURCHASE_NO.Equals(EntityFunctions.AsNonUnicode(purchaseNo))
					&& x.PURCHASE_SEQ.Equals(EntityFunctions.AsNonUnicode(purchaseSeq))
					&& x.CHECK_NO.Equals(EntityFunctions.AsNonUnicode(p.CHECK_NO))
					&& x.RT_NO.Equals(EntityFunctions.AsNonUnicode(rtNo))
					);
				if (tmp == null)
				{
					tmp = new F02020102()
					{
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						PURCHASE_NO = purchaseNo,
						PURCHASE_SEQ = purchaseSeq,
						CHECK_NO = p.CHECK_NO,
						ISPASS = p.ISPASS,
						MEMO = p.MEMO,
						UCC_CODE = p.UCC_CODE,
						ITEM_CODE = p.ITEM_CODE,
						RT_NO = rtNo,
					};
					repo.Add(tmp);
				}
				else
				{
					tmp.ISPASS = p.ISPASS;
					tmp.MEMO = p.MEMO;
					tmp.UCC_CODE = p.UCC_CODE;
					repo.Update(tmp);
				}
			}

			// 更新F02020101的CHECK_ITEM欄位, 當全部檢驗通過時設為1
			var mainItem = repoF02020101.Find(x => x.DC_CODE.Equals(EntityFunctions.AsNonUnicode(dcCode))
				&& x.GUP_CODE.Equals(EntityFunctions.AsNonUnicode(gupCode))
				&& x.CUST_CODE.Equals(EntityFunctions.AsNonUnicode(custCode))
				&& x.PURCHASE_NO.Equals(EntityFunctions.AsNonUnicode(purchaseNo))
				&& x.PURCHASE_SEQ.Equals(EntityFunctions.AsNonUnicode(purchaseSeq))
				&& x.RT_NO.Equals(EntityFunctions.AsNonUnicode(rtNo)));
			if (mainItem != null)
			{
				if (string.IsNullOrEmpty(checkItem))
					mainItem.CHECK_ITEM = (data.Any(x => x.ISPASS == "0" || x.ISPASS == null) ? "0" : "1");
				else
					mainItem.CHECK_ITEM = checkItem;
				mainItem.RECE_DATE = DateTime.Today;
				mainItem.VALI_DATE = dtmValidDate;
				mainItem.MAKE_NO = makeNo;
				repoF02020101.Update(mainItem);
			}

			if (searchItem != null)
			{
				#region 更新商品主檔、商品副檔、商品材積檔

				//var repo03 = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
				var repo05 = new F1905Repository(Schemas.CoreSchema, _wmsTransaction);


				var org1905 = CommonService.GetProductSize(gupCode, custCode, searchItem.ITEM_CODE);
				if (org1905 != null)
				{
					//商品長寬高、重量有異動時，才進行更新
					if (org1905.PACK_LENGTH != searchItem.PACK_LENGTH ||
						org1905.PACK_WIDTH != searchItem.PACK_WIDTH ||
						org1905.PACK_HIGHT != searchItem.PACK_HIGHT ||
						org1905.PACK_WEIGHT != searchItem.PACK_WEIGHT)
					{
						//var size = string.Format("{0}*{1}*{2}", searchItem.PACK_LENGTH, searchItem.PACK_WIDTH, searchItem.PACK_HIGHT);

						//var item03 = CommonService.GetProduct(gupCode, custCode, searchItem.ITEM_CODE);
						////var item05 = CommonService.GetProductSize(gupCode, custCode, searchItem.ITEM_CODE);
						//if (item03 != null)
						//{
						//  //f1903 = SetObject(item03, f1903) as F1903;
						//  item03.ITEM_SIZE = size;
						//  repo03.Update(item03);
						//}

						org1905.PACK_LENGTH = searchItem.PACK_LENGTH;
						org1905.PACK_WIDTH = searchItem.PACK_WIDTH;
						org1905.PACK_HIGHT = searchItem.PACK_HIGHT;
						org1905.PACK_WEIGHT = searchItem.PACK_WEIGHT;
						repo05.Update(org1905);
					}
				}

				#endregion
			}

			return new ExecuteResult() { IsSuccessed = true };
		}

		/// <summary>
		/// 快速檢驗
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="rtNo"></param>
		/// <returns></returns>
		public ExecuteResult QuickUpdateF02020102(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo)
		{
			var result = CheckPurchaseNo(dcCode, gupCode, custCode, purchaseNo);
			if (!result.IsSuccessed)
				return result;
			var repoF190206 = new F190206Repository(Schemas.CoreSchema);
			var repo = new F02020102Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF02020101 = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction);
			var repoF1909 = new F1909Repository(Schemas.CoreSchema);
			var repoF1905 = new F1905Repository(Schemas.CoreSchema);
			var repoF1903 = new F1903Repository(Schemas.CoreSchema);
			var repoF020201 = new F020201Repository(Schemas.CoreSchema);
			var addF02020102List = new List<F02020102>();
			var updF02020101List = new List<F02020101>();
			//取得商品尚未檢驗且狀態為未驗收的商品驗收暫存檔
			var f02020101s = repoF02020101.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PURCHASE_NO == purchaseNo && x.RT_NO == rtNo && x.STATUS == "0" && x.CHECK_ITEM == "0").ToList();
			//只處理驗收數>0
			f02020101s = f02020101s.Where(x => x.RECV_QTY > 0).ToList();
			if (!f02020101s.Any())
				return new ExecuteResult(false, Properties.Resources.P020203Service_AllItemCheckSuccess);

			var itemCodes = f02020101s.Select(x => x.ITEM_CODE).Distinct().ToList();
			var checkType = "00";// 00為進倉
			var checkDatas = repoF190206.GetQuickItemCheckList(dcCode, gupCode, custCode, purchaseNo, rtNo, itemCodes, checkType);

			var checkF190205Type = CheckItemTypeIsExist(gupCode, custCode, itemCodes, checkType);

			if (!checkF190205Type.IsExist)
				return new ExecuteResult() { IsSuccessed = false, Message = checkF190205Type.ErrMessage };

			var f02020102s = repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PURCHASE_NO == purchaseNo && x.RT_NO == rtNo).ToList();
			var f1909 = repoF1909.GetDatasByTrueAndCondition(o => o.GUP_CODE == gupCode && o.CUST_CODE == custCode).FirstOrDefault();
			var f1905s = repoF1905.GetF1905ByItems(gupCode, custCode, itemCodes);
			var f1903s = repoF1903.GetDatasByItems(gupCode, custCode, itemCodes);
			var f020201s = repoF020201.GetDatas(dcCode, gupCode, custCode, purchaseNo);
			var maxDate = DateTime.MaxValue.Date;
			foreach (var u in f02020101s)
			{
				var validDate = u.VALI_DATE ?? new DateTime(9999, 12, 31);
				if (f1909.NEED_ITEMSPEC == "1")
				{
					var f1905 = f1905s.FirstOrDefault(o => o.ITEM_CODE == u.ITEM_CODE);
					if (f1905 == null)
						continue;
					else if (f1905.PACK_HIGHT == 1 && f1905.PACK_LENGTH == 1 && f1905.PACK_WIDTH == 1)
						continue;
				}
				if (f1909.ISLATEST_VALID_DATE == "1")
				{
					var tmpF020201 = f020201s.Where(o => o.ITEM_CODE == u.ITEM_CODE).ToList();
					if (tmpF020201.Any())
					{
						var f02ValiDate = tmpF020201.Max(x => x.VALI_DATE);
						if (f02ValiDate != null)
							if ((validDate <= f02ValiDate.Value && f02ValiDate.Value.Date != maxDate.Date) || (validDate < f02ValiDate.Value && f02ValiDate.Value.Date == maxDate))
								continue;
					}
				}

				var f1903 = f1903s.FirstOrDefault(o => o.ITEM_CODE == u.ITEM_CODE);
				if (f1903 != null && f1903.MAKENO_REQU == "1")
				{
					if (string.IsNullOrWhiteSpace(u.MAKE_NO) || u.MAKE_NO == "0")
						continue;
					if (f1909 != null && f1909.VALID_DATE_CHKYEAR != null)
					{
						var checkYear = (int)f1909.VALID_DATE_CHKYEAR;
						if (DateTime.Today.AddYears(checkYear) < validDate)
							continue;
					}
				}

				var existCheckNos = f02020102s.Where(x => x.DC_CODE == u.DC_CODE && x.GUP_CODE == u.GUP_CODE && x.CUST_CODE == u.CUST_CODE && x.PURCHASE_NO == u.PURCHASE_NO && x.PURCHASE_SEQ == u.PURCHASE_SEQ && x.RT_NO == u.RT_NO).Select(x => x.CHECK_NO).ToList();
				var p = checkDatas.Where(x => !existCheckNos.Contains(x.CHECK_NO) && x.ITEM_CODE == u.ITEM_CODE).ToList();
				addF02020102List.AddRange(p.Select(x => new F02020102
				{
					DC_CODE = u.DC_CODE,
					GUP_CODE = u.GUP_CODE,
					CUST_CODE = u.CUST_CODE,
					PURCHASE_NO = u.PURCHASE_NO,
					PURCHASE_SEQ = u.PURCHASE_SEQ,
					CHECK_NO = x.CHECK_NO,
					ISPASS = "1",
					MEMO = x.MEMO,
					UCC_CODE = x.UCC_CODE,
					ITEM_CODE = u.ITEM_CODE,
					RT_NO = u.RT_NO,
					CRT_STAFF = "system",
					CRT_NAME = "system"
				}));

				// 更新F02020101的CHECK_ITEM欄位, 當全部檢驗通過時設為1
				u.CHECK_ITEM = "1";
				u.QUICK_CHECK = "1";
				u.RECE_DATE = DateTime.Today;
				u.VALI_DATE = u.VALI_DATE;
				updF02020101List.Add(u);
			}
			repo.BulkInsert(addF02020102List);
			repoF02020101.BulkUpdate(updF02020101List);
			string message = string.Empty;
			if (addF02020102List.Select(x => x.PURCHASE_SEQ).Distinct().Count() != f02020101s.Count)
			{
				message = string.Format(Properties.Resources.P020203Service_QuickCheckMessage, f02020101s.Count - addF02020102List.Count);
			}
			return new ExecuteResult() { IsSuccessed = true, Message = message };
		}

		/// <summary>
		/// 檢驗該商品是否有設置檢驗項目
		/// </summary>
		/// <param name="gupCode">業主</param>
		/// <param name="custCode">貨主</param>
		/// <param name="itemCodes">商品清單</param>
		/// <param name="checkType">檢驗類別(00:進倉 02:退貨)</param>
		/// <returns></returns>
		public ItemTypeCheckData CheckItemTypeIsExist(string gupCode, string custCode, List<string> itemCodes, string checkType)
		{
			var result = new ItemTypeCheckData() { IsExist = true };

			var repoF1903 = new F1903Repository(Schemas.CoreSchema);
			var repoF190205 = new F190205Repository(Schemas.CoreSchema);
			var repoF190206 = new F190206Repository(Schemas.CoreSchema);
			var repoF000904 = new F000904Repository(Schemas.CoreSchema);

			//取得需要的資料
			var f000904Data = repoF000904.GetF000904Data("F1902", "TYPE");
			var f1903Temp = repoF1903.GetDatasByItems(gupCode, custCode, itemCodes);
			var f190205s = repoF190205.GetDatasByTrueAndCondition(x => x.CHECK_TYPE == checkType);
			var f190206s = repoF190206.GetDatas(gupCode, custCode, checkType, itemCodes);
			//用來暫存未設定的資料Type
			List<string> notExistType = new List<string>();

			foreach (var item in f1903Temp)
			{
				var f190205 = f190205s.FirstOrDefault(o => o.TYPE == item.TYPE);
				if (f190205 == null)
				{
					if (!f190206s.Any(x => x.ITEM_CODE == item.ITEM_CODE))
					{
						if (!notExistType.Contains(item.TYPE))
						{
							notExistType.Add(item.TYPE);
						}
					}
				}
			}

			if (notExistType.Any())
			{
				var typeNameTemp = new List<string>();
				result.IsExist = false;
				foreach (var type in notExistType)
				{
					var item = f000904Data.FirstOrDefault(x => x.VALUE == type);
					if (item != null)
						typeNameTemp.Add(item.NAME);
					else
						typeNameTemp.Add(type);
				}
				result.ErrMessage = string.Format(Properties.Resources.P020203ItemTypeSet, string.Join("、", typeNameTemp));
			}
			return result;
		}

		#endregion

		#region 上傳圖檔

		/// <summary>
		/// 上傳圖檔
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="userId"></param>
		/// <returns>圖檔流水號 (IMAGE_NO)</returns>
		public ExecuteResult UploadItemImage(string dcCode, string gupCode, string custCode, string purchaseNo, string purchaseSeq, string itemCode, string imagePath, string userId, string fileExt = ".jpg")
		{
			var repo = new F190207Repository(Schemas.CoreSchema, _wmsTransaction);
			// 取得新ID
			var newId = repo.GetNewId(gupCode, itemCode, custCode);

			// 寫入圖檔資料
			var item = new F190207()
			{
				GUP_CODE = gupCode,
				IMAGE_NO = newId,
				IMAGE_PATH = imagePath + itemCode + newId.ToString() + fileExt,
				ITEM_CODE = itemCode,
				CRT_DATE = DateTime.Now,
				CRT_STAFF = userId,
				CRT_NAME = userId
			};
			repo.Add(item);

			// 標記F02020101的ISUPLOAD欄位為已上傳
			var repoF02020101 = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction);
			var tmp = repoF02020101.Find(x => x.DC_CODE.Equals(dcCode) && x.GUP_CODE.Equals(gupCode)
				&& x.CUST_CODE.Equals(custCode) && x.PURCHASE_NO.Equals(purchaseNo) && x.PURCHASE_SEQ.Equals(purchaseSeq));
			if (tmp == null) return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.DataDelete };
			tmp.ISUPLOAD = "1";
			tmp.UPD_DATE = DateTime.Now;
			tmp.UPD_STAFF = userId;
			tmp.UPD_NAME = userId;
			repoF02020101.Update(tmp);

			// 標記F020201的ISUPLOAD為已上傳
			var repoF020201 = new F020201Repository(Schemas.CoreSchema, _wmsTransaction);
			foreach (var p in repoF020201.Filter(x => x.GUP_CODE == gupCode && x.ITEM_CODE == itemCode))
			{
				p.ISUPLOAD = "1";
				repoF020201.Update(p);
			}

			return new ExecuteResult() { IsSuccessed = true, Message = newId.ToString() };
		}

		#endregion

		#region 序號刷讀 與 序號收集
		/// <summary>
		/// 隨機抽驗序號
		/// </summary>
		/// <param name="p020203Data"></param>
		/// <param name="rtNo"></param>
		/// <param name="newSerialNo"></param>
		/// <returns></returns>
		public ExecuteResult RandomCheckSerial(P020203Data p020203Data, string rtNo, string newSerialNo)
		{
			var f020302Repo = new F020302Repository(Schemas.CoreSchema);
			var f02020104Repo = new F02020104Repository(Schemas.CoreSchema);
			var f2501Repo = new F2501Repository(Schemas.CoreSchema);

			if (string.IsNullOrWhiteSpace(newSerialNo))
			{
				return new ExecuteResult(false, Properties.Resources.SerialNoIsNull);
			}

			// 檢查實刷總數是否大於等於應刷數(抽驗數)
			var checkedCount = f02020104Repo.Filter(x => x.PURCHASE_NO == EntityFunctions.AsNonUnicode(p020203Data.PURCHASE_NO)
														&& x.PURCHASE_SEQ == EntityFunctions.AsNonUnicode(p020203Data.PURCHASE_SEQ)
														&& x.RT_NO == EntityFunctions.AsNonUnicode(rtNo)
														&& x.DC_CODE == EntityFunctions.AsNonUnicode(p020203Data.DC_CODE)
														&& x.GUP_CODE == EntityFunctions.AsNonUnicode(p020203Data.GUP_CODE)
														&& x.CUST_CODE == EntityFunctions.AsNonUnicode(p020203Data.CUST_CODE)
														&& x.ITEM_CODE == EntityFunctions.AsNonUnicode(p020203Data.ITEM_CODE)
														&& x.ISPASS == EntityFunctions.AsNonUnicode("1"))
											.Count();

			if (checkedCount >= p020203Data.CHECK_QTY)
			{
				return new ExecuteResult(false, Properties.Resources.CheckQtyError);
			}

			// 檢查序號是否已經重複刷讀於 LOG 中
			newSerialNo = newSerialNo.Trim();
			var isRepeatScan = f02020104Repo.Filter(x => x.PURCHASE_NO == EntityFunctions.AsNonUnicode(p020203Data.PURCHASE_NO)
														&& x.PURCHASE_SEQ == EntityFunctions.AsNonUnicode(p020203Data.PURCHASE_SEQ)
														&& x.RT_NO == EntityFunctions.AsNonUnicode(rtNo)
														&& x.DC_CODE == EntityFunctions.AsNonUnicode(p020203Data.DC_CODE)
														&& x.GUP_CODE == EntityFunctions.AsNonUnicode(p020203Data.GUP_CODE)
														&& x.CUST_CODE == EntityFunctions.AsNonUnicode(p020203Data.CUST_CODE)
														&& x.ITEM_CODE == EntityFunctions.AsNonUnicode(p020203Data.ITEM_CODE)
														&& x.SERIAL_NO == EntityFunctions.AsNonUnicode(newSerialNo)
														&& x.ISPASS == "1")
											.Any();
			if (isRepeatScan)
			{
				return new ExecuteResult(false, Properties.Resources.SerialNoExist);
			}

			p020203Data.SHOP_NO = p020203Data.SHOP_NO ?? string.Empty;

			// 檢查目前刷的序號是否存在於進倉驗收檔(序號收集)。 備註:PO_NO可能是進倉單號或是採購單號(EDI)
			bool existsSerial = f020302Repo.Filter(x => (x.PO_NO == EntityFunctions.AsNonUnicode(p020203Data.PURCHASE_NO) || x.PO_NO == EntityFunctions.AsNonUnicode(p020203Data.SHOP_NO))
														&& x.DC_CODE == EntityFunctions.AsNonUnicode(p020203Data.DC_CODE)
														&& x.GUP_CODE == EntityFunctions.AsNonUnicode(p020203Data.GUP_CODE)
														&& x.CUST_CODE == EntityFunctions.AsNonUnicode(p020203Data.CUST_CODE)
														&& x.ITEM_CODE == EntityFunctions.AsNonUnicode(p020203Data.ITEM_CODE)
														&& x.SERIAL_NO == EntityFunctions.AsNonUnicode(newSerialNo)
														&& x.STATUS == EntityFunctions.AsNonUnicode("0"))
											.Any();

			var status = f2501Repo.Filter(x => x.SERIAL_NO == EntityFunctions.AsNonUnicode(newSerialNo)
											&& x.GUP_CODE == EntityFunctions.AsNonUnicode(p020203Data.GUP_CODE)
											&& x.CUST_CODE == EntityFunctions.AsNonUnicode(p020203Data.CUST_CODE))
									.Select(x => x.STATUS)
									.FirstOrDefault();

			if (existsSerial)
			{
				return InsertF2501(p020203Data.DC_CODE,
									p020203Data.GUP_CODE,
									p020203Data.CUST_CODE,
									p020203Data.PURCHASE_NO,
									p020203Data.PURCHASE_SEQ,
									p020203Data.ITEM_CODE,
									new List<SerialNoResult> { new SerialNoResult() { Checked = true, SerialNo = newSerialNo, CurrentlyStatus = status } },
									isScanSerial: true,
									rtNo: rtNo,
									f020302List: null);
			}
			else
			{
				// 在序號收集的表裡面找不到該序號，就寫入錯誤 LOG 紀錄
				var maxLogSeq = f02020104Repo.Filter(x => x.PURCHASE_NO == EntityFunctions.AsNonUnicode(p020203Data.PURCHASE_NO)
														&& x.PURCHASE_SEQ == EntityFunctions.AsNonUnicode(p020203Data.PURCHASE_SEQ)
														&& x.RT_NO == EntityFunctions.AsNonUnicode(rtNo)
														&& x.DC_CODE == EntityFunctions.AsNonUnicode(p020203Data.DC_CODE)
														&& x.GUP_CODE == EntityFunctions.AsNonUnicode(p020203Data.GUP_CODE)
														&& x.CUST_CODE == EntityFunctions.AsNonUnicode(p020203Data.CUST_CODE))
											 .Max(x => (int?)x.LOG_SEQ);
				var nextLogSeq = maxLogSeq.HasValue ? maxLogSeq.Value + 1 : 1;

				return InsertF02020104(p020203Data.DC_CODE,
									p020203Data.GUP_CODE,
									p020203Data.CUST_CODE,
									p020203Data.PURCHASE_NO,
									p020203Data.PURCHASE_SEQ,
									p020203Data.ITEM_CODE,
									newSerialNo,
									status,
									isPass: "0",
									message: Properties.Resources.F02020104NotExist,
									logSeq: nextLogSeq,
									rtNo: rtNo, batchNo: "");
			}
		}

		/// <summary>
		/// 寫入F02020104單筆資料, 給序號收集使用.
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="purchaseSeq"></param>
		/// <param name="itemCode"></param>
		/// <param name="serialNo"></param>
		/// <param name="status"></param>
		/// <param name="isPass"></param>
		/// <param name="message"></param>
		/// <param name="logSeq"></param>
		/// <returns></returns>
		public ExecuteResult InsertF02020104(string dcCode, string gupCode, string custCode
			, string purchaseNo, string purchaseSeq, string itemCode, string serialNo, string status, string isPass, string message, int logSeq, string rtNo, string batchNo)
		{
			var repo = new F02020104Repository(Schemas.CoreSchema, _wmsTransaction);
			F02020104 data = CreateF02020104(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, itemCode, serialNo, status, isPass, message, logSeq, rtNo, batchNo);
			repo.Add(data);
			return new ExecuteResult() { IsSuccessed = true };
		}

		/// <summary>
		/// 寫入F02020104單筆資料, 給序號收集使用.
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="purchaseSeq"></param>
		/// <param name="itemCode"></param>
		/// <param name="serialNo"></param>
		/// <param name="status"></param>
		/// <param name="isPass"></param>
		/// <param name="message"></param>
		/// <param name="preLogSeq"></param>
		/// <returns></returns>
		public ExecuteResult InsertF02020104s(string dcCode, string gupCode, string custCode
			, string purchaseNo, string purchaseSeq, string itemCode, List<SerialNoResult> serialNoResults, string isPass, int preLogSeq, string rtNo)
		{
			var repo = new F02020104Repository(Schemas.CoreSchema, _wmsTransaction);
			var datas = new List<F02020104>();
			var importSerailCount = 0;
			foreach (var item in serialNoResults)
			{
				importSerailCount++;
				var data = CreateF02020104(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, itemCode, item.SerialNo, item.CurrentlyStatus, isPass, item.Message, preLogSeq + importSerailCount, rtNo, item.BatchNo);
				datas.Add(data);
			}

			repo.BulkInsert(datas);
			return new ExecuteResult() { IsSuccessed = true };
		}

		private F02020104 CreateF02020104(string dcCode, string gupCode, string custCode, string purchaseNo, string purchaseSeq, string itemCode, string serialNo, string status, string isPass, string message, int logSeq, string rtNo, string batchNo)
		{
			return new F02020104
			{
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				ISPASS = isPass,
				ITEM_CODE = itemCode,
				PURCHASE_NO = purchaseNo,
				PURCHASE_SEQ = purchaseSeq,
				SERIAL_NO = serialNo,
				STATUS = string.IsNullOrWhiteSpace(status) ? null : status,
				LOG_SEQ = logSeq,
				RT_NO = rtNo,
				BATCH_NO = batchNo,
				MESSAGE = message
			};
		}

		/// <summary>
		/// 序號收集 - 寫入F2501及F02020104
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="purchaseSeq"></param>
		/// <param name="itemCode"></param>
		/// <param name="serialNoResults"></param>
		/// <param name="isScanSerial">是否為序號刷讀 是:序號刷讀 否:序號收集  (主要是序號刷讀時不更新F2501)</param>
		/// <returns></returns>
		public ExecuteResult InsertF2501(string dcCode, string gupCode, string custCode, string purchaseNo, string purchaseSeq
			, string itemCode, List<SerialNoResult> serialNoResults, bool isScanSerial, string rtNo, List<F020302Data> f020302List)
		{
			var sharedService = new SharedService(_wmsTransaction);
			var f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f02020104Repo = new F02020104Repository(Schemas.CoreSchema, _wmsTransaction);
			var f020301Repo = new F020301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f020302Repo = new F020302Repository(Schemas.CoreSchema, _wmsTransaction);
			var f02020109Repo = new F02020109Repository(Schemas.CoreSchema, _wmsTransaction);
			var orginalData = f02020104Repo.GetDatas(dcCode, gupCode, custCode, purchaseNo, purchaseSeq);
			var f010201Item = f010201Repo.Find(o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.STOCK_NO == purchaseNo);
			var f020302Data = f020301Repo.GetF020302Data(dcCode, gupCode, custCode, purchaseNo, itemCode);
			var searialList = f020302Data.Select(o => o.SERIAL_NO).ToList();
			var newSerialNos = serialNoResults.Select(s => s.SerialNo).ToList();

			var f02020104ExistSerialNos = new List<string>();
			var totalCount = newSerialNos.Count;
			var range = 5000;
			int loopCnt = totalCount / range + (totalCount % range > 0 ? 1 : 0);
			for (var i = 0; i < loopCnt; i++)
			{
				f02020104ExistSerialNos.AddRange(f02020104Repo.GetExistSerialNos(dcCode, gupCode, custCode, itemCode, purchaseNo, rtNo, newSerialNos, "1").ToList());
			}
			var f02020104NotExistSerialNoResults = serialNoResults.Where(s => !f02020104ExistSerialNos.Contains(s.SerialNo)).ToList();
			InsertF02020104s(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, itemCode, f02020104NotExistSerialNoResults, "1", orginalData.Count(), rtNo);
			var importSerailCount = f02020104NotExistSerialNoResults.Count;

			//收集序號來的需 : 新增 F020301 F020302
			var f020301FileName = "";
			if (f020302List != null)
			{
				var fileseq = f020301Repo.GetF020301FileSeq(dcCode, gupCode, custCode, purchaseNo, f010201Item.SHOP_NO) + 1;

				// 刪除進倉序號檔，並將刷讀通過的序號改為不通過
				var delSerialNos = searialList.Where(s => !f020302List.Any(x => x.SERIAL_NO == s));

				if (delSerialNos.Any())
				{
					f020302Repo.DeleteInWithTrueAndCondition(
						keyCondition: o => o.DC_CODE == dcCode
											&& o.GUP_CODE == gupCode
											&& o.CUST_CODE == custCode
											&& o.PO_NO == f010201Item.SHOP_NO
											&& o.ITEM_CODE == itemCode,
						InFieldName: o => o.SERIAL_NO,
						InValues: delSerialNos);

					f02020104Repo.UpdateFieldsInWithTrueAndCondition(
						SET: new { ISPASS = "0" },
						WHERE: o => o.DC_CODE == dcCode
											&& o.GUP_CODE == gupCode
											&& o.CUST_CODE == custCode
											&& o.ITEM_CODE == itemCode
											&& o.PURCHASE_NO == purchaseNo
											&& o.PURCHASE_SEQ == purchaseSeq
											&& o.RT_NO == rtNo
											&& o.ISPASS == "1",
						InFieldName: o => o.SERIAL_NO,
						InValues: delSerialNos);
				}

				var query = f020302List.Where(x => !searialList.Any(s => s == x.SERIAL_NO));
				if (query.Any())
				{
					f020301FileName = string.Format("USERCHK99_{0}{1}", purchaseNo, fileseq.ToString("D2"));

					F020301 f020301 = new F020301
					{
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						PURCHASE_NO = purchaseNo,
						FILE_NAME = f020301FileName,

					};
					f020301Repo.Add(f020301);

					if (query.Any())
					{
						var f020302s = new List<F020302>();
						foreach (var addItem in query)
						{
							F020302 f020302 = new F020302
							{
								DC_CODE = dcCode,
								GUP_CODE = gupCode,
								CUST_CODE = custCode,
								FILE_NAME = f020301FileName,
								PO_NO = f010201Item.SHOP_NO,
								ITEM_CODE = itemCode,
								SERIAL_NO = addItem.SERIAL_NO,
								SERIAL_LEN = (short)addItem.SERIAL_NO.Length,
								VALID_DATE = DateTime.MaxValue.Date,
								STATUS = "0",
								BATCH_NO = addItem.BATCH_NO
							};
							f020302s.Add(f020302);
						}
						f020302Repo.BulkInsert(f020302s);
					}
				}
				//將畫面上資料ISPASS="1"
				var updateOrginalData = orginalData.Where(x => f020302List.Select(y => y.SERIAL_NO)
				.Contains(x.SERIAL_NO)).ToList();
				if (updateOrginalData.Any())
				{
					f02020104Repo.BulkUpdate(updateOrginalData);
				}
			}

			//當變更驗收總量會將ISPASS= "0"，所以不取ISPASS= "1"
			int orginalCount = orginalData.Select(o => o.SERIAL_NO).ToList().Distinct().Count();
			//int orginalCount = orginalData.Where(o => o.ISPASS == "1").Select(o => o.SERIAL_NO).ToList().Distinct().Count();


			// 序號匯入後, 直接更新F02020101的CHECK_SERIAL欄位
			var repoF02020101 = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction);
			var tmp = repoF02020101.Find(x => x.DC_CODE.Equals(EntityFunctions.AsNonUnicode(dcCode))
				&& x.GUP_CODE.Equals(EntityFunctions.AsNonUnicode(gupCode))
				&& x.CUST_CODE.Equals(EntityFunctions.AsNonUnicode(custCode))
				&& x.PURCHASE_NO.Equals(EntityFunctions.AsNonUnicode(purchaseNo))
				&& x.PURCHASE_SEQ.Equals(EntityFunctions.AsNonUnicode(purchaseSeq))
				&& x.RT_NO.Equals(EntityFunctions.AsNonUnicode(rtNo)));
			if (tmp != null)
			{
				var f1903Repo = new F1903Repository(Schemas.CoreSchema);
				var item = f1903Repo.Find(o => o.GUP_CODE == gupCode && o.ITEM_CODE == itemCode && o.CUST_CODE == custCode);
				if ((string.IsNullOrEmpty(item.VIRTUAL_TYPE) && (orginalCount + importSerailCount) >= tmp.CHECK_QTY) || (!string.IsNullOrEmpty(item.VIRTUAL_TYPE) && (orginalCount + importSerailCount) == tmp.RECV_QTY))
				{
					tmp.CHECK_SERIAL = "1";
					repoF02020101.Update(tmp);
				}
				else if (!string.IsNullOrEmpty(item.VIRTUAL_TYPE) && (orginalCount + importSerailCount) > tmp.RECV_QTY)
				{
					return new ExecuteResult() { IsSuccessed = false, Message = string.Format(Properties.Resources.VirtualTtypError1 + Environment.NewLine + Properties.Resources.VirtualTtypError2, tmp.RECV_QTY, orginalCount, importSerailCount) };
				}
			}

			// 刪除不存在序號收集內的不良品序號
			if (f020302List != null)
			{
				var serialList = f020302List.Select(o => o.SERIAL_NO).ToList();
				var f202020109 = f02020109Repo.GetF02020109Datas(dcCode, gupCode, custCode, purchaseNo, Convert.ToInt32(purchaseSeq)).ToList();
				var serialNoNotExistF202020109 = f202020109.Where(o => !serialList.Contains(o.SERIAL_NO)).Select(o => o.SERIAL_NO).ToList();
				f02020109Repo.Delete(dcCode, gupCode, custCode, purchaseNo, purchaseSeq, serialNoNotExistF202020109);
			}



			return new ExecuteResult() { IsSuccessed = true };
		}

		#endregion

		#region 更新特殊採購
		/// <summary>
		/// 更新特殊採購資訊
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="data"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public ExecuteResult UpdateP02020304(string dcCode, string gupCode, string custCode, List<P020203Data> data, string userId)
		{
			var repo = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction);
			foreach (var p in data)
			{
				var tmp = repo.Find(x => x.DC_CODE.Equals(EntityFunctions.AsNonUnicode(dcCode))
					&& x.GUP_CODE.Equals(EntityFunctions.AsNonUnicode(gupCode))
					&& x.CUST_CODE.Equals(EntityFunctions.AsNonUnicode(custCode))
					&& x.PURCHASE_NO.Equals(EntityFunctions.AsNonUnicode(p.PURCHASE_NO))
					&& x.PURCHASE_SEQ.Equals(EntityFunctions.AsNonUnicode(p.PURCHASE_SEQ)));
				if (tmp == null) return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.DataDelete };

				tmp.ISSPECIAL = p.ISSPECIAL;
				tmp.SPECIAL_CODE = p.SPECIAL_CODE;
				tmp.SPECIAL_DESC = p.SPECIAL_DESC;
				tmp.UPD_DATE = DateTime.Now;
				tmp.UPD_STAFF = userId;

				// 如果勾選特殊採購, 將CHECK_ITEM, CHECK_SERIAL也一併勾選
				// 如果反勾選特殊採購, 就不做更新這2個欄位
				if (tmp.ISSPECIAL == "1" && p.BUNDLE_SERIALNO == "1")
				{
					tmp.CHECK_ITEM = "1";
					tmp.CHECK_SERIAL = "1";
				}
				repo.Update(tmp);
			}
			return new ExecuteResult() { IsSuccessed = true };
		}

		#endregion

		#region 商品檢驗_列印驗收後棧板貼紙

		/// <summary>
		/// 商品檢驗_列印驗收後棧板貼紙
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="rtNo">驗收單號</param>
		/// <returns></returns>
		public IQueryable<P0202030500PalletData> GetP0202030500PalletDatas(string dcCode, string gupCode, string custCode, string rtNo)
		{
			var f010203Repo = new F010203Repository(Schemas.CoreSchema);
			return f010203Repo.GetPalletDatas(dcCode, gupCode, custCode, rtNo);
		}

		#endregion

		#region 驗收確認後 產生驗收單報表
		/// <summary>
		/// 取得驗收單報表
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="rtNo"></param>
		/// <returns></returns>
		public IQueryable<AcceptancePurchaseReport> GetAcceptancePurchaseReport(string dcCode, string gupCode, string custCode
			, string purchaseNo, string rtNo, bool isDefect, bool isAcceptanceContainer)
		{
			var repo = new F020201Repository(Schemas.CoreSchema);

			var repo1 = new F1909Repository(Schemas.CoreSchema);
			var f1909 = repo1.GetAll().Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode).AsQueryable().FirstOrDefault();
			var result = new List<AcceptancePurchaseReport>();
			// 是否為驗收單與上架容器查詢列印
			if (isAcceptanceContainer)
			{
				result = repo.GetAcceptancePurchaseContainerReport(dcCode, gupCode, custCode, purchaseNo, rtNo, f1909.ALLOWGUP_VNRSHARE == "1" ? "0" : custCode, isDefect).ToList();
			}
			else
			{
				result = repo.GetAcceptancePurchaseReport(dcCode, gupCode, custCode, purchaseNo, rtNo, f1909.ALLOWGUP_VNRSHARE == "1" ? "0" : custCode, isDefect).ToList();
			}

			var itemService = new ItemService();
			foreach (var item in result)
			{
				var itemPackageRefs = itemService.CountItemPackageRefList(gupCode, custCode, new List<ItemCodeQtyModel> { new ItemCodeQtyModel { ItemCode = item.ITEM_CODE, Qty = (int)item.RECV_QTY } });

				var currRef = itemPackageRefs.Where(x => x.ItemCode == item.ITEM_CODE).FirstOrDefault();

				item.VOLUME_UNIT = currRef == null ? null : currRef.PackageRef;
			}

			return result.AsQueryable();
		}

		#endregion

		#region 檔案上傳後更新狀態

		public IQueryable<FileUploadData> GetFileUploadSetting(string dcCode, string gupCode, string custCode,
			string purchaseNo, string rtNo, bool includeAllItems = false)
		{
			var repo = new F02020106Repository(Schemas.CoreSchema);
			return repo.GetFileUploadSetting(dcCode, gupCode, custCode, purchaseNo, rtNo, includeAllItems);
		}

		/// <summary>
		/// 檔案上傳後更新狀態
		/// 1.更新驗收單狀態:已上傳
		/// 2.刪除進倉驗收暫存檔
		/// 3.更新進倉單狀態:已結案
		/// 4.更新來源單狀態:已結案(呼叫共用Method)
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="rtNo"></param>
		/// <param name="includeAllItems"></param>
		/// <returns></returns>
		public ExecuteResult UpdateStatusByAfterUploadFile(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo, bool includeAllItems = false)
		{
			var executeResult = new ExecuteResult { IsSuccessed = true, Message = "" };
			//取得檔案上傳資訊. 包含商品圖檔和其它檔案 (F02020106裡設定的檔案)
			var repo = new F02020106Repository(Schemas.CoreSchema);
			var f020201Repo = new F020201Repository(Schemas.CoreSchema, _wmsTransaction);
			var sharedService = new SharedService(_wmsTransaction);
			var fileUploadDatas = repo.GetFileUploadSetting(dcCode, gupCode, custCode, purchaseNo, rtNo, includeAllItems).ToList();

			// 若商品圖檔有上傳，就更新為已上傳圖檔
			var itemImageItemCodes = fileUploadDatas.Where(x => x.UPLOAD_TYPE == "00" && x.UPLOADED_COUNT > 0).Select(x => x.ITEM_CODE).ToList();
			if (itemImageItemCodes.Any())
			{
				var setParam = new F020201 { ISUPLOAD = "1" };
				f020201Repo.UpdateFieldsInWithTrueAndCondition(SET: new { setParam.ISUPLOAD },
																WHERE: o => o.DC_CODE == dcCode
																			&& o.GUP_CODE == gupCode
																			&& o.CUST_CODE == custCode
																			&& o.RT_NO == rtNo,
																InFieldName: x => x.ITEM_CODE,
																InValues: itemImageItemCodes);
			}

			// 當必要項都上傳後，就將驗收單所有明細狀態更改為已上傳，並將驗收暫存驗收檔刪除
			if (fileUploadDatas.Where(x => x.ISREQUIRED == "1").All(x => x.UPLOADED_COUNT > 0))
			{
				PurchaseClosed(dcCode, gupCode, custCode, purchaseNo, rtNo);
			}

			return executeResult;
		}
		#endregion

		#region 檢查F020301 是否有資料
		public ExecuteResult ExistsF020301Data(string dcCode, string gupCode, string custCode, string purchaseNo, string itemCode)
		{
			var repoF010201 = new F010201Repository(Schemas.CoreSchema);

			//有資料則不需提示  
			if (!repoF010201.ExistsF020301Data(dcCode, gupCode, custCode, purchaseNo, itemCode))
			{
				return new ExecuteResult(false, Properties.Resources.F020301DataNotExist);
			}

			return new ExecuteResult(true, Properties.Resources.F020301DataExist);
		}
		#endregion

		#region 新增F020301 & F020302

		public ExecuteResult InserF020301AndF020302(string dcCode, string gupCode, string custCode, List<P020205Detail> data)
		{
			var f020301Repo = new F020301Repository(Schemas.CoreSchema, _wmsTransaction);
			var f020302Repo = new F020302Repository(Schemas.CoreSchema, _wmsTransaction);
			var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransaction);

			if (data != null && data.Any())
			{
				var headerData = data.First();

				//檢查資料
				var checkData = f020301Repo.Find(
					x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.FILE_NAME == headerData.FILE_NAME);
				if (checkData != null)
				{
					return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.FileNameExist };
				}

				var f020301 = new F020301
				{
					DC_CODE = dcCode,
					GUP_CODE = gupCode,
					CUST_CODE = custCode,
					FILE_NAME = headerData.FILE_NAME,
				};
				f020301Repo.Add(f020301);

				foreach (var d in data)
				{
					var f020302 = new F020302
					{
						DC_CODE = dcCode,
						GUP_CODE = gupCode,
						CUST_CODE = custCode,
						FILE_NAME = headerData.FILE_NAME,
						PO_NO = d.PO_NO,
						ITEM_CODE = d.ITEM_CODE,
						SERIAL_NO = d.SERIAL_NO,
						SERIAL_LEN = d.SERIAL_LEN,
						VALID_DATE = d.VALID_DATE,
						STATUS = "0",
					};
					f020302Repo.Add(f020302);
				}

				//foreach (var d in data)
				//{
				//	var serialNoData =
				//		f2501Repo.Find(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.SERIAL_NO == d.SERIAL_NO);
				//	if (serialNoData == null)
				//	{
				//		var newSerial = new F2501
				//		{
				//			DC_CODE = dcCode,
				//			GUP_CODE = gupCode,
				//			CUST_CODE = custCode,
				//			SERIAL_NO = d.SERIAL_NO,
				//			STATUS = "A1",
				//			ITEM_CODE = d.ITEM_CODE
				//		};
				//		f2501Repo.Add(newSerial);
				//	}
				//	else
				//	{
				//		serialNoData.STATUS = "A1";
				//		f2501Repo.Update(serialNoData);
				//	}

				//}
			}

			return new ExecuteResult() { IsSuccessed = true };
		}

		#endregion

		#region 取 F020301 Data
		public IQueryable<F020302Data> GetF020302Data(string dcCode, string gupCode, string custCode, string purchaseNo)
		{
			var f020301Repo = new F020301Repository(Schemas.CoreSchema, _wmsTransaction);
			return f020301Repo.GetF020302Data(dcCode, gupCode, custCode, purchaseNo);

		}
		#endregion


		/// <summary>
		/// 更新暫存驗收檔中的虛擬商品，若驗收數已等於進倉驗收數，則更新已刷讀序號欄位
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="rtNo"></param>
		public bool UpdateCheckSerialByVirtualItem(string dcCode, string gupCode, string custCode, string purchaseNo, string rtNo)
		{
			var f02020101Repo = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction);

			var f02020101s = f02020101Repo.AsForUpdate()
											.GetF02020101sByVirtualItem(dcCode, gupCode, custCode, purchaseNo, rtNo); // 底層已經 ToList 就不再多做了...

			if (f02020101s.Any())
			{
				foreach (var f02020101 in f02020101s)
				{
					f02020101.CHECK_SERIAL = "1";
					f02020101Repo.Update(f02020101);
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 檢查進倉單號是否存在
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="deliverDate"></param>
		/// <returns></returns>
		public ExecuteResult CheckPurchaseNo(string dcCode, string gupCode, string custCode, string purchaseNo)
		{
			var f010201Repo = new F010201Repository(Schemas.CoreSchema);
			var f010201 = f010201Repo.GetEnabledStockData(dcCode, gupCode, custCode, purchaseNo);
			if (f010201 == null)
				return new ExecuteResult(false, Properties.Resources.PurchaseNoError);
			else if (f010201.STATUS == "8")
				return new ExecuteResult(false, Properties.Resources.PurchaseStatusError);
			else if (f010201.CUST_COST == "MoveIn")
				return new ExecuteResult(false, Properties.Resources.PurchaseCustCostError);
			else
			{
				var f1909Repo = new F1909Repository(Schemas.CoreSchema);
				var f1909 = f1909Repo.Find(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode);
				if (f1909.IS_PRINT_INSTOCKPALLETSTICKER == "1")
				{
					var f010203Repo = new F010203Repository(Schemas.CoreSchema);
					var isExists = f010203Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.STOCK_NO == purchaseNo).Any();
					if (!isExists)
						return new ExecuteResult(false, Properties.Resources.P020203Service_NoPrintInStockSticker);
				}
				return new ExecuteResult(true, "", f010201.STOCK_NO);
			}

		}

		/// <summary>
		/// 檢查F010201尚未填採購單號且該採購單有序號商品。
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <returns></returns>
		public ExecuteResult CheckShopNo(string dcCode, string gupCode, string custCode, string purchaseNo)
		{
			var f010201Repo = new F010201Repository(Schemas.CoreSchema);
			if (f010201Repo.ExistsEmptyShopNoByBundleSerialNo(dcCode, gupCode, custCode, purchaseNo))
			{
				return new ExecuteResult(false, Properties.Resources.NotExistsEmptyShopNoByBundleSerialNo);
			}

			return new ExecuteResult(true);
		}

		public ExecuteResult CheckRepeatSerails(string dcCode, string gupCode, string custCode, string poNo, string itemCode, string[] largeSerialNo)
		{
			var totalCount = largeSerialNo.Count();
			var range = 2000;
			int loopCnt = totalCount / range + (totalCount % range > 0 ? 1 : 0);
			for (var i = 0; i < loopCnt; i++)
			{
				var f020302Repo = new F020302Repository(Schemas.CoreSchema);
				//原方法，沒有排除自己以外的資料，會導致在前端刪除重新新增跳出序號重複問題
				//var serailRepeatQuery = f020302Repo.InWithTrueAndCondition("SERIAL_NO",
				//	largeSerialNo.Skip(i * range).Take(range).ToList(),
				//	x => x.DC_CODE == dcCode
				//		 && x.GUP_CODE == gupCode
				//		 && x.CUST_CODE == custCode
				//		 && x.ITEM_CODE == itemCode
				//		 && x.STATUS == "0");
				var serailRepeatQuery = f020302Repo.CheckRepeatSerails(dcCode, gupCode, custCode, poNo, itemCode, largeSerialNo.Skip(i * range).Take(range).ToArray());
				if (serailRepeatQuery.Any())
				{
					var repeatSerails = string.Join(Environment.NewLine, serailRepeatQuery.Select(x => x.SERIAL_NO));
					return new ExecuteResult(false, Properties.Resources.RepeatSerails + Environment.NewLine + repeatSerails);
				}
			}

			return new ExecuteResult(true);
		}

		/// <summary>
		/// 更新採購單號為進倉單號
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="purchaseNo"></param>
		/// <returns></returns>
		public ExecuteResult UpdateShopNoBePurchaseNo(string dcCode, string gupCode, string custCode, string purchaseNo)
		{
			var f010201Repo = new F010201Repository(Schemas.CoreSchema);
			var f010201 = f010201Repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.STOCK_NO == purchaseNo);
			if (f010201 == null)
				return new ExecuteResult(false, Properties.Resources.PurchaseNoNotExist);

			f010201.SHOP_NO = purchaseNo;
			f010201Repo.Update(f010201);
			return new ExecuteResult(true);
		}

		/// <summary>
		/// 缺貨確認
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="stockNo"></param>
		/// <param name="itemCode"></param>
		/// <param name="rtNo"></param>
		/// <returns></returns>
		public ExecuteResult OutOfStock(string dcCode, string gupCode, string custCode, string stockNo, string itemCode, string rtNo)
		{
			var f02020104Repo = new F02020104Repository(Schemas.CoreSchema);
			var serialDatas = f02020104Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PURCHASE_NO == stockNo && x.ITEM_CODE == itemCode && x.RT_NO == rtNo && x.ISPASS == "1").Select(x => x.SERIAL_NO).Distinct().ToList();
			var f02020101Repo = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction);
			var item = f02020101Repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PURCHASE_NO == stockNo && x.ITEM_CODE == itemCode);
			if (item != null)
			{
				//更新進貨數,驗收數,抽驗數=已刷讀序號數量
				item.ORDER_QTY = serialDatas.Count;
				item.RECV_QTY = serialDatas.Count;
				item.CHECK_QTY = serialDatas.Count;
				//更新序號檢核成功
				item.CHECK_SERIAL = "1";
				f02020101Repo.Update(item);

				var f010202Repo = new F010202Repository(Schemas.CoreSchema, _wmsTransaction);
				var f010202Item = f010202Repo.Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.STOCK_NO == stockNo && x.ITEM_CODE == itemCode);
				//更新進倉明細檔 此商品進貨數=已刷讀序號數量
				f010202Item.STOCK_QTY = serialDatas.Count;
				f010202Repo.Update(f010202Item);

				//更新進倉序號檔狀態為取消[9](當進倉序號非已刷讀的序號)
				var f020302Repo = new F020302Repository(Schemas.CoreSchema, _wmsTransaction);
				f020302Repo.UpdateSerialCancel(dcCode, gupCode, custCode, stockNo, itemCode, serialDatas);

				return new ExecuteResult(true);
			}
			return new ExecuteResult(false, Properties.Resources.OutOfStockFail);
		}

		public ExecuteResult UpdateF010201(string dcCode, string gupCode, string custCode, string stockNo)
		{

			var f010201Repo = new F010201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f010201 = f010201Repo.Find(a => a.DC_CODE == dcCode && a.STOCK_NO == stockNo
											 && a.GUP_CODE == gupCode && a.CUST_CODE == custCode);
			f010201.STATUS = "2";
			f010201Repo.Update(f010201);
			return new ExecuteResult(true);
		}
		public ExecuteResult InsertOrUpdateP02020307(List<F02020109Data> f02020109Datas)
		{
			ExecuteResult result = null;

			var addDatas = f02020109Datas.Where(o => o.ChangeFlag == "A").ToList();
			var f02020109Repo = new F02020109Repository(Schemas.CoreSchema, _wmsTransaction);
			var delDatas = f02020109Datas.Where(o => o.ChangeFlag == "D").ToList();
			foreach (var f02020109Data in delDatas)
				f02020109Repo.Delete(o => o.DC_CODE == f02020109Data.DC_CODE
															 && o.GUP_CODE == f02020109Data.GUP_CODE
															 && o.CUST_CODE == f02020109Data.CUST_CODE
															 && o.STOCK_NO == f02020109Data.STOCK_NO
															 && o.STOCK_SEQ == f02020109Data.STOCK_SEQ
															 && o.ID == f02020109Data.ID);

			foreach (var f02020109Data in addDatas)
			{
				var f02020109 = new F02020109
				{
					ID = f02020109Data.ID,
					DC_CODE = f02020109Data.DC_CODE,
					GUP_CODE = f02020109Data.GUP_CODE,
					CUST_CODE = f02020109Data.CUST_CODE,
					STOCK_NO = f02020109Data.STOCK_NO,
					STOCK_SEQ = f02020109Data.STOCK_SEQ,
					DEFECT_QTY = f02020109Data.DEFECT_QTY,
					SERIAL_NO = f02020109Data.SERIAL_NO,
					UCC_CODE = f02020109Data.UCC_CODE,
					CAUSE = f02020109Data.OTHER_CAUSE,
					WAREHOUSE_ID = f02020109Data.WAREHOUSE_ID
				};
				f02020109Repo.Add(f02020109);
			}


			if (result == null)
				result = new ExecuteResult { IsSuccessed = true };

			return result;
		}

		public ExecuteResult UpdateF1903(string gupCode, string custCode, string itemCode, string needExpired, DateTime? firstInDate,
	int? saveDay, string eanCode1, string eanCode2, string eanCode3, short? allDln, int? allShp, string isPrecious, string fragile,
	string isEasyLos, string spill, string isMagnetic, string isPerishable, string tmprType, string IsTempControl,string bundleSerial, string IsApple, F1905 updateVolumn)
		{

			
			//
			var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransaction);
			var updateF1903 = f1903Repo.Find(x => x.GUP_CODE.Equals(EntityFunctions.AsNonUnicode(gupCode))
					&& x.CUST_CODE.Equals(EntityFunctions.AsNonUnicode(custCode))
					&& x.ITEM_CODE.Equals(EntityFunctions.AsNonUnicode(itemCode)));

			if (string.IsNullOrEmpty(bundleSerial))
				bundleSerial = "0";

			/// 檢查商品主檔是否序號商品 != 傳入的BundleSerialNo
			if (updateF1903.BUNDLE_SERIALNO != bundleSerial)
			{
				#region 呼叫更新商品主檔API
				var apiRes = UpdatdeItemInfoService.UpdateItemInfo(
					updateF1903.CUST_CODE,
					updateF1903.CUST_ITEM_CODE,
					bundleSerial,
					string.IsNullOrWhiteSpace(eanCode1) ? updateF1903.EAN_CODE4 : eanCode1);
				if (!apiRes.IsSuccessed)
					return apiRes;
				#endregion
			}

			updateF1903.FIRST_IN_DATE = firstInDate;
			updateF1903.NEED_EXPIRED = needExpired;
			updateF1903.SAVE_DAY = saveDay;
			updateF1903.ALL_DLN = allDln;
			updateF1903.ALL_SHP = allShp;
			updateF1903.EAN_CODE1 = eanCode1;
			updateF1903.EAN_CODE2 = eanCode2;
			updateF1903.EAN_CODE3 = eanCode3;
			updateF1903.IS_PRECIOUS = isPrecious;
			updateF1903.FRAGILE = fragile;
			updateF1903.IS_EASY_LOSE = isEasyLos;
			updateF1903.SPILL = spill;
			updateF1903.IS_MAGNETIC = isMagnetic;
			updateF1903.IS_PERISHABLE = isPerishable;
			updateF1903.TMPR_TYPE = tmprType;
			updateF1903.IS_TEMP_CONTROL = IsTempControl;
			updateF1903.BUNDLE_SERIALNO = bundleSerial;
			if (updateF1903.BUNDLE_SERIALNO == "0")
				updateF1903.BUNDLE_SERIALLOC = "0";
			updateF1903.ISAPPLE = IsApple;
			updateF1903.IS_ASYNC = "N";

			if (updateVolumn != null)
			{
				var size = string.Format("{0}*{1}*{2}", updateVolumn.PACK_LENGTH, updateVolumn.PACK_WIDTH, updateVolumn.PACK_HIGHT);
				updateF1903.ITEM_SIZE = size;
			}

			f1903Repo.Update(updateF1903);

			return new ExecuteResult { IsSuccessed = true };

		}

		public ExecuteResult ReGetLmsApiStowShelfAreaGuide(string dcCode, string gupCode, string custCode, string purchaseNo, string[] warehouseList)
		{
			var diffWhId = new List<LmsStowShelfAreaGuideRespense>();

			var f010201Repo = new F010201Repository(Schemas.CoreSchema);
			var f02020101Repo = new F02020101Repository(Schemas.CoreSchema, _wmsTransaction);
			var order = f010201Repo.AsForUpdate().Find(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.STOCK_NO == purchaseNo);
			var f02020101s = f02020101Repo.AsForUpdate().GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.PURCHASE_NO == purchaseNo).ToList();
			var custInNo = order.CUST_ORD_NO;

			var lmsRes = CallLmsApiStowShelfAreaGuide(dcCode, gupCode, custCode, custInNo, f02020101s.Select(x => x.ITEM_CODE).Distinct().ToList());
			if (!lmsRes.IsSucessed)
			{
				return new ExecuteResult() { IsSuccessed = lmsRes.IsSucessed, Message = lmsRes.Msg };
			}
			if (lmsRes.Data.Any())
			{
				foreach (var f02020101 in f02020101s)
				{
					var lmsItemData = lmsRes.Data.Where(x => x.ItemCode == f02020101.ITEM_CODE).FirstOrDefault();
					if (lmsItemData != null)
					{
						if (warehouseList.Contains(lmsItemData.WhId))
							f02020101.TARWAREHOUSE_ID = lmsItemData.WhId;
						else
							diffWhId.Add(lmsItemData);
					}
				}
				f02020101Repo.BulkUpdate(f02020101s);
			}

			if (diffWhId.Any())
				return new ExecuteResult() { IsSuccessed = true, Message = string.Join("\r", diffWhId.Select(x => $"品號{x.ItemCode}上架倉別代碼為「{x.WhId}」，不在倉別清單內。").ToList()) };

			return new ExecuteResult() { IsSuccessed = true, Message = lmsRes.Msg };
		}

		private LmsStowShelfAreaGuideResult CallLmsApiStowShelfAreaGuide(string dcCode, string gupCode, string custCode, string custInNo, List<string> itemCodeList)
		{
			var res = new LmsStowShelfAreaGuideResult { Data = new List<LmsStowShelfAreaGuideRespense>() };
			var srv = new StowShelfAreaService();
			var lmsRes = srv.StowShelfAreaGuide(dcCode, gupCode, custCode, "1", custInNo, itemCodeList);
			res.IsSucessed = lmsRes.IsSuccessed;
			if (lmsRes.IsSuccessed)
			{
				var data = JsonConvert.DeserializeObject<List<Datas.Shared.ApiEntities.StowShelfAreaGuideData>>(
														JsonConvert.SerializeObject(lmsRes.Data));
				if (data != null && data.Any())
				{
					res.Data = data.Select(x => new LmsStowShelfAreaGuideRespense
					{
						ItemCode = x.ItemCode,
						WhId = x.ShelfAreaCode
					}).ToList();
				}
			}
			else
			{
				res.Msg = $"[LMS上架倉別指示]{lmsRes.MsgCode + lmsRes.MsgContent + Environment.NewLine}雖然無法取得上架倉別的指示，但仍然可視為收貨成功";
			}

			return res;
		}

	}
}
