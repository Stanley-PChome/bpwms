using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F14;

namespace Wms3pl.Datas.F19
{
	public partial class F1913Repository : RepositoryBase<F1913, Wms3plDbContext, F1913Repository>
	{
		public F1913Repository(string connName, WmsTransaction wmsTransaction = null)
						: base(connName, wmsTransaction)
		{
		}

		/// <summary>
		/// 復原商品儲位數量，若復原失敗，則表示該效期商品不存在於該儲位中。
		/// </summary>
		/// <param name="DC_CODE"></param>
		/// <param name="GUP_CODE"></param>
		/// <param name="CUST_CODE"></param>
		/// <param name="LOC_CODE">儲位編號</param>
		/// <param name="ITEM_CODE">商品編號</param>
		/// <param name="VALID_DATE">商品效期</param>
		/// <param name="incQty">要增加回商品儲位的數量</param>
		/// <returns></returns>
		public bool TryRecoverQty(string DC_CODE, string GUP_CODE, string CUST_CODE, string LOC_CODE, string ITEM_CODE, DateTime VALID_DATE, int incQty)
		{
			//ExecuteSqlCommand

			var f1913 = Find(item => item.LOC_CODE == LOC_CODE && item.ITEM_CODE == ITEM_CODE && item.VALID_DATE == VALID_DATE && item.DC_CODE == DC_CODE && item.GUP_CODE == GUP_CODE);
			if (f1913 == null)
			{
				return false;
			}

			// 復原該商品數量至該儲位
			f1913.QTY += incQty;

			base.Update(f1913);

			return true;
		}

		public DateTime? GetMinEnterDateByItem(string dcCode, string gupCode, string custCode, string itemCode,
				DateTime validDate, string vnrCode, string serialNo)
		{
			DateTime? enterDate = null;
			var data = _db.F1913s.AsNoTracking()
			.Where(x => x.DC_CODE == dcCode
					&& x.GUP_CODE == gupCode
					&& x.CUST_CODE == custCode
					&& x.ITEM_CODE == itemCode
					&& x.VNR_CODE == vnrCode
					&& x.VALID_DATE == validDate);
			if (data.Any())
			{
				enterDate = data.Min(o => o.ENTER_DATE);
			}

			return enterDate;
		}

		
		public void DeleteDataByBulkDelete(List<F1913> f1913s)
		{
			SqlBulkDeleteForAnyCondition(f1913s, "F1913", new List<string> { "DC_CODE",
																																						"GUP_CODE",
																																						"ITEM_CODE",
																																						"LOC_CODE",
																																						"VALID_DATE",
																																						"ENTER_DATE",
																																						"VNR_CODE",
																																						"SERIAL_NO",
																																						"BOX_CTRL_NO",
																																						"PALLET_CTRL_NO",
																																						"MAKE_NO"
																																						});
		}

		public List<F1913> GetDatas(string dcCode, string gupCode, string custCode, string itemCode, string locCode)
		{

			var result = _db.F1913s.Where(x => x.DC_CODE == dcCode
					&& x.GUP_CODE == gupCode
					&& x.CUST_CODE == custCode
					&& x.ITEM_CODE == itemCode
					&& x.LOC_CODE == locCode
					&& x.QTY > 0);

			return result.ToList();
		}

		public IQueryable<F1913> GetDatas(string dcCode, string gupCode, string custCode, string itemCode)
		{
			var result = _db.F1913s.Where(x => x.DC_CODE == dcCode
					&& x.GUP_CODE == gupCode
					&& x.CUST_CODE == custCode
					&& x.ITEM_CODE == itemCode
					&& x.QTY > 0);

			return result;
		}

		public F1913 GetData(string dcCode, string gupCode, string custCode, string itemCode, string locCode, DateTime validDate, DateTime enterDate, string serialNo, string vnrCode, string boxCtrlNo, string palletCtrlNo, string makeNo)
		{
			var result = _db.F1913s.Where(x =>
					x.DC_CODE == dcCode &&
					x.GUP_CODE == gupCode &&
					x.CUST_CODE == custCode &&
					x.ITEM_CODE == itemCode &&
					x.LOC_CODE == locCode &&
					x.VALID_DATE == validDate &&
					x.ENTER_DATE == enterDate &&
					x.SERIAL_NO == serialNo &&
					x.VNR_CODE == vnrCode &&
					x.BOX_CTRL_NO == boxCtrlNo &&
					x.PALLET_CTRL_NO == palletCtrlNo &&
					x.MAKE_NO == makeNo
					);

			return result.FirstOrDefault();
		}

		public void UpdateQtyByBulkUpdate(List<F1913> f1913s)
		{
			BulkUpdate(f1913s);
		}

		public IQueryable<F1913> GetItemGoldenLocs(string dcCode, string gupCode, string custCode, string itemCode, string warehouseType = null, string warehouseId = null)
		{

			var f1913s = _db.F1913s.Where(x => x.GUP_CODE == gupCode
											&& x.CUST_CODE == custCode
											&& x.DC_CODE == dcCode
											&& x.ITEM_CODE == itemCode);
			var f1912s = _db.F1912s.AsQueryable();
			var f1980s = _db.F1980s.AsQueryable();
			var f1919s = _db.F1919s.Where(x => x.ATYPE_CODE == "A" && x.DC_CODE == dcCode);

			if (!string.IsNullOrEmpty(warehouseId))
			{
				f1980s = f1980s.Where(x => x.WAREHOUSE_ID == warehouseId);
			}
			else if (!string.IsNullOrEmpty(warehouseType))
			{
				f1980s = f1980s.Where(x => x.WAREHOUSE_TYPE == warehouseType);
			}

			var result = from A in f1913s
									 join B in f1912s on new { A.LOC_CODE, A.DC_CODE } equals new { B.LOC_CODE, B.DC_CODE }
									 join C in f1980s on new { B.WAREHOUSE_ID, B.DC_CODE } equals new { C.WAREHOUSE_ID, C.DC_CODE }
									 join D in f1919s on new { B.WAREHOUSE_ID, B.DC_CODE, B.AREA_CODE } equals new { D.WAREHOUSE_ID, D.DC_CODE, D.AREA_CODE }
									 select A;

			return result;
		}

		public List<F1913> GetItemLocData(string dcCode, string gupCode, string custCode, string itemCode, string locCdoe)
		{
			var result = _db.F1913s.Where(x => x.DC_CODE == dcCode &&
																					x.GUP_CODE == gupCode &&
																					x.CUST_CODE == custCode &&
																					x.VALID_DATE >= DateTime.Now);

			if (!string.IsNullOrEmpty(itemCode))
			{
				result = result.Where(x => x.ITEM_CODE == itemCode);
			}

			if (!string.IsNullOrEmpty(locCdoe))
			{
				result = result.Where(x => x.LOC_CODE == locCdoe);
			}


			return result.ToList();
		}

		public IQueryable<string> GetItemMixItemLoc(string dcCode, string gupCode, string custCode, string itemCode)
		{

			var f1913s = _db.F1913s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
																										 x.GUP_CODE == gupCode &&
																										 x.CUST_CODE == custCode &&
																										 x.ITEM_CODE != itemCode);



			var f1913sExists = _db.F1913s.AsNoTracking().Where(x => x.ITEM_CODE == itemCode &&
																														 x.DC_CODE == dcCode &&
																															x.GUP_CODE == gupCode &&
																															x.CUST_CODE == custCode &&
																															f1913s.Select(z => z.LOC_CODE).Contains(x.LOC_CODE))
																								 .Select(x => x.LOC_CODE);

			var result = f1913s.Where(x => f1913sExists.Contains(x.LOC_CODE)).Select(x => x.LOC_CODE);
			return result;
		}

		public int GetItemStock(string dcCode, string gupCode, string custCode, string itemCode, string warehouseType, bool isCareValid = false, string makeNo = "")
		{
			var f1913s = _db.F1913s.AsNoTracking().Where(x => x.CUST_CODE == custCode
																										&& x.DC_CODE == dcCode
																										&& x.ITEM_CODE == itemCode);

			var f1912s = _db.F1912s.AsNoTracking().Where(x => x.AREA_CODE != "-1");

			var f1980s = _db.F1980s.AsNoTracking().Where(x => x.WAREHOUSE_TYPE == warehouseType);
			var f1919s = _db.F1919s.AsNoTracking();
			if (isCareValid)
			{
				f1913s = f1913s.Where(x => x.VALID_DATE >= DateTime.Now);
			}
			if (!string.IsNullOrEmpty(makeNo))
			{
				f1913s = f1913s.Where(x => x.MAKE_NO == makeNo);
			}

			var result = Convert.ToInt32((from A in f1913s
																		join B in f1912s on new { A.LOC_CODE, A.DC_CODE } equals new { B.LOC_CODE, B.DC_CODE }
																		join C in f1980s on new { B.WAREHOUSE_ID, B.DC_CODE } equals new { C.WAREHOUSE_ID, C.DC_CODE }
																		join D in f1919s on new { B.WAREHOUSE_ID, B.DC_CODE, B.AREA_CODE } equals new { D.WAREHOUSE_ID, D.DC_CODE, D.AREA_CODE }
																		select new { A.QTY }).Sum(x => x.QTY));

			return result;
		}

		public IQueryable<F1913> GetDatas(string dcCode, string gupCode, string custCode, string locCode, string itemCode,
				 DateTime validDate, DateTime enterDate, string boxCtrlNo, string palletCtrlNo, string makeNo)
		{
			var result = _db.F1913s.Where(x => x.DC_CODE == dcCode
																	&& x.GUP_CODE == gupCode
																	&& x.CUST_CODE == custCode
																	&& x.LOC_CODE == locCode
																	&& x.ITEM_CODE == itemCode
																	&& x.VALID_DATE == validDate
																	&& x.ENTER_DATE == enterDate
																	&& x.BOX_CTRL_NO == boxCtrlNo
																	&& x.PALLET_CTRL_NO == palletCtrlNo
																	);
			if (!string.IsNullOrWhiteSpace(makeNo))
			{
				result = result.Where(x => x.MAKE_NO == makeNo);
			}
			else
			{
				result = result.Where(x => x.MAKE_NO == null);
			}
			return result;
		}

		public DateTime? GetMinEnterDate(string dcCode, string gupCode, string custCode, string itemCode, string vnrCode, DateTime validDate)
		{
			var result = _db.F1913s.AsNoTracking().Where(x => x.DC_CODE == dcCode
																									&& x.GUP_CODE == gupCode
																									&& x.CUST_CODE == custCode
																									&& x.ITEM_CODE == itemCode
																									&& x.VNR_CODE == vnrCode
																									&& x.VALID_DATE == validDate
																									&& x.QTY > 0).OrderBy(y => y.ENTER_DATE).Select(z => z.ENTER_DATE);


			return result.Any() ? result.First() : (DateTime?)null;

		}

		public DateTime? GetMinValidDate(string dcCode, string gupCode, string custCode, string itemCode, string vnrCode)
		{
			var result = _db.F1913s.AsNoTracking().Where(x => x.DC_CODE == dcCode
																							 && x.GUP_CODE == gupCode
																							 && x.CUST_CODE == custCode
																							 && x.ITEM_CODE == itemCode
																							 && x.VNR_CODE == vnrCode
																							 && x.QTY > 0).OrderBy(y => y.VALID_DATE).Select(z => z.VALID_DATE);
			return result.Any() ? result.First() : (DateTime?)null;
		}


		public IQueryable<F1913> GetDatasByItems(string dcCode, string gupCode, string custCode, List<string> itemCodes)
		{
			var result = _db.F1913s.Where(x => x.DC_CODE == dcCode
																	&& x.GUP_CODE == gupCode
																	&& x.CUST_CODE == custCode);

			if (itemCodes.Count() > 0)
			{
				result = result.Where(y => itemCodes.Contains(y.ITEM_CODE));
			}

			return result;
		}

    /// <summary>
    /// 撈出指定儲位庫存，並排除數量為0的庫存資料
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="gupCode"></param>
    /// <param name="custCode"></param>
    /// <param name="locCodes"></param>
    /// <returns></returns>
		public IQueryable<F1913> GetDatasByLocs(string dcCode, string gupCode, string custCode, List<string> locCodes)
		{
			var result = _db.F1913s.Where(x => x.DC_CODE == dcCode
															&& x.GUP_CODE == gupCode
                              && x.CUST_CODE == custCode
                              && x.QTY > 0);
      if (locCodes.Count() > 0)
			{
				result = result.Where(x => locCodes.Contains(x.LOC_CODE));
			}
			return result;
		}

		#region 效期調整

		public IQueryable<F1913> SearchStockData(string dcCode, string gupCode, string custCode, string locCode, string itemCode,
						DateTime validDate, DateTime enterDate, string boxCtrlNo, string palletCtrlNo, string makeNo, string vnrCode, string serialNo)
		{
			var result = _db.F1913s.Where(x => x.DC_CODE == dcCode
																	&& x.GUP_CODE == gupCode
																	&& x.CUST_CODE == custCode
																	&& x.LOC_CODE == locCode
																	&& x.ITEM_CODE == itemCode
																	&& x.VALID_DATE == validDate
																	&& x.ENTER_DATE == enterDate
																	&& x.BOX_CTRL_NO == boxCtrlNo
																	&& x.PALLET_CTRL_NO == palletCtrlNo
																	&& x.MAKE_NO == makeNo
																	&& x.VNR_CODE == vnrCode
																	&& x.SERIAL_NO == serialNo);

			return result;
		}

		#endregion 效期調整



		public IQueryable<F1913> GetDatasForF151002s(string dcCode, string custCode, List<F151002> f151002s)
		{
			var result = _db.F1913s.Where(x => x.CUST_CODE == custCode &&
			x.DC_CODE == dcCode &&
			f151002s.Any(z => z.TAR_LOC_CODE == x.LOC_CODE &&
			z.ENTER_DATE == x.ENTER_DATE &&
			z.VALID_DATE == x.VALID_DATE &&
			z.MAKE_NO == x.MAKE_NO &&
			z.PALLET_CTRL_NO == x.PALLET_CTRL_NO &&
			(string.IsNullOrWhiteSpace(z.SERIAL_NO) ? "0" : z.SERIAL_NO) == x.SERIAL_NO &&
			z.BOX_CTRL_NO == x.BOX_CTRL_NO &&
			z.VNR_CODE == x.VNR_CODE &&
			z.ITEM_CODE == x.ITEM_CODE));

			return result;
		}

		public IQueryable<F1913> GetDatasForF1511s(string dcCode, string custCode, IQueryable<F1511> f1511s)
		{
			var result = _db.F1913s.Where(x =>
			x.CUST_CODE == custCode &&
			x.DC_CODE == dcCode &&
			f1511s.Any(z => z.LOC_CODE == x.LOC_CODE &&
			z.ENTER_DATE == x.ENTER_DATE &&
			z.VALID_DATE == x.VALID_DATE &&
			z.MAKE_NO == x.MAKE_NO &&
			z.PALLET_CTRL_NO == x.PALLET_CTRL_NO &&
			z.SERIAL_NO == x.SERIAL_NO &&
			z.BOX_CTRL_NO == x.BOX_CTRL_NO &&
			z.ITEM_CODE == x.ITEM_CODE));

			return result;
		}

		public IQueryable<F1913> GetDatasForF051202s(IQueryable<F051202> f051202s)
		{
			var result = _db.F1913s.Where(x =>
			f051202s.Any(z =>
			z.PICK_LOC == x.LOC_CODE &&
			z.ITEM_CODE == x.ITEM_CODE &&
			z.VALID_DATE == x.VALID_DATE &&
			z.ENTER_DATE == x.ENTER_DATE &&
			z.DC_CODE == x.DC_CODE &&
			z.GUP_CODE == x.GUP_CODE &&
			z.CUST_CODE == x.CUST_CODE &&
			z.VNR_CODE == x.VNR_CODE &&
			z.MAKE_NO == x.MAKE_NO &&
			z.PALLET_CTRL_NO == x.PALLET_CTRL_NO &&
			(string.IsNullOrWhiteSpace(z.SERIAL_NO) ? "0" : z.SERIAL_NO) == x.SERIAL_NO &&
			z.BOX_CTRL_NO == x.BOX_CTRL_NO
			));

			return result;
		}

		public IQueryable<StockDataByInventory> GetStockQtyByInventory0(string dcCode, string gupCode, string custCode, string warehouseId, List<StockDataByInventoryParam> param)
		{
			var f1912s = _db.F1912s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
										x.GUP_CODE == gupCode &&
										x.CUST_CODE == custCode &&
										x.WAREHOUSE_ID == warehouseId);

			var locCodes = f1912s.Select(z => z.LOC_CODE);

			#region 實際庫存數
			var f1913s = _db.F1913s.AsNoTracking().Where(x =>
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			locCodes.Contains(x.LOC_CODE) &&
			param.Any(z =>
			z.ITEM_CODE == x.ITEM_CODE &&
			z.VALID_DATE == x.VALID_DATE &&
			z.MAKE_NO == x.MAKE_NO &&
            z.LOC_CODE == x.LOC_CODE &&
            z.ENTER_DATE == x.ENTER_DATE &&
            z.BOX_CTRL_NO == x.BOX_CTRL_NO &&
            z.PALLET_CTRL_NO == x.PALLET_CTRL_NO));

			var stockDatas = f1913s.GroupBy(x => new StockDataByInventoryParam
            {
                ITEM_CODE = x.ITEM_CODE,
                VALID_DATE = Convert.ToDateTime(x.VALID_DATE),
                MAKE_NO = x.MAKE_NO,
                LOC_CODE = x.LOC_CODE,
                ENTER_DATE = x.ENTER_DATE,
                BOX_CTRL_NO = x.BOX_CTRL_NO,
                PALLET_CTRL_NO = x.PALLET_CTRL_NO
            })
			.Select(x => new StockDataByInventory
			{
				ITEM_CODE = x.Key.ITEM_CODE,
				VALID_DATE = x.Key.VALID_DATE,
				MAKE_NO = x.Key.MAKE_NO,
                LOC_CODE = x.Key.LOC_CODE,
                ENTER_DATE = x.Key.ENTER_DATE,
                BOX_CTRL_NO = x.Key.BOX_CTRL_NO,
                PALLET_CTRL_NO = x.Key.PALLET_CTRL_NO,
                QTY = Convert.ToInt32(x.Sum(z => z.QTY))
			});
			#endregion

			#region 虛擬未搬動庫存數
			var f1511s = _db.F1511s.AsNoTracking().Where(x =>
			x.DC_CODE == dcCode &&
			x.GUP_CODE == gupCode &&
			x.CUST_CODE == custCode &&
			locCodes.Contains(x.LOC_CODE) &&
			x.STATUS == "0" &&
			param.Any(z =>
			z.ITEM_CODE == x.ITEM_CODE &&
			z.VALID_DATE == x.VALID_DATE &&
			z.MAKE_NO == x.MAKE_NO &&
            z.LOC_CODE == x.LOC_CODE &&
            z.ENTER_DATE == x.ENTER_DATE &&
            z.BOX_CTRL_NO == x.BOX_CTRL_NO &&
            z.PALLET_CTRL_NO == x.PALLET_CTRL_NO));

			var virtualDatas = f1511s.GroupBy(x => new
            {
                x.ITEM_CODE,
                VALID_DATE = Convert.ToDateTime(x.VALID_DATE),
                x.MAKE_NO,
                x.LOC_CODE,
                x.ENTER_DATE,
                x.BOX_CTRL_NO,
                x.PALLET_CTRL_NO
            }).ToList()
			.Select(x => new StockDataByInventory
			{
				ITEM_CODE = x.Key.ITEM_CODE,
				VALID_DATE = x.Key.VALID_DATE,
				MAKE_NO = x.Key.MAKE_NO,
                LOC_CODE = x.Key.LOC_CODE,
                ENTER_DATE = Convert.ToDateTime(x.Key.ENTER_DATE),
                BOX_CTRL_NO = x.Key.BOX_CTRL_NO,
                PALLET_CTRL_NO = x.Key.PALLET_CTRL_NO,
                QTY = x.Sum(z => z.B_PICK_QTY)
			});
			#endregion

			var datas = from A in param
									join B in stockDatas
									on new { A.ITEM_CODE, A.VALID_DATE, A.MAKE_NO, A.LOC_CODE, A.ENTER_DATE, A.BOX_CTRL_NO, A.PALLET_CTRL_NO } equals new { B.ITEM_CODE, B.VALID_DATE, B.MAKE_NO, B.LOC_CODE, B.ENTER_DATE, B.BOX_CTRL_NO, B.PALLET_CTRL_NO } into subB
									from B in subB.DefaultIfEmpty()
									join C in virtualDatas
									on new { A.ITEM_CODE, A.VALID_DATE, A.MAKE_NO, A.LOC_CODE, A.ENTER_DATE, A.BOX_CTRL_NO, A.PALLET_CTRL_NO } equals new { C.ITEM_CODE, C.VALID_DATE, C.MAKE_NO, C.LOC_CODE, C.ENTER_DATE, C.BOX_CTRL_NO, C.PALLET_CTRL_NO } into subC
									from C in subC.DefaultIfEmpty()
									select new StockDataByInventory
									{
										ITEM_CODE = A.ITEM_CODE,
										VALID_DATE = A.VALID_DATE,
										MAKE_NO = A.MAKE_NO,
                                        LOC_CODE = A.LOC_CODE,
                                        ENTER_DATE = A.ENTER_DATE,
                                        BOX_CTRL_NO = A.BOX_CTRL_NO,
                                        PALLET_CTRL_NO = A.PALLET_CTRL_NO,
										QTY = B == null ? 0 : B.QTY,
										UNMOVE_STOCK_QTY = C == null ? 0 : C.QTY
									};

			return datas.AsQueryable();
		}

		public IQueryable<WcsSkuCodeModel> GetDatasByWcsSnapshotStocks(List<SnapshotStocksReceiptSkuModel> skuList)
		{
			return _db.F1903s.AsNoTracking().Where(x => skuList.Any(z => z.OwnerCode == x.CUST_CODE && z.SkuCode == x.ITEM_CODE))
				.Select(x => new WcsSkuCodeModel { SkuCode = x.ITEM_CODE, OwnerCode = x.CUST_CODE, GupCode = x.GUP_CODE });
		}

		public IQueryable<ExpStockAlert> GetDatasByExpStockAlert(string dcCode, string gupCode, string custCode)
		{
			var f1913s = _db.F1913s.Where(x => x.DC_CODE == dcCode
										&& x.GUP_CODE == gupCode
										&& x.CUST_CODE == custCode
										//&& x.VALID_DATE < DateTime.Now 
										&& x.QTY > 0);
			var f1903s = _db.F1903s.Where(x => x.GUP_CODE == gupCode
										&& x.CUST_CODE == custCode);
			//&& x.ALL_SHP > 0);
			var result = from A in f1913s
									 join B in f1903s
									 on new { A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE } equals new { B.GUP_CODE, B.CUST_CODE, B.ITEM_CODE }
									 where A.VALID_DATE < DateTime.Now.AddDays(Convert.ToDouble(B.ALL_SHP ?? 0))
									 select new ExpStockAlert
									 {
										 DcCode = A.DC_CODE,
										 GupCode = A.GUP_CODE,
										 CustCode = A.CUST_CODE,
										 ItemCode = A.ITEM_CODE,
										 ValidDate = A.VALID_DATE,
										 MakeNo = A.MAKE_NO,
										 Qty = (int)A.QTY,
										 AllShp = B.ALL_SHP ?? 0
									 };
			return result;
		}

		public IQueryable<F1913> GetDatasByQtyNotZero(string dcCode, string gupCode, string custCode, List<string> itemCodes)
		{
			var result = _db.F1913s.AsNoTracking().Where(x => x.DC_CODE == dcCode
																	&& x.GUP_CODE == gupCode
																	&& x.CUST_CODE == custCode
																	&& x.QTY > 0
																	&& itemCodes.Contains(x.ITEM_CODE));

			return result;
		}

		public bool LocCodeIsExistStock(string dcCode, string locCode)
		{
			var result = _db.F1913s.AsNoTracking().Where(x => x.DC_CODE == dcCode
																	&& x.LOC_CODE == locCode
																	&& x.QTY > 0).Any();
			return result;
		}

  }
}
