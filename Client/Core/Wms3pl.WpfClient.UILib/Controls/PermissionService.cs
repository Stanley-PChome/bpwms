using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.UILib.Controls
{
	public partial class PermissionService
	{
		#region CheckPermission

		public static readonly DependencyProperty CheckPermissionProperty =
			DependencyProperty.RegisterAttached("CheckPermission", typeof(bool), typeof(PermissionService),
																					new PropertyMetadata(true));

		public static bool GetCheckPermission(DependencyObject obj)
		{
			return (bool)obj.GetValue(CheckPermissionProperty);
		}

		public static void SetCheckPermission(DependencyObject obj, bool value)
		{
			obj.SetValue(CheckPermissionProperty, value);
		}

		#endregion

		public static readonly DependencyProperty FunctionIdProperty =
			DependencyProperty.RegisterAttached("FunctionId", typeof(string), typeof(PermissionService),
			new PropertyMetadata(default(string), FunctionIdChangeCallBack));

		private static void FunctionIdChangeCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(d))
			{
				var control = d as Control;
				if (control != null)
				{
#if DEBUG
					if (Wms3plSession.Get<UserInfo>().Account.ToUpper() == "WMS")
						return;
#endif
					string functionId = (string)e.NewValue;
					//使用者可以用的功能
					var functions = Wms3plSession.Get<IEnumerable<Function>>();
					bool isEnabled = functions != null && functions.Any(f => f.Id == functionId);
					control.IsEnabled = isEnabled;
					if (!isEnabled)
					{
						control.Loaded += SetControlDisable;
					}
				}
			}
		}

		private static void SetControlDisable(object sender, RoutedEventArgs e)
		{
			var control = (Control)sender;
			BindingOperations.ClearBinding(control, IsEnabledProperty);
			if (control is Button)
				BindingOperations.ClearBinding(control, Button.CommandProperty);
			control.Loaded -= SetControlDisable;
			control.IsEnabled = false;
		}

		public static string GetFunctionId(DependencyObject obj)
		{
			return (string)obj.GetValue(FunctionIdProperty);
		}

		public static void SetFunctionId(DependencyObject obj, string value)
		{
			obj.SetValue(FunctionIdProperty, value);
		}

		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(PermissionService),
			new PropertyMetadata(default(bool), IsEnabledChangeCallBack));

		private static void IsEnabledChangeCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(d))
			{
				var control = d as Control;
				if (control != null && control.IsEnabled)
				{
#if DEBUG
					if (Wms3plSession.Get<UserInfo>().Account.ToUpper() == "WMS")
						return;
#endif
					string functionId = (string)PermissionService.GetFunctionId(d);
					//使用者可以用的功能
					var functions = Wms3plSession.Get<IEnumerable<Function>>();
					if (functions != null)
					{
						//control.Visibility = functions.Any(f => f.Id == functionId)?
						//  Visibility.Visible : Visibility.Collapsed;
						control.IsEnabled = control.IsEnabled & (functions.Any(f => f.Id == functionId) ?
						true : false);
					}
				}
			}
		}

		public static bool GetIsEnabled(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsEnabledProperty);
		}

		public static void SetIsEnabled(DependencyObject obj, bool value)
		{
			obj.SetValue(IsEnabledProperty, value);
		}

		public static readonly DependencyProperty PersonalDataProperty =
			DependencyProperty.RegisterAttached("PersonalDataId", typeof(string), typeof(PermissionService),
			new PropertyMetadata(default(string), PersonalDataIdChangeCallBack));

		private static void PersonalDataIdChangeCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(d))
			{
				if (d != null)
				{
					string personalDataId = (string)e.NewValue;
					//使用者可以瀏覽的個資 * 須改為個資權限設定Table
					var functions = Wms3plSession.Get<IEnumerable<Function>>();
					if (functions != null && !functions.Any(f => f.Id == personalDataId))
					{
						if (d is TextBlock)
						{
							var control = d as TextBox;
							control.IsReadOnly = true;
							control.Text = "".PadLeft(control.Text.Length, 'X');
						}
						else if (d is TextBlock)
						{
							var control = d as TextBlock;
							control.Text = "".PadLeft(control.Text.Length, 'X');
						}
						else if (d is ContentControl)
						{
							var control = d as ContentControl;
							control.Content = "".PadLeft(control.Content.ToString().Length, 'X');
						}
					}
				}
			}
		}

		public static string GetPersonalData(DependencyObject obj)
		{
			return (string)obj.GetValue(PersonalDataProperty);
		}

		public static void SetPersonalData(DependencyObject obj, string value)
		{
			obj.SetValue(PersonalDataProperty, value);
		}

		public static readonly DependencyProperty FunctionNameProperty =
			DependencyProperty.RegisterAttached("FunctionName", typeof(string), typeof(PermissionService));

		public static string GetFunctionName(DependencyObject obj)
		{
			return (string)obj.GetValue(FunctionNameProperty);
		}

		public static void SetFunctionName(DependencyObject obj, string value)
		{
			obj.SetValue(FunctionNameProperty, value);
		}

	}
}
