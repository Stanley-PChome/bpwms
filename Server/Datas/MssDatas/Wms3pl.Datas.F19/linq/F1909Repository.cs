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
    public partial class F1909Repository : RepositoryBase<F1909, Wms3plDbContext, F1909Repository>
	{
		public F1909Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

		public IQueryable<F1909EX> GetF1909Datas()
		{
            var query1 = _db.F1909s
                        .Where(x => x.STATUS != "9")
                        .OrderBy(x => x.CUST_CODE)
                        .Select(x => new F1909EX
                        {
                            ROWNUM = 0,
                            LOC_CODE = "",

                            CUST_CODE = x.CUST_CODE,
                            CUST_NAME = x.CUST_NAME,
                            GUP_CODE = x.GUP_CODE,
                            SHORT_NAME = x.SHORT_NAME,
                            BOSS = x.BOSS,
                            CONTACT = x.CONTACT,
                            TEL = x.TEL,
                            ADDRESS = x.ADDRESS,
                            UNI_FORM = x.UNI_FORM,
                            ITEM_CONTACT = x.ITEM_CONTACT,
                            ITEM_TEL = x.ITEM_TEL,
                            ITEM_CEL = x.ITEM_CEL,
                            ITEM_MAIL = x.ITEM_MAIL,
                            BILL_CONTACT = x.BILL_CONTACT,
                            BILL_TEL = x.BILL_TEL,
                            BILL_CEL = x.BILL_CEL,
                            BILL_MAIL = x.BILL_MAIL,
                            CURRENCY = x.CURRENCY,
                            PAY_FACTOR = x.PAY_FACTOR,
                            PAY_TYPE = x.PAY_TYPE,
                            BANK_CODE = x.BANK_CODE,
                            BANK_NAME = x.BANK_NAME,
                            BANK_ACCOUNT = x.BANK_ACCOUNT,
                            ORDER_ADDRESS = x.ORDER_ADDRESS,
                            MIX_LOC_BATCH = x.MIX_LOC_BATCH,
                            MIX_LOC_ITEM = x.MIX_LOC_ITEM,
                            DC_TRANSFER = x.DC_TRANSFER,
                            BOUNDLE_SERIALLOC = x.BOUNDLE_SERIALLOC,
                            RTN_DC_CODE = x.RTN_DC_CODE,
                            SAM_ITEM = x.SAM_ITEM,
                            INSIDER_TRADING = x.INSIDER_TRADING,
                            INSIDER_TRADING_LIM = x.INSIDER_TRADING_LIM,
                            SPILT_ORDER = x.SPILT_ORDER,
                            SPILT_ORDER_LIM = x.SPILT_ORDER_LIM,
                            B2C_CAN_LACK = x.B2C_CAN_LACK,
                            CAN_FAST = x.CAN_FAST,
                            INSTEAD_INVO = x.INSTEAD_INVO,
                            SPILT_INCHECK = x.SPILT_INCHECK,
                            SPECIAL_IN = x.SPECIAL_IN,
                            CHECK_PERCENT = x.CHECK_PERCENT,
                            NEED_SEAL = x.NEED_SEAL,
                            DM = x.DM,
                            RIBBON = x.RIBBON,
                            RIBBON_BEGIN_DATE = x.RIBBON_BEGIN_DATE,
                            RIBBON_END_DATE = x.RIBBON_END_DATE,
                            CUST_BOX = x.CUST_BOX,
                            SP_BOX = x.SP_BOX,
                            SP_BOX_CODE = x.SP_BOX_CODE,
                            SPBOX_BEGIN_DATE = x.SPBOX_BEGIN_DATE,
                            SPBOX_END_DATE = x.SPBOX_END_DATE,
                            STATUS = x.STATUS,
                            UPD_STAFF = x.UPD_STAFF,
                            CRT_DATE = x.CRT_DATE,
                            UPD_DATE = x.UPD_DATE,
                            CRT_STAFF = x.CRT_STAFF,
                            CRT_NAME = x.CRT_NAME,
                            UPD_NAME = x.UPD_NAME,
                            INVO_ZIP = x.INVO_ZIP,
                            TAX_TYPE = x.TAX_TYPE,
                            INVO_ADDRESS = x.INVO_ADDRESS,
                            DUE_DAY = x.DUE_DAY,
                            INVO_LIM_QTY = x.INVO_LIM_QTY,
                            AUTO_GEN_RTN = x.AUTO_GEN_RTN,
                            SYS_CUST_CODE = x.SYS_CUST_CODE,
                            GUPSHARE = x.GUPSHARE,
                        });
            var query2 = query1.ToList();
            for (int i = 0; i < query2.Count(); i++)
                query2[i].ROWNUM = i + 1;
            return query2.AsQueryable();
            #region (就的程式碼有此註解)
            //SELECT ROWNUM,A.*
            //  FROM (
            //          SELECT F1909.*,F190904.LOC_CODE,f190101.dc_code
            //          FROM F1909  left join f190101
            //          on F1909.CUST_CODE=f190101.CUST_CODE
            //          AND F1909.GUP_CODE=f190101.GUP_CODE
            //          LEFT JOIN F190904
            //          ON F190904.CUST_CODE=f190101.CUST_CODE
            //          AND F190904.GUP_CODE=f190101.GUP_CODE
            //          AND F190904.DC_CODE=f190101.DC_CODE
            //          ORDER BY F1909.CUST_CODE) A 
            #endregion
		}

		public IQueryable<F1909> GetDatasByDc(string dcCode)
		{
            return _db.F1909s
                    .Join(_db.F190101s, a => new { a.GUP_CODE, a.CUST_CODE }, b => new { b.GUP_CODE, b.CUST_CODE }, (a, b) => new { a, b })
                    .Where(x => x.b.DC_CODE == dcCode)
                    .Select(x => x.a);
		}

		public IQueryable<F1909> GetF1909Datas(string gupCode, string custName, string account)
		{
            var query = _db.F1909s.Where(x => x.STATUS != "9");
			if (!string.IsNullOrEmpty(gupCode))
                query = query.Where(x => x.GUP_CODE == gupCode);
			if (!string.IsNullOrEmpty(custName))
                query = query.Where(x => x.CUST_NAME == custName);
            return query.OrderBy(x => x.GUP_CODE).ThenBy(x => x.CUST_CODE)
                    .Select(x => x);
		}

		public IQueryable<F1909> GetAll()
		{
            return _db.F1909s.Select(x=>x);
		}

        public IQueryable<F1909> GetData(string custCode)
        {
            return _db.F1909s.Where(x => x.CUST_CODE == custCode);
        }

        /// <summary>
        /// 取得業主編號
        /// </summary>
        /// <param name="custCode">貨主編號</param>
        /// <returns></returns>
        public string GetGupCode(string custCode)
        {
            var result = _db.F1909s.AsNoTracking().Where(x => x.CUST_CODE == custCode)
                                                  .Select(x=>x.GUP_CODE)
                                                  .FirstOrDefault();
            return result;
        }
    }

}
