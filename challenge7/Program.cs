using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace challenge7
{
    class Program
    {
        /* The Base64-encoded content in this file has been encrypted via AES-128 in ECB mode under the key

            "YELLOW SUBMARINE".
            (case-sensitive, without the quotes; exactly 16 characters; I like "YELLOW SUBMARINE" because it's exactly 16 bytes long, and now you do too).

            Decrypt it. You know the key, after all.
         */
        static void Main(string[] args)
        {
            //http://cryptopals.com/static/challenge-data/7.txt
            byte[] inbytes = Convert.FromBase64String(getWebStuff("http://cryptopals.com/static/challenge-data/7.txt").Result);
            byte[] outbytes = new byte[inbytes.Length];
            byte[] block;
            byte[] key = Encoding.ASCII.GetBytes("YELLOW SUBMARINE");
            AesCryptoServiceProvider acsp = new AesCryptoServiceProvider();
            acsp.Mode = CipherMode.ECB;
            acsp.KeySize = key.Length * 8; // 16 bytes * 8 bits/byte = 128 although this is superfluous since AES-128 means 128 bit key
            acsp.Padding = PaddingMode.PKCS7; // Default padding mode for PKCS7; I tried none but discovered that the input data is in fact padded
            MemoryStream inst = new MemoryStream(inbytes);
            MemoryStream outs = new MemoryStream(outbytes);
            BinaryReader rea = new BinaryReader(inst);
            BinaryWriter writer = new BinaryWriter(outs);
            ICryptoTransform decr = acsp.CreateDecryptor(key, null);
            block = new byte[decr.InputBlockSize];
            int index = 0;
            int oindex = 0;
            // Decrypt block by block, when we find that we are near the end of the stream
            // of input data, perform a final transform.
            while((index = (int)rea.BaseStream.Position) < (int)rea.BaseStream.Length)
            {
                if(((int)rea.BaseStream.Length - index) <= decr.InputBlockSize)
                {
                    // Append last block to output byte stream
                    // by rewinding the output stream pointer oblock bytes
                    // from the end and writing the oblock to the output bytes
                    block = rea.ReadBytes(decr.InputBlockSize);
                    byte[] oblock = decr.TransformFinalBlock(block, 0, decr.InputBlockSize);
                    writer.BaseStream.Seek(-(long)oblock.Length, SeekOrigin.End);
                    writer.Write(oblock);
                    continue;
                }
                block = rea.ReadBytes(decr.InputBlockSize);
                decr.TransformBlock(block, 0, decr.InputBlockSize, outbytes, oindex);
                oindex = index;
            }
            System.Console.WriteLine(Encoding.ASCII.GetString(outbytes));
            System.Console.ReadKey();
        }

        static async Task<string> getWebStuff(string uri)
        {
            HttpClient cli = new HttpClient();
            string result = await cli.GetStringAsync(uri);
            return result;

        }
    }
}
