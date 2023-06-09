using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wms3pl.WpfClient.Common
{
    public class NameValuePair<T>
    {
        public string Name { get; set; }
        public T Value { get; set; }

        public NameValuePair()
        {
        }

        public NameValuePair(string name, T value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
