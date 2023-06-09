using System;
namespace Wms3pl.WpfClient.Common
{
	public class WCFHelper
	{
		/// <summary>
		/// 移除WCF回傳ToDataTable多餘的字串
		/// -
		/// WCF取得資料ToDataTable時,會產生額外的字串作為別名
		/// </summary>
		/// <param name="columnName">欄位名稱</param>
		public static string ReplaceColumnName(string columnName)
		{
			return columnName.Replace("k__BackingField", string.Empty);
		}
	}
}
