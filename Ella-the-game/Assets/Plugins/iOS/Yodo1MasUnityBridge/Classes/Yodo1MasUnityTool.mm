//
//  Yodo1MasUnityTool.mm
//

#import "Yodo1MasUnityTool.h"

#define UNITY_PROJECT 1

#if UNITY_PROJECT
/// Unity3d引擎 项目
#else

#ifdef __cplusplus
extern "C" {
#endif
#if UNITY_VERSION < 500
    void UnityPause(bool pause) {}
    #else
    void UnityPause(int pause){}
    #endif
    void UnitySendMessage(const char* obj, const char* method, const char* msg) {}
    
#ifdef __cplusplus
}
#endif

#endif

NSString* Yodo1MasCreateNSString(const char* string)
{
    return string ? [NSString stringWithUTF8String:string] : [NSString stringWithUTF8String:""];
}

char* Yodo1MasMakeStringCopy(const char* string)
{
    if (string == NULL)
        return NULL;
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}
