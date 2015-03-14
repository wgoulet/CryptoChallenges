using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CryptoUtils
{
    public static class Detectors
    {
        public static double LikelyEnglish(string instr)
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
            string[] di = new string[] { "th", "er", "on", "an", "re", "he", "in", "ed", "nd", "ha", "at", "en", "es", "of", "or", "nt", "ea", "ti", "to", "it", "st", "io", "le", "is", "ou", "ar", "as", "de", "rt", "ve" };
            string[] tri = new string[] { "the", "and", "tha", "ent", "ion", "tio", "for", "nde", "has", "nce", "edt", "tis", "oft", "sth", "men" };
            string[] dou = new string[] { "ss", "ee", "tt", "ff", "ll", "mm", "oo" };
            Dictionary<string, Dictionary<string, Dictionary<string, int>>> freqmap = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
            freqmap[instr] = new Dictionary<string, Dictionary<string, int>>();
            freqmap[instr]["di"] = new Dictionary<string, int>();
            freqmap[instr]["tri"] = new Dictionary<string, int>();
            freqmap[instr]["dou"] = new Dictionary<string, int>();
            foreach (string s in di)
                freqmap[instr]["di"][s] = 0;
            foreach (string s2 in tri)
                freqmap[instr]["tri"][s2] = 0;
            foreach (string s3 in dou)
                freqmap[instr]["dou"][s3] = 0;

            // Iterate through the digraph/trigraph/double maps and count the number of times
            // each symbol is found in the input string.
            // For each digraph/trigraph/double map, count the number of symbols with a non-zero count. 
            // This sum, divided by the total count of all symbols, will be our likely value
            double actual = 0;

            List<string> keys = new List<string>();
            keys.AddRange(freqmap[instr]["di"].Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                if ((freqmap[instr]["di"][keys[i]] = Regex.Matches(instr, keys[i]).Count) > 0)
                    actual++;
            }

            keys = new List<string>();
            keys.AddRange(freqmap[instr]["tri"].Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                if ((freqmap[instr]["tri"][keys[i]] = Regex.Matches(instr, keys[i]).Count) > 0)
                    actual++;
            }

            keys = new List<string>();
            keys.AddRange(freqmap[instr]["dou"].Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                if ((freqmap[instr]["dou"][keys[i]] = Regex.Matches(instr, keys[i]).Count) > 0)
                    actual++;
            }

            allsymbolcount = freqmap[instr]["di"].Count + freqmap[instr]["tri"].Count + freqmap[instr]["dou"].Count;

            return actual / allsymbolcount;
        }
    }
}
