using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F19;


namespace Wms3pl.WebServices.Trans.Shared.Helper
{
	/// <summary>
	/// 文字內容切割
	/// </summary>
	public static class FileContentCutHelper
	{
		/// <summary>
		/// 文字內容切割By區間轉對應物件
		/// </summary>
		/// <typeparam name="T">對應物件</typeparam>
		/// <param name="fileContentMapColumnList">對應物件欄位設定</param>
		/// <param name="content">文字內容</param>
		/// <returns></returns>
		public static T ConvertFileContentByIntervalToObject<T>(List<F190010> fileContentMapColumnList,string content)
		{
			var obj = Activator.CreateInstance<T>();
			foreach (var property in typeof(T).GetProperties())
			{
				if(property.CanRead && property.CanWrite)
				{
					var findMap = fileContentMapColumnList.FirstOrDefault(x => x.COL_NAME == property.Name);
					if(findMap!=null)
					{
						var maxIndex = fileContentMapColumnList.Select(x => x.BEGIN_INDEX).Max();
						var value = string.Empty;
						//最後一個索引取到底或無結束索引位置
						if (findMap.BEGIN_INDEX == maxIndex || !findMap.END_INDEX.HasValue)
							value = content.Substring((int)findMap.BEGIN_INDEX);
						else
							value = content.Substring((int)findMap.BEGIN_INDEX, (int)(findMap.END_INDEX.Value - findMap.BEGIN_INDEX  + 1));

						SetPadChar(findMap, ref value);
						property.SetValue(obj,value);

					}
				}
			}
			return obj; 
		}
		/// <summary>
		/// 文字內容切割By分隔字元轉對應物件
		/// </summary>
		/// <typeparam name="T">對應物件</typeparam>
		/// <param name="fileContentMapColumnList">對應物件欄位設定</param>
		/// <param name="Separate">分隔字元</param>
		/// <param name="content">文字內容</param>
		/// <returns></returns>
		public static T ConvertFileContentByBySeparateToObject<T>(List<F190010> fileContentMapColumnList,char[] Separate,  string content)
		{
			var obj = Activator.CreateInstance<T>();
			foreach (var property in typeof(T).GetProperties())
			{
				if (property.CanRead && property.CanWrite)
				{
					var findMap = fileContentMapColumnList.FirstOrDefault(x => x.COL_NAME == property.Name);
					if (findMap != null)
					{
						var value = content.Split(Separate)[findMap.BEGIN_INDEX];
						SetPadChar(findMap, ref value);
						property.SetValue(obj, value);
					}
				}
			}
			return obj;
		}

		/// <summary>
		/// 依設定靠左或靠右補字元
		/// </summary>
		/// <param name="findMap"></param>
		/// <param name="value"></param>
		private static void SetPadChar(F190010 findMap,ref string value)
		{
			if (!string.IsNullOrWhiteSpace(findMap.PAD_ADDCHAR))
			{
				switch (findMap.PAD_TYPE)
				{
					case "L":
						value = value.PadLeft((int)(findMap.COL_LENGTH ?? 0), findMap.PAD_ADDCHAR[0]);
						break;
					case "R":
						value = value.PadRight((int)(findMap.COL_LENGTH ?? 0), findMap.PAD_ADDCHAR[0]);
						break;
				}
			}
		}
	}
}
