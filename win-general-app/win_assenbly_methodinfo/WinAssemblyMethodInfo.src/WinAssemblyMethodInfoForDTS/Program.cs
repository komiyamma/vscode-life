using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace WinAssemblyMethodInfoForDTS
{

    class NameSpaceNested : IComparable
    {
        public int NestLevel;
        public NameSpaceNested ParentNameSpace;
        public string NameSpace;
        public string FullNameSpace
        {
            get
            {
                return ParentNameSpace.FullNameSpace + "." + NameSpace;
            }
        }

        // 並べ替え用途
        public int CompareTo(object obj)
        {
            NameSpaceNested other = obj as NameSpaceNested;
            if (this.NestLevel < other.NestLevel)
            {
                return -1;
            }
            if (this.NestLevel == other.NestLevel)
            {
                return this.FullNameSpace.CompareTo(other.FullNameSpace);
            }
            if (this.NestLevel > other.NestLevel)
            {
                return +1;
            }
            return 0;
        }

        //objと自分自身が等価のときはtrueを返す
        public override bool Equals(object obj)
        {
            //objがnullか、型が違うときは、等価でない
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            //FullNameSpaceで比較する
            NameSpaceNested c = (NameSpaceNested)obj;
            return (this.FullNameSpace == c.FullNameSpace);
        }

        //Equalsがtrueを返すときに同じ値を返す
        public override int GetHashCode()
        {
            return this.FullNameSpace.GetHashCode();
        }

    }

    class Program
    {
        static List<NameSpaceNested> nsList;


        static string ReplaceCsToTsType(string s)
        {
            s = s.Replace("System.Int32", "number")
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
            .Replace("+", ".");

            Regex rg = new Regex(@"\[.+?\]");
            s = Regex.Replace(s, @"\[.+?\]", "");
            s = Regex.Replace(s, @"`1", "<T>");
            s = Regex.Replace(s, @"`2", "<T, U>");
            s = Regex.Replace(s, @"`3", "<T, U, V>");

            return s;

        }

        static bool isAnyMode = false;
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                return;
            }
            if (args.Length >= 3)
            {
                if (args[2] == "any")
                {
                    isAnyMode = true;
                }
            }

            if (args.Length >= 4)
            {
                if (args[3] == "any")
                {
                    isAnyMode = true;
                }
            }

            //"C:\test"以下のファイルをすべて取得する
            IEnumerable<string> files2 = System.IO.Directory.EnumerateFiles(@".", "*.dll");
            //ファイルを列挙する
            foreach (string f in files2)
            {
                try
                {
                    var full = System.IO.Path.GetFullPath(f);
                    Assembly asm = Assembly.LoadFile(full);
                    Type[] types = asm.GetTypes();
                    foreach (Type t in types)
                    {
                        AnalyzeAssembly(t, args[0], args[1]);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            //"C:\test"以下のファイルをすべて取得する
            IEnumerable<string> files = System.IO.Directory.EnumerateFiles(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319", "*.dll");
            //ファイルを列挙する
            foreach (string f in files)
            {
                try
                {
                    Assembly asm = Assembly.LoadFile(f);
                    Type[] types = asm.GetTypes();
                    foreach (Type t in types)
                    {
                        AnalyzeAssembly(t, args[0], args[1]);
                    }
                }
                catch (Exception)
                {

                }
            }


        }


        static void AnalyzeAssembly(Type t, string strNameSpace, string strClassName)
        {
            nsList = new List<NameSpaceNested>();

            if ((t.Namespace == strNameSpace || strNameSpace == "any") && t.Name == strClassName)
            {
                string _ns = t.Namespace;
                if (_ns == null || _ns == "")
                {
                    _ns = "any";
                }
                Console.WriteLine("名前:{0}", t.Name);
                Console.WriteLine("名前空間:{0}", _ns == "any" ? "無し" : _ns);
                Console.WriteLine("完全限定名:{0}", t.FullName);
                Console.WriteLine("このメンバを宣言するクラス:{0}", t.DeclaringType);
                Console.WriteLine("親クラス:{0}", t.BaseType);
                Console.WriteLine("属性:{0}", t.Attributes);
                Console.WriteLine();


                var exist = nsList.Find((ns) => { return ns.FullNameSpace == _ns; });
                if (exist == null)
                {
                    string[] s = _ns.Split('.');
                    List<string> nls = new List<string>();
                    nls.AddRange(s);

                    int nNextLevel = 0;

                    NameSpaceNested cut_parent = null;
                    while (nls.Count > 0)
                    {
                        var ns = new NameSpaceNested();
                        ns.ParentNameSpace = cut_parent;
                        ns.NestLevel = nNextLevel;
                        ns.NameSpace = nls[0];
                        cut_parent = ns;
                        nls.RemoveAt(0);
                        nNextLevel++;

                        nsList.Add(ns);
                    }
                }

                nsList.Sort();

                int nLastNext = 0;
                for (int n = 0; n < nsList.Count; n++)
                {
                    if (nsList[n].NameSpace != "any")
                    {
                        Console.Write(new string(' ', 4 * n));
                        if (n == 0)
                        {
                            Console.Write("declare ");
                        }
                        Console.WriteLine("namespace " + nsList[n].NameSpace + " {");

                        // 一番深いネームスペースのところで…
                        if (n == nsList.Count - 1)
                        {
                            nLastNext = n;
                            AnalyzeMemberInfo(t, n + 1);
                        }

                    }
                    else
                    {
                        Console.Write("declare ");
                        nLastNext = n;
                        AnalyzeMemberInfo(t, 0);
                    }
                }

                for (int n = nLastNext; n >= 0; n--)
                {
                    if (nsList[n].NameSpace != "any")
                    {
                        Console.Write(new string(' ', 4 * n));
                        Console.WriteLine("}");
                    }
                }
            }

        }

        static bool IsNoTypeScriptClass(String str)
        {
            if (str != "string" && str != "number" && str != "boolean")
            {
                return true;
            }
            return false;
        }

        static void AnalyzeMemberInfo(Type t, int nestLevel)
        {
            Console.Write(new string(' ', nestLevel * 4));
            Console.WriteLine("interface " + ReplaceCsToTsType(t.Name) + " {");
            AnalyzeConstructorInfo(t, nestLevel);
            AnalyzeFieldInfo(t, nestLevel);
            AnalyzePropertyInfo(t, nestLevel);
            AnalyzeMethodInfo(t, nestLevel);
            Console.Write(new string(' ', (nestLevel) * 4));
            Console.WriteLine("}");

        }

        static void AnalyzeConstructorInfo(Type t, int nestLevel)
        {
            ConstructorInfo[] conss = t.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static );
            foreach (ConstructorInfo m in conss)
            {
                try
                {
                    Console.Write(new string(' ', (nestLevel + 1) * 4));
                    //アクセシビリティを表示
                    //ここではIs...プロパティを使っているが、
                    //Attributesプロパティを調べても同じ
                    /*
                    if (m.IsPublic)
                        Console.Write("public ");
                    if (m.IsPrivate)
                        Console.Write("private ");
                    if (m.IsAssembly)
                        Console.Write("internal ");
                    if (m.IsFamily)
                        Console.Write("protected ");
                    if (m.IsFamilyOrAssembly)
                        Console.Write("internal protected ");


                    //その他修飾子を表示
                    if (m.IsStatic)
                        Console.Write("static ");
                    if (m.IsAbstract)
                        Console.Write("abstract ");
                    else if (m.IsVirtual)
                        Console.Write(""); // NestConsoleWrite("virtual ");
                    */

                    //メソッド名を表示
                    Console.Write("new");

                    //パラメータを表示
                    ParameterInfo[] prms = m.GetParameters();
                    Console.Write("(");
                    for (int i = 0; i < prms.Length; i++)
                    {
                        ParameterInfo p = prms[i];
                        string s = ReplaceCsToTsType(p.ParameterType.ToString());
                        Console.Write(p.Name + ": " + s);
                        if (prms.Length - 1 > i)
                            Console.Write(", ");
                    }
                    Console.Write(")");

                    Console.Write(";");

                    Console.WriteLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }

        static void AnalyzeFieldInfo(Type t, int nestLevel)
        {

            //メソッドの一覧を取得する
            FieldInfo[] props = t.GetFields(
            BindingFlags.Public | /* BindingFlags.NonPublic | */
            BindingFlags.Instance | BindingFlags.Static);

            foreach (FieldInfo m in props)
            {
                try
                {
                    if (!m.IsPublic)
                    {
                        continue;
                    }
                    string s = m.FieldType.FullName;
                    s = ReplaceCsToTsType(s);
                    if (isAnyMode)
                    {
                        if (IsNoTypeScriptClass(s))
                        {
                            s = "any";
                        }
                    }

                    Console.Write(new string(' ', (nestLevel + 1) * 4));

                    // Console.Write("public ");
                    Console.Write(m.Name + " :" + s);

                    Console.Write(";");

                    Console.WriteLine();
                }
                catch (Exception e)
                {
                    // Console.WriteLine(e.Message);
                }
            }

        }

        static void AnalyzePropertyInfo(Type t, int nestLevel)
        {

            //メソッドの一覧を取得する
            PropertyInfo[] props = t.GetProperties(
            BindingFlags.Public | /* BindingFlags.NonPublic | */
            BindingFlags.Instance | BindingFlags.Static);

            foreach (PropertyInfo m in props)
            {
                try
                {

                    string s = m.PropertyType.FullName;
                    s = ReplaceCsToTsType(s);
                    if (isAnyMode)
                    {
                        if (IsNoTypeScriptClass(s))
                        {
                            s = "any";
                        }
                    }

                    Console.Write(new string(' ', (nestLevel + 1) * 4));

                    // Console.Write("public ");
                    Console.Write( m.Name + " :" + s);

                    Console.Write(";");

                    Console.WriteLine();
                }
                catch (Exception e)
                {
                    // Console.WriteLine(e.Message);
                }
            }

        }

        static void AnalyzeMethodInfo(Type t, int nestLevel)
        {
            // すでに出力済みなもの
            HashSet<string> doneWrite = new HashSet<string>();

            //メソッドの一覧を取得する
            MethodInfo[] methods = t.GetMethods(
            BindingFlags.Public | /* BindingFlags.NonPublic | */
            BindingFlags.Instance | BindingFlags.Static);

            foreach (MethodInfo m in methods)
            {
                try
                {

                    //特別な名前のメソッドは表示しない
                    if (m.IsSpecialName)
                    {
                        continue;
                    }

                    Console.Write(new string(' ', (nestLevel + 1) * 4));

                    //アクセシビリティを表示
                    //ここではIs...プロパティを使っているが、
                    //Attributesプロパティを調べても同じ
                    /*
                    if (m.IsPublic)
                        Console.Write("public ");
                    if (m.IsPrivate)
                        Console.Write("private ");
                    if (m.IsAssembly)
                        Console.Write("internal ");
                    if (m.IsFamily)
                        Console.Write("protected ");
                    if (m.IsFamilyOrAssembly)
                        Console.Write("internal protected ");
                    */

                    //その他修飾子を表示
                    /*
                    if (m.IsStatic)
                        Console.Write("static ");
                    if (m.IsAbstract)
                        Console.Write("abstract ");
                    else if (m.IsVirtual)
                        Console.Write(""); // NestConsoleWrite("virtual ");
                    */

                    //メソッド名を表示
                    Console.Write(m.Name);

                    //パラメータを表示
                    ParameterInfo[] prms = m.GetParameters();
                    Console.Write("(");
                    for (int i = 0; i < prms.Length; i++)
                    {
                        ParameterInfo p = prms[i];
                        string s = ReplaceCsToTsType(p.ParameterType.ToString());
                        if (isAnyMode)
                        {
                            if (IsNoTypeScriptClass(s))
                            {
                                s = "any";
                            }
                        }
                        Console.Write(p.Name + ": " + s);
                        if (prms.Length - 1 > i)
                            Console.Write(", ");
                    }
                    Console.Write(")");

                    Console.Write(": ");

                    //戻り値を表示
                    if (m.ReturnType == typeof(void))
                        Console.Write("void");
                    else
                    {
                        var s = ReplaceCsToTsType(m.ReturnType.ToString());
                        if (isAnyMode)
                        {
                            if (IsNoTypeScriptClass(s))
                            {
                                s = "any";
                            }
                        }
                        Console.Write(s + "");
                    }

                    Console.Write(";");

                    Console.WriteLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
