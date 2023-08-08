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
    public partial class F1953Repository : RepositoryBase<F1953, Wms3plDbContext, F1953Repository>
    {
        public F1953Repository(string connName, WmsTransaction wmsTransaction = null)
         : base(connName, wmsTransaction)
        {
        }
        public IQueryable<FunctionShowInfo> GetFunctionShowInfos(string account)
        {
            var F1953s = _db.F1953s.AsNoTracking();
            var f192401s = _db.F192401s.AsNoTracking().Where(x=>x.EMP_ID == account);
            var f195301s = _db.F195301s.AsNoTracking();
            var f1954s = _db.F1954s.AsNoTracking().Where(x => x.FUN_CODE.Substring(0, 1) != "B");
            var result = (from A in F1953s
                         join B in f192401s on A.GRP_ID equals B.GRP_ID
                         join C in f195301s on A.GRP_ID equals C.GRP_ID
                         join D in f1954s on C.FUN_CODE equals D.FUN_CODE
                         select new FunctionShowInfo
                         {
                             GRP_ID = A.GRP_ID,
                             SHOWINFO = A.SHOWINFO,
                             FUN_CODE = C.FUN_CODE
                         }).Distinct();
            return result;
        }
        
        public List<decimal> GetF1953Data()
        {
            var result = _db.F1953s.AsNoTracking().Select(x => x.GRP_ID).ToList();
            return result;
        }
    }
}
