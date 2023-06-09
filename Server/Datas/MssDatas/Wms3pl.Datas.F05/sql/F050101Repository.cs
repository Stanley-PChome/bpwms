using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F050101Repository : RepositoryBase<F050101, Wms3plDbContext, F050101Repository>
	{
		/// <summary>
		/// 訂單主檔
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="ordDateFrom"></param>
		/// <param name="ordDateTo"></param>
		/// <param name="ordNo"></param>
		/// <param name="arriveDateFrom"></param>
		/// <param name="arriveDateTo"></param>
		/// <param name="custOrdNo"></param>
		/// <param name="status"></param>
		/// <param name="retailCode"></param>
		/// <param name="custName"></param>
		/// <param name="wmsOrdNo"></param>
		/// <param name="pastNo"></param>
		/// <param name="address"></param>
		/// <returns></returns>
		public IQueryable<F050101Ex> GetF050101ExDatas(string gupCode, string custCode, string dcCode, DateTime? ordDateFrom, DateTime? ordDateTo, string ordNo, DateTime? arriveDateFrom,
						DateTime? arriveDateTo, string custOrdNo, string status, string retailCode, string custName, string wmsOrdNo, string pastNo, string address, string channel, string delvType, string allId,string moveOutTarget)
		{

			var parameterList = new List<SqlParameter>
												{
																new SqlParameter("@p0", SqlDbType.VarChar){ Value = dcCode},
																new SqlParameter("@p1", SqlDbType.VarChar){ Value = gupCode},
																new SqlParameter("@p2", SqlDbType.VarChar){ Value = custCode}
												};

			var sql = $@"SELECT DISTINCT *
							FROM (
							SELECT  A.ORD_NO,
										  A.CUST_ORD_NO,
											A.ORD_TYPE,
											A.RETAIL_CODE,
											A.ORD_DATE,
											A.CUST_NAME,
											A.SELF_TAKE,
											A.FRAGILE_LABEL,
											A.GUARANTEE,
											A.SA,
											A.GENDER,
											A.AGE,
											A.SA_QTY,
											A.SA_CHECK_QTY,
											A.TEL,
											A.ADDRESS,
											A.CONSIGNEE,
											A.ARRIVAL_DATE,
											A.TRAN_CODE,
											A.SP_DELV,
											A.CUST_COST,
											A.BATCH_NO,
											A.CHANNEL,
											A.POSM,
											A.CONTACT,
											A.CONTACT_TEL,
											A.TEL_2,
											A.SPECIAL_BUS,
											A.ALL_ID,
											A.COLLECT,
											A.COLLECT_AMT,
											A.MEMO,
											A.GUP_CODE,
											A.CUST_CODE,
											A.DC_CODE,
											A.CRT_STAFF,
											A.CRT_DATE,
											A.UPD_STAFF,
											A.UPD_DATE,
											A.CRT_NAME,
											A.UPD_NAME,
											A.TYPE_ID,
											A.CAN_FAST,
											A.TEL_1,
											A.TEL_AREA,
											A.PRINT_RECEIPT,
											A.RECEIPT_NO,
											A.RECEIPT_NO_HELP,
											A.RECEIPT_TITLE,
											A.RECEIPT_ADDRESS,
											A.BUSINESS_NO,
											A.DISTR_CAR_NO,
											A.CVS_TAKE,
											A.SUBCHANNEL,
											--以上欄位為F050101欄位，除了 STATUS 要額外組合狀態以外
											'0' EDI_FLAG, -- 派車單的狀態(0待處理1系統已派車,2結案(配送商已拿貨))
											C.ESERVICE,  --超取服務商
											D.NAME CHANNEL_NAME, --通路名稱
											E.NAME DELV_NAME,   --配送方式
											F.ALL_COMP,          --配送商
											(CASE WHEN G.PROC_FLAG = '9' THEN '6'       -- 訂單取消 = 取消訂單			
											WHEN G.STATUS = 9 THEN '7'                 -- 出貨單狀態為取消 = 已包裝不出貨
											WHEN G.STATUS = 5 THEN '8'                 -- 出貨單狀態為已出貨 = 結案
											WHEN G.STATUS = 6 THEN '5'                 -- 出貨單已扣帳 = 已扣帳
											WHEN G.STATUS = 1 OR G.STATUS = 2 THEN '4' -- 出貨單已包裝或已稽核待出貨 = 已包裝
											WHEN G.PICK_STATUS = 1 THEN '3'            -- 揀貨單已揀貨 = 已揀貨
											WHEN G.PROC_FLAG = '1' THEN '2'            -- 訂單已配庫 = 產生批次
									    ELSE A.STATUS END) STATUS,                  -- 其它則使用原訂單主檔狀態
											 A.ROUND_PIECE,                               --來回件
											 A.FAST_DEAL_TYPE,
											 A.SUG_BOX_NO,
											 A.MOVE_OUT_TARGET,
											 A.PACKING_TYPE,
											 A.ISPACKCHECK,
                       (SELECT TOP(1) LOGISTIC_NAME FROM F0002 WHERE DC_CODE = @p0 AND LOGISTIC_CODE = A.SUG_LOGISTIC_CODE) SUG_LOGISTIC_CODE,
                       A.NP_FLAG
							FROM F050101 A 							
							LEFT JOIN F050304 C
							ON C.DC_CODE = A.DC_CODE
							AND C.GUP_CODE = A.GUP_CODE
							AND C.CUST_CODE = A.CUST_CODE
							AND C.ORD_NO = A.ORD_NO
							LEFT JOIN VW_F000904_LANG D
								ON D.TOPIC='F050101' AND D.SUBTOPIC='CHANNEL' AND D.VALUE = A.CHANNEL AND D.LANG = '{Current.Lang}'
							LEFT JOIN VW_F000904_LANG E
								ON E.TOPIC='P050302' AND E.SUBTOPIC='DELV_TYPE' AND E.VALUE = CASE WHEN A.SELF_TAKE = '1' THEN '0' ELSE CASE WHEN A.CVS_TAKE='1' THEN '1' ELSE '2' END END AND E.LANG = '{Current.Lang}' 
							LEFT JOIN F1947 F
								ON F.DC_CODE = A.DC_CODE
								AND F.ALL_ID = A.ALL_ID      
							LEFT JOIN (
							SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ORD_NO,
											MIN(C.PROC_FLAG) PROC_FLAG,MIN(E.STATUS) STATUS,MIN(F.PICK_STATUS) PICK_STATUS
							FROM F050101 A
							JOIN F050301 C
							ON C.DC_CODE = A.DC_CODE
							AND C.GUP_CODE = A.GUP_CODE
							AND C.CUST_CODE = A.CUST_CODE
							AND C.ORD_NO = A.ORD_NO                              
							JOIN F05030101 D
							ON D.DC_CODE = C.DC_CODE
							AND D.GUP_CODE = C.GUP_CODE
							AND D.CUST_CODE = C.CUST_CODE
							AND D.ORD_NO = C.ORD_NO
							JOIN F050801 E
							ON E.DC_CODE = D.DC_CODE
							AND E.GUP_CODE = D.GUP_CODE
							AND E.CUST_CODE = D.CUST_CODE
							AND E.WMS_ORD_NO = D.WMS_ORD_NO
							LEFT JOIN F051202 F
							ON F.DC_CODE = E.DC_CODE
							AND F.GUP_CODE = E.GUP_CODE
							AND F.CUST_CODE = E.CUST_CODE
							AND F.WMS_ORD_NO = E.WMS_ORD_NO    
							GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ORD_NO) G
							ON G.DC_CODE  = A.DC_CODE
							AND G.GUP_CODE = A.GUP_CODE
							AND G.CUST_CODE = A.CUST_CODE
							AND G.ORD_NO = A.ORD_NO   
             ) A -- 為了重組 STATUS，在包一層才能用於查詢條件判斷
						 WHERE A.DC_CODE = @p0 AND A.GUP_CODE = @p1 AND A.CUST_CODE = @p2 ";

			// 託運單與出貨單查詢
			if (!string.IsNullOrEmpty(pastNo) || !string.IsNullOrEmpty(wmsOrdNo))
			{
				sql += @" AND EXISTS
										(SELECT 1
										   FROM F05030101 B
												JOIN F050801 C
												   ON     B.WMS_ORD_NO = C.WMS_ORD_NO
													  AND B.CUST_CODE = C.CUST_CODE
													  AND B.GUP_CODE = C.GUP_CODE
													  AND B.DC_CODE = C.DC_CODE
												LEFT JOIN F055001 I
												   ON     C.WMS_ORD_NO = I.WMS_ORD_NO
													  AND C.CUST_CODE = I.CUST_CODE
													  AND C.GUP_CODE = I.GUP_CODE
													  AND C.DC_CODE = I.DC_CODE
										  WHERE     A.ORD_NO = B.ORD_NO
												AND A.CUST_CODE = B.CUST_CODE
												AND A.GUP_CODE = B.GUP_CODE
												AND A.DC_CODE = B.DC_CODE";

				if (!string.IsNullOrWhiteSpace(pastNo))
				{
					sql += @" AND I.PAST_NO = @p" + parameterList.Count();
					parameterList.Add(new SqlParameter("@p" + parameterList.Count(), SqlDbType.VarChar) { Value = pastNo });
				}
				if (!string.IsNullOrWhiteSpace(wmsOrdNo))
				{
					sql += @" AND C.WMS_ORD_NO = @p" + parameterList.Count();
					parameterList.Add(new SqlParameter("@p" + parameterList.Count(), SqlDbType.VarChar) { Value = wmsOrdNo });
				}
				sql += @" )";
			}

			// 不知道為啥當初要定 A，要改成 string.empty 的話，VW_F000904_LANG 也要跟著改
			if (status == "A")
			{
				sql += " AND A.STATUS <> '9' ";
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(status))
				{
					sql += @" AND A.STATUS = @p" + parameterList.Count();
					parameterList.Add(new SqlParameter("@p" + parameterList.Count(), SqlDbType.VarChar) { Value = status });
				}
			}

			if (ordDateFrom.HasValue && ordDateTo.HasValue)
			{
				sql += @" AND A.ORD_DATE >= @p" + parameterList.Count();
				parameterList.Add(new SqlParameter("@p"+ parameterList.Count(), SqlDbType.DateTime2) { Value = ordDateFrom.Value });
				sql += @" AND A.ORD_DATE <= @p" + parameterList.Count();
				parameterList.Add(new SqlParameter("@p"+ parameterList.Count(), SqlDbType.DateTime2) { Value = ordDateTo.Value });
			}
			
			if (arriveDateFrom.HasValue && arriveDateTo.HasValue)
			{
				sql += @" AND A.ARRIVAL_DATE >= @p" + parameterList.Count();
				parameterList.Add(new SqlParameter("@p" + parameterList.Count(), SqlDbType.DateTime2) { Value = arriveDateFrom.Value });
				sql += @" AND A.ARRIVAL_DATE <= @p" + parameterList.Count();
				parameterList.Add(new SqlParameter("@p" + parameterList.Count(), SqlDbType.DateTime2) { Value = arriveDateTo.Value });
			}

			if (!string.IsNullOrWhiteSpace(ordNo))
			{
				sql += @" AND A.ORD_NO = @p" + parameterList.Count();
				parameterList.Add(new SqlParameter("@p" + parameterList.Count(), SqlDbType.VarChar) { Value = ordNo });
			}
			if (!string.IsNullOrWhiteSpace(custOrdNo))
			{
				sql += @" AND A.CUST_ORD_NO = @p" + parameterList.Count();
				parameterList.Add(new SqlParameter("@p" + parameterList.Count(), SqlDbType.VarChar) { Value = custOrdNo });
			}
			if (!string.IsNullOrWhiteSpace(retailCode))
			{
				sql += @" AND A.RETAIL_CODE = @p" + parameterList.Count();
				parameterList.Add(new SqlParameter("@p" + parameterList.Count(), SqlDbType.VarChar) { Value = retailCode });
			}
			if (!string.IsNullOrWhiteSpace(custName))
			{
				sql += @" AND A.CUST_NAME = @p" + parameterList.Count();
				parameterList.Add(new SqlParameter("@p" + parameterList.Count(), SqlDbType.VarChar) { Value = custName });
			}
			if (!string.IsNullOrWhiteSpace(address))
			{
				sql += @" AND A.ADDRESS = @p" + parameterList.Count();
				AesCryptor.Current.Encode((string)address);
				parameterList.Add(new SqlParameter("@p" + parameterList.Count(), SqlDbType.VarChar) { Value = AesCryptor.Current.Encode((string)address) });
			}
			if (!string.IsNullOrWhiteSpace(channel))
			{
				sql += @" AND A.CHANNEL = @p" + parameterList.Count();
				parameterList.Add(new SqlParameter("@p" + parameterList.Count(), SqlDbType.VarChar) { Value = channel });
			}
			if (!string.IsNullOrWhiteSpace(allId))
			{
				sql += @" AND A.ALL_ID = @p" + parameterList.Count();
				parameterList.Add(new SqlParameter("@p" + parameterList.Count(), SqlDbType.VarChar) { Value = allId });
			}
			if (!string.IsNullOrWhiteSpace(moveOutTarget))
			{
				sql += @" AND A.MOVE_OUT_TARGET = @p" + parameterList.Count();
				parameterList.Add(new SqlParameter("@p" + parameterList.Count(), SqlDbType.NVarChar) { Value = moveOutTarget });
			}
			//sql += parameterList.CombineNotNullOrEmpty(" AND A.ORD_NO = @p{0} ", ordNo);
			//sql += parameterList.CombineNotNullOrEmpty(" AND A.CUST_ORD_NO = @p{0} ", custOrdNo);
			//sql += parameterList.CombineNotNullOrEmpty(" AND A.RETAIL_CODE = @p{0} ", retailCode);
			//sql += parameterList.CombineNotNullOrEmpty(" AND A.CUST_NAME = @p{0} ", custName);
			//sql += parameterList.CombineNotNullOrEmpty(" AND A.ADDRESS = @p{0} ", AesCryptor.Current.Encode((string)address));

			//sql += parameterList.CombineNotNullOrEmpty(" AND A.CHANNEL = @p{0} ", channel);
			//sql += parameterList.CombineNotNullOrEmpty(" AND A.ALL_ID = @p{0} ", allId);
			//sql += parameterList.CombineNotNullOrEmpty(" AND A.MOVE_OUT_TARGET = @p{0} ", moveOutTarget);
			switch (delvType)
			{
				case "0": //自取
					sql += " AND A.SELF_TAKE = 1";
					break;
				case "1": //超取
					sql += " AND A.CVS_TAKE = '1'";
					break;
				case "2": //宅配
					sql += " AND A.SELF_TAKE ='0' AND A.CVS_TAKE ='0' ";
					break;
				default:
					break;
			}

			sql += " ORDER BY A.ORD_NO ";

			var result = SqlQuery<F050101Ex>(sql, parameterList.ToArray());

			return result;
		}

		public IQueryable<F050102WithF050801> GetF050102WithF050801s(string gupCode, string custCode, string dcCode, string wmsordno)
		{
			var parameters = new List<SqlParameter>
						{
								new SqlParameter("@p0", gupCode),
								new SqlParameter("@p1", custCode),
								new SqlParameter("@p2", dcCode),
								new SqlParameter("@p3", wmsordno)
						};

			var sql = @"
SELECT 
  A.WMS_ORD_NO, A.ITEM_CODE, B.ITEM_NAME, B.ITEM_COLOR, B.ITEM_SPEC, B.ITEM_SIZE, SUM(A_DELV_QTY) AS A_DELV_QTY, 
  SUM(B_DELV_QTY) AS B_DELV_QTY, 
  SUM(ISNULL(C.LACK_QTY, 0)) AS LACK_QTY, A.SERIAL_NO
FROM 
  F050802 A 
  LEFT JOIN F1903 B ON A.ITEM_CODE = B.ITEM_CODE 
  AND A.GUP_CODE = B.GUP_CODE 
  AND A.CUST_CODE = B.CUST_CODE 
  LEFT JOIN (
    SELECT 
      DC_CODE, 
      GUP_CODE, 
      CUST_CODE, 
      WMS_ORD_NO, 
      ITEM_CODE, 
      ISDELETED, 
      RETURN_FLAG, 
      SUM(
        ISNULL(LACK_QTY, 0)
      ) AS LACK_QTY 
    FROM 
      F051206 
    GROUP BY 
      DC_CODE, GUP_CODE, CUST_CODE, WMS_ORD_NO, ITEM_CODE, ISDELETED, RETURN_FLAG, ITEM_CODE
  ) C ON A.DC_CODE = C.DC_CODE 
  AND A.GUP_CODE = C.GUP_CODE 
  AND A.CUST_CODE = C.CUST_CODE 
  AND A.WMS_ORD_NO = C.WMS_ORD_NO 
  AND A.ITEM_CODE = C.ITEM_CODE 
  AND C.ISDELETED = '0' 
  AND (
    C.RETURN_FLAG <> '3' 
    OR C.RETURN_FLAG IS NULL
  ) 
WHERE 
  A.GUP_CODE = @p0 
  AND A.CUST_CODE = @p1 
  AND A.DC_CODE = @p2 
  AND A.WMS_ORD_NO = @p3 
GROUP BY 
  A.WMS_ORD_NO, A.ITEM_CODE, B.ITEM_NAME, B.ITEM_COLOR, B.ITEM_SPEC, B.ITEM_SIZE, A.SERIAL_NO";


			var query = SqlQuery<F050102WithF050801>(sql, parameters.ToArray());
			return query;
		}

		public void UpdateLackToCancelOrder(string gupCode, string custCode)
		{
			var parms = new List<SqlParameter>
						{
								new SqlParameter("@p0", Current.Staff),
								new SqlParameter("@p1", Current.StaffName),
								new SqlParameter("@p2", gupCode),
								new SqlParameter("@p3", custCode),
                new SqlParameter("@p4", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 }
            };

			var sql = @" UPDATE F050101 SET F050101.STATUS ='9',F050101.UPD_DATE = @p4,F050101.UPD_STAFF=@p0,F050101.UPD_NAME=@p1 FROM F050101 F050101
										WHERE EXISTS(SELECT A.GUP_CODE,A.CUST_CODE,A.ORD_NO
										 FROM F050101 A
										 JOIN F050301 B
										 ON B.DC_CODE = A.DC_CODE
										 AND B.GUP_CODE = A.GUP_CODE
										 AND B.CUST_CODE = A.CUST_CODE
										 AND B.ORD_NO = A.ORD_NO
										 WHERE B.PROC_FLAG ='0'
										 AND A.GUP_CODE = F050101.GUP_CODE
										 AND A.CUST_CODE = F050101.CUST_CODE
										 AND A.ORD_NO = F050101.ORD_NO)
										 AND F050101.GUP_CODE = @p2
										 AND F050101.CUST_CODE = @p3 ";

			ExecuteSqlCommand(sql, parms.ToArray());
		}

        public F050101 GetDataByWmsOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var parameters = new List<SqlParameter>
                        {
                                new SqlParameter("@p0", dcCode),
                                new SqlParameter("@p1", gupCode),
                                new SqlParameter("@p2", custCode),
                                new SqlParameter("@p3", wmsOrdNo)
                        };

            var sql = @"SELECT A.* 
                        FROM F050101 A
                        JOIN F05030101 B
                        ON A.DC_CODE = B.DC_CODE
                        AND A.GUP_CODE = B.GUP_CODE
                        AND A.CUST_CODE = B.CUST_CODE
                        AND A.ORD_NO = B.ORD_NO
                        WHERE B.DC_CODE = @p0
                        AND B.GUP_CODE = @p1
                        AND B.CUST_CODE = @p2
                        AND B.WMS_ORD_NO = @p3
                        ";


            return SqlQuery<F050101>(sql, parameters.ToArray()).FirstOrDefault();
        }

        /// <summary>
        /// 用出貨編號查詢訂單資料
        /// </summary>
        /// <param name="dcCode">物流中心</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="WmsOrdNo">出貨單號</param>
        /// <returns></returns>
        public IQueryable<F050101> GetOrdNoByWmsOrdNo(string dcCode, string gupCode, string custCode, string WmsOrdNo)
        {
            var sql = @"
                SELECT b.* FROM F05030101 a INNER JOIN F050101 b 
                    ON a.DC_CODE=b.DC_CODE AND a.GUP_CODE=b.GUP_CODE AND a.CUST_CODE=b.CUST_CODE AND a.ORD_NO=b.ORD_NO
                WHERE a.DC_CODE=@p0 AND a.GUP_CODE=@p1 AND a.CUST_CODE=@p2 AND a.WMS_ORD_NO=@p3";
            var para = new List<SqlParameter>()
            {
                new SqlParameter("@p0",SqlDbType.VarChar){Value=dcCode},
                new SqlParameter("@p1",SqlDbType.VarChar){Value=gupCode},
                new SqlParameter("@p2",SqlDbType.VarChar){Value=custCode},
                new SqlParameter("@p3",SqlDbType.VarChar){Value=WmsOrdNo}
            };

            return SqlQuery<F050101>(sql, para.ToArray());
        }

        /// <summary>
        /// 用訂單編號查詢(如果是廠退會回傳NULL)
        /// </summary>
        /// <param name="dcCode">物流中心</param>
        /// <param name="gupCode">業主編號</param>
        /// <param name="custCode">貨主編號</param>
        /// <param name="WmsOrdNo">訂單編號</param>
        /// <returns></returns>
        public F050101 GetOrdNoByOrdNo(string dcCode, string gupCode, string custCode, string OrdNo)
        {
            var sql = @"SELECT * FROM F050101 WHERE DC_CODE=@p0 AND GUP_CODE=@p1 AND CUST_CODE=@p2 AND ORD_NO=@p3";
            var para = new List<SqlParameter>()
            {
                new SqlParameter("p0",SqlDbType.VarChar){Value=dcCode},
                new SqlParameter("p1",SqlDbType.VarChar){Value=gupCode},
                new SqlParameter("p2",SqlDbType.VarChar){Value=custCode},
                new SqlParameter("p3",SqlDbType.VarChar){Value=OrdNo},
            };
            return SqlQuery<F050101>(sql, para.ToArray()).SingleOrDefault();
        }
    }
}