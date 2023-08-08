using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.DBCore;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.Datas.F05
{
	public partial class F050001Repository : RepositoryBase<F050001, Wms3plDbContext, F050001Repository>
	{
    public IQueryable<F050001Data> GetF050001Datas(string dcCode, string gupCode, string custCode, string ordType, string ordSDate, string ordEDate, string arrivalSDate, string arrivalEDate, string ordNo, string custOrdNo, string consignee, string itemCode, string itemName, string sourceType, string retailCode, string carPeriod, string delvNo, string custCost, string fastDealType, string crossCode, string channel, string subChannel)
    {
			var parameters = new List<SqlParameter>
						{
                new SqlParameter("@p0", dcCode) { SqlDbType = SqlDbType.VarChar },
                new SqlParameter("@p1", gupCode) { SqlDbType = SqlDbType.VarChar },
                new SqlParameter("@p2", custCode) { SqlDbType = SqlDbType.VarChar },
                new SqlParameter("@p3", ordType) { SqlDbType = SqlDbType.Char },
                new SqlParameter("@p4", ordSDate) { SqlDbType = SqlDbType.DateTime2 },
                new SqlParameter("@p5", ordEDate) { SqlDbType = SqlDbType.DateTime2 }
            };

			string b2bColumns = string.Empty;
			string b2bJoinFrom = string.Empty;
			if (ordType == "0")
			{
				b2bColumns = " , a.RETAIL_CODE , j.RETAIL_NAME , n.NAME CAR_PERIOD , k.DELV_NO , k.DELV_WAY ";
				b2bJoinFrom = $@" 
         LEFT JOIN F1909 i ON i.GUP_CODE = a.GUP_CODE AND i.CUST_CODE = a.CUST_CODE
				 LEFT JOIN F1910 j ON j.GUP_CODE = a.GUP_CODE AND j.CUST_CODE = CASE WHEN i.ALLOWGUP_RETAILSHARE ='1' THEN '0' ELSE a.CUST_CODE END AND j.RETAIL_CODE = a.RETAIL_CODE
				 LEFT JOIN F19471601 k ON k.DC_CODE = a.DC_CODE AND k.GUP_CODE = a.GUP_CODE AND k.CUST_CODE = a.CUST_CODE AND k.RETAIL_CODE = a.RETAIL_CODE
				 LEFT JOIN F194716 m ON m.DC_CODE = k.DC_CODE AND m.GUP_CODE = k.GUP_CODE AND m.CUST_CODE = k.CUST_CODE AND m.DELV_NO = k.DELV_NO				
         LEFT JOIN VW_F000904_LANG n ON n.TOPIC='F194716' AND n.SUBTOPIC='CAR_PERIOD' AND n.VALUE = m.CAR_PERIOD AND n.LANG = '{Current.Lang}' ";
			}

			string sql = $@"SELECT DISTINCT
                                                a.ORD_NO,
                                                a.CUST_ORD_NO,
                                                a.ORD_TYPE,
                                                a.ORD_DATE,
                                                a.PROC_FLAG,
                                                a.CONSIGNEE,
                                                a.ARRIVAL_DATE,
                                                a.BATCH_NO,
                                                a.GUP_CODE,
                                                a.CUST_CODE,
                                                a.DC_CODE,
                                                a.SOURCE_TYPE,
                                                a.CUST_COST,
												                        a.FAST_DEAL_TYPE,
                                                e.NAME as CHANNEL,
                                                e1.NAME as SUBCHANNEL,
                                                c.SOURCE_NAME,
                                                CASE
                                                   WHEN a.CVS_TAKE = '1' THEN '超取'
                                                   WHEN a.SELF_TAKE = '1' THEN '自取'
                                                   ELSE '宅配'
                                                END
                                                   AS DELV_TYPE,
                                                g.ALL_COMP,
                                                h.COLLECT_AMT,
                                                a.CRT_DATE,
												(SELECT CROSS_NAME FROM F0001 WHERE CROSS_CODE = a.MOVE_OUT_TARGET AND CROSS_TYPE='01') MOVE_OUT_TARGET 
												{b2bColumns}
                                           FROM F050001 a
                                                LEFT JOIN F050002 b
                                                   ON     a.ORD_NO = b.ORD_NO
                                                      AND a.DC_CODE = b.DC_CODE
                                                      AND a.GUP_CODE = b.GUP_CODE
                                                      AND a.CUST_CODE = b.CUST_CODE
                                                LEFT JOIN F1903 z
                                                   ON     b.ITEM_CODE = z.ITEM_CODE
                                                      AND b.GUP_CODE = z.GUP_CODE
                                                      AND b.CUST_CODE = z.CUST_CODE
                                                LEFT JOIN F000902 c ON c.SOURCE_TYPE = a.SOURCE_TYPE
                                                LEFT JOIN VW_F000904_LANG e ON a.CHANNEL = e.VALUE and e.TOPIC='F050101' AND e.SUBTOPIC='CHANNEL' AND e.LANG = '{Current.Lang}'
                                                LEFT JOIN VW_F000904_LANG e1 ON a.SUBCHANNEL = e1.VALUE and e1.TOPIC='F050101' AND e1.SUBTOPIC='SUBCHANNEL' AND e1.LANG = '{Current.Lang}'
                                                LEFT JOIN F050304 f
                                                   ON     a.ORD_NO = f.ORD_NO
                                                      AND a.DC_CODE = f.DC_CODE
                                                      AND a.GUP_CODE = f.GUP_CODE
                                                      AND a.CUST_CODE = f.CUST_CODE
                                                LEFT JOIN F1947 g ON a.ALL_ID = g.ALL_ID AND a.DC_CODE = g.DC_CODE
                                                LEFT JOIN F050101 h
                                                   ON     a.ORD_NO = h.ORD_NO
                                                      AND a.DC_CODE = h.DC_CODE
                                                      AND a.GUP_CODE = h.GUP_CODE
                                                      AND a.CUST_CODE = h.CUST_CODE
												{b2bJoinFrom}
                                          WHERE     a.DC_CODE = @p0
                                                AND a.GUP_CODE = @p1
                                                AND a.CUST_CODE = @p2
                                                AND a.ORD_TYPE = @p3
                                                AND a.ORD_DATE >= @p4
                                                AND a.ORD_DATE <= @p5
                                                AND a.PROC_FLAG = '0' ";
			if (!string.IsNullOrWhiteSpace(arrivalSDate))
			{
				sql += " AND	a.ARRIVAL_DATE >= @p6 ";
				parameters.Add(new SqlParameter("@p6", arrivalSDate));
			}

			if (!string.IsNullOrWhiteSpace(arrivalEDate))
			{
				sql += " AND	a.ARRIVAL_DATE <= @p7 ";
				parameters.Add(new SqlParameter("@p7", arrivalEDate));
			}

			if (!string.IsNullOrWhiteSpace(ordNo))
			{
				sql += " AND	a.ORD_NO = @p8 ";
				parameters.Add(new SqlParameter("@p8", ordNo));
			}

			if (!string.IsNullOrWhiteSpace(custOrdNo))
			{
				sql += " AND	a.CUST_ORD_NO = @p9 ";
				parameters.Add(new SqlParameter("@p9", custOrdNo));
			}

			if (!string.IsNullOrWhiteSpace(itemCode))
			{
				sql += " AND	b.ITEM_CODE = @p10 ";
				parameters.Add(new SqlParameter("@p10", itemCode));
			}

			if (!string.IsNullOrWhiteSpace(consignee))
			{
				sql += " AND	a.CONSIGNEE = @p11 ";
				parameters.Add(new SqlParameter("@p11", AesCryptor.Current.Encode(consignee)));
			}

			if (!string.IsNullOrWhiteSpace(itemName))
			{
				sql += " AND	z.ITEM_NAME like @p12 ";
				parameters.Add(new SqlParameter("@p12", "%" + itemName + "%"));
			}

			if (!string.IsNullOrWhiteSpace(custCost))
			{
				sql += " AND	a.CUST_COST = @p13 ";
				parameters.Add(new SqlParameter("@p13", custCost));
			}

			if (!string.IsNullOrEmpty(sourceType))
			{
				if (sourceType == "01")
					sql += " AND a.SOURCE_TYPE is null ";
				else
				{
					sql += " AND a.SOURCE_TYPE = @p14";
					parameters.Add(new SqlParameter("@p14", sourceType));
				}
			}

			if (!string.IsNullOrWhiteSpace(fastDealType))
			{
				sql += " AND	a.FAST_DEAL_TYPE = @p18 ";
				parameters.Add(new SqlParameter("@p18", fastDealType));
			}

			if (!string.IsNullOrWhiteSpace(crossCode))
			{
				sql += " AND a.MOVE_OUT_TARGET = @p19 ";
				parameters.Add(new SqlParameter("@p19", crossCode));
			}

			if (ordType == "0")
			{
				string b2bCondition = string.Empty;

				if (!string.IsNullOrWhiteSpace(retailCode))
				{
					b2bCondition += " AND j.RETAIL_CODE = @p15";
					parameters.Add(new SqlParameter("@p15", retailCode));
				}

				if (!string.IsNullOrWhiteSpace(carPeriod))
				{
					b2bCondition += " AND m.CAR_PERIOD = @p16";
					parameters.Add(new SqlParameter("@p16", carPeriod));
				}

				if (!string.IsNullOrWhiteSpace(delvNo))
				{
					b2bCondition += " AND m.DELV_NO like @p17";
					parameters.Add(new SqlParameter("@p17", "%" + delvNo + "%"));
				}

				sql += b2bCondition;
			}

      if (!string.IsNullOrWhiteSpace(channel) && channel != "00")
      {
        sql += $" AND a.CHANNEL = @{parameters.Count} ";
        parameters.Add(new SqlParameter($"@{parameters.Count}", channel) { SqlDbType = SqlDbType.VarChar });
      }

      if (!string.IsNullOrWhiteSpace(subChannel) && subChannel != "00")
      {
        sql += $" AND a.SUBCHANNEL = @{parameters.Count} ";
        parameters.Add(new SqlParameter($"@{parameters.Count}", subChannel) { SqlDbType = SqlDbType.VarChar });
      }

      var result = SqlQuery<F050001Data>(sql, parameters.ToArray());

			return result;
		}

		public void UpdateZipCodeDc(string ordNo, string gupCode, string custCode, string zipCode, string dcCode, string typeId, string addressParse)
		{
			var param = new List<SqlParameter>
												{
																new SqlParameter("@p0", zipCode),
																new SqlParameter("@p1", dcCode),
																new SqlParameter("@p2", typeId),
																new SqlParameter("@p3", Current.Staff),
																new SqlParameter("@p4", Current.StaffName),
																new SqlParameter("@p5", addressParse ?? (object)DBNull.Value),
																new SqlParameter("@p6", ordNo),
																new SqlParameter("@p7", gupCode),
																new SqlParameter("@p8", custCode),
                                new SqlParameter("@p9", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 }
                        };

			string sql = @"
			            	Update F050001
			            	   Set ZIP_CODE=@p0, 
                                   DC_CODE=@p1, 
                                   TYPE_ID=@p2, 
                                   UPD_DATE=@p9, 
                                   UPD_STAFF=@p3, 
                                   UPD_NAME=@p4, 
                                   ADDRESS_PARSE=@p5
			            	 Where ORD_NO=@p6
			            	   And GUP_CODE=@p7
			            	   And CUST_CODE=@p8
			            ";

			ExecuteSqlCommand(sql, param.ToArray());
		}

		public void Delete(List<string> ordNos, string gupCode, string custCode, string dcCode)
		{
			var parameters = new List<object>
						{
								gupCode,
								custCode,
								dcCode
						};

			int paramStartIndex = parameters.Count;
			var inSql = parameters.CombineSqlInParameters("ORD_NO", ordNos, ref paramStartIndex);

			string sql = @"
				Delete 
				  From F050001
				 Where GUP_CODE=@p0
				   And CUST_CODE=@p1
				   And DC_CODE=@p2
				   And " + inSql;

			ExecuteSqlCommand(sql, parameters.ToArray());
		}

		public void SetStatus(string status, List<string> ordNos)
		{
			var parameters = new List<object> {
								status,
                DateTime.Now,
                Current.Staff,
								Current.StaffName
						};
			int paramStartIndex = parameters.Count;
			var inSql = parameters.CombineSqlInParameters("ORD_NO", ordNos, ref paramStartIndex);
			string sql = @"
				Update F050001 
                    Set PROC_FLAG=@p0,
                        UPD_DATE =@p1,   
			            UPD_STAFF = @p2,
                        UPD_NAME = @p3 
				Where " + inSql;

			ExecuteSqlCommand(sql, parameters.ToArray());
		}

		public void SetStatus(string newStatus, string oldStatus, List<string> limitDcList = null)
		{
			var param = new List<object>
												{
													newStatus,
													Current.Staff,
													Current.StaffName,
                          DateTime.Now,
                          oldStatus
												};

			string sql = @"Update F050001
				             Set PROC_FLAG=@p0, 
                                 UPD_STAFF = @p1, 
                                 UPD_NAME = @p2, 
                                 UPD_DATE = @p3
				           Where PROC_FLAG=@p4 AND CUST_COST NOT IN('MoveOut') ";
			if (limitDcList != null && limitDcList.Any())
			{
				sql += param.CombineSqlInParameters("AND DC_CODE ", limitDcList);
			}
			ExecuteSqlCommand(sql, param.ToArray());
		}

		public void DeleteF050001(string ordNo)
		{
			var sqlParams = new SqlParameter[]
			{
								 new SqlParameter("@p0", ordNo)
			};

			string sql = @"delete from  F050001 Where ORD_NO=@p0";

			ExecuteSqlCommand(sql, sqlParams.ToArray());
		}

		public void DeleteHasAllot()
		{
			var sql = @" DELETE X FROM
							F050001 X
							WHERE exists(
							SELECT A.DC_CODE,A.GUP_CODE,A.CUST_CODE,A.ORD_NO 
							FROM F050001 A
							JOIN F050301 B
							ON A.DC_CODE = B.DC_CODE
							AND A.GUP_CODE = B.GUP_CODE
							AND A.CUST_CODE = B.CUST_CODE
							AND A.ORD_NO = B.ORD_NO
							WHERE B.PROC_FLAG IN('1','9')
							AND X.DC_CODE = A.DC_CODE
							AND X.GUP_CODE = A.GUP_CODE
							AND X.CUST_CODE = A.CUST_CODE
							AND X.ORD_NO = A.ORD_NO) ";

			ExecuteSqlCommand(sql);
		}

		public void DeleteLackOrder(string gupCode, string custCode)
		{
			var parms = new List<SqlParameter>
						{
								new SqlParameter("@p0", gupCode),
								new SqlParameter("@p1", custCode)
						};


			var sql = @"DELETE X FROM F050001 X
							WHERE EXISTS
							(SELECT A.GUP_CODE,A.CUST_CODE,A.ORD_NO
							 FROM F050301 A
							 WHERE A.PROC_FLAG ='0'
							 AND A.GUP_CODE = @p0
							 AND A.CUST_CODE = @p1
							 AND X.GUP_CODE = A.GUP_CODE
							 AND X.CUST_CODE = A.CUST_CODE
							 AND X.ORD_NO = A.ORD_NO)
                         ";

			ExecuteSqlCommand(sql, parms.ToArray());
		}

		#region 查詢4小時未配庫的出貨單
		/// <summary>
		/// 為配庫訂單
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="ordType"></param>
		/// <param name="fastDealType"></param>
		/// <param name="custCost"></param>
		/// <param name="ordNo"></param>
		/// <param name="custOrdNo"></param>
		/// <param name="onlyShowMoreThanFourHours"></param>
		/// <returns></returns>
		public IQueryable<UndistributedOrder> GetUndistributedOrder(string dcCode, string gupCode, string custCode,
			string ordType, string fastDealType, string custCost, string ordNo, string custOrdNo, bool onlyShowMoreThanFourHours)
		{
			var param = new List<SqlParameter>
			{
				new SqlParameter("@p0",SqlDbType.VarChar){ Value = dcCode},
				new SqlParameter("@p1",SqlDbType.VarChar){ Value = gupCode},
				new SqlParameter("@p2",SqlDbType.VarChar){ Value = custCode},
        new SqlParameter("@p3",SqlDbType.DateTime2){ Value = DateTime.Now }
      };
			var sql = @"SELECT A.ORD_DATE ,
							ORD_NO ,
							CUST_ORD_NO ,
							SOURCE_NO ,
							(SELECT TOP(1) NAME  FROM F000904 WHERE TOPIC='F050101' AND SUBTOPIC='CUST_COST' AND VALUE = A.CUST_COST) CUST_COST_NAME ,
							(SELECT TOP(1) NAME  FROM F000904 WHERE TOPIC='F050101' AND SUBTOPIC='FAST_DEAL_TYPE' AND VALUE = A.FAST_DEAL_TYPE) FAST_DEAL_TYPE_NAME ,
							--MOVE_OUT_TARGET ,
							CRT_DATE,
							(CASE WHEN A.CRT_DATE < DateAdd(hour,-4,@p3) THEN '1' ELSE '0' END) MORE_THEN_FOUR_HOURS
						FROM F050001 A WHERE A.DC_CODE = @p0
							AND A.GUP_CODE = @p1
							AND A.CUST_CODE = @p2";

			if (!string.IsNullOrWhiteSpace(ordType))
			{
				sql += $" AND A.ORD_TYPE = @p{param.Count}";
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.Char) { Value = ordType });
			}

			if (!string.IsNullOrWhiteSpace(fastDealType))
			{
				sql += $" AND A.FAST_DEAL_TYPE = @p{param.Count}";
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.Char) { Value = fastDealType });
			}

			if (!string.IsNullOrWhiteSpace(custCost))
			{
				sql += $" AND A.CUST_COST = @p{param.Count}";
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.NVarChar) { Value = custCost });
			}

			if (!string.IsNullOrWhiteSpace(ordNo))
			{
				sql += $" AND A.ORD_NO = @p{param.Count}";
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.VarChar) { Value = ordNo });
			}

			if (!string.IsNullOrWhiteSpace(custOrdNo))
			{
				sql += $" AND A.CUST_ORD_NO = @p{param.Count}";
				param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.VarChar) { Value = custOrdNo });
			}

			if (onlyShowMoreThanFourHours)
			{
				sql += $" AND A.CRT_DATE < DateAdd(hour,-4,@p{param.Count})";
        param.Add(new SqlParameter("@p" + param.Count(), SqlDbType.DateTime2) { Value = DateTime.Now });
      }

			sql += " ORDER BY CRT_DATE";

			return SqlQueryWithSqlParameterSetDbType<UndistributedOrder>(sql, param.ToArray());
		}
        #endregion

        #region 跨庫出貨配庫補貨排程
        /// <summary>
		/// 未配庫跨庫出貨訂單商品訂購數
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="custCost"></param>
		/// <returns></returns>
        public IQueryable<MoveoutOrdItemQty> GetMoveOutOrdItemOty(String dcCode, String gupCode, String custCode)
        {
            var param = new List<SqlParameter>
            {
                new SqlParameter("@p0",SqlDbType.VarChar) {Value = dcCode},
                new SqlParameter("@p1",SqlDbType.VarChar) {Value = gupCode},
                new SqlParameter("@p2",SqlDbType.VarChar) {Value = custCode}
            };

            var sql = @"SELECT A.DC_CODE, 
                        A.GUP_CODE, 
                        A.CUST_CODE, 
                        B.ITEM_CODE, 
                        ISNULL(B.MAKE_NO,'') MAKE_NO, 
                        ISNULL(B.SERIAL_NO,'') SERIAL_NO, 
                        SUM(B.ORD_QTY) ORD_QTY 
                        FROM F050001 A 
                        JOIN F050002 B 
                        ON A.ORD_NO = B.ORD_NO 
                        WHERE A.DC_CODE = @p0 
                        AND A.GUP_CODE = @p1 
                        AND A.CUST_CODE = @p2 
                        AND A.CUST_COST = 'MoveOut'
                        GROUP BY A.DC_CODE, A.GUP_CODE, A.CUST_CODE, B.ITEM_CODE, ISNULL(B.MAKE_NO,'')
                        ORDER BY B.ITEM_CODE, ISNULL(B.MAKE_NO,'') DESC";

            return SqlQuery<MoveoutOrdItemQty>(sql, param.ToArray());
        }
		#endregion

		public string LockF050001()
		{
			var sql = @"Select Top 1 UPD_LOCK_TABLE_NAME From F0000 With(UPDLOCK,ROWLOCK) Where UPD_LOCK_TABLE_NAME='F050001';";
			return SqlQuery<string>(sql).FirstOrDefault();

		}

    public void LockNonAllotOrderStatus(string allotBatchNo, decimal ticketId, int maxRecord)
    {
      var limitDcSql = new List<string>();

      var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",Current.Staff){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1",Current.StaffName){SqlDbType = SqlDbType.NVarChar},
				new SqlParameter("@p2",allotBatchNo){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p3",ticketId){SqlDbType = SqlDbType.Decimal},
			};

      var sql = $@" UPDATE F050001
                   SET PROC_FLAG ='1',UPD_STAFF=@p0,UPD_NAME=@p1,ALLOT_BATCH_NO=@p2
                   FROM (
                         SELECT TOP ({maxRecord}) DC_CODE,GUP_CODE,CUST_CODE,ORD_NO
                           FROM F050001 
                          WHERE CUST_COST NOT IN('MoveOut') AND PROC_FLAG='0' AND TICKET_ID = @p3
                          ORDER BY FAST_DEAL_TYPE DESC,ORD_NO ASC) A
                   WHERE F050001.DC_CODE = A.DC_CODE
                     AND F050001.GUP_CODE = A.GUP_CODE
                     AND F050001.CUST_CODE = A.CUST_CODE
                     AND F050001.ORD_NO = A.ORD_NO
                   ";

      ExecuteSqlCommandWithSqlParameterSetDbType(sql, parms.ToArray());
    }

    public IQueryable<string> CheckHasAllotOrdNos(List<string> ordNos)
		{
			var parms = new List<SqlParameter>();
			
			var sql = $@" SELECT ORD_NO
                      FROM F050001 
                     WHERE PROC_FLAG ='1'";
			if(ordNos.Any())
			{
				sql+= parms.CombineSqlInParameters("AND ORD_NO ", ordNos, SqlDbType.VarChar);
			}
			else
			{
				sql += " AND 1=0";
			}
			return SqlQuery<string>(sql, parms.ToArray());
		}
		public void LockNonAllotOrderStatusByOrdNos(string allotBatchNo, List<string> ordNos)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",Current.Staff){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1",Current.StaffName){SqlDbType = SqlDbType.NVarChar},
				new SqlParameter("@p2",allotBatchNo){SqlDbType = SqlDbType.VarChar},
			};
			var sql = $@" UPDATE F050001
                    SET PROC_FLAG ='1',UPD_STAFF=@p0,UPD_NAME=@p1,ALLOT_BATCH_NO=@p2
                    WHERE 1=1 ";
			if (ordNos.Any())
				sql += parms.CombineSqlInParameters(" AND ORD_NO ", ordNos, SqlDbType.VarChar);
			else
				sql += " AND 1 = 0 ";

			ExecuteSqlCommandWithSqlParameterSetDbType(sql, parms.ToArray());
		}

		public IQueryable<F050001> GetOrdersByAllotBatchNo(string allotBatchNo)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",allotBatchNo){SqlDbType = SqlDbType.VarChar},
			};
			var sql = @" SELECT *
                     FROM F050001 
                    WHERE ALLOT_BATCH_NO = @p0 ";
			return SqlQuery<F050001>(sql, parms.ToArray());
		}
		public IQueryable<F050001> GetOrdersByOrdNos(List<string> ordNos)
		{
			var parms = new List<SqlParameter>();
			
			var sql = @" SELECT *
                     FROM F050001 
                    WHERE  1= 1 ";

			if (ordNos.Any())
				sql += parms.CombineSqlInParameters(" AND ORD_NO ", ordNos, SqlDbType.VarChar);
			else
				sql += " AND 1 = 0 ";
			return SqlQuery<F050001>(sql, parms.ToArray());
		}
		public void UnLockByAllotBatchNo(string allotBatchNo)
		{
			var parms = new List<SqlParameter>
			{
				new SqlParameter("@p0",Current.Staff){SqlDbType = SqlDbType.VarChar},
				new SqlParameter("@p1",Current.StaffName){SqlDbType = SqlDbType.NVarChar},
				new SqlParameter("@p2",allotBatchNo){SqlDbType = SqlDbType.VarChar},
        new SqlParameter("@p3", DateTime.Now) { SqlDbType = SqlDbType.DateTime2 }
      };
			var sql = @"  UPDATE F050001
                    SET PROC_FLAG='0',UPD_DATE = @p3, UPD_STAFF=@p0 , UPD_NAME=@p1,ALLOT_BATCH_NO = NULL
                    WHERE ALLOT_BATCH_NO = @p2 ";
			ExecuteSqlCommandWithSqlParameterSetDbType(sql, parms.ToArray());
		}

	}
}
