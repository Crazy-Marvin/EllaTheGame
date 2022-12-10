//
//  Yodo1MasRewardAdView+Bridge.h
//  UnityFramework
//
//  Created by 周玉震 on 2021/11/28.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "Yodo1MasRewardAd.h"

NS_ASSUME_NONNULL_BEGIN

@interface Yodo1MasBridgeRewardAdConfig : NSObject

+ (Yodo1MasBridgeRewardAdConfig *)parse:(id)json;

@property (nonatomic, copy) NSString *adPlacement;
@property (nonatomic, assign) BOOL autoDelayIfLoadFail;

@end

@interface Yodo1MasRewardAd (Bridge)

@property (nonatomic, strong) Yodo1MasBridgeRewardAdConfig *yodo1_config;

@end

NS_ASSUME_NONNULL_END
