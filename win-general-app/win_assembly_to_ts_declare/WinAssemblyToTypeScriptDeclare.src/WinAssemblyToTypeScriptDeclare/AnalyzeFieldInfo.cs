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
                    // Console.WriteLine(e.Message);
                }
            }

        }
        static void AnalyzeFieldInfo(FieldInfo m, int nestLevel)
        {
            if (!m.IsPublic)
            {
                return;
            }
            string s = m.FieldType.FullName;
            s = ReplaceCsToTs(s);

            // 複雑過ぎるかどうか
            var genlist = m.FieldType.GetGenericArguments();
            bool isComplex = IsGenericAnyCondtion(genlist, (g) => { return g.ToString().Contains("."); });
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

            // TypeScriptのプリミティブ型でないならば
            if (NeverTypeScriptPrimitiveType(s))
            {
                // 新たに処理するべきタスクとして登録する
                RegistClassTypeToTaskList(s);
            }

            ConsoleTabSpace(nestLevel + 1);

            Console.WriteLine(m.Name + " :" + s + ";");
        }
    }
}
