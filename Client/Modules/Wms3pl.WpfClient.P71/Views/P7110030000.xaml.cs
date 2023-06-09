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
using Wms3pl.WpfClient.P71.ViewModel;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P71.Views
{
	/// <summary>
	/// P7110030000.xaml 的互動邏輯
	/// </summary>
	public partial class P7110030000 : Wms3plUserControl
	{
		public P7110030000()
		{
			InitializeComponent();
			Vm.OnFocusAction += FocusAction;
			
		}

		private void FocusAction(OperateMode obj)
		{
			switch (obj)
			{
				case OperateMode.Add:
					SetFocusedElement(txtAddSOUTH_PRIORITY_QTY);
					break;
				case OperateMode.Edit:
					SetFocusedElement(txtEditSOUTH_PRIORITY_QTY);
					break;
			}
		}
	}
}