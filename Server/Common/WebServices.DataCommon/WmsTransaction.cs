using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Wms3pl.WebServices.DataCommon
{
	public class WmsTransaction
	{
		public WmsTransaction()
		{
			// DbContexts = new Dictionary<Type, DbContext>();
			SqlCommands = new List<WmsTransSqlCommand>();
			_bulkInsertCommands = new Dictionary<Type, WmsTransBulkInsertCommand>();
			EFCoreBulkInfos = new List<EFCoreBulkInfo>();
		}
		// public Dictionary<Type, DbContext> DbContexts { get; set; }
		public DbContext DbContext { get; set; }
		internal DatabaseType DatabaseType { get; set; }
		public List<WmsTransSqlCommand> SqlCommands { get; set; }
		public List<EFCoreBulkInfo> EFCoreBulkInfos { get; set; }
		private Dictionary<string, object> _cacheDatas = new Dictionary<string, object>();
		private Dictionary<Type, object> _cacheEntities = new Dictionary<Type, object>();
		private Dictionary<Type, WmsTransBulkInsertCommand> _bulkInsertCommands = new Dictionary<Type, WmsTransBulkInsertCommand>();
		/// <summary>
		/// 在呼叫時 Repository.AddToBulkInsert 時，是否使用大量新增優先 Commit 功能
		/// </summary>
		public bool UseBulkInsertFirst { get; set; }

		public string Complete()
		{
			// 沒SQL要執行，且沒快取要清除時，不做任何事情
			if (!SqlCommands.Any() && !_cacheDatas.Any() && !_bulkInsertCommands.Any() && !EFCoreBulkInfos.Any())
				return string.Empty;
			try
      {
        using (var transScope = new TransactionScope(TransactionScopeOption.Required,
          new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = new System.TimeSpan(0, 15, 0) }))
				{
					foreach (var sql in _bulkInsertCommands)
					{
						sql.Value.EFCoreBulk();
					}

					foreach (var sql in SqlCommands)
					{
						var resultCount = 0;
						if (sql.Parameters is SqlParameter[])
							resultCount = ExecuteCommandHelper.ExecuteSqlCommand(DbContext, sql.SqlCommandText, (SqlParameter[])sql.Parameters);
						else
							resultCount = ExecuteCommandHelper.ExecuteSqlCommand(DbContext, sql.SqlCommandText, (object[])sql.Parameters);
						if (sql.ExeCountMustGreaterZero && resultCount <= 0)
						{
							return sql.ExeCountZeroMessage;
						}
					}

					if (DatabaseType == DatabaseType.MSSql && EFCoreBulkInfos != null)
					{
						foreach (var efCoreBulkInfo in EFCoreBulkInfos)
						{
							switch (efCoreBulkInfo.DbProcessType)
							{
								case DbProcessType.Insert:
									ExecuteCommandHelper.BulkInsertSimplyMSSQLBulkOperations(DbContext, efCoreBulkInfo.EntityName, efCoreBulkInfo.Entities, batchSize: efCoreBulkInfo.BatchSize, propertiesToExclude: efCoreBulkInfo.PropertiesToExclude, isDefaultColumnModify: efCoreBulkInfo.IsDefaultColumnModify);
									break;
								case DbProcessType.Update:
									ExecuteCommandHelper.BulkUpdateSimplyMSSQLBulkOperations(DbContext, efCoreBulkInfo.EntityName, efCoreBulkInfo.Entities, batchSize: efCoreBulkInfo.BatchSize, propertiesToExclude: efCoreBulkInfo.PropertiesToExclude, isDefaultColumnModify: efCoreBulkInfo.IsDefaultColumnModify);
									break;
								case DbProcessType.Delete:
									ExecuteCommandHelper.BulkDeleteSimplyMSSQLBulkOperations(DbContext, efCoreBulkInfo.EntityName, efCoreBulkInfo.Entities, batchSize: efCoreBulkInfo.BatchSize);
									break;
							}
						}
					}

					SqlCommands.Clear();
					_cacheDatas.Clear();
					_cacheEntities.Clear();
					if (EFCoreBulkInfos != null)
						EFCoreBulkInfos.Clear();
					_bulkInsertCommands.Clear();

					transScope.Complete();

				}
			}
			finally
			{
				if (DbContext.Database.GetDbConnection().State != System.Data.ConnectionState.Closed)
					DbContext.Database.GetDbConnection().Close();
			}
			return string.Empty;
		}

		internal void CacheDatas<T>(string key, List<T> datas) where T : class
		{
			if (_cacheDatas.Keys.Contains(key))
				throw new Exception(string.Format("WmsTransaction CacheDatas Key 「{0}」已存在", key));

			_cacheDatas.Add(key, datas);
		}

		internal List<T> GetCacheDatas<T>(string key) where T : class
		{
			return _cacheDatas[key] as List<T>;
		}

		internal void CacheEntities<T>(T entity) where T : class
		{
			List<T> cacheEntities;
			if (_cacheEntities.Keys.Contains(typeof(T)))
				cacheEntities = _cacheEntities[typeof(T)] as List<T>;
			else
			{
				cacheEntities = new List<T>();
				_cacheEntities.Add(typeof(T), cacheEntities);
			}

			cacheEntities.Add(entity);
		}

		internal List<T> GetCacheEntities<T>() where T : class
		{
			if (_cacheEntities.Keys.Contains(typeof(T)))
				return _cacheEntities[typeof(T)] as List<T>;
			else
				return new List<T>();
		}

		internal void AddBulkInsertEntity<T>(DbContext dbContext, T entity, Dictionary<string, object> fieldsDefaultValue = null, params string[] withoutColumns) where T : class
		{
			if (!_bulkInsertCommands.ContainsKey(typeof(T)))
				_bulkInsertCommands.Add(typeof(T), new WmsTransBulkInsertCommand(dbContext, typeof(T), fieldsDefaultValue, withoutColumns));

			_bulkInsertCommands[typeof(T)].Add(entity);
		}



		internal void SetEFCoreBulkInfos<T>()
		{
		}

	}

	public class WmsTransSqlCommand
	{
		public DbContext DbContext { get; set; }
		public string SqlCommandText { get; set; }
		public object[] Parameters { get; set; }
		/// <summary>
		/// 執行結果數需大於0
		/// </summary>
		public bool ExeCountMustGreaterZero { get; set; }
		/// <summary>
		/// 執行結果數為0時，回傳的訊息
		/// </summary>
		public string ExeCountZeroMessage { get; set; }


		public int ArrayBindCount { get; set; }

	}

	public class WmsTransBulkInsertCommand
	{
		public DbContext DbContext { get; private set; }
		public List<object> Entities { get; private set; }
		public Type EntityType { get; private set; }
		private Dictionary<string, object> _fieldsDefaultValue;
		private string[] _withoutColumns;

		public WmsTransBulkInsertCommand(DbContext dbContext, Type entityType, Dictionary<string, object> fieldsDefaultValue = null, params string[] withoutColumns)
		{
			DbContext = dbContext;
			EntityType = entityType;
			_fieldsDefaultValue = fieldsDefaultValue;
			_withoutColumns = withoutColumns;
			Entities = new List<object>();
		}

		public void Add(object entity)
		{
			Entities.Add(entity);
		}

		public void EFCoreBulk()
		{
			var batchSize = 4000;
			ExecuteCommandHelper.BulkInsertSimplyMSSQLBulkOperations(DbContext, EntityType.Name, Entities, batchSize: batchSize);
		}
	}

	public class EFCoreBulkInfo
	{
		public string EntityName { get; set; }
		public int BatchSize { get; set; }
		public DbContext DbContext { get; set; }
		public DbProcessType DbProcessType { get; set; }
		public List<object> Entities { get; set; }
		public List<string> PropertiesToExclude { get; set; }
		public bool IsDefaultColumnModify { get; set; }

	}

	public enum DbProcessType
	{
		Insert = 0,
		Update = 1,
		Delete = 2,
		Query = 3
	}
}
