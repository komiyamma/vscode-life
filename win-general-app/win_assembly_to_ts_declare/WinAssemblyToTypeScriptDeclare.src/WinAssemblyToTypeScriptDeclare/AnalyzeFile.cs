using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;

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

            while (true)
            {
                var nextTask = GetNextTask();
                if (nextTask == null)
                {
                    break;
                }
                DoNextTask(nextTask);
            }
        }

        static void AnalyzeAll(string[] args)
        {
            if (args.Length < 2)
            {
                return;
            }

            // 引数のうち、オプション系
            AnalizeArgsOption(args);

            //"C:\test"以下のファイルをすべて取得する
            IEnumerable<string> files1 = System.IO.Directory.EnumerateFiles(@".", "*.dll");
            ForEachLoadAssembly(files1);
            //"C:\test"以下のファイルをすべて取得する
            IEnumerable<string> files2 = System.IO.Directory.EnumerateFiles(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319", "*.dll");
            ForEachLoadAssembly(files2);

            ForEachAnalyzeAssembly();

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
                    if (!preline.Contains(line))
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

        static List<KeyValuePair<string, string>> NsAndCnList = new List<KeyValuePair<string, string>>();
        enum NameSpaceAndClassName { None, NameSpace, ClassName };
        // 引数分析。オプション系
        static void AnalizeArgsOption(string[] args)
        {
            foreach (var v in args)
            {
                Match mDeep = Regex.Match(v, @"\-\-?deep:(\d+)");
                if (mDeep.Success)
                {
                    var deep = mDeep.Groups[1].Value;
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
                Match mComplex = Regex.Match(v, @"\-\-?complex");
                if (mComplex.Success)
                {
                    m_isAcceptComplexType = true;

                }
            }

            List<string> ns_and_cn = new List<string>();
            ns_and_cn.AddRange(args);
            // オプション系は全部削除
            ns_and_cn.RemoveAll((arg) => { return arg.StartsWith("-"); });

            for (int ix = 0; ix < ns_and_cn.Count - 1; ix += 2)
            {
                try
                {
                    var (strNameSpace, strClassName) = (ns_and_cn[ix], ns_and_cn[ix + 1]);
                    var pair = new KeyValuePair<string, string>(strNameSpace, strClassName);
                    NsAndCnList.Add(pair);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        static List<KeyValuePair<string, Assembly>> asmMap = new List<KeyValuePair<string, Assembly>>();

        // ファイルリストを対象の名前空間とクラスの定義があるかどうか探して分析する。
        static void ForEachLoadAssembly(IEnumerable<string> files)
        {
            // すでにロード済みならなにもしない
            if (asmMap.Count > 0 )
            {
                return;
            }
            //ファイルを列挙する
            foreach (string f in files)
            {
                try
                {
                    string full = System.IO.Path.GetFullPath(f);
                    Assembly asm = Assembly.LoadFile(full);
                    var pair = new KeyValuePair<string, Assembly>(full, asm);
                    asmMap.Add(pair);
                }
                catch (Exception)
                {

                }
            }
        }

        // ファイルリストを対象の名前空間とクラスの定義があるかどうか探して分析する。
        static void ForEachAnalyzeAssembly()
        {
            //ファイルを列挙する
            foreach (var asm in asmMap)
            {
                try
                {
                    var types = asm.Value.ExportedTypes;
                    foreach (Type t in types)
                    {
                        foreach (var pair in NsAndCnList)
                        {
                            AnalyzeAssembly(t, pair.Key, pair.Value);
                            WriteConsoleUniqueLine();
                            SetNextStringWriter();
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
