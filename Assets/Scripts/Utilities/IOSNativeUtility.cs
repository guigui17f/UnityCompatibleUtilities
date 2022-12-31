#if UNITY_IOS
using System.Runtime.InteropServices;

public class IOSNativeUtility
{
    [DllImport("__Internal")]
    private static extern void IOSNativeUtilities_OpenApplicationSettings();

    [DllImport("__Internal")]
    private static extern void IOSNativeUtilities_SetAudioCategoryPlayback();

    [DllImport("__Internal")]
    private static extern void IOSNativeUtilities_SetAudioCategoryAmbient();

    public static void OpenApplicationSettings()
    {
        IOSNativeUtilities_OpenApplicationSettings();
    }

    public static void ToggleAudioToIgnoreMute()
    {
        IOSNativeUtilities_SetAudioCategoryPlayback();
    }

    public static void ToggleAudioToCompatibleMode()
    {
        IOSNativeUtilities_SetAudioCategoryAmbient();
    }
}
#endif