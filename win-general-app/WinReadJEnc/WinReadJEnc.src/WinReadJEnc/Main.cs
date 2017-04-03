using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WinReadJEnc
{
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        public static void Main(String[] args)
        {
            if (args.Length < 1) {
                return;
            }

            string filepath = args[0];

            if (!File.Exists(filepath))
            {
                return;
            }

            var fileinfo = new FileInfo(filepath);

            //文字コード自動判別読み出しクラスを生成
            using (WinReadJEnc.FileReader reader = new FileReader(fileinfo))
            {
                //判別読み出し実行。判別結果はReadメソッドの戻り値で把握できます
                WinReadJEnc.CharCode c = reader.Read(fileinfo);
                //戻り値のNameプロパティから文字コード名を取得できます

                //GetEncoding()を呼び出すと、エンコーディングを取得できます
                System.Text.Encoding enc = c.GetEncoding();

                Console.WriteLine(enc.CodePage);
                Console.WriteLine(enc.WebName);
            }

        }
    }
}
