using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;

namespace CryptoChallengesSet1
{
    public static class Generators
    {
        public static byte[] genRandBytes(int keylen, bool printable = false)
        {
            List<byte> retval = new List<byte>();
            byte[] onebyte = new byte[1];
            IEnumerable<int> range = Enumerable.Range(1, keylen);
            // Generate random bytes in printable ASCII range
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            foreach (int i in range)
            {
                if (printable)
                {
                    while (!(32 < (int)onebyte[0]) || !((int)onebyte[0] < 127))
                    {
                        rng.GetBytes(onebyte);
                    }
                    retval.Add(onebyte[0]);
                    onebyte[0] = 0;
                }
                else
                {
                    rng.GetBytes(onebyte);
                    retval.Add(onebyte[0]);
                }
            }

            return retval.ToArray();
        }
    }
}
