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
	public partial class F16050301Repository : RepositoryBase<F16050301, Wms3plDbContext, F16050301Repository>
	{
        public bool DeleteF16050301Serial(string destoryNo)
        {
            //只要更新主檔，狀態一律改回 : 待確認
            var sqlParamers = new List<SqlParameter>();
            sqlParamers.Add(new SqlParameter("@p0", destoryNo));
            string sql = @"
				delete F16050301 where DESTROY_NO =@p0 				
			";
            ExecuteSqlCommand(sql, sqlParamers.ToArray());
            return true;
        }
    }
}
