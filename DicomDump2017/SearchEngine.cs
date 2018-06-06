using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;

namespace DicomDump2017
{
    class SearchEngine
    {
        // DICOMタグ・エレメントのリスト
        Dictionary dictionary = new Dictionary();
        string filename;
        string transfer_syntax = string.Empty;
        int height, width;
        int wc, ww;
        int bits_allocated, bits_stored, high_bits;


        public SearchEngine(string filename)
        {
            this.filename = filename;
        }

        public string CheckTransferSyntax()
        {
            FileStream file = new FileStream(filename, FileMode.Open);

            int i = 0;

            //ファイルの先頭が、ファイル全体の長さに収まっている場合のみ
            while (i < file.Length)
            {
                //タグ・エレメントのバイト列４バイトを読み込む
                byte[] tmp = new byte[4];

                for (int j = 0; j < 4; j++)
                    tmp[j] = (byte)file.ReadByte();

                //リスト検索にかける
                var res = SearchTagElement(tmp[0], tmp[1], tmp[2], tmp[3]);

                if (res)
                {
                    Console.Write((file.Position - 4) + ":");
                    Console.Write("<{0}, {1}>", tmp[1].ToString("X2") + tmp[0].ToString("X2"), tmp[3].ToString("X2") + tmp[2].ToString("X2"));
                    Console.Write("--->");

                    byte[] vr = new byte[2];

                    vr[0] = (byte)file.ReadByte();
                    vr[1] = (byte)file.ReadByte();

                    // VR リストの中に、バイト配列があるかどうかを検索する
                    var resVrType = SearchVR(vr[0], vr[1]);

                    Console.Write("VR-->(" + (char)vr[0] + (char)vr[1] + " : " + resVrType + ") : ");

                    //ひとまず、タグ・エレメント直後の位置に戻しておく
                    file.Seek(-2, SeekOrigin.Current);

                    // VR リストの中に合致するバイト配列があれば
                    if (resVrType)
                    {
                        if ('U' == (char)vr[0] && 'I' == (char)vr[1])
                        {
                            if (0x02 == tmp[0] && 0x00 == tmp[1] && 0x10 == tmp[2] && 0x00 == tmp[3])
                            {
                                file.Seek(2, SeekOrigin.Current);

                                byte[] data_len = new byte[2];

                                data_len[0] = (byte)file.ReadByte();
                                data_len[1] = (byte)file.ReadByte();

                                int len = 0;

                                len += data_len[0];
                                len += data_len[1] * 256;

                                Console.WriteLine("ValueLength-->" + len);

                                byte[] value = new byte[len];

                                for (int j = 0; j < len; j++)
                                {
                                    value[j] = (byte)file.ReadByte();
                                }

                                transfer_syntax = Encoding.UTF8.GetString(value);

                                // 転送構文が未記入の DICOM ファイルは、パースできない仕様。
                                //ファイルエンドで、終了する。

                                file.Close();

                                //MessageBox.Show(transfer_syntax);

                                return transfer_syntax;
                            }
                            // 転送構文の VR 値が、省略されている場合を前提としていない
                            else
                            {
                            }
                        }
                    }
                }

                i++;
            }

            file.Close();

            return transfer_syntax;
        }

        public bool ParseTagElement()
        {
            bool rtn = false;

            FileStream file = new FileStream(filename, FileMode.Open);

            int i = 0;

            while (i < file.Length)
            {
                //タグ・エレメントのバイト列４バイトを読み込む
                byte[] tmp = new byte[4];

                for (int j = 0; j < 4; j++)
                    tmp[j] = (byte)file.ReadByte();

                //リスト検索にかける
                var res = SearchTagElement(tmp[0], tmp[1], tmp[2], tmp[3]);

                //バイト配列がタグ・エレメントリストの中にあったら
                if (res)
                {
                    Console.Write((file.Position - 4) + ":");
                    Console.Write("<{0}, {1}>", tmp[1].ToString("X2") + tmp[0].ToString("X2"), tmp[3].ToString("X2") + tmp[2].ToString("X2"));
                    Console.Write("--->");

                    byte[] vr = new byte[2];

                    vr[0] = (byte)file.ReadByte();
                    vr[1] = (byte)file.ReadByte();

                    // ピクセルデータの格納域
                    if ((0xE0 == tmp[0] && 0x7F == tmp[1] && 0x10 == tmp[2] && 0x00 == tmp[3]) /* && (0x4F == vr[0] && 0x42 == vr[1]) */)
                    {
                        var l = file.Position;
                        i = (int)l;

                        Console.WriteLine("Hello = " + i);

                        FileInfo info = new FileInfo(filename);
                        long size = info.Length;
                        byte[] buffer = new byte[size];
                        file.Read(buffer, i - 6, (int)size - i - 6);


                        byte[] bytes = new byte[size - i];

                        Console.WriteLine(bytes.Length.ToString());

                        bufferCopy(bytes, buffer, i, (int)size - i);

                        Console.WriteLine("SIZE = " + size);

                        var bitmap = CreateBitmap(bytes, width, height);

                        Console.WriteLine();

                        break;
                    }

                    // VR リストの中に、バイト配列があるかどうかを検索する
                    var resVrType = SearchVR(vr[0], vr[1]);

                    Console.Write("VR-->(" + (char)vr[0] + (char)vr[1] + " : " + resVrType + ") : ");

                    //ひとまず、タグ・エレメント直後の位置に戻しておく
                    file.Seek(-2, SeekOrigin.Current);

                    // VR リストの中に合致するバイト配列があれば
                    if (resVrType)
                    {
                        if ('U' == (char)vr[0] && 'S' == (char)vr[1])
                        {
                            int a = 0;

                            // Rows
                            if (0x28 == tmp[0] && 0x00 == tmp[1] && 0x10 == tmp[2] && 0x00 == tmp[3])
                            {
                                a = GetValueByTagElement(file);
                                height = a;
                            }

                            // Coloumns
                            if (0x28 == tmp[0] && 0x00 == tmp[1] && 0x11 == tmp[2] && 0x00 == tmp[3])
                            {
                                a = GetValueByTagElement(file);
                                width = a;
                            }

                            // Bits Allocated
                            if (0x28 == tmp[0] && 0x00 == tmp[1] && 0x00 == tmp[2] && 0x01 == tmp[3])
                            {
                                a = GetValueByTagElement(file);
                                bits_allocated = a;
                            }

                            // Bits Stored
                            if (0x28 == tmp[0] && 0x00 == tmp[1] && 0x01 == tmp[2] && 0x01 == tmp[3])
                            {
                                a = GetValueByTagElement(file);
                                bits_stored = a;
                            }

                            // High Bits
                            if (0x28 == tmp[0] && 0x00 == tmp[1] && 0x02 == tmp[2] && 0x01 == tmp[3])
                            {
                                a = GetValueByTagElement(file);
                                high_bits = a;
                            }

                            Console.WriteLine("A = " + a);
                        }
                        else
                        {
                            file.Seek(2, SeekOrigin.Current);

                            byte[] data_len = new byte[2];

                            data_len[0] = (byte)file.ReadByte();
                            data_len[1] = (byte)file.ReadByte();

                            int len = 0;

                            len += data_len[0];
                            len += data_len[1] * 256;

                            Console.WriteLine("ValueLength-->" + len);

                            byte[] value = new byte[len];

                            for (int j = 0; j < len; j++)
                            {
                                value[j] = (byte)file.ReadByte();
                            }

                            var str = Encoding.UTF8.GetString(value);

                            Console.WriteLine(str);
                        }
                    }
                    // VR リストの中に、合致するバイト配列がなければ                   
                    else
                    {
                        //タグ・エレメントの直後の２バイトがデータ長の可能性があるので、２バイト読み込む
                        byte[] data_len = new byte[2];

                        data_len[0] = (byte)file.ReadByte();
                        data_len[1] = (byte)file.ReadByte();

                        int len = 0;

                        len += data_len[0];
                        len += data_len[1] * 256;

                        Console.WriteLine("ValueLength-->" + len);

                        file.Seek(2, SeekOrigin.Current);

                        byte[] value = new byte[len];

                        for (int j = 0; j < len; j++)
                        {
                            value[j] = (byte)file.ReadByte();
                        }

                        var str = Encoding.UTF8.GetString(value);

                        Console.WriteLine(str);
                    }

                }

                //タグ・エレメントが偶数バイトの配置で始まるので、２バイト巻き戻して再度検証する
                file.Seek(-2, SeekOrigin.Current);

                i++;
            }

            file.Close();

            rtn = true;

            return rtn;
        }

        private int GetValueByTagElement(FileStream file)
        {
            //２バイト進めて、データ長の２バイトを読み込むための準備をする
            file.Seek(2, SeekOrigin.Current);

            byte[] data_len = new byte[2];

            data_len[0] = (byte)file.ReadByte();
            data_len[1] = (byte)file.ReadByte();

            int len = 0;

            len += data_len[0];
            len += data_len[1] * 256;

            Console.WriteLine("ValueLength-->" + len);

            byte[] value = new byte[len];

            for (int j = 0; j < len; j++)
            {
                //value[j] = (byte)file.ReadByte();
                value[j] = (byte)file.ReadByte();
            }

            int val = value[1] * 256 + value[0];

            return val;
        }

        private void bufferCopy(byte[] obyte, byte[] ibyte, int istart, int length)
        {
            for (int k = 0; k < length; k++)
            {
                obyte[k] = ibyte[istart + k];
            }
        }

        /// <summary>
        /// DICOM タグ・エレメントのリストと、ファイルから読み込んだバイト列を比較する。
        /// 合致していたら、その先頭位置がタグ・エレメントの先頭位置になる。
        /// 合致判定とともに、discover フラグに真を入れて、再度同じバイト列があっても、合致と判定しないことにする。
        /// </summary>
        /// <param name="tag_high"></param>
        /// <param name="tag_low"></param>
        /// <param name="element_high"></param>
        /// <param name="element_low"></param>
        /// <returns></returns>
        private bool SearchTagElement(byte tag_high, byte tag_low, byte element_high, byte element_low)
        {
            bool result = false;

            //タグ・エレメントのリストをすべて回す
            for (int i = 0; i < dictionary.tag_element.Count; i++)
            {
                //タグ・エレメントの発見リストが偽の場合のみ検索する
                if (false == dictionary.tag_element[i].discover)
                {
                    //タグをの合致を検証する
                    if (dictionary.tag_element[i].tag_high == tag_high && dictionary.tag_element[i].tag_low == tag_low)
                    {
                        //エレメントの合致を検証する
                        if (dictionary.tag_element[i].element_high == element_high && dictionary.tag_element[i].element_low == element_low)
                        {
                            dictionary.tag_element[i].discover = true;
                            return true;
                        }
                    }
                }
            }

            return result;
        }

        private Bitmap CreateBitmap(byte[] source, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            byte[] rgb = new byte[width * height * 3];

            for (int i = 0; i < width * height; i++)
            {
                int value = source[2 * i] + source[2 * i + 1] * 256;
                //value >>= 4;//(bits_stored - high_bits);

                value >>= 2;

                rgb[3 * i] = (byte)value;
                rgb[3 * i + 1] = (byte)value;
                rgb[3 * i + 2] = (byte)value;

                //Console.WriteLine(value);
            }

            System.Runtime.InteropServices.Marshal.Copy(rgb, 0, ptr, width * height * 3);
            bitmap.UnlockBits(bmpData);
            bitmap.Save(@"output\sample.bmp");

            return bitmap;
        }

        private bool SearchVR(byte b1, byte b2)
        {
            bool result = false;

            if ('A' == b1 && 'E' == b2)
            {
                result = true;
            }
            if ('A' == b1 && 'S' == b2)
            {
                result = true;
            }
            if ('A' == b1 && 'T' == b2)
            {
                result = true;
            }
            if ('C' == b1 && 'S' == b2)
            {
                result = true;
            }
            if ('D' == b1 && 'A' == b2)
            {
                result = true;
            }
            if ('D' == b1 && 'S' == b2)
            {
                result = true;
            }
            if ('D' == b1 && 'T' == b2)
            {
                result = true;
            }
            if ('F' == b1 && 'L' == b2)
            {
                result = true;
            }
            if ('F' == b1 && 'D' == b2)
            {
                result = true;
            }
            if ('I' == b1 && 'O' == b2)
            {
                result = true;
            }
            if ('I' == b1 && 'S' == b2)
            {
                result = true;
            }
            if ('L' == b1 && 'O' == b2)
            {
                result = true;
            }
            if ('L' == b1 && 'T' == b2)
            {
                result = true;
            }
            if ('O' == b1 && 'B' == b2)
            {
                result = true;
            }
            if ('O' == b1 && 'F' == b2)
            {
                result = true;
            }
            if ('O' == b1 && 'W' == b2)
            {
                result = true;
            }
            if ('P' == b1 && 'N' == b2)
            {
                result = true;
            }
            if ('S' == b1 && 'H' == b2)
            {
                result = true;
            }
            if ('S' == b1 && 'L' == b2)
            {
                result = true;
            }
            if ('S' == b1 && 'Q' == b2)
            {
                result = true;
            }
            if ('S' == b1 && 'S' == b2)
            {
                result = true;
            }
            if ('S' == b1 && 'T' == b2)
            {
                result = true;
            }
            if ('T' == b1 && 'M' == b2)
            {
                result = true;
            }
            if ('U' == b1 && 'I' == b2)
            {
                result = true;
            }
            if ('U' == b1 && 'L' == b2)
            {
                result = true;
            }
            if ('U' == b1 && 'N' == b2)
            {
                result = true;
            }
            if ('U' == b1 && 'S' == b2)
            {
                result = true;
            }
            if ('U' == b1 && 'T' == b2)
            {
                result = true;
            }

            return result;
        }

        private void DebugWrite(byte[] bytes)
        {
            StreamWriter sw = new StreamWriter("test.txt");

            foreach (var s in bytes)
                sw.WriteLine(s.ToString("X2"));

            sw.Close();
        }

        // 設計が固まったら移動
        public string ConvertExplicitLittleEndianDicomFile(string input, string output)
        {
            //Processオブジェクトを作成
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            //ComSpec(cmd.exe)のパスを取得して、FileNameプロパティに指定
            p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            //出力を読み取れるようにする
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = false;
            //ウィンドウを表示しないようにする
            p.StartInfo.CreateNoWindow = true;
            //コマンドラインを指定（"/c"は実行後閉じるために必要）
            p.StartInfo.Arguments = @"/c dcmtk\bin\dcmdjpeg.exe " + input + " " + output;

            //起動
            p.Start();

            //出力を読み取る
            string results = p.StandardOutput.ReadToEnd();

            //プロセス終了まで待機する
            //WaitForExitはReadToEndの後である必要がある
            //(親プロセス、子プロセスでブロック防止のため)
            p.WaitForExit();
            p.Close();

            return results;
        }
    }
}
