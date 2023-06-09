using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.ExDataServices.P01ExDataService;

namespace Wms3pl.WpfClient.ExDataServices
{
	public static class ExDataMapper
	{
		/// <summary>
		/// 建立 Wcf 自動產生的 Entity，將來源 Entity 屬性內容對應到新的 Wcf Entity。
		/// </summary>
		/// <typeparam name="TSource">Entity 物件</typeparam>
		/// <typeparam name="TDestination">Wcf 產生物件的型別</typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static TDestination Map<TSource, TDestination>(this TSource source)
		{
			var destinationInstance = Activator.CreateInstance<TDestination>();

			return CloneProperties(source, destinationInstance);
		}

		/// <summary>
		/// 應用於如果要複製清單內容的，產生物件的方式是使用 Map。
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TDestination"></typeparam>
		/// <param name="sourceCollection"></param>
		/// <returns></returns>
		public static IEnumerable<TDestination> MapCollection<TSource, TDestination>(this IEnumerable<TSource> sourceCollection)
		{
			foreach (var source in sourceCollection)
			{
				var destinationInstance = Map<TSource, TDestination>(source);
				yield return destinationInstance;
			}
		}

		/// <summary>
		/// 將相同的屬性內容從 source 做淺層複製到 destination 物件，不存在的屬性則會略過複製。
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TDestination"></typeparam>
		/// <param name="source">一般類別</param>
		/// <param name="destination">destination 支援為 WCF 自動產生的物件。</param>
		public static TDestination CloneProperties<TSource, TDestination>(this TSource source, TDestination destination)
		{
			// 是否為 Wcf 自動產生的 Entity
			bool isWcfAutoCreateEntity = Attribute.IsDefined(typeof(TDestination), typeof(System.Runtime.Serialization.DataContractAttribute));

			foreach (var p in typeof(TSource).GetProperties())
			{
				if (!p.CanWrite)
					continue;

				// 若為 Wcf 自動產生的物件，則欄位名稱後面加上k__BackingField，否則使用原本的屬性名稱(DataService用)
				string propertyName = isWcfAutoCreateEntity ? string.Format("{0}k__BackingField", p.Name) : p.Name;
				var destinationProperty = typeof(TDestination).GetProperty(propertyName);

				//若為 Wcf 自動產生的物件，private欄位名稱後面加上Field，public欄位則維持原來名稱，ex private OIDField , public OID
				//否則使用原本的屬性名稱(ExDataService用)
				if (destinationProperty == null)
				{
					destinationProperty = typeof(TDestination).GetProperty(p.Name);
				}

				// 也可能是從 wcf 轉不是 wcf 的物件
				if (destinationProperty == null)
				{
					destinationProperty = typeof(TDestination).GetProperty(p.Name.Replace("k__BackingField", string.Empty));
				}

				// 若目的地物件不包含原本 Entity 的屬性，則會略過。
				if (destinationProperty != null)
				{
					var sourceValue = p.GetValue(source, null);
					destinationProperty.SetValue(destination, sourceValue, null);
				}
			}

			return destination;
		}

		/// <summary>
		/// 建立目前 T 的淺層複本 (Shallow Copy)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		public static T Clone<T>(this T source)
		{
			if (source == null)
				return default(T);

			var destinationInstance = Activator.CreateInstance<T>();

			return CloneProperties(source, destinationInstance);
		}

		/// <summary>
		/// 將物件的所有字串屬性成員做 Trim()，並返回原本相同參考位址的物件。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static T Trim<T>(this T obj) where T : class
		{
			if (obj != null)
			{
				var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
				foreach (var p in properties)
				{
					if (p.CanWrite && p.PropertyType == typeof(string))
					{
						var str = p.GetValue(obj, null);
						if (str != null)
						{
							p.SetValue(obj, str.ToString().Trim());
						}
					}
				}
			}

			return obj;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="destination">建議是從 FXXEntities 取得的 Entity</param>
		/// <param name="ignoreSetKeyAndHistoryProperties">是否忽略設定 PK 與 CRT_STAFF, CRT_DATE, CRT_NAME, UPD_STAFF, UPD_DATE, UPD_NAME</param>
		/// <param name="ignoreProperties">自訂要忽略的屬性</param>
		public static void SetProperties<T>(this T source, T destination, bool ignoreSetKeyAndHistoryProperties = false, params string[] ignoreProperties)
		{
			// 取得要忽略設定的屬性名稱
			string[] keyNames = null;
			if (!ignoreSetKeyAndHistoryProperties)
			{
				var historyProperties = new string[] { "CRT_STAFF", "CRT_DATE", "CRT_NAME", "UPD_STAFF", "UPD_DATE", "UPD_NAME" };

				var dataServiceKeyAttribute = typeof(T).GetCustomAttribute<System.Data.Services.Common.DataServiceKeyAttribute>();
				keyNames = (dataServiceKeyAttribute != null)
						 ? dataServiceKeyAttribute.KeyNames.Union(historyProperties).ToArray()
						 : historyProperties;
			}

			// 將原來屬性值複製到目的屬性值
			foreach (var p in typeof(T).GetProperties())
			{
				if (!p.CanWrite)
					continue;

				if (!ignoreSetKeyAndHistoryProperties && keyNames.Contains(p.Name))
					continue;

				if (ignoreProperties.Contains(p.Name))
					continue;

				var sourceValue = p.GetValue(source, null);
				p.SetValue(destination, sourceValue, null);
			}
		}

        public static object Map<T1, T2>(List<F010301Main> mainData)
        {
            throw new NotImplementedException();
        }
    }
}
