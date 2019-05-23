using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ToolKitty.WebSockets
{
    public struct WSKey
    {
        private static readonly Encoding
            UTF8 = Encoding.UTF8;
        private static readonly SHA1 
            SHA1 = SHA1.Create();

        public Guid ID;
        public string Text;

        public static WSKey FromString(string text)
        {
            var bytes = Convert.FromBase64String(text);

            return new WSKey {
                ID = new Guid(bytes),
                Text = text,
            };
        }

        public static WSKey FromID(Guid id)
        {
            var bytes = id.ToByteArray();

            return new WSKey {
                Text = Convert.ToBase64String(bytes),
                ID = id,
            };
        }

        public string ComputeAccept()
        {
            var text = Text + WSConsts.KeySuffix;
            var sha1 = SHA1.ComputeHash(UTF8.GetBytes(text));

            return Convert.ToBase64String(sha1);
        }
    }
}
