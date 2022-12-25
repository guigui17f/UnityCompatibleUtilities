using System;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace GUIGUI17F
{
    public class DevicePermissionUtility
    {
        public static event Action<UserAuthorization, bool> OnRequestPermissionFinish;
#if UNITY_ANDROID
        private static PermissionCallbacks _permissionCallbacks;
#endif

        public static bool HasPermission(UserAuthorization permission)
        {
#if UNITY_ANDROID
            return Permission.HasUserAuthorizedPermission(GetPermissionName(permission));
#elif UNITY_IOS
            return Application.HasUserAuthorization(permission);
#else
            return false;
#endif
        }

        public static async void RequestPermission(UserAuthorization permission)
        {
#if UNITY_ANDROID
            if (_permissionCallbacks == null)
            {
                InitializePermissionCallbacks();
            }
            Permission.RequestUserPermission(GetPermissionName(permission), _permissionCallbacks);
#elif UNITY_IOS
            AsyncOperation operation = Application.RequestUserAuthorization(permission);
            while (!operation.isDone)
            {
                await Task.Delay(30);
            }
            OnRequestPermissionFinish?.Invoke(permission, HasPermission(permission));
#else
            OnRequestPermissionFinish?.Invoke(permission, false);
#endif
        }

#if UNITY_ANDROID
        private static void InitializePermissionCallbacks()
        {
            _permissionCallbacks = new PermissionCallbacks();
            _permissionCallbacks.PermissionGranted += OnAndroidPermissionGranted;
            _permissionCallbacks.PermissionDenied += OnAndroidPermissionDenied;
            _permissionCallbacks.PermissionDeniedAndDontAskAgain += OnAndroidPermissionDeniedAndDontAskAgain;
        }

        private static void OnAndroidPermissionGranted(string permissionName)
        {
            OnRequestPermissionFinish?.Invoke(GetAuthorizationType(permissionName), true);
        }

        private static void OnAndroidPermissionDenied(string permissionName)
        {
            OnRequestPermissionFinish?.Invoke(GetAuthorizationType(permissionName), false);
        }

        private static void OnAndroidPermissionDeniedAndDontAskAgain(string permissionName)
        {
            OnRequestPermissionFinish?.Invoke(GetAuthorizationType(permissionName), false);
        }

        private static string GetPermissionName(UserAuthorization permission)
        {
            switch (permission)
            {
                case UserAuthorization.Microphone:
                    return Permission.Microphone;
                default:
                    return Permission.Camera;
            }
        }

        private static UserAuthorization GetAuthorizationType(string permissionName)
        {
            switch (permissionName)
            {
                case Permission.Microphone:
                    return UserAuthorization.Microphone;
                default:
                    return UserAuthorization.WebCam;
            }
        }
#endif
    }
}