#if UNITY_ANDROID
using System;
using UnityEngine;

namespace GUIGUI17F
{
    public class AndroidMediaUtility
    {
        /// <summary>
        /// scan the media files under the given path to update the media library
        /// </summary>
        /// <param name="callback">parameter: the scan path</param>
        public static void ScanMedia(string scanPath, Action<string> callback)
        {
            using (AndroidJavaClass mediaScannerConnection = new AndroidJavaClass("android.media.MediaScannerConnection"))
            {
                AndroidMediaScanCompletedListener listener = new AndroidMediaScanCompletedListener();
                listener.OnScanCompleted += callback;
                mediaScannerConnection.CallStatic("scanFile", AndroidNativeUtility.UnityActivity, new[] { scanPath }, null, listener);
            }
        }
    }
}
#endif