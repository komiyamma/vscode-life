using System;
using System.Collections.Generic;
using System.Reflection;

namespace WinAssemblyToTypeScriptDeclare
{
    partial class WinAssemblyToTypeScriptDeclare
    {
        // 可変長引数である
        static bool IsParams(ParameterInfo param)
        {
            return param.IsDefined(typeof(ParamArrayAttribute), false);
        }

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
                generic_param_type_list.Add(TypeToString(g, ingeneric: true));
            }

            return generic_param_type_list;

        }

        // タイプを文字列に
        static string TypeToString(Type t, bool ingeneric = false)
        {
            string ts = t.ToString();

            var tp = t;
            var gen = GetGenericParameterTypeStringList(tp);
            var genstr = "";
            if (gen.Count > 0)
            {
                genstr = "<" + String.Join(", ", gen) + ">";
            }

            var ns = "";
            // nullではなくしかも、長さがある
            if (tp.Namespace?.Length > 0)
            {
                ns = tp.Namespace + ".";
            }

            bool isClass = true;
            // ジェネリックパラメータとハッキリ出ているか、もしくは、toStringしたなかに、名前空間文字が含まれてない
            if (t.IsGenericParameter || (ns.Length > 0) && !tp.ToString().Contains(ns))
            {
                isClass = false;
                ns = "";
            }
            if (gen.Count == 0)
            {
                ts = ns + tp.Name;
            }
            else
            {
                if (tp.IsArray) {
                    ts = ns + tp.Name.Replace("[]", "") + genstr + "[]";
                }
                else
                {
                    ts = ns + tp.Name + genstr;
                }
            }

            ts = ReplaceCsToTs(ts);

            if (isClass)
            {
                ModifyType(ts, false);
            }

            return ts;
        }

        // タイプを最後に修正する。その後登録もついでに
        static string ModifyType(string ts, bool IsGenericAnyCondtion)
        {
            // 複雑OKモードでなければ、型として「any」にしておく
            if (!m_isAcceptComplexType && IsGenericAnyCondtion)
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

            return ts;

        }

        // 使っては駄目な変数名を修正する
        static string ModifyVarName(ParameterInfo pinfo)
        {
            if (pinfo.Name == "function")
            {
                return "_function";
            }
            if (IsParams(pinfo))
            {
                return "..." + pinfo.Name;
            }

            return pinfo.Name;
        }

        // フラグ。あちこちに書き散らさないようにするだけ
        static BindingFlags GetBindingFlags()
        {
            return BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
        }


        // 全てのメンバーを分析する
        static void AnalyzeMemberInfo(Type t, int nestLevel)
        {
            SWTabSpace(nestLevel);

            // クラス名名
            var gname = ReplaceCsToTs(t.Name);

            // Genericなら、
            var genericTypes = t.GetGenericArguments();
            var genlist = GetGenericParameterTypeStringList(t);

            // メンバー名<Generic, ...>の形に
            if (genlist.Count > 0)
            {
                gname = gname + "<" + String.Join(", ", genlist) + ">";
            }

            SW.WriteLine("interface " + gname + " {");

            AnalyzeConstructorInfoList(t, nestLevel);
            AnalyzeFieldInfoList(t, nestLevel);
            AnalyzePropertyInfoList(t, nestLevel);
            AnalyzeEventInfoList(t, nestLevel);
            AnalyzeMethodInfoList(t, nestLevel);

            SWTabSpace(nestLevel);
            SW.WriteLine("}");

        }

    }
}
