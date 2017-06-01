using System;
using System.Collections.Generic;

namespace WinAssemblyToTypeScriptDeclare
{
    /// <summary>
    /// １つのアセンブリ中において…
    /// </summary>
    partial class WinAssemblyToTypeScriptDeclare
    {
        static bool m_isTypeAnyMode = false;

        static void PrintClassDetail(Type t, String _ns, int n)
        {
            SWTabSpace(n); SW.WriteLine("/**");
            SWTabSpace(n); SW.WriteLine("名前:{0}", t.Name.Replace("+", "."));
            SWTabSpace(n); SW.WriteLine("名前空間:{0}", _ns == "NONE" ? "無し" : _ns);
            SWTabSpace(n); SW.WriteLine("完全限定名:{0}", t.FullName);
            SWTabSpace(n); SW.WriteLine("このメンバを宣言するクラス:{0}", t.DeclaringType);
            SWTabSpace(n); SW.WriteLine("親クラス:{0}", t.BaseType);
            SWTabSpace(n); SW.WriteLine("属性:{0}", t.Attributes);
            SWTabSpace(n); SW.WriteLine("*/");
        }

        /// <summary>
        /// ネームスペースを全部分解して、連結リスト的なものへと収める
        /// </summary>
        /// <param name="_ns"></param>
        /// <returns></returns>
        static List<NameSpaceNested> GetNameSpaceNestedData(string _ns)
        {
            List<NameSpaceNested> nsList = new List<NameSpaceNested>();
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

            return nsList;
        }

        /// <summary>
        /// クラスの説明文のコメント部を出力
        /// </summary>
        /// <param name="t"></param>
        /// <param name="_ns"></param>
        /// <param name="nsList"></param>
        static void PrintClassCommentLabel(Type t, string _ns, List<NameSpaceNested> nsList)
        {
            int nLastNext = 0;
            for (int n = 0; n < nsList.Count; n++)
            {
                if (nsList[n].NameSpace != "any" && nsList[n].NameSpace != "NONE")
                {
                    SWTabSpace(n);
                    if (n == 0)
                    {
                        SW.Write("declare ");
                    }
                    SW.WriteLine("namespace " + nsList[n].NameSpace + " {");

                    // 一番深いネームスペースのところで…
                    if (n == nsList.Count - 1)
                    {
                        PrintClassDetail(t, _ns, n + 1);
                        nLastNext = n;
                        AnalyzeMemberInfo(t, n + 1);
                    }

                }
                else
                {
                    PrintClassDetail(t, _ns, 0);

                    SW.Write("declare ");
                    nLastNext = n;
                    AnalyzeMemberInfo(t, 0);
                }
            }

            for (int n = nLastNext; n >= 0; n--)
            {
                if (nsList[n].NameSpace != "any" && nsList[n].NameSpace != "NONE")
                {
                    SWTabSpace(n);
                    SW.WriteLine("}");
                }
            }
        }

        /// <summary>
        /// オブジェクトが指定の名前空間とクラス名なら分析
        /// </summary>
        /// <param name="t">比較対象のオブジェクト</param>
        /// <param name="strNameSpace">指定の名前空間</param>
        /// <param name="strClassName">指定のクラス名</param>
        static void AnalyzeAssembly(Type t, string strNameSpace, string strClassName)
        {

            // 通常のネームスペース系
            var cond1 = (t.Namespace == strNameSpace || strNameSpace == "any" || strNameSpace == "NONE") && t.Name == strClassName;

            // ネームスペースとクラス名のそれぞれは一致しないのに、合算すると一致するということは…
            // ネストクラスになっている可能性がある。これはTypeScriptでは表現できない。
            var fullname1 = t.FullName.Replace("+", ".");
            var fullname2 = strNameSpace + "." + strClassName;
            var cond2 = fullname1 == fullname2;

            string _ns = t.Namespace;

            // 通常の条件は満たさないが、名前空間とクラス名を全部くっつけると一致する場合は、TypeScriptで実現できないので
            // ちょっと構造を変えてパッチ。クラスも１つ名前空間としてしまう
            if (!cond1 && cond2)
            {
                _ns = strNameSpace;
            }

            if (cond1 || cond2)
            {
                if (_ns == null || _ns == "")
                {
                    _ns = "NONE";

                    // NameSpaceはちゃんと存在するのに、NONE指定ならやらない
                } else if (strNameSpace == "NONE") {
                    return;
                }

                // 対象の「名前空間、クラス名」の組み合わせはすでに、出力済み？
                var item = TaskItems.Find( (tsk) => { return tsk.strNameSpace == _ns && tsk.strClassName == t.Name; } );
                // すでに登録済みで、すでに処理済み
                if (item != null && item.Status >= TaskItem.DoStatus.Done)
                {
                    return;
                }

                // まだなので、新たなタスクを作る。今からこのまま実行するので「済状態」を同時に付ける
                if (item == null)
                {
                    item = new TaskItem { strNameSpace = _ns, strClassName = t.Name, Status = TaskItem.DoStatus.Done };
                    TaskItems.Add(item);
                } else
                {
                    item.Status = TaskItem.DoStatus.Done;
                }


                var nsList = GetNameSpaceNestedData(_ns);

                PrintClassCommentLabel(t, _ns, nsList);
            }

        }
    }
}