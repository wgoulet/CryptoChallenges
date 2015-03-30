using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using CryptoChallengesSet1;

namespace s2challenge3
{
    class Program
    {
        /*An ECB/CBC detection oracle
        Now that you have ECB and CBC working:

        Write a function to generate a random AES key; that's just 16 random bytes.

        Write a function that encrypts data under an unknown key --- that is, a function that generates a random key and encrypts under it.

        The function should look like:

        encryption_oracle(your-input)
        => [MEANINGLESS JIBBER JABBER]
        Under the hood, have the function append 5-10 bytes (count chosen randomly) before the plaintext and 5-10 bytes after the plaintext.

        Now, have the function choose to encrypt under ECB 1/2 the time, and under CBC the other half (just use random IVs each time for CBC). Use rand(2) to decide which to use.

        Detect the block cipher mode the function is using each time. You should end up with a piece of code that, pointed at a block box that might be encrypting ECB or CBC, tells you which one is happening.
         */
        static void Main(string[] args)
        {
            byte[] key = Generators.genRandBytes(16, true);
            byte[] iv = Generators.genRandBytes(16, true);
            byte[] myecbpenguin = null;
            byte[] dbuf = File.ReadAllBytes(".\\tux.ppm");
            BinaryReader reader = new BinaryReader(new MemoryStream(dbuf));
            //Get PPM magic number
            while (reader.ReadByte() != 10) ;
            //Skip comment
            while (reader.ReadByte() != 10) ;
            //Finally skip color setup info
            while (reader.ReadByte() != 10) ;
            //Remaining bytes are stuffed into image
            myecbpenguin = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
            //if length of buffer is not a multiple of AES blocksize (16)
            //lets increase the length of output buffer to account for any 
            //required padding
            int outputencrbytes = myecbpenguin.Length;
            while (outputencrbytes % 16 != 0)
                outputencrbytes++;
            byte[] outbuf = new byte[16 + outputencrbytes];
            BinaryWriter writer = new BinaryWriter(new MemoryStream(outbuf));
            writer.Write(Encoding.ASCII.GetBytes("P"));
            writer.Write(Encoding.ASCII.GetBytes("6"));
            writer.Write((byte)0x0a);
            writer.Write(Encoding.ASCII.GetBytes("#"));
            writer.Write((byte)0x0a);
            writer.Write(Encoding.ASCII.GetBytes("1"));
            writer.Write(Encoding.ASCII.GetBytes("9"));
            writer.Write(Encoding.ASCII.GetBytes("6"));
            writer.Write((byte)0x20);
            writer.Write(Encoding.ASCII.GetBytes("2"));
            writer.Write(Encoding.ASCII.GetBytes("1"));
            writer.Write(Encoding.ASCII.GetBytes("6"));
            writer.Write((byte)0x0a);
            writer.Write(Encoding.ASCII.GetBytes("2"));
            writer.Write(Encoding.ASCII.GetBytes("5"));
            writer.Write(Encoding.ASCII.GetBytes("5"));
            writer.Write(Transforms.aesecb(key,myecbpenguin,Transforms.Encrypt,true));
            writer.Close();
            System.Console.WriteLine(Detectors.AESModeDetector(dbuf));
            File.WriteAllBytes(".\\ecbtux.jpg", outbuf);
            System.Console.ReadKey();
        }
    }
}
