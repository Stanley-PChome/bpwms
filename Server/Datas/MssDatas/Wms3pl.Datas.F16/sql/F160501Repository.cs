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

namespace Wms3pl.Datas.F16
{
	public partial class F160501Repository : RepositoryBase<F160501, Wms3plDbContext, F160501Repository>
	{
        //銷毀查詢
        public IQueryable<F160501Data> Get160501QueryData(
            string dcItem, string gupCode, string custCode, string destoryNo, DateTime? postingSDate, DateTime? postingEDate, string custOrdNo
            , string status, string ordNo, DateTime? crtSDate, DateTime? crtEDate)
        {


            var sqlParamers = new List<object>
            {
                dcItem ,gupCode , custCode
            };

            var sql = @"SELECT DISTINCT
							   A.DESTROY_NO,
							   A.DESTROY_DATE,
							   A.DISTR_CAR,
							   A.STATUS,
							   A.POSTING_DATE,
							   A.DC_CODE,
							   A.GUP_CODE,
							   A.CUST_CODE,
							   A.CRT_DATE,
							   A.CRT_NAME,
							   A.MEMO,
							   A.CUST_ORD_NO,
							   A.UPD_DATE,
							   A.UPD_NAME,
							   C.DC_NAME,
							   A.DISTR_CAR_NO,
							   ISNULL (G.STATUS, '0') AS EDI_FLAG
						  FROM F160501 A
							   JOIN F1901 C ON C.DC_CODE = A.DC_CODE
							   LEFT JOIN F050301 D
								  ON     D.DC_CODE = A.DC_CODE
									 AND D.GUP_CODE = A.GUP_CODE
									 AND D.CUST_CODE = A.CUST_CODE
									 AND D.SOURCE_NO = A.DESTROY_NO
							   LEFT JOIN F05030101 E
								  ON     E.DC_CODE = D.DC_CODE
									 AND E.GUP_CODE = D.GUP_CODE
									 AND E.CUST_CODE = D.CUST_CODE
									 AND E.ORD_NO = D.ORD_NO
							   LEFT JOIN F050801 F
								  ON     F.DC_CODE = E.DC_CODE
									 AND F.GUP_CODE = E.GUP_CODE
									 AND F.CUST_CODE = E.CUST_CODE
									 AND F.WMS_ORD_NO = E.WMS_ORD_NO
							   LEFT JOIN F700101 G
								  ON     G.DC_CODE = A.DC_CODE
									 AND G.DISTR_CAR_NO = A.DISTR_CAR_NO
									 AND G.STATUS != '9'
							   LEFT JOIN F050001 J
								  ON     A.DESTROY_NO = J.SOURCE_NO
									 AND A.DC_CODE = J.DC_CODE
									 AND A.GUP_CODE = J.GUP_CODE
									 AND A.CUST_CODE = J.CUST_CODE               
						 WHERE A.DC_CODE = @p0 AND A.GUP_CODE = @p1 AND A.CUST_CODE = @p2					
			";

            //DESTROY_NO
            if (!string.IsNullOrEmpty(destoryNo))
            {
                sql += " AND A.DESTROY_NO = @p" + sqlParamers.Count;
                sqlParamers.Add(destoryNo);
            }

            //過帳日期-起
            if (postingSDate.HasValue)
            {
                sql += " AND A.POSTING_DATE >= @p" + sqlParamers.Count;
                sqlParamers.Add(postingSDate);
            }
            //過帳日期-迄
            if (postingEDate.HasValue)
            {
                postingEDate = postingEDate.Value.AddDays(1);
                sql += " AND A.POSTING_DATE <= @p" + sqlParamers.Count;
                sqlParamers.Add(postingEDate);
            }

            //CUST_ORD_NO
            if (!string.IsNullOrEmpty(custOrdNo))
            {
                sql += " AND A.CUST_ORD_NO = @p" + sqlParamers.Count;
                sqlParamers.Add(custOrdNo);
            }

            //STATUS
            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND A.STATUS = @p" + sqlParamers.Count;
                sqlParamers.Add(status);
            }
            else
            {
                sql += " AND A.STATUS <> '9'";
            }
            //WMS_ORD_NO
            if (!string.IsNullOrEmpty(ordNo))
            {
                sql += " AND F.WMS_ORD_NO = @p" + sqlParamers.Count;
                sqlParamers.Add(ordNo);
            }

            //建立日期-起
            if (crtEDate.HasValue)
            {
                sql += " AND A.CRT_DATE >= @p" + sqlParamers.Count;
                sqlParamers.Add(crtSDate);
            }
            //建立日期-迄
            if (crtEDate.HasValue)
            {
                crtEDate = crtEDate.Value.AddDays(1);
                sql += " AND A.CRT_DATE <= @p" + sqlParamers.Count;
                sqlParamers.Add(crtEDate);
            }

            sql += " order by A.DESTROY_NO ";

            var result = SqlQuery<F160501Data>(sql, sqlParamers.ToArray()).AsQueryable();
            return result;
        }


        public bool UpdateF160501s(F160501 f160501)
        {
            //只要更新主檔，狀態一律改回 : 待確認
            var sqlParamers = new List<SqlParameter>();
            sqlParamers.Add(new SqlParameter("@p0", f160501.DESTROY_NO));
            sqlParamers.Add(new SqlParameter("@p1", f160501.DISTR_CAR));
            sqlParamers.Add(new SqlParameter("@p2", f160501.MEMO));
            sqlParamers.Add(new SqlParameter("@p3", Current.Staff));
            sqlParamers.Add(new SqlParameter("@p4", Current.StaffName));
            sqlParamers.Add(new SqlParameter("@p5", f160501.DC_CODE));
            sqlParamers.Add(new SqlParameter("@p6", f160501.GUP_CODE));
            sqlParamers.Add(new SqlParameter("@p7", f160501.CUST_CODE));
            string sql = @"

				update F160501 set 	DISTR_CAR =@p1,	MEMO =@p2 
					, UPD_DATE=dbo.GetSysDate() ,UPD_STAFF =@p3,UPD_NAME=@p4
				where DESTROY_NO =@p0 
                        AND DC_CODE  = @p5 
                        AND GUP_CODE = @p6
                        AND CUST_CODE= @p7
			";
            ExecuteSqlCommand(sql, sqlParamers.ToArray());
            return true;
        }


        public bool UpdateF160501Status(string destoryNo, string status, string dcCode,string gupCode,string custCode)
        {
            //只要更新主檔，狀態一律改回 : 待確認
            var sqlParamers = new List<SqlParameter>();
            sqlParamers.Add(new SqlParameter("@p0", destoryNo));
            sqlParamers.Add(new SqlParameter("@p1", status));
            sqlParamers.Add(new SqlParameter("@p2", Current.Staff));
            sqlParamers.Add(new SqlParameter("@p3", Current.StaffName));
            sqlParamers.Add(new SqlParameter("@p5", dcCode));
            sqlParamers.Add(new SqlParameter("@p6", gupCode));
            sqlParamers.Add(new SqlParameter("@p7", custCode));
            string sql = @"

				update F160501 set STATUS =@p1
						, UPD_DATE=dbo.GetSysDate() ,UPD_STAFF =@p2,UPD_NAME=@p3
				where DESTROY_NO =@p0 	
                        AND DC_CODE  = @p5 
                        AND GUP_CODE = @p6
                        AND CUST_CODE= @p7
			";
            ExecuteSqlCommand(sql, sqlParamers.ToArray());
            return true;
        }

        public IQueryable<F160501Status> GetF160501Status(string dcCode, string gupCode, string custCode, string destoryNo)
        {
            var parms = new object[] { dcCode, gupCode, custCode, destoryNo };
            string sql = @"			
				Select A.DESTROY_NO ,A.STATUS 
                from F160501 A
				where A.DC_CODE = @p0 AND A.GUP_CODE = @p1 AND A.CUST_CODE = @p2 AND A.DESTROY_NO =@p3
			";
            var result = SqlQuery<F160501Status>(sql, parms).AsQueryable();
            return result;
        }

        public IQueryable<SettleData> GetSettleData(string dcCode, string gupCode, string custCode, DateTime settleDate)
        {
            var parameter = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", settleDate),
                new SqlParameter("@p4", settleDate.AddDays(1))
            };
            var sql = @"SELECT ROW_NUMBER()OVER(ORDER BY TB.DC_CODE,TB.GUP_CODE,TB.CUST_CODE,TB.ITEM_CODE)ROWNUM,TB.* FROM (
							SELECT @p3 CAL_DATE,A.DC_CODE,A.GUP_CODE,A.CUST_CODE,
										 B.ITEM_CODE,SUM (B.DESTROY_QTY) QTY,
										 '01' DELV_ACC_TYPE
								FROM F160501 A
								JOIN F160502 B
									ON A.DC_CODE = B.DC_CODE
								 AND A.GUP_CODE = B.GUP_CODE
								 AND A.CUST_CODE = B.CUST_CODE
								 AND A.DESTROY_NO = B.DESTROY_NO
							 WHERE (A.DC_CODE = @p0 OR @p0 = '000') AND A.GUP_CODE = @p1 AND A.CUST_CODE = @p2
										 AND A.POSTING_DATE >= @p3 AND A.POSTING_DATE < @p4 AND STATUS = '4'
						GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,B.ITEM_CODE) TB";
            return SqlQuery<SettleData>(sql, parameter.ToArray());
        }

        public IQueryable<F160501ItemType> GetF160501ItemType(string dcCode, string gupCode, string custCode, string destoryNo)
        {
            var sqlParamers = new List<SqlParameter>();
            sqlParamers.Add(new SqlParameter("@p0", dcCode));
            sqlParamers.Add(new SqlParameter("@p1", gupCode));
            sqlParamers.Add(new SqlParameter("@p2", custCode));
            sqlParamers.Add(new SqlParameter("@p3", destoryNo));

            string sql = @"			
				SELECT A.DISTR_CAR_NO , B.ITEM_CODE , C.VIRTUAL_TYPE
				FROM F160501 A 
				JOIN F160502 B ON A.DC_CODE = B.DC_CODE AND A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE 
									AND A.DESTROY_NO = B.DESTROY_NO  
				JOIN F1903 C ON C.ITEM_CODE = B.ITEM_CODE AND B.GUP_CODE = A.GUP_CODE AND C.CUST_CODE = B.CUST_CODE
				WHERE 
					A.DC_CODE = @p0
					AND A.GUP_CODE = @p1 
					AND A.CUST_CODE = @p2
					AND A.DESTROY_NO =@p3
			";
            var result = SqlQuery<F160501ItemType>(sql, sqlParamers.ToArray()).AsQueryable();
            return result;
        }

     
    }
}
