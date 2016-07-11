namespace NendUnityPlugin.Platform
{
	using Utils = NendUnityPlugin.Common.NendUtils;

	internal class NendAdNativeInterfaceFactory
	{
		internal static NendAdBannerInterface CreateBannerAdInterface ()
		{
			#if UNITY_IPHONE && !UNITY_EDITOR
			return new NendUnityPlugin.Platform.iOS.IOSBannerInterface();
			#elif UNITY_ANDROID && !UNITY_EDITOR
			return new NendUnityPlugin.Platform.Android.AndroidBannerInterface();
			#else
			return new BannerStub ();
			#endif
		}
			
		#if UNITY_ANDROID
		internal static NendAdIconInterface CreateIconAdInterface ()
		{		    
            #if !UNITY_EDITOR
			return new NendUnityPlugin.Platform.Android.AndroidIconInterface();
     	    #else
			return new IconStub ();
		    #endif
		}
		#endif

		internal static NendAdInterstitialInterface CreateInterstitialAdInterface ()
		{
			#if UNITY_IPHONE && !UNITY_EDITOR
			return new NendUnityPlugin.Platform.iOS.IOSInterstitialInterface();
			#elif UNITY_ANDROID && !UNITY_EDITOR
			return new NendUnityPlugin.Platform.Android.AndroidInterstitialInterface();
			#else
			return new InterstitialStub ();
			#endif
		}

		#if UNITY_EDITOR
		internal static INativeAdClient CreateNativeAdClient (Utils.NativeAdType type)
		#else
		internal static INativeAdClient CreateNativeAdClient (Utils.Account account)
		#endif
		{
			#if UNITY_IPHONE && !UNITY_EDITOR
			string apiKey = account.iOS.apiKey;
			string spotId = account.iOS.spotID.ToString();
			return new NendUnityPlugin.Platform.iOS.IOSNativeAdClient(apiKey, spotId);
			#elif UNITY_ANDROID && !UNITY_EDITOR
			string apiKey = account.android.apiKey;
			string spotId = account.android.spotID.ToString ();
			return new NendUnityPlugin.Platform.Android.AndroidNativeAdClient(apiKey, spotId);
			#else
			return new NativeClientStub (type);
			#endif
		}

		private class BannerStub : NendAdBannerInterface
		{
			public void TryCreateBanner (string paramString)
			{
				UnityEngine.Debug.Log ("TryCreateBanner: " + paramString);
			}

			public void ShowBanner (string gameObject)
			{
				UnityEngine.Debug.Log ("ShowBanner: " + gameObject);
			}

			public void HideBanner (string gameObject)
			{
				UnityEngine.Debug.Log ("HideBanner: " + gameObject);
			}

			public void ResumeBanner (string gameObject)
			{
				UnityEngine.Debug.Log ("ResumeBanner: " + gameObject);
			}

			public void PauseBanner (string gameObject)
			{
				UnityEngine.Debug.Log ("PauseBanner: " + gameObject);
			}

			public void DestroyBanner (string gameObject)
			{
				UnityEngine.Debug.Log ("DestroyBanner: " + gameObject);
			}

			public void LayoutBanner (string gameObject, string paramString)
			{
				UnityEngine.Debug.Log ("LayoutBanner: " + gameObject + ", " + paramString);
			}
		}

		#if UNITY_ANDROID
		private class IconStub : NendAdIconInterface
		{
			public void TryCreateIcons (string paramString)
			{
				UnityEngine.Debug.Log ("TryCreateIcons: " + paramString);
			}

			public void ShowIcons (string gameObject)
			{
				UnityEngine.Debug.Log ("ShowIcons: " + gameObject);
			}

			public void HideIcons (string gameObject)
			{
				UnityEngine.Debug.Log ("HideIcons: " + gameObject);
			}

			public void ResumeIcons (string gameObject)
			{
				UnityEngine.Debug.Log ("ResumeIcons: " + gameObject);
			}

			public void PauseIcons (string gameObject)
			{
				UnityEngine.Debug.Log ("PauseIcons: " + gameObject);
			}

			public void DestroyIcons (string gameObject)
			{
				UnityEngine.Debug.Log ("DestroyIcons: " + gameObject);
			}

			public void LayoutIcons (string gameObject, string paramString)
			{
				UnityEngine.Debug.Log ("LayoutIcons: " + gameObject + ", " + paramString);
			}
		}
		#endif

		private class InterstitialStub : NendAdInterstitialInterface
		{
			public void LoadInterstitialAd (string apiKey, string spotId, bool isOutputLog)
			{
				UnityEngine.Debug.Log ("LoadInterstitialAd: " + apiKey + ", " + spotId + ", " + isOutputLog);
			}

			public void ShowInterstitialAd (string spotId)
			{
				UnityEngine.Debug.Log ("ShowInterstitialAd: " + spotId);
			}

			public void DismissInterstitialAd ()
			{
				UnityEngine.Debug.Log ("DismissInterstitialAd");
			}
		}

		#if UNITY_EDITOR
		private class NativeClientStub : INativeAdClient, System.IDisposable
		{
			private Utils.NativeAdType m_AdType;

			internal NativeClientStub (Utils.NativeAdType adType)
			{
				m_AdType = adType;
			}

			~NativeClientStub ()
			{
				Dispose ();
			}

			public void Dispose ()
			{
				UnityEngine.Debug.Log ("Dispose native ad client.");
			}

			public void LoadNativeAd (System.Action<INativeAd, int, string> callback)
			{
				UnityEngine.Debug.Log ("LoadNativeAd");
				callback (new DummyNativeAd (m_AdType), 200, "");
			}
		}

		private class DummyNativeAd : INativeAd, System.IDisposable
		{
			private Utils.NativeAdType m_AdType;

			internal DummyNativeAd (Utils.NativeAdType adType)
			{
				m_AdType = adType;
			}

			~DummyNativeAd ()
			{
				Dispose ();
			}

			public void Dispose ()
			{
				UnityEngine.Debug.Log ("Dispose native ad object.");
			}

			public void SendImpression ()
			{
				UnityEngine.Debug.Log ("SendImpression");
			}

			public void PerformAdClick ()
			{
				UnityEngine.Debug.Log ("PerformAdClick");
			}

			public void PerformInformationClick ()
			{
				UnityEngine.Debug.Log ("PerformInformationClick");
			}

			public string GetAdvertisingExplicitlyText (int advertisingExplicitly)
			{
				switch (advertisingExplicitly) {
				case 0:
					return "PR";
				case 1:
					return "Sponsored";
				case 2:
					return "広告";
				case 3:
					return "プロモーション";
				default:
					return "";
				}
			}

			public string ShortText {
				get {
					return "国内・海外の旅行予約はnendトラベル";
				}
			}

			public string LongText {
				get {
					return "国内旅行・海外旅行のツアーやホテル予約が簡単。日程や条件から細かく検索できます";
				}
			}

			public string PromotionUrl {
				get {
					return "nend.net";
				}
			}

			public string PromotionName {
				get {
					return "nendトラベル";
				}
			}

			public string ActionButtonText {
				get {
					return "サイトへ行く";
				}
			}

			public string AdImageUrl {
				get {
					switch (m_AdType) {
					case Utils.NativeAdType.SmallSquare:
						return "https://img1.nend.net/img/banner/329/71105/1307068.png";
					case Utils.NativeAdType.SmallWide:
						return "https://img1.nend.net/img/banner/329/70572/1292196.png";
					case Utils.NativeAdType.LargeWide:
						return "https://img1.nend.net/img/banner/329/71103/1307041.png";
					default:
						return null;
					}
				}
			}

			public int AdImageWidth {
				get {
					switch (m_AdType) {
					case Utils.NativeAdType.SmallSquare:
						return 80;
					case Utils.NativeAdType.SmallWide:
						return 80;
					case Utils.NativeAdType.LargeWide:
						return 300;
					default:
						return 0;
					}
				}
			}

			public int AdImageHeight {
				get {
					switch (m_AdType) {
					case Utils.NativeAdType.SmallSquare:
						return 80;
					case Utils.NativeAdType.SmallWide:
						return 60;
					case Utils.NativeAdType.LargeWide:
						return 180;
					default:
						return 0;
					}
				}
			}

			public string LogoImageUrl {
				get {
					if (Utils.NativeAdType.LargeWide == m_AdType) {
						return "https://img1.nend.net/img/banner/329/71103/1307042.png";
					} else {
						return null;
					}
				}
			}

			public int LogoImageWidth {
				get {
					if (Utils.NativeAdType.LargeWide == m_AdType) {
						return 40;
					} else {
						return 0;
					}
				}
			}

			public int LogoImageHeight {
				get {
					if (Utils.NativeAdType.LargeWide == m_AdType) {
						return 40;
					} else {
						return 0;
					}
				}
			}
		}
		#endif
	}
}