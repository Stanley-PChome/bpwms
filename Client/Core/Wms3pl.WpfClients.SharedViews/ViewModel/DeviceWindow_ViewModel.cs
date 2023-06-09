using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.UILib;
using Wms3pl.WpfClient.DataServices.F91DataService;
using System.Collections.ObjectModel;
using Wms3pl.WpfClient.ExDataServices.P91ExDataService;
using Wms3pl.WpfClient.UILib.Services;
using Wms3pl.WpfClient.ExDataServices;
using wcf = Wms3pl.WpfClient.ExDataServices.P91WcfService;
using System.Text.RegularExpressions;
using Wms3pl.WpfClient.Common.Helpers;
using Wms3pl.WpfClient.VideoServer;

namespace Wms3pl.WpfClients.SharedViews.ViewModel
{
	public partial class DeviceWindow_ViewModel : InputViewModelBase
	{
		public DeviceWindow_ViewModel()
		{
			if (!IsInDesignMode)
			{
				
			}
		}
	}
}
