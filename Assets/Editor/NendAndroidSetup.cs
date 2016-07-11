using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Linq;

namespace NendUnityPlugin
{
	public class NendAndroidSetup : EditorWindow
	{
		static bool debuggable = false;
		static bool skipGmsProcess = false;

		private const string AndroidSDKRoot = "AndroidSdkRoot";
		private const string GmsDirectoryPath = "extras/google/m2repository/com/google/android/gms";
		private const string GmsArtifactName = "play-services-basement";
		private const string AndroidLibraryDirectoryPath = "Plugins/Android";

		[MenuItem ("NendSDK/Android Setup", false, 1)]
		public static void MenuItemAndroidSetup ()
		{
			NendAndroidSetup window = (NendAndroidSetup)EditorWindow.GetWindow (typeof(NendAndroidSetup));
			var vec2 = new Vector2 (200, 100);
			window.minSize = vec2;
			window.maxSize = vec2;
			window.Show ();
		}

		void OnGUI ()
		{
			skipGmsProcess = EditorGUILayout.Toggle ("Skip Google Play Services", skipGmsProcess);
			debuggable = EditorGUILayout.Toggle ("Debuggable", debuggable);
			if (GUILayout.Button ("Configure")) {
				DoSetup ();
			}
		}

		public static void DoSetup ()
		{
			Debug.Log ("Processing...");
			CreateAndroidLibraryDirectoryIfNeeded ();
			if (!skipGmsProcess) {
				AddGooglePlayServicesLibrary ();
			}
			ConfigureAndroidManifest ();
			AssetDatabase.Refresh ();
			EditorWindow.GetWindow (typeof(NendAndroidSetup)).Close ();
			Debug.Log ("Done!");
		}
			
		private static string ToPlatformDirectorySeparator (string path)
		{
			return path.Replace ("/", Path.DirectorySeparatorChar.ToString ());
		}

		private static void CreateAndroidLibraryDirectoryIfNeeded ()
		{
			string path = Path.Combine (Application.dataPath, ToPlatformDirectorySeparator (AndroidLibraryDirectoryPath));
			if (!Directory.Exists (path)) {
				Directory.CreateDirectory (path);
				Debug.Log ("Created: 'Plugins/Android' directory.");
			} else {
				Debug.Log ("'Plugins/Android' directory is already exist.");
			}
		}

		private static void AddGooglePlayServicesLibrary ()
		{
			string androidSDKPath = EditorPrefs.GetString (AndroidSDKRoot, null);
			if (string.IsNullOrEmpty (androidSDKPath)) {
				Debug.LogWarning ("AndroidSdkRoot is not setup.");
				return;
			}
			Debug.Log ("AndroidSDK path: " + androidSDKPath);
			if (!Directory.Exists (androidSDKPath)) {
				Debug.LogWarning ("AndroidSDK is not installed.");
				return;
			}
			string gmsPath = Path.Combine (androidSDKPath, ToPlatformDirectorySeparator (GmsDirectoryPath));
			if (!Directory.Exists (gmsPath)) {
				Debug.LogWarning ("The Google Play services library is not installed.");
				return;
			}

			string libraryDirectoryPath = Path.Combine (Application.dataPath, ToPlatformDirectorySeparator (AndroidLibraryDirectoryPath));
			string[] archives = Directory.GetFiles (libraryDirectoryPath, "play-services-basement-?.?.?.aar");
			if (null != archives && 0 < archives.Length) {
				Debug.Log ("The play-services-basement is already exist.");
				return;
			}
			string artifactPath = Path.Combine (androidSDKPath, Path.Combine (GmsDirectoryPath, GmsArtifactName));
			var directoryInfo = new DirectoryInfo (artifactPath);
			if (directoryInfo.Exists) {
				DirectoryInfo[] infos = directoryInfo.GetDirectories ("?.?.?");
				if (null == infos || 0 == infos.Length) {
					Debug.LogWarning ("The Google Play services library is not installed.");
					return;
				}
				var max = infos
					.Select (di => di.Name)
					.Aggregate ((current, next) => {
						int currentVersion = int.Parse (current.Replace (".", ""));
						int nextVersion = int.Parse (next.Replace (".", ""));
						return nextVersion > currentVersion ? next : current;
					});
				string archiveName = string.Format ("play-services-basement-{0}.aar", max);
				string aarPathFrom = Path.Combine (artifactPath, Path.Combine (max, archiveName));
				string aarPathTo = Path.Combine (libraryDirectoryPath, archiveName);
				FileUtil.CopyFileOrDirectory (aarPathFrom, aarPathTo);
				Debug.Log ("Added: " + aarPathTo);
			} else {
				Debug.LogWarning ("The Google Play services library is not installed.");
			}
		}

		private static void ConfigureAndroidManifest ()
		{
			string manifestPathDest = Path.Combine (Application.dataPath, ToPlatformDirectorySeparator ("Plugins/Android/AndroidManifest.xml"));
			if (!File.Exists (manifestPathDest)) {
				if (!debuggable) {
					Debug.Log ("There is no need to change the AndroidManifest.");
					return;
				}

				string[] UnityAndroidManifestPathList = {
					Path.Combine (EditorApplication.applicationPath, ToPlatformDirectorySeparator ("../PlaybackEngines/AndroidPlayer/Apk/AndroidManifest.xml")),
					Path.Combine (EditorApplication.applicationContentsPath, ToPlatformDirectorySeparator ("PlaybackEngines/AndroidPlayer/Apk/AndroidManifest.xml")),
					Path.Combine (EditorApplication.applicationContentsPath, ToPlatformDirectorySeparator ("PlaybackEngines/AndroidPlayer/AndroidManifest.xml"))
				};
					
				string defaultManifestPath = null;
				foreach (string path in UnityAndroidManifestPathList) {
					if (File.Exists (path)) {
						defaultManifestPath = path;
						Debug.Log ("Found AndroidManifest at " + path);
						break;
					}
				}
				if (null == defaultManifestPath) {
					Debug.LogWarning ("Couldn't find default AndroidManifest.");
					return;
				}
				FileUtil.CopyFileOrDirectory (defaultManifestPath, manifestPathDest);
			} else {
				Debug.Log ("The AndroidManifest is already exist.");
			}
		
			XmlDocument doc = new XmlDocument ();
			doc.Load (manifestPathDest);
		
			XmlNode applicationNode = doc.SelectSingleNode ("manifest/application");
			if (null == applicationNode) {
				Debug.LogWarning ("The application tag is not found.");
				return;
			}
		
			string ns = applicationNode.GetNamespaceOfPrefix ("android");
			XmlNamespaceManager nsManager = new XmlNamespaceManager (doc.NameTable);
			nsManager.AddNamespace ("android", ns);
				
			XmlNode nendDebuggableNode = applicationNode.SelectSingleNode (@"//meta-data[@android:name='NendDebuggable']", nsManager);
			if (null != nendDebuggableNode) {
				XmlElement element = (XmlElement)nendDebuggableNode;
				element.SetAttribute ("value", ns, debuggable.ToString ().ToLower ());
				Debug.Log ("Modified: " + element.OuterXml);
			} else if (debuggable) { 
				XmlElement element = CreateNendDebuggableElement (doc, ns);
				applicationNode.AppendChild (element);
				Debug.Log ("Added: " + element.OuterXml);
			} else {
				Debug.Log ("There is no need to create a NendDebuggable element.");
			}
			doc.Save (manifestPathDest);
		}
			
		private static XmlElement CreateNendDebuggableElement (XmlDocument doc, string ns)
		{
			XmlElement element = doc.CreateElement ("meta-data");
			element.SetAttribute ("name", ns, "NendDebuggable");
			element.SetAttribute ("value", ns, "true");
			return element;
		}
	}
}