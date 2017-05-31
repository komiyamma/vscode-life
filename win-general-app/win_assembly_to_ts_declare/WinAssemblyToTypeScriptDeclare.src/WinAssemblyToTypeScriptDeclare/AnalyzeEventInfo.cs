using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace WinAssemblyToTypeScriptDeclare
{
    partial class WinAssemblyToTypeScriptDeclare
    {
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
        static void AnalyzeEventInfo(EventInfo p, int nestLevel)
        {

            // TypeScript向けに変換
            var ts = TypeToString(p.EventHandlerType);

            // 引数一覧
            var genepara = p.EventHandlerType.GetGenericArguments();

            // 「.」があったら、複雑すぎると判断する。
            bool isComplex = IsGenericAnyCondtion(genepara, (g) => { return g.ToString().Contains("."); });

            ts = ModifyType(ts, isComplex);

            SWTabSpace(nestLevel + 1);

            SW.WriteLine(p.Name + " :" + ts + ";");
        }

    }
}
