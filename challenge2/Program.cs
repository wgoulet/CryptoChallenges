using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoChallengesSet1
{
    class Program
    {
        /* Fixed XOR
            Write a function that takes two equal-length buffers and produces their XOR combination.

            If your function works properly, then when you feed it the string:

            1c0111001f010100061a024b53535009181c
            ... after hex decoding, and when XOR'd against:

            686974207468652062756c6c277320657965
            ... should produce:

            746865206b696420646f6e277420706c6179
         */
        static void Main(string[] args)
        {
            string s1 = "1c0111001f010100061a024b53535009181c";
            string s2 = "686974207468652062756c6c277320657965";
            byte[] b1 = Program.hex2bytearray(s1);
            byte[] b2 = Program.hex2bytearray(s2);
            if(b1.Length == b2.Length)
            {
                byte[] b3 = new byte[b1.Length];
                for(int i = 0;i<b1.Length;i++)
                {
                    b3[i] = Convert.ToByte(b1[i] ^ b2[i]);
                }
                System.Console.WriteLine(Program.bytearray2hexstr(b3));
                System.Console.ReadKey();

            }
        }
        
        static byte[] hex2bytearray(string instr)
        {
            StringBuilder tr = new StringBuilder();
            byte[] inbytes = new byte[instr.Length / 2];
            int bytect = 0;
            for (int i = 0; i < instr.Length; i += 2)
            {
                tr.Append(instr[i]);
                tr.Append(instr[i + 1]);
                inbytes[bytect] = Convert.ToByte(tr.ToString(), 16);
                tr.Clear();
                bytect++;
            }
            return inbytes;
        }

        static string bytearray2hexstr(byte[] inbytes)
        {
            StringBuilder tr = new StringBuilder();
            foreach(byte b in inbytes)
            {
                tr.Append(Convert.ToString(b, 16));
            }
            return tr.ToString();
        }
    }
}
