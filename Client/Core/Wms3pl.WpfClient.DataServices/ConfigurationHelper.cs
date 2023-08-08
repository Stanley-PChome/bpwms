using System;
using System.Collections;
using System.Linq;
using System.Configuration;
using System.Reflection;
using System.Data.Services.Client;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Common.WcfDataServices;
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.DataServices.F00DataService;
using Wms3pl.WpfClient.DataServices.F01DataService;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.DataServices.F05DataService;
using Wms3pl.WpfClient.DataServices.F14DataService;
using Wms3pl.WpfClient.DataServices.F15DataService;
using Wms3pl.WpfClient.DataServices.F16DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F20DataService;
using Wms3pl.WpfClient.DataServices.F25DataService;
using Wms3pl.WpfClient.DataServices.F70DataService;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.DataServices.F50DataService;
using Wms3pl.WpfClient.DataServices.F06DataService;

namespace Wms3pl.WpfClient.DataServices
{
	public class ConfigurationHelper
	{
		private static Hashtable DataServiceUriStrings
		{
			get
			{
				var uriStrings = new Hashtable();
				uriStrings.Add(typeof(F00Entities), "{0}/F00DataService.svc/");
				uriStrings.Add(typeof(F01Entities), "{0}/F01DataService.svc/");
				uriStrings.Add(typeof(F02Entities), "{0}/F02DataService.svc/");
				uriStrings.Add(typeof(F05Entities), "{0}/F05DataService.svc/");
        uriStrings.Add(typeof(F06Entities), "{0}/F06DataService.svc/");
				uriStrings.Add(typeof(F14Entities), "{0}/F14DataService.svc/");
				uriStrings.Add(typeof(F15Entities), "{0}/F15DataService.svc/");
				uriStrings.Add(typeof(F16Entities), "{0}/F16DataService.svc/");
				uriStrings.Add(typeof(F19Entities), "{0}/F19DataService.svc/");
				uriStrings.Add(typeof(F20Entities), "{0}/F20DataService.svc/");
				uriStrings.Add(typeof(F25Entities), "{0}/F25DataService.svc/");
				uriStrings.Add(typeof(F70Entities), "{0}/F70DataService.svc/");
				uriStrings.Add(typeof(F91Entities), "{0}/F91DataService.svc/");
				uriStrings.Add(typeof(F50Entities), "{0}/F50DataService.svc/");

				return uriStrings;
			}
		}

		public static T GetProxy<T>(bool isSecretePersonalData, string functionCode, bool enableIgnoreProperties = true, bool isLongTermSchema = false) where T : DataServiceContext
		{
			var apServer = ConfigurationManager.AppSettings["APServerUrl"];
			var uri = new Uri(string.Format(DataServiceUriStrings[typeof(T)].ToString(), apServer));
			var proxy = Activator.CreateInstance(typeof(T), uri, enableIgnoreProperties) as T;
			proxy.IgnoreResourceNotFoundException = true;
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			if (globalInfo != null)
			{
				globalInfo.IsSecretePersonalData = isSecretePersonalData;
				globalInfo.FunctionCode = functionCode;
			}

			if (!isLongTermSchema)
			{
				proxy.SendingRequest2 += DataServiceContextEx.OnSendingRequest;
				proxy.BuildingRequest += DataServiceContextEx.BuildingRequest;

			}
			else
			{
				proxy.SendingRequest2 += DataServiceContextEx.OnSendingRequestLongTermSchema;
				proxy.BuildingRequest += DataServiceContextEx.BuildingRequestLongTermSchema;

			}


			return proxy;
		}

		public static T GetProxyLongTermSchema<T>(bool isSecretePersonalData, string functionCode, bool enableIgnoreProperties = true) where T : DataServiceContext
		{
			return GetProxy<T>(enableIgnoreProperties, functionCode, true);
		}

		public static TEntity FindByKey<TEntity>(object keyConditionObject, bool isSecretePersonalData, string functionCode, bool enableIgnoreProperties = true, bool isLongTermSchema = false)
		{
			var query = GetProxyQuery<TEntity>(keyConditionObject, isSecretePersonalData, functionCode, enableIgnoreProperties, isLongTermSchema);
			if (query == null)
				return default(TEntity);

			return query.SingleOrDefault();
		}

		static System.Data.Services.Client.DataServiceQuery<TEntity> GetProxyQuery<TEntity>(object keyConditionObject, bool isSecretePersonalData, string functionCode, bool enableIgnoreProperties = true, bool isLongTermSchema = false)
		{
			// 0.取 FXX
			var entityType = typeof(TEntity);
			var entityName = entityType.Name;
			var fxx = entityName.Substring(0, 3);

			// 1.取 Proxy Helpter
			var method = typeof(Wms3pl.WpfClient.DataServices.ConfigurationHelper).GetMethod("GetProxy", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

			// 2.取 Fxx Entities
			var entitiesType = Type.GetType(string.Format("Wms3pl.WpfClient.DataServices.{0}DataService.{0}Entities, Wms3pl.WpfClient.DataServices", fxx));
			method = method.MakeGenericMethod(entitiesType);
			var proxy = method.Invoke(null, new object[] { isSecretePersonalData, functionCode, enableIgnoreProperties, isLongTermSchema });

			// 1.取 FxxEntities 的屬性
			var fxxEntityProperty = proxy.GetType().GetProperty(entityName + "s");	// s是wms專案訂的格式，表示該 entity 的複數
			var entitiesDataServiceQuery = fxxEntityProperty.GetValue(proxy);
			return GetProxyQuery<TEntity>(entitiesDataServiceQuery, keyConditionObject);
		}

		public static TEntity FindByKey<TEntity>(DataServiceQuery<TEntity> entitiesDataServiceQuery, object keyParamterObject)
		{
			var query = GetProxyQuery<TEntity>(entitiesDataServiceQuery, keyParamterObject);
			if (query == null)
				return default(TEntity);

			return query.SingleOrDefault();
		}

		static System.Data.Services.Client.DataServiceQuery<TEntity> GetProxyQuery<TEntity>(object entitiesDataServiceQuery, object keyConditionObject)
		{
			var entityType = typeof(TEntity);
			var entitiesDataServiceQueryType = entitiesDataServiceQuery.GetType();

			// 2.組條件
			var addQueryMethod = entitiesDataServiceQueryType.GetMethod("AddQueryOption");

			// 3.取該Entity的所有PK Names
			var dataServiceKeyAttribute = entityType.GetCustomAttribute<System.Data.Services.Common.DataServiceKeyAttribute>();
			if (dataServiceKeyAttribute == null)
				throw new Exception("GetProxyQuery<T> 的泛型T沒有 DataServiceKeyAttribute 特性");

			// 4.將來源Entity的值組成Condition
			var sourceType = keyConditionObject.GetType();

			var keyConditions = dataServiceKeyAttribute.KeyNames.Select(key =>
			{
				var prop = sourceType.GetProperty(key);
				if (prop == null || !prop.CanRead)
					return null;

				var value = prop.GetValue(keyConditionObject);
				if (value == null)
					return null;

				value = DataServiceQueryHelper.CombineTypeFormatString(value);

				var condition = string.Format("{0} eq {1}", key, value);
				return condition;
			});

			if (keyConditions.Any(condition => condition == null))
				throw new ArgumentException("keyConditionObject 的 Key 不能為 null");

			// 使用標準 DataServiceQuery 語法
			var filter = string.Join(" and ", keyConditions);
			entitiesDataServiceQuery = addQueryMethod.Invoke(entitiesDataServiceQuery, new object[] { "$filter", filter });
			return entitiesDataServiceQuery as System.Data.Services.Client.DataServiceQuery<TEntity>;
		}
	}
}
