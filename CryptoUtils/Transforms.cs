using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoUtils
{
    public static class Transforms
    {
        public static string bytearray2hexstr(byte[] inbytes)
        {
            StringBuilder tr = new StringBuilder();
            foreach (byte b in inbytes)
            {
                tr.Append(Convert.ToString(b, 16));
            }
            return tr.ToString();
        }
        public static byte[] hex2bytearray(string instr)
        {
            StringBuilder tr = new StringBuilder();
            byte[] inbytes = new byte[instr.Length / 2];
            int bytect = 0;
            for (int i = 0; i < instr.Length - 1; i += 2)
            {
                tr.Append(instr[i]);
                tr.Append(instr[i + 1]);
                inbytes[bytect] = Convert.ToByte(tr.ToString(), 16);
                tr.Clear();
                bytect++;
            }
            return inbytes;
        }
        public static string DecryptXor(string instr, out char key,byte[] inbytes = null)
        {
            if(inbytes == null)
                inbytes = Transforms.hex2bytearray(instr);
            Dictionary<char, Dictionary<char, int>> freqmap = new Dictionary<char, Dictionary<char, int>>();
            for (int i = 32; i < 128; i++)
            {
                freqmap[Convert.ToChar(i)] = new Dictionary<char, int>();
                // # Histogram of english frequencies is from http://math.fau.edu/richman/histogram.htm
                // sorted in order of highest to lowest freq
                // ['e','t','o','i','r','s','n','a','u']
                freqmap[Convert.ToChar(i)].Add('e', 0);
                freqmap[Convert.ToChar(i)].Add('t', 0);
                freqmap[Convert.ToChar(i)].Add('o', 0);
                freqmap[Convert.ToChar(i)].Add('a', 0);
                freqmap[Convert.ToChar(i)].Add('i', 0);
                freqmap[Convert.ToChar(i)].Add('r', 0);
                freqmap[Convert.ToChar(i)].Add('s', 0);
                freqmap[Convert.ToChar(i)].Add('n', 0);
                freqmap[Convert.ToChar(i)].Add('u', 0);

                foreach (byte b in inbytes)
                {
                    char t = Convert.ToChar(b ^ i);
                    Dictionary<char, int> dict = freqmap[Convert.ToChar(i)];
                    if (dict.Keys.Contains<char>(t))
                    {
                        dict[t] += 1;
                    }

                }
            }
            List<Tuple<int, char, Dictionary<char, int>>> ecount = new List<Tuple<int, char, Dictionary<char, int>>>();
            List<Tuple<char, Dictionary<char, int>>> candidates = new List<Tuple<char, Dictionary<char, int>>>();
            int maxct = 0;
            foreach (KeyValuePair<char, Dictionary<char, int>> entry in freqmap)
            {
                // Find the entry with the highest number of character
                // counts across all histogram characters (i.e. 20 hits for 'e'
                // is less significant than 1 hit for e,t,o,a etc.)
                Dictionary<char, int> dict = entry.Value;
                int histhitcount = 0;
                Dictionary<char, int> histcharcount = new Dictionary<char, int>();
                foreach (char c in dict.Keys)
                {
                    if (dict[c] > 0)
                    {
                        histhitcount++;
                        histcharcount[c] = dict[c];
                    }
                }
                if (histhitcount > 0)
                    ecount.Add(Tuple.Create(histhitcount, entry.Key, histcharcount));
            }
            // Find the candidate keys by looking for the largest histogram hit count value
            // In case there are ties, look at the histcharcount structure and pick the 
            // key that has the highest frequency count for characters at the beginning
            // of the histogram
            foreach (Tuple<int, char, Dictionary<char, int>> t in ecount)
            {
                if (t.Item1 > maxct)
                {
                    maxct = t.Item1;
                }
            }
            // Get set of tuples with maxct; this could be simplified with LINQ query
            foreach (Tuple<int, char, Dictionary<char, int>> t in ecount)
            {
                if (t.Item1 == maxct)
                {
                    candidates.Add(Tuple.Create(t.Item2, t.Item3));
                }
            }

            if (candidates.Count > 1)
            {
                // Pick candidate with highest count of chars 
                // from the first 4 chars of the histogram
                int histhi = 0;
                int index = 0;
                int kindex = 0;
                List<char> histogram = new List<char>();
                histogram.AddRange(new char[] { 'e', 't', 'o', 'i' });
                int ct = 0;
                foreach (Tuple<char, Dictionary<char, int>> t in candidates)
                {
                    foreach (char c in histogram)
                    {
                        if (t.Item2.ContainsKey(c))
                        {
                            ct += t.Item2[c];
                        }
                    }

                    if (ct > histhi)
                    {
                        histhi = ct;
                        kindex = index;
                        ct = 0;
                    }
                    index++;
                }
                key = candidates[kindex].Item1;
            }
            else
            {
                key = candidates.First().Item1;
            }

            StringBuilder sb = new StringBuilder();
            foreach (byte b in inbytes)
            {
                sb.Append(Convert.ToChar(b ^ (int)key));
            }
            return sb.ToString();
        }
        public static string EncryptRotXor(string instr, string key)
        {
            StringBuilder retval = new StringBuilder();
            byte[] bstr = new byte[instr.Length];
            int index = 0;
            foreach (char c in instr)
            {
                bstr[index] = Convert.ToByte(c);
                index++;
            }
            int j = 0;
            int value = 0;
            foreach (byte b in bstr)
            {
                if (j == key.Length)
                    j = 0;
                value = b ^ Convert.ToByte(key.ToCharArray()[j]);
                retval.Append(value.ToString("x2"));
                j++;
            }
            return retval.ToString();
        }
    }
}
