using System.Security.Cryptography;
using Security2.Rsa;

namespace Security2.Web.Utils
{
    public class RsaServerKeys : RsaKeys
    {
        /// <inheritdoc />
        public RsaServerKeys(RSAParameters publicKey, RSAParameters privateKey) : base(publicKey, privateKey)
        {
        }
    }
}
