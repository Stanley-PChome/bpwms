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
using Wms3pl.WpfClient.DataServices.F19DataService;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.P91.Services;
using Wms3pl.WpfClient.DataServices.F91DataService;
using Wms3pl.WpfClient.Services;
using wcf = Wms3pl.WpfClient.ExDataServices.P91WcfService;
namespace Wms3pl.WpfClient.P91.ViewModel
{
	public partial class P9101010300_ViewModel : InputViewModelBase
	{
		public Action OnExitAction = delegate { };
		public P9101010300_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
			}

		}

		#region 資料連結
		private F910201 _baseData;
		public F910201 BaseData
		{
			get { return _baseData; }
			set { _baseData = value; RaisePropertyChanged("BaseData"); }
		}

		/// <summary>
		/// 已設定的加工動作對應標籤
		/// </summary>
		private ObservableCollection<F910204> _f910204List = new ObservableCollection<F910204>();
		public ObservableCollection<F910204> F910204List
		{
			get { return _f910204List; }
			set { _f910204List = value; RaisePropertyChanged("F910204List"); }
		}

		/// <summary>
		/// 記錄原始的清單, 在儲存時要來比對, 找出新增/修改/刪除的資料
		/// </summary>
		private List<F910204> _orgList = new List<F910204>();
		public List<F910204> OrgList
		{
			get { return _orgList; }
			set { _orgList = value; }
		}

		/// <summary>
		/// 加工動作清單
		/// </summary>
		private List<NameValuePair<string>> _f910005List = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> F910005List
		{
			get { return _f910005List; }
			set { _f910005List = value; RaisePropertyChanged("F910005List"); }
		}

		/// <summary>
		/// 標籤編號
		/// </summary>
		private List<NameValuePair<string>> _f1970List = new List<NameValuePair<string>>();
		public List<NameValuePair<string>> F1970List
		{
			get { return _f1970List; }
			set { _f1970List = value; RaisePropertyChanged("F1970List"); }
		}

		private F910204 _selectedF910204;
		public F910204 SelectedF910204
		{
			get { return _selectedF910204; }
			set
			{
				_selectedF910204 = value;
				if (value == null) OrgF910204 = null;
				else
					OrgF910204 = new F910204() { ACTION_NO = value.ACTION_NO, PROCESS_NO = value.PROCESS_NO, GUP_CODE = value.GUP_CODE, CUST_CODE = value.CUST_CODE, DC_CODE = value.DC_CODE, LABEL_NO = value.LABEL_NO, ORDER_BY = value.ORDER_BY };
				RaisePropertyChanged("SelectedF910204");
			}
		}

		/// <summary>
		/// 備份選取的項目, 用來取消時可以回復該筆資料
		/// </summary>
		private F910204 _orgF910204;
		public F910204 OrgF910204
		{
			get { return _orgF910204; }
			set { _orgF910204 = value; }
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
			DoSearchMapptingTable();

			// 1. 查詢已設定的加工單動作對應標籤
			var proxy = GetProxy<F91Entities>();
			var tmp3 = proxy.F910204s.Where(x => x.PROCESS_NO == BaseData.PROCESS_NO && x.DC_CODE == BaseData.DC_CODE && x.GUP_CODE == BaseData.GUP_CODE && x.CUST_CODE == BaseData.CUST_CODE).ToObservableCollection();
			F910204List = tmp3;
			SelectedF910204 = F910204List.FirstOrDefault();

			// 2. 備份原始資料
			OrgList = tmp3.Select(AutoMapper.Mapper.DynamicMap<F910204>).ToList();
		}

		private void DoSearchMapptingTable()
		{
			IsBusy = true;
			// 0. 查詢F910005
			var proxy = GetProxy<F91Entities>();
			var tmp1 = proxy.F910005s.Select(x => new NameValuePair<string>() { Name = x.ACTION_NAME, Value = x.ACTION_NO }).ToList();
			F910005List = tmp1;

			// 1. 查詢F1970
			var proxy19 = GetProxy<F19Entities>();
			var tmp2 =
				proxy19.F1970s.Where(
					x =>
						(x.LABEL_TYPE == "2" || x.LABEL_TYPE == "0") && x.GUP_CODE == BaseData.GUP_CODE &&
						x.CUST_CODE == BaseData.CUST_CODE && x.STATUS == "0")
					.Select(x => new NameValuePair<string>() { Name = x.LABEL_NAME, Value = x.LABEL_CODE }).ToList();
			tmp2.Insert(0, new NameValuePair<string>() { Name = "", Value = "" });
			F1970List = tmp2;
			IsBusy = false;
		}
		#endregion Search

		#region Add
		public ICommand AddCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoAdd(),
					() => UserOperateMode == OperateMode.Query && BaseData.STATUS == "0"
				);
			}
		}

		private void DoAdd()
		{
			if (!CheckCanEdit())
				return;

			//執行新增動作
			UserOperateMode = OperateMode.Add;
			List<F910204> tmp = F910204List.ToList();
			tmp.Add(new F910204() { DC_CODE = BaseData.DC_CODE, CUST_CODE = BaseData.CUST_CODE, GUP_CODE = BaseData.GUP_CODE, PROCESS_NO = BaseData.PROCESS_NO });
			F910204List = tmp.ToObservableCollection();
			SelectedF910204 = F910204List.Last();
		}
		#endregion Add

		#region Edit
		public ICommand EditCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoEdit(),
					() => UserOperateMode == OperateMode.Query && SelectedF910204 != null
				);
			}
		}

		private void DoEdit()
		{
			//執行編輯動作
			UserOperateMode = OperateMode.Edit;
		}
		#endregion Edit

		#region Cancel
		public ICommand CancelCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoCancel(),
					() => (UserOperateMode == OperateMode.Query || UserOperateMode == OperateMode.Add)
						|| (SelectedF910204 != null && OrgF910204 != null
						&& (SelectedF910204.LABEL_NO != OrgF910204.LABEL_NO || SelectedF910204.ORDER_BY != OrgF910204.ORDER_BY || SelectedF910204.ACTION_NO != OrgF910204.ACTION_NO))
				);
			}
		}

		private void DoCancel()
		{
			//執行取消動作
			if (UserOperateMode == OperateMode.Add)
			{
				var tmp = F910204List.ToList();
				tmp.Remove(F910204List.LastOrDefault());
				F910204List = tmp.ToObservableCollection();
				SelectedF910204 = F910204List.FirstOrDefault();
			}
			else
			{
				SelectedF910204 = OrgF910204;
				F910204List = OrgList.Select(AutoMapper.Mapper.DynamicMap<F910204>).ToObservableCollection();
			}
			UserOperateMode = OperateMode.Query;
		}
		#endregion Cancel

		#region Delete
		public ICommand DeleteCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoDelete(),
					() => UserOperateMode == OperateMode.Query && SelectedF910204 != null && BaseData.STATUS == "0"
				);
			}
		}

		/// <summary>
		/// 刪除時直接刪掉
		/// </summary>
		private void DoDelete()
		{
			if (!CheckCanEdit())
				return;

			//執行刪除動作
			var tmp = F910204List.ToList();
			var tmp1 = tmp.Find(x => x.ACTION_NO == SelectedF910204.ACTION_NO && x.ORDER_BY == SelectedF910204.ORDER_BY);
			if (tmp1 != null)
			{
				var proxy = GetProxy<F91Entities>();
				var data = proxy.F910204s.Where(x => x.DC_CODE == SelectedF910204.DC_CODE && x.GUP_CODE == SelectedF910204.GUP_CODE && x.CUST_CODE == SelectedF910204.CUST_CODE && x.PROCESS_NO == SelectedF910204.PROCESS_NO && x.ORDER_BY == SelectedF910204.ORDER_BY && x.ACTION_NO == SelectedF910204.ACTION_NO).FirstOrDefault();
				proxy.DeleteObject(data);
				proxy.SaveChanges();
				tmp.Remove(tmp1);
			}
			F910204List = tmp.ToObservableCollection();
			SelectedF910204 = F910204List.FirstOrDefault();
		}
		#endregion Delete

		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),
					() => (UserOperateMode == OperateMode.Query || UserOperateMode == OperateMode.Add)
						&& BaseData.STATUS == "0"
						&& (SelectedF910204 != null && OrgF910204 != null)
						&& (SelectedF910204.LABEL_NO != OrgF910204.LABEL_NO || SelectedF910204.ORDER_BY != OrgF910204.ORDER_BY || SelectedF910204.ACTION_NO != OrgF910204.ACTION_NO)
						&& (SelectedF910204.ACTION_NO != null && !string.IsNullOrEmpty(SelectedF910204.ACTION_NO))
					);
			}
		}

		/// <summary>
		/// 檢查加工單狀態是否能預選標籤
		/// </summary>
		/// <returns></returns>
		bool CheckCanEdit()
		{
			var f910201 = FindByKey<F910201>(BaseData);
			if (f910201.STATUS != "0")
			{
				ShowWarningMessage(Properties.Resources.P9101010300_ViewModel_CantEdit);
				BaseData = f910201;
				return false;
			}

			return true;
		}

		private void DoSave()
		{
			if (!CheckCanEdit())
				return;

			if (IsDuplicated())
			{
				ShowMessage(Messages.WarningDuplicatedData);
				return;
			}
			if (UserOperateMode == OperateMode.Query)
			{
				// 確認時, 為避免修改的是KEY值(Key值不能直接用EF儲存), 所以一律先刪再加
				var proxy = GetProxy<F91Entities>();
				var data = proxy.F910204s.Where(x => x.DC_CODE == OrgF910204.DC_CODE && x.GUP_CODE == OrgF910204.GUP_CODE && x.CUST_CODE == OrgF910204.CUST_CODE && x.PROCESS_NO == OrgF910204.PROCESS_NO && x.ORDER_BY == OrgF910204.ORDER_BY && x.ACTION_NO == OrgF910204.ACTION_NO).FirstOrDefault();
				proxy.DeleteObject(data);
				proxy.SaveChanges();
			}
			{
				//執行新增儲存動作
				var proxy = GetProxy<F91Entities>();
				proxy.AddToF910204s(new F910204()
				{
					DC_CODE = SelectedF910204.DC_CODE,
					GUP_CODE = SelectedF910204.GUP_CODE,
					CUST_CODE = SelectedF910204.CUST_CODE,
					PROCESS_NO = SelectedF910204.PROCESS_NO,
					ACTION_NO = SelectedF910204.ACTION_NO,
					ORDER_BY = SelectedF910204.ORDER_BY,
					LABEL_NO = SelectedF910204.LABEL_NO
				});
				proxy.SaveChanges();
				ShowMessage(Messages.Success);
				UserOperateMode = OperateMode.Query;
				OrgF910204 = AutoMapper.Mapper.DynamicMap<F910204>(SelectedF910204);
			}
			OrgList = F910204List.Select(AutoMapper.Mapper.DynamicMap<F910204>).ToList();

		}

		/// <summary>
		/// 檢查資料是否有重複
		/// </summary>
		/// <returns>true: 資料重複</returns>
		private bool IsDuplicated()
		{
			var proxy = GetProxy<F91Entities>();
			var tmp = proxy.F910204s.Where(x => x.DC_CODE == SelectedF910204.DC_CODE && x.GUP_CODE == SelectedF910204.GUP_CODE && x.CUST_CODE == SelectedF910204.CUST_CODE && x.PROCESS_NO == SelectedF910204.PROCESS_NO && x.ACTION_NO == SelectedF910204.ACTION_NO && x.ORDER_BY == SelectedF910204.ORDER_BY).Count();

			if (UserOperateMode == OperateMode.Query)
			{
				// 是編輯的話, 只會找到一筆資料
				return (tmp > 1);
			}
			else
			{
				// 否則不應該找到任何資料
				return (tmp > 0);
			}
		}

		#region 不知道用不用的到
		/// <summary>
		/// 取得被刪除的項目
		/// </summary>
		/// <returns></returns>
		private List<F910204> GetRemovedList()
		{
			// 取得被刪除的項目
			var tmp = from a in OrgList
					  join b in F910204List on new { a.ORDER_BY, a.ACTION_NO } equals new { b.ORDER_BY, b.ACTION_NO } into d
					  from c in d.DefaultIfEmpty()
					  where c == null
					  select a;
			return tmp.ToList();
		}

		/// <summary>
		/// 取得新增的項目
		/// </summary>
		/// <returns></returns>
		private List<F910204> GetNewList()
		{
			var tmp = from a in F910204List
					  join b in OrgList on new { a.ORDER_BY, a.ACTION_NO } equals new { b.ORDER_BY, b.ACTION_NO } into d
					  from c in d.DefaultIfEmpty()
					  where c == null
					  select a;
			return tmp.ToList();
		}

		/// <summary>
		/// 取得修改的項目
		/// </summary>
		/// <returns></returns>
		private List<F910204> GetUpdateList()
		{
			var tmp = from a in F910204List
					  join b in OrgList on new { a.ORDER_BY, a.ACTION_NO } equals new { b.ORDER_BY, b.ACTION_NO } into d
					  from c in d.DefaultIfEmpty()
					  where c != null
					  select a;
			return tmp.ToList();
		}
		#endregion
		#endregion Save

		public ICommand ExitCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoExit(),
					() => UserOperateMode != OperateMode.Add,
					o => OnExitAction()
				);
			}
		}
		private void DoExit()
		{
			if (F910204List.Count > 0)
			{
				// 資料列有設定，則Update F910201.PROC_STATUS=2 Where PROC_STATUS=1
				var proxy = GetProxy<F91Entities>();
				var data = proxy.F910201s.Where(x => x.DC_CODE == BaseData.DC_CODE && x.GUP_CODE == BaseData.GUP_CODE && x.CUST_CODE == BaseData.CUST_CODE && x.PROCESS_NO == BaseData.PROCESS_NO && x.PROC_STATUS == "1").FirstOrDefault();
				if (data != null)
				{
					data.PROC_STATUS = "2";
					proxy.UpdateObject(data);
					proxy.SaveChanges();
				}
			}
		}
		#endregion

	}
}
