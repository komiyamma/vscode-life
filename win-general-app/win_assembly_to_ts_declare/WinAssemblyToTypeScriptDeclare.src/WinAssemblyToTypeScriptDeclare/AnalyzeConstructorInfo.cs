using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace WinAssemblyToTypeScriptDeclare
{
    partial class WinAssemblyToTypeScriptDeclare
    {
        static void AnalyzeConstructorInfoList(Type t, int nestLevel)
        {
            var genericParameterTypeStringList = GetGenericParameterTypeStringList(t);

            ConstructorInfo[] conss = t.GetConstructors(GetBindingFlags());
            foreach (ConstructorInfo m in conss)
            {
                try
                {
                    AnalyzeConstructorInfo(m, nestLevel, genericParameterTypeStringList);
                }
                catch (Exception)
                {
                    // Console.WriteLine(e.Message);
                }
            }
        }

        static void AnalyzeConstructorInfo(ConstructorInfo m, int nestLevel, List<string> genericParameterTypeStringList)
        {
            ConsoleTabSpace(nestLevel + 1);

            //メソッド名を表示
            Console.Write("new");

            //パラメータを表示
            ParameterInfo[] prms = m.GetParameters();
            Console.Write("(");
            for (int i = 0; i < prms.Length; i++)
            {
                ParameterInfo p = prms[i];
                string s = ReplaceCsToTs(p.ParameterType.ToString());

                // 複雑過ぎるかどうか
                var genlist = p.ParameterType.GetGenericArguments();
                bool isComplex = IsGenericAnyCondtion(genlist,
                    (g) =>
                    {
                        // 「.」が付いていたら複雑だ
                        return
                        g.ToString().Contains(".") ||
                        // クラスに無いのに、関数が突然Genericというのは、場合によってはTypeScriptでは無理が出る
                        (!genericParameterTypeStringList.Exists((e) => { return e.ToString() == g.ToString(); }));
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
                // TypeScriptのプリミティブ型でないなら
                if (NeverTypeScriptPrimitiveType(s))
                {
                    // 新たに処理するべきタスクとして登録する
                    RegistClassTypeToTaskList(s);
                }

                Console.Write(p.Name + ": " + s);

                // 引数がまだ残ってるなら、「,」で繋げて次へ
                if (prms.Length - 1 > i)
                {
                    Console.Write(", ");
                }
            }

            // 引数が全部終了
            Console.WriteLine(");");
        }
    }
}
