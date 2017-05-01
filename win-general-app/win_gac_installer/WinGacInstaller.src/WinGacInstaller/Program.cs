using System;
using System.Windows.Forms;

namespace WinGacInstaller
{
    public class WinGacInstaller
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowError("引数が不正です。");
                return;
            }

            System.EnterpriseServices.Internal.Publish pub = new System.EnterpriseServices.Internal.Publish();
            if (args[0] == "/i")
            {
                try
                {
                    //GACにインストール
                    pub.GacInstall(args[1]);
                }
                catch
                {
                    ShowError("GACへのインストールに失敗しました。");
                    return;
                }
            }
            else if (args[0] == "/u")
            {
                try
                {
                    //GACからアンインストール
                    pub.GacRemove(args[1]);
                }
                catch
                {
                    ShowError("GACからのアンインストールに失敗しました。");
                    return;
                }
            }
            else
            {
                ShowError("引数が不正です。");
                return;
            }

            System.Environment.ExitCode = 0;
        }

        private static void ShowError(string msg)
        {
            System.Windows.Forms.MessageBox.Show(null, msg + "\nGACへのインストール/アンインストールに失敗しました。", "エラー", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            System.Environment.ExitCode = 1;
        }
    }
}