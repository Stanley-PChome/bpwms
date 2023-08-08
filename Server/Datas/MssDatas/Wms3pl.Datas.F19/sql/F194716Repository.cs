using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F194716Repository : RepositoryBase<F194716, Wms3plDbContext, F194716Repository>
    {
        public void UpdateHasKey(F194716 data)
        {
            var sql = @" UPDATE F194716
                      SET CAR_PERIOD = @p0,
                          CAR_GUP = @p1,
                          DRIVER_ID = @p2,
                          DRIVER_NAME = @p3,
                          EXTRA_FEE = @p4,
                          UPD_DATE = @p5,
                          UPD_STAFF = @p6,
                          UPD_NAME = @p7,
                          REGION_FEE = @p8,
                          OIL_FEE = @p9,
                          OVERTIME_FEE = @p10,
                          PACK_FIELD = @p11
                    WHERE GUP_CODE = @p12
                      AND CUST_CODE = @p13
                      AND DC_CODE = @p14
                      AND DELV_NO = @p15";

            var parms = new List<object>
            {
                data.CAR_PERIOD,
                data.CAR_GUP,
                data.DRIVER_ID,
                data.DRIVER_NAME,
                data.EXTRA_FEE,
                DateTime.Now,
                Current.Staff,
                Current.StaffName,
                data.REGION_FEE,
                data.OIL_FEE,
                data.OVERTIME_FEE,
                data.PACK_FIELD,
                data.GUP_CODE,
                data.CUST_CODE,
                data.DC_CODE,
                data.DELV_NO
            };

            ExecuteSqlCommand(sql, parms.ToArray());
        }
    }
}
