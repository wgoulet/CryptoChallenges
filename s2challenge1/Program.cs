using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s2challenge1
{
    class Program
    {
        static void Main(string[] args)
        {
            /*Implement PKCS#7 padding
                A block cipher transforms a fixed-sized block (usually 8 or 16 bytes) of plaintext into ciphertext. But we almost never want to transform a single block; we encrypt irregularly-sized messages.

                One way we account for irregularly-sized messages is by padding, creating a plaintext that is an even multiple of the blocksize. The most popular padding scheme is called PKCS#7.

                So: pad any block to a specific block length, by appending the number of bytes of padding to the end of the block. For instance,

                "YELLOW SUBMARINE"
                ... padded to 20 bytes would be:

                "YELLOW SUBMARINE\x04\x04\x04\x04"
            */
            string instr = "YELLOW SUBMARINE but I'm really not much of a \"fan\" per se";
            int blocksize = 256;
            byte[] outbytes = CryptoChallengesSet1.Transforms.pkcs7pad(Encoding.ASCII.GetBytes(instr), blocksize);
            StringBuilder sb = new StringBuilder();
            foreach(byte b in outbytes)
            {
                if((Convert.ToInt32(b) < 128) && (Convert.ToInt32(b) > 31))
                {
                    sb.Append(Convert.ToChar(b));
                }
                else
                {
                    sb.AppendFormat("{0:x2}", b);
                }
            }
            Console.WriteLine(sb.ToString());
            Console.ReadKey();
        }
    }
}
