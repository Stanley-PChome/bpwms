using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.P71.Entities
{
	public class P710101MasterData
	{
		public string FloorNo { get; set; }
		public string ChannelNo { get; set; }
		public string PlainNo { get; set; }
		public string LocLevelNo { get; set; }
		public string LocTypeNo { get; set; }
		public int OldLocCount { get; set; }
		public int ChangeCount { get; set; }
		public int NowLocCount { get; set; }
		public string SettingStatus { get; set; }
	}

	public class P710102MasterData
	{
		public string Floor { get; set; }
		public string ChannelNo { get; set; }
		public string PlainNo { get; set; }
		public string LocLevelNo { get; set; }
		public string LocTypeNo { get; set; }
		public int OldLocCount { get; set; }
		public int ChangeCount { get; set; }
		public int NowLocCount { get; set; }
		public int TotelLocCount { get; set; }
		public string SettingStatus { get; set; }
	}
}
