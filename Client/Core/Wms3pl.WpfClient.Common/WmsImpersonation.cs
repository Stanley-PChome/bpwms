using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.Common
{
	public class WmsImpersonation
	{
		private static WindowsIdentity ImpersonateWms(out WindowsImpersonationContext wic)
		{
			wic = null;
			var globalInfo = Wms3plSession.Get<GlobalInfo>();
			var wi = WindowsImpersonation.GetWindowsIdentity(globalInfo.ImpersonationAccount, globalInfo.ImpersonationDomain,
					globalInfo.ImpersonationPassword);
			if (wi != null)
				wic = wi.Impersonate();
			return wi;
		}

		private static void DisposeWmsWi(WindowsIdentity wi, WindowsImpersonationContext wic)
		{
			if (wic != null)
			{
				wic.Undo();
				wic.Dispose();
			}
			if (wi != null)
				wi.Dispose();
		}


		public static T Run<T>(Func<T> func)
		{
			WindowsImpersonationContext wic;
			var wi = WmsImpersonation.ImpersonateWms(out wic);
			try
			{
				return func();
			}
			finally
			{
				WmsImpersonation.DisposeWmsWi(wi, wic);
			}
		}

		public static void Run(Action action)
		{
			WindowsImpersonationContext wic;
			var wi = WmsImpersonation.ImpersonateWms(out wic);
			try
			{
				action();
			}
			finally
			{
				WmsImpersonation.DisposeWmsWi(wi, wic);
			}
		}
	}
}
