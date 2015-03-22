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
            //string[] inlines = { getWebStuff("http://cryptopals.com/static/challenge-data/8.txt").Result };
            
            // First, load the lines into a dictionary that we can use to store info about them in
            foreach (string s in inlines)
            {
                linemap.Add(s.Replace("\r\n", ""), new List<int>());
            }
            StringBuilder sb = new StringBuilder();
            // Look for any strings that have repeated 16 byte blocks in them
            int linemapct = 1;
            foreach (KeyValuePair<string, List<int>> e in linemap)
            {
                byte[] inbytes = Convert.FromBase64String(e.Key);
                MemoryStream str = new MemoryStream(inbytes);
                BinaryReader reader = new BinaryReader(str);
                int numblocks = inbytes.Length / 16;
                for (int i = 0; i < numblocks; i++)
                {
                    block = reader.ReadBytes(16);
                    foreach(byte b in block)
                    {
                        sb.AppendFormat("{0}", b);
                        sb.AppendLine();
                    }
                    // If we find a duplicate block in the current line
                    // store the line and the index of the duplicate block
                    if (!dedup.Contains(block))
                        dedup.Add(block);
                    else
                    {
                        linemap[e.Key].Add(i);
                    }

                }
                linemap[e.Key].Add(linemapct);
                linemapct++;
            }
            foreach(KeyValuePair<string,List<int>> e in linemap)
            {
               
                block = Convert.FromBase64String(e.Key);
                foreach(byte b in block)
                {
                    sb.AppendFormat("{0}", b);
                    sb.AppendLine();
                }
                StreamWriter wri = new StreamWriter(".\\freqdat.csv");
                wri.WriteLine(sb.ToString());
                wri.Close();
                if (e.Value.Count == 1)
                    continue;
                sb = new StringBuilder();
                sb.AppendFormat("Ciphertext from input {0} contains duplicate blocks at ",e.Key);
                for(int i = 0;i < e.Value.Count - 1;i++)
                {
                    sb.AppendFormat("{0} ", e.Value[i]);
                }
                sb.AppendFormat(" at line {0}",e.Value.Last<int>());
                System.Console.WriteLine(sb.ToString());
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
