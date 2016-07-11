namespace NendUnityPlugin.AD.Native.Trackers
{
	using System.Linq;
	using UnityEngine;

	internal class CameraImpressionTracker : OverlayImpressionTracker
	{
		private Camera m_Camera;

		internal CameraImpressionTracker (RectTransform targetTransform, RectTransform canvasTransform, Camera camera) : base (targetTransform, canvasTransform)
		{
			m_Camera = camera;
		}

		protected override Vector2[] ConvertWorldToScreenCorners (Vector3[] worldCorners)
		{
			#if UNITY_5
			return worldCorners.Select (v => RectTransformUtility.WorldToScreenPoint (m_Camera, v)).ToArray ();
			#else
			var corners = new Vector2[worldCorners.Length];
			for (int i = 0; i < corners.Length; i++) {
				corners [i] = RectTransformUtility.WorldToScreenPoint (m_Camera, worldCorners [i]);
			}
			return corners;
			#endif
		}
	}
}