namespace NendUnityPlugin.AD.Native.Validations
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Linq;

	using Log = NendUnityPlugin.AD.Native.Utils.NendAdLogger;

	public class VisibilityValidator : IValidator
	{
		private GameObject m_Root;

		internal VisibilityValidator (GameObject root)
		{
			m_Root = root;
		}

		public virtual bool Validate<T> (T target) where T : Graphic
		{
			if (null == target) {
				Log.W ("The UI component is null.");
				return false;
			}

			if (!target.isActiveAndEnabled) {
				Log.W ("The {0} is not active.", target.name);
				return false;
			}

			if (null == target.transform.parent) {
				Log.W ("The {0} has no parent transform.", target.name);
				return false;
			}
				
			if (.0f == target.color.a) {
				Log.W ("A color of {0} is transparent.", target.name);
				return false;
			}

			var transform = target.rectTransform;

			var localScale = transform.localScale;
			if (.0f >= localScale.x || .0f >= localScale.y) {
				Log.W ("A scale of {0} is 0. x: {1:F}, y: {1:F}", target.name, localScale.x, localScale.y);
				return false;
			}
			if (localScale.x != localScale.y) {
				Log.W ("The scale of X and Y of the {0} is different. x: {1:F}, y: {1:F}", target.name, localScale.x, localScale.y);
				return false;
			}

			var size = transform.rect.size;
			if (.0f >= size.x || .0f >= size.y) {
				Log.W ("A size of {0} must not be less 0. w: {1:F}, h: {1:F}", target.name, size.x, size.y);
				return false;
			}

			var canvasGroup = target.GetComponent<CanvasGroup> ();
			if (null != canvasGroup && .0f == canvasGroup.alpha) {
				Log.W ("An alpha of {0} is 0.", target.name);
				return false;
			} else if (null == canvasGroup || !canvasGroup.ignoreParentGroups) {
				CanvasGroup[] parentGroups = target.GetComponentsInParent<CanvasGroup> ();
				if (parentGroups.Where (g => g.gameObject != m_Root && .0f == g.alpha).Any ()) {
					Log.W ("An alpha of {0} is 0.", target.name);
					return false;
				}
			}

			return true;
		}
	}
}