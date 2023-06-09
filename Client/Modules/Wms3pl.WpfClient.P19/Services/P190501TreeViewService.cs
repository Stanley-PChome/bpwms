using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;

namespace Wms3pl.WpfClient.P19.Services
{
	public partial class P190501TreeViewService
	{
		private List<P190501TreeView> _all;
		private List<P190501TreeView> _root;

		/// <summary>
		/// 製作樹目錄, 商業邏輯在此，不包含沒有貨主的 DC Root
		/// </summary>
		/// <param name="Items"></param>
		/// <returns></returns>
		public IEnumerable<P190501TreeView> MakeTree(IEnumerable<P190501TreeView> Items)
		{
			var levelVendors = new List<P190501TreeView>();

			foreach (var p in _root)
			{
				levelVendors.Add(p);
				DoMakeTree(Items, levelVendors, p, 1);
			}

			foreach (var node in _root)
			{
				if (!node.TreeView.Any())
				{
					levelVendors.Remove(node);
				}
			}

			return levelVendors.AsEnumerable();
		}

		private void DoMakeTree(IEnumerable<P190501TreeView> items, List<P190501TreeView> levelItems, P190501TreeView parent, int level)
		{
			var children = (from a in items
							where a.Code == parent.Id
							select a).OrderBy(x => x.Id).ToList();

			foreach (var sub in children)
			{
				sub.Level = level;
				parent.TreeView.Add(sub);
				sub.Parent = parent;
				DoMakeTree(items, levelItems, sub, level + 1);
			}
		}

		public IEnumerable<P190501TreeView> AllItems(string dcCode,string gupId, string functionCode)
		{
			// 取得ROOT資料
			_root = GenerateRoot(functionCode);

			// 取得底層資料
			var proxy = ConfigurationExHelper.GetExProxy<P19ExDataSource>(false, functionCode);
			var result = proxy.CreateQuery<F190101Data>("GetF190101MappingTable")
				.AddQueryOption("dcCode", string.Format("'{0}'", dcCode))
				.AddQueryOption("gupId", string.Format("'{0}'", gupId))
				.ToList();
			var levels = result.Select(x => new P190501TreeView()
			{
				Id = x.CUST_CODE,
				Code = x.DC_CODE,
				Name = x.CUST_CODE + " - " + x.CUST_NAME,
				Src = x
			});
			return levels;
		}

		private List<P190501TreeView> GenerateRoot(string functionCode)
		{
			var proxy = ConfigurationHelper.GetProxy<F19Entities>(false, functionCode);
			var tmpDc = proxy.F1901s.ToList();
			return tmpDc.Select(x => new P190501TreeView()
			{
				Id = x.DC_CODE,
				Code = string.Empty,
				Name = x.DC_CODE + " - " + x.DC_NAME,
				Src = null
			}).ToList();
		}
	}
}
