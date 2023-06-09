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
using Wms3pl.WpfClient.P71.Entities;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.Common;

namespace Wms3pl.WpfClient.P71.Views
{
	/// <summary>
	/// P7101010100.xaml 的互動邏輯
	/// </summary>
	public partial class P7101010100 : Wms3plWindow
	{
		public P7101010100(F1980Data f1980Data, UseModelType userModelType)
		{
			InitializeComponent();
			Vm.View = this;
			Vm.ClosedCancelClick += ClosedCancelClick;
			Vm.ClosedSuccessClick += ClosedSuccessClick;
			Vm.DisplayUseModelType = userModelType;
			Vm.AdjustFrom += OpenAdjustFrom;
			Vm.BindEditData(f1980Data);
		}

		private void OpenAdjustFrom()
		{
			var win = new P7101010101(Vm.OldMasterDataList)
			{
				Owner = System.Windows.Window.GetWindow(this),
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
			};
			win.ShowDialog();
			if (win.DialogResult == true)
			{
				var ns = Vm.OldMasterDataList.Except(win.EditData.ToObservableCollection());
				var notModifyCount = 0;
				var modifyCount = 0;
				foreach (var d in ns)
				{
					Vm.SelectMasertData = d;
					Vm.GetDetailData(d.FloorNo, null);
					var notModifyData = Vm.DetailDataList.Where(o => !o.Item.IsEditData && o.Item.CHANNEL == d.ChannelNo && o.Item.PLAIN == d.PlainNo && o.Item.LOC_LEVEL == d.LocLevelNo).ToList();
					if (notModifyData.Any())
					{
						//沒有不可異動的儲位表示可以刪除
						win.EditData.Add(Vm.OldMasterDataList.FirstOrDefault(o => o.FloorNo == d.FloorNo && o.ChannelNo == d.ChannelNo && o.LocTypeNo == d.LocTypeNo && o.LocLevelNo == d.LocLevelNo && o.PlainNo == d.PlainNo));
						notModifyCount += notModifyData.Count();
					}
					var modifyData = Vm.DetailDataList.Where(o => o.Item.IsEditData && o.Item.CHANNEL == d.ChannelNo && o.Item.PLAIN == d.PlainNo && o.Item.LOC_LEVEL == d.LocLevelNo).ToList();
					foreach (var m in modifyData)
					{
						if (!Vm.ExcludeLoc.Contains(m.Item.LOC_CODE))
						{
							modifyCount++;
							Vm.ExcludeLoc.Add(m.Item.LOC_CODE);
							var oldSelectMaster = win.EditData.Where(o => o.FloorNo == d.FloorNo && o.ChannelNo == d.ChannelNo && o.LocTypeNo == d.LocTypeNo && o.LocLevelNo == d.LocLevelNo && o.PlainNo == d.PlainNo).FirstOrDefault();
							if (oldSelectMaster != null)
								Vm.CountMasterLoc(oldSelectMaster);
						}
					}
				}
				if (Vm.ShowConfirmMessage(string.Format(Properties.Resources.P7101010100xamlcs_ModifyCheckMessage, notModifyCount, modifyCount, Environment.NewLine, notModifyCount + modifyCount)) == UILib.Services.DialogResponse.Yes)
				{
					Vm.OldMasterDataList = win.EditData.OrderBy(o => o.FloorNo).ThenBy(o => o.ChannelNo).ThenBy(o => o.PlainNo).ThenBy(o => o.LocLevelNo).ThenBy(o => o.LocTypeNo).ToObservableCollection();
					Vm.LocCount -= modifyCount;//Vm.OldDetailDataList.RemoveAll(o => o.CHANNEL == n.ChannelNo && o.PLAIN == n.PlainNo && o.LOC_LEVEL == n.LocLevelNo && o.LOC_CODE.Substring(0, 1) == n.FloorNo);

					if (Vm.OldMasterDataList == null || !Vm.OldMasterDataList.Any())
					{
						Vm.MasterDataList = new ObservableCollection<P710101MasterData>();
						Vm.DetailDataList = new SelectionList<ExDataServices.P71WcfService.P710101DetailData>(new List<ExDataServices.P71WcfService.P710101DetailData>());
						return;
					}
					Vm.MasterDataList = Vm.OldMasterDataList.Where(o => o.FloorNo == Vm.SelectedQueryFloor).ToObservableCollection();
				}
				else
				{
					foreach (var d in ns)
					{
						Vm.SelectMasertData = d;
						Vm.GetDetailData(d.FloorNo, null);
						var modifyData = Vm.DetailDataList.Where(o => o.Item.IsEditData && o.Item.CHANNEL == d.ChannelNo && o.Item.PLAIN == d.PlainNo && o.Item.LOC_LEVEL == d.LocLevelNo).ToList();
						foreach (var m in modifyData)
						{
							if (Vm.ExcludeLoc.Contains(m.Item.LOC_CODE))
							{
								Vm.ExcludeLoc.Remove(m.Item.LOC_CODE);
								var oldSelectMaster = Vm.OldMasterDataList.Where(o => o.FloorNo == d.FloorNo && o.ChannelNo == d.ChannelNo && o.LocTypeNo == d.LocTypeNo && o.LocLevelNo == d.LocLevelNo && o.PlainNo == d.PlainNo).FirstOrDefault();
								if (oldSelectMaster != null)
									Vm.CountMasterLoc(oldSelectMaster);
							}
						}
					}
				}
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
	}
}
