//
//  Yodo1MasBannerAdView+Bridge.m
//  UnityFramework
//
//  Created by 周玉震 on 2021/11/28.
//

#import "Yodo1MasRewardAd+Bridge.h"
#import <objc/runtime.h>

@implementation Yodo1MasBridgeRewardAdConfig

+ (Yodo1MasBridgeRewardAdConfig *)parse:(id)json {
    Yodo1MasBridgeRewardAdConfig *config = [[Yodo1MasBridgeRewardAdConfig alloc] init];
    if (json[@"adPlacement"]) {
        config.adPlacement = json[@"adPlacement"];
    } else {
        config.adPlacement = @"";
    }
    
    if (json[@"autoDelayIfLoadFail"]) {
        config.autoDelayIfLoadFail = [json[@"autoDelayIfLoadFail"] boolValue];
    } else {
        config.autoDelayIfLoadFail = NO;
    }

    return config;
}

@end

@implementation Yodo1MasRewardAd(Bridge)

- (Yodo1MasBridgeRewardAdConfig *)yodo1_config {
    return objc_getAssociatedObject(self, _cmd);
}

- (void)setYodo1_config:(Yodo1MasBridgeRewardAdConfig *)yodo1_config {
    objc_setAssociatedObject(self, @selector(yodo1_config), yodo1_config, OBJC_ASSOCIATION_RETAIN_NONATOMIC);
}


@end
