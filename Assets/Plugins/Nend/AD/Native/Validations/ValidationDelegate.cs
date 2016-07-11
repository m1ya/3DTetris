namespace NendUnityPlugin.AD.Native.Validations
{
	using System;
	using System.Linq;
	using UnityEngine;
	using UnityEngine.UI;

	internal class ValidationDelegate <T> where T : Graphic, IValidatable
	{
		private readonly T m_Target;
		private IValidator[] m_Validators;
		private Action<IValidatable> m_DirtyCallback;
		private bool m_IsRegisteredCallback = false;

		internal ValidationDelegate (T target)
		{
			m_Target = target;
		}

		internal void BeginValidation (IValidator[] validators, Action<IValidatable> dirtyCallback)
		{
			m_Validators = validators;
			m_DirtyCallback = dirtyCallback;
			if (!m_IsRegisteredCallback) {
				m_Target.RegisterDirtyLayoutCallback (OnDirtyLayout);
				m_Target.RegisterDirtyVerticesCallback (OnDirtyLayout);
				m_IsRegisteredCallback = true;
			}
		}

		internal void StopValidation ()
		{
			m_Validators = null;
			m_DirtyCallback = null;
			if (m_IsRegisteredCallback) {
				m_Target.UnregisterDirtyLayoutCallback (OnDirtyLayout);
				m_Target.UnregisterDirtyVerticesCallback (OnDirtyLayout);
				m_IsRegisteredCallback = false;
			}
		}

		internal bool Validate ()
		{
			if (null != m_Validators && 0 < m_Validators.Length) {
				return m_Validators.All (v => v.Validate (m_Target));
			} else {
				return true;
			}
		}

		internal void FireDirtyCallback ()
		{
			OnDirtyLayout ();
		}

		#region DirtyLayoutCallback

		private void OnDirtyLayout ()
		{
			if (null != m_DirtyCallback) {
				m_DirtyCallback (m_Target);
			}
		}

		#endregion
	}
}

