using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DicomDump2017
{
    class Program
    {
        // DICOMタグ・エレメントのリスト
        static Dictionary dictionary = new Dictionary();

        static void Main(string[] args)
        {
           //string filename = @"C:\Users\tellex\Downloads\CT_JPG_IR6a.dcm";

            string filename = @"mr.dcm";

            //string filename = @"C:\Users\tellex\Downloads\MR_JPG_IR6a.dcm";

            //string filename = "MRI.000";

            //string filename = @"C:\Users\tellex\Downloads\viewer-master\viewer-master\DICOMDump\DICOMDump\bin\Debug\temp.dcm";

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

                //バイト配列がタグ・エレメントリストの中にあったら
                if(res)
                {
                    Console.Write((file.Position-4) + ":");
                    Console.Write("<{0}, {1}>", tmp[1].ToString("X2") + tmp[0].ToString("X2"), tmp[3].ToString("X2") + tmp[2].ToString("X2"));
                    Console.Write("--->");

                    byte[] vr = new byte[2];

                    vr[0] = (byte)file.ReadByte();
                    vr[1] = (byte)file.ReadByte();

                    // ピクセルデータの格納域
                    if ((0xE0 == tmp[0] && 0x7F == tmp[1] && 0x10 == tmp[2] && 0x00 == tmp[3]) && (0x4F == vr[0] && 0x42 == vr[1]))
                    {
                        /*
                        byte[] len_tmp = new byte[6];

                        len_tmp[0] = (byte)file.ReadByte();
                        len_tmp[1] = (byte)file.ReadByte();
                        len_tmp[2] = (byte)file.ReadByte();
                        len_tmp[3] = (byte)file.ReadByte();
                        len_tmp[4] = (byte)file.ReadByte();
                        len_tmp[5] = (byte)file.ReadByte();

                        long data_len = 0;

                        int weight = 1;
                        foreach(var s in len_tmp)
                        {
                            data_len += s * weight;
                            weight *= 16;
                        }

                        Console.Write("VR-->(True) : ");
                        Console.WriteLine("ValueLength-->{0}", data_len);
                        */

                        Console.WriteLine();

                        return;
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

                            Console.WriteLine(val);

                        }
                        else
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
        static bool SearchTagElement(byte tag_high, byte tag_low, byte element_high, byte element_low)
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

        static bool SearchVR(byte b1, byte b2)
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
            if('I' == b1 && 'S' == b2)
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
    }
}
/*
 
    明示的・暗黙的VRの切り替え
        エレメント直後の2バイト
            VRリストに当て込んで、あたりがなければ
                5、6バイト目がデータ長の値7、8バイト目は読み飛ばしもしくは0チェックのみ
    7FE0、0010の例外
     
     */