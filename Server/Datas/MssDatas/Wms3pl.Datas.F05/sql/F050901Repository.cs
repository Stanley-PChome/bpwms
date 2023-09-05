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
    public partial class F050901Repository : RepositoryBase<F050901, Wms3plDbContext, F050901Repository>
    {
        public override void BulkInsert(IEnumerable<F050901> entities, params string[] withoutColumns)
        {
            var fieldsDefaultValue = new Dictionary<string, object>
            {
                {"STATUS","0"},
                {"CUST_EDI_STATUS","0"},
                {"DISTR_EDI_STATUS", "0"},
                {"CUST_EDI_QTY", 0}
            };

            base.BulkInsert(entities, fieldsDefaultValue, withoutColumns);
        }

        public void BulkUpdateDistrEdiStatus<T>(List<T> datas)
        {
            string sqls = string.Empty;

            var sql = @" UPDATE F050901
                       SET DISTR_EDI_STATUS ='1',
						   UPD_DATE = @p{0},
						   UPD_STAFF = @p{1}, 
						   UPD_NAME = @p{2}
					   WHERE	DISTR_EDI_STATUS ='0'
						    AND DC_CODE =@p{3}
						    AND GUP_CODE =@p{4}
						    AND CUST_CODE =@p{5}
                            AND CONSIGN_NO = @p{6};
                        ";

            // 用於資料筆數計步
            int index = 0;

            // 宣告ObjArray 共 參數*資料比數 長度
            var parmsList = new object[7 * datas.Count];

            var properties = typeof(T).GetProperties();

            foreach (var d in datas)
            {
                int subIndex = index * 7;

                // 指定位置依序填入對應的參數資料
                for (int i = 0; i < 7; i++)
                {
                    switch (i)
                    {
                        case 0: 
                            parmsList[i + subIndex] = DateTime.Now;
                            break;
                        case 1: 
                            parmsList[i + subIndex] = Current.Staff;
                            break;
                        case 2: 
                            parmsList[i + subIndex] = Current.StaffName;
                            break;
                        case 3: 
                            parmsList[i + subIndex] = properties.Where(x => x.Name == "DC_CODE").SingleOrDefault().GetValue(d);
                            break;
                        case 4:
                            parmsList[i + subIndex] = properties.Where(x => x.Name == "GUP_CODE").SingleOrDefault().GetValue(d);
                            break;
                        case 5:
                            parmsList[i + subIndex] = properties.Where(x => x.Name == "CUST_CODE").SingleOrDefault().GetValue(d);
                            break;
                        case 6:
                            parmsList[i + subIndex] = properties.Where(x => x.Name == "CONSIGN_NO").SingleOrDefault().GetValue(d);
                            break;
                    }
                }

                // 將每筆UpdateCommand的參數替換後，累加至AllCommand
                sqls += string.Format(sql, subIndex, subIndex + 1, subIndex + 2, subIndex + 3, subIndex + 4, subIndex + 5, subIndex + 6);

                index++;
            }

            ExecuteSqlCommand(sqls, parmsList);
        }

        public void BulkUpdateDistrEdiStatusSod(List<F050901> datas, string status)
        {
            DateTime now = DateTime.Now;
            List<F050901> entities = new List<F050901>();
            List<string> conditionColumns = new List<string> { "DC_CODE", "GUP_CODE", "CUST_CODE", "CONSIGN_NO" };
            List<string> specifiedColumns = new List<string>();

            if (status == "3")
            {
                entities = datas.Select(x => new F050901
                {
                    UPD_DATE = now,
                    UPD_STAFF = Current.Staff,
                    UPD_NAME = Current.StaffName,
                    STATUS = x.STATUS,
                    PAST_DATE = x.PAST_DATE,
                    SEND_DATE = x.SEND_DATE != null ? x.SEND_DATE : x.PAST_DATE,
                    RESULT = x.RESULT,
                    DC_CODE = x.DC_CODE,
                    GUP_CODE = x.GUP_CODE,
                    CUST_CODE = x.CUST_CODE,
                    CONSIGN_NO = x.CONSIGN_NO
                }).ToList();

                specifiedColumns = new List<string> { "UPD_DATE", "UPD_STAFF", "UPD_NAME", "STATUS", "PAST_DATE", "SEND_DATE", "RESULT" };
            }
            else if (status != "4")
            {
                entities = datas.Select(x => new F050901
                {
                    UPD_DATE = now,
                    UPD_STAFF = Current.Staff,
                    UPD_NAME = Current.StaffName,
                    STATUS = x.STATUS,
                    PAST_DATE = x.PAST_DATE,
                    SEND_DATE = x.SEND_DATE,
                    RESULT = x.RESULT,
                    DC_CODE = x.DC_CODE,
                    GUP_CODE = x.GUP_CODE,
                    CUST_CODE = x.CUST_CODE,
                    CONSIGN_NO = x.CONSIGN_NO
                }).ToList();

                specifiedColumns = new List<string> { "UPD_DATE", "UPD_STAFF", "UPD_NAME", "STATUS", "PAST_DATE", "SEND_DATE", "RESULT" };
            }
            else
            {
                entities = datas.Select(x => new F050901
                {
                    UPD_DATE = now,
                    UPD_STAFF = Current.Staff,
                    UPD_NAME = Current.StaffName,
                    STATUS = x.STATUS,
                    RESULT = x.RESULT,
                    DC_CODE = x.DC_CODE,
                    GUP_CODE = x.GUP_CODE,
                    CUST_CODE = x.CUST_CODE,
                    CONSIGN_NO = x.CONSIGN_NO
                }).ToList();

                specifiedColumns = new List<string> { "UPD_DATE", "UPD_STAFF", "UPD_NAME", "STATUS", "RESULT" };
            }

            SqlBulkSpecifiedUpdateForAnyCondition(
                entities, "F050901",
                conditionColumns,
                specifiedColumns);
        }

        /// <summary>
		/// 從尚未編輯的派車單裡面找到明細所有的託運單號
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="wmsNos"></param>
		/// <returns></returns>
		public IQueryable<F050901> FromF700102GetConsignNos(string dcCode, string gupCode, string custCode, IEnumerable<string> distrCarNos)
        {
            // DISTINCT 是因為 WMS_NO 有可能儲存 DISTR_CAR_NO， JOIN 後資料就會重複，這邊只是要撈出該派車單號的所有託運單
            var sqlFormat = @"SELECT DISTINCT A.*
							  FROM F050901 A
								   JOIN F700102 B
									  ON     A.DC_CODE = B.DC_CODE
										 AND A.GUP_CODE = B.GUP_CODE
										 AND A.CUST_CODE = B.CUST_CODE
										 AND (A.WMS_NO = B.WMS_NO OR A.WMS_NO = B.DISTR_CAR_NO)
							 WHERE B.DC_CODE = @p0 AND B.GUP_CODE = @p1 AND B.CUST_CODE = @p2 {0}
							ORDER BY A.CONSIGN_NO";

            var paramList = new List<object> { dcCode, gupCode, custCode };
            var distrCarNosInSql = paramList.CombineSqlInParameters("AND B.DISTR_CAR_NO", distrCarNos);
            var sql = string.Format(sqlFormat, distrCarNosInSql);

            var result = SqlQuery<F050901>(sql, paramList.ToArray());

            return result;
        }

        public void UpdateData(string dcCode, string gupCode, string custCode, string consinNo, string wmsNo)
        {
            var parameters = new List<object>
            {
                DateTime.Now,
                Current.Staff,
                Current.StaffName,
                consinNo,
                dcCode,
                gupCode,
                custCode,
                wmsNo
            };

            var sql = $@" UPDATE F050901 
                       SET 
						   UPD_DATE =@p0,
						   UPD_STAFF = @p1, 
						   UPD_NAME = @p2,
						   CONSIGN_NO =@p3
							WHERE	    DC_CODE =@p4
									AND GUP_CODE =@p5
									AND CUST_CODE =@p6
									AND WMS_NO = @p7";

            ExecuteSqlCommand(sql, parameters.ToArray());
        }

        

        public IQueryable<HctShipReturn> GetHctShipReturns(HctShipReturnParam hctShipReturnParam)
        {
#if DEBUG
            var isTest = "1";
#else
			var isTest = "0";
#endif


            var parms = new List<object>
            {
              new SqlParameter("@p0", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };

            var sql = $@" SELECT A.DC_CODE,
                                 A.GUP_CODE,
                                 A.CUST_CODE,
                                 B.ALL_ID,
                                 B.WMS_ORD_NO,
                                 D.CUST_ORD_NO,
                                 A.CONSIGN_NO,
                                 ISNULL(E.LOGCENTER_ID,F.LOGCENTER_ID) CONTRACT_CUST_NO,
                                 '' RECEIVER_CODE,
                                 D.CONSIGNEE RECEIVER_NAME ,
                                 D.TEL RECEIVER_PHONE,
                                 D.TEL_1 RECEIVER_MOBILE,
                                 D.ADDRESS RECEIVER_ADDRESS,
                                 CASE WHEN L.PACKAGE_BOX_NO = '1' AND B.COLLECT_AMT >0 THEN B.COLLECT_AMT ELSE 0 END  COLLECT_AMT,
                                 '' EGAMT,CONVERT(varchar, @p0,112) SEND_DATE,
                                 G.ZIP_CODE SEND_CODE,
                                 '' ARRIVAL_CODE,
                                 '' EKAMT,CONVERT(varchar,A.BOXQTY) PIECES,
                                 '' ADD_PIECES,
                                 '' WEIGHT,
                                 '' EBAMT,
                                 '' ERAMT,
                                 '' ESAMT,
                                 '' EDAMT,
                                 '' ELAMT, 
                                 CASE WHEN B.COLLECT_AMT >0 THEN '41' ELSE '11' END  SUMMON_TYPE,
                                 CASE WHEN B.ROUND_PIECE ='1' THEN '6' ELSE '1' END ITEM_KIND,
                                 '1' ITEM_TYPE,
                                 '' ASSIGN_DATE,
                                 CASE D.DELV_PERIOD WHEN '1' THEN '09-12' WHEN '2' THEN '12-17' WHEN '3' THEN '17-20' ELSE '' END ASSIGN_TIME,
                                 '' SUPPLIER_CODE,   
                                 H.DC_NAME SUPPLIER_NAME,
                                 H.TEL SUPPLIER_PHONE,
                                 '' SUPPLIER_MOBILE,
                                 H.ADDRESS SUPPLIER_ADDRESS,
                                 CASE WHEN B.ROUND_PIECE ='1' THEN '(來回件)' + D.MEMO ELSE D.MEMO END MEMO,
                                 '' ESEL,
                                 '' EPRINT,
                                 '' RECEIVER_ZIP_CODE 
					FROM F050901 A
					JOIN F050801 B
					ON B.DC_CODE = A.DC_CODE
					AND B.GUP_CODE = A.GUP_CODE
					AND B.CUST_CODE = A.CUST_CODE
					AND B.WMS_ORD_NO = A.WMS_NO
					JOIN F05030101 C
					ON C.DC_CODE = B.DC_CODE
					AND C.GUP_CODE = B.GUP_CODE
					AND C.CUST_CODE = B.CUST_CODE
					AND C.WMS_ORD_NO = B.WMS_ORD_NO
					JOIN F050301 D
					ON D.DC_CODE = C.DC_CODE
					AND D.GUP_CODE = C.GUP_CODE
					AND D.CUST_CODE = C.CUST_CODE
					AND D.ORD_NO = C.ORD_NO
					LEFT JOIN F194710 E
					ON E.DC_CODE = A.DC_CODE
					AND E.GUP_CODE = A.GUP_CODE
					AND E.CUST_CODE = A.CUST_CODE
					AND E.LOG_ID = A.DELIVID_SEQ_NAME
					AND E.ISTEST ='{isTest}'
					LEFT JOIN F194710 F
					ON F.DC_CODE = A.DC_CODE
					AND F.GUP_CODE = '00'
					AND F.CUST_CODE = '00'
					AND F.LOG_ID = A.DELIVID_SEQ_NAME
					AND F.ISTEST ='{isTest}'
					JOIN F194704 G
					ON G.DC_CODE = B.DC_CODE
					AND G.GUP_CODE = B.GUP_CODE
					AND G.CUST_CODE = B.CUST_CODE
					AND G.ALL_ID = B.ALL_ID
					JOIN F1901 H
					ON H.DC_CODE = A.DC_CODE
					JOIN (
					SELECT DISTINCT DC_CODE,LOG_ID
					FROM F194710) I
					ON I.DC_CODE = A.DC_CODE
					AND I.LOG_ID = A.DELIVID_SEQ_NAME
					JOIN F700102 J
					  ON J.DC_CODE = B.DC_CODE
					 AND J.GUP_CODE = B.GUP_CODE
					 AND J.CUST_CODE = B.CUST_CODE
					 AND J.WMS_NO = B.WMS_ORD_NO
					JOIN F700101 K
					  ON K.DC_CODE = J.DC_CODE
					 AND K.DISTR_CAR_NO = J.DISTR_CAR_NO
                    JOIN F055001 L
                      ON L.DC_CODE = A.DC_CODE
                     AND L.GUP_CODE = A.GUP_CODE
                     AND L.CUST_CODE = A.CUST_CODE
                     AND L.WMS_ORD_NO = A.WMS_NO
                     AND L.PAST_NO = A.CONSIGN_NO
                     AND (A.BOXQTY = 1 OR (A.BOXQTY >1 AND L.PACKAGE_BOX_NO ='1')) --一件多箱取第一箱回檔
					 WHERE A.DISTR_EDI_STATUS ='0' --取得配送商未回檔資料
					 	AND B.STATUS IN('1','2','5','6') --必須狀態為包裝完
					 	AND K.HAVE_WMS_NO='1' --有單派車
					 	AND K.STATUS <>'9' -- 非取消派車單
					 	AND J.DISTR_USE ='01' --正物流 
                      ";//一定要換行不然下面篩選條件會被註解掉

            //客戶代號
            if (!string.IsNullOrWhiteSpace(hctShipReturnParam.CustomerId))
            {
                sql += " AND ISNULL(E.LOGCENTER_ID,F.LOGCENTER_ID) = @p" + parms.Count;
                parms.Add(hctShipReturnParam.CustomerId);
            }

            //物流中心
            if (!string.IsNullOrWhiteSpace(hctShipReturnParam.DcCode))
            {
                sql += " AND A.DC_CODE = @p" + parms.Count;
                parms.Add(hctShipReturnParam.DcCode);
            }
            //業主
            if (!string.IsNullOrWhiteSpace(hctShipReturnParam.GupCode))
            {
                sql += " AND A.GUP_CODE = @p" + parms.Count;
                parms.Add(hctShipReturnParam.GupCode);
            }
            //貨主
            if (!string.IsNullOrWhiteSpace(hctShipReturnParam.CustCode))
            {
                sql += " AND A.CUST_CODE = @p" + parms.Count;
                parms.Add(hctShipReturnParam.CustCode);
            }
            //通路
            if (!string.IsNullOrWhiteSpace(hctShipReturnParam.Channel))
            {
                sql += " AND D.CHANNEL = @p" + parms.Count;
                parms.Add(hctShipReturnParam.Channel);
            }

            //配送商
            if (!string.IsNullOrWhiteSpace(hctShipReturnParam.AllId))
            {
                sql += " AND D.ALL_ID = @p" + parms.Count;
                parms.Add(hctShipReturnParam.AllId);
            }
            //配次
            if (!string.IsNullOrWhiteSpace(hctShipReturnParam.DelvTimes))
            {
                sql += " AND J.DELV_TIMES = @p" + parms.Count;
                parms.Add(hctShipReturnParam.DelvTimes);
            }
            //訂單日期-起
            if (hctShipReturnParam.OrdSDate.HasValue)
            {
                sql += " AND D.ORD_DATE >= @p" + parms.Count;
                parms.Add(hctShipReturnParam.OrdSDate);
            }
            //訂單日期-迄
            if (hctShipReturnParam.OrdEDate.HasValue)
            {
                sql += " AND D.ORD_DATE <= @p" + parms.Count;
                parms.Add(hctShipReturnParam.OrdEDate);
            }
            //出貨日期-起
            if (hctShipReturnParam.DelvSDate.HasValue)
            {
                sql += " AND B.DELV_DATE >= @p" + parms.Count;
                parms.Add(hctShipReturnParam.DelvSDate);
            }

            //出貨日期-迄
            if (hctShipReturnParam.DelvEDate.HasValue)
            {
                //delvEDate = delvEDate.Value.AddDays(1);
                sql += " AND B.DELV_DATE <= @p" + parms.Count;
                parms.Add(hctShipReturnParam.DelvEDate);
            }

            var result = SqlQuery<HctShipReturn>(sql, parms.ToArray());

            return result;
        }

        public IQueryable<KTJShipReturn> GetKTJShipReturns(HctShipReturnParam hctShipReturnParam)
        {
#if DEBUG
            var isTest = "1";
#else
			var isTest = "0";
#endif


            var parms = new List<object>
            {
              new SqlParameter("@p0", DateTime.Now) {SqlDbType = SqlDbType.DateTime2}
            };

            var sql = $@" 
				SELECT A.DC_CODE,
                       A.GUP_CODE,
					   A.CUST_CODE,
					   B.ALL_ID,
					   B.WMS_ORD_NO,
					   D.CUST_ORD_NO,                                                   --出貨單號
					   A.CONSIGN_NO,                                                    --託運單號
					   ISNULL (E.LOGCENTER_ID, F.LOGCENTER_ID) CONTRACT_CUST_NO,           --客戶編號
					   D.CONSIGNEE RECEIVER_NAME,                                      --收件人姓名
					   D.TEL RECEIVER_PHONE,                                           --收件人電話
					   D.ADDRESS RECEIVER_ADDRESS,                                     --收件人地址
					   CASE
						  WHEN L.PACKAGE_BOX_NO = '1' AND B.COLLECT_AMT > 0
						  THEN
							 B.COLLECT_AMT
						  ELSE
							 0
					   END
						  COLLECT_AMT,                                                  --代收金額
					   CONVERT (varchar, @p0, 112) SEND_DATE,                         --出貨日期
					   G.ZIP_CODE SEND_CODE,                                             --集貨站
					   CONVERT (varchar, A.BOXQTY) PIECES,                                         --件數
					   D.DELV_PERIOD  ASSIGN_TIME,                                     --配送時段
					   H.DC_NAME SUPPLIER_NAME,                                        --寄貨人名稱
					   H.TEL SUPPLIER_PHONE,                                           --寄件人電話
					   H.ADDRESS SUPPLIER_ADDRESS,                                     --寄件人地址
					   D.MEMO,                                                            --備註
					   '' RECEIVER_ZIP_CODE,                                         --收件人郵遞區號
					   B.A_ARRIVAL_DATE,                                                --指送日期
					   '' DELV_ORD_NO,                                                  --出貨單號
					   '' VOLUME_QTY,                                                     --才數
					   H.ZIP_CODE SUPPLIER_ZIP                                         --寄件人郵區
				  FROM F050901 A
					   JOIN F050801 B
						  ON     B.DC_CODE = A.DC_CODE
							 AND B.GUP_CODE = A.GUP_CODE
							 AND B.CUST_CODE = A.CUST_CODE
							 AND B.WMS_ORD_NO = A.WMS_NO
					   JOIN F05030101 C
						  ON     C.DC_CODE = B.DC_CODE
							 AND C.GUP_CODE = B.GUP_CODE
							 AND C.CUST_CODE = B.CUST_CODE
							 AND C.WMS_ORD_NO = B.WMS_ORD_NO
					   JOIN F050301 D
						  ON     D.DC_CODE = C.DC_CODE
							 AND D.GUP_CODE = C.GUP_CODE
							 AND D.CUST_CODE = C.CUST_CODE
							 AND D.ORD_NO = C.ORD_NO
					   LEFT JOIN F194710 E
						  ON     E.DC_CODE = A.DC_CODE
							 AND E.GUP_CODE = A.GUP_CODE
							 AND E.CUST_CODE = A.CUST_CODE
							 AND E.LOG_ID = A.DELIVID_SEQ_NAME
							 AND E.ISTEST = '{isTest}'
					   LEFT JOIN F194710 F
						  ON     F.DC_CODE = A.DC_CODE
							 AND F.GUP_CODE = '00'
							 AND F.CUST_CODE = '00'
							 AND F.LOG_ID = A.DELIVID_SEQ_NAME
							 AND F.ISTEST = '{isTest}'
					   JOIN F194704 G
						  ON     G.DC_CODE = B.DC_CODE
							 AND G.GUP_CODE = B.GUP_CODE
							 AND G.CUST_CODE = B.CUST_CODE
							 AND G.ALL_ID = B.ALL_ID
					   JOIN F1901 H ON H.DC_CODE = A.DC_CODE
					   JOIN (SELECT DISTINCT DC_CODE, LOG_ID
							   FROM F194710) I
						  ON I.DC_CODE = A.DC_CODE AND I.LOG_ID = A.DELIVID_SEQ_NAME
					   JOIN F700102 J
						  ON     J.DC_CODE = B.DC_CODE
							 AND J.GUP_CODE = B.GUP_CODE
							 AND J.CUST_CODE = B.CUST_CODE
							 AND J.WMS_NO = B.WMS_ORD_NO
					   JOIN F700101 K
						  ON K.DC_CODE = J.DC_CODE AND K.DISTR_CAR_NO = J.DISTR_CAR_NO
					   JOIN F055001 L
						  ON     L.DC_CODE = A.DC_CODE
							 AND L.GUP_CODE = A.GUP_CODE
							 AND L.CUST_CODE = A.CUST_CODE
							 AND L.WMS_ORD_NO = A.WMS_NO
							 AND L.PAST_NO = A.CONSIGN_NO
				 WHERE     A.DISTR_EDI_STATUS = '0'                               --取得配送商未回檔資料
					   AND B.STATUS IN ('1',
										'2',
										'5',
										'6')                                        --必須狀態為包裝完
					   AND K.HAVE_WMS_NO = '1'                                          --有單派車
					   AND K.STATUS <> '9'                                           -- 非取消派車單
					   AND J.DISTR_USE = '01'                                            --正物流
                      ";//一定要換行不然下面篩選條件會被註解掉

            //客戶代號
            if (!string.IsNullOrWhiteSpace(hctShipReturnParam.CustomerId))
            {
                sql += " AND ISNULL(E.LOGCENTER_ID,F.LOGCENTER_ID) = @p" + parms.Count;
                parms.Add(hctShipReturnParam.CustomerId);
            }

            //物流中心
            if (!string.IsNullOrWhiteSpace(hctShipReturnParam.DcCode))
            {
                sql += " AND A.DC_CODE = @p" + parms.Count;
                parms.Add(hctShipReturnParam.DcCode);
            }
            //業主
            if (!string.IsNullOrWhiteSpace(hctShipReturnParam.GupCode))
            {
                sql += " AND A.GUP_CODE = @p" + parms.Count;
                parms.Add(hctShipReturnParam.GupCode);
            }
            //貨主
            if (!string.IsNullOrWhiteSpace(hctShipReturnParam.CustCode))
            {
                sql += " AND A.CUST_CODE = @p" + parms.Count;
                parms.Add(hctShipReturnParam.CustCode);
            }
            //通路
            if (!string.IsNullOrWhiteSpace(hctShipReturnParam.Channel))
            {
                sql += " AND D.CHANNEL = @p" + parms.Count;
                parms.Add(hctShipReturnParam.Channel);
            }

            //配送商
            if (!string.IsNullOrWhiteSpace(hctShipReturnParam.AllId))
            {
                sql += " AND D.ALL_ID = @p" + parms.Count;
                parms.Add(hctShipReturnParam.AllId);
            }
            //配次
            if (!string.IsNullOrWhiteSpace(hctShipReturnParam.DelvTimes))
            {
                sql += " AND J.DELV_TIMES = @p" + parms.Count;
                parms.Add(hctShipReturnParam.DelvTimes);
            }
            //訂單日期-起
            if (hctShipReturnParam.OrdSDate.HasValue)
            {
                sql += " AND D.ORD_DATE >= @p" + parms.Count;
                parms.Add(hctShipReturnParam.OrdSDate);
            }
            //訂單日期-迄
            if (hctShipReturnParam.OrdEDate.HasValue)
            {
                sql += " AND D.ORD_DATE <= @p" + parms.Count;
                parms.Add(hctShipReturnParam.OrdEDate);
            }
            //出貨日期-起
            if (hctShipReturnParam.DelvSDate.HasValue)
            {
                sql += " AND B.DELV_DATE >= @p" + parms.Count;
                parms.Add(hctShipReturnParam.DelvSDate);
            }

            //出貨日期-迄
            if (hctShipReturnParam.DelvEDate.HasValue)
            {
                //delvEDate = delvEDate.Value.AddDays(1);
                sql += " AND B.DELV_DATE <= @p" + parms.Count;
                parms.Add(hctShipReturnParam.DelvEDate);
            }

            var result = SqlQuery<KTJShipReturn>(sql, parms.ToArray());

            return result;
        }

		public IQueryable<F050901> GetDatasByWmsOrdNos(string dcCode, string gupCode, string custCode, List<string> wmsOrdNos)
		{
			var sqlParameter = new List<SqlParameter>();
			sqlParameter.Add(new SqlParameter("@p0", dcCode) { SqlDbType = System.Data.SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p1", gupCode) { SqlDbType = System.Data.SqlDbType.VarChar });
			sqlParameter.Add(new SqlParameter("@p2", custCode) { SqlDbType = System.Data.SqlDbType.VarChar });

			var sql = $@" SELECT * FROM F050901
							WHERE DC_CODE = @p0
							AND GUP_CODE = @p1
							AND CUST_CODE = @p2 ";
			sql += sqlParameter.CombineSqlInParameters(" AND WMS_NO", wmsOrdNos, System.Data.SqlDbType.VarChar);

			return SqlQuery<F050901>(sql, sqlParameter.ToArray());
		}

	}
}
