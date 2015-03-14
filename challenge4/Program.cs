using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using CryptoUtils;

namespace challenge4
{
    class Program
    {
        /* Detect single-character XOR
            One of the 60-character strings in this file has been encrypted by single-character XOR.
            Find it.
            (Your code from #3 should help.)
         */
        static void Main(string[] args)
        {
            StreamReader reader = File.OpenText("cdata.txt");
            char key;
            string cleartext = string.Empty;
            while (!reader.EndOfStream)
            {
                // If there is a 10% or greater match of digraphs/trigraphs/doubles, assume its english cleartext
                cleartext = CryptoUtils.Transforms.DecryptXor(reader.ReadLine(), out key);
                if (CryptoUtils.Detectors.LikelyEnglish(cleartext) > 0.1)
                    System.Console.WriteLine(String.Format("Found text {0} with key {1} in input file", cleartext.Trim(), key));
            }
            System.Console.ReadKey();

        }

    }
}




