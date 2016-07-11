namespace NendUnityPlugin.AD.Native.Utils
{
	/// <summary>
	/// Logger.
	/// </summary>
	public class NendAdLogger
	{
		/// <summary>
		/// Log level.
		/// </summary>
		public enum NendAdLogLevel : int
		{
			/// <summary>
			/// Output more than debug log.
			/// </summary>
			Debug = 0,
			/// <summary>
			/// Output more than warning log.
			/// </summary>
			Warn = 1,
			/// <summary>
			/// Output more than errpr log.
			/// </summary>
			Error = 2,
			/// <summary>
			/// Does not output the log.
			/// </summary>
			None = int.MaxValue
		}

		private static NendAdLogLevel s_LogLevel = NendAdLogLevel.None;

		/// <summary>
		/// Sets the log level.
		/// </summary>
		/// <value>The log level.</value>
		public static NendAdLogLevel LogLevel {
			set {
				s_LogLevel = value;
			}
		}

		internal static void D (string format, params object[] args)
		{
			Log (NendAdLogLevel.Debug, format, args);
		}

		internal static void W (string format, params object[] args)
		{
			Log (NendAdLogLevel.Warn, format, args);
		}

		internal static void E (string format, params object[] args)
		{
			Log (NendAdLogLevel.Error, format, args);
		}

		private static void Log (NendAdLogLevel level, string format, params object[] args)
		{
			if (level >= s_LogLevel) {
				switch (level) {
				case NendAdLogLevel.Debug:
					#if UNITY_5
					UnityEngine.Debug.LogFormat (format, args);
					#else
					UnityEngine.Debug.Log (string.Format (format, args));
					#endif
					break;
				case NendAdLogLevel.Warn:
					#if UNITY_5
					UnityEngine.Debug.LogWarningFormat (format, args);
					#else
					UnityEngine.Debug.LogWarning (string.Format (format, args));
					#endif
					break;
				case NendAdLogLevel.Error:
					#if UNITY_5
					UnityEngine.Debug.LogErrorFormat (format, args);
					#else
					UnityEngine.Debug.LogError (string.Format (format, args));
					#endif
					break;
				}
			}
		}
	}
}

