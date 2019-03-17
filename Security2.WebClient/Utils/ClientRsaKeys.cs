using System.Security.Cryptography;
using Security2.Rsa;

namespace Security2.WebClient.Utils
{
    public class ClientRsaKeys
    {
        public RsaPublicKey RsaPublicKeyServer { get; set; }

        public RsaPublicKey RsaPublicKeyUser { get; set; }

        public RSAParameters RsaPrivateKeyUser { get; set; }
    }
}
