//
//  Yodo1MasNativeAdView+Bridge.h
//  UnityFramework
//
//  Created by 周玉震 on 2021/11/28.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#if __has_include(<Yodo1MasCore/Yodo1MasNativeAdView.h>)
#import <Yodo1MasCore/Yodo1MasNativeAdView.h>
#else
#import "Yodo1MasNativeAdView.h"
#endif

NS_ASSUME_NONNULL_BEGIN

typedef NS_ENUM(NSInteger, Yodo1MasAdNativeAlign) {
    Yodo1MasAdNativeAlignLeft = 1,
    Yodo1MasAdNativeAlignHorizontalCenter = 1 << 1,
    Yodo1MasAdNativeAlignRight = 1 << 2,
    Yodo1MasAdNativeAlignTop = 1 << 3,
    Yodo1MasAdNativeAlignVerticalCenter = 1 << 4,
    Yodo1MasAdNativeAlignBottom = 1 << 5
};

@interface Yodo1MasBridgeNativeAdConfig : NSObject

+ (Yodo1MasBridgeNativeAdConfig *)parse:(id)json;

@property (nonatomic, assign) Yodo1MasAdNativeAlign position;
@property (nonatomic, assign) CGFloat offsetX;
@property (nonatomic, assign) CGFloat offsetY;
@property (nonatomic, assign) CGFloat x;
@property (nonatomic, assign) CGFloat y;
@property (nonatomic, assign) CGFloat width;
@property (nonatomic, assign) CGFloat height;
@property (nonatomic, copy) NSString *adPlacement;
@property (nonatomic, copy) NSString *customData;
@property (nonatomic, copy) NSString *indexId;
@property (nonatomic, copy) NSString *backgroundColor;

@property (nonatomic, assign) int payRevenueEventCount;

@end

@interface Yodo1MasNativeAdView(Bridge)

@property (nonatomic, strong) Yodo1MasBridgeNativeAdConfig *yodo1_config;

@end

NS_ASSUME_NONNULL_END
