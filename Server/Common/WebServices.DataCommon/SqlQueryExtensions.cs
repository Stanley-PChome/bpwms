using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Wms3pl.WebServices.DataCommon
{
    public static class SqlQueryExtensions
    {
        public static IEnumerable<T> SqlQuery<T>(this DbContext db, string sql, params object[] parameters)
        {
            var isException = false;
            var beforeExecuteTime = DateTime.Now;
            var conn = db.Database.GetDbConnection();
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            try
            {
                return SqlQueryImpl<T>(conn, sql, parameters);
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
                var sqlWithParams = (parameters != null) ? string.Format("{0}\nParameters:{1}", sql, strParameters)
                                                         : sql;
                ExecuteCommandHelper.TxtLog(sqlWithParams, ex);
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                ExecuteCommandHelper.LogSql(db, sql, beforeExecuteTime, isException, parameters);
                conn.Close();
            }
        }

        private static IEnumerable<T> SqlQueryImpl<T>(DbConnection conn, string sql, params object[] parameters)
        {
            var results = new List<T>();
            using (var command = conn.CreateCommand())
            {
                command.CommandText = sql;
                command.CommandTimeout = 600; // 設定Command Timeout為10min
                command.Parameters.AddRange(parameters);
                using (var reader = command.ExecuteReader())
                {
                    Func<DbDataReader, T> func;
                    if (typeof(T).IsValueType || typeof(T) == typeof(string))
                    {
                        func = CreateMappingValueFunc<T>(reader);
                    }
                    else
                    {
                        func = CreateMappingObjectFunc<T>(reader);
                    }
                    while (reader.Read())
                    {
                        var result = func(reader);
                        results.Add(result);
                    }
                }
            }
            return results;
        }



        public static async Task<IEnumerable<T>> SqlQueryAsync<T>(this DbContext db, string sql, params object[] parameters)
        {
            var isException = false;
            var beforeExecuteTime = DateTime.Now;
            var conn = db.Database.GetDbConnection();
            var results = new List<T>();
            if (conn.State == ConnectionState.Closed)
                await conn.OpenAsync();
            try
            {

                return await SqlQueryAsyncImpl<T>(conn, sql, parameters);
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
                var sqlWithParams = (parameters != null) ? string.Format("{0}\nParameters:{1}", sql, strParameters)
                                                         : sql;
                ExecuteCommandHelper.TxtLog(sqlWithParams, ex);
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                ExecuteCommandHelper.LogSql(db, sql, beforeExecuteTime, isException, parameters);
                conn.Close();
            }

        }

        private static async Task<IEnumerable<T>> SqlQueryAsyncImpl<T>(DbConnection conn, string sql, params object[] parameters)
        {
            var results = new List<T>();
            using (var command = conn.CreateCommand())
            {
                command.CommandText = sql;
                command.CommandTimeout = 600; // 設定Command Timeout為10min
                command.Parameters.AddRange(parameters);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    Func<DbDataReader, T> func;
                    if (typeof(T).IsValueType || typeof(T) == typeof(string))
                    {
                        func = CreateMappingValueFunc<T>(reader);
                    }
                    else
                    {
                        func = CreateMappingObjectFunc<T>(reader);
                    }
                    while (await reader.ReadAsync())
                    {
                        var result = func(reader);
                        results.Add(result);
                    }
                }
            }
            return results;
        }

        private static Func<DbDataReader, T> CreateMappingObjectFunc<T>(DbDataReader reader)
        {
            var objType = typeof(T);
            var fieldNames = Enumerable.Range(0, reader.FieldCount).Select(index => reader.GetName(index)).ToArray();

            var propInfos = objType.GetProperties().ToList();
            var tempProps = fieldNames.Select((fieldName, index) =>
            {
                var propInfo = propInfos.Find(p => p.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
                return new
                {
                    index,
                    fieldName,
                    propInfo
                };
            }).Where(f => f.propInfo != null);

            var exBodys = new List<Expression>();


            var exReaderParam = Expression.Parameter(typeof(DbDataReader), "reader");

            var exObjVar = Expression.Variable(objType, "obj");
            var exNew = Expression.New(objType);
            exBodys.Add(Expression.Assign(exObjVar, exNew));

            var exValueVar = Expression.Variable(typeof(object), "value");
            exBodys.Add(Expression.Assign(exValueVar, Expression.Constant(null)));

            var getItemMethod = typeof(DbDataReader).GetMethods().First(m => m.Name == "get_Item"
                && m.GetParameters().First().ParameterType == typeof(int));

            foreach (var p in tempProps)
            {
                //reader[0]
                var exCall = Expression.Call(
                  exReaderParam, getItemMethod,
                  Expression.Constant(p.index)
                );

                // value = reader[0];
                exBodys.Add(Expression.Assign(exValueVar, exCall));

                //user.Name = (string)value;
                var exProp = Expression.Property(exObjVar, p.propInfo.Name);

                UnaryExpression exConvert;
                var propertyType = p.propInfo.PropertyType;
                var propTypeCode = Type.GetTypeCode(propertyType);
                if (propTypeCode == TypeCode.Object && propertyType.IsGenericType)
                {
                    propTypeCode = Type.GetTypeCode(Nullable.GetUnderlyingType(propertyType));
                }
                if (propTypeCode == TypeCode.Decimal || propTypeCode == TypeCode.Int32 || propTypeCode == TypeCode.Int16 ||
                    propTypeCode == TypeCode.Int64 || propTypeCode == TypeCode.Double || propTypeCode == TypeCode.Single)
                {
                    exConvert = Expression.Convert(ConvertToType(exValueVar, propTypeCode), propertyType);
                }
                else
                {
                    exConvert = Expression.Convert(exValueVar, propertyType);
                }
                var exPropAssign = Expression.Assign(exProp, exConvert);

                //if ( !(value is System.DBNull))
                //    (string)value
                var exIfThenElse = Expression.IfThen(
                  Expression.Not(Expression.TypeIs(exValueVar, typeof(System.DBNull)))
                  , exPropAssign
                );

                exBodys.Add(exIfThenElse);
            }

            exBodys.Add(exObjVar);

            // Compiler Expression
            var lambda = Expression.Lambda<Func<DbDataReader, T>>(
              Expression.Block(
                new[] { exObjVar, exValueVar },
                exBodys
              ), exReaderParam
            );

            return lambda.Compile();
        }

        private static Func<DbDataReader, T> CreateMappingValueFunc<T>(IDataReader reader)
        {
            //1. 取得sql select所有欄位名稱

            var exBodys = new List<Expression>();// 方法(IDataReader reader)

            var exReaderParam = Expression.Parameter(typeof(DbDataReader), "reader");


            // var value = defalut(object);
            var exValueVar = Expression.Variable(typeof(object), "value");
            {
                exBodys.Add(Expression.Assign(exValueVar, Expression.Constant(null)));
            }


            var getItemMethod = typeof(DbDataReader).GetMethods().Where(w => w.Name == "get_Item")
              .First(w => w.GetParameters().First().ParameterType == typeof(int));

            Expression exCall = Expression.Call(
                    exReaderParam, getItemMethod,
                    Expression.Constant(0)
                );

            // value = reader[0];
            exBodys.Add(Expression.Assign(exValueVar, exCall));

            var exResultVar = Expression.Variable(typeof(T));
            //user.Name = (string)value;

            UnaryExpression exConvert;
            var rtnType = typeof(T);
            var rtnTypeCode = Type.GetTypeCode(rtnType);
            if (rtnTypeCode == TypeCode.Object && rtnType.IsGenericType)
            {
                rtnTypeCode = Type.GetTypeCode(Nullable.GetUnderlyingType(rtnType));
            }
            if (rtnTypeCode == TypeCode.Decimal || rtnTypeCode == TypeCode.Int32 || rtnTypeCode == TypeCode.Int16 ||
                rtnTypeCode == TypeCode.Int64 || rtnTypeCode == TypeCode.Double || rtnTypeCode == TypeCode.Single)
            {
                exConvert = Expression.Convert(ConvertToType(exValueVar, rtnTypeCode), rtnType); //(string)value
            }
            else
            {
                exConvert = Expression.Convert(exValueVar, rtnType); //(string)value
            }
            var exPropAssign = Expression.Assign(exResultVar, exConvert);

            //if ( !(value is System.DBNull))
            //    (string)value
            var exIfThenElse = Expression.IfThen(
                Expression.Not(Expression.TypeIs(exValueVar, typeof(System.DBNull)))
                , exPropAssign
            );


            exBodys.Add(exIfThenElse);

            exBodys.Add(exResultVar);


            // Compiler Expression 
            var lambda = Expression.Lambda<Func<DbDataReader, T>>(
             Expression.Block(Expression.Block(
                new[] { exResultVar, exValueVar },
                exBodys
              )), exReaderParam
            );

            return lambda.Compile();
        }

        private static MethodCallExpression ConvertToType(
        ParameterExpression sourceParameter,
        TypeCode typeCode)
        {
            var changeTypeMethod = typeof(Convert).GetMethod("ChangeType", new Type[] { typeof(object), typeof(TypeCode) });
            var callExpressionReturningObject = Expression.Call(changeTypeMethod, sourceParameter, Expression.Constant(typeCode));
            return callExpressionReturningObject;
        }
    }
}
