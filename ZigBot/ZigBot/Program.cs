using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZigBot
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Bot zigBot = new Bot(args[0], args[1]);
                zigBot.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                Console.WriteLine("InnerException: " + ex.InnerException.Message);
            }

            Console.ReadKey();
        }
    }
}
