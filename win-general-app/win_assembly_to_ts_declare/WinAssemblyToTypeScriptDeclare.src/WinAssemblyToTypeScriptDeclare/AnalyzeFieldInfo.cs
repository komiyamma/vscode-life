using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace WinAssemblyToTypeScriptDeclare
{
    partial class WinAssemblyToTypeScriptDeclare
    {
        static void AnalyzeFieldInfoList(Type t, int nestLevel)
        {

            //メソッドの一覧を取得する
            FieldInfo[] props = t.GetFields(GetBindingFlags());

            foreach (FieldInfo m in props)
            {
                try
                {
                    AnalyzeFieldInfo(m, nestLevel);
                }
                catch (Exception)
                {
                    // SW.WriteLine(e.Message);
                }
            }

        }
        static void AnalyzeFieldInfo(FieldInfo m, int nestLevel)
        {
            if (!m.IsPublic)
            {
                return;
            }

            var ts = TypeToString(m.FieldType);

            // 複雑過ぎるかどうか
            var genlist = m.FieldType.GetGenericArguments();
            bool isComplex = IsGenericAnyCondtion(genlist, (g) => { return g.ToString().Contains("."); });

            ts = ModifyType(ts, isComplex);

            SWTabSpace(nestLevel + 1);

            SW.WriteLine(m.Name + " :" + ts + ";");
        }
    }
}
