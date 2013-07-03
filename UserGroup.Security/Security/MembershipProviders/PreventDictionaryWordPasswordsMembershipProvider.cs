using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.Security;

using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Security;

using UserGroup.Security.Utilities.StringUtilities;
using UserGroup.Security.Utilities.StringUtilities.Base;

namespace UserGroup.Security.Security.MembershipProviders
{
    public class PreventDictionaryWordPasswordsMembershipProvider : MembershipProvider 
    {
        private static IEnumerable<string> _DictionaryWords;
        private static IEnumerable<string> DictionaryWords
        {
            get
            {
                if (_DictionaryWords == null)
                {
                    _DictionaryWords = GetDictionaryWords();
                }

                return _DictionaryWords;
            }
        }

        public override string Name
        {
            get
            {
                return InnerMembershipProvider.Name;
            }
        }

        public override string ApplicationName
        {
            get
            {
                return InnerMembershipProvider.ApplicationName;
            }
            set
            {
                InnerMembershipProvider.ApplicationName = value;
            }
        }

        public override bool EnablePasswordReset
        {
            get
            {
                return InnerMembershipProvider.EnablePasswordReset;
            }
        }

        public override bool EnablePasswordRetrieval
        {
            get
            {
                return InnerMembershipProvider.EnablePasswordRetrieval;
            }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                return InnerMembershipProvider.MaxInvalidPasswordAttempts;
            }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get
            {
                return InnerMembershipProvider.MinRequiredNonAlphanumericCharacters;
            }
        }

        public override int MinRequiredPasswordLength
        {
            get
            {
                return InnerMembershipProvider.MinRequiredPasswordLength;
            }
        }

        public override int PasswordAttemptWindow
        {
            get
            {
                return InnerMembershipProvider.PasswordAttemptWindow;
            }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get
            {
                return InnerMembershipProvider.PasswordFormat;
            }
        }

        public override string PasswordStrengthRegularExpression
        {
            get
            {
                return InnerMembershipProvider.PasswordStrengthRegularExpression;
            }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                return InnerMembershipProvider.RequiresQuestionAndAnswer;
            }
        }

        public override bool RequiresUniqueEmail
        {
            get
            {
                return InnerMembershipProvider.RequiresUniqueEmail;
            }
        }

        private MembershipProvider InnerMembershipProvider { get; set; }
        private ISubstringsChecker<string> DictionaryWordsSubstringsChecker { get; set; }

        public PreventDictionaryWordPasswordsMembershipProvider()
            : this(CreateNewSqlMembershipProvider(), CreateNewDictionaryWordsSubstringsChecker())
        {
        }
        
        public PreventDictionaryWordPasswordsMembershipProvider(MembershipProvider innerMembershipProvider, ISubstringsChecker<string> dictionaryWordsSubstringsChecker)
        {
            SetInnerMembershipProvider(innerMembershipProvider);
            SetDictionaryWordsSubstringsChecker(dictionaryWordsSubstringsChecker);
        }

        private void SetInnerMembershipProvider(MembershipProvider innerMembershipProvider)
        {
            Assert.ArgumentNotNull(innerMembershipProvider, "innerMembershipProvider");
            InnerMembershipProvider = innerMembershipProvider;
        }

        private void SetDictionaryWordsSubstringsChecker(ISubstringsChecker<string> dictionaryWordsSubstringsChecker)
        {
            Assert.ArgumentNotNull(dictionaryWordsSubstringsChecker, "dictionaryWordsSubstringsChecker");
            DictionaryWordsSubstringsChecker = dictionaryWordsSubstringsChecker;
        }

        private static MembershipProvider CreateNewSqlMembershipProvider()
        {
            return new SqlMembershipProvider();
        }

        private static ISubstringsChecker<string> CreateNewDictionaryWordsSubstringsChecker()
        {
            return CreateNewStringSubstringsChecker(DictionaryWords);
        }

        private static ISubstringsChecker<string> CreateNewStringSubstringsChecker(IEnumerable<string> substrings)
        {
            Assert.ArgumentNotNull(substrings, "substrings");
            const bool ignoreCase = true;
            return StringSubstringsChecker.CreateNewStringSubstringsContainer(substrings, ignoreCase);
        }

        private static IEnumerable<string> GetDictionaryWords()
        {
            return Factory.GetStringSet("dictionaryWords/word");
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (DoesPasswordContainDictionaryWord(newPassword))
            {
                return false;
            }

            return InnerMembershipProvider.ChangePassword(username, oldPassword, newPassword);
        }

        private bool DoesPasswordContainDictionaryWord(string password)
        {
            Assert.ArgumentNotNullOrEmpty(password, "password");
            DictionaryWordsSubstringsChecker.Source = password;
            return DictionaryWordsSubstringsChecker.ContainsSubstrings();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            return InnerMembershipProvider.ChangePasswordQuestionAndAnswer(username, password, newPasswordQuestion, newPasswordAnswer);
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            if (DoesPasswordContainDictionaryWord(password))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            return InnerMembershipProvider.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
        }

        public override bool DeleteUser(string userName, bool deleteAllRelatedData)
        {
            return InnerMembershipProvider.DeleteUser(userName, deleteAllRelatedData);
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return InnerMembershipProvider.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection FindUsersByName(string userNameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return InnerMembershipProvider.FindUsersByName(userNameToMatch, pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            return InnerMembershipProvider.GetAllUsers(pageIndex, pageSize, out totalRecords);
        }

        public override int GetNumberOfUsersOnline()
        {
            return InnerMembershipProvider.GetNumberOfUsersOnline();
        }

        public override string GetPassword(string username, string answer)
        {
            return InnerMembershipProvider.GetPassword(username, answer);
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            return InnerMembershipProvider.GetUser(providerUserKey, userIsOnline);
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            return InnerMembershipProvider.GetUser(username, userIsOnline);
        }

        public override string GetUserNameByEmail(string email)
        {
            return InnerMembershipProvider.GetUserNameByEmail(email);
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            InnerMembershipProvider.Initialize(name, config);
        }

        public override string ResetPassword(string username, string answer)
        {
            return InnerMembershipProvider.ResetPassword(username, answer);
        }

        public override bool UnlockUser(string userName)
        {
            return InnerMembershipProvider.UnlockUser(userName);
        }

        public override void UpdateUser(MembershipUser user)
        {
            InnerMembershipProvider.UpdateUser(user);
        }

        public override bool ValidateUser(string username, string password)
        {
            return InnerMembershipProvider.ValidateUser(username, password);
        }
    }
}
