namespace NendUnityPlugin.Platform
{
	internal interface INativeAdClient
	{
		void LoadNativeAd (System.Action<INativeAd, int, string> callback);
	}
}