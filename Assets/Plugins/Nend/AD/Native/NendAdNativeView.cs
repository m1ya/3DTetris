namespace NendUnityPlugin.AD.Native
{
	using System;
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	using NendUnityPlugin.Platform;
	using NendUnityPlugin.AD.Native.Utils;
	using NendUnityPlugin.AD.Native.Validations;
	using NendUnityPlugin.AD.Native.Trackers;
	using NendUnityPlugin.AD.Native.Events;

	using Log = NendUnityPlugin.AD.Native.Utils.NendAdLogger;

	/// <summary>
	/// Native ad view.
	/// </summary>
	[RequireComponent (typeof(Mask))]
	[RequireComponent (typeof(RectTransform))]
	public sealed class NendAdNativeView : MonoBehaviour, IPointerClickHandler
	{
		/// <summary>
		/// Advertising explicitly.
		/// </summary>
		public enum AdvertisingExplicitly : int
		{
			PR = 0,
			Sponsored,
			AD,
			Promotion,
		}

		[SerializeField] private int viewTag;
		[SerializeField] private bool renderWhenLoaded = true;
		[SerializeField] private bool renderWhenActivated = true;
		[SerializeField] private AdvertisingExplicitly advertisingExplicitly;
		[SerializeField] private NendAdNativeText prText;
		[SerializeField] private NendAdNativeText shortText;
		[SerializeField] private NendAdNativeText longText;
		[SerializeField] private NendAdNativeText promotionNameText;
		[SerializeField] private NendAdNativeText promotionUrlText;
		[SerializeField] private NendAdNativeText actionButtonText;
		[SerializeField] private NendAdNativeImage adImage;
		[SerializeField] private NendAdNativeImage logoImage;

		#region AdEvents

		/// <summary>
		/// The ad shown.
		/// </summary>
		public NativeAdViewEvent AdShown;

		/// <summary>
		/// The ad failed to show.
		/// </summary>
		public NativeAdViewEvent AdFailedToShow;

		/// <summary>
		/// The ad clicked.
		/// </summary>
		public NativeAdViewEvent AdClicked;

		#endregion

		private bool m_Rendered = false;
		private bool m_Impression = false;
		private INativeAd m_NativeAd = null;
		private IValidator m_VisibilityValidator = null;
		private IValidator m_ImageVisibilityValidator = null;
		private IValidator m_RectTransformValidator = null;
		private Canvas m_Canvas = null;
		private RectTransform m_RectTransform = null;
		private IImpressionTracker m_ImpressionTracker = null;
		private float m_TimeElapsed = .0f;
		private HashSet<IValidatable> m_Validatables = new HashSet<IValidatable> ();

		/// <summary>
		/// Gets or sets the view tag.
		/// </summary>
		/// <value>The view tag.</value>
		public int ViewTag {
			get {
				return viewTag;
			}
			set {
				viewTag = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="NendUnityPlugin.AD.Native.NendAdNativeView"/> is loaded.
		/// </summary>
		/// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
		public bool Loaded { 
			get {
				return null != m_NativeAd;
			}
		}

		internal INativeAd NativeAd { 
			set {
				if (value != m_NativeAd) {
					m_NativeAd = value;
					if (renderWhenLoaded || m_Rendered) {
						m_Rendered = false;
						m_Impression = false;
						SafeStartCoroutine (RenderSelf ());
					}
				}
			} 
		}

		private Canvas ParentCanvas { 
			get { 
				return m_Canvas ?? (m_Canvas = GetComponentInParent <Canvas> ()); 
			} 
		}

		private RectTransform AdRectTransform {
			get {
				return m_RectTransform ?? (m_RectTransform = GetComponent <RectTransform> ());
			}
		}

		private IImpressionTracker ImpressionTracker {
			get {
				return m_ImpressionTracker ?? (m_ImpressionTracker = CreateTrackerForCurrentRenderMode ());
			}
		}

		void Awake ()
		{
			ClearSelf (false);
		}

		// Use this for initialization
		void Start ()
		{
			m_VisibilityValidator = new VisibilityValidator (gameObject);
			m_ImageVisibilityValidator = new ImageVisibilityValidator (gameObject);
			m_RectTransformValidator = new RectTransformValidator (AdRectTransform);

			if (null != prText) {
				var handler = prText.gameObject.AddComponent <PRTextClickHandler> ();
				handler.OnClick += (sender, e) => {
					if (null != m_NativeAd && m_Rendered) {
						m_NativeAd.PerformInformationClick ();
					}
				};
			}
		}
			
		// Update is called once per frame
		void Update ()
		{
			m_TimeElapsed += Time.deltaTime;
			if (m_TimeElapsed >= 0.5f) {
				if (!m_Impression && m_Rendered && Loaded) {
					Log.D ("Tracking impression... ViewTag: {0}", ViewTag);
					if (ImpressionTracker.IsImpression ()) {
						m_NativeAd.SendImpression ();
						Log.D ("Impression ad. ViewTag: {0}", ViewTag);
						m_Impression = true;
					}
				}
				m_TimeElapsed = .0f;
			}
		}

		void OnEnable ()
		{
			if (Loaded && renderWhenActivated) {
				SafeStartCoroutine (RenderSelf ());
			}
		}

		void OnDisable ()
		{
			ClearSelf (false);
		}

		void OnDestroy ()
		{
			Log.D ("OnDestroy: {0}", name);
			m_NativeAd = null;
		}

		#region Public

		/// <summary>
		/// Renders the ad.
		/// </summary>
		/// <returns><c>true</c>, if ad was rendered, <c>false</c> otherwise.</returns>
		public bool RenderAd ()
		{
			if (Loaded && !m_Rendered) {
				SafeStartCoroutine (RenderSelf ());
				return true;
			} else {
				return false;
			}
		}

		#endregion

		#region Internal

		private IImpressionTracker CreateTrackerForCurrentRenderMode ()
		{
			IImpressionTracker tracker = null;
			Canvas canvas = ParentCanvas;
			var canvasTransform = canvas.GetComponent <RectTransform> ();
			switch (canvas.renderMode) {
			case RenderMode.ScreenSpaceOverlay:
				tracker = new OverlayImpressionTracker (AdRectTransform, canvasTransform);
				break;
			case RenderMode.ScreenSpaceCamera:
				tracker = new CameraImpressionTracker (AdRectTransform, canvasTransform, canvas.worldCamera);
				break;
			case RenderMode.WorldSpace:
				tracker = new WorldImpressionTracker (AdRectTransform, canvas.worldCamera ?? Camera.main);
				break;
			}
			return tracker;
		}

		private IEnumerator RenderSelf ()
		{
			yield return null;

			if (!CheckUIConfiguration ()) {
				ClearSelf ();
				yield break;
			}
				
			RenderTextIfNotNull (prText, m_NativeAd.GetAdvertisingExplicitlyText ((int)advertisingExplicitly));
			RenderTextIfNotNull (shortText, m_NativeAd.ShortText);
			RenderTextIfNotNull (longText, m_NativeAd.LongText);
			RenderTextIfNotNull (promotionNameText, m_NativeAd.PromotionName);
			RenderTextIfNotNull (promotionUrlText, m_NativeAd.PromotionUrl);
			RenderTextIfNotNull (actionButtonText, m_NativeAd.ActionButtonText);

			yield return new WaitForEndOfFrame ();

			RegisterValidators ();

			if (m_Validatables.All (v => v.Validate ())) {
				RenderImageIfNotNull (logoImage, m_NativeAd.LogoImageUrl);
				if (!RenderImageIfNotNull (adImage, m_NativeAd.AdImageUrl)) {
					OnRendered ();
				}
			} else {
				ClearSelf ();
			}
		}

		private bool CheckUIConfiguration ()
		{
			if (!IsExist (prText)) {
				Log.W ("Must include PR text.");
				return false;
			}
			if (!IsExist (shortText) && !IsExist (longText) && !IsExist (promotionNameText)) {
				Log.W ("Must include one of short text, long text and promotion name.");
				return false;
			}
			if (!string.IsNullOrEmpty (m_NativeAd.AdImageUrl) && !IsExist (adImage)) {
				Log.W ("In the case of ad types for displaying an image, must include AD image.");
				return false;
			}
			return true;
		}

		private void OnRendered ()
		{
			m_Rendered = true;
			RectTransformChangeListener.Instance.AttachNativeAdView (this, OnReapplyDrivenProperties);
			PostAdShown ();
		}

		private void ClearSelf (bool notifyError = true)
		{
			m_Rendered = false;

			foreach (var validatable in m_Validatables) {
				validatable.StopValidation ();
			}
			m_Validatables.Clear ();

			RectTransformChangeListener.Instance.DetachNativeAdView (this, OnReapplyDrivenProperties);

			ClearTextIfNotNull (prText);
			ClearTextIfNotNull (shortText);
			ClearTextIfNotNull (longText);
			ClearTextIfNotNull (promotionNameText);
			ClearTextIfNotNull (promotionUrlText);
			ClearTextIfNotNull (actionButtonText);
			ClearImageIfNotNull (adImage);
			ClearImageIfNotNull (logoImage);

			if (notifyError) {
				PostAdFailedToShow ();
			}
		}

		private void RegisterValidators ()
		{
			if (null != prText) {
				RegisterValidator (prText, m_VisibilityValidator, m_RectTransformValidator);
			}

			if (null != shortText) {
				RegisterValidator (shortText, m_VisibilityValidator, m_RectTransformValidator);
			}

			if (null != longText) {
				RegisterValidator (longText, m_VisibilityValidator, m_RectTransformValidator);
			}

			if (null != promotionNameText) {
				RegisterValidator (promotionNameText, m_VisibilityValidator, m_RectTransformValidator);
			}

			if (null != promotionUrlText) {
				RegisterValidator (promotionUrlText, m_RectTransformValidator);
			}

			if (null != actionButtonText) {
				RegisterValidator (actionButtonText, m_RectTransformValidator);
			}

			if (null != adImage && !string.IsNullOrEmpty (m_NativeAd.AdImageUrl)) {
				var imageSize = new Vector2 (m_NativeAd.AdImageWidth, m_NativeAd.AdImageHeight);
				RegisterValidator (adImage, m_ImageVisibilityValidator, m_RectTransformValidator, new ImageScaleValidator (imageSize), new ImageClipValidator ());
			}

			if (null != logoImage && !string.IsNullOrEmpty (m_NativeAd.LogoImageUrl)) {
				var imageSize = new Vector2 (m_NativeAd.LogoImageWidth, m_NativeAd.LogoImageHeight);
				RegisterValidator (logoImage, m_RectTransformValidator, new ImageScaleValidator (imageSize, false), new ImageClipValidator (false));
			}
		}

		private void RegisterValidator (IValidatable target, params IValidator[] validators)
		{
			target.BeginValidation (validators, (validatable) => {
				SafeStartCoroutine (ValidateDirtyObject (validatable));
			});
			m_Validatables.Add (target);
		}

		private IEnumerator ValidateDirtyObject (IValidatable validatable)
		{
			if (null != validatable) {
				yield return new WaitForEndOfFrame ();
				if (m_Validatables.Contains (validatable) && !validatable.Validate ()) {
					ClearSelf ();
				}
			}
		}

		private IEnumerator LoadImage (string url, NendAdNativeImage target)
		{
			Texture2D texture = TextureCache.Instance.Get (url);
			if (null != texture) {
				OnLoadImage (target, texture);
				yield break;
			}

			using (var www = new WWW (url)) {
				yield return www;
				if (string.IsNullOrEmpty (www.error)) {
					texture = www.texture;
					TextureCache.Instance.Put (url, texture);
					OnLoadImage (target, texture);
				} else {
					Log.E ("Failed to download image: {0}", www.error);
					if (target == adImage) {
						ClearSelf ();
					}
				}
			}
		}

		private void OnLoadImage (NendAdNativeImage image, Texture2D texture)
		{
			image.texture = texture;
			if (image == adImage) {
				OnRendered ();
			}
		}

		private bool RenderTextIfNotNull (NendAdNativeText target, string text)
		{
			if (null != target) {
				target.TextInternal = text;
				return true;
			} else {
				return false;
			}
		}

		private bool RenderImageIfNotNull (NendAdNativeImage target, string url)
		{
			if (null != target && !string.IsNullOrEmpty (url)) {
				SafeStartCoroutine (LoadImage (url, target));
				return true;
			} else {
				return false;
			}
		}

		private void ClearTextIfNotNull (NendAdNativeText target)
		{
			if (null != target) {
				target.TextInternal = String.Empty;
			}
		}

		private void ClearImageIfNotNull (NendAdNativeImage target)
		{
			if (null != target) {
				target.texture = null;
			}
		}

		private bool IsExist (Graphic graphic)
		{
			return null != graphic && graphic.transform.IsChildOf (this.transform);
		}

		private void SafeStartCoroutine (IEnumerator enumerator)
		{
			if (gameObject.activeSelf && gameObject.activeInHierarchy) {
				StartCoroutine (enumerator);
			}
		}

		#endregion

		#region RectTransform#reapplyDrivenProperties

		private void OnReapplyDrivenProperties (Graphic graphic)
		{
			SafeStartCoroutine (ValidateDirtyObject (graphic as IValidatable));
		}

		#endregion

		#region Notifications

		private void PostAdShown ()
		{
			if (null != AdShown) {
				AdShown.Invoke (this);
			}
		}

		private void PostAdFailedToShow ()
		{
			if (null != AdFailedToShow) {
				AdFailedToShow.Invoke (this);
			}
		}

		private void PostAdClicked ()
		{
			if (null != AdClicked) {
				AdClicked.Invoke (this);
			}
		}

		#endregion

		#region ClickEvents

		public void OnPointerClick (PointerEventData eventData)
		{
			if (null != m_NativeAd && m_Rendered) {
				PostAdClicked ();
				m_NativeAd.PerformAdClick ();
			}
		}

		private class PRTextClickHandler : MonoBehaviour, IPointerClickHandler
		{
			internal event EventHandler OnClick;

			public void OnPointerClick (PointerEventData eventData)
			{
				EventHandler handler = OnClick;
				handler (this, EventArgs.Empty);
			}
		}

		#endregion
	}
}