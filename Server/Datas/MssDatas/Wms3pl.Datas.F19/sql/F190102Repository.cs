using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F190102Repository : RepositoryBase<F190102, Wms3plDbContext, F190102Repository>
    {
        public void DeletedF190102Datas(List<string> datas)
        {       
            var param = new List<object>();
            int paramStartIndex = 0;
            var inSql = param.CombineSqlInParameters("(DC_CODE+DELV_EFFIC)", datas, ref paramStartIndex);
            string sql = string.Format("DELETE F190102 WHERE {0}", inSql);
            ExecuteSqlCommand(sql, param.ToArray());
        }
        public IQueryable<F190102JoinF000904> GetF190102JoinF000904Datas(string DC_CODE, string TOPIC, string Subtopic)
        {
            List<SqlParameter> Parameters = new List<SqlParameter>();
            string sql = @"SELECT a.DELV_EFFIC
                                             ,ISNULL(b.NAME,'') Delv_Effic_Name
                                       FROM F190102 a left join VW_F000904_LANG b
                                       on a.DELV_EFFIC=b.VALUE
                                       WHERE a.DC_CODE=@p0
                                       and b.TOPIC=@p1
                                       and b.SUBTOPIC=@p2
									   and b.LANG =@p3";
            Parameters.Add(new SqlParameter("@p0", DC_CODE));
            Parameters.Add(new SqlParameter("@p1", TOPIC));
            Parameters.Add(new SqlParameter("@p2", Subtopic));
            Parameters.Add(new SqlParameter("@p3", Current.Lang));
            var result = SqlQuery<F190102JoinF000904>(sql.ToString(), Parameters.ToArray()).AsQueryable();
            return result;
        }
    }
}
