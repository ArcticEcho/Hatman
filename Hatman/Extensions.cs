using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ChatExchangeDotNet;

namespace Hatman
{
    public static class Extensions
    {
        private static readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        private static readonly Regex friendlyUsername = new Regex(@"\p{Lu}?\p{Ll}*", RegOpts);

        public const RegexOptions RegOpts = RegexOptions.Compiled | RegexOptions.CultureInvariant;



        public static T PickRandom<T>(this IEnumerable<T> items)
        {
            if (items == null) { throw new ArgumentNullException("items"); }

            var n = new byte[4];
            rng.GetBytes(n);

            return items.ElementAt((int)(BitConverter.ToUInt32(n, 0) % items.Count()));
        }

        public static string GetChatFriendlyUsername(this User user)
        {
            var n = new byte[4];
            rng.GetBytes(n);

            var ms = friendlyUsername.Matches(user.Name);
            var name = "";

            foreach (Match m in ms)
            {
                if (name.Length > 3) { break; }
                name += m.Value;
            }

            return name;
        }
    }
}
