using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibertyV.Utils
{
    static class Hash
    {
        public static uint jenkins_one_at_a_time_hash(string key) {
            uint hash = 0;
            foreach (char c in key) {
                hash += ((uint)c & 0xFF);
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }
            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);
            return hash;
        }

    }
}
