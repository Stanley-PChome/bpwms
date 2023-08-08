using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F19540202Repository : RepositoryBase<F19540202, Wms3plDbContext, F19540202Repository>
    {
        public void CopyMenuCategoryToNewMenuCategory(string oldMenuCode, string newMenuCode)
        {
            var sql = @" INSERT INTO F19540202(MENU_CODE,CATEGORY_LEVEL,CATEGORY,CATEGORY_SORT,CRT_DATE,CRT_STAFF,CRT_NAME)
									 SELECT @p0,CATEGORY_LEVEL,CATEGORY,CATEGORY_SORT,@p1 CRT_DATE,@p2 CRT_STAFF,@p3 CRT_NAME
										 FROM F19540202
								  	WHERE MENU_CODE = @p4 ";

            ExecuteSqlCommand(sql, new object[] { newMenuCode, DateTime.Now, Current.Staff, Current.StaffName, oldMenuCode });
        }
    }
}
