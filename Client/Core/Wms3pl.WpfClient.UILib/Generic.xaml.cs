using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Data;
using Wms3pl.WpfClient.Common;

namespace Wms3pl.WpfClient.UILib
{
	public partial class Generic
	{
		#region 編輯Grid設定
		private bool _isClick = false;
		private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			DataGridCell cell = sender as DataGridCell;
			if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
			{
				_isClick = true;
				if (!cell.IsFocused)
				{
					cell.Focus();
				}
				DataGrid dataGrid = VisualTreeHelperExtension.FindVisualParent<DataGrid>(cell);
				if (dataGrid != null)
				{
					DataGridRow row = VisualTreeHelperExtension.FindVisualParent<DataGridRow>(cell);
					if (!row.DataContext.Equals(dataGrid.SelectedItem))
						dataGrid.SelectedItem = null; //每次點選到新的cell，就先把DataGrid原先選擇到的Row清除，才不會變多選
					if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
					{
						if (!cell.IsSelected)
							cell.IsSelected = true;
					}
					else
					{
						if (row != null && !row.IsSelected)
						{
							row.IsSelected = true;
						}
					}
				}
			}
			_isClick = false;
		}

		protected void DataGridCell_GotFocus(object sender, RoutedEventArgs e)
		{
			var cell = ((DataGridCell)sender);
			if (!_isClick && cell.IsEnabled && !cell.IsReadOnly)
			{
				cell.IsEditing = true;
			}
			_isClick = false;
		}

		protected void DataGridCell_MouseRightButtonUp(object sender, RoutedEventArgs e)
		{
			var cell = ((DataGridCell)sender);

			if (cell.IsSelected && !cell.IsFocused)
				cell.Focus();
		}


		private void CopyColumnData(object sender, RoutedEventArgs e)
		{
			MenuItem menuItem = sender as MenuItem;
			if (menuItem != null)
			{
				var x = menuItem.CommandParameter;
				ContextMenu cm = menuItem.CommandParameter as ContextMenu;
				if (cm != null)
				{
					DataGrid gvItem = cm.PlacementTarget as DataGrid;
					GridCopyFieldValue(gvItem);
				}
			}
		}

		protected void GridCopyFieldValueExecuted(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
		{
			var grid = sender as DataGrid;
			GridCopyFieldValue(grid);
		}

		private void GridCopyFieldValue(DataGrid grid)
		{
			if (grid.SelectedIndex < 0 || grid.CurrentCell == null || grid.CurrentCell.Column == null)
				return;
			var data = grid.CurrentCell.Item;
			var cellContent = grid.CurrentCell.Column.GetCellContent(data);
			if (cellContent == null)
			{
				var item = grid.Items[grid.SelectedIndex - 1];
				Type type = item.GetType();
				var pi = type.GetProperty(grid.CurrentCell.Column.SortMemberPath);
				object priorValue = pi.GetValue(item);
				Clipboard.SetText(priorValue.ToString());
			}
			else if (cellContent is TextBlock)
				Clipboard.SetText(((TextBlock)cellContent).Text);
			else
			{
				var textBlock = DataGridHelper.GetVisualChild<TextBlock>((Visual)cellContent);
				Clipboard.SetText(textBlock.Text);
			}
			//Clipboard.SetText(data.GetType().GetProperty(grid.CurrentCell.Column.SortMemberPath).GetValue(data).ToString());
		}
        #endregion

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            var text = ((TextBox)sender).Text;
            int len = text.Length;

            //檢核第1個字元 0~9
            int position = CheckDate(text, 49, 57, 0, 0);
            if (position != -1)
                ((TextBox)sender).Text = "";
            //檢核第2~4個字元 1~9
            if (len >= 2)
            {
                position = CheckDate(text, 48, 57, 1, 3);
                if (position != -1)
                    ((TextBox)sender).Text = text.Substring(0, position);
            }
            //檢核第5個字元 /
            if (len >= 5)
            {
                position = CheckDate(text, 47, 47, 4, 4);
                if (position != -1)
                    ((TextBox)sender).Text = text.Substring(0, position);
            }
            
            if (len >= 6)
            {
                //先檢核第6個字元是否為數字
                position = CheckDate(text, 48, 57, 5, 5);
                if (position != -1)
                    ((TextBox)sender).Text = text.Substring(0, position);

                
                if (text[5] == 48)//第6個字元 0
                {
                    //檢核第7個字元 1~9
                    position = CheckDate(text, 49, 57, 6, 6);
                    if (position != -1)
                        ((TextBox)sender).Text = text.Substring(0, position);
                }
                else if (text[5] == 49)//第6個字元 1
                {
                    //檢核第7個字元 0~2
                    position = CheckDate(text, 48, 50, 6, 6);
                    if (position != -1)
                        ((TextBox)sender).Text = text.Substring(0, position);
                }
                else //第6個字元 2-9
                    ((TextBox)sender).Text = text.Substring(0, 6);
                
            }
            //超過7個字元的處理
            if (len > 7)
                ((TextBox)sender).Text = text.Substring(0, 7);
        }

        /// <summary>
        /// 檢核char範圍是否符合條件
        /// 不符合的話，會回傳該位置的index
        /// </summary>
        /// <param name="s">要驗證的字串</param>
        /// <param name="charStart">起始char範圍</param>
        /// <param name="charEnd">結束char範圍</param>
        /// <param name="start">起始驗證字元位置</param>
        /// <param name="end">結束驗證字元位置</param>
        /// <returns></returns>
        private int CheckDate(string s, int charStart, int charEnd, int start, int end)
        {
            var ch = s.ToCharArray();

            for (int i = start; i <= end && i < ch.Length; i++)
                if (ch[i] < charStart || charEnd < ch[i])
                    return i;
            return -1;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var text = ((TextBox)sender).Text;

            //長度剛好六碼的處理
            if (((TextBox)sender).Text.Length == 6)
            {
                //驗證第6個字元是否為0
                int position = CheckDate(text, 48, 48, 5, 5);
                if (position == -1)
                    ((TextBox)sender).Text = "";
                else
                    ((TextBox)sender).Text = text.Insert(5, "0");//如果第6個字元非0的話，就插入0
            }
            //不足字元數的處理
            if (((TextBox)sender).Text.Length < 7)
                ((TextBox)sender).Text = "";
        }
    }
}
