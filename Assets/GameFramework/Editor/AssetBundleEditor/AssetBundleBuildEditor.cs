using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class AssetBundleBuildEditor :EditorWindow {

	private static string _configPath="ProjectSettings/AssetBundleEditorConifg.json";
	private static AssetBundleConifgInfo _config;
	private static List<string> _buildTargets;
	private static string _outPutPath="";

	private Vector2 _scrollViewPos;

	[MenuItem("Tools/AssetBundles Options")]
	public static void AssetBundilesOptions()
	{
		LoadConfig();

		GetWindowWithRect<AssetBundleBuildEditor>(new Rect(Screen.width/2,Screen.height/2,400,30));
	}

	[MenuItem("Tools/Build AssetBundles")]
	public static void BuildAssetBundles()
	{
		LoadConfig();

		BuildPipeline.BuildAssetBundles(_config.BuildPath,BuildAssetBundleOptions.None,EditorUserBuildSettings.activeBuildTarget);
	}

	[MenuItem("Tools/Build AssetBundles All Targets")]
	public static void BuildAssetBundlesAllTargets()
	{
		LoadConfig();

		//List<BuildTarget> targets=new List<BuildTarget>();
		for(int i=0;i<_config.BuildTargets.Count;i++)
		{
			BuildTarget target= (BuildTarget)_config.BuildTargets[i];
			string buildPath=Path.Combine(_config.BuildPath,target.ToString());
			BuildPipeline.BuildAssetBundles(buildPath,BuildAssetBundleOptions.None,target);
		}
		
	}

	/// <summary>
	/// OnGUI is called for rendering and handling GUI events.
	/// This function can be called multiple times per frame (one call per event).
	/// </summary>
	void OnGUI()
	{
		if (_config==null)
		{
			return;
		}

		GUILayout.BeginVertical("HelpBox");

		GUILayout.BeginHorizontal("Box");
		GUILayout.Label("Version");
		GUILayout.Label(_config.Version.ToString());
		if(GUILayout.Button("RESET",GUILayout.Width(60)))
		{

		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal("Box");
		GUILayout.Label("BuildPath");
		GUILayout.Label(_config.BuildPath);

		if(GUILayout.Button("BROWSE",GUILayout.Width(60)))
		{

		}
		GUILayout.EndHorizontal();
		
		_scrollViewPos=GUILayout.BeginScrollView(_scrollViewPos,"Box");
		string[] targets= Enum.GetNames(typeof(BuildTarget));
		for(int i=0;i<targets.Length;i++)
		{
			GUILayout.Toggle(false,targets[i]);
		}
		GUILayout.EndScrollView();

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("OK",GUILayout.Width(60)))
		{

		}
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
	}

	//加载配置信息
	private static void LoadConfig()
	{

		if (!File.Exists(_configPath))
		{
			File.WriteAllText(_configPath,JsonUtility.ToJson(new AssetBundleConifgInfo()));
		}

		_config=JsonUtility.FromJson<AssetBundleConifgInfo>(File.ReadAllText(_configPath));
		// _config.Targets=new List<BuildTarget>();
		// for(int i=0;i<_config.BuildTargets.Count;i++)
		// {
		// 	_config.Targets.Add((BuildTarget)_config.BuildTargets[i]);
		// }
	}




	//ab包的配置文件信息
	[System.Serializable]
	public class AssetBundleConifgInfo
	{
		public int Version=-1;
		public string BuildPath="";
		public List<int> BuildTargets=new List<int>();

	}
		
}
