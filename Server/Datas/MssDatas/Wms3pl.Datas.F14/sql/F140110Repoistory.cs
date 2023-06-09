using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F14
{
	public partial class F140110Repository : RepositoryBase<F140110, Wms3plDbContext, F140110Repository>
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="inventoryNo"></param>
        /// <param name="isSecond"></param>
        /// <param name="locCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="itemName"></param>
        /// <param name="orginalItemCode"></param>
        public void UpdateItemForSql(string dcCode, string gupCode, string custCode, string inventoryNo, string isSecond,
            string locCode, string itemCode, string itemName, string orginalItemCode)
        {

            var sql = @"
                        UPDATE F140110 
                        SET    ITEM_CODE = @p0, 
                               ITEM_NAME = @p1, 
                               UPD_DATE = dbo.GetSysDate(), 
                               UPD_STAFF = @p2, 
                               UPD_NAME = @p3 
                        WHERE  DC_CODE = @p4 
                           AND GUP_CODE = @p5 
                           AND CUST_CODE = @p6 
                           AND INVENTORY_NO = @p7 
                           AND ISSECOND = @p8 
                           AND LOC_CODE = @p9 
                           AND ITEM_CODE = @p10 
                        ";
            var param = new object[]{
                itemCode
                ,itemName
                ,Current.Staff
                ,Current.StaffName
                ,dcCode
                ,gupCode
                ,custCode
                ,inventoryNo
                ,isSecond
                ,locCode
                ,orginalItemCode};
            ExecuteSqlCommand(sql, param);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="inventoryNo"></param>
        /// <param name="isSecond"></param>
        /// <param name="locCode"></param>
        /// <param name="crtStaff"></param>
        /// <param name="crtStaffName"></param>
        public void DeleteByLocNotInventory(string dcCode, string gupCode, string custCode, string inventoryNo,
            string isSecond, string locCode, string crtStaff, string crtStaffName)
        {
            var param = new object[] { dcCode, gupCode, custCode, inventoryNo, isSecond, locCode, crtStaff, crtStaffName };
            var sql = @"
                        DELETE FROM F140110 
                        WHERE  DC_CODE = @p0 
                           AND GUP_CODE = @p1 
                           AND CUST_CODE = @p2 
                           AND INVENTORY_NO = @p3 
                           AND ISSECOND = @p4 
                           AND LOC_CODE = @p5 
                           AND CRT_STAFF = @p6 
                           AND CRT_NAME = @p7 
                           AND ( INVENTORY_QTY IS NULL 
                                  OR ITEM_CODE = '******' ) 
                        ";
            ExecuteSqlCommand(sql, param);
        }
    }
}
