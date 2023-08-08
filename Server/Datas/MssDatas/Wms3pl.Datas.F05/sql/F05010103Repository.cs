using System.Collections.Generic;
using System.Data.SqlClient;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F05010103Repository : RepositoryBase<F05010103, Wms3plDbContext, F05010103Repository>
	{
        public void Insert(F05010103 f05010103)
        {
            var param = new List<SqlParameter>
                        {
                                new SqlParameter("@p0", f05010103.DC_CODE),
                                new SqlParameter("@p1", f05010103.GUP_CODE),
                                new SqlParameter("@p2", f05010103.CUST_CODE),
                                new SqlParameter("@p3", f05010103.TYPE),
                                new SqlParameter("@p4", f05010103.ORD_NO),
                                new SqlParameter("@p5", f05010103.DELV_RETAILCODE),
                                new SqlParameter("@p6", f05010103.DELV_RETAILNAME),
                                new SqlParameter("@p7", f05010103.CONSIGN_NO),
                                new SqlParameter("@p8", f05010103.CRT_DATE.ToString("yyyy/MM/dd HH:mm:ss")),
                                new SqlParameter("@p9", f05010103.CRT_STAFF),
                                new SqlParameter("@p10", f05010103.CRT_NAME)
                        };

            var sql = @" INSERT INTO F05010103
                                (DC_CODE,
                                GUP_CODE,
                                CUST_CODE,
                                TYPE,ORD_NO,
                                DELV_RETAILCODE,
                                DELV_RETAILNAME,
                                CONSIGN_NO,
                                CRT_DATE,
                                CRT_STAFF,
                                CRT_NAME)
				         VALUES(@p0,
                                @p1,
                                @p2,
                                @p3,
                                @p4,
                                @p5,
                                @p6,
                                @p7,
                                @p8,
                                @p9,
                                @p10) ";

            ExecuteSqlCommand(sql, param.ToArray());
        }
    }
}
