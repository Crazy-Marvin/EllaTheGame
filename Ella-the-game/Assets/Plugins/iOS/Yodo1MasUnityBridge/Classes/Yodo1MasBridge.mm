//
//  UnityYodo1Mas.mm
//  Pods
//
//  Created by sunmeng on 2020/12/21.
//

#import "Yodo1MasBridge.h"
#import "Yodo1Mas.h"
#import "Yodo1MasUnityTool.h"

static NSString* kYodo1MasGameObject;
static NSString* kYodo1MasMethodName;

@interface Yodo1MasBridge : NSObject <Yodo1MasRewardAdDelegate, Yodo1MasInterstitialAdDelegate, Yodo1MasBannerAdDelegate>

+ (UIViewController*)getRootViewController;

+ (UIViewController*)topMostViewController:(UIViewController*)controller;

+ (NSString *)stringWithJSONObject:(id)obj error:(NSError**)error;

+ (id)JSONObjectWithString:(NSString*)str error:(NSError**)error;

+ (NSString*)convertToInitJsonString:(int)success error:(NSString*)errorMsg;

+ (NSString*)getSendMessage:(int)flag data:(NSString*)data;

+ (Yodo1MasBridge *)sharedInstance;

- (void)initWithAppId:(NSString *)appId successful:(Yodo1MasInitSuccessful)successful fail:(Yodo1MasInitFail)fail;

- (BOOL)isRewardedAdLoaded;
- (void)showRewardedAd;
- (void)showRewardedAd:(NSString *)placementId;

- (BOOL)isInterstitialAdLoaded;
- (void)showInterstitialAd;
- (void)showInterstitialAd:(NSString *)placementId;

- (BOOL)isBannerAdLoaded;
- (void)showBannerAd;
- (void)showBannerAdWithPlacement:(NSString *)placementId;
- (void)showBannerAdWithAlign:(Yodo1MasAdBannerAlign)align;
- (void)showBannerAdWithAlign:(Yodo1MasAdBannerAlign)align offset:(CGPoint)offset;
- (void)showBannerAdWithPlacement:(NSString *)placement align:(Yodo1MasAdBannerAlign)align offset:(CGPoint)offset;
- (void)dismissBannerAd;
- (void)dismissBannerAdWithDestroy:(BOOL)destroy;
@end

@implementation Yodo1MasBridge

+ (Yodo1MasBridge *)sharedInstance {
    static Yodo1MasBridge *_instance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _instance = [[Yodo1MasBridge alloc] init];
    });
    return _instance;
}

- (void)initWithAppId:(NSString *)appId successful:(Yodo1MasInitSuccessful)successful fail:(Yodo1MasInitFail)fail {
    [Yodo1Mas sharedInstance].rewardAdDelegate = self;
    [Yodo1Mas sharedInstance].interstitialAdDelegate = self;
    [Yodo1Mas sharedInstance].bannerAdDelegate = self;
    
    [[Yodo1Mas sharedInstance] initWithAppId:appId successful:successful fail:fail];
}

- (BOOL)isRewardedAdLoaded {
    return [[Yodo1Mas sharedInstance] isRewardAdLoaded];
}
- (void)showRewardedAd {
    [[Yodo1Mas sharedInstance] showRewardAd];
}

- (void)showRewardedAd:(NSString *)placementId {
    [[Yodo1Mas sharedInstance] showRewardAdWithPlacement:placementId];
}

- (BOOL)isInterstitialAdLoaded {
    return [[Yodo1Mas sharedInstance] isInterstitialAdLoaded];
}
- (void)showInterstitialAd {
    [[Yodo1Mas sharedInstance] showInterstitialAd];
}

- (void)showInterstitialAd:(NSString *)placementId {
    [[Yodo1Mas sharedInstance] showInterstitialAdWithPlacement:placementId];
}

- (BOOL)isBannerAdLoaded {
    return [[Yodo1Mas sharedInstance] isBannerAdLoaded];
}
- (void)showBannerAd {
    [[Yodo1Mas sharedInstance] showBannerAd];
}

- (void)showBannerAdWithPlacement:(NSString *)placementId {
    [[Yodo1Mas sharedInstance] showBannerAdWithPlacement:placementId];
}

- (void)showBannerAdWithAlign:(Yodo1MasAdBannerAlign)align {
    [[Yodo1Mas sharedInstance] showBannerAdWithAlign:align];
}

- (void)showBannerAdWithAlign:(Yodo1MasAdBannerAlign)align offset:(CGPoint)offset {
    [[Yodo1Mas sharedInstance] showBannerAdWithAlign:align offset:offset];
}

- (void)showBannerAdWithPlacement:(NSString *)placement align:(Yodo1MasAdBannerAlign)align offset:(CGPoint)offset {
    [[Yodo1Mas sharedInstance] showBannerAdWithPlacement:placement align:align offset:offset];
}

- (void)dismissBannerAd
{
    [[Yodo1Mas sharedInstance] dismissBannerAd];
}

- (void)dismissBannerAdWithDestroy:(BOOL)destroy
{
    [[Yodo1Mas sharedInstance] dismissBannerAdWithDestroy:destroy];
}

#pragma mark - Yodo1MasAdDelegate
- (void)onAdOpened:(Yodo1MasAdEvent *)event {
    if (event == nil) {
        return;
    }
    NSString* data = [Yodo1MasBridge stringWithJSONObject:event.getJsonObject error:nil];
    NSString* msg = [Yodo1MasBridge getSendMessage:1 data:data];
    UnitySendMessage([kYodo1MasGameObject cStringUsingEncoding:NSUTF8StringEncoding], [kYodo1MasMethodName cStringUsingEncoding:NSUTF8StringEncoding], [msg cStringUsingEncoding:NSUTF8StringEncoding]);

}

- (void)onAdClosed:(Yodo1MasAdEvent *)event {
    if (event == nil) {
        return;
    }
    NSString* data = [Yodo1MasBridge stringWithJSONObject:event.getJsonObject error:nil];
    NSString* msg = [Yodo1MasBridge getSendMessage:1 data:data];
    UnitySendMessage([kYodo1MasGameObject cStringUsingEncoding:NSUTF8StringEncoding], [kYodo1MasMethodName cStringUsingEncoding:NSUTF8StringEncoding], [msg cStringUsingEncoding:NSUTF8StringEncoding]);
}

- (void)onAdError:(Yodo1MasAdEvent *)event error:(Yodo1MasError *)error {
    if (event == nil) {
        return;
    }
    NSString* data = [Yodo1MasBridge stringWithJSONObject:event.getJsonObject error:nil];
    NSString* msg = [Yodo1MasBridge getSendMessage:1 data:data];
    UnitySendMessage([kYodo1MasGameObject cStringUsingEncoding:NSUTF8StringEncoding], [kYodo1MasMethodName cStringUsingEncoding:NSUTF8StringEncoding], [msg cStringUsingEncoding:NSUTF8StringEncoding]);
}

#pragma mark - Yodo1MasRewardAdvertDelegate
- (void)onAdRewardEarned:(Yodo1MasAdEvent *)event {
    if (event == nil) {
        return;
    }
    NSString* data = [Yodo1MasBridge stringWithJSONObject:event.getJsonObject error:nil];
    NSString* msg = [Yodo1MasBridge getSendMessage:1 data:data];
    UnitySendMessage([kYodo1MasGameObject cStringUsingEncoding:NSUTF8StringEncoding], [kYodo1MasMethodName cStringUsingEncoding:NSUTF8StringEncoding], [msg cStringUsingEncoding:NSUTF8StringEncoding]);
}

+ (UIViewController*)getRootViewController {
    UIWindow* window = [[UIApplication sharedApplication] keyWindow];
    if (window.windowLevel != UIWindowLevelNormal) {
        NSArray* windows = [[UIApplication sharedApplication] windows];
        for (UIWindow* _window in windows) {
            if (_window.windowLevel == UIWindowLevelNormal) {
                window = _window;
                break;
            }
        }
    }
    UIViewController* viewController = nil;
    for (UIView* subView in [window subviews]) {
        UIResponder* responder = [subView nextResponder];
        if ([responder isKindOfClass:[UIViewController class]]) {
            viewController = [self topMostViewController:(UIViewController*)responder];
        }
    }
    if (!viewController) {
        viewController = UIApplication.sharedApplication.keyWindow.rootViewController;
    }
    return viewController;
}

+ (UIViewController*)topMostViewController:(UIViewController*)controller {
    BOOL isPresenting = NO;
    do {
        // this path is called only on iOS 6+, so -presentedViewController is fine here.
        UIViewController* presented = [controller presentedViewController];
        isPresenting = presented != nil;
        if (presented != nil) {
            controller = presented;
        }
        
    } while (isPresenting);
    
    return controller;
}

+ (NSString*)stringWithJSONObject:(id)obj error:(NSError**)error {
    if (obj) {
        if (NSClassFromString(@"NSJSONSerialization")) {
            NSData* data = nil;
            @try {
                data = [NSJSONSerialization dataWithJSONObject:obj options:0 error:error];
            }
            @catch (NSException* exception)
            {
                *error = [NSError errorWithDomain:[exception description] code:0 userInfo:nil];
                return nil;
            }
            @finally
            {
            }
            
            if (data) {
                return [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
            }
        }
    }
    return nil;
}

+ (id)JSONObjectWithString:(NSString*)str error:(NSError**)error {
    if (str) {
        if (NSClassFromString(@"NSJSONSerialization")) {
            return [NSJSONSerialization JSONObjectWithData:[str dataUsingEncoding:NSUTF8StringEncoding]
                                                   options:NSJSONReadingAllowFragments
                                                     error:error];
        }
    }
    return nil;
}

+ (NSString*)convertToInitJsonString:(int)success masError:(Yodo1MasError*) error {
    NSMutableDictionary* dict = [NSMutableDictionary dictionary];
    [dict setObject:[NSNumber numberWithInt:success] forKey:@"success"];
    
    if (error != nil) {
        NSString* errorJsonString = [Yodo1MasBridge stringWithJSONObject:[error getJsonObject] error:nil];
        [dict setObject:errorJsonString forKey:@"error"];
    }
    
    NSString* data = [Yodo1MasBridge stringWithJSONObject:dict error:nil];
    return data;
}

+ (NSString*)getSendMessage:(int)flag data:(NSString*)data {
    NSMutableDictionary* dict = [NSMutableDictionary dictionary];
    [dict setObject:[NSNumber numberWithInt:flag] forKey:@"flag"];
    [dict setObject:data forKey:@"data"];
    
    NSError* parseJSONError = nil;
    NSString* msg = [Yodo1MasBridge stringWithJSONObject:dict error:&parseJSONError];
    NSString* jsonError = @"";
    if(parseJSONError){
        jsonError = @"Convert result to json failed!";
    }
    return msg;
}

@end

#pragma mark- ///Unity3d

#ifdef __cplusplus

extern "C" {

#pragma mark - Privacy

void UnityMasSetUserConsent(BOOL consent)
{
    [Yodo1Mas sharedInstance].isGDPRUserConsent = consent;
}

bool UnityMasIsUserConsent()
{
    return [Yodo1Mas sharedInstance].isGDPRUserConsent;
}

void UnityMasSetTagForUnderAgeOfConsent(BOOL isBelowConsentAge)
{
    [Yodo1Mas sharedInstance].isCOPPAAgeRestricted = isBelowConsentAge;
}

bool UnityMasIsTagForUnderAgeOfConsent()
{
    return [Yodo1Mas sharedInstance].isCOPPAAgeRestricted;
}

void UnityMasSetDoNotSell(BOOL doNotSell)
{
    [Yodo1Mas sharedInstance].isCCPADoNotSell = doNotSell;
}

bool UnityMasIsDoNotSell()
{
    return [Yodo1Mas sharedInstance].isCCPADoNotSell;
}

#pragma mark - Initialize

void UnityMasInitWithAppKey(const char* appKey,const char* gameObjectName, const char* callbackMethodName)
{
    NSString* m_appKey = Yodo1MasCreateNSString(appKey);
    NSCAssert(m_appKey != nil, @"AppKey 没有设置!");
    
    NSString* m_gameObject = Yodo1MasCreateNSString(gameObjectName);
    NSCAssert(m_gameObject != nil, @"Unity3d gameObject isn't set!");
    kYodo1MasGameObject = m_gameObject;
    
    NSString* m_methodName = Yodo1MasCreateNSString(callbackMethodName);
    NSCAssert(m_methodName != nil, @"Unity3d callback method isn't set!");
    kYodo1MasMethodName = m_methodName;
    
    [[Yodo1MasBridge sharedInstance] initWithAppId:m_appKey successful:^{
        NSString* data = [Yodo1MasBridge convertToInitJsonString:1 masError:nil];
        NSString* msg = [Yodo1MasBridge getSendMessage:0 data:data];
        UnitySendMessage([kYodo1MasGameObject cStringUsingEncoding:NSUTF8StringEncoding], [kYodo1MasMethodName cStringUsingEncoding:NSUTF8StringEncoding], [msg cStringUsingEncoding:NSUTF8StringEncoding]);
    } fail:^(Yodo1MasError * _Nonnull error) {
        NSString* data = [Yodo1MasBridge convertToInitJsonString:0 masError:error];
        NSString* msg = [Yodo1MasBridge getSendMessage:0 data:data];
        UnitySendMessage([kYodo1MasGameObject cStringUsingEncoding:NSUTF8StringEncoding], [kYodo1MasMethodName cStringUsingEncoding:NSUTF8StringEncoding], [msg cStringUsingEncoding:NSUTF8StringEncoding]);
    }];
}

void UnitySetAdBuildConfig(const char * config) {
    NSString * jsonString = Yodo1MasCreateNSString(config);
    NSError * error = nil;
    id dict = [Yodo1MasBridge JSONObjectWithString:jsonString error:&error];
    if (error) {
        return;
    }
    Yodo1MasAdBuildConfig * buildConfig = [Yodo1MasAdBuildConfig instance];
    if (dict[@"enableAdaptiveBanner"]) {
        buildConfig.enableAdaptiveBanner = [dict[@"enableAdaptiveBanner"] boolValue];
    }
    if (dict[@"enableUserPrivacyDialog"]) {
        buildConfig.enableUserPrivacyDialog = [dict[@"enableUserPrivacyDialog"] boolValue];
    }
    if (dict[@"userAgreementUrl"]) {
        NSString* userAgreementUrl = dict[@"userAgreementUrl"];
        if (userAgreementUrl != nil && userAgreementUrl.length > 0) {
            buildConfig.userAgreementUrl = userAgreementUrl;
        }
    }
    if (dict[@"privacyPolicyUrl"]) {
        NSString* privacyPolicyUrl = dict[@"privacyPolicyUrl"];
        if (privacyPolicyUrl != nil && privacyPolicyUrl.length > 0) {
            buildConfig.privacyPolicyUrl = privacyPolicyUrl;
        }
    }
    [[Yodo1Mas sharedInstance] setAdBuildConfig:buildConfig];
}

#pragma mark - Unity Banner

bool UnityIsBannerAdLoaded()
{
    return [[Yodo1MasBridge sharedInstance] isBannerAdLoaded];
}

void UnityShowBannerAd()
{
    [[Yodo1MasBridge sharedInstance] showBannerAd];
}

void UnityShowBannerAdWithPlacement(const char* placementId)
{
    NSString* m_placementId = Yodo1MasCreateNSString(placementId);
    [[Yodo1MasBridge sharedInstance] showBannerAdWithPlacement:m_placementId];
}

void UnityShowBannerAdWithAlign(int align)
{
    [[Yodo1MasBridge sharedInstance] showBannerAdWithAlign:(Yodo1MasAdBannerAlign)align];
}

void UnityShowBannerAdWithAlignAndOffset(int align, int offsetX, int offsetY)
{
    [[Yodo1MasBridge sharedInstance] showBannerAdWithAlign:(Yodo1MasAdBannerAlign)align offset:CGPointMake(offsetX, offsetY)];
}

void UnityShowBannerAdWithPlacementAndAlignAndOffset(const char* placementId, int align, int offsetX, int offsetY)
{
    NSString* m_placementId = Yodo1MasCreateNSString(placementId);
    [[Yodo1MasBridge sharedInstance] showBannerAdWithPlacement:m_placementId align:(Yodo1MasAdBannerAlign)align offset:CGPointMake(offsetX, offsetY)];
}

void UnityDismissBannerAd()
{
    [[Yodo1MasBridge sharedInstance] dismissBannerAd];
}

void UnityDismissBannerAdWithDestroy(bool destroy)
{
    [[Yodo1MasBridge sharedInstance] dismissBannerAdWithDestroy:destroy];
}

#pragma mark - Unity Interstitial

bool UnityIsInterstitialLoaded()
{
    return [[Yodo1MasBridge sharedInstance] isInterstitialAdLoaded];
}

void UnityShowInterstitialAd()
{
    [[Yodo1MasBridge sharedInstance] showInterstitialAd];
}

void UnityShowInterstitialAdWithPlacementId(const char* placementId)
{
    NSString* m_placementId = Yodo1MasCreateNSString(placementId);
    [[Yodo1MasBridge sharedInstance] showInterstitialAd:m_placementId];
}

#pragma mark - Unity Rewarded Ad

bool UnityIsRewardedAdLoaded()
{
    return [[Yodo1MasBridge sharedInstance] isRewardedAdLoaded];
}

void UnityShowRewardedAd()
{
    [[Yodo1MasBridge sharedInstance] showRewardedAd];
}

void UnityShowRewardedAdWithPlacementId(const char* placementId)
{
    NSString* m_placementId = Yodo1MasCreateNSString(placementId);
    [[Yodo1MasBridge sharedInstance] showRewardedAd:m_placementId];
}

}
#endif
