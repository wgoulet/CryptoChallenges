using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
            StreamReader reader = File.OpenText(".\\cdata6.txt");
            byte[] inbytes = Convert.FromBase64String(reader.ReadToEnd());
            int maxkeylen = 40;
            int mineditdist = int.MaxValue;
            MemoryStream memstr = new MemoryStream(inbytes);
            Dictionary<int, List<int>> keylenmap = new Dictionary<int, List<int>>();
            int index = 0;
            int readct = 0;
            for (int i = 2; i < maxkeylen; i++)
            {
                byte[] b1 = new byte[i];
                byte[] b2 = new byte[i];
                readct = memstr.Read(b1,0,i);
                memstr.Read(b2,0,i);
                index = (int)memstr.Position;
                int cdist = CryptoUtils.Detectors.HammingDistance(s1, s2);
                if ((cdist / i) < mineditdist)
                {
                    mineditdist = cdist / i;
                    if (!keylenmap.ContainsKey(mineditdist))
                        keylenmap[mineditdist] = new List<int>();
                    keylenmap[mineditdist].Add(i);
                }
            }
            foreach(int l in keylenmap[mineditdist])
            {
                System.Console.WriteLine(String.Format("Keylen {0} has smallest HammingDistance {1}",l,mineditdist));
            }

            System.Console.ReadKey();
        }


    }
}
