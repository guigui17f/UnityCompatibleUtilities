using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Debug = UnityEngine.Debug;

namespace GUIGUI17F
{
    /// <summary>
    /// helper to download a file list with CRC32 verification
    /// </summary>
    public class FileDownloadHelper
    {
        private const string LogTag = "FileDownloadHelper";
        private const int MaxRetry = 3;

        /// <summary>
        /// parameter0: all file download success, parameter1: success download file list
        /// </summary>
        public event Action<bool, List<string>> DownloadFinished;

        public event Action<int> ProgressChanged;

        private readonly string _baseUrl;
        private readonly string _storageDirectory;

        public FileDownloadHelper(string baseUrl, string storageDirectory)
        {
            _baseUrl = baseUrl;
            _storageDirectory = storageDirectory;
        }

        public void Download(Dictionary<string, string> fileDictionary, string storageFilePostfix)
        {
            int progress = 0;
            int step = 100 / fileDictionary.Count;
            bool allSuccess = true;
            List<string> successList = new List<string>();
            foreach (KeyValuePair<string, string> item in fileDictionary)
            {
                string storagePath = Path.Combine(_storageDirectory, item.Key + storageFilePostfix);
                string downloadUrl = _baseUrl + item.Key;
                int retryCount = 0;
                bool downloadSuccess = false;
                do
                {
                    //download file
                    bool downloadFailed = false;
                    while (!DownloadFile(downloadUrl, storagePath, 1024))
                    {
                        retryCount++;
                        if (retryCount > MaxRetry)
                        {
                            downloadFailed = true;
                            break;
                        }
                    }
                    if (downloadFailed)
                    {
                        break;
                    }

                    //check file hash
                    uint checkNum = CRC32.GetFileCRC32(storagePath);
                    if (checkNum.ToString("x") != item.Value)
                    {
                        File.Delete(storagePath);
                        retryCount++;
                        if (retryCount > MaxRetry)
                        {
                            break;
                        }
                    }
                    else
                    {
                        downloadSuccess = true;
                    }
                } while (!downloadSuccess);

                if (downloadSuccess)
                {
                    successList.Add(item.Key);
                }
                else
                {
                    allSuccess = false;
                }
                progress += step;
                ProgressChanged?.Invoke(progress);
            }
            DownloadFinished?.Invoke(allSuccess, successList);
        }

        private bool DownloadFile(string fileUrl, string storagePath, int bufferSize)
        {
            bool success = false;
            using (FileStream fs = File.Open(storagePath, FileMode.Append, FileAccess.Write))
            {
                HttpWebRequest request = WebRequest.Create(fileUrl) as HttpWebRequest;
                long offset = fs.Length;
                if (offset > 0)
                {
                    //add request offset based on downloaded file length
                    request.AddRange((int)offset);
                }
                try
                {
                    using (Stream rs = request.GetResponse().GetResponseStream())
                    {
                        byte[] buffer = new byte[bufferSize];
                        try
                        {
                            int n = rs.Read(buffer, 0, buffer.Length);
                            while (n > 0)
                            {
                                fs.Write(buffer, 0, n);
                                n = rs.Read(buffer, 0, buffer.Length);
                            }
                            rs.Close();
                            fs.Flush();
                            fs.Close();
                            success = true;
                        }
                        catch (Exception e)
                        {
                            Debug.unityLogger.LogError(LogTag, e.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    //download success if the request offset is out of the range
                    if (File.Exists(storagePath))
                    {
                        success = true;
                    }
                    Debug.unityLogger.LogError(LogTag, e.Message);
                }
            }
            return success;
        }
    }
}