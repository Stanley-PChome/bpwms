using System;
using System.Linq;
using System.Windows.Input;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using wcf = Wms3pl.WpfClient.ExDataServices.P18WcfService;
using Wms3pl.WpfClient.ExDataServices.P18ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Wms3pl.WpfClient.P18.ViewModel
{
	public partial class P1801010100_ViewModel : InputViewModelBase
	{
		private string _gupCode;
		private string _custCode;
		public P1801010100_ViewModel()
		{
			if (!IsInDesignMode)
			{
				_gupCode = Wms3plSession.Get<GlobalInfo>().GupCode;
				_custCode = Wms3plSession.Get<GlobalInfo>().CustCode;

			}

		}

		#region 新效期
		private DateTime _newValidDate;

		public DateTime NewValidDate
		{
			get { return _newValidDate; }
			set
			{
				Set(() => NewValidDate, ref _newValidDate, value);
			}
		}
		#endregion

		#region 新批號
		private string _newMakeNo;

		public string NewMakeNo
		{
			get { return _newMakeNo; }
			set
			{
				Set(() => NewMakeNo, ref _newMakeNo, value);
			}
		}
		#endregion

		#region 庫存資料
		private StockQueryData1 _tmpDgQueryData;

		public StockQueryData1 TmpDgQueryData
		{
			get { return _tmpDgQueryData; }
			set
			{
				Set(() => TmpDgQueryData, ref _tmpDgQueryData, value);
			}
		}
        #endregion

        #region 新商品數量
        private Int64 _NewQTY;
        public Int64 NewQTY
        {
            get { return _NewQTY; }
            set { Set(() => NewQTY, ref _NewQTY, value); }
        }
        #endregion 新商品數量

        public ICommand SaveCommand
		{
			get
			{
				return CreateBusyAsyncCommand(
					o =>
					{

					},
					() => true,
					o =>
					{
						if (DoValid())
						{
							if (DoSave())
							{
								CloseAction();

							}
						}
					}
					);
			}
		}
		public bool DoSave()
		{
			var proxy = GetWcfProxy<wcf.P18WcfServiceClient>();
			var tmpData = TmpDgQueryData.Clone();
			var updData = CombinF1913(tmpData);
			var wcfF1913 = ExDataMapper.Map<F1913, wcf.F1913>(updData);
			if (string.IsNullOrWhiteSpace(NewMakeNo))
				NewMakeNo = "";

			NewMakeNo.Trim();
            NewMakeNo = NewMakeNo.ToUpper();
            var result = proxy.RunWcfMethod(w => w.UpdateValidDateAndBatchNo(wcfF1913, NewValidDate, NewMakeNo, NewQTY));
            if (!result.IsSuccessed)
			{
                ShowResultMessage(result);
				return false;
			}
			else
			{
				if (TmpDgQueryData.VALID_DATE.CompareTo(NewValidDate) != 0 && (TmpDgQueryData.MAKE_NO ?? "").CompareTo(NewMakeNo) != 0)
				{
					ShowInfoMessage(Properties.Resources.P1801010100_ValidDateAndMakeNoAdjustSuccess);
				}
				else if(TmpDgQueryData.VALID_DATE.CompareTo(NewValidDate) != 0)
				{
					ShowInfoMessage(Properties.Resources.P1801010100_ValidDateAdjustSuccess);
				}
				else
				{
					ShowInfoMessage(Properties.Resources.P1801010100_MakeNoAdjustSuccess);
				}
				return true;
			}
		}
		private bool DoValid()
		{
      var CheckExistOrgF1913 = CombinF1913(TmpDgQueryData);
      var existOrgF1913 = GetF1913Data(CheckExistOrgF1913).FirstOrDefault();
      if (existOrgF1913 == null)
      {
        ShowWarningMessage(Properties.Resources.P1801010100_StockHasNotFound);
        return false;
      }
      if (string.IsNullOrWhiteSpace(NewMakeNo))
        NewMakeNo = "";

      NewMakeNo = NewMakeNo.Trim().ToUpper();

      if (TmpDgQueryData.VALID_DATE.CompareTo(NewValidDate) == 0 && (TmpDgQueryData.MAKE_NO ?? "").CompareTo(NewMakeNo) == 0 && TmpDgQueryData.QTY == NewQTY)
      {
        ShowInfoMessage(Properties.Resources.P1801010100_NotAdjValidDateAndBatchNo);
        return false;
      }
      else if (TmpDgQueryData.VALID_DATE.CompareTo(NewValidDate) == 0 && (TmpDgQueryData.MAKE_NO ?? "").CompareTo(NewMakeNo) == 0 && TmpDgQueryData.QTY != NewQTY)
      {
        ShowInfoMessage("必須至少變更批號或效期才可以調整數量");
        return false;
      }

      Regex re = new Regex("^[0-9|A-Z|a-z|_|-]+$");
      if (!re.IsMatch(NewMakeNo) && String.IsNullOrEmpty(NewMakeNo))
      {
        ShowInfoMessage("請輸入正確的批號");
        return false;
      }

      if (NewMakeNo == "0")
      {
        ShowInfoMessage("批號不可輸入0");
        return false;
      }

      var tmpData = TmpDgQueryData.Clone();
      tmpData.VALID_DATE = DateTime.Parse(NewValidDate.ToString("yyyy/MM/dd"));
      tmpData.MAKE_NO = NewMakeNo;
      var CheckExistNewF1913 = CombinF1913(tmpData);

      var existF1913 = GetF1913Data(CheckExistNewF1913).FirstOrDefault();
      if (existF1913 != null)
      {
        if (ShowConfirmMessage(Properties.Resources.P1801010100_ConfirmStockHasExists) == DialogResponse.No)
          return false;
        if (TmpDgQueryData.VALID_DATE.CompareTo(NewValidDate) != 0 && NewValidDate < DateTime.Today)
        {
          if (ShowConfirmMessage(Properties.Resources.P1801010100_VALID_DATESamllThanToday) == DialogResponse.No)
            return false;
        }
      }
      else
      {
        if (TmpDgQueryData.VALID_DATE.CompareTo(NewValidDate) != 0 && NewValidDate < DateTime.Today)
        {
          if (ShowConfirmMessage(Properties.Resources.P1801010100_VALID_DATESamllThanToday) == DialogResponse.No)
            return false;
        }
      }

      if (NewQTY < 1)
      {
        ShowInfoMessage("調整數量必須大於0");
        return false;
      }
      else if (NewQTY > TmpDgQueryData.QTY)
      {
        ShowInfoMessage($"不可超過原庫存數{TmpDgQueryData.QTY}");
        return false;
      }

      return true;
    }

    public F1913 CombinF1913(StockQueryData1 tmpData)
    {
      var tmpF1913 = new F1913()
      {
        DC_CODE = tmpData.DC_CODE,
        GUP_CODE = tmpData.GUP_CODE,
        CUST_CODE = tmpData.CUST_CODE,
        LOC_CODE = tmpData.LOC_CODE.Replace("-", ""),
        ITEM_CODE = tmpData.ITEM_CODE,
        VALID_DATE = DateTime.Parse(tmpData.VALID_DATE.ToString("yyyy/MM/dd")),
        ENTER_DATE = DateTime.Parse(tmpData.ENTER_DATE.ToString("yyyy/MM/dd")),
        BOX_CTRL_NO = string.IsNullOrWhiteSpace(tmpData.BOX_CTRL_NO) ? "0" : tmpData.BOX_CTRL_NO,
        PALLET_CTRL_NO = string.IsNullOrWhiteSpace(tmpData.PALLET_CTRL_NO) ? "0" : tmpData.PALLET_CTRL_NO,
        SERIAL_NO = string.IsNullOrWhiteSpace(tmpData.SERIAL_NO) ? "0" : tmpData.SERIAL_NO,
        MAKE_NO = string.IsNullOrWhiteSpace(tmpData.MAKE_NO) ? "0" : tmpData.MAKE_NO,
        VNR_CODE = "000000",  //F1913的這欄位固定是這個值，所以就直接指定，不然會有BUG
        QTY = tmpData.QTY
      };
      return tmpF1913;
    }

		public List<wcf.F1913> GetF1913Data(F1913 tmpF1913)
		{
			var proxy = GetWcfProxy<wcf.P18WcfServiceClient>();
			var wcfF1913 = ExDataMapper.Map<F1913, wcf.F1913>(tmpF1913);
			var tmp = proxy.RunWcfMethod(w => w.GetF1913Data(wcfF1913).ToList());
			return tmp;
		}
		public Action CloseAction = delegate { };
		public ICommand ExitCommand
		{
			get
			{

				return CreateBusyAsyncCommand(
					o =>
					{
					},
					() => true,
					o =>
					{
						if (ValidDate()) CloseAction();
						else return;
					}
					);
			}
		}
		public bool ValidDate()
		{
			if (string.IsNullOrWhiteSpace(NewMakeNo))
				NewMakeNo = "";

			NewMakeNo.Trim();
            if (TmpDgQueryData.VALID_DATE.CompareTo(NewValidDate) != 0 || (TmpDgQueryData.MAKE_NO ?? "").CompareTo(NewMakeNo) != 0 || TmpDgQueryData.QTY != NewQTY)
            {
				if (ShowConfirmMessage(Properties.Resources.P1801010100_ValidDateAndBathNoNotSaveYet) == DialogResponse.No)
					return false;
			}
			return true;
		}
	}
}