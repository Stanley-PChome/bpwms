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

namespace Wms3pl.WpfClient.P08.Views
{
	/// <summary>
	/// P0806130000.xaml 的互動邏輯
	/// </summary>
	public partial class P0806130000 : Wms3plWindow
	{
		public P0806130000()
		{
			InitializeComponent();
			Vm.DGBtnSwitch = (t) => DGBtnSwitch(t);
			Vm.OnSearchEmpIDComplete = () => FocusAfterTxtEmpID();
			Vm.OnSearchOrderNoComplete = () => FocusAfterTxtOrderNO();

		}

		/// <summary>
		/// 刪除按鈕的控制切換
		/// </summary>
		/// <param name="status"></param>
		private void DGBtnSwitch(bool status)
		{
			if (status)
			{
				DG1.Columns[6].Visibility = Visibility.Visible;
				DispatcherAction(() =>
				{
					SetFocusedElement(TxtEmpIDSearch,true);
				});
			}
			else
			{
				DG1.Columns[6].Visibility = Visibility.Collapsed;
				DispatcherAction(() =>
				{
					SetFocusedElement(TxtEmpID, true);
				});
			}


		}

		/// <summary>
		/// 工號欄位的Enter觸發
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TxtEmpID_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter || e.Key == Key.Tab)
			{
				Vm.SetEmpIDInfo();
			}
			else
			{
				return;
			}
		}

		/// <summary>
		/// 揀貨單號欄位的Enter觸發
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TxtOrderNO_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			Vm.SetOrderNOInfo();
		}



		/// <summary>
		/// 刪除按鈕的該筆資料抓取
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GDBtnDeleteAction(object sender, RoutedEventArgs e)
		{
			Vm.DoDelete((Button)sender);
		}

		/// <summary>
		/// FocuseSelectAll工號欄位
		/// </summary>
		private void FocusAfterTxtEmpID()
		{
			SetFocusedElement(TxtEmpID);
			TxtEmpID.SelectAll();
		}

		/// <summary>
		///  FocuseSelectAll揀貨單號欄位
		/// </summary>
		private void FocusAfterTxtOrderNO()
		{
			SetFocusedElement(TxtOrderNO);
			TxtOrderNO.SelectAll();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TxtEmpIDSearch_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			Vm.DoSerach();
			SetFocusedElement(TxtEmpIDSearch);
			TxtEmpIDSearch.SelectAll();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TxtOrderNOSearch_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter)
				return;
			Vm.DoSerach();
			SetFocusedElement(TxtOrderNOSearch);
			TxtOrderNOSearch.SelectAll();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Vm.DoEmpBind();
			DGBtnSwitch(false);
			SetFocusedElement(TxtEmpID, true);
		}
	}
}
