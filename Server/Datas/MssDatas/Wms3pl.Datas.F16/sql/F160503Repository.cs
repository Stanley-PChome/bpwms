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
	public partial class F160503Repository : RepositoryBase<F160503, Wms3plDbContext, F160503Repository>
	{

        public IQueryable<F160501FileData> GetDestoryNoFile(string destoryNo)
        {
            var sqlParamers = new List<object>
            {
                destoryNo
            };
            var sql = @"					
						select A.DESTROY_NO,C.UPLOAD_SEQ 
							,C.UPLOAD_S_PATH ,C.UPLOAD_C_PATH ,C.UPLOAD_DESC
							,C.DC_CODE ,C.GUP_CODE ,C.CUST_CODE

						from F160501 A 
						left join F16050301 B on A.DESTROY_NO = B.DESTROY_NO AND A.DC_CODE =A.DC_CODE 
												 AND A.GUP_CODE =B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE
						left join F160503 C on C.UPLOAD_SEQ  = B.UPLOAD_SEQ AND B.DC_CODE =C.DC_CODE 
												 AND B.GUP_CODE =C.GUP_CODE AND B.CUST_CODE = C.CUST_CODE
						where A.DESTROY_NO =@p0  order by A.DESTROY_NO
			";

            var result = SqlQuery<F160501FileData>(sql, sqlParamers.ToArray()).AsQueryable();
            return result;
        }

        public IQueryable<F160501FileData> GetDestoryNoRelation(string destoryNo)
        {
            var sqlParamers = new List<object>
            {
                destoryNo
            };
            var sql = @"					
						select distinct A.DESTROY_NO,A.UPLOAD_SEQ
							,B.UPLOAD_SEQ ,B.UPLOAD_S_PATH ,B.UPLOAD_C_PATH ,B.UPLOAD_DESC
							,B.DC_CODE ,B.GUP_CODE ,B.CUST_CODE ,'1' DB_Flag , '0' IsDelete
						from F16050301 A
						left join F160503 B on B.UPLOAD_SEQ  = A.UPLOAD_SEQ AND B.DC_CODE =A.DC_CODE 
                                                 AND B.GUP_CODE =A.GUP_CODE AND B.CUST_CODE = A.CUST_CODE                                                 
						where A.UPLOAD_SEQ IN (
							select UPLOAD_SEQ from F16050301 where DESTROY_NO =@p0
						)
						order by A.DESTROY_NO ,A.UPLOAD_SEQ 
						";
            var result = SqlQuery<F160501FileData>(sql, sqlParamers.ToArray()).AsQueryable();
            return result;
        }

        public bool DeleteF160503File(string uploadSeq)
        {
            //只要更新主檔，狀態一律改回 : 待確認
            var sqlParamers = new List<SqlParameter>();
            sqlParamers.Add(new SqlParameter("@p0", uploadSeq));
            string sql = @"
				delete F160503 where UPLOAD_SEQ =@p0 				
			";
            ExecuteSqlCommand(sql, sqlParamers.ToArray());
            return true;
        }
    }
}
