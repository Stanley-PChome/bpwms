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

namespace Wms3pl.WpfClient.P91.Views
{
	/// <summary>
	/// P9103050000.xaml 的互動邏輯
	/// </summary>
	public partial class P9103050000 : Wms3plUserControl
	{
		public P9103050000()
		{
			InitializeComponent();
			Vm.OnFocusAction += FocusAction;
		}

		private void FocusAction(OperateMode obj)
		{
			switch (obj)
			{
				case OperateMode.Add:
					SetFocusedElement(txtPRODUCE_NO);
					break;
				case OperateMode.Edit:
					SetFocusedElement(txtPRODUCE_NAME);
					break;
			}
		}
	}
}
