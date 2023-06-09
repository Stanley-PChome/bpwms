using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F194716Repository : RepositoryBase<F194716, Wms3plDbContext, F194716Repository>
    {
        public F194716Repository(string connName, WmsTransaction wmsTransaction = null)
       : base(connName, wmsTransaction)
        {
        }

        public IQueryable<RetailCarPeriod> GetRetailCarPeriods(string dcCode, string gupCode, string custCode, List<string> retailCodes)
        {
            var f194716s = _db.F194716s.AsNoTracking();
            var f19471601s = _db.F19471601s.AsNoTracking().Where(x => x.DC_CODE == dcCode
                                                                && x.GUP_CODE == gupCode
                                                                && x.CUST_CODE == custCode
                                                                && retailCodes.Contains(x.RETAIL_CODE));
            var result = from A in f194716s
                         join B in f19471601s
                         on new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.DELV_NO }
                         equals new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, B.DELV_NO }
                         select new RetailCarPeriod
                         {
                             DC_CODE = A.DC_CODE,
                             GUP_CODE = A.GUP_CODE,
                             CUST_CODE = A.CUST_CODE,
                             DELV_NO = A.DELV_NO,
                             RETAIL_CODE = B.RETAIL_CODE
                         };
            return result.AsQueryable();
        }

        public IQueryable<F194716> GetF194716Datas(string gupCode, string custCode, string dcCode, string carPeriod, string delvNo, string carGup, string retailCode)
        {
            var result = _db.F194716s.Where(x => x.GUP_CODE == gupCode
                                            && x.CUST_CODE == custCode);
            var f19471601s = _db.F19471601s.Where(x => x.RETAIL_CODE == retailCode).Select(x=>new {
                x.DC_CODE,
                x.GUP_CODE,
                x.CUST_CODE,
                x. DELV_NO
            });


            if (!string.IsNullOrEmpty(dcCode))
            {
                result = result = result.Where(x => x.DC_CODE == dcCode);
            }
            if (!string.IsNullOrEmpty(carPeriod))
            {
                result = result.Where(x => x.CAR_PERIOD == carPeriod);
            }
            if (!string.IsNullOrEmpty(carGup))
            {
                result = result.Where(x => x.CAR_GUP == carGup);
            }
            if (!string.IsNullOrEmpty(retailCode))
            {
                result = result.Where(x => f19471601s.Select(p => p.DC_CODE).Contains(x.DC_CODE)
                                        && f19471601s.Select(q => q.GUP_CODE).Contains(x.GUP_CODE)
                                        && f19471601s.Select(r => r.CUST_CODE).Contains(x.CUST_CODE)
                                        && f19471601s.Select(s => s.DELV_NO).Contains(x.DELV_NO));
            }
            if (!string.IsNullOrEmpty(delvNo))
            {
                result = result.Where(x => x.DELV_NO == delvNo);
            }
            return result;
        }
    }
}
