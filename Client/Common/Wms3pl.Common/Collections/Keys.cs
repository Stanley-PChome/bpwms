using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.Common.Collections
{
	public struct Keys<TKey1, TKey2, TKey3>
	{
		public TKey1 Key1;
		public TKey2 Key2;
		public TKey3 Key3;

		public Keys(TKey1 key1, TKey2 key2, TKey3 key3)
			: this()
		{
			Key1 = key1;
			Key2 = key2;
			Key3 = key3;
		}
	}
}
