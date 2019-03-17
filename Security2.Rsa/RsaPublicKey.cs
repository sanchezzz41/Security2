using System;
using System.Security.Cryptography;

namespace Security2.Rsa
{
    public class RsaPublicKey
    {
        public string Modulus { get; set; }

        public string Exponent { get; set; }

        /// <inheritdoc />
        public RsaPublicKey(byte[] modulus, byte[] exponent)
        {
            Modulus = Convert.ToBase64String(modulus);
            Exponent = Convert.ToBase64String(exponent);
        }

        public RsaPublicKey(RSAParameters publicKey) 
            : this(publicKey.Modulus, publicKey.Exponent)
        {
        }

        public RsaPublicKey()
        {
            
        }

        public RSAParameters GetRsaParameters()
        {
            return new RSAParameters
            {
                Modulus = GetModulus(),
                Exponent = GetExponent()
            };
        }

        public byte[] GetModulus()
        {
            return Convert.FromBase64String(Modulus);
        }

        public byte[] GetExponent()
        {
            return Convert.FromBase64String(Exponent);
        }
    }
}
