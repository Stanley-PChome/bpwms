using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F1925Repository : RepositoryBase<F1925, Wms3plDbContext, F1925Repository>
    {
		public bool ExistF1924(string depId)
		{
			var param = new List<object>{ depId};
			var sql = @"SELECT * FROM F1924 WHERE DEP_ID = @p0";
			var result = SqlQuery<F1924>(sql, param.ToArray()).Any()?true:false;
			return result;
		}
    }
}
