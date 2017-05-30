using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace WinAssemblyToTypeScriptDeclare
{
    partial class WinAssemblyToTypeScriptDeclare
    {
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
                    // Console.WriteLine(e.Message);
                }
            }
        }
        static void AnalyzePropertyInfo(PropertyInfo p, int nestLevel)
        {
            // プロパティの名前空間も含めたフル名
            string ts = p.PropertyType.FullName;

            // TypeScript向けに変換
            ts = ReplaceCsToTs(ts);

            // 引数一覧
            var genepara = p.PropertyType.GetGenericArguments();

            // 「.」があったら、複雑すぎると判断する。
            bool isComplex = IsGenericAnyCondtion(genepara, (g) => { return g.ToString().Contains("."); });

            // 複雑OKモードでなければ、型として「any」にしておく
            if (!m_isAcceptComplexType && isComplex)
            {
                ts = "any";
            }

            // もともとanyモードなら
            if (m_isTypeAnyMode)
            {
                // TypeScriptのプリミティブ型でないならば
                if (NeverTypeScriptPrimitiveType(ts))
                {
                    // 型はanyにしておく
                    ts = "any";
                }
            }

            // TypeScriptのプリミティブ型でないなら
            if (NeverTypeScriptPrimitiveType(ts))
            {
                // 新たに処理するべきタスクとして登録する
                RegistClassTypeToTaskList(ts);
            }

            ConsoleTabSpace(nestLevel + 1);

            Console.WriteLine(p.Name + " :" + ts + ";");
        }

    }
}
