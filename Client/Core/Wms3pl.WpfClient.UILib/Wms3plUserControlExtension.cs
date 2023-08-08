using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using Telerik.Windows.Controls;

namespace Wms3pl.WpfClient.UILib
{
	public static class Wms3plUserControlExtension
	{
		public static void ShowExceptionMessage(this UserControl userControl, Exception exception)
		{
			ShowExceptionMessageInternal(userControl, exception);
		}
		public static void ShowExceptionMessage(this Wms3plWindow window, Exception exception)
		{
			ShowExceptionMessageInternal(window, exception);
		}

		private static void ShowExceptionMessageInternal(Control control, Exception exception)
		{
			var master = control.FindName("Master") as BasicMaster;
			var button = new Button
			{
				Content = new TextBlock() { Text = "系統發生錯誤!", TextWrapping = TextWrapping.Wrap },
				Style = control.FindResource("LinkButtonStyle") as Style,
				Foreground = Brushes.Red
			};

			button.Click += (sender, e) =>
			{
				var viewer = new ScrollViewer
				{
					Content = new TextBox
					{
						Text = GetExceptionMessage(exception)
					},
					HorizontalScrollBarVisibility = ScrollBarVisibility.Visible
				};
				var win = new Window { Content = viewer };
				win.ShowDialog();
			};

			if (master != null)
			{
				master.Message = button;
				master.HasError = true;
			}
			else
			{
				var errorPanel = control.FindName("Errors") as WrapPanel;
				errorPanel.Children.Add(button);
				errorPanel.Visibility = Visibility.Visible;
			}

			ChangeHeaderTemplate(control, "ErrorHeaderTemplate", true);
		}

		/// <summary>
		/// 顯示忙碌的 HeaderTemplate
		/// </summary>
		/// <param name="control"></param>
		public static void ShowRunning(this Control control)
		{
			ChangeHeaderTemplate(control, "BusyHeaderTemplate", false);
		}

		/// <summary>
		/// 切換 HeaderTemplate
		/// </summary>
		/// <param name="control"></param>
		/// <param name="headerTemplate"></param>
		/// <param name="setTabIsSelected"></param>
		public static void ChangeHeaderTemplate(this Control control, string headerTemplate, bool setTabIsSelected)
		{
			var doc = control.Parent as RadDocumentPane;
			if (doc != null)
			{
				doc.TitleTemplate = doc.HeaderTemplate = control.FindResource(headerTemplate) as DataTemplate;
				if (setTabIsSelected)
					doc.IsSelected = true;
			}
		}

		public static string GetExceptionMessage(Exception e)
		{
			var builder = new StringBuilder();
		
			if (e is System.ServiceModel.FaultException<System.ServiceModel.ExceptionDetail>) {
				var eDetail = ((System.ServiceModel.FaultException<System.ServiceModel.ExceptionDetail>)e).Detail;
				do
				{
					builder.AppendFormat("Message:{0}\nStackTrace:{1}\n\n", eDetail.Message, eDetail.StackTrace);
					eDetail = eDetail.InnerException;
				} while (eDetail != null);
			}
			else
			{
				do
				{
					builder.AppendFormat("Message:{0}\nStackTrace:{1}\n\n", e.Message, e.StackTrace);
					e = e.InnerException;
				} while (e != null);
			}
			return builder.ToString();
		}

		public static void ShowMessage(this Wms3plUserControl userControl, string message, bool hasError = false)
		{
			if (userControl.CheckAccess())
			{
				var master = userControl.FindName("Master") as BasicMaster;
				if (master != null)
				{
					master.Message = message;
					master.HasError = hasError;
				}

				var headerTemplate = hasError ? "ErrorHeaderTemplate" : "NormalHeaderTemplate";
				var doc = userControl.Parent as RadDocumentPane;
				if (doc != null)
				{
					doc.TitleTemplate = doc.HeaderTemplate = userControl.FindResource(headerTemplate) as DataTemplate;
					doc.IsSelected = true;
				}
			}
		}

		public static void ShowStatusMessage(this Wms3plUserControl userControl, string message)
		{
			Messenger.Default.Send<NotificationMessage<string>>(
				new NotificationMessage<string>(userControl, message, "Show"));
		}
	}
}