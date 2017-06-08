using System;
using System.Reflection;

namespace WinAssemblyToTypeScriptDeclare
{
    partial class WinAssemblyToTypeScriptDeclare
    {
        /// <summary>
        /// フィールド群の分析
        /// </summary>
        /// <param name="t">オブジェクト</param>
        /// <param name="nestLevel">整形用</param>
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
        /// <summary>
        /// １つのフィールドの分析
        /// </summary>
        /// <param name="m">オブジェクト</param>
        /// <param name="nestLevel">整形用</param>
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

            // 読み取り専用
            if (m.IsInitOnly || m.IsLiteral)
            {
                SW.Write("readonly ");
            }

            SW.WriteLine(m.Name + " :" + ts + ";");
        }
    }
}
