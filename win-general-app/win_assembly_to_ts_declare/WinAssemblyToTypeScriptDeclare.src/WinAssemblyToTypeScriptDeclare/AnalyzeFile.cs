using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;

namespace WinAssemblyToTypeScriptDeclare
{
    partial class WinAssemblyToTypeScriptDeclare
    {
        // 分析の深さ。これをこえると、TypeScriptの元々ある規定の型以外は
        // 全てanyにすることで、早々に切り上げる。
        static int m_AnalyzeDeepLevel = 1;

        // 複雑な型でTypeScriptの文法ではエラー覚悟で出力するのを許容するかどうか。
        static bool m_isAcceptComplexType = false;


        static StringWriter SW = new StringWriter();

        static void Main(string[] args)
        {
            AnalyzeAll(args);
        }

        static void AnalyzeAll(string[] args)
        {
            if (args.Length < 2)
            {
                return;
            }

            // 引数のうち、オプション系
            AnalizeArgsOption(args);

            var (strNameSpace, strClassName) = (args[0], args[1]);

            //"C:\test"以下のファイルをすべて取得する
            IEnumerable<string> files1 = System.IO.Directory.EnumerateFiles(@".", "*.dll");
            ForEachAnalyzeAssembly(files1, strNameSpace, strClassName);

            //"C:\test"以下のファイルをすべて取得する
            IEnumerable<string> files2 = System.IO.Directory.EnumerateFiles(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319", "*.dll");
            ForEachAnalyzeAssembly(files2, strNameSpace, strClassName);

            WriteConsoleUniqueLine();
            SetNextStringWriter();

            DoNextTask();
        }

        // 重複行を削除して出力
        static void WriteConsoleUniqueLine()
        {
            using (StringReader sr = new StringReader(SW.ToString()))
            {
                List<string> preline = new List<string>();
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();
                    if (!preline.Contains(line) )
                    {
                        preline.Add(line);
                        Console.WriteLine(line);
                    }
                }
            }
        }

        // 次のStringWriterをセット
        static void SetNextStringWriter()
        {
            SW.Close();
            SW = new StringWriter();
        }


        // 引数分析。オプション系
        static void AnalizeArgsOption(string[] args)
        {
            foreach (var v in args)
            {
                Match m = Regex.Match(v, @"\-\-?deep:(\d+)");
                if (m.Success)
                {
                    var deep = m.Groups[1].Value;
                    m_AnalyzeDeepLevel = Int32.Parse(deep);
                    if (m_AnalyzeDeepLevel >= 2)
                    {
                        m_AnalyzeDeepLevel = 2;
                    }
                    if (m_AnalyzeDeepLevel == 0)
                    {
                        m_isTypeAnyMode = true;
                    }

                }
                Match m2 = Regex.Match(v, @"\-\-?complex");
                if (m2.Success)
                {
                    m_isAcceptComplexType = true;

                }
            }
        }

        // ファイルリストを対象の名前空間とクラスの定義があるかどうか探して分析する。
        static void ForEachAnalyzeAssembly(IEnumerable<string> files, string strNameSpace, string strClassName)
        {
            //ファイルを列挙する
            foreach (string f in files)
            {
                try
                {
                    string full = System.IO.Path.GetFullPath(f);
                    Assembly asm = Assembly.LoadFile(full);
                    Type[] types = asm.GetTypes();
                    foreach (Type t in types)
                    {
                        AnalyzeAssembly(t, strNameSpace, strClassName);
                    }
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
