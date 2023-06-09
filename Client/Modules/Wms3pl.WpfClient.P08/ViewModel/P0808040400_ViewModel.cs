using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P08ExDataService;
using Wms3pl.WpfClient.UILib;
using wcf = Wms3pl.WpfClient.ExDataServices.P08WcfService;

namespace Wms3pl.WpfClient.P08.ViewModel
{
	public class P0808040400_ViewModel : InputViewModelBase
	{
		public P0808040400_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				DcList = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			}

		}

		#region Property
		#region 物流中心
		//物流中心清單
		private List<NameValuePair<string>> _dcList;

		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				Set(() => DcList, ref _dcList, value);
			}
		}

		/// <summary>
		/// 選取的物流中心
		/// </summary>
		private string _selectedDc = string.Empty;
		public string SelectedDc
		{
			get { return _selectedDc; }
			set
			{ Set(() => SelectedDc, ref _selectedDc, value); }
		}
		#endregion

		#region 容器條碼
		private string _containerBarcode;
		public string ContainerBarcode
		{
			get { return _containerBarcode; }
			set { Set(() => ContainerBarcode, ref _containerBarcode, value); }
		}
		#endregion

		#region 查詢結果
		private ObservableCollection<BatchPickData> _batchPickData = new ObservableCollection<BatchPickData>();
		public ObservableCollection<BatchPickData> BatchPickData
		{
			get { return _batchPickData; }
			set
			{
				Set(() => BatchPickData, ref _batchPickData, value);
			}
		}
		#endregion

		private string _gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
		private string _custCode = Wms3plSession.Get<GlobalInfo>().CustCode;
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
			//執行查詢動作
			var proxy = GetExProxy<P08ExDataSource>();
			if (CheckData())
			{
				BatchPickData = proxy.CreateQuery<BatchPickData>("GetBatchPickData")
				.AddQueryExOption("dcCode", SelectedDc)
				.AddQueryExOption("gupCode", _gupCode)
				.AddQueryExOption("custCode", _custCode)
				.AddQueryExOption("containerBarcode", ContainerBarcode)
				.ToList().ToObservableCollection();

				if (BatchPickData == null || !BatchPickData.Any())
				{
					ShowMessage(Messages.InfoNoData);
				}
			}
			
			
		}

		private bool CheckData()
		{
			if (string.IsNullOrWhiteSpace(SelectedDc))
			{
				DialogService.ShowMessage("物流中心必須選擇");
				return false;
			}
			if (string.IsNullOrWhiteSpace(ContainerBarcode))
			{
				DialogService.ShowMessage("容器條碼不得為空");
				return false;
			}

			return true;

			
		}

		
		#endregion Search

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoAdd()
		{
			UserOperateMode = OperateMode.Add;
			//執行新增動作
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoEdit()
		{
			UserOperateMode = OperateMode.Edit;
			//執行編輯動作
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoCancel()
		{
			//執行取消動作

			//UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(), () => UserOperateMode == OperateMode.Query
					);
			}
		}

		private void DoDelete()
		{
			//執行刪除動作
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(), () => UserOperateMode != OperateMode.Query
					);
			}
		}

		private void DoSave()
		{
			//執行確認儲存動作

			UserOperateMode = OperateMode.Query;
		}
		#endregion Save
		
	}
}