using System;
using System.Linq;
using System.Text;

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
            data = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
            
            var length = data.Length/alf.Length + 2;
            var alfLocal = GetAlfFullString(alf, alf.Length * length);
            
            var keyLocal = GetKeyFullString(key, data.Length);
            var resultEncData = "";
            //Encrypt
            for (int i = 0; i < data.Length; i++)
            {
                var dataChar = data[i];

                var indexInEng = alfLocal.IndexOf(dataChar);
                indexInEng += int.Parse(keyLocal[i].ToString()) + i;
                resultEncData += alfLocal[indexInEng].ToString();
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
            
            var length = encryptData.Length/alf.Length + 2;
            
            var alfLocal = GetAlfFullString(alf, alf.Length * length);
            
            var keyLocal = GetKeyFullString(key, encryptData.Length);
            //Decrypt
            for (int i = 0; i < encryptData.Length; i++)
            {
                var dataChar = encryptData[i];

                var indexInEng = alfLocal.LastIndexOf(dataChar);
                indexInEng -= (int.Parse(keyLocal[i].ToString()) + i);
                resultEncData += alfLocal[indexInEng].ToString();
            }

            resultEncData = Encoding.UTF8.GetString(Convert.FromBase64String(resultEncData));
            return resultEncData;
        }

        private string GetAlfFullString(string key, int count)
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