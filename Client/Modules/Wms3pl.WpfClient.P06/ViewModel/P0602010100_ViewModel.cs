using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P06ExDataService;
using Wms3pl.WpfClient.UILib;

namespace Wms3pl.WpfClient.P06.ViewModel
{
	public class P0602010100_ViewModel: InputViewModelBase
	{

		public Action OnSave = delegate { };
		public P0602010100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				//初始化執行時所需的值及資料
			}
		}

		private F051206LackList _data;
		public F051206LackList Data
		{
			get { return _data; }
			set
			{
				Set(ref _data, value);
			}
		}

		private int? _lackQty;
		public int? LackQty
		{
			get { return _lackQty; }
			set
			{
				Set(ref _lackQty, value);
			}
		}

		private bool _saveSuccess = false;
		#region Save
		public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o => DoSave(),
					() => LackQty.HasValue,
					o => DoSaveCompleted()
					);
			}
		}

		private void DoSave()
		{
			if (LackQty < 0)
			{
				DialogService.ShowMessage(Properties.Resources.LackQtyNotLessZero);
				return;
			}
			if (LackQty > Data.LACK_QTY)
			{
				DialogService.ShowMessage(Properties.Resources.LackQtyNotGreaterOrigin);
				return;
			}

			var proxyEx = GetExProxy<P06ExDataSource>();
			var res = proxyEx.CreateQuery<ExecuteResult>("ModifyLackQty")
								.AddQueryExOption("dcCode", Data.DC_CODE)
								.AddQueryExOption("gupCode", Data.GUP_CODE)
                .AddQueryExOption("custCode", Data.CUST_CODE)
								.AddQueryExOption("lackSeq", Data.LACK_SEQ)
								.AddQueryExOption("pickOrdNo", Data.PICK_ORD_NO)
								.AddQueryExOption("pickOrdSeq", Data.PICK_ORD_SEQ)
								.AddQueryExOption("lackQty", LackQty).SingleOrDefault();

      if (res?.IsSuccessed == true)
      {
        DialogService.ShowMessage("修改成功");
      }
      else
        DialogService.ShowMessage(res.Message);

      _saveSuccess = (res?.IsSuccessed) ?? false;
    }

		private void DoSaveCompleted()
		{
			if (_saveSuccess)
			{
				OnSave();
			}
		}
		#endregion Save
	}
}
