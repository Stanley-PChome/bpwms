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

namespace Wms3pl.Datas.F19
{
    public partial class F1980Repository : RepositoryBase<F1980, Wms3plDbContext, F1980Repository>
    {
        public F1980Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
        {
        }

        #region P1601020000 使用者被設定的作業區(倉別清單)
        public IQueryable<UserWarehouse> GetUserWarehouse(string userId, string gupCode, string custCode)
        {
            var result = (from A in _db.F1980s
                          join B in _db.F1912s.Where(x => (x.GUP_CODE == gupCode || x.GUP_CODE == "0") && (x.CUST_CODE == custCode || x.CUST_CODE == "0"))
                          on new { A.DC_CODE, A.WAREHOUSE_ID } equals new { B.DC_CODE, B.WAREHOUSE_ID }
                          join C in _db.F196301s on new { A.DC_CODE, B.LOC_CODE } equals new { C.DC_CODE, C.LOC_CODE }
                          join E in _db.F192403s on C.WORK_ID equals E.WORK_ID
													where E.EMP_ID == userId
                          group new { A } by new { E.EMP_ID, A.DC_CODE, A.WAREHOUSE_ID, A.WAREHOUSE_NAME, A.TMPR_TYPE } into K
                          select new UserWarehouse
                          {
                              DC_CODE = K.Key.DC_CODE,
                              Value = K.Key.WAREHOUSE_ID,
                              Name = K.Key.WAREHOUSE_NAME,
                              TMPR_TYPE = K.Key.TMPR_TYPE
                          }).Distinct().AsNoTracking();
            return result;
            #endregion
        }

        public F1980 GetFirstData(string dcCode, string warehouseType)
        {
            var result = _db.F1980s.Where(x => x.DC_CODE == dcCode
                                        && x.WAREHOUSE_TYPE == warehouseType).FirstOrDefault();

            return result;
        }

        public IQueryable<InventoryWareHouse> GetInventoryWareHouses(string dcCode, string wareHouseType, string tool)
        {
            var f1980s = _db.F1980s.AsNoTracking().Where(x => x.DC_CODE == dcCode);
            var f1919s = _db.F1919s.AsNoTracking();
            var f191902s = _db.F191902s.AsNoTracking();

						if (!string.IsNullOrWhiteSpace(tool))
							f1980s = f1980s.Where(x => x.DEVICE_TYPE == tool);


						if (!string.IsNullOrEmpty(wareHouseType))
            {
                f1980s = f1980s.Where(x => x.WAREHOUSE_TYPE == wareHouseType);
               
            }

            if (tool == "1")
            {
                f191902s = f191902s.Where(x => x.PICK_TOOL == "4");
              
            }

            var result = from A in f1980s
                         join B in f1919s on new { A.DC_CODE, A.WAREHOUSE_ID } equals new { B.DC_CODE, B.WAREHOUSE_ID } 
                         join C in f191902s on new { A.DC_CODE, A.WAREHOUSE_ID, B.AREA_CODE } equals new { C.DC_CODE, C.WAREHOUSE_ID, C.AREA_CODE } into CC
                         from C in CC.DefaultIfEmpty()
                         select new InventoryWareHouse
                         {
                             DC_CODE = A.DC_CODE,
                             WAREHOUSE_ID = A.WAREHOUSE_ID,
                             WAREHOUSE_NAME = A.WAREHOUSE_NAME,
                             AREA_CODE = B.AREA_CODE,
                             AREA_NAME = B.AREA_NAME
                         };

            return result;
        }

        public F1980 GetF1980ByLocCode(string dcCode, string locCode)
        {
            var f1980s = _db.F1980s;
            var f1912s = _db.F1912s.Where(x => x.DC_CODE == dcCode
                                       && x.LOC_CODE == locCode);

            var result = from A in f1980s
                         join B in f1912s on new { A.DC_CODE, A.WAREHOUSE_ID } equals new { B.DC_CODE, B.WAREHOUSE_ID }
                         select A;

            return result.FirstOrDefault();
        }

        public IQueryable<WareHouseTmprTypeByLocCode> GetWareHouseTmprTypeByLocCode(string dcCode, List<string> locCodes)
        {
            var result = from A in _db.F1980s.AsNoTracking()
                         join B in _db.F1912s.AsNoTracking() on new { A.DC_CODE, A.WAREHOUSE_ID } equals new { B.DC_CODE, B.WAREHOUSE_ID }
                         where B.DC_CODE == dcCode && locCodes.Contains(B.LOC_CODE)
                         select new WareHouseTmprTypeByLocCode
                         {
                             DC_CODE = A.DC_CODE,
                             LOC_CODE = B.LOC_CODE,
                             TMPR_TYPE = A.TMPR_TYPE
                         };

            return result;
        }

        public IQueryable<WareHouseIdByWareHouseType> GetWareHouseIdByWareHouseTypeList(string gupCode, string custCode)
        {
            var f1980s = _db.F1980s.AsNoTracking();
            var f198001s = _db.F198001s.Where(x => x.ITEM_PICK_WARE == "1").AsNoTracking();
			      var f190101s = _db.F190101s.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode).Select(x=> new { x.DC_CODE }).Distinct().AsNoTracking();
            //var f1912s = _db.F1912s.Where(x => x.GUP_CODE == gupCode
            //                             && (x.CUST_CODE =="0" || custCode.Contains(x.CUST_CODE)));
            var result = (from A in f1980s
                         join B in f198001s on A.WAREHOUSE_TYPE equals B.TYPE_ID
                         join C in f190101s on new { A.DC_CODE } equals new { C.DC_CODE }
                         select new WareHouseIdByWareHouseType
                         {
                             WAREHOUSE_ID = A.WAREHOUSE_ID,
                             WAREHOUSE_NAME = A.WAREHOUSE_NAME,
                             WAREHOUSE_TYPE = A.WAREHOUSE_TYPE
                         }).Distinct();

            return result.AsNoTracking();
        }

        public IQueryable<F1980> GetDatas(string dcCode, List<string> warehouseIds)
        {
            var result = _db.F1980s.Where(x => x.DC_CODE == dcCode
                                        && warehouseIds.Contains(x.WAREHOUSE_ID));
            return result;
        }

        public IQueryable<WareHouseTmprTypeByLocCode> GetWareHouseTmprTypeByLocCodes(List<string> dcCodes, List<string> locCodes)
        {
            var f1980s = _db.F1980s.AsNoTracking().Where(x=> dcCodes.Contains(x.DC_CODE));
            var f1912s = _db.F1912s.AsNoTracking().Where(x=>locCodes.Contains(x.LOC_CODE));
            var result = from A in f1980s
                         join B in f1912s on new { A.DC_CODE, A.WAREHOUSE_ID } equals new { B.DC_CODE, B.WAREHOUSE_ID }
                         select new WareHouseTmprTypeByLocCode
                         {
                             DC_CODE = A.DC_CODE,
                             LOC_CODE = B.LOC_CODE,
                             TMPR_TYPE = A.TMPR_TYPE
                         };
            return result;
        }

        public WareHouseTmprTypeByLocCode GetWareHouseTmprTypeByWareHouse(string dcCode, string warehouseId)
        {
            var f1980s = _db.F1980s.AsNoTracking().Where(x => x.DC_CODE == dcCode
                                                        && x.WAREHOUSE_ID == warehouseId);
            var f1912s = _db.F1912s.AsNoTracking();

            var result = from A in f1980s
                         join B in f1912s on new { A.DC_CODE, A.WAREHOUSE_ID } equals new { B.DC_CODE, B.WAREHOUSE_ID }
                         select new WareHouseTmprTypeByLocCode
                         {
                             DC_CODE = A.DC_CODE,
                             LOC_CODE = B.LOC_CODE,
                             TMPR_TYPE = A.TMPR_TYPE
                         };
            return result.FirstOrDefault();
        }

        /// <summary>
        /// 取得倉別名稱
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public string GetWhName(string dcCode, string warehouseId)
        {
            var result = _db.F1980s.AsNoTracking().Where(x => x.DC_CODE == dcCode && x.WAREHOUSE_ID == warehouseId)
                                                  .Select(x=>x.WAREHOUSE_NAME)
                                                  .SingleOrDefault();

            return result;
        }

        public IQueryable<F1980> GetDatasForWarehouseTypes(string dcCode, List<string> warehouseTypes)
        {
            var result = _db.F1980s.Where(x => x.DC_CODE == dcCode && warehouseTypes.Contains(x.WAREHOUSE_TYPE));
            return result;
        }

        public IQueryable<F1980> GetDatasByWarehouseId(List<string> warehouseIds)
        {
            return _db.F1980s.Where(x => warehouseIds.Contains(x.WAREHOUSE_ID));
        }

		public IQueryable<F1980> GetAutoWarehourse(string dcCode)
		{
			var result = _db.F1980s.Where(x => x.DC_CODE == dcCode && x.DEVICE_TYPE != "0");
			return result;
		}

		public IQueryable<F1980> GetDatasByWarehouseId(string dcCode, List<string> warehouseIds)
		{
			return _db.F1980s.AsNoTracking().Where(x => x.DC_CODE == dcCode && warehouseIds.Contains(x.WAREHOUSE_ID));
		}

        public IQueryable<F1980> GetDataByGoodWh(string dcCode)
        {
            var result = _db.F1980s.Where(x => x.DC_CODE == dcCode && x.WAREHOUSE_ID.StartsWith("G"));
            return result;
        }
    }
}
