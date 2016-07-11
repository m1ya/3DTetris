namespace NendUnityPlugin.AD.Native.Validations
{
	public interface IValidator
	{
		bool Validate <T> (T target) where T : UnityEngine.UI.Graphic;
	}
}