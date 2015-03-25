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
            byte[] iv = new byte[key.Length]; // already 0's out the array, but lets be sure
            iv = Enumerable.Repeat<byte>(0, iv.Length).ToArray();
            byte[] block = CryptoChallengesSet1.Transforms.aescbc(iv, key, inbytes, CryptoChallengesSet1.Transforms.Decrypt);
            StringBuilder sb = new StringBuilder();
            sb.Append(Encoding.ASCII.GetString(block));
            Console.WriteLine(sb.ToString());
            Console.ReadKey();
        }
    }
}
