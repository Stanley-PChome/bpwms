using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.UILib
{
	public static class ToolbarButtonName {
		private static List<NameValuePair<string>> ToolbarButtons = new List<NameValuePair<string>>() {
			new NameValuePair<string>() {Name = "AddButton", Value = "新增"},
			new NameValuePair<string>() {Name = "FirstButton", Value = "首筆"},
			new NameValuePair<string>() {Name = "PriorButton", Value = "上筆"},
			new NameValuePair<string>() {Name = "NextButton", Value = "下筆"},
			new NameValuePair<string>() {Name = "LastButton", Value = "末筆"},
			new NameValuePair<string>() {Name = "SaveButton", Value = "確認"},
            new NameValuePair<string>() {Name = "ApproveButton", Value = "核准"},
            new NameValuePair<string>() {Name = "EditButton", Value = "編輯"},
			new NameValuePair<string>() {Name = "CancelButton", Value = "取消"},
			new NameValuePair<string>() {Name = "DeleteButton", Value = "刪除"},
			new NameValuePair<string>() {Name = "SearchButton", Value = "查詢"},
			new NameValuePair<string>() {Name = "PrintButton", Value = "列印"},
			new NameValuePair<string>() {Name = "PreviewButton", Value = "預覽"},
			new NameValuePair<string>() {Name = "ExportButton", Value = "匯出"},
			new NameValuePair<string>() {Name = "ExcelButton", Value = "匯出Excel"},
			new NameValuePair<string>() {Name = "ImportExcelButton", Value = "匯入Excel"},
			new NameValuePair<string>() {Name = "ImportButton", Value = "匯入"},
			new NameValuePair<string>() {Name = "ExitButton", Value = "離開"},
			new NameValuePair<string>() {Name = "CloseOutButton", Value = "結案"}
		};

		/// <summary>
		/// 回傳Toolbar按鈕文字, 傳入Style.
		/// 用在上版時的XAML內容解析
		/// </summary>
		/// <param name="styleKey"></param>
		/// <returns></returns>
		public static string GetButtonName(string styleKey) {
			var tmp = ToolbarButtons.FirstOrDefault(x => x.Name.Equals(styleKey));
			if (tmp == null) return string.Empty;
			return tmp.Value;
		}

		/// <summary>
		/// 傳入Attribute的值, Parse出其名稱
		/// </summary>
		/// <param name="attrValue"></param>
		/// <returns></returns>
		public static string GetProgramId(string attrValue)
		{
			var tmp = attrValue.Split('.');
			return tmp.Last();
		}
	}

}
