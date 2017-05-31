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
                    // SW.WriteLine(e.Message);
                }
            }
        }

        static void AnalyzeConstructorInfo(ConstructorInfo m, int nestLevel, List<string> genericParameterTypeStringList)
        {
            SWTabSpace(nestLevel + 1);

            //メソッド名を表示
            SW.Write("new");

            //パラメータを表示
            ParameterInfo[] prms = m.GetParameters();
            SW.Write("(");
            for (int i = 0; i < prms.Length; i++)
            {
                ParameterInfo p = prms[i];
                string ts = TypeToString(p.ParameterType);

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

                ts = ModifyType(ts, isComplex);

                // 使ってはダメな変数名
                if (p.Name == "function")
                {
                    SW.Write("_function" + ": " + ts);
                }　else { 
                    SW.Write(p.Name + ": " + ts);
                }

                // 引数がまだ残ってるなら、「,」で繋げて次へ
                if (prms.Length - 1 > i)
                {
                    SW.Write(", ");
                }
            }

            // 引数が全部終了
            SW.WriteLine(");");
        }
    }
}
