using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
	public static class ResourceManagerExtension
	{
		public static void LoadGameObject(this ResourceManager resource, string assetPath, Action<GameObject> callback)
		{
			resource.Asset.LoadAsset<GameObject>(assetPath, callback);
		}

		public static GameObject LoadGameObjectSync(this ResourceManager resource, string assetPath)
		{
			return resource.Asset.LoadAsset<GameObject>(assetPath);
		}

		public static void LoadTextAsset(this ResourceManager resource, string assetPath, Action<TextAsset> callback)
		{
			resource.Asset.LoadAsset<TextAsset>(assetPath, callback);
		}

		public static TextAsset LoadTextAssetSync(this ResourceManager resource, string assetPath)
		{
			return resource.Asset.LoadAsset<TextAsset>(assetPath);
		}

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