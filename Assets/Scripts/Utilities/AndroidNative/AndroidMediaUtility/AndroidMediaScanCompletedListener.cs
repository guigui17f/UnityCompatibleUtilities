#if UNITY_ANDROID
using System;
using UnityEngine;

namespace GUIGUI17F
{
    public class AndroidMediaScanCompletedListener : AndroidJavaProxy
    {
        /// <summary>
        /// arg: scan path
        /// </summary>
        public event Action<string> OnScanCompleted;

        public AndroidMediaScanCompletedListener() : base("android.media.MediaScannerConnection$OnScanCompletedListener")
        {
        }

        public void onScanCompleted(string path, AndroidJavaObject uri)
        {
            uri?.Dispose();
            OnScanCompleted?.Invoke(path);
        }
    }
}
#endif