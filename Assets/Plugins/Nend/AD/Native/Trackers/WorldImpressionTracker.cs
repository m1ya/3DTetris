namespace NendUnityPlugin.AD.Native.Trackers
{
	using UnityEngine;

	internal class WorldImpressionTracker : BaseImpressionTracker
	{
		private Camera m_Camera;

		internal WorldImpressionTracker (RectTransform targetTransform, Camera camera) : base (targetTransform)
		{
			m_Camera = camera;
		}

		public override bool IsImpression ()
		{
			if (base.IsImpression ()) {
				var screenPoint = m_Camera.WorldToViewportPoint (m_TargetTransform.position);
				return .0f < screenPoint.z && .0f < screenPoint.x && 1.0f > screenPoint.x && .0f < screenPoint.y && 1.0f > screenPoint.y;
			} else {
				return false;
			}
		}
	}
}

