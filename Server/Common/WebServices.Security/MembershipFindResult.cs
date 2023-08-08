using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Security;

namespace Wms3pl.WebServices.Security
{
  [DataContract]
  public class MembershipFindResult
  {
    [DataMember]
    public int RecordCount;
    [DataMember]
    public IEnumerable<MembershipUser> Users;

    public MembershipFindResult()
    {
    }

    public MembershipFindResult(IEnumerable<MembershipUser> users, int recordCount)
    {
      Users = users;
      RecordCount = recordCount;
    }
  } 

}