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
           // string s1 = "this is a test";
            //string s2 = "wokka wokka!!!";
            //int hdist = CryptoUtils.Detectors.HammingDistance(s1, s2);
            //hdist = CryptoUtils.Detectors.HammingDistance(Encoding.ASCII.GetBytes(s1.ToCharArray()), Encoding.ASCII.GetBytes(s2.ToCharArray()));
            StreamReader reader = File.OpenText(".\\cdata6.txt");
            byte[] inbytes = Convert.FromBase64String(reader.ReadToEnd());
            int maxkeylen = 40;
            double mineditdist = double.MaxValue;
            MemoryStream memstr = new MemoryStream(inbytes);
            Dictionary<double, List<int>> keylenmap = new Dictionary<double, List<int>>();
            int index = 0;
            int readct = 0;
            for (int i = 2; i < maxkeylen; i++)
            {
                byte[] b1 = new byte[i];
                StringBuilder sb = new StringBuilder();
                byte[] b2 = new byte[i];
                readct = memstr.Read(b1,0,i);
                memstr.Read(b2,0,i);
                int cdist = CryptoUtils.Detectors.HammingDistance(b1, b2);
                //if ((cdist / i) <= mineditdist)
                //{
                    mineditdist = cdist / (double)i;
                    if (!keylenmap.ContainsKey(mineditdist))
                        keylenmap[mineditdist] = new List<int>();
                    keylenmap[mineditdist].Add(i);
                //}
                memstr.Seek(0, SeekOrigin.Begin);
            }
            double[] sortlist = keylenmap.Keys.ToArray();

            Array.Sort(sortlist);

            foreach (double d in new ArraySegment<double>(sortlist, 0, 20))
            {

                foreach(int l in keylenmap[d])
                //for (int l = 2; l < 60; l++)
                {
                    System.Console.WriteLine(String.Format("Keylen {0} has smallest HammingDistance {1}", l, d));
                    // Break the ciphertext
                    memstr.Seek(0, SeekOrigin.Begin);
                    List<byte[]> cblocks = new List<byte[]>();
                    // if inbytes count is not a multiple of keylength, pad it with 0s
                    if (((int)memstr.Length % l) != 0)
                    {
                        // Copy incoming memory stream to a new buffer with padding
                        int plen = (int)memstr.Length;
                        while (plen % l != 0)
                            plen++;
                        byte[] newbuf = new byte[plen];
                        memstr.Read(newbuf, 0, (int)memstr.Length);
                        memstr.Dispose();
                        memstr = new MemoryStream(newbuf);
                    }
                    int blockcount = (int)memstr.Length / l;
                    for (int i = 0; i < blockcount; i++)
                    {
                        byte[] block = new byte[l];
                        memstr.Read(block, 0, l);
                        cblocks.Add(block);
                    }
                    List<byte[]> tblocks = new List<byte[]>();
                    for (int bindex = 0; bindex < l; bindex++)
                    {
                        index = 0;
                        byte[] tblock = new byte[blockcount];
                        foreach (byte[] block in cblocks)
                        {
                            tblock[index] = block[bindex];
                            index++;
                        }
                        tblocks.Add(tblock);
                    }
                    List<char> keys = new List<char>();
                    List<string> ptext = new List<string>();
                    foreach (byte[] tblock in tblocks)
                    {
                        char key = char.MinValue;
                        ptext.Add(CryptoUtils.Transforms.FindKeyDecryptXor(string.Empty, out key, tblock));
                        keys.Add(key);
                    }
                    StringBuilder sb = new StringBuilder();
                    sb.Append(keys.ToArray());
                    System.Console.WriteLine(sb.ToString());
                }
            }

            // From above, we know the probable key. Let user type in key from best looking output displayed above
            System.Console.WriteLine("Enter the guessed key from the list above:");
            string keystr = System.Console.ReadLine();

            memstr.Seek(0, SeekOrigin.Begin);
            System.Console.WriteLine(CryptoUtils.Transforms.DecryptRotXor(inbytes, keystr));
            System.Console.ReadKey();
        }


    }
}
