//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #资源管理类 资源加载&内置对象池# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 17点11分# </time>
//-----------------------------------------------------------------------

namespace GameFramework.Taurus
{
    public sealed class ResourceManager:GameFrameworkModule
    {
        public override void OnClose()
        {

        }
    }


    /// <summary>
    /// 路径类型
    /// </summary>
    public enum PathType
    {
        /// <summary>
        /// 只读路径
        /// </summary>
        ReadOnly,
        /// <summary>
        /// 读写路径
        /// </summary>
        ReadWrite,
        /// <summary>
        /// 应用程序根目录
        /// </summary>
        DataPath,
        /// <summary>
        /// 临时缓存
        /// </summary>
        TemporaryCache,
    }

}