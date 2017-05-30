using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace WinAssemblyToTypeScriptDeclare
{
    partial class WinAssemblyToTypeScriptDeclare
    {
        // Type配列について、１つでもfuncを満たしているか
        static bool IsGenericAnyCondtion(Type[] GenericArguments, Predicate<Type> func)
        {
            if (GenericArguments.Length > 0)
            {
                foreach (var g in GenericArguments)
                {
                    if (func(g))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Type tのGeneric型のタイプ名の文字リストを得る
        static List<string> GetGenericParameterTypeStringList(Type t)
        {
            var generics = t.GetGenericArguments();
            List<string> generic_param_type_list = new List<string>();
            foreach (var g in generics)
            {
                generic_param_type_list.Add(g.ToString());
            }

            return generic_param_type_list;

        }

        // フラグ。あちこちに書き散らさないようにするだけ
        static BindingFlags GetBindingFlags()
        {
            return BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        }

        // 全てのメンバーを分析する
        static void AnalyzeMemberInfo(Type t, int nestLevel)
        {
            ConsoleTabSpace(nestLevel);

            // クラス名名
            var gname = ReplaceCsToTs(t.Name);

            // Genericなら、
            var genericTypes = t.GetGenericArguments();
            var genlist = GetGenericParameterTypeStringList(t);

            // メンバー名<Generic, ...>の形に
            if (genlist.Count > 0)
            {
                string[] list = genlist.ToArray();
                gname = gname + "<" + String.Join(", ", list) + ">";
            }

            Console.WriteLine("interface " + gname + " {");

            AnalyzeConstructorInfoList(t, nestLevel);
            AnalyzeFieldInfoList(t, nestLevel);
            AnalyzePropertyInfoList(t, nestLevel);
            AnalyzeMethodInfoList(t, nestLevel);

            ConsoleTabSpace(nestLevel);
            Console.WriteLine("}");

        }

    }
}
