using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GUIGUI17F
{
    public class SymmetricAlgorithmHelper : IDisposable
    {
        public enum AlgorithmName
        {
            DES,
            AES,
            Rijndael,
            TripleDES,
            RC2
        }

        public SymmetricAlgorithm Algorithm => _symmetricAlgorithm;

        private SymmetricAlgorithm _symmetricAlgorithm;
        private ICryptoTransform _encryptor;
        private ICryptoTransform _decryptor;

        /// <summary>
        /// remember to call Dispose() when this instance is no longer needed
        /// </summary>
        public SymmetricAlgorithmHelper(AlgorithmName algorithmName, byte[] key, byte[] iv, bool autoInit, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
        {
            switch (algorithmName)
            {
                case AlgorithmName.DES:
                    _symmetricAlgorithm = DES.Create();
                    break;
                case AlgorithmName.AES:
                    _symmetricAlgorithm = Aes.Create();
                    break;
                case AlgorithmName.Rijndael:
                    _symmetricAlgorithm = Rijndael.Create();
                    break;
                case AlgorithmName.TripleDES:
                    _symmetricAlgorithm = TripleDES.Create();
                    break;
                case AlgorithmName.RC2:
                    _symmetricAlgorithm = RC2.Create();
                    break;
            }
            _symmetricAlgorithm.Key = key;
            _symmetricAlgorithm.IV = iv;
            _symmetricAlgorithm.Mode = mode;
            _symmetricAlgorithm.Padding = padding;
            if (autoInit)
            {
                Initialize();
            }
        }

        /// <summary>
        /// call Initialize() after you setup custom parameters for Algorithm property
        /// </summary>
        public void Initialize()
        {
            _encryptor?.Dispose();
            _decryptor?.Dispose();
            _encryptor = _symmetricAlgorithm.CreateEncryptor();
            _decryptor = _symmetricAlgorithm.CreateDecryptor();
        }

        public string Encrypt(string origin, Encoding encoding)
        {
            byte[] encryptedData = EncryptData(encoding.GetBytes(origin));
            return Convert.ToBase64String(encryptedData);
        }

        public byte[] EncryptData(byte[] originData)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, _encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(originData, 0, originData.Length);
                    cryptoStream.FlushFinalBlock();
                }
                return memoryStream.ToArray();
            }
        }

        public string Decrypt(string encrypted, Encoding encoding)
        {
            byte[] originData = DecryptData(Convert.FromBase64String(encrypted));
            return encoding.GetString(originData);
        }

        public byte[] DecryptData(byte[] encryptedData)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, _decryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(encryptedData, 0, encryptedData.Length);
                    cryptoStream.FlushFinalBlock();
                }
                return memoryStream.ToArray();
            }
        }

        public void Dispose()
        {
            _encryptor?.Dispose();
            _decryptor?.Dispose();
            _symmetricAlgorithm.Dispose();
        }
    }
}