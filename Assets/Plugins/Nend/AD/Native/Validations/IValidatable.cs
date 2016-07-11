namespace NendUnityPlugin.AD.Native.Validations
{
	public interface IValidatable
	{
		bool Validate ();

		void BeginValidation (IValidator[] validators, System.Action<IValidatable> dirtyCallback);

		void StopValidation ();
	}
}