namespace NendUnityPlugin.AD.Native.Trackers
{
	using UnityEngine;
	using UnityEngine.UI;

	internal class BaseImpressionTracker : IImpressionTracker
	{
		protected RectTransform m_TargetTransform;

		private Image m_Image;
		private CanvasGroup m_CanvasGroup;
		private bool m_TryToGetImageComponent;
		private bool m_TryToGetCanvasGroupComponent;

		internal BaseImpressionTracker (RectTransform targetTransform)
		{
			m_TargetTransform = targetTransform;
			m_TryToGetImageComponent = false;
			m_TryToGetCanvasGroupComponent = false;
		}

		public virtual bool IsImpression ()
		{
			if (.0f >= m_TargetTransform.localScale.x || .0f >= m_TargetTransform.localScale.y) {
				return false;
			}

			// Assume using the Panel.
			if (!m_TryToGetImageComponent) {
				m_Image = m_TargetTransform.GetComponent <Image> ();
				m_TryToGetImageComponent = true;
			}
			if (null != m_Image && .0f == m_Image.color.a) {
				return false;
			}

			if (!m_TryToGetCanvasGroupComponent) {
				m_CanvasGroup = m_TargetTransform.GetComponent<CanvasGroup> ();
				m_TryToGetCanvasGroupComponent = true;
			}
			if (null != m_CanvasGroup && .0f == m_CanvasGroup.alpha) {
				return false;
			}

			return true;
		}
	}
}

