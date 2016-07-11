namespace NendUnityPlugin.AD.Native
{
	using System;
	using UnityEngine.UI;

	using NendUnityPlugin.AD.Native.Validations;
	using Log = NendUnityPlugin.AD.Native.Utils.NendAdLogger;

	public sealed class NendAdNativeText : Text, IValidatable
	{
		private ValidationDelegate <NendAdNativeText> m_Delegate;

		public NendAdNativeText ()
		{
			m_Delegate = new ValidationDelegate <NendAdNativeText> (this);
		}

		public override string text {
			get {
				return base.text;
			}
			set {
				Log.W ("Not Supported.");
			}
		}

		internal string TextInternal {
			set {
				base.text = value;
			}
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
