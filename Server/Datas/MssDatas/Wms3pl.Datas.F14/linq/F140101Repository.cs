using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F14
{
	public partial class F140101Repository : RepositoryBase<F140101, Wms3plDbContext, F140101Repository>
	{
		public F140101Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}


		public IQueryable<GetDetailInvResData> GetInventoryDetailList(string dcNo, string custNo, string gupCode, string invNo)
		{
			var f140101s = _db.F140101s.AsNoTracking().Where(x => x.DC_CODE == dcNo
																										&& x.CUST_CODE == custNo
																										&& x.GUP_CODE == gupCode
																										&& x.INVENTORY_NO == invNo);
			var f140104s = _db.F140104s.AsNoTracking().Where(x => x.FIRST_QTY == null);
			var f140105s = _db.F140105s.AsNoTracking().Where(x => x.SECOND_QTY == null);
			var f1912s = _db.F1912s.AsNoTracking();
			var f1903s = _db.F1903s.AsNoTracking().Where(x => x.GUP_CODE == gupCode
																										 && x.CUST_CODE == custNo);
			// int i = 0;
			var n = (from A in f140101s
					 join B in f140104s on new { A.INVENTORY_NO, A.DC_CODE, A.GUP_CODE, A.CUST_CODE }
												equals new { B.INVENTORY_NO, B.DC_CODE, B.GUP_CODE, B.CUST_CODE }
					 join D in f1912s on new { B.DC_CODE, B.LOC_CODE }
											equals new { D.DC_CODE, D.LOC_CODE }
					 join E in f1903s on B.ITEM_CODE equals E.ITEM_CODE
					 group new { B, D, E } by new
					 {
						 B.INVENTORY_NO,
						 B.ITEM_CODE,
						 B.LOC_CODE,
						 B.WAREHOUSE_ID,
						 B.VALID_DATE,
						 B.ENTER_DATE,
						 B.QTY,
						 D.AREA_CODE,
						 D.CHANNEL,
						 B.MAKE_NO,
						 B.FIRST_QTY,
						 B.PALLET_CTRL_NO,
						 E.BUNDLE_SERIALNO,
						 E.BUNDLE_SERIALLOC
					 } into g
					 select new
					 {
						 InvNo = g.Key.INVENTORY_NO,
						 ItemNo = g.Key.ITEM_CODE,
						 Loc = g.Key.LOC_CODE,
						 WhNo = g.Key.WAREHOUSE_ID,
						 WhName = g.Key.WAREHOUSE_ID,
						 ValidDate = g.Key.VALID_DATE.ToString("yyyy/MM/dd"),
						 EnterDate = g.Key.ENTER_DATE.ToString("yyyy/MM/dd"),
						 StockQty = g.Key.QTY,
						 Zone = g.Key.AREA_CODE,
						 Aisle = g.Key.CHANNEL,
						 MkNo = g.Key.MAKE_NO,
						 ActQty = g.Key.FIRST_QTY,
						 PalletNo = g.Key.PALLET_CTRL_NO,
						 SnType = g.Key.BUNDLE_SERIALLOC == "1" ? "2" : (g.Key.BUNDLE_SERIALNO == "1" ? "1" : "0")
					 }).Union(
										 from A in f140101s
										 join C in f140105s on new { A.INVENTORY_NO, A.DC_CODE, A.GUP_CODE, A.CUST_CODE }
																	 equals new { C.INVENTORY_NO, C.DC_CODE, C.GUP_CODE, C.CUST_CODE }
										 join D in f1912s on new { C.DC_CODE, C.LOC_CODE }
																	 equals new { D.DC_CODE, D.LOC_CODE }
										 join E in f1903s on C.ITEM_CODE equals E.ITEM_CODE
										 group new { C, D, E } by new
										 {
											 C.INVENTORY_NO,
											 C.ITEM_CODE,
											 C.LOC_CODE,
											 C.WAREHOUSE_ID,
											 C.VALID_DATE,
											 C.ENTER_DATE,
											 C.QTY,
											 D.AREA_CODE,
											 D.CHANNEL,
											 C.MAKE_NO,
											 C.FIRST_QTY,
											 C.PALLET_CTRL_NO,
											 E.BUNDLE_SERIALNO,
											 E.BUNDLE_SERIALLOC
										 } into h
										 select new
										 {
											 InvNo = h.Key.INVENTORY_NO,
											 ItemNo = h.Key.ITEM_CODE,
											 Loc = h.Key.LOC_CODE,
											 WhNo = h.Key.WAREHOUSE_ID,
											 WhName = h.Key.WAREHOUSE_ID,
											 ValidDate = h.Key.VALID_DATE.ToString("yyyy/MM/dd"),
											 EnterDate = h.Key.ENTER_DATE.ToString("yyyy/MM/dd"),
											 StockQty = h.Key.QTY,
											 Zone = h.Key.AREA_CODE,
											 Aisle = h.Key.CHANNEL,
											 MkNo = h.Key.MAKE_NO,
											 ActQty = h.Key.FIRST_QTY,
											 PalletNo = h.Key.PALLET_CTRL_NO,
											 SnType = h.Key.BUNDLE_SERIALLOC == "1" ? "2" : (h.Key.BUNDLE_SERIALNO == "1" ? "1" : "0")
										 }
					);

			var result = n.AsEnumerable().OrderBy(x => x.Loc).Select((x, index) => new GetDetailInvResData
			{
				InvNo = x.InvNo,
				ItemNo = x.ItemNo,
				Loc = x.Loc,
				WhNo = x.WhNo,
				WhName = x.WhName,
				ValidDate = x.ValidDate,
				EnterDate = x.EnterDate,
				StockQty = Convert.ToInt32(x.StockQty),
				Zone = x.Zone,
				Aisle = x.Aisle,
				MkNo = x.MkNo,
				ActQty = x.ActQty,
				PalletNo = x.PalletNo,
				InvSeq = index + 1,
				SnType = x.SnType
			});

			return result.AsQueryable();
		}

        public IQueryable<GetDetailInvResData> GetInventoryDetailAllColList(string dcNo, string custNo, string gupCode, string invNo)
        {
            var f140101s = _db.F140101s.AsNoTracking().Where(x => x.DC_CODE == dcNo
                                                                  && x.CUST_CODE == custNo
                                                                  && x.GUP_CODE == gupCode
                                                                  && x.INVENTORY_NO == invNo);
            var f140104s = _db.F140104s.AsNoTracking().Where(x => x.FIRST_QTY == null);
            var f140105s = _db.F140105s.AsNoTracking().Where(x => x.SECOND_QTY == null);
            var f1912s = _db.F1912s.AsNoTracking();
            var f1903s = _db.F1903s.AsNoTracking().Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custNo);
            var f1905s = _db.F1905s.AsNoTracking().Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custNo);

            // int i = 0;
            var n = (from A in f140101s
                     join B in f140104s on new { A.INVENTORY_NO, A.DC_CODE, A.GUP_CODE, A.CUST_CODE }
                     equals new { B.INVENTORY_NO, B.DC_CODE, B.GUP_CODE, B.CUST_CODE }
                     join D in f1912s on new { B.DC_CODE, B.LOC_CODE }
                     equals new { D.DC_CODE, D.LOC_CODE }
                     join E in f1903s on B.ITEM_CODE equals E.ITEM_CODE
                     join F in f1905s on E.ITEM_CODE equals F.ITEM_CODE
                     group new { B, D, E, F } by new
                     {
                         B.INVENTORY_NO,
                         B.ITEM_CODE,
                         B.LOC_CODE,
                         B.WAREHOUSE_ID,
                         B.VALID_DATE,
                         B.ENTER_DATE,
                         B.QTY,
                         D.AREA_CODE,
                         D.CHANNEL,
                         B.MAKE_NO,
                         B.FIRST_QTY,
                         B.PALLET_CTRL_NO,
                         E.BUNDLE_SERIALNO,
                         E.BUNDLE_SERIALLOC,
                         E.ITEM_UNIT,
                         E.ITEM_NAME,
                         E.ITEM_SIZE,
                         E.ITEM_COLOR,
                         E.ITEM_SPEC,
                         E.EAN_CODE1,
                         E.EAN_CODE2,
                         E.EAN_CODE3,
                         F.PACK_WEIGHT,
                         E.CTNS
                     } into g
                     select new
                     {
                         InvNo = g.Key.INVENTORY_NO,
                         ItemNo = g.Key.ITEM_CODE,
                         Loc = g.Key.LOC_CODE,
                         WhNo = g.Key.WAREHOUSE_ID,
                         WhName = g.Key.WAREHOUSE_ID,
                         ValidDate = g.Key.VALID_DATE.ToString("yyyy/MM/dd"),
                         EnterDate = g.Key.ENTER_DATE.ToString("yyyy/MM/dd"),
                         StockQty = g.Key.QTY,
                         Zone = g.Key.AREA_CODE,
                         Aisle = g.Key.CHANNEL,
                         MkNo = g.Key.MAKE_NO,
                         ActQty = g.Key.FIRST_QTY,
                         PalletNo = g.Key.PALLET_CTRL_NO,
                         SnType = g.Key.BUNDLE_SERIALLOC == "1" ? "2" : (g.Key.BUNDLE_SERIALNO == "1" ? "1" : "0"),
                         Unit = g.Key.ITEM_UNIT,
                         ProductName = g.Key.ITEM_NAME,
                         ProductSize = g.Key.ITEM_SIZE,
                         ProductColor = g.Key.ITEM_COLOR,
                         ProductSpec = g.Key.ITEM_SPEC,
                         Barcode1 = g.Key.EAN_CODE1,
                         Barcode2 = g.Key.EAN_CODE2,
                         Barcode3 = g.Key.EAN_CODE3,
                         Weight = g.Key.PACK_WEIGHT,
                         BoxQty = g.Key.CTNS
                     }).Union(
                                         from A in f140101s
                                         join C in f140105s on new { A.INVENTORY_NO, A.DC_CODE, A.GUP_CODE, A.CUST_CODE }
                                                                     equals new { C.INVENTORY_NO, C.DC_CODE, C.GUP_CODE, C.CUST_CODE }
                                         join D in f1912s on new { C.DC_CODE, C.LOC_CODE }
                                                                     equals new { D.DC_CODE, D.LOC_CODE }
                                         join E in f1903s on C.ITEM_CODE equals E.ITEM_CODE
                                         join F in f1905s on E.ITEM_CODE equals F.ITEM_CODE
                                         group new { C, D, E } by new
                                         {
                                             C.INVENTORY_NO,
                                             C.ITEM_CODE,
                                             C.LOC_CODE,
                                             C.WAREHOUSE_ID,
                                             C.VALID_DATE,
                                             C.ENTER_DATE,
                                             C.QTY,
                                             D.AREA_CODE,
                                             D.CHANNEL,
                                             C.MAKE_NO,
                                             C.FIRST_QTY,
                                             C.PALLET_CTRL_NO,
                                             E.BUNDLE_SERIALNO,
                                             E.BUNDLE_SERIALLOC,
                                             E.ITEM_UNIT,
                                             E.ITEM_NAME,
                                             E.ITEM_SIZE,
                                             E.ITEM_COLOR,
                                             E.ITEM_SPEC,
                                             E.EAN_CODE1,
                                             E.EAN_CODE2,
                                             E.EAN_CODE3,
                                             F.PACK_WEIGHT,
                                             E.CTNS
                                         } into h
                                         select new
                                         {
                                             InvNo = h.Key.INVENTORY_NO,
                                             ItemNo = h.Key.ITEM_CODE,
                                             Loc = h.Key.LOC_CODE,
                                             WhNo = h.Key.WAREHOUSE_ID,
                                             WhName = h.Key.WAREHOUSE_ID,
                                             ValidDate = h.Key.VALID_DATE.ToString("yyyy/MM/dd"),
                                             EnterDate = h.Key.ENTER_DATE.ToString("yyyy/MM/dd"),
                                             StockQty = h.Key.QTY,
                                             Zone = h.Key.AREA_CODE,
                                             Aisle = h.Key.CHANNEL,
                                             MkNo = h.Key.MAKE_NO,
                                             ActQty = h.Key.FIRST_QTY,
                                             PalletNo = h.Key.PALLET_CTRL_NO,
                                             SnType = h.Key.BUNDLE_SERIALLOC == "1" ? "2" : (h.Key.BUNDLE_SERIALNO == "1" ? "1" : "0"),
                                             Unit = h.Key.ITEM_UNIT,
                                             ProductName = h.Key.ITEM_NAME,
                                             ProductSize = h.Key.ITEM_SIZE,
                                             ProductColor = h.Key.ITEM_COLOR,
                                             ProductSpec = h.Key.ITEM_SPEC,
                                             Barcode1 = h.Key.EAN_CODE1,
                                             Barcode2 = h.Key.EAN_CODE2,
                                             Barcode3 = h.Key.EAN_CODE3,
                                             Weight = h.Key.PACK_WEIGHT,
                                             BoxQty = h.Key.CTNS
                                         }
                    );

            var result = n.AsEnumerable().OrderBy(x => x.Loc).Select((x, index) => new GetDetailInvResData
            {
                InvNo = x.InvNo,
                ItemNo = x.ItemNo,
                Loc = x.Loc,
                WhNo = x.WhNo,
                WhName = x.WhName,
                ValidDate = x.ValidDate,
                EnterDate = x.EnterDate,
                StockQty = Convert.ToInt32(x.StockQty),
                Zone = x.Zone,
                Aisle = x.Aisle,
                MkNo = x.MkNo,
                ActQty = x.ActQty,
                PalletNo = x.PalletNo,
                InvSeq = index + 1,
                SnType = x.SnType,
                Unit = x.Unit,
                ProductName = x.ProductName,
                ProductSize = x.ProductSize,
                ProductColor = x.ProductColor,
                ProductSpec = x.ProductSpec,
                Barcode1 = x.Barcode1,
                Barcode2 = x.Barcode2,
                Barcode3 = x.Barcode3,
                Weight = x.Weight,
                BoxQty = x.BoxQty
            });

            return result.AsQueryable();
        }

		public IQueryable<F140101> GetDatasByInventoryAdjustConfirm(string dcCode, string gupCode, string custCode, List<string> inventoryNos)
		{
			return _db.F140101s.Where(x =>
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			inventoryNos.Contains(x.INVENTORY_NO) &&
			x.STATUS != "9");
		}

		public IQueryable<InventoryDoc> GetInventoryDoc(string dcCode, string gupCode, string custCode, string inventoryNo, List<string> excludeWmsNos)
		{
			var allocationNos = _db.F151001s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.SOURCE_NO == inventoryNo && x.STATUS != "9")
				.Select(x => new InventoryDoc {
					WMS_NO = x.ALLOCATION_NO
				}).ToList();
			var adjustNos = _db.F200101s.Where(x => x.DC_CODE == dcCode && x.GUP_CODE == gupCode && x.CUST_CODE == custCode && x.SOURCE_NO == inventoryNo)
				.Select(x => new InventoryDoc {
					WMS_NO = x.ADJUST_NO
				}).ToList();

			if (excludeWmsNos.Any())
			{
				allocationNos = allocationNos.Where(x => !excludeWmsNos.Contains(x.WMS_NO)).ToList();
				adjustNos = adjustNos.Where(x => !excludeWmsNos.Contains(x.WMS_NO)).ToList();
			}

			var result = new List<InventoryDoc>();
			if (allocationNos.Any())
				result.AddRange(allocationNos);
			if (adjustNos.Any())
				result.AddRange(adjustNos);

			return result.AsQueryable();


		}
	}
}
