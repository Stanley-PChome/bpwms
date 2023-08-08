using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F07;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P16.Services
{
	public partial class P160201Service
	{
		private WmsTransaction _wmsTransaction;

		public P160201Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		F1908 GetF1908(string gupCode,string custCode, string vnrCode)
		{
			var f1909Repo = new F1909Repository(Schemas.CoreSchema);
			var f1909 = f1909Repo.Find(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode);
			var f1908Repo = new F1908Repository(Schemas.CoreSchema);
			var f1908 = f1908Repo.GetEnabledVnrData(gupCode, f1909.ALLOWGUP_VNRSHARE == "1" ? "0" : custCode, vnrCode);
			return f1908;
		}

		/// <summary>
		/// 建立廠退單
		/// </summary>
		/// <param name="addF160201"></param>
		/// <param name="addF160202s"></param>
		/// <returns></returns>
		public ExecuteResult InsertF160201(F160201 addF160201, F160202[] addF160202s)
		{
			var sharedService = new SharedService();
			var f160201Repo = new F160201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f160202Repo = new F160202Repository(Schemas.CoreSchema, _wmsTransaction);

			var f1908 = GetF1908(addF160201.GUP_CODE, addF160201.CUST_CODE, addF160201.VNR_CODE);
			if (f1908 == null)
				return new ExecuteResult(false, Properties.Resources.P160201Service_VNR_NotFound);

			//bool isDcAddress = (addF160201.SELF_TAKE == "1") ? false : sharedService.IsMatchAddressIsDCAddress(addF160201.DC_CODE, f1908.ADDRESS);
			//if (isDcAddress)
			//{
			//	addF160201.SELF_TAKE = "1";
			//}

			var newOrdCode = sharedService.GetNewOrdCode("V");

			addF160201.RTN_VNR_NO = newOrdCode;
			addF160201.STATUS = "0";
			f160201Repo.Add(addF160201);

			int seq = 0;
			foreach (var item in addF160202s)
			{
				seq++;
				item.RTN_VNR_NO = newOrdCode;
				item.RTN_VNR_SEQ = (short)seq;
				item.DC_CODE = addF160201.DC_CODE;
				item.GUP_CODE = addF160201.GUP_CODE;
				item.CUST_CODE = addF160201.CUST_CODE;

				f160202Repo.Add(item);
			}

			return new ExecuteResult() { IsSuccessed = true, Message = Properties.Resources.P160201Service_VNR_RTN_Inserted + "\n" + newOrdCode  };
		}

		///<summary>
		///編輯退貨單
		///</summary>
		///<param name="editF160201"></param>
		///<param name="editF160202s"></param>
		///<returns></returns>
		public ExecuteResult UpdateF160201(F160201 editF160201, F160202[] editF160202s)
		{
			var sharedService = new SharedService();
			var f160201Repo = new F160201Repository(Schemas.CoreSchema, _wmsTransaction);
			var f160202Repo = new F160202Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1951Repo = new F1951Repository(Schemas.CoreSchema, _wmsTransaction);

			var f160201 = f160201Repo.Find(item => item.RTN_VNR_NO == EntityFunctions.AsNonUnicode(editF160201.RTN_VNR_NO)
												&& item.DC_CODE == EntityFunctions.AsNonUnicode(editF160201.DC_CODE)
												&& item.GUP_CODE == EntityFunctions.AsNonUnicode(editF160201.GUP_CODE)
												&& item.CUST_CODE == EntityFunctions.AsNonUnicode(editF160201.CUST_CODE));

			var error = ValidateF160201(f160201);
			if (!String.IsNullOrEmpty(error))
				return new ExecuteResult { Message = error };


			if (f160201.STATUS == "0")
			{
				f160201.CUST_ORD_NO = editF160201.CUST_ORD_NO;
				f160201.ORD_PROP = editF160201.ORD_PROP;
				f160201.POSTING_DATE = editF160201.POSTING_DATE;
				f160201.COST_CENTER = editF160201.COST_CENTER;
				f160201.MEMO = editF160201.MEMO;
				f160201.RTN_VNR_CAUSE = editF160201.RTN_VNR_CAUSE;
				f160201.RTN_VNR_DATE = editF160201.RTN_VNR_DATE;
				f160201.RTN_VNR_TYPE_ID = editF160201.RTN_VNR_TYPE_ID;
				f160201.SELF_TAKE = editF160201.SELF_TAKE;
				f160201.VNR_CODE = editF160201.VNR_CODE;
        f160201.ADDRESS = editF160201.ADDRESS;
        f160201.ITEM_CONTACT = editF160201.ITEM_CONTACT;
        f160201.ITEM_TEL = editF160201.ITEM_TEL;
				f160201.DELIVERY_WAY = editF160201.DELIVERY_WAY;
				f160201.TYPE_ID = editF160201.TYPE_ID;

				var f1908 = GetF1908(f160201.GUP_CODE,f160201.CUST_CODE,f160201.VNR_CODE);
				if (f1908 == null)
					return new ExecuteResult(false, Properties.Resources.P160201Service_VNR_NotFound);

				//isDcAddress = (f160201.SELF_TAKE == "1") ? false : sharedService.IsMatchAddressIsDCAddress(f160201.DC_CODE, f1908.ADDRESS);
				//if (isDcAddress)
				//{
				//	f160201.SELF_TAKE = "1";
				//}

				f160202Repo.Delete(item => item.RTN_VNR_NO == editF160201.RTN_VNR_NO
												&& item.DC_CODE == editF160201.DC_CODE
												&& item.GUP_CODE == editF160201.GUP_CODE
												&& item.CUST_CODE == editF160201.CUST_CODE);

				int seq = 0;
				foreach (var item in editF160202s)
				{
					seq++;
					item.RTN_VNR_NO = f160201.RTN_VNR_NO;
					item.RTN_VNR_SEQ = (short)seq;
					item.DC_CODE = f160201.DC_CODE;
					item.GUP_CODE = f160201.GUP_CODE;
					item.CUST_CODE = f160201.CUST_CODE;
					f160202Repo.Add(item);
				}
			}

			f160201Repo.Update(f160201);

			return new ExecuteResult() { IsSuccessed = true, Message = Properties.Resources.P160201Service_VNR_NO_Edit };
		}

		private string ValidateF160201(F160201 f160201)
		{
			if (f160201 == null)
			{
				return Properties.Resources.P160201Service_VNR_NO_NotFound;
			}

			switch (f160201.STATUS)
			{
				case "9":
					return Properties.Resources.P160201Service_VNR_NO_Deleted;
				case "2":
					return Properties.Resources.P160201Service_VNR_NO_Close;
			}

			return string.Empty;
		}

		public ExecuteResult DelF075103s(string custCode,string custOrdCode)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var f075103Repo = new F075103Repository(Schemas.CoreSchema, _wmsTransaction);
			f075103Repo.DelF075103ByKey(custCode, custOrdCode);
			return result;
		}
	}
}
