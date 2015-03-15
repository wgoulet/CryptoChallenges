using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoUtils;

namespace challenge6
{
    class Program
    {
        static void Main(string[] args)
        {
            string s1 = "this is a testb";
            string s2 = "wokka wokka!!!b3";
            int hdist = CryptoUtils.Detectors.HammingDistance(s1, s2);

        }

      
    }
}
