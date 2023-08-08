using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices.P06ExDataService;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P06.Views
{
	/// <summary>
	/// P0601030000.xaml 的互動邏輯
	/// </summary>
	public partial class P0601030000 : Wms3plUserControl
	{
		public P0601030000()
		{
			InitializeComponent();
			SetFocusedElement(txtCustOrdNo);
		}

		public void OnChecked(object sender, RoutedEventArgs e)
		{
			
			Vm.GetContainerBarcode();
			//if (SearchResults != null)
			//{
			//	var proxyWcf = new wcf.P06WcfServiceClient();
			//	//var proxy = GetExProxy<P06ExDataSource>();
			//	//var proxy = GetProxy<F07Entities>();
			//	var selectedOrdNoList = (from selectionItem in SearchResults
			//							 where selectionItem.IsSelected
			//							 select selectionItem.Item.ORD_NO).ToArray();
			//	if (selectedOrdNoList.Any())
			//	{
			//		//var proxy = new wcf.P06WcfServiceClient();
			//		var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel,
			//												() => proxyWcf.GetContainerBarcode(SearchDcCode, _gupCode, _custCode, selectedOrdNoList));
			//		//var query = proxy.CreateQuery<List<string>>("GetContainerBarcode")
			//		//	.AddQueryExOption("dcCode", SearchDcCode)
			//		//	.AddQueryExOption("gupCode", _gupCode)
			//		//	.AddQueryExOption("custCode", _custCode)
			//		//	.AddQueryExOption("ordNos", OrdNos);
			//		if (result.IsSuccessed == false)
			//		{
			//			DialogService.ShowMessage(result.Message);
			//		}
			//		else
			//		{
			//			ContainerBarcodeResult = result.No.Split(',').ToList();
			//		}
			//	}
			//}
		}

		private void btnSerachProduct_Click(object sender, RoutedEventArgs e)
		{
			WinSearchProduct winSearchProduct = new WinSearchProduct();
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			winSearchProduct.GupCode = globalInfo.GupCode;
			winSearchProduct.CustCode = globalInfo.CustCode;
			winSearchProduct.ShowDialog();

			if (winSearchProduct.SelectData != null)
			{
				Vm.SearchItemCode = winSearchProduct.SelectData.ITEM_CODE;
			}
		}
	}
}
