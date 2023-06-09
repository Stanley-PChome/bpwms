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

namespace Wms3pl.WpfClient.P02.ViewModel
{
	public partial class P0203030000_ViewModel : InputViewModelBase
	{
		public P0203030000_ViewModel()
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

	}
}
