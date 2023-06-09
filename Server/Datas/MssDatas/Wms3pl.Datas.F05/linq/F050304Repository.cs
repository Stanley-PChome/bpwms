using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
using Microsoft.EntityFrameworkCore;

namespace Wms3pl.Datas.F05
{
    public partial class F050304Repository : RepositoryBase<F050304, Wms3plDbContext, F050304Repository>
    {
        public F050304Repository(string connName, WmsTransaction wmsTransaction = null) : base(connName, wmsTransaction)
        {
        }

        public F050304 GetDataByWmsNo(string dcCode, string gupCode, string custCode, string wmsNo)
        {
            var f05030101Data = _db.F05030101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                         x.GUP_CODE == gupCode &&
                                                                         x.CUST_CODE == custCode &&
                                                                         x.WMS_ORD_NO == wmsNo);

            var f050304Data = _db.F050304s.Where(x => x.DC_CODE == dcCode &&
                                                      x.GUP_CODE == gupCode &&
                                                      x.CUST_CODE == custCode &&
                                                      f05030101Data.Select(z => z.ORD_NO).Contains(x.ORD_NO));

            var result = from A in f050304Data
                         join B in f05030101Data
                         on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.ORD_NO } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.ORD_NO }
                         select A;

            return result.FirstOrDefault();
        }


        public F050304Ex GetF050304ExData(string dcCode, string gupCode, string custCode, string wmsNo)
        {
            var f05030101Data = _db.F05030101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                         x.GUP_CODE == gupCode &&
                                                                         x.CUST_CODE == custCode &&
                                                                         x.WMS_ORD_NO == wmsNo);

            var f050304Data = _db.F050304s.Where(x => x.DC_CODE == dcCode &&
                                                      x.GUP_CODE == gupCode &&
                                                      x.CUST_CODE == custCode &&
                                                      f05030101Data.Select(z => z.ORD_NO).Contains(x.ORD_NO));

            var result = from A in f050304Data
                         join B in f05030101Data
                         on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.ORD_NO } equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.ORD_NO }
                         select new F050304Ex
                         {
                             CONSIGN_NO = A.CONSIGN_NO,
                             ORD_NO = A.ORD_NO,
                             WMS_ORD_NO = B.WMS_ORD_NO,
                             ALL_ID = A.ALL_ID
                         };

            return result.FirstOrDefault();
        }

        public IQueryable<F050304AddEService> GetF050304ExDatas(string dcCode, string gupCode, string custCode, string ordNo)
        {
             var f050101Data = _db.F050101s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                      x.GUP_CODE == gupCode &&
                                                                      x.CUST_CODE == custCode &&
                                                                      x.ORD_NO == ordNo);

            var f050304Data = _db.F050304s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
                                                                      x.GUP_CODE == gupCode &&
                                                                      x.CUST_CODE == custCode &&
                                                                      x.ORD_NO == ordNo);

            var result = from A in f050101Data
                         join B in f050304Data
                         on new { A.ORD_NO, A.DC_CODE, A.GUP_CODE, A.CUST_CODE } equals new { B.ORD_NO, B.DC_CODE, B.GUP_CODE, B.CUST_CODE } into subB
                         from B in subB.DefaultIfEmpty()
                         select new F050304AddEService
                         {
                             DC_CODE = A.DC_CODE,
                             GUP_CODE = A.GUP_CODE,
                             CUST_CODE = A.CUST_CODE,
                             ORD_NO = A.ORD_NO,
                             BATCH_NO = A.BATCH_NO,
                             CUST_ORD_NO = A.CUST_ORD_NO,
                             ALL_ID = A.ALL_ID,
                             DELV_RETAILCODE = B.DELV_RETAILCODE ?? null,
                             DELV_RETAILNAME = B.DELV_RETAILNAME ?? null,
                             CONSIGN_NO = B.CONSIGN_NO ?? null,
                             DELV_DATE = B.DELV_DATE ?? null,
                             RETURN_DATE = B.RETURN_DATE ?? null,
                             CRT_DATE = A.CRT_DATE,
                             CRT_STAFF = A.CRT_STAFF,
                             CRT_NAME = A.CRT_NAME,
                             UPD_DATE = A.UPD_DATE,
                             UPD_STAFF = A.UPD_STAFF,
                             UPD_NAME = A.UPD_NAME,
                             ESERVICE = B.ESERVICE,
                             ORD_TYPE = A.ORD_TYPE,
                             RETAIL_CODE = A.RETAIL_CODE,
                             ORD_DATE = A.ORD_DATE,
                             STATUS = A.STATUS,
                             CUST_NAME = A.CUST_NAME,
                             SELF_TAKE = A.SELF_TAKE,
                             FRAGILE_LABEL = A.FRAGILE_LABEL,
                             GUARANTEE = A.GUARANTEE,
                             SA = A.SA,
                             GENDER = A.GENDER,
                             AGE = A.AGE,
                             SA_QTY = A.SA_QTY,
                             TEL = A.TEL,
                             ADDRESS = A.ADDRESS,
                             CONSIGNEE = A.CONSIGNEE,
                             ARRIVAL_DATE = A.ARRIVAL_DATE,
                             TRAN_CODE = A.TRAN_CODE,
                             SP_DELV = A.SP_DELV,
                             CUST_COST = A.CUST_COST,
                             CHANNEL = A.CHANNEL,
                             POSM = A.POSM,
                             CONTACT = A.CONTACT,
                             CONTACT_TEL = A.CONTACT_TEL,
                             TEL_2 = A.TEL_2,
                             SPECIAL_BUS = A.SPECIAL_BUS,
                             COLLECT = A.COLLECT,
                             COLLECT_AMT = A.COLLECT_AMT,
                             MEMO = A.MEMO,
                             TYPE_ID = A.TYPE_ID,
                             CAN_FAST = A.CAN_FAST,
                             TEL_1 = A.TEL_1,
                             TEL_AREA = A.TEL_AREA,
                             PRINT_RECEIPT = A.PRINT_RECEIPT,
                             RECEIPT_NO = A.RECEIPT_NO,
                             RECEIPT_NO_HELP = A.RECEIPT_NO_HELP,
                             RECEIPT_TITLE = A.RECEIPT_TITLE,
                             RECEIPT_ADDRESS = A.RECEIPT_ADDRESS,
                             BUSINESS_NO = A.BUSINESS_NO,
                             DISTR_CAR_NO = A.DISTR_CAR_NO,
                             HAVE_ITEM_INVO = A.HAVE_ITEM_INVO,
                             NP_FLAG = A.NP_FLAG,
                             EXTENSION_A = A.EXTENSION_A,
                             EXTENSION_B = A.EXTENSION_B,
                             EXTENSION_C = A.EXTENSION_C,
                             EXTENSION_D = A.EXTENSION_D,
                             EXTENSION_E = A.EXTENSION_E,
                             SA_CHECK_QTY = A.SA_CHECK_QTY,
                             DELV_PERIOD = A.DELV_PERIOD,
                             CVS_TAKE = A.CVS_TAKE,
                             SUBCHANNEL = A.SUBCHANNEL,
                             CHECK_CODE = A.CHECK_CODE
                         };

            return result;
        }

       

        public IQueryable<F050304> GetDatas(string gupCode, string custCode, List<string> ordNos)
        {
            var result = _db.F050304s.Where(x => x.GUP_CODE == gupCode &&
                                                 x.CUST_CODE == custCode &&
                                                 ordNos.Contains(x.ORD_NO));

            return result;
        }
    }
}
