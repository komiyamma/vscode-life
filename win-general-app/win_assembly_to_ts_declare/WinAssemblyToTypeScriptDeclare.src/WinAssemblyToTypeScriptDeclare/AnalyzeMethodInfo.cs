using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;

namespace WinAssemblyToTypeScriptDeclare
{
    partial class WinAssemblyToTypeScriptDeclare
    {
        // インデクサの分析
        static void AnalyzeIndexerInfo(MethodInfo m, int nestLevel)
        {
            SWTabSpace(nestLevel + 1);

            StringWriter sw_for_ix = new StringWriter();
            bool indexer_can_flush = true;

            ParameterInfo[] prms = m.GetParameters();
            sw_for_ix.Write("[");
            
            // TypeScriptのインデクサは１つだけが対応なので１つの時だけ
            if (prms.Length != 1)
            {
                indexer_can_flush = false;
            }

            // まわす必要ないのだけれど、２つ以上に対応する時のために一応まわしておく
            for (int i = 0; i < prms.Length; i++)
            {
                // パラメータ
                ParameterInfo p = prms[i];

                /*
                // フル名があれば、それ、なければネーム
                var tname = p.ParameterType.FullName != null ? p.ParameterType.FullName : p.ParameterType.Name;
                
                var ts = ReplaceCsToTs(tname);
                */
                var ts = TypeToString(p.ParameterType);

                ts = ModifyType(ts, false);

                sw_for_ix.Write(p.Name + ": " + ts);
                if (prms.Length - 1 > i)
                {
                    sw_for_ix.Write(", ");
                }

                // TypeScriptの型はどちらかである必要がある
                if (ts != "string" && ts != "number" )
                {
                    indexer_can_flush = false;
                }
            }

            var rts = m.ReturnType.ToString();

            rts = ModifyType(rts, false);

            sw_for_ix.Write("]: " + rts + ";");
            sw_for_ix.WriteLine();

            // 大丈夫なインデクサなら出力するが、ダメならコメントアウト状態で出力
            if (indexer_can_flush)
            {
                SW.Write(sw_for_ix);
            } else
            {
                SW.Write("// " + sw_for_ix);
            }
        }

        static void AnalyzeResultInfo(MethodInfo m, int nestLevel, List<string> genericParameterTypeStringList)
        {
            //戻り値を表示
            if (m.ReturnType == typeof(void))
            {
                SW.Write("void");
            }
            else
            {

                var ts = TypeToString(m.ReturnType);

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


                ts = ModifyType(ts, isComplex);

                SW.Write(ts + "");
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
                            if (!prmList.Contains("TANY"))
                            {
                                prmList.Add("TANY");
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
                    SW.WriteLine(e.Message);
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

            SWTabSpace(nestLevel + 1);

            //メソッド名を表示
            SW.Write(m.Name);

            var prmList = GetMethodGenericTypeList(m, genericParameterTypeStringList);

            if (prmList.Count > 0)
            {
                SW.Write("<" + String.Join(", ", prmList) + ">");
            }

            SW.Write("(");

            //パラメータを表示
            ParameterInfo[] prms = m.GetParameters();
            for (int i = 0; i < prms.Length; i++)
            {
                ParameterInfo p = prms[i];
                var ts = TypeToString(p.ParameterType);

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

                ts = ModifyType(ts, isComplex);


                // 使ってはダメな変数名
                if (p.Name == "function")
                {
                    SW.Write("_function" + ": " + ts);
                }
                else
                {
                    SW.Write(p.Name + ": " + ts);
                }
                // 引数がまだ残ってるなら、「,」で繋げて次へ
                if (prms.Length - 1 > i)
                {
                    SW.Write(", ");
                }
            }
            SW.Write("): ");

            // 戻り値を分析
            AnalyzeResultInfo(m, nestLevel, genericParameterTypeStringList);

            SW.WriteLine(";");
        }

    }
}
