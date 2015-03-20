using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoChallengesSet1
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             *  Burning 'em, if you ain't quick and nimble
                I go crazy when I hear a cymbal
                Encrypt it, under the key "ICE", using repeating-key XOR.

                In repeating-key XOR, you'll sequentially apply each byte of the key; the first byte of plaintext will be XOR'd against I, the next C, the next E, then I again for the 4th byte, and so on.

                It should come out to:

                0b3637272a2b2e63622c2e69692a23693a2a3c6324202d623d63343c2a26226324272765272
                a282b2f20430a652e2c652a3124333a653e2b2027630c692b20283165286326302e27282f
             */

            string s1 = "Burning 'em, if you ain't quick and nimble`r`nI go crazy when I hear a cymbal";
            System.Console.WriteLine(Transforms.EncryptRotXor(s1, "ICE"));
            System.Console.ReadKey();
            
        }
    }
}
