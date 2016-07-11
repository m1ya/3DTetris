namespace NendUnityPlugin.Platform.iOS
{
	using System;
	using System.Runtime.InteropServices;
	using System.Collections.Generic;

	using Callback = System.Action<INativeAd, int, string>;
	using Log = NendUnityPlugin.AD.Native.Utils.NendAdLogger;

	internal class IOSNativeAdClient : INativeAdClient, IDisposable
	{
		internal delegate void NendUnityNativeAdCallback (IntPtr client, IntPtr nativeAd, int code, string message);

		private IntPtr m_ClientPtr;
		private List<Callback> m_Callbacks;

		internal IOSNativeAdClient (string apiKey, string spotId)
		{
			m_ClientPtr = _CreateNativeAdClient (apiKey, spotId);
			m_Callbacks = new List<Callback> ();
		}

		~IOSNativeAdClient ()
		{
			Dispose ();
		}

		public void Dispose ()
		{
			Log.D ("Dispose IOSNativeAdClient.");
			m_Callbacks.Clear ();
			_ReleaseNativeAdClient (m_ClientPtr);
			if (IntPtr.Zero != m_ClientPtr) {
				GCHandle handle = (GCHandle)m_ClientPtr;
				handle.Free ();
				m_ClientPtr = IntPtr.Zero;
			}
		}

		public void LoadNativeAd (Callback callback)
		{
			m_Callbacks.Add (callback);
			IntPtr selfPtr = (IntPtr)GCHandle.Alloc (this);
			_LoadNativeAd (selfPtr, m_ClientPtr, NativeAdCallback);
		}

		[AOT.MonoPInvokeCallbackAttribute (typeof(NendUnityNativeAdCallback))]
		private static void NativeAdCallback (IntPtr client, IntPtr nativeAd, int code, string message)
		{
			GCHandle handle = (GCHandle)client;
			IOSNativeAdClient self = handle.Target as IOSNativeAdClient;
			if (0 < self.m_Callbacks.Count) {
				var callback = self.m_Callbacks [0];
				self.m_Callbacks.RemoveAt (0);
				if (200 == code) {
					var ad = new IOSNativeAd (nativeAd);
					callback (ad, code, null);
				} else {
					callback (null, code, message);
				}
			}
			handle.Free ();
		}

		[DllImport ("__Internal")]
		private static extern IntPtr _CreateNativeAdClient (string apiKey, string spotId);

		[DllImport ("__Internal")]
		private static extern void _ReleaseNativeAdClient (IntPtr iosClient);

		[DllImport ("__Internal")]
		private static extern void _LoadNativeAd (IntPtr self, IntPtr iosClient, NendUnityNativeAdCallback callback);
	}
}

