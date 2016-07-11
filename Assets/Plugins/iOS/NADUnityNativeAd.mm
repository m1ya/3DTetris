//
//  NADUnityNativeAd.m
//  Unity-iPhone
//

#import "NADUnityNativeAd.h"

#import "NADNativeClient.h"
#import "NADUnityGlobal.h"

static NSMutableDictionary *objectCache;

typedef NS_ENUM(NSInteger, NADUnityAdvertisingExplicitly) {
    NADUnityAdvertisingExplicitlyPR = 0,
    NADUnityAdvertisingExplicitlySponsored = 1,
    NADUnityAdvertisingExplicitlyAD = 2,
    NADUnityAdvertisingExplicitlyPromotion = 3,
};

static void NADUnityCacheObject(NSObject *object)
{
    if (!objectCache) {
        objectCache = [NSMutableDictionary new];
    }
    objectCache[@(object.hash)] = object;
}

static void NADUnityRemoveObject(NSObject *object)
{
    if (objectCache) {
        [objectCache removeObjectForKey:@(object.hash)];
    }
}

//==============================================================================

@interface NendUnityNativeAd : NSObject

@property (nonatomic, strong) NADNative *nativeAd;
@property (nonatomic, readonly, copy) NSString *shortText;
@property (nonatomic, readonly, copy) NSString *longText;
@property (nonatomic, readonly, copy) NSString *promotionName;
@property (nonatomic, readonly, copy) NSString *promotionUrl;
@property (nonatomic, readonly, copy) NSString *actionButtonText;
@property (nonatomic, readonly, copy) NSString *adImageUrl;
@property (nonatomic, readonly) NSInteger adImageWidth;
@property (nonatomic, readonly) NSInteger adImageHeight;
@property (nonatomic, readonly, copy) NSString *logoImageUrl;
@property (nonatomic, readonly) NSInteger logoImageWidth;
@property (nonatomic, readonly) NSInteger logoImageHeight;

- (instancetype)initWithNativeAd:(NADNative *)nativeAd;
- (NSString *)prTextWithUnityAdvertisingExplicitly:(NSInteger)unityAdvertisingExplicitly;
- (void)performAdClick;
- (void)performInformationClick;
- (void)sendImpression;

@end

//==============================================================================

@implementation NendUnityNativeAd

- (instancetype)initWithNativeAd:(NADNative *)nativeAd
{
    self = [super init];
    if (self) {
        _nativeAd = nativeAd;
    }
    return self;
}

- (NSString *)shortText
{
    return [self.nativeAd valueForKey:@"shortText"];
}

- (NSString *)longText
{
    return [self.nativeAd valueForKey:@"longText"];
}

- (NSString *)promotionName
{
    return [self.nativeAd valueForKey:@"promotionName"];
}

- (NSString *)promotionUrl
{
    return [self.nativeAd valueForKey:@"promotionUrl"];
}

- (NSString *)actionButtonText
{
    return [self.nativeAd valueForKey:@"actionButtonText"];
}

- (NSString *)adImageUrl
{
    return [self.nativeAd valueForKey:@"imageUrl"];
}

- (NSInteger)adImageWidth
{
    NSValue *value = [self.nativeAd valueForKey:@"imageSize"];
    return value.CGSizeValue.width;
}

- (NSInteger)adImageHeight
{
    NSValue *value = [self.nativeAd valueForKey:@"imageSize"];
    return value.CGSizeValue.height;
}

- (NSString *)logoImageUrl
{
    return [self.nativeAd valueForKey:@"logoUrl"];
}

- (NSInteger)logoImageWidth
{
    NSValue *value = [self.nativeAd valueForKey:@"logoSize"];
    return value.CGSizeValue.width;
}

- (NSInteger)logoImageHeight
{
    NSValue *value = [self.nativeAd valueForKey:@"logoSize"];
    return value.CGSizeValue.height;
}

- (NSString *)prTextWithUnityAdvertisingExplicitly:(NSInteger)unityAdvertisingExplicitly
{
    NADNativeAdvertisingExplicitly explicitly;
    switch (unityAdvertisingExplicitly) {
        case NADUnityAdvertisingExplicitlyPR:
            explicitly = NADNativeAdvertisingExplicitlyPR;
            break;
        case NADUnityAdvertisingExplicitlySponsored:
            explicitly = NADNativeAdvertisingExplicitlySponsored;
            break;
        case NADUnityAdvertisingExplicitlyAD:
            explicitly = NADNativeAdvertisingExplicitlyAD;
            break;
        case NADUnityAdvertisingExplicitlyPromotion:
            explicitly = NADNativeAdvertisingExplicitlyPromotion;
            break;
        default:
            explicitly = NADNativeAdvertisingExplicitlyPR;
            break;
    }
    [self.nativeAd setValue:@(explicitly) forKey:@"advertisingExplicitly"];
    return [self.nativeAd valueForKey:@"prText"];
}

- (void)performAdClick
{
    SEL selector = NSSelectorFromString(@"performAdClick");
    [self performInternalSelector:selector];
}

- (void)performInformationClick
{
    SEL selector = NSSelectorFromString(@"performInformationClick");
    [self performInternalSelector:selector];
}

- (void)sendImpression
{
    SEL selector = NSSelectorFromString(@"sendImpression");
    [self performInternalSelector:selector];
}

- (void)performInternalSelector:(SEL)selector
{
    if ([self.nativeAd respondsToSelector:selector]) {
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Warc-performSelector-leaks"
        [self.nativeAd performSelector:selector];
#pragma clang diagnostic pop
    } else {
        NSLog(@"Not Found: %@", NSStringFromSelector(selector));
    }
}

@end

///-----------------------------------------------
/// @name C Interfaces
///-----------------------------------------------

NADNativeClientPtr _CreateNativeAdClient(const char* apiKey, const char* spotId)
{
    NADNativeClient *client = [[NADNativeClient alloc] initWithSpotId:NADUnityCreateNSString(spotId)
                                                               apiKey:NADUnityCreateNSString(apiKey)
                                                advertisingExplicitly:NADNativeAdvertisingExplicitlyAD];
    NADUnityCacheObject(client);
    return (__bridge NADNativeClientPtr)client;
}

void _ReleaseNativeAdClient(NADNativeClientPtr nativeAdClient)
{
    NADNativeClient *client = (__bridge NADNativeClient *)nativeAdClient;
    NADUnityRemoveObject(client);
}

void _LoadNativeAd(NendUnityNativeAdClientPtr unityNativeAdClient, NADNativeClientPtr nativeAdClient, NendUnityNativeAdCallback callback)
{
    NADNativeClient *client = (__bridge NADNativeClient *)nativeAdClient;
    [client loadWithCompletionBlock:^(NADNative *ad, NSError *error) {
        if (ad) {
            NendUnityNativeAd *nativeAd = [[NendUnityNativeAd alloc] initWithNativeAd:ad];
            NADUnityCacheObject(nativeAd);
            callback(unityNativeAdClient, (__bridge NADNativeClientPtr)nativeAd, 200, "");
        } else {
            callback(unityNativeAdClient, NULL, (int)error.code, NADUnityMakeStringCopy(error.localizedDescription));
        }
    }];
}

const char* _GetShortText(NendUnityNativeAdPtr nativeAd)
{
    NendUnityNativeAd *ad = (__bridge NendUnityNativeAd *)nativeAd;
    return NADUnityMakeStringCopy(ad.shortText);
}

const char* _GetLongText(NendUnityNativeAdPtr nativeAd)
{
    NendUnityNativeAd *ad = (__bridge NendUnityNativeAd *)nativeAd;
    return NADUnityMakeStringCopy(ad.longText);
}

const char* _GetPromotionName(NendUnityNativeAdPtr nativeAd)
{
    NendUnityNativeAd *ad = (__bridge NendUnityNativeAd *)nativeAd;
    return NADUnityMakeStringCopy(ad.promotionName);
}

const char* _GetPromotionUrl(NendUnityNativeAdPtr nativeAd)
{
    NendUnityNativeAd *ad = (__bridge NendUnityNativeAd *)nativeAd;
    return NADUnityMakeStringCopy(ad.promotionUrl);
}

const char* _GetActionButtonText(NendUnityNativeAdPtr nativeAd)
{
    NendUnityNativeAd *ad = (__bridge NendUnityNativeAd *)nativeAd;
    return NADUnityMakeStringCopy(ad.actionButtonText);
}

const char* _GetAdImageUrl(NendUnityNativeAdPtr nativeAd)
{
    NendUnityNativeAd *ad = (__bridge NendUnityNativeAd *)nativeAd;
    return NADUnityMakeStringCopy(ad.adImageUrl);
}

const char* _GetLogoImageUrl(NendUnityNativeAdPtr nativeAd)
{
    NendUnityNativeAd *ad = (__bridge NendUnityNativeAd *)nativeAd;
    return NADUnityMakeStringCopy(ad.logoImageUrl);
}

const char* _GetAdvertisingExplicitlyText(NendUnityNativeAdPtr nativeAd, int unityAdvertisingExplicitly)
{
    NendUnityNativeAd *ad = (__bridge NendUnityNativeAd *)nativeAd;
    return NADUnityMakeStringCopy([ad prTextWithUnityAdvertisingExplicitly:unityAdvertisingExplicitly]);
}

int _GetAdImageWidth(NendUnityNativeAdPtr nativeAd)
{
    NendUnityNativeAd *ad = (__bridge NendUnityNativeAd *)nativeAd;
    return (int)ad.adImageWidth;
}

int _GetAdImageHeight(NendUnityNativeAdPtr nativeAd)
{
    NendUnityNativeAd *ad = (__bridge NendUnityNativeAd *)nativeAd;
    return (int)ad.adImageHeight;
}

int _GetLogoImageWidth(NendUnityNativeAdPtr nativeAd)
{
    NendUnityNativeAd *ad = (__bridge NendUnityNativeAd *)nativeAd;
    return (int)ad.logoImageWidth;
}

int _GetLogoImageHeight(NendUnityNativeAdPtr nativeAd)
{
    NendUnityNativeAd *ad = (__bridge NendUnityNativeAd *)nativeAd;
    return (int)ad.logoImageHeight;
}

void _PerformAdClick(NendUnityNativeAdPtr nativeAd)
{
    NendUnityNativeAd *ad = (__bridge NendUnityNativeAd *)nativeAd;
    [ad performAdClick];
}

void _PerformInformationClick(NendUnityNativeAdPtr nativeAd)
{
    NendUnityNativeAd *ad = (__bridge NendUnityNativeAd *)nativeAd;
    [ad performInformationClick];
}

void _SendImpression(NendUnityNativeAdPtr nativeAd)
{
    NendUnityNativeAd *ad = (__bridge NendUnityNativeAd *)nativeAd;
    [ad sendImpression];
}

void _ReleaseNativeAd(NendUnityNativeAdPtr nativeAd)
{
    NendUnityNativeAd *ad = (__bridge NendUnityNativeAd *)nativeAd;
    NADUnityRemoveObject(ad);
}
