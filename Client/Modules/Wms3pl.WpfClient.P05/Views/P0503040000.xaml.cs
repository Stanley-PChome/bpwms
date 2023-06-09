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
using System.Collections.Generic;
using System.Linq;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P05.Views
{
	/// <summary>
	/// P0503040000.xaml 的互動邏輯
	/// </summary>
	public partial class P0503040000 : Wms3plUserControl
	{
		public P0503040000()
		{
			InitializeComponent();

			Vm.OpenP05030401 = OpenP05030401;

		}

		private void OpenP05030401()
		{
      var win = new P0503040100("0") { Owner = Wms3plViewer.GetWindow(this) };
      win.Vm.DcCode = Vm.SelectedDc;
			win.Vm.CalNo = Vm.CalNo;
			win.Vm.StartLoad();
			Vm.ShowInfoMessage(Properties.Resources.P0503040000_CalculationFinish);
			// 檢查揀位的良品倉庫存是否足夠，不足者，補貨區是否有庫存：
			// 無: nothing，顯示”試算完成“
			// 有: 請問是否產生補貨調撥單? YES: 產生調撥單，顯示”產生補貨調撥單，單號TXXXX、TXXXX” NO: 顯示”試算完成“
			//var datas = win.Vm.OrdItemDatas.Where(a => a.STOCK_QTY_C > 0 && a.TOTAL_ORD_QTY - a.STOCK_QTY_B > 0 ).ToList();
			//if (datas.Any() && Vm.ShowConfirmMessage($"{Properties.Resources.P0503040000_CheckYesNo}{Properties.Resources.P0503040000_AllocationNoB}？") == DialogResponse.Yes)
			//{
			//	// 將不足數改成最多可補多少貨
			//	datas.ForEach(a => a.NOT_ENOUGH = (a.TOTAL_ORD_QTY - a.STOCK_QTY_B > a.STOCK_QTY_C) ? a.STOCK_QTY_C :(a.TOTAL_ORD_QTY - a.STOCK_QTY_B));

			//	var result = win.Vm.P05030401CreateAllocation(datas);
			//	if (result.IsSuccessed)
			//		Vm.ShowWarningMessage($"{Properties.Resources.P0503040000_CalculationFinish}{Environment.NewLine}{Properties.Resources.P0503040000_AllocationNoB}，{Properties.Resources.P0503040000_AllocationNoA}{result.Message}");
			//	else
			//		Vm.ShowWarningMessage($"{Properties.Resources.P0503040000_CalculationFinish}{Environment.NewLine}{result.Message}");
			//}
			//else
			//	Vm.ShowInfoMessage(Properties.Resources.P0503040000_CalculationFinish);

			if (!string.IsNullOrEmpty(win.Vm.CalNo))
			{
				win.ShowDialog();
			}
      //有在試算功能中執行配庫就要重新查詢
      if (win.Vm.IsDoAllocation)
      {
        Vm.SearchCommand.Execute(null);
      }

		}
	}
}
