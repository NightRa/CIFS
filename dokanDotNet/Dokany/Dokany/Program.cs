using System;
using System.IO;
using System.Threading;
using DokanNet;
using Dokany.CifsDriver;
using Dokany.Model.Entries;
using Dokany.Model.IndexExamples;

namespace Dokany
{
    class Program
    {
        static void Main(string[] args)
        {
            CifsDriverInstance instance = new CifsDriverInstance(Examples.Index4());
            var t = new Thread(() => CheckForChange(instance.Index));
            t.IsBackground = true;
            t.Start();
            try
            {
                instance.Mount("n:\\", DokanOptions.DebugMode, 5);
            }
            catch (Exception e)
            {
                Console.WriteLine("A fatal error occurred:");
                Console.WriteLine(e.StackTrace);
            }
        }

        private static void CheckForChange(Index index)
        {
            using (var streamWriter = File.CreateText(@"C:\Users\Yuval\Desktop\log.txt"))
            {

                var hash = 100;
                while (true)
                {
                    var newHash = index.MainFolder.GetHashCode();
                    if (hash == newHash)
                        Thread.Sleep(10);
                    else
                    {
                        hash = newHash;
                        streamWriter.WriteLine("**************************");
                        streamWriter.WriteLine(index.MainFolder.ToString());
                        streamWriter.Flush();
                    }
                }
            }
        }
    }
}
