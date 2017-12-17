/*
 * Copyright (c) 2017 Akitsugu Komiyama
 * under the MITLicense
 */


using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Linq;


namespace WinApplicationMutexLaunch
{

    class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr handletest01);

        [DllImport("user32.dll")]
        static extern bool OpenIcon(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        enum VirtualKeyStates : int
        {
            VK_SHIFT = 0x10,
            VK_CONTROL = 0x11,
        }

        [DllImport("USER32.dll")]
        static extern short GetKeyState(VirtualKeyStates nVirtKey);

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                return;
            }

            // 第１引数はターゲットとなるexeのフルパス
            string strTargetExeFullPath = args[0];
            if (!File.Exists(strTargetExeFullPath))
            {
                Console.Write($"File Not Exist: {strTargetExeFullPath}");
                return;
            }
            // そのディレクトリを求める
            string strTargetDir = Path.GetDirectoryName(strTargetExeFullPath);

            // ターゲットのウィンドウ名
            string strTargetWndClassName = args[1];

            IntPtr hWndHandle = FindWindow(strTargetWndClassName, null);

            // SHIFTキーを押しているか、対象のアプリが立ち上がっていないならば･･･
            if (GetKeyState(VirtualKeyStates.VK_SHIFT) < 0 || hWndHandle == IntPtr.Zero)
            {

                // カレントディレクトリ変更
                Directory.SetCurrentDirectory(strTargetDir);

                // 最初の２つの要素はカットして、残りは空文字が現れるまで
                args = args.Skip(2).TakeWhile(s => s != string.Empty).ToArray();

                // １つずつダブルクォートで囲む
                args = args.Select(s => $"\"{s}\"").ToArray();

                // 半角空白で繋げて１つにする
                string strPrms = string.Join(" ", args);

                // 外部プログラムとして実行
                Process.Start(strTargetExeFullPath, strPrms);

            }
            else
            {
                // もしアイコン化していたら元に戻す
                if (IsIconic(hWndHandle))
                {
                    // 元に戻す
                    OpenIcon(hWndHandle);
                }

                // それがアクティブだ!!
                SetForegroundWindow(hWndHandle);
            }

        }
    }
}
