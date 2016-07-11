#if UNITY_ANDROID
namespace NendUnityPlugin.Platform.Android
{
    using System;
    using UnityEngine;

    using Log = NendUnityPlugin.AD.Native.Utils.NendAdLogger;
	
    internal class AndroidNativeAd : INativeAd, IDisposable
	{
		private const string UnityPlayerClassName = "com.unity3d.player.UnityPlayer";
		private AndroidJavaObject m_NativeAd;

		internal AndroidNativeAd (AndroidJavaObject nativeAd)
		{
			m_NativeAd = nativeAd;
		}

		~AndroidNativeAd ()
		{
			Dispose ();
		}

		public void Dispose ()
		{
            Log.D ("Dispose AndroidNativeAd.");
			if (null != m_NativeAd) {
				m_NativeAd.Dispose ();
				m_NativeAd = null;
			}
		}

		public void SendImpression ()
		{
			m_NativeAd.Call ("sendImpression");
		}

		public void PerformAdClick ()
		{
			using (var unityPlayer = new AndroidJavaClass (UnityPlayerClassName)) {
				using (var activity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity")) {
					m_NativeAd.Call ("performAdClick", activity);
				}
			}
		}

		public void PerformInformationClick ()
		{
			using (var unityPlayer = new AndroidJavaClass (UnityPlayerClassName)) {
				using (var activity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity")) {
					m_NativeAd.Call ("performInformationClick", activity);
				}
			}
		}

		public string GetAdvertisingExplicitlyText (int advertisingExplicitly)
		{
			return m_NativeAd.Call<string> ("getAdvertisingExplicitlyText", advertisingExplicitly);
		}

		public string ShortText {
			get {
				return m_NativeAd.Call<string> ("getShortText");
			}
		}

		public string LongText {
			get {
				return m_NativeAd.Call<string> ("getLongText");
			}
		}

		public string PromotionUrl {
			get {
				return m_NativeAd.Call<string> ("getPromotionUrl");
			}
		}

		public string PromotionName {
			get {
				return m_NativeAd.Call<string> ("getPromotionName");
			}
		}

		public string ActionButtonText {
			get {
				return m_NativeAd.Call<string> ("getActionButtonText");
			}
		}

		public string AdImageUrl {
			get {
				return m_NativeAd.Call<string> ("getAdImageUrl");
			}
		}

		public int AdImageWidth {
			get {
				return m_NativeAd.Call<int> ("getAdImageWidth");
			}
		}

		public int AdImageHeight {
			get {
				return m_NativeAd.Call<int> ("getAdImageHeight");
			}
		}

		public string LogoImageUrl {
			get {
				return m_NativeAd.Call<string> ("getLogoImageUrl");
			}
		}

		public int LogoImageWidth {
			get {
				return m_NativeAd.Call<int> ("getLogoImageWidth");
			}
		}

		public int LogoImageHeight {
			get {
				return m_NativeAd.Call<int> ("getLogoImageHeight");
			}
		}
	}
}
#endif