namespace NendUnityPlugin.AD.Native.Validations
{
	using System;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;

	using Log = NendUnityPlugin.AD.Native.Utils.NendAdLogger;

	internal class ImageClipValidator : IValidator
	{
		private const decimal MAX_CLIP = 0.06m;

		private bool m_IsClippingEnabled;

		internal ImageClipValidator (bool isClippingEnabled = true)
		{
			m_IsClippingEnabled = isClippingEnabled;
		}

		public bool Validate<T> (T target) where T : Graphic
		{
			if (target is RawImage) {
				var image = target as RawImage;
				var uvRect = image.uvRect;

				if (!IsClippedRect (uvRect)) {
					return true;
				}

				if (!m_IsClippingEnabled) {
					Log.W ("The {0} not allow to clipping.", target.name);
					return false;
				}
	
				var leftClip = (decimal)uvRect.x;
				var rightClip = 1.0m - (leftClip + (decimal)uvRect.width);
				var topClip = (decimal)uvRect.y;
				var bottomClip = 1.0m - (topClip + (decimal)uvRect.height);

				if (!IsValidClipRatios (leftClip, rightClip, topClip, bottomClip)) {
					Log.W ("A clipping ratio of {0} is invalid, A clipping ratio of image must be less than 6%. uvRect: {1}", target.name, uvRect);
					return false;
				}

				return true;
			} else {
				return false;
			}
		}

		private bool IsValidClipRatios (params decimal[] values)
		{
			return values.All (d => MAX_CLIP >= d);
		}

		private static bool IsClippedRect (Rect rect)
		{
			return IsBetweenExclusive (Math.Abs (rect.x), .0f, 1.0f) || IsBetweenExclusive (Math.Abs (rect.y), .0f, 1.0f)
			|| IsBetweenExclusive (rect.width, .0f, 1.0f) || IsBetweenExclusive (rect.height, .0f, 1.0f);
		}

		private static bool IsBetweenExclusive (float value, float min, float max)
		{
			return min < value && max > value;
		}
	}
}