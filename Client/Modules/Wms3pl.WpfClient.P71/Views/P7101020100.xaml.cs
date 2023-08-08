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
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;

namespace Wms3pl.WpfClient.P71.Views
{
	/// <summary>
	/// P7101020100.xaml 的互動邏輯
	/// </summary>
	public partial class P7101020100 : Wms3plWindow
	{
		public P7101020100(UseModelType userModelType,string dcCode,string gupCode,string custCode,string warehouseId, F1919Data f1919Data)
		{
			InitializeComponent();
			Vm.View = this;
			Vm.IsAdd = f1919Data == null;
			if(!Vm.Bind(dcCode,gupCode,custCode,warehouseId))
            {
                Vm.IsBindSuccess = false;
                return;
            }
			Vm.ClosedCancelClick += ClosedCancelClick;
			Vm.ClosedSuccessClick += ClosedSuccessClick;
			Vm.DisplayUseModelType = userModelType;
			Vm.AdjustFrom += OpenAdjustFrom;
			if (!Vm.IsAdd)
				Vm.BindEditData(f1919Data);
		}
		private void OpenAdjustFrom()
		{
			var win = new P7101020101(Vm.OldMasterDataList,Vm.OldDetailDataList,Vm.SelectedLoc,Vm.AreaType,Vm.AreaName)
			{
				Owner = System.Windows.Window.GetWindow(this),
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
			};
			win.ShowDialog();
			if (win.DialogResult == true)
			{
				Vm.SelectedLoc = win.EditData;
				Vm.CountMasterLoc(Vm.SelectMasertData,true,true);
				Vm.GetDetailData();
			}
		}

		private void ClosedCancelClick()
		{
			DialogResult = false;
			Close();
		}

		private void ClosedSuccessClick()
		{
			DialogResult = true;
			Close();
		}

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(!Vm.IsBindSuccess)
            {
                ShowMessage("查無此倉別");
                this.Close();
            }
        }
    }
}
