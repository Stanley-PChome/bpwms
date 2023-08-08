using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F02;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F01;
using Wms3pl.Datas.F19;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P02.Services
{
	public partial class P020101Service
	{
		private WmsTransaction _wmsTransaction;
		public P020101Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		#region P020101 進倉預排
		/// <summary>
		/// 更新進場預排的碼頭資訊
		/// </summary>
		/// <param name="purchaseNo"></param>
		/// <param name="serialNo"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="pierCode"></param>
		/// <param name="date"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public ExecuteResult UpdateF020103(string purchaseNo, Int16 serialNo, string dcCode, string gupCode
			, string custCode, string pierCode, string date, string userId)
		{
			var repo = new F020103Repository(Schemas.CoreSchema, _wmsTransaction);
			DateTime dt = Convert.ToDateTime(date);

			var tmp = repo.Find(x => x.PURCHASE_NO.Equals(purchaseNo)
				&& x.SERIAL_NO.Equals(serialNo)
				&& x.DC_CODE.Equals(dcCode) && x.GUP_CODE.Equals(gupCode)
				&& x.CUST_CODE.Equals(custCode)
				&& x.ARRIVE_DATE.Equals(dt)
				);

			if (tmp == null) return new ExecuteResult() { IsSuccessed = false, Message = "資料已被刪除, 請重新查詢" };

			tmp.PIER_CODE = pierCode;
			tmp.UPD_DATE = DateTime.Now;
			tmp.UPD_STAFF = userId;
			//tmp.ARRIVE_TIME = time;//2015-04-10 Alan
			repo.Update(tmp);
			return new ExecuteResult() { IsSuccessed = true };
		}

		/// <summary>
		/// 取得進倉預排清單
		/// </summary>
		/// <param name="arriveDate"></param>
		/// <param name="time"></param>
		/// <param name="dcCode"></param>
		/// <returns></returns>
		public IQueryable<F020103Detail> GetF020103Detail(DateTime arriveDate, string time
			, string dcCode, string vendorCode, string custCode, string gupCode)
		{
			var repo = new F020103Repository(Schemas.CoreSchema, _wmsTransaction);
			return repo.GetF020103Detail(arriveDate, time, dcCode, vendorCode, custCode, gupCode);
		}

		/// <summary>
		/// 刪除進場預排資料
		/// </summary>
		/// <param name="date"></param>
		/// <param name="time"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		public ExecuteResult Delete(string date, Int16 serialNo, string purchaseNo
			, string dcCode, string gupCode, string custCode)
		{
			var result = new ExecuteResult() { IsSuccessed = false };
			var repo = new F020103Repository(Schemas.CoreSchema, _wmsTransaction);
			var tmp = repo.Find(date, serialNo, purchaseNo, dcCode, gupCode, custCode);
			if (tmp == null)
			{
				result.IsSuccessed = false;
				result.Message = "進場預排資料已被刪除, 請重新查詢";
			}
			else if (!string.IsNullOrEmpty(tmp.INTIME))
			{
				result.IsSuccessed = false;
				result.Message = "已進場不可刪除, 請重新查詢";
			}
			else
			{
				result = repo.Delete(date, serialNo, purchaseNo, dcCode, gupCode, custCode);
			}

			return result;
		}

		public ExecuteResult InsertOrUpdateF020103(F020103 editF020103, bool isAdd)
		{
			var repo = new F020103Repository(Schemas.CoreSchema, _wmsTransaction);

			if (isAdd)
			{
				editF020103.ORDER_QTY = GetItemSumQty(editF020103.PURCHASE_NO, editF020103.DC_CODE, editF020103.GUP_CODE, editF020103.CUST_CODE);
				editF020103.ITEM_QTY = GetItemCount(editF020103.PURCHASE_NO, editF020103.DC_CODE, editF020103.GUP_CODE, editF020103.CUST_CODE);
				editF020103.ORDER_VOLUME = GetOrderSize(editF020103.PURCHASE_NO, editF020103.DC_CODE, editF020103.GUP_CODE, editF020103.CUST_CODE);
				editF020103.SERIAL_NO = repo.GetNewId(editF020103.DC_CODE, editF020103.GUP_CODE, editF020103.CUST_CODE, editF020103.PURCHASE_NO, editF020103.ARRIVE_DATE);
				repo.Add(editF020103);
			}
			else
			{
				var f020103 = repo.Find(x => x.PURCHASE_NO.Equals(editF020103.PURCHASE_NO)
										&& x.SERIAL_NO.Equals(editF020103.SERIAL_NO)
										&& x.DC_CODE.Equals(editF020103.DC_CODE)
										&& x.GUP_CODE.Equals(editF020103.GUP_CODE)
										&& x.CUST_CODE.Equals(editF020103.CUST_CODE)
										&& x.ARRIVE_DATE.Equals(editF020103.ARRIVE_DATE));

				if (f020103 == null)
					return new ExecuteResult(false, "資料已被刪除, 請重新查詢");

				f020103.PIER_CODE = editF020103.PIER_CODE;
				repo.Update(f020103);
			}

			return new ExecuteResult(true);
		}

		/// <summary>
		/// 寫入進場預排資料
		/// </summary>
		/// <param name="date"></param>
		/// <param name="time"></param>
		/// <param name="purchaseNo"></param>
		/// <param name="pierCode"></param>
		/// <param name="vendorCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public ExecuteResult InsertF020103(DateTime date, string time
			, string purchaseNo, string pierCode, string vendorCode, string dcCode
			, string gupCode, string custCode, string userId)
		{
			var result = new ExecuteResult() { IsSuccessed = false };
			var repo = new F020103Repository(Schemas.CoreSchema, _wmsTransaction);

			// 取得新ID, 回寫到SERIAL_NO
			Int16 newId = repo.GetNewId(dcCode, gupCode, custCode, purchaseNo, date);

			repo.Add(new F020103()
			{
				ARRIVE_DATE = date,
				ARRIVE_TIME = time,
				PURCHASE_NO = purchaseNo,
				PIER_CODE = pierCode,
				VNR_CODE = vendorCode,
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				CRT_STAFF = userId,
				ORDER_QTY = GetItemSumQty(purchaseNo, dcCode, gupCode, custCode),
				ITEM_QTY = GetItemCount(purchaseNo, dcCode, gupCode, custCode),
				ORDER_VOLUME = GetOrderSize(purchaseNo, dcCode, gupCode, custCode),
				SERIAL_NO = newId
			});
			result.IsSuccessed = true;

			return result;
		}

		/// <summary>
		/// 取得訂單的ITEM數
		/// </summary>
		/// <param name="orderNo"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		public int GetItemCount(string orderNo, string dcCode, string gupCode, string custCode)
		{
			var repo = new F010202Repository(Schemas.CoreSchema);
			var items = repo.Filter(x => x.DC_CODE.Equals(dcCode) && x.GUP_CODE.Equals(gupCode)
				&& x.CUST_CODE.Equals(custCode) && x.STOCK_NO.Equals(orderNo))
				.GroupBy(x => x.ITEM_CODE).ToList();
			return items.Count();
		}

		/// <summary>
		/// 取得訂單的總訂貨數量
		/// </summary>
		/// <param name="orderNo"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		public int GetItemSumQty(string orderNo, string dcCode, string gupCode, string custCode)
		{
			var repo = new F010202Repository(Schemas.CoreSchema);
			var items = repo.Filter(x => x.DC_CODE.Equals(dcCode) && x.GUP_CODE.Equals(gupCode)
				&& x.CUST_CODE.Equals(custCode) && x.STOCK_NO.Equals(orderNo)).ToList();
			return Convert.ToInt32(items.Sum(x => x.STOCK_QTY));
		}

		/// <summary>
		/// 取得訂單的總總積. 使用PACK_WIDTH, PACK_HEIGHT, PACK_LENGTH
		/// </summary>
		/// <param name="orderNo"></param>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
		public decimal GetOrderSize(string orderNo, string dcCode, string gupCode, string custCode)
		{
			var repo = new F010202Repository(Schemas.CoreSchema);
			var items = repo.Filter(x => x.DC_CODE.Equals(dcCode) && x.GUP_CODE.Equals(gupCode)
				&& x.CUST_CODE.Equals(custCode) && x.STOCK_NO.Equals(orderNo)).ToList();
      var commonservice = new CommonService();
			decimal unitSize = 0;
      var f1905s = commonservice.GetProductSizeList(gupCode, custCode, items.Select(x => x.ITEM_CODE).ToList());
      foreach (var p in items)
			{
        var item = f1905s.FirstOrDefault(x => x.ITEM_CODE.Equals(p.ITEM_CODE));
				unitSize += Convert.ToInt32(item.PACK_HIGHT) * Convert.ToInt32(item.PACK_LENGTH) * Convert.ToInt32(item.PACK_WIDTH) * Convert.ToInt32(p.STOCK_QTY);
			}
			return unitSize;
		}
		#endregion
	}
}
