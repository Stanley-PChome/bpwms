using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.UILib.Controls
{
	public class ValidationDataGrid : DataGrid
	{
		public bool HasError
		{
			get { return (bool)GetValue(HasErrorProperty); }
			set { SetValue(HasErrorProperty, value); }
		}

		public static readonly DependencyProperty HasErrorProperty =
				DependencyProperty.Register("HasError", typeof(bool), typeof(ValidationDataGrid), new UIPropertyMetadata(false));



		protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
		{
			base.OnItemsSourceChanged(oldValue, newValue);
			HasError = false;
		}

		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnItemsChanged(e);
			HasError = false;
		}

		protected override void OnExecutedCommitEdit(ExecutedRoutedEventArgs e)
		{
			DataGridRow selectedRow = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as DataGridRow;
			base.OnExecutedCommitEdit(e);
			if (selectedRow != null)
			{
				HasError = Validation.GetHasError(selectedRow);
			}
		}

		protected override void OnExecutedCancelEdit(ExecutedRoutedEventArgs e)
		{
			base.OnExecutedCancelEdit(e);
			HasError = false;
		}
	}
}
