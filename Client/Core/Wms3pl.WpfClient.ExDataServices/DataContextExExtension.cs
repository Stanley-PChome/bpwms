using System;
using System.Data.Services.Client;
using System.Threading;
using System.Net;
using Wms3pl.WpfClient.DataServices.WcfDataServices;
using Wms3pl.WpfClient.Common.WcfDataServices;

namespace Wms3pl.WpfClient.ExDataServices.P01ExDataService
{
	#region P01
	public partial class P01ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
		public P01ExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
			: this(dataServiceUri)
		{
			this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}
namespace Wms3pl.WpfClient.ExDataServices.P02ExDataService
{
	#region P02
	public partial class P02ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
		public P02ExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
			: this(dataServiceUri)
		{
			this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
			  (DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}

namespace Wms3pl.WpfClient.ExDataServices.P05ExDataService
{
	#region P05
	public partial class P05ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
		public P05ExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
			: this(dataServiceUri)
		{
			this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}

namespace Wms3pl.WpfClient.ExDataServices.P06ExDataService
{
	#region P06
	public partial class P06ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
		public P06ExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
			: this(dataServiceUri)
		{
			this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}

namespace Wms3pl.WpfClient.ExDataServices.P08ExDataService
{
	#region P08
	public partial class P08ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
		public P08ExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
			: this(dataServiceUri)
		{
			this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}

namespace Wms3pl.WpfClient.ExDataServices.P14ExDataService
{
	#region P14
	public partial class P14ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
		public P14ExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
			: this(dataServiceUri)
		{
			this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}

namespace Wms3pl.WpfClient.ExDataServices.P15ExDataService
{
	#region P15
	public partial class P15ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
		public P15ExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
			: this(dataServiceUri)
		{
			this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}

namespace Wms3pl.WpfClient.ExDataServices.P16ExDataService
{
	#region P16
	public partial class P16ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
		public P16ExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
			: this(dataServiceUri)
		{
			this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}

namespace Wms3pl.WpfClient.ExDataServices.P18ExDataService
{
  #region P18
  public partial class P18ExDataSource : global::System.Data.Services.Client.DataServiceContext
  {
    public P18ExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
      : this(dataServiceUri)
    {
      this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
        (DataServiceContextEx.DataServiceContextEx_WritingEntity);
    }
  }
  #endregion
}


namespace Wms3pl.WpfClient.ExDataServices.P19ExDataService
{
	#region F19
	/// <summary>
	/// There are no comments for S1905ModelContainer in the schema.
	/// </summary>
	public partial class P19ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
		public P19ExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
			: this(dataServiceUri)
		{
			this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion F19
}

namespace Wms3pl.WpfClient.ExDataServices.P20ExDataService
{
	#region F20
	/// <summary>
	/// There are no comments for S1905ModelContainer in the schema.
	/// </summary>
	public partial class P20ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
		public P20ExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
			: this(dataServiceUri)
		{
			this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion F20
}

namespace Wms3pl.WpfClient.ExDataServices.P21ExDataService
{
	#region F21
	/// <summary>
	/// There are no comments for S1905ModelContainer in the schema.
	/// </summary>
	public partial class P21ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
		public P21ExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
			: this(dataServiceUri)
		{
			this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion F21
}

namespace Wms3pl.WpfClient.ExDataServices.P25ExDataService
{
	#region F25
	/// <summary>
	/// There are no comments for S1905ModelContainer in the schema.
	/// </summary>
	public partial class P25ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
		public P25ExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
			: this(dataServiceUri)
		{
			this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion F25
}

namespace Wms3pl.WpfClient.ExDataServices.P50ExDataService
{
	#region P50
	public partial class P50ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
		public P50ExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
			: this(dataServiceUri)
		{
			this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}

namespace Wms3pl.WpfClient.ExDataServices.P70ExDataService
{
	#region P70
	public partial class P70ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
		public P70ExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
			: this(dataServiceUri)
		{
			this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}

	#endregion
}

namespace Wms3pl.WpfClient.ExDataServices.P71ExDataService
{
	#region P71
	public partial class P71ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
		public P71ExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
			: this(dataServiceUri)
		{
			this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}

namespace Wms3pl.WpfClient.ExDataServices.P91ExDataService
{
	#region P91
	public partial class P91ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
		public P91ExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
			: this(dataServiceUri)
		{
			this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}

	#endregion
}



namespace Wms3pl.WpfClient.ExDataServices.ShareExDataService
{
	#region Share
	public partial class ShareExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
		public ShareExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
			: this(dataServiceUri)
		{
			this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}

	#endregion
}


namespace Wms3pl.WpfClient.ExDataServices.R01ExDataService
{
	#region R01
	public partial class R01ExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
		public R01ExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
			: this(dataServiceUri)
		{
			this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}

namespace Wms3pl.WpfClient.ExDataServices.SignalRExDataService
{
	#region SignalR
	public partial class SignalRExDataSource : global::System.Data.Services.Client.DataServiceContext
	{
		public SignalRExDataSource(Uri dataServiceUri, bool enableIgnoreProperties)
			: this(dataServiceUri)
		{
			this.WritingEntity += new EventHandler<ReadingWritingEntityEventArgs>
				(DataServiceContextEx.DataServiceContextEx_WritingEntity);
		}
	}
	#endregion
}