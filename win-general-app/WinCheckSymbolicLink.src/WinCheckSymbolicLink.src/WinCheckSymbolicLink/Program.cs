using System;
using System.IO;
using System.Diagnostics;

namespace WinCheckSymbolicLink
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0) { 
                var target = args[0];
                if (Directory.Exists(target)) {
                    FileAttributes attrs = File.GetAttributes(target);
                    if ((attrs & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                    {
                        Console.WriteLine("true");
                        return;
                    }
                }
            }
            Console.WriteLine("false");
            return;
        }
    }
}
