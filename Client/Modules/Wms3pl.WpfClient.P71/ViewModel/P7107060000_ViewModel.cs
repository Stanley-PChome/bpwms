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
using Wms3pl.WpfClient.Services;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.DataServices.F70DataService;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices.P71ExDataService;
using wcf = Wms3pl.WpfClient.ExDataServices.P71WcfService;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Win32;
using Wms3pl.WpfClient.UILib.Services;

namespace Wms3pl.WpfClient.P71.ViewModel
{
	public partial class P7107060000_ViewModel : InputViewModelBase
	{
		public Action OnAddProcessMode = delegate { };
		public Action OnEditProcessMode = delegate { };
		public Action OnQueryProcessMode = delegate { };

		public P7107060000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				DcList = GetDcList();
				GrpList = GetF1953List();

				ImportSDate = DateTime.Today;
				ImportEDate = DateTime.Today;


				if (DcList.Any())
					DcQuery = DcList.First().Value;
			}

		}

		#region DC  設定
		//物流中心List
		private List<NameValuePair<string>> _dcList;
		public List<NameValuePair<string>> DcList
		{
			get { return _dcList; }
			set
			{
				_dcList = value;
				RaisePropertyChanged("DcList");
			}
		}

		//Query Dc Value
		private string _dcQuery;
		public string DcQuery
		{
			get { return _dcQuery; }
			set
			{
				_dcQuery = value;
				RaisePropertyChanged("DcQuery");
			}
		}



		#endregion

		#region 群組 List
		//物流中心List
		private List<NameValuePair<string>> _grpList;
		public List<NameValuePair<string>> GrpList
		{
			get { return _grpList; }
			set
			{
				_grpList = value;
				RaisePropertyChanged("GrpList");
			}
		}
		#endregion

		#region F700701 SelectData
		private SelectionItem<F700701QueryData> _selectedF700701;

		public SelectionItem<F700701QueryData> SelectedF700701
		{
			get { return _selectedF700701; }
			set
			{
				_selectedF700701 = value;
				RaisePropertyChanged("SelectedF700701");
			}
		}
		#endregion


		#region 投入日期-查詢  參數
		private DateTime? _importSDate;
		public DateTime? ImportSDate
		{
			get { return _importSDate; }
			set
			{
				_importSDate = value;
				RaisePropertyChanged("ImportSDate");
			}
		}

		private DateTime? _importEDate;
		public DateTime? ImportEDate
		{
			get { return _importEDate; }
			set
			{
				_importEDate = value;
				RaisePropertyChanged("ImportEDate");
			}
		}

		#endregion

		#region Grid List
		private SelectionList<F700701QueryData> _f700701List;

		public SelectionList<F700701QueryData> F700701List
		{
			get { return _f700701List; }
			set
			{
				_f700701List = value;
				RaisePropertyChanged("F700701List");
			}
		}
		#endregion

		#region F700701Add
		private F700701QueryData _f700701Add;

		public F700701QueryData F700701Add
		{
			get { return _f700701Add; }
			set
			{
				_f700701Add = value;
				RaisePropertyChanged("F700701Add");
			}
		}
		#endregion

		#region Function

		#region 取物流中心資料

		public List<NameValuePair<string>> GetDcList()
		{
			var data = Wms3plSession.Get<GlobalInfo>().DcCodeList;
			return data;
		}

		#endregion

		#region 取群組資料
		private List<NameValuePair<string>> GetF1953List()
		{
			var proxy = GetProxy<F19Entities>();
			var data = proxy.F1953s.Where(o => o.ISDELETED == "0").ToList();
			var list = (from o in data
						select new NameValuePair<string>
						{
							Name = o.GRP_NAME,
							Value = o.GRP_ID.ToString()
						}).ToList();

			return list;
		}
		#endregion

		#endregion

		#region Search
		public ICommand SearchCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSearch(), () => UserOperateMode == OperateMode.Query
					, o =>
					{

						OnQueryProcessMode();
					});
			}
		}

		private void DoSearch()
		{
			var proxy = GetExProxy<P71ExDataSource>();
			var f700701Data = proxy.CreateQuery<F700701QueryData>("GetF700701QueryData")
									   .AddQueryOption("dcCode", string.Format("'{0}'", DcQuery))
									   .AddQueryOption("importSDate", string.Format("'{0}'", ImportSDate.ToString()))
									   .AddQueryOption("importEDate", string.Format("'{0}'", ImportEDate.ToString())).ToList();

			F700701List = new SelectionList<F700701QueryData>(f700701Data.ToList());
		}
		#endregion Search



		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => { }, () => UserOperateMode == OperateMode.Query
					, o =>
					{
						DoAdd();
						OnAddProcessMode();
					}
					);
			}
		}

		private void DoAdd()
		{
			UserOperateMode = OperateMode.Add;

			F700701Add = new F700701QueryData();
			if (F700701List == null)
				F700701List = new SelectionList<F700701QueryData>(new List<F700701QueryData>());

			var si = new SelectionItem<F700701QueryData>(F700701Add);
			si.Item.IMPORT_DATE = DateTime.Today;
			F700701List.Add(si);
			SelectedF700701 = si;
		}
		#endregion Add

		#region Edit
		ICommand _editCommand;
		public ICommand EditCommand
		{
			get
			{
				return _editCommand ?? (_editCommand = new RelayCommand(() =>
				{
					UserOperateMode = OperateMode.Edit;
					OnEditProcessMode();
				}, () => UserOperateMode == OperateMode.Query && SelectedF700701 != null));

			}
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(), () => UserOperateMode != OperateMode.Query
					, o =>
					{
						SearchCommand.Execute(null);
					});
			}
		}

		private void DoCancel()
		{
			//執行取消動作

			UserOperateMode = OperateMode.Query;
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
				bool result = false;
				return CreateBusyAsyncCommand(
					o => result = DoSave(), () => UserOperateMode != OperateMode.Query
					, o =>
					{
						if (result)
						{
							UserOperateMode = OperateMode.Query;
							SearchCommand.Execute(null);
						}
					}
					);
			}
		}

		private bool DoSave()
		{
			//執行確認儲存動作


			bool result = false;
			if (UserOperateMode == OperateMode.Add)
			{
				result = DoAddSave();
			}
			else
			{
				result = DoEditSave();
			}
			return result;
		}


		private bool DoAddSave()
		{

			F700701 f700701 = new F700701
			{
				DC_CODE = DcQuery,
				IMPORT_DATE = SelectedF700701.Item.IMPORT_DATE,
				GRP_ID = SelectedF700701.Item.GRP_ID,
				PERSON_NUMBER = SelectedF700701.Item.PERSON_NUMBER,
				WORK_HOUR = SelectedF700701.Item.WORK_HOUR,
				SALARY = SelectedF700701.Item.SALARY,
			};


			var proxy = new wcf.P71WcfServiceClient();
			var wcf700701 = ExDataMapper.Map<F700701, wcf.F700701>(f700701);
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
							() => proxy.InsertF700701(wcf700701));

			if (result.IsSuccessed)
			{
				ImportEDate = SelectedF700701.Item.IMPORT_DATE;
				ShowMessage(Messages.InfoAddSuccess);
			}
			else
			{
				DialogService.ShowMessage(result.Message);
			}


			return result.IsSuccessed;
		}

		private bool DoEditSave()
		{

			F700701 f700701 = new F700701
			{
				DC_CODE = DcQuery,
				IMPORT_DATE = SelectedF700701.Item.IMPORT_DATE,
				GRP_ID = SelectedF700701.Item.GRP_ID,
				PERSON_NUMBER = SelectedF700701.Item.PERSON_NUMBER,
				WORK_HOUR = SelectedF700701.Item.WORK_HOUR,
				SALARY = SelectedF700701.Item.SALARY,
			};


			var proxy = new wcf.P71WcfServiceClient();
			var wcf700701 = ExDataMapper.Map<F700701, wcf.F700701>(f700701);
			var result = RunWcfMethod<wcf.ExecuteResult>(proxy.InnerChannel,
							() => proxy.UpdateF700701(wcf700701));

			if (result.IsSuccessed)
			{
				ShowMessage(Messages.InfoUpdateSuccess);
			}
			else
			{
				DialogService.ShowMessage(result.Message);
			}

			return result.IsSuccessed;
		}

		#endregion Save
	}
}
