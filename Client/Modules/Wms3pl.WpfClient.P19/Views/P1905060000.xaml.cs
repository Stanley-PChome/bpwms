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
using Wms3pl.WpfClient.UILib.Services;
using Microsoft.Win32;
using Wms3pl.WpfClient.P19.ViewModel;

namespace Wms3pl.WpfClient.P19.Views
{
	/// <summary>
	/// Wms3plWpfClientUserControl1.xaml 的互動邏輯
	/// </summary>
	public partial class P1905060000 : Wms3plUserControl
	{
		public P1905060000()
		{
			InitializeComponent();
			Vm.GetePassword += () => pbPassword.Password;
			Vm.GetConfirmPassword += () => pbConfirmPassword.Password;
			Vm.ClearPassWord += ClearPassword;
			Vm.UserOperateModeFocus += UserOperateModeFocus;
			SetFocusedElement(txtEmpID);
		}

		private void UserOperateModeFocus(OperateMode obj)
		{
			switch (obj)
			{
				case OperateMode.Edit:
					SetFocusedElement(pbPassword);
					break;
			}
		}

		private void ClearPassword()
		{
			pbPassword.Password = string.Empty;
			pbConfirmPassword.Password = string.Empty;
		}
	}
}
