namespace NendUnityPlugin.AD.Native.Validations
{
	using UnityEngine;
	using UnityEngine.UI;

	using Log = NendUnityPlugin.AD.Native.Utils.NendAdLogger;

	internal class ImageVisibilityValidator : VisibilityValidator
	{
		internal ImageVisibilityValidator (GameObject root) : base (root)
		{
		}

		public override bool Validate<T> (T target)
		{
			if (!base.Validate (target)) {
				return false;
			}
			if (target is RawImage) {
				var image = target as RawImage;
				var uvRect = image.uvRect;
				if (.0f >= image.uvRect.width || .0f >= image.uvRect.height) {
					Log.W ("The size of UVRect for {0} must be more than 0. rect: {1}", image.name, uvRect);
					return false;
				}
				if (1.0f < uvRect.width || 1.0f < uvRect.height) {
					Log.W ("The size of UVRect for {0} must be below 1. rect: {1}", image.name, uvRect);
					return false;
				}
				return true;
			} else {
				return false;
			}
		}
	}
}

