using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class PlatformBuild : EditorWindow {

	private string bundleVersion = "";
	private int bundleVersionCode = 0;
	private string bundleIdentifier = "";
	private string iconPath = "";
	private string srPluginPath = "";
	private string apkPath = "";
	
	private int	mLines = 0;
	private const int COLUMN_COUNT = 5;

	private List<SDKConfigData> mListSDKConfig = new List<SDKConfigData> ();

	[MenuItem("BuildAPK/Platform Window")]
	static void Init(){
		EditorWindow.GetWindow<PlatformBuild>(true, "Platform Window");
	}

	private void LoadConfig(){
		mListSDKConfig.Clear();

		XMLParser xml = XMLParser.Create(Application.dataPath + "/SDKBuildConfig.xml");
		if(xml != null){
			XmlNode path = xml.GetSingleNode("pluginPath");
			srPluginPath = (path as XmlElement).GetAttribute("content");

			path = xml.GetSingleNode("apkPath");
			apkPath = (path as XmlElement).GetAttribute("content");

			XmlNode node = xml.GetSingleNode("channels");
			XmlNodeList channelNodeS = node.SelectNodes("channel");
			foreach(XmlNode data in channelNodeS){
				SDKConfigData config = new SDKConfigData();
				config.Channel = (data.SelectSingleNode("name") as XmlElement).GetAttribute("content");
				config.BundleID = (data.SelectSingleNode("bundleId") as XmlElement).GetAttribute("content");
				config.Version = (data.SelectSingleNode("version") as XmlElement).GetAttribute("content");
				config.VersionCode = (data.SelectSingleNode("versionCode") as XmlElement).GetAttribute("content");
				mListSDKConfig.Add(config);
			}
		}

		mLines = (mListSDKConfig.Count%COLUMN_COUNT == 0)?(mListSDKConfig.Count/COLUMN_COUNT):(mListSDKConfig.Count/COLUMN_COUNT + 1);
	}

	private void OnGUI()
	{
		GUILayout.Space (20);
		GUILayout.BeginHorizontal();
		GUILayout.Label ("点击LoadConfig, 加载SDK配置", GUILayout.Height(20f));
		if (GUILayout.Button("LoadConfig", GUILayout.Width(100f), GUILayout.Height(20f))){
			LoadConfig();
		}
		GUILayout.EndHorizontal();

		GUILayout.Space (20f);

		for (int i = 0; i < mLines; i++) {
			Rect rec = new Rect(50f, 50f*(i + 1), 500f, 60f);
			GUILayout.BeginArea(rec);
			GUILayout.BeginHorizontal ();
			for(int j = 0; j < COLUMN_COUNT; j++){
				if((j + i*COLUMN_COUNT) > (mListSDKConfig.Count - 1)){
					break;
				}else{
					if (GUILayout.Button(mListSDKConfig[j + i*COLUMN_COUNT].Channel, GUILayout.Width(100f))){
						SDKConfigData data = mListSDKConfig[j + i*COLUMN_COUNT];
						PlatformTag(data.Channel, data.BundleID, data.Version, data.VersionCode);
					}
				}
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndArea();
		}
	}

	void PlatformTag(string channel, string bundleId, string version, string versionCode){
		apkPath += channel + ".apk";
		srPluginPath += channel + "/Android/";

		bundleIdentifier = bundleId;
		bundleVersion = version;
		int.TryParse (versionCode, out bundleVersionCode);

		switch (channel) {
		case "XiaoYou":
			BuildForXiaoYou();	
			break;
		default:
			break;
		}
	}

	void BuildForXiaoYou(){
		iconPath = "Assets/Icon/Android/appicon_36.png";
		GenericBuild ();
	}

	void GenericBuild(){
		PlayerSettings.bundleIdentifier = bundleIdentifier;
		PlayerSettings.bundleVersion = bundleVersion;
		PlayerSettings.Android.bundleVersionCode = bundleVersionCode;

		Texture2D[] icons = new Texture2D[1];
		icons [0] = AssetDatabase.LoadMainAssetAtPath (iconPath) as Texture2D;
		PlayerSettings.SetIconsForTargetGroup (BuildTargetGroup.Android, icons);

		DirectoryInfo resDirect = Directory.CreateDirectory (srPluginPath);
		if (resDirect != null) {
			//delete plugins android
			FileOption file = new FileOption();
			file.DeletePathFiles(Application.dataPath + "/Plugins/Android/");

			//copy plugins for android
			file.CopyFolder(srPluginPath, Application.dataPath + "/Plugins/Android/");
		} else {
			Debug.LogError(srPluginPath + " is not Existed");
		}

		//delete old apk
		if (File.Exists (apkPath)) {
			File.Delete(apkPath);
		}

		string res = BuildPipeline.BuildPlayer (FindEnabledEditorScenes (), apkPath, BuildTarget.Android, BuildOptions.None);
		if (res.Length > 0) {
			Debug.LogError("Build Player Error:---" + res);
		}

		EditorWindow.GetWindow<PlatformBuild> ().Repaint ();
	}

	string[] FindEnabledEditorScenes(){
		List<string> editorScenes = new List<string> ();
		foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
			if(!scene.enabled)
				continue;
			editorScenes.Add(scene.path);
		}

		return editorScenes.ToArray ();
	}
}
