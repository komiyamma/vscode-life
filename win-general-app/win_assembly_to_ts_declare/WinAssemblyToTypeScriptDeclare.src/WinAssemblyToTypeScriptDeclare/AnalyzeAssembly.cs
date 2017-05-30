using System;
using System.Collections.Generic;

namespace WinAssemblyToTypeScriptDeclare
{
    partial class WinAssemblyToTypeScriptDeclare
    {
        static List<NameSpaceNested> nsList;

        static bool m_isTypeAnyMode = false;

        static void PrintClassDetail(Type t, String _ns, int n)
        {
            ConsoleTabSpace(n); Console.WriteLine("/**");
            ConsoleTabSpace(n); Console.WriteLine("名前:{0}", t.Name);
            ConsoleTabSpace(n); Console.WriteLine("名前空間:{0}", _ns == "NONE" ? "無し" : _ns);
            ConsoleTabSpace(n); Console.WriteLine("完全限定名:{0}", t.FullName);
            ConsoleTabSpace(n); Console.WriteLine("このメンバを宣言するクラス:{0}", t.DeclaringType);
            ConsoleTabSpace(n); Console.WriteLine("親クラス:{0}", t.BaseType);
            ConsoleTabSpace(n); Console.WriteLine("属性:{0}", t.Attributes);
            ConsoleTabSpace(n); Console.WriteLine("*/");

        }

        static void AnalyzeAssembly(Type t, string strNameSpace, string strClassName)
        {
            nsList = new List<NameSpaceNested>();

            if ((t.Namespace == strNameSpace || strNameSpace == "any" || strNameSpace == "NONE") && t.Name == strClassName)
            {
                string _ns = t.Namespace;
                if (_ns == null || _ns == "")
                {
                    _ns = "NONE";

                    // NameSpaceはちゃんと存在するのに、NONE指定ならやらない
                } else if (strNameSpace == "NONE") {
                    return;
                }

                // 対象の「名前空間、クラス名」の組み合わせはすでに、出力済み？
                var item = TaskItems.Find( (tsk) => { return tsk.strNameSpace == _ns && tsk.strClassName == t.Name; } );
                // すでに登録済みで、すでに処理済み
                if (item != null && item.Status >= 2)
                {
                    return;
                }

                if (item == null)
                {
                    item = new TaskItem { strNameSpace = _ns, strClassName = t.Name, Status = 2 };
                    TaskItems.Add(item);
                } else
                {
                    item.Status = 2;
                }

                var exist = nsList.Find((ns) => { return ns.FullNameSpace == _ns; });
                if (exist == null)
                {
                    string[] s = _ns.Split('.');
                    List<string> nls = new List<string>();
                    nls.AddRange(s);

                    int nNextLevel = 0;

                    NameSpaceNested cut_parent = null;
                    while (nls.Count > 0)
                    {
                        var ns = new NameSpaceNested();
                        ns.ParentNameSpace = cut_parent;
                        ns.NestLevel = nNextLevel;
                        ns.NameSpace = nls[0];
                        cut_parent = ns;
                        nls.RemoveAt(0);
                        nNextLevel++;

                        nsList.Add(ns);
                    }
                }

                nsList.Sort();

                int nLastNext = 0;
                for (int n = 0; n < nsList.Count; n++)
                {
                    if (nsList[n].NameSpace != "any" && nsList[n].NameSpace != "NONE")
                    {
                        ConsoleTabSpace(n);
                        if (n == 0)
                        {
                            Console.Write("declare ");
                        }
                        Console.WriteLine("namespace " + nsList[n].NameSpace + " {");

                        // 一番深いネームスペースのところで…
                        if (n == nsList.Count - 1)
                        {
                            PrintClassDetail(t, _ns, n+1);
                            nLastNext = n;
                            AnalyzeMemberInfo(t, n + 1);
                        }

                    }
                    else
                    {
                        PrintClassDetail(t, _ns, 0);

                        Console.Write("declare ");
                        nLastNext = n;
                        AnalyzeMemberInfo(t, 0);
                    }
                }

                for (int n = nLastNext; n >= 0; n--)
                {
                    if (nsList[n].NameSpace != "any" && nsList[n].NameSpace != "NONE")
                    {
                        ConsoleTabSpace(n);
                        Console.WriteLine("}");
                    }
                }
            }

        }
    }
}