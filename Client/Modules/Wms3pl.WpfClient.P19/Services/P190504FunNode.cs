using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms3pl.WpfClient.DataServices.F19DataService;

namespace Wms3pl.WpfClient.P19.Services
{
	public class P190504FunNode : ObservableObject
	{
		/// <summary>
		///  
		/// </summary>
		private bool _isChecked;

		/// <summary>
		///  
		/// </summary>
		public bool IsChecked
		{
			get
			{
				return _isChecked;
			}
			set
			{
				Set(() => IsChecked, ref _isChecked, value);
			}
		}

		private bool _isExpend;
		public bool IsExpend
		{
			get { return _isExpend; }
			set
			{
				Set(() => IsExpend, ref _isExpend, value);
			}
		}
		public int Level { get; set; }

		public string Name { get; set; }

		public P190504FunNode Parent { get; private set; }

		public List<P190504FunNode> FunNodes { get; set; }

		public F1954 F1954Data { get; set; }

		private string _moduleId = null;
		public string ModuleId
		{
			get
			{
				if (_moduleId == null && Level > 0)
					_moduleId = F1954Data.FUN_CODE.Substring(0,1 + Level * 2);

				return _moduleId;
			}
		}

		public P190504FunNode(string name, P190504FunNode parent, int level)
		{
			Name = name;
			Parent = parent;
			FunNodes = new List<P190504FunNode>();
			Level = level;
		}

		public P190504FunNode(F1954 f1954, bool isChecked, P190504FunNode parent,int level)
			: this(string.Format("{0} - {1}", f1954.FUN_CODE, f1954.FUN_NAME), parent,level)
		{
			IsChecked = isChecked;
			F1954Data = f1954;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
