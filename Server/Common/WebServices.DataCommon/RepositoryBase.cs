using Wms3pl.WebServices.DataCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text;
using System.Data.Objects;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Transactions;

namespace Wms3pl.WebServices.DataCommon
{
	public class RepositoryBase<T, Tdb, TRep> : IRepositoryBase<T>
			where T : class
			where Tdb : Wms3plDbContextBase, new()
			where TRep : class
	{
		protected Tdb _db;
		protected WmsTransaction _wmsTransaction;
		private bool _Encoded = false;
		private IList<T> _entityCaches = new List<T>();
		private DatabaseType _databaseType;
		private char _preDbParamSymbol = '@';
		private string _preParam;
		private static Tdb _staticdb;

		public RepositoryBase(string connName, WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
			// if (wmsTransaction == null || !wmsTransaction.DbContexts.Keys.Contains(typeof(Tdb)))
			if (wmsTransaction == null || wmsTransaction.DbContext == null)
			{
				DbContextOptionsBuilder dbOptBuilder = new DbContextOptionsBuilder();
				var connStr = ConfigurationManager.ConnectionStrings[connName].ConnectionString;
				_databaseType = DatabaseType.MSSql;
				dbOptBuilder = dbOptBuilder.UseSqlServer(connStr,b=> b.CommandTimeout(600)); // 設定Command Timeout為10min
				var notAddTransationIsolationLevelFuntionCodeList = new List<string>
				{
					"P0814010000","P0814020000"
				};
				if (wmsTransaction != null)
				{
					if (wmsTransaction.DbContext == null)
					{
						_db = Activator.CreateInstance(typeof(Tdb), dbOptBuilder.Options) as Tdb;
						_db.ChangeTracker.AutoDetectChangesEnabled = false;
						if(!notAddTransationIsolationLevelFuntionCodeList.Contains(Current.FunctionCode))
						  _db.Database.ExecuteSqlCommand("set transaction isolation level read committed");

						wmsTransaction.DbContext = _db;
					}
					wmsTransaction.DatabaseType = _databaseType;
				}
				else
				{
					_db = Activator.CreateInstance(typeof(Tdb), dbOptBuilder.Options) as Tdb;
					_db.ChangeTracker.AutoDetectChangesEnabled = false;
					if (!notAddTransationIsolationLevelFuntionCodeList.Contains(Current.FunctionCode))
						_db.Database.ExecuteSqlCommand("set transaction isolation level read committed");
					//CreateNotTransationDbConnect(dbOptBuilder);
				}
			}
			else
			{
				_db = (Tdb)wmsTransaction.DbContext;
				_databaseType = wmsTransaction.DatabaseType;
			}

			_preDbParamSymbol = '@';

			_preParam = _preDbParamSymbol + "p";
			ExecuteCommandHelper.PreDbParamSymbol = _preDbParamSymbol;
			SqlParamterListExtension.PreDbParamSymbol = _preDbParamSymbol;
		}

		// 非wmsTransation連線要共用相同連線(之後再使用，因為linq 未tolist 跑 foreach 使用script select會有sql錯誤)
		private void CreateNotTransationDbConnect(DbContextOptionsBuilder dbOptBuilder)
		{
			if(HttpContext.Current != null)
			{
				if (HttpContext.Current.Items.Contains("CacheDb"))
				{
					_db = HttpContext.Current.Items["CacheDb"] as Tdb;
				}
				else
				{
					_db = Activator.CreateInstance(typeof(Tdb), dbOptBuilder.Options) as Tdb;
					HttpContext.Current.Items.Add("CacheDb", _db);
				}
			}
			else
			{
				if (_staticdb == null)
					_staticdb = Activator.CreateInstance(typeof(Tdb), dbOptBuilder.Options) as Tdb;
				
				_db = _staticdb;
			}
			_db.ChangeTracker.AutoDetectChangesEnabled = false;
		}

		private List<T> GetAll()
		{
			return All.ToList();
		}

		/// <summary>
		/// 以Key查詢單一筆資料
		/// </summary>
		/// <param name="keyCondition">Key的Lambda條件式</param>
		/// <param name="isForUpdate">是否為了Update而查詢</param>
		/// <param name="isByCache">是否先由Cache查詢</param>
		/// <returns></returns>
		public T Find(Expression<Func<T, bool>> keyCondition, bool isForUpdate = true, bool isByCache = true)
		{
			T entity = null;
			if (isByCache)
			{
				if (_wmsTransaction != null)
				{
					entity = _wmsTransaction.GetCacheEntities<T>().SingleOrDefault(keyCondition.Compile());
				}
				else
				{
					entity = _entityCaches.SingleOrDefault(keyCondition.Compile());
				}
				if (entity != null)
					return entity;
			}

			var index = 0;
			var paramers = new List<object>();
			var keyConditionStr = "";
			GetKeyCondition(keyCondition.Body, ref keyConditionStr, paramers, ref index, keyCondition.Body.NodeType);
			var sql = string.Format("Select * From {0} Where {1}", typeof(T).Name, keyConditionStr);
			_isForUpdate = isForUpdate;

			entity = DoSqlQuery<T>(sql,false, paramers.ToArray()).SingleOrDefault();
			if (entity != null)
			{
				if (_wmsTransaction != null)
					_wmsTransaction.CacheEntities<T>(entity);
				else
					_entityCaches.Add(entity);
			}
			return entity;
		}

		/// <summary>
		/// 用In 及 條件為 = 和 And 查詢資料
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="inColumn"></param>
		/// <param name="columnValues"></param>
		/// <param name="trueAndCondition"></param>
		public IQueryable<T> InWithTrueAndCondition<TValue>(Expression<Func<T, object>> inColumn, List<TValue> columnValues, Expression<Func<T, bool>> trueAndCondition = null)
		{
			var propertyName = GetPropertyName(inColumn);
			return InWithTrueAndCondition(propertyName, columnValues, trueAndCondition);
		}

		/// <summary>
		/// 用In 及 條件為 = 和 And 查詢資料
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="inColumn"></param>
		/// <param name="columnValues"></param>
		/// <param name="trueAndCondition"></param>
		/// <returns></returns>
		public IQueryable<T> InWithTrueAndCondition<TValue>(string inColumn, List<TValue> columnValues, Expression<Func<T, bool>> trueAndCondition = null)
		{
			var index = 0;
			var paramers = new List<object>();
			var keyConditionStr = "";
			if (trueAndCondition != null)
				GetKeyCondition(trueAndCondition.Body, ref keyConditionStr, paramers, ref index, trueAndCondition.Body.NodeType);

			var inSql = paramers.CombineSqlInParameters(inColumn, columnValues, ref index);
			if (keyConditionStr != "")
				inSql = " And " + inSql;

			var sql = string.Format("Select * From {0} Where {1}{2}", typeof(T).Name, keyConditionStr, inSql);
			return DoSqlQuery<T>(sql, false,paramers.ToArray());
		}

		/// <summary>
		/// 用條件為 = 和 And 查詢資料
		/// </summary>
		/// <param name="trueAndCondition"></param>
		/// <returns></returns>
		public IQueryable<T> GetDatasByTrueAndCondition(Expression<Func<T, bool>> trueAndCondition = null)
		{
			var index = 0;
			var paramers = new List<object>();
			var keyConditionStr = "";
			if (trueAndCondition != null)
				GetKeyCondition(trueAndCondition.Body, ref keyConditionStr, paramers, ref index, trueAndCondition.Body.NodeType);
			if (keyConditionStr != "")
				keyConditionStr = " Where " + keyConditionStr;

			var sql = string.Format("Select * From {0}{1}", typeof(T).Name, keyConditionStr);
			return DoSqlQuery<T>(sql, false, paramers.ToArray());
		}

		private IQueryable<T> All { get { return _db.Set<T>(); } }

		public bool AutoDetectChangesEnabled { get { return _db.ChangeTracker.AutoDetectChangesEnabled; } set { _db.ChangeTracker.AutoDetectChangesEnabled = value; } }

		/// <summary>
		/// 新增
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="withoutColumns">指定值非Null的欄位，不組入Insert的SQL Command</param>
		public virtual void Add(T entity, params string[] withoutColumns)
		{
			AddImpl(entity,false, withoutColumns);
		}

		public virtual void Add(T entity,  bool isDefaultColumnModify = false, params string[] withoutColumns)
		{
			AddImpl(entity, isDefaultColumnModify,withoutColumns);
		}

		/// <summary>
		/// 新增
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="withoutColumns">指定值非Null的欄位，不組入Insert的SQL Command</param>
		private void AddImpl(T entity, bool isDefaultColumnModify = false, params string[] withoutColumns)
		{
			var colNames = new List<string>();
			var pStrs = new List<string>();
			var paramers = new List<object>();
			var i = 0;
			var p = new StringBuilder();
			var isEncryptable = typeof(IEncryptable).IsAssignableFrom(typeof(T));
			foreach (var pro in typeof(T).GetProperties())
			{
				if (pro.Name == "EncryptedProperties")
					continue;

				p.Clear();

				var value = ExecuteCommandHelper.GetPropertyValueForWms(pro, entity, isDefaultColumnModify);

				if (value != null && (pro.PropertyType == typeof(string) || (!pro.PropertyType.IsClass && !pro.PropertyType.IsInterface)))
				{
					if (withoutColumns == null || !withoutColumns.Any() || !withoutColumns.Contains(pro.Name))
					{
						p.Append(_preParam).Append(i.ToString());
						colNames.Add(pro.Name);
						pStrs.Add(p.ToString());
						object[] attributes = pro.GetCustomAttributes(typeof(EncryptedAttribute), false);
						if (isEncryptable && attributes.Any())
						{
							value = AesCryptor.Current.Encode((string)value);
						}
						paramers.Add(value);
						i++;
					}
				}
			}
			var sql = string.Format("Insert Into {0}({1}) Values({2})", typeof(T).Name, string.Join(",", colNames.ToArray()), string.Join(",", pStrs.ToArray()));
			_Encoded = true;
			ExecuteSqlCommandObjectParams(sql, false, false, null,false, paramers.ToArray());
			_Encoded = false;
			if (entity != null)
			{
				if (_wmsTransaction != null)
					_wmsTransaction.CacheEntities<T>(entity);
				else
					_entityCaches.Add(entity);
			}
		}

		/// <summary>
		/// 若有打開 UseBulkInsertFirst 與 WmsTransaction，則 Commit 時，Entity會優先新增大量插入，否則會使用一般 Add
		/// </summary>
		/// <param name="entity"></param>
		public virtual void AddToBulkInsert(T entity, Dictionary<string, object> fieldsDefaultValue = null, params string[] withoutColumns)
		{
			if (_wmsTransaction != null && _wmsTransaction.UseBulkInsertFirst)
			{
				_wmsTransaction.AddBulkInsertEntity(_db, entity, fieldsDefaultValue, withoutColumns);
			}
			else
			{
				AddImpl(entity,false, withoutColumns);
			}
		}

		/// <summary>
		///  大量新增
		/// </summary>
		/// <param name="entities"></param>
		/// <param name="withoutColumns"></param>
		public virtual void BulkInsert(IEnumerable<T> entities, params string[] withoutColumns)
		{
			BulkInsertSimplyBulk(entities, null, false, withoutColumns);
		}

		public virtual void BulkInsert(IEnumerable<T> entities, bool isDefaultColumnModify = false, params string[] withoutColumns)
		{
			BulkInsertSimplyBulk(entities, null, isDefaultColumnModify, withoutColumns);
		}

		public virtual void BulkInsert(IEnumerable<T> entities, Dictionary<string, object> fieldsDefaultValue = null, params string[] withoutColumns)
		{
			BulkInsertSimplyBulk(entities, fieldsDefaultValue, false, withoutColumns);
		}
		public virtual void BulkInsert(IEnumerable<T> entities, Dictionary<string, object> fieldsDefaultValue = null, bool isDefaultColumnModify = false, params string[] withoutColumns)
		{
			BulkInsertSimplyBulk(entities, fieldsDefaultValue, isDefaultColumnModify, withoutColumns);
		}

		

		private static void EncryptValue(IEnumerable<T> entities, bool isForInsert = true)
		{
			var isEncryptable = typeof(IEncryptable).IsAssignableFrom(typeof(T));
			foreach (var entity in entities)
			{
				if (isEncryptable)
				{
					foreach (var pro in typeof(T).GetProperties())
					{
						object[] attributes = pro.GetCustomAttributes(typeof(EncryptedAttribute), false);
						if (attributes.Any())
						{
							var value = ExecuteCommandHelper.GetPropertyValueForWms(pro, entity);
							if (isForInsert)
							{
								value = AesCryptor.Current.Encode((string)value);
							}
							else
							{
								if (value is string && value != null)
								{
									object[] secretAttributes = pro.GetCustomAttributes(typeof(SecretPersonalDataAttribute), false);
									if (secretAttributes.Any())
									{
										var secreteType = ((SecretPersonalDataAttribute)secretAttributes.First()).SecretType;
										if (secreteType == "NOT" && (string)value != SecretePersonalHelper.SecretePersonalColumn((string)value, secreteType))
											value = AesCryptor.Current.Encode((string)value);
										else
											throw new Exception(string.Format("須加密欄位:{0}，值不可為個資遮罩的值:{1}!", pro.Name, value));
									}
									else
									{
										value = AesCryptor.Current.Encode((string)value);
									}
								}
							}
							pro.SetValue(entity, value);
						}
					}
				}
			}
		}

		//部份Table有子關聯,需要override自訂
		public virtual void Update(T entity, bool isDefaultColumnModify = false, bool isModifyKeyColumn = false, List<string> exceptColumns = null)
		{

			var entityKeys = GetEntityKey(entity);
			var colNames = new List<string>();
			var paramers = new List<object>();
			var i = 0;
			var p = new StringBuilder();
			var p1 = new StringBuilder();
			var isEncryptable = typeof(IEncryptable).IsAssignableFrom(typeof(T));
			foreach (var pro in typeof(T).GetProperties())
			{
				// 如果不允許修改key欄位，則要排除key欄位的更新
				if (!isModifyKeyColumn && entityKeys.Any(x => x.Name == pro.Name))
					continue;

				// 更新排除建立日期、建立人員、建立人名欄位
				if (pro.Name == "CRT_DATE" || pro.Name == "CRT_STAFF" || pro.Name == "CRT_NAME")
					continue;

				if (pro.Name == "EncryptedProperties")
					continue;

				p.Clear();
				p1.Clear();
				var value = pro.GetValue(entity);
				if (pro.PropertyType == typeof(string) || (!pro.PropertyType.IsClass && !pro.PropertyType.IsInterface))
				{
					var isProUpdate = true;
					object[] attributes = pro.GetCustomAttributes(typeof(EncryptedAttribute), false);
					p1.Append(_preParam).Append(i.ToString());
					if (!isDefaultColumnModify)
					{
						if (pro.Name.ToUpper() == "UPD_DATE")
							value = DateTime.Now;
						else if (pro.Name.ToUpper() == "UPD_STAFF")
							value = Current.Staff ?? "System";
						else if (pro.Name.ToUpper() == "UPD_NAME")
							value = Current.StaffName ?? "System";
					}

					if (exceptColumns != null && exceptColumns.Contains(pro.Name))
						continue;

					if (isEncryptable && attributes.Any())
					{
						if (value is string && value != null)
						{
							object[] secretAttributes = pro.GetCustomAttributes(typeof(SecretPersonalDataAttribute), false);
							if (secretAttributes.Any())
							{
								var secreteType = ((SecretPersonalDataAttribute)secretAttributes.First()).SecretType;
								if (secreteType == "NOT" && (string)value != SecretePersonalHelper.SecretePersonalColumn((string)value, secreteType))
									value = AesCryptor.Current.Encode((string)value);
								else
									isProUpdate = false;
							}
							else
							{
								value = AesCryptor.Current.Encode((string)value);
							}
						}
					}
					if (isProUpdate)
					{
						colNames.Add(p.Append(pro.Name).Append("=").Append(_preParam).Append(i.ToString()).ToString());
						paramers.Add(value);
						i++;
					}
				}
			}
			var keyConditionStr = "";
			foreach (var kv in entityKeys)
			{
				if (!string.IsNullOrEmpty(keyConditionStr))
					keyConditionStr += string.Format(" And {0}={1}{2}", kv.Name, _preParam, i);
				else
					keyConditionStr += string.Format("{0}={1}{2}", kv.Name, _preParam, i);


				paramers.Add(typeof(T).GetProperty(kv.Name).GetValue(entity));
				i++;
			}
			var sql = string.Format("Update {0} Set {1} Where {2}", typeof(T).Name, string.Join(",", colNames.ToArray()), keyConditionStr);
			ExecuteSqlCommandObjectParams(sql, false, false, null,false, paramers.ToArray());
		}


		public virtual void BulkUpdate(IEnumerable<T> entities,bool isDefaultColumnModify = false)
		{
			BulkUpdateSimplyBulk(entities, isDefaultColumnModify);
		}

		



		static readonly string[] FixedUpdateFields = { "UPD_STAFF", "UPD_NAME", "UPD_DATE" };
		/// <summary>
		/// 更新欄位
		/// </summary>
		/// <typeparam name="TUpdate">匿名物件</typeparam>
		/// <param name="SET">匿名物件，指定要 SET 欄位的值</param>
		/// <param name="WHERE"> WHERE 條件</param>
		public void UpdateFields<TUpdate>(TUpdate SET, Expression<Func<T, bool>> WHERE = null)
		{
			UpdateFieldsInWithTrueAndCondition<TUpdate, string>(SET, WHERE, null, null);
		}

		/// <summary>
		/// 更新欄位，可使用 IN 來篩選條件
		/// </summary>
		/// <typeparam name="TUpdate"></typeparam>
		/// <param name="SET"></param>
		/// <param name="InFieldName">要 IN 的欄位屬性名稱</param>
		/// <param name="InValues">IN 的 Values</param>
		/// <param name="WHERE"></param>
		public void UpdateFieldsInWithTrueAndCondition<TUpdate, TValue>(TUpdate SET, Expression<Func<T, bool>> WHERE, Expression<Func<T, object>> InFieldName, IEnumerable<TValue> InValues)
		{
			var properties = typeof(TUpdate).GetProperties();

			// 動態檢核欄位程式設計師是否有寫錯匿名物件的欄位名稱
			var validateNamesQuery = properties.Where(p => typeof(T).GetProperty(p.Name) == null).Select(p => p.Name);
			if (validateNamesQuery.Any())
				throw new Exception(string.Format("要更新的欄位{0}名稱不存在於資料表 {1}!", string.Join(", ", validateNamesQuery), typeof(T).Name));

			var validateTypesQuery = properties.Where(p => typeof(T).GetProperty(p.Name).PropertyType.IsValueType != p.PropertyType.IsValueType).Select(p => p.Name);
			if (validateTypesQuery.Any())
				throw new Exception(string.Format("要更新的欄位{0}型態與 {1} 類別不同!", string.Join(", ", validateTypesQuery), typeof(T).Name));

			// 組合要 SET 欄位的 SQL 
			var setNames = properties.Select(x => x.Name).Concat(FixedUpdateFields);
			var updateSql = string.Join(", ", setNames.Select((name, index) => string.Format("{0} = {1}{2}", name, _preParam, index)));

			// 取得 SET 欄位的參數
			List<object> paramList = properties.Select(x => x.GetValue(SET)).ToList();
			// 加上固定更新欄位
			paramList.Add(Current.Staff);
			paramList.Add(Current.StaffName);
			paramList.Add(DateTime.Now);
			int parameterIndex = paramList.Count;

			// 組合 WHERE SQL 條件
			var keyConditionSql = "";
			if (WHERE != null)
			{
				GetKeyCondition(WHERE.Body, ref keyConditionSql, paramList, ref parameterIndex, WHERE.Body.NodeType);
			}

			if (InFieldName != null && InValues != null)
			{
				var inFieldName = GetPropertyName(InFieldName);
				keyConditionSql += paramList.CombineSqlInParameters(string.Format("AND {0}", inFieldName), InValues, ref parameterIndex);
			}

			var sql = string.Format("UPDATE {0} SET {1} {2} {3}", typeof(T).Name, updateSql, string.IsNullOrWhiteSpace(keyConditionSql) ? "" : "WHERE", keyConditionSql);
			ExecuteSqlCommand(sql, paramList.ToArray());
		}

		//部份Table有子關聯,需要override自訂
		public virtual void Delete(Expression<Func<T, bool>> keyCondition)
		{
			DeleteInWithTrueAndCondition<string>(keyCondition, null, null);
		}

		public virtual void DeleteInWithTrueAndCondition<TValue>(Expression<Func<T, bool>> keyCondition, Expression<Func<T, object>> InFieldName, IEnumerable<TValue> InValues)
		{
			var index = 0;
			var paramers = new List<object>();
			var keyConditionStr = "";
			GetKeyCondition(keyCondition.Body, ref keyConditionStr, paramers, ref index, keyCondition.Body.NodeType);

			if (InFieldName != null && InValues != null)
			{
				var inFieldName = GetPropertyName(InFieldName);
				keyConditionStr += paramers.CombineSqlInParameters(string.Format("AND {0}", inFieldName), InValues, ref index);
			}

			var sql = string.Format("Delete From {0} Where {1}", typeof(T).Name, keyConditionStr);
			ExecuteSqlCommandObjectParams(sql, false, false, null,false, paramers.ToArray());
		}



		protected void Save()
		{
			if (_wmsTransaction == null)
				_db.SaveChanges();
		}

		public int ExecuteResultCount { get; set; }

		protected void ExeSqlCmdCountMustGreaterZero(string sqlcommand, string exeCountZeroMessage, params SqlParameter[] parameters)
		{
			ExecuteSqlCommand(sqlcommand, true, exeCountZeroMessage,false, parameters);
		}

		protected void ExecuteSqlCommand(string sqlcommand, params SqlParameter[] parameters)
		{
			ExecuteSqlCommand(sqlcommand, false, null,false, parameters);
		}

		protected void ExecuteSqlCommandWithSqlParameterSetDbType(string sqlcommand, params SqlParameter[] parameters)
		{
			ExecuteSqlCommand(sqlcommand, false, null, true, parameters);
		}

		private void ExecuteSqlCommand(string sqlcommand, bool exeCountMustGreaterZero, string exeCountZeroMessage,bool isSqlParameterSetDbType, params SqlParameter[] parameters)
		{
			_isForUpdate = false;
			EncodeForSqlUpdate(sqlcommand, parameters);

			if (_wmsTransaction == null)
				ExecuteResultCount = ExecuteCommandHelper.ExecuteSqlCommand(_db, sqlcommand, isSqlParameterSetDbType, parameters);
			else
				_wmsTransaction.SqlCommands.Add(new WmsTransSqlCommand { DbContext = _db, SqlCommandText = sqlcommand, Parameters = parameters, ExeCountMustGreaterZero = exeCountMustGreaterZero, ExeCountZeroMessage = exeCountZeroMessage,IsSqlParameterSetDbType = isSqlParameterSetDbType });
		}


		public int? ExecuteSqlCommand(string sqlcommand, string outputParamName, params SqlParameter[] parameters)
		{
			return SqlExecuteSqlCommandWithOutputParameter(sqlcommand, outputParamName, parameters);
		}


		private int? SqlExecuteSqlCommandWithOutputParameter(string sqlcommand, string outputParamName, SqlParameter[] parameters)
		{
			var sqlParams = new List<SqlParameter>();
			foreach (var param in parameters)
			{
				var sqlParam = new SqlParameter(param.ParameterName, SqlDbType.NVarChar);
				sqlParam.Direction = ParameterDirection.Input;
				sqlParam.Value = param.Value;
				sqlParams.Add(sqlParam);
			}
			if (!string.IsNullOrWhiteSpace(outputParamName))
			{
				var sqlParam = new SqlParameter(outputParamName, SqlDbType.BigInt);
				sqlParam.Direction = ParameterDirection.Output;
				sqlParams.Add(sqlParam);
			}

			return ExecuteCommandHelper.ExecuteSqlCommandWithOutputParameter(_db, sqlcommand, sqlParams.ToArray());
		}

		protected void ExecuteSqlCommand(string sqlcommand)
		{
			ExecuteSqlCommandNonParams(sqlcommand, false, null);
		}

    public void ExecuteSqlCommand(string sqlcommand, out int seq, params SqlParameter[] parameters)
    {
      ExecuteCommandHelper.ExecuteSqlCommandWithOutputParameter(_db, sqlcommand, out seq, parameters);
    }

    private void ExecuteSqlCommandNonParams(string sqlcommand, bool exeCountMustGreaterZero, string exeCountZeroMessage)
		{
			_isForUpdate = false;
			if (_wmsTransaction == null)
				ExecuteResultCount = ExecuteCommandHelper.ExecuteSqlCommand(_db, sqlcommand);
			else
				_wmsTransaction.SqlCommands.Add(new WmsTransSqlCommand { DbContext = _db, SqlCommandText = sqlcommand, Parameters = null, ExeCountMustGreaterZero = exeCountMustGreaterZero, ExeCountZeroMessage = exeCountZeroMessage });
		}

		protected void ExeSqlCmdCountMustGreaterZero(string sqlcommand, string exeCountZeroMessage, params object[] parameters)
		{
			ExecuteSqlCommandObjectParams(sqlcommand, true, true, exeCountZeroMessage,false, parameters);
		}

		protected void ExecuteSqlCommand(string sqlcommand, params object[] parameters)
		{
			ExecuteSqlCommandObjectParams(sqlcommand, true, false, null,false, parameters);
		}

		private void ExecuteSqlCommandObjectParams(string sqlcommand, bool isEncodeForSqlUpdate, bool exeCountMustGreaterZero, string exeCountZeroMessage,bool isSqlParameterSetDbType, params object[] parameters)
		{
			_isForUpdate = false;
			if (isEncodeForSqlUpdate)
				EncodeForSqlUpdate(sqlcommand, parameters);

			if (_wmsTransaction == null)
				ExecuteResultCount = ExecuteCommandHelper.ExecuteSqlCommand(_db, sqlcommand,false, parameters);
			else
				_wmsTransaction.SqlCommands.Add(new WmsTransSqlCommand { DbContext = _db, SqlCommandText = sqlcommand, Parameters = parameters, ExeCountMustGreaterZero = exeCountMustGreaterZero, ExeCountZeroMessage = exeCountZeroMessage });
		}

		protected IQueryable<TE> SqlQuery<TE>(string sqlcommand)
		{
			var results = DoSqlQuery<TE>(sqlcommand,false, null);
			return results.AsQueryable();
		}

		protected IQueryable<TE> SqlQuery<TE>(string sqlcommand, params SqlParameter[] parameters)
		{
			var results = DoSqlQuery<TE>(sqlcommand, false, parameters);
			return results.AsQueryable();
		}

		/// <summary>
		/// 用於查詢條件有nvarchar的欄位並有指定資料庫型態
		/// </summary>
		/// <typeparam name="TE"></typeparam>
		/// <param name="sqlcommand"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		protected IQueryable<TE> SqlQueryWithSqlParameterSetDbType<TE>(string sqlcommand, params SqlParameter[] parameters)
		{
			var results = DoSqlQuery<TE>(sqlcommand, true, parameters);
			return results.AsQueryable();
		}

		protected IQueryable<TE> SqlQuery<TE>(string sqlcommand, params object[] parameters)
		{
			var results = DoSqlQuery<TE>(sqlcommand,false, parameters);
			return results.AsQueryable();
		}

		private IQueryable<TE> DoSqlQuery<TE>(string sqlcommand, bool isSqlParameterSetDbType = false, params object[] parameters)
		{
			_db.IsSqlQuery = true;
			List<TE> results;
			if (parameters == null)
				results = ExecuteCommandHelper.ExecuteQuery<TE>(_db, sqlcommand).ToList();
			else if (parameters is SqlParameter[])
				results = ExecuteCommandHelper.ExecuteQuery<TE>(_db, sqlcommand, isSqlParameterSetDbType,(SqlParameter[])parameters).ToList();
			else
				results = ExecuteCommandHelper.ExecuteQuery<TE>(_db, sqlcommand, isSqlParameterSetDbType,(object[])parameters).ToList();

			var encryptionSecretPropInfos = GetEncryptionSecretPropInfos(typeof(TE));
			var encryptPropertyInfos = encryptionSecretPropInfos.Where(e => e.HasEncryption).Select(e => e.PropertyInfo).ToList();
			var secretEncryptionSecretPropInfos = encryptionSecretPropInfos.Where(e => e.HasSecret).ToList();
			if (typeof(IEncryptable).IsAssignableFrom(typeof(TE)))
			{
				foreach (var entity in results)
				{
					DecodeValue((IEncryptable)entity, encryptPropertyInfos);
					SecretePersonalData((IEncryptable)entity, secretEncryptionSecretPropInfos);
				}
			}
			_db.IsSqlQuery = false;

			if (_isForUpdate)
			{
				_isForUpdate = false;
				//AttachDatas(results);
			}
			return results.AsQueryable();
		}

		public DataTable SqlQueryObjParmToDataTable(string sqlcommand, string tableName, params object[] objParameters)
		{
			return ExecuteCommandHelper.SqlQueryToDataTable(_db, sqlcommand, tableName, objParameters);
		}

		public DataTable SqlQueryToDataTable(string sqlcommand, string tableName, params SqlParameter[] parameters)
		{
			return ExecuteCommandHelper.SqlQueryToDataTable(_db, sqlcommand, tableName, parameters);
		}

		public IQueryable<T> Filter(Expression<Func<T, bool>> condition)
		{
			return All.Where(condition).AsQueryable();
		}

		private void GetKeyCondition(Expression expression, ref string keyCondition, List<object> parameters, ref int parameterIndex, ExpressionType noteType)
		{
			Expression expressionRight = null;
			Expression expressionLeft = null;
			if (expression is BinaryExpression)
			{
				expressionRight = ((BinaryExpression)expression).Right;
				expressionLeft = ((BinaryExpression)expression).Left;
			}
			else if (expression is MethodCallExpression)
			{
				if (((MethodCallExpression)expression).Method.Name == "Equals" && ((MethodCallExpression)expression).Arguments.Count == 1)
				{
					expressionLeft = ((MethodCallExpression)expression).Object;
					expressionRight = ((MethodCallExpression)expression).Arguments[0];
				}
				else
					throw new Exception(string.Format("Expression 不支援此方法{0}", ((MethodCallExpression)expression).Method.Name));
			}

			if (expressionRight is ConstantExpression || expressionRight is UnaryExpression
					|| (expressionRight is MethodCallExpression && (expressionLeft is UnaryExpression || expressionLeft is MemberExpression))
					|| (expressionRight is MemberExpression))
			{
				object value;
				if (expressionRight is UnaryExpression
						|| (expressionRight is MethodCallExpression && (expressionLeft is UnaryExpression || expressionLeft is MemberExpression))
						|| (expressionRight is MemberExpression))
				{
					if (expressionRight is UnaryExpression)
						expressionRight = ((UnaryExpression)expressionRight).Operand;
					if (expressionRight.Type == typeof(decimal))
						value = Expression.Lambda<Func<decimal>>(expressionRight).Compile()();
					else if (expressionRight.Type == typeof(string))
						value = Expression.Lambda<Func<string>>(expressionRight).Compile()();
					else if (expressionRight.Type == typeof(DateTime))
						value = Expression.Lambda<Func<DateTime>>(expressionRight).Compile()();
					else if (expressionRight.Type == typeof(DateTime?))
						value = Expression.Lambda<Func<DateTime?>>(expressionRight).Compile()();
					else if (expressionRight.Type == typeof(Int32))
						value = Expression.Lambda<Func<Int32>>(expressionRight).Compile()();
					else if (expressionRight.Type == typeof(Int16))
						value = Expression.Lambda<Func<Int16>>(expressionRight).Compile()();
					else if (expressionRight.Type == typeof(Int64))
						value = Expression.Lambda<Func<Int64>>(expressionRight).Compile()();
					else if (expressionRight.Type == typeof(short?))
						value = Expression.Lambda<Func<short?>>(expressionRight).Compile()();
					else if (expressionRight.Type == typeof(Int64?))
						value = Expression.Lambda<Func<Int64?>>(expressionRight).Compile()();
					else
						throw new Exception("Expression 值的型別不支援");
				}
				else
					value = ((ConstantExpression)expressionRight).Value;
				var colName = "";
				if (expressionLeft is UnaryExpression)
					colName = ((MemberExpression)((UnaryExpression)expressionLeft).Operand).Member.Name;
				else if (expressionLeft is MethodCallExpression)
					colName = ((MemberExpression)((MethodCallExpression)expressionLeft).Arguments[0]).Member.Name;
				else
					colName = ((MemberExpression)expressionLeft).Member.Name;
				if (!string.IsNullOrEmpty(keyCondition))
				{
					var opera = "";
					switch (noteType)
					{
						case ExpressionType.AndAlso:
							opera = "And";
							break;
						//case ExpressionType.OrElse:
						//	opera = "Or";
						//	break;
						default:
							opera = "And";
							break;
					}
					keyCondition += string.Format(" {0} {1}={2}{3}", opera, colName, _preParam, parameterIndex);
				}
				else
					keyCondition += string.Format("{0}={1}{2}", colName, _preParam, parameterIndex);

				parameters.Add(value);
				parameterIndex++;
			}
			else
			{
				GetKeyCondition(expressionLeft, ref keyCondition, parameters, ref parameterIndex, expression.NodeType);
				GetKeyCondition(expressionRight, ref keyCondition, parameters, ref parameterIndex, expression.NodeType);
			}
		}

		public static string GetPropertyName(Expression<Func<T, object>> func)
		{
			var memberExpression = func.Body is MemberExpression
													 ? func.Body as MemberExpression
													 : (func.Body as UnaryExpression).Operand as MemberExpression;
			var propertyInfo = memberExpression.Member as PropertyInfo;
			return propertyInfo.Name;
		}


		private IReadOnlyList<IProperty> GetEntityKey(T entity)
		{
			var keys = _db.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties;
			return keys;
		}
		

		private void SecretePersonalData(IEncryptable entity, List<EncryptionSecretPropInfo> encryptionSecretPropInfos)
		{
			if (Current.IsSecretePersonalData)
			{
				foreach (var encryptionSecretPropInfo in encryptionSecretPropInfos)
				{
					if (encryptionSecretPropInfo.HasSecret)
					{
						var origValue = encryptionSecretPropInfo.PropertyInfo.GetValue(entity) as string;
						if (!string.IsNullOrEmpty(origValue))
							encryptionSecretPropInfo.PropertyInfo.SetValue(entity, SecretePersonalHelper.SecretePersonalColumn(origValue, encryptionSecretPropInfo.SecretType));
					}
				}
			}
		}



		private void DecodeValue(IEncryptable entity, List<PropertyInfo> propertyInfos)
		{
			foreach (var propertyInfo in propertyInfos)
			{
				var origValue = propertyInfo.GetValue(entity) as string;
				propertyInfo.SetValue(entity, AesCryptor.Current.Decode(origValue));
			}
		}

		private List<EncryptionSecretPropInfo> GetEncryptionSecretPropInfos(Type entityType)
		{
			var encryptedProps = new List<EncryptionSecretPropInfo>();
			foreach (var propertyInfo in entityType.GetProperties())
			{
				object[] attributes = propertyInfo.GetCustomAttributes(typeof(EncryptedAttribute), false);
				object[] secretAttributes = propertyInfo.GetCustomAttributes(typeof(SecretPersonalDataAttribute), false);
				var hasSecret = secretAttributes.Any();
				var hasEncryption = attributes.Any();
				if (hasEncryption || hasSecret)
				{
					var secretType = string.Empty;
					if (hasSecret)
					{
						secretType = ((SecretPersonalDataAttribute)secretAttributes.First()).SecretType;
					}
					var encryptionSecretedPropInfo = new EncryptionSecretPropInfo
					{
						PropertyInfo = propertyInfo,
						HasEncryption = hasEncryption,
						HasSecret = hasSecret,
						SecretType = secretType
					};
					encryptedProps.Add(encryptionSecretedPropInfo);
				}
			}
			return encryptedProps;
		}

		private void EncodeForSqlUpdate(string sqlcommand, params object[] parameters)
		{
			var subSql = sqlcommand.TrimStart().Substring(0, 6);
			if (!_Encoded && typeof(IEncryptable).IsAssignableFrom(typeof(T)) && sqlcommand.TrimStart().Substring(0, 6).ToUpper() == "UPDATE")
			{
				var tmpSql = sqlcommand.Substring(sqlcommand.IndexOf(subSql) + 6).TrimStart();
				var tableName = tmpSql.Substring(0, tmpSql.IndexOf(" ")).Trim();
				if (tableName.ToUpper() != typeof(T).Name.ToUpper())
					throw new Exception(string.Format("不可在{0}的Repository中Update {1}的Table", typeof(T).Name, tableName));
				var upperSql = sqlcommand.ToUpper();
				var indexSet = upperSql.IndexOf("SET");
				var indexWhere = upperSql.IndexOf("WHERE");
				var strColVl = (indexWhere < 0) ?
						upperSql.Substring(indexSet + 3) : upperSql.Substring(indexSet + 3, indexWhere - (indexSet + 3));
				var colVls = strColVl.Split('=');
				var entity = Activator.CreateInstance(typeof(T)) as IEncryptable;
				var j = 0;
				var preParam = _preDbParamSymbol + "P";
				for (var c = 0; c < colVls.Length - 1; c++)
				{
					var colVl = colVls[c];
					var indexComma = colVl.LastIndexOf(',');
					var col = (indexComma < 0) ? colVl.Trim() : colVl.Substring(indexComma + 1).Trim();
					var colVl2 = colVls[c + 1].Trim();
					var indexComma2 = colVl2.LastIndexOf(',');
					var v = (indexComma2 < 0) ? colVl2.Trim() : colVl2.Substring(0, colVl2.LastIndexOf(',')).Trim();
					var propInfo = entity.GetType().GetProperties().ToList().FirstOrDefault(p => p.Name.ToUpper() == col.ToUpper());

					if (propInfo != null)
					{
						object[] attributes = propInfo.GetCustomAttributes(typeof(EncryptedAttribute), false);
						if (v.Contains(preParam) && attributes.Any())
						{
							if (parameters is SqlParameter[])
							{
								v = v.Substring(v.IndexOf(preParam));
								var i = 2;
								while (true)
								{
									if (i == v.Length) break; //長度超過也離開
									int num;
									var s = v.Substring(i, 1);
									var isInt = int.TryParse(s, out num);
									if (!isInt)
									{
										break; //遇到不是數字字元也離開
									}
									i = i + 1;
								}
								v = v.Substring(0, i);
								var param = ((SqlParameter[])parameters).Where(a => a.ParameterName.ToUpper() == v).Single();
								param.Value = AesCryptor.Current.Encode((string)param.Value);
							}
							else
							{
								parameters[j] = AesCryptor.Current.Encode((string)parameters[j]);
							}
						}
					}
					j++;
				}
			}
		}


		private bool _isForUpdate = false;
		public TRep AsForUpdate()
		{
			_isForUpdate = true;
			return this as TRep;
		}

		public void CacheDatas(string key, List<T> datas)
		{
			_wmsTransaction.CacheDatas<T>(key, datas);
		}

		public List<T> GetCacheDatas(string key)
		{
			if (_wmsTransaction == null)
				return null;
			return _wmsTransaction.GetCacheDatas<T>(key);
		}

		public List<T> GetCacheDatasForUpdate(string key)
		{
			var cacheDatas = GetCacheDatas(key);
			return cacheDatas;
		}


		#region SimplyBulk

		/// <summary>
		/// SimplyBulk大量新增
		/// </summary>
		/// <param name="entities"></param>
		/// <param name="withoutColumns"></param>
		private void BulkInsertSimplyBulk(IEnumerable<T> entities, Dictionary<string, object> fieldsDefaultValue = null, bool isDefaultColumnModify = false, params string[] withoutColumns)
		{
			if (entities.Count() < 100)
			{
				foreach (var entity in entities)
					Add(entity, withoutColumns);
			}
			else
			{
				foreach (var entity in entities)
				{
					if (fieldsDefaultValue != null)
					{
						if (fieldsDefaultValue != null)
							ExecuteCommandHelper.SetDefaultValueIfNullOrWhiteSpace(typeof(T), entity, fieldsDefaultValue);
					}
				}

				List<string> propertiesToExclude = null;
				if (withoutColumns != null)
					propertiesToExclude = withoutColumns.ToList();

				var batchSize = 2000;
				if (_wmsTransaction != null)
					_wmsTransaction.EFCoreBulkInfos.Add(new EFCoreBulkInfo { DbContext = _db, EntityName = typeof(T).Name, Entities = entities.Cast<object>().ToList(), DbProcessType = DbProcessType.Insert, PropertiesToExclude = propertiesToExclude, IsDefaultColumnModify = isDefaultColumnModify, BatchSize = batchSize });
				else
				{
					ExecuteCommandHelper.BulkInsertSimplyMSSQLBulkOperations(_db, typeof(T).Name, entities.ToList(), batchSize: batchSize, propertiesToExclude: propertiesToExclude, isDefaultColumnModify: isDefaultColumnModify);
				}
			}
		}

		/// <summary>
		///  SimplyBulk大量修改
		/// </summary>
		/// <param name="entities"></param>
		/// <param name="withoutColumns"></param>
		private void BulkUpdateSimplyBulk(IEnumerable<T> entities, bool isDefaultColumnModify)
		{
			if (entities.Count() < 100)
			{
				foreach (var entity in entities)
					Update(entity, isDefaultColumnModify);
			}
			else
			{
				var batchSize = 2000;
				if (_wmsTransaction != null)
					_wmsTransaction.EFCoreBulkInfos.Add(new EFCoreBulkInfo { DbContext = _db, EntityName = typeof(T).Name, Entities = entities.Cast<object>().ToList(), DbProcessType = DbProcessType.Update, IsDefaultColumnModify = isDefaultColumnModify, BatchSize = batchSize });
				else
				{
					ExecuteCommandHelper.BulkUpdateSimplyMSSQLBulkOperations(_db, typeof(T).Name, entities.ToList(), batchSize: batchSize, isDefaultColumnModify: isDefaultColumnModify);
				}
			}
		}

		/// <summary>
		/// MSSQL大量修改，任意條件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entites"></param>
		/// <param name="conn"></param>
		/// <param name="tableName"></param>
		/// <param name="conditionColumns">任意條件</param>
		/// <param name="excludeColumns"></param>
		/// <param name="batchSize"></param>
		/// <param name="sqlBulkCopyOptions"></param>
		public void SqlBulkUpdateForAnyCondition(List<T> entites, string tableName, List<string> conditionColumns, List<string> excludeColumns = null)
		{
			var conn = (SqlConnection)_db.Database.GetDbConnection();
			int batchSize = 2000;
			SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.Default;
			SimplyMSSQLBulkOperations.BulkOperations.BulkUpdateForAnyCondition(entites, conn, tableName, conditionColumns, excludeColumns, batchSize, sqlBulkCopyOptions);
		}

		/// <summary>
		/// MSSQL指定欄位大量修改，任意條件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entites"></param>
		/// <param name="conn"></param>
		/// <param name="tableName"></param>
		/// <param name="conditionColumns">任意條件</param>
		/// <param name="specifiedColumns"></param>
		/// <param name="batchSize"></param>
		/// <param name="sqlBulkCopyOptions"></param>
		public void SqlBulkSpecifiedUpdateForAnyCondition(List<T> entites, string tableName, List<string> conditionColumns, List<string> specifiedColumns = null)
		{
			var conn = (SqlConnection)_db.Database.GetDbConnection();
			int batchSize = 2000;
			SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.Default;
			SimplyMSSQLBulkOperations.BulkOperations.BulkSpecifiedUpdateForAnyCondition(entites, conn, tableName, conditionColumns, specifiedColumns, batchSize, sqlBulkCopyOptions);
		}

		/// <summary>
		/// MSSQL大量刪除，任意條件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entites"></param>
		/// <param name="conn"></param>
		/// <param name="tableName"></param>
		/// <param name="conditionColumns">任意條件</param>
		/// <param name="excludeColumns"></param>
		/// <param name="batchSize"></param>
		/// <param name="sqlBulkCopyOptions"></param>
		public void SqlBulkDeleteForAnyCondition(List<T> entites, string tableName, List<string> conditionColumns)
		{
			var conn = (SqlConnection)_db.Database.GetDbConnection();
			int batchSize = 2000;
			SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.Default;
			SimplyMSSQLBulkOperations.BulkOperations.BulkDeleteForAnyCondition(entites, conn, tableName, conditionColumns, batchSize, sqlBulkCopyOptions);
		}
		#endregion

		public T UseTransationScope(TransactionScope trans,Func<T> func)
		{
			T obj;
			try
			{
				using (trans)
				{
					obj = func();
					trans.Complete();
				}
			}
			finally
			{
				if (_db.Database.GetDbConnection().State != ConnectionState.Closed)
					_db.Database.CloseConnection();
			}

			return obj;
		}

		/// <summary>
		/// 手動關閉連線
		/// 如果有包UseTransation時，且呼叫其他Repo，就得手動關閉連線
		/// </summary>
		public void CloseDb()
		{
			if (_db.Database.GetDbConnection().State != ConnectionState.Closed)
				_db.Database.CloseConnection();
		}
	}

	public class EncryptionSecretPropInfo
	{
		public PropertyInfo PropertyInfo { get; set; }
		public bool HasEncryption { get; set; }
		public bool HasSecret { get; set; }
		public string SecretType { get; set; }
	}
}