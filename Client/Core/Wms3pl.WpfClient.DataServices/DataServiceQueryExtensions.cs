using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.DataServices
{
	public static class DataServiceQueryExtensions
	{
		/// <summary>
		/// 依照傳入的 keyParamterObject 與要取得的 TEntity Key 組為查詢條件，並回傳第一筆符合的項目。
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="query"></param>
		/// <param name="keyParamterObject"></param>
		/// <returns></returns>
		public static TEntity FindByKey<TEntity>(this DataServiceQuery<TEntity> query, object keyParamterObject)
		 where TEntity : System.ComponentModel.INotifyPropertyChanged
		{
			return ConfigurationHelper.FindByKey<TEntity>(query, keyParamterObject);
		}

	}
}
