﻿using System;
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
	/// P1902050000.xaml 的互動邏輯
	/// </summary>
	public partial class P1902050000 : Wms3plUserControl
	{
		public P1902050000()
		{
			InitializeComponent();
			Vm.OnScrollIntoViewSelectedItem += dgF190205.ScrollIntoView;
		}

		//void ScrollIntoViewSelectedItem(object selectedItem)
		//{
		//	base.ScrollIntoView(dgF190205, selectedItem);
		//}

	}
}