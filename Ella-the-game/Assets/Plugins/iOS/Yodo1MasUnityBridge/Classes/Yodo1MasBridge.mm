//
//  UnityYodo1Mas.mm
//  Pods
//
//  Created by sunmeng on 2020/12/21.
//

#import "Yodo1MasBridge.h"

#import "Yodo1Mas.h"
#import "Yodo1MasUnityTool.h"
#import "UnityAppController.h"

#import "Yodo1MasRewardAd+Bridge.h"
#import "Yodo1MasInterstitialAd+Bridge.h"
#import "Yodo1MasRewardedInterstitialAd+Bridge.h"
#import "Yodo1MasAppOpenAd+Bridge.h"
#import "Yodo1MasNativeAdView+Bridge.h"
#import "Yodo1MasBannerAdView+Bridge.h"
#import "Yodo1MasBannerAdHelper.h"

static NSString* kYodo1MasGameObject;
static NSString* kYodo1MasMethodName;

@interface Yodo1MasBridge : NSObject <
Yodo1MasRewardAdDelegate,
Yodo1MasRewardDelegate,
Yodo1MasInterstitialAdDelegate,
Yodo1MasInterstitialDelegate,
Yodo1MasBannerAdDelegate,
Yodo1MasBannerAdViewDelegate,
Yodo1MasNativeAdViewDelegate,
Yodo1MasRewardedInterstitialAdDelegate,
Yodo1MasAppOpenAdDelegate>

+ (UIViewController*)getRootViewController;

+ (UIViewController*)topMostViewController:(UIViewController*)controller;

+ (NSString *)stringWithJSONObject:(id)obj error:(NSError**)error;

+ (id)JSONObjectWithString:(NSString*)str error:(NSError**)error;

+ (NSString*)getSendMessage:(int)flag data:(NSString*)data;

+ (Yodo1MasBridge *)sharedInstance;

- (void)initWithAppKey:(NSString *)appId successful:(Yodo1MasInitSuccessful)successful fail:(Yodo1MasInitFail)fail;
- (void)initMasWithAppKey:(NSString *)appKey successful:(Yodo1MasInitSuccessful)successful fail:(Yodo1MasInitFail)fail;

#pragma mark - Reward
- (BOOL)isRewardedAdLoaded;
- (void)showRewardedAd;
- (void)showRewardedAd:(NSString *)placementId;

- (void)loadRewardAdV2:(NSString *)param;
- (BOOL)isRewardedAdLoadedV2:(NSString *)param;
- (void)showRewardAdV2:(NSString *)param;
- (void)destroyRewardAdV2:(NSString *)param;

#pragma mark - Interstitial
- (BOOL)isInterstitialAdLoaded;
- (void)showInterstitialAd;
- (void)showInterstitialAd:(NSString *)placementId;

- (void)loadInterstitialAdV2:(NSString *)param;
- (BOOL)isInterstitialAdLoadedV2:(NSString *)param;
- (void)showInterstitialAdV2:(NSString *)param;
- (void)destroyInterstitialAdV2:(NSString *)param;

#pragma mark - AppOpen
- (void)loadAppOpenAd:(NSString *)param;
- (BOOL)isAppOpenAdLoaded:(NSString *)param;
- (void)showAppOpenAd:(NSString *)param;
- (void)destroyAppOpenAd:(NSString *)param;

#pragma mark - RewardedInterstitial
- (void)loadRewardedInterstitialAd:(NSString *)param;
- (BOOL)isRewardedInterstitialAdLoaded:(NSString *)param;
- (void)showRewardedInterstitialAd:(NSString *)param;
- (void)destroyRewardedInterstitialAd:(NSString *)param;

#pragma mark - Native
@property (nonatomic, strong) NSMutableDictionary<NSString*, Yodo1MasNativeAdView *> *nativeViews;
- (void)loadNativeAd:(NSString *)param;
- (void)showNativeAd:(NSString *)param;
- (void)hideNativeAd:(NSString *)param;
- (void)destroyNativeAd:(NSString *)param;


#pragma mark - Banner
- (BOOL)isBannerAdLoaded;
- (void)showBannerAd;
- (void)showBannerAdWithPlacement:(NSString *)placementId;
- (void)showBannerAdWithAlign:(Yodo1MasAdBannerAlign)align;
- (void)showBannerAdWithAlign:(Yodo1MasAdBannerAlign)align offset:(CGPoint)offset;
- (void)showBannerAdWithPlacement:(NSString *)placement align:(Yodo1MasAdBannerAlign)align offset:(CGPoint)offset;
- (void)dismissBannerAd;
- (void)dismissBannerAdWithDestroy:(BOOL)destroy;

@property (nonatomic, strong) NSMutableDictionary<NSString*, Yodo1MasBannerAdView *> *bannerViews;
- (void)loadBannerAdV2:(NSString *)param;
- (void)showBannerAdV2:(NSString *)param;
- (void)hideBannerAdV2:(NSString *)param;
- (void)destroyBannerAdV2:(NSString *)param;

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

- (void)initWithAppKey:(NSString *)appKey successful:(Yodo1MasInitSuccessful)successful fail:(Yodo1MasInitFail)fail {
    [Yodo1Mas sharedInstance].rewardAdDelegate = self;
    [Yodo1Mas sharedInstance].interstitialAdDelegate = self;
    [Yodo1Mas sharedInstance].bannerAdDelegate = self;
    
    _bannerViews = [NSMutableDictionary dictionary];
    _nativeViews = [NSMutableDictionary dictionary];
    
    [[Yodo1Mas sharedInstance] initWithAppKey:appKey successful:successful fail:fail];
    
    if (![UIDevice currentDevice].generatesDeviceOrientationNotifications) {
        [[UIDevice currentDevice] beginGeneratingDeviceOrientationNotifications];
    }
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(handleDeviceOrientationChange:)
                                         name:UIDeviceOrientationDidChangeNotification object:nil];
}

- (void)initMasWithAppKey:(NSString *)appKey successful:(Yodo1MasInitSuccessful)successful fail:(Yodo1MasInitFail)fail {
    
    _bannerViews = [NSMutableDictionary dictionary];
    _nativeViews = [NSMutableDictionary dictionary];
    
    [[Yodo1Mas sharedInstance] initMasWithAppKey:appKey successful:successful fail:fail];
    
    if (![UIDevice currentDevice].generatesDeviceOrientationNotifications) {
        [[UIDevice currentDevice] beginGeneratingDeviceOrientationNotifications];
    }
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(handleDeviceOrientationChange:)
                                         name:UIDeviceOrientationDidChangeNotification object:nil];
}

#pragma mark - Reward
- (Yodo1MasRewardAd *)getRewardAdFromJson:(NSString *)json {
    NSError *error = nil;
    id dict = [Yodo1MasBridge JSONObjectWithString:json error:&error];
    if (!dict || error) {
        return nil;
    }
    
    Yodo1MasBridgeRewardAdConfig *config = [Yodo1MasBridgeRewardAdConfig parse:dict];
 
    Yodo1MasRewardAd *ad = [Yodo1MasRewardAd sharedInstance];
    ad.yodo1_config = config;
    ad.adDelegate = self;
    ad.autoDelayIfLoadFail = config.autoDelayIfLoadFail;
    return ad;
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

- (void)loadRewardAdV2:(NSString *)param {
    Yodo1MasRewardAd *ad = [self getRewardAdFromJson:param];
    [ad loadAd];
}

- (BOOL)isRewardedAdLoadedV2:(NSString *)param {
    Yodo1MasRewardAd *ad = [self getRewardAdFromJson:param];
    return ad.isLoaded;
}

- (void)showRewardAdV2:(NSString *)param {
    Yodo1MasRewardAd* ad = [self getRewardAdFromJson:param];
    [ad showAdWithPlacement:ad.yodo1_config.adPlacement];
}

- (void)destroyRewardAdV2:(NSString *)param {
    Yodo1MasRewardAd *ad = [self getRewardAdFromJson:param];
    [ad destroy];
}

#pragma mark - Yodo1MasRewardDelegate
- (void)onRewardAdLoaded:(Yodo1MasRewardAd *)ad {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeLoaded type:Yodo1MasAdTypeReward];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onRewardAdFailedToLoad:(Yodo1MasRewardAd *)ad withError:(Yodo1MasError *)error {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeLoadFail type:Yodo1MasAdTypeReward error:error];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onRewardAdOpened:(Yodo1MasRewardAd *)ad {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeOpened type:Yodo1MasAdTypeReward];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onRewardAdFailedToOpen:(Yodo1MasRewardAd *)ad withError:(Yodo1MasError *)error {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeOpenFail type:Yodo1MasAdTypeReward error:error];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onRewardAdClosed:(Yodo1MasRewardAd *)ad {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeClosed type:Yodo1MasAdTypeReward];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onRewardAdEarned:(Yodo1MasRewardAd *)ad {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeRewardEarned type:Yodo1MasAdTypeReward];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

#pragma mark - Interstitial
- (Yodo1MasInterstitialAd *)getInterstitialAdFromJson:(NSString *)json {
    NSError *error = nil;
    id dict = [Yodo1MasBridge JSONObjectWithString:json error:&error];
    if (!dict || error) {
        return nil;
    }
    
    Yodo1MasBridgeInterstitialAdConfig *config = [Yodo1MasBridgeInterstitialAdConfig parse:dict];
  
    Yodo1MasInterstitialAd *ad = [Yodo1MasInterstitialAd sharedInstance];
    ad.yodo1_config = config;
    ad.adDelegate = self;
    ad.autoDelayIfLoadFail = config.autoDelayIfLoadFail;
    return ad;
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

- (void)loadInterstitialAdV2:(NSString *)param {
    Yodo1MasInterstitialAd *ad = [self getInterstitialAdFromJson:param];
    [ad loadAd];
}

- (BOOL)isInterstitialAdLoadedV2:(NSString *)param {
    Yodo1MasInterstitialAd *ad = [self getInterstitialAdFromJson:param];
    return ad.isLoaded;
}

- (void)showInterstitialAdV2:(NSString *)param {
    Yodo1MasInterstitialAd* ad = [self getInterstitialAdFromJson:param];
    [ad showAdWithPlacement:ad.yodo1_config.adPlacement];
}

- (void)destroyInterstitialAdV2:(NSString *)param {
    Yodo1MasInterstitialAd *ad = [self getInterstitialAdFromJson:param];
    if (ad) {
        [ad destroy];
    }
    ad = nil;
}

#pragma mark - Yodo1MasInterstitialDelegate
- (void)onInterstitialAdLoaded:(Yodo1MasInterstitialAd *)ad {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeLoaded type:Yodo1MasAdTypeInterstitial];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onInterstitialAdFailedToLoad:(Yodo1MasInterstitialAd *)ad withError:(Yodo1MasError *)error {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeLoadFail type:Yodo1MasAdTypeInterstitial error:error];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onInterstitialAdOpened:(Yodo1MasInterstitialAd *)ad {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeOpened type:Yodo1MasAdTypeInterstitial];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onInterstitialAdFailedToOpen:(Yodo1MasInterstitialAd *)ad withError:(Yodo1MasError *)error {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeOpenFail type:Yodo1MasAdTypeInterstitial error:error];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onInterstitialAdClosed:(Yodo1MasInterstitialAd *)ad {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeClosed type:Yodo1MasAdTypeInterstitial];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

#pragma mark - AppOpen
- (Yodo1MasAppOpenAd *)getAppOpenAdFromJson:(NSString *)json {
    NSError *error = nil;
    id dict = [Yodo1MasBridge JSONObjectWithString:json error:&error];
    if (!dict || error) {
        return nil;
    }
    
    Yodo1MasBridgeAppOpenAdConfig *config = [Yodo1MasBridgeAppOpenAdConfig parse:dict];
 
    Yodo1MasAppOpenAd *ad = [Yodo1MasAppOpenAd sharedInstance];
    ad.yodo1_config = config;
    ad.adDelegate = self;
    ad.autoDelayIfLoadFail = config.autoDelayIfLoadFail;
    return ad;
}

- (void)loadAppOpenAd:(NSString *)param {
    Yodo1MasAppOpenAd *ad = [self getAppOpenAdFromJson:param];
    [ad loadAd];
}

- (BOOL)isAppOpenAdLoaded:(NSString *)param {
    Yodo1MasAppOpenAd *ad = [self getAppOpenAdFromJson:param];
    return ad.isLoaded;
}

- (void)showAppOpenAd:(NSString *)param {
    Yodo1MasAppOpenAd* ad = [self getAppOpenAdFromJson:param];
    [ad showAdWithPlacement:ad.yodo1_config.adPlacement];
}

- (void)destroyAppOpenAd:(NSString *)param {
    Yodo1MasAppOpenAd *ad = [self getAppOpenAdFromJson:param];
    [ad destroy];
}

#pragma mark - Yodo1MasAppOpenAdDelegate
- (void)onAppOpenAdLoaded:(Yodo1MasAppOpenAd *)ad {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeLoaded type:Yodo1MasAdTypeAppOpen];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onAppOpenAdFailedToLoad:(Yodo1MasAppOpenAd *)ad withError:(Yodo1MasError *)error {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeLoadFail type:Yodo1MasAdTypeAppOpen error:error];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onAppOpenAdOpened:(Yodo1MasAppOpenAd *)ad {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeOpened type:Yodo1MasAdTypeAppOpen];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onAppOpenAdFailedToOpen:(Yodo1MasAppOpenAd *)ad withError:(Yodo1MasError *)error {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeOpenFail type:Yodo1MasAdTypeAppOpen error:error];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onAppOpenAdClosed:(Yodo1MasAppOpenAd *)ad {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeClosed type:Yodo1MasAdTypeAppOpen];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

#pragma mark - RewardedInterstitial
- (Yodo1MasRewardedInterstitialAd *)getRewardedInterstitialAdFromJson:(NSString *)json {
    NSError *error = nil;
    id dict = [Yodo1MasBridge JSONObjectWithString:json error:&error];
    if (!dict || error) {
        return nil;
    }
    
    Yodo1MasBridgeRewardedInterstitialAdConfig *config = [Yodo1MasBridgeRewardedInterstitialAdConfig parse:dict];
 
    Yodo1MasRewardedInterstitialAd *ad = [Yodo1MasRewardedInterstitialAd sharedInstance];
    ad.yodo1_config = config;
    ad.adDelegate = self;
    ad.autoDelayIfLoadFail = config.autoDelayIfLoadFail;
    return ad;
}

- (void)loadRewardedInterstitialAd:(NSString *)param {
    Yodo1MasRewardedInterstitialAd *ad = [self getRewardedInterstitialAdFromJson:param];
    [ad loadAd];
}

- (BOOL)isRewardedInterstitialAdLoaded:(NSString *)param {
    Yodo1MasRewardedInterstitialAd *ad = [self getRewardedInterstitialAdFromJson:param];
    return ad.isLoaded;
}

- (void)showRewardedInterstitialAd:(NSString *)param {
    Yodo1MasRewardedInterstitialAd* ad = [self getRewardedInterstitialAdFromJson:param];
    [ad showAdWithPlacement:ad.yodo1_config.adPlacement];
}

- (void)destroyRewardedInterstitialAd:(NSString *)param {
    Yodo1MasRewardedInterstitialAd *ad = [self getRewardedInterstitialAdFromJson:param];
    [ad destroy];
}

#pragma mark - Yodo1MasRewardedInterstitialDelegate
- (void)onRewardedInterstitialAdLoaded:(Yodo1MasRewardedInterstitialAd *)ad {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeLoaded type:Yodo1MasAdTypeRewardedInterstitial];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onRewardedInterstitialAdFailedToLoad:(Yodo1MasRewardedInterstitialAd *)ad withError:(Yodo1MasError *)error {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeLoadFail type:Yodo1MasAdTypeRewardedInterstitial error:error];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onRewardedInterstitialAdOpened:(Yodo1MasRewardedInterstitialAd *)ad {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeOpened type:Yodo1MasAdTypeRewardedInterstitial];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onRewardedInterstitialAdFailedToOpen:(Yodo1MasRewardedInterstitialAd *)ad withError:(Yodo1MasError *)error {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeOpenFail type:Yodo1MasAdTypeRewardedInterstitial error:error];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onRewardedInterstitialAdClosed:(Yodo1MasRewardedInterstitialAd *)ad {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeClosed type:Yodo1MasAdTypeRewardedInterstitial];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onRewardedInterstitialAdEarned:(Yodo1MasRewardedInterstitialAd *)ad {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeRewardEarned type:Yodo1MasAdTypeRewardedInterstitial];
    [Yodo1MasBridge sendMessageWithEvent: event];
}

#pragma mark - Banner
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

- (void)dismissBannerAd {
    [[Yodo1Mas sharedInstance] dismissBannerAd];
}

- (void)dismissBannerAdWithDestroy:(BOOL)destroy {
    [[Yodo1Mas sharedInstance] dismissBannerAdWithDestroy:destroy];
}

- (Yodo1MasBannerAdView *)getBannerViewFromJson:(NSString *)json needInit:(BOOL)needInit {
    NSError *error = nil;
    id dict = [Yodo1MasBridge JSONObjectWithString:json error:&error];
    if (!dict || error) {
        return nil;
    }
    
    Yodo1MasBridgeBannerAdConfig *config = [Yodo1MasBridgeBannerAdConfig parse:dict];
    if (!config.indexId) {
        return nil;
    }
    Yodo1MasBannerAdView *adView = self.bannerViews[config.indexId];
    if (!adView && needInit) {
        adView = [[Yodo1MasBannerAdView alloc] init];
        adView.yodo1_config = config;
        adView.adDelegate = self;
        [adView setAdSize:config.adSize];
        if (config.adPlacement != nil && config.adPlacement.length > 0) {
            [adView setAdPlacement:config.adPlacement];
        }
        self.bannerViews[config.indexId] = adView;
    }
    return adView;
}

- (void)loadBannerAdV2:(NSString *)jsonString {
    Yodo1MasBannerAdView *adView = [self getBannerViewFromJson:jsonString needInit:YES];

    [GetAppController().rootViewController.view addSubview:adView];
    [adView loadAd];
    [self adjustFrame:adView];
}

- (void)handleDeviceOrientationChange:(NSNotification *)notification {
    UIDeviceOrientation deviceOrientation = [UIDevice currentDevice].orientation;
    switch (deviceOrientation) {
        case UIDeviceOrientationLandscapeLeft:
        case UIDeviceOrientationLandscapeRight:
        case UIDeviceOrientationPortrait:
        case UIDeviceOrientationPortraitUpsideDown:
            [self adjustFrame];
            break;
        default:
            break;
    }
}

- (void)adjustFrame:(Yodo1MasBannerAdView *)adView {
    if (adView == nil) {
        return;
    }
    
    Yodo1MasBridgeBannerAdConfig *config = adView.yodo1_config;
    if (!config) {
        return;
    }
    
    if (config.customPosition.x > 0 || config.customPosition.y > 0) {
        [[Yodo1MasBannerAdHelper sharedInstance] adjustFrame:adView adPosition:Yodo1MasAdBannerAlignLeft | Yodo1MasAdBannerAlignTop offset:config.customPosition];
    } else {
        [[Yodo1MasBannerAdHelper sharedInstance] adjustFrame:adView adPosition:config.adPosition offset:CGPointZero];
    }
}

- (void)adjustFrame {
    for (Yodo1MasBannerAdView *adView in self.bannerViews.allValues) {
        [self adjustFrame:adView];
    }
}

- (void)showBannerAdV2:(NSString *)param {
    Yodo1MasBannerAdView* adView = [self getBannerViewFromJson:param needInit:NO];
    if (adView) {
        [adView removeFromSuperview];
        UnityAppController * controller = GetAppController();
        [controller.rootViewController.view addSubview:adView];
        [self adjustFrame:adView];
    }
}

- (void)hideBannerAdV2:(NSString *)param {
    Yodo1MasBannerAdView *adView = [self getBannerViewFromJson:param needInit:NO];
    if (adView) {
        [adView removeFromSuperview];
    }
}

- (void)destroyBannerAdV2:(NSString *)param {
    Yodo1MasBannerAdView *adView = [self getBannerViewFromJson:param needInit:NO];
    if (adView) {
        [adView removeFromSuperview];
        [adView destroy];
        [self.bannerViews removeObjectForKey:adView.yodo1_config.indexId];
    }
    adView = nil;
}

- (CGFloat)getBannerWidth:(int)type {
    return [Yodo1MasBanner sizeFromAdSize:(Yodo1MasBannerAdSize)type].width;
}

- (CGFloat)getBannerHeight:(int)type {
    return [Yodo1MasBanner sizeFromAdSize:(Yodo1MasBannerAdSize)type].height;
}

- (CGFloat)getBannerWidthInPixels:(int)type {
    return [Yodo1MasBanner pixelsFromAdSize:(Yodo1MasBannerAdSize)type].width;
}

- (CGFloat)getBannerHeightInPixels:(int)type {
    return [Yodo1MasBanner pixelsFromAdSize:(Yodo1MasBannerAdSize)type].height;
}

#pragma mark - Yodo1MasBannerAdViewDelegate
- (void)onBannerAdLoaded:(Yodo1MasBannerAdView *)banner {
    [self adjustFrame:banner];

    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeLoaded type:Yodo1MasAdTypeBanner];
    if (event == nil) {
        return;
    }
    
    NSString *index = banner.yodo1_config.indexId;
    NSMutableDictionary *dic = (NSMutableDictionary*)event.getJsonObject;
    [dic setObject:index forKey:@"indexId"];
    
    [Yodo1MasBridge sendMessageWithJson: dic];
}

- (void)onBannerAdFailedToLoad:(Yodo1MasBannerAdView *)banner withError:(Yodo1MasError *)error {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:(Yodo1MasAdEventCode)1004 type:Yodo1MasAdTypeBanner error:error];
    if (event == nil) {
        return;
    }
    NSString* index = banner.yodo1_config.indexId;
    NSMutableDictionary* dic = (NSMutableDictionary*)event.getJsonObject;
    [dic setObject:index forKey:@"indexId"];
    
    [Yodo1MasBridge sendMessageWithJson: dic];
}

- (void)onBannerAdOpened:(Yodo1MasBannerAdView *)banner {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeOpened type:Yodo1MasAdTypeBanner];
    if (event == nil) {
        return;
    }
    NSString* index = banner.yodo1_config.indexId;
    NSMutableDictionary* dic = (NSMutableDictionary*)event.getJsonObject;
    [dic setObject:index forKey:@"indexId"];
    
    [Yodo1MasBridge sendMessageWithJson: dic];
}

- (void)onBannerAdClosed:(Yodo1MasBannerAdView *)banner {
//    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeClosed type:Yodo1MasAdTypeBanner];
//    if (event == nil) {
//        return;
//    }
//    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onBannerAdFailedToOpen:(Yodo1MasBannerAdView *)banner withError:(Yodo1MasError *)error {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:(Yodo1MasAdEventCode)1005 type:Yodo1MasAdTypeBanner error:error];
    if (event == nil) {
        return;
    }
    NSString *index = banner.yodo1_config.indexId;
    NSMutableDictionary* dic = (NSMutableDictionary*)event.getJsonObject;
    [dic setObject:index forKey:@"indexId"];
    
    [Yodo1MasBridge sendMessageWithJson: dic];
}

#pragma mark - Native
- (Yodo1MasNativeAdView *)getNativeViewFromJson:(NSString *)json needInit:(BOOL)needInit {
    NSError *error = nil;
    id dict = [Yodo1MasBridge JSONObjectWithString:json error:&error];
    if (!dict || error) {
        return nil;
    }
    
    Yodo1MasBridgeNativeAdConfig *config = [Yodo1MasBridgeNativeAdConfig parse:dict];
    if (!config.indexId) {
        return nil;
    }

    UIView *superview = GetAppController().rootViewController.view;
    Yodo1MasAdNativeAlign align = config.position;
    CGFloat width = config.width / UIScreen.mainScreen.scale;
    CGFloat height = config.height / UIScreen.mainScreen.scale;
    CGFloat offsetX = config.offsetX / UIScreen.mainScreen.scale;
    CGFloat offsetY = config.offsetY / UIScreen.mainScreen.scale;
    CGRect frame = CGRectMake(0, 0, width, height);
    if (align != 0) {
        if ((align & Yodo1MasAdNativeAlignLeft) == Yodo1MasAdNativeAlignLeft) {
            frame.origin.x = 0;
        } else if ((align & Yodo1MasAdNativeAlignRight) == Yodo1MasAdNativeAlignRight) {
            frame.origin.x = superview.bounds.size.width - frame.size.width;
        } else {
            frame.origin.x = (superview.bounds.size.width - frame.size.width) / 2;
        }
           
        // vertical
        if ((align & Yodo1MasAdNativeAlignTop) == Yodo1MasAdNativeAlignTop) {
            if (@available(iOS 11, *)) {
                frame.origin.y = superview.safeAreaInsets.top;
            } else {
                frame.origin.y = 0;
            }
        } else if ((align & Yodo1MasAdNativeAlignBottom) == Yodo1MasAdNativeAlignBottom) {
            if (@available(iOS 11, *)) {
                frame.origin.y = superview.bounds.size.height - frame.size.height - superview.safeAreaInsets.bottom;
            } else {
                frame.origin.y = superview.bounds.size.height - frame.size.height;
            }
        } else {
            frame.origin.y = (superview.bounds.size.height - frame.size.height) / 2;
        }
        
        frame.origin.x += offsetX;
        frame.origin.y += offsetY;
    } else {
        frame.origin.x = config.x / UIScreen.mainScreen.scale;
        frame.origin.y = config.y / UIScreen.mainScreen.scale;
    }
    
    Yodo1MasNativeAdView *adView = self.nativeViews[config.indexId];
    if (!adView && needInit) {
        adView = [[Yodo1MasNativeAdView alloc] init];
        adView.yodo1_config = config;
        adView.adDelegate = self;
        if (config.adPlacement != nil && config.adPlacement.length > 0) {
            [adView setAdPlacement:config.adPlacement];
        }
        self.nativeViews[config.indexId] = adView;
    }
    if (config.backgroundColor) {
        if ([config.backgroundColor isEqualToString:@"#000000"]) {
            config.backgroundColor = @"#010101";
        }
        UIColor *color = [Yodo1MasUserPrivacyConfig colorWithHexString:config.backgroundColor];
        if (color && color != [UIColor clearColor]) {
            adView.adBackgroundColor = color;
        } else {
            adView.adBackgroundColor = nil;
        }
    }
    
    adView.frame = frame;
    return adView;
}

- (void)loadNativeAd:(NSString *)param {
    Yodo1MasNativeAdView *adView = [self getNativeViewFromJson:param needInit:YES];

    [GetAppController().rootViewController.view addSubview:adView];
    [adView loadAd];
}

- (void)showNativeAd:(NSString *)param {
    Yodo1MasNativeAdView* adView = [self getNativeViewFromJson:param needInit:NO];
    if (adView) {
        [adView removeFromSuperview];
        UnityAppController * controller = GetAppController();
        [controller.rootViewController.view addSubview:adView];
    }
}

- (void)hideNativeAd:(NSString *)param {
    Yodo1MasNativeAdView *adView = [self getNativeViewFromJson:param needInit:NO];
    if (adView) {
        [adView removeFromSuperview];
    }
}

- (void)destroyNativeAd:(NSString *)param {
    Yodo1MasNativeAdView *adView = [self getNativeViewFromJson:param needInit:NO];
    if (adView) {
        [adView removeFromSuperview];
        [adView destroy];
        [self.nativeViews removeObjectForKey:adView.yodo1_config.indexId];
    }
    adView = nil;
}



#pragma mark - Yodo1MasNativeAdViewDelegate
- (void)onNativeAdLoaded:(Yodo1MasNativeAdView *)view {
    
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:Yodo1MasAdEventCodeLoaded type:Yodo1MasAdTypeNative];
    if (event == nil) {
        return;
    }
    
    NSString *index = view.yodo1_config.indexId;
    NSMutableDictionary *dic = (NSMutableDictionary*)event.getJsonObject;
    [dic setObject:index forKey:@"indexId"];
    
    [Yodo1MasBridge sendMessageWithJson: dic];
}

- (void)onNativeAdFailedToLoad:(Yodo1MasNativeAdView *)view withError:(Yodo1MasError *)error {
    Yodo1MasAdEvent *event = [[Yodo1MasAdEvent alloc] initWithCode:(Yodo1MasAdEventCode)1004 type:Yodo1MasAdTypeNative error:error];
    if (event == nil) {
        return;
    }
    NSString* index = view.yodo1_config.indexId;
    NSMutableDictionary* dic = (NSMutableDictionary*)event.getJsonObject;
    [dic setObject:index forKey:@"indexId"];
    
    [Yodo1MasBridge sendMessageWithJson: dic];
}

#pragma mark - Yodo1MasAdDelegate
- (void)onAdOpened:(Yodo1MasAdEvent *)event {
    if (event == nil) {
        return;
    }
    [Yodo1MasBridge sendMessageWithEvent: event];

}

- (void)onAdClosed:(Yodo1MasAdEvent *)event {
    if (event == nil) {
        return;
    }
    [Yodo1MasBridge sendMessageWithEvent: event];
}

- (void)onAdError:(Yodo1MasAdEvent *)event error:(Yodo1MasError *)error {
    if (event == nil) {
        return;
    }
    [Yodo1MasBridge sendMessageWithEvent: event];
}

#pragma mark - Yodo1MasRewardAdvertDelegate
- (void)onAdRewardEarned:(Yodo1MasAdEvent *)event {
    if (event == nil) {
        return;
    }
    [Yodo1MasBridge sendMessageWithEvent: event];
}

#pragma mark - Private
+ (void)sendMessageWithEvent:(Yodo1MasAdEvent *)event {
    [Yodo1MasBridge sendMessageWithJson: event.getJsonObject];
}

+ (void)sendMessageWithJson:(id)json {
    NSString* data = [Yodo1MasBridge stringWithJSONObject:json error:nil];
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

int UnityMasUserAge()
{
    return (int)[Yodo1Mas sharedInstance].userAge;
}

int UnityMasAttrackingStatus()
{
    return (int)[Yodo1Mas sharedInstance].attrackingStatus;
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
    
    [[Yodo1MasBridge sharedInstance] initWithAppKey:m_appKey successful:^{
        NSString* data = [Yodo1MasBridge convertToInitJsonString:1 masError:nil];
        NSString* msg = [Yodo1MasBridge getSendMessage:0 data:data];
        UnitySendMessage([kYodo1MasGameObject cStringUsingEncoding:NSUTF8StringEncoding], [kYodo1MasMethodName cStringUsingEncoding:NSUTF8StringEncoding], [msg cStringUsingEncoding:NSUTF8StringEncoding]);
    } fail:^(Yodo1MasError * _Nonnull error) {
        NSString* data = [Yodo1MasBridge convertToInitJsonString:0 masError:error];
        NSString* msg = [Yodo1MasBridge getSendMessage:0 data:data];
        UnitySendMessage([kYodo1MasGameObject cStringUsingEncoding:NSUTF8StringEncoding], [kYodo1MasMethodName cStringUsingEncoding:NSUTF8StringEncoding], [msg cStringUsingEncoding:NSUTF8StringEncoding]);
    }];
}

void UnityMasInitMasWithAppKey(const char* appKey,const char* gameObjectName, const char* callbackMethodName)
{
    NSString* m_appKey = Yodo1MasCreateNSString(appKey);
    NSCAssert(m_appKey != nil, @"AppKey 没有设置!");
    
    NSString* m_gameObject = Yodo1MasCreateNSString(gameObjectName);
    NSCAssert(m_gameObject != nil, @"Unity3d gameObject isn't set!");
    kYodo1MasGameObject = m_gameObject;
    
    NSString* m_methodName = Yodo1MasCreateNSString(callbackMethodName);
    NSCAssert(m_methodName != nil, @"Unity3d callback method isn't set!");
    kYodo1MasMethodName = m_methodName;
    
    [[Yodo1MasBridge sharedInstance] initMasWithAppKey:m_appKey successful:^{
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
    
    
    id agePop = dict[@"userPrivacyConfig"];
    if (agePop) {
        NSError * error = nil;
        id ageJson = [Yodo1MasBridge JSONObjectWithString:agePop error:&error];
        if (error || !ageJson) {
            return;
        }
        
        Yodo1MasUserPrivacyConfig *privacyConfig = [Yodo1MasUserPrivacyConfig instance];
         
        NSString *titleBackgroundColorStr = ageJson[@"titleBackgroundColor"];
        if (titleBackgroundColorStr) {
            UIColor *color = [Yodo1MasUserPrivacyConfig colorWithHexString:titleBackgroundColorStr];
            if (color && color != [UIColor clearColor]) {
                privacyConfig.titleBackgroundColor = color;
            }
        }
        
        NSString *titleTextColorStr = ageJson[@"titleTextColor"];
        if (titleTextColorStr) {
            UIColor *color = [Yodo1MasUserPrivacyConfig colorWithHexString:titleTextColorStr];
            if (color && color != [UIColor clearColor]) {
                privacyConfig.titleTextColor = color;
            }
        }
        
        NSString *contentBackgroundColorStr = ageJson[@"contentBackgroundColor"];
        if (contentBackgroundColorStr) {
            UIColor *color = [Yodo1MasUserPrivacyConfig colorWithHexString:contentBackgroundColorStr];
            if (color && color != [UIColor clearColor]) {
                privacyConfig.contentBackgroundColor = color;
            }
        }
        
        NSString *contentTextColorStr = ageJson[@"contentTextColor"];
        if (contentTextColorStr) {
            UIColor *color = [Yodo1MasUserPrivacyConfig colorWithHexString:contentTextColorStr];
            if (color && color != [UIColor clearColor]) {
                privacyConfig.contentTextColor = color;
            }
        }
        
        NSString *buttonBackgroundColorStr = ageJson[@"buttonBackgroundColor"];
        if (buttonBackgroundColorStr) {
            UIColor *color = [Yodo1MasUserPrivacyConfig colorWithHexString:buttonBackgroundColorStr];
            if (color && color != [UIColor clearColor]) {
                privacyConfig.buttonBackgroundColor = color;
            }
        }
        
        NSString *buttonTextColorStr = ageJson[@"buttonTextColor"];
        if (buttonTextColorStr) {
            UIColor *color = [Yodo1MasUserPrivacyConfig colorWithHexString:buttonTextColorStr];
            if (color && color != [UIColor clearColor]) {
                privacyConfig.buttonTextColor = color;
            }
        }
        
        buildConfig.userPrivacyConfig = privacyConfig;
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
    [[Yodo1MasBridge sharedInstance] showBannerAdWithAlign:(Yodo1MasAdBannerAlign)align offset:CGPointMake(offsetX/UIScreen.mainScreen.scale, offsetY/UIScreen.mainScreen.scale)];
}

void UnityShowBannerAdWithPlacementAndAlignAndOffset(const char* placementId, int align, int offsetX, int offsetY)
{
    NSString* m_placementId = Yodo1MasCreateNSString(placementId);
    [[Yodo1MasBridge sharedInstance] showBannerAdWithPlacement:m_placementId align:(Yodo1MasAdBannerAlign)align offset:CGPointMake(offsetX/UIScreen.mainScreen.scale, offsetY/UIScreen.mainScreen.scale)];
}

void UnityDismissBannerAd()
{
    [[Yodo1MasBridge sharedInstance] dismissBannerAd];
}

void UnityDismissBannerAdWithDestroy(bool destroy)
{
    [[Yodo1MasBridge sharedInstance] dismissBannerAdWithDestroy:destroy];
}

#pragma mark - Unity Banner V2
void UnityLoadBannerAdV2(const char* param) {
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] loadBannerAdV2:m_param];
}
void UnityShowBannerAdV2(const char* param) {
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] showBannerAdV2:m_param];
}
void UnityHideBannerAdV2(const char* param) {
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] hideBannerAdV2:m_param];
}
void UnityDestroyBannerAdV2(const char* param) {
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] destroyBannerAdV2:m_param];
}

int UnityGetBannerWidthV2(int type) {
    return (int)[[Yodo1MasBridge sharedInstance] getBannerWidth:type];
}

int UnityGetBannerHeightV2(int type) {
    return (int)[[Yodo1MasBridge sharedInstance] getBannerHeight:type];
}

float UnityGetBannerWidthInPixelsV2(int type) {
    float width =  [[Yodo1MasBridge sharedInstance] getBannerWidthInPixels:type];
    return width;
}

float UnityGetBannerHeightInPixelsV2(int type) {
    float height = [[Yodo1MasBridge sharedInstance] getBannerHeightInPixels:type];
    return height;
}

#pragma mark - Unity Native
void UnityLoadNativeAd(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] loadNativeAd:m_param];
}

void UnityShowNativeAd(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] showNativeAd:m_param];
}

void UnityHideNativeAd(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] hideNativeAd:m_param];
}

void UnityDestroyNativeAd(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] destroyNativeAd:m_param];
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

void UnityLoadInterstitialAdV2(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] loadInterstitialAdV2:m_param];
}

bool UnityIsInterstitialLoadedV2(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    return [[Yodo1MasBridge sharedInstance] isInterstitialAdLoadedV2:m_param];
}

void UnityShowInterstitialAdV2(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] showInterstitialAdV2:m_param];
}

void UnityDestroyInterstitialAdV2(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] destroyInterstitialAdV2:m_param];
}

#pragma mark - Unity Rewarded

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

void UnityLoadRewardAdV2(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] loadRewardAdV2:m_param];
}

bool UnityIsRewardedAdLoadedV2(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    return [[Yodo1MasBridge sharedInstance] isRewardedAdLoadedV2:m_param];
}

void UnityShowRewardAdV2(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] showRewardAdV2:m_param];
}

void UnityDestroyRewardAdV2(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] destroyRewardAdV2:m_param];
}

void UnityLoadRewardedInterstitialAd(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] loadRewardedInterstitialAd:m_param];
}

bool UnityIsRewardedInterstitialAdLoaded(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    return [[Yodo1MasBridge sharedInstance] isRewardedInterstitialAdLoaded:m_param];
}

void UnityShowRewardedInterstitialAd(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] showRewardedInterstitialAd:m_param];
}

void UnityDestroyRewardedInterstitialAd(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] destroyRewardedInterstitialAd:m_param];
}

void UnityLoadAppOpenAd(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] loadAppOpenAd:m_param];
}

bool UnityIsAppOpenAdLoaded(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    return [[Yodo1MasBridge sharedInstance] isAppOpenAdLoaded:m_param];
}

void UnityShowAppOpenAd(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] showAppOpenAd:m_param];
}

void UnityDestroyAppOpenAd(const char* param)
{
    NSString* m_param = Yodo1MasCreateNSString(param);
    [[Yodo1MasBridge sharedInstance] destroyAppOpenAd:m_param];
}

}
#endif
