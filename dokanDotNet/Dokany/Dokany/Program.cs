using System;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DokanNet;
using Dokany.CifsDriver;
using Dokany.Model.IndexExamples;
using Dokany.Model.Pointers;

namespace Dokany
{
    class Program
    {
        static void Main(string[] args)
        {
            CifsDriverInstance instance = new CifsDriverInstance(Examples.Index3());
            //Task printWhenChanged = Task.Run(PrintWhenChanged(instance));
            try
            {
                
                //var str = Hash.Random(20, new Random()).ToString();
                //Console.WriteLine(str);
                //byte[] array = Encoding.ASCII.GetBytes(str);
                //File.WriteAllBytes(@"C:\Users\Yuval\Desktop\yuval.txt", array);
                instance.Mount("n:\\", DokanOptions.DebugMode, 5);
            }
            catch (Exception e)
            {
                Console.WriteLine("A fatal error occurred:");
                Console.WriteLine(e.StackTrace);
            }
        }

        private static string GetData<T>(T value)
        {
            StringBuilder str = new StringBuilder();
            //System.Security.AccessControl.
            foreach (var prop in value.GetType().GetProperties())
            {
                str.AppendFormat("{0}={1}", prop.Name, prop.GetValue(value, null));
                str.AppendLine();
            }
            return str.ToString();
        }

        private static Action PrintWhenChanged(CifsDriverInstance instance)
        {
            var knownInstance = instance.DeepCopy();
            Console.WriteLine(knownInstance.ToString());
            return (() =>
            {
                if (instance.Equals(knownInstance))
                    Thread.Sleep(5);
                else
                {
                    Console.WriteLine(knownInstance);
                    knownInstance = instance.DeepCopy();
                }
            });
        }
    }
}
