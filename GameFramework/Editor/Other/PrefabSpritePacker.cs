using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace Wanderer.GameFramework
{

    public class PrefabSpritePacker
    {
        [MenuItem("Assets/Prefab To SpriteAtlas")]
        private static void PrefabToSpriteAtlas()
        {
			string selectPath = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (AssetDatabase.IsValidFolder(selectPath))
			{
				var assets = AssetDatabase.FindAssets("t:Prefab", new string[] { selectPath });
				if (assets != null)
				{
                    HashSet<Object> objects = new HashSet<Object>();
                    foreach (var item in assets)
					{
                        string assetPath = AssetDatabase.GUIDToAssetPath(item);
						foreach (var item02 in GameObjectGetDependencies(AssetDatabase.LoadAssetAtPath<GameObject>(assetPath)))
						{
                            objects.Add(item02);
                        }
					}
                    SaveSpriteAtlas(objects, $"{selectPath}/SpriteAtlas.spriteatlas");
				}
			}
			else
			{
                GameObject[] go = Selection.gameObjects;
                if (go == null)
                    return;
                foreach (GameObject g in go)
                {
                    GameObjectToSpriteAtlas(g);
                }
            }
        }


        [MenuItem("Assets/SpriteAtlas Not In Build")]
        private static void SpriteAtlasNotInBuild()
        {
            var assets = AssetDatabase.FindAssets("t:SpriteAtlas");
            if (assets != null)
            {
                foreach (var item in assets)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(item);
                    SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(assetPath);
                    if (spriteAtlas != null)
                    {
                        spriteAtlas.SetIncludeInBuild(false);
                    }
                }
                AssetDatabase.Refresh();
            }
        }


        private static void GameObjectToSpriteAtlas(GameObject go)
        {
            string assetPath = AssetDatabase.GetAssetPath(go);
            HashSet<Object> objects = new HashSet<Object>();
			foreach (var item in GameObjectGetDependencies(go))
			{
                objects.Add(item);
            }
            SaveSpriteAtlas(objects, $"{assetPath}.spriteatlas");
        }

        private static List<Object> GameObjectGetDependencies(GameObject go)
        {
            string assetPath = AssetDatabase.GetAssetPath(go);
            string[] dependencies = AssetDatabase.GetDependencies(assetPath);
            if (dependencies != null)
            {
                List<Object> depSprites = new List<Object>();
                foreach (var item in dependencies)
                {
                    // string _otherGUID = AssetDatabase.AssetPathToGUID(item);
                    var itemSprite = AssetDatabase.LoadAssetAtPath<Object>(item);
                    if (itemSprite is Sprite || itemSprite is Texture2D)
                    {
                        depSprites.Add(itemSprite);
                    }
                }
                return depSprites;
            }
            return new List<Object>();
        }


        public static void SaveSpriteAtlas(HashSet<Object> objs, string saPath)
        {
            if (objs == null || objs.Count == 0)
                return;
		
            TextureImporterPlatformSettings androidSettings = new TextureImporterPlatformSettings();
            androidSettings.name = "Android";
            androidSettings.overridden = true;
            androidSettings.maxTextureSize = 4096;
            androidSettings.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
            androidSettings.format = TextureImporterFormat.ETC2_RGBA8;

            TextureImporterPlatformSettings iOSSettings = new TextureImporterPlatformSettings();
            iOSSettings.name = "iOS";
            iOSSettings.overridden = true;
            iOSSettings.maxTextureSize = 4096;
            iOSSettings.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
            iOSSettings.format = TextureImporterFormat.PVRTC_RGBA4;

            // SpriteAtlasUtility.
            Object[] objArray = new Object[objs.Count];
            int index = 0;
			foreach (var item in objs)
			{
                objArray[index] = item;
                index++;
            }
                 SpriteAtlas spriteAtlas = new SpriteAtlas();
                spriteAtlas.SetIncludeInBuild(false);
                spriteAtlas.Add(objArray);

                spriteAtlas.SetPlatformSettings(androidSettings);
                // spriteAtlas.SetPlatformSettings(iOSSettings);

                AssetDatabase.CreateAsset(spriteAtlas, saPath);
                AssetDatabase.Refresh();

                spriteAtlas.SetPlatformSettings(iOSSettings);
                AssetDatabase.Refresh();
                // int result = spriteAtlas.GetSprites(depSprites.ToArray());
                // Debug.Log($"GetSprites:{result}");
        }


        //static void AddPackAtlas(SpriteAtlas atlas, Object[] spt)
        //{
        //    MethodInfo methodInfo = System.Type
        //         .GetType("UnityEditor.U2D.SpriteAtlasExtensions, UnityEditor")
        //         .GetMethod("Add", BindingFlags.Public | BindingFlags.Static);
        //    if (methodInfo != null)
        //        methodInfo.Invoke(null, new object[] { atlas, spt });
        //    else
        //        Debug.Log("methodInfo is null");
        //  //  PackAtlas(atlas);
        //}

        //static void PackAtlas(SpriteAtlas atlas)
        //{
        //    System.Type
        //        .GetType("UnityEditor.U2D.SpriteAtlasUtility, UnityEditor")
        //        .GetMethod("PackAtlases", BindingFlags.NonPublic | BindingFlags.Static)
        //        .Invoke(null, new object[] { new[] { atlas }, EditorUserBuildSettings.activeBuildTarget });
        //}

    }
}
