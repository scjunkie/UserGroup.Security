using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sitecore.Diagnostics;

using UserGroup.Security.Utilities.StringUtilities.Base;

namespace UserGroup.Security.Utilities.StringUtilities
{
    public class StringSubstringsChecker : SubstringsChecker<string>
    {
        private bool IgnoreCase { get; set; }

        private StringSubstringsChecker(IEnumerable<string> substrings)
            : this(null, substrings, false)
        {
        }

        private StringSubstringsChecker(IEnumerable<string> substrings, bool ignoreCase)
            : this(null, substrings, ignoreCase)
        {
        }

        private StringSubstringsChecker(string source, IEnumerable<string> substrings, bool ignoreCase)
            : base(source)
        {
            SetSubstrings(substrings);
            SetIgnoreCase(ignoreCase);
        }

        private void SetSubstrings(IEnumerable<string> substrings)
        {
            AssertSubstrings(substrings);
            Substrings = substrings;
        }

        private static void AssertSubstrings(IEnumerable<string> substrings)
        {
            Assert.ArgumentNotNull(substrings, "substrings");
            Assert.ArgumentCondition(substrings.Any(), "substrings", "substrings must contain as at least one string!");
        }

        private void SetIgnoreCase(bool ignoreCase)
        {
            IgnoreCase = ignoreCase;
        }

        protected override bool CanDoCheck()
        {
            return !string.IsNullOrEmpty(Source);
        }

        protected override bool DoCheck()
        {
            Assert.ArgumentNotNullOrEmpty(Source, "Source");

            foreach (string substring in Substrings)
            {
                if(DoesSourceContainSubstring(substring))
                {
                    return true;
                }
            }

            return false;
        }

        private bool DoesSourceContainSubstring(string substring)
        {
            if (IgnoreCase)
            {
                return !IsNotFoundIndex(Source.IndexOf(substring, StringComparison.CurrentCultureIgnoreCase));
            }

            return !IsNotFoundIndex(Source.IndexOf(substring));
        }

        private static bool IsNotFoundIndex(int index)
        {
            const int notFound = -1;
            return index == notFound;
        }

        public static ISubstringsChecker<string> CreateNewStringSubstringsContainer(IEnumerable<string> substrings)
        {
            return new StringSubstringsChecker(substrings);
        }

        public static ISubstringsChecker<string> CreateNewStringSubstringsContainer(IEnumerable<string> substrings, bool ignoreCase)
        {
            return new StringSubstringsChecker(substrings, ignoreCase);
        }

        public static ISubstringsChecker<string> CreateNewStringSubstringsContainer(string source, IEnumerable<string> substrings, bool ignoreCase)
        {
            return new StringSubstringsChecker(source, substrings, ignoreCase);
        }
    }
}
