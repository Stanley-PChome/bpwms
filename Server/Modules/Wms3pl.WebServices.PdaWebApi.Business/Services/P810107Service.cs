using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F25;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
    public class P810107Service
    {
        protected WmsTransaction _wmsTransation;
        public P810107Service(WmsTransaction wmsTransation = null)
        {
            _wmsTransation = wmsTransation;

        }
        public ApiResult GetInv(GetInvReq req, string gupCode)
        {
            var p81Service = new P81Service();
            var f140101Repo = new F140101Repository(Schemas.CoreSchema);

            // 帳號檢核
            var checkAcc = p81Service.CheckAcc(req.AccNo).Count();

            // 檢核人員功能權限
            var checkAccFunction = p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

            // 檢核人員貨主權限
            var checkAccCustCode = p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

            // 檢核人員物流中心權限
            var checkAccDc = p81Service.CheckAccDc(req.DcNo, req.AccNo);

            if (string.IsNullOrWhiteSpace(req.FuncNo) ||
                string.IsNullOrWhiteSpace(req.AccNo) ||
                string.IsNullOrWhiteSpace(req.DcNo) ||
                string.IsNullOrWhiteSpace(req.CustNo) ||
                checkAcc == 0 ||
                checkAccFunction == 0 ||
                checkAccCustCode == 0 ||
                checkAccDc == 0
                )
                return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

            IQueryable<GetInvRes> getInventoryList = f140101Repo.GetInventoryList(req.DcNo, req.CustNo, gupCode, req.InvNo, req.InvDate);

            foreach (var item in getInventoryList)
            {
                item.InvTypeName = p81Service.GetTopicValueName("F140101", "INVENTORY_TYPE", item.InvType);
                item.StatusName = p81Service.GetTopicValueName("F140101", "STATUS", item.Status);
            }

            return new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001"), Data = getInventoryList };
        }

        /// <summary>
        /// 盤點作業-盤點明細查詢
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public virtual ApiResult GetDetailInv(GetDetailInvReq req, string gupCode)
        {
            P81Service p81Service = new P81Service();
            F140101Repository f140101Repo = new F140101Repository(Schemas.CoreSchema);
            GetDetailInvRes getDetailInvRes = new GetDetailInvRes();

            // 帳號檢核
            var checkAcc = p81Service.CheckAcc(req.AccNo).Count();

            // 檢核人員功能權限
            var checkAccFunction = p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

            // 檢核人員貨主權限
            var checkAccCustCode = p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

            // 檢核人員物流中心權限
            var checkAccDc = p81Service.CheckAccDc(req.DcNo, req.AccNo);

            // 必填檢查
            if (string.IsNullOrWhiteSpace(req.FuncNo) ||
                string.IsNullOrWhiteSpace(req.AccNo) ||
                string.IsNullOrWhiteSpace(req.DcNo) ||
                 string.IsNullOrWhiteSpace(req.CustNo) ||
                 string.IsNullOrWhiteSpace(req.InvNo) ||
                 checkAcc == 0 ||
                 checkAccFunction == 0 ||
                 checkAccCustCode == 0 ||
                 checkAccDc == 0)
                return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

            // 取得盤點明細
            IQueryable<GetDetailInvResData> getInventoryList = f140101Repo.GetInventoryDetailList(req.DcNo, req.CustNo, gupCode, req.InvNo);

            // 取得調撥路線編號
            var routeDataList = p81Service.GetRouteList(getInventoryList.Select(x => new GetRouteListReq
            {
                No = x.InvNo,
                Seq = x.InvSeq,
                LocCode = x.Loc
            }).ToList());

            return new ApiResult
            {
                IsSuccessed = true,
                MsgCode = "10001",
                MsgContent = p81Service.GetMsg("10001"),
                Data = getInventoryList.Select(x => new GetDetailInvRes
                {
                    InvNo = x.InvNo,
                    ItemNo = x.ItemNo,
                    WhNo = x.WhNo,
                    WhName = p81Service.GetWhName(req.DcNo, x.WhNo),
                    Loc = x.Loc,
                    ValidDate = x.ValidDate,
                    EnterDate = x.EnterDate,
                    StockQty = x.StockQty,
                    Router = routeDataList.Where(z => x.InvNo == z.No && x.InvSeq == z.Seq && x.Loc == z.LocCode).SingleOrDefault().Route,
                    Zone = x.Zone,
                    Aisle = x.Aisle,
                    Sn = x.Sn,
                    MkNo = x.MkNo,
                    ActQty = x.ActQty,
                    PalletNo = x.PalletNo,
                    BoxNo = x.BoxNo,
                    SnType = x.SnType
                }).ToList()
            };
        }

    /// <summary>
    /// 盤點作業-盤點確認
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    public ApiResult PostInvConfirm(PostInvConfirmReq req, string gupCode)
    {
      P81Service p81Service = new P81Service();
      F140101Repository f140101Repo = new F140101Repository(Schemas.CoreSchema, _wmsTransation);
      F1903Repository f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransation);
      F1912Repository f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransation);
      F140104Repository f140104Repo = new F140104Repository(Schemas.CoreSchema, _wmsTransation);
      F140105Repository f140105Repo = new F140105Repository(Schemas.CoreSchema, _wmsTransation);
      F140113Repository f140113Repo = new F140113Repository(Schemas.CoreSchema, _wmsTransation);
      F0070Repository f0070Repo = new F0070Repository(Schemas.CoreSchema, _wmsTransation);
      DateTime dateTime;

      ApiResult apiResult = new ApiResult { IsSuccessed = true, MsgCode = "10001", MsgContent = p81Service.GetMsg("10001") };

      // 帳號檢核
      var checkAcc = p81Service.CheckAcc(req.AccNo).Count();

      // 檢核人員功能權限
      var checkAccFunction = p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

      // 檢核人員貨主權限
      var checkAccCustCode = p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

      // 檢核人員物流中心權限
      var checkAccDc = p81Service.CheckAccDc(req.DcNo, req.AccNo);

      // 必填檢查
      if (string.IsNullOrWhiteSpace(req.FuncNo) ||
          string.IsNullOrWhiteSpace(req.AccNo) ||
          string.IsNullOrWhiteSpace(req.DcNo) ||
           string.IsNullOrWhiteSpace(req.CustNo) ||
           string.IsNullOrWhiteSpace(req.InvNo) ||
           string.IsNullOrWhiteSpace(req.ItemNo) ||
           string.IsNullOrWhiteSpace(req.Loc) ||
           string.IsNullOrWhiteSpace(req.ValidDate) ||
           string.IsNullOrWhiteSpace(req.EnterDate) ||
           req.ActQty < 0 ||
           checkAcc == 0 ||
           checkAccFunction == 0 ||
           checkAccCustCode == 0 ||
           checkAccDc == 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

      // 檢查盤點單號是否存在於F140101
      var f140101 = f140101Repo.AsForUpdate().Find(x => x.DC_CODE == req.DcNo && x.CUST_CODE == req.CustNo && x.GUP_CODE == gupCode && x.INVENTORY_NO == req.InvNo);
      if (f140101 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "20654", MsgContent = p81Service.GetMsg("20654") };

      // 檢查品號是否存在於F1903
      if (f1903Repo.Find(x => x.GUP_CODE == gupCode && x.CUST_CODE == req.CustNo && x.ITEM_CODE == req.ItemNo) == null) //未確定文件是否錯誤
        return new ApiResult { IsSuccessed = false, MsgCode = "20655", MsgContent = p81Service.GetMsg("20655") };

      // 查儲位是否存在於F1912
      var f1912 = f1912Repo.Find(x => x.DC_CODE == req.DcNo && x.LOC_CODE == req.Loc);
      if (f1912 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "20656", MsgContent = p81Service.GetMsg("20656") };

      var validDate = (DateTime.TryParse(req.ValidDate, out dateTime) ? Convert.ToDateTime(req.ValidDate) : default(DateTime));
      var enterDate = (DateTime.TryParse(req.EnterDate, out dateTime) ? Convert.ToDateTime(req.EnterDate) : default(DateTime));
      var boxNo = (string.IsNullOrWhiteSpace(req.BoxNo) ? "0" : req.BoxNo);
      var palletNo = (string.IsNullOrWhiteSpace(req.PalletNo) ? "0" : req.PalletNo);
      var mkNo = (string.IsNullOrWhiteSpace(req.MkNo) ? "0" : req.MkNo);

      // 檢查資料是否存在於F140104或F140105
      var f140104 = f140104Repo.AsForUpdate().Find(x =>
                      x.DC_CODE == req.DcNo
                      && x.GUP_CODE == gupCode
                      && x.CUST_CODE == req.CustNo
                      && x.INVENTORY_NO == req.InvNo
                      && x.LOC_CODE == req.Loc
                      && x.ITEM_CODE == req.ItemNo
                      && x.VALID_DATE == validDate
                      && x.ENTER_DATE == enterDate
                      && x.BOX_CTRL_NO == boxNo
                      && x.PALLET_CTRL_NO == palletNo
                      && x.MAKE_NO == mkNo);

      var f140105 = f140105Repo.AsForUpdate().Find(x =>
                      x.DC_CODE == req.DcNo
                      && x.GUP_CODE == gupCode
                      && x.CUST_CODE == req.CustNo
                      && x.INVENTORY_NO == req.InvNo
                      && x.LOC_CODE == req.Loc
                      && x.ITEM_CODE == req.ItemNo
                      && x.VALID_DATE == validDate
                      && x.ENTER_DATE == enterDate
                      && x.BOX_CTRL_NO == boxNo
                      && x.PALLET_CTRL_NO == palletNo
                      && x.MAKE_NO == mkNo);

      if (f140104 == null && f140105 == null)
        return new ApiResult { IsSuccessed = false, MsgCode = "20657", MsgContent = p81Service.GetMsg("20657") };

      // 檢查傳入的盤點數量是否小於0
      if (req.ActQty < 0)
        return new ApiResult { IsSuccessed = false, MsgCode = "20653", MsgContent = p81Service.GetMsg("20653") };

      if (f140101.ISSECOND == "0")
      {
        // 新增資料表:F140113
        f140113Repo.Add(new F140113
        {
          INVENTORY_NO = req.InvNo,
          ITEM_CODE = req.ItemNo,
          ITEM_NAME = p81Service.GetItemName(gupCode, req.CustNo, req.ItemNo),
          LOC_CODE = req.Loc,
          ISSECOND = "0",
          INVENTORY_QTY = req.ActQty,
          DC_CODE = req.DcNo,
          GUP_CODE = gupCode,
          CUST_CODE = req.CustNo,
          STATUS = "1",
          VALID_DATE = validDate,
          MAKE_NO = req.MkNo
        });

        // 更新資料表:F140104

        f140104.FIRST_QTY = req.ActQty;
        f140104.FST_INVENTORY_STAFF = Current.Staff;
        f140104.FST_INVENTORY_NAME = Current.StaffName;
        f140104.FST_INVENTORY_DATE = DateTime.Now;
        f140104.FST_INVENTORY_PC = f0070Repo
          .GetDatasByTrueAndCondition(x => x.USERNAME == Current.Staff && x.GROUPNAME == "Pda")
          .OrderBy(x => x.CRT_DATE)
          .Select(x => x.CONNECTID)
          .LastOrDefault();

        f140104Repo.Update(f140104);
      }
      else if (f140101.ISSECOND == "1")
      {
        f140113Repo.Add(new F140113
        {
          INVENTORY_NO = req.InvNo,
          ITEM_CODE = req.ItemNo,
          ITEM_NAME = p81Service.GetItemName(gupCode, req.CustNo, req.ItemNo),
          LOC_CODE = req.Loc,
          CUST_CODE = req.CustNo,
          ISSECOND = "1",
          INVENTORY_QTY = req.ActQty,
          DC_CODE = req.DcNo,
          GUP_CODE = gupCode,
          STATUS = "1",
          VALID_DATE = validDate,
          MAKE_NO = req.MkNo
        });

        // 更新資料表:F140105
        f140105.SECOND_QTY = req.ActQty;
        //f140105.FST_INVENTORY_STAFF = f140104.FST_INVENTORY_STAFF;
        //f140105.FST_INVENTORY_NAME = f140104.FST_INVENTORY_NAME;
        //f140105.FST_INVENTORY_DATE = f140104.FST_INVENTORY_DATE;
        //f140105.FST_INVENTORY_PC = f140104.FST_INVENTORY_PC;

        f140105.SEC_INVENTORY_STAFF = Current.Staff;
        f140105.SEC_INVENTORY_NAME = Current.StaffName;
        f140105.SEC_INVENTORY_DATE = DateTime.Now;
        f140105.SEC_INVENTORY_PC = f0070Repo
          .GetDatasByTrueAndCondition(x => x.USERNAME == Current.Staff && x.GROUPNAME == "Pda")
          .OrderBy(x => x.CRT_DATE)
          .Select(x => x.CONNECTID)
          .LastOrDefault();

        f140105Repo.Update(f140105);
      }

      if (f140101.STATUS == "0")
      {
        f140101.STATUS = "1";
        f140101.UPD_DATE = DateTime.Now;
        f140101.UPD_STAFF = Current.Staff;
        f140101.UPD_NAME = Current.StaffName;
        f140101Repo.Update(f140101);
      }

      _wmsTransation.Complete();

      return new ApiResult { IsSuccessed = true, MsgCode = "20601", MsgContent = p81Service.GetMsg("20601") };
    }

        /// <summary>
        /// 盤點作業-新增盤點商品
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ApiResult PostInvNewItem(PostInvNewItemReq req, string gupCode)
        {
            var p81Service = new P81Service();
            var f140101Repo = new F140101Repository(Schemas.CoreSchema, _wmsTransation);
            var f1903Repo = new F1903Repository(Schemas.CoreSchema, _wmsTransation);
            var f1912Repo = new F1912Repository(Schemas.CoreSchema, _wmsTransation);
            var f1980Repo = new F1980Repository(Schemas.CoreSchema);
            var f140104Repo = new F140104Repository(Schemas.CoreSchema, _wmsTransation);
            var f140105Repo = new F140105Repository(Schemas.CoreSchema, _wmsTransation);
            var f140113Repo = new F140113Repository(Schemas.CoreSchema, _wmsTransation);
            var f0070Repo = new F0070Repository(Schemas.CoreSchema, _wmsTransation);
            var f2501Repo = new F2501Repository(Schemas.CoreSchema, _wmsTransation);
            DateTime dateTime;

            // 帳號檢核
            var checkAcc = p81Service.CheckAcc(req.AccNo).Count();

            // 檢核人員功能權限
            var checkAccFunction = p81Service.CheckAccFunction(req.FuncNo, req.AccNo);

            // 檢核人員貨主權限
            var checkAccCustCode = p81Service.CheckAccCustCode(req.CustNo, req.AccNo);

            // 檢核人員物流中心權限
            var checkAccDc = p81Service.CheckAccDc(req.DcNo, req.AccNo);

            // 必填檢查
            if (string.IsNullOrWhiteSpace(req.FuncNo) ||
                string.IsNullOrWhiteSpace(req.AccNo) ||
                string.IsNullOrWhiteSpace(req.DcNo) ||
                 string.IsNullOrWhiteSpace(req.CustNo) ||
                 string.IsNullOrWhiteSpace(req.InvNo) ||
                 string.IsNullOrWhiteSpace(req.ItemNo) ||
                 string.IsNullOrWhiteSpace(req.Loc) ||
                 string.IsNullOrWhiteSpace(req.ValidDate) ||
                 string.IsNullOrWhiteSpace(req.EnterDate) ||
                 req.ActQty < 0 ||
                 checkAcc == 0 ||
                 checkAccFunction == 0 ||
                 checkAccCustCode == 0 ||
                 checkAccDc == 0)
                return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

            // 檢查盤點單號是否存在於F140101
            var f140101 = f140101Repo.Find(x => x.DC_CODE == req.DcNo && x.CUST_CODE == req.CustNo && x.GUP_CODE == gupCode && x.INVENTORY_NO == req.InvNo); //未確定文件是否有錯誤
            if (f140101 == null)
                return new ApiResult { IsSuccessed = false, MsgCode = "20654", MsgContent = p81Service.GetMsg("20654") };

            // 取得商品品號
            var f1903 = p81Service.GetItemCode(req.CustNo, req.ItemNo);

            // 檢查品號是否存在於F1903
            if (f1903 == null)
                return new ApiResult { IsSuccessed = false, MsgCode = "20655", MsgContent = p81Service.GetMsg("20655") };

            // 查儲位是否存在於F1912
            var f1912 = f1912Repo.Find(x => x.DC_CODE == req.DcNo && x.LOC_CODE == req.Loc);
            if (f1912 == null)
                return new ApiResult { IsSuccessed = false, MsgCode = "20656", MsgContent = p81Service.GetMsg("20656") };

            // 查詢該儲位的倉別
            var f1980 = f1980Repo.Find(x => x.DC_CODE == req.DcNo && x.WAREHOUSE_ID == f1912.WAREHOUSE_ID);
            if (f1980 == null)
                return new ApiResult { IsSuccessed = false, MsgCode = "20659", MsgContent = p81Service.GetMsg("20659") };

            // 該儲位的倉別是否為自動倉
            if (f1980.DEVICE_TYPE != "0")
                return new ApiResult { IsSuccessed = false, MsgCode = "20660", MsgContent = p81Service.GetMsg("20660") };

            var validDate = (DateTime.TryParse(req.ValidDate, out dateTime) ? Convert.ToDateTime(req.ValidDate) : default(DateTime));
            var enterDate = (DateTime.TryParse(req.EnterDate, out dateTime) ? Convert.ToDateTime(req.EnterDate) : default(DateTime));
            var boxNo = (string.IsNullOrWhiteSpace(req.BoxNo) ? "0" : req.BoxNo);
            var palletNo = (string.IsNullOrWhiteSpace(req.PalletNo) ? "0" : req.PalletNo);
            var mkNo = (string.IsNullOrWhiteSpace(req.MkNo) ? "0" : req.MkNo);

            // 檢查資料是否存在於F140104或F140105
            var f140104 = f140104Repo.Find(
                x => x.DC_CODE == req.DcNo
                            && x.GUP_CODE == gupCode
                            && x.CUST_CODE == req.CustNo
                            && x.INVENTORY_NO == req.InvNo
                            && x.LOC_CODE == req.Loc
                            && x.ITEM_CODE == f1903.ITEM_CODE
                            && x.VALID_DATE == validDate
                            && x.ENTER_DATE == enterDate
                            && x.BOX_CTRL_NO == boxNo
                            && x.PALLET_CTRL_NO == palletNo
                            && x.MAKE_NO == mkNo);

            var f140105 = f140105Repo.Find(x => x.DC_CODE == req.DcNo
                            && x.GUP_CODE == gupCode
                            && x.CUST_CODE == req.CustNo
                            && x.INVENTORY_NO == req.InvNo
                            && x.LOC_CODE == req.Loc
                            && x.ITEM_CODE == f1903.ITEM_CODE
                            && x.VALID_DATE == validDate
                            && x.ENTER_DATE == enterDate
                            && x.BOX_CTRL_NO == boxNo
                            && x.PALLET_CTRL_NO == palletNo
                            && x.MAKE_NO == mkNo);
            if (f140104 != null || f140105 != null)
                return new ApiResult { IsSuccessed = false, MsgCode = "20658", MsgContent = p81Service.GetMsg("20658") };

            // 檢查傳入的盤點數量是否小於0
            if (req.ActQty < 0)
                return new ApiResult { IsSuccessed = false, MsgCode = "20653", MsgContent = p81Service.GetMsg("20653") };

            // 序號綁儲位商品，數量不可超過1
            if (f1903.BUNDLE_SERIALLOC == "1" && req.ActQty > 1)
                return new ApiResult { IsSuccessed = false, MsgCode = "20661", MsgContent = p81Service.GetMsg("20661") };

            var f2501 = f2501Repo.Find(x => x.GUP_CODE == gupCode &&
                              x.CUST_CODE == req.CustNo &&
                              x.ITEM_CODE == f1903.ITEM_CODE &&
                              x.SERIAL_NO == req.Sn);

            // 檢查序號綁儲位商品序號是否存在於F2501
            if (f1903.BUNDLE_SERIALLOC == "1" && f2501 == null)
                return new ApiResult { IsSuccessed = false, MsgCode = "21951", MsgContent = p81Service.GetMsg("21951") };

            if (f140101.ISSECOND == "0")
            {
                F140113 f140113 = new F140113
                {
                    INVENTORY_NO = req.InvNo,
                    ITEM_CODE = f1903.ITEM_CODE,
                    ITEM_NAME = p81Service.GetItemName(gupCode, req.CustNo, f1903.ITEM_CODE),
                    LOC_CODE = req.Loc,
                    ISSECOND = "0",
                    INVENTORY_QTY = req.ActQty,
                    DC_CODE = req.DcNo,
                    GUP_CODE = gupCode,
                    CUST_CODE = req.CustNo,
                    STATUS = "1",
                    VALID_DATE = validDate,
                    MAKE_NO = req.MkNo
                };
                f140113Repo.Add(f140113);

                // 新增資料表:F140104
                f140104Repo.Add(new F140104
                {
                    INVENTORY_NO = req.InvNo,
                    WAREHOUSE_ID = f1912.WAREHOUSE_ID, // 當只判斷LOC_CODE可能會抓到其他物流中心的儲位編號
                    LOC_CODE = req.Loc,
                    ITEM_CODE = f1903.ITEM_CODE,
                    VALID_DATE = DateTime.TryParse(req.ValidDate, out dateTime) ? Convert.ToDateTime(req.ValidDate) : default(DateTime),
                    ENTER_DATE = DateTime.TryParse(req.EnterDate, out dateTime) ? Convert.ToDateTime(req.EnterDate) : default(DateTime),
                    QTY = 0,
                    FIRST_QTY = req.ActQty,
                    FLUSHBACK = "0",
                    DC_CODE = req.DcNo,
                    GUP_CODE = gupCode,
                    CUST_CODE = req.CustNo,
                    FST_INVENTORY_STAFF = Current.Staff,
                    FST_INVENTORY_NAME = Current.StaffName,
                    FST_INVENTORY_DATE = DateTime.Now,
                    FST_INVENTORY_PC = f0070Repo.GetDatasByTrueAndCondition(x => x.USERNAME == Current.Staff).OrderBy(x => x.CRT_DATE).Select(x => x.HOSTNAME).LastOrDefault(),
                    BOX_CTRL_NO = boxNo,
                    PALLET_CTRL_NO = palletNo,
                    MAKE_NO = mkNo,
                    DEVICE_STOCK_QTY = 0
                });
            }
            else if (f140101.ISSECOND == "1")
            {
                f140113Repo.Add(new F140113
                {
                    INVENTORY_NO = req.InvNo,
                    ITEM_CODE = f1903.ITEM_CODE,
                    ITEM_NAME = p81Service.GetItemName(gupCode, req.CustNo, f1903.ITEM_CODE),
                    LOC_CODE = req.Loc,
                    CUST_CODE = req.CustNo,
                    ISSECOND = "1",
                    INVENTORY_QTY = req.ActQty,
                    DC_CODE = req.DcNo,
                    GUP_CODE = gupCode,
                    STATUS = "1",
                    VALID_DATE = validDate,
                    MAKE_NO = req.MkNo
                });

                // 新增資料表:F140105
                f140105Repo.Add(new F140105
                {
                    INVENTORY_NO = req.InvNo,
                    WAREHOUSE_ID = f1912.WAREHOUSE_ID, // 當只判斷LOC_CODE可能會抓到其他物流中心的儲位編號
                    LOC_CODE = req.Loc,
                    ITEM_CODE = f1903.ITEM_CODE,
                    VALID_DATE = DateTime.TryParse(req.ValidDate, out dateTime) ? Convert.ToDateTime(req.ValidDate) : default(DateTime),
                    ENTER_DATE = DateTime.TryParse(req.EnterDate, out dateTime) ? Convert.ToDateTime(req.EnterDate) : default(DateTime),
                    QTY = 0,
                    SECOND_QTY = req.ActQty,
                    FLUSHBACK = "0",
                    DC_CODE = req.DcNo,
                    GUP_CODE = gupCode,
                    CUST_CODE = req.CustNo,
                    SEC_INVENTORY_STAFF = Current.Staff,
                    SEC_INVENTORY_NAME = Current.StaffName,
                    SEC_INVENTORY_DATE = DateTime.Now,
                    SEC_INVENTORY_PC = f0070Repo.GetDatasByTrueAndCondition(x => x.USERNAME == Current.Staff).OrderBy(x => x.CRT_DATE).Select(x => x.HOSTNAME).LastOrDefault(),
                    BOX_CTRL_NO = boxNo,
                    PALLET_CTRL_NO = palletNo,
                    MAKE_NO = mkNo,
                    DEVICE_STOCK_QTY = 0
                });
            }

            _wmsTransation.Complete();

            return new ApiResult { IsSuccessed = true, MsgCode = "20601", MsgContent = p81Service.GetMsg("20601") };
        }
    }
}
