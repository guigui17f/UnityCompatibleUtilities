using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GUIGUI17F
{
    /// <summary>
    /// remember to call Dispose() when this instance is no longer needed
    /// </summary>
    public class HashAlgorithmHelper : IDisposable
    {
        public enum AlgorithmName
        {
            MD5,
            HMACMD5,
            SHA1,
            HMACSHA1,
            SHA256,
            HMACSHA256,
            SHA384,
            HMACSHA384,
            SHA512,
            HMACSHA512
        }
        
        public HashAlgorithm Algorithm => _hashAlgorithm;

        private HashAlgorithm _hashAlgorithm;

        public HashAlgorithmHelper(AlgorithmName algorithmName)
        {
            switch (algorithmName)
            {
                case AlgorithmName.MD5:
                    _hashAlgorithm = MD5.Create();
                    break;
                case AlgorithmName.HMACMD5:
                    _hashAlgorithm = HMAC.Create("HMACMD5");
                    break;
                case AlgorithmName.SHA1:
                    _hashAlgorithm = SHA1.Create();
                    break;
                case AlgorithmName.HMACSHA1:
                    _hashAlgorithm = HMAC.Create("HMACSHA1");
                    break;
                case AlgorithmName.SHA256:
                    _hashAlgorithm = SHA256.Create();
                    break;
                case AlgorithmName.HMACSHA256:
                    _hashAlgorithm = HMAC.Create("HMACSHA256");
                    break;
                case AlgorithmName.SHA384:
                    _hashAlgorithm = SHA384.Create();
                    break;
                case AlgorithmName.HMACSHA384:
                    _hashAlgorithm = HMAC.Create("HMACSHA384");
                    break;
                case AlgorithmName.SHA512:
                    _hashAlgorithm = SHA512.Create();
                    break;
                case AlgorithmName.HMACSHA512:
                    _hashAlgorithm = HMAC.Create("HMACSHA512");
                    break;
            }
        }

        public string GetHash(byte[] sourceData)
        {
            byte[] hash = GetHashData(sourceData);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

        public byte[] GetHashData(byte[] sourceData)
        {
            return _hashAlgorithm.ComputeHash(sourceData);
        }

        public string GetHash(string sourceString, Encoding encoding)
        {
            byte[] hash = GetHashData(sourceString, encoding);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

        public byte[] GetHashData(string sourceString, Encoding encoding)
        {
            return GetHashData(encoding.GetBytes(sourceString));
        }

        public string GetFileHash(string filePath)
        {
            byte[] hash = GetFileHashData(filePath);
            if (hash == null)
            {
                return null;
            }
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

        public byte[] GetFileHashData(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            using (FileStream fs = File.OpenRead(filePath))
            {
                return _hashAlgorithm.ComputeHash(fs);
            }
        }

        public void Dispose()
        {
            _hashAlgorithm.Dispose();
        }
    }
}