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
	public partial class F160504Repository : RepositoryBase<F160504, Wms3plDbContext, F160504Repository>
	{

        public IQueryable<F160502Data> Get160504SerialData(string dcCode, string gupCode, string custCode, string destoryNo)
        {
            var sqlParamers = new List<object>
            {
                dcCode,
                gupCode,
                custCode,
                destoryNo
            };
            var sql = @"		
			
					 select A.DESTROY_NO, A.DC_CODE ,A.GUP_CODE ,A.CUST_CODE ,A.ITEM_CODE
							,B.ITEM_NAME ,B.ITEM_SIZE ,B.ITEM_SPEC ,B.ITEM_COLOR 
							,'是' VIRTUAL_TYPE
							,1 DESTROY_QTY 
							,'1' BUNDLE_SERIALNO
							,A.SERIAL_NO ITEM_SERIALNO
                    from F160504 A
                    join F1903 B ON B.GUP_CODE=A.GUP_CODE AND B.ITEM_CODE = A.ITEM_CODE AND B.CUST_CODE = A.CUST_CODE
					where A.DC_CODE =@p0 AND A.GUP_CODE =@p1 AND A.CUST_CODE =@p2 AND A.DESTROY_NO = @p3			
			";

            sql += " order by A.SERIAL_SEQ ";

            var result = SqlQuery<F160502Data>(sql, sqlParamers.ToArray()).AsQueryable();
            return result;
        }

        public bool DeleteF160504s(string destroyNo)
        {

            var sqlParamers = new List<SqlParameter>();
            sqlParamers.Add(new SqlParameter("@p0", destroyNo));

            string sql = @"
				delete from F160504 where DESTROY_NO =@p0 				
			";
            ExecuteSqlCommand(sql, sqlParamers.ToArray());
            return true;
        }
    }
}
