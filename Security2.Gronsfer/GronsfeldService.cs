namespace Security2.Gronsfer
{
    /// <summary>
    /// Сервис для работы с гронсфельдом
    /// </summary>
    public class GronsfeldService
    {
        private readonly GronfeldEncrypt _encrypt;
        private readonly GronsfeldOptions _options;

        public GronsfeldService(GronfeldEncrypt encrypt, GronsfeldOptions options)
        {
            _encrypt = encrypt;
            _options = options;
        }

        public string Encrypt(string data, string key)
        {
            return _encrypt.EncryptGronsfeld(_options.Alp, key, data);
        }

        public string Decrypt(string data, string key)
        {
            return _encrypt.DecryptGronsfeld(_options.Alp, key, data);
        }
    }
}
