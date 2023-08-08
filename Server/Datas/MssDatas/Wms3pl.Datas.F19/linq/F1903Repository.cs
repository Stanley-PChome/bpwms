using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1903Repository : RepositoryBase<F1903, Wms3plDbContext, F1903Repository>
    {
        public F1903Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        /// <summary>
        /// 取得商品溫層
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        public TmprTypeModel GetTmprTypeData(string gupCode, string itemCode)
        {
            var f1903s = _db.F1903s.AsNoTracking().Where(x => x.GUP_CODE == gupCode
                                                           && x.ITEM_CODE == itemCode);
            var vwF000904Langs = _db.VW_F000904_LANGs.AsNoTracking().Where(x => x.TOPIC == "F1903"
                                                                         && x.SUBTOPIC == "TMPR_TYPE"
                                                                         && x.LANG == Current.Lang);

            var result = (from A in f1903s
                          join V in vwF000904Langs on A.TMPR_TYPE equals V.VALUE
                          select new TmprTypeModel
                          {
                              Name = A.ITEM_NAME,
                              TmprType = A.TMPR_TYPE,
                              TmprTypeName = V.NAME
                          }).FirstOrDefault();

            return result;
        }

        /// <summary>
        /// 取得商品名稱
        /// </summary>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="itemCode">品號</param>
        /// <returns></returns>
        public string GetItemName(string gupCode, string custCode, string itemCode)
        {
            var result = _db.F1903s.AsNoTracking().Where(x => x.GUP_CODE == gupCode
                                                         && x.CUST_CODE == custCode
                                                         && x.ITEM_CODE == itemCode)
                                                   .Select(x => x.ITEM_NAME)
                                                   .SingleOrDefault();
            return result;
        }

        // 檢查產品溫層
        public GetTmprData GetF1903Tmpr(string itemCode, string custCode, string gupCode)
        {
            var f1903s = _db.F1903s.AsNoTracking().Where(x => x.ITEM_CODE == itemCode
                                                      && x.CUST_CODE == custCode
                                                      && x.GUP_CODE == gupCode);
            var vwF000904Langs = _db.VW_F000904_LANGs.AsNoTracking().Where(x => x.TOPIC == "F1903"
                                                                             && x.SUBTOPIC == "TMPR_TYPE"
                                                                             && x.LANG == Current.Lang);

            var result = from A in f1903s
                         join B in vwF000904Langs on A.TMPR_TYPE equals B.VALUE
                         select new GetTmprData
                         {
                             TmprType = A.TMPR_TYPE
                         };

            return result.SingleOrDefault();
        }

        #region 原F1902Repository移到F1903Repository

        /// <summary>
		/// 以商品編號取得商品名稱跟大分類名稱
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="itemCode"></param>
		/// <param name="custCode"></param>
		/// /// <returns></returns>
        public IQueryable<F1903WithF1915> GetF1903WithF1915(string gupCode, string itemCode, string custCode)
        {
            var query = _db.F1903s.AsNoTracking().Join(_db.F1915s.AsNoTracking(), B => new { B.LTYPE, B.GUP_CODE, B.CUST_CODE }, C => new { LTYPE = C.ACODE, C.GUP_CODE, C.CUST_CODE }, (B, C) => new { B, C });
            var result = query.Where(x => x.B.GUP_CODE == gupCode && x.B.ITEM_CODE == itemCode && x.C.CUST_CODE == custCode).Select(x => new F1903WithF1915
            {
                GUP_CODE = x.B.GUP_CODE,
                ITEM_CODE = x.B.ITEM_CODE,
                ITEM_NAME = x.B.ITEM_NAME,
                CLA_NAME = x.C.CLA_NAME
            }).Take(1);
            return result;
        }

        public IQueryable<string> GetSameItems(string gupCode, string itemCode, string custCode, List<string> notInItemCodes)
        {
            var eanCodeList = _db.F1903s.AsNoTracking().Where(x => x.GUP_CODE == gupCode
                                                           && x.ITEM_CODE == itemCode
                                                           && x.CUST_CODE == custCode
                                                           ).Select(x => x.EAN_CODE1);

            var result = _db.F1903s.AsNoTracking().Where(x => x.GUP_CODE == gupCode
                                                         && x.ITEM_CODE != itemCode
                                                         && x.CUST_CODE == custCode
                                                         && eanCodeList.Contains(x.EAN_CODE1)
                                                         && !notInItemCodes.Contains(x.ITEM_CODE))
                                                  .Select(x => x.ITEM_CODE);

            return result;
        }

        /// <summary>
		/// 取得是否為紙箱品項
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <returns></returns>
        public IQueryable<F1903> GetF1903sByCarton(string gupCode, string custCode, string isCarton)
        {
            var result = _db.F1903s.Where(x => x.ISCARTON == isCarton
                                           && x.GUP_CODE == gupCode
                                           && x.CUST_CODE == custCode).OrderBy(x => x.ITEM_CODE);
            return result;
        }

        /// <summary>
        /// 以品項編號或條碼來取得品項
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="itemCodeOrEanCode"></param>
        /// <returns></returns>
        public F1903 GetF1903ByItemCodeOrEanCode(string gupCode, string itemCodeOrEanCode, string custCode)
        {
            var result = _db.F1903s.Where(x => x.GUP_CODE == gupCode
                                           && x.CUST_CODE == custCode
                                           && (x.ITEM_CODE == itemCodeOrEanCode || x.EAN_CODE1 == itemCodeOrEanCode || x.EAN_CODE2 == itemCodeOrEanCode || x.EAN_CODE3 == itemCodeOrEanCode));
            return result.FirstOrDefault();
        }

        /// <summary>
        /// 取得符合商品編號或國際條碼的商品編號
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="itemCodeOrEanCodes"></param>
        /// <returns></returns>
        public IQueryable<ItemCodePair> GetByItemCodeOrEanCode(string gupCode, IEnumerable<string> itemCodeOrEanCodes)
        {
            itemCodeOrEanCodes = itemCodeOrEanCodes.Distinct().ToList();
            var query = _db.F1903s.AsNoTracking()
                .Where(x => x.GUP_CODE == gupCode)
                .Where(x =>
                    itemCodeOrEanCodes.Contains(x.ITEM_CODE) ||
                    itemCodeOrEanCodes.Contains(x.EAN_CODE1) ||
                    itemCodeOrEanCodes.Contains(x.EAN_CODE2) ||
                    itemCodeOrEanCodes.Contains(x.EAN_CODE3)
                );

            var result = query.Select(x => new ItemCodePair
            {
                ITEM_CODE = x.ITEM_CODE,
                EAN_CODE1 = x.EAN_CODE1,
                EAN_CODE2 = x.EAN_CODE2,
                EAN_CODE3 = x.EAN_CODE3
            });
            return result;
        }

        /// <summary>
        /// 取得紙箱商品
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="searchCode">商品編號,紙箱條碼</param>
        /// <returns></returns>
        public IQueryable<F1903> GetCartonItem(string gupCode, string custCode, string searchCode)
        {
            var result = _db.F1903s.Where(x => x.GUP_CODE == gupCode
                                          && x.CUST_CODE == custCode
                                          && x.ISCARTON == "1"
                                          && (x.ITEM_CODE == searchCode
                                          || x.EAN_CODE1 == searchCode
                                          || x.EAN_CODE2 == searchCode
                                          || x.EAN_CODE3 == searchCode));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="itemName">起始字串為模糊查詢</param>
        /// <returns></returns>
        public IQueryable<F1903> GetF1912s(string gupCode, string custCode, string itemCode, string itemName)
        {
            var result = _db.F1903s.Where(x => x.GUP_CODE == gupCode
                                            && x.ITEM_CODE == (string.IsNullOrEmpty(itemCode)?x.ITEM_CODE:itemCode)
                                            && x.SND_TYPE != "9"
                                            && x.CUST_CODE == custCode
                                            && x.ITEM_NAME.StartsWith(itemName));
            return result;
        }

        public IQueryable<F1903> GetF1903sByItemName(string gupCode, string custCode, string itemName, int itemSearchMaximum)
        {
            var result = _db.F1903s.Where(x => x.SND_TYPE == "0"
                                          && x.GUP_CODE == gupCode
                                          && x.CUST_CODE == custCode
                                          && x.ITEM_NAME.Contains(itemName)).Take(itemSearchMaximum);

            return result;
        }

        public IQueryable<F1903> GetF1903sByItemCode(string gupCode, string itemCode,string custCode, string account)
        {
           
            var result = _db.F1903s.Where(x => x.ITEM_CODE == itemCode);


            if(!string.IsNullOrEmpty(gupCode) && !string.IsNullOrEmpty(custCode))
            {
                result = result.Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode);
            }
            else //業主全部要去篩選只有此物流中心業主
            {
                result = result.Where(x => _db.F192402s.Any(y => y.EMP_ID == account 
                                                          && y.GUP_CODE == x.GUP_CODE 
                                                          && y.CUST_CODE == x.CUST_CODE));
            }
            return result;
        }

        public IQueryable<F1903> GetDatasByEanCodes(string gupCode, string custCode, List<string> eanCodes)
        {
            var result = _db.F1903s.Where(x => x.GUP_CODE == gupCode 
                                            && x.CUST_CODE == custCode
                                            && eanCodes.Contains(x.EAN_CODE1));
            return result;
        }

        public bool IsExits(string gupCode, string custCode, string lType = "", string mType = "", string sType = "")
        {
            var query = _db.F1903s
                            .Where(x => x.GUP_CODE == gupCode
                                      && x.CUST_CODE == custCode);
            if (!string.IsNullOrEmpty(lType))
                query = query.Where(x => x.LTYPE == lType);
            if (!string.IsNullOrEmpty(mType))
                query = query.Where(x => x.MTYPE == mType);
            if (!string.IsNullOrEmpty(sType))
                query = query.Where(x => x.STYPE == sType);

            return query.Any();
        }

        public IQueryable<F1903> GetF1912s1(string gupCode, string custCode, string[] itemCodes, string itemName, string itemSpec, string lType)
        {


            var result = _db.F1903s.Where(x => x.GUP_CODE == gupCode
                                          && x.SND_TYPE != "9"
                                          && x.CUST_CODE == custCode
                                          && x.ITEM_NAME.StartsWith(string.IsNullOrEmpty(itemName) ? x.ITEM_NAME : itemName)
                                          && (x.ITEM_SPEC == itemSpec || string.IsNullOrEmpty(itemSpec))
                                          && (x.LTYPE == lType || string.IsNullOrEmpty(lType)));

            if (itemCodes.Any())
            {
                result = result.Where(x => itemCodes.Contains(x.ITEM_CODE));
            }

            return result;
        }
        #endregion

        #region 取得商品品號
        public F1903 GetItemCode(string custCode, string itemCode)
        {
            return _db.F1903s.Where(x => x.CUST_CODE == custCode &&
                                   (x.ITEM_CODE == itemCode ||
                                    x.EAN_CODE1 == itemCode ||
                                    x.EAN_CODE2 == itemCode ||
                                    x.EAN_CODE3 == itemCode)).FirstOrDefault();
        }
        #endregion

        public IQueryable<F1903> GetF1903sBySerialNo(string gupCode, string custCode, string serialNo)
        {
            var f2501 = _db.F2501s.Where(x => x.SERIAL_NO == serialNo);
            var f1903s = _db.F1903s;
            var result = from A in f2501
                         join B in f1903s
                         on new { A.GUP_CODE, A.CUST_CODE, A.ITEM_CODE } equals new { B.GUP_CODE, B.CUST_CODE, B.ITEM_CODE }
                         select B;

            return result;
        }

		public IQueryable<F1903> GetDatasByItemCodesOrVnrCodes(string gupCode, string custCode, List<string> itemCodes, List<string> vnrCodes)
		{
			var f1903s = _db.F1903s.AsNoTracking().Where(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode);

			if (itemCodes != null && itemCodes.Any())
				f1903s = f1903s.Where(x => itemCodes.Contains(x.ITEM_CODE));

			if (vnrCodes != null && vnrCodes.Any())
				f1903s = f1903s.Where(x => vnrCodes.Contains(x.VNR_CODE));

			return f1903s;
		}
		public IQueryable<F1903> GetItemByCondition(string gupCode, string custCode, string condition)
		{
			return _db.F1903s.AsNoTracking().Where(x => x.CUST_CODE == custCode &&
																		x.GUP_CODE == gupCode &&
																	 (x.ITEM_CODE == condition ||
																		x.EAN_CODE1 == condition ||
																		x.EAN_CODE2 == condition ||
																		x.EAN_CODE3 == condition));
		}

   //     public IQueryable<F1903> GetDatasByWcsItemAsync(string gupCode, string custCode,int maxRecord)
   //     {
   //         var result = _db.F1903s.Where(x => 
   //         x.GUP_CODE == gupCode && 
   //         x.CUST_CODE == custCode &&
   //         (string.IsNullOrWhiteSpace(x.IS_ASYNC) || x.IS_ASYNC == "N" || x.IS_ASYNC == "F") && x.ISCARTON !="1").Take(maxRecord);

			//return result;
   //     }
    }
}
