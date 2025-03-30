//
//  Yodo1MasRewardedInterstitialAd+Bridge.h
//  UnityFramework
//
//  Created by 周玉震 on 2021/11/28.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#if __has_include(<Yodo1MasCore/Yodo1MasRewardedInterstitialAd.h>)
#import <Yodo1MasCore/Yodo1MasRewardedInterstitialAd.h>
#else
#import "Yodo1MasRewardedInterstitialAd.h"
#endif

NS_ASSUME_NONNULL_BEGIN

@interface Yodo1MasBridgeRewardedInterstitialAdConfig : NSObject

+ (Yodo1MasBridgeRewardedInterstitialAdConfig *)parse:(id)json;

@property (nonatomic, copy) NSString *adPlacement;
@property (nonatomic, copy) NSString *customData;
@property (nonatomic, assign) BOOL autoDelayIfLoadFail;
@property (nonatomic, assign) int payRevenueEventCount;

@end

@interface Yodo1MasRewardedInterstitialAd(Bridge)

@property (nonatomic, strong) Yodo1MasBridgeRewardedInterstitialAdConfig *yodo1_config;

@end

NS_ASSUME_NONNULL_END
