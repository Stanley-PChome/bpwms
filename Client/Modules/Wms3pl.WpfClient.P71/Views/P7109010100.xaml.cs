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
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P71.Views
{
    /// <summary>
    /// P7109010100.xaml 的互動邏輯
    /// </summary>
    public partial class P7109010100 : Wms3plWindow
    {
        /// <summary>
        /// 配送商主檔查詢
        /// </summary>
        public P7109010100()
        {
            InitializeComponent();
            Vm.DoExit += DoExitWin;
        }

        private void DoExitWin()
        {
            this.DialogResult = true;
            this.Close();
        }

        public F1947Ex SelectedData { get { return Vm.SelectedItem; } }

        private void ExitCommand_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
