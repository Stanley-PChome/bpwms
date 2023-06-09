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
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P19.Views
{
    /// <summary>
    /// P1901100000.xaml 的互動邏輯
    /// </summary>
    public partial class P1901100000 : Wms3plUserControl
    {
        public P1901100000()
        {
            InitializeComponent();
            Vm.ToFirstTab += ToFirstTab;
            Vm.OnUpdateTab += OnUpdateTab;
            Vm.ScrollIntoView += ScrollIntoViewBySearchResult;
        }

        private void OnUpdateTab()
        {
            this.ControlView(() =>
            {
                //tabItemSetting.IsEnabled = true;
            });
        }

        private void ScrollIntoViewBySearchResult()
        {
            ScrollIntoView(dgSearchResult, Vm.SelectedData, Main);
        }

        private void ToFirstTab()
        {
            //SetFocusedElement(Main);
        }

        private void lblSearch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var win = new P1901100000();
            //win.Owner = this.Parent as Window;
            //win.ShowDialog();
            //if (win.DialogResult.HasValue && win.DialogResult.Value)
            //{
            //    Vm.SetData(win.SelectData);
            //}
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Vm.SearchCommand.Execute(null);
            }
        }
    }
}
