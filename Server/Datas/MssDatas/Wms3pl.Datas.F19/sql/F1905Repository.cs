using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
	public partial class F1905Repository : RepositoryBase<F1905, Wms3plDbContext, F1905Repository>
	{

        public IQueryable<F1905Data> GetPackCase(string gupCode,string custCode, string itemCode, string itemName = null)
        {
            var paramers = new List<SqlParameter>();

            string gupCodeCondition = string.Empty;
            string custCodeCondition = string.Empty;
            string itemCodeCondition = (string.IsNullOrEmpty(itemCode)) ? string.Empty : "AND a.ITEM_CODE LIKE '" + itemCode + "%' ";
            string itemNameCondition = (string.IsNullOrEmpty(itemName)) ? string.Empty : "AND d.ITEM_NAME LIKE '" + itemName + "%' ";
            if (!string.IsNullOrEmpty(gupCode) && gupCode != "0")
            {
                gupCodeCondition = string.Format("AND a.GUP_CODE = @p{0} ", paramers.Count());
                paramers.Add(new SqlParameter(string.Format("@p{0}", paramers.Count()), gupCode));
            }
						if (!string.IsNullOrEmpty(custCode) && custCode != "0")
						{
							gupCodeCondition = string.Format("AND a.CUST_CODE = @p{0} ", paramers.Count());
							paramers.Add(new SqlParameter(string.Format("@p{0}", paramers.Count()), custCode));
						}
						var strSQL = @"SELECT TOP 50
                                a.ITEM_CODE,d.ITEM_NAME,a.GUP_CODE,b.GUP_NAME,a.PACK_WEIGHT
        									 FROM F1905 a 
        							LEFT JOIN F1929 b on a.GUP_CODE = b.GUP_CODE
        							LEFT JOIN F1903 d on a.GUP_CODE = d.GUP_CODE AND a.ITEM_CODE = d.ITEM_CODE AND a.CUST_CODE = d.CUST_CODE
                                WHERE 1=1 ";
            strSQL += gupCodeCondition;
            strSQL += custCodeCondition;
            strSQL += itemCodeCondition;
            strSQL += itemNameCondition;
            return SqlQuery<F1905Data>(strSQL, paramers.ToArray());
        }



    }
}
