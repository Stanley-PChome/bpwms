using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Text;
using Wms3pl.WpfClient.Common;
using Wms3pl.WpfClient.Services;

namespace Wms3pl.WpfClient.P02.Services
{
    public partial class DelvNoService
    {
              
        private List<DelvNo> _allDelvNos;

				public IEnumerable<DelvNo> MakeTree(List<DelvNo> DelvNos)
        {
            var levelDelvNos = new List<DelvNo>();
            var currentDelvNo = "";

            var rootDelvNo = new DelvNo { Id = "D", Name ="所有", Level = 0 };
            var parentDelvNo = new DelvNo();
            levelDelvNos.Add(rootDelvNo);

            //對於每個 Function，找到 Parent
            foreach (var delvno in DelvNos)
            {
                var curentId = delvno.Id;
                if (curentId != currentDelvNo)
                {
                    currentDelvNo = curentId;
                    parentDelvNo = new DelvNo { Id = curentId, Name = delvno.Name, Level = 1 };
                    parentDelvNo.Parent = rootDelvNo;
                    if (!parentDelvNo.Parent.DelvNos.Contains(parentDelvNo))
                        parentDelvNo.Parent.DelvNos.Add(parentDelvNo);
                }
            }

            return levelDelvNos.AsEnumerable();
        }

				public List<DelvNo> AllDelvNos
        {
            get
            {                
                return _allDelvNos;
            }
        }    
    }
}
