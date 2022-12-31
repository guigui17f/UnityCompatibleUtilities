#import "AVFoundation/AVFoundation.h"

@implementation IOSNativeUtilities
extern "C"
{
    void IOSNativeUtilities_OpenApplicationSettings()
    {
        NSURL* url = [NSURL URLWithString: UIApplicationOpenSettingsURLString];
        if ([[UIApplication sharedApplication] canOpenURL:url]) 
        {
            [[UIApplication sharedApplication] openURL:url options:@{} completionHandler:nil];
        }
    }

    void IOSNativeUtilities_SetAudioCategoryPlayback()
    {
        [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryPlayback error:nil];
        [[AVAudioSession sharedInstance] setActive:YES error:nil];
    }
    
    void IOSNativeUtilities_SetAudioCategoryAmbient()
    {
        [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryAmbient error:nil];
        [[AVAudioSession sharedInstance] setActive:YES error:nil];
    }
}
@end

