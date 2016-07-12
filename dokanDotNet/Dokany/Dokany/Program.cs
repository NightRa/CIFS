using System;
using DokanNet;
using Dokany.CifsDriver;
using Dokany.Model.IndexExamples;

namespace Dokany
{
    class Program
    {
        static void Main(string[] args)
        {
            CifsDriverInstance instance = new CifsDriverInstance(Examples.Index3());
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
    }
}
