namespace NendUnityPlugin.AD.Native
{
	using System;
	using UnityEngine.UI;

	using NendUnityPlugin.AD.Native.Validations;

	public sealed class NendAdNativeImage : RawImage, IValidatable
	{
		private ValidationDelegate<NendAdNativeImage> m_Delegate;

		public NendAdNativeImage ()
		{
			m_Delegate = new ValidationDelegate <NendAdNativeImage> (this);
		}

		protected override void OnDisable ()
		{
			base.OnDisable ();
			m_Delegate.FireDirtyCallback ();
		}

		protected override void OnCanvasGroupChanged ()
		{
			base.OnCanvasGroupChanged ();
			m_Delegate.FireDirtyCallback ();
		}

		#region Validation

		public void BeginValidation (IValidator[] validators, Action<IValidatable> dirtyCallback)
		{
			m_Delegate.BeginValidation (validators, dirtyCallback);
		}

		public void StopValidation ()
		{
			m_Delegate.StopValidation ();
		}

		public bool Validate ()
		{
			return m_Delegate.Validate ();
		}

		#endregion
	}
}