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
	public partial class F140104Repository : RepositoryBase<F140104, Wms3plDbContext, F140104Repository>
	{
		public F140104Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
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
		/// <param name="enterDate"></param>
		/// <param name="validDate"></param>
		/// <param name="makeNo"></param>
		/// <returns></returns>
		public IQueryable<InventoryDetailItem> GetInventoryDetailItems(string dcCode, string gupCode, string custCode,
			string inventoryNo, string locCode, string itemCode, DateTime enterDate, DateTime validDate, string makeNo)
		{
			var param = new List<object> { dcCode, gupCode, custCode, inventoryNo, locCode, itemCode, enterDate, validDate };

			var q = (from a in _db.F140104s
							 join b in _db.F1903s on new { a.GUP_CODE, a.ITEM_CODE, a.CUST_CODE } equals new { b.GUP_CODE, b.ITEM_CODE, b.CUST_CODE }
							 join c in _db.F1980s on new { a.DC_CODE, a.WAREHOUSE_ID } equals new { c.DC_CODE, c.WAREHOUSE_ID }
							 where a.DC_CODE == dcCode
							 && a.GUP_CODE == gupCode
							 && a.CUST_CODE == custCode
							 && a.INVENTORY_NO == inventoryNo
							 && a.LOC_CODE == locCode
							 && a.ITEM_CODE == itemCode
							 && a.ENTER_DATE == enterDate
							 && a.VALID_DATE == validDate
							 && a.MAKE_NO == (!string.IsNullOrWhiteSpace(makeNo) ? makeNo : null)
							 select new InventoryDetailItem
							 {
								 ChangeStatus = "N",
								 LOC_CODE = a.LOC_CODE,
								 ITEM_CODE = a.ITEM_CODE,
								 ITEM_NAME = b.ITEM_NAME,
								 ITEM_SPEC = b.ITEM_SPEC,
								 ITEM_COLOR = b.ITEM_COLOR,
								 ITEM_SIZE = b.ITEM_SIZE,
								 VALID_DATE = a.VALID_DATE,
								 ENTER_DATE = a.ENTER_DATE,
								 WAREHOUSE_ID = a.WAREHOUSE_ID,
								 WAREHOUSE_NAME = c.WAREHOUSE_NAME,
								 QTY = Convert.ToInt32(a.QTY),
								 FIRST_QTY_ORG = a.FIRST_QTY,
								 FIRST_QTY = a.FIRST_QTY,
								 SECOND_QTY_ORG = a.SECOND_QTY,
								 SECOND_QTY = a.SECOND_QTY,
								 FLUSHBACK_ORG = a.FLUSHBACK,
								 FLUSHBACK = a.FLUSHBACK,
								 BOX_CTRL_NO = a.BOX_CTRL_NO,
								 PALLET_CTRL_NO = a.PALLET_CTRL_NO,
								 MAKE_NO = a.MAKE_NO
							 }).ToList();
			var rI = 1;
			foreach (var item in q)
			{
				item.ROWNUM = rI;
				rI++;
			}
			return q.AsQueryable();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <param name="isSecond"></param>
		/// <returns></returns>
		public bool CheckInventoryDetailHasEnterQty(string dcCode, string gupCode, string custCode, string inventoryNo,
			string isSecond)
		{
			IQueryable<InventorySimpleData> q;
			if (isSecond == "0")
			{
				q = from A in _db.F140104s
						where A.DC_CODE == dcCode
						&& A.GUP_CODE == gupCode
						&& A.CUST_CODE == custCode
						&& A.INVENTORY_NO == inventoryNo
						&& A.FIRST_QTY >= 0
						select new InventorySimpleData
						{
							INVENTORY_NO = A.INVENTORY_NO
						};
			}
			else
			{
				q = from A in _db.F140105s
						where A.DC_CODE == dcCode
						&& A.GUP_CODE == gupCode
						&& A.CUST_CODE == custCode
						&& A.INVENTORY_NO == inventoryNo
						&& A.SECOND_QTY >= 0
						select new InventorySimpleData
						{
							INVENTORY_NO = A.INVENTORY_NO
						};
			}
			return q.Any();
		}

		public bool CheckFirstInventoryIsFinish(string dcCode, string gupCode, string custCode, string inventoryNo)
		{
			var q = from A in _db.F140104s
					where A.DC_CODE == dcCode
					&& A.GUP_CODE == gupCode
					&& A.CUST_CODE == custCode
					&& A.INVENTORY_NO == inventoryNo
					&& A.FIRST_QTY >= 0
					select new InventorySimpleData
					{
						INVENTORY_NO = A.INVENTORY_NO
					};
			return q.Any();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <returns></returns>
		public IQueryable<F140104> GetF140104FirstQtyNull(string dcCode, string gupCode, string custCode, string inventoryNo)
		{
			var q = from A in _db.F140104s
							where A.DC_CODE == dcCode
							&& A.GUP_CODE == gupCode
							&& A.CUST_CODE == custCode
							&& A.INVENTORY_NO == inventoryNo
							&& !A.FIRST_QTY.HasValue
							select A;
			return q;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="inventoryNo"></param>
		/// <param name="isSecond"></param>
		/// <returns></returns>
		public IQueryable<InventoryLocItem> GetInventoryLocItems(string dcCode, string gupCode, string custCode,
			string inventoryNo, string isSecond)
		{
			IQueryable<InventoryLocItem> q;
			if (isSecond == "1")
			{
				q = (from A in _db.F140105s
						 where A.DC_CODE == dcCode
						 && A.GUP_CODE == gupCode
						 && A.CUST_CODE == custCode
						 && A.INVENTORY_NO == inventoryNo
						 select new InventoryLocItem
						 {
							 LOC_CODE = A.LOC_CODE,
							 ITEM_CODE = A.ITEM_CODE
						 }).Distinct();
			}
			else
			{
				q = (from A in _db.F140104s
						 where A.DC_CODE == dcCode
						 && A.GUP_CODE == gupCode
						 && A.CUST_CODE == custCode
						 && A.INVENTORY_NO == inventoryNo
						 select new InventoryLocItem
						 {
							 LOC_CODE = A.LOC_CODE,
							 ITEM_CODE = A.ITEM_CODE
						 }).Distinct();
			}
			var result = q.OrderBy(o => o.LOC_CODE).ThenBy(o => o.ITEM_CODE).ToList();
			var rI = 1;
			foreach (var item in result)
			{
				item.ROWNUM = rI;
				rI++;
			}
			return result.AsQueryable();
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
		public IQueryable<F140104> FindF140104(string dcCode, string gupCode, string custCode, string inventoryNo, string locCode,
			 string itemCode, DateTime validDate, DateTime enterDate, string boxCtrlNo, string palletCtrlNo, string makeNo)
		{
			var q = from A in _db.F140104s
							where A.DC_CODE == dcCode
							&& A.GUP_CODE == gupCode
							&& A.CUST_CODE == custCode
							&& A.INVENTORY_NO == inventoryNo
							&& A.LOC_CODE == locCode
							&& A.ITEM_CODE == itemCode
							&& A.VALID_DATE == validDate
							&& A.ENTER_DATE == enterDate
							&& A.BOX_CTRL_NO == boxCtrlNo
							&& A.PALLET_CTRL_NO == palletCtrlNo
							select A;

			if (!string.IsNullOrWhiteSpace(makeNo))
			{
				q = q.Where(a => a.MAKE_NO == makeNo);

			}
			else
			{
				q = q.Where(a => a.MAKE_NO == "0");
			}

			return q;
		}

		public IQueryable<F140104> GetDatasByWcsInventoryNos(string dcCode, string gupCode, string custCode, List<string> inventoryNos)
		{
			return _db.F140104s.AsNoTracking().Where(x =>
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			inventoryNos.Contains(x.INVENTORY_NO));
		}
	}
}
