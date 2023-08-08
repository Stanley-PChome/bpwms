using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F14
{
	public partial class F140105Repository : RepositoryBase<F140105, Wms3plDbContext, F140105Repository>
	{
		public F140105Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		/// <summary>
		/// 計算查詢回來的盤點詳細的數量
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <param name="wareHouseId"></param>
		/// <param name="begLocCode"></param>
		/// <param name="endLocCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public int CountInventoryDetailItems0(string dcCode, string gupCode, string custCode,
		string inventoryNo, string wareHouseId, string begLocCode, string endLocCode, string itemCode)
		{
			var param = new List<object> { dcCode, gupCode, custCode, inventoryNo, begLocCode, endLocCode };

			var q = from A in _db.F140105s
							join B in _db.F1903s on new { A.GUP_CODE, A.ITEM_CODE, A.CUST_CODE } equals new { B.GUP_CODE, B.ITEM_CODE, B.CUST_CODE }
							join C in _db.F1980s on new { A.DC_CODE, A.WAREHOUSE_ID } equals new { C.DC_CODE, C.WAREHOUSE_ID }
							where A.DC_CODE == dcCode
							&& A.GUP_CODE == gupCode
							&& A.CUST_CODE == custCode
							&& A.INVENTORY_NO == inventoryNo
							&& string.Compare(A.LOC_CODE, begLocCode) >= 0
							&& string.Compare(A.LOC_CODE, endLocCode) <= 0
							select A;

			if (!string.IsNullOrEmpty(itemCode))
			{
				q = q.Where(a => a.ITEM_CODE == itemCode);

			}
			if (!string.IsNullOrEmpty(wareHouseId))
			{
				q = q.Where(a => a.WAREHOUSE_ID == wareHouseId);

			}
			return q.Count();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <returns></returns>
		public IQueryable<F140105> GetF140105FirstQtyNull(string dcCode, string gupCode, string custCode, string inventoryNo)
		{
			var q = from f in _db.F140105s
							where f.DC_CODE == dcCode
							&& f.GUP_CODE == gupCode
							&& f.CUST_CODE == custCode
							&& f.INVENTORY_NO == inventoryNo
							&& !f.SECOND_QTY.HasValue
							select f;

			return q;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <returns></returns>
		public bool CheckF140105Exist(string dcCode, string gupCode, string custCode, string inventoryNo)
		{
			var q = from f in _db.F140105s
							where f.DC_CODE == dcCode
							&& f.GUP_CODE == gupCode
							&& f.CUST_CODE == custCode
							&& f.INVENTORY_NO == inventoryNo
							&& f.SECOND_QTY != null
							select f;

			return q.Any();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <param name="locCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="validDate"></param>
		/// <param name="enterDate"></param>
		/// <param name="boxCtrlNo"></param>
		/// <param name="palletCtrlNo"></param>
		/// <param name="makeNo"></param>
		/// <returns></returns>
		public IQueryable<F140105> FindF140105(string dcCode, string gupCode, string custCode, string inventoryNo, string locCode,
string itemCode, DateTime validDate, DateTime enterDate, string boxCtrlNo, string palletCtrlNo, string makeNo)
		{
			List<object> param = new List<object>() { dcCode, gupCode, custCode, inventoryNo, locCode, itemCode, validDate, enterDate, boxCtrlNo, palletCtrlNo };

			var q = from f in _db.F140105s
							where f.DC_CODE == dcCode
							&& f.GUP_CODE == gupCode
							&& f.CUST_CODE == custCode
							&& f.INVENTORY_NO == inventoryNo
							&& f.LOC_CODE == locCode
							&& f.ITEM_CODE == itemCode
							&& f.VALID_DATE == validDate
							&& f.ENTER_DATE == enterDate
							&& f.BOX_CTRL_NO == boxCtrlNo
							&& f.PALLET_CTRL_NO == palletCtrlNo
							select f;

			if (!string.IsNullOrWhiteSpace(makeNo))
			{
				q = q.Where(f => f.MAKE_NO == makeNo);

			}
			//else
			//{
			//    sql = string.Format("{0} {1}", sql, " AND A.MAKE_NO is Null ");
			//}

			return q;
		}


		public bool CheckSecondInventoryIsExist(string dcCode, string gupCode, string custCode, string inventoryNo)
		{
			return _db.F140105s.AsNoTracking().Where(x =>
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			x.INVENTORY_NO == inventoryNo &&
			x.SECOND_QTY != null).Any();
		}
	}
}
