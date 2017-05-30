using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace WinAssemblyToTypeScriptDeclare
{
    partial class WinAssemblyToTypeScriptDeclare
    {
        // 処理する必要があるリスト。
        // <名前空間, クラス名, 処理済みかどうか>
        class TaskItem
        {
            public string strNameSpace { get; set; }
            public string strClassName { get; set; }
            public int Status { get; set; }
            public int Nest { get; set; }
        }
        static List<TaskItem> TaskItems = new List<TaskItem>();

        static void DoNextTask()
        {
            var notDone = TaskItems.Find((tsk) => { return tsk.Status == 0; });

            // まだ未処理のものがある。
            if (notDone != null)
            {
                string[] not_done_args = { notDone.strNameSpace, notDone.strClassName, notDone.Nest >= m_AnalyzeDeepLevel - 1 ? "-deep:0" : "", "-deep:" + m_AnalyzeDeepLevel }; // ずっとやり続けると終わりがなくなるので、anyで
                try
                {
                    // 無限ループにならないように、実際には未発見だとしても「処理した」ということにする。
                    notDone.Status = 1;
                    notDone.Nest++;
                    AnalyzeAll(not_done_args);
                    notDone.Status = 2;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }

        // 名前空間＋クラス名を、新たに分析するべきタスクとして乗せる
        static void RegistClassTypeToTaskList(string ts)
        {
            ts = ReplaceCSGenericToILAssemblyGeneric(ts);
            var (strNameSpace, strClassName) = SplitStringNameSpaceAndClassName(ts);

            // 新たに登録しようとしているクラスがすでにタスクにある？
            var TaskItem = TaskItems.Find((tsk) => { return tsk.strClassName == strClassName && tsk.strNameSpace == strNameSpace; });
            // 無いなら
            if (TaskItem == null)
            {
                // タスクに登録
                TaskItems.Add(new TaskItem { strClassName = strClassName, strNameSpace = strNameSpace, Status = 0 });
            }
        }


    }


}
