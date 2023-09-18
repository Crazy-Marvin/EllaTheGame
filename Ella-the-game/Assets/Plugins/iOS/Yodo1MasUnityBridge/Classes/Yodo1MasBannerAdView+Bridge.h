//
//  Yodo1MasBannerAdView+Bridge.h
//  UnityFramework
//
//  Created by 周玉震 on 2021/11/28.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "Yodo1MasBannerAdView.h"

NS_ASSUME_NONNULL_BEGIN

@interface Yodo1MasBridgeBannerAdConfig : NSObject

+ (Yodo1MasBridgeBannerAdConfig *)parse:(id)json;

@property (nonatomic, assign) Yodo1MasBannerAdSize adSize;
@property (nonatomic, assign) Yodo1MasAdBannerAlign adPosition;
@property (nonatomic, assign) CGPoint customPosition;
@property (nonatomic, copy) NSString *adPlacement;
@property (nonatomic, copy) NSString *indexId;
@property (nonatomic, copy) NSString *backgroundColor;

@end

@interface Yodo1MasBannerAdView(Bridge)

@property (nonatomic, strong) Yodo1MasBridgeBannerAdConfig *yodo1_config;

@end

NS_ASSUME_NONNULL_END
