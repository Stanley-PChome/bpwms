using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Services;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace Wms3pl.WebServices.DataCommon
{
    public partial class DataServiceBase<T> : DataService<T>
        where T : Wms3plDbContextBase, new()
    {
		    private static DatabaseType _databaseType;

        public static T GetDataContext()
        {
            var schema = DbSchemaHelper.GetSchema();

            if (string.IsNullOrEmpty(schema))
            {
#if DEBUG
                var defaultContext = ModelHelper<T>.GetDbContext();
                return defaultContext;
#else
          throw new HttpException(400, "未指定 Schema");
#endif
            }
            else
            {
							
							  DbContextOptionsBuilder dbOptBuilder = new DbContextOptionsBuilder();
                var connStr = ConfigurationManager.ConnectionStrings[schema].ConnectionString;
							  _databaseType = DatabaseType.MSSql;
							  dbOptBuilder = dbOptBuilder.UseSqlServer(connStr);
								T context = Activator.CreateInstance(typeof(T), dbOptBuilder.Options) as T;
                ModelHelper<T>.RegisterLogging(context);
                return context;
            }
        }

        public static T GetDataContext(string schema)
        {
            if (string.IsNullOrEmpty(schema))
            {
#if DEBUG
                var defaultContext = ModelHelper<T>.GetDbContext();
                return defaultContext;
#else
          throw new HttpException(400, "未指定 Schema");
#endif
            }
            else
            {
                DbContextOptionsBuilder dbOptBuilder = new DbContextOptionsBuilder();
                var connStr = ConfigurationManager.ConnectionStrings[schema].ConnectionString;
							  _databaseType = DatabaseType.MSSql;
					      dbOptBuilder = dbOptBuilder.UseSqlServer(connStr);
                T context = Activator.CreateInstance(typeof(T), dbOptBuilder.Options) as T;
                ModelHelper<T>.RegisterLogging(context);
                return context;
            }
        }



        protected override T CreateDataSource()
        {
            var schema = DbSchemaHelper.GetSchema();
            if (string.IsNullOrEmpty(schema))
            {
#if DEBUG
                var context = ModelHelper<T>.GetDbContext();
                return context;
#else
          throw new HttpException(400, "未指定 Schema");
#endif
            }
            else
            {
								DbContextOptionsBuilder dbOptBuilder = new DbContextOptionsBuilder();
								var connStr = ConfigurationManager.ConnectionStrings[schema].ConnectionString;
								var providerName = ConfigurationManager.ConnectionStrings[schema].ProviderName;
								_databaseType = DatabaseType.MSSql;
								dbOptBuilder = dbOptBuilder.UseSqlServer(connStr);
								T context = Activator.CreateInstance(typeof(T), dbOptBuilder.Options) as T;
								ModelHelper<T>.RegisterLogging(context);
								return context;
            }
        }

        protected override void HandleException(HandleExceptionArgs args)
        {
            if (args.Exception is DataServiceException && ((DataServiceException)args.Exception).StatusCode == 404)
                return;

            ExceptionPolicy.HandleException(args.Exception, Constants.DefaultPolicy);
        }
    }
}
