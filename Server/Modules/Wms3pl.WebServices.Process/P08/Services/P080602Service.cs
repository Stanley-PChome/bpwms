
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P08.Services
{
	public partial class P080602Service
	{
		private WmsTransaction _wmsTransaction;
		public P080602Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<ExecuteResult> DoChickIn(string dcCode, string gupCode, string custCode, string delvDate, string pickTime, string wmsOrdNo)
		{
			List<ExecuteResult> results = new List<ExecuteResult>();
			ExecuteResult result = new ExecuteResult() { IsSuccessed = true };
			var wmsOrdNoList = wmsOrdNo.Split(',').ToList();
			var f052901repo = new F052901Repository(Schemas.CoreSchema, _wmsTransaction);
			var f052902repo = new F052902Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050802repo = new F050802Repository(Schemas.CoreSchema, _wmsTransaction);

			// 2. 新增播種主檔及副檔
			DateTime _delvDate = DateTime.Now.Date;
			if (!DateTime.TryParse(delvDate, out _delvDate))
			{
				result.IsSuccessed = false;
				result.Message = Properties.Resources.P080602Service_DelvDateError;
				results.Add(result);
				return results.AsQueryable();
			}

			foreach (var _wmsOrdNo in wmsOrdNoList)
			{
				// 2.1.0 檢核是否已報到
				var isExist = f052901repo.Filter(x => x.DC_CODE == EntityFunctions.AsNonUnicode(dcCode) &&
																						 x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode) &&
																						 x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode) &&
																						 x.WMS_ORD_NO == EntityFunctions.AsNonUnicode(_wmsOrdNo)).Any();
				if (!isExist)
				{
					//2.1 新增主檔
					F052901 f052901 = new F052901();
					f052901.WMS_ORD_NO = _wmsOrdNo;
					f052901.DC_CODE = dcCode;
					f052901.GUP_CODE = gupCode;
					f052901.CUST_CODE = custCode;
					f052901.DELV_DATE = _delvDate;
					f052901.PICK_TIME = pickTime;
					f052901repo.Add(f052901);
				}
				//2.2 新增副檔
				var f050802Groups = f050802repo.GetGroupItem(dcCode, gupCode, custCode, _wmsOrdNo);
				bool isExistItem = false;
				var checkf050802 = f050802Groups.FirstOrDefault();
				if (checkf050802 == null)
				{
					result.IsSuccessed = false;
					result.Message = Properties.Resources.P080602Service_F050802IsNull;
					results.Add(result);
					return results.AsQueryable();
				}

				// 2.2.0 檢核是否已報到
				isExistItem = f052902repo.Filter(x => x.DC_CODE == EntityFunctions.AsNonUnicode(checkf050802.DC_CODE) &&
																						 x.GUP_CODE == EntityFunctions.AsNonUnicode(checkf050802.GUP_CODE) &&
																						 x.CUST_CODE == EntityFunctions.AsNonUnicode(checkf050802.CUST_CODE) &&
																						 x.WMS_ORD_NO == EntityFunctions.AsNonUnicode(checkf050802.WMS_ORD_NO)).Any();


				if (!isExistItem)
				{
					foreach (var f050802 in f050802Groups)
					{
						//2.2.1 新增副檔
						var f052902 = new F052902()
						{
							WMS_ORD_NO = f050802.WMS_ORD_NO,
							DC_CODE = f050802.DC_CODE,
							GUP_CODE = f050802.GUP_CODE,
							CUST_CODE = f050802.CUST_CODE,
							ITEM_CODE = f050802.ITEM_CODE,
							B_SET_QTY = f050802.SUM_B_SET_QTY
						};
						f052902repo.Add(f052902);
					}
				}
			}
			results.Add(result);
			return results.AsQueryable();
		}

		/// <summary>
		/// 合流作業的缺貨註記
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsOrdNo"></param>
		/// <param name="itemCode"></param>
		/// <param name="pickOrdNo"></param>
		/// <param name="lackQtyTotal"></param>
		/// <returns></returns>
		public ExecuteResult AddLackData(string dcCode,
										string gupCode,
										string custCode,
										string wmsOrdNo,
										string itemCode,
										string pickOrdNo,
										int lackQtyTotal)
		{
			var f051202repo = new F051202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f052902repo = new F052902Repository(Schemas.CoreSchema, _wmsTransaction);
			var f05030101repo = new F05030101Repository(Schemas.CoreSchema, _wmsTransaction);
			var f051206repo = new F051206Repository(Schemas.CoreSchema, _wmsTransaction);

			if (lackQtyTotal < 0)
			{
				return new ExecuteResult(false, Properties.Resources.P080602Service_LackQtyTotalError);
			}

			var f05030101 = f05030101repo.Find(x => x.DC_CODE == dcCode
											&& x.GUP_CODE == gupCode
											&& x.CUST_CODE == custCode
											&& x.WMS_ORD_NO == wmsOrdNo);
			if (f05030101 == null)
			{
				return new ExecuteResult(false, Properties.Resources.P080602Service_F05030101IsNull);
			}

			var f051202s = f051202repo.Filter(x => x.PICK_ORD_NO == EntityFunctions.AsNonUnicode(pickOrdNo)
												&& x.WMS_ORD_NO == EntityFunctions.AsNonUnicode(wmsOrdNo)
												&& x.DC_CODE == EntityFunctions.AsNonUnicode(dcCode)
												&& x.GUP_CODE == EntityFunctions.AsNonUnicode(gupCode)
												&& x.CUST_CODE == EntityFunctions.AsNonUnicode(custCode)
												&& x.ITEM_CODE == EntityFunctions.AsNonUnicode(itemCode))
									  .OrderByDescending(x => x.VALID_DATE)
									  .ThenByDescending(x => x.ENTER_DATE)
									  .ToList();

			if (!f051202s.Any())
			{
				return new ExecuteResult(false, Properties.Resources.P080602Service_PickOrdNoIsNull);
			}

			var f1511Repo = new F1511Repository(Schemas.CoreSchema, _wmsTransaction);
			// 一張出貨單會有多張揀貨單明細，目前按照效期、入庫日 DESC 來扣實際揀貨量，
			// 被扣的量就是缺貨的數量，則會新增缺貨註記。
			int lackQty = lackQtyTotal;
			foreach (var f051202 in f051202s)
			{
				if (lackQty == 0 || f051202.A_PICK_QTY == 0)
					break;

				var f1511Item =
				f1511Repo.Find(
					o =>
						o.DC_CODE == f051202.DC_CODE && o.GUP_CODE == f051202.GUP_CODE && o.CUST_CODE == f051202.CUST_CODE &&
						o.ORDER_NO == f051202.PICK_ORD_NO && o.ORDER_SEQ == f051202.PICK_ORD_SEQ);

				if (f051202.A_PICK_QTY >= lackQty)
				{
					AddF051206(f051202, f05030101.ORD_NO, lackQty);
					f051202.A_PICK_QTY -= lackQty;
					lackQty = 0;
				}
				else
				{
					AddF051206(f051202, f05030101.ORD_NO, f051202.A_PICK_QTY);
					lackQty -= f051202.A_PICK_QTY;
					f051202.A_PICK_QTY = 0;
				}
				f1511Item.A_PICK_QTY = f051202.A_PICK_QTY;
				f1511Repo.Update(f1511Item);
				// 更新實際揀貨數，因為包裝會採用揀貨單明細的實際揀貨數來計算出貨數
				f051202repo.Update(f051202);
			}

			if (lackQty > 0)
			{
				return new ExecuteResult(false, string.Format(Properties.Resources.P080602Service_LackQtyOver, lackQtyTotal, itemCode, lackQty));
			}

			return new ExecuteResult(true);
		}

		private void AddF051206(F051202 f051202, string ordNo, int lackQty)
		{
			var f051206repo = new F051206Repository(Schemas.CoreSchema, _wmsTransaction);
			// 新增缺貨新訊 F051206
			f051206repo.Add(new F051206
			{
				WMS_ORD_NO = f051202.WMS_ORD_NO,
				PICK_ORD_NO = f051202.PICK_ORD_NO,
				PICK_ORD_SEQ = f051202.PICK_ORD_SEQ,
				ITEM_CODE = f051202.ITEM_CODE,
				CUST_CODE = f051202.CUST_CODE,
				GUP_CODE = f051202.GUP_CODE,
				DC_CODE = f051202.DC_CODE,
				ORD_NO = ordNo,
				LACK_QTY = lackQty,
				TRANS_FLAG = "0"
			});
		}
	}
}

