using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F16
{
	public partial class F161601Repository : RepositoryBase<F161601, Wms3plDbContext, F161601Repository>
	{
		public F161601Repository(string connName, WmsTransaction wmsTransaction = null)
			: base(connName, wmsTransaction)
		{
		}
        public IQueryable<F161601> GetUpLocDataByDc(string dcCode, DateTime rtnApplyDate)
        {
            return _db.F161601s
                .Join(_db.F151001s, A => new { A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.RTN_APPLY_NO }, B => new { B.DC_CODE, B.GUP_CODE, B.CUST_CODE, RTN_APPLY_NO = B.SOURCE_NO }, (A, B) => new { A, B })
                .Where(x => x.A.DC_CODE == dcCode)
                .Where(x => x.A.RTN_APPLY_DATE == rtnApplyDate)
                .Where(x => x.B.STATUS == "5")
                .Select(x=>x.A);
        }
    }

}