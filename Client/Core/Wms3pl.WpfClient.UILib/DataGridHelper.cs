using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Controls;
using Telerik.Windows.Controls;
using System.Windows.Media;
using System.Data;

namespace Wms3pl.WpfClient.Common
{
	public class DataGridHelper
	{
		public static void FocusCell(DataGrid dg, int rowindex, int columnindex)
		{
			// select the new cell
			var dataGridCell = GetCell(dg, rowindex, columnindex);
			//dataGridCell.Focus();
			var tb = dataGridCell.FindChildrenByType<Control>().FirstOrDefault();
			if (tb != null)
			{
				tb.Focus();
			}
			dg.SelectedItem = null; //每次點選到新的cell，就先把DataGrid原先選擇到的Row清除，才不會變多選
			if (dg.SelectionUnit != DataGridSelectionUnit.FullRow)
			{
				if (!dataGridCell.IsSelected)
					dataGridCell.IsSelected = true;
			}
			else
			{
				var row = VisualTreeHelperExtension.FindVisualParent<DataGridRow>(dataGridCell);
				if (row != null && !row.IsSelected)
				{
					row.IsSelected = true;
				}
			}
		}

		public static void FocusTelerikRadMaskedDateTimeInputCell(DataGrid dg, int rowindex, int columnindex)
		{
			// select the new cell
			var dataGridCell = GetCell(dg, rowindex, columnindex);
			//dataGridCell.Focus();      
			var tb = dataGridCell.FindChildrenByType<RadMaskedDateTimeInput>().FirstOrDefault();
			if (tb != null)
			{
				tb.Focus();
				var textBox = tb.FindChildByType<TextBox>();
				textBox.AcceptsReturn = false;
				textBox.Focus();
			}
			dg.SelectedItem = null; //每次點選到新的cell，就先把DataGrid原先選擇到的Row清除，才不會變多選
			if (dg.SelectionUnit != DataGridSelectionUnit.FullRow)
			{
				if (!dataGridCell.IsSelected)
					dataGridCell.IsSelected = true;
			}
			else
			{
				var row = VisualTreeHelperExtension.FindVisualParent<DataGridRow>(dataGridCell);
				if (row != null && !row.IsSelected)
				{
					row.IsSelected = true;
				}
			}
		}

		public static DataGridCell GetCell(DataGrid dg, int row, int column)
		{
			var rowContainer = GetRow(dg, row);

			if (rowContainer != null)
			{
				var presenter = VisualTreeHelperExtension.GetVisualChild<DataGridCellsPresenter>(rowContainer);

				// try to get the cell but it may possibly be virtualized
				var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
				if (cell == null)
				{
					// now try to bring into view and retreive the cell
					dg.ScrollIntoView(rowContainer, dg.Columns[column]);
					cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
				}
				return cell;
			}
			return null;
		}

		public static DataGridRow GetRow(DataGrid dg, int index)
		{
			var row = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(index);
			if (row == null)
			{
				// may be virtualized, bring into view and try again
				dg.ScrollIntoView(dg.Items[index]);
				row = (DataGridRow)dg.ItemContainerGenerator.ContainerFromIndex(index);
			}
			return row;
		}


		public static DataTable ConvertDataGridToDataTable(DataGrid dataGrid)
		{
			var dt = new DataTable();
			var cellCount = dataGrid.Columns.Count();
			foreach (var col in dataGrid.Columns)
			{
				var header = string.Empty;
				if (col.Header == null && col.HeaderTemplate != null)
				{
					var headerContent = col.HeaderTemplate.LoadContent();
                    if (headerContent is TextBlock)
                        header = ((TextBlock)headerContent).Text;
                    else if (headerContent is CheckBox)
                    {
                        //header = "選擇";
                        header = (headerContent as CheckBox).Content.ToString();
                    }
                    else
					{
						var textBlock = GetVisualChild<TextBlock>((Visual)headerContent);
						header = textBlock.Text;
					}
				}
				else if (col.Header != null)
					header = col.Header.ToString();
				dt.Columns.Add(header);
			}
			foreach (var item in dataGrid.Items)
			{
				// find row for the first selected item
				DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(item);
				if (row == null)
				{
					dataGrid.ScrollIntoView(item);
					dataGrid.UpdateLayout();
					row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(item);
				}
				if (row != null)
				{
					var dr = dt.NewRow();
					DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
					// find grid cell object for the cell with index 0
					for (var i = 0; i < cellCount; i++)
					{
						DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(i) as DataGridCell;
						if (cell == null)
						{
							dataGrid.ScrollIntoView(row, dataGrid.Columns[i]);
							cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(i);
						}

						if (cell != null)
						{
							if (cell.Content is TextBlock)
								dr[i] = ((TextBlock)cell.Content).Text;
							else if (cell.Content is CheckBox)
								dr[i] = ((CheckBox)cell.Content).IsChecked.ToString();
							else
							{
								var textBlock = GetVisualChild<TextBlock>((Visual)cell.Content);
								dr[i] = textBlock != null ? textBlock.Text : string.Empty;
							}
						}
					}
					dt.Rows.Add(dr);
				}
			}
			return dt;
		}

		public static T GetVisualChild<T>(Visual parent) where T : Visual
		{
			T child = default(T);
			int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < numVisuals; i++)
			{
				Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
				child = v as T;
				if (child == null) child = GetVisualChild<T>(v);
				if (child != null) break;
			}
			return child;
		}
	}
}
