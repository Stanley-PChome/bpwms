using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.DataServices;
using Wms3pl.WpfClient.DataServices.F19DataService;
using Wms3pl.WpfClient.ExDataServices;
using Wms3pl.WpfClient.ExDataServices.P19ExDataService;
namespace Wms3pl.WpfClient.P19.Services
{
	public partial class P190507TreeViewService
	{
		private List<P190507TreeView> _all;
		private List<P190507TreeView> _root;
		private List<NameValuePair<string>> _dcList;

		public P190507TreeViewService(List<NameValuePair<string>> dcList)
		{
			_dcList = dcList;
		}


		/// <summary>
		/// 製作樹目錄, 商業邏輯在此
		/// </summary>
		/// <param name="Items"></param>
		/// <returns></returns>
		public IEnumerable<P190507TreeView> MakeTree(IEnumerable<P190507TreeView> Items)
		{
			var levelVendors = new List<P190507TreeView>();

			//var rootVendor = new P190507TreeView { Id = "", Code = "", Name = "儲位", Level = 0 };
			foreach (var p in _root)
			{
				levelVendors.Add(p);
				DoMakeTree(Items, levelVendors, p, 1);
			}

			return levelVendors.AsEnumerable();
		}

		private void DoMakeTree(IEnumerable<P190507TreeView> items, List<P190507TreeView> levelItems, P190507TreeView parent, int level)
		{
			var children = (from a in items
							where a.Code == parent.Id && a.DcCode == parent.DcCode
							select a).OrderBy(x => x.Id).ToList();

			foreach (var sub in children)
			{
				sub.Level = level;
				parent.TreeView.Add(sub);
				sub.Parent = parent;
				DoMakeTree(items, levelItems, sub, level + 1);
			}
		}

		/// <summary>
		/// 產生TreeView
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		public IEnumerable<P190507TreeView> AllItems(List<F1912> src)
		{
			// 取得ROOT資料
			_root = GenerateRoot(src).ToList();

			// 先取得階層資料
			var levels = GenerateLevels(src).ToList();

			// 取得底層資料
			var _all = from i in src
					   select new P190507TreeView()
					   {
						   Id = i.LOC_CODE,
						   Code = i.AREA_CODE.PadLeft(2, '0') + i.CHANNEL.PadLeft(2, '0') + i.PLAIN.PadLeft(2, '0') + i.LOC_LEVEL.PadLeft(2, '0'), // 父層ID
						   Name = i.LOC_CODE.Trim() + Properties.Resources.Loc_Code1,
						   Src = i,
						   DcCode = i.DC_CODE
					   };
			levels.AddRange(_all.ToList());
			return levels;
		}

		private IEnumerable<P190507TreeView> GenerateRoot(List<F1912> src)
		{
			var tmpDc = (from i in src
						   group i by i.DC_CODE into g
						   where g.Key != null
						   select new P190507TreeView()
						   {
							   Id = g.Key,
							   Code = string.Empty,
							   Name = _dcList.First(dc => dc.Value == g.Key).Name,
							   Src = null,
							   DcCode = g.Key
						   }).OrderBy(x => x.Id).ToList();
			return tmpDc;
		}

		/// <summary>
		/// 產生階層所需的資料
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		private IEnumerable<P190507TreeView> GenerateLevels(List<F1912> src)
		{
			var result = new List<P190507TreeView>();
			var tmpArea = (from i in src
						   group i by new { i.DC_CODE, i.AREA_CODE } into g
						   where g.Key != null
						   select new P190507TreeView()
							{
								Id = g.Key.AREA_CODE.PadLeft(2, '0'),
								Code = g.Key.DC_CODE,
								Name = g.Key.AREA_CODE + Properties.Resources.AREA_CODE,
								Src = null,
								DcCode = g.Key.DC_CODE
							}).ToList();
			var tmpChannel = (from i in src
							  group i by new { i.DC_CODE, i.AREA_CODE, i.CHANNEL } into g
							  where g.Key != null
							  select new P190507TreeView()
							  {
								  Id = g.Key.AREA_CODE.PadLeft(2, '0') + g.Key.CHANNEL.PadLeft(2, '0'),
								  Code = g.Key.AREA_CODE.PadLeft(2, '0'), // 父層ID
								  Name = g.Key.CHANNEL + Properties.Resources.CHANNEL1, // 顯示時不加上前置0, 保留原資料
								  Src = null,
								  DcCode = g.Key.DC_CODE
							  }).ToList();
			var tmpPlain = (from i in src
							group i by new { i.DC_CODE, i.AREA_CODE, i.CHANNEL, i.PLAIN } into g
							where g.Key != null
							select new P190507TreeView()
							{
								Id = g.Key.AREA_CODE.PadLeft(2, '0') + g.Key.CHANNEL.PadLeft(2, '0') + g.Key.PLAIN.PadLeft(2, '0'),
								Code = g.Key.AREA_CODE.PadLeft(2, '0') + g.Key.CHANNEL.PadLeft(2, '0'), // 父層ID
								Name = g.Key.PLAIN + Properties.Resources.PLAIN, // 顯示時不加上前置0, 保留原資料
								Src = null,
								DcCode = g.Key.DC_CODE
							}).ToList();
			var tmpLevel = (from i in src
							group i by new { i.DC_CODE, i.AREA_CODE, i.CHANNEL, i.PLAIN, i.LOC_LEVEL } into g
							where g.Key != null
							select new P190507TreeView()
							{
								Id = g.Key.AREA_CODE.PadLeft(2, '0') + g.Key.CHANNEL.PadLeft(2, '0') + g.Key.PLAIN.PadLeft(2, '0') + g.Key.LOC_LEVEL.PadLeft(2, '0'),
								Code = g.Key.AREA_CODE.PadLeft(2, '0') + g.Key.CHANNEL.PadLeft(2, '0') + g.Key.PLAIN.PadLeft(2, '0'), // 父層ID
								Name = g.Key.LOC_LEVEL + Properties.Resources.LOC_LEVEL, // 顯示時不加上前置0, 保留原資料
								Src = null,
								DcCode = g.Key.DC_CODE
							}).ToList();
			result.AddRange(tmpArea);
			result.AddRange(tmpChannel);
			result.AddRange(tmpPlain);
			result.AddRange(tmpLevel);
			return result;
		}
	}
}
