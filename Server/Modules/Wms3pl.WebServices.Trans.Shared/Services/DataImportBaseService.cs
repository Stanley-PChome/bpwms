using System;
using System.Collections.Generic;
using System.Linq;
using Wms3pl.Datas.F00;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Shared.Services;
using Wms3pl.WebServices.Trans.Shared.Enum;
using Wms3pl.WebServices.Trans.Shared.Helper;

namespace Wms3pl.WebServices.Trans.Shared.Services
{
	public abstract class DataImportBaseService<T>
	{
		protected WmsTransaction _wmsTransaction;
		protected DataImportFile _dataImportFile;
		protected FileContentCutType _cutType;
		protected string _separate;
		protected List<F190010> _fileContentMapColumnList;
		protected List<T> _dataList;
		protected string _dcCode;
		protected string _gupCode;
		protected string _custCode;
		protected SharedService _sharedService;

		public DataImportBaseService(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
			_sharedService = new SharedService(wmsTransaction);
		}
		/// <summary>
		/// 檔案內容與欄位對應設定
		/// </summary>
		protected　virtual void SetFileContentMapColumnList()
		{
			var f190010Repo = new F190010Repository(Schemas.CoreSchema);
			_fileContentMapColumnList = f190010Repo.GetDatasByTrueAndCondition(x => x.FILE_KEYWORD == _dataImportFile.FileStartTag && x.FILE_EXTENSION == _dataImportFile.FileExtension).ToList();
		}

		/// <summary>
		/// 將原始資料寫入暫存資料表
		/// </summary>
		protected abstract void WriteOrginalDataToTempTable();

		/// <summary>
		/// 開始匯入資料
		/// </summary>
		protected abstract ExecuteResult StartImportDataToTable();

		/// <summary>
		/// 設定物件預設值
		/// </summary>
		/// <param name="obj"></param>
		protected virtual void SetDataDefaultValue(T obj)
		{

		}

		/// <summary>
		/// 將檔案內容轉成物件清單
		/// </summary>
		protected void ConvertFileContentToObject()
		{
			_dataList = new List<T>();
			foreach (var fileContent in _dataImportFile.FileContent)
			{
				var obj = Activator.CreateInstance<T>();
				switch (_cutType)
				{
					case FileContentCutType.ByInterval:
						obj = FileContentCutHelper.ConvertFileContentByIntervalToObject<T>(_fileContentMapColumnList, fileContent);
						break;
					case FileContentCutType.BySeparate:
						obj = FileContentCutHelper.ConvertFileContentByBySeparateToObject<T>(_fileContentMapColumnList, _separate.ToArray(), fileContent);
						break;
				}
				if(obj!=null)
				{
					SetDataDefaultValue(obj);
					_dataList.Add(obj);
				}
					
			}
		}

		/// <summary>
		/// 執行匯入
		/// </summary>
		/// <param name="cutType"></param>
		/// <param name="separate"></param>
		/// <returns></returns>
		public ExecuteResult ExecuteImport(string dcCode,string gupCode,string custCode, DataImportFile dataImportFile,FileContentCutType cutType, string separate =",")
		{
			_dcCode = dcCode;
			_gupCode = gupCode;
			_custCode = custCode;
			_cutType = cutType;
			_separate = separate;
			_dataImportFile = dataImportFile;
			SetFileContentMapColumnList();
			//檔案內容轉物件
			ConvertFileContentToObject();
			//原始資料物件寫入資料庫
			WriteOrginalDataToTempTable();
			//開始執行匯入
			var result =  StartImportDataToTable();
			return result;
		}
	}
}
