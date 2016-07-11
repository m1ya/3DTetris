namespace NendUnityPlugin.Platform.iOS
{
	using System;
	using System.Runtime.InteropServices;

	using Log = NendUnityPlugin.AD.Native.Utils.NendAdLogger;

	internal class IOSNativeAd : INativeAd, IDisposable
	{
		private IntPtr m_NativeAdPtr;

		internal IOSNativeAd (IntPtr nativeAd)
		{
			m_NativeAdPtr = nativeAd;
		}

		~IOSNativeAd ()
		{
			Dispose ();
		}

		public void Dispose ()
		{
			Log.D ("Dispose IOSNativeAd.");
			_ReleaseNativeAd (m_NativeAdPtr);
			if (IntPtr.Zero != m_NativeAdPtr) {
				GCHandle handle = (GCHandle)m_NativeAdPtr;
				handle.Free ();
				m_NativeAdPtr = IntPtr.Zero;
			}
		}

		public void SendImpression ()
		{
			_SendImpression (m_NativeAdPtr);
		}

		public void PerformAdClick ()
		{
			_PerformAdClick (m_NativeAdPtr);
		}

		public void PerformInformationClick ()
		{
			_PerformInformationClick (m_NativeAdPtr);
		}

		public string GetAdvertisingExplicitlyText (int advertisingExplicitly)
		{
			return _GetAdvertisingExplicitlyText (m_NativeAdPtr, advertisingExplicitly);
		}

		public string ShortText {
			get {
				return _GetShortText (m_NativeAdPtr);
			}
		}

		public string LongText {
			get {
				return _GetLongText (m_NativeAdPtr);
			}
		}

		public string PromotionUrl {
			get {
				return _GetPromotionUrl (m_NativeAdPtr);
			}
		}

		public string PromotionName {
			get {
				return _GetPromotionName (m_NativeAdPtr);
			}
		}

		public string ActionButtonText {
			get {
				return _GetActionButtonText (m_NativeAdPtr);
			}
		}

		public string AdImageUrl {
			get {
				return _GetAdImageUrl (m_NativeAdPtr);
			}
		}

		public int AdImageWidth {
			get {
				return _GetAdImageWidth (m_NativeAdPtr);
			}
		}

		public int AdImageHeight {
			get {
				return _GetAdImageHeight (m_NativeAdPtr);
			}
		}

		public string LogoImageUrl {
			get {
				return _GetLogoImageUrl (m_NativeAdPtr);
			}
		}

		public int LogoImageWidth {
			get {
				return _GetLogoImageWidth (m_NativeAdPtr);
			}
		}

		public int LogoImageHeight {
			get {
				return _GetLogoImageHeight (m_NativeAdPtr);
			}
		}

		[DllImport ("__Internal")]
		private static extern string _GetShortText (IntPtr nativeAd);

		[DllImport ("__Internal")]
		private static extern string _GetLongText (IntPtr nativeAd);

		[DllImport ("__Internal")]
		private static extern string _GetPromotionName (IntPtr nativeAd);

		[DllImport ("__Internal")]
		private static extern string _GetPromotionUrl (IntPtr nativeAd);

		[DllImport ("__Internal")]
		private static extern string _GetActionButtonText (IntPtr nativeAd);

		[DllImport ("__Internal")]
		private static extern string _GetAdImageUrl (IntPtr nativeAd);

		[DllImport ("__Internal")]
		private static extern string _GetLogoImageUrl (IntPtr nativeAd);

		[DllImport ("__Internal")]
		private static extern string _GetAdvertisingExplicitlyText (IntPtr nativeAd, int unityAdvertisingExplicitly);

		[DllImport ("__Internal")]
		private static extern int _GetAdImageWidth (IntPtr nativeAd);

		[DllImport ("__Internal")]
		private static extern int _GetAdImageHeight (IntPtr nativeAd);

		[DllImport ("__Internal")]
		private static extern int _GetLogoImageWidth (IntPtr nativeAd);

		[DllImport ("__Internal")]
		private static extern int _GetLogoImageHeight (IntPtr nativeAd);

		[DllImport ("__Internal")]
		private static extern void _PerformAdClick (IntPtr nativeAd);

		[DllImport ("__Internal")]
		private static extern void _PerformInformationClick (IntPtr nativeAd);

		[DllImport ("__Internal")]
		private static extern void _SendImpression (IntPtr nativeAd);

		[DllImport ("__Internal")]
		private static extern void _ReleaseNativeAd (IntPtr nativeAd);
	}
}

