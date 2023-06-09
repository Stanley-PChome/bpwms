
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.Datas.F19;
using Wms3pl.Datas.F50;
using Wms3pl.Datas.F91;
using Wms3pl.Datas.Shared.Entities;
using Wms3pl.WebServices.DataCommon;
using Wms3pl.WebServices.Process.P19.Services;

namespace Wms3pl.WebServices.Process.P91.Services
{
	public partial class P910501Service
	{
		private WmsTransaction _wmsTransaction;
		public P910501Service(WmsTransaction wmsTransaction = null)
		{
			_wmsTransaction = wmsTransaction;
		}

		public ExecuteResult AddOrUpdateUcDeviceSetting(DeviceData deviceData, WorkstationData workstationData)
		{
			var f1946Repo = new F1946Repository(Schemas.CoreSchema, _wmsTransaction);
			var f910501Repo = new F910501Repository(Schemas.CoreSchema, _wmsTransaction);
			
			var f910501 = f910501Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == deviceData.DC_CODE && x.DEVICE_IP == deviceData.DEVICE_IP).FirstOrDefault();
			if (workstationData != null)
			{
				var f1946 = f1946Repo.GetDatasByTrueAndCondition(x => x.DC_CODE == workstationData.DC_CODE && x.WORKSTATION_CODE == workstationData.WORKSTATION_CODE).FirstOrDefault();
				if (f1946 == null)
				{
					f1946Repo.Add(new F1946
					{
						DC_CODE = workstationData.DC_CODE,
						WORKSTATION_CODE = workstationData.WORKSTATION_CODE,
						WORKSTATION_TYPE = workstationData.WORKSTATION_TYPE,
						WORKSTATION_GROUP = workstationData.WORKSTATION_GROUP,
						STATUS = "0"
					});
				}
				else
				{
					f1946Repo.Update(workstationData);
				}
			}

			if (f910501 == null)
			{
				f910501Repo.Add(new F910501
				{
					DC_CODE = deviceData.DC_CODE,
					DEVICE_IP = deviceData.DEVICE_IP,
					LABELING = deviceData.LABELING,
					PRINTER = deviceData.PRINTER,
					VIDEO_ERROR = "1",
					MATRIX_PRINTER = deviceData.MATRIX_PRINTER,
					WEIGHING_ERROR = "1",
					NOTRUNAGV = "1",
					NOTCAPS = "1",
					WORKSTATION_CODE = deviceData.WORKSTATION_CODE,
					WORKSTATION_TYPE = deviceData.WORKSTATION_TYPE,
					WORKSTATION_GROUP = deviceData.WORKSTATION_GROUP,
					NO_SPEC_REPROTS = "0",
					CLOSE_BY_BOXNO = "0"
				});
			}
			else
			{
				f910501Repo.Update910501(deviceData);
			}
			return new ExecuteResult(true);
		}
	}
}

