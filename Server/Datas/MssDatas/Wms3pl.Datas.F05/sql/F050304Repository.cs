using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
    public partial class F050304Repository : RepositoryBase<F050304, Wms3plDbContext, F050304Repository>
    {
        #region 若原始資料為超取,編輯後為非超取,則需要DeleteF050304
        public void DeleteF050304(string dcCode, string gupCode, string custCode, string ordNo)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", ordNo)
            };
            var sql = @" DELETE 
                                   FROM F050304 
                                  WHERE DC_CODE = @p0 
                                    AND GUP_CODE = @p1 
                                    AND CUST_CODE = @p2 
                                    AND ORD_NO = @p3 ";
            ExecuteSqlCommand(sql, parameters.ToArray());
        }
        #endregion

        #region 若原始資料為超取,編輯後為非超取,則需要DeleteF050304
        public void UpdateF050304(F050304 f050304)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", f050304.ALL_ID),
                new SqlParameter("@p1", f050304.BATCH_NO),
                new SqlParameter("@p2", f050304.CONSIGN_NO),
                new SqlParameter("@p3", f050304.ESERVICE),
                new SqlParameter("@p4", f050304.DELV_RETAILCODE),
                new SqlParameter("@p5", f050304.DELV_RETAILNAME),
                new SqlParameter("@p6", Current.Staff),
                new SqlParameter("@p7", Current.StaffName),
                new SqlParameter("@p8", f050304.DC_CODE),
                new SqlParameter("@p9", f050304.GUP_CODE),
                new SqlParameter("@p10", f050304.CUST_CODE),
                new SqlParameter("@p11", f050304.ORD_NO),
                new SqlParameter("@p12", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 }
            };
            var sql = @" UPDATE F050304 SET ALL_ID = @p0,
                                         BATCH_NO = @p1, 
                                         CONSIGN_NO = @p2,
                                         ESERVICE = @p3,
                                         DELV_RETAILCODE = @p4,
                                         DELV_RETAILNAME = @p5, 
                                         UPD_DATE = @p12,
                                         UPD_STAFF = @p6, 
                                         UPD_NAME = @p7
                                          WHERE DC_CODE = @p8 
                                   AND GUP_CODE = @p9 
                                   AND CUST_CODE = @p10 
                                   AND ORD_NO = @p11 ";

            ExecuteSqlCommand(sql, parameters.ToArray());
        }
        #endregion
    }
}
