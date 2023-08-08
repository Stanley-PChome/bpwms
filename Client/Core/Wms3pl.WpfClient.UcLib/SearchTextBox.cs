using MessageBoxUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.UcLib
{
	public abstract class SearchTextBox : TextBox
	{
		#region Event
		/// <summary>
		/// 查詢結果改變前
		/// </summary>
		public event RoutedEventHandler BeforeResultChanged;
		protected void OnBeforeResultChanged()
		{
			if (BeforeResultChanged != null)
				BeforeResultChanged(this, new RoutedEventArgs());
		}

		/// <summary>
		/// 查詢結果改變後
		/// </summary>
		public event RoutedEventHandler AfterResultChanged;
		protected void OnAfterResultChanged()
		{
			if (AfterResultChanged != null)
				AfterResultChanged(this, new RoutedEventArgs());
		}
		#endregion

		public string GupCode
		{
			get
			{
				return (string)GetValue(GupCodeProperty);
			}
			set
			{
				SetValue(GupCodeProperty, value);
			}
		}

		public static readonly DependencyProperty GupCodeProperty =
			DependencyProperty.Register("GupCode", typeof(string),
				typeof(SearchTextBox), new PropertyMetadata(string.Empty));

		public string CustCode
		{
			get
			{
				return (string)GetValue(CustCodeProperty);
			}
			set
			{
				SetValue(CustCodeProperty, value);
			}
		}

		public static readonly DependencyProperty CustCodeProperty =
			DependencyProperty.Register("CustCode", typeof(string),
				typeof(SearchTextBox), new PropertyMetadata(string.Empty));


		public bool HasResult
		{
			get
			{
				return (bool)GetValue(HasResultProperty);
			}
			set
			{
				SetValue(HasResultProperty, value);
			}
		}

		public static readonly DependencyProperty HasResultProperty =
			DependencyProperty.Register("HasResult", typeof(bool),
				typeof(SearchTextBox), new PropertyMetadata(false));

		#region 避免重複查詢的屬性
		protected string _preSearchGupCode;
		protected string _preSearchCustCode;
		protected string _preSearchText;
		#endregion

		public void SearchTextByChanged()
		{
			// 查過就不在重複查詢
			if (_preSearchText == Text && _preSearchGupCode == GupCode && _preSearchCustCode == CustCode)
				return;


			if (Text != null)
				Text = Text.Trim();

			SearchResultCode(Text);
		}

		public void SearchResultCode(string text)
		{
			if (string.IsNullOrEmpty(GupCode))
				GupCode = Wms3plSession.Get<GlobalInfo>().GupCode;

			if (string.IsNullOrEmpty(CustCode))
				CustCode = Wms3plSession.Get<GlobalInfo>().CustCode;

			_preSearchText = text;
			_preSearchGupCode = GupCode;
			_preSearchCustCode = CustCode;

			OnBeforeResultChanged();

			if (string.IsNullOrEmpty(_preSearchText))
			{
				// Clear Data
				SetData(null);
				OnAfterResultChanged();
				return;
			}

			if (!SetData(GetEntity()))
			{
				var msg = Messages.InfoNoData;
				WPFMessageBox.Show(msg.Message, msg.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
			}

			OnAfterResultChanged();
		}


		public abstract bool SetData(object entity);

		public abstract object GetEntity();

		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			// 當沒有輸入 Text 時，清除資料
			if (string.IsNullOrWhiteSpace(Text) && !string.IsNullOrWhiteSpace(_preSearchText))
			{
				SearchTextByChanged();
			}

			base.OnTextChanged(e);
		}

		protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				SearchTextByChanged();

			}
			base.OnKeyDown(e);
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			SearchTextByChanged();

			base.OnLostFocus(e);
		}

	}
}
