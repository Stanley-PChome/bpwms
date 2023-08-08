using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web.Security;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Security
{
  [ServiceContract(Namespace = Constants.SmartNameSpace, Name = "MembershipProvider")]

  public interface IMembershipProvider
  {
    string ProviderName { get; set; }

    [OperationContract]
    DateTime TestInput(DateTime date);

    [OperationContract]
    ProviderProperties GetProviderProperties();

    [OperationContract]
    MembershipUser GetUserByKey(object providerUserKey, bool userIsOnline);

    [OperationContract]
    MembershipCreateResult CreateUser(string username, string password,
                                      string email, string passwordQuestion,
                                      string passwordAnswer, bool isApproved,
                                      object providerUserKey);

    [OperationContract]
    MembershipCreateResult AddUser(MembershipUser user);

    [OperationContract]
    bool ChangePasswordQuestionAndAnswer(string username, string password,
                                         string newPasswordQuestion, string newPasswordAnswer);

    [OperationContract]
    string GetPassword(string username, string answer);

    [OperationContract]
    [FaultContract(typeof(FaultDetail))]
    bool ChangePassword(string username, string oldPassword, string newPassword);

    [OperationContract]
    string ResetPassword(string username, string answer);

    [OperationContract]
    void UpdateUser(MembershipUser user);

    [OperationContract]
    bool ValidateUser(string username, string password);

    [OperationContract]
    bool UnlockUser(string userName);

    [OperationContract]
    MembershipUser GetUserByName(string username, bool userIsOnline);

    [OperationContract]
    string GetUserNameByEmail(string email);

    [OperationContract]
    bool DeleteUser(string username, bool deleteAllRelatedData);

    [OperationContract]
    MembershipFindResult GetAllUsers(int pageIndex, int pageSize);

    [OperationContract]
    int GetNumberOfUsersOnline();

    [OperationContract]
    MembershipFindResult FindUsersByName(string usernameToMatch, int pageIndex,
                                         int pageSize);

    [OperationContract]
    MembershipFindResult FindUsersByEmail(string emailToMatch, int pageIndex,
                                          int pageSize);
  }

}
