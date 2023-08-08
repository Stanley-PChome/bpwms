using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F192401Repository : RepositoryBase<F192401, Wms3plDbContext, F192401Repository>
    {
        public F192401Repository(string connName, WmsTransaction wmsTransaction = null)
         : base(connName, wmsTransaction)
        {
        }

        public IQueryable<FuncList> GetUserFuncList(string accNo, string isSync)
        {
            var f192401s = _db.F192401s.AsNoTracking().Where(x => x.EMP_ID == accNo);
            var f195301s = _db.F195301s.AsNoTracking().Where(x=>x.FUN_CODE.Contains("P80") && !x.FUN_CODE.EndsWith("000000"));

            // 若不要同步則過濾掉"同步"功能
            if (isSync == "0")
                f195301s = f195301s.Where(x => x.FUN_CODE != "P8001010000");

            var f1954s = _db.F1954s.AsNoTracking();
            var result = (from A in f192401s
                         join B in f195301s on A.GRP_ID equals B.GRP_ID
                         join C in f1954s on B.FUN_CODE equals C.FUN_CODE
                         select new FuncList
                         {
                             FuncNo = B.FUN_CODE,
                             FuncName = C.FUN_NAME,
                             MainShow = C.MAIN_SHOW,
                             SideShow = C.SIDE_SHOW
                         }).Distinct().ToList();
            for (int i = 0; i < result.Count; i++)
            {
                result[i].FuncSeq = i + 1;
            }
            return result.AsQueryable();
        }

    }
}
