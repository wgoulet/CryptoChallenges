using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

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
            //instr = "theronanththth";
            string cleartext = string.Empty;
            while (!reader.EndOfStream)
            {
                cleartext = Program.DecryptXor(reader.ReadLine(), out key);
                System.Console.WriteLine(String.Format("Likelihood english factor for {0} is {1}", cleartext.Trim(), Program.LikelyEnglish(cleartext)));
                if (Program.LikelyEnglish(cleartext) > 0.1)
                    System.Console.WriteLine(String.Format("Found text {0} with key {1} in input file", cleartext.Trim(), key));
            }
            System.Console.ReadKey();

        }
        private static string DecryptXor(string instr, out char key)
        {
            byte[] inbytes = Program.hex2bytearray(instr);
            Dictionary<char, Dictionary<char, int>> freqmap = new Dictionary<char, Dictionary<char, int>>();
            for (int i = 0; i < 255; i++)
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

        private static double LikelyEnglish(string instr)
        {
            /* From http://www.simonsingh.net/The_Black_Chamber/hintsandtips.html
             * Order Of Frequency Of Digraphs
                th er on an re he in ed nd ha at en es of or nt ea ti to it st io le is ou ar as de rt ve
             * Order Of Frequency Of Trigraphs
                the and tha ent ion tio for nde has nce edt tis oft sth men
             * Order Of Frequency Of Most Common Doubles
                ss ee tt ff ll mm oo
            */
            double allsymbolcount = 0;
            // Initialize data structures that will hold maps of a count of each digraph/trigraph/double string
            Dictionary<string, List<string>> symmap = new Dictionary<string, List<string>>();
            symmap["di"] = new List<string>();
            string[] di = new string[]{ "th", "er", "on", "an", "re", "he", "in", "ed", "nd", "ha", "at", "en", "es", "of", "or", "nt", "ea", "ti", "to", "it", "st", "io", "le", "is", "ou", "ar", "as", "de", "rt", "ve" };
            string[] tri = new string[] { "the", "and", "tha", "ent", "ion", "tio", "for", "nde", "has", "nce", "edt", "tis", "oft", "sth", "men" };
            string[] dou = new string[] { "ss", "ee", "tt", "ff", "ll", "mm", "oo" };
            Dictionary<string, Dictionary<string,Dictionary<string, int>>> freqmap = new Dictionary<string, Dictionary<string,Dictionary<string, int>>>();
            freqmap[instr] = new Dictionary<string,Dictionary<string, int>>();
            freqmap[instr]["di"] = new Dictionary<string,int>();
            freqmap[instr]["tri"] = new Dictionary<string,int>();
            freqmap[instr]["dou"] = new Dictionary<string,int>();
            foreach(string s in di)
                freqmap[instr]["di"][s] = 0; 
            foreach(string s2 in tri)
                freqmap[instr]["tri"][s2] = 0;
            foreach(string s3 in dou)
                freqmap[instr]["dou"][s3] = 0;

            // Iterate through the digraph/trigraph/double maps and count the number of times
            // each symbol is found in the input string.
            // For each digraph/trigraph/double map, count the number of symbols with a non-zero count. 
            // This sum, divided by the total count of all symbols, will be our likely value
            double actual = 0;
             
            List<string> keys = new List<string>();
            keys.AddRange(freqmap[instr]["di"].Keys);
            for (int i = 0; i < keys.Count;i++ )
            {
                if((freqmap[instr]["di"][keys[i]] = Regex.Matches(instr, keys[i]).Count) > 0)
                    actual++;
            }

            keys = new List<string>();
            keys.AddRange(freqmap[instr]["tri"].Keys);
            for (int i = 0; i < keys.Count;i++ )
            {
                if((freqmap[instr]["tri"][keys[i]] = Regex.Matches(instr, keys[i]).Count) > 0)
                    actual++;
            }
            
            keys = new List<string>();
            keys.AddRange(freqmap[instr]["dou"].Keys);
            for (int i = 0; i < keys.Count;i++ )
            {
                if ((freqmap[instr]["dou"][keys[i]] = Regex.Matches(instr, keys[i]).Count) > 0)
                    actual++;
            }

            allsymbolcount = freqmap[instr]["di"].Count + freqmap[instr]["tri"].Count + freqmap[instr]["dou"].Count;

            return actual/allsymbolcount;
        }

        static string bytearray2hexstr(byte[] inbytes)
        {
            StringBuilder tr = new StringBuilder();
            foreach (byte b in inbytes)
            {
                tr.Append(Convert.ToString(b, 16));
            }
            return tr.ToString();
        }
        static byte[] hex2bytearray(string instr)
        {
            StringBuilder tr = new StringBuilder();
            byte[] inbytes = new byte[instr.Length / 2];
            int bytect = 0;
            for (int i = 0; i < instr.Length; i += 2)
            {
                tr.Append(instr[i]);
                tr.Append(instr[i + 1]);
                inbytes[bytect] = Convert.ToByte(tr.ToString(), 16);
                tr.Clear();
                bytect++;
            }
            return inbytes;
        }
    }
}




