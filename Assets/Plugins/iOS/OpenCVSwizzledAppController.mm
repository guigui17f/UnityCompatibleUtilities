// this script is an alternative of the default "OpenCVForUnityAppController" script in the "OpenCV for Unity" plugin:
// https://assetstore.unity.com/packages/tools/integration/opencv-for-unity-21088
// it solved the UnityAppController implementation conflict, now you can use your own UnityAppController implementation while including this plugin

#import <objc/runtime.h>
#import "UnityAppController.h"
#include "Unity/IUnityGraphics.h"

extern "C" void OpenCVForUnity_UnityPluginLoad(IUnityInterfaces *interfaces);
extern "C" void OpenCVForUnity_UnityPluginUnload();

#pragma mark - App controller subclasssing

@implementation UnityAppController (OpenCVSwizzledAppController)

static IMP __original_shouldAttachRenderDelegate_Imp __unused;

+(void)load {
    Method method = class_getInstanceMethod([self class], @selector(shouldAttachRenderDelegate:));
    __original_shouldAttachRenderDelegate_Imp = method_setImplementation(method, (IMP)__swizzled_shouldAttachRenderDelegate);
}

void __swizzled_shouldAttachRenderDelegate(id self, SEL _cmd, UIApplication* launchOptions) {
    UnityRegisterRenderingPluginV5(&OpenCVForUnity_UnityPluginLoad, &OpenCVForUnity_UnityPluginUnload);
    if(__original_shouldAttachRenderDelegate_Imp) {
        ((void(*)(id, SEL, UIApplication*))__original_shouldAttachRenderDelegate_Imp)(self, _cmd, launchOptions);
    }
}

@end