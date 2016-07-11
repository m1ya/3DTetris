namespace NendUnityPlugin.AD.Native
{
	using System;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Events;

	using NendUnityPlugin.Platform;
	using NendUnityPlugin.Common;
	using NendUnityPlugin.AD.Native.Events;

	using Log = NendUnityPlugin.AD.Native.Utils.NendAdLogger;

	/// <summary>
	/// Native ad.
	/// </summary>
	public sealed class NendAdNative : MonoBehaviour
	{
		[SerializeField] private NendUtils.Account account;
		[SerializeField] private bool loadWhenActivated = true;
		[SerializeField] private NendAdNativeView[] views;

		#region AdEvents

		/// <summary>
		/// The ad loaded.
		/// </summary>
		public NativeAdViewEvent AdLoaded;

		/// <summary>
		/// The ad failed to receive.
		/// </summary>
		public NativeAdViewFailedToLoadEvent AdFailedToReceive;

		#endregion

		#if UNITY_EDITOR
		[SerializeField] private NendUtils.NativeAdType type;
		#endif

		private INativeAdClient m_Client = null;

		private INativeAdClient Client {
			get {
				if (null == m_Client) {
					#if UNITY_EDITOR
					m_Client = NendAdNativeInterfaceFactory.CreateNativeAdClient (type);
					#else
					m_Client = NendAdNativeInterfaceFactory.CreateNativeAdClient (account);
					#endif
				}
				return m_Client;
			}
		}
			
		// Use this for initialization
		void Start ()
		{
			if (loadWhenActivated) {
				LoadAd ();
			}
		}

		void OnDestroy ()
		{
			Log.D ("OnDestroy: {0}", name);
			views = null;
			m_Client = null;
		}

		#region Public

		/// <summary>
		/// Gets the list of NendAdNativeView.
		/// </summary>
		/// <value>The list of NendAdNativeView.</value>
		public NendAdNativeView[] Views {
			get {
				return views;
			}
		}

		/// <summary>
		/// Registers the NendAdNativeView.
		/// </summary>
		/// <param name="view">NendAdNativeView</param>
		public void RegisterAdView (NendAdNativeView view)
		{
			if (null != view && views.All (v => v != view)) {
				views = views.Concat (new NendAdNativeView[] { view }).ToArray ();
			}
		}

		/// <summary>
		/// Unregisters the NendAdNativeView.
		/// </summary>
		/// <param name="view">NendAdNativeView</param>
		public void UnregisterAdView (NendAdNativeView view)
		{
			if (null != view) {
				var index = Array.IndexOf (views, view);
				if (-1 != index) {
					Array.Clear (views, index, 1);
				}
			}
		}

		/// <summary>
		/// Loads the ad.
		/// </summary>
		public void LoadAd ()
		{
			if (null != views && 0 < views.Length) {
				LoadAd (views.Where (v => null != v).DefaultIfEmpty ().ToArray ());
			} else {
				Log.W ("views are empty.");
			}
		}

		/// <summary>
		/// Loads the ad.
		/// </summary>
		/// <param name="tag">Tag of NendAdNativeView.</param>
		public void LoadAd (int tag)
		{
			if (null != views && 0 < views.Length) {
				LoadAd (views.Where (v => null != v && v.ViewTag == tag).DefaultIfEmpty ().ToArray ());
			} else {
				Log.W ("views are empty.");
			}
		}

		#endregion

		#region Internal

		private void LoadAd (NendAdNativeView[] views)
		{
			foreach (var view in views) {
				if (null == view) {
					continue;
				}
				var nativeAdView = view; // Important
				Client.LoadNativeAd ((nativeAd, code, message) => {
					if (null != nativeAd) {
						Log.D ("Load native ad. tag: {0}", nativeAdView.ViewTag);
						nativeAdView.NativeAd = nativeAd;
						PostAdLoaded (nativeAdView);
					} else {
						Log.E ("Failed to load ad. code: {0}, message: {1}", code, message);
						PostAdFailedToReceive (nativeAdView, code, message);
					}
				});
			}
		}

		#endregion

		#region Notifications

		private void PostAdLoaded (NendAdNativeView view)
		{
			if (null != AdLoaded) {
				AdLoaded.Invoke (view);
			}
		}

		private void PostAdFailedToReceive (NendAdNativeView view, int code, string message)
		{
			if (null != AdFailedToReceive) {
				AdFailedToReceive.Invoke (view, code, message);
			}
		}

		#endregion
	}
}