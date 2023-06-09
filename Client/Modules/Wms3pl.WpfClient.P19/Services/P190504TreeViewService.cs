using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;

namespace Wms3pl.WpfClient.P19.Services
{
	public partial class P190504TreeViewService
	{
		private List<P190504TreeView> _all;

		public IEnumerable<P190504TreeView> MakeTree(IEnumerable<P190504TreeView> Items)
		{
			var levelVendors = new List<P190504TreeView>();

			var rootVendor = new P190504TreeView { Id = "0000000000", Code = "", Name = Properties.Resources.P1905040000_FuncList, Level = 0 };
			levelVendors.Add(rootVendor);

			DoMakeTree(Items, levelVendors, rootVendor, 1);

			return levelVendors.AsEnumerable();
		}

		private void DoMakeTree(IEnumerable<P190504TreeView> items, List<P190504TreeView> levelItems, P190504TreeView parent, int level)
		{
			var children = (from a in items
							where a.Code == parent.Id
							select a).ToList();

			foreach (var sub in children)
			{
				sub.Level = level;
				parent.TreeView.Add(sub);
				sub.Parent = parent;
				DoMakeTree(items, levelItems, sub, level + 1);
			}
		}

		public IEnumerable<P190504TreeView> AllItems(string functionCode)
		{
			F19Entities proxy = ConfigurationHelper.GetProxy<F19Entities>(false, functionCode);
			var q = from i in proxy.F1954s
					where i.DISABLE.Equals("0")
					// where i.CUST_CODE.Equals(custCode) && i.FUN_CODE.StartsWith("P", true, System.Globalization.CultureInfo.CurrentCulture)
					orderby i.FUN_CODE
					select new P190504TreeView
					{
						Id = i.FUN_CODE,
						Code = GetParentKey(i.FUN_CODE),
						Name = i.FUN_CODE + " - " + i.FUN_NAME
					};
			_all = q.ToList();
			proxy = null;
			return _all;
		}

		/// <summary>
		/// 取前8碼做為Parent Key
		/// </summary>
		/// <param name="funCode"></param>
		/// <returns></returns>
		private string GetParentKey(string funCode)
		{
			string result = string.Empty;
			//bool startWithP = funCode.StartsWith("P");
			string startChar = funCode.Substring(0, 1);
			if (funCode.StartsWith("B"))
			{
				result = funCode.Replace("B", "").Substring(0, 8);
				return result.PadRight(11, '0');
			}

			if (funCode.EndsWith("00000000")) return result.PadRight(10, '0');
			funCode = funCode.Replace(startChar, "");
			for (int i = 0; i < 10; i+=2)
			{
				string tmp = funCode.Substring(i, 2);
				if (tmp.Equals("00"))
				{
					if (i != 0) result = startChar + funCode.Substring(0, i - 2);
					break;
				}
			}
			return result.PadRight(11, '0');
		}
	}
}
