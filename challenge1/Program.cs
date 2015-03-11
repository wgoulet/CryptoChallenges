using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace challenge1
{
    class Program
    {
        /* Convert hex to base64
            The string:

            49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d
            Should produce:

            SSdtIGtpbGxpbmcgeW91ciBicmFpbiBsaWtlIGEgcG9pc29ub3VzIG11c2hyb29t
            So go ahead and make that happen. You'll need to use this code for the rest of the exercises.
         */
        static void Main(string[] args)
        {
            string instr = "49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d";
            StringBuilder tr = new StringBuilder();
            byte[] inbytes = new byte[instr.Length / 2];
            int bytect = 0;
            for(int i =0; i< instr.Length; i+=2)
            {
                tr.Append(instr[i]);
                tr.Append(instr[i + 1]);
                inbytes[bytect] = Convert.ToByte(tr.ToString(),16);
                tr.Clear();
                bytect++;
            }
            System.Console.WriteLine(Convert.ToBase64String(inbytes));
            System.Console.ReadKey();
        }
    }
}
