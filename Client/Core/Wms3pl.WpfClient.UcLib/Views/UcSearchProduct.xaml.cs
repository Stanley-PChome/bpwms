using MessageBoxUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.DataServices.F25DataService;
using Wms3pl.WpfClient.UcLib.ViewModel;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.UcLib.Views
{
	/// <summary>
	/// UcSearchProduct.xaml 的互動邏輯
	/// </summary>
	public partial class UcSearchProduct : UserControl
	{
		public UcSearchProduct()
		{
			InitializeComponent();
		}

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
				typeof(UcSearchProduct), new PropertyMetadata(string.Empty));

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
				typeof(UcSearchProduct), new PropertyMetadata(string.Empty));


		public string ItemCode
		{
			get
			{
				return (string)GetValue(ItemCodeProperty);
			}
			set
			{
				SetValue(ItemCodeProperty, value);
			}
		}
		public string SerialNo
		{
			get
			{
				return (string)GetValue(SerialNoProperty);
			}
			set
			{
				SetValue(SerialNoProperty, value);
			}
		}

		private static void ItemCodeChangeCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(d))
			{
				var uc = d as UcSearchProduct;
				if (uc != null)
				{
					string itemCode = (string)e.NewValue;
					if (string.IsNullOrWhiteSpace(itemCode) && !string.IsNullOrWhiteSpace(uc._preSearchItemCode))
						uc.GetItemData();
				}
			}
		}

		private static void SerialNoChangeCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(d))
			{
				var uc = d as UcSearchProduct;
				if (uc != null)
				{
					string serialNo = (string)e.NewValue;
					if (string.IsNullOrWhiteSpace(serialNo) && !string.IsNullOrWhiteSpace(uc._preSearchSerialNo))
						uc.GetItemDataFromSerialNo();
				}
			}
		}

		public static readonly DependencyProperty ItemCodeProperty =
			DependencyProperty.Register("ItemCode", typeof(string),
				typeof(UcSearchProduct), new PropertyMetadata("", ItemCodeChangeCallBack));
		public static readonly DependencyProperty SerialNoProperty =
			DependencyProperty.Register("SerialNo", typeof(string),
				typeof(UcSearchProduct), new PropertyMetadata("", SerialNoChangeCallBack));

		public string ItemName
		{
			get
			{
				return (string)GetValue(ItemNameProperty);
			}
			set
			{
				SetValue(ItemNameProperty, value);
			}
		}
		public static readonly DependencyProperty ItemNameProperty =
			DependencyProperty.Register("ItemName", typeof(string),
				typeof(UcSearchProduct), new PropertyMetadata(""));

		public string ItemSize
		{
			get
			{
				return (string)GetValue(ItemSizeProperty);
			}
			set
			{
				SetValue(ItemSizeProperty, value);
			}
		}
		public static readonly DependencyProperty ItemSizeProperty =
			DependencyProperty.Register("ItemSize", typeof(string),
				typeof(UcSearchProduct), new PropertyMetadata(""));

		public string ItemSpec
		{
			get
			{
				return (string)GetValue(ItemSpecProperty);
			}
			set
			{
				SetValue(ItemSpecProperty, value);
			}
		}
		public static readonly DependencyProperty ItemSpecProperty =
			DependencyProperty.Register("ItemSpec", typeof(string),
				typeof(UcSearchProduct), new PropertyMetadata(""));

		public string ItemColor
		{
			get
			{
				return (string)GetValue(ItemColorProperty);
			}
			set
			{
				SetValue(ItemColorProperty, value);
			}
		}
		public static readonly DependencyProperty ItemColorProperty =
			DependencyProperty.Register("ItemColor", typeof(string),
				typeof(UcSearchProduct), new PropertyMetadata(""));

		public bool HasItem
		{
			get
			{
				return (bool)GetValue(HasItemProperty);
			}
			set
			{
				SetValue(HasItemProperty, value);
			}
		}
		public static readonly DependencyProperty HasItemProperty =
			DependencyProperty.Register("HasItem", typeof(bool),
				typeof(UcSearchProduct), new PropertyMetadata(false));

		public string CustItemCode
		{
			get
			{
				return (string)GetValue(CustItemCodeProperty);
			}
			set
			{
				SetValue(CustItemCodeProperty, value);
			}
		}
		public static readonly DependencyProperty CustItemCodeProperty =
			DependencyProperty.Register("CustItemCode", typeof(string),
				typeof(UcSearchProduct), new PropertyMetadata(""));

		public string EanCode1
		{
			get
			{
				return (string)GetValue(EanCode1Property);
			}
			set
			{
				SetValue(EanCode1Property, value);
			}
		}
		public static readonly DependencyProperty EanCode1Property =
			DependencyProperty.Register("EanCode1", typeof(string),
				typeof(UcSearchProduct), new PropertyMetadata(""));

		public string EanCode2
		{
			get
			{
				return (string)GetValue(EanCode2Property);
			}
			set
			{
				SetValue(EanCode2Property, value);
			}
		}
		public static readonly DependencyProperty EanCode2Property =
			DependencyProperty.Register("EanCode2", typeof(string),
				typeof(UcSearchProduct), new PropertyMetadata(""));

		public string EanCode3
		{
			get
			{
				return (string)GetValue(EanCode3Property);
			}
			set
			{
				SetValue(EanCode3Property, value);
			}
		}
		public static readonly DependencyProperty EanCode3Property =
			DependencyProperty.Register("EanCode3", typeof(string),
				typeof(UcSearchProduct), new PropertyMetadata(""));
		#region Visibility DependencyProperty


		public Visibility ItemNameVisibility
		{
			get
			{
				return (Visibility)GetValue(ItemNameVisibilityProperty);
			}
			set
			{
				SetValue(ItemNameVisibilityProperty, value);
			}
		}
		public static readonly DependencyProperty ItemNameVisibilityProperty =
			DependencyProperty.Register("ItemNameVisibility", typeof(Visibility),
				typeof(UcSearchProduct), new PropertyMetadata(Visibility.Visible));

		public Visibility ItemSizeVisibility
		{
			get
			{
				return (Visibility)GetValue(ItemSizeVisibilityProperty);
			}
			set
			{
				SetValue(ItemSizeVisibilityProperty, value);
			}
		}
		public static readonly DependencyProperty ItemSizeVisibilityProperty =
			DependencyProperty.Register("ItemSizeVisibility", typeof(Visibility),
				typeof(UcSearchProduct), new PropertyMetadata(Visibility.Visible));

		public Visibility ItemSpecVisibility
		{
			get
			{
				return (Visibility)GetValue(ItemSpecVisibilityProperty);
			}
			set
			{
				SetValue(ItemSpecVisibilityProperty, value);
			}
		}
		public static readonly DependencyProperty ItemSpecVisibilityProperty =
			DependencyProperty.Register("ItemSpecVisibility", typeof(Visibility),
				typeof(UcSearchProduct), new PropertyMetadata(Visibility.Visible));

		public Visibility ItemColorVisibility
		{
			get
			{
				return (Visibility)GetValue(ItemColorVisibilityProperty);
			}
			set
			{
				SetValue(ItemColorVisibilityProperty, value);
			}
		}
		public static readonly DependencyProperty ItemColorVisibilityProperty =
			DependencyProperty.Register("ItemColorVisibility", typeof(Visibility),
				typeof(UcSearchProduct), new PropertyMetadata(Visibility.Visible));
		#endregion

		#region Event

		public event RoutedEventHandler BeforeItemChanged;
		protected void OnBeforeItemChanged()
		{
			if (BeforeItemChanged != null)
				BeforeItemChanged(this, new RoutedEventArgs());
		}

		public event RoutedEventHandler AfterItemChanged;
		protected void OnAfterItemChanged()
		{
			if (AfterItemChanged != null)
				AfterItemChanged(this, new RoutedEventArgs());
		}
		#endregion

		#region 避免重複查詢的屬性
		private string _preSearchGupCode;
		private string _preSearchCustCode;
		private string _preSearchItemCode;
		private string _preSearchSerialNo;
		#endregion

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(GupCode))
				GupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			if (string.IsNullOrEmpty(CustCode))
				CustCode = Wms3plSession.Get<GlobalInfo>().CustCode;

			var win = new WinSearchProduct();
			win.GupCode = GupCode;
			win.CustCode = CustCode;
			win.Owner = this.Parent as Window;
			win.ShowDialog();
			if (win.DialogResult.HasValue && win.DialogResult.Value)
			{
				SetData(win.SelectData);
			}
		}

		private void TxtItemCode_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				GetItemData();
			}
		}

		private void TxtSerialNo_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				GetItemDataFromSerialNo();
			}
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
		}

		public void GetItemData()
		{
			// 業主代碼可能從外部帶，若為 string.Empty 則只判斷品項代碼是否相同即可
			//if (_preSearchItemCode == ItemCode && _preSearchGupCode == GupCode && _preSearchCustCode == CustCode && _preSearchSerialNo == SerialNo)
			//	return;

			if (string.IsNullOrEmpty(GupCode))
				GupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			if (string.IsNullOrEmpty(CustCode))
				CustCode = Wms3plSession.Get<GlobalInfo>().CustCode;

			//_preSearchItemCode = ItemCode;
			//_preSearchGupCode = GupCode;
			//_preSearchCustCode = CustCode;

			OnBeforeItemChanged();

			if (string.IsNullOrWhiteSpace(ItemCode))
			{
				// 當沒有輸入要搜尋的品項編號時，就不用在去搜尋
				SetData(null, ignoreMsgBox: true);
				return;
			}

			var proxy = ConfigurationHelper.GetProxy<F19Entities>(false, "SearchProduct");
			if (proxy.F1903s.Where(a => (a.ITEM_CODE == ItemCode || a.EAN_CODE1 == ItemCode || a.EAN_CODE2 == ItemCode || a.EAN_CODE3 == ItemCode) && a.GUP_CODE == GupCode && a.CUST_CODE == CustCode && a.SND_TYPE == "0").Count() == 0)
			{
				SetData(null);
				OnAfterItemChanged();
				return;
			}

			var viewModel = this.DataContext as UcSearchProduct_ViewModel;
			var itemCodeList = viewModel.FindItems(GupCode, CustCode, ItemCode);

			F1903 q = null;
			//var findItemData = Vm.FindItems(GupCode, CustCode, ItemCode);
			if (itemCodeList.Count() > 1) //輸入為國際條碼
			{
				WinSearchProduct winSearchProduct = new WinSearchProduct(true);
				var globalInfo = Wms3plSession.Get<GlobalInfo>();
				winSearchProduct.GupCode = globalInfo.GupCode;
				winSearchProduct.CustCode = globalInfo.CustCode;
				winSearchProduct.IsItemCodeChecked = true;
				winSearchProduct.SearchEanCode = ItemCode;
				winSearchProduct.DoSearchEanCode();
				winSearchProduct.ShowDialog();

				if (winSearchProduct.SelectData != null)
				{
					q = winSearchProduct.SelectData;
				}
			}
			else
			{
				ItemCode = itemCodeList.FirstOrDefault();
				q = proxy.F1903s.Where(x => x.ITEM_CODE == ItemCode &&
											 x.GUP_CODE == GupCode &&
											 x.CUST_CODE == CustCode).FirstOrDefault();
			}
			
			SerialNo = null;
			SetData(q);

			OnAfterItemChanged();
		}

		private void GetItemDataFromSerialNo(bool? isClickButton = null)
		{
			if (string.IsNullOrEmpty(GupCode))
				GupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
			if (string.IsNullOrEmpty(CustCode))
				CustCode = Wms3plSession.Get<GlobalInfo>().CustCode;

			OnBeforeItemChanged();

			if (string.IsNullOrWhiteSpace(SerialNo))
			{
				// 當沒有輸入要搜尋的序號時，就不用在去搜尋
				// SetData(null, ignoreMsgBox: true);
				return;
			}

			if (isClickButton.HasValue)
			{
				FindF1903();
			}
			else
			{
				if (string.IsNullOrWhiteSpace(ItemCode))
				{
					FindF1903();
				}
			}

		}

		public void FindF1903()
		{
			SerialNo = SerialNo.ToUpper();
			var f25Proxy = ConfigurationHelper.GetProxy<F25Entities>(false, "SearchProduct");
			var f19Proxy = ConfigurationHelper.GetProxy<F19Entities>(false, "SearchProduct");
			var f2501 = f25Proxy.F2501s.Where(x => x.GUP_CODE == GupCode && x.CUST_CODE == CustCode && x.SERIAL_NO == SerialNo).FirstOrDefault();
			if (f2501 == null)
			{
				SetData(null);
				OnAfterItemChanged();
				return;
			}

			var q = f19Proxy.F1903s.Where(x => x.ITEM_CODE == f2501.ITEM_CODE &&
											 x.GUP_CODE == GupCode &&
											 x.CUST_CODE == CustCode);
			var qry = q.FirstOrDefault();
			SetData(qry);

			OnAfterItemChanged();
		}

		private void SetData(F1903 f1903, bool ignoreMsgBox = false)
		{
			if (f1903 != null)
			{
				ItemCode = f1903.ITEM_CODE;
				ItemName = f1903.ITEM_NAME;
				ItemColor = f1903.ITEM_COLOR;
				ItemSize = f1903.ITEM_SIZE;
				ItemSpec = f1903.ITEM_SPEC;
				HasItem = true;
				CustItemCode = f1903.CUST_ITEM_CODE;
				EanCode1 = f1903.EAN_CODE1;
				EanCode2 = f1903.EAN_CODE2;
				EanCode3 = f1903.EAN_CODE3;
			}
			else
			{
				ItemName = string.Empty;
				ItemColor = string.Empty;
				ItemSize = string.Empty;
				ItemSpec = string.Empty;
				ItemCode = string.Empty;
				SerialNo = string.Empty;
				HasItem = false;
				CustItemCode = string.Empty;
				EanCode1 = string.Empty;
				EanCode2 = string.Empty;
				EanCode3 = string.Empty;
				if (!ignoreMsgBox)
				{
					var msg = Messages.InfoNoData;
					WPFMessageBox.Show(msg.Message, msg.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}

		private void TxtItemCode_LostFocus(object sender, RoutedEventArgs e)
		{
			GetItemData();
		}
		private void TxtSerialNo_LostFocus(object sender, RoutedEventArgs e)
		{
			GetItemDataFromSerialNo();
		}

		private void TxtSerialNo_ButtonClick(object sender, RoutedEventArgs e)
		{
			GetItemDataFromSerialNo(true);
		}
	}


}
