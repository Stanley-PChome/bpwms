using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms3pl.WpfClient.P19.Services
{
	public class P190507LocNode : ObservableObject
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

		private string _name;

		public string Name
		{
			get { return _name; }
			set
			{
				Set(ref _name, value);
			}
		}

        private bool _isExpanded = false;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { Set(ref _isExpanded, value); }
        }


        public P190507LocNode Parent { get; private set; }

		public List<P190507LocNode> LocNodes { get; set; }

		public P190507LocData LocData { get; set; }

		public P190507LocNode(string name, P190507LocNode parent)
		{

			Name = name;
			Parent = parent;
			LocNodes = new List<P190507LocNode>();
		}
	}
}
