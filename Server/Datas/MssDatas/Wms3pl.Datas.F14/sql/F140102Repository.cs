using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F14
{
	public partial class F140102Repository : RepositoryBase<F140102, Wms3plDbContext, F140102Repository>
	{
        public bool IsExistDatasByTheSameWareHouseChannel(string dcCode, string gupCode, string custCode, short inventoryYear,
            short inventoryMon, string warehouseId, string begFloor, string endFloor, string begChannel, string endChannel, string begPlain, string endPlain)
        {
            var param = new object[] { dcCode, gupCode, custCode, inventoryYear, inventoryMon, warehouseId, begFloor, endFloor, begFloor, endFloor, begChannel, endChannel, begChannel, endChannel, begPlain, endPlain, begPlain, endPlain };
            var sql = @"
                        SELECT *
                        FROM   F140102 A
                               INNER JOIN F140101 B
                                       ON B.DC_CODE = A.DC_CODE
                                          AND B.GUP_CODE = A.GUP_CODE
                                          AND B.CUST_CODE = A.CUST_CODE
                                          AND B.INVENTORY_NO = A.INVENTORY_NO
                        WHERE  A.DC_CODE = @p0
                               AND A.GUP_CODE = @p1
                               AND A.CUST_CODE = @p2
                               AND B.INVENTORY_YEAR = @p3
                               AND B.INVENTORY_MON = @p4
                               AND A.WAREHOUSE_ID = @p5
                               AND ( ( A.FLOOR_BEGIN >= @p6
                                       AND A.FLOOR_BEGIN <= @p7 )
                                      OR ( A.FLOOR_END >= @p8
                                           AND A.FLOOR_END <= @p9 ) )
                               AND ( ( A.CHANNEL_BEGIN >= @p10
                                       AND A.CHANNEL_BEGIN <= @p11 )
                                      OR ( A.CHANNEL_END >= @p12
                                           AND A.CHANNEL_END <= @p13 ) )
                               AND ( ( A.PLAIN_BEGIN >= @p14
                                       AND A.PLAIN_BEGIN <= @p15 )
                                      OR ( A.PLAIN_END >= @p16
                                           AND A.PLAIN_END <= @p17 ) )
                               AND B.STATUS <> '9' 
                    ";
            return SqlQuery<F140102>(sql, param).Any();
        }
    }
}
