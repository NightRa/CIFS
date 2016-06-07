using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokany.Util
{
    public interface IDeepCopiable<out T>
    {
        T DeepCopy();
    }
}
