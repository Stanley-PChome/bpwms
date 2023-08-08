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

namespace Wms3pl.WpfClient.P19.Views
{
	/// <summary>
	/// P1901280000.xaml 的互動邏輯
	/// </summary>
	public partial class P1901280000 : Wms3plUserControl
	{
		public Action OnSaved = delegate { };

		public P1901280000()
		{
			InitializeComponent();
			Vm.OnFocusAction += FocusAction;
			Vm.OnSaved += Saved;
			SetFocusedElement(txtSearchOutsourceName);
		}

		public F1928 SelectedF1928
		{
			get { return Vm.SelectedF1928; }
		}

		private void Saved()
		{
			if (!string.IsNullOrWhiteSpace(Vm.NewUniForm))
			{
				OnSaved();
			}
		}

		private void FocusAction(OperateMode obj)
		{
			switch (obj)
			{
				case OperateMode.Add:
					SetFocusedElement(txtOUTSOURCE_ID);
					break;

				case OperateMode.Edit:
					SetFocusedElement(txtOUTSOURCE_NAME);
					break;
			}
		}

		public void SetNewUniForm(string uniForm)
		{
			Vm.NewUniForm = uniForm;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Vm.CheckLoad();
		}
	}
}
