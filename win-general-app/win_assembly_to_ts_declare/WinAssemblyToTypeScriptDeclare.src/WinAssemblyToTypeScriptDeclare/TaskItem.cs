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
            public enum DoStatus { Unregist, Regist, Done };

            public string strNameSpace { get; set; }
            public string strClassName { get; set; }
            public DoStatus Status { get; set; }
            public int Nest { get; set; }
        }
        static List<TaskItem> TaskItems = new List<TaskItem>();

        static TaskItem GetNextTask()
        {
            var nextTask = TaskItems.Find((tsk) => { return tsk.Status == TaskItem.DoStatus.Unregist; });

            return nextTask;
        }


        static void DoNextTask(TaskItem nextTask)
        {
            if (nextTask != null)
            {
                string[] next_analyze_args = { nextTask.strNameSpace, nextTask.strClassName, nextTask.Nest >= m_AnalyzeDeepLevel - 1 ? "-deep:0" : "", "-deep:" + m_AnalyzeDeepLevel }; // ずっとやり続けると終わりがなくなるので、anyで
                try
                {
                    // 無限ループにならないように、実際には未発見だとしても「処理した」ということにする。
                    nextTask.Status = TaskItem.DoStatus.Regist;
                    nextTask.Nest++;
                    AnalyzeAll(next_analyze_args);
                    nextTask.Status = TaskItem.DoStatus.Done;
                }
                catch (Exception e)
                {
                    SW.WriteLine(e.Message);
                }

            }
        }

        // 名前空間＋クラス名を、新たに分析するべきタスクとして乗せる
        static void RegistClassTypeToTaskList(string ts)
        {
            ts = ReplaceCSGenericToILAssemblyGeneric(ts);
            var (strNameSpace, strClassName) = SplitStringNameSpaceAndClassName(ts);

            // 新たに登録しようとしているクラスがすでにタスクにある？
            var sameTask = TaskItems.Find((tsk) => { return tsk.strClassName == strClassName && tsk.strNameSpace == strNameSpace; });
            // 無いなら
            if (sameTask == null)
            {
                // タスクに登録
                TaskItems.Add(new TaskItem { strClassName = strClassName, strNameSpace = strNameSpace, Status = TaskItem.DoStatus.Unregist });
            }
        }
    }


}
