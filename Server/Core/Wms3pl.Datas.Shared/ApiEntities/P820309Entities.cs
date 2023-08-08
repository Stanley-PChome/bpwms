using System.Collections.Generic;

namespace Wms3pl.Datas.Shared.ApiEntities
{
	/// <summary>
	/// 人員資訊同步
	/// </summary>
	public class PostUserDataReq
	{
		public string DcCode { get; set; }
		public string OwnerCode { get; set; }
		public int? UserTotal { get; set; }
		public List<PostUserDataResult> UserList { get; set; }
	}
	public class PostUserDataResult
	{
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string UserPw { get; set; }
		public int? Status { get; set; }
		public string UserType { get; set; }
		public string UserGroup { get; set; }
	}
	
}
