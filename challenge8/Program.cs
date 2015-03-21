using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace challenge8
{
    class Program
    {
        static void Main(string[] args)
        {
            /* In this file are a bunch of hex-encoded ciphertexts.
             One of them has been encrypted with ECB.
             Detect it.
             Remember that the problem with ECB is that it is stateless and deterministic; the same 16 byte plaintext block will always produce the same 16 byte ciphertext.
             */
            char[] delims = { '\n' };
            Dictionary<string, List<int>> linemap = new Dictionary<string, List<int>>();
            byte[] block = new byte[16];
            HashSet<byte[]> dedup = new HashSet<byte[]>(new ByteHashComparer());
            string[] inlines = getWebStuff("http://cryptopals.com/static/challenge-data/8.txt").Result.Split(delims);
            // First, load the lines into a dictionary that we can use to store info about them in
            foreach (string s in inlines)
            {
                linemap.Add(s, new List<int>());
            }
            // Look for any strings that have repeated 16 byte blocks in them
            byte[] b1 = {0xff,0xfa,0x3b};
            byte[] b2 = {0xff,0xfa,0x3b};
            dedup.Add(b1);
            if(!dedup.Contains(b2))
                dedup.Add(b2);
            foreach (KeyValuePair<string, List<int>> e in linemap)
            {
                byte[] inbytes = Convert.FromBase64String(e.Key);
                MemoryStream str = new MemoryStream(inbytes);
                BinaryReader reader = new BinaryReader(str);
                int numblocks = inbytes.Length / 16;
                for (int i = 0; i < numblocks; i++)
                {
                    block = reader.ReadBytes(16);
                    if (!dedup.Contains(block))
                        dedup.Add(block);
                    else
                        linemap[e.Key].Add(1);

                }
            }
            foreach(KeyValuePair<string,List<int>> e in linemap)
            {
                if (e.Value.Count == 0)
                    continue;
                if (e.Value.First<int>() > 0)
                    System.Console.WriteLine(e.Key);
            }
            System.Console.ReadKey();
        }

        static async Task<string> getWebStuff(string uri)
        {
            // If we already saved this content to a local resource,
            // return that instead.
            StreamReader rdr = null;
            string result = string.Empty;
            if (File.Exists(".\\cache"))
            {
                rdr = new StreamReader(".\\cache");
                result = await rdr.ReadToEndAsync();
                return result;
            }
            HttpClient cli = new HttpClient();
            result = await cli.GetStringAsync(uri);
            StreamWriter str = new StreamWriter(File.OpenWrite(".\\cache"));
            str.WriteLine(result);
            str.Close();
            return result;

        }
    }
    public class ByteHashComparer : EqualityComparer<byte[]>
    {
        public override bool Equals(byte[] x, byte[] y)
        {
            if (x.Length != y.Length)
                return false;
            for(int i = 0;i< x.Length;i++)
            {
                if (x[i] != y[i])
                    return false;
            }
            return true;
        }

        public override int GetHashCode(byte[] obj)
        {
            int hashcode = 0;
            foreach (byte b in obj)
               hashcode += b.GetHashCode();
            return hashcode;
        }
    }
}
