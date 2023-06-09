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
    public partial class F194704Repository : RepositoryBase<F194704, Wms3plDbContext, F194704Repository>
    {
        public F194704Repository(string connName, WmsTransaction wmsTransaction = null)
        : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F194704Data> GetF194704Datas(string dcCode, string gupCode, string custCode)
        {
            var f194704s = _db.F194704s.AsNoTracking().Where(x => x.DC_CODE == dcCode);
            var f1947s = _db.F1947s.AsNoTracking();
            var f1929s = _db.F1929s.AsNoTracking();
            var f1909s = _db.F1909s.AsNoTracking();
            var f1901s = _db.F1901s.AsNoTracking();

            if (!string.IsNullOrEmpty(gupCode))
            {
                f194704s = f194704s.Where(x => x.GUP_CODE == gupCode);
            }

            if (!string.IsNullOrEmpty(custCode))
            {
                f194704s = f194704s.Where(x => x.CUST_CODE == custCode);
            }

            var result = from A in f194704s
                         join B in f1947s on new { A.DC_CODE, A.ALL_ID } equals new { B.DC_CODE, B.ALL_ID }
                         join C in f1929s on A.GUP_CODE equals C.GUP_CODE
                         join D in f1909s on new { A.GUP_CODE, A.CUST_CODE } equals new { D.GUP_CODE, D.CUST_CODE }
                         join E in f1901s on A.DC_CODE equals E.DC_CODE
                         orderby A.ALL_ID, A.GUP_CODE, A.CUST_CODE
                         select new F194704Data
                         {
                             ALL_ID = A.ALL_ID,
                             DC_CODE = A.DC_CODE,
                             GUP_CODE = A.GUP_CODE,
                             CUST_CODE = A.CUST_CODE,
                             CRT_STAFF = A.CRT_STAFF,
                             CRT_DATE = A.CRT_DATE,
                             UPD_STAFF = A.UPD_STAFF,
                             UPD_DATE = A.UPD_DATE,
                             CRT_NAME = A.CRT_NAME,
                             UPD_NAME = A.UPD_NAME,
                             ALL_COMP = B.ALL_COMP,
                             DC_NAME = E.DC_NAME,
                             GUP_NAME = C.GUP_NAME,
                             CUST_NAME = D.CUST_NAME,
                             CONSIGN_FORMAT = A.CONSIGN_FORMAT,
                             GET_CONSIGN_NO = A.GET_CONSIGN_NO,
                             PRINT_CONSIGN = A.PRINT_CONSIGN,
                             PRINTER_TYPE = A.PRINTER_TYPE,
                             AUTO_PRINT_CONSIGN = A.AUTO_PRINT_CONSIGN,
                             ZIP_CODE = A.ZIP_CODE,
                             ADDBOX_GET_CONSIGN_NO = A.ADDBOX_GET_CONSIGN_NO
                         };

            return result;
        }
    }
}
