using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F91
{
    public partial class F91020502Repository : RepositoryBase<F91020502, Wms3plDbContext, F91020502Repository>
    {
        public F91020502Repository(string connName, WmsTransaction wmsTransaction = null)
            : base(connName, wmsTransaction)
        {
        }

        /// <summary>
        /// 取得建立揀料單時產生的調撥單號
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="processNo"></param>
        /// <returns></returns>
        public IQueryable<string> GetPickAllocationNos(string dcCode, string gupCode, string custCode, string processNo)
        {
            var q = from a in _db.F91020502s
                    join b in _db.F910205s on new { a.PICK_NO, a.GUP_CODE, a.CUST_CODE, a.DC_CODE }
                    equals new { b.PICK_NO, b.GUP_CODE, b.CUST_CODE, b.DC_CODE }
                    where a.DC_CODE == dcCode
                    && a.GUP_CODE == gupCode
                    && a.CUST_CODE == custCode
                    && b.PROCESS_NO == processNo
                    select a.ALLOCATION_NO;
            return q;
        }
    }
}
