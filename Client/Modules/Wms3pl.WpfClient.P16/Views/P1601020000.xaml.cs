using MessageBoxUtils;
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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.P16.Report;
using Wms3pl.WpfClient.P16.ViewModel;
using Wms3pl.WpfClient.UcLib.Views;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P16.Views
{
	/// <summary>
	/// P1601020000.xaml 的互動邏輯
	/// </summary>
	public partial class P1601020000 : Wms3plUserControl
	{
		public P1601020000()
		{
			InitializeComponent();
			Vm.DoPrintReport += GetReport;
		}

		private void GetReport(PrintType printType)
		{
			var list1 = Vm.P160102Reports;
			if (list1 == null || list1.Count == 0)
			{
				DialogService.ShowMessage(Properties.Resources.P1601020000xamlcs_NullData);
				return;
			}
			var list2 = Vm.P160102ReportsDT;



			var report = new Report.RP1601020001();
            
            //string gupName = list2.Rows[0][3].ToString();
            //string custName = list2.Rows[0][5].ToString();
            //string AppSTAFF = list2.Rows[0][23].ToString();
            //string AppName = list2.Rows[0][24].ToString();
            report.Load(@"RP1601020001.rpt");
            report.SummaryInfo.ReportTitle = Wms3plSession.Get<GlobalInfo>().GupName + "－" +
                                 Wms3plSession.Get<GlobalInfo>().CustName +
                                 Properties.Resources.P1601020000xamlcs_TransferApplicateNo;
            report.SetDataSource(list2);
			//report.SetText("ReportT", gupName + " - " + custName + Properties.Resources.P1601020000xamlcs_TransferApplicateNo);
			//report.SetText("ApproveUser", AppSTAFF + " - " + AppName);
			report.SetParameterValue("StaffName", Wms3plSession.CurrentUserInfo.AccountName);
			var win = new Wms3plViewer { Owner = Wms3plViewer.GetWindow(this) };
			win.CallReport(report, printType);
		}
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            

            var win = new WinSearchProduct();
            win.GupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
            win.CustCode = Wms3plSession.Get<GlobalInfo>().CustCode;
            win.Owner = this.Parent as Window;
            win.ShowDialog();
            if (win.DialogResult.HasValue && win.DialogResult.Value)
            {
                SetSearchData(win.SelectData);
            }
        }

        private void SetSearchData(F1903 f1903)
        {
            if (f1903 != null)
            {
                Vm.SearchItem_Code = f1903.ITEM_CODE;
                Vm.SearchItem_Name = f1903.ITEM_NAME;
            }
            else
            {
                Vm.SearchItem_Code = string.Empty;
                Vm.SearchItem_Name = string.Empty;
            }
        }
    }
}
