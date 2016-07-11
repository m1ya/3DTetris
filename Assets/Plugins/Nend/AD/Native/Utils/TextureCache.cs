namespace NendUnityPlugin.AD.Native.Utils
{
	using System.Collections.Generic;
	using UnityEngine;

	using Log = NendUnityPlugin.AD.Native.Utils.NendAdLogger;

	internal class TextureCache
	{
		private static TextureCache s_Instance = null;
		private Dictionary<string, Texture2D> m_Cache;

		internal static TextureCache Instance {
			get {
				return s_Instance ?? (s_Instance = new TextureCache ());
			}
		}

		private TextureCache ()
		{
			m_Cache = new Dictionary<string, Texture2D> ();
		}

		internal Texture2D Get (string url)
		{
			if (m_Cache.ContainsKey (url)) {
				Log.D ("Get texture from cache.");
				return m_Cache [url];
			} else {
				return null;
			}
		}

		internal void Put (string url, Texture2D texture)
		{
			if (null != texture) {
				m_Cache [url] = texture;
			}
		}
	}
}

