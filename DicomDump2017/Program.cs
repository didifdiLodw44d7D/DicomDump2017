using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace DicomDump2017
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = @"temp.dcm";

            //filename = "MRI.000";

            filename = @"MR_LEE_IR6a.dcm";

            //filename = @"cr.dcm";

            var engine = new SearchEngine(filename);

            var transfer_syntax = engine.CheckTransferSyntax();

            if ("1.2.840.10008.1.2.1" == transfer_syntax.Substring(0, transfer_syntax.Length-1))
            {
                engine.ParseTagElement();
            }
            else
            {
                // 圧縮済み(Jpeg Lossless)だったら、非圧縮に解凍する

                // テンポラリファイルネームの付与

                // 解凍

                //

                for (int i = 0; i < 10; i++)
                    Console.WriteLine("Hello Kitty");
            }
        }
    }
}

/*
 * <0002,0010> 転送構文で処理分岐
 * dcmdjpeg の迂回パスを作成
 * 部品化
 * オブジェクト指向への移行
 * bit_stored,high_bitの処理
 */