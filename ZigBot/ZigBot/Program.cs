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
            //string t1 = "/test@zig arg1 arg2 arg3";
            //string t2 = "/test@haobot arg1 arg2 arg3";
            //string t3 = "/test arg1 arg2 arg3";

            //var a = t1.Split(new string[] { " ", "@zig" }, StringSplitOptions.RemoveEmptyEntries);
            //var b = t2.Split(new string[] { " ", "@zig" }, StringSplitOptions.RemoveEmptyEntries);
            //var c = t3.Split(new string[] { " ", "@zig" }, StringSplitOptions.RemoveEmptyEntries);
            //var d = t1.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            //if (d[0].Contains('@'))
            //{
            //    var botname = d[0].Substring(d[0].IndexOf('@') + 1);
            //    d[0] = d[0].Substring(0, d[0].IndexOf('@'));
            //}

            //Console.WriteLine(t1.StartsWith("/test"));
            //Console.WriteLine(t2.StartsWith("/test"));


            try
            {
                Bot zigBot = new Bot("75026284:AAHWu1ACDdRFYc0NYhBWCseJZMR4cOucW_o", "teste.txt");
                zigBot.Start();
                //zigBot.Test();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                Console.WriteLine("InnerException: " + ex.InnerException.Message);
            }

            //using (StreamWriter sw = File.CreateText("teste.txt"))
            //{
            //    sw.Write("testando");
            //}

            //Console.WriteLine(File.ReadAllText("teste.txt"));

            Console.ReadKey();
        }
    }
}
