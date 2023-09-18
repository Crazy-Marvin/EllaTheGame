//
//  Yodo1MasBannerAdView+Bridge.m
//  UnityFramework
//
//  Created by 周玉震 on 2021/11/28.
//

#import "Yodo1MasBannerAdView+Bridge.h"
#import <objc/runtime.h>

@implementation Yodo1MasBridgeBannerAdConfig

+ (Yodo1MasBridgeBannerAdConfig *)parse:(id)json {
    Yodo1MasBridgeBannerAdConfig *config = [[Yodo1MasBridgeBannerAdConfig alloc] init];
    config.adSize = Yodo1MasBannerAdSizeBanner;
    if (json[@"adSize"]) {
        int size = [json[@"adSize"] intValue];
        if (size == 0) {
            config.adSize = Yodo1MasBannerAdSizeBanner;
        } else if (size == 1) {
            config.adSize = Yodo1MasBannerAdSizeLargeBanner;
        } else if (size == 2) {
            config.adSize = Yodo1MasBannerAdSizeIABMediumRectangle;
        } else if (size == 3) {
            config.adSize = Yodo1MasBannerAdSizeSmartBanner;
        } else if (size == 4) {
            config.adSize = Yodo1MasBannerAdSizeAdaptiveBanner;
        }
    }
    config.adPosition = Yodo1MasAdBannerAlignBottom | Yodo1MasAdBannerAlignHorizontalCenter;
    if (json[@"adPosition"]) {
        config.adPosition = [json[@"adPosition"] integerValue];
    }
    
    int customAdPositionX = 0;
    if (json[@"customAdPositionX"]) {
        customAdPositionX = [json[@"customAdPositionX"] intValue];
    }

    int customAdPositionY = 0;
    if (json[@"customAdPositionY"]) {
        customAdPositionY = [json[@"customAdPositionY"] intValue];
    }
    config.customPosition = CGPointMake(customAdPositionX/UIScreen.mainScreen.scale, customAdPositionY/UIScreen.mainScreen.scale);
    
    if (json[@"adPlacement"]) {
        config.adPlacement = json[@"adPlacement"];
    } else {
        config.adPlacement = @"";
    }
    
    if (json[@"indexId"]) {
        config.indexId = json[@"indexId"];
    } else {
        config.indexId = @"";
    }
    
    if (json[@"backgroundColor"]) {
        config.backgroundColor = json[@"backgroundColor"];
    } else {
        config.backgroundColor = nil;
    }
    
    return config;
}

@end

@implementation Yodo1MasBannerAdView(Bridge)

- (Yodo1MasBridgeBannerAdConfig *)yodo1_config {
    return objc_getAssociatedObject(self, _cmd);
}

- (void)setYodo1_config:(Yodo1MasBridgeBannerAdConfig *)yodo1_config {
    objc_setAssociatedObject(self, @selector(yodo1_config), yodo1_config, OBJC_ASSOCIATION_RETAIN_NONATOMIC);
}


@end
