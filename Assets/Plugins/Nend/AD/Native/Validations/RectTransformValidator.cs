namespace NendUnityPlugin.AD.Native.Validations
{
	using UnityEngine;
	using UnityEngine.UI;

	using Log = NendUnityPlugin.AD.Native.Utils.NendAdLogger;

	internal class RectTransformValidator : IValidator
	{
		private RectTransform m_ParentTransform;

		internal RectTransformValidator (RectTransform parentTransform)
		{
			m_ParentTransform = parentTransform;
		}

		public bool Validate<T> (T target) where T : Graphic
		{
			if (IsPlacedInsideOfParentTransform (target.rectTransform, m_ParentTransform)) {
				return true;
			} else {
				Log.W ("The {0} is not placed completely on inside of the Panel.", target.name);
				return false;
			}
		}

		private static bool IsPlacedInsideOfParentTransform (RectTransform childTransform, RectTransform parentTransform)
		{
			var localX = (decimal)childTransform.localPosition.x;
			var localY = (decimal)childTransform.localPosition.y;
			var parentWidth = (decimal)parentTransform.rect.width;
			var parentHeight = (decimal)parentTransform.rect.height;

			var posX = localX + parentWidth * (decimal)parentTransform.pivot.x;
			var posY = localY + parentHeight * (decimal)parentTransform.pivot.y;

			var width = (decimal)childTransform.rect.width;
			var height = (decimal)childTransform.rect.height;

			var left = posX - width * (decimal)childTransform.pivot.x;
			var right = left + width;
			var bottom = posY - height * (decimal)childTransform.pivot.y;
			var top = bottom + height;

			return 0m <= decimal.Round (left)
			&& 0m <= decimal.Round (bottom)
			&& decimal.Round (parentWidth) >= decimal.Round (right)
			&& decimal.Round (parentHeight) >= decimal.Round (top);
		}
	}
}

