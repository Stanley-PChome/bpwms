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
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P71.Views
{
	/// <summary>
	/// P7109030000.xaml 的互動邏輯
	/// </summary>
	public partial class P7109030000 : Wms3plUserControl
	{
		public Action OnSaved = delegate { };

		public P7109030000()
		{
			InitializeComponent();
			Vm.ToFirstTab += ToFirstTab;
            Vm.OnUpdateTab += OnUpdateTab;
			Vm.OnSaved += Saved;
            Vm.OnDataGridScrollIntoView += DataGridScrollIntoView;
		}

        private void OnUpdateTab()
        {
            this.ControlView(() =>
            {
                tabItemSetting.IsEnabled = true;
            });
        }

		public F1909 SelectedF1909
		{
			get { return Vm.SelectedData; }
		}

		private void Saved()
		{
			if (!string.IsNullOrWhiteSpace(Vm.NewUniForm))
			{
				OnSaved();
			}
		}

		public void SetNewUniForm(string uniForm)
		{
			Vm.NewUniForm = uniForm;
		}

		private void ToFirstTab()
		{
			SetFocusedElement(Main);
            
            
		}
        private void DataGridScrollIntoView()
        {
            var dg = dgSearchResult;

            ScrollIntoView(dgSearchResult, Vm.SelectedData);
            //this.ControlView(() =>
            //    {
            //        dg.Focus();
            //        dg.SelectedItem = Vm.SelectedData;
            //        if (dg.SelectedItem != null)
            //            dg.ScrollIntoView(dg.SelectedItem);
            //    });
        }

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Vm.CheckLoad();
		}
	}
}
