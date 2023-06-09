using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F02
{
	public partial class F02020107Repository : RepositoryBase<F02020107, Wms3plDbContext, F02020107Repository>
	{
		public F02020107Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}

        /// <summary>
        /// 取得託運單號
        /// </summary>
        /// <param name="dcNo"></param>
        /// <param name="custNo"></param>
        /// <param name="gupCode"></param>
        /// <param name="wmsNo"></param>
        /// <returns></returns>
        public IQueryable<string> GetAllocationNoDatas(string dcNo, string custNo, string gupCode, string wmsNo)
        {
            var result = _db.F02020107s.AsNoTracking().Where(x => x.DC_CODE == dcNo
                                                             && x.CUST_CODE == custNo
                                                             && x.GUP_CODE == gupCode
                                                             && x.RT_NO == wmsNo)
                                                      .Select(x=>x.ALLOCATION_NO)
                                                      .Distinct();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dcNo"></param>
        /// <param name="custNo"></param>
        /// <param name="gupCode"></param>
        /// <param name="allocNo"></param>
        /// <returns></returns>
        public IQueryable<string> GetRtNo(string dcNo, string custNo, string gupCode, string allocNo)
        {
            var result = _db.F02020107s.AsNoTracking().Where(x => x.DC_CODE == dcNo
                                                          && x.CUST_CODE == custNo
                                                          && x.GUP_CODE == gupCode
                                                          && x.ALLOCATION_NO == allocNo)
                                                      .Select(x=>x.RT_NO);
            return result;
        }

        public IQueryable<F02020107> GetDatasForAllNos(string dcCode, string gupCode, string custCode, List<string> allocNos)
        {
            return _db.F02020107s.AsNoTracking().Where(x => x.DC_CODE == dcCode &&
            x.CUST_CODE == custCode &&
            x.GUP_CODE == gupCode &&
            allocNos.Contains(x.ALLOCATION_NO));
        }
    }
}
