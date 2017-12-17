/*
 * Copyright (c) 2017 Akitsugu Komiyama
 * under the MIT License
 */

using System;
using System.Reflection;

namespace WinAssemblyToTypeScriptDeclare
{
    partial class WinAssemblyToTypeScriptDeclare
    {
        /// <summary>
        /// イベントタイプ群の分析
        /// </summary>
        /// <param name="t">オブジェクト</param>
        /// <param name="nestLevel">整形用</param>
        static void AnalyzeEventInfoList(Type t, int nestLevel)
        {
            // プロパティの一覧を取得する
            EventInfo[] props = t.GetEvents(GetBindingFlags());

            foreach (EventInfo p in props)
            {
                try
                {
                    AnalyzeEventInfo(p, nestLevel);
                }
                catch (Exception)
                {
                    // SW.WriteLine(e.Message);
                }
            }
        }
        /// <summary>
        /// １つのイベントの分析
        /// </summary>
        /// <param name="p">オブジェクト</param>
        /// <param name="nestLevel">整形用</param>
        static void AnalyzeEventInfo(EventInfo p, int nestLevel)
        {
            // TypeScript向けに変換
            var ts = TypeToString(p.EventHandlerType);

            // 引数一覧
            var genlist = p.EventHandlerType.GetGenericArguments();

            // 「.」があったら、複雑すぎると判断する。
            bool isComplex = IsGenericAnyCondtion(genlist, (g) => { return g.ToString().Contains("."); });

            ts = ModifyType(ts, isComplex);

            SWTabSpace(nestLevel + 1);

            SW.WriteLine(p.Name + " :" + ts + ";");
        }

    }
}
