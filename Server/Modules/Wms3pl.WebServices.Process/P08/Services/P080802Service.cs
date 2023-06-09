using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F05;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.ServiceEntites;
using Wms3pl.WebServices.Shared.Services;

namespace Wms3pl.WebServices.Process.P08.Services
{
	public partial class P080802Service
	{
		private WmsTransaction _wmsTransaction;
		public P080802Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult SetWmsOrdAudited(string wmsOrdNo, string gupCode, string custCode, string dcCode, string pastNo)
		{
			var f055001Rep = new F055001Repository(Schemas.CoreSchema, _wmsTransaction);
			var count = f055001Rep.GetCountByNotStatus(wmsOrdNo, gupCode, custCode, dcCode, "1");
			if (count > 0)
				return new ExecuteResult { IsSuccessed = true, Message = Properties.Resources.P080802Service_F055001StatusNotOne };
			var f050801Rep = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050801 = f050801Rep.Find(a => a.GUP_CODE == gupCode && a.CUST_CODE == custCode && a.DC_CODE == dcCode && a.WMS_ORD_NO == wmsOrdNo);
			if (f050801 == null)
				return new ExecuteResult { IsSuccessed = false, Message = Properties.Resources.P080802Service_F050801NotExist };
			var f1909Repo = new F1909Repository(Schemas.CoreSchema);
			var f1909 = f1909Repo.GetDatasByTrueAndCondition(x => x.GUP_CODE == gupCode && x.CUST_CODE == custCode).FirstOrDefault();
			if(f1909!=null && f1909.ISDELV_LOADING_CHECKCODE == "1" && string.IsNullOrWhiteSpace(f050801.DELV_CHECKCODE))
			{
				var msgType = "11";//裝車稽核
				var sourceType = string.IsNullOrEmpty(f050801.SOURCE_TYPE) ? "00" : f050801.SOURCE_TYPE;
				var f05500102Repo = new F05500102Repository(Schemas.CoreSchema);
				var f05500102 = f05500102Repo.Find(x => x.SOURCE_TYPE == sourceType && x.MSG_TYPE == msgType);
				if (f05500102 != null)
					return new ExecuteResult(false, f05500102.MESSAGE);
				else
					return new ExecuteResult(false, Properties.Resources.P080802Service_F05500102IsNull);
			}
			f050801.STATUS = 2;
			f050801Rep.Update(f050801);

			var data = f055001Rep.AsForUpdate()
				   .GetDatasByTrueAndCondition(
					   o => o.DC_CODE == dcCode && o.GUP_CODE == gupCode && o.CUST_CODE == custCode && o.WMS_ORD_NO == wmsOrdNo);
			foreach (var f055001 in data)
			{
				f055001.AUDIT_DATE = DateTime.Now;
				f055001.AUDIT_STAFF = Current.Staff;
				f055001.AUDIT_NAME = Current.StaffName;
				f055001Rep.Update(f055001);
			}

			var sharedService = new SharedService(_wmsTransaction);
			var shareResult = sharedService.UpdateSourceNoStatus(SourceType.Order, dcCode, gupCode, custCode, wmsOrdNo, f050801.STATUS.ToString());


			return new ExecuteResult { IsSuccessed = true };
		}

		public ExecuteResult UploadDelvCheckCode(string dcCode,string gupCode,string custCode,List<UploadDelvCheckCode> delvCheckCodeList)
		{
			var consignNos = delvCheckCodeList.Where(x=>!string.IsNullOrWhiteSpace(x.CONSIGN_NO)).Select(x => x.CONSIGN_NO).ToList();
			var f050901Repo = new F050901Repository(Schemas.CoreSchema);
			var f050901s = f050901Repo.GetDatasByConsignNo(dcCode, gupCode, custCode, consignNos);
			var f050801Repo = new F050801Repository(Schemas.CoreSchema, _wmsTransaction);
			var f050801s = f050801Repo.AsForUpdate().GetDatas(dcCode, gupCode, custCode, f050901s.Select(x => x.WMS_NO).ToList());
			var updF050801s = new List<F050801>();
			var messageList = new List<string>();
			var rowIndex = 1;
			foreach(var delvCheckCode in delvCheckCodeList)
			{
				var f050901 = f050901s.FirstOrDefault(x => x.CONSIGN_NO == delvCheckCode.CONSIGN_NO);
				if (f050901 == null)
				{
					messageList.Add(string.Format(Properties.Resources.P080802Service_F050901IsNull, rowIndex));
					rowIndex++;
					continue;
				}
				var f050801 = f050801s.FirstOrDefault(x => x.WMS_ORD_NO == f050901.WMS_NO);
				if(f050801 == null)
				{
					messageList.Add(string.Format(Properties.Resources.P080802Service_F050801IsNull, rowIndex));
					rowIndex++;
					continue;
				}
				f050801.DELV_CHECKCODE = delvCheckCode.DELV_CHECKCODE;
				updF050801s.Add(f050801);
				rowIndex++;
			}
			if(updF050801s.Any())
				f050801Repo.BulkUpdate(updF050801s);

			return new ExecuteResult { IsSuccessed = !messageList.Any(),Message = string.Join(Environment.NewLine,messageList) };
		}
	}
}

