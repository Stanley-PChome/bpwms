using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F19540201Repository : RepositoryBase<F19540201, Wms3plDbContext, F19540201Repository>
    {
        public void CopyMenuToNewMenu(string oldMenuCode, string newMenuCode)
        {
            var sql = @" INSERT INTO F19540201(MENU_CODE,CATEGORY,SUB_CATEGORY,FUN_CODE,FUN_SORT,CRT_DATE,CRT_STAFF,CRT_NAME)
									 SELECT @p0,CATEGORY,SUB_CATEGORY,FUN_CODE,FUN_SORT,dbo.GetSysDate() CRT_DATE,@p1 CRT_STAFF,@p2 CRT_NAME
										 FROM F19540201
								  	WHERE MENU_CODE = @p3 ";
            ExecuteSqlCommand(sql, new object[] { newMenuCode, Current.Staff, Current.StaffName, oldMenuCode });
        }

    }
}
