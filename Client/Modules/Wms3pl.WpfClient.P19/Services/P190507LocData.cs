using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.P19.Services
{
	public class P190507LocData
	{
		public string DC_CODE { get; set; }
		/// <summary>
		/// 儲區型態(F1919)
		/// </summary>
		public string AREA_CODE { get; set; }

		private string _locCode;

		/// <summary>
		/// 儲位編號
		/// </summary>
		public string LOC_CODE
		{
			get { return _locCode; }
			set
			{
				_locCode = value;
			}
		}


		public string Floor
		{
			get
			{
				return LOC_CODE.Substring(0, 1);
			}
		}

		public string Channel
		{
			get
			{
				return LOC_CODE.Substring(1, 2);
			}
		}

		public string Plain
		{
			get
			{
				return LOC_CODE.Substring(3, 2);
			}
		}

		public string LocLevel
		{
			get
			{
				return LOC_CODE.Substring(5, 2);
			}
		}
	}
}
