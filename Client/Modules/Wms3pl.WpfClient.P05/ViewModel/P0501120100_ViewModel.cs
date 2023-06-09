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
using Wms3pl.WpfClient.ExDataServices.P05ExDataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P05WcfService;

namespace Wms3pl.WpfClient.P05.ViewModel
{
	public class P0501120100_ViewModel : InputViewModelBase
	{
		public P0501120100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				Init();
			}

		}

		#region Property

		#region 彙總批次主檔
		private P050112Batch _master;

		public P050112Batch Master
		{
			get { return _master; }
			set
			{
				Set(() => Master, ref _master, value);
			}
		}
		#endregion

		#region 彙總單號清單
		private List<BatchPickStation> _batchPickStationList;

		public List<BatchPickStation> BatchPickStationList
		{
			get { return _batchPickStationList; }
			set
			{
				Set(() => BatchPickStationList, ref _batchPickStationList, value);
			}
		}
		#endregion

		#region 選取的彙總單號
		private BatchPickStation _selectedItem;

		public BatchPickStation SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				Set(() => SelectedItem, ref _selectedItem, value);
			}
		}
		#endregion


		#region 工作站清單
		private List<NameValuePair<string>> _stationList;

		public List<NameValuePair<string>> StationList
		{
			get { return _stationList; }
			set
			{
				Set(() => StationList, ref _stationList, value);
			}
		}
		#endregion



		#endregion

		#region Method
		private void Init()
		{
			StationList = GetBaseTableService.GetF000904List(FunctionCode, "P081201", "Workstation");
		}
		#endregion

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
			var proxy = GetExProxy<P05ExDataSource>();
			BatchPickStationList = proxy.CreateQuery<BatchPickStation>("GetBatchPickStations")
				.AddQueryExOption("dcCode", Master.DC_CODE)
				.AddQueryExOption("gupCode", Master.GUP_CODE)
				.AddQueryExOption("custCode", Master.CUST_CODE)
				.AddQueryExOption("batchNo", Master.BATCH_NO).ToList();
		}
		#endregion Search

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => UserOperateMode == OperateMode.Query && Master!=null && Master.PICK_STATUS != "2" && Master.PICK_STATUS !="9"
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作
			var batchPickStations = ExDataMapper.MapCollection<BatchPickStation, wcf.BatchPickStation>(BatchPickStationList).ToArray();

			var proxy = GetWcfProxy<wcf.P05WcfServiceClient>();
			var result = proxy.RunWcfMethod(w => w.AdjustAGVStations(Master.DC_CODE, Master.GUP_CODE, Master.CUST_CODE, Master.BATCH_NO, batchPickStations));
			ShowResultMessage(result.IsSuccessed, result.Message);
			DispatcherAction(() =>
			{
				SearchCommand.Execute(null);
			});

		}
		#endregion Save
	}
}
