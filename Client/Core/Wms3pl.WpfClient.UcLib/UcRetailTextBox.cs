using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;

namespace Wms3pl.WpfClient.UcLib
{
	public class UcRetailTextBox : SearchTextBox
	{
		public string RetailName
		{
			get
			{
				return (string)GetValue(RetailNameProperty);
			}
			set
			{
				SetValue(RetailNameProperty, value);
			}
		}
		public static readonly DependencyProperty RetailNameProperty =
			DependencyProperty.Register("RetailName", typeof(string),
				typeof(UcRetailTextBox), new PropertyMetadata(string.Empty));

		public string Contact
		{
			get
			{
				return (string)GetValue(ContactProperty);
			}
			set
			{
				SetValue(ContactProperty, value);
			}
		}
		public static readonly DependencyProperty ContactProperty =
			DependencyProperty.Register("Contact", typeof(string),
				typeof(UcRetailTextBox), new PropertyMetadata(string.Empty));

		public string Address
		{
			get
			{
				return (string)GetValue(AddressProperty);
			}
			set
			{
				SetValue(AddressProperty, value);
			}
		}
		public static readonly DependencyProperty AddressProperty =
			DependencyProperty.Register("Address", typeof(string),
				typeof(UcRetailTextBox), new PropertyMetadata(string.Empty));

		public string Tel
		{
			get
			{
				return (string)GetValue(TelProperty);
			}
			set
			{
				SetValue(TelProperty, value);
			}
		}
		public static readonly DependencyProperty TelProperty =
			DependencyProperty.Register("Tel", typeof(string),
				typeof(UcRetailTextBox), new PropertyMetadata(string.Empty));

		public override bool SetData(object entity)
		{
			var f1910 = entity as F1910;
			HasResult = (f1910 != null);

			if (HasResult)
			{
				Text = f1910.RETAIL_CODE;
				RetailName = f1910.RETAIL_NAME;
				Contact = f1910.CONTACT;
				Address = f1910.ADDRESS;
				Tel = f1910.TEL;
			}
			else
			{
				RetailName = string.Empty;
				Contact = string.Empty;
				Address = string.Empty;
				Tel = string.Empty;
			}

			return HasResult;
		}

		public override object GetEntity()
		{
			var proxy = ConfigurationHelper.GetProxy<F19Entities>(false, "UcRetailTextBox");
			var f1909 = proxy.F1909s.Where(x => x.GUP_CODE == _preSearchGupCode && x.CUST_CODE == _preSearchCustCode).FirstOrDefault();
			var custCode = _preSearchCustCode;
			if (f1909 != null && f1909.ALLOWGUP_RETAILSHARE == "1")
				custCode = "0";
			var query = from a in proxy.F1910s
						where a.GUP_CODE == _preSearchGupCode
						&& a.RETAIL_CODE == _preSearchText
						&& a.CUST_CODE == custCode
						select a;

			var result = query.FirstOrDefault();
			return result;
		}
	}
}
