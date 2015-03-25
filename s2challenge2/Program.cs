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
            byte[] inbytes = Convert.FromBase64String(File.ReadAllText(".\\ch10.txt"));
            byte[] key = Encoding.ASCII.GetBytes("YELLOW SUBMARINE");
            int blocksize = key.Length * 8; // blocksize is same as keysize
            AesCryptoServiceProvider acsp = new AesCryptoServiceProvider();
            KeySizes[] sizes = acsp.LegalKeySizes;
            acsp.Key = key;
            acsp.Mode = CipherMode.ECB;
            acsp.Padding = PaddingMode.None;
            byte[] iv = new byte[acsp.BlockSize / 8]; // already 0's out the array, but lets be sure
            iv = Enumerable.Repeat<byte>(0, iv.Length).ToArray();
            ICryptoTransform aes = acsp.CreateDecryptor();
            // Since we're gonna be reading in bytes, the numblocks
            // needs to be calculated as number of 16 byte blocks
            int numblocks = inbytes.Length / (blocksize / 8);
            BinaryWriter writer = new BinaryWriter(new MemoryStream());
            BinaryReader reader = new BinaryReader(new MemoryStream(inbytes));
            List<byte[]> prevblocks = new List<byte[]>();
            byte[] prevblock = null;
            prevblocks.Add(iv);
            for (int i = 0; i < numblocks; i++)
            {
                if (i != numblocks - 1)
                {
                    // Use normal transform until we get to the final block
                    byte[] iblock = reader.ReadBytes(blocksize / 8);
                    byte[] oblock = new byte[blocksize / 8];
                    aes.TransformBlock(iblock, 0, 16, oblock, 0);
                    // Recover previous ciphertext block, so back
                    // up 2 blocks worth (16 bytes) only if we have
                    // already used the IV
                    if (prevblocks.Count == 1)
                    {
                        prevblock = prevblocks.Last();
                    }
                    else
                    {
                        long currpos = reader.BaseStream.Position;
                        reader.BaseStream.Seek(currpos - 32, SeekOrigin.Begin);
                        prevblock = reader.ReadBytes(blocksize / 8);
                        reader.BaseStream.Seek(currpos, SeekOrigin.Begin);
                    }
                    prevblocks.Add(CryptoChallengesSet1.Transforms.xorbytes(prevblock, oblock));
                }
                else
                {
                    byte[] iblock = reader.ReadBytes((int)reader.BaseStream.Length - (int)reader.BaseStream.Position);
                    byte[] oblock = aes.TransformFinalBlock(iblock, 0, iblock.Length);
                    // Recover previous ciphertext block, so back
                    // up 2 blocks worth (16 bytes)
                    long currpos = reader.BaseStream.Position;
                    reader.BaseStream.Seek(currpos - 32, SeekOrigin.Begin);
                    prevblock = reader.ReadBytes(blocksize / 8);
                    reader.BaseStream.Seek(currpos, SeekOrigin.Begin);
                    prevblocks.Add(CryptoChallengesSet1.Transforms.xorbytes(prevblock, oblock));
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
