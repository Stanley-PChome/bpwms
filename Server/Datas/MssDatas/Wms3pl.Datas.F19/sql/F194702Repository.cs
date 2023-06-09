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
    public partial class F194702Repository : RepositoryBase<F194702, Wms3plDbContext, F194702Repository>
    {
        public IQueryable<F194702Data> GetF194702()
        {
            var sql = $@"SELECT a.CAR_KIND_ID,a.CAR_KIND_NAME,a.CAR_SIZE,a.TMPR_TYPE,						
						(SELECT NAME FROM VW_F000904_LANG WHERE TOPIC='F194702' AND SUBTOPIC='TMPR_TYPE' AND VALUE=a.TMPR_TYPE AND LANG = '{Current.Lang}') AS TMPR_TYPE_TEXT						
						FROM F194702 a
						ORDER BY a.CAR_KIND_ID";

            var result = SqlQuery<F194702Data>(sql).AsQueryable();
            return result;
        }
    }
}
