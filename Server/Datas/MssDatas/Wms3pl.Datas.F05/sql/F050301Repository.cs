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
	public partial class F050301Repository : RepositoryBase<F050301, Wms3plDbContext, F050301Repository>
	{
		/// <summary>
		/// 將指定的訂單編號狀態設為指定的狀態
		/// </summary>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="dcCode"></param>
		/// <param name="ordNoList"></param>
		/// <param name="status">指定的狀態</param>
		public void UpdateStatus(string gupCode, string custCode, string dcCode, IEnumerable<string> ordNoList, string status)
		{
			var parameters = new List<object>
						{
								status,
								Current.Staff,
								Current.StaffName,
                DateTime.Now,
                gupCode,
								custCode,
								dcCode
						};

			int paramStartIndex = parameters.Count;
			var inSql = parameters.CombineSqlInParameters("ORD_NO", ordNoList, ref paramStartIndex);
			var sql = @"Update F050301 Set 
                            PROC_FLAG = @p0, 
                            UPD_STAFF = @p1,
                            UPD_NAME = @p2, 
                            UPD_DATE = @p3
						Where GUP_CODE = @p4
						And CUST_CODE = @p5
						And DC_CODE = @p6
						And " + inSql;

			ExecuteSqlCommand(sql, parameters.ToArray());
		}

		public IQueryable<F050301Data> GetF050301Datas(string dcCode, string gupCode, string custCode, string delvDate,
				string pickTime, string custOrdNo, string itemCode, string consignee, string ordNo, string workType)
		{
			var parameters = new List<SqlParameter>
						{
                            new SqlParameter("@p0",System.Data.SqlDbType.VarChar){Value=dcCode},
                            new SqlParameter("@p1",System.Data.SqlDbType.VarChar){Value=gupCode},
                            new SqlParameter("@p2",System.Data.SqlDbType.VarChar){Value=custCode},
                            new SqlParameter("@p3",System.Data.SqlDbType.Date){Value=delvDate},
                            new SqlParameter("@p4",System.Data.SqlDbType.VarChar){Value=pickTime}
                        };
			var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY A.DELV_DATE,A.PICK_TIME,A.CUST_ORD_NO,A.ORD_NO ASC) ROWNUM, A.* 
					     FROM ( 
					      SELECT * FROM ( SELECT DISTINCT TOP 100 PERCENT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,
										 C.DELV_DATE,C.PICK_TIME,A.CUST_ORD_NO,A.ORD_NO,A.ORD_TYPE,A.CONSIGNEE,A.ZIP_CODE,A.ADDRESS, STRING_AGG(C.STATUS, ',') STATUS_LIST
					        FROM F050301 A 
					       INNER JOIN F05030101 B 
					          ON B.DC_CODE= A.DC_CODE 
					         AND B.GUP_CODE = A.GUP_CODE 
					         AND B.CUST_CODE = A.CUST_CODE 
					         AND B.ORD_NO = A.ORD_NO 
					       INNER JOIN F050801 C 
					          ON C.DC_CODE = B.DC_CODE 
					         AND C.GUP_CODE = B.GUP_CODE 
					         AND C.CUST_CODE = B.CUST_CODE 
					         AND C.WMS_ORD_NO = B.WMS_ORD_NO 
					       INNER JOIN F050802 D 
					          ON D.DC_CODE = C.DC_CODE 
					         AND D.GUP_CODE = C.GUP_CODE 
					         AND D.CUST_CODE = C.CUST_CODE 
					         AND D.WMS_ORD_NO = C.WMS_ORD_NO 
					       INNER JOIN F1903 F
					         ON F.GUP_CODE = D.GUP_CODE 
					         AND F.CUST_CODE = D.CUST_CODE 
					         AND F.ITEM_CODE = D.ITEM_CODE 
					       WHERE A.PROC_FLAG = '1'
					  		 AND A.DC_CODE = @p0 
					         AND A.GUP_CODE = @p1 
					         AND A.CUST_CODE = @p2 
					         AND C.DELV_DATE = @p3
					         AND C.PICK_TIME = @p4 
					         AND A.PROC_FLAG !='9'
                  ";
			if (!string.IsNullOrEmpty(custOrdNo))
			{
				sql += "        AND A.CUST_ORD_NO =@p" + parameters.Count;
				parameters.Add(new SqlParameter("@p" + parameters.Count, custOrdNo));
			}
			if (!string.IsNullOrEmpty(itemCode))
			{
				sql += $"        AND (D.ITEM_CODE =@p{parameters.Count} OR F.EAN_CODE1 =@p{parameters.Count} OR F.EAN_CODE2 =@p{parameters.Count} OR F.EAN_CODE3 =@p{parameters.Count})";
				parameters.Add(new SqlParameter("@p" + parameters.Count, itemCode));
			}
			//if (!string.IsNullOrEmpty(consignee))
			//	sql += "        AND A.CONSIGNEE LIKE '%" + consignee + "%' ";

			if (!string.IsNullOrEmpty(ordNo))
			{
				sql += "       AND A.ORD_NO = @p" + parameters.Count;
				parameters.Add(new SqlParameter("@p" + parameters.Count, ordNo));
			}

			string unionString = string.Empty;
			if (workType == "5")
				unionString = @"
						 UNION 
					SELECT G.DC_CODE,
						   G.GUP_CODE,
						   G.CUST_CODE,
						   NULL DELV_DATE,
						   NULL PICK_TIME,
						   G.CUST_ORD_NO,
						   G.ORD_NO,
						   G.ORD_TYPE,
						   G.CONSIGNEE,
						   G.ZIP_CODE,
						   G.ADDRESS
					  FROM F050001 G
					  UNION
					  SELECT F.DC_CODE,
						   F.GUP_CODE,
						   F.CUST_CODE,
						   NULL DELV_DATE,
						   NULL PICK_TIME,
						   F.CUST_ORD_NO,
						   F.ORD_NO,
						   F.ORD_TYPE,
						   F.CONSIGNEE,
						   F.ZIP_CODE,
						   F.ADDRESS
					  FROM F050301 F
					 WHERE F.PROC_FLAG = '0'
				";
			sql += $@" GROUP BY A.DC_CODE,A.GUP_CODE,A.CUST_CODE,
								 C.DELV_DATE,C.PICK_TIME,A.CUST_ORD_NO,A.ORD_NO,A.ORD_TYPE,A.CONSIGNEE,A.ZIP_CODE,A.ADDRESS
								 ORDER BY C.DELV_DATE,C.PICK_TIME,A.CUST_ORD_NO,A.ORD_NO) H
							{unionString}
				      ) A WHERE A.STATUS_LIST NOT LIKE '%2%' --不能查出F050801.STATUS為2(已包裝)或5(已裝車)或6(已扣帳)或9(取消)
								AND A.STATUS_LIST NOT LIKE '%5%'
								AND A.STATUS_LIST NOT LIKE '%6%'
								AND A.STATUS_LIST NOT LIKE '%9%'
							";

			var result = SqlQuery<F050301Data>(sql, parameters.ToArray());


			if (!string.IsNullOrEmpty(consignee))
			{
				result = result.Where(o => o.CONSIGNEE != null && o.CONSIGNEE.Contains(consignee));
			}

			return result;
		}

		public IQueryable<F050301Data> GetF050301Datas(string dcCode, string gupCode, string custCode, List<string> ordNos, string workType)
		{
			var parameters = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode)
						};
			var sql = @" SELECT ROW_NUMBER()OVER(ORDER BY A.DELV_DATE,A.PICK_TIME,A.CUST_ORD_NO,A.ORD_NO ASC) ROWNUM, A.* 
					     FROM ( 
					      SELECT * FROM ( SELECT DISTINCT TOP 100 PERCENT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,
										 C.DELV_DATE,C.PICK_TIME,A.CUST_ORD_NO,A.ORD_NO,A.ORD_TYPE,A.CONSIGNEE,A.ZIP_CODE,A.ADDRESS
					        FROM F050301 A 
					       INNER JOIN F05030101 B 
					          ON B.DC_CODE= A.DC_CODE 
					         AND B.GUP_CODE = A.GUP_CODE 
					         AND B.CUST_CODE = A.CUST_CODE 
					         AND B.ORD_NO = A.ORD_NO 
					       INNER JOIN F050801 C 
					          ON C.DC_CODE = B.DC_CODE 
					         AND C.GUP_CODE = B.GUP_CODE 
					         AND C.CUST_CODE = B.CUST_CODE 
					         AND C.WMS_ORD_NO = B.WMS_ORD_NO 
					       INNER JOIN F050802 D 
					          ON D.DC_CODE = C.DC_CODE 
					         AND D.GUP_CODE = C.GUP_CODE 
					         AND D.CUST_CODE = C.CUST_CODE 
					         AND D.WMS_ORD_NO = C.WMS_ORD_NO 
					       INNER JOIN F1903 F
					         ON F.GUP_CODE = D.GUP_CODE 
					         AND F.CUST_CODE = D.CUST_CODE 
					         AND F.ITEM_CODE = D.ITEM_CODE 
					       WHERE A.PROC_FLAG = '1'
					  		 AND A.DC_CODE = @p0 
					         AND A.GUP_CODE = @p1 
					         AND A.CUST_CODE = @p2 
					         AND A.PROC_FLAG !='9'
                             AND C.STATUS IN ('0', '1', '2')
                            ";

			if (ordNos.Any())
				sql += $"       AND A.ORD_NO IN ('{ string.Join("','", ordNos) }') ";

			string unionString = string.Empty;
			if (workType == "5")
				unionString = @"
						 UNION 
					SELECT G.DC_CODE,
						   G.GUP_CODE,
						   G.CUST_CODE,
						   NULL DELV_DATE,
						   NULL PICK_TIME,
						   G.CUST_ORD_NO,
						   G.ORD_NO,
						   G.ORD_TYPE,
						   G.CONSIGNEE,
						   G.ZIP_CODE,
						   G.ADDRESS
					  FROM F050001 G
					  UNION
					  SELECT F.DC_CODE,
						   F.GUP_CODE,
						   F.CUST_CODE,
						   NULL DELV_DATE,
						   NULL PICK_TIME,
						   F.CUST_ORD_NO,
						   F.ORD_NO,
						   F.ORD_TYPE,
						   F.CONSIGNEE,
						   F.ZIP_CODE,
						   F.ADDRESS
					  FROM F050301 F
					 WHERE F.PROC_FLAG = '0'
				";
			sql += $@" ORDER BY C.DELV_DATE,C.PICK_TIME,A.CUST_ORD_NO,A.ORD_NO) H
							{unionString}
				      ) A ";

			var result = SqlQuery<F050301Data>(sql, parameters.ToArray());

			return result;
		}

		public bool IsOrderNotPackage(string dcCode, string gupCode, string custCode, string ordNo)
		{
			var parameters = new List<SqlParameter>
						{
								new SqlParameter("@p0", dcCode),
								new SqlParameter("@p1", gupCode),
								new SqlParameter("@p2", custCode),
								new SqlParameter("@p3", ordNo)
						};
			var sql = @" SELECT A.* 
                         FROM ( 
                          SELECT DISTINCT TOP 100 PERCENT A.ORD_NO 
                            FROM F050301 A 
                            LEFT JOIN ( -- 包裝前訂單 有找到代表已為已包裝訂單 沒找到才是包裝前訂單 
                              SELECT DISTINCT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ORD_NO 
                                FROM F050301 A 
                               INNER JOIN F05030101 B ON B.DC_CODE = A.DC_CODE AND B.GUP_CODE = A.GUP_CODE AND B.CUST_CODE = A.CUST_CODE AND B.ORD_NO = A.ORD_NO 
                               INNER JOIN F050801 C ON C.DC_CODE = B.DC_CODE AND C.GUP_CODE = B.GUP_CODE AND C.CUST_CODE = B.CUST_CODE AND C.WMS_ORD_NO = B.WMS_ORD_NO 
                               INNER JOIN F055001 D ON D.DC_CODE = C.DC_CODE AND D.GUP_CODE = C.GUP_CODE AND D.CUST_CODE = C.CUST_CODE AND D.WMS_ORD_NO = C.WMS_ORD_NO 
                                   ) E ON E.DC_CODE = A.DC_CODE 
                      								 AND E.GUP_CODE = A.GUP_CODE 
                      								 AND E.CUST_CODE = A.CUST_CODE 
                      								 AND E.ORD_NO = A.ORD_NO 
                           WHERE  E.ORD_NO IS NULL 
                      				AND A.DC_CODE = @p0 
                             AND A.GUP_CODE = @p1 
                             AND A.CUST_CODE = @p2 
                             AND A.ORD_NO = @p3 
                            ) A ";

			var result = SqlQuery<F050301Data>(sql, parameters.ToArray());

			return result.Any();
		}

		public IQueryable<EgsReturnConsign> GetEgsReturnConsigns(EgsReturnConsignParam param)
		{

			var parms = new List<SqlParameter>
      {
        new SqlParameter("@p0", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
      };

			var sql = @" SELECT DISTINCT 
													A.DC_CODE, --物流中心
													A.GUP_CODE, --業主
													A.CUST_CODE,--貨主
                          C.ALL_ID, --配送商
													C.WMS_ORD_NO, --出貨單號
													'1' CONSIGN_TYPE, --托單類別(1:客戶自行列印託運單,2:速達協助列印 (由速達系統分配託運單號),3:已有單號，由速達列印(A4二模) –逆物流收退貨)
													D.CONSIGN_NO, --託運單號
													A.CUST_ORD_NO, --訂單編號
													H.CUSTOMER_ID, --契客代號
													'00' + C.TMPR_TYPE  TEMPERATURE, --溫層(0001:常溫  0002:冷藏 0003:冷凍)
													'' DISTANCE, --距離(00:同縣市 01:外縣市 02:離島)
													CASE  ISNULL(I.DEFAULT_BOXSIZE,ISNULL(L.DEFAULT_BOXSIZE,M.DEFAULT_BOXSIZE))
													WHEN 60 THEN '0001'
													WHEN 90 THEN '0002'
													WHEN 120 THEN '0003'
													WHEN 150 THEN '0004'
												ELSE '0001' 
										    END SPEC,   --規格(0001: 60cm   0002: 90cm   0003: 120cm  0004: 150cm)
												CASE WHEN ISNULL(C.COLLECT_AMT,0) >0
												THEN 'Y' ELSE 'N' END ISCOLLECT, --是否代收(	N:否  Y:是)
												C.COLLECT_AMT, -- 代收金額
												'N' ARRIVEDPAY, --是否到付(無作用，請固定填N)
												'01' PAYCASH, --是否付現(00:付現 01:月結)
												A.CONSIGNEE RECEIVER_NAME, -- 收件者姓名
												'***' RECEIVER_MOBILE, --收件者手機號碼
												'***' RECEIVER_PHONE, --收件者電話
												'' RECEIVER_SUDA5,--收件人地址的速達五碼郵遞區號(必填)
												A.ADDRESS RECEIVER_ADDRESS, --收件者地址(必填)
												ISNULL(I.CHANNEL_NAME,ISNULL(L.CHANNEL_NAME,ISNULL(M.CHANNEL_NAME,J.DC_NAME))) SENDER_NAME, --寄件者姓名(必填)
												ISNULL(I.CHANNEL_TEL,ISNULL(L.CHANNEL_TEL,ISNULL(M.CHANNEL_TEL,J.TEL))) SENDER_TEL,--寄件者電話
												'' SENDER_MOBILE, --寄件者手機
												ISNULL(K.ZIP_CODE,'') SENDER_SUDA5, --寄件者地址速達五碼郵遞區號(必填)
												ISNULL(I.CHANNEL_ADDRESS,ISNULL(L.CHANNEL_ADDRESS,ISNULL(M.CHANNEL_ADDRESS,J.ADDRESS))) SENDER_ADDRESS, --寄件者地址(必填)
												Format(@p0, N'yyyyMMddHHmmss') SHIP_DATE, --契客出貨日期(系統日YYYYMMDDhhmmss共14碼) 正物流為系統日 逆物流為預定配達日/出貨日的時間
												'4' PICKUP_TIMEZONE, --預定取件時段(1: 9~12    2: 12~17    3: 17~20   4: 不限時(固定4不限時))
												C.DELV_PERIOD  DELV_TIMEZONE, --預定配達時段(1: 9~12    2: 12~17   3: 17~20   4: 不限時  5:20~21(需限定區域))
                        '' MEMBER_ID, --會員編號
												'' ITEM_NAME,--物品名稱
												'N' ISFRAGILE,--易碎物品(固定填N)
												'N' ISPRECISON_INSTRUMENT,--精密儀器(固定填N)
												A.MEMO, --備註
												'' SD_ROUTE, --SD路線代碼 
                        E.DISTR_USE   
									FROM F050301 A
									JOIN F05030101 B
									  ON B.DC_CODE = A.DC_CODE
									 AND B.GUP_CODE = A.GUP_CODE
									 AND B.CUST_CODE = A.CUST_CODE
									 AND B.ORD_NO = A.ORD_NO
									JOIN F050801 C
									  ON C.DC_CODE = B.DC_CODE
									 AND C.GUP_CODE = B.GUP_CODE
									 AND C.CUST_CODE = B.CUST_CODE
									 AND C.WMS_ORD_NO = B.WMS_ORD_NO
									JOIN F050901 D
									  ON D.DC_CODE = C.DC_CODE
									 AND D.GUP_CODE =C.GUP_CODE
									 AND D.CUST_CODE = C.CUST_CODE
									 AND D.WMS_NO = C.WMS_ORD_NO
									JOIN F700102 E
									  ON E.DC_CODE = C.DC_CODE
									 AND E.GUP_CODE = C.GUP_CODE
									 AND E.CUST_CODE = C.CUST_CODE
									 AND E.WMS_NO = C.WMS_ORD_NO
									JOIN F700101 F
									  ON F.DC_CODE = E.DC_CODE
									 AND F.DISTR_CAR_NO = E.DISTR_CAR_NO
									JOIN F19471201 G
									  ON G.DC_CODE = D.DC_CODE
									 AND G.GUP_CODE = D.GUP_CODE
                   AND G.CUST_CODE = D.CUST_CODE
                   AND (G.CHANNEL = A.CHANNEL OR G.CHANNEL ='00')
                   AND G.ALL_ID = C.ALL_ID
									 AND G.CONSIGN_NO = D.CONSIGN_NO
									 AND G.ISUSED='1'
									JOIN F194712 H
									  ON H.DC_CODE = G.DC_CODE
									 AND H.GUP_CODE = G.GUP_CODE
                   AND H.CUST_CODE = G.CUST_CODE
									 AND H.ALL_ID = G.ALL_ID
                   AND H.CHANNEL = G.CHANNEL
									 AND H.CUSTOMER_ID = G.CUSTOMER_ID
								 	 AND H.CONSIGN_TYPE = G.CONSIGN_TYPE
                   AND H.ISTEST = G.ISTEST
									LEFT JOIN F190905 I
									  ON I.DC_CODE = A.DC_CODE
									 AND I.GUP_CODE =A.GUP_CODE
									 AND I.CUST_CODE = A.CUST_CODE
									 AND I.ALL_ID = C.ALL_ID
									 AND I.CHANNEL = A.CHANNEL
									 AND I.SUBCHANNEL = A.SUBCHANNEL
									LEFT JOIN F1901 J
									  ON J.DC_CODE = A.DC_CODE
									LEFT JOIN F190905 L
									  ON L.DC_CODE = A.DC_CODE
									 AND L.GUP_CODE =A.GUP_CODE
									 AND L.CUST_CODE = A.CUST_CODE
									 AND L.ALL_ID = C.ALL_ID
									 AND L.CHANNEL = '00'
									 AND L.SUBCHANNEL = A.SUBCHANNEL
                  LEFT JOIN F190905 M
                    ON M.DC_CODE = A.DC_CODE
                   AND M.GUP_CODE = A.GUP_CODE
                   AND M.CUST_CODE = A.CUST_CODE
                   AND M.ALL_ID = C.ALL_ID
                   AND M.CHANNEL = A.CHANNEL
                   AND M.SUBCHANNEL = '00'
                  LEFT JOIN F194704 K
                    ON K.DC_CODE = A.DC_CODE
                   AND K.GUP_CODE = A.GUP_CODE
                   AND K.CUST_CODE = A.CUST_CODE
                   AND K.ALL_ID = C.ALL_ID
								 WHERE D.DISTR_EDI_STATUS ='0' --取得配送商未回檔資料
									 AND C.STATUS IN ('1','2','5','6') --必須狀態為包裝完
                   AND F.HAVE_WMS_NO='1' --有單派車
                   AND F.STATUS <> '9' -- 非取消派車單
                   AND E.DISTR_USE ='01' --正物流
                   ";
			//客戶代號
			if (!string.IsNullOrWhiteSpace(param.CustomerId))
			{
				sql += " AND H.CUSTOMER_ID = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.CustomerId));
			}

			//物流中心
			if (!string.IsNullOrWhiteSpace(param.DcCode))
			{
				sql += " AND A.DC_CODE = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.DcCode));
			}
			//業主
			if (!string.IsNullOrWhiteSpace(param.GupCode))
			{
				sql += " AND A.GUP_CODE = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.GupCode));
			}
			//貨主
			if (!string.IsNullOrWhiteSpace(param.CustCode))
			{
				sql += " AND A.CUST_CODE = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.CustCode));
			}
			//通路
			if (!string.IsNullOrWhiteSpace(param.Channel))
			{
				sql += " AND A.CHANNEL = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.Channel));
			}

			//配送商
			if (!string.IsNullOrWhiteSpace(param.AllId))
			{
				sql += " AND A.ALL_ID = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.AllId));
			}
			//配次
			if (!string.IsNullOrWhiteSpace(param.DelvTimes))
			{
				sql += " AND E.DELV_TIMES = @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.DelvTimes));
			}
			//訂單日期-起
			if (param.OrdSDate.HasValue)
			{
				sql += " AND A.ORD_DATE >= @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.OrdSDate));
			}
			//訂單日期-迄
			if (param.OrdEDate.HasValue)
			{
				sql += " AND A.ORD_DATE <= @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.OrdEDate));
			}
			//出貨日期-起
			if (param.DelvSDate.HasValue)
			{
				sql += " AND C.DELV_DATE >= @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.DelvSDate));
			}

			//出貨日期-迄
			if (param.DelvEDate.HasValue)
			{
				//delvEDate = delvEDate.Value.AddDays(1);
				sql += " AND C.DELV_DATE <= @p" + parms.Count;
				parms.Add(new SqlParameter("@p" + parms.Count, param.DelvEDate));
			}

#if DEBUG
			sql += " AND G.ISTEST = '1' ";
#else
			sql+= " AND G.ISTEST = '0' ";
#endif

			var result = SqlQuery<EgsReturnConsign>(sql, parms.ToArray());
			return result;
		}









		public void UpdateLackToCancelOrder(string gupCode, string custCode)
		{
			var sql = @"  UPDATE F050301
							 SET PROC_FLAG ='8',UPD_DATE = @p0,UPD_STAFF=@p1,UPD_NAME=@p2 FROM F050301 F050301
										WHERE EXISTS(SELECT A.GUP_CODE,A.CUST_CODE,A.ORD_NO
										 FROM F050101 A
										 JOIN F050301 B
										 ON B.DC_CODE = A.DC_CODE
										 AND B.GUP_CODE = A.GUP_CODE
										 AND B.CUST_CODE = A.CUST_CODE
										 AND B.ORD_NO = A.ORD_NO
										 WHERE B.PROC_FLAG ='0'
										 AND F050301.GUP_CODE = A.GUP_CODE
										 AND F050301.CUST_CODE = A.CUST_CODE
										 AND F050301.ORD_NO = A.ORD_NO)
										 AND F050301.GUP_CODE = @p3
										 AND F050301.CUST_CODE = @p4";
			var parms = new object[] { DateTime.Now, Current.Staff, Current.StaffName, gupCode, custCode };
			ExecuteSqlCommand(sql, parms);
		}

		public void DeleteLackOrder(string gupCode, string custCode)
		{
			var sql = @" DELETE F050301
										WHERE PROC_FLAG ='0'
										 AND GUP_CODE = @p0
										 AND CUST_CODE = @p1";
			ExecuteSqlCommand(sql, new object[] { gupCode, custCode });
		}

		public IQueryable<F050301> GetData(string dcCode,string gupCode,string custCode,List<string> ordNos)
		{
			var parameter = new List<object> { dcCode, gupCode, custCode };

			var sql = @"SELECT * FROM F050301 
						WHERE DC_CODE = @p0
						AND GUP_CODE = @p1
						AND CUST_CODE = @p2";

			sql += parameter.CombineSqlInParameters(" AND ORD_NO", ordNos);

			var result = SqlQuery<F050301>(sql, parameter.ToArray());

			return result;
		}

		public IQueryable<OrdNoStatusModel> GetOrdNoStatusData(string dcCode, string gupCode, string custCode, string ordNo)
		{
			var parameter = new List<object> { dcCode, gupCode, custCode , ordNo };

			var sql = @"SELECT A.ORD_NO, C.WMS_ORD_NO, C.STATUS FROM F050301 A 
					       INNER JOIN F05030101 B 
					          ON B.DC_CODE= A.DC_CODE 
					         AND B.GUP_CODE = A.GUP_CODE 
					         AND B.CUST_CODE = A.CUST_CODE 
					         AND B.ORD_NO = A.ORD_NO 
					       INNER JOIN F050801 C 
					          ON C.DC_CODE = B.DC_CODE 
					         AND C.GUP_CODE = B.GUP_CODE 
					         AND C.CUST_CODE = B.CUST_CODE 
					         AND C.WMS_ORD_NO = B.WMS_ORD_NO 
							 AND A.DC_CODE = @p0
					         AND A.GUP_CODE = @p1 
					         AND A.CUST_CODE = @p2
									 AND A.ORD_NO = @p3
								";

			var result = SqlQuery<OrdNoStatusModel>(sql, parameter.ToArray());

			return result;
		}

        public bool IsExistF050301CustOrdNo(string dcCode, string gupCode, string custCode, string custOrdNo)
        {
            var parameters = new List<SqlParameter>
                        {
                                new SqlParameter("@p0", dcCode),
                                new SqlParameter("@p1", gupCode),
                                new SqlParameter("@p2", custCode),
                                new SqlParameter("@p3", custOrdNo)
                        };
            var sql = @" SELECT TOP 1 *   
                            FROM F050301 
                            WHERE DC_CODE = @p0
						    AND GUP_CODE = @p1
						    AND CUST_CODE = @p2
                            AND CUST_ORD_NO = @p3
                            ORDER BY CRT_DATE DESC
                            ";
            var result = SqlQuery<string>(sql, parameters.ToArray());

            return result.Any();
        }

        public string GetFstCustOrdNo(string dcCode, string gupCode, string custCode, string wmsOrdNo)
        {
            var parameter = new List<object> { dcCode, gupCode, custCode, wmsOrdNo };

            var sql = @"SELECT TOP 1 A.CUST_ORD_NO 
                        FROM F050301 A
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

            var result = SqlQuery<string>(sql, parameter.ToArray()).FirstOrDefault();

            return result;
        }
		
		public F050301 GetDataByWmsOrdNo(string dcCode, string gupCode, string CustCode, string wmsOrdNo)
		{
			var param = new object[] { dcCode, gupCode, CustCode, wmsOrdNo };
			var sql = @"SELECT  A.* 
            FROM F050301 A
						JOIN F05030101 B 
						ON A.DC_CODE  = B.DC_CODE 
						AND A.GUP_CODE  = B.GUP_CODE 
						AND A.CUST_CODE  = B.CUST_CODE 
						AND A.ORD_NO = B.ORD_NO 
						WHERE B.DC_CODE = @p0
						AND B.GUP_CODE = @p1
						AND B.CUST_CODE = @p2
						AND B.WMS_ORD_NO = @p3";
			var result = SqlQuery<F050301>(sql, param).FirstOrDefault();
			return result;
		}

		public IQueryable<F050301> GetDatasByWmsOrdNo(string dcCode, string gupCode, string CustCode, string wmsOrdNo)
		{
			var param = new object[] { dcCode, gupCode, CustCode, wmsOrdNo };
			var sql = @"SELECT  A.* 
            FROM F050301 A
						JOIN F05030101 B 
						ON A.DC_CODE  = B.DC_CODE 
						AND A.GUP_CODE  = B.GUP_CODE 
						AND A.CUST_CODE  = B.CUST_CODE 
						AND A.ORD_NO = B.ORD_NO 
						WHERE B.DC_CODE = @p0
						AND B.GUP_CODE = @p1
						AND B.CUST_CODE = @p2
						AND B.WMS_ORD_NO = @p3";
			var result = SqlQuery<F050301>(sql, param);
			return result;
		}

    /// <summary>
    /// 撈可能在配庫執行中打取消過來的訂單 (F050101.STATUS=9 AND F050301.PROC_FLAG<>9)
    /// </summary>
    public IQueryable<F050301> GetCancelNotCompleteOrd()
    {
      var sql = @"SELECT * FROM F050301 A 
                  JOIN F050101 B 
                      ON A.DC_CODE = B.DC_CODE 
                      AND A.GUP_CODE = B.GUP_CODE 
                      AND A.CUST_CODE = B.CUST_CODE 
                      AND A.ORD_NO = B.ORD_NO 
				          JOIN F05030101 C
				              ON B.DC_CODE=C.DC_CODE
					            AND B.GUP_CODE=C.GUP_CODE
					            AND C.CUST_CODE=C.CUST_CODE
					            AND B.ORD_NO=C.ORD_NO
				          JOIN F050801 D
				              ON D.DC_CODE=C.DC_CODE
					            AND D.GUP_CODE=C.GUP_CODE
					            AND D.CUST_CODE=C.CUST_CODE
					            AND D.WMS_ORD_NO=C.WMS_ORD_NO
                  WHERE B.STATUS = '9'
					            AND A.PROC_FLAG <> '9'
					            AND D.STATUS='0'";

      return SqlQuery<F050301>(sql);
    }
  }
}
