using System.Windows;

namespace Wms3pl.WpfClient.Common
{
	public class Wms3plSession
	{
		public static T Get<T>()  where T : class
		{
			return Application.Current.Properties[typeof(T).ToString()] as T;
		}

		public static void Set<T>(T t) where T : class
		{
			Application.Current.Properties[typeof(T).ToString()] = t;
		}

		/// <summary>
		/// 取得登入者的資訊
		/// </summary>
		public static UserInfo CurrentUserInfo
		{
			get
			{
				return Get<UserInfo>();
			}
		}
	}
}
