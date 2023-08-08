using System.Runtime.Serialization;
using System.Web.Security;

namespace Wms3pl.WebServices.Security
{
  [DataContract]
  public class ProviderProperties
  {
    public ProviderProperties()
    {
    }

    public ProviderProperties(bool enablePasswordRetrieval, bool enablePasswordReset, bool requiresQuestionAndAnswer,
                              int maxInvalidPasswordAttempts,
                              int passwordAttemptWindow, bool requiresUniqueEmail,
                              MembershipPasswordFormat passwordFormat, int minRequiredPasswordLength,
                              int minRequiredNonAlphanumericCharacters,
                              string passwordStrengthRegularExpression, string applicationName)
    {
      EnablePasswordRetrieval = enablePasswordRetrieval;
      EnablePasswordReset = enablePasswordReset;
      RequiresQuestionAndAnswer = requiresQuestionAndAnswer;
      MaxInvalidPasswordAttempts = maxInvalidPasswordAttempts;
      PasswordAttemptWindow = passwordAttemptWindow;
      RequiresUniqueEmail = requiresUniqueEmail;
      PasswordFormat = passwordFormat;
      MinRequiredPasswordLength = minRequiredPasswordLength;
      MinRequiredNonAlphanumericCharacters = minRequiredNonAlphanumericCharacters;
      PasswordStrengthRegularExpression = passwordStrengthRegularExpression;
      ApplicationName = applicationName;
    }

    [DataMember]
    public bool EnablePasswordRetrieval { get; set; }

    [DataMember]
    public bool EnablePasswordReset { get; set; }

    [DataMember]
    public bool RequiresQuestionAndAnswer { get; set; }

    [DataMember]
    public int MaxInvalidPasswordAttempts { get; set; }

    [DataMember]
    public int PasswordAttemptWindow { get; set; }

    [DataMember]
    public bool RequiresUniqueEmail { get; set; }

    [DataMember]
    public MembershipPasswordFormat PasswordFormat { get; set; }

    [DataMember]
    public int MinRequiredPasswordLength { get; set; }

    [DataMember]
    public int MinRequiredNonAlphanumericCharacters { get; set; }

    [DataMember]
    public string PasswordStrengthRegularExpression { get; set; }

    [DataMember]
    public string ApplicationName { get; set; }
  } 

}