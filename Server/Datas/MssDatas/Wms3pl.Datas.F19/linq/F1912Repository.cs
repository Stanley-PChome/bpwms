using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F15;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1912Repository : RepositoryBase<F1912, Wms3plDbContext, F1912Repository>
    {
        public F1912Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }
        /// <summary>
        /// 倉別儲位資料
        /// </summary>
        /// <param name="wareHouseId">倉別編號</param>
        /// <returns></returns>
        public IQueryable<F1912> GetF1912DatasByWareHouseId(string dcCode, string wareHouseId)
        {
            var areaF1912Data = this.Filter(o => o.WAREHOUSE_ID == wareHouseId && o.DC_CODE == dcCode);
            return areaF1912Data;
        }
        /// <summary>
        /// 取得該作業群組已設定之儲位
        /// </summary>
        /// <param name="workgroupId"></param>
        /// <param name="dcCode"></param>
        /// <returns></returns>
        public IQueryable<F1912> GetAssignedLoc(string workgroupId, string dcCode)
        {
            // 2015/3/10 與鈺綺討論因 UI 上的 DC 變動時，要不要更新已設定儲位的資料，
            // 為了不更新已設定的儲位，改為在已設定儲位顯示所有DC的階層節點。
            var query = _db.F196301s
                .Join(_db.F1912s, a => new { a.DC_CODE, a.LOC_CODE }, b => new { b.DC_CODE, b.LOC_CODE }, (a, b) => new { a, b })
                .Where(x => x.a.WORK_ID.ToString() == workgroupId)
                .Select(x => x.b);
            return query;
        }

        /// <summary>
        /// 取得該作業群組未設定之儲位
        /// </summary>
        /// <param name="workgroupId"></param>
        /// <param name="dcCode"></param>
        /// <returns></returns>
        public IQueryable<F1912> GetUnAssignedLoc(string workgroupId, string dcCode, string warehouseId
            , string floor, string startLocCode, string endLocCode)
        {
            var queryLOC_CODE = _db.F196301s.Where(b => b.WORK_ID.ToString() == workgroupId && b.DC_CODE == dcCode).Select(x => x.LOC_CODE);

            return _db.F1912s
                .Where(x => x.DC_CODE == dcCode)
                .Where(x => queryLOC_CODE.Contains(x.LOC_CODE))
                .Where(x => (x.WAREHOUSE_ID == warehouseId | warehouseId == null))
                .Where(x => (x.FLOOR == floor | floor == null))
                .Where(x => string.Compare(x.LOC_CODE, startLocCode) >= 0 | startLocCode == null)
                .Where(x => string.Compare(x.LOC_CODE, endLocCode) <= 0 | endLocCode == null)
                .OrderBy(x => x.LOC_CODE)
                .Select(x => x);
        }

        /// <summary>
        /// 儲區儲位資料
        /// </summary>
        /// <param name="areaCode">儲區代號</param>
        /// <returns></returns>
        public IQueryable<F1912> GetF1912DatasByAreaCode(string dcCode, string areaCode)
        {
            var areaF1912Data = this.Filter(o => o.AREA_CODE == areaCode && o.DC_CODE == dcCode);
            return areaF1912Data;
        }

        /// <summary>
        /// 取得存在該倉別型態的儲位
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="warehouseType"></param>
        /// <param name="locCodes"></param>
        /// <returns></returns>
        public IQueryable<string> GetLocCodesByWarehouseType(string dcCode, string gupCode, string custCode, string warehouseType, IEnumerable<string> locCodes)
        {
            var query = from a in _db.F1912s
                        join b in _db.F1980s on new { a.WAREHOUSE_ID, a.DC_CODE } equals new { b.WAREHOUSE_ID, b.DC_CODE } into b1
                        from c in b1.DefaultIfEmpty()
                        where (a.DC_CODE == dcCode)
                            && (a.GUP_CODE == gupCode || a.GUP_CODE == "0")
                            && (a.CUST_CODE == custCode || a.CUST_CODE == "0")
                            && (c.WAREHOUSE_TYPE == warehouseType)
                            && (locCodes.Contains(a.LOC_CODE))
                        select a.LOC_CODE;
            return query;
        }

        public IQueryable<F1912> GetDatas(List<string> dcCodes, List<string> locCodes)
        {
            return _db.F1912s.Where(x => dcCodes.Contains(x.DC_CODE))
                .Where(x => locCodes.Contains(x.LOC_CODE))
                .Select(x => x);
        }
        public IQueryable<string> GetFloors(string dcCode, string warehouseId)
        {
            var query = _db.F1912s
                .Where(x => x.DC_CODE == dcCode)
                .Where(x => x.WAREHOUSE_ID == (warehouseId ?? x.WAREHOUSE_ID))
                .GroupBy(x => x.FLOOR)
                .Select(x => new { FLOOR = x.Key })
                .OrderBy(x => x.FLOOR);
            return query.Select(x => x.FLOOR);
        }

        //	/// <summary>
        //	/// 取得該物流中心目前所有的通道
        //	/// </summary>
        //	/// <param name="dcCode"></param>
        //	/// <param name="warehouseId"></param>
        //	/// <param name="floor"></param>
        //	/// <returns></returns>
        public IQueryable<string> GetChanels(string dcCode, string warehouseId, string floor)
        {
            var query = _db.F1912s
                            .Where(x => x.DC_CODE == dcCode)
                            .Where(x => x.WAREHOUSE_ID == (warehouseId ?? x.WAREHOUSE_ID))
                            .Where(x => x.FLOOR == (floor ?? x.FLOOR));
            return query.GroupBy(x => x.CHANNEL).Select(x => x.Key).OrderBy(x => x);
        }

        public IQueryable<WareHouseFloor> GetWareHouseFloorList(string wareHouseType = null)
        {
            var q = _db.F1912s.Join(_db.F1980s, a => new { a.DC_CODE, a.WAREHOUSE_ID }, b => new { b.DC_CODE, b.WAREHOUSE_ID }, (a, b) => new { a, b });
            if (!string.IsNullOrEmpty(wareHouseType)) q = q.Where(x => x.b.WAREHOUSE_TYPE == wareHouseType);
            var qResult = q.Select(x => new WareHouseFloor
            {
                WAREHOUSE_ID = x.a.WAREHOUSE_ID,
                FLOOR = x.a.FLOOR,
                ROWNUM = 0
            })
            .Distinct()
            .OrderBy(x => x.WAREHOUSE_ID)
            .ThenBy(x => x.FLOOR)
            .ToList();

            //賦予編號
            for (int i = 0; i < qResult.Count(); i++) qResult[i].ROWNUM = i + 1;
            return qResult.AsQueryable();
        }

        public IQueryable<WareHouseChannel> GetWareHouseChannelList(string wareHouseType = null)
        {
            var q = _db.F1912s.Join(_db.F1980s, a => new { a.DC_CODE, a.WAREHOUSE_ID }, b => new { b.DC_CODE, b.WAREHOUSE_ID }, (a, b) => new { a, b });
            if (!string.IsNullOrEmpty(wareHouseType)) q = q.Where(x => x.b.WAREHOUSE_TYPE == wareHouseType);
            var qResult = q.Select(x => new WareHouseChannel
            {
                WAREHOUSE_ID = x.a.WAREHOUSE_ID,
                CHANNEL = x.a.CHANNEL,
                ROWNUM = 0
            })
            .Distinct()
            .OrderBy(x => x.WAREHOUSE_ID)
            .ThenBy(x => x.CHANNEL)
            .ToList();

            //賦予編號
            for (int i = 0; i < qResult.Count(); i++) qResult[i].ROWNUM = i + 1;
            return qResult.AsQueryable();
        }

        public IQueryable<WareHousePlain> GetWareHousePlainList(string wareHouseType = null)
        {
            var q = _db.F1912s.Join(_db.F1980s, a => new { a.DC_CODE, a.WAREHOUSE_ID }, b => new { b.DC_CODE, b.WAREHOUSE_ID }, (a, b) => new { a, b });
            if (!string.IsNullOrEmpty(wareHouseType)) q = q.Where(x => x.b.WAREHOUSE_TYPE == wareHouseType);
            var qResult = q.Select(x => new WareHousePlain
            {
                WAREHOUSE_ID = x.a.WAREHOUSE_ID,
                PLAIN = x.a.PLAIN,
                ROWNUM = 0
            })
            .Distinct()
            .OrderBy(x => x.WAREHOUSE_ID)
            .ThenBy(x => x.PLAIN)
            .ToList();

            //賦予編號
            for (int i = 0; i < qResult.Count(); i++) qResult[i].ROWNUM = i + 1;
            return qResult.AsQueryable();
        }

        public IQueryable<F1912> GetReturnF055001Datas(string dcCode, string gupCode, string custCode, string warehouseType)
        {
            return _db.F1912s
                        .Join(_db.F1980s, a => new { a.DC_CODE, a.WAREHOUSE_ID }, b => new { b.DC_CODE, b.WAREHOUSE_ID }, (a, b) => new { a, b })
                        .Where(x => x.a.DC_CODE == dcCode)
                        .Where(x => x.a.GUP_CODE == gupCode || x.a.GUP_CODE == "0")
                        .Where(x => x.a.CUST_CODE == custCode || x.a.CUST_CODE == "0")
                        .Where(x => x.b.WAREHOUSE_TYPE == warehouseType)
                        .OrderBy(x => x.a.DC_CODE)
                        .ThenBy(x => x.a.LOC_CODE)
                        .Select(x => x.a);
        }
        /// <summary>
        /// 檢查目前登入的貨主與儲位目前使用的貨主編號是否不同的儲位資料
        /// </summary>
        /// <returns></returns>
        public IQueryable<F1912> GetDifferentNowCustCodeLoc(string dcCode, string locCode, string nowCustCode)
        {
            return _db.F1912s
                        .Join(_db.F1980s, a => new { a.DC_CODE, a.WAREHOUSE_ID }, b => new { b.DC_CODE, b.WAREHOUSE_ID }, (a, b) => new { a, b })
                        .Join(_db.F198001s, ab => new { ab.b.WAREHOUSE_TYPE }, c => new { WAREHOUSE_TYPE = c.TYPE_ID }, (ab, c) => new { ab, c })
                        .Where(x => x.c.LOC_MUSTSAME_NOWCUSTCODE == "1")
                        .Where(x => x.ab.a.DC_CODE == dcCode)
                        .Where(x => x.ab.a.LOC_CODE == locCode)
                        .Where(x => x.ab.a.NOW_CUST_CODE != "0")
                        .Where(x => x.ab.a.NOW_CUST_CODE != nowCustCode)
                        .Select(x => x.ab.a);
        }

        /// <summary>
        /// 取得儲位主檔資料
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="nowCustCode"></param>
        /// <param name="locCode"></param>
        /// <returns></returns>
        public IQueryable<F1912> GetF1912Datas(string dcCode, string gupCode, string nowCustCode, string locCode)
        {
            var result = _db.F1912s.Where(x => x.DC_CODE == dcCode
                                       && x.GUP_CODE == gupCode
                                       && x.NOW_CUST_CODE == nowCustCode
                                       && x.LOC_CODE == locCode);
            return result;
        }

        /// <summary>
        /// 取得儲位溫層
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="locCode"></param>
        /// <returns></returns>
        public TmprTypeModel GetTmprTypeData(string dcCode, string locCode)
        {
            var f1912s = _db.F1912s.AsNoTracking().Where(x => x.DC_CODE == dcCode
                                                      && x.LOC_CODE == locCode);

            var f1980s = _db.F1980s.AsNoTracking();

            var vwF000904Langs = _db.VW_F000904_LANGs.AsNoTracking().Where(x => x.TOPIC == "F1980"
                                                                        && x.SUBTOPIC == "TMPR_TYPE"
                                                                        && x.LANG == Current.Lang);
            var result = (from A in f1912s
                          join B in f1980s on new { A.DC_CODE, A.WAREHOUSE_ID } equals new { B.DC_CODE, B.WAREHOUSE_ID }
                          join V in vwF000904Langs on B.TMPR_TYPE equals V.VALUE
                          select new TmprTypeModel
                          {
                              Name = B.WAREHOUSE_NAME,
                              TmprType = B.TMPR_TYPE,
                              TmprTypeName = V.NAME
                          }).FirstOrDefault();

            return result;
        }

        /// <summary>
        /// 查詢庫存資料
        /// </summary>
        /// <returns></returns>
        public IQueryable<GetStockRes> GetInventoryInfo(string custNo, string dcNo, string gupCode, string itemNo, string mkNo,
            string sn, string whNo, string begLoc, string endLoc, string begPalletNo, string endPalletNo, DateTime? begEnterDate,
            DateTime? endEnterDate, DateTime? begValidDate, DateTime? endValidDate)
    {
      var f1943s = _db.F1943s.AsNoTracking(); // D
      var f1912s = _db.F1912s.AsNoTracking().Where(x => x.DC_CODE == dcNo); //A
      var f1913s = _db.F1913s.AsNoTracking().Where(x => x.CUST_CODE == custNo
                                                && x.GUP_CODE == gupCode); // B

      var f1903s = _db.F1903s.AsNoTracking(); // C

      var f1980 = _db.F1980s.AsNoTracking();  //E

      var f91000302 = _db.F91000302s.AsNoTracking().Where(x => x.ITEM_TYPE_ID == "001"); //F 

      var f1511s = _db.F1511s.AsNoTracking().Where(x => x.DC_CODE == dcNo && x.CUST_CODE == custNo && x.GUP_CODE == gupCode && x.STATUS == "0");

      // 若ItemNo有值, 增加條件 F1913.item_code=ItemNo
      if (!string.IsNullOrWhiteSpace(itemNo))
      {
        var itemCodes = f1903s.Where(x =>
        x.GUP_CODE == gupCode &&
        x.CUST_CODE == custNo &&
        (x.ITEM_CODE == itemNo ||
        x.EAN_CODE1 == itemNo ||
        x.EAN_CODE2 == itemNo ||
        x.EAN_CODE3 == itemNo)).Select(x => x.ITEM_CODE).ToList();

        f1913s = f1913s.Where(x => itemCodes.Contains(x.ITEM_CODE));
      }

      // 若MkNo有值, 增加條件F1913.make_no=MkNo
      if (!string.IsNullOrWhiteSpace(mkNo))
      {
        f1913s = f1913s.Where(x => x.MAKE_NO == mkNo);
      }

      // 若Sn有值，增加條件F1913.serial_no=Sn
      if (!string.IsNullOrWhiteSpace(sn))
      {
        var f2501 = _db.F2501s.AsNoTracking().Where(x => x.SERIAL_NO == sn && x.GUP_CODE == gupCode && x.CUST_CODE == custNo && x.STATUS == "A1").FirstOrDefault();
        var itemCode = f2501 != null ? f2501.ITEM_CODE : string.Empty;
        if (f1903s.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custNo && x.ITEM_CODE == itemCode).FirstOrDefault()?.BUNDLE_SERIALLOC == "1")
          f1913s = f1913s.Where(x => x.SERIAL_NO == sn);
        else
          f1913s = f1913s.Where(x => x.ITEM_CODE == itemCode);
      }

      // 若WhNo是ALL，不需要增加條件
      // 若WhNo不是ALL，增加條件F1912.warehouse_id=WhNo
      if (whNo != "ALL" && !string.IsNullOrWhiteSpace(whNo))
      {
        f1912s = f1912s.Where(x => x.WAREHOUSE_ID == whNo);
      }

      // 若BegLoc有值，增加條件F1913.loc_code>=BegLoc and <= EndLoc
      if (!string.IsNullOrWhiteSpace(begLoc))
      {
        f1913s = f1913s.Where(x => x.LOC_CODE.CompareTo(begLoc) == 1
                                && x.LOC_CODE.CompareTo(endLoc) == -1
                                || x.LOC_CODE.CompareTo(begLoc) == 0
                                || x.LOC_CODE.CompareTo(endLoc) == 0);
      }

      // 若BegPalletNo有值，增加條件F1913.pallet_no >=BegPalletNo and <=EndPalletNo
      if (!string.IsNullOrWhiteSpace(begPalletNo))
      {
        f1913s = f1913s.Where(x => x.PALLET_CTRL_NO.CompareTo(begPalletNo) == 1
                                && x.PALLET_CTRL_NO.CompareTo(endPalletNo) == -1
                                || x.PALLET_CTRL_NO.CompareTo(begPalletNo) == 0
                                || x.PALLET_CTRL_NO.CompareTo(endPalletNo) == 0);
      }

      // 若BegEnterDate有值，增加條件F1913.enter_date>= BegEnterDate and <=EndEnterDate
      if (begEnterDate != null)
      {
        f1913s = f1913s.Where(x => x.ENTER_DATE >= begEnterDate && x.ENTER_DATE <= endEnterDate);
      }

      // 若BegValidDate有值，增加條件F1913.valid_date>= BegValidDate and <= EndValidDate
      if (begValidDate != null)
      {
        f1913s = f1913s.Where(x => x.VALID_DATE >= begValidDate && x.VALID_DATE <= endValidDate);
      }


      var result = from A in f1912s
                   join B in f1913s on new { A.DC_CODE, A.LOC_CODE } equals new { B.DC_CODE, B.LOC_CODE }
                   join C in f1903s on new { B.CUST_CODE, B.ITEM_CODE } equals new { C.CUST_CODE, C.ITEM_CODE }
                   join D in f1943s on A.NOW_STATUS_ID equals D.LOC_STATUS_ID
                   join E in f1980 on new { A.DC_CODE, A.WAREHOUSE_ID } equals new { E.DC_CODE, E.WAREHOUSE_ID }
                   join F in f91000302 on C.ITEM_UNIT equals F.ACC_UNIT
                   let f1511 = f1511s
                    .Where(x => x.LOC_CODE == B.LOC_CODE && x.ITEM_CODE == B.ITEM_CODE && x.VALID_DATE == B.VALID_DATE && x.ENTER_DATE == B.ENTER_DATE && x.MAKE_NO == B.MAKE_NO)
                   select new GetStockRes
                   {
                     DcNo = B.CUST_CODE,
                     CustNo = B.CUST_CODE,
                     WhName = E.WAREHOUSE_NAME,
                     Loc = B.LOC_CODE,
                     ItemNo = B.ITEM_CODE,
                     PackRef = null,
                     ValidDate = B.VALID_DATE,
                     EnterDate = B.ENTER_DATE,
                     Unit = F.ACC_UNIT_NAME,
                     MkNo = B.MAKE_NO,
                     Sn = B.SERIAL_NO,
                     StockQty = Convert.ToInt32(B.QTY),
                     BoxNo = B.BOX_CTRL_NO,
                     PalletNo = B.PALLET_CTRL_NO,
                     LocStatus = D.LOC_STATUS_NAME,
                     DiffVDate = (B.VALID_DATE - DateTime.Now).Days,
                     ItemName = C.ITEM_NAME,
                     ItemSize = C.ITEM_SIZE,
                     ItemColor = C.ITEM_COLOR,
                     ItemSpec = C.ITEM_SPEC,
                     BPickQty = f1511.Sum(x => x.B_PICK_QTY),
                     EANCode1 = C.EAN_CODE1,
                     BundleSerialNo = C.BUNDLE_SERIALNO
                   };
      
      return result;
    }

    /// <summary>
    /// 搬移作業-儲位查詢
    /// </summary>
    /// <param name="dcCode"></param>
    /// <param name="loc"></param>
    /// <param name="custNo"></param>
    /// <param name="gupCode"></param>
    /// <param name="itemNo"></param>
    /// <returns></returns>
    public IQueryable<GetMoveLocRes> GetMoveLocRes(string dcCode, string loc, string custNo, string gupCode, string itemNo)
        {
            var f1912s = _db.F1912s.AsNoTracking().Where(x => x.DC_CODE == dcCode
                                                           && x.LOC_CODE == loc);
            var f1913s = _db.F1913s.AsNoTracking().Where(x => x.CUST_CODE == custNo
                                                           && x.GUP_CODE == gupCode);
            var f1980s = _db.F1980s.AsNoTracking();

            var f1903s = _db.F1903s.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(itemNo))
            {
                f1913s = from A in f1913s
                         join B in f1903s on new { A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE } equals new { B.GUP_CODE, B.CUST_CODE, B.ITEM_CODE }
                         where B.ITEM_CODE == itemNo || B.EAN_CODE1 == itemNo || B.EAN_CODE2 == itemNo || B.EAN_CODE3 == itemNo
                         select A;
            }

            var result = from A in f1912s
                         join B in f1913s on new { A.DC_CODE, A.LOC_CODE } equals new { B.DC_CODE, B.LOC_CODE }
                         join C in f1980s on new { A.DC_CODE, A.WAREHOUSE_ID } equals new { C.DC_CODE, C.WAREHOUSE_ID }
                         join D in f1903s on new { B.GUP_CODE, B.CUST_CODE, B.ITEM_CODE } equals new { D.GUP_CODE, D.CUST_CODE, D.ITEM_CODE }
                         where B.QTY != 0
                         select new GetMoveLocRes
                         {
                             ItemNo = B.ITEM_CODE,
                             ItemName = D.ITEM_NAME,
                             WhName = C.WAREHOUSE_NAME,
                             Loc = B.LOC_CODE,
                             ValidDate = B.VALID_DATE.ToString("yyyy/MM/dd"),
                             EnterDate = B.ENTER_DATE.ToString("yyyy/MM/dd"),
                             StockQty = Convert.ToInt32(B.QTY),
                             Sn = B.SERIAL_NO == "0" ? string.Empty : B.SERIAL_NO,
                             MkNo = B.MAKE_NO,
                             PalletNo = B.PALLET_CTRL_NO,
                             BoxNo = B.BOX_CTRL_NO
                         };
            return result;
        }

    public IQueryable<GetMoveItemLocRes> GetMoveItemLocRes(string dcCode, string loc, string custNo, string gupCode, string itemNo, string sn)
    {

      var f1912s = _db.F1912s.AsNoTracking().Where(x => x.DC_CODE == dcCode && x.AREA_CODE.Trim() != "-1" && x.NOW_STATUS_ID != "03" && x.NOW_STATUS_ID != "04");
      var f1913s = _db.F1913s.AsNoTracking().Where(x => x.CUST_CODE == custNo && x.GUP_CODE == gupCode);
      var f1980s = _db.F1980s.AsNoTracking();
      var f1903s = _db.F1903s.AsNoTracking();

      if (!string.IsNullOrWhiteSpace(sn))
      {
        var f2501 = _db.F2501s.AsNoTracking().Where(x =>
        x.GUP_CODE == gupCode &&
        x.CUST_CODE == custNo &&
        x.SERIAL_NO == sn &&
        x.STATUS == "A1").FirstOrDefault();

        if (f2501 == null)
          f1913s = new List<F1913>().AsQueryable();
        else
        {
          if (f1903s.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custNo && x.ITEM_CODE == f2501.ITEM_CODE).FirstOrDefault()?.BUNDLE_SERIALLOC == "1")
            f1913s = f1913s.Where(x => x.SERIAL_NO == sn);
          else
            f1913s = f1913s.Where(x => x.ITEM_CODE == f2501.ITEM_CODE);
        }

      }
      else
      {
        if (!string.IsNullOrWhiteSpace(itemNo))
        {
          var itemCodes = f1903s.Where(x =>
          x.GUP_CODE == gupCode &&
          x.CUST_CODE == custNo &&
          (x.ITEM_CODE == itemNo ||
          x.EAN_CODE1 == itemNo ||
          x.EAN_CODE2 == itemNo ||
          x.EAN_CODE3 == itemNo)).Select(x => x.ITEM_CODE).ToList();

          f1913s = f1913s.Where(x => itemCodes.Contains(x.ITEM_CODE));
          f1903s = f1903s.Where(x => itemCodes.Contains(x.ITEM_CODE));
        }

        if (!string.IsNullOrWhiteSpace(loc))
        {
          f1912s = f1912s.Where(x => x.LOC_CODE == loc);
        }
      }

      //var stackQty = (from A in f1912s
      //               join B in f1913s on A.LOC_CODE equals B.LOC_CODE
      //               join C in f1980s on new { A.WAREHOUSE_ID, A.DC_CODE } equals new { C.WAREHOUSE_ID, C.DC_CODE }
      //               select new { B.QTY }).Sum(x=>x.QTY);

      var result = from A in f1912s
                   join B in f1913s on A.LOC_CODE equals B.LOC_CODE
                   join C in f1980s on new { A.WAREHOUSE_ID, A.DC_CODE } equals new { C.WAREHOUSE_ID, C.DC_CODE }
                   join D in f1903s on new { B.GUP_CODE, B.CUST_CODE, B.ITEM_CODE } equals new { D.GUP_CODE, D.CUST_CODE, D.ITEM_CODE }
                   where B.QTY != 0
                   group B by new { B.ITEM_CODE, B.LOC_CODE, C.WAREHOUSE_NAME, D.ITEM_NAME } into g
                   select new GetMoveItemLocRes
                   {
                     WhName = g.Key.WAREHOUSE_NAME,
                     ItemName = g.Key.ITEM_NAME,
                     ItemNo = g.Key.ITEM_CODE,
                     Loc = g.Key.LOC_CODE,
                     StockQty = Convert.ToInt32(g.Sum(x => x.QTY))
                   };

      return result;
    }

        /// <summary>
        /// 檢核儲位是否存在
        /// </summary>
        /// <param name="dcCode"></param>
        /// <returns></returns>
        public bool CheckLocExist(string dcCode)
        {
            var result = _db.F1912s.AsNoTracking().Where(x => x.DC_CODE == dcCode);

            return result.Count() > 0;
        }

        /// <summary>
        /// 檢核儲位是否非此貨主儲位
        /// </summary>
        /// <param name="CustCode"></param>
        /// <returns></returns>
        public CheckCustCodeLoc CheckCustCodeLoc(string dcCode, string locCode)
        {
            var result = _db.F1912s.AsNoTracking().Where(x => x.DC_CODE == dcCode
                                                         && x.LOC_CODE == locCode)
                                                  .Select(x => new CheckCustCodeLoc
                                                  {
                                                      CUST_CODE = x.CUST_CODE,
                                                      NOW_CUST_CODE = x.NOW_CUST_CODE
                                                  });

            return result.FirstOrDefault();
        }


        /// <summary>
        /// 檢查儲位溫層
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="locCode"></param>
        /// <returns></returns>
        public GetTmprData GetF1912Tmpr(string dcCode, string locCode)
        {
            var f1912s = _db.F1912s.AsNoTracking().Where(x => x.DC_CODE == dcCode
                                                         && x.LOC_CODE == locCode);

            var f1980s = _db.F1980s.AsNoTracking();

            var vwF000904Langs = _db.VW_F000904_LANGs.AsNoTracking().Where(x => x.TOPIC == "F1980"
                                                                            && x.SUBTOPIC == "TMPR_TYPE"
                                                                            && x.LANG == Current.Lang);

            var result = from A in f1912s
                         join B in f1980s on new { A.DC_CODE, A.WAREHOUSE_ID } equals new { B.DC_CODE, B.WAREHOUSE_ID }
                         join C in vwF000904Langs on B.TMPR_TYPE equals C.VALUE
                         select new GetTmprData
                         {
                             TmprType = B.TMPR_TYPE,
                             TmprTypeName = C.NAME
                         };

            return result.SingleOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="locCodes"></param>
        /// <returns></returns>
        public IQueryable<F1912> GetF1912Data(string dcCode, List<string> locCodes)
        {
            return _db.F1912s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
            locCodes.Contains(x.LOC_CODE));
        }

        public IQueryable<string> GetLocCodeByWarehouse(string dcCode, List<string> locTypeId)
        {
            return _db.F1912s.AsNoTracking().Where(x => x.DC_CODE == dcCode && locTypeId.Any(z => x.WAREHOUSE_ID.StartsWith(z))).Select(x => x.LOC_CODE);
        }

        public IQueryable<F1912> GetDatasByLocCodesNoTracking(string dcCode, List<string> locCodes)
        {
            return _db.F1912s.AsNoTracking().Where(x => x.DC_CODE == dcCode && locCodes.Contains(x.LOC_CODE));
        }

        public IQueryable<F1912> GetDatasByLocCodesAndWarehouseId(string dcCode, string warehouseId, List<string> locCodes)
        {
            return _db.F1912s.AsNoTracking().Where(x => x.DC_CODE == dcCode && x.WAREHOUSE_ID == warehouseId && locCodes.Contains(x.LOC_CODE));
        }

        public IQueryable<F1912> GetDatasByWarehouseIds(string dcCode, List<string> warehouseIds)
        {
            return _db.F1912s.AsNoTracking().Where(x => x.DC_CODE == dcCode && warehouseIds.Contains(x.WAREHOUSE_ID));
        }

        public string GetMinLocCode(string dcCode, string warehouseId)
        {
            var f1912s = _db.F1912s.AsNoTracking().Where(x => x.DC_CODE == dcCode && x.WAREHOUSE_ID == warehouseId && x.NOW_CUST_CODE == "0");

            if (f1912s.Any())
                return f1912s.Min(z => z.LOC_CODE);
            else
                return null;
        }
    }
}
