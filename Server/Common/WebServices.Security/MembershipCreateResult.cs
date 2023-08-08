using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Security;

namespace Wms3pl.WebServices.Security
{
  [DataContract]
  public class MembershipCreateResult
  {
    [DataMember]
    public MembershipCreateStatus CreateStatus;
    [DataMember]
    public MembershipUser User;

    public MembershipCreateResult()
    {
    }

    public MembershipCreateResult(MembershipUser user, MembershipCreateStatus createStatus)
    {
      User = user;
      CreateStatus = createStatus;
    }
  } 

}