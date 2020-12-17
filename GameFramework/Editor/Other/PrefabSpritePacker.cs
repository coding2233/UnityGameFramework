using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace Wanderer.GameFramework
{

    public class PrefabSpritePacker
    {
        private static SpriteAtlasPackingSettings _packingSettings = new SpriteAtlasPackingSettings();
        private static TextureImporterPlatformSettings _androidSettings = new TextureImporterPlatformSettings();
        private static TextureImporterPlatformSettings _iOSSettings = new TextureImporterPlatformSettings();

        [MenuItem("Assets/Prefab To SpriteAtlas")]
        private static void PrefabToSpriteAtlas()
        {
            if (Selection.objects == null)
                return;

            bool selectFolder = false;

            if (Selection.assetGUIDs.Length == 1)
            {
                string selectPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
                if (AssetDatabase.IsValidFolder(selectPath))
                {
                    selectFolder = true;

                    var assets = AssetDatabase.FindAssets("t:Prefab", new string[] { selectPath });
					if (assets != null)
					{
						HashSet<Object> objects = new HashSet<Object>();
						foreach (var item in assets)
						{
							string assetPath = AssetDatabase.GUIDToAssetPath(item);
							Debug.Log(assetPath);
							foreach (var item02 in GameObjectGetDependencies(AssetDatabase.LoadAssetAtPath<GameObject>(assetPath)))
							{
								objects.Add(item02);
							}
						}
						string fileName = Path.GetFileNameWithoutExtension(selectPath);
						SaveSpriteAtlas(objects, $"{selectPath}/{fileName}_spriteatlas.spriteatlas");
					}
                    
                }
            }
            //没有选择文件夹
            if (!selectFolder)
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

        [MenuItem("Assets/SpriteAtlas Global Setting")]
        private static void SpriteAtlasGlobalSetting()
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
                        SetSpriteAtlas(spriteAtlas);
                    }
                }
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// GameObject设置SpriteAtlas
        /// </summary>
        /// <param name="go"></param>
        private static void GameObjectToSpriteAtlas(GameObject go)
        {
            string assetPath = AssetDatabase.GetAssetPath(go);
            HashSet<Object> objects = new HashSet<Object>();
			foreach (var item in GameObjectGetDependencies(go))
			{
                objects.Add(item);
            }
            SaveSpriteAtlas(objects, $"{assetPath}_spriteatlas.spriteatlas");
        }

        /// <summary>
        /// 获取所有的引用
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        private static List<Object> GameObjectGetDependencies(GameObject go)
        {
            string assetPath = AssetDatabase.GetAssetPath(go);
            string[] dependencies = AssetDatabase.GetDependencies(assetPath);
            if (dependencies != null)
            {
                List<Object> depObjects = new List<Object>();
                foreach (var item in dependencies)
                {
                    // string _otherGUID = AssetDatabase.AssetPathToGUID(item);
                    var itemObject = AssetDatabase.LoadAssetAtPath<Object>(item);
                    if (itemObject is Sprite || itemObject is Texture2D)
                    {
                        if (itemObject != null)
                        {
                            depObjects.Add(itemObject);
                        }
                    }
                }
                return depObjects;
            }
            return new List<Object>();
        }


        /// <summary>
        /// 保存SpriteAtlas
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="saPath"></param>
        public static void SaveSpriteAtlas(HashSet<Object> objs, string saPath)
        {
            if (objs == null || objs.Count == 0)
                return;

            // SpriteAtlasUtility.
            Object[] objArray = new Object[objs.Count];
            int index = 0;
			foreach (var item in objs)
			{
                objArray[index] = item;
                index++;
            }
            SpriteAtlas spriteAtlas = new SpriteAtlas();
            spriteAtlas.Add(objArray);
            SetSpriteAtlas(spriteAtlas);
            AssetDatabase.CreateAsset(spriteAtlas, saPath);
            AssetDatabase.Refresh();
        }

   

        /// <summary>
        /// 设置图集参数
        /// </summary>
        /// <param name="spriteAtlas"></param>
        private static void SetSpriteAtlas(SpriteAtlas spriteAtlas)
        {
            _packingSettings.enableTightPacking = false;
            _packingSettings.enableRotation = false;
            _packingSettings.padding = 4;

            _androidSettings.name = "Android";
            _androidSettings.overridden = true;
           // androidSettings.maxTextureSize = 2048;
          //  _androidSettings.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
            _androidSettings.format = TextureImporterFormat.ETC2_RGBA8;

            _iOSSettings.name = "iOS";
            _iOSSettings.overridden = true;
            //iOSSettings.maxTextureSize = 2048;
           // _iOSSettings.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
            _iOSSettings.format = TextureImporterFormat.PVRTC_RGBA4;

            spriteAtlas.SetIncludeInBuild(false);
            spriteAtlas.SetPackingSettings(_packingSettings);
            spriteAtlas.SetPlatformSettings(_androidSettings);
            spriteAtlas.SetPlatformSettings(_iOSSettings);
        }

    }
}
