namespace NendUnityPlugin.AD.Native.Validations
{
	using UnityEngine;
	using UnityEngine.UI;

	using Log = NendUnityPlugin.AD.Native.Utils.NendAdLogger;

	internal class ImageScaleValidator : IValidator
	{
		private const float MIN_SCALE = 0.3f;
		private const float MAX_SCALE = 1.5f;

		private Vector2 m_ImageSize;
		private bool m_HasScaleLimit;

		internal ImageScaleValidator (Vector2 imageSize, bool hasScaleLimit = true)
		{
			m_ImageSize = imageSize;
			m_HasScaleLimit = hasScaleLimit;
		}

		public bool Validate<T> (T target) where T : Graphic
		{
			if (target is RawImage) {
				var image = target as RawImage;
				var uvRect = image.uvRect;
				Vector2 uiSize = GetScaledSize (target);
				var textureSize = new Vector2 (uiSize.x / uvRect.width, uiSize.y / uvRect.height);
				var widthScale = textureSize.x / m_ImageSize.x;
				var heightScale = textureSize.y / m_ImageSize.y;

				if (widthScale != heightScale) {
					Log.W ("If want to scale the {0}, need to keep aspect ratio. w: {1:F}, h: {2:F}", target.name, widthScale, heightScale);
					return false;
				} else if (m_HasScaleLimit && (MIN_SCALE > widthScale || MAX_SCALE < widthScale)) {
					Log.W ("A scale of {0} is invalid, A scale of image must be between 30% and 150%. scale: {1:F}", target.name, widthScale);
					return false;
				}

				return true;
			} else {
				return false;
			}
		}

		private static Vector2 GetScaledSize (Graphic graphic)
		{
			RectTransform transform = graphic.rectTransform;
			return Vector2.Scale (transform.rect.size, transform.localScale);
		}
	}
}

