using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.Datas.F16;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.Shared.Services;
using System.Data.Objects;

namespace Wms3pl.WebServices.Process.P16.Services
{
	public partial class P160503Service
	{
		private WmsTransaction _wmsTransaction;
		private F160502Repository _f160502Repo = new F160502Repository(Schemas.CoreSchema);
		public P160503Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public IQueryable<F160501FileData> GetDestoryNoFile(string destoryNo)
		{

			var repF160503 = new F160503Repository(Schemas.CoreSchema);
			return repF160503.GetDestoryNoFile(destoryNo);

		}

		public IQueryable<F160501FileData> GetDestoryNoRelation(string destoryNo)
		{

			var repF160503 = new F160503Repository(Schemas.CoreSchema);
			return repF160503.GetDestoryNoRelation(destoryNo);

		}

		public ExecuteResult UpdateF160503(List<F160501FileData> serialData, List<F160501FileData> fileData, List<F160501FileData> fileDeleteData)
		{
			var result = new ExecuteResult { IsSuccessed = true };
			var repF160503 = new F160503Repository(Schemas.CoreSchema, _wmsTransaction);
			var repF16050301 = new F16050301Repository(Schemas.CoreSchema, _wmsTransaction);
			bool firstRun = true;

			//原本是db 撈出來不變更
			var serialInsertData = serialData.Where(o => o.DB_Flag != "1").ToList();
			var fileInsertData = fileData.Where(o => o.DB_Flag != "1").ToList();

			foreach (var fileItem in fileInsertData)
			{
				foreach (var serialItem in serialInsertData)
				{
					fileItem.DC_CODE = serialItem.DC_CODE;
					fileItem.GUP_CODE = serialItem.GUP_CODE;
					fileItem.CUST_CODE = serialItem.CUST_CODE;

					F16050301 f16050301 = new F16050301();
					f16050301.DC_CODE = serialItem.DC_CODE;
					f16050301.GUP_CODE = serialItem.GUP_CODE;
					f16050301.CUST_CODE = serialItem.CUST_CODE;
					f16050301.DESTROY_NO = serialItem.DESTROY_NO;
					f16050301.UPLOAD_SEQ = fileItem.UPLOAD_SEQ;
					repF16050301.Add(f16050301);

					if (firstRun)
					{
						var f160501Repo = new F160501Repository(Schemas.CoreSchema, _wmsTransaction);
						f160501Repo.UpdateF160501Status(serialItem.DESTROY_NO, "4", serialItem.DC_CODE, serialItem.GUP_CODE, serialItem.CUST_CODE);
					}
			
				}

				F160503 f160503 = new F160503();
				f160503.DC_CODE = fileItem.DC_CODE;
				f160503.GUP_CODE = fileItem.GUP_CODE;
				f160503.CUST_CODE = fileItem.CUST_CODE;
				f160503.UPLOAD_SEQ = fileItem.UPLOAD_SEQ;
				f160503.UPLOAD_DESC = fileItem.UPLOAD_DESC;
				f160503.UPLOAD_C_PATH = fileItem.UPLOAD_C_PATH;
				f160503.UPLOAD_S_PATH = fileItem.UPLOAD_S_PATH;
				repF160503.Add(f160503);

				firstRun = false;
			}
			
			//刪除ViewModel 選擇刪除上傳檔案 (原 DB 資料)
			foreach (var itmes in fileDeleteData)
			{
				repF160503.DeleteF160503File(itmes.UPLOAD_SEQ);
				repF16050301.DeleteF16050301Serial(itmes.DESTROY_NO);
			}
			
			return result;

		}
	}
}
