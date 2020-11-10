using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Wanderer.GameFramework
{
    public interface IUIAnimation
    {
        /// <summary>
        /// 目标的ui view
        /// </summary>
        UIView TargetUIView { get;}
        /// <summary>
        /// 动画目标物体
        /// </summary>
        /// <param name="target"></param>
        void SetTarget(GameObject target);
        /// <summary>
        /// 运行动画
        /// </summary>
        /// <returns></returns>
        UniTask<int> Run();
    }
}