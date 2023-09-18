//
//  Yodo1MasAppOpenAd+Bridge.h
//  UnityFramework
//
//  Created by 周玉震 on 2021/11/28.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "Yodo1MasAppOpenAd.h"

NS_ASSUME_NONNULL_BEGIN

@interface Yodo1MasBridgeAppOpenAdConfig : NSObject

+ (Yodo1MasBridgeAppOpenAdConfig *)parse:(id)json;

@property (nonatomic, copy) NSString *adPlacement;
@property (nonatomic, assign) BOOL autoDelayIfLoadFail;

@end

@interface Yodo1MasAppOpenAd(Bridge)

@property (nonatomic, strong) Yodo1MasBridgeAppOpenAdConfig *yodo1_config;

@end

NS_ASSUME_NONNULL_END
