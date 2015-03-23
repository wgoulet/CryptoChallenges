using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace s2challenge2
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] inbytes = File.ReadAllBytes(".\\ch10.txt");
            byte[] key = Encoding.ASCII.GetBytes("YELLOW SUBMARINE");
            int blocksize = key.Length * 8; // blocksize is same as keysize
            AesCryptoServiceProvider acsp = new AesCryptoServiceProvider();
            KeySizes[] sizes = acsp.LegalKeySizes;
            acsp.Key = key;
            acsp.Mode = CipherMode.ECB;
            byte[] iv = new byte[acsp.BlockSize / 8]; // already 0's out the array, but lets be sure
            iv = Enumerable.Repeat<byte>(0, iv.Length).ToArray();
            acsp.IV = iv;
            ICryptoTransform aes = acsp.CreateDecryptor();
            int numblocks = inbytes.Length / blocksize;
            BinaryWriter writer = new BinaryWriter(new MemoryStream());
            BinaryReader reader = new BinaryReader(new MemoryStream(inbytes));
            List<byte[]> prevblocks = new List<byte[]>();
            prevblocks.Add(iv);
            for (int i = 0; i < numblocks; i++)
            {
                if(i != numblocks - 1)
                {
                    // Use normal transform until we get to the final block
                    byte[] iblock = reader.ReadBytes(blocksize / 8);
                    byte[] oblock = new byte[blocksize / 8];
                    aes.TransformBlock(iblock, 0, 16, oblock, 0);
                    prevblocks.Add(CryptoChallengesSet1.Transforms.xorbytes(oblock, prevblocks.Last()));
                }
                else
                {
                    byte[] iblock = reader.ReadBytes(blocksize / 8);
                    byte[] oblock = aes.TransformFinalBlock(iblock, 0, iblock.Length);
                }
            }
            StringBuilder sb = new StringBuilder();
            foreach (byte[] block in prevblocks)
                sb.Append(Encoding.ASCII.GetString(block));
            Console.WriteLine(sb.ToString());
            Console.ReadKey();
        }
    }
}
