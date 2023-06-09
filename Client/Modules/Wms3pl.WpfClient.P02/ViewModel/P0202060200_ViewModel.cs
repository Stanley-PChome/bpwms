using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices.F02DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P02ExDataService;
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.UILib;
using ContainerDetailData = Wms3pl.WpfClient.ExDataServices.P02ExDataService.ContainerDetailData;
using wcf = Wms3pl.WpfClient.ExDataServices.SharedWcfService;

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0202060200_ViewModel : InputViewModelBase
	{
		private string _custCode { get { return Wms3plSession.Get<GlobalInfo>().CustCode; } }
		private string _gupCode { get { return Wms3plSession.Get<GlobalInfo>().GupCode; } }


		public P0202060200_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
				InitControls();
			}
		}

		private void InitControls()
		{

		}

		#region 物流中心編號
		private string _dcCode;
		public string DcCode
		{
			get { return _dcCode; }
			set { Set(() => DcCode, ref _dcCode, value); }
		}

		#endregion

		#region 驗收容器上架頭檔
		// 驗收容器清單
		private List<F020501> _f020501Data;
		public List<F020501> F020501Data
		{
			get { return _f020501Data; }
			set { Set(() => F020501Data, ref _f020501Data, value); }
		}

		// 選擇的驗收容器
		private F020501 _selectContainerDetailData;
		public F020501 SelectContainerDetailData
		{
			get { return _selectContainerDetailData; }
			set
			{
				Set(() => SelectContainerDetailData, ref _selectContainerDetailData, value);
			}
		}

		#endregion



		#region 容器條碼

		// 容器條碼
		private List<NameValuePair<string>> _containerCodeList;
		public List<NameValuePair<string>> ContainerCodeList
		{
			get { return _containerCodeList; }
			set
			{
				Set(() => ContainerCodeList, ref _containerCodeList, value);
			}
		}

		// 所選擇的容器條碼
		private string _selectF020501Id;
		public string SelectF020501Id
		{
			get { return _selectF020501Id; }
			set
			{
				Set(() => SelectF020501Id, ref _selectF020501Id, value);
				SetContainerDetialData();
			}
		}
		#endregion

		#region 取得容器條碼清單
		public void GetContainerCodeList()
		{

			ContainerCodeList = (from A in F020501Data
													 select new NameValuePair<string>
													 {
														 Name = A.CONTAINER_CODE,
														 Value = A.ID.ToString()
													 }).ToList();
		}
		#endregion


		#region 取得驗收容器上架清單
		public void GetF020501Data()
		{
			var proxy = GetProxy<F02Entities>();
			F020501Data = proxy.F020501s.ToList().Where(x => x.DC_CODE == DcCode &&
			x.GUP_CODE == _gupCode &&
			x.CUST_CODE == _custCode &&
      !new[] { "6", "9" }.Contains(x.STATUS)).OrderBy(x => x.CONTAINER_CODE).ToList();
    }
		#endregion

		#region 上架倉別
		private string _pickWareId;
		public string PickWareId
		{
			get { return _pickWareId; }
			set { Set(() => PickWareId, ref _pickWareId, value); }
		}
		#endregion

		#region 容器狀態
		private string _status;
		public string Status
		{
			get { return _status; }
			set { Set(() => Status, ref _status, value); }
		}
		#endregion

		#region 驗收容器上架明細
		private List<ContainerDetailData> _dgList = new List<ContainerDetailData>();
		public List<ContainerDetailData> DgList
		{
			get { return _dgList; }
			set { Set(() => DgList, ref _dgList, value); }
		}

		private string _warehouse;
		public string Warehouse
		{
			get { return _warehouse; }
			set { Set(() => Warehouse, ref _warehouse, value); }
		}

		#endregion

		#region Math
		// 取得上架倉別名稱
		public string GetPickWareName(string pickWareId)
		{
			var proxy = GetProxy<F19Entities>();
			return proxy.F1980s.Where(x => x.WAREHOUSE_ID == pickWareId).FirstOrDefault()?.WAREHOUSE_NAME;
		}

		// 取得容器狀態名稱
		public string GetStatus(string status)
		{
			return GetBaseTableService.GetF000904List(FunctionCode, "F020501", "STATUS").Where(x => x.Value == status).FirstOrDefault()?.Name;
		}

		// 查詢驗收容器明細
		public void DoSearch()
		{
			var proxyEx = GetExProxy<P02ExDataSource>();
			DgList = proxyEx.CreateQuery<ContainerDetailData>("GetContainerDetail")
				.AddQueryExOption("dcCode", DcCode)
				.AddQueryExOption("gupCode", _gupCode)
				.AddQueryExOption("custCode", _custCode)
				.AddQueryExOption("f020501Id", SelectContainerDetailData.ID.ToString()).ToList();

		}

		//關箱Command
		public ICommand ContainerCloseBoxCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoContainerCloseBox(),
					() => UserOperateMode == OperateMode.Query && SelectContainerDetailData?.STATUS == "0",
					o => {
						GetF020501Data();
						SetContainerDetialData();
					});

			}
		}

		// 關箱
		private void DoContainerCloseBox()
		{
			var proxy = GetWcfProxy<wcf.SharedWcfServiceClient>();
      var result = proxy.RunWcfMethod(w => w.ContainerCloseBoxWithRT(Convert.ToInt64(SelectF020501Id), null, null));
      if (!result.IsSuccessed)
			{
				DialogService.ShowMessage(result.Message);
			}
		}

		private void SetContainerDetialData()
		{
			var proxy = GetProxy<F19Entities>();
			SelectContainerDetailData = F020501Data.Where(x => x.ID == Convert.ToInt64(SelectF020501Id)).First();
			Status = GetStatus(SelectContainerDetailData.STATUS);
			var warehouseNmae = proxy.F1980s.Where(x => x.WAREHOUSE_ID == SelectContainerDetailData.PICK_WARE_ID && x.DC_CODE == DcCode).FirstOrDefault()?.WAREHOUSE_NAME;
			Warehouse = $"{SelectContainerDetailData.PICK_WARE_ID} {warehouseNmae}";

			DoSearch();
		}
		#endregion
	}
}