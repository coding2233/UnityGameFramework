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
	private static string _rootPath;

	private static readonly Dictionary<BuildTarget,bool> _allTargets=new Dictionary<BuildTarget, bool>();

	[MenuItem("Tools/AssetBundles Options %#O")]
	public static void AssetBundilesOptions()
	{
		_rootPath=Path.GetDirectoryName(Application.dataPath);

		LoadConfig();

		BuildTarget[] allTargets= (BuildTarget[])Enum.GetValues(typeof(BuildTarget));
		//_allTargets
		foreach (var item in allTargets)
		{
			int index=(int)item;
			if(_config.BuildTargets.Contains(index))
				_allTargets[item]=true;
			else
				_allTargets[item]=false;
		}

		GetWindowWithRect<AssetBundleBuildEditor>(new Rect(200,300,400,300),false,"Options");
	}

	[MenuItem("Tools/Build AssetBundles %#T")]
	public static void BuildAssetBundles()
	{
		LoadConfig();

		BuildTarget target=EditorUserBuildSettings.activeBuildTarget;
		//打包编辑器的激活平台
		BuildTarget(target);

		//保存平台信息
		SavePlatformVersion(new List<BuildTarget>(){target});
	}

	[MenuItem("Tools/Build AssetBundles Targets %#Y")]
	public static void BuildAssetBundlesAllTargets()
	{
		LoadConfig();

		List<BuildTarget> targets=new List<BuildTarget>();
		for(int i=0;i<_config.BuildTargets.Count;i++)
		{
			BuildTarget target= (BuildTarget)_config.BuildTargets[i];
			BuildTarget(target);
			targets.Add(target);
		}

		//保存平台信息
		SavePlatformVersion(targets);
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
		GUILayout.Label("Version:");
		GUILayout.Label(_config.Version.ToString());
		if(GUILayout.Button("RESET",GUILayout.Width(60)))
		{
			_config.Version=0;
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal("Box");
		GUILayout.Label("BuildPath:");
		GUILayout.TextArea(string.IsNullOrEmpty(_config.BuildPath)?_rootPath:_config.BuildPath);

		if(GUILayout.Button("BROWSE",GUILayout.Width(60)))
		{
			string path=(string.IsNullOrEmpty(_config.BuildPath)||!Directory.Exists(_config.BuildPath))?_rootPath:_config.BuildPath;
		
			path=EditorUtility.OpenFolderPanel("Build Path",path,"");
			if(!string.IsNullOrEmpty(path))
			{
				if(path.Contains(_rootPath))
				{
					path=path.Replace(_rootPath,"");
					if(path.IndexOf("/")==0)
					{
						path= path.Substring(1,path.Length-1);
					}
				}
				_config.BuildPath=path;
			}
			return;
		}
		GUILayout.EndHorizontal();
		
		_scrollViewPos=GUILayout.BeginScrollView(_scrollViewPos,"Box");
		foreach (var item in _allTargets)
		{
			bool value=GUILayout.Toggle(item.Value,item.Key.ToString());
			if(value!=item.Value)
			{
				_allTargets[item.Key]=value;
				break;
			}
		}
		GUILayout.EndScrollView();

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("OK",GUILayout.Width(60)))
		{
			//保存配置文件
			SaveConfig();
			//关闭窗口
			Close();
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
	}

	//保存配置信息
	private static void SaveConfig()
	{
		_config.BuildTargets=new List<int>();
		foreach(var item in _allTargets)
		{
			if (item.Value)
			{
				_config.BuildTargets.Add((int)item.Key);
			}
		}
		string json=JsonUtility.ToJson(_config);
		File.WriteAllText(_configPath,json);
	}

	//资源打包
	private static void BuildTarget(BuildTarget target,BuildAssetBundleOptions options=BuildAssetBundleOptions.None)
	{
		string buildPath=Path.Combine(_config.BuildPath,target.ToString());
		if(!Directory.Exists(buildPath))
				Directory.CreateDirectory(buildPath);
		BuildPipeline.BuildAssetBundles(buildPath,options,target);

		//保存资源版本信息
		SaveAssetVersion(buildPath,target);
	}

	//保存资源版本信息
	private static void SaveAssetVersion(string buildPath,BuildTarget target)
	{
		AssetVersionInfo assetVersionInfo=new  AssetVersionInfo();
		assetVersionInfo.Version=_config.Version;
		assetVersionInfo.AssetHashInfos=new List<AssetHashInfo>();

		AssetBundle targetBundle=AssetBundle.LoadFromFile(Path.Combine(buildPath,target.ToString()));
		AssetBundleManifest manifest=targetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
		string[] assetNames=manifest.GetAllAssetBundles();
		for (int i = 0; i < assetNames.Length; i++)
		{
			AssetHashInfo assetHashInfo=new AssetHashInfo();
			assetHashInfo.Name=assetNames[i];
			assetHashInfo.Hash=manifest.GetAssetBundleHash(assetNames[i]).ToString();
			assetVersionInfo.AssetHashInfos.Add(assetHashInfo);
		}

		string json=JsonUtility.ToJson(assetVersionInfo);
		File.WriteAllText(Path.Combine(buildPath,"version.txt"),json);
		targetBundle.Unload(true);
	}

	//保存平台版本信息
	private static void SavePlatformVersion(List<BuildTarget> targets)
	{
		PlatformVersionInfo platformInfo=new PlatformVersionInfo();
		platformInfo.Version=_config.Version;
		platformInfo.Platforms=new List<string>();
		foreach (var item in targets)
		{
			platformInfo.Platforms.Add(item.ToString());
		}
		string json=JsonUtility.ToJson(platformInfo);
		//保存平台信息
		File.WriteAllText(Path.Combine(_config.BuildPath,"version.txt"),json);
		//更新资源版本号 -- 保存配置文件
		_config.Version++;
		SaveConfig();
		//打开文件夹
		EditorUtility.OpenWithDefaultApp(_config.BuildPath);
	}

	//ab包的配置文件信息
	[System.Serializable]
	public class AssetBundleConifgInfo
	{
		public int Version=0;
		public string BuildPath="";
		public List<int> BuildTargets=new List<int>();

	}

	//平台资源信息
	[System.Serializable]
	public class PlatformVersionInfo
	{
		public int Version;
		public List<string> Platforms=new List<string>();
	}

	//资源版本信息
	[System.Serializable]
	public class AssetVersionInfo
	{
		public int Version=0;
		public List<AssetHashInfo> AssetHashInfos=new List<AssetHashInfo>();
	}
	
	//资源hash值
	[System.Serializable]
	public class AssetHashInfo
	{
		public string Name;
		public string Hash;
	}

	
}
