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
	public partial class F160204Repository : RepositoryBase<F160204, Wms3plDbContext, F160204Repository>
	{

        public IQueryable<F160204Detail> ConvertToF160204Detail(string dcCode, string gupCode, string custCode,
            string returnNo)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", returnNo)
            };

            var sql = @"SELECT	A.RTN_VNR_NO,C.CUST_ORD_NO,A.ITEM_CODE,A.GUP_CODE,A.DC_CODE,A.CUST_CODE,
								B.ITEM_NAME AS ITEM_NAME,B.ITEM_SIZE AS ITEM_SIZE,
								B.ITEM_SPEC AS ITEM_SPEC,B.ITEM_COLOR AS ITEM_COLOR,
								A.RTN_VNR_QTY AS RTN_VNR_QTY_SUM, 
								A.RTN_WMS_QTY AS RTN_VNR_QTY_GRAND_TOTAL, 
								(A.RTN_VNR_QTY - A.RTN_WMS_QTY) AS RTN_VNR_QTY_REMAINDER,
                A.RTN_VNR_SEQ,
                IsNull(Replicate('0',2 - Len(isnull(A.RTN_VNR_SEQ ,0))), '') + CONVERT(VARCHAR,A.RTN_VNR_SEQ) RTN_VNR_SEQ_SHOW,
                A.MAKE_NO
						FROM F160202 A
            JOIN F1903 B
						  ON A.GUP_CODE = B.GUP_CODE AND
								A.ITEM_CODE = B.ITEM_CODE AND
								A.CUST_CODE = B.CUST_CODE 
						JOIN F160201 C
              ON C.DC_CODE = A.DC_CODE
             AND C.GUP_CODE = A.GUP_CODE
             AND C.CUST_CODE = A.CUST_CODE
             AND C.RTN_VNR_NO = A.RTN_VNR_NO
          WHERE	A.DC_CODE = @p0 AND
								A.GUP_CODE = @p1 AND 
								A.CUST_CODE = @p2 AND
								A.RTN_VNR_NO = @p3
					 ";

            return SqlQuery<F160204Detail>(sql, parameters.ToArray());
        }

        public IQueryable<F160204SearchResult> GetF160204SearchResult(string dcCode, string gupCode, string custCode,
            DateTime createBeginDateTime, DateTime createEndDateTime, string returnWmsNo, string returnVnrNo,
            string orderNo,string empId,string empName,string custOrdNo)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", createBeginDateTime),
                new SqlParameter("@p4", createEndDateTime)
            };

            var sql = $@"SELECT	A.RTN_WMS_NO,A.CRT_DATE,A.CRT_STAFF,A.CRT_NAME,A.RTN_WMS_DATE,
                               (SELECT ORD_PROP_NAME FROM F000903 WHERE ORD_PROP = A.ORD_PROP) AS ORD_PROP_TEXT,
                               A.DELIVERY_WAY,
                               (SELECT NAME FROM VW_F000904_LANG WHERE TOPIC='F160201' AND SUBTOPIC='DELIVERY_WAY' AND VALUE=A.DELIVERY_WAY AND LANG ='{Current.Lang}') DELIVERY_WAY_NAME,
                               A.TYPE_ID,C.TYPE_NAME,B.ADDRESS,B.ITEM_TEL,B.ITEM_CONTACT,A.VNR_CODE,D.VNR_NAME,A.MEMO 
						FROM	F160204 A 
                        JOIN F160201 B
						ON A.DC_CODE = B.DC_CODE 
						AND A.GUP_CODE  = B.GUP_CODE 
						AND A.CUST_CODE = B.CUST_CODE 
						AND A.RTN_VNR_NO = B.RTN_VNR_NO 
            LEFT JOIN F198001 C
              ON C.TYPE_ID = A.TYPE_ID
            JOIN F1908 D 
              ON D.GUP_CODE = A.GUP_CODE
             AND D.CUST_CODE = A.CUST_CODE
             AND D.VNR_CODE = A.VNR_CODE
						WHERE	A.DC_CODE = @p0 AND
								A.GUP_CODE = @p1 AND 
								A.CUST_CODE = @p2 AND
								A.CRT_DATE >= @p3 AND
							    A.CRT_DATE <= @p4 ";

            if (!String.IsNullOrEmpty(returnWmsNo))
            {
                sql += " AND A.RTN_WMS_NO = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, returnWmsNo));
            }

            if (!String.IsNullOrEmpty(returnVnrNo))
            {
                sql += " AND A.RTN_VNR_NO = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, returnVnrNo));
            }

            if (!String.IsNullOrEmpty(orderNo))
            {
                sql += @" AND A.RTN_WMS_NO = (SELECT SOURCE_NO FROM F050301 B 
                                                JOIN F05030101 C
                                                ON B.DC_CODE = C.DC_CODE
                                                AND B.GUP_CODE = C.GUP_CODE
                                                AND B.CUST_CODE = C.CUST_CODE
                                                AND B.ORD_NO = C.ORD_NO
                                                WHERE B.DC_CODE = @p0
                                                AND A.GUP_CODE = @p1
                                                AND A.CUST_CODE = @p2
                                                AND C.WMS_ORD_NO = @p" + parameters.Count + ")";
                //sql += " AND A.RTN_WMS_NO = (SELECT SOURCE_NO FROM F050001 WHERE ORD_NO = @p" + parameters.Count + ")";
                parameters.Add(new SqlParameter("@p" + parameters.Count, orderNo));
            }

            if (!string.IsNullOrEmpty(empId))
            {
                sql += " AND A.CRT_STAFF = @p" + parameters.Count;
                parameters.Add(new SqlParameter("@p" + parameters.Count, empId));
            }
            
            if (!string.IsNullOrEmpty(empName))
            {
                sql += " AND A.CRT_NAME LIKE '%' + @p" + parameters.Count() + " + '%' ";
                //sql += parameters.Combine(" AND A.CRT_NAME LIKE '%'+@p{0}+'%'", parameters.Count);
                //sql += $" AND A.CRT_NAME LIKE '%@p{parameters.Count}%'";
                parameters.Add(new SqlParameter("@p" + parameters.Count, empName));
            }

						if (!string.IsNullOrEmpty(custOrdNo))
						{
				       sql += "AND A.CUST_ORD_NO = @p"+ parameters.Count;
				       parameters.Add(new SqlParameter("@p"+parameters.Count,custOrdNo));
						}

            sql += @" GROUP BY A.RTN_WMS_NO,A.CRT_DATE,A.CRT_STAFF,A.CRT_NAME,A.RTN_WMS_DATE, A.ORD_PROP,A.DELIVERY_WAY,A.TYPE_ID,C.TYPE_NAME,B.ADDRESS,B.ITEM_TEL,B.ITEM_CONTACT,A.VNR_CODE,D.VNR_NAME,A.MEMO";
            sql += @" ORDER BY A.CRT_DATE ASC";

            var result = SqlQuery<F160204SearchResult>(sql, parameters.ToArray()).AsQueryable();
            return result;
        }

        public IQueryable<F160204SearchResult> GetF160204SearchResultDetail(string dcCode, string gupCode, string custCode,
            string returnWmsNo)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", returnWmsNo)
            };

            var sql = @"SELECT	ROW_NUMBER()OVER(ORDER BY A.RTN_WMS_SEQ ASC) ROWNUM,A.RTN_WMS_NO,A.RTN_WMS_SEQ,A.RTN_VNR_NO,A.VNR_CODE,B.VNR_NAME,
								C.ITEM_CODE, C.ITEM_NAME AS ITEM_NAME,C.ITEM_SIZE AS ITEM_SIZE,
								C.ITEM_SPEC AS ITEM_SPEC,C.ITEM_COLOR AS ITEM_COLOR,A.RTN_WMS_QTY,A.CUST_ORD_NO,
								IsNull(Replicate('0',2 - Len(isnull(A.RTN_VNR_SEQ ,0))), '') + CONVERT(VARCHAR,A.RTN_VNR_SEQ) RTN_VNR_SEQ,A.MAKE_NO
						FROM	F160204 A,
								F1908 B,
								F1903 C
						WHERE	A.DC_CODE = @p0 AND
								A.GUP_CODE = @p1 AND 
								A.CUST_CODE = @p2 AND
								A.RTN_WMS_NO = @p3 AND
								A.VNR_CODE = B.VNR_CODE AND
								A.GUP_CODE = B.GUP_CODE AND
                        A.CUST_CODE = C.CUST_CODE AND
                                B.CUST_CODE=
								(SELECT 
								CASE WHEN
								D.ALLOWGUP_VNRSHARE='1'
								THEN '0' 
								ELSE @p2
								END
								FROM F1909 D WHERE D.GUP_CODE = @p1 AND D.CUST_CODE = @p2 ) AND

								A.ITEM_CODE = C.ITEM_CODE AND
								A.GUP_CODE = C.GUP_CODE
								";

            var result = SqlQuery<F160204SearchResult>(sql, parameters.ToArray()).AsQueryable();
            return result;
        }

		    public IQueryable<F160204> GetDatasByRtnWmsNos(string dcCode,string gupCode,string custCode,List<string> rtnWmsNos)
				{
			    var parms = new List<object> { dcCode, gupCode, custCode };
					var sql = @" SELECT *
                        FROM F160204
                        WHERE DC_CODE = @p0
                          AND GUP_CODE = @p1
                          AND CUST_CODE = @p2 ";
			    if(rtnWmsNos.Any())
					{
						sql += parms.CombineNotNullOrEmptySqlInParameters("AND RTN_WMS_NO", rtnWmsNos);
					}
					return SqlQuery<F160204>(sql,parms.ToArray());
			  
				}

        public void UpdateProcFlag(string dcCode, string gupCode, string custCode, string wmsOrdNo, string procFlag)
        {
            var parm = new List<SqlParameter>
            {
                new SqlParameter("@p0", procFlag),
                new SqlParameter("@p1", Current.Staff),
                new SqlParameter("@p2", Current.StaffName),
                new SqlParameter("@p3", dcCode),
                new SqlParameter("@p4", gupCode),
                new SqlParameter("@p5", custCode),
                new SqlParameter("@p6", wmsOrdNo)
            };

            var sql = @"
                        UPDATE X SET PROC_FLAG=@p0, UPD_DATE = dbo.GetSysDate(), UPD_STAFF=@p1, UPD_NAME=@p2 FROM F160204 X
                        WHERE EXISTS(
                        SELECT A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.RTN_WMS_NO, A.RTN_WMS_SEQ 
						FROM F160204 A
						JOIN F050301 B
						ON A.DC_CODE = B.DC_CODE
						AND A.GUP_CODE = B.GUP_CODE
						AND A.CUST_CODE = B.CUST_CODE
						JOIN F05030101 C
						ON B.DC_CODE = C.DC_CODE
						AND B.GUP_CODE = C.GUP_CODE
						AND B.CUST_CODE = C.CUST_CODE
						AND B.ORD_NO = C.ORD_NO
						AND B.SOURCE_NO = A.RTN_WMS_NO
						WHERE X.DC_CODE = A.DC_CODE
						AND X.GUP_CODE = A.GUP_CODE
						AND X.CUST_CODE = A.CUST_CODE
						AND X.RTN_WMS_NO = A.RTN_WMS_NO
						AND X.RTN_WMS_SEQ = A.RTN_WMS_SEQ
						AND C.DC_CODE = @p3
						AND C.GUP_CODE = @p4
						AND C.CUST_CODE = @p5
						AND C.WMS_ORD_NO = @p6
						GROUP BY A.DC_CODE, A.GUP_CODE, A.CUST_CODE, A.RTN_WMS_NO, A.RTN_WMS_SEQ 
                        ) 
                        ";

            ExecuteSqlCommand(sql, parm.ToArray());
        }

		public IQueryable<string> GetCustOrdNosByRtnWmsNo(string dcCode, string gupCode, string custCode, string rtnWmsNo)
		{
			var parameters = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode),
								new SqlParameter("@p3", rtnWmsNo)
						};

			var sql = @"SELECT B.CUST_ORD_NO
									FROM F160204 A
									JOIN F160201 B
									ON A.DC_CODE = B.DC_CODE
									AND A.GUP_CODE = B.GUP_CODE
									AND A.CUST_CODE = B.CUST_CODE
									AND A.RTN_VNR_NO = B.RTN_VNR_NO
									WHERE A.DC_CODE = @p0
									AND A.GUP_CODE = @p1
									AND A.CUST_CODE = @p2
									AND A.RTN_WMS_NO = @p3
									GROUP BY B.CUST_ORD_NO, B.RTN_VNR_NO
					 ";

			return SqlQuery<string>(sql, parameters.ToArray());
		}
	}
}
