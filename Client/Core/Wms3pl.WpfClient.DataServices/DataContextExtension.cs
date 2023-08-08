using System;
using System.Data.Services.Client;
using System.Threading;
using System.Net;
using Wms3pl.WpfClient.DataServices.WcfDataServices;
using Wms3pl.WpfClient.Common.WcfDataServices;

namespace Wms3pl.WpfClient.DataServices.F00DataService
{
	#region F00
	/// <summary>
	/// There are no comments for S1905ModelContainer in the schema.
	/// </summary>
	public partial class F00Entities : Wms3plDbContext
	{
		public F00Entities(Uri dataServiceUri, bool enableIgnoreProperties)
			: base(dataServiceUri)
		{
			if (enableIgnoreProperties)
				this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
					(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
    #endregion
}

namespace Wms3pl.WpfClient.DataServices.F01DataService
{
	#region F01
	/// <summary>
	/// There are no comments for S1905ModelContainer in the schema.
	/// </summary>
	public partial class F01Entities : Wms3plDbContext
    {
		public F01Entities(Uri dataServiceUri, bool enableIgnoreProperties)
			: base(dataServiceUri)
		{
			if (enableIgnoreProperties)
				this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
					(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}
namespace Wms3pl.WpfClient.DataServices.F02DataService
{
	#region F02
	/// <summary>
	/// There are no comments for S1905ModelContainer in the schema.
	/// </summary>
	public partial class F02Entities : Wms3plDbContext
    {
        public F02Entities(Uri dataServiceUri, bool enableIgnoreProperties)
            : base(dataServiceUri)
        {
			if (enableIgnoreProperties)
				this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				  (DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}

namespace Wms3pl.WpfClient.DataServices.F05DataService
{
	#region F05
	/// <summary>
	/// There are no comments for S1905ModelContainer in the schema.
	/// </summary>
	public partial class F05Entities : Wms3plDbContext
    {
        public F05Entities(Uri dataServiceUri, bool enableIgnoreProperties)
            : base(dataServiceUri)
        {
			if (enableIgnoreProperties)
				this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
					(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}

namespace Wms3pl.WpfClient.DataServices.F06DataService
{
    #region F06
    public partial class F06Entities : Wms3plDbContext
    {
        public F06Entities(Uri dataServiceUri, bool enableIgnoreProperties)
            : base(dataServiceUri)
        {
            if (enableIgnoreProperties)
                this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
                    (DataServiceContextEx.DataServiceContextEx_WritingEntity);
        }
    }
    #endregion
}

namespace Wms3pl.WpfClient.DataServices.F14DataService
{
	#region F14
	/// <summary>
	/// There are no comments for S1905ModelContainer in the schema.
	/// </summary>
	public partial class F14Entities : Wms3plDbContext
    {
        public F14Entities(Uri dataServiceUri, bool enableIgnoreProperties)
            : base(dataServiceUri)
        {
			if (enableIgnoreProperties)
				this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
					(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}

namespace Wms3pl.WpfClient.DataServices.F15DataService
{
	#region F15
	/// <summary>
	/// There are no comments for S1905ModelContainer in the schema.
	/// </summary>
	public partial class F15Entities : Wms3plDbContext
    {
        public F15Entities(Uri dataServiceUri, bool enableIgnoreProperties)
            : base(dataServiceUri)
        {
			if (enableIgnoreProperties)
				this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
					(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}

namespace Wms3pl.WpfClient.DataServices.F16DataService
{
	#region F16
	/// <summary>
	/// There are no comments for S1905ModelContainer in the schema.
	/// </summary>
	public partial class F16Entities : Wms3plDbContext
    {
        public F16Entities(Uri dataServiceUri, bool enableIgnoreProperties)
            : base(dataServiceUri)
        {
			if (enableIgnoreProperties)
				this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
					(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}

namespace Wms3pl.WpfClient.DataServices.F19DataService
{
	#region F19
	/// <summary>
	/// There are no comments for S1905ModelContainer in the schema.
	/// </summary>
	public partial class F19Entities : Wms3plDbContext
    {
        public F19Entities(Uri dataServiceUri, bool enableIgnoreProperties)
            : base(dataServiceUri)
        {
			if (enableIgnoreProperties)
				this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
					(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion F19
}

namespace Wms3pl.WpfClient.DataServices.F20DataService
{
	#region F20
	/// <summary>
	/// There are no comments for S1905ModelContainer in the schema.
	/// </summary>
	public partial class F20Entities : Wms3plDbContext
    {
        public F20Entities(Uri dataServiceUri, bool enableIgnoreProperties)
            : base(dataServiceUri)
        {
			if (enableIgnoreProperties)
				this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
					(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion F20
}

namespace Wms3pl.WpfClient.DataServices.F25DataService
{
	#region F25
	/// <summary>
	/// There are no comments for S1905ModelContainer in the schema.
	/// </summary>
	public partial class F25Entities : Wms3plDbContext
    {
        public F25Entities(Uri dataServiceUri, bool enableIgnoreProperties)
            : base(dataServiceUri)
        {
			if (enableIgnoreProperties)
				this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				  (DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}

namespace Wms3pl.WpfClient.DataServices.F70DataService
{
	#region F70
	/// <summary>
	/// There are no comments for S1905ModelContainer in the schema.
	/// </summary>
	public partial class F70Entities : Wms3plDbContext
    {
        public F70Entities(Uri dataServiceUri, bool enableIgnoreProperties)
            : base(dataServiceUri)
        {
			if (enableIgnoreProperties)
				this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
					(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}

namespace Wms3pl.WpfClient.DataServices.F91DataService
{
	#region F91
	/// <summary>
	/// There are no comments for S1905ModelContainer in the schema.
	/// </summary>
	public partial class F91Entities : Wms3plDbContext
    {
        public F91Entities(Uri dataServiceUri, bool enableIgnoreProperties)
            : base(dataServiceUri)
        {
			if (enableIgnoreProperties)
				this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				  (DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}

namespace Wms3pl.WpfClient.DataServices.F50DataService
{
	#region F50
	/// <summary>
	/// There are no comments for S1905ModelContainer in the schema.
	/// </summary>
	public partial class F50Entities : Wms3plDbContext
    {
        public F50Entities(Uri dataServiceUri, bool enableIgnoreProperties)
            : base(dataServiceUri)
        {
			if (enableIgnoreProperties)
				this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
					(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}

