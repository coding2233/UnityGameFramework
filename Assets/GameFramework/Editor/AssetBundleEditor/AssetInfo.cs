//-----------------------------------------------------------------------
// <copyright>
//     Copyright (c) 2018 Zhang Yang. All rights reserved.
// </copyright>
// <describe> #assetbundle信息# </describe>
// <email> yeozhang@qq.com </email>
// <time> #2018年6月22日 18点33分# </time>
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace GameFramework.Taurus
{
    public class AssetInfo
    {
        /// <summary>
        /// 资源全路径
        /// </summary>
        public string AssetFullPath
        {
            get;
            private set;
        }
        /// <summary>
        /// 资源路径
        /// </summary>
        public string AssetPath
        {
            get;
            private set;
        }
        /// <summary>
        /// 资源名称
        /// </summary>
        public string AssetName
        {
            get;
            private set;
        }
        /// <summary>
        /// 资源的GUID（文件夹无效）
        /// </summary>
        public string GUID
        {
            get;
            private set;
        }
        /// <summary>
        /// 资源文件类型
        /// </summary>
        public FileType AssetFileType
        {
            get;
            private set;
        }
        /// <summary>
        /// 资源类型（文件夹无效）
        /// </summary>
        public Type AssetType
        {
            get;
            private set;
        }
        /// <summary>
        /// 资源是否勾选
        /// </summary>
        public bool IsChecked
        {
            get;
            set;
        }
        /// <summary>
        /// 文件夹是否展开（资源无效）
        /// </summary>
        public bool IsExpanding
        {
            get;
            set;
        }
        /// <summary>
        /// 所属AB包（文件夹无效）
        /// </summary>
        public string Bundled
        {
            get;
            set;
        }
        /// <summary>
        /// 文件夹的子资源（资源无效）
        /// </summary>
        public List<AssetInfo> ChildAssetInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 文件夹类型资源
        /// </summary>
        public AssetInfo(string fullPath, string name, bool isExpanding)
        {
            AssetFullPath = fullPath;
            AssetPath = "Assets" + fullPath.Replace(Application.dataPath.Replace("/", "\\"), "");
            AssetName = name;
            GUID = "";
            AssetFileType = FileType.Folder;
            AssetType = null;
            IsChecked = false;
            IsExpanding = isExpanding;
            Bundled = "";
            ChildAssetInfo = new List<AssetInfo>();
        }

        /// <summary>
        /// 文件类型资源
        /// </summary>
        public AssetInfo(string fullPath, string name, string extension)
        {
            AssetFullPath = fullPath;
            AssetPath = "Assets" + fullPath.Replace(Application.dataPath.Replace("/", "\\"), "");
            AssetName = name;
            GUID = AssetDatabase.AssetPathToGUID(AssetPath);
            AssetFileType = AssetBundleTool.GetFileTypeByExtension(extension);
            AssetType = AssetDatabase.LoadMainAssetAtPath(AssetPath).GetType();
            IsChecked = false;
            IsExpanding = false;
            Bundled = "";
            ChildAssetInfo = null;
        }
    }
}
