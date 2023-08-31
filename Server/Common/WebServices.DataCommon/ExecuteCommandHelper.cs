using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System.Data.Entity;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Data;
using System.Data.EntityClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

using System.Data.Common;

namespace Wms3pl.WebServices.DataCommon
{
    public class ExecuteCommandHelper
    {
        internal static char PreDbParamSymbol { get; set; }

        
        /// <summary>
        /// 執行Insert、Update、Delete Sql Command
        /// 傳入Bind By Name Parameters(僅可用:p0、:p1、:p2或@p0、@p1、@p2等格式傳入!)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sqlcommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int ExecuteSqlCommand(DbContext context, string sqlcommand, bool isSqlParameterSetDbType = false, params object[] parameters)
        {

            if (parameters == null)
            {
                return ExecuteSqlCommandImpl(context, sqlcommand);
            }
            else
            {
                object[] parameterByPosition;
                parameterByPosition = ParameterByNameToByPosition(context, sqlcommand, parameters, isSqlParameterSetDbType);

                return ExecuteSqlCommandImpl(context, sqlcommand, parameterByPosition);
            }
        }

        public static int ExecuteSqlCommand(DbContext context, string sqlcommand)
        {
            return ExecuteSqlCommandImpl(context, sqlcommand);
        }

        ///// <summary>
        ///// 執行Select Sql Command
        ///// 傳入Bind By Name Parameters(僅可用:p0、:p1、:p2或@p0、@p1、@p2等格式傳入!)
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="context"></param>
        ///// <param name="sqlcommand"></param>
        ///// <param name="parameters"></param>
        ///// <returns></returns>
        //public static IQueryable<T> ExecuteQuery<T>(DbContext context, string sqlcommand, params SqlParameter[] parameters) where T : class
        //{
        //    return SqlQueryImpl<T>(context, sqlcommand, parameters);
        //}

        //public static IQueryable<T> ExecuteQuery<T>(DbContext context, string sqlcommand, params object[] parameters) where T : class
        //{
        //    return ExecuteQuery<T>(context, sqlcommand, false, parameters);
        //}

        /// <summary>
        /// 執行Select Sql Command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="sqlcommand"></param>
        /// <param name="isByPositionOrName">是否參數已依位置或名稱</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IQueryable<T> ExecuteQuery<T>(DbContext context, string sqlcommand, bool isSqlParameterSetDbType = false, params object[] parameters)
        {
            if (parameters != null)
            {
                object[] parameterByPosition;
                parameterByPosition = SqlQueryParameterByNameToByPosition(context, sqlcommand, parameters, isSqlParameterSetDbType);
                return SqlQueryImpl<T>(context, sqlcommand, parameterByPosition);
            }
            else
            {
                return SqlQueryImpl<T>(context, sqlcommand);
            }
        }

        public static IQueryable<T> ExecuteQuery<T>(DbContext context, string sqlcommand)
        {
            return SqlQueryImpl<T>(context, sqlcommand);
        }

        public static DataTable SqlQueryToDataTable(DbContext context, string sqlcommand, string tableName, params object[] parameters)
        {
            var dt = new DataTable(tableName);
            var conn = context.Database.GetDbConnection();
            var isHadOpened = false;

            parameters = SqlQueryParameterByNameToByPosition(context, sqlcommand, parameters);

            try
            {
                if (conn.State != ConnectionState.Open)
                    context.Database.OpenConnection();
                else
                    isHadOpened = true;

                var cmd = conn.CreateCommand();
                cmd.CommandText = sqlcommand;
                cmd.Parameters.AddRange(parameters);
                using (cmd)
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open && !isHadOpened)
                    context.Database.CloseConnection();

            }

            return dt;
        }

        /// <summary>
        /// 取得屬性值，其中會將預設三種欄位名稱帶系統值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pro"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static object GetPropertyValueForWms<T>(System.Reflection.PropertyInfo pro, T entity, bool isDefaultColumnModify = false)
        {
            if (!isDefaultColumnModify && pro.Name.Equals("CRT_DATE", StringComparison.OrdinalIgnoreCase))
            {
                var d = pro.GetValue(entity);
                if(d!=null && ((DateTime)d) == DateTime.MinValue)
                {
                    return DateTime.Now;
                }
                else
                {
                    return pro.GetValue(entity);
                }
            }
            else if (!isDefaultColumnModify &&  pro.Name.Equals("CRT_STAFF", StringComparison.OrdinalIgnoreCase))
                return Current.Staff ?? "System";
            else if (!isDefaultColumnModify &&  pro.Name.Equals("CRT_NAME", StringComparison.OrdinalIgnoreCase))
                return Current.StaffName ?? "System";
            else
                return pro.GetValue(entity);
        }

        public static DbType TypeToDbType(Type type)
        {
            var type1 = GetNonNullableType(type);
            return (DbType)Enum.Parse(typeof(DbType), type1.Name);
        }

        public static Type GetNonNullableType(Type type)
        {
            var type1 = Nullable.GetUnderlyingType(type);
            if (type1 == null)
                return type;
            else
                return type1;
        }

        /// <summary>
        /// 設定預設的值，若使用大量新增，會需要用到這個方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="defaultValueDict"></param>
        internal static void SetDefaultValueIfNullOrWhiteSpace(Type entityType, object entity, Dictionary<string, object> defaultValueDict)
        {
            foreach (var kvp in defaultValueDict)
            {
                var name = kvp.Key;
                var property = entityType.GetProperty(kvp.Key);
                if (property == null)
                    throw new Exception(string.Format("要設定預設值的{0}欄位不存在於{1}資料表中!", name, entityType.Name));

                var entityValue = property.GetValue(entity);
                if (IsSetDefaultValue(property.PropertyType, entityValue))
                    property.SetValue(entity, kvp.Value);
            }
        }

        private static bool IsSetDefaultValue(Type propertyType, object entityValue)
        {
            if (propertyType == typeof(string))
            {
                var value = Convert.ToString(entityValue);
                if (string.IsNullOrWhiteSpace(value))
                {
                    return true;
                }
            }
            else if (propertyType == typeof(DateTime))
            {
                var value = Convert.ToDateTime(entityValue);
                if (value == default(DateTime))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// ParameterByNameToByPosition
        /// </summary>
        /// <param name="sqlcommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static object[] ParameterByNameToByPosition(DbContext context, string sqlcommand, object[] parameters,bool isSqlParameterSetDbType=false)
        {
						if (parameters is SqlParameter[])
						{
				      if(!isSqlParameterSetDbType)
							{
									// 強制轉換nvarchar 轉varchar
									foreach (SqlParameter p in parameters)
									{
										if (p.SqlDbType == SqlDbType.NVarChar)
											p.SqlDbType = SqlDbType.VarChar;
										if (p.Value == null)
											p.Value = DBNull.Value;
									}
							}
							else
							{
								foreach (SqlParameter p in (SqlParameter[])parameters)
								{
									if (p.Value == null)
										p.Value = DBNull.Value;
								}
							}
							
							return parameters;
						}
						else
						{
							return ParameterByNameToByPositionImpl(context, sqlcommand, parameters,true);
						}
				}

        /// <summary>
        /// ParameterByNameToByPosition
        /// </summary>
        /// <param name="sqlcommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static object[] SqlQueryParameterByNameToByPosition(DbContext context, string sqlcommand, object[] parameters,bool isSqlParameterSetDbType = false)
        {
						if (!(parameters is SqlParameter[]))
						{
							parameters = ParameterByNameToByPositionImpl(context, sqlcommand, parameters, true);
						}
						// 只針對UPDATE 與 DELETE 才強制轉換
						else if(!isSqlParameterSetDbType && !sqlcommand.ToUpper().Contains("INSERT"))
						{
				      // 強制轉換nvarchar 轉varchar
							foreach(SqlParameter p in parameters)
							{
					       if(p.SqlDbType == SqlDbType.NVarChar)
						          p.SqlDbType = SqlDbType.VarChar;
								 if (p.Value == null)
										p.Value = DBNull.Value;
							}
						}
						else
						{
								foreach (SqlParameter p in parameters)
								{
									if (p.Value == null)
										p.Value = DBNull.Value;
								}
						}
						return parameters;
				}


        /// <summary>
        /// ParameterByNameToByPositionImpl
        /// </summary>
        /// <param name="sqlcommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object[] ParameterByNameToByPositionImpl(DbContext context, string sqlcommand, object[] parameters, bool retrunDbParameter = false)
        {
            var parameterByPosition = new List<object>();

            var paras = sqlcommand.Split(PreDbParamSymbol).ToList();
            paras.RemoveAt(0); //第一筆一定不是參數
            var parmIndex = 0;
            if (paras.Count() > 0)
            {
                foreach (var para in paras)
                {
                    //由P開頭
                    if (para.Substring(0, 1).ToUpper() == "P")
                    {
                        var i = 1;
                        var index = string.Empty;
                        while (true)
                        {
                            if (i == para.Length) break; //長度超過也離開
                            int num;
                            var s = para.Substring(i, 1);
                            var isInt = int.TryParse(s, out num);
                            if (isInt)
                            {
                                index = index + num;
                            }
                            else
                            {
                                break; //遇到不是數字字元也離開
                            }
                            i = i + 1;
                        }

                        //如果判定是正確的參數，則加入PatameterByPosition
                        if (index.Length > 0)
                        {
                            if (parameters is SqlParameter[])
                            {
                                var p = parameters.Where(a => ((SqlParameter)a).ParameterName.ToLower() == string.Format("{0}p{1}", PreDbParamSymbol, index)).FirstOrDefault();
                                if (p != null)
                                {
                                    if (retrunDbParameter)
                                    {
                                        // MSSQL是依Parameter的名稱不是順序
                                        if (!parameterByPosition.Contains(p))
                                        {
											                      if(p == null)
																						{
																								p = DBNull.Value;
																						}
                                            parameterByPosition.Add(p);
                                        }
                                    }
                                    else
                                    {
										                    var p1 = (SqlParameter)p;
																				if (p1.Value == null)
																				{
																					p1.Value = DBNull.Value;
																				}
																				
																				parameterByPosition.Add(p1.Value);
                                    }
                                }
                            }
                            else if (parmIndex < parameters.Length)
                            {
                                if (retrunDbParameter)
                                {
                                    var key = string.Format("{0}p{1}", PreDbParamSymbol, index);
                                    // MSSQL是依Parameter的名稱不是順序
                                    if (!parameterByPosition.Any(p => ((SqlParameter)p).ParameterName == key) )
                                    {
										                    if(parameters[parmIndex] == null)
																				{
																					parameters[parmIndex] = DBNull.Value;
																					parameterByPosition.Add(new SqlParameter(string.Format("{0}p{1}", PreDbParamSymbol, index), parameters[parmIndex]));

																				}
																				else if(parameters[parmIndex].GetType().Equals(typeof(string)))
																				{
																					parameterByPosition.Add(new SqlParameter(string.Format("{0}p{1}", PreDbParamSymbol, index), parameters[parmIndex]) { SqlDbType = SqlDbType.VarChar});
																				}
																				else
																					 parameterByPosition.Add(new SqlParameter(string.Format("{0}p{1}", PreDbParamSymbol, index), parameters[parmIndex]));
                                    }
                                }
                                else
                                {
																	if (parameters[parmIndex] == null)
																	{
																		parameters[parmIndex] = DBNull.Value;
																	}
																	parameterByPosition.Add(parameters[parmIndex]);
                                }
                                parmIndex++;
                            }
                        }
                    }
                }
                if (parameterByPosition.Count > 0)
                {
                    if (retrunDbParameter)
                    {
                        return GetDbParamaters(context, parameterByPosition.ToArray()).Cast<object>().ToArray();
                    }
                    else
                    {
                        return parameterByPosition.ToArray();
                    }
                }
            }
            return parameters;
        }

        private static IQueryable<T> SqlQueryImpl<T>(DbContext context, string sqlcommand, params object[] parameters)
        {
            return context.SqlQuery<T>(sqlcommand, parameters).AsQueryable();
        }

        private static int ExecuteSqlCommandImpl(DbContext context, string sqlcommand, params object[] parameters)
        {
            var isException = false;
            var beforeExecuteTime = DateTime.Now;

            try
            {
                return context.Database.ExecuteSqlCommand(sqlcommand, parameters);
            }
            catch (Exception ex)
            {
                isException = true;
                var strParameters = string.Empty;
                if (parameters is DbParameter[])
                {
                    strParameters = string.Join(",", parameters.Select(p => $"{((DbParameter)p).ParameterName}={((DbParameter)p).Value}"));
                }
                else
                {
                    strParameters = string.Join(",", parameters);
                }
                var sqlWithParams = (parameters != null) ? string.Format("{0}\nParameters:{1}", sqlcommand, strParameters)
                                                         : sqlcommand;
                TxtLog(sqlWithParams, ex);
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                LogSql(context, sqlcommand, beforeExecuteTime, isException, parameters);
            }
        }

        internal static void LogSql(DbContext context, string sql, DateTime beforeExecuteTime, bool isException, params object[] parameters)
        {
            var strParameters = string.Empty;
            if (parameters is DbParameter[])
            {
                strParameters = string.Join(",", parameters.Select(p => $"{((DbParameter)p).ParameterName}={((DbParameter)p).Value}"));
            }
            else
            {
                strParameters = string.Join(",", parameters);
            }
            var sqlWithParams = (parameters.Any()) ? string.Format("{0}\nParameters:{1}", sql, strParameters)
                                                     : sql;
            LogSqlImpl(context, sqlWithParams, beforeExecuteTime, isException);
        }

        private static void LogSqlImpl(DbContext context, string sqlWithParams, DateTime beforeExecuteTime, bool isException)
        {
						var conn = context.Database.GetDbConnection();
						try
            {
                WmsSqlLogHelper.Log(context, sqlWithParams, beforeExecuteTime, isException, PreDbParamSymbol);
            }
            catch (Exception ex)
            {
                TxtLog(sqlWithParams, ex);
								throw new Exception(ex.Message, ex);
            }
        }

        internal static void TxtLog(string sqlWithParams, Exception ex)
        {
            #region 寫入LOG到文字檔
            var entry = new LogEntry()
            {
                Message = string.Format("Exception:{1}{2}\r\nCommand is executing:\n{0}", sqlWithParams, ex.StackTrace, ex.Message),
                Categories = new string[] { "Sql" }
            };
            Logger.Write(entry);
            #endregion
        } 

        public static int? ExecuteSqlCommandWithOutputParameter(DbContext context, string sqlcommand, params DbParameter[] parameters)
        {
            int? value = null;
            var returnParameterName = string.Empty;
            var conn = context.Database.GetDbConnection();
            var isHadOpened = false;
            if (conn.State != ConnectionState.Open)
                context.Database.OpenConnection();
            else
                isHadOpened = true;

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sqlcommand;
                cmd.CommandType = System.Data.CommandType.Text;

                if (parameters != null)
                {
                    foreach (var p in parameters)
                    {
                        if (p.Direction == ParameterDirection.Output)
                            returnParameterName = p.ParameterName;
                        cmd.Parameters.Add(p);
                    }
                }

                try
                {
                    cmd.ExecuteNonQuery();
                    if (!string.IsNullOrWhiteSpace(returnParameterName) && cmd.Parameters[returnParameterName].Value != null)
                    {
                        int v;
                        if (int.TryParse(cmd.Parameters[returnParameterName].Value.ToString(), out v))
                        {
                            value = v;
                        }

                    }
                }
                finally
                {
                    if (conn.State == ConnectionState.Open && !isHadOpened)
                        context.Database.CloseConnection();
                }
            }

            return value;
        }

        public static void ExecuteSqlCommandWithOutputParameter(DbContext context, string sqlcommand, out int seq, params DbParameter[] parameters)
        {
          var returnParameterNames = new List<string>();
          var conn = context.Database.GetDbConnection();
          var isHadOpened = false;
          if (conn.State != ConnectionState.Open)
            context.Database.OpenConnection();
          else
            isHadOpened = true;

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = sqlcommand;
            cmd.CommandType = System.Data.CommandType.Text;

            if (parameters != null)
            {
              foreach (var p in parameters)
              {
                if (p.Direction == ParameterDirection.Output)
                  returnParameterNames.Add(p.ParameterName);
                cmd.Parameters.Add(p);
              }
            }

            try
            {
              cmd.ExecuteNonQuery();
              if (returnParameterNames.Any())
              {
                int.TryParse(cmd.Parameters[returnParameterNames[0]].Value.ToString(), out seq);
              }
              else
              {
                seq = 0;
              }

            }
            finally
            {
              if (conn.State == ConnectionState.Open && !isHadOpened)
                context.Database.CloseConnection();
            }
          }
        }

    #region SimplyMSSQLBulkOperations Bulk Process


    public static void BulkInsertSimplyMSSQLBulkOperations<TEntity>(DbContext context,string tableName, IList<TEntity> entities, SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.FireTriggers, int? batchSize = null, List<string> propertiesToExclude = null, bool isDefaultColumnModify = false) where TEntity : class
		{
			foreach (var entity in entities)
			{
				entity.GetType().GetProperty("Gid", typeof(Guid))?.SetValue(entity, Guid.NewGuid(), null);
				if (!isDefaultColumnModify)
				{
					entity.GetType().GetProperty("CRT_STAFF", typeof(string))?.SetValue(entity, string.IsNullOrWhiteSpace(Current.Staff) ?
							"System" : Current.Staff, null);
					entity.GetType().GetProperty("CRT_NAME", typeof(string))?.SetValue(entity, Current.StaffName ?? "System", null);
					entity.GetType().GetProperty("CRT_DATE", typeof(DateTime))?.SetValue(entity, DateTime.Now, null);
				}
				DataEncryptionProvider.EncryptEntity(entity);
			}
			var conn = (SqlConnection)context.Database.GetDbConnection();
			if (!batchSize.HasValue)
				batchSize = 2000;

			SimplyMSSQLBulkOperations.BulkOperations.BulkInsert(entities.ToList(), conn, tableName, propertiesToExclude, batchSize.Value,sqlBulkCopyOptions);

		}

		public static void BulkUpdateSimplyMSSQLBulkOperations<TEntity>(DbContext context, string tableName, IList<TEntity> entities, SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.Default, int? batchSize = null, List<string> propertiesToExclude = null, bool isDefaultColumnModify = false) where TEntity : class
		{
			foreach (var entity in entities)
			{
				if (!isDefaultColumnModify)
				{
					entity.GetType().GetProperty("UPD_STAFF", typeof(string))?.SetValue(entity, Current.Staff ?? "System", null);
					entity.GetType().GetProperty("UPD_NAME", typeof(string))?.SetValue(entity, Current.StaffName ?? "System", null);
					entity.GetType().GetProperty("UPD_DATE", typeof(DateTime?))?.SetValue(entity, DateTime.Now, null);
				}
				DataEncryptionProvider.EncryptEntity(entity);
			}

			if (propertiesToExclude == null)
				propertiesToExclude = new List<string>();
			// 排除建立日期，建立人員，建立時間不更新
			propertiesToExclude.Add("CRT_DATE");
			propertiesToExclude.Add("CRT_STAFF");
			propertiesToExclude.Add("CRT_NAME");
			if (!batchSize.HasValue)
				batchSize = 2000;
			var conn = (SqlConnection)context.Database.GetDbConnection();
			SimplyMSSQLBulkOperations.BulkOperations.BulkUpdate(entities.ToList(), conn, tableName,null,propertiesToExclude,batchSize.Value,sqlBulkCopyOptions);
		}


		public static void BulkDeleteSimplyMSSQLBulkOperations<TEntity>(DbContext context, string tableName, IList<TEntity> entities, SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.Default, int? batchSize = null) where TEntity : class
		{
			if (!batchSize.HasValue)
				batchSize = 2000;

			var conn = (SqlConnection)context.Database.GetDbConnection();
			SimplyMSSQLBulkOperations.BulkOperations.BulkDelete(entities.ToList(), conn, tableName,null,batchSize.Value,sqlBulkCopyOptions);
		}
		#endregion



		internal static List<DbParameter> GetDbParamaters(DbContext db, object[] parameters)
        {
            var dbParams = new List<DbParameter>();
            var i = 0;
            foreach (var parm in parameters)
            {
                DbParameter dbParam = null;
                if (parm is SqlParameter)
                {
                    var sqlParm = parm as SqlParameter;
                    dbParam = sqlParm;
                  
                }
                else
                {
					        if(parm!=null && parm.GetType().Equals(typeof(string)))
										dbParam = new SqlParameter($"@p{i}", parm) { SqlDbType = SqlDbType.VarChar};
													else
										dbParam = new SqlParameter($"@p{i}", parm);
                }
                dbParams.Add(dbParam);
                i++;
            }

            return dbParams;
        }
    }
}
