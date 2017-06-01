using System;
using System.Reflection;

namespace WinAssemblyToTypeScriptDeclare
{
    partial class WinAssemblyToTypeScriptDeclare
    {
        /// <summary>
        /// プロパティタイプ群の分析
        /// </summary>
        /// <param name="t">オブジェクト</param>
        /// <param name="nestLevel">整形用</param>
        static void AnalyzePropertyInfoList(Type t, int nestLevel)
        {
            // プロパティの一覧を取得する
            PropertyInfo[] props = t.GetProperties(GetBindingFlags());

            foreach (PropertyInfo p in props)
            {
                try
                {
                    AnalyzePropertyInfo(p, nestLevel);
                }
                catch (Exception)
                {
                    // SW.WriteLine(e.Message);
                }
            }
        }
        /// <summary>
        /// １つのプロパティの分析
        /// </summary>
        /// <param name="p">オブジェクト</param>
        /// <param name="nestLevel">整形用</param>
        static void AnalyzePropertyInfo(PropertyInfo p, int nestLevel)
        {
 
            // TypeScript向けに変換
            var ts = TypeToString(p.PropertyType);

            // 引数一覧
            var genlist = p.PropertyType.GetGenericArguments();

            // 「.」があったら、複雑すぎると判断する。
            bool isComplex = IsGenericAnyCondtion(genlist, (g) => { return g.ToString().Contains("."); });

            ts = ModifyType(ts, isComplex);

            SWTabSpace(nestLevel + 1);

            SW.WriteLine(p.Name + " :" + ts + ";");
        }

    }
}
