using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F02;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F01
{
    public partial class F010301_HISTORYRepository : RepositoryBase<F010301_HISTORY, Wms3plDbContext, F010301_HISTORYRepository>
    {
        /// <summary>
        /// 取得昨日之前的碼頭收貨刷貨檔
        /// </summary>
        /// <returns></returns>
        public IQueryable<F010301> GetOldF010301Datas()
        {
            var sql = "SELECT * FROM F010301 WHERE CRT_DATE<=dbo.getsysdate()";
            return SqlQuery<F010301>(sql);
        }

    }
}
