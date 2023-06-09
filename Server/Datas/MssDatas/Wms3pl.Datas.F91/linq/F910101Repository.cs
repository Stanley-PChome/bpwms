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

namespace Wms3pl.Datas.F91
{
    public partial class F910101Repository : RepositoryBase<F910101, Wms3plDbContext, F910101Repository>
    {
        public F910101Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="bomNo"></param>
        /// <param name="itemCode"></param>
        /// <param name="status"></param>
        /// <param name="bomType"></param>
        /// <returns></returns>
        public IQueryable<F910101Ex> GetF910101Datas(string gupCode, string custCode, string bomNo, string itemCode, string status, string bomType)
        {
            var q = from a in _db.F910101s
                    join b in _db.F1903s on new { a.GUP_CODE, a.ITEM_CODE, a.CUST_CODE }
                    equals new { b.GUP_CODE, b.ITEM_CODE, b.CUST_CODE }
                    join c in _db.F91000302s on new { p1 = a.UNIT_ID, p2 = "001" }
                    equals new { p1 = c.ACC_UNIT, p2 = c.ITEM_TYPE_ID } into cc
                    from c in cc.DefaultIfEmpty()
                    where a.GUP_CODE == gupCode
                    && a.BOM_NO == (string.IsNullOrEmpty(bomNo) ? a.BOM_NO : bomNo)
                    && a.ITEM_CODE == (string.IsNullOrEmpty(itemCode) ? a.ITEM_CODE : itemCode)
                    && a.STATUS == status
                    && a.BOM_TYPE == bomType
                    select new F910101Ex
                    {
                        BOM_NO = a.BOM_NO,
                        GUP_CODE = a.GUP_CODE,
                        CUST_CODE = a.CUST_CODE,
                        ITEM_CODE = a.ITEM_CODE,
                        ITEM_NAME = b.ITEM_NAME,
                        STATUS = a.STATUS,
                        BOM_TYPE = a.BOM_TYPE,
                        BOM_NAME = a.BOM_NAME,
                        UNIT_ID = a.UNIT_ID,
                        UNIT = c.ACC_UNIT_NAME,
                        CHECK_PERCENT = Convert.ToDecimal( a.CHECK_PERCENT),
                        SPEC_DESC = a.SPEC_DESC,
                        PACKAGE_DESC = a.PACKAGE_DESC,
                        CRT_STAFF = a.CRT_STAFF,
                        CRT_NAME = a.CRT_NAME,
                        CRT_DATE = a.CRT_DATE,
                        UPD_STAFF = a.UPD_STAFF,
                        UPD_NAME = a.UPD_NAME,
                        UPD_DATE = a.UPD_DATE,
                        ISPROCESS = a.ISPROCESS
                    };
            var result = q.OrderBy(c => c.BOM_NO).ToList();
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
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="bomNo"></param>
        /// <returns></returns>
        public IQueryable<F910101> GetF910101ByBomNoDatas(string gupCode, string custCode, List<string> bomNo)
        {
            var q = from a in _db.F910101s
                    where a.GUP_CODE == gupCode
                    && a.CUST_CODE == custCode
                    && bomNo.Contains(a.BOM_NO)
                    select a;
            return q;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="bomNo"></param>
        /// <returns></returns>
        public IQueryable<F910102Ex> GetF910102Datas(string gupCode, string custCode, string bomNo)
        {
            var q = from  a in _db.F910102s
                    join b in _db.F1903s on new { p0 = a.GUP_CODE, p1 = a.MATERIAL_CODE, p2 = a.CUST_CODE}
                    equals new { p0 = b.GUP_CODE, p1 = b.ITEM_CODE, p2 = b.CUST_CODE}
                    where a.GUP_CODE == gupCode
                    && a.CUST_CODE == custCode
                    && a.BOM_NO == bomNo
                    orderby a.COMBIN_ORDER
                    select new 
                    {
                        a.BOM_NO,
                        a.MATERIAL_CODE,
                        b.ITEM_NAME,
                        a.COMBIN_ORDER,
                        b.ITEM_SIZE,
                        b.ITEM_SPEC,
                        b.ITEM_COLOR,
                        b.BUNDLE_SERIALNO,
                        a.BOM_QTY,
                        a.CRT_STAFF,
                        a.CRT_NAME,
                        a.CRT_DATE,
                        a.UPD_STAFF,
                        a.UPD_NAME,
                        a.UPD_DATE,
                        a.GUP_CODE,
                        a.CUST_CODE
                    };
            var result = q.AsEnumerable().Select((c, i) => new F910102Ex
            {
                ROWNUM = i + 1,
                BOM_NO = c.BOM_NO,
                MATERIAL_CODE = c.MATERIAL_CODE,
                MATERIAL_NAME = c.ITEM_NAME,
                COMBIN_ORDER = c.COMBIN_ORDER,
                ITEM_SIZE = c.ITEM_SIZE,
                ITEM_SPEC = c.ITEM_SPEC,
                ITEM_COLOR = c.ITEM_COLOR,
                BUNDLE_SERIALNO = c.BUNDLE_SERIALNO,
                BOM_QTY = c.BOM_QTY,
                CRT_STAFF = c.CRT_STAFF,
                CRT_NAME = c.CRT_NAME,
                CRT_DATE = c.CRT_DATE,
                UPD_STAFF = c.UPD_STAFF,
                UPD_NAME = c.UPD_NAME,
                UPD_DATE = c.UPD_DATE,
                GUP_CODE = c.GUP_CODE,
                CUST_CODE = c.CUST_CODE

            });

            return result.AsQueryable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public IQueryable<F910101Ex2> GetF910101Ex2(string gupCode, string custCode, string status)
        {
            var q = from a in _db.F910101s
                    join b in _db.F1903s on new { a.GUP_CODE, a.CUST_CODE, a.ITEM_CODE }
                    equals new { b.GUP_CODE, b.CUST_CODE, b.ITEM_CODE } into bb
                    from b in bb.DefaultIfEmpty()
                    join c in _db.F91000302s on new { p0 = a.UNIT_ID, p1 = "001" }
                    equals new { p0 = c.ACC_UNIT, p1 = c.ITEM_TYPE_ID } into cc
                    from c in cc.DefaultIfEmpty()
                    join d in _db.F1909s on new { a.GUP_CODE, a.CUST_CODE }
                    equals new { d.GUP_CODE, d.CUST_CODE } into dd
                    from d in dd.DefaultIfEmpty()
                    join e in _db.F1929s on a.GUP_CODE equals e.GUP_CODE into ee
                    from e in ee.DefaultIfEmpty()
                    where (a.GUP_CODE == gupCode || string.IsNullOrEmpty(a.GUP_CODE))
                    && a.CUST_CODE == custCode
                    && (a.STATUS == status || string.IsNullOrEmpty(a.STATUS))
                    orderby a.BOM_NO
                    select new F910101Ex2
                    {
                        GUP_CODE = a.GUP_CODE,
                        ITEM_CODE = a.ITEM_CODE,
                        BOM_NO = a.BOM_NO,
                        BOM_TYPE = a.BOM_TYPE,
                        BOM_TYPE_NAME = a.BOM_TYPE == "0" ? "組合" : a.BOM_TYPE == "1" ? "拆解" : string.Empty,
                        UNIT_ID = a.UNIT_ID,
                        UNIT = c.ACC_UNIT_NAME,
                        CUST_NAME = d.CUST_NAME,
                        GUP_NAME = e.GUP_NAME,
                        CUST_CODE = a.CUST_CODE,
                        ITEM_NAME = b.ITEM_NAME
                    };
            return q;
        }

        

        

        public IQueryable<F910101> GetDatas(string gupCode, string custCode, List<string> itemCodes)
        {
            return _db.F910101s.AsNoTracking().Where(x => x.GUP_CODE == gupCode &&
            x.CUST_CODE == custCode &&
						itemCodes.Contains(x.ITEM_CODE)  &&
            x.BOM_TYPE == "0" &&
            x.STATUS == "0");
        }
    }
}
