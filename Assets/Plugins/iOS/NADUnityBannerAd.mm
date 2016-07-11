//
//  NADUnityBannerAd.m
//  Unity-iPhone
//

#import "NADUnityBannerAd.h"

#import "NADView.h"
#import "NADUnityGlobal.h"

extern UIView* UnityGetGLView();

static NSMutableDictionary* _bannerAdDict = [NSMutableDictionary new];

//==============================================================================

@interface NADViewEventDispatcher : NSObject <NADViewDelegate>

@property (nonatomic, copy) NSString* gameObject;
@property (nonatomic, copy) dispatch_block_t loadCompletionHandler;

- (instancetype)initWithGameObject:(NSString*)gameObject;

@end

//==============================================================================

@interface BannerParams : NSObject

@property (nonatomic, copy) NSString* gameObject;
@property (nonatomic, copy) NSString* apiKey;
@property (nonatomic, copy) NSString* spotID;
@property (nonatomic) BOOL outputLog;
@property (nonatomic) BOOL adjustSize;
@property (nonatomic) NSInteger size;
@property (nonatomic) NSInteger gravity;
@property (nonatomic) NSInteger leftMargin;
@property (nonatomic) NSInteger topMargin;
@property (nonatomic) NSInteger rightMargin;
@property (nonatomic) NSInteger bottomMargin;
@property (nonatomic, strong) UIColor *backgroundColor;

+ (instancetype)paramWithString:(NSString*)paramString;
- (void)updateLayoutWithString:(NSString*)paramString;

@end

//==============================================================================

@interface NendRotationHandler : NSObject

@property (nonatomic, copy) dispatch_block_t block;

- (void)startHandlingUsingBlock:(dispatch_block_t)block;
- (void)stopHandling;

@end

//==============================================================================

@interface NendAdBanner : NSObject

@property (nonatomic, strong) NADView* bannerView;
@property (nonatomic, strong) NADViewEventDispatcher *dispatcher;
@property (nonatomic, strong) BannerParams* params;
@property (nonatomic, strong) NendRotationHandler* rotationHandler;

+ (instancetype)bannerAdWithParams:(BannerParams*)params;

- (void)load;
- (void)show;
- (void)hide;
- (void)resume;
- (void)pause;
- (void)updateLayoutWithString:(NSString*)paramString;

@end

//==============================================================================

typedef NS_ENUM(NSInteger, NendUnityGravity) {
    NendUnityGravityLeft = 1,
    NendUnityGravityTop = 2,
    NendUnityGravityRight = 4,
    NendUnityGravityBottom = 8,
    NendUnityGravityCenterVertical = 16,
    NendUnityGravityCenterHorizontal = 32,
};

typedef NS_ENUM(NSInteger, NendUnityBannerSize) {
    NendUnityBannerSize320X50 = 0,
    NendUnityBannerSize320X100 = 1,
    NendUnityBannerSize300X100 = 2,
    NendUnityBannerSize300X250 = 3,
    NendUnityBannerSize728X90 = 4,
};

static CGSize BannerSize(NendUnityBannerSize size)
{
    switch (size) {
        case NendUnityBannerSize320X50:
            return CGSizeMake(320, 50);
        case NendUnityBannerSize320X100:
            return CGSizeMake(320, 100);
        case NendUnityBannerSize300X100:
            return CGSizeMake(300, 100);
        case NendUnityBannerSize300X250:
            return CGSizeMake(300, 250);
        case NendUnityBannerSize728X90:
            return CGSizeMake(728, 90);
        default:
            return CGSizeZero;
    }
}

static CGPoint CalculatePoint(NSInteger gravity, CGSize viewSize, NSInteger left, NSInteger top, NSInteger right, NSInteger bottom)
{
    CGPoint point = CGPointZero;
    CGSize screenSize = UnityGetGLView().bounds.size;
    
    if (0 != (gravity & NendUnityGravityCenterHorizontal)) {
        point.x = (screenSize.width - viewSize.width) / 2;
    }
    if (0 != (gravity & NendUnityGravityRight)) {
        point.x = screenSize.width - viewSize.width;
    }
    if (0 != (gravity & NendUnityGravityLeft)) {
        point.x = 0.0f;
    }
    
    if (0 != (gravity & NendUnityGravityCenterVertical)) {
        point.y = (screenSize.height - viewSize.height) / 2;
    }
    if (0 != (gravity & NendUnityGravityBottom)) {
        point.y = screenSize.height - viewSize.height;
    }
    if (0 != (gravity & NendUnityGravityTop)) {
        point.y = 0.0f;
    }
    
    point.x += left;
    point.y += top;
    point.x -= right;
    point.y -= bottom;
    
    return point;
}

//==============================================================================

@implementation NADViewEventDispatcher

- (instancetype)initWithGameObject:(NSString*)gameObject
{
    self = [super init];
    if (self) {
        _gameObject = [gameObject copy];
        _loadCompletionHandler = NULL;
    }
    return self;
}

- (void)dealloc
{
    _loadCompletionHandler = NULL;
}

- (void)nadViewDidFinishLoad:(NADView*)adView
{
    if (self.loadCompletionHandler) {
        self.loadCompletionHandler();
        self.loadCompletionHandler = NULL;
    }
    UnitySendMessage([self.gameObject UTF8String], "NendAdView_OnFinishLoad", "");
}

- (void)nadViewDidFailToReceiveAd:(NADView*)adView
{
    NSString* message = [NSString stringWithFormat:@"%ld:%@", (long)adView.error.code, adView.error.domain];
    UnitySendMessage([self.gameObject UTF8String], "NendAdView_OnFailToReceiveAd", (char *)[message UTF8String]);
}

- (void)nadViewDidReceiveAd:(NADView*)adView
{
    UnitySendMessage([self.gameObject UTF8String], "NendAdView_OnReceiveAd", "");
}

- (void)nadViewDidClickAd:(NADView*)adView
{
    UnitySendMessage([self.gameObject UTF8String], "NendAdView_OnClickAd", "");
}

- (void)nadViewDidClickInformation:(NADView *)adView
{
    UnitySendMessage([self.gameObject UTF8String], "NendAdView_OnClickInformation", "");
}

@end

//==============================================================================

@implementation BannerParams

+ (instancetype)paramWithString:(NSString*)paramString
{
    return [[BannerParams alloc] initWithParamString:paramString];
}

- (instancetype)initWithParamString:(NSString*)paramString
{
    self = [super init];
    if (self) {
        NSArray* paramArray = [paramString componentsSeparatedByString:@":"];
        int index = 0;
        _gameObject = [(NSString*)paramArray[index++] copy];
        _apiKey = [(NSString*)paramArray[index++] copy];
        _spotID = [(NSString*)paramArray[index++] copy];
        _outputLog = [@"true" isEqualToString:(NSString*)paramArray[index++]];
        _adjustSize = [@"true" isEqualToString:(NSString*)paramArray[index++]];
        _size = [paramArray[index++] integerValue];
        _gravity = [paramArray[index++] integerValue];
        _leftMargin = [paramArray[index++] integerValue];
        _topMargin = [paramArray[index++] integerValue];
        _rightMargin = [paramArray[index++] integerValue];
        _bottomMargin = [paramArray[index++] integerValue];
        CGFloat r = [paramArray[index++] floatValue];
        CGFloat g = [paramArray[index++] floatValue];
        CGFloat b = [paramArray[index++] floatValue];
        CGFloat a = [paramArray[index++] floatValue];
        _backgroundColor = [[UIColor alloc] initWithRed:r green:g blue:b alpha:a];
        
    }
    return self;
}

- (void)updateLayoutWithString:(NSString*)paramString
{
    NSArray* paramArray = [paramString componentsSeparatedByString:@":"];
    int index = 0;
    self.gravity = [paramArray[index++] integerValue];
    self.leftMargin = [paramArray[index++] integerValue];
    self.topMargin = [paramArray[index++] integerValue];
    self.rightMargin = [paramArray[index++] integerValue];
    self.bottomMargin = [paramArray[index++] integerValue];
}

@end

//==============================================================================

@implementation NendAdBanner

+ (instancetype)bannerAdWithParams:(BannerParams*)params
{
    return [[NendAdBanner alloc] initWithParams:params];
}

- (instancetype)initWithParams:(BannerParams*)params;
{
    self = [super init];
    if (self) {
        _params = params;
        _rotationHandler = [NendRotationHandler new];
        
        _bannerView = [[NADView alloc] initWithIsAdjustAdSize:params.adjustSize];
        _bannerView.backgroundColor = params.backgroundColor;
        _bannerView.hidden = YES;
        
        _dispatcher = [[NADViewEventDispatcher alloc] initWithGameObject:params.gameObject];
        __weak NendAdBanner* weakSelf = self;
        _dispatcher.loadCompletionHandler = ^{
            if (weakSelf) {
                [weakSelf layout];
            }
        };
        
        _bannerView.delegate = _dispatcher;
        _bannerView.isOutputLog = params.outputLog;
        [_bannerView setNendID:params.apiKey spotID:params.spotID];
    }
    
    return self;
}

- (void)dealloc
{
    [_bannerView removeFromSuperview];
    _bannerView.delegate = nil;
    _bannerView = nil;
    _params = nil;
}

- (void)load
{
    if (self.bannerView) {
        [self.bannerView load];
    }
}

- (void)show
{
    if (self.bannerView && self.bannerView.hidden) {
        __weak NendAdBanner* weakSelf = self;
        [self.rotationHandler startHandlingUsingBlock:^{
            if (weakSelf) {
                [weakSelf didRotate];
            }
        }];
        [self layout];
        self.bannerView.hidden = NO;
    }
}

- (void)hide
{
    if (self.bannerView && !self.bannerView.hidden) {
        [self.rotationHandler stopHandling];
        self.bannerView.hidden = YES;
    }
}

- (void)resume
{
    if (self.bannerView) {
        [self.bannerView resume];
    }
}

- (void)pause
{
    if (self.bannerView) {
        [self.bannerView pause];
    }
}

- (void)layout
{
    if (!self.bannerView) {
        return;
    }
    
    CGSize bannerSize;
    if (self.params.adjustSize) {
        bannerSize = self.bannerView.frame.size;
        if (0.f == bannerSize.width || 0.f == bannerSize.height) {
            return;
        }
    } else {
        bannerSize = BannerSize((NendUnityBannerSize)self.params.size);
    }
    
    CGPoint point = CalculatePoint(self.params.gravity, bannerSize, self.params.leftMargin, self.params.topMargin, self.params.rightMargin, self.params.bottomMargin);
    self.bannerView.frame = CGRectMake(point.x, point.y, bannerSize.width, bannerSize.height);
}

- (void)updateLayoutWithString:(NSString*)paramString
{
    [self.params updateLayoutWithString:paramString];
    [self layout];
}

- (void)didRotate
{
    if (self.bannerView && !self.bannerView.hidden) {
        [self layout];
    }
}

@end

//==============================================================================

@implementation NendRotationHandler

- (instancetype)init
{
    self = [super init];
    if (self) {
        [[NSNotificationCenter defaultCenter] addObserver:self
                                                 selector:@selector(didRotate:)
                                                     name:UIDeviceOrientationDidChangeNotification
                                                   object:nil];
        
    }
    return self;
}

- (void)startHandlingUsingBlock:(dispatch_block_t)block
{
    self.block = block;
}

- (void)stopHandling
{
    self.block = NULL;
}

- (void)dealloc
{
    [[NSNotificationCenter defaultCenter] removeObserver:self name:UIDeviceOrientationDidChangeNotification object:nil];
    _block = NULL;
}

- (void)didRotate:(NSNotification *)note
{
    if (self.block) {
        dispatch_async(dispatch_get_main_queue(), ^{
            self.block();
        });
    }
}

@end

///-----------------------------------------------
/// @name C Interfaces
///-----------------------------------------------

void _TryCreateBanner(const char* paramString)
{
    BOOL didLoaded = NO;
    BannerParams* params = [BannerParams paramWithString:NADUnityCreateNSString(paramString)];
    NendAdBanner* ad = _bannerAdDict[params.gameObject];
    
    if (!ad) {
        ad = [NendAdBanner bannerAdWithParams:params];
        _bannerAdDict[params.gameObject] = ad;
    } else {
        didLoaded = YES;
    }
    
    if (!ad.bannerView.superview) {
        [UnityGetGLView() addSubview:ad.bannerView];
    }
    
    if (!didLoaded) {
        [ad.bannerView load];
    }
}

void _ShowBanner(const char* gameObject)
{
    NendAdBanner* ad = _bannerAdDict[NADUnityCreateNSString(gameObject)];
    if (ad) {
        [ad show];
    }
}

void _HideBanner(const char* gameObject)
{
    NendAdBanner* ad = _bannerAdDict[NADUnityCreateNSString(gameObject)];
    if (ad) {
        [ad hide];
    }
}

void _ResumeBanner(const char* gameObject)
{
    NendAdBanner* ad = _bannerAdDict[NADUnityCreateNSString(gameObject)];
    if (ad) {
        [ad resume];
    }
}

void _PauseBanner(const char* gameObject)
{
    NendAdBanner* ad = _bannerAdDict[NADUnityCreateNSString(gameObject)];
    if (ad) {
        [ad pause];
    }
}

void _DestroyBanner(const char* gameObject)
{
    [_bannerAdDict removeObjectForKey:NADUnityCreateNSString(gameObject)];
}

void _LayoutBanner(const char* gameObject, const char* paramString)
{
    NendAdBanner* ad = _bannerAdDict[NADUnityCreateNSString(gameObject)];
    if (ad) {
        [ad updateLayoutWithString:NADUnityCreateNSString(paramString)];
    }
}
