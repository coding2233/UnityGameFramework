using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wanderer.GameFramework
{
	public class UIComponent : MonoBehaviour
	{
		private void Awake()
		{
			GameFrameworkMode.GetModule<UIManager>().SetUIViewParent(transform);
		}
	}
}