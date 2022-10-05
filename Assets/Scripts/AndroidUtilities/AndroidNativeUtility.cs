#if UNITY_ANDROID
using UnityEngine;

namespace GUIGUI17F
{
    public class AndroidNativeUtility
    {
        public static AndroidJavaObject UnityActivity
        {
            get
            {
                if (_unityActivity == null)
                {
                    using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                    {
                        _unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    }
                }
                return _unityActivity;
            }
        }

        private static AndroidJavaObject _unityActivity;

        /// <summary>
        /// android.os.Environment.getDataDirectory()
        /// </summary>
        public static string GetDataDirectory()
        {
            using (AndroidJavaClass environment = new AndroidJavaClass("android.os.Environment"))
            {
                using (AndroidJavaObject dataDirectory = environment.CallStatic<AndroidJavaObject>("getDataDirectory"))
                {
                    return dataDirectory.Call<string>("getAbsolutePath");
                }
            }
        }

        /// <summary>
        /// android.os.Environment.getExternalStorageDirectory()
        /// </summary>
        public static string GetExternalStorageDirectory()
        {
            using (AndroidJavaClass environment = new AndroidJavaClass("android.os.Environment"))
            {
                string storageState = environment.CallStatic<string>("getExternalStorageState");
                switch (storageState)
                {
                    case "mounted":
                    case "mounted_ro":
                        using (AndroidJavaObject storageDirectory = environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
                        {
                            return storageDirectory.Call<string>("getAbsolutePath");
                        }
                    default:
                        return null;
                }
            }
        }
    }
}
#endif