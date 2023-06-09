using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using MessageBoxUtils;
using Wms3pl.WpfClient.UILib.Controls;
using WPFCustomMessageBox;

namespace Wms3pl.WpfClient.UILib.Services
{
	public partial class WpfDialogService : IDialogService
	{
		private Window _windowOwner;
		private Dispatcher _dispatcher;

		#region ctor

		public WpfDialogService()
		{
			_dispatcher = Dispatcher.CurrentDispatcher;
			SetOwnerWindow();
		}

		public WpfDialogService(FrameworkElement view)
		{
			View = view;
		}

		#endregion

		private void SetOwnerWindow()
		{
			if (View != null)
			{
				_dispatcher = View.Dispatcher;
				if (View is Window)
					_windowOwner = View as Window;
				else if (View is Wms3plPage)
					_windowOwner = (View as Wms3plPage).Parent as Window;
				else if (View is Wms3plUserControl)
					_windowOwner = Application.Current.MainWindow;
				else
				{
					_windowOwner = Window.GetWindow(View);
				}
			}
			else
			{
				if (_windowOwner == null)
					_windowOwner = Application.Current.MainWindow;
			}
		}

		#region Properties

		private FrameworkElement _view;

		public FrameworkElement View
		{
			get { return _view; }
			set
			{
				_view = value;
				SetOwnerWindow();
			}
		}

		#endregion



		static MessageBoxButton GetButton(DialogButton button)
		{
			switch (button)
			{
				case DialogButton.OK: return MessageBoxButton.OK;
				case DialogButton.OKCancel: return MessageBoxButton.OKCancel;
				case DialogButton.YesNo: return MessageBoxButton.YesNo;
				case DialogButton.YesNoCancel: return MessageBoxButton.YesNoCancel;
			}
			throw new ArgumentOutOfRangeException("button", "Invalid button");
		}

		static MessageBoxImage GetImage(DialogImage image)
		{
			switch (image)
			{
				case DialogImage.Asterisk: return MessageBoxImage.Asterisk;
				case DialogImage.Error: return MessageBoxImage.Error;
				case DialogImage.Exclamation: return MessageBoxImage.Exclamation;
				case DialogImage.Hand: return MessageBoxImage.Hand;
				case DialogImage.Information: return MessageBoxImage.Information;
				case DialogImage.None: return MessageBoxImage.None;
				case DialogImage.Question: return MessageBoxImage.Question;
				case DialogImage.Stop: return MessageBoxImage.Stop;
				case DialogImage.Warning: return MessageBoxImage.Warning;
			}
			throw new ArgumentOutOfRangeException("image", "Invalid image");
		}

		static DialogResponse GetResponse(MessageBoxResult result)
		{
			switch (result)
			{
				case MessageBoxResult.Cancel: return DialogResponse.Cancel;
				case MessageBoxResult.No: return DialogResponse.No;
				case MessageBoxResult.None: return DialogResponse.None;
				case MessageBoxResult.OK: return DialogResponse.OK;
				case MessageBoxResult.Yes: return DialogResponse.Yes;
			}
			throw new ArgumentOutOfRangeException("result", "Invalid result");
		}

		void ActivateWindow(Window window)
		{
			if (window != null)
			{
				if (window.WindowState == WindowState.Minimized) window.WindowState = WindowState.Normal;
				window.Activate();
			}

			if (View is Wms3plUserControl)
			{
				((Wms3plUserControl)View).Activate();
			}
		}

		#region Public Methods

		public DialogResponse ShowException(String message, DialogImage image = DialogImage.Error)
		{
            message = MsgHelper.ConvertNewLine(message);
            if (View != null)
			{
				ActivateWindow(_windowOwner);
			}
			WPFMessageBox.Show(message, "錯誤", MessageBoxButton.OK, GetImage(image));

			return DialogResponse.OK;
		}

		public DialogResponse ShowMessage(String message, String caption, DialogButton button, DialogImage image)
		{
            message = MsgHelper.ConvertNewLine(message);
            var defaultButton = GetDefaultButton(button);
			if (View == null || _windowOwner == null)
			{
				if (_dispatcher.CheckAccess())
					return GetResponse(WPFMessageBox.Show(message, caption, GetButton(button), GetImage(image), defaultButton));
				else
				{
					DialogResponse result = DialogResponse.OK;
					_dispatcher.Invoke((ThreadStart)delegate
																						{
																							MessageBoxResult messageBoxResult = WPFMessageBox.Show(message, caption, GetButton(button),
																																																		 GetImage(image), defaultButton);

																							result = GetResponse(messageBoxResult);
																						});

					return result;
				}
			}
			else
			{
				return ShowMessage(_windowOwner, message, caption, button, image, defaultButton);
			}
		}

		private MessageBoxResult GetDefaultButton(DialogButton button)
		{
			switch (button)
			{
				case DialogButton.OKCancel:
					return MessageBoxResult.OK;
				case DialogButton.YesNo:
					return MessageBoxResult.Yes;
				case DialogButton.YesNoCancel:
					return MessageBoxResult.Yes;
				default:
					return MessageBoxResult.OK;
			}
		}

		public DialogResponse ShowMessage(String message, String caption, DialogButton button, DialogImage image,
																			MessageBoxResult defaultButton)
		{
            message = MsgHelper.ConvertNewLine(message);
            if (View == null || _windowOwner == null)
				return GetResponse(WPFMessageBox.Show(message, caption, GetButton(button), GetImage(image), defaultButton));
			else
			{
				return ShowMessage(_windowOwner, message, caption, button, image, defaultButton);
			}
		}

		public void ShowMessage(String message)
		{
            message = MsgHelper.ConvertNewLine(message);
            _dispatcher.Invoke(new Action(
																														 () =>
																														 {
																															 Window owner = _windowOwner ??
																																							Application.Current.
																																								MainWindow;
																															 if (owner != null)
																															 {
																																 ActivateWindow(owner);
																																 WPFMessageBox.Show(owner, message, "訊息",
																																								 MessageBoxButton.OK,
																																								 MessageBoxImage.Asterisk, MessageBoxResult.OK);
																															 }
																															 else
																															 {
																																 WPFMessageBox.Show(message, "訊息", MessageBoxButton.OK,
																																								 MessageBoxImage.Asterisk, MessageBoxResult.OK);
																															 }
																														 }));
		}

		public DialogResponse ShowMessage(Window window, String message)
		{
            message = MsgHelper.ConvertNewLine(message);
            ActivateWindow(window);
			return GetResponse(WPFMessageBox.Show(window, message, "訊息", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK));
		}

		public DialogResponse ShowMessage(Window owner, String message, String caption, DialogButton button,
																			DialogImage image)
		{
            message = MsgHelper.ConvertNewLine(message);
            Dispatcher dispatcher = owner != null ? owner.Dispatcher : _dispatcher;

			if (dispatcher == null || dispatcher.CheckAccess())
			{
				if (owner != null)
				{
					ActivateWindow(owner);
					return GetResponse(WPFMessageBox.Show(owner, message, caption, GetButton(button), GetImage(image)));
				}
				else
				{
					return GetResponse(WPFMessageBox.Show(message, caption, GetButton(button), GetImage(image)));
				}
			}

			DialogResponse result = DialogResponse.OK;
			dispatcher.Invoke((ThreadStart)delegate
			{
				MessageBoxResult messageBoxResult;

				if (owner != null)
				{
					/* We are on the UI thread, and hence no need to invoke the call.*/
					messageBoxResult = WPFMessageBox.Show(owner, message, caption,
																						 GetButton(button), GetImage(image));
				}
				else
				{
					messageBoxResult = WPFMessageBox.Show(message, caption, GetButton(button),
																						 GetImage(image));
				}
				result = GetResponse(messageBoxResult);
			});

			return result;
		}

		public DialogResponse ShowMessage(Window owner, String message, String caption, DialogButton button,
																			DialogImage image, MessageBoxResult defaultButton)
		{
            message = MsgHelper.ConvertNewLine(message);
            Dispatcher dispatcher = owner != null ? owner.Dispatcher : _dispatcher;

			if (dispatcher == null || dispatcher.CheckAccess())
			{
				if (owner != null)
				{
					ActivateWindow(owner);
					return GetResponse(WPFMessageBox.Show(owner, message, caption, GetButton(button), GetImage(image), defaultButton));
				}
				else
				{
					return GetResponse(WPFMessageBox.Show(message, caption, GetButton(button), GetImage(image), defaultButton));
				}
			}

			DialogResponse result = DialogResponse.OK;
			dispatcher.Invoke((ThreadStart)delegate
			{
				MessageBoxResult messageBoxResult;

				if (owner != null)
				{
					/* We are on the UI thread, and hence no need to invoke the call.*/
					messageBoxResult = WPFMessageBox.Show(owner, message, caption,
																						 GetButton(button), GetImage(image),
																						 defaultButton);
				}
				else
				{
					messageBoxResult = WPFMessageBox.Show(message, caption, GetButton(button),
																						 GetImage(image), defaultButton);
				}
				result = GetResponse(messageBoxResult);
			});

			return result;
		}

		public DialogResponse ShowMessage(String message, String caption, string yesButtonText, string noButtonText, string cancelButtonText,
																			DialogImage image)
		{
			if (View == null || _windowOwner == null)
			{
				return ShowMessage(null, message, caption, yesButtonText, noButtonText, cancelButtonText, image);
			}
			else
			{
				return ShowMessage(_windowOwner, message, caption, yesButtonText, noButtonText, cancelButtonText, image);
			}
		}

		public DialogResponse ShowMessage(Window owner, String message, String caption, string yesButtonText, string noButtonText, string cancelButtonText,
																			DialogImage image)
		{
			message = MsgHelper.ConvertNewLine(message);
			Dispatcher dispatcher = owner != null ? owner.Dispatcher : _dispatcher;

			if (dispatcher == null || dispatcher.CheckAccess())
			{
				if (owner != null)
				{
					ActivateWindow(owner);
					return GetResponse(CustomMessageBox.ShowYesNoCancel(owner, message, caption, yesButtonText, noButtonText, cancelButtonText, GetImage(image)));
				}
				else
				{
					return GetResponse(CustomMessageBox.ShowYesNoCancel(message, caption, yesButtonText, noButtonText, cancelButtonText, GetImage(image)));
				}
			}

			DialogResponse result = DialogResponse.OK;
			dispatcher.Invoke((ThreadStart)delegate
			{
				if (owner != null)
				{
					result = GetResponse(CustomMessageBox.ShowYesNoCancel(owner, message, caption, yesButtonText, noButtonText, cancelButtonText, GetImage(image)));
				}
				else
				{
					result = GetResponse(CustomMessageBox.ShowYesNoCancel(message, caption, yesButtonText, noButtonText, cancelButtonText, GetImage(image)));
				}
			});

			return result;
		}

		#endregion

	}
}
