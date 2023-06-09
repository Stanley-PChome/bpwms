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
	public partial class F161202Repository : RepositoryBase<F161202, Wms3plDbContext, F161202Repository>
	{
        /// <summary>
        /// 將已回拋GOHAPPY的出貨單號押上FLAG
        /// </summary>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="dcCode"></param>
        /// <param name="wmsOdrNoList"></param>
        public void UpdateF161202Flag(string gupCode, string custCode, string dcCode, string wmsOdrNo)
        {
            var parameters = new List<object>
            {
                Current.Staff,
                Current.StaffName,
                gupCode,
                custCode,
                dcCode,
                wmsOdrNo
            };
            var sql = @"
            Update F161202 
               Set RTN_CUS_FLAG = '1', UPD_STAFF = :p0, UPD_NAME = :p1, UPD_DATE = SYSDATE
			 Where GUP_CODE = :p2
			   And CUST_CODE = :p3
			   And DC_CODE = :p4
			   And RETURN_NO = :p5 ";

            ExecuteSqlCommand(sql, parameters.ToArray());
        }
    }
}
