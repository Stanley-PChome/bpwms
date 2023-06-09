using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Collections.Generic
{

	public static class Extenders
	{

		public static DataTable ToDataTable<T>(this IEnumerable<T> collection, string tableName)
		{
			var table = ToDataTable(collection);
			table.TableName = tableName;
			return table;
		}

		public static DataTable ToDataTable<T>(this IEnumerable<T> collection)
		{
			if (collection == null)
				throw new ArgumentNullException("No collection");

			var dt = new DataTable();
			var entityType = typeof(T);

			var properyTypes = from pi in entityType.GetProperties()
							   where pi.GetIndexParameters().Length == 0 //avoid indexer
							   select pi;

			//prepare columns
			foreach (var pi in properyTypes)
			{
				var propertyType = pi.PropertyType;
				if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
					propertyType = Nullable.GetUnderlyingType(propertyType);
				dt.Columns.Add(pi.Name, propertyType);
			}

			//Convert entity values to rows
			foreach (T item in collection)
			{
				DataRow dr = dt.NewRow();
				dr.BeginEdit();
				foreach (var pi in properyTypes)
				{
					var value = pi.GetValue(item, null);
					if (pi.PropertyType != typeof(string))
						dr[pi.Name] = value ?? DBNull.Value;
					else
					{
						dr[pi.Name] = value;
					}
				}
				dr.EndEdit();
				dt.Rows.Add(dr);
			}
			return dt;
		}

		public static DataTable ToDataTable<T>(this IEnumerable<T> collection, string tableName, Dictionary<string, string> columnTitles)
		{
			var table = ToDataTable(collection, columnTitles);
			table.TableName = tableName;
			return table;
		}

		public static DataTable ToDataTable<T>(this IEnumerable<T> collection, Dictionary<string, string> columnTitles)
		{
			if (collection == null)
				throw new ArgumentNullException("No collection");
			if (columnTitles == null)
				throw new ArgumentNullException("No columnTitles");

			var dt = new DataTable();
			var entityType = typeof(T);

			var properyTypes = from pi in entityType.GetProperties()
							   where pi.GetIndexParameters().Length == 0 //avoid indexer
							   select pi;

			//prepare columns
			foreach (var key in columnTitles.Keys)
			{
				var propertyType = properyTypes.Where(x => x.Name == key).Select(x => x.PropertyType).FirstOrDefault();
				if (propertyType == null)
					continue;

				if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
					propertyType = Nullable.GetUnderlyingType(propertyType);

				dt.Columns.Add(columnTitles[key], propertyType);
			}


			//Convert entity values to rows
			foreach (T item in collection)
			{
				DataRow dr = dt.NewRow();
				dr.BeginEdit();
				foreach (var pi in properyTypes)
				{
					if (columnTitles.ContainsKey(pi.Name))
					{
						var value = pi.GetValue(item, null);
						if (pi.PropertyType != typeof(string))
							dr[columnTitles[pi.Name]] = value ?? DBNull.Value;
						else
						{
							dr[columnTitles[pi.Name]] = value;
						}
					}
				}
				dr.EndEdit();
				dt.Rows.Add(dr);
			}
			return dt;
		}

		public static IEnumerable<T> ToEntities<T>(this DataTable dt) where T : class, new()
		{
			foreach (DataRow row in dt.Rows)
			{
				T result = new T();
				foreach (DataColumn column in dt.Columns)
				{
					typeof(T).GetProperty(column.ColumnName).SetValue(result, row[column.ColumnName].DbNullToNull(), null);
				}
				yield return result;
			}
		}

		private static object DbNullToNull(this object original)
		{
			return original == DBNull.Value ? null : original;
		}
	}
}
