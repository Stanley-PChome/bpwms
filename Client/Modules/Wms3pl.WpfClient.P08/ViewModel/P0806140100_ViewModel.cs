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
using Wms3pl.WpfClient.ExDataServices.P08WcfService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public class P0806140100_ViewModel : InputViewModelBase
	{

		#region 分貨站層清單
		private List<PickAllotLoc> _pickAllotLocs;

		public List<PickAllotLoc> PickAllotLocs
		{
			get { return _pickAllotLocs; }
			set
			{
				Set(() => PickAllotLocs, ref _pickAllotLocs, value);
			}
		}
		#endregion


		#region 箱明細
		private List<ShipOrderPickAllotDetail> _boxDetail;

		public List<ShipOrderPickAllotDetail> BoxDetail
		{
			get { return _boxDetail; }
			set
			{
				Set(() => BoxDetail, ref _boxDetail, value);
			}
		}
		#endregion


		#region 選取的明細
		private ShipOrderPickAllotDetail _selectedBoxItem;

		public ShipOrderPickAllotDetail SelectedBoxItem
		{
			get { return _selectedBoxItem; }
			set
			{
				Set(() => SelectedBoxItem, ref _selectedBoxItem, value);
			}
		}
		#endregion



		#region 箱清單
		private List<NameValuePair<string>> _boxList;

		public List<NameValuePair<string>> BoxList
		{
			get { return _boxList; }
			set
			{
				Set(() => BoxList, ref _boxList, value);
			}
		}
		#endregion


		#region 選取的箱號
		private string _selectedBoxNo;

		public string SelectedBoxNo
		{
			get { return _selectedBoxNo; }
			set
			{
				Set(() => SelectedBoxNo, ref _selectedBoxNo, value);
				if (value != null)
					SearchCommand.Execute(null);
			}
		}
		#endregion


		public P0806140100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
			}

		}
		public void Init()
		{
			BoxList = PickAllotLocs
			.Where(x => x.IsBinding)
			.Select(x => new NameValuePair<string> { Name = x.BoxNo,Value=x.BoxNo }).ToList();
			if (BoxList.Any())
				SelectedBoxNo = BoxList.First().Value;
		}

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoSearch()
		{
			//執行查詢動
			DispatcherAction(() =>
			{
				BoxDetail = PickAllotLocs.First(x => x.BoxNo == SelectedBoxNo).Details.ToList();
				if (BoxDetail.Any())
					SelectedBoxItem = BoxDetail.First();
			});
		}
		#endregion Search

	}
}
