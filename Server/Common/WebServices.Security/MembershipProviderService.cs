using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Security;
using Wms3pl.WebServices.DataCommon;

namespace Wms3pl.WebServices.Security
{
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
  public partial class MembershipProviderService : IMembershipProvider
  {
    private System.Web.Security.MembershipProvider Provider
    {
      get
      {
        if (string.IsNullOrEmpty(ProviderName))
        {
          return Membership.Provider;
        }
        return Membership.Providers[ProviderName];
      }
    }

    #region IMembershipProvider Members

    public DateTime TestInput(DateTime date)
    {
      return date;
    }

    public string ProviderName { get; set; }

    public ProviderProperties GetProviderProperties()
    {
      var returnValue = new ProviderProperties
      {
        ApplicationName = Provider.ApplicationName,
        EnablePasswordReset = Provider.EnablePasswordReset,
        EnablePasswordRetrieval = Provider.EnablePasswordRetrieval,
        MaxInvalidPasswordAttempts = Provider.MaxInvalidPasswordAttempts,
        MinRequiredNonAlphanumericCharacters =
            Provider.MinRequiredNonAlphanumericCharacters,
        MinRequiredPasswordLength = Provider.MinRequiredPasswordLength,
        PasswordAttemptWindow = Provider.PasswordAttemptWindow,
        PasswordFormat = Provider.PasswordFormat,
        PasswordStrengthRegularExpression = Provider.PasswordStrengthRegularExpression,
        RequiresQuestionAndAnswer = Provider.RequiresQuestionAndAnswer,
        RequiresUniqueEmail = Provider.RequiresUniqueEmail
      };
      return returnValue;
    }

    public MembershipUser GetUserByKey(object providerUserKey, bool userIsOnline)
    {
      return Provider.GetUser(providerUserKey, userIsOnline);
    }

    public MembershipCreateResult CreateUser(string username, string password,
                                             string email, string passwordQuestion,
                                             string passwordAnswer, bool isApproved,
                                             object providerUserKey)
    {
      MembershipCreateStatus status;
      MembershipUser user = Provider.CreateUser(username, password, email, passwordQuestion, passwordAnswer,
                                                isApproved,
                                                providerUserKey, out status);
      return new MembershipCreateResult(user, status);
    }

    public MembershipCreateResult AddUser(MembershipUser user)
    {
      return new MembershipCreateResult(user, MembershipCreateStatus.DuplicateEmail);
    }

    public bool ChangePasswordQuestionAndAnswer(string username, string password,
                                                string newPasswordQuestion, string newPasswordAnswer)
    {
      return Provider.ChangePasswordQuestionAndAnswer(username, password, newPasswordQuestion, newPasswordAnswer);
    }

    public string GetPassword(string username, string answer)
    {
      return Provider.GetPassword(username, answer);
    }

    public bool ChangePassword(string username, string oldPassword, string newPassword)
    {
      try
      {
        return Provider.ChangePassword(username, oldPassword, newPassword);
      }
      catch (ArgumentException ex)
      {
        var detail = new FaultDetail() {Code= 100001, ExceptionMessage = ex.Message};
        throw new FaultException<FaultDetail>(detail, ex.Message,
          FaultCode.CreateSenderFaultCode("SenderFault", Constants.SmartNameSpace));
      }
      
    }

    public string ResetPassword(string username, string answer)
    {
      string newPassword = Provider.ResetPassword(username, answer);
      Utility.SendMail(answer, newPassword);
      return "";
    }

    public void UpdateUser(MembershipUser user)
    {
      Provider.UpdateUser(user);
    }

    public bool ValidateUser(string username, string password)
    {
      return Provider.ValidateUser(username, password);
    }

    public bool UnlockUser(string userName)
    {
      return Provider.UnlockUser(userName);
    }

    public MembershipUser GetUserByName(string username, bool userIsOnline)
    {
      return Provider.GetUser(username, userIsOnline);
    }

    public string GetUserNameByEmail(string email)
    {
      return Provider.GetUserNameByEmail(email);
    }

    public bool DeleteUser(string username, bool deleteAllRelatedData)
    {
      return Provider.DeleteUser(username, deleteAllRelatedData);
    }

    public MembershipFindResult GetAllUsers(int pageIndex, int pageSize)
    {
      int totalRecords;
      MembershipUserCollection users = Provider.GetAllUsers(pageIndex, pageSize, out totalRecords);
      var list = new List<MembershipUser>();
      foreach (MembershipUser user in users)
      {
        list.Add(user);
      }
      return new MembershipFindResult(list, totalRecords);
    }

    public int GetNumberOfUsersOnline()
    {
      return Provider.GetNumberOfUsersOnline();
    }

    public MembershipFindResult FindUsersByName(string usernameToMatch, int pageIndex,
                                                int pageSize)
    {
      int totalRecords;
      MembershipUserCollection users = Provider.FindUsersByName(usernameToMatch, pageIndex, pageSize,
                                                                out totalRecords);
      var list = new List<MembershipUser>();
      foreach (MembershipUser user in users)
      {
        list.Add(user);
      }
      return new MembershipFindResult(list, totalRecords);
    }

    public MembershipFindResult FindUsersByEmail(string emailToMatch, int pageIndex,
                                                 int pageSize)
    {
      int totalRecords;
      MembershipUserCollection users = Provider.FindUsersByEmail(emailToMatch, pageIndex, pageSize,
                                                                 out totalRecords);
      var list = new List<MembershipUser>();
      foreach (MembershipUser user in users)
      {
        list.Add(user);
      }
      return new MembershipFindResult(list, totalRecords);
    }

    #endregion
  } 

}