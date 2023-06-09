using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.Common.Helpers;

namespace Wms3pl.WpfClient.ExDataServices
{
	public static class ExQueryExtensions
	{
		/// <summary>
		/// 如果是字串，則會自動加上單引號，若為 null 會傳回相同的 Query，然後呼叫原本的 AddQueryExOption 並回傳。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query"></param>
		/// <param name="name">字串值，包含要加入之查詢字串選項的名稱。</param>
		/// <param name="value">物件，包含查詢字串選項的值。</param>
		/// <returns>包含要求之查詢選項的新查詢，且此查詢選項已附加至提供之查詢的 URI</returns>
		public static DataServiceQuery<T> AddQueryExOption<T>(this DataServiceQuery<T> query, string name, object value)
		{
			if (value == null)
				return query;

			query = query.AddQueryOption(name, DataServiceQueryHelper.CombineTypeFormatString(value));
			return query;
		}
	}
}
