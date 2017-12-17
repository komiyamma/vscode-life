/*
 * Copyright (c) 2017 Akitsugu Komiyama
 * under the MIT License
 */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace WinAssemblyToTypeScriptDeclare
{
    partial class WinAssemblyToTypeScriptDeclare
    {
        /// <summary>
        ///  コンストラクタタイプのメソッド群の分析
        /// </summary>
        /// <param name="t">オブジェクト</param>
        /// <param name="nestLevel">整形用</param>
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
        
        /// <summary>
        /// １つのコンストラクタの分析
        /// </summary>
        /// <param name="m">オブジェクト</param>
        /// <param name="nestLevel">整形用</param>
        /// <param name="genericParameterTypeStringList">クラス自体のジェネリックパラメータ</param>
        static void AnalyzeConstructorInfo(ConstructorInfo m, int nestLevel, List<string> genericParameterTypeStringList)
        {
            SWTabSpace(nestLevel + 1);

            //メソッド名を表示
            SW.Write("new");

            var prmList = GetMethodGenericTypeList(m, genericParameterTypeStringList);

            if (prmList.Count > 0)
            {
                SW.Write("<" + String.Join(", ", prmList) + ">");
            }

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

                var varname = ModifyVarName(p);
                if (ts == "any" && IsParams(p) )
                {
                    SW.Write(varname + ": " + ts + "[]");
                } else
                {
                    SW.Write(varname + ": " + ts);
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
