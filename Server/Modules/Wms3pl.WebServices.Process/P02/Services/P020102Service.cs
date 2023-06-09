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

namespace Wms3pl.WebServices.Process.P02.Services
{
	public partial class P020102Service
	{
		private WmsTransaction _wmsTransaction;
		public P020102Service(WmsTransaction wmsTransaction = null)
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
		/// <param name="carNumber"></param>
		/// <param name="bookInTime"></param>
		/// <param name="userId"></param>
		/// <param name="arriveDate"></param> 
		/// <returns></returns>
		public ExecuteResult UpdateF020103(string purchaseNo, Int16 serialNo, string dcCode, string gupCode
			, string custCode, string pierCode, string carNumber, string bookInTime, string userId, string arriveDate)
		{
			var repo = new F020103Repository(Schemas.CoreSchema, _wmsTransaction);

			DateTime dt = Convert.ToDateTime(arriveDate);

			var tmp = repo.Find(x => x.PURCHASE_NO.Equals(purchaseNo)
				&& x.SERIAL_NO.Equals(serialNo)
				&& x.DC_CODE.Equals(dcCode)
				&& x.GUP_CODE.Equals(gupCode)
				&& x.CUST_CODE.Equals(custCode)
				&& x.ARRIVE_DATE.Equals(dt));

			if (tmp == null) return new ExecuteResult() { IsSuccessed = false, Message = "資料已被刪除, 請重新查詢" };

			tmp.PIER_CODE = pierCode;
			tmp.UPD_DATE = DateTime.Now;
			tmp.UPD_STAFF = userId;
			tmp.BOOK_INTIME = bookInTime;
			tmp.CAR_NUMBER = carNumber;
			repo.Update(tmp);
			return new ExecuteResult() { IsSuccessed = true };
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
		/// <param name="inTime"></param>
		/// <returns></returns>
		public ExecuteResult InsertF020103(DateTime date, string bookInTime, string carNumber
			, string purchaseNo, string pierCode, string vendorCode, string dcCode
			, string gupCode, string custCode, string inTime = "")
		{
			var result = new ExecuteResult() { IsSuccessed = false };
			var repo = new F020103Repository(Schemas.CoreSchema, _wmsTransaction);
			var srvP020101 = new P020101Service(_wmsTransaction);

			// 取得新ID, 回寫到SERIAL_NO
			Int16 newId = repo.GetNewId(dcCode, gupCode, custCode, purchaseNo, date);

			repo.Add(new F020103()
			{
				ARRIVE_DATE = date,
				ARRIVE_TIME = "9", // 從進場管理新增時, 預設為"9"
				PURCHASE_NO = purchaseNo,
				PIER_CODE = pierCode,
				CAR_NUMBER = carNumber,
				BOOK_INTIME = bookInTime,
				INTIME = inTime,
				VNR_CODE = vendorCode,
				DC_CODE = dcCode,
				GUP_CODE = gupCode,
				CUST_CODE = custCode,
				ORDER_QTY = srvP020101.GetItemSumQty(purchaseNo, dcCode, gupCode, custCode),
				ITEM_QTY = srvP020101.GetItemCount(purchaseNo, dcCode, gupCode, custCode),
				ORDER_VOLUME = srvP020101.GetOrderSize(purchaseNo, dcCode, gupCode, custCode),
				SERIAL_NO = newId
			});
			result.IsSuccessed = true;

			return result;
		}

		#endregion
	}


}
