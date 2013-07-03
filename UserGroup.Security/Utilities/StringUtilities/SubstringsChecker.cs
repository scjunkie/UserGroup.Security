using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserGroup.Security.Utilities.StringUtilities.Base
{
    public abstract class SubstringsChecker<T> : ISubstringsChecker<T>
    {
        public T Source { get; set; }

        public virtual IEnumerable<string> Substrings { get; set; }

        protected SubstringsChecker(T source)
        {
            SetSource(source);
        }

        private void SetSource(T source)
        {
            AssertSource(source);
            Source = source;
        }

        protected virtual void AssertSource(T source)
        {
            // a hook for subclasses to assert Source objects
        }

        public bool ContainsSubstrings()
        {
            if (CanDoCheck())
            {
                return DoCheck();
            }

            return false;
        }

        protected abstract bool CanDoCheck();
        protected abstract bool DoCheck();
    }
}
