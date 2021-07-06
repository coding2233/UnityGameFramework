using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
	public static class ResourceManagerExtension
	{
		//Object
		public static void LoadObject(this ResourceManager resource, string assetPath, Action<UnityEngine.Object> callback)
		{
			resource.Asset.LoadAsset<UnityEngine.Object>(assetPath, callback);
		}

		public static UnityEngine.Object LoadObjectSync(this ResourceManager resource, string assetPath)
		{
			return resource.Asset.LoadAsset<UnityEngine.Object>(assetPath);
		}

		//GameObject
		public static void LoadGameObject(this ResourceManager resource, string assetPath, Action<GameObject> callback)
		{
			resource.Asset.LoadAsset<GameObject>(assetPath, callback);
		}

		public static GameObject LoadGameObjectSync(this ResourceManager resource, string assetPath)
		{
			return resource.Asset.LoadAsset<GameObject>(assetPath);
		}

		//Texture
		public static void LoadTextAsset(this ResourceManager resource, string assetPath, Action<TextAsset> callback)
		{
			resource.Asset.LoadAsset<TextAsset>(assetPath, callback);
		}

		public static TextAsset LoadTextAssetSync(this ResourceManager resource, string assetPath)
		{
			return resource.Asset.LoadAsset<TextAsset>(assetPath);
		}
		
		//Sprite
		public static void LoadSprite(this ResourceManager resource, string assetPath, Action<Sprite> callback)
		{
			resource.Asset.LoadAsset<Sprite>(assetPath, callback);
		}

		public static Sprite LoadSpriteSync(this ResourceManager resource, string assetPath)
		{
			return resource.Asset.LoadAsset<Sprite>(assetPath);
		}
	}
}