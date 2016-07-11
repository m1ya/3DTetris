namespace NendUnityPlugin.AD.Native.Utils
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	using NendUnityPlugin.AD.Native;
	using NendUnityPlugin.AD.Native.Validations;

	internal class RectTransformChangeListener
	{
		internal delegate void ReapplyDrivenPropertiesHandler (Graphic target);

		internal event ReapplyDrivenPropertiesHandler reapplyDrivenPropertiesHandler;

		private static RectTransformChangeListener s_Instance = null;
		private HashSet<Graphic> m_NativeAdGraphics;
		private HashSet<MonoBehaviour> m_NativeAdViews;

		internal static RectTransformChangeListener Instance {
			get {
				return s_Instance ?? (s_Instance = new RectTransformChangeListener ());
			}
		}

		private RectTransformChangeListener ()
		{
			RectTransform.reapplyDrivenProperties += OnReapplyDrivenProperties;
			m_NativeAdGraphics = new HashSet<Graphic> ();
			m_NativeAdViews = new HashSet <MonoBehaviour> ();
		}

		internal void AttachNativeAdView (NendAdNativeView nativeAdView, ReapplyDrivenPropertiesHandler handler)
		{
			if (!ContainsHandler (handler)) {
				reapplyDrivenPropertiesHandler += handler;
			}
			m_NativeAdViews.Add (nativeAdView);
		}

		internal void DetachNativeAdView (NendAdNativeView nativeAdView, ReapplyDrivenPropertiesHandler handler)
		{
			if (ContainsHandler (handler)) {
				reapplyDrivenPropertiesHandler -= handler;
			}
			m_NativeAdGraphics.RemoveWhere (g => g.transform.IsChildOf (nativeAdView.transform));
			m_NativeAdViews.Remove (nativeAdView);
		}

		private bool ContainsHandler (ReapplyDrivenPropertiesHandler handler)
		{
			if (null == reapplyDrivenPropertiesHandler) {
				return false;
			}
			Delegate[] delegates = reapplyDrivenPropertiesHandler.GetInvocationList ();
			return delegates.Any (d => d == handler);
		}

		private void OnReapplyDrivenProperties (RectTransform driven)
		{
			if (null == reapplyDrivenPropertiesHandler
			    || 0 == reapplyDrivenPropertiesHandler.GetInvocationList ().Length) {
				return;
			}
				
			Graphic graphic = FindNativeAdGraphic (driven);
			if (null != graphic) {
				reapplyDrivenPropertiesHandler (graphic);
			}
		}

		private Graphic FindNativeAdGraphic (RectTransform transform)
		{
			Graphic graphic = m_NativeAdGraphics.FirstOrDefault (g => g.rectTransform == transform);
			if (null == graphic) {
				bool isTarget = m_NativeAdViews.Any (v => transform.IsChildOf (v.transform));
				if (isTarget) {
					var component = transform.GetComponent <Graphic> ();
					if (null != component && component is IValidatable) {
						m_NativeAdGraphics.Add (component);
						graphic = component;
					}
				}
			}
			return graphic;
		}
	}
}

