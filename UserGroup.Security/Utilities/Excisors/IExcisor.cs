using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserGroup.Security.Utilities.Excisors
{
    public interface IExcisor<T, U>
    {
        U Excise(T source);
    }
}
