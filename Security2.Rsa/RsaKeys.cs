using System.Security.Cryptography;

namespace Security2.Rsa
{
    public class RsaKeys
    {
        public RSAParameters PublicKey { get; set; }

        public RSAParameters PrivateKey { get; set; }

        /// <inheritdoc />
        public RsaKeys(RSAParameters publicKey, RSAParameters privateKey)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }
    }
}
