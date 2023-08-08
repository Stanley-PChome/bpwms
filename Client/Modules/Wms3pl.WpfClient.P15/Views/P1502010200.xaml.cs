using System;
using System.Collections.Generic;
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
using Wms3pl.WpfClient.DataServices.F15DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices.P15ExDataService;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P15.Views
{
	/// <summary>
	/// P1502010200.xaml 的互動邏輯
	/// </summary>
	public partial class P1502010200 : Wms3plWindow
	{
		public P1502010200(F151001 sourceData,List<F151001DetailDatas> detailDatas)
		{
			InitializeComponent();
			Vm.SourceData = sourceData;
			Vm.ReturnData = detailDatas;
			IsExpend();
			Vm.Closed += Close;
		}

		public F151001 SourceData { get; set; }
		public List<F151002> ReturnData { get; set; }

		public string RunStatus="";

		private void CancelCommand_Click(object sender, RoutedEventArgs e)
		{
			var dr = DialogService.ShowMessage(Properties.Resources.P1502010100xamlcs_CancelEdit, Properties.Resources.P1502010000xamlcs_Information, UILib.Services.DialogButton.YesNo, UILib.Services.DialogImage.Warning);
			if (dr == UILib.Services.DialogResponse.Yes)
				Window.Close();
		}

		private void IsExpend()
		{
			//if (Vm.SourceData != null)
			//{
			//	var validCol = DgItemList.Columns.Where(x => x.Header != null && x.Header.ToString() == Properties.Resources.VALID_DATE).FirstOrDefault();
			//	if (validCol != null)
			//		validCol.Visibility = (Vm.SourceData.ISEXPENDDATE == "1") ? Visibility.Visible : Visibility.Collapsed;
			//	var enterCol = DgItemList.Columns.Where(x => x.Header != null && x.Header.ToString() == Properties.Resources.ENTER_DATE).FirstOrDefault();
			//	if (enterCol != null)
			//		enterCol.Visibility = (Vm.SourceData.ISEXPENDDATE == "1") ? Visibility.Visible : Visibility.Collapsed;
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
                F1903 f1903 = winSearchProduct.SelectData;
                Vm.AppendItemCode(f1903);
            }
        }
    }
}
