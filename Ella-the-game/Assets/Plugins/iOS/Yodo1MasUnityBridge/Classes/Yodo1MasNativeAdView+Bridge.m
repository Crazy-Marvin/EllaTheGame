//
//  Yodo1MasBannerAdView+Bridge.m
//  UnityFramework
//
//  Created by 周玉震 on 2021/11/28.
//

#import "Yodo1MasNativeAdView+Bridge.h"
#import <objc/runtime.h>

@implementation Yodo1MasBridgeNativeAdConfig

+ (Yodo1MasBridgeNativeAdConfig *)parse:(id)json {
    Yodo1MasBridgeNativeAdConfig *config = [[Yodo1MasBridgeNativeAdConfig alloc] init];
    
    if (json[@"position"]) {
        config.position = [json[@"position"] intValue];
    }
    if (json[@"offsetX"]) {
        config.offsetX = [json[@"offsetX"] floatValue];
    }
    if (json[@"offsetY"]) {
        config.offsetY = [json[@"offsetY"] floatValue];
    }
    if (json[@"x"]) {
        config.x = [json[@"x"] floatValue];
    }
    if (json[@"y"]) {
        config.y = [json[@"y"] floatValue];
    }
    if (json[@"width"]) {
        config.width = [json[@"width"] floatValue];
    }
    if (json[@"height"]) {
        config.height = [json[@"height"] floatValue];
    }
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

@implementation Yodo1MasNativeAdView(Bridge)

- (Yodo1MasBridgeNativeAdConfig *)yodo1_config {
    return objc_getAssociatedObject(self, _cmd);
}

- (void)setYodo1_config:(Yodo1MasBridgeNativeAdConfig *)yodo1_config {
    objc_setAssociatedObject(self, @selector(yodo1_config), yodo1_config, OBJC_ASSOCIATION_RETAIN_NONATOMIC);
}


@end
