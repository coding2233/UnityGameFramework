using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
	public static class ResourceManagerExtension
	{
		public static void LoadSprite(this ResourceManager resource, string assetPath, Action<Sprite> callback)
		{

			resource.LoadAsset<Sprite>(assetPath, callback);
		}

		public static Sprite LoadSpriteSync(this ResourceManager resource, string assetPath)
		{
			return resource.LoadAssetSync<Sprite>(assetPath);
		}
	}
}