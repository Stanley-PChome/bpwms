using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;
namespace Wms3pl.Datas.F19
{
    public partial class F191205Repository : RepositoryBase<F191205, Wms3plDbContext, F191205Repository>
	{
		public void RemoveByMaxCrtDate(string dcCode,DateTime maxCrtDate)
		{
			var sql = @" DELETE FROM F191205 
                   WHERE DC_CODE = @p0 
                     AND CRT_DATE <= @p1 ";
			var parms = new object[] { dcCode, maxCrtDate };
			ExecuteSqlCommand(sql, parms);
		}

        public void RemoveByLocCode(string dcCode, string locCode)
        {
            var sql = @" DELETE FROM F191205 
                   WHERE DC_CODE = @p0 
                     AND LOC_CODE = @p1 ";
            var parms = new object[] { dcCode, locCode };
            ExecuteSqlCommand(sql, parms);
        }

        public void RemoveByCalvolumn(string dcCode)
        {
            var sql = @" DELETE A
                         FROM F191205 A
                         JOIN F1912 B
                           ON A.DC_CODE = B.DC_CODE
                          AND A.LOC_CODE = B.LOC_CODE
                         JOIN F198001 C
                           ON SUBSTRING(B.WAREHOUSE_ID, 1, 1) = C.TYPE_ID
                        WHERE C.CALVOLUMN = '0'
                          AND A.DC_CODE = @p0 ";
            var parms = new object[] { dcCode };
            ExecuteSqlCommand(sql, parms);
        }
    }
}
