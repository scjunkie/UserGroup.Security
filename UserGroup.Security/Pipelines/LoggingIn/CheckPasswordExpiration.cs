using System;
using System.Web.Security;

using Sitecore.Diagnostics;
using Sitecore.Pipelines.LoggingIn;
using Sitecore.Web;

namespace UserGroup.Security.Pipelines.LoggingIn
{
    public class CheckPasswordExpiration
    {
        private TimeSpan TimeSpanToExpirePassword { get; set; }
        private string ChangePasswordPageUrl { get; set; }

        public void Process(LoggingInArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (!IsEnabled())
            {
                return;
            }

            MembershipUser user = GetMembershipUser(args);
            if (HasPasswordExpired(user))
            {
                WebUtil.Redirect(ChangePasswordPageUrl);
            }
        }

        private bool IsEnabled()
        {
            return IsTimeSpanToExpirePasswordSet() && IsChangePasswordPageUrlSet();
        }

        private bool IsTimeSpanToExpirePasswordSet()
        {
            return TimeSpanToExpirePassword > default(TimeSpan);
        }

        private bool IsChangePasswordPageUrlSet()
        {
            return !string.IsNullOrWhiteSpace(ChangePasswordPageUrl);
        }

        private static MembershipUser GetMembershipUser(LoggingInArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNullOrEmpty(args.Username, "args.Username");
            return Membership.GetUser(args.Username, false);
        }

        private bool HasPasswordExpired(MembershipUser user)
        {
            return user.LastPasswordChangedDate.Add(TimeSpanToExpirePassword) <= DateTime.Now;
        }
    }
}
