using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.ExDataServices.P91ExDataService;
using System.Collections.ObjectModel;
using wcf = Wms3pl.WpfClient.ExDataServices.P91WcfService;

namespace Wms3pl.WpfClient.P91.ViewModel
{
	public partial class P9101010200_ViewModel : InputViewModelBase
	{
		public P9101010200_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
			}

		}

		#region 資料連結
		private F910201 _baseData = null;
		public F910201 BaseData
		{
			get { return _baseData; }
			set { _baseData = value; RaisePropertyChanged("BaseData"); }
		}

		private ObservableCollection<SelectionItem<F910004Data>> _data = new ObservableCollection<SelectionItem<F910004Data>>();
		public ObservableCollection<SelectionItem<F910004Data>> Data
		{
			get { return _data; }
			set { _data = value; RaisePropertyChanged("Data"); }
		}
		#endregion

		#region Command
		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch()
					);
			}
		}

		private void DoSearch()
		{
			//執行查詢動作
			var proxy = GetExProxy<P91ExDataSource>();
			var result = proxy.CreateQuery<F910004Data>("GetF910004Data")
				.AddQueryOption("dcCode", string.Format("'{0}'", BaseData.DC_CODE))
				.Select(x => new SelectionItem<F910004Data>(x, false)).ToList();

			// 取出已設定的項目, 要Bind到IsSelectedOld
			var proxy2 = GetProxy<F91Entities>();
			var f910203 = proxy2.F910203s.Where(x => x.DC_CODE == BaseData.DC_CODE && x.GUP_CODE == BaseData.GUP_CODE && x.CUST_CODE == BaseData.CUST_CODE && x.PROCESS_NO == BaseData.PROCESS_NO).ToList();
			foreach (var p in f910203)
			{
				var tmp = result.FirstOrDefault(x => x.Item.PRODUCE_NO == p.PRODUCE_NO);
				if (tmp != null)
				{
					tmp.IsSelected = true;
					tmp.IsSelectedOld = true;
				}
			}
			Data = result.ToObservableCollection();
		}
		#endregion Search

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),
					() => BaseData.STATUS == "0" && Data.Any(x => x.IsSelected != x.IsSelectedOld)
				);
			}
		}

		private void DoSave()
		{
			var f910201 = FindByKey<F910201>(BaseData);
			if (f910201.STATUS != "0")
			{
				ShowWarningMessage(Properties.Resources.P9101010200_ViewModel_CantSelectProductLine);
				BaseData = f910201;
				return;
			}

			var newItems = Data.Where(x => x.IsSelected == true && x.IsSelectedOld == false)
				.Select(x => AutoMapper.Mapper.DynamicMap<wcf.F910004Data>(x.Item)).ToList();
			var removeItems = Data.Where(x => x.IsSelectedOld == true && x.IsSelected == false)
				.Select(x => AutoMapper.Mapper.DynamicMap<wcf.F910004Data>(x.Item)).ToList();
			// 丟到WcfService去做資料儲存的動作
			var proxyWcf = new wcf.P91WcfServiceClient();
			var result = RunWcfMethod<wcf.ExecuteResult>(proxyWcf.InnerChannel, () => proxyWcf.UpdateF910203(BaseData.DC_CODE, BaseData.GUP_CODE, BaseData.CUST_CODE, BaseData.PROCESS_NO, newItems.ToArray(), removeItems.ToArray()));
			if (result.IsSuccessed == true)
			{
				ShowMessage(Messages.Success2);
				DoSearch();
			}
			else
				ShowMessage(Messages.Failed);
		}
		#endregion Save

		#region 全選
		public void CheckAll(bool isCheck)
		{
			IsBusy = true;
			if (Data == null) return;
			foreach (var p in Data)
				p.IsSelected = isCheck;
			IsBusy = false;
		}
		#endregion
		#endregion
	}
}
