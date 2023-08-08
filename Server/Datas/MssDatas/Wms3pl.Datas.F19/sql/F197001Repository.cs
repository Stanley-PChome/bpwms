using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F19
{
    public partial class F197001Repository : RepositoryBase<F197001, Wms3plDbContext, F197001Repository>
    {
        public ExecuteResult UpdateF197001(F197001 f197001Data)
        {
            var sqlParamers = new List<SqlParameter>();
            sqlParamers.Add(new SqlParameter("@p0", f197001Data.WARRANTY));
            sqlParamers.Add(new SqlParameter("@p1", f197001Data.WARRANTY_S_Y));
            sqlParamers.Add(new SqlParameter("@p2", f197001Data.WARRANTY_S_M));
            sqlParamers.Add(new SqlParameter("@p3", f197001Data.WARRANTY_Y));
            sqlParamers.Add(new SqlParameter("@p4", f197001Data.WARRANTY_M));
            sqlParamers.Add(new SqlParameter("@p5", f197001Data.WARRANTY_D));
            sqlParamers.Add(new SqlParameter("@p6", f197001Data.OUTSOURCE));
            sqlParamers.Add(new SqlParameter("@p7", f197001Data.CHECK_STAFF));
            sqlParamers.Add(new SqlParameter("@p8", f197001Data.ITEM_DESC_A));
            sqlParamers.Add(new SqlParameter("@p9", f197001Data.ITEM_DESC_B));
            sqlParamers.Add(new SqlParameter("@p10", f197001Data.ITEM_DESC_C));
            sqlParamers.Add(new SqlParameter("@p11", Current.Staff));
            sqlParamers.Add(new SqlParameter("@p12", Current.StaffName));
            //WHERE 
            sqlParamers.Add(new SqlParameter("@p13", f197001Data.LABEL_SEQ));
            sqlParamers.Add(new SqlParameter("@p14", f197001Data.LABEL_CODE));
            sqlParamers.Add(new SqlParameter("@p15", f197001Data.GUP_CODE));
            sqlParamers.Add(new SqlParameter("@p16", f197001Data.CUST_CODE));
            sqlParamers.Add(new SqlParameter("@p17", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 });

            string sql = @"
				update F197001 set 
				WARRANTY = @p0,
				WARRANTY_S_Y =@p1,
				WARRANTY_S_M =@p2,
				WARRANTY_Y =@p3,				
				WARRANTY_M=@p4,
				WARRANTY_D=@p5,
				OUTSOURCE=@p6,
				CHECK_STAFF=@p7,
				ITEM_DESC_A=@p8,
				ITEM_DESC_B=@p9,
				ITEM_DESC_C=@p10,
				UPD_STAFF =@p11,
				UPD_NAME = @p12,
				UPD_DATE=@p17
				where	LABEL_SEQ =@p13  AND LABEL_CODE =@P14
						AND GUP_CODE =@P15	AND CUST_CODE =@P16											
			";

            if (!string.IsNullOrEmpty(f197001Data.ITEM_CODE))
            {
                sql += " AND ITEM_CODE = @p" + sqlParamers.Count;
                sqlParamers.Add(new SqlParameter("@p" + sqlParamers.Count, f197001Data.ITEM_CODE));
            }
            if (!string.IsNullOrEmpty(f197001Data.VNR_CODE))
            {
                sql += " AND VNR_CODE = @p" + sqlParamers.Count;
                sqlParamers.Add(new SqlParameter("@p" + sqlParamers.Count, f197001Data.VNR_CODE));
            }

            ExecuteSqlCommand(sql, sqlParamers.ToArray());
            return new ExecuteResult { IsSuccessed = true };
        }

        public ExecuteResult DelF197001(F197001Data f197001Data)
        {
            var sqlParamers = new List<SqlParameter>();

            //WHERE 
            sqlParamers.Add(new SqlParameter("@p0", f197001Data.LABEL_SEQ));
            sqlParamers.Add(new SqlParameter("@p1", f197001Data.LABEL_CODE));
            sqlParamers.Add(new SqlParameter("@p2", f197001Data.GUP_CODE));
            sqlParamers.Add(new SqlParameter("@p3", f197001Data.CUST_CODE));

            string sql = @"
				delete from F197001 
				where	LABEL_SEQ =@p0  AND LABEL_CODE =@p1
						AND GUP_CODE =@p2	AND CUST_CODE =@p3										
			";

            if (!string.IsNullOrEmpty(f197001Data.ITEM_CODE))
            {
                sql += " AND ITEM_CODE = @p" + sqlParamers.Count;
                sqlParamers.Add(new SqlParameter("@p" + sqlParamers.Count, f197001Data.ITEM_CODE));
            }
            if (!string.IsNullOrEmpty(f197001Data.VNR_CODE))
            {
                sql += " AND VNR_CODE = @p" + sqlParamers.Count;
                sqlParamers.Add(new SqlParameter("@p" + sqlParamers.Count, f197001Data.VNR_CODE));
            }

            ExecuteSqlCommand(sql, sqlParamers.ToArray());
            return new ExecuteResult { IsSuccessed = true };
        }

        public IQueryable<F197001Data> GetF197001Data(string gupCode, string custCode, string labelCode, string itemCode, string vnrCode)
        {
            var sqlParamers = new List<SqlParameter>();
            sqlParamers.Add(new SqlParameter("@p0", gupCode));
            sqlParamers.Add(new SqlParameter("@p1", custCode));

            string sql = @"			
			
			select 
				A.LABEL_SEQ,A.LABEL_CODE,B.LABEL_NAME   
				,A.VNR_CODE , C.VNR_NAME
				,A.ITEM_CODE ,D.ITEM_NAME ,D.ITEM_COLOR ,D.ITEM_SIZE ,D.ITEM_SPEC
				,D.CUST_ITEM_CODE ,D.SIM_SPEC SUGR
				,A.WARRANTY,A.WARRANTY_S_Y,A.WARRANTY_S_M
				,A.WARRANTY_Y,A.WARRANTY_M,A.WARRANTY_D
				,A.OUTSOURCE,A.CHECK_STAFF
				,A.ITEM_DESC_A,A.ITEM_DESC_B,A.ITEM_DESC_C
				,A.GUP_CODE ,A.CUST_CODE
				,A.CRT_DATE ,A.CRT_NAME ,A.UPD_DATE ,A.UPD_NAME
				,1 AS Qty
			from F197001 A
			join F1970 B on A.LABEL_CODE = B.LABEL_CODE and A.GUP_CODE =B.GUP_CODE and A.CUST_CODE =B.CUST_CODE
			left join F1908 C on C.VNR_CODE =A.VNR_CODE and A.GUP_CODE =C.GUP_CODE
			left join F1903 D on D.ITEM_CODE =A.ITEM_CODE  and A.GUP_CODE =D.GUP_CODE and A.CUST_CODE =D.CUST_CODE  
			where A.GUP_CODE =@p0 and  A.CUST_CODE =@p1
			";

            if (!string.IsNullOrEmpty(labelCode))
            {
                sql += " AND A.LABEL_CODE = @p" + sqlParamers.Count;
                sqlParamers.Add(new SqlParameter("@p" + sqlParamers.Count, labelCode));
            }

            if (!string.IsNullOrEmpty(itemCode))
            {
                sql += " AND A.ITEM_CODE = @p" + sqlParamers.Count;
                sqlParamers.Add(new SqlParameter("@p" + sqlParamers.Count, itemCode));
            }

            if (!string.IsNullOrEmpty(vnrCode))
            {
                sql += " AND A.VNR_CODE = @p" + sqlParamers.Count;
                sqlParamers.Add(new SqlParameter("@p" + sqlParamers.Count, vnrCode));
            }

            var result = SqlQuery<F197001Data>(sql, sqlParamers.ToArray()).AsQueryable();
            return result;

        }
    }
}
