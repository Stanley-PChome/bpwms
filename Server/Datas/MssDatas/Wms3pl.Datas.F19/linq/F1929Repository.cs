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
    public partial class F1929Repository : RepositoryBase<F1929, Wms3plDbContext, F1929Repository>
    {
        public F1929Repository(string connName, WmsTransaction wmsTransaction = null)
       : base(connName, wmsTransaction)
        {
        }

        public IQueryable<F1929> GetDatas()
        {
            var result = _db.F1929s.AsQueryable();

            return result;
        }

        public IQueryable<F1929WithF1909Test> GetF1929WithF1909Tests(string gupCode)
        {
            var f1929s = _db.F1929s.AsNoTracking().Where(x => x.GUP_CODE == gupCode);
            var f1909s = _db.F1909s.AsNoTracking();
            var result = from A in f1929s
                         join B in f1909s on A.GUP_CODE equals B.GUP_CODE
                         select new F1929WithF1909Test
                         {
                             GUP_CODE = A.GUP_CODE,
                             GUP_NAME = A.GUP_NAME,
                             CRT_DATE = A.CRT_DATE,
                             CUST_CODE = B.CUST_CODE,
                             CUST_NAME = B.CUST_NAME
                         };
            return result;
        }
    }
}
