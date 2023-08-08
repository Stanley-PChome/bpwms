using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F190301Repository : RepositoryBase<F190301, Wms3plDbContext, F190301Repository>
    {
        public F190301Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F190301Data> GetItemPack(string gupCode, string custCode, string itemCode, string itemName)
        {
            var query = from a in _db.F190301s
                        join b in _db.F1903s on new { a.GUP_CODE, a.ITEM_CODE, a.CUST_CODE } equals new { b.GUP_CODE, b.ITEM_CODE, b.CUST_CODE } into b1
                        from b2 in b1.DefaultIfEmpty()
                        where a.GUP_CODE == gupCode && a.CUST_CODE == custCode
                        select new { a, b2 };
            if (!string.IsNullOrEmpty(itemCode))
                query = query.Where(x => x.a.ITEM_CODE.StartsWith(itemCode));
            if (!string.IsNullOrEmpty(itemName))
                query = query.Where(x => x.b2.ITEM_NAME.StartsWith(itemName));
            return query.Select(x => new F190301Data
            {
                ITEM_CODE = x.a.ITEM_CODE,
                UNIT_LEVEL = x.a.UNIT_LEVEL,
                UNIT_ID = x.a.UNIT_ID,
                UNIT_QTY = x.a.UNIT_QTY,
                LENGTH = x.a.LENGTH,
                WIDTH = x.a.WIDTH,
                HIGHT = x.a.HIGHT,
                WEIGHT = x.a.WEIGHT,
                GUP_CODE = x.a.GUP_CODE,
                CRT_STAFF = x.a.CRT_STAFF,
                CRT_DATE = x.a.CRT_DATE,
                CRT_NAME = x.a.CRT_NAME,
                UPD_STAFF = x.a.UPD_STAFF,
                UPD_DATE = x.a.UPD_DATE,
                UPD_NAME = x.a.UPD_NAME,
                ITEM_NAME = x.b2.ITEM_NAME,
                CUST_CODE = x.b2.CUST_CODE,
                SYS_UNIT = x.a.SYS_UNIT
            });
        }

        /// <summary>
        /// 配庫需要取得 UnitQty 用
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="ACC_UNIT_NAMEList"></param>
        /// <param name="itemCodeList"></param>
        /// <returns></returns>
        public IQueryable<F190301WithF91000302> GetUnitQtyDatas(string gupCode, List<string> accUnitNameList, List<string> itemCodeList)
        {
            var query = _db.F190301s.Join(_db.F91000302s, A => new { UNIT_ID = A.UNIT_ID }, B => new { UNIT_ID = B.ACC_UNIT }, (A, B) => new { A, B })
                    .Where(x => x.A.GUP_CODE == gupCode)
                    .Where(x => x.B.ITEM_TYPE_ID == "001");
            query = query.Where(x => accUnitNameList.Contains(x.B.ACC_UNIT_NAME));
            query = query.Where(x => itemCodeList.Contains(x.A.ITEM_CODE));
            return query.Select(x => new F190301WithF91000302
            {
                GUP_CODE = x.A.GUP_CODE,
                ITEM_CODE = x.A.ITEM_CODE,
                UNIT_ID = x.A.UNIT_ID,
                UNIT_QTY = x.A.UNIT_QTY,
                ACC_UNIT_NAME = x.B.ACC_UNIT_NAME
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="itemCodes"></param>
        /// <param name="unitIds"></param>
        /// <returns></returns>
        public IQueryable<F190301> GetDatas(string gupCode, string custCode, List<string> itemCodes, List<string> unitIds)
        {
            return _db.F190301s.Where(x => x.GUP_CODE == gupCode
                                         && x.CUST_CODE == custCode
                                         &&
                                           itemCodes.Contains(x.ITEM_CODE) &&
                                           unitIds.Contains(x.UNIT_ID));
        }

        public IQueryable<F190301> GetDatasByItemCodes(string gupCode, string custCode, List<string> itemCodes)
        {
            return _db.F190301s.Where(x => x.GUP_CODE == gupCode &&
                                           x.CUST_CODE == custCode &&
                                           itemCodes.Contains(x.ITEM_CODE));
        }

        public IQueryable<F190301> GetDatasByF190301s(List<F190301> f190301s)
        {
            return _db.F190301s.Where(x => f190301s.Any(z => z.GUP_CODE == x.GUP_CODE &&
                                                             z.CUST_CODE == x.CUST_CODE &&
                                                             z.ITEM_CODE == x.ITEM_CODE));
        }
    }
}
