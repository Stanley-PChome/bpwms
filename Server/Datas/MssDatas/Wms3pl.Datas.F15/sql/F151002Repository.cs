using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.Shared.ApiEntities;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.Datas.Shared.Pda.Entitues;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F15
{
    public partial class F151002Repository : RepositoryBase<F151002, Wms3plDbContext, F151002Repository>
    {
        public IQueryable<F151002Data> GetF151002Datas(string srcDcCode, string gupCode, string custCode, string allocationNo,
            string userId, bool isAllowStatus2, bool isDiffWareHouse = false)
        {
            var param = new List<SqlParameter>
            {
                new SqlParameter("@p0", srcDcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", userId)
            };
            var allocationSql = "";
            if (!string.IsNullOrEmpty(allocationNo))
            {
                allocationSql = " AND A.ALLOCATION_NO = @p" + param.Count;
                param.Add(new SqlParameter("@p" + param.Count, allocationNo));
            }

            var sql = @" SELECT  ROW_NUMBER ()OVER(ORDER BY A.ALLOCATION_NO,A.SRC_LOC_CODE,A.ITEM_CODE,A.VALID_DATE,A.SERIAL_NO ) ROWNUM,A.*,B.COMBIN_NO FROM ( 
					       SELECT TOP 100 PERCENT A.ALLOCATION_NO,A.ITEM_CODE,C.ITEM_NAME,A.SRC_LOC_CODE,A.SERIAL_NO,A.VALID_DATE, 
								            A.DC_CODE,A.GUP_CODE,A.CUST_CODE,B.SRC_WAREHOUSE_ID,ISNULL(D.WAREHOUSE_NAME,' ') WAREHOUSE_NAME,SRC_QTY,ISNULL(E.STATUS,'1') STATUS,ISNULL(G.ACC_UNIT_NAME,' ') RET_UNIT,B.SRC_MOVE_STAFF,B.SRC_MOVE_NAME, 
								            C.EAN_CODE1,C.EAN_CODE2,C.EAN_CODE3,A.BOX_NO,A.SERIAL_NO AS SERIAL_NOByShow ,
                                            A.MAKE_NO,A.BOX_CTRL_NO,A.PALLET_CTRL_NO
					         FROM 
					   			( SELECT A.ALLOCATION_NO,A.ITEM_CODE,A.SRC_LOC_CODE,A.SERIAL_NO,A.VALID_DATE,A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.BOX_NO,SUM(A.SRC_QTY) SRC_QTY  ,
                                A.MAKE_NO,A.BOX_CTRL_NO,A.PALLET_CTRL_NO
					             FROM F151002 A 
								          GROUP BY A.ALLOCATION_NO,A.SRC_LOC_CODE,A.ITEM_CODE,A.VALID_DATE,A.SERIAL_NO,A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.BOX_NO ,
                                          A.MAKE_NO,A.BOX_CTRL_NO,A.PALLET_CTRL_NO
					         ) A
					        INNER JOIN F151001 B 
					           ON B.ALLOCATION_NO = A.ALLOCATION_NO 
					          AND B.DC_CODE = A.DC_CODE 
					          AND B.GUP_CODE = A.GUP_CODE 
					          AND B.CUST_CODE = A.CUST_CODE ";
            if (!isAllowStatus2)
                sql += "        AND B.STATUS IN('0','1') AND B.LOCK_STATUS = '0' "; //調撥單狀態為「0待處理」或[1已列印調撥單]  
            else
                sql += " AND B.STATUS IN('0','1','2') AND B.LOCK_STATUS IN('0','1') ";  //如果只是更新數量後重查就允許讀取狀態為[2下架處理中]

            if (!isDiffWareHouse)
                sql += "        AND (B.SRC_DC_CODE= B.TAR_DC_CODE OR B.TAR_DC_CODE IS NULL) " + //來源與目的同DC"
                                "        AND (B.SRC_WAREHOUSE_ID	= B.TAR_WAREHOUSE_ID OR B.TAR_WAREHOUSE_ID IS NULL) ";  // 來源與目的倉相同
            else
                sql += "        AND (B.SRC_DC_CODE <> B.TAR_DC_CODE  " +    //來源與目的不DC 或"
                                "         OR B.SRC_WAREHOUSE_ID	<> B.TAR_WAREHOUSE_ID) ";  // 來源與目的倉不相同

            sql += @"      INNER JOIN F1903 C 
													ON C.GUP_CODE = A.GUP_CODE AND C.ITEM_CODE = A.ITEM_CODE AND C.CUST_CODE = A.CUST_CODE
								       LEFT JOIN F1980 D 
								         ON D.DC_CODE = B.SRC_DC_CODE AND D.WAREHOUSE_ID = B.SRC_WAREHOUSE_ID 
								       LEFT JOIN F91000302 G 
								         ON G.ITEM_TYPE_ID ='001' AND G.ACC_UNIT = C.ITEM_UNIT 
								       LEFT JOIN (
								            SELECT DISTINCT E.ALLOCATION_NO,E.SRC_LOC_CODE,E.ITEM_CODE,E.VALID_DATE,E.SERIAL_NO, 
								                      E.DC_CODE,E.GUP_CODE,E.CUST_CODE,E.STATUS ,
                                                      E.MAKE_NO,E.BOX_CTRL_NO,E.PALLET_CTRL_NO
								              FROM F151002 E 
								             WHERE E.STATUS = '0' AND E.SRC_QTY > 0) E
								         ON E.ALLOCATION_NO = A.ALLOCATION_NO 
								        AND E.SRC_LOC_CODE = A.SRC_LOC_CODE 
								        AND E.ITEM_CODE = A.ITEM_CODE 
								        AND E.VALID_DATE = A.VALID_DATE 
								        AND ISNULL(E.SERIAL_NO,' ') = ISNULL(A.SERIAL_NO,' ') 
								        AND E.DC_CODE = A.DC_CODE 
								        AND E.GUP_CODE = A.GUP_CODE 
								        AND E.CUST_CODE = A.CUST_CODE 
                                        AND E.MAKE_NO = A.MAKE_NO
                                        AND E.BOX_CTRL_NO = A.BOX_CTRL_NO
                                        AND E.PALLET_CTRL_NO = A.PALLET_CTRL_NO
								       LEFT JOIN F1903 F 
												  ON F.GUP_CODE = A.GUP_CODE AND F.CUST_CODE = A.CUST_CODE AND F.ITEM_CODE = A.ITEM_CODE 
								      WHERE A.SRC_QTY >0  --//下架數>0 
								        AND B.SRC_DC_CODE  = @p0 
								        AND A.GUP_CODE = @p1 
								        AND A.CUST_CODE = @p2 
												AND D.DEVICE_TYPE  = '0'" +
                                allocationSql +
                                @"        AND NOT EXISTS (   -- //必須為使用者可用儲位權限
								            SELECT 1  
								              FROM F151002 F 
															              INNER JOIN F151001 M  
															               ON F.ALLOCATION_NO = M.ALLOCATION_NO 
															               AND F.DC_CODE = M.DC_CODE 
															               AND F.GUP_CODE = M.GUP_CODE 
															               AND F.CUST_CODE = M.CUST_CODE 
								              LEFT JOIN F1912 G ON G.LOC_CODE = F.SRC_LOC_CODE AND G.DC_CODE = M.SRC_DC_CODE 
								              LEFT JOIN F196301 H ON H.LOC_CODE =G.LOC_CODE 
								              LEFT JOIN F1963 I ON I.WORK_ID = H.WORK_ID 
								              LEFT JOIN F192403 J ON J.WORK_ID = I.WORK_ID 
								              LEFT JOIN F1924 K ON K.EMP_ID = J.EMP_ID 
								             WHERE  H.LOC_CODE IS NULL AND K.EMP_ID=@p3 
								               AND F.DC_CODE = A.DC_CODE 
								               AND F.GUP_CODE =A.GUP_CODE 
								               AND F.CUST_CODE=A.CUST_CODE 
								               AND F.ALLOCATION_NO = A.ALLOCATION_NO 
								        )  
								     ORDER BY  A.ALLOCATION_NO,A.SRC_LOC_CODE,A.ITEM_CODE,A.VALID_DATE,A.SERIAL_NO 
								 ) A 
					   LEFT JOIN F2501 B 
								   ON A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND A.SERIAL_NO = B.SERIAL_NO AND B.COMBIN_NO IS NOT NULL AND B.BOUNDLE_ITEM_CODE IS NULL ";
            var result = SqlQuery<F151002Data>(sql, param.ToArray()).AsQueryable();
            return result;
        }

        public IQueryable<F151002ItemLocData> GetF151002ItemLocDatas(string dcCode, string gupCode, string custCode,
            string allocationNo, string itemCode, bool isDiffWareHouse = false)
        {
            var param = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", allocationNo),
                new SqlParameter("@p4", itemCode)
            };


            var sql = "					SELECT ROW_NUMBER() OVER(ORDER BY A.ALLOCATION_NO,A.SRC_LOC_CODE,ISNULL(A.SRC_VALID_DATE,A.VALID_DATE),ISNULL(A.SRC_MAKE_NO,A.MAKE_NO)) ROWNUM,A.ALLOCATION_NO,B.SRC_WAREHOUSE_ID,ISNULL(C.WAREHOUSE_NAME,' ') WAREHOUSE_NAME,A.ITEM_CODE,ISNULL(A.SRC_VALID_DATE,A.VALID_DATE) SRC_VALID_DATE, " +
                                "								 A.SRC_LOC_CODE,A.DC_CODE,A.GUP_CODE,A.CUST_CODE,SUM(A.A_SRC_QTY) A_SRC_QTY ,ISNULL(A.SRC_MAKE_NO,A.MAKE_NO) SRC_MAKE_NO   " +
                                "						FROM F151002 A " +
                                "          INNER JOIN F151001 B " +
                                "             ON B.ALLOCATION_NO = A.ALLOCATION_NO " +
                                "            AND B.DC_CODE = A.DC_CODE " +
                                "            AND B.GUP_CODE = A.GUP_CODE " +
                                "            AND B.CUST_CODE = A.CUST_CODE " +
                                "            AND B.STATUS IN('0','2') "; //調撥單狀態為「0待處理」或[2下架處理中]

            if (!isDiffWareHouse)
                sql += "        AND (B.SRC_DC_CODE = B.TAR_DC_CODE OR B.TAR_DC_CODE IS NULL) " + //來源與目的同DC"
                                "        AND (B.SRC_WAREHOUSE_ID	=B.TAR_WAREHOUSE_ID OR B.TAR_WAREHOUSE_ID IS NULL) ";  // 來源與目的倉相同
            else
                sql += "             AND (B.SRC_DC_CODE <> B.TAR_DC_CODE  " +    //來源與目的不DC 或"
                                "             OR B.SRC_WAREHOUSE_ID	<> B.TAR_WAREHOUSE_ID) ";  // 來源與目的倉不相同

            sql += "           LEFT JOIN F1980 C " +
                                    "             ON C.DC_CODE = B.DC_CODE AND C.WAREHOUSE_ID = B.SRC_WAREHOUSE_ID " +
                                    "					 WHERE A.A_SRC_QTY >0 " +
                                    "						 AND A.DC_CODE = @p0 " +
                                    "					   AND A.GUP_CODE = @p1 " +
                                    "						 AND A.CUST_CODE = @p2 " +
                                    "						 AND A.ALLOCATION_NO= @p3 " +
                                    "						 AND A.ITEM_CODE = @p4 " +
                                    "					 GROUP BY A.ALLOCATION_NO,A.ITEM_CODE,ISNULL(A.SRC_VALID_DATE,A.VALID_DATE),A.SRC_LOC_CODE, " +
                                    "					          A.DC_CODE,A.GUP_CODE,A.CUST_CODE,B.SRC_WAREHOUSE_ID,ISNULL(C.WAREHOUSE_NAME,' '),ISNULL(A.SRC_MAKE_NO,A.MAKE_NO)   " +
                                    "					 ORDER BY A.ALLOCATION_NO,A.SRC_LOC_CODE,ISNULL(A.SRC_VALID_DATE,A.VALID_DATE),ISNULL(A.SRC_MAKE_NO,A.MAKE_NO) ";

            var result = SqlQuery<F151002ItemLocData>(sql, param.ToArray()).AsQueryable();
            return result;
        }

        public IQueryable<F151002DataByTar> GetF151002DataByTars(string tarDcCode, string gupCode, string custCode, string allocationNo,
            string userId, bool isAllowStatus4, bool isDiffWareHouse = false)
        {
            var param = new List<SqlParameter>
            {
                new SqlParameter("@p0", tarDcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", userId)
            };
            var allocationSql = "";
            if (!string.IsNullOrEmpty(allocationNo))
            {
                allocationSql = " AND A.ALLOCATION_NO = @p" + param.Count;
                param.Add(new SqlParameter("@p" + param.Count, allocationNo));
            }

            var sql = @" SELECT ROW_NUMBER ()OVER(ORDER BY A.ALLOCATION_NO,A.SUG_LOC_CODE,A.ITEM_CODE,A.VALID_DATE,A.SERIAL_NO ) ROWNUM,A.*,B.COMBIN_NO FROM (  
					       SELECT top 100 percent A.ALLOCATION_NO,A.ITEM_CODE,C.ITEM_NAME,A.SUG_LOC_CODE,A.SERIAL_NO,A.VALID_DATE,  
					              A.DC_CODE,A.GUP_CODE,A.CUST_CODE,B.TAR_WAREHOUSE_ID,ISNULL(D.WAREHOUSE_NAME,' ') WAREHOUSE_NAME,TAR_QTY,ISNULL(E.STATUS,'2') STATUS,ISNULL(G.ACC_UNIT_NAME,' ') RET_UNIT,B.TAR_MOVE_STAFF,B.TAR_MOVE_NAME, 
								            C.EAN_CODE1,C.EAN_CODE2,C.EAN_CODE3,A.BOX_NO,A.SERIAL_NO AS SERIAL_NOByShow ,
                                  A.MAKE_NO ,A.BOX_CTRL_NO,A.PALLET_CTRL_NO
					         FROM 
												( SELECT A.ALLOCATION_NO,A.ITEM_CODE,A.SUG_LOC_CODE,A.SERIAL_NO,ISNULL(A.SRC_VALID_DATE,A.VALID_DATE) VALID_DATE,A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.BOX_NO,SUM(A.TAR_QTY) TAR_QTY ,
                                                 ISNULL(A.SRC_MAKE_NO,A.MAKE_NO) MAKE_NO ,A.BOX_CTRL_NO,A.PALLET_CTRL_NO
					             FROM F151002 A 
								         GROUP BY A.ALLOCATION_NO,A.SUG_LOC_CODE,A.ITEM_CODE,ISNULL(A.SRC_VALID_DATE,A.VALID_DATE),A.SERIAL_NO,A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.BOX_NO ,ISNULL(A.SRC_MAKE_NO,A.MAKE_NO),A.BOX_CTRL_NO,A.PALLET_CTRL_NO
					         ) A
					        INNER JOIN F151001 B  
					           ON B.ALLOCATION_NO = A.ALLOCATION_NO  
					          AND B.DC_CODE = A.DC_CODE  
					          AND B.GUP_CODE = A.GUP_CODE  
					          AND B.CUST_CODE = A.CUST_CODE ";
            if (!isAllowStatus4)
                sql += "        AND B.STATUS ='3' AND B.LOCK_STATUS ='2' ";//調撥單狀態為「3已下架處理」
            else
                sql += "        AND B.STATUS IN('3','4') AND B.LOCK_STATUS IN('2','3') ";//調撥單狀態為「3已下架處理」或[4上架處理中]

            if (!isDiffWareHouse)
                sql += "        AND (B.SRC_DC_CODE = B.TAR_DC_CODE OR B.SRC_DC_CODE IS NULL) " + //來源與目的同DC"
                                "        AND (B.SRC_WAREHOUSE_ID	=B.TAR_WAREHOUSE_ID OR B.SRC_WAREHOUSE_ID IS NULL) ";  // 來源與目的倉相同
            else
                sql += "        AND (B.SRC_DC_CODE <> B.TAR_DC_CODE  " +    //來源與目的不DC 或"
                                "         OR B.SRC_WAREHOUSE_ID	<> B.TAR_WAREHOUSE_ID) ";  // 來源與目的倉不相同

            sql += @"      INNER JOIN F1903 C 
												ON C.GUP_CODE = A.GUP_CODE AND C.ITEM_CODE = A.ITEM_CODE AND C.CUST_CODE = A.CUST_CODE 
							       LEFT JOIN F1980 D  
							         ON D.DC_CODE = B.TAR_DC_CODE AND D.WAREHOUSE_ID = B.TAR_WAREHOUSE_ID 
							       LEFT JOIN F91000302 G 
							         ON G.ITEM_TYPE_ID ='001' AND G.ACC_UNIT = C.ITEM_UNIT 
							       LEFT JOIN (
							            SELECT DISTINCT E.ALLOCATION_NO,E.SUG_LOC_CODE,E.ITEM_CODE,ISNULL(E.SRC_VALID_DATE,E.VALID_DATE) VALID_DATE,E.SERIAL_NO, 
                                                  E.DC_CODE,E.GUP_CODE,E.CUST_CODE,E.STATUS ,ISNULL(E.SRC_MAKE_NO,E.MAKE_NO) MAKE_NO,E.BOX_CTRL_NO,E.PALLET_CTRL_NO  
							              FROM F151002 E  
							             WHERE E.STATUS = '1' AND E.TAR_QTY>0) E 
							         ON E.ALLOCATION_NO = A.ALLOCATION_NO  
							        AND E.SUG_LOC_CODE = A.SUG_LOC_CODE 
							        AND E.ITEM_CODE = A.ITEM_CODE 
							        AND E.VALID_DATE = A.VALID_DATE 
							        AND ISNULL(E.SERIAL_NO,' ') = ISNULL(A.SERIAL_NO,' ') 
							        AND E.DC_CODE = A.DC_CODE 
							        AND E.GUP_CODE = A.GUP_CODE 
							        AND E.CUST_CODE = A.CUST_CODE 
                                     AND E.MAKE_NO = A.MAKE_NO
                                     AND E.BOX_CTRL_NO = A.BOX_CTRL_NO
                                     AND E.PALLET_CTRL_NO = A.PALLET_CTRL_NO

                                   LEFT JOIN F1903 F 
											  ON F.GUP_CODE = A.GUP_CODE AND F.CUST_CODE = A.CUST_CODE AND F.ITEM_CODE = A.ITEM_CODE 
							      WHERE A.TAR_QTY >0 " + //上架數>0 
                            "        AND B.TAR_DC_CODE  = @p0 " +
                            "        AND A.GUP_CODE = @p1 " +
                            "        AND A.CUST_CODE = @p2 " +
                            allocationSql +
                            "        AND NOT EXISTS (   " + //必須為使用者可用儲位權限
                            @"            SELECT 1  
							              FROM F151002 F 
															              INNER JOIN F151001 M  
															               ON F.ALLOCATION_NO = M.ALLOCATION_NO 
															               AND F.DC_CODE = M.DC_CODE 
															               AND F.GUP_CODE = M.GUP_CODE 
															               AND F.CUST_CODE = M.CUST_CODE 
															              LEFT JOIN F1912 G ON G.LOC_CODE = F.SUG_LOC_CODE AND G.DC_CODE = M.TAR_DC_CODE 
							              LEFT JOIN F196301 H ON H.LOC_CODE =G.LOC_CODE
							              LEFT JOIN F1963 I ON I.WORK_ID = H.WORK_ID 
							              LEFT JOIN F192403 J ON J.WORK_ID = I.WORK_ID 
							              LEFT JOIN F1924 K ON K.EMP_ID = J.EMP_ID 
							             WHERE  H.LOC_CODE IS NULL AND K.EMP_ID=@p3 
							               AND F.DC_CODE = A.DC_CODE 
							               AND F.GUP_CODE =A.GUP_CODE 
							               AND F.CUST_CODE=A.CUST_CODE 
							               AND F.ALLOCATION_NO = A.ALLOCATION_NO 
							        ) 
							     ORDER BY  A.ALLOCATION_NO,A.SUG_LOC_CODE,A.ITEM_CODE,A.VALID_DATE,A.SERIAL_NO 
							 ) A 
							 LEFT JOIN F2501 B 
							   ON A.GUP_CODE = B.GUP_CODE AND A.CUST_CODE = B.CUST_CODE AND A.SERIAL_NO = B.SERIAL_NO AND B.COMBIN_NO IS NOT NULL AND B.BOUNDLE_ITEM_CODE IS NULL ";

            var result = SqlQuery<F151002DataByTar>(sql, param.ToArray()).AsQueryable();
            return result;
        }

        public IQueryable<F151002ItemLocDataByTar> GetF151002ItemLocDataByTars(string dcCode, string gupCode, string custCode,
            string allocationNo, string itemCode, bool isDiffWareHouse = false, string stickerPalletNo = "")
        {
            var param = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", allocationNo),
                new SqlParameter("@p4", itemCode)
            };
            var sql2 = string.Empty;
            if (!string.IsNullOrWhiteSpace(stickerPalletNo))
            {
                sql2 += " AND A.STICKER_PALLET_NO = @p" + param.Count;
                param.Add(new SqlParameter("@p" + param.Count, stickerPalletNo));
            }


            var sql = " SELECT ROW_NUMBER ()OVER(ORDER BY A.ALLOCATION_NO,A.TAR_LOC_CODE,ISNULL(A.TAR_VALID_DATE,ISNULL(A.SRC_VALID_DATE,A.VALID_DATE)) ,ISNULL(A.TAR_MAKE_NO,ISNULL(A.SRC_MAKE_NO,A.MAKE_NO)) ) ROWNUM,A.ALLOCATION_NO,B.TAR_WAREHOUSE_ID,ISNULL(C.WAREHOUSE_NAME,' ') WAREHOUSE_NAME,A.ITEM_CODE,ISNULL(A.TAR_VALID_DATE,ISNULL(A.SRC_VALID_DATE,A.VALID_DATE)) TAR_VALID_DATE, " +
                                "								A.TAR_LOC_CODE,A.DC_CODE,A.GUP_CODE,A.CUST_CODE,SUM(A.A_TAR_QTY) A_TAR_QTY ,ISNULL(A.TAR_MAKE_NO,ISNULL(A.SRC_MAKE_NO,A.MAKE_NO)) TAR_MAKE_NO  " +
                                "						FROM F151002 A " +
                                "          INNER JOIN F151001 B " +
                                "             ON B.ALLOCATION_NO = A.ALLOCATION_NO " +
                                "            AND B.DC_CODE = A.DC_CODE " +
                                "            AND B.GUP_CODE = A.GUP_CODE " +
                                "            AND B.CUST_CODE = A.CUST_CODE " +
                                "            AND B.STATUS IN('3','4') "; //調撥單狀態為「3已下架處理」或[4上架處理中]

            if (!isDiffWareHouse)
                sql += "        AND (B.SRC_DC_CODE = B.TAR_DC_CODE OR B.SRC_DC_CODE IS NULL) " + //來源與目的同DC"
                                "        AND (B.SRC_WAREHOUSE_ID	=B.TAR_WAREHOUSE_ID OR B.SRC_WAREHOUSE_ID IS NULL) ";  // 來源與目的倉相同
            else
                sql += "            AND (B.SRC_DC_CODE <> B.TAR_DC_CODE  " +    //來源與目的不DC 或"
                                "             OR B.SRC_WAREHOUSE_ID	<> B.TAR_WAREHOUSE_ID) ";  // 來源與目的倉不相同

            sql += "           LEFT JOIN F1980 C " +
                                "             ON C.DC_CODE = B.TAR_DC_CODE AND C.WAREHOUSE_ID = B.TAR_WAREHOUSE_ID " +
                                "					 WHERE A.A_TAR_QTY >0 " +
                                "						 AND A.DC_CODE = @p0 " +
                                "					   AND A.GUP_CODE = @p1 " +
                                "						 AND A.CUST_CODE = @p2 " +
                                "						 AND A.ALLOCATION_NO= @p3 " +
                                "						 AND A.ITEM_CODE = @p4 " +
                                sql2 +
                                "					 GROUP BY A.ALLOCATION_NO,A.ITEM_CODE,ISNULL(A.TAR_VALID_DATE,ISNULL(A.SRC_VALID_DATE,A.VALID_DATE)),A.TAR_LOC_CODE, " +
                                "					          A.DC_CODE,A.GUP_CODE,A.CUST_CODE,B.TAR_WAREHOUSE_ID,ISNULL(C.WAREHOUSE_NAME,' ') ,ISNULL(A.TAR_MAKE_NO,ISNULL(A.SRC_MAKE_NO,A.MAKE_NO))  " +
                                "					 ORDER BY A.ALLOCATION_NO,A.TAR_LOC_CODE,ISNULL(A.TAR_VALID_DATE,ISNULL(A.SRC_VALID_DATE,A.VALID_DATE)) ,ISNULL(A.TAR_MAKE_NO,ISNULL(A.SRC_MAKE_NO,A.MAKE_NO))";

            var result = SqlQuery<F151002ItemLocDataByTar>(sql, param.ToArray()).AsQueryable();
            return result;
        }

        public void DeleteDatas(string ordNo, string gupCode, string custCode, string dcCode)
        {
            var parameters = new List<object>
            {
                ordNo,
                gupCode,
                custCode,
                dcCode
            };

            var sql = @"
				Delete From F151002 
				 Where ALLOCATION_NO=@p0
					 And GUP_CODE = @p1
					 And CUST_CODE = @p2
					 And DC_CODE = @p3";

            ExecuteSqlCommand(sql, parameters.ToArray());
        }

        public void UpdateData(string dcCode, string gupCode, string custCode, string allocationNo, int allocationSeq, long srcQty, long aSrcQty, long tarQty, long aTarQty, string status, string userId, string userName, bool isSrc, string stickerPalletNo = "")
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", allocationNo),
                new SqlParameter("@p4", allocationSeq),
                new SqlParameter("@p5", srcQty),
                new SqlParameter("@p6",aSrcQty),
                new SqlParameter("@p7",tarQty),
                new SqlParameter("@p8",aTarQty),
                new SqlParameter("@p9",status),
                new SqlParameter("@p10",userId),
                new SqlParameter("@p11",userName),
                new SqlParameter("@p12",stickerPalletNo),
                new SqlParameter("@p13", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };
            var addsql = "";
            if (isSrc)
                addsql = aSrcQty == 0 ? " ,SRC_STAFF = null ,SRC_NAME = null,SRC_DATE = null " : " ,SRC_STAFF = @p10,SRC_NAME=@p11,SRC_DATE= @p13,STICKER_PALLET_NO=@p12 ";
            else
                addsql = aTarQty == 0 ? " ,TAR_STAFF = null ,TAR_NAME = null,TAR_DATE = null " : " ,TAR_STAFF = @p10,TAR_NAME=@p11,TAR_DATE= @p13,STICKER_PALLET_NO=@p12 ";

            var sql =
                " UPDATE F151002 SET SRC_QTY = @p5, " +
                "									 A_SRC_QTY= @p6, " +
                "									 TAR_QTY = @p7, " +
                "									 A_TAR_QTY = @p8, " +
                "									 STATUS = @p9, " +
                "									 UPD_STAFF = @p10 , " +
                "									 UPD_NAME = @p11 ," +
                "									 UPD_DATE = @p13 " +
                addsql +
                "  WHERE DC_CODE = @p0 " +
                "    AND GUP_CODE = @p1 " +
                "    AND CUST_CODE = @p2 " +
                "    AND ALLOCATION_NO = @p3 " +
                "    AND ALLOCATION_SEQ = @p4 ";

            ExecuteSqlCommand(sql, parameters.ToArray());
        }


        public void DeleteData(string dcCode, string gupCode, string custCode, string allocationNo, int allocationSeq)
        {
            var param = new List<SqlParameter>
            {
                new SqlParameter("@p0", dcCode),
                new SqlParameter("@p1", gupCode),
                new SqlParameter("@p2", custCode),
                new SqlParameter("@p3", allocationNo),
                new SqlParameter("@p4", allocationSeq)
            };
            var sql = " DELETE FROM F151002 " +
                                "  WHERE DC_CODE = @p0 " +
                                "    AND GUP_CODE = @p1 " +
                                "    AND CUST_CODE = @p2 " +
                                "    AND ALLOCATION_NO = @p3 " +
                                "    AND ALLOCATION_SEQ = @p4 ";
            ExecuteSqlCommand(sql, param.ToArray());
        }

        public IQueryable<F151002ItemData> GetF151002ItemQty(string dcCode, string gupCode, string custCode, string allocationNo, string itemCode, string locCodeS)
        {
            string sql = @"
            	SELECT A.ALLOCATION_NO,A.ITEM_CODE ,A.SRC_LOC_CODE                         
                                 , SUM(ISNULL(A.SRC_QTY,0)) SRC_QTY
                                 , SUM(ISNULL(A.A_SRC_QTY,0)) A_SRC_QTY
                                 , SUM(ISNULL(A.TAR_QTY,0)) TAR_QTY
                                 , SUM(ISNULL(A.A_TAR_QTY,0)) A_TAR_QTY
                FROM F151002 A					
            	WHERE A.ALLOCATION_NO = @p0
            		    AND A.DC_CODE = @p1
            		    AND A.GUP_CODE = @p2
            		    AND A.CUST_CODE = @p3
                                 AND A.SRC_LOC_CODE = @p4
                                 AND A.ITEM_CODE = @p5
                GROUP BY A.ALLOCATION_NO,A.ITEM_CODE ,A.SRC_LOC_CODE 
            ";
            var param = new[] {
                         new SqlParameter("@p0", allocationNo),
                         new SqlParameter("@p1", dcCode),
                         new SqlParameter("@p2", gupCode),
                 new SqlParameter("@p3", custCode),
                         new SqlParameter("@p4", locCodeS),
                 new SqlParameter("@p5", itemCode)
                     };

            var result = SqlQuery<F151002ItemData>(sql, param);
            return result;
        }

        public IQueryable<F151002ItemData> GetF151002ItemQtyByExpendData(string dcCode, string gupCode, string custCode, string allocationNo, string itemCode, string locCodeS)
        {
            string sql = @"
				SELECT A.ALLOCATION_NO,A.ITEM_CODE ,A.SRC_LOC_CODE,VALID_DATE,ENTER_DATE                         
                        , SUM(ISNULL(A.SRC_QTY,0)) SRC_QTY
                        , SUM(ISNULL(A.A_SRC_QTY,0)) A_SRC_QTY
                        , SUM(ISNULL(A.TAR_QTY,0)) TAR_QTY
                        , SUM(ISNULL(A.A_TAR_QTY,0)) A_TAR_QTY
			    FROM F151002 A					
				WHERE A.ALLOCATION_NO = @p0
					    AND A.DC_CODE = @p1
					    AND A.GUP_CODE = @p2
					    AND A.CUST_CODE = @p3
                        AND A.SRC_LOC_CODE = @p4
                        AND A.ITEM_CODE = @p5
       GROUP BY A.ALLOCATION_NO,A.ITEM_CODE ,A.SRC_LOC_CODE,VALID_DATE,ENTER_DATE 
			";
            var param = new[] {
                new SqlParameter("@p0", allocationNo),
                new SqlParameter("@p1", dcCode),
                new SqlParameter("@p2", gupCode),
        new SqlParameter("@p3", custCode),
                new SqlParameter("@p4", locCodeS),
        new SqlParameter("@p5", itemCode)
            };

            var result = SqlQuery<F151002ItemData>(sql, param);
            return result;
        }

        public void BulkDeleteData(string gupCode, string custCode, string dcCode, List<string> allocationNos)
        {
            var parameters = new List<object>
            {
                gupCode,
                custCode,
                dcCode
            };

            var sql = @"
				Delete From F151002 
				 Where  GUP_CODE = @p0
					 And CUST_CODE = @p1
					 And DC_CODE = @p2";
            sql += parameters.CombineSqlInParameters(" AND ALLOCATION_NO ", allocationNos);
            ExecuteSqlCommand(sql, parameters.ToArray());
        }

        /// <summary>
        /// 調撥明細查詢
        /// </summary>
        /// <param name="dcNo">物流中心編號</param>
        /// <param name="custNo">貨主編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="allocNo">調撥單號</param>
        /// <param name="allocType">單據類別</param>
        /// <returns></returns>
        public IQueryable<GetAllocDetailForData> GetAllocDetail(string dcNo, string custNo, string gupCode, string allocNo, string allocType)
        {
            var parm = new List<SqlParameter>();
            parm.Add(new SqlParameter("@p0", dcNo));
            parm.Add(new SqlParameter("@p1", custNo));
            parm.Add(new SqlParameter("@p2", gupCode));
            parm.Add(new SqlParameter("@p3", allocNo));
            parm.Add(new SqlParameter("@p4", allocType));

            #region 條件過濾
            string conditionA = string.Empty;
            string conditionB = string.Empty;

            if (allocType.Equals("01") || allocType.Equals("03"))
            {
                conditionA += " AND A.STATUS = '1' ";
            }
            else if (allocType.Equals("02"))
            {
                conditionA += " AND A.STATUS = '0' ";
            }

            #endregion

            string sql = $@"SELECT 
                        	A.ALLOCATION_NO AS AllocNo,
                        	A.ALLOCATION_SEQ AS AllocSeq,
                        	A.ITEM_CODE AS ItemNo,
                        	(CASE WHEN @p4='01' OR @p4='03' THEN 
                            (SELECT WAREHOUSE_NAME FROM F1980 WHERE WAREHOUSE_ID = B.TAR_WAREHOUSE_ID AND DC_CODE = B.DC_CODE ) 
                            WHEN @p4='02' THEN 
                            (SELECT WAREHOUSE_NAME FROM F1980 WHERE WAREHOUSE_ID = B.SRC_WAREHOUSE_ID AND DC_CODE = B.DC_CODE )  END
                            )AS WhName,
                        	(CASE WHEN @p4='01' OR @p4='03' THEN A.SUG_LOC_CODE WHEN @p4='02' THEN A.SRC_LOC_CODE END )AS SugLoc,
                        	A.VALID_DATE AS ValidDate,
                        	A.ENTER_DATE AS EnterDate,
                        	A.MAKE_NO AS MkNo,
                        	A.SERIAL_NO AS Sn,
                        	(CASE WHEN @p4='01' OR @p4='03' THEN A.TAR_QTY WHEN @p4='02' THEN A.SRC_QTY END )AS Qty,
                        	(CASE WHEN @p4='01' OR @p4='03' THEN A.A_TAR_QTY WHEN @p4='02' THEN A.A_SRC_QTY END )AS ActQty,
                        	A.PALLET_CTRL_NO AS PalletNo,
													C.EAN_CODE1 AS EanCode1
                        FROM F151002 A
                        JOIN F151001 B ON A.ALLOCATION_NO = B.ALLOCATION_NO 
                        				   AND A.DC_CODE = B.DC_CODE 
                        				   AND A.GUP_CODE = B.GUP_CODE 
                        				   AND A.CUST_CODE = B.CUST_CODE 
												JOIN F1903 C ON A.GUP_CODE = C.GUP_CODE
																		AND A.CUST_CODE = C.CUST_CODE
																		AND A.ITEM_CODE = C.ITEM_CODE
                        WHERE A.DC_CODE = @p0
                          AND A.CUST_CODE = @p1
                          AND A.GUP_CODE = @p2
                          AND A.ALLOCATION_NO = @p3 
                        {conditionA}
                        ORDER BY A.ALLOCATION_SEQ ";
            var result = SqlQuery<GetAllocDetailForData>(sql, parm.ToArray());
            return result;
        }
        /// <summary>
        /// 調撥明細查詢(全欄位)
        /// </summary>
        /// <param name="dcNo">物流中心編號</param>
        /// <param name="custNo">貨主編號</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="allocNo">調撥單號</param>
        /// <param name="allocType">單據類別</param>
        /// <returns></returns>
        public IQueryable<GetAllocDetail> GetAllocDetailAllCol(string dcNo, string custNo, string gupCode, string allocNo, string allocType)
        {
            var parm = new List<SqlParameter>();
            parm.Add(new SqlParameter("@p0", dcNo));
            parm.Add(new SqlParameter("@p1", custNo));
            parm.Add(new SqlParameter("@p2", gupCode));
            parm.Add(new SqlParameter("@p3", allocNo));
            parm.Add(new SqlParameter("@p4", allocType));

            string sql = $@"SELECT 
                        	A.ALLOCATION_NO AllocNo,
                        	CONVERT(VARCHAR, A.ALLOCATION_SEQ) AllocSeq,
                        	A.ITEM_CODE ItemNo,
                        	E.WAREHOUSE_NAME WhName,
                        	{(allocType == "02" ? " A.SRC_LOC_CODE " : " A.SUG_LOC_CODE ")} SugLoc,
                        	A.VALID_DATE ValidDate,
                        	A.ENTER_DATE EnterDate,
                        	A.MAKE_NO MkNo,
                        	A.SERIAL_NO Sn,
                        	{(allocType == "02" ? " A.SRC_QTY " : " A.TAR_QTY ")} Qty,
                        	{(allocType == "02" ? " A.A_SRC_QTY " : " A.A_TAR_QTY ")} ActQty,
                        	A.PALLET_CTRL_NO PalletNo,
							C.EAN_CODE1 EanCode1,
                            C.ITEM_UNIT Unit,
							C.ITEM_NAME ProductName,
							C.ITEM_SIZE ProductSize,
							C.ITEM_COLOR ProductColor,
							C.ITEM_SPEC ProductSpec,
							C.EAN_CODE1 Barcode1,
							C.EAN_CODE2 Barcode2,
							C.EAN_CODE3 Barcode3,
                            D.PACK_WEIGHT Weight,
                            C.CTNS BoxQty,
                            A.CUST_CODE CustNo
                        FROM F151002 A
                        JOIN F151001 B ON A.ALLOCATION_NO = B.ALLOCATION_NO 
                        			  AND A.DC_CODE = B.DC_CODE 
                        			  AND A.GUP_CODE = B.GUP_CODE 
                        			  AND A.CUST_CODE = B.CUST_CODE 
						LEFT JOIN F1903 C ON A.GUP_CODE = C.GUP_CODE
						                 AND A.CUST_CODE = C.CUST_CODE
						                 AND A.ITEM_CODE = C.ITEM_CODE
						LEFT JOIN F1905 D ON A.GUP_CODE = D.GUP_CODE
						                 AND A.CUST_CODE = D.CUST_CODE
						                 AND A.ITEM_CODE = D.ITEM_CODE
                        {(allocType == "02" ? @" JOIN F1980 E ON B.SRC_WAREHOUSE_ID = E.WAREHOUSE_ID
                                                             AND B.DC_CODE = E.DC_CODE "
                                            : @" JOIN F1980 E ON B.TAR_WAREHOUSE_ID = E.WAREHOUSE_ID
                                                             AND B.DC_CODE = E.DC_CODE ")}
                        WHERE A.DC_CODE = @p0
                          AND A.CUST_CODE = @p1
                          AND A.GUP_CODE = @p2
                          AND A.ALLOCATION_NO = @p3 
                        {(allocType == "02" ? " AND A.STATUS = '0' " : " AND A.STATUS = '1' ")}
                        ORDER BY A.ALLOCATION_SEQ ";
            var result = SqlQuery<GetAllocDetail>(sql, parm.ToArray());
            return result;
        }
        public IQueryable<WcsOutboundSkuModel> GetWcsDetail(string dcCode, string gupCode, string custCode, string allocNo)
        {
            var parm = new List<SqlParameter>();
            parm.Add(new SqlParameter("@p0", dcCode));
            parm.Add(new SqlParameter("@p1", gupCode));
            parm.Add(new SqlParameter("@p2", custCode));
            parm.Add(new SqlParameter("@p3", allocNo));

            string sql = $@"SELECT
                            A.ALLOCATION_SEQ RowNum,
                            A.ITEM_CODE SkuCode,
                            A.SRC_QTY SkuQty,
                            1 SkuLevel,
                            convert(varchar, A.VALID_DATE, 111) ExpiryDate,
                            A.MAKE_NO OutBatchCode
                            FROM F151002 A
                            JOIN F1903 B
                            ON A.ITEM_CODE = B.ITEM_CODE
                            AND A.GUP_CODE = B.GUP_CODE
                            AND A.CUST_CODE = B.CUST_CODE
                            WHERE A.DC_CODE = @p0
                            AND A.GUP_CODE = @p1
                            AND A.CUST_CODE = @p2
                            AND A.ALLOCATION_NO = @p3
                            ";
            var result = SqlQuery<WcsOutboundSkuModel>(sql, parm.ToArray());
            return result;
        }

        /// <summary>
        /// 取得捕貨調撥明細
        /// </summary>
        /// <param name="dcCode"></param>
        /// <param name="gupCode"></param>
        /// <param name="custCode"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        public IQueryable<AllocDetailByReplenish> GetDataByReplenish(string dcCode, string gupCode, string custCode, string itemCode)
        {
            var parm = new List<SqlParameter>();
            parm.Add(new SqlParameter("@p0", dcCode));
            parm.Add(new SqlParameter("@p1", gupCode));
            parm.Add(new SqlParameter("@p2", custCode));
            parm.Add(new SqlParameter("@p3", itemCode));

            string sql = $@"SELECT B.ALLOCATION_NO AllocNo, 
                            	   B.ITEM_CODE ItemCode, 
                            	   B.MAKE_NO MakeNo, 
                            	   B.SERIAL_NO SerialNo, 
                            	   CASE WHEN A.ALLOCATION_TYPE = '5' AND B.STATUS = '0' THEN B.SRC_QTY - B.A_SRC_QTY ELSE B.TAR_QTY END TarQty
                            	   FROM F151002 B 
                            	   JOIN F151001 A
                            	   ON B.DC_CODE = A.DC_CODE
                            	   AND B.GUP_CODE = A.GUP_CODE
                            	   AND B.CUST_CODE = A.CUST_CODE
                            	   AND B.ALLOCATION_NO = A.ALLOCATION_NO
                            WHERE B.DC_CODE = @p0
                            AND B.GUP_CODE = @p1
                            AND B.CUST_CODE = @p2
                            AND A.ALLOCATION_TYPE IN ('2', '3', '5', '6')
                            AND A.STATUS <> '5' AND A.STATUS <> '9'
                            AND B.ITEM_CODE = @p3
                            ";
            var result = SqlQuery<AllocDetailByReplenish>(sql, parm.ToArray());
            return result;
        }

        public F151002 GetDataByAllocSeq(string dcCode, string gupCode, string custCode, string allocNo, Int16 allocSeq)
        {
            var parm = new List<SqlParameter>();
            parm.Add(new SqlParameter("@p0", dcCode));
            parm.Add(new SqlParameter("@p1", gupCode));
            parm.Add(new SqlParameter("@p2", custCode));
            parm.Add(new SqlParameter("@p3", allocNo));
            parm.Add(new SqlParameter("@p4", allocSeq));

            string sql = $@"SELECT * 
                            FROM F151002
                            WHERE DC_CODE = @p0
                            AND GUP_CODE = @p1
                            AND CUST_CODE = @p2
                            AND ALLOCATION_NO = @p3
                            AND ALLOCATION_SEQ = @p4
                            ";
            var result = SqlQuery<F151002>(sql, parm.ToArray()).FirstOrDefault();
            return result;
        }

				public IQueryable<F151002> GetDatas(string dcCode, string gupCode, string custCode, string allocationNo)
				{
						var parm = new List<SqlParameter>();
						parm.Add(new SqlParameter("@p0", SqlDbType.VarChar) {Value = dcCode });
						parm.Add(new SqlParameter("@p1", SqlDbType.VarChar) { Value = gupCode });
						parm.Add(new SqlParameter("@p2", SqlDbType.VarChar) { Value = custCode });
						parm.Add(new SqlParameter("@p3", SqlDbType.VarChar) { Value = allocationNo });


						string sql = $@"SELECT * 
															FROM F151002
															WHERE DC_CODE = @p0
															AND GUP_CODE = @p1
															AND CUST_CODE = @p2
															AND ALLOCATION_NO = @p3
													";
						return SqlQuery<F151002>(sql, parm.ToArray());
		}

		public IQueryable<F151002> GetDatasWithNoCancel(string dcCode, string gupCode, string custCode, string allocationNo)
		{
			var parm = new List<SqlParameter>();
			parm.Add(new SqlParameter("@p0", SqlDbType.VarChar) { Value = dcCode });
			parm.Add(new SqlParameter("@p1", SqlDbType.VarChar) { Value = gupCode });
			parm.Add(new SqlParameter("@p2", SqlDbType.VarChar) { Value = custCode });
			parm.Add(new SqlParameter("@p3", SqlDbType.VarChar) { Value = allocationNo });


			string sql = $@"SELECT * 
															FROM F151002
															WHERE DC_CODE = @p0
															AND GUP_CODE = @p1
															AND CUST_CODE = @p2
															AND ALLOCATION_NO = @p3
                              AND STATUS <>'9'
													";
			return SqlQuery<F151002>(sql, parm.ToArray());
		}

    public IQueryable<OrderCancelInfo> GetOrderCancelInfoData(string dcCode, string gupCode, string custCode, List<string> pickOrdNos)
    {
      var param = new List<SqlParameter>
      {
        new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
        new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar }
      };

      var sql = @"
                SELECT 
                  '已產生虛擬回復上架單' TYPE,
                  REFENCE_NO ORD_NO,
                  REFENCE_SEQ SEQ_NO,
                  ITEM_CODE,
                  VALID_DATE,
                  MAKE_NO,
                  SERIAL_NO,
                  TAR_QTY B_QTY,
                  A_TAR_QTY A_QTY,
                  TAR_LOC_CODE RETURN_LOC_CODE,
                  ALLOCATION_NO
                FROM F151002
                WHERE 
                  DC_CODE = @p0
                  AND GUP_CODE = @p1
                  AND CUST_CODE = @p2
                ";

      sql += param.CombineSqlInParameters(" AND REFENCE_NO", pickOrdNos, SqlDbType.VarChar);

      return SqlQuery<OrderCancelInfo>(sql, param.ToArray());
    }
  }
}