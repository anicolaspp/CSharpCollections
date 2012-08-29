using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nikos
{
    public interface IPrint
    {
        string Print(params object[] args);
    }
}
