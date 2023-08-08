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

namespace Wms3pl.WpfClient.P06.ViewModel
{
	/// <summary>
	/// UI繪製用Class,開發後無用須刪除
	/// </summary>
	public partial class P0600000000_ViewModel : InputViewModelBase
	{
		public P0600000000_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料				
			}

		}

		#region Search
		private ICommand _searchCommand;
		public ICommand SearchCommand
		{
			get
			{
				return _searchCommand ?? (_searchCommand = new RelayCommand(async () =>
				{
					IsBusy = true;
					try
					{
						await Task.Run(() => DoSearchAsync());
					}
					catch (Exception ex)
					{
						Exception = ex;
					}
					IsBusy = false;
				},
				() => !IsBusy));
			}
		}

		private void DoSearchAsync()
		{
			//執行查詢動作
		}
		#endregion Search

		#region Add
		private ICommand _addCommand;
		public ICommand AddCommand
		{
			get
			{
				return _addCommand ?? (_addCommand = new RelayCommand(async () =>
				{
					IsBusy = true;
					try
					{
						await Task.Run(() => DoAddAsync());
					}
					catch (Exception ex)
					{
						Exception = ex;
					}
					IsBusy = false;
				},
				() => !IsBusy));
			}
		}

		private void DoAddAsync()
		{
			//執行新增動作
		}
		#endregion Add

		#region Edit
		private ICommand _editCommand;
		public ICommand EditCommand
		{
			get
			{
				return _editCommand ?? (_editCommand = new RelayCommand(async () =>
				{
					IsBusy = true;
					try
					{
						await Task.Run(() => DoEditAsync());
					}
					catch (Exception ex)
					{
						Exception = ex;
					}
					IsBusy = false;
				},
				() => !IsBusy));
			}
		}

		private void DoEditAsync()
		{
			//執行編輯動作
		}
		#endregion Edit

		#region Cancel
		private ICommand _cancelCommand;
		public ICommand CancelCommand
		{
			get
			{
				return _cancelCommand ?? (_cancelCommand = new RelayCommand(async () =>
				{
					IsBusy = true;
					try
					{
						await Task.Run(() => DoCancelAsync());
					}
					catch (Exception ex)
					{
						Exception = ex;
					}
					IsBusy = false;
				},
				() => !IsBusy));
			}
		}

		private void DoCancelAsync()
		{
			//執行取消動作
		}
		#endregion Cancel

		#region Delete
		private ICommand _deleteCommand;
		public ICommand DeleteCommand
		{
			get
			{
				return _deleteCommand ?? (_deleteCommand = new RelayCommand(async () =>
				{
					IsBusy = true;
					try
					{
						await Task.Run(() => DoDeleteAsync());
					}
					catch (Exception ex)
					{
						Exception = ex;
					}
					IsBusy = false;
				},
				() => !IsBusy));
			}
		}

		private void DoDeleteAsync()
		{
			//執行刪除動作
		}
		#endregion Delete

		#region Save
		private ICommand _saveCommand;
		public ICommand SaveCommand
		{
			get
			{
				return _saveCommand ?? (_saveCommand = new RelayCommand(async () =>
				{
					IsBusy = true;
					try
					{
						await Task.Run(() => DoSaveAsync());
					}
					catch (Exception ex)
					{
						Exception = ex;
					}
					IsBusy = false;
				},
				() => !IsBusy));
			}
		}

		private void DoSaveAsync()
		{
			//執行確認儲存動作
		}
		#endregion Save

		public List<DgDataClass> DgItemSource { get; set; }
		public List<DgDataClass> DgItemSource2 { get; set; }
		public class DgDataClass
		{
			public string Str1 { get; set; }
			public string Str2 { get; set; }
			public string Str3 { get; set; }
			public string Str4 { get; set; }
			public string Str5 { get; set; }
			public string Str6 { get; set; }
			public string Str7 { get; set; }
			public string Str8 { get; set; }
			public string Str9 { get; set; }
			public string Str10 { get; set; }

			public string Str11 { get; set; }
			public string Str12 { get; set; }
			public string Str13 { get; set; }
			public string Str14 { get; set; }
			public string Str15 { get; set; }
			public string Str16 { get; set; }
			public string Str17 { get; set; }
			public string Str18 { get; set; }
			public string Str19 { get; set; }
			public string Str20 { get; set; }
			public string Str21 { get; set; }
			public string Str22 { get; set; }
			public string Str23 { get; set; }
			public string Str24 { get; set; }
			public string Str25 { get; set; }
			public string Str26 { get; set; }
			public string Str27 { get; set; }
			public string Str28 { get; set; }
			public string Str29 { get; set; }
			public bool Bool1 { get; set; }
			public bool Bool2 { get; set; }
			public bool Bool3 { get; set; }
			public bool Bool4 { get; set; }
			public bool Bool5 { get; set; }
			public int Int1 { get; set; }
			public int Int2 { get; set; }
			public int Int3 { get; set; }
		}
	}
}
