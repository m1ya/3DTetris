namespace NendUnityPlugin.AD.Native.Trackers
{
	using UnityEngine;
	using System.Linq;

	internal class OverlayImpressionTracker : BaseImpressionTracker
	{
		protected RectTransform m_CanvasTransform;

		internal OverlayImpressionTracker (RectTransform targetTransform, RectTransform canvasTransform) : base (targetTransform)
		{
			m_CanvasTransform = canvasTransform;
		}

		public override bool IsImpression ()
		{
			if (!base.IsImpression ()) {
				return false;
			}

			Vector3[] adCorners = new Vector3[4];
			Vector3[] canvasCorners = new Vector3[4];
			m_TargetTransform.GetWorldCorners (adCorners);
			m_CanvasTransform.GetWorldCorners (canvasCorners);

			Vector2[] adCorners2D = ConvertWorldToScreenCorners (adCorners);
			Vector2[] canvasCorners2D = ConvertWorldToScreenCorners (canvasCorners);

			return adCorners2D [1].x >= canvasCorners2D [1].x && adCorners2D [1].y <= canvasCorners2D [1].y// top left
			&& adCorners2D [3].x <= canvasCorners2D [3].x && adCorners2D [3].y >= canvasCorners2D [3].y; // bottom right
		}

		protected virtual Vector2[] ConvertWorldToScreenCorners (Vector3[] worldCorners)
		{
			#if UNITY_5
			return worldCorners.Select (v => new Vector2 (v.x, v.y)).ToArray ();
			#else
			var corners = new Vector2[worldCorners.Length];
			for (int i = 0; i < corners.Length; i++) {
				corners [i] = new Vector2 (worldCorners [i].x, worldCorners [i].y);
			}
			return corners;
			#endif
		}
	}
}

