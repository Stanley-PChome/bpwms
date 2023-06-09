using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;

namespace Wms3pl.WpfClient.Common.WcfDataServices
{
    public class Wms3plBehaviorElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new Wms3plEndpointBehavior();
        }

        public override Type BehaviorType
        {
            get
            {
                return typeof(Wms3plEndpointBehavior);
            }
        }
    }
}
