namespace NendUnityPlugin.Common
{
	using System;

	public class NendNativeAdErrorEventArgs : EventArgs
	{
		/// <summary>
		/// Error code.
		/// </summary>
		/// \sa NendUnityPlugin.Common.NendNativeErrorCode
		public NendNativeErrorCode ErrorCode { get; set; }

		/// <summary>
		/// Error message.
		/// </summary>
		public String Message { get; set; }
	}
}