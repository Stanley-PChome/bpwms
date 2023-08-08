using System.Linq;
using Wms3pl.Datas.F14;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.WebServices.DataCommon;


namespace Wms3pl.WebServices.PdaWebApi.Business.Services
{
    public class P810107CustomService : P810107Service
    {
        public P810107CustomService(WmsTransaction wmsTransation)
        {
            _wmsTransation = wmsTransation;
        }

        /// <summary>
        /// 盤點作業-盤點明細查詢
        /// </summary>
        /// <param name="getDetailInvReq"></param>
        /// <returns></returns>
        public override ApiResult GetDetailInv(GetDetailInvReq getDetailInvReq, string gupCode)
        {
            var p81Service = new P81Service();
            var f140101Repo = new F140101Repository(Schemas.CoreSchema);

            // 帳號檢核
            var checkAcc = p81Service.CheckAcc(getDetailInvReq.AccNo).Count();

            // 檢核人員功能權限
            var checkAccFunction = p81Service.CheckAccFunction(getDetailInvReq.FuncNo, getDetailInvReq.AccNo);

            // 檢核人員貨主權限
            var checkAccCustCode = p81Service.CheckAccCustCode(getDetailInvReq.CustNo, getDetailInvReq.AccNo);

            // 檢核人員物流中心權限
            var checkAccDc = p81Service.CheckAccDc(getDetailInvReq.DcNo, getDetailInvReq.AccNo);

            // 必填檢查
            if (string.IsNullOrWhiteSpace(getDetailInvReq.FuncNo) ||
                string.IsNullOrWhiteSpace(getDetailInvReq.AccNo) ||
                string.IsNullOrWhiteSpace(getDetailInvReq.DcNo) ||
                 string.IsNullOrWhiteSpace(getDetailInvReq.CustNo) ||
                 string.IsNullOrWhiteSpace(getDetailInvReq.InvNo) ||
                 checkAcc == 0 ||
                 checkAccFunction == 0 ||
                 checkAccCustCode == 0 ||
                 checkAccDc == 0)
                return new ApiResult { IsSuccessed = false, MsgCode = "20069", MsgContent = p81Service.GetMsg("20069") };

            // 取得盤點明細
            var getInventoryList = f140101Repo.GetInventoryDetailAllColList(getDetailInvReq.DcNo, getDetailInvReq.CustNo, gupCode, getDetailInvReq.InvNo).ToList();

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
                    WhName = p81Service.GetWhName(getDetailInvReq.DcNo, x.WhNo),
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
                }).ToList()
            };
        }
    }
}
