using System;
using System.Threading;
using System.Xml;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args?.Length == 0) throw new Exception("путь к директории не задан");

                Files files = new Files(args[0]);

                files.GetFilesPath();
                files.ReadFiles();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                Environment.Exit(0);
            }

        }
    }
}
