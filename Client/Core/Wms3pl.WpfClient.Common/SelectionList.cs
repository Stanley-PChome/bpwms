using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Wms3pl.WpfClient.Common
{
	public class SelectionList<T> : ObservableCollection<SelectionItem<T>>
	{
		public SelectionList(IEnumerable<T> list, bool isSelected = false) :
			base(ToItemEnumerable(list, isSelected))
		{
		}

		private static IEnumerable<SelectionItem<T>> ToItemEnumerable(IEnumerable<T> items, bool isSelected = false)
		{
			var list = new List<SelectionItem<T>>();
			if (items != null)
			{
				list.AddRange(items.Select(item => new SelectionItem<T>(item, isSelected)));
			}
			return list;
		}

		/// <summary>
		/// 若預先設定 IsSelected 等屬性，可直接帶 SelectionItem 型別初始化
		/// </summary>
		/// <param name="selectionItems"></param>
		public SelectionList(IEnumerable<SelectionItem<T>> selectionItems)
			: base(selectionItems)
		{
		}
	}

	public static class SelectionListExtension
	{
		public static SelectionList<T> ToSelectionList<T>(this IEnumerable<T> items, bool isSelected = false)
		{
			SelectionList<T> list = new SelectionList<T>(items, isSelected);
			return list;
		}

		public static SelectionList<DataRow> ToSelectionList(this DataTable table)
		{
			var q = from DataRow r in table.Rows
							select r;
			return new SelectionList<DataRow>(q);
		}
	}
}
