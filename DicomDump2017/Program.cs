using System;
using System.IO;
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
        enum LogAction { start = 0, end = 1, exception = 2 }

        static void Main(string[] args)
        {
            WriteLog(LogAction.start);

            try
            {
                string filename = string.Empty;

                //filename = @"dcm\temp.dcm";

                //filename = @"dcm\MRI.000";

                //filename = @"dcm\MR_LEE_IR6a.dcm";

                filename = @"dcm\cr.dcm";

                var engine = new SearchEngine(filename);

                var transfer_syntax = engine.CheckTransferSyntax();

                // <0002,0010> 転送構文で処理分岐
                if ("1.2.840.10008.1.2.1" == transfer_syntax.Substring(0, transfer_syntax.Length - 1))
                {
                    engine.ParseTagElement();
                }
                // 圧縮済み(Jpeg Lossless)だったら、解凍する
                else if ("1.2.840.10008.1.2.4.70" == transfer_syntax)
                {
                    // 解凍
                    engine.ConvertExplicitLittleEndianDicomFile(filename, @"output\temporary.dcm");

                    engine = new SearchEngine(@"output\temporary.dcm");

                    var ret = engine.ParseTagElement();

                    if (ret)
                    {
                        try
                        {
                            File.Delete(@"output\temporary.dcm");
                        }
                        catch
                        {
                            Console.WriteLine("ファイルを削除できませんでした");
                        }
                    }
                }
            }
            catch
            {
                WriteLog(LogAction.exception);
            }
            finally
            {
                WriteLog(LogAction.end);
            }
        }

        static void WriteLog(LogAction act)
        {
            StreamWriter sw = new StreamWriter("pilot_log.txt", true, Encoding.UTF8);

            if (LogAction.start == act)
                sw.WriteLine("試験を開始しました <" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ">");

            if (LogAction.end== act)
                sw.WriteLine("試験を終了しました <" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ">");

            if (LogAction.exception == act)
                sw.WriteLine(" ---> 試験が中断しました <" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ">");

            sw.Close();
        }
    }
}

/*
 * dcmdjpeg の迂回パスを作成
 * オブジェクト指向への移行
 * bit_stored,high_bitの処理
 */