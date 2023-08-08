using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.DataCommon
{
	/// <summary>
	/// 提供 List'1 SqlParamter, List'1 object 串條件的擴充方法
	/// </summary>
	public static class SqlParamterListExtension
	{
        internal static char PreDbParamSymbol { get; set; }
        /// <summary>
        /// 產生格式化的SQL，並將參數加入倒參數清單中的擴充方法，若輸入參數是 null or string.Empty 則回傳空字串。
        /// </summary>
        /// <param name="parameterList"></param>
        /// <param name="partialSql"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public static string CombineNotNullOrEmpty(this List<object> parameterList, string partialSql, string param)
		{
			if (string.IsNullOrEmpty(param))
			{
				return string.Empty;
			}
			return Combine<object>(parameterList, partialSql, new List<object> { param });
		}

		/// <summary>
		/// 產生格式化的SQL，並將參數加入倒參數清單中的擴充方法，若輸入參數是 null or string.Empty 則回傳空字串。
		/// </summary>
		/// <param name="parameterList"></param>
		/// <param name="partialSql"></param>
		/// <param name="params"></param>
		/// <returns></returns>
		public static string CombineNotNullOrEmpty(this List<SqlParameter> parameterList, string partialSql, string param)
		{
			if (string.IsNullOrEmpty(param))
			{
				return string.Empty;
			}
			return Combine<SqlParameter>(parameterList, partialSql,
										 new List<SqlParameter> { new SqlParameter(PreDbParamSymbol + "p" + parameterList.Count, param) { SqlDbType = SqlDbType.VarChar } });
		}

		/// <summary>
		/// 產生格式化的SQL，並將參數加入倒參數清單中的擴充方法。
		/// </summary>
		/// <param name="parameterList"></param>
		/// <param name="partialSql"></param>
		/// <param name="params"></param>
		/// <returns></returns>
		public static string Combine(this List<object> parameterList, string partialSql, params object[] @params)
		{
			return Combine<object>(parameterList, partialSql, @params);
		}

		/// <summary>
		/// 產生格式化的SQL，並將參數加入倒參數清單中的擴充方法。
		/// </summary>
		/// <typeparam name="TParam"></typeparam>
		/// <param name="parameterList"></param>
		/// <param name="partialSql"></param>
		/// <param name="params"></param>
		/// <returns></returns>
		public static string Combine<TParam>(this List<SqlParameter> parameterList, string partialSql, params TParam[] @params)
		{
			var count = parameterList.Count;
			if(typeof(TParam).Equals(typeof(string)))
				return Combine<SqlParameter>(parameterList,
										 partialSql,
										 @params.Select((item, index) => new SqlParameter($"{PreDbParamSymbol}p" + (count + index), item) {SqlDbType = SqlDbType.VarChar}));
			else
			return Combine<SqlParameter>(parameterList,
									 partialSql,
									 @params.Select((item, index) => new SqlParameter($"{PreDbParamSymbol}p" + (count + index), item)));
		}

		private static string Combine<TParam>(this List<TParam> parameterList, string partialSql, IEnumerable<TParam> @params)
		{
			var count = parameterList.Count;
			var paramIndexes = @params.Select((item, index) => (object)(count + index)).ToArray();
			parameterList.AddRange(@params);
			return string.Format(partialSql, paramIndexes);
		}

		/// <summary>
		/// 支援超過1000個參數。回傳 (fieldName IN (:p4,:p5,....,:p1003) OR fieldName IN (:p1004,:p1005,....,:p2003),...)，其中參數索引是由 paramStartIndex 決定開始
		/// </summary>
		/// <typeparam name="TParam"></typeparam>
		/// <param name="parameters"></param>
		/// <param name="paritalSqlAndFieldName">要使用 IN 的欄位名稱(可寫 A.ORD_NO 或 A.STATUS = '123' AND A.ORD_NO ，以最後一個欄位為IN的欄位名稱)</param>
		/// <param name="inParameters"></param>
		/// <returns></returns>
		public static string CombineNotNullOrEmptySqlInParameters<TParam>(this List<object> parameters, string paritalSqlAndFieldName, IEnumerable<TParam> inParameters)
		{
			if (!inParameters.Any() || inParameters == null)
			{
				return string.Empty;
			}
			int index = parameters.Count;
			return parameters.CombineSqlInParameters<TParam>(paritalSqlAndFieldName, inParameters, ref index);
		}

		/// <summary>
		/// 支援超過1000個參數。回傳 (fieldName IN (:p4,:p5,....,:p1003) OR fieldName IN (:p1004,:p1005,....,:p2003),...)，其中參數索引是由 paramStartIndex 決定開始
		/// </summary>
		/// <typeparam name="TParam"></typeparam>
		/// <param name="parameters"></param>
		/// <param name="paritalSqlAndFieldName">要使用 IN 的欄位名稱(可寫 A.ORD_NO 或 A.STATUS = '123' AND A.ORD_NO ，以最後一個欄位為IN的欄位名稱)</param>
		/// <param name="inParameters"></param>
		/// <returns></returns>
		public static string CombineSqlInParameters<TParam>(this List<object> parameters, string paritalSqlAndFieldName, IEnumerable<TParam> inParameters)
		{
			int index = parameters.Count;
			return parameters.CombineSqlInParameters<TParam>(paritalSqlAndFieldName, inParameters, ref index);
		}

		/// <summary>
		/// 支援超過1000個參數。回傳 (fieldName IN (:p4,:p5,....,:p1003) OR fieldName IN (:p1004,:p1005,....,:p2003),...)，其中參數索引是由 paramStartIndex 決定開始
		/// </summary>
		/// <typeparam name="TParam"></typeparam>
		/// <param name="parameters"></param>
		/// <param name="paritalSqlAndFieldName">要使用 IN 的欄位名稱(可寫 A.ORD_NO 或 A.STATUS = '123' AND A.ORD_NO ，以最後一個欄位為IN的欄位名稱)</param>
		/// <param name="inParameters"></param>
		/// <param name="paramStartIndex">:p 的開始值</param>
		/// <returns></returns>
		public static string CombineSqlInParameters<TParam>(this List<object> parameters, string paritalSqlAndFieldName,
			IEnumerable<TParam> inParameters, ref int paramStartIndex)
		{
			return parameters.CombineSqlInParameters<TParam>(string.Empty, paritalSqlAndFieldName, inParameters, ref paramStartIndex, true);
		}

		/// <summary>
		/// 支援超過1000個參數。回傳 (fieldName IN (:p4,:p5,....,:p1003) OR fieldName IN (:p1004,:p1005,....,:p2003),...)，其中參數索引是由 paramStartIndex 決定開始
		/// </summary>
		/// <typeparam name="TParam"></typeparam>
		/// <param name="parameters"></param>
		/// <param name="paritalSql">部分固定的SQL，例如可寫 AND </param>
		/// <param name="fieldName">固定欄位，例如可寫 NVL(BOUNDLE_ITEM_CODE ,ITEM_CODE)</param>
		/// <param name="inParameters"></param>
		/// <param name="paramStartIndex"></param>
		/// <returns></returns>
		public static string CombineSqlInParameters<TParam>(this List<object> parameters, string paritalSql, string fieldName,
			IEnumerable<TParam> inParameters, ref int paramStartIndex)
		{
			return parameters.CombineSqlInParameters<TParam>(paritalSql, fieldName, inParameters, ref paramStartIndex, true);
		}

		/// <summary>
		/// 支援超過1000個參數。回傳 (fieldName NOT IN (:p4,:p5,....,:p1003) AND fieldName NOT IN (:p1004,:p1005,....,:p2003),...)，其中參數索引是由 paramStartIndex 決定開始
		/// </summary>
		/// <typeparam name="TParam"></typeparam>
		/// <param name="parameters"></param>
		/// <param name="paritalSqlAndFieldName">要使用 NOT IN 的欄位名稱(可寫 A.ORD_NO 或 A.STATUS = '123' AND A.ORD_NO ，以最後一個欄位為NOT IN的欄位名稱)</param>
		/// <param name="inParameters"></param>
		/// <returns></returns>
		public static string CombineSqlNotInParameters<TParam>(this List<object> parameters, string paritalSqlAndFieldName, 
			IEnumerable<TParam> inParameters)
		{
			int index = parameters.Count;
			return CombineSqlNotInParameters(parameters, paritalSqlAndFieldName, inParameters, ref index);
		}

		/// <summary>
		/// 支援超過1000個參數。回傳 (fieldName IN (:p4,:p5,....,:p1003) OR fieldName IN (:p1004,:p1005,....,:p2003),...)，其中參數索引是由 paramStartIndex 決定開始
		/// </summary>
		/// <typeparam name="TParam"></typeparam>
		/// <param name="parameters"></param>
		/// <param name="paritalSql">部分固定的SQL，例如可寫 AND </param>
		/// <param name="fieldName">固定欄位，例如可寫 NVL(BOUNDLE_ITEM_CODE ,ITEM_CODE)</param>
		/// <param name="inParameters"></param>
		/// <returns></returns>
		public static string CombineSqlInParameters<TParam>(this List<object> parameters, string paritalSql, string fieldName,
			IEnumerable<TParam> inParameters)
		{
			int index = parameters.Count;
			return CombineSqlInParameters(parameters, paritalSql, fieldName, inParameters, ref index);
		}

		/// <summary>
		/// 支援超過1000個參數。回傳 (fieldName NOT IN (:p4,:p5,....,:p1003) AND fieldName NOT IN (:p1004,:p1005,....,:p2003),...)，其中參數索引是由 paramStartIndex 決定開始
		/// </summary>
		/// <typeparam name="TParam"></typeparam>
		/// <param name="parameters"></param>
		/// <param name="paritalSqlAndFieldName">要使用 NOT IN 的欄位名稱(可寫 A.ORD_NO 或 A.STATUS = '123' AND A.ORD_NO ，以最後一個欄位為NOT IN的欄位名稱)</param>
		/// <param name="inParameters"></param>
		/// <param name="paramStartIndex">:p 的開始值</param>
		/// <returns></returns>
		public static string CombineSqlNotInParameters<TParam>(this List<object> parameters, string paritalSqlAndFieldName,
			IEnumerable<TParam> inParameters, ref int paramStartIndex)
		{
			return parameters.CombineSqlInParameters<TParam>(string.Empty, paritalSqlAndFieldName, inParameters, ref paramStartIndex, false);
		}


    /// <summary>
    /// 支援超過1000個參數。回傳 (fieldName NOT IN (:p4,:p5,....,:p1003) AND fieldName NOT IN (:p1004,:p1005,....,:p2003),...)，其中參數索引是由 paramStartIndex 決定開始
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    /// <param name="parameters"></param>
    /// <param name="paritalSql">部分固定的SQL，例如可寫 AND </param>
    /// <param name="fieldName">固定欄位，例如可寫 NVL(BOUNDLE_ITEM_CODE ,ITEM_CODE)</param>
    /// <param name="inParameters"></param>
    /// <param name="paramStartIndex"></param>
    /// <returns></returns>
    public static string CombineSqlNotInParameters<TParam>(this List<object> parameters, string paritalSql, string fieldName,
			IEnumerable<TParam> inParameters, ref int paramStartIndex)
		{
			return parameters.CombineSqlInParameters<TParam>(paritalSql, fieldName, inParameters, ref paramStartIndex, false);
		}


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    /// <param name="parameters"></param>
    /// <param name="paritalSqlAndFieldName">可寫 A.ORD_NO 或 A.STATUS = '123' AND A.ORD_NO，以最後一個欄位為IN的欄位名稱 </param>
    /// <param name="inParameters"></param>
    /// <param name="paramStartIndex"></param>
    /// <param name="isIn"></param>
    /// <returns></returns>
    private static string CombineSqlInParameters<TParam>(this List<object> parameters, string paritalSql, string paritalSqlAndFieldName, IEnumerable<TParam> inParameters, ref int paramStartIndex, bool isIn = true)
		{
			const int InClauseMaximum = 1000;
			var sb = new StringBuilder();
			IEnumerable<TParam> inParamQuery = inParameters;
			int startIndex = paramStartIndex;

			// 取得要 IN 的欄位名稱與前面的條件 SQL 敘述
			string fieldName = null;
			string beginSql = null;
			if (string.IsNullOrEmpty(paritalSql))
			{
				var sqlOperators = paritalSqlAndFieldName.Split(new char[] { ' ', '\t' }).ToList();
				fieldName = sqlOperators.Last(x => !string.IsNullOrEmpty(x));
				int fieldNameIndex = sqlOperators.LastIndexOf(fieldName);
				beginSql = string.Join(" ", sqlOperators.Where((x, i) => i < fieldNameIndex));
			}
			else
			{
				fieldName = paritalSqlAndFieldName;
				beginSql = paritalSql;
			}

			// 前面的條件 SQL 敘述
			sb.Append(" ").Append(beginSql);

			if (inParameters == null || !inParameters.Any())
			{
				sb.Append((isIn) ? " 1=0 " : " 1=1 ");
				return sb.ToString();
			}

			// ( fieldName IN ) 部分
			sb.Append(" (");
			// 這個迴圈主要是組合這樣的範例: CUST_CODE IN (:p4,:p5,....,:p1003) OR CUST_CODE IN (:p1004,:p1005,....,:p2003)
			while (true)
			{
				// ex: CUST_CODE IN (:p4,:p5,....,:p1003)
				sb.Append(fieldName);
				#region NOT
				if (!isIn)
				{
					sb.Append(" NOT");
				}
				#endregion
				sb.Append(" IN (");
				#region sb.Append :p4,:p5,....,:p1003
				var paramStringQuery = inParamQuery.Select((param, index) => $"{PreDbParamSymbol}p{startIndex + index}")
													.Take(InClauseMaximum);

				sb.Append(string.Join(",", paramStringQuery));
				#endregion
				sb.Append(")");

				// 並將參數的內容帶入，並計數下一個開始的參數索引
				foreach (var inParam in inParamQuery.Take(InClauseMaximum))
				{
					startIndex++;
					parameters.Add(inParam);
				}

				// 檢查是否超過 IN Clause Maximum，有的話繼續用 OR 
				inParamQuery = inParamQuery.Skip(InClauseMaximum);

				if (inParamQuery.Any())
				{
					if (isIn)
					{
						sb.Append(" OR ");
					}
					else
					{
						sb.Append(" AND ");
					}
				}
				else
				{
					break;
				}
			}

			sb.Append(") ");

			paramStartIndex = startIndex;
			return sb.ToString();
		}


    #region 使用SqlParameter

    /// <summary>
    /// 支援超過1000個參數。回傳 (fieldName NOT IN (:p4,:p5,....,:p1003) AND fieldName NOT IN (:p1004,:p1005,....,:p2003),...)，其中參數索引是由 paramStartIndex 決定開始
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    /// <param name="parameters"></param>
    /// <param name="paritalSqlAndFieldName">要使用 NOT IN 的欄位名稱(可寫 A.ORD_NO 或 A.STATUS = '123' AND A.ORD_NO ，以最後一個欄位為NOT IN的欄位名稱)</param>
    /// <param name="inParameters"></param>
    /// <param name="sqlDbType">資料庫結構</param>
    /// <returns></returns>
    public static string CombineSqlNotInParameters<TParam>(this List<SqlParameter> parameters, string paritalSqlAndFieldName, IEnumerable<TParam> inParameters, SqlDbType sqlDbType)
    {
      int index = parameters.Count;
      return parameters.CombineSqlNotInParameters<TParam>(paritalSqlAndFieldName, inParameters, ref index, sqlDbType);
    }

    /// <summary>
    /// 支援超過1000個參數。回傳 (fieldName NOT IN (:p4,:p5,....,:p1003) AND fieldName NOT IN (:p1004,:p1005,....,:p2003),...)，其中參數索引是由 paramStartIndex 決定開始
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    /// <param name="parameters"></param>
    /// <param name="paritalSqlAndFieldName">要使用 NOT IN 的欄位名稱(可寫 A.ORD_NO 或 A.STATUS = '123' AND A.ORD_NO ，以最後一個欄位為NOT IN的欄位名稱)</param>
    /// <param name="inParameters"></param>
    /// <param name="paramStartIndex">:p 的開始值</param>
    /// <param name="sqlDbType">資料庫結構</param>
    /// <returns></returns>
    public static string CombineSqlNotInParameters<TParam>(this List<SqlParameter> parameters, string paritalSqlAndFieldName,
      IEnumerable<TParam> inParameters, ref int paramStartIndex, SqlDbType sqlDbType)
    {
      return parameters.CombineSqlInParametersUseDbType<TParam>(string.Empty, paritalSqlAndFieldName, inParameters, ref paramStartIndex, sqlDbType, false);
    }


    /// <summary>
    /// 支援超過1000個參數。回傳 (fieldName IN (:p4,:p5,....,:p1003) OR fieldName IN (:p1004,:p1005,....,:p2003),...)，其中參數索引是由 paramStartIndex 決定開始
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    /// <param name="parameters"></param>
    /// <param name="paritalSqlAndFieldName">要使用 IN 的欄位名稱(可寫 A.ORD_NO 或 A.STATUS = '123' AND A.ORD_NO ，以最後一個欄位為IN的欄位名稱)</param>
    /// <param name="inParameters"></param>
    /// <returns></returns>
    public static string CombineSqlInParameters<TParam>(this List<SqlParameter> parameters, string paritalSqlAndFieldName, IEnumerable<TParam> inParameters, SqlDbType sqlDbType)
    {
      int index = parameters.Count;
      return parameters.CombineSqlInParameters<TParam>(paritalSqlAndFieldName, inParameters, ref index, sqlDbType);
    }

    /// <summary>
    /// 支援超過1000個參數。回傳 (fieldName IN (:p4,:p5,....,:p1003) OR fieldName IN (:p1004,:p1005,....,:p2003),...)，其中參數索引是由 paramStartIndex 決定開始
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    /// <param name="parameters"></param>
    /// <param name="paritalSqlAndFieldName">要使用 IN 的欄位名稱(可寫 A.ORD_NO 或 A.STATUS = '123' AND A.ORD_NO ，以最後一個欄位為IN的欄位名稱)</param>
    /// <param name="inParameters"></param>
    /// <param name="paramStartIndex">:p 的開始值</param>
    /// <returns></returns>
    public static string CombineSqlInParameters<TParam>(this List<SqlParameter> parameters, string paritalSqlAndFieldName,
      IEnumerable<TParam> inParameters, ref int paramStartIndex, SqlDbType sqlDbType)
    {
      return parameters.CombineSqlInParametersUseDbType<TParam>(string.Empty, paritalSqlAndFieldName, inParameters, ref paramStartIndex, sqlDbType, true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    /// <param name="parameters"></param>
    /// <param name="paritalSqlAndFieldName">可寫 A.ORD_NO 或 A.STATUS = '123' AND A.ORD_NO，以最後一個欄位為IN的欄位名稱 </param>
    /// <param name="inParameters"></param>
    /// <param name="paramStartIndex"></param>
    /// <param name="isIn"></param>
    /// <returns></returns>
    private static string CombineSqlInParametersUseDbType<TParam>(this List<SqlParameter> parameters, string paritalSql, string paritalSqlAndFieldName, IEnumerable<TParam> inParameters, ref int paramStartIndex, SqlDbType sqlDbType, bool isIn = true)
    {
      const int InClauseMaximum = 1000;
      var sb = new StringBuilder();
      IEnumerable<TParam> inParamQuery = inParameters;
      int startIndex = paramStartIndex;

      // 取得要 IN 的欄位名稱與前面的條件 SQL 敘述
      string fieldName = null;
      string beginSql = null;
      if (string.IsNullOrEmpty(paritalSql))
      {
        var sqlOperators = paritalSqlAndFieldName.Split(new char[] { ' ', '\t' }).ToList();
        fieldName = sqlOperators.Last(x => !string.IsNullOrEmpty(x));
        int fieldNameIndex = sqlOperators.LastIndexOf(fieldName);
        beginSql = string.Join(" ", sqlOperators.Where((x, i) => i < fieldNameIndex));
      }
      else
      {
        fieldName = paritalSqlAndFieldName;
        beginSql = paritalSql;
      }

      // 前面的條件 SQL 敘述
      sb.Append(" ").Append(beginSql);

      if (inParameters == null || !inParameters.Any() || !inParameters.Any(o => o != null))
      {
        sb.Append((isIn) ? " 1=0 " : " 1=1 ");
        return sb.ToString();
      }

      // ( fieldName IN ) 部分
      sb.Append(" (");
      // 這個迴圈主要是組合這樣的範例: CUST_CODE IN (:p4,:p5,....,:p1003) OR CUST_CODE IN (:p1004,:p1005,....,:p2003)
      while (true)
      {
        // ex: CUST_CODE IN (:p4,:p5,....,:p1003)
        sb.Append(fieldName);
        #region NOT
        if (!isIn)
        {
          sb.Append(" NOT");
        }
        #endregion
        sb.Append(" IN (");
        #region sb.Append :p4,:p5,....,:p1003
        var paramStringQuery = inParamQuery.Select((param, index) => $"{PreDbParamSymbol}p{startIndex + index}")
                          .Take(InClauseMaximum);

        sb.Append(string.Join(",", paramStringQuery));
        #endregion
        sb.Append(")");

        // 並將參數的內容帶入，並計數下一個開始的參數索引
        foreach (var inParam in inParamQuery.Take(InClauseMaximum))
        {
          if (inParam == null)
            parameters.Add(new SqlParameter($"@p{startIndex}", DBNull.Value));
          else
            parameters.Add(new SqlParameter($"@p{startIndex}", inParam) { SqlDbType = sqlDbType } );

          startIndex++;
        }

        // 檢查是否超過 IN Clause Maximum，有的話繼續用 OR 
        inParamQuery = inParamQuery.Skip(InClauseMaximum);

        if (inParamQuery.Any())
        {
          if (isIn)
          {
            sb.Append(" OR ");
          }
          else
          {
            sb.Append(" AND ");
          }
        }
        else
        {
          break;
        }
      }

      sb.Append(") ");

      paramStartIndex = startIndex;
      return sb.ToString();
    }
    #endregion
  }
}
