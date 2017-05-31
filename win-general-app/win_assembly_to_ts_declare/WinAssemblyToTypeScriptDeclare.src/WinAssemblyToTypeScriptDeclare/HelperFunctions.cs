using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;

namespace WinAssemblyToTypeScriptDeclare
{
    partial class WinAssemblyToTypeScriptDeclare
    {
        // C#の型⇒TypeScriptの型
        static string ReplaceCsTypeToTsType(string ts)
        {
            ts = ts.Replace("System.Int32", "number")
            .Replace("System.UInt32", "number")
            .Replace("System.Int64", "number")
            .Replace("System.UInt64", "number")
            .Replace("System.Decimal", "number")
            .Replace("System.UInt64", "number")
            .Replace("System.Double", "number")
            .Replace("System.Single", "number")
            .Replace("System.Boolean", "boolean")
            .Replace("System.String", "string")
            .Replace("System.Object", "any")
            .Replace("System.Void", "void");

            return ts;
        }

        // TypeScriptでは実現できない型等の修飾は、仕方がないので、消す
        static string DeleteImpossibleQualifier(string ts)
        {
            ts = ts.Replace("&", "")
            .Replace("*", "");

            return ts;
        }

        // ILの名前空間表記をTypeScriptの名前空間の表記に
        static string ILAssemblyNameSpaceSplitSimbolToTypeScriptNameSpaceSplitSimbol(string ts)
        {
            ts = ts.Replace("+", ".");

            return ts;
        }

        // ILのGenericの表記を、TypeScriptのGenericの表記に
        static string ILAssemblyGenericToTypeScriptGeneric(string ts)
        {
            ts = Regex.Replace(ts, @"`1\[\[(.+?),.+?\]\]", "`1[$1]");
            ts = Regex.Replace(ts, @"`1\<\[(.+?),.+?\]\>", "`1<$1>");
            ts = Regex.Replace(ts, @"`2\[\[(.+?),.+?\]\\s*,\s*\[(.+?),.+?\]]", "`1[$1, $2]");
            ts = Regex.Replace(ts, @"`2\<\[(.+?),.+?\]\\s*,\s*\[(.+?),.+?\]>", "`1<$1, $2>");

            ts = Regex.Replace(ts, @"`1\[(.+?)\]", @"<$1>");
            // 離れてるタイプはTypeScriptでは許されないのでanyにする
            if (Regex.Match(ts, @"`1([^,]*?)\[(.+?)\]").Success)
            {
                return "any";
            }
            ts = Regex.Replace(ts, @"`1", "");

            ts = Regex.Replace(ts, @"`2\[([^,]+?)\s*,\s*([^,]+?)\]", @"<$1, $2>");
            // 離れてるタイプはTypeScriptでは許されないのでanyにする
            if (Regex.Match(ts, @"`2([^,]*?)\[([^,]+?)\s*,\s*([^,]+?)\]").Success)
            {
                return "any";
            }
            ts = Regex.Replace(ts, @"`2", "");

            ts = Regex.Replace(ts, @"`3\[([^,]+?)\s*,\s*([^,]+?),\s*([^,]+?)\]", @"<$1, $2, $3>");

            // 離れてるタイプはTypeScriptでは許されないのでanyにする
            if (Regex.Match(ts, @"`3([^,]*?)\[([^,]+?)\s*,\s*([^,]+?),\s*([^,]+?)\]").Success)
            {
                return "any";
            }
            ts = Regex.Replace(ts, @"`3", "");

            ts = Regex.Replace(ts, @"`4\[([^,]+?)\s*,\s*([^,]+?),\s*([^,]+?),\s*([^,]+?)\]", @"<$1, $2, $3, $4>");
            if (Regex.Match(ts, @"`4([^,]*?)\[([^,]+?)\s*,\s*([^,]+?),\s*([^,]+?),\s*([^,]+?)\]").Success)
            {
                return "any";
            }
            ts = Regex.Replace(ts, @"`4", "");

            ts = Regex.Replace(ts, @"`5\[([^,]+?)\s*,\s*([^,]+?),\s*([^,]+?),\s*([^,]+?),\s*([^,]+?)\]", @"<$1, $2, $3, $4, $5>");
            if (Regex.Match(ts, @"`5([^,]*?)\[([^,]+?)\s*,\s*([^,]+?),\s*([^,]+?),\s*([^,]+?),\s*([^,]+?)\]").Success)
            {
                return "any";
            }
            ts = Regex.Replace(ts, @"`5", "");

            ts = Regex.Replace(ts, @"`6\[([^,]+?)\s*,\s*([^,]+?),\s*([^,]+?),\s*([^,]+?),\s*([^,]+?),\s*([^,]+?)\]", @"<$1, $2, $3, $4, $5, $6>");
            if (Regex.Match(ts, @"`6([^,]*?)\[([^,]+?)\s*,\s*([^,]+?),\s*([^,]+?),\s*([^,]+?),\s*([^,]+?),\s*([^,]+?)\]").Success)
            {
                return "any";
            }
            ts = Regex.Replace(ts, @"`6", "");

            // どうもうまく変換できないことがあるのでパッチ

            return ts;

        }

        // CSharp⇒TypeScriptの表記に
        static string ReplaceCsToTs(string ts)
        {
            ts = ReplaceCsTypeToTsType(ts);
            ts = ILAssemblyNameSpaceSplitSimbolToTypeScriptNameSpaceSplitSimbol(ts);
            ts = DeleteImpossibleQualifier(ts);
            ts = ILAssemblyGenericToTypeScriptGeneric(ts);

            return ts;
        }

        // TypeScriptの値型
        static bool IsTypeScriptPrimitiveType(string ts)
        {
            if (ts == "string" || ts == "number" || ts == "boolean" || ts == "any" || ts == "void" ) {
                return true;
            }

            return false;
        }

        // TypeScriptの値型ではない
        static bool NeverTypeScriptPrimitiveType(String ts)
        {
            return !IsTypeScriptPrimitiveType(ts);
        }

        // C#のGeneric表記をISAssemblyのGeneric表記にする
        static string ReplaceCSGenericToILAssemblyGeneric(string ts)
        {
            ts = ts.Replace("[]", "");
            ts = ts.Replace("&", "");
            ts = Regex.Replace(ts, "<[^,>]+?>", @"`1");
            ts = Regex.Replace(ts, "<[^,>]+?, [^,>]+?>", @"`2");
            ts = Regex.Replace(ts, "<[^,>]+?, [^,>]+?, [^,>]+?>", @"`3");
            ts = Regex.Replace(ts, "<[^,>]+?, [^,>]+?, [^,>]+?, [^,>]+?>", @"`4");
            ts = Regex.Replace(ts, "<[^,>]+?, [^,>]+?, [^,>]+?, [^,>]+?, [^,>]+?>", @"`5");
            ts = Regex.Replace(ts, "<[^,>]+?, [^,>]+?, [^,>]+?, [^,>]+?, [^,>]+?>, [^,>]+?>", @"`6");

            return ts;
        }

        // 文字列でひとつづきになっている「名前空間.クラス名」を、「名前空間」と「クラス名」に分解して返す
        static (string strNameSpace, string strClassName) SplitStringNameSpaceAndClassName(string ts)
        {
            string[] splited = ts.Split('.');
            string strClassName = "";
            string strNameSpace = "NONE";
            if (splited.Length == 1)
            {
                strClassName = splited[0];
            }
            if (splited.Length > 1)
            {
                strClassName = splited[splited.Length - 1]; // 最後の要素がクラス名
                string[] namespace_splited = new string[splited.Length - 1];
                Array.Copy(splited, namespace_splited, namespace_splited.Length);
                string ns = String.Join(".", namespace_splited);
                strNameSpace = ns;
            }

            return (strNameSpace, strClassName);
        }

        static void SWTabSpace(int cnt)
        {
            SW.Write(new string(' ', cnt * 4));
        }

    }
}
