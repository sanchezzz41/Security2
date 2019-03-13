namespace Security2.Gronsfer
{
    public class GronfeldEncrypt
    {
        /// <summary>
        /// Шифрование
        /// </summary>
        /// <param name="alf">Алфавит</param>
        /// <param name="key">Ключ</param>
        /// <param name="data">Данные для шифрованияы</param>
        /// <returns></returns>
        public string EncryptGronsfeld(string alf, string key, string data)
        {
            var alfLocal = GetKeyFullString(alf, alf.Length * 2);
            var keyLocal = GetKeyFullString(key, data.Length);
            var resultEncData = "";
            //Encrypt
            for (int i = 0; i < data.Length; i++)
            {
                var dataChar = data[i];
                var isUpper = char.IsUpper(dataChar);
                dataChar = dataChar.ToString().ToLower()[0];

                var indexInEng = alfLocal.IndexOf(dataChar);
                indexInEng += int.Parse(keyLocal[i].ToString());
                resultEncData += isUpper ? alfLocal[indexInEng].ToString().ToUpper() : alfLocal[indexInEng].ToString();
            }

            return resultEncData;
        }

        /// <summary>
        /// Дешифрование
        /// </summary>
        /// <param name="alf">Алфавит</param>
        /// <param name="key">Ключ</param>
        /// <param name="encryptData">Данные для дешифровки</param>
        /// <returns></returns>
        public string DecryptGronsfeld(string alf, string key, string encryptData)
        {
            var resultEncData = "";
            var alfLocal = GetKeyFullString(alf, alf.Length * 2);
            var keyLocal = GetKeyFullString(key, encryptData.Length);
            //Decrypt
            for (int i = 0; i < encryptData.Length; i++)
            {
                var dataChar = encryptData[i];
                var isUpper = char.IsUpper(dataChar);
                dataChar = dataChar.ToString().ToLower()[0];

                var indexInEng = alfLocal.LastIndexOf(dataChar);
                indexInEng -= int.Parse(keyLocal[i].ToString());
                resultEncData += isUpper ? alfLocal[indexInEng].ToString().ToUpper() : alfLocal[indexInEng].ToString();
            }

            return resultEncData;
        }

        private string GetKeyFullString(string key, int count)
        {
            var resultKey = "";
            for (int i = 0, keyIndex = 0; i < count; i++, keyIndex++)
            {
                if (keyIndex == key.Length)
                    keyIndex = 0;
                resultKey += key[keyIndex];
            }

            return resultKey;
        }
    }
}