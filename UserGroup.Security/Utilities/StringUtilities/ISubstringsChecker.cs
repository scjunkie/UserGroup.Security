using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserGroup.Security.Utilities.StringUtilities
{
    public interface ISubstringsChecker<T>
    {
        T Source { get; set; }
        
        IEnumerable<string> Substrings { get; set; }

        bool ContainsSubstrings();
    }
}