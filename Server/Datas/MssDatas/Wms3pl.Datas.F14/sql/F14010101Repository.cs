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
	public partial class F14010101Repository : RepositoryBase<F14010101, Wms3plDbContext, F14010101Repository>
	{
        public void DeleteF14010101(string dcCode, string gupCode, string custCode, string inventoryNo, string locCode,
             string itemCode, DateTime validDate, DateTime enterDate, string boxCtrlNo, string palletCtrlNo, string makeNo)
        {
            List<object> param = new List<object>() { dcCode, gupCode, custCode, inventoryNo, locCode, itemCode, validDate, enterDate, boxCtrlNo, palletCtrlNo };

            var sql = @" DELETE FROM F14010101
                        WHERE  DC_CODE = @p0 
                               AND GUP_CODE = @p1 
                               AND CUST_CODE = @p2 
                               AND INVENTORY_NO = @p3 
                               AND LOC_CODE = @p4 
                               AND ITEM_CODE = @p5 
                               AND VALID_DATE = @p6 
                               AND ENTER_DATE = @p7 
                               AND BOX_CTRL_NO = @p8 
                               AND PALLET_CTRL_NO = @p9 ";

            if (!string.IsNullOrWhiteSpace(makeNo))
            {
                param.Add(makeNo);
                sql= string.Format("{0} {1}", sql, " AND MAKE_NO = @p10 ");
            }
            else
            {
                sql = string.Format("{0} {1}", sql, " AND MAKE_NO IS NULL ");
            }

            ExecuteSqlCommand(sql, param.ToArray());
        }
    }
}
