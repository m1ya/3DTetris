#if UNITY_ANDROID
namespace NendUnityPlugin.Platform.Android
{
	using System;
	using UnityEngine;
	using System.Collections.Generic;

	using Callback = System.Action<INativeAd, int, string>;
	using Log = NendUnityPlugin.AD.Native.Utils.NendAdLogger;

	internal class AndroidNativeAdClient : AndroidJavaProxy, INativeAdClient, IDisposable
	{
		private const string NendAdNativeListenerClassName = "net.nend.unity.plugin.NendUnityNativeAdListener";
		private const string NendAdNativeClientClassName = "net.nend.unity.plugin.NendUnityNativeAdClient";
		private const string UnityPlayerClassName = "com.unity3d.player.UnityPlayer";

		private AndroidJavaObject m_Client;
		private List<Callback> m_Callbacks;

		internal AndroidNativeAdClient (string apiKey, string spotId) : base (NendAdNativeListenerClassName)
		{
			using (var unityPlayer = new AndroidJavaClass (UnityPlayerClassName)) {
				using (var activity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity")) {
					m_Client = new AndroidJavaObject (NendAdNativeClientClassName, activity, int.Parse (spotId), apiKey);
					m_Client.Call ("setUnityAdListener", this);
				}
			}
			m_Callbacks = new List<Callback> ();
		}

		~AndroidNativeAdClient ()
		{
			Dispose ();
		}

		public void LoadNativeAd (Callback callback)
		{
			m_Callbacks.Add (callback);
			m_Client.Call ("loadAd");
		}

		public void Dispose ()
		{
			Log.D ("Dispose AndroidNativeAdClient.");
			m_Callbacks.Clear ();
			if (null != m_Client) {
				m_Client.Dispose ();
				m_Client = null;
			}
		}

		void onResponse (AndroidJavaObject ad, int code, string message)
		{
			if (0 < m_Callbacks.Count) {
				var callback = m_Callbacks [0];
				m_Callbacks.RemoveAt (0);
				if (null != ad) {
					callback (new AndroidNativeAd (ad), code, null);
				} else {
					callback (null, code, message);
				}
			}
		}
	}
}
#endif