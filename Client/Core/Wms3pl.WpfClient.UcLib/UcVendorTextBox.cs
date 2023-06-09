using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;

namespace Wms3pl.WpfClient.UcLib
{
	public class UcVendorTextBox : SearchTextBox
	{
		public F1908 F1908Data
		{
			get
			{
				return (F1908)GetValue(F1908DataProperty);
			}
			set
			{
				SetValue(F1908DataProperty, value);
			}
		}

		public static readonly DependencyProperty F1908DataProperty =
			DependencyProperty.Register("F1908Data", typeof(F1908),
				typeof(UcVendorTextBox), new PropertyMetadata(default(F1908), F1908DataChangeCallBack));

		private static void F1908DataChangeCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(d))
			{
				var uc = d as UcVendorTextBox;
				if (uc == null)
					return;

				var f1908 = (F1908)e.NewValue;
				if (f1908 == null)
					return;

				uc.SetData(f1908);
			}
		}

		public string VnrName
		{
			get
			{
				return (string)GetValue(VnrNameProperty);
			}
			set
			{
				SetValue(VnrNameProperty, value);
			}
		}
		public static readonly DependencyProperty VnrNameProperty =
			DependencyProperty.Register("VnrName", typeof(string),
				typeof(UcVendorTextBox), new PropertyMetadata(string.Empty));

		public string ItemContact
		{
			get
			{
				return (string)GetValue(ItemContactProperty);
			}
			set
			{
				SetValue(ItemContactProperty, value);
			}
		}
		public static readonly DependencyProperty ItemContactProperty =
			DependencyProperty.Register("ItemContact", typeof(string),
				typeof(UcVendorTextBox), new PropertyMetadata(string.Empty));

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
				typeof(UcVendorTextBox), new PropertyMetadata(string.Empty));

		public string ItemTel
		{
			get
			{
				return (string)GetValue(ItemTelProperty);
			}
			set
			{
				SetValue(ItemTelProperty, value);
			}
		}
		public static readonly DependencyProperty ItemTelProperty =
			DependencyProperty.Register("ItemTel", typeof(string),
				typeof(UcVendorTextBox), new PropertyMetadata(string.Empty));

		public string DeliveryWay
		{
			get
			{
				return (string)GetValue(DeliveryWayProperty);
			}
			set
			{
				SetValue(DeliveryWayProperty, value);
			}
		}
		public static readonly DependencyProperty DeliveryWayProperty =
			DependencyProperty.Register("DeliveryWay", typeof(string),
				typeof(UcVendorTextBox), new PropertyMetadata(string.Empty));

		public override bool SetData(object entity)
		{
			var f1908 = entity as F1908;
			HasResult = (f1908 != null);

			if (HasResult)
			{
				Text = f1908.VNR_CODE;
				VnrName = f1908.VNR_NAME;
				ItemContact = f1908.ITEM_CONTACT;
				Address = f1908.ADDRESS;
				ItemTel = f1908.ITEM_TEL;
				DeliveryWay = f1908.DELIVERY_WAY;
			}
			else
			{
				VnrName = string.Empty;
				ItemContact = string.Empty;
				Address = string.Empty;
				ItemTel = string.Empty;
				DeliveryWay = string.Empty;
			}

			return HasResult;
		}

		public override object GetEntity()
		{
			var proxy = ConfigurationHelper.GetProxy<F19Entities>(false, "VendorTextBox");
			var list = proxy.CreateQuery<F1908>("GetAllowedF1908s")
							.AddQueryExOption("gupCode", GupCode)
							.AddQueryExOption("custCode",CustCode)
							.AddQueryExOption("vnrCode", _preSearchText)
							.ToList();

			return list.FirstOrDefault();
		}
	}
}
