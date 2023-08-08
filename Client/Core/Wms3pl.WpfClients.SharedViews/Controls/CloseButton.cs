using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Wms3pl.WpfClient.UILib;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Docking;

namespace Wms3pl.WpfClients.SharedViews.Controls
{
	public static class CloseButton
	{
		public static readonly DependencyProperty IsCloseButtonProperty =
			DependencyProperty.RegisterAttached("IsCloseButton", typeof(bool), typeof(CloseButton), new PropertyMetadata(OnIsCloseButtonChanged));

		public static readonly DependencyProperty HideInAutoHideAreaProperty =
			DependencyProperty.RegisterAttached("HideInAutoHideArea", typeof(bool), typeof(CloseButton), new PropertyMetadata(OnHideInAutoHideAreaChanged));

		public static bool GetIsCloseButton(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsCloseButtonProperty);
		}

		public static bool GetHideInAutoHideArea(DependencyObject obj)
		{
			return (bool)obj.GetValue(HideInAutoHideAreaProperty);
		}

		public static void SetIsCloseButton(DependencyObject obj, bool value)
		{
			obj.SetValue(IsCloseButtonProperty, value);
		}

		public static void SetHideInAutoHideArea(DependencyObject obj, bool value)
		{
			obj.SetValue(HideInAutoHideAreaProperty, value);
		}

		private static void OnIsCloseButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var oldValue = (bool)e.OldValue;
			var newValue = (bool)e.NewValue;
			var button = d as ButtonBase;

			if (button != null && oldValue != newValue)
			{
				if (!oldValue)
				{
					button.Click += OnCloseButtonClick;
				}
				else
				{
					button.Click -= OnCloseButtonClick;
				}
			}
		}

		private static void OnHideInAutoHideAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var oldValue = (bool)e.OldValue;
			var newValue = (bool)e.NewValue;
			var button = d as FrameworkElement;

			if (button != null && oldValue != newValue)
			{
				if (!oldValue)
				{
					button.Loaded += OnCloseButtonLoaded;
				}
				else
				{
					button.Loaded -= OnCloseButtonLoaded;
				}
			}
		}

		private static void OnCloseButtonClick(object sender, RoutedEventArgs args)
		{
			var button = sender as FrameworkElement;

			if (button != null)
			{
				var pane = button.ParentOfType<RadPane>();
				var group = pane.ParentOfType<RadPaneGroup>();
				if (pane != null)
				{
					Action close = () =>
														 {
															 pane.IsHidden = true; //會觸發 RadDocking 的 PreviewClose
															 pane.DataContext = null;
															 pane.Header = null;
															 pane.Content = null;
															 pane.RemoveFromParent();
															 group.Items.Remove(pane);
														 };
					if (pane.Content is Wms3plUserControl)
					{
						((Wms3plUserControl)pane.Content).Close(close); //為了呼叫 Wms3plUserControl 的 OnClosing
					}
					else
					{
						close();
					}

				}
			}
			args.Handled = true;
		}

		private static void OnCloseButtonLoaded(object sender, RoutedEventArgs args)
		{
			var button = sender as FrameworkElement;

			if (button != null)
			{
				var pane = button.ParentOfType<RadPane>();
				if (pane != null)
				{
					button.Visibility =
						pane.Parent is AutoHideArea
								? Visibility.Collapsed
								: Visibility.Visible;
				}
			}
		}
	}
}
