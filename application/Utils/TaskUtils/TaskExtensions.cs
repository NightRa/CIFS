using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.TaskUtils
{
    public static class TaskExtensions
    {
        public static Task<T2> Map<T1, T2>(this Task<T1> @this, Func<T1, T2> mapFunc)
        {
            return Task.Factory.StartNew(() => mapFunc(@this.Result));
        }
        public static Task<T2> FlatMap<T1, T2>(this Task<T1> @this, Func<T1, Task<T2>> mapFunc)
        {
            return Task.Factory.StartNew(() => mapFunc(@this.Result).Result);
        }
    }
}
