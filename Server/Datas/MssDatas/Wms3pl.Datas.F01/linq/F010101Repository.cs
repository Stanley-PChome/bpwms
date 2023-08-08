using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F01
{
    public partial class F010101Repository : RepositoryBase<F010101, Wms3plDbContext, F010101Repository>
    {
        public F010101Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        /// <summary>
        /// 取得採購單資料
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="shopDateS"></param>
        /// <param name="shopDateE"></param>
        /// <param name="shopNo"></param>
        /// <param name="vnrCode"></param>
        /// <param name="vnrName"></param>
        /// <param name="itemCode"></param>
        /// <param name="custOrdNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IQueryable<F010101ShopNoList> GetF010101ShopNoList(string dcCode, string gupCode, string custCode,
            string shopDateS, string shopDateE, string shopNo, string vnrCode, string vnrName, string itemCode, string custOrdNo,
            string status)
        {

            #region 採購單主檔資料
            var f010101Data = _db.F010101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     x.SHOP_DATE >= Convert.ToDateTime(shopDateS) &&
                                                                     x.SHOP_DATE <= Convert.ToDateTime(shopDateE));

            if (!string.IsNullOrWhiteSpace(shopNo))
            {
                f010101Data = f010101Data.Where(x => x.SHOP_NO == shopNo);
            }

            if (!string.IsNullOrWhiteSpace(vnrCode))
            {
                f010101Data = f010101Data.Where(x => x.VNR_CODE == vnrCode);
            }

            if (!string.IsNullOrWhiteSpace(custOrdNo))
            {
                f010101Data = f010101Data.Where(x => x.CUST_ORD_NO == custOrdNo);
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                f010101Data = f010101Data.Where(x => x.STATUS != "9");
            }
            else
            {
                f010101Data = f010101Data.Where(x => x.STATUS == status);
            }
            #endregion

            #region 採購單明細資料
            var f010102Data = _db.F010102s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     f010101Data.Select(z => z.SHOP_NO).Contains(x.SHOP_NO));

            if (!string.IsNullOrWhiteSpace(itemCode))
            {
                f010102Data = f010102Data.Where(x => x.ITEM_CODE == itemCode);
            }
            #endregion

            #region 廠商主檔資料
            var f1908Data = _db.F1908s.AsNoTracking().Where(x => x.GUP_CODE == gupCode && f010101Data.Select(z => z.VNR_CODE).Contains(x.VNR_CODE));
            if (!string.IsNullOrWhiteSpace(vnrName))
            {
                f1908Data = f1908Data.Where(x => x.VNR_NAME.StartsWith(vnrName));
            }
            #endregion

            #region 貨主檔資料
            var f1909Data = _db.F1909s.AsNoTracking().Select(x => new
            {
                GUP_CODE = x.GUP_CODE,
                CUST_CODE = x.ALLOWGUP_VNRSHARE == "1" ? "0" : custCode
            });
            #endregion

            #region 組資料
            var result = (from A in f010101Data
                          join B in f010102Data
                          on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.SHOP_NO } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.SHOP_NO }
                          join D in f1908Data
                          on new { A.GUP_CODE, A.VNR_CODE } equals new { D.GUP_CODE, D.VNR_CODE } into subD
                          from D in subD.DefaultIfEmpty()
                          join C in f1909Data
                          on new { D.CUST_CODE, D.GUP_CODE } equals new { C.CUST_CODE, C.GUP_CODE }
                          select new F010101ShopNoList
                          {
                              DC_CODE = A.DC_CODE,
                              GUP_CODE = A.GUP_CODE,
                              CUST_CODE = A.CUST_CODE,
                              SHOP_DATE = A.SHOP_DATE,
                              SHOP_NO = A.SHOP_NO,
                              VNR_CODE = A.VNR_CODE,
                              VNR_NAME = D.VNR_NAME ?? null,
                              STATUS = A.STATUS,
                              DELIVER_DATE = A.DELIVER_DATE
                          }).Distinct().OrderBy(x => x.SHOP_NO);
            #endregion

            return result;
        }

        /// <summary>
        /// 取得採購單資料
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="shopNo"></param>
        /// <returns></returns>
        public IQueryable<F010101Data> GetF010101Datas(string dcCode, string gupCode, string custCode, string shopNo)
        {
            var result = from A in _db.F010101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                          x.GUP_CODE == gupCode &&
                                                                          x.CUST_CODE == custCode &&
                                                                          x.SHOP_NO == shopNo)
                         join B in _db.F1908s.AsNoTracking().Where(x => x.GUP_CODE == gupCode)
                         on new { A.GUP_CODE, A.VNR_CODE } equals new { B.GUP_CODE, B.VNR_CODE } into subB
                         from B in subB.DefaultIfEmpty()
                         select new F010101Data
                         {
                             DC_CODE = A.DC_CODE,
                             GUP_CODE = A.GUP_CODE,
                             CUST_CODE = A.CUST_CODE,
                             SHOP_DATE = A.SHOP_DATE,
                             SHOP_NO = A.SHOP_NO,
                             DELIVER_DATE = A.DELIVER_DATE,
                             CUST_ORD_NO = A.CUST_ORD_NO,
                             VNR_CODE = A.VNR_CODE,
                             VNR_NAME = B.VNR_NAME ?? null,
                             TEL = B.TEL ?? null,
                             ADDRESS = B.ADDRESS ?? null,
                             BUSPER = B.BOSS ?? null,
                             PAY_TYPE = B.PAY_TYPE ?? null,
                             INVO_TYPE = B.INVO_TYPE ?? null,
                             TAX_TYPE = B.TAX_TYPE ?? null,
                             INVOICE_DATE = A.INVOICE_DATE,
                             CONTACT_TEL = A.CONTACT_TEL,
                             INV_ADDRESS = B.INV_ADDRESS ?? null,
                             SHOP_CAUSE = A.SHOP_CAUSE,
                             MEMO = A.MEMO,
                             STATUS = A.STATUS,
                             CRT_DATE = A.CRT_DATE,
                             CRT_STAFF = A.CRT_STAFF,
                             CRT_NAME = A.CRT_NAME,
                             UPD_DATE = A.UPD_DATE,
                             UPD_STAFF = A.UPD_STAFF,
                             UPD_NAME = A.UPD_NAME,
                             ORD_PROP = A.ORD_PROP
                         };

            return result;
        }

        /// <summary>
        /// 取得採購單報表資料
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="shopNo"></param>
        /// <returns></returns>
        public IQueryable<F010101ReportData> GetF010101Reports(string dcCode, string gupCode, string custCode, string shopNo)
        {

            #region 採購單主檔資料
            var f010101Data = _db.F010101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     x.SHOP_NO == shopNo);
            #endregion

            #region 採購單明細資料
            var f010102Data = _db.F010102s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                     x.GUP_CODE == gupCode &&
                                                                     x.CUST_CODE == custCode &&
                                                                     f010101Data.Select(z => z.SHOP_NO).Contains(x.SHOP_NO));
            #endregion

            #region 參數設定資料(付款方式、稅別、發票樣式)
            var vwF000904LangData = _db.VW_F000904_LANGs.AsNoTracking().Where(x => x.TOPIC == "F1908" &&
                                                                                                x.LANG == Current.Lang);
            // 付款方式
            var payTypeData = vwF000904LangData.Where(x => x.SUBTOPIC == "PAY_TYPE").Select(x => new { x.NAME, x.VALUE });

            // 稅別
            var taxTypeData = vwF000904LangData.Where(x => x.SUBTOPIC == "TAX_TYPE").Select(x => new { x.NAME, x.VALUE });

            // 發票樣式
            var invoTypeData = vwF000904LangData.Where(x => x.SUBTOPIC == "INVO_TYPE").Select(x => new { x.NAME, x.VALUE });
            #endregion

            #region 組資料
            var result = (from A in f010101Data
                          join B in f010102Data
                          on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.SHOP_NO } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.SHOP_NO }
                          join C in _db.F1903s.AsNoTracking().Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode && f010102Data.Select(z => z.ITEM_CODE).Contains(x.ITEM_CODE))
                          on new { B.GUP_CODE, B.CUST_CODE, B.ITEM_CODE } equals new { C.GUP_CODE, C.CUST_CODE, C.ITEM_CODE }
                          join D in _db.F1908s.AsNoTracking().Where(x => x.GUP_CODE == gupCode && f010101Data.Select(z => z.VNR_CODE).Contains(x.VNR_CODE))
                          on new { A.GUP_CODE, A.VNR_CODE } equals new { D.GUP_CODE, D.VNR_CODE } into subD
                          from D in subD.DefaultIfEmpty()
                          join E in _db.F1929s.AsNoTracking().Where(x => x.GUP_CODE == gupCode)
                          on A.GUP_CODE equals E.GUP_CODE into subE
                          from E in subE.DefaultIfEmpty()
                          join F in _db.F1909s.AsNoTracking().Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode)
                          on new { A.GUP_CODE, A.CUST_CODE } equals new { F.GUP_CODE, F.CUST_CODE } into subF
                          from F in subF.DefaultIfEmpty()
                          join G in _db.F1901s.AsNoTracking().Where(x => x.DC_CODE == dcCode)
                          on A.DC_CODE equals G.DC_CODE into subG
                          from G in subG.DefaultIfEmpty()
                          join H in payTypeData
                          on D.PAY_TYPE equals H.VALUE into subH
                          from H in subH.DefaultIfEmpty()
                          join I in taxTypeData
                          on D.TAX_TYPE equals I.VALUE into subI
                          from I in subI.DefaultIfEmpty()
                          join J in invoTypeData
                          on D.INVO_TYPE equals J.VALUE into subJ
                          from J in subJ.DefaultIfEmpty()
                          join K in _db.F1951s.AsNoTracking().Where(x => x.UCT_ID == "PO").Select(x => new { x.UCC_CODE, x.CAUSE })
                          on A.SHOP_CAUSE equals K.UCC_CODE into subK
                          from K in subK.DefaultIfEmpty()
                          select new F010101ReportData
                          {
                              DC_CODE = G.DC_NAME ?? null,
                              GUP_CODE = E.GUP_NAME ?? null,
                              CUST_CODE = F.CUST_NAME ?? null,
                              SHOP_DATE = A.SHOP_DATE,
                              SHOP_NO = A.SHOP_NO,
                              DELIVER_DATE = A.DELIVER_DATE,
                              CUST_ORD_NO = A.CUST_ORD_NO,
                              VNR_CODE = A.VNR_CODE,
                              VNR_NAME = D.VNR_NAME ?? null,
                              TEL = D.TEL ?? null,
                              ADDRESS = D.ADDRESS ?? null,
                              BUSPER = D.BOSS ?? null,
                              INVOICE_DATE = A.INVOICE_DATE,
                              CONTACT_TEL = A.CONTACT_TEL,
                              INV_ADDRESS = D.INV_ADDRESS ?? null,
                              SHOP_CAUSE = K.CAUSE ?? null,
                              MEMO = A.MEMO,
                              PAY_TYPE = H.NAME ?? null,
                              TAX_TYPE = I.NAME ?? null,
                              INVO_TYPE = J.NAME ?? null,
                              ITEM_CODE = B.ITEM_CODE,
                              ITEM_NAME = C.ITEM_NAME,
                              ITEM_SIZE = C.ITEM_SIZE,
                              ITEM_SPEC = C.ITEM_SPEC,
                              ITEM_COLOR = C.ITEM_COLOR,
                              SHOP_QTY = B.SHOP_QTY
                          });

            // RowNum
            var res = result.OrderBy(x => x.ITEM_CODE).ToList();
            for (int i = 0; i < res.Count; i++) { res[i].ROWNUM = i + 1; }

            #endregion

            return res.AsQueryable();
        }
    }
}
