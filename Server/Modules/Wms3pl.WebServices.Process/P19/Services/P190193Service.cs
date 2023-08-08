using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Process.P19.Services
{
	public partial class P190193Service
	{
		private WmsTransaction _wmsTransaction;
		public P190193Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		/// <summary>
		/// 新增集貨格類型
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="gupCode"></param>
		/// <param name="custCode"></param>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public ExecuteResult InsertOrUpdateF194501(string dcCode, string cellType, string cellName, int length, int depth, int heigth, string volumeRate, string typeMode)
		{
			var f194501Repo = new F194501Repository(Schemas.CoreSchema, _wmsTransaction);
			if (string.IsNullOrWhiteSpace(dcCode.Trim()))
			{
				return new ExecuteResult { IsSuccessed = false, Message = "請輸入物流中心" };
			}
			if (string.IsNullOrWhiteSpace(cellType.Trim()))
			{
				return new ExecuteResult { IsSuccessed = false, Message = "請輸入集貨格類型編號" };
			}
			if(!Regex.IsMatch(cellType.Trim(), @"^[A-Za-z0-9+-]+$"))
			{
				return new ExecuteResult { IsSuccessed = false, Message = "集貨格料架類型必須輸入英文或數字" };
			}
			if (cellType.Trim().Length>4)
			{
				return new ExecuteResult { IsSuccessed = false, Message = "集貨格料架類型必須小於4個字元" };
			}
			if (string.IsNullOrWhiteSpace(cellName.Trim()))
			{
				return new ExecuteResult { IsSuccessed = false, Message = "請輸入集貨格類型名稱" };
			}
			if (cellName.Trim().Length > 20)
			{
				return new ExecuteResult { IsSuccessed = false, Message = "集貨格料架名稱必須小於20個字元" };
			}
			if (length <= 0 || depth <= 0 || heigth <= 0)
			{
				return new ExecuteResult { IsSuccessed = false, Message = "長、寬、高及容積率都必需大於 0" };
			}
			if (!decimal.TryParse(volumeRate, out decimal defaultDeciaml))
			{
				return new ExecuteResult { IsSuccessed = false, Message = "容積率必須為數字" };
			}
			else
			{
				if (Convert.ToDecimal(volumeRate) > 100 || Convert.ToDecimal(volumeRate) <= 0)
				{
					return new ExecuteResult { IsSuccessed = false, Message = "容積率必須介於1到100之間" };
				}
			}

			// 驗證長寬高容積率乘積是否溢位
			checked
			{
				try
				{
					var maxVolume = (long)Math.Truncate(length * depth * heigth * Convert.ToDecimal(volumeRate) / 100);
				}
				catch (Exception)
				{
					return new ExecuteResult { IsSuccessed = false, Message = $"最大可放容量必須小於{long.MaxValue}" };
				}
			}

			var tmpVolumeRate = Convert.ToDecimal(volumeRate);
			var f194501 = f194501Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode
			&& x.CELL_TYPE == cellType).FirstOrDefault();
			if(typeMode == "Add")
			{
				if (f194501 != null)
				{
					return new ExecuteResult { IsSuccessed = false, Message = "集貨格類型編號已存在" };
				}

				f194501Repo.Add(new F194501
				{
					DC_CODE = dcCode,
					CELL_TYPE = cellType,
					CELL_NAME = cellName,
					LENGTH = length,
					DEPTH = depth,
					HEIGHT = heigth,
					VOLUME_RATE = tmpVolumeRate,
					MAX_VOLUME = (long)Math.Truncate(length * depth * heigth * tmpVolumeRate / 100)
				});
			}
			else if(typeMode == "Edit")
			{
				if (f194501 == null)
				{
					return new ExecuteResult { IsSuccessed = false, Message = "集貨格類型編號不存在" };
				}

				f194501.CELL_NAME = cellName;
				f194501.LENGTH = length;
				f194501.DEPTH = depth;
				f194501.HEIGHT = heigth;
				f194501.VOLUME_RATE = tmpVolumeRate;
				f194501.MAX_VOLUME = (long)Math.Truncate(length * depth * heigth * tmpVolumeRate / 100);
				f194501Repo.Update(f194501);
			}
			return new ExecuteResult(true);

		}

		/// <summary>
		/// 刪除集貨格類型
		/// </summary>
		/// <param name="dcCode"></param>
		/// <param name="cellType"></param>
		/// <returns></returns>
		public ExecuteResult DeleteF194501(string dcCode,string cellType)
		{
			var f1945Repo = new F1945Repository(Schemas.CoreSchema, _wmsTransaction);
			var f194501Repo = new F194501Repository(Schemas.CoreSchema, _wmsTransaction);
			var f1945 = f1945Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == dcCode && x.CELL_TYPE == cellType).FirstOrDefault();
			if (f1945 != null)
			{
				return new ExecuteResult { IsSuccessed = false, Message = "集貨格料架類型已被使用，不可刪除" };
			}
			
			f194501Repo.DeleteF194501(dcCode, cellType);

			return new ExecuteResult(true);
		}
	}
}
