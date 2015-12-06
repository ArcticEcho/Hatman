using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Hatman
{
    public static class Extensions
    {
        private static readonly Regex friendlyUsername = new Regex(@"\p{Lu}?\p{Ll}*", RegOpts);

        internal static int SelfID;

        public const RegexOptions RegOpts = RegexOptions.Compiled | RegexOptions.CultureInvariant;
        public static readonly RNGCryptoServiceProvider RNG = new RNGCryptoServiceProvider();



        public static T PickRandom<T>(this IEnumerable<T> items)
        {
            if (items == null) { throw new ArgumentNullException("items"); }

            var n = new byte[4];
            RNG.GetBytes(n);

            return items.ElementAt((int)(BitConverter.ToUInt32(n, 0) % items.Count()));
        }
    }
}
