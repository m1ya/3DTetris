using UnityEngine;
using UnityEditor;
using NendUnityPlugin.AD.Native;

public class NendAdNativeMenuItems : MonoBehaviour
{
	[MenuItem ("GameObject/UI/Nend/Text", true)]
	static bool ValidateAddNativeAdText ()
	{
		return ValidateMenuItem ();
	}

	[MenuItem ("GameObject/UI/Nend/Image", true)]
	static bool ValidateAddNativeAdImage ()
	{
		return ValidateMenuItem ();
	}

	[MenuItem ("GameObject/UI/Nend/Text")]
	static void AddNativeAdText (MenuCommand menuCommand)
	{
		var go = new GameObject ("Text");
		var text = go.AddComponent <NendAdNativeText> ();

		// default settings.
		text.rectTransform.sizeDelta = new Vector2 (160.0f, 30.0f);
		text.color = Color.black;

		PrepareGameObject (go, menuCommand);
	}

	[MenuItem ("GameObject/UI/Nend/Image")]
	static void AddNativeAdImage (MenuCommand menuCommand)
	{
		var go = new GameObject ("Image");
		go.AddComponent <NendAdNativeImage> ();
		PrepareGameObject (go, menuCommand);
	}

	private static void PrepareGameObject (GameObject go, MenuCommand menuCommand)
	{
		GameObjectUtility.SetParentAndAlign (go, menuCommand.context as GameObject);
		Undo.RegisterCreatedObjectUndo (go, "Create " + go.name);
		Selection.activeObject = go;
	}

	private static bool ValidateMenuItem ()
	{
		if (null != Selection.activeTransform) {
			var nativeAdView = Selection.activeTransform.GetComponent <NendAdNativeView> ();
			if (null != nativeAdView) {
				return true;
			} else {
				Debug.LogWarning ("Not supported.");
				return false;
			}
		} else {
			Debug.LogWarning ("Not supported.");
			return false;
		}
	}
}