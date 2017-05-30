using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace WinAssemblyToTypeScriptDeclare
{
    partial class WinAssemblyToTypeScriptDeclare
    {
        static void AnalyzeIndexerInfo(MethodInfo m, int nestLevel)
        {
            ConsoleTabSpace(nestLevel + 1);

            ParameterInfo[] prms = m.GetParameters();
            Console.Write("[");
            // TypeScriptのインデクサは１つだけが対応なので１つの時だけ
            if (prms.Length == 1)
            {
                // まわす必要ないのだけれど、２つ以上に対応する時のために一応まわしておく
                for (int i = 0; i < prms.Length; i++)
                {
                    if (i == 0)
                    {
                        // パラメータ
                        ParameterInfo p = prms[i];
                        // フル名があれば、それ、なければネーム
                        var tname = p.ParameterType.FullName != null ? p.ParameterType.FullName : p.ParameterType.Name;

                        var s = ReplaceCsToTs(tname);
                        Console.Write(p.Name + ": " + s);
                        if (prms.Length - 1 > i)
                        {
                            Console.Write(", ");
                        }
                    }
                }
            }
            Console.Write("]: " + m.ReturnType.ToString() + ";");
            Console.WriteLine();
        }

        static void AnalyzeResultInfo(MethodInfo m, int nestLevel, List<string> genericParameterTypeStringList)
        {
            //戻り値を表示
            if (m.ReturnType == typeof(void))
            {
                Console.Write("void");
            }
            else
            {
                string s = m.ReturnType.ToString();
                s = ReplaceCsToTs(s);

                // 複雑過ぎるかどうか
                var genlist = m.ReturnType.GetGenericArguments();
                bool isComplex = IsGenericAnyCondtion(genlist,
                    (g) => {
                        // クラスに無いのに、関数が突然Genericというのは、場合によってはTypeScriptでは無理が出る
                        if (!genericParameterTypeStringList.Exists((e) => { return e.ToString() == g.ToString(); }))
                        {
                            return true;
                        }
                        return false;
                    }
                );

                // 複雑OKモードでなければ、型として「any」にしておく
                if (!m_isAcceptComplexType && isComplex)
                {
                    s = "any";
                }

                // もともとanyモードなら
                if (m_isTypeAnyMode)
                {
                    // TypeScriptのプリミティブ型でないならば
                    if (NeverTypeScriptPrimitiveType(s))
                    {
                        s = "any";
                    }
                }
                // TypeScriptのプリミティブ型でないならば
                if (NeverTypeScriptPrimitiveType(s))
                {
                    RegistClassTypeToTaskList(s);
                }
                Console.Write(s + "");
            }
        }

        static List<string> GetMethodGenericTypeList(MethodInfo m, List<string> genericParameterTypeStringList)
        {
            //パラメータを表示
            ParameterInfo[] prms = m.GetParameters();

            List<string> prmList = new List<string>();
            for (int i = 0; i < prms.Length; i++)
            {
                ParameterInfo p = prms[i];
                var genepara = p.ParameterType.GetGenericArguments();

                foreach (var g in genepara)
                {
                    if (!genericParameterTypeStringList.Exists((e) => { return e.ToString() == g.ToString(); }))
                    {
                        if (g.ToString().Contains("."))
                        {
                            if (!prmList.Contains("D"))
                            {
                                prmList.Add("D");
                            }
                        }
                        else
                        {
                            if (!prmList.Contains(g.ToString()))
                            {
                                prmList.Add(g.ToString());
                            }
                        }
                    }
                }

                if (genepara.Length == 0)
                {
                    var s = m.ToString();
                    var param = Regex.Replace(s, @"^.+\s+" + m.Name + @"\[(.+?)\]\(.+$", "$1");
                    if (s != param)
                    {
                        string[] list = param.Split(',');
                        foreach (var l in list)
                        {
                            if (!prmList.Contains(l))
                            {
                                prmList.Add(l);
                            }
                        }
                    }
                }
            }

            {
                var s = m.ToString();
                s = s.Replace("[]", "");
                var param = Regex.Replace(s, @"^.+\s+" + m.Name + @"\[(.+?)\]\(.+$", "$1");
                if (s != param)
                {
                    string[] list = param.Split(',');
                    foreach (var l in list)
                    {
                        if (!prmList.Contains(l))
                        {
                            prmList.Add(l);
                        }
                    }
                }
            }

            return prmList;

        }

        static void AnalyzeMethodInfoList(Type t, int nestLevel)
        {

            //メソッドの一覧を取得する
            MethodInfo[] methods = t.GetMethods(GetBindingFlags());

            var genericParameterTypeStringList = GetGenericParameterTypeStringList(t);

            foreach (MethodInfo m in methods)
            {
                try
                {
                    AnalyzeMethodInfo(m, nestLevel, genericParameterTypeStringList);                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        static void AnalyzeMethodInfo(MethodInfo m, int nestLevel, List<string> genericParameterTypeStringList)
        {
            //特別な名前のメソッドは表示しない
            if (m.IsSpecialName)
            {
                if (m.Name == "get_Item")
                {
                    AnalyzeIndexerInfo(m, nestLevel);
                }
                return;
            }

            ConsoleTabSpace(nestLevel + 1);

            //メソッド名を表示
            Console.Write(m.Name);

            var prmList = GetMethodGenericTypeList(m, genericParameterTypeStringList);

            if (prmList.Count > 0)
            {
                Console.Write("<" + String.Join(", ", prmList.ToArray()) + ">");
            }

            Console.Write("(");

            //パラメータを表示
            ParameterInfo[] prms = m.GetParameters();
            for (int i = 0; i < prms.Length; i++)
            {
                ParameterInfo p = prms[i];
                string ts = ReplaceCsToTs(p.ParameterType.ToString());
                // 複雑過ぎるかどうか
                var genlist = p.ParameterType.GetGenericArguments();
                bool isComplex = IsGenericAnyCondtion(genlist,
                    (g) => {
                        // 「.」が付いていたら複雑だ
                        return g.ToString().Contains(".") ||
                        // クラスに無いのに、関数が突然Genericというのは、場合によってはTypeScriptでは無理が出る
                        (!genericParameterTypeStringList.Exists((e) => { return e.ToString() == g.ToString(); }));
                    }
                );

                // 複雑OKモードでなければ、型として「any」にしておく
                if (!m_isAcceptComplexType && isComplex)
                {
                    ts = "any";
                }
                // もともとanyモードなら
                if (m_isTypeAnyMode)
                {
                    // TypeScriptのプリミティブ型でないならば
                    if (NeverTypeScriptPrimitiveType(ts))
                    {
                        ts = "any";
                    }
                }
                // TypeScriptのプリミティブ型でないならば
                if (NeverTypeScriptPrimitiveType(ts))
                {
                    // 新たに処理するべきタスクとして登録する
                    RegistClassTypeToTaskList(ts);
                }

                // 使ってはダメな変数名
                if (p.Name == "function")
                {
                    Console.Write("_function" + ": " + ts);
                }
                else
                {
                    Console.Write(p.Name + ": " + ts);
                }
                // 引数がまだ残ってるなら、「,」で繋げて次へ
                if (prms.Length - 1 > i)
                {
                    Console.Write(", ");
                }
            }
            Console.Write("): ");

            // 戻り値を分析
            AnalyzeResultInfo(m, nestLevel, genericParameterTypeStringList);

            Console.WriteLine(";");
        }
    }
}
