
using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Common.Helper;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F70;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P70.Services
{
	/// <summary>
	/// 進行派車管理的新增、編輯、匯入的檢核服務
	/// </summary>
	public partial class P70010401Service
	{
		private WmsTransaction _wmsTransaction;

		public List<DistrCarData> DistrCarDataList { get; private set; }
		public ConsignService ConsignService { get; private set; }

		public IEnumerable<DistrCarData> notErrorQuery { get { return DistrCarDataList.Where(x => !x.HasError); } }

		public P70010401Service(List<DistrCarData> distrCarDataList, WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
			DistrCarDataList = distrCarDataList;
			ConsignService = new ConsignService(_wmsTransaction);
		}


		/// <summary>
		/// 當派車單要編輯或刪除時，要先做的檢查
		/// </summary>
		/// <param name="f700101"></param>
		/// <returns></returns>
		public ExecuteResult CheckCanEditOrDelete(bool isEdit, string dcCode, params string[] distrCarNos)
		{
			var result = CheckIsPackaged(dcCode, distrCarNos);
			if (!result.IsSuccessed)
				return result;

			return CheckF700101Status(isEdit, dcCode, distrCarNos);
		}

		/// <summary>
		/// 檢查該派車單
		/// </summary>
		/// <param name="f700101"></param>
		/// <returns></returns>
		public ExecuteResult CheckIsPackaged(string dcCode, params string[] distrCarNos)
		{
			var f700101Repo = new F700101Repository(Schemas.CoreSchema, _wmsTransaction);
			var notCanEditWmsOrdNos = f700101Repo.GetPackagedWmsOrdNos(dcCode, distrCarNos);

			if (notCanEditWmsOrdNos.Any())
				return new ExecuteResult(false, Properties.Resources.P70010401Service_notCanEditWmsOrdNos + string.Join(Environment.NewLine, notCanEditWmsOrdNos));

			return new ExecuteResult(true);
		}

		/// <summary>
		/// 檢查派車單狀態
		/// </summary>
		/// <param name="f700101"></param>
		/// <returns></returns>
		public ExecuteResult CheckF700101Status(bool isEdit, string dcCode, params string[] distrCarNos)
		{
			var f700101Repo = new F700101Repository(Schemas.CoreSchema, _wmsTransaction);
			var dbF700101s = f700101Repo.InWithTrueAndCondition(x => x.DISTR_CAR_NO, distrCarNos.ToList(), x => x.DC_CODE == dcCode).ToList();

			if (dbF700101s.All(x => x.STATUS == "0"))
				return new ExecuteResult(true);

			// 處理中可編輯，但刪除必須是無單才可刪除
			if (dbF700101s.All(x => x.HAVE_WMS_NO == "0" && x.STATUS == "1")
				&& (isEdit || dbF700101s.All(x => x.HAVE_WMS_NO == "0")))
				return new ExecuteResult(true);

			return new ExecuteResult(false, Properties.Resources.P70010401Service_CantDeleteEditWmsOrdNosByEdi);
		}

		/// <summary>
		/// 將派車單號更新到該物流單的來源單號
		/// </summary>
		public void UpdateDistrCarNoByWmsNo()
		{
			// 將派車單號更新到有勾稽的物流單號
			foreach (var item in notErrorQuery.Where(x => x.F700101.HAVE_WMS_NO == "1"))
			{
				var results = item.F700102List.Select(f700102 => UpdateDistrCarNoByWmsNo(f700102, f700102.DISTR_CAR_NO, Properties.Resources.P70010401Service_Insert)).ToList();
				item.SetErrorMessage(results.Where(x => !x.IsSuccessed).Select(x => x.Message).FirstOrDefault());
			}
		}

		public ExecuteResult UpdateDistrCarNoByWmsNo(F700102 f700102, string distrCarNo, string statusName)
		{
			switch (f700102.ORD_TYPE)
			{
				case "S": //訂單
					var f050101Repo = new F050101Repository(Schemas.CoreSchema, _wmsTransaction);
					var f050101 = f050101Repo.Find(x => x.ORD_NO == f700102.WMS_NO && x.DC_CODE == f700102.DC_CODE && x.GUP_CODE == f700102.GUP_CODE && x.CUST_CODE == f700102.CUST_CODE);
					if (f050101 != null)
					{
						f050101.DISTR_CAR_NO = distrCarNo;
						f050101Repo.Update(f050101);
					}
					else
						return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.P70010401Service_DirstrNo + statusName + Properties.Resources.P70010401Service_ErrorWMS_NO + f700102.WMS_NO + Properties.Resources.P70010401Service_NotExist };
					break;
				case "O": //出貨單
					break;
				case "T"://調撥單
					break;
				case "D": //銷毀單
					var f160501Repo = new F160501Repository(Schemas.CoreSchema, _wmsTransaction);
					var f160501 = f160501Repo.Find(x => x.DESTROY_NO == f700102.WMS_NO && x.DC_CODE == f700102.DC_CODE && x.GUP_CODE == f700102.GUP_CODE && x.CUST_CODE == f700102.CUST_CODE);
					if (f160501 != null)
					{
						f160501.DISTR_CAR_NO = distrCarNo;
						f160501Repo.Update(f160501);
					}
					else
						return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.P70010401Service_DirstrNo + statusName + Properties.Resources.P70010401Service_Fail_Destroy + f700102.WMS_NO + Properties.Resources.P70010401Service_NotExist };
					break;
				case "R"://退貨單
					var f161201Repo = new F161201Repository(Schemas.CoreSchema, _wmsTransaction);
					var f161201 = f161201Repo.Find(x => x.RETURN_NO == f700102.WMS_NO && x.DC_CODE == f700102.DC_CODE && x.GUP_CODE == f700102.GUP_CODE && x.CUST_CODE == f700102.CUST_CODE);
					if (f161201 != null)
					{
						f161201.DISTR_CAR_NO = distrCarNo;
						f161201Repo.Update(f161201);
					}
					else
						return new ExecuteResult() { IsSuccessed = false, Message = Properties.Resources.P70010401Service_DirstrNo + statusName + Properties.Resources.P70010401Service_Fail_RetrnNo + f700102.WMS_NO + Properties.Resources.P70010401Service_NotExist };
					break;
			}
			return new ExecuteResult { IsSuccessed = true, Message = "" };

		}
	}
}

